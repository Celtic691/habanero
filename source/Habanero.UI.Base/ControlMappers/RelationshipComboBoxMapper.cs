using System;
using System.Collections;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using log4net;

namespace Habanero.UI.Base
{
    /// <summary>
    /// An interface for a mapper that <br/>
    /// Wraps/Decorates a ComboBox in order to display a collection of business objects
    /// in a combobox so that the user can select a business object for the purposes 
    /// of setting a related Business Object. 
    /// </summary>
    //TODO Mark 19 Jun 2009: Shouldn't this inherit from ControlMapper?
    public class RelationshipComboBoxMapper : ILookupComboBoxMapper
    {
        /// <summary>
        /// Uses for logging 
        /// </summary>
        protected static readonly ILog log = LogManager.GetLogger("Habanero.UI.Base.RelationshipComboBoxMapper");
        /// <summary>
        /// Gets the error provider for this control <see cref="IErrorProvider"/>
        /// </summary>
        public IErrorProvider ErrorProvider { get; private set; }
        
        private IClassDef _classDef;

//        private IBusinessObjectCollection _businessObjectCollection;
        private IBusinessObject _businessObject;
        /// <summary>
        /// The relationshipDef that is used for this Mapper.
        /// </summary>
        protected IRelationshipDef _relationshipDef;
        private ISingleRelationship _singleRelationship;
        private ILookupComboBoxMapperStrategy _mapperStrategy;
        private readonly ComboBoxCollectionSelector _comboBoxCollectionSelector;

        /// <summary>
        /// The Control <see cref="IComboBox"/> that is being mapped by this Mapper.
        /// </summary>
        public IComboBox Control { get; private set; }


        /// <summary>
        /// Is this control readonly or can the value be changed via the user interface.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// The Control factory used to create controls of the specified type.
        /// </summary>
        public IControlFactory ControlFactory { get; private set; }

        /// <summary>
        /// The name of the relationship that is being mapped by this Mapper
        /// </summary>
        public string RelationshipName { get; private set; }

        /// <summary>
        /// Get and Set whether to include a blank item in the selector or not.
        /// By default this is true.
        /// </summary>
        public bool IncludeBlankItem { get; set; }

        /// <summary>
        /// Gets or sets the SelectedIndexChanged event handler assigned to this mapper
        /// </summary>
        public EventHandler SelectedIndexChangedHandler { get; set; }

        /// <summary>
        /// Constructs a <see cref="RelationshipComboBoxMapper"/> with the <paramref name="comboBox"/>
        ///  <paramref name="relationshipName"/>
        /// </summary>
        /// <param name="comboBox">The combo box that is being mapped to</param>
        /// <param name="relationshipName">The name of the relation that is being mapped to</param>
        /// <param name="isReadOnly">Whether the Combo box can be used to edit from or whether it is only viewable</param>
        /// <param name="controlFactory">A control factory that is used to create control mappers etc</param>
        public RelationshipComboBoxMapper
            (IComboBox comboBox, string relationshipName, bool isReadOnly, IControlFactory controlFactory)
        {
            if (comboBox == null) throw new ArgumentNullException("comboBox");
            if (relationshipName == null) throw new ArgumentNullException("relationshipName");
            if (controlFactory == null) throw new ArgumentNullException("controlFactory");

            IsReadOnly = isReadOnly;
            ControlFactory = controlFactory;
            Control = comboBox;
            RelationshipName = relationshipName;
            this.IncludeBlankItem = true;
            _mapperStrategy = ControlFactory.CreateLookupComboBoxDefaultMapperStrategy();
            _mapperStrategy.AddHandlers(this);
            UpdateIsEditable();
            _comboBoxCollectionSelector = new ComboBoxCollectionSelector(comboBox, controlFactory, false);
        }
        /// <summary>
        /// Returns the name of the property being edited in the control
        /// </summary>
        public string PropertyName
        {
            get { return this.RelationshipName; }
        }
        ///<summary>
        /// Gets and Sets the Class Def of the Business object whose property
        /// this control maps.
        ///</summary>
        public IClassDef ClassDef
        {
            get { return _classDef; }
            set
            {
                _classDef = value;
                SetUpRelationship();
            }
        }
        /// <summary>
        /// Gets and sets the Business Object Collection that is used to fill the combo box items.
        /// </summary>
        public IBusinessObjectCollection BusinessObjectCollection
        {
            get { return _comboBoxCollectionSelector.Collection; }
            set
            {
                CheckBusinessObjectCollectionCorrectType(value);
               _comboBoxCollectionSelector.SetCollection(value, this.IncludeBlankItem);
            }
        }

