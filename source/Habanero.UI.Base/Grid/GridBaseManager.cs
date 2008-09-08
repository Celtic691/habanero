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

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;

namespace Habanero.UI.Base
{
    /// <summary>
    /// This manager groups common logic for IEditableGridControl objects.
    /// Do not use this object in working code.
    /// </summary>
    public class GridBaseManager
    {
        private readonly IGridBase _gridBase;
        private IDataSetProvider _dataSetProvider;
        private IBusinessObjectCollection _boCol;
        private string _uiDefName;
        private DataView _dataTableDefaultView;
        public event EventHandler CollectionChanged;
        private GridLoaderDelegate _gridLoader;
        private IClassDef _classDef;

        public GridBaseManager(IGridBase gridBase, string uiDefName)
        {
            _gridBase = gridBase;
            _uiDefName = uiDefName;
            _gridBase.AutoGenerateColumns = false;
            _gridLoader = DefaultGridLoader;
            _gridBase.AllowUserToAddRows = false;
        }

        public GridBaseManager(IGridBase gridBase) : this(gridBase, "default")
        {
        }

        /// <summary>
        /// See <see cref="IGridBase.SetBusinessObjectCollection"/>
        /// </summary>
        public void SetBusinessObjectCollection(IBusinessObjectCollection col)
        {
            if (_gridBase.Columns.Count <= 0)
            {
                throw new GridBaseSetUpException(
                    "You cannot call SetBusinessObjectCollection if the grid's columns have not been set up");
            }
            _boCol = col;
            if (_boCol == null)
            {
                SelectedBusinessObject = null;
                _gridLoader(_gridBase, null);
                FireCollectionChanged();
                return;
            }
            //Hack: this is to overcome abug in Gizmox where the grid was freezing after delete
            // but should not cause a problem with win since it removed the currently selected item which is the deleted item
            col.BusinessObjectRemoved += delegate { SelectedBusinessObject = null; };

            _gridLoader(_gridBase, _boCol);

            if (_gridBase.Rows.Count > 0)
            {
                SelectedBusinessObject = null;
                _gridBase.Rows[0].Selected = true;
            }
            FireCollectionChanged();
        }

        /// <summary>
        /// See <see cref="IGridBase.DataSetProvider"/>
        /// </summary>
        public IDataSetProvider DataSetProvider
        {
            get { return _dataSetProvider; }
        }

        /// <summary>
        /// Sets the default grid loader
        /// </summary>
        public void DefaultGridLoader(IGridBase gridBase, IBusinessObjectCollection boCol)
        {
            if (boCol == null)
            {
                gridBase.DataSource = null;
                return;
            }
            gridBase.DataSource = GetDataTable(boCol);
        }

        protected DataView GetDataTable(IBusinessObjectCollection boCol)
        {
            _dataSetProvider = _gridBase.CreateDataSetProvider(boCol);
            if (this.ClassDef == null)
            {
                this.ClassDef = _boCol.ClassDef;
            }  
            UIDef uiDef = ((ClassDef)this.ClassDef).GetUIDef(UiDefName);
            if (uiDef == null)
            {
                throw new ArgumentException(
                    String.Format(
                        "You cannot Get the data for the grid {0} since the uiDef {1} cannot be found for the classDef {2}",
                        this._gridBase.Name, UiDefName, ((ClassDef)this.ClassDef).ClassName));
            }
            DataTable dataTable = _dataSetProvider.GetDataTable(uiDef.UIGrid);
            _dataTableDefaultView = dataTable.DefaultView;
            return this._dataTableDefaultView;
        }

