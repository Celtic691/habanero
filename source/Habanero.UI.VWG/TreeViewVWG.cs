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
using Gizmox.WebGUI.Forms;
using Habanero.UI.Base;
using TreeViewAction=Habanero.UI.Base.TreeViewAction;
using TreeViewCancelEventArgs = Gizmox.WebGUI.Forms.TreeViewCancelEventArgs;
using TreeViewCancelEventHandler = Habanero.UI.Base.TreeViewCancelEventHandler;
using TreeViewEventArgs=Gizmox.WebGUI.Forms.TreeViewEventArgs;
using TreeViewEventHandler=Habanero.UI.Base.TreeViewEventHandler;

namespace Habanero.UI.VWG
{
    /// <summary>
    /// Displays a hierarchical collection of labeled items, each represented by a TreeNode
    /// </summary>
    public class TreeViewVWG : TreeView, ITreeView
    {
        private ITreeNode _topNode;
        private event TreeViewEventHandler _afterSelect;
        private event TreeViewCancelEventHandler _beforeSelect;

        #region Utility Methods

        private static ITreeNode GetITreeNode(TreeNode treeNode)
        {
            if (treeNode == null) return null;
            return new TreeNodeVWG(treeNode);
            //ITreeNode treeNodeValue = treeNode as ITreeNode;
            //if (treeNodeValue == null)
            //{
            //    treeNodeValue = new TreeNodeVWG(treeNode);
            //}
            //return treeNodeValue;
        }

        private static TreeNode GetTreeNode(ITreeNode treeNode)
        {
            if (treeNode == null) return null;
            return (TreeNode)treeNode.OriginalNode;
        }

        #endregion // Utility Methods

        #region Event Handling

        event TreeViewEventHandler ITreeView.AfterSelect
        {
            add { _afterSelect += value; }
            remove { _afterSelect -= value; }
        }

        event TreeViewCancelEventHandler ITreeView.BeforeSelect
        {
            add { _beforeSelect += value; }
            remove { _beforeSelect -= value; }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            if (this._afterSelect != null)
            {
                Base.TreeViewEventArgs treeViewEventArgs = new Base.TreeViewEventArgs(
                    GetITreeNode(e.Node), (TreeViewAction)e.Action);
                this._afterSelect(this, treeViewEventArgs);
            }
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);
            if (this._beforeSelect != null && !e.Cancel)
            {
                Base.TreeViewCancelEventArgs treeViewCancelEventArgs = new Base.TreeViewCancelEventArgs(
                    GetITreeNode(e.Node), e.Cancel, (TreeViewAction)e.Action);
                this._beforeSelect(this, treeViewCancelEventArgs);
                e.Cancel = treeViewCancelEventArgs.Cancel;
            }
        }

        #endregion // Event Handling

        /// <summary>
        /// Gets or sets the anchoring style.
        /// </summary>
        /// <value></value>
        Base.AnchorStyles IControlHabanero.Anchor
        {
            get { return (Base.AnchorStyles)base.Anchor; }
            set { base.Anchor = (Gizmox.WebGUI.Forms.AnchorStyles)value; }
        }

        /// <summary>
        /// Gets the collection of controls contained within the control
        /// </summary>
        IControlCollection IControlHabanero.Controls
        {
            get { return new ControlCollectionVWG(base.Controls); }
        }

        /// <summary>
        /// Gets or sets which control borders are docked to its parent
        /// control and determines how a control is resized with its parent
        /// </summary>
        Base.DockStyle IControlHabanero.Dock
        {
            get { return (Base.DockStyle)base.Dock; }
            set { base.Dock = (Gizmox.WebGUI.Forms.DockStyle)value; }
        }

        public new ITreeNodeCollection Nodes
        {
            get { return new TreeNodeCollectionVWG(base.Nodes); }
        }

        public ITreeNode TopNode
        {
            get { return _topNode;}
            set { _topNode = value; }
        }

        public ITreeNode SelectedNode
        {
            get { return GetITreeNode(base.SelectedNode); }
            set { base.SelectedNode = GetTreeNode(value); }
        }

        public class TreeNodeVWG : TreeNode, ITreeNode
        {
            private readonly TreeNode _originalNode;

            public TreeNodeVWG(TreeNode node)
            {
                _originalNode = node;
            }

            string ITreeNode.Text
            {
                get { return _originalNode.Text; }
                set { _originalNode.Text = value; }
            }

            public ITreeNode Parent
            {
                get { return GetITreeNode(_originalNode.Parent); }
            }

            public ITreeNodeCollection Nodes
            {
                get { return new TreeNodeCollectionVWG(_originalNode.Nodes); }
            }

            public object OriginalNode
            {
                get { return _originalNode; }
            }
        }

        public class TreeNodeCollectionVWG : ITreeNodeCollection
        {
            private readonly TreeNodeCollection _nodes;

            public TreeNodeCollectionVWG(TreeNodeCollection nodes)
            {
                _nodes = nodes;
            }

            public int Count
            {
                get { return _nodes.Count; }
            }

            public ITreeNode this[int index]
            {
                get { return GetITreeNode(_nodes[index]); }
            }

            public void Add(ITreeNode treeNode)
            {
                _nodes.Add(GetTreeNode(treeNode));
            }
        }
    }
}