        private void CheckBusinessObjectCollectionCorrectType(IBusinessObjectCollection value)
        {
            if (_relationshipDef == null) return;

            if (value != null && _relationshipDef.RelatedObjectClassType != value.ClassDef.ClassType)
            {
                string errorMessage = string.Format
                    ("You cannot set the Business Object Collection to the '{0}' "
                     + "since it is not of the appropriate type ('{1}') for this '{2}'", value.ClassDef.ClassNameFull,
                     _relationshipDef.RelatedObjectClassName, this.GetType().FullName);
                throw new HabaneroDeveloperException(errorMessage, errorMessage);
            }
        }

        /// <summary>
        /// Sets the property value to that provided.  If the property value
        /// is invalid, the error provider will be given the reason why the
        /// value is invalid.
        /// </summary>
        protected virtual void SetRelatedBusinessObject(IBusinessObject value)
        {
            if (_businessObject == null) return;

            try
            {
                _singleRelationship.SetRelatedObject(value);
            }
            catch (HabaneroIncorrectTypeException)
            {
                ////TODO Brett 24 Mar 2009: Write tests and implement                this.ErrorProvider.SetError(Control, ex.Message);
                return;
            }
            UpdateErrorProviderErrorMessage();
        }

        private void CheckRelationshipIsSingle(string relationshipName, IRelationshipDef relationshipDef)
        {
            if (this.ClassDef == null) return;

            if (IsSingleRelationship(relationshipDef)) return;
            string message = "The relationship '" + relationshipName + "' for the ClassDef '"
                             + this.ClassDef.ClassNameFull + "' is not a single relationship."
                             + " The 'RelationshipComboBoxMapper' can only be used for single relationships";
            throw new HabaneroDeveloperException(message, message);
        }

        private static void CheckRelationshipDefined(IClassDef classDef, string relationshipName)
        {
            if (classDef.RelationshipDefCol.Contains(relationshipName)) return;
            string message = "The relationship '" + relationshipName + "' does not exist on the ClassDef '"
                             + classDef.ClassNameFull + "'";
            throw new HabaneroDeveloperException(message, message);
        }

        private static bool IsSingleRelationship(IRelationshipDef relationshipDef)
        {
            return typeof (SingleRelationshipDef).IsInstanceOfType(relationshipDef);
        }

        /// <summary>
        /// Returns the control being mapped
        /// </summary>
        IControlHabanero IControlMapper.Control
        {
            get { return this.Control; }
        }

        /// <summary>
        /// Gets and sets the business object that has a property
        /// being mapped by this mapper.  In other words, this property
        /// does not return the exact business object being shown in the
        /// control, but rather the business object shown in the
        /// form.  Where the business object has been amended or
        /// altered, the <see cref="UpdateControlValueFromBusinessObject"/> method is automatically called here to 
        /// implement the changes in the control itself.
        /// </summary>
        public virtual IBusinessObject BusinessObject
        {
            get { return _businessObject; }
            set
            {
                SetupRelationshipForBO(value);
                CheckBusinessObjectCorrectType(value);

                RemoveCurrentBOHandlers();
                _businessObject = value;
                _singleRelationship = _businessObject == null ? null : GetRelationship();
                UpdateIsEditable();
                LoadCollectionForBusinessObject();
                UpdateControlValueFromBusinessObject();
                AddCurrentBOHandlers();
                //                this.UpdateErrorProviderErrorMessage();
            }
        }
        /// <summary>
        /// Provides an overrideable method for Loading the collection of business objects
        /// </summary>
        protected virtual void LoadCollectionForBusinessObject() { 
        }


        private void SetupRelationshipForBO(IBusinessObject businessObject)
        {
            if (businessObject == null) return;
            if (this.ClassDef != null) return;

            this.ClassDef = businessObject.ClassDef;
            SetUpRelationship();
        }

        private void SetUpRelationship()
        {
            CheckRelationshipDefined(this.ClassDef, this.RelationshipName);
            _relationshipDef = this.ClassDef.RelationshipDefCol[this.RelationshipName];
            CheckRelationshipIsSingle(this.RelationshipName, _relationshipDef);
        }

        private void UpdateIsEditable()
        {
            if (IsReadOnly || _businessObject == null)
            {
                this.Control.Enabled = false;
                return;
            }
            this.Control.Enabled = true;
            if (IsRelationshipComposition())
            {
                if (_businessObject != null)
                {
                    this.Control.Enabled = _businessObject.Status.IsNew;
                }
            }
        }

        private bool IsRelationshipComposition()
        {
            if (_singleRelationship == null) return false;
            return _singleRelationship.RelationshipDef.RelationshipType == RelationshipType.Composition;
        }

        private void RemoveCurrentBOHandlers()
        {
            if (_businessObject == null) return;
            _singleRelationship.RelatedBusinessObjectChanged -= RelatedBusinessObjectChanged_Handler;
            _businessObject.Saved -= _businessObject_OnSaved;
        }

