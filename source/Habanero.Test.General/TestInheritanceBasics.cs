using Habanero.BO.ClassDefinition;
using Habanero.BO;
using NUnit.Framework;

namespace Habanero.Test.General
{
    [TestFixture]
    public class TestInheritanceBasics
    {
        private ClassDef shapeClassDef;
        private ClassDef circleClassDef;
        private BusinessObject objShape;
        private BusinessObject objCircle;

        [TestFixtureSetUp]
        public void SetupTest()
        {
            shapeClassDef = Shape.GetClassDef();
            circleClassDef = Circle.GetClassDef();
            objShape = new Shape();
            objCircle = new Circle();
        }

        [Test]
        public void TestSuperClassDefProperty()
        {
        	Assert.AreSame(shapeClassDef, circleClassDef.SuperClassDef.SuperClassClassDef,
                           "SuperClassDef.ClassDef property on ClassDef should return the SuperClass's ClassDef");
        }

        [Test]
        public void TestCreateShapeObject()
        {
            Assert.AreSame(typeof (Shape), objShape.GetType(),
                           "objShape should be of type Shape, but is of type " + objShape.GetType().Name);
        }

        [Test]
        public void TestCreateCircleObject()
        {
            Assert.AreSame(typeof (Circle), objCircle.GetType(),
                           "objCircle should be of type Circle, but is of type " + objCircle.GetType().Name);
        }

        [Test]
        public void TestObjCircleIsAShape()
        {
            Assert.IsTrue(objCircle is Shape, "A Circle object should be a Shape");
        }

        [Test]
        public void TestObjCircleHasCorrectProperties()
        {
            objCircle.GetPropertyValue("ShapeName");
        }

        [Test]
        public void TestObjCircleHasShapeKeys()
        {
            Assert.AreEqual(1, objCircle.GetBOKeyCol().Count, "The Circle should have one key inherited from Shape");
        }

        //[Test]
        //public void TestObjCircleHasShapeRelationship()
        //{
        //    Assert.AreEqual(1, objCircle.Relationships.Count,
        //                    "The Circle object should have one relationship inherited from Shape");
        //}
    }
}