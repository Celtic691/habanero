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
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using Gizmox.WebGUI.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.UI.Base;

namespace Habanero.UI.WebGUI
{
    public partial class ListViewGiz : ListView, IListView
    {
        private Hashtable _listItemsHash;
        private ListViewItemCollectionGiz _objectCollection;
        private SelectedListViewItemCollectionGiz _selectedObjectCollection;

        public ListViewGiz()
        {
            InitializeComponent();
            _listItemsHash = new Hashtable();
            _objectCollection = new ListViewItemCollectionGiz(base.Items);
            _selectedObjectCollection = new SelectedListViewItemCollectionGiz(base.SelectedItems);
        }

        public ListViewGiz(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            ListView giz = new ListView();
            
           
        }


        public IListViewItemCollection Items
        {
            get { return _objectCollection; }
        }

        public IListViewItem SelectedItem
        {
            get { return (IListViewItem) base.SelectedItem; }
            set { base.SelectedItem=(ListViewItem) value; }
        }

        public ISelectedListViewItemCollection SelectedItems
        {
            get { return (ISelectedListViewItemCollection)new SelectedListViewItemCollectionGiz(base.SelectedItems); }
        }

        public IColumnHeaderCollection Columns
        {
            get { return new IColumnHeaderCollectionGiz(base.Columns); ; }
        }


        /// <summary>
        /// Adds the business objects in the collection to the ListView. This
        /// method is used by SetCollection
        /// </summary>
        /// <param name="collection">The business object collection</param>
        public void SetCollection(IBusinessObjectCollection collection)
        {
            base.Clear();
            _listItemsHash.Clear();
            base.MultiSelect = true;
            foreach (IBusinessObject bo in collection)
            {
                base.Items.Add(CreateListViewItem(bo));
            }
        }

        public IListViewItem CreateListViewItem(string displayName)
        {
            return (new ListViewItemGiz(displayName));
        }

        /// <summary>
        /// Creates a ListViewItem from the business object provided.  This
        /// method is used by SetListViewCollection() to populate the ListView.
        /// </summary>
        /// <param name="bo">The business object to represent</param>
        /// <returns>Returns a new ListViewItem</returns>
        private ListViewItem CreateListViewItem(IBusinessObject bo)
        {
            ListViewItem boItem = new ListViewItem(bo.ToString());
            boItem.Tag = bo;
            _listItemsHash.Add(bo, boItem);
            return boItem;
        }

        public IControlCollection Controls
        {
            get { throw new NotImplementedException(); }
        }
        Base.DockStyle IControlChilli.Dock
        {
            get { return (Base.DockStyle)base.Dock; }
            set { base.Dock = (Gizmox.WebGUI.Forms.DockStyle)value; }
        }
    }

    internal class IColumnHeaderCollectionGiz : IColumnHeaderCollection
    {
        private readonly ListView.ColumnHeaderCollection _columns;

        public IColumnHeaderCollectionGiz(ListView.ColumnHeaderCollection columns)
        {
            if (columns == null) throw new ArgumentNullException("columns");
            _columns = columns;
        }


        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value></value>
        public int Count
        {
            get { return _columns.Count; }
        }
    }

    internal class ListViewItemCollectionGiz : IListViewItemCollection
    {
        private readonly ListView.ListViewItemCollection _items;

        public ListViewItemCollectionGiz(ListView.ListViewItemCollection items)
        {
            this._items = items;
        }


        //public IListViewItem Add(IListViewItem objListViewItem)
        //{
        //    return _items.Add((ListViewItem) objListViewItem);
        //}

        public IListViewItem Add(IListViewItem objListViewItem)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public IListViewItem this[int index]
        {
            get { return new ListViewItemGiz(_items[index]); }
        }

        public IListViewItem Add(string strText)
        {
            return new ListViewItemGiz(_items.Add(strText));
        }


    }

    internal class ListViewItemGiz: IListViewItem
    {
        private readonly ListViewItem _item;

        public ListViewItemGiz(ListViewItem item)
        {
            this._item = item;
        }

        public ListViewItemGiz(string displayName)
        {
            this._item.Text = displayName;
        }

        public void Remove()
        {
            _item.Remove();
        }

        public int Index
        {
            get { return _item.Index; }
        }

        public bool Selected
        {
            get { return _item.Selected; }
            set { _item.Selected = value; }
        }

        public bool Checked
        {
            get { return _item.Checked; }
            set { _item.Checked=value; }
        }

        public IListView ListView
        {
            get { return (IListView) _item.ListView; }
        }

        public string Text
        {
            get { return _item.Text; }
            set { _item.Text=value; }
        }

        public object Tag
        {
            get { return(_item.Tag); }
            set { _item.Tag = value; }
        }
    }


    internal class SelectedListViewItemCollectionGiz: ISelectedListViewItemCollection
    {
        private ListView.SelectedListViewItemCollection _selectedObjectCollection;


        public SelectedListViewItemCollectionGiz(ListView.SelectedListViewItemCollection items)
        {
            _selectedObjectCollection = items;
        }

        public int Count
        {
            get { return _selectedObjectCollection.Count; }
        }

        public void Add(object item)
        {
            
        }

        public IEnumerator GetEnumerator()
        {
           return _selectedObjectCollection.GetEnumerator();
        }

        public bool IsReadOnly
        {
            get { return _selectedObjectCollection.IsReadOnly; }
        }

        public IListViewItem this[int index]
        {
            get { return (IListViewItem) _selectedObjectCollection[index]; }
        }

        public bool Contains(IListViewItem objListViewItem)
        {
           return _selectedObjectCollection.Contains((ListViewItem) objListViewItem);
        }

        public int IndexOf(IListViewItem objListViewItem)
        {
            return _selectedObjectCollection.IndexOf((ListViewItem) objListViewItem);
        }
    }
}
