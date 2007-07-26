using System;
using Habanero.BO.ClassDefinition;

namespace Habanero.Test.General
{
    /// <summary>
    /// Summary description for FilledCircle.
    /// </summary>
    public class FilledCircle : Circle
    {

        public static ClassDef GetClassDef()
        {
            if (!ClassDef.IsDefined(typeof (FilledCircle)))
            {
                return CreateClassDef();
            }
            else
            {
                return ClassDef.ClassDefs[typeof (FilledCircle)];
            }
        }

        protected override ClassDef ConstructClassDef()
        {
            _classDef = GetClassDef();
            return _classDef;
        }

        private static ClassDef CreateClassDef()
        {
            PropDefCol lPropDefCol = new PropDefCol();
            PropDef propDef =
                new PropDef("Colour", typeof (int), PropReadWriteRule.ReadWrite, "Colour", null);
            lPropDefCol.Add(propDef);
            propDef = lPropDefCol.Add("FilledCircleID", typeof (Guid), PropReadWriteRule.WriteOnce, null);
            PrimaryKeyDef primaryKey = new PrimaryKeyDef();
            primaryKey.IsObjectID = true;
            primaryKey.Add(lPropDefCol["FilledCircleID"]);
            KeyDefCol keysCol = new KeyDefCol();
            RelationshipDefCol relDefCol = new RelationshipDefCol();
            ClassDef lClassDef = new ClassDef(typeof (FilledCircle), primaryKey, lPropDefCol, keysCol, relDefCol);
            lClassDef.SuperClassDef = new SuperClassDef(Circle.GetClassDef(), ORMapping.ConcreteTableInheritance);
			ClassDef.ClassDefs.Add(lClassDef);
            return lClassDef;
        }
    }
}