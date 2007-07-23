using Habanero.Bo.ClassDefinition;
using Habanero.Bo;
using Habanero.Test;
using NUnit.Framework;

namespace Habanero.Test.Bo
{
    /// <summary>
    /// Summary description for TestSingleRelationship.
    /// </summary>
    [TestFixture]
    public class TestSingleRelationship
    {
        private ClassDef itsClassDef;
        private ClassDef itsRelatedClassDef;

        [TestFixtureSetUp]
        public void SetupTestFixture()
        {
            ClassDef.ClassDefs.Clear();
            itsClassDef = MyBo.LoadClassDefWithRelationship();
            itsRelatedClassDef = MyRelatedBo.LoadClassDef();
        }

        [Test]
        public void TestSetRelatedObject()
        {
            MyBo bo1 = (MyBo) itsClassDef.CreateNewBusinessObject();
            MyRelatedBo relatedBo1 = (MyRelatedBo) itsRelatedClassDef.CreateNewBusinessObject();
            ((SingleRelationship) bo1.Relationships["MyRelationship"]).SetRelatedObject(relatedBo1);
            Assert.AreSame(relatedBo1, bo1.Relationships.GetRelatedObject("MyRelationship"));
            Assert.AreSame(bo1.GetPropertyValue("RelatedID"), relatedBo1.GetPropertyValue("MyRelatedBoID"));
        }
    }
}