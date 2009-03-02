using Habanero.Base;
using Habanero.BO;
using Habanero.UI.Base;
using Habanero.UI.VWG;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{

    public class TestGridSelectorVWG : TestGridSelectorWin
    {
//        private const string _gridIdColumnName = "HABANERO_OBJECTID";
        protected override IControlFactory GetControlFactory()
        {
            ControlFactoryVWG factory = new ControlFactoryVWG();
            GlobalUIRegistry.ControlFactory = factory;
            return factory;
        }
        protected override IBOSelector CreateSelector()
        {
            TestGridBase.GridBaseVWGStub gridBase = new TestGridBase.GridBaseVWGStub();
            Gizmox.WebGUI.Forms.Form frm = new Gizmox.WebGUI.Forms.Form();
            frm.Controls.Add(gridBase);
            SetupGridColumnsForMyBo(gridBase);
            return gridBase;
        }
//        [Test]
//        public virtual void Test_Constructor_nullControlFactory_RaisesError()
//        {
//            //---------------Set up test pack-------------------
//
//            //---------------Assert Precondition----------------
//
//            //---------------Execute Test ----------------------
//            try
//            {
//                new GridSelectorVWG(null);
//                Assert.Fail("expected ArgumentNullException");
//            }
//            //---------------Test Result -----------------------
//            catch (ArgumentNullException ex)
//            {
//                StringAssert.Contains("Value cannot be null", ex.Message);
//                StringAssert.Contains("controlFactory", ex.ParamName);
//            }
//        }
    }

    /// <summary>
    /// This test class tests the GridSelector class.
    /// </summary>
    [TestFixture]
    public class TestGridSelectorWin : TestBOSelectorWin
    {
        private const string _gridIdColumnName = "HABANERO_OBJECTID";
        protected override IControlFactory GetControlFactory()
        {
            ControlFactoryWin factory = new ControlFactoryWin();
            GlobalUIRegistry.ControlFactory = factory;
            return factory;
        }

        protected override void SetSelectedIndex(IBOSelector selector, int index)
        {
            int count = 0;
            foreach (IDataGridViewRow row in ((IGridBase) selector).Rows)
            {
                if (count == index)
                {
                    IBusinessObject businessObjectAtRow = ((IGridBase) selector).GetBusinessObjectAtRow(count);
                    selector.SelectedBusinessObject = businessObjectAtRow;
                }
                count++;
            }
        }

        protected override int SelectedIndex(IBOSelector selector)
        {
            IGridBase gridSelector = (IGridBase) selector;
            IDataGridViewRow currentRow = null;
            if (gridSelector.SelectedRows.Count > 0)
            {
                currentRow = gridSelector.SelectedRows[0];
            }
            if (currentRow == null) return -1;

            return gridSelector.Rows.IndexOf(currentRow);
        }

        protected override IBOSelector CreateSelector()
        {
            TestGridBase.GridBaseWinStub gridBase = new TestGridBase.GridBaseWinStub();
            System.Windows.Forms.Form frm = new System.Windows.Forms.Form();
            frm.Controls.Add(gridBase);
            SetupGridColumnsForMyBo(gridBase);
            return gridBase;
        }
        protected static void SetupGridColumnsForMyBo(IDataGridView gridBase)
        {
            gridBase.Columns.Add(_gridIdColumnName, _gridIdColumnName);
            gridBase.Columns.Add("TestProp", "TestProp");
        }
        protected override int NumberOfLeadingBlankRows()
        {
            return 0;
        }

        [Test]
        public void Test_Constructor_GridSet()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            IBOSelector selector = CreateSelector();
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(IGridBase), selector);
        }

        [Ignore(" Not sure how to impolement this in grids.")] //TODO  01 Mar 2009:
        [Test]
        public override void Test_Set_SelectedBusinessObject_ItemNotInList_SetsItemNull()
        {
            //---------------Set up test pack-------------------
            IBOSelector selector = CreateSelector();
            MyBO myBO = new MyBO();
            MyBO myBO2 = new MyBO();
            BusinessObjectCollection<MyBO> collection = new BusinessObjectCollection<MyBO> { myBO, myBO2 };
            selector.BusinessObjectCollection = collection;
            SetSelectedIndex(selector, ActualIndex(1));
            //---------------Assert Precondition----------------
            Assert.AreEqual(collection.Count + NumberOfLeadingBlankRows(), selector.NoOfItems, "The blank item and others");
            Assert.AreEqual(ActualIndex(1), SelectedIndex(selector));
            Assert.AreEqual(myBO2, selector.SelectedBusinessObject);
            //---------------Execute Test ----------------------
            selector.SelectedBusinessObject = new MyBO();
            //---------------Test Result -----------------------
            Assert.AreEqual(ActualIndex(2), selector.NoOfItems, "The blank item");
            Assert.IsNull(selector.SelectedBusinessObject);
            Assert.AreEqual(-1, SelectedIndex(selector));
        }

        [Ignore(" Not sure how to implement this in grids")] //TODO  01 Mar 2009:
        [Test]
        public override void Test_AutoSelectsFirstItem()
        {
            //---------------Set up test pack-------------------
            IBOSelector selector = CreateSelector();
            MyBO myBO = new MyBO();
            MyBO myBO2 = new MyBO();
            BusinessObjectCollection<MyBO> collection = new BusinessObjectCollection<MyBO> { myBO, myBO2 };
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, selector.NoOfItems);
            Assert.AreEqual(-1, SelectedIndex(selector));
            Assert.AreEqual(null, selector.SelectedBusinessObject);
            //---------------Execute Test ----------------------
            selector.BusinessObjectCollection = collection;
            //---------------Test Result -----------------------
            Assert.AreEqual(collection.Count + NumberOfLeadingBlankRows(), selector.NoOfItems, "The blank item");
            Assert.AreSame(myBO, selector.SelectedBusinessObject);
            Assert.AreEqual(ActualIndex(0), SelectedIndex(selector));
        }
        [Ignore(" Not Yet implemented")] //TODO  01 Mar 2009:
        [Test]
        public void TestEditItemFromCollectionUpdatesItemInSelector()
        {
        }

    }
}