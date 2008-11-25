//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.Util;
using log4net;

namespace Habanero.BO
{
    /// <summary>
    /// Provides a super-class for business objects. This class contains all
    /// the common functionality used by business objects.
    /// This Class implements the Layer SuperType - Fowler (xxx)
    /// </summary>
    public class BusinessObject : IBusinessObject, ISerializable
    {
        private static readonly ILog log = LogManager.GetLogger("Habanero.BO.BusinessObject");

        #region IBusinessObject Members

        public event EventHandler<BOEventArgs> Updated;

        public event EventHandler<BOEventArgs> Saved;
        public event EventHandler<BOEventArgs> Deleted;
        public event EventHandler<BOEventArgs> Restored;


        public event EventHandler<BOEventArgs> PropertyUpdated;

        #endregion

        #region Fields

        protected BOPropCol _boPropCol;

        //set object as new by default.
        private BOStatus _boStatus;
        protected IBusinessObjectUpdateLog _businessObjectUpdateLog;

        protected ClassDef _classDef;
        protected IConcurrencyControl _concurrencyControl;
        protected BOKeyCol _keysCol;
        protected IPrimaryKey _primaryKey;
        protected IRelationshipCol _relationshipCol;
        private ITransactionLog _transactionLog;
        private IBusinessObjectAuthorisation _authorisationRules;
        #endregion //Fields

        #region Constructors

        /// <summary>
        /// Constructor to initialise a new business object
        /// </summary>
        public BusinessObject() : this(null)
        {
        }

        protected BusinessObject(SerializationInfo info, StreamingContext context) {
            Initialise(ClassDef.ClassDefs[this.GetType()]);
            foreach (IBOProp prop in _boPropCol)
            {
                prop.InitialiseProp(info.GetValue(prop.PropertyName, prop.PropertyType));
            }
            _boStatus = (BOStatus) info.GetValue("Status", typeof (BOStatus));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (IBOProp prop in _boPropCol)
            {
                info.AddValue(prop.PropertyName, prop.Value);
            }
            info.AddValue("Status",this.Status);
        }

        /// <summary>
        /// Constructor that specifies a class definition
        /// </summary>
        /// <param name="def">The class definition</param>
        protected internal BusinessObject(ClassDef def)
        {
            Initialise(def);
        }

        private void InitialisePrimaryKeyPropertiesBasedOnParentClass(Guid myID)
        {
            ClassDef currentClassDef = _classDef;
            if (currentClassDef == null) return;
            while (currentClassDef.IsUsingClassTableInheritance())
            {
                while (currentClassDef.SuperClassClassDef != null &&
                       currentClassDef.SuperClassClassDef.PrimaryKeyDef == null)
                {
                    currentClassDef = currentClassDef.SuperClassClassDef;
                }

                if (currentClassDef.SuperClassClassDef == null) continue;
                if (currentClassDef.SuperClassClassDef.PrimaryKeyDef != null)
                {
                    InitialisePropertyValue(currentClassDef.SuperClassClassDef.PrimaryKeyDef.KeyName, myID);
                }
                currentClassDef = currentClassDef.SuperClassClassDef;
            }
        }

