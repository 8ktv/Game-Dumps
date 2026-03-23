using System.Collections.Generic;
using UnityEngine.UIElements.Internal;

namespace UnityEngine.UIElements;

public class MultiColumnListViewController : BaseListViewController
{
	private MultiColumnController m_ColumnController;

	public MultiColumnController columnController => m_ColumnController;

	internal MultiColumnCollectionHeader header => m_ColumnController?.header;

	public MultiColumnListViewController(Columns columns, SortColumnDescriptions sortDescriptions, List<SortColumnDescription> sortedColumns)
	{
		m_ColumnController = new MultiColumnController(columns, sortDescriptions, sortedColumns);
		base.itemsSourceSizeChanged += SortIfNeeded;
		base.itemsSourceChanged += SortIfNeeded;
	}

	internal override void PreRefresh()
	{
		base.PreRefresh();
		m_ColumnController.SortIfNeeded();
	}

	private void SortIfNeeded()
	{
		m_ColumnController.UpdateDragger();
		if (m_ColumnController.sortingMode == ColumnSortingMode.Default)
		{
			base.view.RefreshItems();
		}
	}

	internal override void InvokeMakeItem(ReusableCollectionItem reusableItem)
	{
		if (reusableItem is ReusableMultiColumnListViewItem reusableMultiColumnListViewItem)
		{
			reusableMultiColumnListViewItem.Init(MakeItem(), m_ColumnController.header.columns, base.baseListView.reorderMode == ListViewReorderMode.Animated);
			PostInitRegistration(reusableMultiColumnListViewItem);
		}
		else
		{
			base.InvokeMakeItem(reusableItem);
		}
	}

	internal override void InvokeBindItem(ReusableCollectionItem reusableItem, int index)
	{
		base.InvokeBindItem(reusableItem, index);
		if (reusableItem is ReusableListViewItem reusableListViewItem)
		{
			bool flag = m_ColumnController.header.sortingEnabled && m_ColumnController.header.sortedColumnReadonly.Count > 0;
			reusableListViewItem.SetDragHandleEnabled(!flag);
		}
	}

	public override object GetItemForIndex(int index)
	{
		int sourceIndex = columnController.GetSourceIndex(index);
		return base.GetItemForIndex(sourceIndex);
	}

	public override int GetIndexForId(int id)
	{
		int indexForId = base.GetIndexForId(id);
		return columnController.GetSortedIndex(indexForId);
	}

	public override int GetIdForIndex(int index)
	{
		int sourceIndex = columnController.GetSourceIndex(index);
		return base.GetIdForIndex(sourceIndex);
	}

	protected override VisualElement MakeItem()
	{
		return m_ColumnController.MakeItem();
	}

	protected override void BindItem(VisualElement element, int index)
	{
		m_ColumnController.BindItem(element, index, GetItemForIndex(index));
	}

	protected override void UnbindItem(VisualElement element, int index)
	{
		m_ColumnController.UnbindItem(element, index);
	}

	protected override void DestroyItem(VisualElement element)
	{
		m_ColumnController.DestroyItem(element);
	}

	protected override void PrepareView()
	{
		m_ColumnController.PrepareView(base.view);
		base.baseListView.reorderModeChanged += UpdateReorderClassList;
	}

	public override void Dispose()
	{
		base.baseListView.reorderModeChanged -= UpdateReorderClassList;
		m_ColumnController.Dispose();
		m_ColumnController = null;
		base.Dispose();
	}

	private void UpdateReorderClassList()
	{
		m_ColumnController.header.EnableInClassList(MultiColumnCollectionHeader.reorderableUssClassName, base.baseListView.reorderable && base.baseListView.reorderMode == ListViewReorderMode.Animated);
	}
}
