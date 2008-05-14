using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.UI.Base
{
    public class GridBaseManager
    {
        private readonly IGridBase _gridBase;
        //private BOCollectionDataSetProvider _dataSetProvider;
        //private DataView _dataTableDefaultView;
        private IBusinessObjectCollection _boCol;
        public event EventHandler CollectionChanged;

        public GridBaseManager(IGridBase gridBase)
        {
            _gridBase = gridBase;
            _gridBase.AutoGenerateColumns = false;
        }

        public BusinessObject SelectedBusinessObject
        {
            get
            {
                int rownum = -1;
                for (int i = 0; i < _gridBase.Rows.Count; i++)
                    if (_gridBase.Rows[i].Selected) rownum = i;
                if (rownum < 0) return null;
                return this.GetBusinessObjectAtRow(rownum);
            }
            set
            {
                IDataGridViewRowCollection gridRows = _gridBase.Rows;
                for (int i = 0; i < gridRows.Count; i++)
                {
                    IDataGridViewRow row = gridRows[i];
                    row.Selected = false;
                }
                if (value == null) return;
                int j = 0;
                foreach (IBusinessObject businessObject in _boCol)
                {
                    if (businessObject == value)
                    {
                        gridRows[j].Selected = true;
                        break;
                    }
                    j++;
                }
                //foreach (DataRowView dataRowView in _dataTableDefaultView)
                //{
                //    if ((string)dataRowView.Row["ID"] == value.ID.ToString())
                //    {
                //        gridRows[j].Selected = true;
                //        break;
                //    }
                //    j++;
                //}

            }
        }

        public IList<BusinessObject> SelectedBusinessObjects
        {
            get
            {
                if (_boCol == null) return new List<BusinessObject>();
                BusinessObjectCollection<BusinessObject> busObjects = new BusinessObjectCollection<BusinessObject>(_boCol.ClassDef);
                foreach (IDataGridViewRow row in _gridBase.SelectedRows)
                {
                    busObjects.Add((BusinessObject) row.DataBoundItem);
                }
                return busObjects;
            }
        }



        /// <summary>
        /// Sets the grid's collection to the one specified
        /// </summary>
        /// <param name="col">The collection to display in the grid</param>
        public void SetCollection(IBusinessObjectCollection col)
        {
            if (_gridBase.Columns.Count <= 0 )
            {
                throw new GridBaseSetUpException(
                    "You cannot call SetCollection if the grid's columns have not been set up");
                
            }
            _boCol = col;
            //_dataSetProvider = new BOCollectionReadOnlyDataSetProvider(col);
            //_dataSetProvider.ObjectInitialiser = _objectInitialiser;
            //_uiName = uiName;
            //ClassDef classDef = collection.ClassDef;
            //UIDef uiDef = classDef.GetUIDef(uiName);
            //UIGrid uiGrid = uiDef.UIGrid;
            //DataTable dataTable = _dataSetProvider.GetDataTable(uiGrid);

            _gridBase.AutoGenerateColumns = true;
            //_dataTableDefaultView = dataTable.DefaultView;
            _gridBase.AllowUserToAddRows = false;
            //_gridBase.DataSource = dataTable;

            _gridBase.DataSource = col;
            //_gridBase.Columns.Clear();
            //SetupColumns(columns);
            if (_gridBase.Rows.Count > 0)
            {
                SelectedBusinessObject = null;
                _gridBase.Rows[0].Selected = true;
            }
            FireCollectionChanged();
        }
        //private void SetupColumns(IDataGridViewColumnCollection columns)
        //{
        //    foreach (IDataGridViewColumn column in columns)
        //    {
        //        AddColumn(column);
        //    }
        //}

        //public int AddColumn(IDataGridViewColumn column)
        //{
        //    return _gridBase.Columns.Add(column, "");
        //}

        /// <summary>
        /// Sets the grid's collection to the one specified, but using the
        /// STA thread
        /// </summary>
        /// <param name="collection">The collection to display in the grid</param>
        /// <param name="uiName">The name of the uidef to use</param>
        /// TODO: Refactor
        //private void SetCollectionInSTAThread(IBusinessObjectCollection collection, string uiName)
        //{
            ////_collection = collection;
            ////_dataSetProvider = CreateBusinessObjectCollectionDataSetProvider(_collection);
            //_dataSetProvider.ObjectInitialiser = _objectInitialiser;
            //_uiName = uiName;
            //ClassDef classDef = collection.ClassDef;
            //UIDef uiDef = classDef.GetUIDef(uiName);
            //UIGrid uiGrid = uiDef.UIGrid;
            //_dataTable = _dataSetProvider.GetDataTable(uiGrid);
            //_dataTable.TableName = "Table";
            //_dateColumnIndices.Clear();

            //this.Columns.Clear();

            //DataGridViewColumn col = new DataGridViewTextBoxColumn(); // DataGridViewTextBoxColumn();
            ////col.Visible = false;
            //col.Width = 0;

            //col.Visible = false;
            //col.ReadOnly = true;
            //DataColumn dataColumn = _dataTable.Columns[0];
            //col.HeaderText = dataColumn.Caption;
            //col.Name = dataColumn.ColumnName;
            //col.DataPropertyName = dataColumn.ColumnName;
            //this.Columns.Add(col);
            //int colNum = 1;
            //foreach (UIGridColumn gridColumn in uiGrid)
            //{
            //    dataColumn = _dataTable.Columns[colNum];
            //    PropDef propDef = null;
            //    if (classDef.PropDefColIncludingInheritance.Contains(gridColumn.PropertyName))
            //    {
            //        propDef = classDef.PropDefColIncludingInheritance[gridColumn.PropertyName];
            //    }
            //    if (gridColumn.GridControlType == typeof(DataGridViewComboBoxColumn))
            //    {
            //        DataGridViewComboBoxColumn comboBoxCol = new DataGridViewComboBoxColumn();
            //        ILookupList source =
            //            (ILookupList)_dataTable.Columns[colNum].ExtendedProperties["LookupList"];
            //        if (source != null)
            //        {
            //            DataTable table = new DataTable();
            //            table.Columns.Add("id");
            //            table.Columns.Add("str");

            //            table.LoadDataRow(new object[] { "", "" }, true);
            //            foreach (KeyValuePair<string, object> pair in source.GetLookupList())
            //            {
            //                table.LoadDataRow(new object[] { pair.Value, pair.Key }, true);
            //            }
            //            comboBoxCol.DataSource = table;
            //            comboBoxCol.ValueMember = "str";
            //            comboBoxCol.DisplayMember = "str";
            //        }
            //        comboBoxCol.DataPropertyName = dataColumn.ColumnName;
            //        col = comboBoxCol;
            //    }
            //    else if (gridColumn.GridControlType == typeof(DataGridViewCheckBoxColumn))
            //    {
            //        DataGridViewCheckBoxColumn checkBoxCol = new DataGridViewCheckBoxColumn();
            //        col = checkBoxCol;
            //    }
            //    else if (gridColumn.GridControlType == typeof(DataGridViewDateTimeColumn))
            //    {
            //        DataGridViewDateTimeColumn dateTimeCol = new DataGridViewDateTimeColumn();
            //        col = dateTimeCol;
            //        _dateColumnIndices.Add(colNum, (string)gridColumn.GetParameterValue("dateFormat"));
            //    }
            //    else
            //    {
            //        col = (DataGridViewColumn)Activator.CreateInstance(gridColumn.GridControlType);
            //    }
            //    int width = (int)(dataColumn.ExtendedProperties["Width"]);
            //    col.Width = width;
            //    if (width == 0)
            //    {
            //        col.Visible = false;
            //    }
            //    col.ReadOnly = !gridColumn.Editable;
            //    col.HeaderText = dataColumn.Caption;
            //    col.Name = dataColumn.ColumnName;
            //    col.DataPropertyName = dataColumn.ColumnName;
            //    //col.MappingName = dataColumn.ColumnName;
            //    col.SortMode = DataGridViewColumnSortMode.Automatic;

            //    SetAlignment(col, gridColumn);
            //    if (CompulsoryColumnsBold && propDef != null && propDef.Compulsory)
            //    {
            //        Font newFont = new Font(DefaultCellStyle.Font, FontStyle.Bold);
            //        col.HeaderCell.Style.Font = newFont;
            //    }

            //    if (propDef != null && propDef.PropertyType == typeof(DateTime)
            //        && gridColumn.GridControlType != typeof(DataGridViewDateTimeColumn))
            //    {
            //        _dateColumnIndices.Add(colNum, (string)gridColumn.GetParameterValue("dateFormat"));
            //    }

            //    //if (propDef != null && propDef.PropertyName != gridColumn.GetHeading(classDef))
            //    //{
            //    //    foreach (BusinessObject bo in _collection)
            //    //    {
            //    //        BOProp boProp = bo.Props[propDef.PropertyName];
            //    //        if (!boProp.HasDisplayName())
            //    //        {
            //    //            boProp.DisplayName = gridColumn.GetHeading(classDef);
            //    //        }
            //    //    }
            //    //}

            //    Columns.Add(col);
            //    colNum++;
            //}

            //_dataTableDefaultView = _dataTable.DefaultView;
            //this.AutoGenerateColumns = false;
            //this.DataSource = _dataTableDefaultView;
            ////this.DataSource = _dataTable;
            //foreach (DataGridViewColumn dataGridViewColumn in this.Columns)
            //{
            //    if (!dataGridViewColumn.Visible)
            //    {
            //        dataGridViewColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //        dataGridViewColumn.Resizable = DataGridViewTriState.False;
            //    }
            //}
            //SetSorting(uiGrid);
            //if (_currentFilterClause != null)
            //{
            //    ApplyFilter(_currentFilterClause);
            //}
            //FireCollectionChanged();
        //}

        private void FireCollectionChanged()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns the business object collection being displayed in the grid
        /// </summary>
        /// <returns>Returns a business collection</returns>
        public IBusinessObjectCollection GetCollection()
        {
            return _boCol;
        }

        /// <summary>
        /// Returns the business object at the row specified
        /// </summary>
        /// <param name="row">The row number in question</param>
        /// <returns>Returns the busines object at that row, or null
        /// if none is found</returns>
        public BusinessObject GetBusinessObjectAtRow(int row)
        {
            return _boCol[row];
            //int i = 0; 
            //foreach (DataRowView dataRowView in _dataTableDefaultView)
            //{
            //    if (i++ == row)
            //    {
            //        return this._dataSetProvider.Find((string)dataRowView.Row["ID"]);
            //    }
            //}
            //return null;
        }

        public void Clear()
        {
            SetCollection(null);
        }

        /// <summary>
        /// Sets the sort column and indicates whether
        /// it should be sorted in ascending or descending order
        /// </summary>
        /// <param name="columnName">The column number to set</param>
        /// <param name="isBoProperty">Whether the property is a business
        /// object property</param>
        /// <param name="ascending">Whether sorting should be done in ascending
        /// order ("false" sets it to descending order)</param>
        public void SetSortColumn(string columnName, bool isBoProperty, bool ascending)
        {
            _boCol.Sort(columnName, isBoProperty, ascending);
            SetCollection(_boCol);
        }


    }
    public class GridBaseSetUpException : Exception
    {
        public GridBaseSetUpException(string message):base(message)
        {
        }
    }
}