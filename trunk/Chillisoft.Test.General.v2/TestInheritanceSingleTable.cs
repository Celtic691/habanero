using System.Data;
using Chillisoft.Bo.ClassDefinition.v2;
using Chillisoft.Bo.v2;
using Chillisoft.Db.v2;
using Chillisoft.Util.v2;
using NUnit.Framework;

namespace Chillisoft.Test.General.v2
{
    [TestFixture]
    public class TestInheritanceSingleTable : TestInheritanceBase
    {
        public static void RunTest()
        {
            TestInheritanceSingleTable test = new TestInheritanceSingleTable();
            test.SetupTest();
        }


        protected override void SetupInheritanceSpecifics()
        {
            Circle.GetClassDef().SuperClassDesc =
                new SuperClassDesc(Shape.GetClassDef(), ORMapping.SingleTableInheritance);
        }

        protected override void SetStrID()
        {
            strID = (string) DatabaseUtil.PrepareValue(objCircle.GetPropertyValue("ShapeID"));
        }

        [Test]
        public void TestCircleIsUsingSingleTableInheritance()
        {
            Assert.AreEqual(ORMapping.SingleTableInheritance, Circle.GetClassDef().SuperClassDesc.ORMapping);
        }

        [Test]
        public void TestCircleHasShapeIDAsPrimaryKey()
        {
            try
            {
                objCircle.PrimaryKey.Contains("ShapeID");
            }
            catch (HabaneroArgumentException)
            {
                Assert.Fail("An object using SingleTableInheritance should receive its superclass as its primary key.");
            }
        }

        [Test]
        public void TestCircleHasCorrectPropertyNames()
        {
            objCircle.GetPropertyValue("ShapeName");
            objCircle.GetPropertyValue("Radius");
            objCircle.GetPropertyValue("ShapeID");
        }

        [Test, ExpectedException(typeof (PropertyNameInvalidException))]
        public void TestCircleDoesntHaveCircleID()
        {
            objCircle.GetPropertyValue("CircleID");
        }

        [Test]
        public void TestCircleInsertSQL()
        {
            Assert.AreEqual(1, itsInsertSql.Count,
                            "There should only be one insert Sql statement when using Single Table Inheritance.");
            Assert.AreEqual("INSERT INTO tbShape (Radius, ShapeID, ShapeName) VALUES (?Param0, ?Param1, ?Param2)",
                            itsInsertSql[0].Statement.ToString(),
                            "Concrete Table Inheritance insert SQL seems to be incorrect.");
            Assert.AreEqual(strID, ((IDbDataParameter) itsInsertSql[0].Parameters[1]).Value,
                            "Parameter ShapeID has incorrect value");
            Assert.AreEqual("MyShape", ((IDbDataParameter) itsInsertSql[0].Parameters[2]).Value,
                            "Parameter ShapeName has incorrect value");
            Assert.AreEqual(10, ((IDbDataParameter) itsInsertSql[0].Parameters[0]).Value,
                            "Parameter Radius has incorrect value");
        }

        [Test]
        public void TestCircleUpdateSql()
        {
            Assert.AreEqual(1, itsUpdateSql.Count,
                            "There should only be one update sql statement when using single table inheritance.");
            Assert.AreEqual(
                "UPDATE tbShape SET Radius = ?Param0, ShapeID = ?Param1, ShapeName = ?Param2 WHERE ShapeID = ?Param3",
                itsUpdateSql[0].Statement.ToString());
            Assert.AreEqual(strID, ((IDbDataParameter) itsUpdateSql[0].Parameters[1]).Value,
                            "Parameter ShapeID has incorrect value");
            Assert.AreEqual("MyShape", ((IDbDataParameter) itsUpdateSql[0].Parameters[2]).Value,
                            "Parameter ShapeName has incorrect value");
            Assert.AreEqual(10, ((IDbDataParameter) itsUpdateSql[0].Parameters[0]).Value,
                            "Parameter Radius has incorrect value");
            Assert.AreEqual(strID, ((IDbDataParameter) itsUpdateSql[0].Parameters[3]).Value,
                            "Parameter ShapeID has incorrect value");
        }

        [Test]
        public void TestCircleDeleteSql()
        {
            Assert.AreEqual(1, itsDeleteSql.Count,
                            "There should only be one delete sql statement when using single table inheritance.");
            Assert.AreEqual("DELETE FROM tbShape WHERE ShapeID = ?Param0", itsDeleteSql[0].Statement.ToString(),
                            "Delete SQL for single table inheritance is incorrect.");
            Assert.AreEqual(strID, ((IDbDataParameter) itsDeleteSql[0].Parameters[0]).Value,
                            "Parameter ShapeID has incorrect value for delete sql when using Single Table inheritance.");
        }

        [Test]
        public void TestSelectSql()
        {
            Assert.AreEqual(
                "SELECT tbShape.Radius, tbShape.ShapeID, tbShape.ShapeName FROM tbShape WHERE ShapeID = ?Param0",
                selectSql.Statement.ToString(), "Select sql is incorrect for single table inheritance.");
            Assert.AreEqual(strID, ((IDbDataParameter) selectSql.Parameters[0]).Value,
                            "Parameter ShapeID is incorrect in select where clause for single table inheritance.");
        }
    }
}