        /// <summary>
        /// See <see cref="IGridBase.SelectedBusinessObject"/>
        /// </summary>
        public IBusinessObject SelectedBusinessObject
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
                //TODO_Port: RemovePreviouslySelectedBusinessObjectsEventHandler();
                if (_boCol == null && value != null)
                {
                    throw new GridBaseInitialiseException(
                        "You cannot call SelectedBusinessObject if the collection is not set");
                }
                IDataGridViewRowCollection gridRows = _gridBase.Rows;
                ClearAllSelectedRows(gridRows);
                if (value == null) return;
                BusinessObject bo = (BusinessObject) value;
                int rowNum = 0;
                foreach (IDataGridViewRow row in gridRows)
                {
                    if (row.Cells["ID"].Value.ToString() == bo.ID.ToString())
                    {
                        gridRows[rowNum].Selected = true;
                        _gridBase.ChangeToPageOfRow(rowNum);
                        break;
                    }
                    rowNum++;
                }
            }
        }


        //TODO_Port:
        //private void RemovePreviouslySelectedBusinessObjectsEventHandler()
        //{
        //    if (SelectedBusinessObject != null)
        //    {
        //        SelectedBusinessObject.Updated -= bo_Updated;
        //    }
        //}

        private static void ClearAllSelectedRows(IDataGridViewRowCollection gridRows)
        {
            for (int i = 0; i < gridRows.Count; i++)
            {
                IDataGridViewRow row = gridRows[i];
                row.Selected = false;
            }
        }

        ///// <summary>
        ///// Calls the BusinessObjectAdded() handler
        ///// </summary>
        ///// <param name="bo">The business object added</param>
        //private void FireBusinessObjectEdited(BusinessObject bo)
        //{
        //    _gridBase.SelectedBusinessObjectEdited(bo);
        //}

        /// <summary>
        /// See <see cref="IGridBase.SelectedBusinessObjects"/>
        /// </summary>
        public IList<BusinessObject> SelectedBusinessObjects
        {
            get
            {
                if (_boCol == null) return new List<BusinessObject>();
                BusinessObjectCollection<BusinessObject> busObjects =
                    new BusinessObjectCollection<BusinessObject>(this.ClassDef);
                foreach (IDataGridViewRow row in _gridBase.SelectedRows)
                {
                    BusinessObject businessObject = (BusinessObject) GetBusinessObjectAtRow(row.Index);
                    busObjects.Add(businessObject);
                }
                return busObjects;
            }
        }

        /// <summary>
        /// See <see cref="IGridBase.GridLoader"/>
        /// </summary>
        public GridLoaderDelegate GridLoader
        {
            get { return this._gridLoader; }
            set { this._gridLoader = value; }
        }

        /// <summary>
        /// See <see cref="IGridControl.UiDefName"/>
        /// </summary>
        public string UiDefName
        {
            get { return _uiDefName; }
            set { _uiDefName = value; }
        }

        /// <summary>
        /// See <see cref="IGridControl.ClassDef"/>
        /// </summary>
        public IClassDef ClassDef
        {
            get { return _classDef; }
            set { _classDef = value; }
        }

        private void FireCollectionChanged()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// See <see cref="IGridBase.GetBusinessObjectCollection"/>
        /// </summary>
        public IBusinessObjectCollection GetBusinessObjectCollection()
        {
            return _boCol;
        }

        /// <summary>
        /// See <see cref="IGridBase.GetBusinessObjectAtRow"/>
        /// </summary>
        public IBusinessObject GetBusinessObjectAtRow(int rowIndex)
        {
            int i = 0;

            if (_gridBase.DataSource is DataView)
            {
                foreach (DataRowView dataRowView in _dataTableDefaultView)
                {
                    if (i++ == rowIndex)
                    {
                        return this._dataSetProvider.Find((string) dataRowView.Row["ID"]);
                    }
                }
            }else
            {
                IDataGridViewRow findRow =_gridBase.Rows[rowIndex];
                return this._boCol.Find(findRow.Cells["ID"].Value.ToString());
            }
            return null;
        }

        /// <summary>
        /// See <see cref="IGridBase.Clear"/>
        /// </summary>
        public void Clear()
        {
            SetBusinessObjectCollection(null);
        }

        public void SetSortColumn(string columnName, bool isAscending)
        {
            if (_gridBase.DataSource is DataView)
            {
                if (isAscending)
                {
                    ((DataView) _gridBase.DataSource).Sort = columnName + " ASC";
                }
                else
                {
                    ((DataView) _gridBase.DataSource).Sort = columnName + " DESC";
                }
            }
        }

        /// <summary>
        /// See <see cref="IGridBase.ApplyFilter"/>
        /// </summary>
        public void ApplyFilter(IFilterClause filterClause)
        {
            if (_dataTableDefaultView == null)
            {
                throw new GridBaseInitialiseException(
                    "You cannot apply filters as the collection for the grid has not been set");
            }
            if (filterClause != null)
            {
                _dataTableDefaultView.RowFilter = filterClause.GetFilterClauseString();
            }
            else
            {
                _dataTableDefaultView.RowFilter = null;
            }
        }
        
        /// <summary>
        /// See <see cref="IGridBase.RefreshGrid"/>
        /// </summary>
        public void RefreshGrid()
        {
            IBusinessObjectCollection col = this._gridBase.GetBusinessObjectCollection();
            IBusinessObject bo = this._gridBase.SelectedBusinessObject;
            SetBusinessObjectCollection(null);
            SetBusinessObjectCollection(col);
            SelectedBusinessObject = bo;
        }
    }

    /// <summary>
    /// Thrown when a failure occurs while setting up a grid
    /// </summary>
    public class GridBaseSetUpException : Exception
    {
        public GridBaseSetUpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GridBaseSetUpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GridBaseSetUpException(string message) : base(message)
        {
        }

        public GridBaseSetUpException()
        {
        }
    }

    /// <summary>
    /// Thrown when a failure occurs on a grid, indicating that Habanero developers
    /// need to pay attention to aspects of the code
    /// </summary>
    public class GridDeveloperException : HabaneroDeveloperException
    {
        public GridDeveloperException(string message) : base(message, "")
        {
        }

        public GridDeveloperException(string message, Exception inner) : base(message, "", inner)
        {
        }

        public GridDeveloperException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GridDeveloperException()
        {
        }
    }

    /// <summary>
    /// Thrown when a failure occurred during the initialisation of a grid
    /// </summary>
    public class GridBaseInitialiseException : HabaneroDeveloperException
    {
        public GridBaseInitialiseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public GridBaseInitialiseException(string message, Exception inner) : base(message, "", inner)
        {
        }

        public GridBaseInitialiseException(string message) : base(message, "")
        {
        }

        public GridBaseInitialiseException()
        {
        }
    }
}