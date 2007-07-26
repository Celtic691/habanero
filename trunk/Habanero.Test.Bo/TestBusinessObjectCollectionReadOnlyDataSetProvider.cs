using Habanero.BO;
using Habanero.Base;
using NUnit.Framework;
using BusinessObject=Habanero.BO.BusinessObject;

namespace Habanero.Test.BO
{
    /// <summary>
    /// Summary description for TestBusinessObjectCollectionReadOnlyDataSetProvider.
    /// </summary>
    [TestFixture]
    public class TestBusinessObjectCollectionReadOnlyDataSetProvider : TestBusinessObjectCollectionDataProvider
    {
        protected override IDataSetProvider CreateDataSetProvider(BusinessObjectCollection<BusinessObject> col)
        {
            return new BOCollectionReadOnlyDataSetProvider(itsCollection);
        }

        [Test]
        public void TestUpdateBusinessObjectUpdatesRow()
        {
            itsBo1.SetPropertyValue("TestProp", "UpdatedValue");
            Assert.AreEqual("UpdatedValue", itsTable.Rows[0][1]);
        }

        [Test]
        public void TestAddBusinessObjectAddsRow()
        {
            BusinessObject bo3 = itsClassDef.CreateNewBusinessObject(itsConnection);
            bo3.SetPropertyValue("TestProp", "bo3prop1");
            bo3.SetPropertyValue("TestProp2", "s1");
            itsCollection.Add(bo3);
            Assert.AreEqual(3, itsTable.Rows.Count);
            Assert.AreEqual("bo3prop1", itsTable.Rows[2][1]);
        }

        [Test]
        public void TestAddBusinessObjectAndUpdateUpdatesNewRow()
        {
            BusinessObject bo3 = itsClassDef.CreateNewBusinessObject(itsConnection);
            bo3.SetPropertyValue("TestProp", "bo3prop1");
            bo3.SetPropertyValue("TestProp2", "s2");
            itsCollection.Add(bo3);
            bo3.SetPropertyValue("TestProp", "UpdatedValue");
            Assert.AreEqual("UpdatedValue", itsTable.Rows[2][1]);
        }

        [Test]
        public void TestRemoveBusinessObjectRemovesRow()
        {
            itsCollection.Remove(itsBo1);
            Assert.AreEqual(1, itsTable.Rows.Count);
        }
    }
}