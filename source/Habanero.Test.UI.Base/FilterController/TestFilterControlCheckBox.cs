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
using Habanero.UI.Base;
using Habanero.UI.Base.FilterControl;
using Habanero.UI.WebGUI;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{
    [TestFixture]
    public class TestFilterControlCheckBox
    {
        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.
        }

        [TearDown]
        public void TearDownTest()
        {
            //runs every time any testmethod is complete
        }
        #region Tests
        [Test]
        public void TestAddCheckBoxGizmox()
        {
            TestAddCheckBox(new ControlFactoryGizmox());
        }

        [Test]
        public void TestAddCheckBoxWinForms()
        {
            TestAddCheckBox(new ControlFactoryWin());
        }

        public void TestAddCheckBox(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            //IFilterClause nullClause = new DataViewNullFilterClause();
            IFilterControl filterControl = factory.CreateFilterControl();
            //---------------Execute Test ----------------------
            string labelName = "aa";
            IControlChilli cb = filterControl.AddBooleanFilterCheckBox(labelName, "", true);

            //---------------Test Result -----------------------
            Assert.IsNotNull(cb);
            Assert.IsTrue(cb is ICheckBox);
            ICheckBox cBox = (ICheckBox)cb  ;
            Assert.AreEqual(labelName, cBox.Text);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void TestAddStringFilterCheckBoxGiz()
        {
            TestAddStringFilterCheckBox(new ControlFactoryGizmox());
        }
        [Test]
        public void TestAddStringFilterCheckBoxWinForms()
        {
            TestAddStringFilterCheckBox(new ControlFactoryWin());
        }
        public void TestAddStringFilterCheckBox(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory filterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            //---------------Execute Test ----------------------
            filterControl.AddBooleanFilterCheckBox("Test?", "TestColumn", true);
            //---------------Test Result -----------------------
            IFilterClause clause =
                filterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpEquals, "true");

            Assert.AreEqual(clause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());

            //---------------Tear Down -------------------------          
        }
        [Test]
        public void TestGetCheckBoxFilterClauseWinForms()
        {
            TestGetCheckBoxFilterClause(new ControlFactoryWin());
        }

        [Test]
        public void TestGetCheckBoxFilterClauseGiz()
        {
            TestGetCheckBoxFilterClause(new ControlFactoryGizmox());
        }

        public void TestGetCheckBoxFilterClause(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory filterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            ICheckBox checkBox = filterControl.AddBooleanFilterCheckBox("Test?", "TestColumn", true);

            //---------------Execute Test ----------------------
            checkBox.Checked = false;
            string filterClauseString = filterControl.GetFilterClause().GetFilterClauseString();

            //---------------Test Result -----------------------
            IFilterClause clause =
                filterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpEquals, "false");
            Assert.AreEqual(clause.GetFilterClauseString(), filterClauseString);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void TestTwoCheckBoxFilterWinForms()
        {
            TestTwoCheckBoxFilter(new ControlFactoryWin());
        }

        [Test]
        public void TestTwoCheckBoxFilterGiz()
        {
            TestTwoCheckBoxFilter(new ControlFactoryGizmox());
        }

        public void TestTwoCheckBoxFilter(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory itsFilterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            filterControl.AddBooleanFilterCheckBox("Test1?", "TestColumn1", true);
            filterControl.AddBooleanFilterCheckBox("Test2?", "TestColumn2", false);

            //---------------Execute Test ----------------------
            string filterClauseString = filterControl.GetFilterClause().GetFilterClauseString();

            //---------------Test Result -----------------------
            IFilterClause clause1 =
                itsFilterClauseFactory.CreateStringFilterClause("TestColumn1", FilterClauseOperator.OpEquals, "true");
            IFilterClause clause2 =
                itsFilterClauseFactory.CreateStringFilterClause("TestColumn2", FilterClauseOperator.OpEquals, "false");
            IFilterClause fullClause =
                itsFilterClauseFactory.CreateCompositeFilterClause(clause1, FilterClauseCompositeOperator.OpAnd, clause2);
            Assert.AreEqual(fullClause.GetFilterClauseString(), filterClauseString);

            //---------------Tear Down -------------------------          
        }

        #endregion


    }
}
