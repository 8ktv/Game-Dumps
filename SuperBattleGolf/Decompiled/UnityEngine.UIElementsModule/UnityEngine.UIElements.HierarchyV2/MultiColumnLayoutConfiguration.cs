using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.UIElements.Internal;

namespace UnityEngine.UIElements.HierarchyV2;

internal class MultiColumnLayoutConfiguration : CollectionViewLayoutConfiguration
{
	private Columns m_Columns;

	private MultiColumnCollectionHeader m_MultiColumnHeader;

	private VisualElement m_HeaderContainer;

	private const string k_HeaderViewDataKey = "Header";

	private const string k_HeaderContainerViewDataKey = "unity-multi-column-header-container";

	private readonly PropertyName k_BoundColumnVePropertyName = "__unity-multi-column-bound-column";

	private readonly PropertyName bindableElementPropertyName = "__unity-multi-column-bindable-element";

	internal MultiColumnCollectionHeader header => m_MultiColumnHeader;

	public VisualElement headerContainer => m_HeaderContainer;

	[CreateProperty]
	public Columns columns
	{
		get
		{
			return m_Columns;
		}
		set
		{
			if (value == null)
			{
				m_Columns.Clear();
				return;
			}
			m_Columns = value;
			if (m_Columns.Count > 0)
			{
				CreateMultiColumnHeader();
			}
		}
	}

	public event Action<ContextualMenuPopulateEvent, Column> headerContextMenuPopulateEvent;

	public MultiColumnLayoutConfiguration()
	{
		columns = new Columns();
		base.makeCell = (Func<VisualElement>)Delegate.Combine(base.makeCell, new Func<VisualElement>(MakeCell));
		base.bindCell = (Action<VisualElement, int>)Delegate.Combine(base.bindCell, new Action<VisualElement, int>(BindCell));
		base.unbindCell = (Action<VisualElement, int>)Delegate.Combine(base.unbindCell, new Action<VisualElement, int>(UnbindCell));
		base.destroyCell = (Action<VisualElement>)Delegate.Combine(base.destroyCell, new Action<VisualElement>(DestroyCell));
	}

	private VisualElement DefaultMakeCellItem()
	{
		Label label = new Label();
		label.AddToClassList(MultiColumnController.cellUssClassName);
		return label;
	}

	private VisualElement MakeCell()
	{
		if (m_MultiColumnHeader == null)
		{
			return new Label();
		}
		VisualElement visualElement = new VisualElement
		{
			name = MultiColumnController.rowContainerUssClassName
		};
		visualElement.AddToClassList(MultiColumnController.rowContainerUssClassName);
		foreach (Column visible in m_MultiColumnHeader.columns.visibleList)
		{
			VisualElement visualElement2 = new VisualElement();
			visualElement2.AddToClassList(MultiColumnController.cellUssClassName);
			VisualElement visualElement3 = visible.makeCell?.Invoke() ?? DefaultMakeCellItem();
			visualElement2.SetProperty(bindableElementPropertyName, visualElement3);
			visualElement2.Add(visualElement3);
			visualElement.Add(visualElement2);
		}
		return visualElement;
	}

	private void BindCell(VisualElement element, int index)
	{
		int num = 0;
		element.style.width = header.worldBoundingBox.width;
		foreach (Column visible in m_MultiColumnHeader.columns.visibleList)
		{
			if (m_MultiColumnHeader.columnDataMap.TryGetValue(visible, out var value))
			{
				VisualElement visualElement = element[num++];
				VisualElement arg = visualElement.GetProperty(bindableElementPropertyName) as VisualElement;
				if (visible.bindCell != null)
				{
					visible.bindCell(arg, index);
				}
				visualElement.style.width = value.control.resolvedStyle.width;
				visualElement.SetProperty(k_BoundColumnVePropertyName, visible);
			}
		}
	}

	private void UnbindCell(VisualElement element, int index)
	{
		foreach (VisualElement item in element.Children())
		{
			if (item.GetProperty(k_BoundColumnVePropertyName) is Column column)
			{
				VisualElement arg = item.GetProperty(bindableElementPropertyName) as VisualElement;
				column.unbindCell?.Invoke(arg, index);
			}
		}
	}