        /// <summary>
        /// A destructor for the object
        /// </summary>
        ~BusinessObject()
        {
            try
            {
                if (ClassDef == null) return;
                if (ID != null)
                {
                    BusinessObjectManager.Instance.Remove(this);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error disposing BusinessObject.", ex);
            }
            finally
            {
                ReleaseWriteLocks();
//                ReleaseReadLocks();                
            }
        }


        private void Initialise(ClassDef classDef)
        {
            _boStatus = new BOStatus(this);
            _boStatus.IsDeleted = false;
            _boStatus.IsDirty = false;
            _boStatus.IsEditing = false;
            _boStatus.IsNew = true;
            _classDef = classDef ?? ClassDef.ClassDefs[GetType()];
            ConstructFromClassDef(true);
            Guid myID = Guid.NewGuid();
            if (_primaryKey != null)
            {
                _primaryKey.SetObjectGuidID(myID);
            }
            InitialisePrimaryKeyPropertiesBasedOnParentClass(myID);

            if (_classDef == null)
            {
                throw new HabaneroDeveloperException(
                    "There is an error constructing a business object. Please refer to the system administrator",
                    "The Class could not be constructed since no classdef could be loaded");
            }
            if (ID == null)
            {
                throw new HabaneroDeveloperException(
                    "There is an error constructing a business object. Please refer to the system administrator",
                    "The Class could not be constructed since no _primaryKey has been created");
            }
        }
        /// <summary>
        /// Constructs the class
        /// </summary>
        /// <param name="newObject">Whether the object is new or not</param>
        protected virtual void ConstructFromClassDef(bool newObject)
        {
            if (_classDef == null) _classDef = ConstructClassDef();

            CheckClassDefNotNull();

            _boPropCol = _classDef.createBOPropertyCol(newObject);
            _keysCol = _classDef.createBOKeyCol(_boPropCol);

            SetPrimaryKeyForInheritedClass();

            _relationshipCol = _classDef.CreateRelationshipCol(_boPropCol, this);
        }

        private void CheckClassDefNotNull()
        {
            if (_classDef == null)
            {
                throw new NullReferenceException(String.Format(
                                                     "An error occurred while loading the class definitions (ClassDef.xml) for " +
                                                     "'{0}'. Check that the class exists in that " +
                                                     "namespace and assembly and that there are corresponding " +
                                                     "class definitions for this class.\n" + 
                                                     "Please check that the ClassDef.xml file is either an imbedded resource " + 
                                                     "or is copied to the output directory via the appropriate postbuild command " + 
                                                     "(for more help see FAQ)", GetType()));
            }
        }

        private void SetPrimaryKeyForInheritedClass()
        {
            ClassDef classDefToUseForPrimaryKey = GetClassDefToUseForPrimaryKey();

            if ((classDefToUseForPrimaryKey.SuperClassDef == null) ||
                (classDefToUseForPrimaryKey.IsUsingConcreteTableInheritance()) ||
                (_classDef.IsUsingClassTableInheritance()))
            {
                if (classDefToUseForPrimaryKey.PrimaryKeyDef != null)
                {
                    _primaryKey = (BOPrimaryKey) classDefToUseForPrimaryKey.PrimaryKeyDef.CreateBOKey(_boPropCol);
                }
            }
            else
            {
                if (classDefToUseForPrimaryKey.PrimaryKeyDef != null)
                {
                    _primaryKey = (BOPrimaryKey)
                                  classDefToUseForPrimaryKey.SuperClassClassDef.PrimaryKeyDef.CreateBOKey(_boPropCol);
                }
            }
        }

        private ClassDef GetClassDefToUseForPrimaryKey()
        {
            ClassDef classDefToUseForPrimaryKey = _classDef;
            while (classDefToUseForPrimaryKey.IsUsingSingleTableInheritance())
            {
                classDefToUseForPrimaryKey = classDefToUseForPrimaryKey.SuperClassClassDef;
            }
            return classDefToUseForPrimaryKey;
        }
        /// <summary>
        /// Constructs a class definition
        /// </summary>
        /// <returns>Returns a class definition</returns>
        protected virtual ClassDef ConstructClassDef()
        {
            return ClassDef.ClassDefs.Contains(GetType()) ? ClassDef.ClassDefs[GetType()] : null;
        }
        #endregion //Constructors

        #region Properties

        /// <summary>
        /// Returns an XML string that contains the changes in the object
        /// since the last persistance to the database
        /// </summary>
        internal string DirtyXML
        {
            get
            {
                return "<" + ClassName + " ID='" + ID + "'>" +
                       _boPropCol.DirtyXml + "</" + ClassName + ">";
            }
        }

        /// <summary>
        /// Gets and sets the collection of relationships
        /// </summary>
        public RelationshipCol Relationships
        {
            get { return (RelationshipCol) _relationshipCol; }
            set { _relationshipCol = value; }
        }


        /// <summary>
        /// Returns or sets the class definition. Setting the classdef is not recommended
        /// </summary>
        public ClassDef ClassDef
        {
            get { return _classDef; }
            set { _classDef = value; }
        }

        /// <summary>
        /// Returns the class name as specified in the class definition
        /// </summary>
        protected internal string ClassName
        {
            get { return _classDef.ClassName; }
        }

        /// <summary>
        /// Returns the table name
        /// </summary>
        protected internal string TableName
        {
            get
            {
                ClassDef classDefToUseForPrimaryKey = GetClassDefToUseForPrimaryKey();
                return (classDefToUseForPrimaryKey.IsUsingSingleTableInheritance()) 
                        ? classDefToUseForPrimaryKey.SuperClassClassDef.TableName 
                        : classDefToUseForPrimaryKey.TableName;
            }
        }

        /// <summary>
        /// Returns the primary key ID of this object.  If there is no primary key on this
        /// class, the primary key of the nearest suitable parent is found and populated
        /// with the values held for that key in this object.  This is a possible situation
        /// in some forms of inheritance.
        /// </summary>
        public IPrimaryKey ID
        {
            get
            {
                if (_primaryKey == null)
                {
                    if (ClassDef == null)
                    {
                        throw new NullReferenceException(String.Format(
                                                             "An error occurred while retrieving the class definitions for " +
                                                             "'{0}'. Check that the class exists in that " +
                                                             "namespace and assembly and that there are corresponding " +
                                                             "class definitions for this class.", GetType()));
                    }
                    PrimaryKeyDef primaryKeyDef = ClassDef.GetPrimaryKeyDef();
                    if (primaryKeyDef != null)
                    {
                        BOPrimaryKey parentPrimaryKey = new BOPrimaryKey(primaryKeyDef);
                        foreach (PropDef propDef in parentPrimaryKey.KeyDef)
                        {
                            BOProp prop = new BOProp(propDef);
                            prop.Value = Props[prop.PropertyName].Value;
                            parentPrimaryKey.Add(prop);
                        }
                        return parentPrimaryKey;
                    }
                }
                return _primaryKey;
            }
        }

        /// <summary>
        /// Gets and sets the collection of relationships
        /// </summary>
        IRelationshipCol IBusinessObject.Relationships
        {
            get { return _relationshipCol; }
            set { _relationshipCol = value; }
        }

        /// <summary>
        /// Returns or sets the class definition. Setting the classdef is not recommended
        /// </summary>
        IClassDef IBusinessObject.ClassDef
        {
            get { return _classDef; }
            set { _classDef = (ClassDef) value; }
        }


        /// <summary>
        /// Sets the concurrency control object
        /// </summary>
        /// <param name="concurrencyControl">The concurrency control</param>
        protected void SetConcurrencyControl(IConcurrencyControl concurrencyControl)
        {
            _concurrencyControl = concurrencyControl;
        }

        /// <summary>
        /// Sets the transaction log to that specified
        /// </summary>
        /// <param name="transactionLog">A transaction log</param>
        protected void SetTransactionLog(ITransactionLog transactionLog)
        {
            _transactionLog = transactionLog;
        }
        /// <summary>
        /// Sets the IBusinessObjectAuthorisation to that specified
        /// </summary>
        /// <param name="authorisationRules">The authorisation Rules</param>
        protected internal void SetAuthorisationRules(IBusinessObjectAuthorisation authorisationRules)
        {
            _authorisationRules = authorisationRules;
        }
        /// <summary>
        /// Sets the business object update log to the one specified
        /// </summary>
        /// <param name="businessObjectUpdateLog">A businessObject update log object</param>
        protected void SetBusinessObjectUpdateLog(IBusinessObjectUpdateLog businessObjectUpdateLog)
        {
            _businessObjectUpdateLog = businessObjectUpdateLog;
        }

        /// <summary>
        /// Returns the collection of BOKeys
        /// </summary>
        /// <returns>Returns a BOKeyCol object</returns>
        internal BOKeyCol GetBOKeyCol()
        {
            return _keysCol;
        }

        /// <summary>
        /// Returns a string useful for debugging output
        /// </summary>
        /// <returns>Returns an output string</returns>
        internal string GetDebugOutput()
        {
            string output = "";
            output += "Type: " + GetType().Name + Environment.NewLine;
            foreach (DictionaryEntry entry in _boPropCol)
            {
                BOProp prop = (BOProp) entry.Value;
                output += prop.PropertyName + " - " + prop.PropertyValueString + Environment.NewLine;
            }
            return output;
        }

        #endregion //Properties

        #region Editing Property Values

        /// <summary>
        /// The primary key for this business object 
        /// </summary>
        protected IPrimaryKey PrimaryKey
        {
            get { return _primaryKey; }
        }

        ///<summary>
        /// This method can be overridden by a class that inherits from Business object.
        /// The method allows the Business object developer to add customised rules that determine.
        /// The Creatable rules of a business object.
        /// E.g. Certain users may not be allowed to create certain Business Objects.
        /// </summary>
        public virtual bool IsCreatable(out string message)
        {
            message = "";
            if (_authorisationRules == null) return true;
            if (_authorisationRules.IsAuthorised(BusinessObjectActions.CanCreate)) return true;
            message = string.Format("The logged on user {0} is not authorised to create a {1}",
                    Thread.CurrentPrincipal.Identity.Name, this.ClassName);
            return false;
        }

        ///<summary>
        /// This method can be overridden by a class that inherits from Business object.
        /// The method allows the Business object developer to add customised rules that determine.
        /// The Readable rules of a business object.
        /// E.g. Certain users may not be allowed to view certain objects.
        /// </summary>
        public virtual bool IsReadable(out string message)
        {
            message = "";
            if (_authorisationRules == null) return true;
            if (_authorisationRules.IsAuthorised(BusinessObjectActions.CanRead)) return true;
            message = string.Format("The logged on user {0} is not authorised to read a {1}",
                    Thread.CurrentPrincipal.Identity.Name, this.ClassName);
            return false;
        }

        ///<summary>
        /// This method can be overridden by a class that inherits from Business object.
        /// The method allows the Business object developer to add customised rules that determine.
        /// The editable state of a business object.
        /// E.g. Once an invoice is paid it is no longer editable. Or when a course is old it is no
        /// longer editable. This allows a UI developer to standise Code for enabling and disabling controls.
        /// These rules are applied to new object as well so if you want a new object 
        /// to be editable then you must include this.Status.IsNew in evaluating IsEditable.
        /// It also allows the Application developer to implement security controlling the 
        ///   Editability of a particular Business Object.
        /// </summary>
        public virtual bool IsEditable(out string message)
        {
            message = "";
            if (_authorisationRules == null) return true;
            if (_authorisationRules.IsAuthorised(BusinessObjectActions.CanUpdate)) return true;
            message = string.Format("The logged on user {0} is not authorised to update {1} Identified By {2}",
                    Thread.CurrentPrincipal.Identity.Name, this.ClassName, this.ID.GetObjectId());
            return false;
        }

        ///<summary>
        /// This method can be overridden by a class that inherits from Business object.
        /// The method allows the Business object developer to add customised rules that determine.
        /// The Deletable state of a business object. E.g. Invoices can never be delted once created. 
        /// Objects cannot be deteled once they have reached certain stages e.g. a customer order after it is accepted.
        /// These rules are applied to new object as well so if you want a new object 
        /// to be deletable then you must include this.Status.IsNew in evaluating IsDeletable.
        /// It also allows the Application developer to implement security controlling the 
        ///   Deletability of a particular Business Object.
        ///</summary>
        public virtual bool IsDeletable(out string message)
        {
            message = "";
            if (_authorisationRules == null) return true;
            if(_authorisationRules.IsAuthorised(BusinessObjectActions.CanDelete)) return true;
            message = string.Format("The logged on user {0} is not authorised to delete {1} Identified By {2}", 
                    Thread.CurrentPrincipal.Identity.Name, this.ClassName, this.ID.GetObjectId());
            return false;
        }

        ///<summary>
        /// Returns the value under the property name specified
        ///</summary>
        ///<param name="propName">The property name</param>
        ///<typeparam name="T">The type to cast the retrieved property value to.</typeparam>
        ///<returns>Returns the value if found</returns>
        public T GetPropertyValue<T>(string propName)
        {
            return (T)GetPropertyValue(propName);
        }

        /// <summary>
        /// Returns the value under the property name specified
        /// </summary>
        /// <param name="propName">The property name</param>
        /// <returns>Returns the value if found</returns>
        public object GetPropertyValue(string propName)
        {
            if (propName == null) throw new ArgumentNullException("propName");
            string message;
            if (!this.IsReadable(out message)) throw new BusObjReadException(message);

            if (Props.Contains(propName))
                return Props[propName].Value;
            throw new InvalidPropertyNameException("Property '" + propName +
                                                   "' does not exist on a business object of type '" +
                                                   GetType().Name + "'");
        }

        /// <summary>
        /// Returns the value under the property name specified, accessing it through the 'source'
        /// </summary>
        /// <param name="source">The source of the property ie - the relationship or C# property this property is on</param>
        /// <param name="propName">The property name</param>
        /// <returns>Returns the value if found</returns>
        public object GetPropertyValue(Source source, string propName)
        {
            if (source == null || String.IsNullOrEmpty(source.Name)) return GetPropertyValue(propName);
            IBusinessObject businessObject = Relationships.GetRelatedObject(source.Name);
            if (businessObject == null) return null;
            if (source.Joins.Count > 0)
            {
                return businessObject.GetPropertyValue(source.Joins[0].ToSource, propName);
            } 
            return businessObject.GetPropertyValue(propName);
            
        }


        /// <summary>
        /// Sets a property value to a new value
        /// </summary>
        /// <param name="propName">The property name</param>
        /// <param name="newPropValue">The new value to set to</param>
        public void SetPropertyValue(string propName, object newPropValue)
        {
            IBOProp prop = Props[propName];
            if (prop == null)
            {
                throw new InvalidPropertyNameException(String.Format(
                                                           "The given property name '{0}' does not exist in the " +
                                                           "collection of properties for the class '{1}'.",
                                                           propName, ClassName));
            }

            if (!(newPropValue is Guid))
            {
                if (newPropValue is string && prop.PropertyType == typeof (Guid))
                {
                    Guid guidValue;
                    if (StringUtilities.GuidTryParse((string) newPropValue, out guidValue))
                    {
                        newPropValue = guidValue;
                    }
                    else
                    {
                        if (ClassDef.GetPropDef(propName).HasLookupList())
                        {
                            Dictionary<string, object> lookupList =
                                ClassDef.GetPropDef(propName).LookupList.GetLookupList();
                            try
                            {
                                newPropValue = lookupList[(string) newPropValue];
                            }
                            catch (KeyNotFoundException ex)
                            {
                                throw new HabaneroApplicationException(
                                    "You are trying to set the value for a lookup property " + propName + " to '" +
                                    newPropValue + "' this value does not exist in the lookup list", ex);
                            }
                            if (newPropValue is IBusinessObject)
                            {
                                newPropValue = ((BusinessObject) (newPropValue))._primaryKey.GetAsGuid();
                            }
                        }
                    }
                }
                if (newPropValue != null && newPropValue.Equals(DBNull.Value) && prop.PropertyType == typeof (bool))
                {
                    newPropValue = false;
                }
            }
            if (DBNull.Value.Equals(newPropValue))
            {
                newPropValue = null;
            }
            if (prop.PropertyType.IsSubclassOf(typeof (CustomProperty)))
            {
                if (newPropValue != null && prop.PropertyType != newPropValue.GetType())
                {
                    newPropValue = Activator.CreateInstance(prop.PropertyType, new object[] {newPropValue, false});
                }
            }
            if (newPropValue is IBusinessObject)
            {
                if (prop.PropertyType == typeof (Guid))
                    newPropValue = ((BusinessObject) newPropValue)._primaryKey.GetAsGuid();
                else newPropValue = ((BusinessObject) newPropValue).ID[0].Value.ToString();
            }
            else if (newPropValue is string && ClassDef.GetPropDef(propName).HasLookupList())
            {
                Dictionary<string, object> lookupList = ClassDef.GetPropDef(propName).LookupList.GetLookupList();
                if (lookupList.ContainsKey((string) newPropValue))
                    newPropValue = lookupList[(string) newPropValue];
                if (newPropValue is IBusinessObject)
                {
                    if (prop.PropertyType == typeof (Guid))
                        newPropValue = ((BusinessObject) newPropValue)._primaryKey.GetAsGuid();
                    else newPropValue = ((BusinessObject) newPropValue).ID[0].Value.ToString();
                }
            }
            // If the property will be changed by this set then
            // check if object is already editing (i.e. another property value has 
            // been changed if it is not then check that this object is still fresh
            // if the object is not fresh then throw appropriate exception.
            if (PropValueHasChanged(prop.Value, newPropValue))
            {
                if (!Status.IsEditing)
                {
                    BeginEdit();
                }
                _boStatus.IsDirty = true;
                prop.Value = newPropValue;
                if (prop.IsValid)
                {
                    //FireUpdatedEvent();
                    FirePropertyUpdatedEvent();
                }
            }
        }

        /// <summary>
        /// The BOProps in this business object
        /// </summary>
        public BOPropCol Props
        {
            get { return _boPropCol; }
        }

        /// <summary>
        /// Indicates whether all of the property values are valid
        /// </summary>
        /// <param name="invalidReason">A string to modify with a reason
        /// for any invalid values</param>
        /// <returns>Returns true if all are valid</returns>
        public bool IsValid(out string invalidReason)
        {
            invalidReason = "";
            if (Status.IsDeleted) return true;

            string customRuleErrors;
            bool valid = Props.IsValid(out invalidReason);
            valid &= AreCustomRulesValid(out customRuleErrors);
            if (!String.IsNullOrEmpty(customRuleErrors))
            {
                invalidReason = customRuleErrors + Environment.NewLine + invalidReason;
            }
            return valid;
        }

        /// <summary>
        /// Indicates whether all of the property values are valid
        /// </summary>
        /// <returns>Returns true if all are valid</returns>
        public bool IsValid()
        {
            string invalidReason;
            return IsValid(out invalidReason);
        }

        /// <summary>
        /// The IBOState <see cref="IBOStatus"/> object for this BusinessObject, which records the status information of the object
        /// </summary>
        public IBOStatus Status
        {
            get { return _boStatus; }
        }


        /// <summary>
        /// Sets the object's state into editing mode.  The original state can
        /// be restored with Restore() and changes can be committed to the
        /// database by calling Save().
        /// </summary>
        private void BeginEdit()
        {
            BeginEdit(false);
        }

        /// <summary>
        /// Sets the object's state into editing mode.  The original state can
        /// be restored with Restore() and changes can be committed to the
        /// database by calling Save().
        /// </summary>
        private void BeginEdit(bool delete)
        {
            string message;
            if (!IsEditable(out message) && !delete)
            {
                throw new BusObjEditableException(this, message);
            }
            CheckNotEditing();
            CheckConcurrencyBeforeBeginEditing();
            _boStatus.IsEditing = true;
        }

        /// <summary>
        /// Checks whether editing is already taking place, in which case
        /// an exception is thrown
        /// </summary>
        /// <exception cref="EditingException">Thrown if editing is taking
        /// place</exception>
        private void CheckNotEditing()
        {
            if (Status.IsEditing)
            {
                throw new EditingException(ClassName, ID.ToString(), this);
            }
        }

        /// <summary>
        /// Returns the value under the property name specified
        /// </summary>
        /// <param name="propName">The property name</param>
        /// <returns>Returns a string</returns>
        internal string GetPropertyValueString(string propName)
        {
            return Props[propName].PropertyValueString;
        }

        /// <summary>
        /// Returns the named property value as long as it is a Guid and is
        /// contained in the primary key
        /// </summary>
        /// <param name="propName">The property name</param>
        /// <returns>Returns the property value</returns>
        internal object GetPropertyValueToDisplay(string propName)
        {
            object propertyValue = GetPropertyValue(propName);

            if (Props[propName].PropertyType == typeof (Guid) && GetPropertyValue(propName) != null &&
                !ID.Contains(propName))
            {
                Guid myGuid = (Guid) propertyValue;
                ILookupList lookupList = ClassDef.GetLookupList(propName);
                Dictionary<string, object> list = lookupList.GetLookupList();

                foreach (KeyValuePair<string, object> pair in list)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value is IBusinessObject)
                    {
                        BusinessObject bo = (BusinessObject) pair.Value;
                        if (bo._primaryKey.GetAsGuid().Equals(myGuid))
                        {
                            return pair.Key;
                        }
                    }
                    else
                    {
                        if (pair.Value.Equals(myGuid))
                        {
                            return pair.Key;
                        }
                    }
                }
                //Item was not found in the list, so try some alternate methods
                if (lookupList is BusinessObjectLookupList)
                {
                    BusinessObjectLookupList businessObjectLookupList = lookupList as BusinessObjectLookupList;
                    ClassDef classDef = businessObjectLookupList.LookupBoClassDef;
                    IBusinessObject businessObject = null;
                    PrimaryKeyDef primaryKeyDef = classDef.GetPrimaryKeyDef();
                    if (primaryKeyDef.IsGuidObjectID)
                    {
                        BOPropCol boPropCol = classDef.createBOPropertyCol(true);
                        BOPrimaryKey boPrimaryKey = primaryKeyDef.CreateBOKey(boPropCol) as BOPrimaryKey;
                        if (boPrimaryKey != null)
                        {
                            boPrimaryKey[0].Value = myGuid;
                            businessObject = BORegistry.DataAccessor.BusinessObjectLoader.GetBusinessObject(classDef, boPrimaryKey);
                        }
                        if (businessObject != null) return businessObject;
                    }
                }

                return myGuid;
            }
            if (ClassDef.GetPropDef(propName).HasLookupList())
            {
                Dictionary<string, object> lookupList = ClassDef.GetLookupList(propName).GetLookupList();
                foreach (KeyValuePair<string, object> pair in lookupList)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value is string && pair.Value.Equals(Convert.ToString(propertyValue)))
                    {
                        return pair.Key;
                    }
                    if (pair.Value.Equals(propertyValue)) return pair.Key;

