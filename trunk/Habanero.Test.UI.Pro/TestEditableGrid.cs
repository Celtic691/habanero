using System.Windows.Forms;
using Habanero.BO.ClassDefinition;
using Habanero.BO;
using Habanero.Test;
using Habanero.UI.Base;
using Habanero.UI.Grid;
using NUnit.Framework;

namespace Habanero.Test.UI.Application
{
    /// <summary>
    /// Summary description for TestEditableGrid.
    /// </summary>
    [TestFixture]
    public class TestEditableGrid
    {
        private EditableGrid grid;

        [SetUp]
        public void SetupTest()
        {
            grid = new EditableGrid();
            ClassDef.ClassDefs.Clear();
        }

        [Test]
        public void TestSetBusinessObjectCollection()
        {
            ClassDef classDef = MyBO.LoadClassDefWithBoolean();
            SetupGrid(classDef);

            Assert.AreEqual(4, grid.DataTable.Columns.Count);
            Assert.AreEqual(2, grid.DataTable.Rows.Count);
        }

        private void SetupGrid(ClassDef classDef)
        {
            BusinessObjectCollection<BusinessObject> col = new BusinessObjectCollection<BusinessObject>(classDef);
            BusinessObject bo1 = classDef.CreateNewBusinessObject();
            bo1.SetPropertyValue("TestProp", "Value1");
            bo1.SetPropertyValue("TestProp2", "Value2");
            BusinessObject bo2 = classDef.CreateNewBusinessObject();
            bo2.SetPropertyValue("TestProp", "2Value1");
            bo2.SetPropertyValue("TestProp2", "2Value2");
            col.Add(bo1);
            col.Add(bo2);
            grid.SetCollection(col);
            
        }

        [Test]
        public void TestColumnTypes()
        {
            ClassDef classDef = MyBO.LoadClassDefWithBoolean();
            SetupGrid(classDef);

            Assert.AreSame(typeof (DataGridViewTextBoxColumn), grid.Columns[0].GetType());
            Assert.AreSame(typeof (DataGridViewTextBoxColumn), grid.Columns[1].GetType());
            Assert.AreSame(typeof (DataGridViewTextBoxColumn), grid.Columns[2].GetType());
            Assert.AreSame(typeof (DataGridViewCheckBoxColumn), grid.Columns[3].GetType());
        }


        [Test]
        public void TestColumnTypesCombo()
        {
            ClassDef classDef = MyBO.LoadDefaultClassDef();
            SetupGrid(classDef);

            Assert.AreSame(typeof (DataGridViewTextBoxColumn), grid.Columns[0].GetType());
            Assert.AreSame(typeof (DataGridViewTextBoxColumn), grid.Columns[1].GetType());
            Assert.AreSame(typeof (DataGridViewComboBoxColumn), grid.Columns[2].GetType());
        }
    }
}