	private void DestroyCell(VisualElement element)
	{
		foreach (VisualElement item in element.Children())
		{
			if (item.GetProperty(k_BoundColumnVePropertyName) is Column column)
			{
				VisualElement obj = item.GetProperty(bindableElementPropertyName) as VisualElement;
				column.destroyCell?.Invoke(obj);
				item.ClearProperty(k_BoundColumnVePropertyName);
			}
		}
	}

	public VisualElement CreateMultiColumnHeader()
	{
		if (m_MultiColumnHeader != null)
		{
			Dispose();
		}
		m_MultiColumnHeader = new MultiColumnCollectionHeader(columns, new SortColumnDescriptions(), new List<SortColumnDescription>())
		{
			viewDataKey = "Header"
		};
		m_MultiColumnHeader.contextMenuPopulateEvent += OnContextMenuPopulateEvent;
		m_MultiColumnHeader.columnResized += OnColumnResized;
		m_MultiColumnHeader.viewDataRestored += OnViewDataRestored;
		m_MultiColumnHeader.columns.columnAdded += OnColumnAdded;
		m_MultiColumnHeader.columns.columnRemoved += OnColumnRemoved;
		m_MultiColumnHeader.columns.columnReordered += OnColumnReordered;
		m_MultiColumnHeader.columns.columnChanged += OnColumnsChanged;
		m_MultiColumnHeader.columns.changed += OnColumnChanged;
		m_HeaderContainer = new VisualElement
		{
			name = MultiColumnController.headerContainerUssClassName
		};
		m_HeaderContainer.AddToClassList(MultiColumnController.headerContainerUssClassName);
		m_HeaderContainer.viewDataKey = "unity-multi-column-header-container";
		m_HeaderContainer.Add(m_MultiColumnHeader);
		return m_HeaderContainer;
	}

	private void Dispose()
	{
		m_MultiColumnHeader.contextMenuPopulateEvent -= OnContextMenuPopulateEvent;
		m_MultiColumnHeader.columnResized -= OnColumnResized;
		m_MultiColumnHeader.viewDataRestored -= OnViewDataRestored;
		m_MultiColumnHeader.columns.columnAdded -= OnColumnAdded;
		m_MultiColumnHeader.columns.columnRemoved -= OnColumnRemoved;
		m_MultiColumnHeader.columns.columnReordered -= OnColumnReordered;
		m_MultiColumnHeader.columns.columnChanged -= OnColumnsChanged;
		m_MultiColumnHeader.columns.changed -= OnColumnChanged;
		m_MultiColumnHeader.RemoveFromHierarchy();
		m_MultiColumnHeader.Dispose();
		m_MultiColumnHeader = null;
		m_HeaderContainer.RemoveFromHierarchy();
		m_HeaderContainer = null;
	}

	private void OnContextMenuPopulateEvent(ContextualMenuPopulateEvent evt, Column column)
	{
		this.headerContextMenuPopulateEvent?.Invoke(evt, column);
	}

	private void OnColumnResized(int index, float width)
	{
		if (!m_View.isRebuildScheduled)
		{
			m_View.Query<VisualElement>(null, MultiColumnController.rowContainerUssClassName).ForEach((VisualElement element) => element[index].style.width = width);
		}
	}

	private void OnColumnAdded(Column column, int index)
	{
		m_View.Rebuild();
	}

	private void OnColumnRemoved(Column column)
	{
		m_View.Rebuild();
	}

	private void OnColumnReordered(Column column, int from, int to)
	{
		if (!m_MultiColumnHeader.isApplyingViewState)
		{
			m_View.Rebuild();
		}
	}

	private void OnColumnsChanged(Column column, ColumnDataType type)
	{
		if (!m_MultiColumnHeader.isApplyingViewState && type == ColumnDataType.Visibility)
		{
			m_View.ScheduleRebuild();
		}
	}

	private void OnColumnChanged(ColumnsDataType type)
	{
		if (!m_MultiColumnHeader.isApplyingViewState && type == ColumnsDataType.PrimaryColumn)
		{
			m_View.ScheduleRebuild();
		}
	}

	private void OnViewDataRestored()
	{
		m_View.Rebuild();
	}
}
