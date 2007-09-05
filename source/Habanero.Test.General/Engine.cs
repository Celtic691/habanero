using System;
using System.Collections;
using System.Collections.Generic;
using Habanero.BO.ClassDefinition;
using Habanero.BO;
using Habanero.DB;

namespace Habanero.Test.General
{
    /// <summary>
    /// Summary description for Engine.
    /// </summary>
    public class Engine : BusinessObject
    {
        #region Constructors

        internal Engine() : base()
        {
        }

        internal Engine(BOPrimaryKey id) : base(id)
        {
        }

        protected static ClassDef GetClassDef()
        {
            if (!ClassDef.IsDefined(typeof (Engine)))
            {
                return CreateClassDef();
            }
            else
            {
                return ClassDef.ClassDefs[typeof (Engine)];
            }
        }

        protected override ClassDef ConstructClassDef()
        {
            return GetClassDef();
        }

        private static ClassDef CreateClassDef()
        {
            PropDefCol lPropDefCol = CreateBOPropDef();

            KeyDefCol keysCol = new KeyDefCol();

            PrimaryKeyDef primaryKey = new PrimaryKeyDef();
            primaryKey.IsObjectID = true;
            primaryKey.Add(lPropDefCol["EngineID"]);

            RelationshipDefCol relDefCol = CreateRelationshipDefCol(lPropDefCol);

            ClassDef lClassDef = new ClassDef(typeof (Engine), primaryKey, lPropDefCol, keysCol, relDefCol);
            lClassDef.TableName = "Table_Engine";
			ClassDef.ClassDefs.Add(lClassDef);
            return lClassDef;
        }

        private static RelationshipDefCol CreateRelationshipDefCol(PropDefCol lPropDefCol)
        {
            RelationshipDefCol relDefCol = new RelationshipDefCol();

            //Define Engine Relationships
            RelKeyDef relKeyDef = new RelKeyDef();
            PropDef propDef = lPropDefCol["CarID"];

            RelPropDef lRelPropDef = new RelPropDef(propDef, "CarID");
            relKeyDef.Add(lRelPropDef);

            RelationshipDef relDef = new SingleRelationshipDef("Car", typeof (Car), relKeyDef, false);

            relDefCol.Add(relDef);

            return relDefCol;
        }

        private static PropDefCol CreateBOPropDef()
        {
            PropDefCol lPropDefCol = new PropDefCol();
            PropDef propDef =
                new PropDef("EngineNo", typeof (String), PropReadWriteRule.ReadWrite, "ENGINE_NO", null);
            lPropDefCol.Add(propDef);

            propDef =
                lPropDefCol.Add("EngineID", typeof (Guid), PropReadWriteRule.WriteOnce, "Engine_ID", null);
            propDef = lPropDefCol.Add("CarID", typeof (Guid), PropReadWriteRule.WriteOnce, "CAR_ID", null);

            return lPropDefCol;
        }

        /// <summary>
        /// returns the Engine identified by id.
        /// </summary>
        /// <remarks>
        /// If the Contact person is already leaded then an identical copy of it will be returned.
        /// </remarks>
        /// <param name="id">The object primary Key</param>
        /// <returns>The loaded business object</returns>
        /// <exception cref="Habanero.BO.BusObjDeleteConcurrencyControlException">
        ///  if the object has been deleted already</exception>
        public static Engine GetEngine(BOPrimaryKey id)
        {
            Engine myEngine = (Engine)BOLoader.Instance.GetLoadedBusinessObject(id);
            if (myEngine == null)
            {
                myEngine = new Engine(id);
            }
            return myEngine;
        }

        #endregion //Constructors

        #region persistance

        #endregion /persistance

        #region Properties

        #endregion //Properties

        #region Relationships

        public Car GetCar()
        {
            return (Car) Relationships.GetRelatedObject("Car");
        }

        #endregion //Relationships

        #region ForTesting

        internal static void ClearEngineCol()
        {
            BusinessObject.ClearLoadedBusinessObjectBaseCol();
        }

        internal static void DeleteAllEngines()
        {
            string sql = "DELETE FROM " + GetClassDef().TableName;
            DatabaseConnection.CurrentConnection.ExecuteRawSql(sql);
        }

        #endregion

        #region ForCollections 

        //class
        protected internal string GetObjectNewID()
        {
            return _primaryKey.GetObjectNewID();
        }

        protected internal static BusinessObjectCollection<BusinessObject> LoadBusinessObjCol()
        {
            return LoadBusinessObjCol("", "");
        }

        protected internal static BusinessObjectCollection<BusinessObject> LoadBusinessObjCol(string searchCriteria,
                                                                                  string orderByClause)
        {
            BusinessObjectCollection<BusinessObject> bOCol = new BusinessObjectCollection<BusinessObject>(GetClassDef());
            bOCol.Load(searchCriteria, orderByClause);
            return bOCol;
        }

        #endregion
    }
}