                    if (!(pair.Value is IBusinessObject)) continue;

                    BusinessObject bo = (BusinessObject) pair.Value;
                    if (String.Compare(bo.ID.ToString(), GetPropertyValueString(propName)) == 0)
                    {
                        return pair.Value.ToString();
                    }
                    if (bo.ID[0].Value != null &&
                        String.Compare(bo.ID[0].Value.ToString(), GetPropertyValueString(propName)) == 0)
                    {
                        return pair.Value.ToString();
                    }
                }
                return propertyValue;
            }
            return propertyValue;
        }

        /// <summary>
        /// Returns the property value as in GetPropertyValueToDisplay(), but
        /// returns the value as a string
        /// </summary>
        /// <param name="propName">The property name</param>
        /// <returns>Returns a string</returns>
        internal string GetPropertyStringValueToDisplay(string propName)
        {
            object val = GetPropertyValueToDisplay(propName);
            return val != null ? val.ToString() : "";
        }

        internal static bool PropValueHasChanged(object propValue, object newPropValue)
        {
            if (propValue == newPropValue) return false;
            if (propValue != null) return !propValue.Equals(newPropValue);
            return (newPropValue != null && !string.IsNullOrEmpty(Convert.ToString(newPropValue)));
        }

        /// <summary>
        /// Sets an initial property value
        /// </summary>
        /// <param name="propName">The property name</param>
        /// <param name="propValue">The value to initialise to</param>
        private void InitialisePropertyValue(string propName, object propValue)
        {
            IBOProp prop = Props[propName];
            prop.Value = propValue;
        }

        #endregion //Editing Property Values

        #region Persistance

        internal bool HasAutoIncrementingField
        {
            get { return _boPropCol.HasAutoIncrementingField; }
        }

        /// <summary>
        /// Commits to the database any changes made to the object
        /// </summary>
        public virtual void Save()
        {
            ITransactionCommitter committer =
                BORegistry.DataAccessor.CreateTransactionCommitter();
            committer.AddBusinessObject(this);
            committer.CommitTransaction();
        }

        /// <summary>
        /// Cancel all edits made to the object since it was loaded from the 
        /// database or last saved to the database
        /// </summary>
        public void Restore()
        {
            _boPropCol.RestorePropertyValues();
            _boStatus.IsDeleted = false;
            _boStatus.IsEditing = false;
            _boStatus.IsDirty = false;
            ReleaseWriteLocks();
            FireUpdatedEvent();
            FireRestoredEvent();
        }

        /// <summary>
        /// Marks the business object for deleting.  Calling Save() will
        /// then carry out the deletion from the database.
        /// </summary>
        public void Delete()
        {
            CheckIsDeletable();
            if (Status.IsNew)
            {
                throw new HabaneroDeveloperException(
                    String.Format("This '{0}' cannot be deleted as it has never existed in the database.",
                                  ClassDef.DisplayName),
                    String.Format("A '{0}' cannot be deleted when its status is new and does not exist in the database.",
                                  ClassDef.ClassName));

            }
            if (!Status.IsEditing)
            {
                BeginEdit(true);
            }
            _boStatus.IsDirty = true;
            _boStatus.IsDeleted = true;
        }

        /// <summary>
        /// Extra preparation or steps to take out after loading the business
        /// object
        /// </summary>
        protected internal virtual void AfterLoad()
        {
        }

        /// <summary>
        /// Carries out updates to the object after changes have been
        /// committed to the database
        /// </summary>
        protected internal void UpdateStateAsPersisted()
        {
            if (Status.IsDeleted)
            {
                SetStateAsPermanentlyDeleted();
                BusinessObjectManager.Instance.Remove(this);
                FireDeleted();
            }
            else
            {
                BusinessObjectManager.Instance.Remove(this);
                StorePersistedPropertyValues();
                SetStateAsUpdated();
                if (!BusinessObjectManager.Instance.Contains(ID))
                {
                    BusinessObjectManager.Instance.Add(this);
                }
                FireSaved();
            }
            AfterSave();
            ReleaseWriteLocks();
        }

        private void StorePersistedPropertyValues()
        {
            _boPropCol.BackupPropertyValues();
        }


        private void SetStateAsUpdated()
        {
            _boStatus.IsNew = false;
            if (!(_boPropCol == null))
            {
                _boPropCol.SetIsObjectNew(false);
            }
            _boStatus.IsDeleted = false;
            _boStatus.IsDirty = false;
            _boStatus.IsEditing = false;
        }

        private void SetStateAsPermanentlyDeleted()
        {
            _boStatus.IsNew = true;
            _boStatus.IsDeleted = true;
            _boStatus.IsDirty = false;
            _boStatus.IsEditing = false;
        }

        ///<summary>
        /// Executes any custom code required by the business object before it is persisted to the database.
        /// This has the additionl capability of creating or updating other business objects and adding these
        /// to the transaction committer.
        /// <remarks> Recursive call to UpdateObjectBeforePersisting will not be done i.e. it is the bo developers responsibility to implement</remarks>
        ///</summary>
        ///<param name="transactionCommitter">the transaction committer that is executing the transaction</param>
        protected internal virtual void UpdateObjectBeforePersisting(ITransactionCommitter transactionCommitter)
        {
            //if (_transactionLog != null)
            //{
            //    transactionCommitter.AddTransaction(_transactionLog);
            //}
            if (_businessObjectUpdateLog != null && (Status.IsNew || (Status.IsDirty && !Status.IsDeleted)))
            {
                _businessObjectUpdateLog.Update();
            }
        }

        /// <summary>
        /// This returns the Transaction Log object set up for this BusinessObject.
        /// </summary>
        internal ITransactionLog TransactionLog
        {
            get { return _transactionLog; }
        }

        private void CheckIsDeletable()
        {
            string errMsg;
            if (!IsDeletable(out errMsg))
            {
                throw new BusObjDeleteException(this, errMsg);
            }
        }

        protected void FireUpdatedEvent()
        {
            if (Updated != null)
            {
                Updated(this, new BOEventArgs(this));
            }
        }


        protected void FirePropertyUpdatedEvent()
        {
            if (PropertyUpdated != null)
            {
                PropertyUpdated(this, new BOEventArgs(this));
            }
        }

        private void FireRestoredEvent()
        {
            if (Restored != null)
            {
                Restored(this, new BOEventArgs(this));
            }
        }

        private void FireSaved()
        {
            if (Saved != null)
            {
                Saved(this, new BOEventArgs(this));
            }
        }

        private void FireDeleted()
        {
            if (Deleted != null)
            {
                Deleted(this, new BOEventArgs(this));
            }
        }

        /// <summary>
        /// Override this method in subclasses of BusinessObject to check custom rules for that
        /// class.  The default implementation returns true and sets customRuleErrors to the empty string.
        /// </summary>
        /// <param name="customRuleErrors">The error string to display</param>
        /// <returns>true if no custom rule errors are encountered.</returns>
        protected virtual bool AreCustomRulesValid(out string customRuleErrors)
        {
            customRuleErrors = "";
            return true;
        }

        #endregion //Persistance

        #region Concurrency

        /// <summary>
        /// Checks concurrency before persisting to the database
        /// </summary>
        protected internal virtual void CheckConcurrencyBeforePersisting()
        {
            if (!(_concurrencyControl == null))
            {
                _concurrencyControl.CheckConcurrencyBeforePersisting();
            }
            UpdatedConcurrencyControlPropertiesBeforePersisting();
        }

        /// <summary>
        /// Checks concurrency before beginning to edit an object's values
        /// </summary>
        protected virtual void CheckConcurrencyBeforeBeginEditing()
        {
            if (!(_concurrencyControl == null))
            {
                _concurrencyControl.CheckConcurrencyBeforeBeginEditing();
            }
        }

        /// <summary>
        /// Updates the concurrency control properties
        /// </summary>
        protected virtual void UpdatedConcurrencyControlPropertiesBeforePersisting()
        {
            if (!(_concurrencyControl == null))
            {
                _concurrencyControl.UpdatePropertiesWithLatestConcurrencyInfoBeforePersisting();
            }
        }

        /// <summary>
        /// Releases write locks from the database
        /// </summary>
        protected virtual void ReleaseWriteLocks()
        {
            if (!(_concurrencyControl == null))
            {
                _concurrencyControl.ReleaseWriteLocks();
            }
        }