        /// <summary>
        /// Gets or sets the strategy assigned to this mapper <see cref="ILookupComboBoxMapperStrategy"/>
        /// </summary>
        public ILookupComboBoxMapperStrategy MapperStrategy
        {
            get { return _mapperStrategy; }
            set
            {
                _mapperStrategy = value;
                _mapperStrategy.RemoveCurrentHandlers(this);
                _mapperStrategy.AddHandlers(this);
            }
        }



        private void AddCurrentBOHandlers()
        {
            if (_businessObject == null) return;
            _singleRelationship.RelatedBusinessObjectChanged += RelatedBusinessObjectChanged_Handler;
            _businessObject.Saved += _businessObject_OnSaved;
        }

        private void _businessObject_OnSaved(object sender, BOEventArgs e)
        {
            UpdateIsEditable();
        }

        private void RelatedBusinessObjectChanged_Handler(object sender, EventArgs e)
        {
            UpdateControlValueFromBusinessObject();
        }

        private void CheckBusinessObjectCorrectType(IBusinessObject value)
        {
            if (value != null && !this.ClassDef.ClassType.IsInstanceOfType(value))
            {
                string errorMessage = string.Format
                    ("You cannot set the Business Object to the '{0}' identified as '{1}' "
                     + "since it is not of the appropriate type ('{2}') for this '{3}'", value.ClassDef.ClassNameFull,
                     value.ToString(), this.ClassDef.ClassNameFull, this.GetType().FullName);
                throw new HabaneroDeveloperException(errorMessage, errorMessage);
            }
        }

        /// <summary>
        /// Updates the value on the control from the corresponding property
        /// on the represented <see cref="IControlMapper.BusinessObject"/>
        /// </summary>
        public virtual void UpdateControlValueFromBusinessObject()
        {
            InternalUpdateControlValueFromBo();
            this.UpdateErrorProviderErrorMessage();
        }

        /// <summary>
        /// Updates the value on the control from the corresponding property
        /// on the represented <see cref="IControlMapper.BusinessObject"/>
        /// </summary>
        protected virtual void InternalUpdateControlValueFromBo()
        {
            object relatedBO = GetRelatedBusinessObject();
            if (relatedBO != null)
            {
                IComboBoxObjectCollection comboBoxObjectCollection = this.Control.Items;
                if (!comboBoxObjectCollection.Contains(relatedBO))
                {
                    comboBoxObjectCollection.Add(relatedBO);
                }
            }
            _comboBoxCollectionSelector.DeregisterForControlEvents();
            try
            {
                Control.SelectedItem = relatedBO;
            }
            finally
            {
                _comboBoxCollectionSelector.RegisterForControlEvents();
            }
        }

        /// <summary>
        /// Returns the property value of the business object being mapped
        /// </summary>
        /// <returns>Returns the property value in appropriate object form</returns>
        protected internal object GetRelatedBusinessObject()
        {
            return _singleRelationship == null ? null : _singleRelationship.GetRelatedObject();
        }

        private ISingleRelationship GetRelationship()
        {
            IRelationship relationship = _businessObject == null
                                             ? null
                                             : _businessObject.Relationships[this.RelationshipName];
            ISingleRelationship singleRelationship = null;
            if (relationship is ISingleRelationship)
            {
                singleRelationship = (ISingleRelationship) relationship;
            }
            return singleRelationship;
        }

        /// <summary>
        /// Sets the Error Provider Error with the appropriate value for the property e.g. if it is invalid then
        ///  sets the error provider with the invalid reason else sets the error provider with a zero length string.
        /// </summary>
        public virtual void UpdateErrorProviderErrorMessage()
        {
        }

        /// <summary>
        /// Returns the Error Provider's Error message.
        /// </summary>
        /// <returns></returns>
        public string GetErrorMessage()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// A form field can have attributes defined in the class definition.
        /// These attributes are passed to the control mapper via a hashtable
        /// so that the control mapper can adjust its behaviour accordingly.
        /// </summary>
        /// <param name="attributes">A hashtable of attributes, which consists
        /// of name-value pairs, where name is the attribute name.  This is usually
        /// set in the XML definitions for the class's user interface.</param>
        public void SetPropertyAttributes(Hashtable attributes)
        {
            
        }

        /// <summary>
        /// Updates the properties on the represented business object
        /// </summary>
        public void ApplyChangesToBusinessObject()
        {
            try
            {
                object item = this.Control.SelectedItem;
                if (item is IBusinessObject)
                {
                    RemoveCurrentBOHandlers();
                    try
                    {
                        SetRelatedBusinessObject((IBusinessObject) item);
                    }
                    finally
                    {
                        AddCurrentBOHandlers();
                    }
                }
                else
                {
                    SetRelatedBusinessObject(null);
                }
            }
            catch (Exception ex)
            {
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "", "Error ");
            }
        }
    }
}