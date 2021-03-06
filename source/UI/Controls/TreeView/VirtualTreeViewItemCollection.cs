﻿// <copyright file="VirtualTreeItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.TreeView.Data;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class VirtualTreeViewItemCollection : VirtualItemCollection<TreeViewControl, TreeViewItem>, IItemCollection<TreeViewItem>
	{
		private TreeViewData _treeViewData;

		public VirtualTreeViewItemCollection(TreeViewControl treeViewControl)
		{
			TreeViewControl = treeViewControl;
		}

		public TreeViewItemRootCollection Items => TreeViewControl.Items;

		public TreeViewControl TreeViewControl { get; }

		public TreeViewData TreeViewData
		{
			get => _treeViewData;
			set
			{
				if (ReferenceEquals(_treeViewData, value))
					return;

				_treeViewData = value;

				Source = _treeViewData.DataPlainListView;
			}
		}

		protected override void ObservableSourceOnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.ObservableSourceOnCollectionChanged(e);

			TreeViewControl.InvalidatePanelInternal();
		}

		protected override void OnGeneratedItemAttached(int index, TreeViewItem item)
		{
			base.OnGeneratedItemAttached(index, item);

			item.TreeViewItemData = TreeViewData.GetNode(index);
			
			var itemsControl = (IItemsControl<TreeViewItem>)TreeViewControl;

			itemsControl.OnItemAttaching(item);
			itemsControl.OnItemAttached(item);
		}

		protected override void OnGeneratedItemDetached(int index, TreeViewItem item)
		{
			item.TreeViewItemData = null;

			var itemsControl = (IItemsControl<TreeViewItem>)TreeViewControl;

			itemsControl.OnItemDetaching(item);
			itemsControl.OnItemDetached(item);
			
			base.OnGeneratedItemDetached(index, item);
		}

		public int ActualCount => TreeViewData?.VisibleFlatCount ?? 0;

		public override int GetIndexFromItem(TreeViewItem item)
		{
			if (TreeViewData == null || item?.TreeViewItemData == null)
				return -1;

			return TreeViewData.FindIndex(item.TreeViewItemData);
		}

		public void BringIntoView(int index)
		{
			var virtualPanel = (IItemsHost<TreeViewItem>) TreeViewControl?.ItemsPresenterInternal?.ItemsHostInternal;

			virtualPanel?.BringIntoView(new BringIntoViewRequest<TreeViewItem>(index, TreeViewControl.DefaultBringIntoViewMode));
		}
	}
}