//        /// <summary>
//        /// Releases write locks from the database
//        /// </summary>
//        protected virtual void ReleaseReadLocks()
//        {
//            if (!(_concurrencyControl == null))
//            {
//                _concurrencyControl.ReleaseReadLocks();
//            }
//        }
        #endregion //Concurrency

        ///<summary>
        /// Callec by the transaction committer in the case where the transaction failed
        /// and was rolled back.
        ///</summary>
        protected internal virtual void UpdateAsTransactionRolledBack()
        {
            if (!(_concurrencyControl == null))
            {
                _concurrencyControl.UpdateAsTransactionRolledBack();
            }
        }

        /// <summary>
        /// Called by the business object when the transaction has been successfully committed
        /// to the database. Called in cases of insert, delete and update.
        /// </summary>
        protected internal virtual void AfterSave()
        {
            FireUpdatedEvent();
        }

        /// <summary>
        /// Sets the status of the business object to the status true or false.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="value"></param>
        internal void SetStatus(BOStatus.Statuses status, bool value)
        {
            _boStatus.SetBOFlagValue(status, value);
        }
        /// <summary>
        /// Checks if the object can be persisted. This
        /// Checks the basic rules e.g. If you are deleting then
        ///   IsDeletable if creating a new object then IsCreatable
        ///   if Updating then IsEditable.
        /// </summary>
        /// <param name="errMsg">The appropriate error message if the 
        ///  Business Object cannot be persisted</param>
        /// <returns></returns>
        protected internal virtual bool CanBePersisted(out string errMsg)
        {
            errMsg = "";
            if (this.Status.IsDeleted && this.Status.IsNew)
            {
                errMsg = "The object has already been deleted from the dataBase and cannot be persisted again";
                return false;
            }
            if (this.Status.IsDeleted && !this.Status.IsNew )
            {
                return this.IsDeletable(out errMsg);
            }

            if (this.Status.IsNew)
            {
                return this.IsCreatable(out errMsg);
            }
            
            return !this.Status.IsDirty || this.IsEditable(out errMsg);
        }

    }
}
