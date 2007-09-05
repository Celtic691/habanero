using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.DB;
using NMock;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestBusinessObjectLookupList : TestUsingDatabase
    {

        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            this.SetupDBConnection();
            ClassDef.ClassDefs.Clear();
            ContactPerson.LoadDefaultClassDef();
        }

        [SetUp]
        public void SetupTest()
        {

        }

        [Test]
        public void TestGetLookupList() 
        {
            BusinessObjectLookupList source = new BusinessObjectLookupList(typeof (ContactPerson));

            Dictionary<string, object> col = source.GetLookupList(DatabaseConnection.CurrentConnection);
            Assert.AreEqual(3, col.Count);
            foreach (object o in col.Values) {
                Assert.AreSame(typeof(ContactPerson), o.GetType());
            }
        }

        [Test]
        public void TestCallingGetLookupListTwiceOnlyAccessesDbOnce()
        {
            BusinessObjectLookupList source = new BusinessObjectLookupList(typeof(ContactPerson));
            Dictionary<string, object> col = source.GetLookupList(DatabaseConnection.CurrentConnection);
            Dictionary<string, object> col2 = source.GetLookupList(DatabaseConnection.CurrentConnection);
            Assert.AreSame(col2, col);
        }

        [Test]
        public void TestLookupListTimeout()
        {
            BusinessObjectLookupList source = new BusinessObjectLookupList(typeof(ContactPerson), 100);
            Dictionary<string, object> col = source.GetLookupList(DatabaseConnection.CurrentConnection);
            System.Threading.Thread.Sleep(250);
            Dictionary<string, object> col2 = source.GetLookupList(DatabaseConnection.CurrentConnection);
            Assert.AreNotSame(col2, col);
        }

        [Test]
        public void TestLookupListCriteria()
        {
            BusinessObjectLookupList source = new BusinessObjectLookupList("Habanero.Test.BO", 
                "ContactPerson", "surname='zzz'");
            Dictionary<string, object> col = source.GetLookupList(DatabaseConnection.CurrentConnection);
            Assert.AreEqual(1, col.Count);
        }

    }
}
