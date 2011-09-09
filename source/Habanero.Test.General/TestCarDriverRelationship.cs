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

using Habanero.Base;
using Habanero.BO;
using NUnit.Framework;

namespace Habanero.Test.General
{
    /// <summary>
    /// Summary description for TestCarDriverRelationship.
    /// </summary>
    /// 	
    [TestFixture]
    public class TestCarDriverRelationship : TestUsingDatabase
    {
        public TestCarDriverRelationship()
        {
        }

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            this.SetupDBConnection();
        }

        public static void RunTest()
        {
            TestCarDriverRelationship test = new TestCarDriverRelationship();
            test.TestGetCarDriver();
        }

        [Test]
        public void TestGetCarDriver()
        {
            Car.DeleteAllCars();
            ContactPersonCompositeKey.DeleteAllContactPeople();

            Car car = new Car();
            ContactPersonCompositeKey person = new ContactPersonCompositeKey();
            person.SetPropertyValue("Surname", "Driver Surname");
            person.SetPropertyValue("PK1Prop1", "Driver1");
            person.SetPropertyValue("PK1Prop2", "Driver2");
            person.Save();
            car.SetPropertyValue("CarRegNo", "NP32459");
            car.SetPropertyValue("DriverFK1", person.GetPropertyValue("PK1Prop1"));
            car.SetPropertyValue("DriverFK2", person.GetPropertyValue("PK1Prop2"));
            IPrimaryKey bob = car.GetDriver().ID;
            IPrimaryKey bo2 = person.ID;
            Assert.IsTrue(bob.Equals(bo2));
            Assert.AreEqual(bob, bo2);
            //Assert.AreEqual(car.GetDriver().ID, person.ID);
        }

        [Test]
        public void TestGetCarDriverNull()
        {
            Car.DeleteAllCars();
            Car car = new Car();
            car.SetPropertyValue("CarRegNo", "NP32459");
            Assert.IsTrue(car.GetDriver() == null);
        }

        [Test]
        public void TestGetCarDriverIsSame()
        {
            Car.DeleteAllCars();
            ContactPersonCompositeKey.DeleteAllContactPeople();

            Car car = new Car();
            ContactPersonCompositeKey person = new ContactPersonCompositeKey();
            person.SetPropertyValue("Surname", "Driver Surname");
            person.SetPropertyValue("PK1Prop1", "Driver11");
            person.SetPropertyValue("PK1Prop2", "Driver21");
            person.Save();
            car.SetPropertyValue("CarRegNo", "NP32459");
            car.SetPropertyValue("DriverFK1", person.GetPropertyValue("PK1Prop1"));
            car.SetPropertyValue("DriverFK2", person.GetPropertyValue("PK1Prop2"));
            Assert.AreEqual(car.GetDriver().ID, person.ID);
            Assert.IsTrue(ReferenceEquals(person, car.GetDriver()));

            person = car.GetDriver();
            ContactPersonCompositeKey.ClearContactPersonCol();
            Assert.IsTrue(ReferenceEquals(person, car.GetDriver()),
                          "Should be the same since the Driver reference is being " +
                          "maintained in the car class and the object is therefore " +
                          "not being reloaded");
        }
    }
}