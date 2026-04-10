using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.HierarchyV2;

internal class CollectionViewDragger : DragEventsProcessor
{
	internal struct DragPosition : IEquatable<DragPosition>
	{
		public int insertAtIndex;

		public RecycledItem recycledItem;

		public DragAndDropPosition dropPosition;

		public bool Equals(DragPosition other)
		{
			return insertAtIndex == other.insertAtIndex && object.Equals(recycledItem, other.recycledItem) && dropPosition == other.dropPosition;
		}

		public override bool Equals(object obj)
		{
			return obj is DragPosition other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(insertAtIndex, recycledItem, dropPosition);
		}
	}

	private const int k_AutoScrollAreaSize = 10;

	private const int k_PanSpeed = 20;

	private const int k_BetweenElementsAreaSize = 5;

	private const int k_DragHoverBarHeight = 2;

	private const int k_DefaultCursorId = 8;

	private DragPosition m_LastDragPosition;

	private VisualElement m_DragHoverBar;

	private CollectionView targetView => m_Target as CollectionView;

	private ScrollContainer targetScrollView => targetView.scrollView;

	private bool enabled { get; set; } = true;

	public ICollectionDragAndDropController dragAndDropController { get; set; }

	public CollectionViewDragger(CollectionView listView)
		: base(listView)
	{
	}

	protected override bool CanStartDrag(Vector3 pointerPosition, EventModifiers modifiers)
	{
		if (dragAndDropController == null)
		{
			return false;
		}
		if (!targetScrollView.contentContainer.worldBound.Contains(pointerPosition))
		{
			return false;
		}
		RecycledItem recycledItem = GetRecycledItem(pointerPosition);
		if (recycledItem != null && targetView.HasCanStartDrag())
		{
			IEnumerable<int> enumerable2;
			if (!targetView.hasSelection)
			{
				IEnumerable<int> enumerable = new int[1] { recycledItem.index };
				enumerable2 = enumerable;
			}
			else
			{
				enumerable2 = targetView.selectedIndices;
			}
			IEnumerable<int> indices = enumerable2;
			return targetView.RaiseCanStartDrag(recycledItem, indices, modifiers);
		}
		if (targetView.hasSelection)
		{
			return dragAndDropController.CanStartDrag(targetView.selectedIndices);
		}
		return recycledItem != null && dragAndDropController.CanStartDrag(new int[1] { recycledItem.index });
	}

	protected internal override StartDragArgs StartDrag(Vector3 pointerPosition, EventModifiers modifiers)
	{
		RecycledItem recycledItem = GetRecycledItem(pointerPosition);
		IEnumerable<int> itemIds;
		if (recycledItem != null)
		{
			if (targetView.selectionType == SelectionType.None)
			{
				itemIds = new int[1] { recycledItem.index };
			}
			else
			{
				if (!targetView.IsSelected(recycledItem.index))
				{
					targetView.SetSelection(recycledItem.index);
				}
				itemIds = targetView.selectedIndices;
			}
		}
		else
		{
			IEnumerable<int> enumerable2;
			if (!targetView.hasSelection)
			{
				IEnumerable<int> enumerable = Array.Empty<int>();
				enumerable2 = enumerable;
			}
			else
			{
				enumerable2 = targetView.selectedIndices;
			}
			itemIds = enumerable2;
		}
		StartDragArgs args = dragAndDropController.SetupDragAndDrop(itemIds);
		args.modifiers = modifiers;
		args = targetView.RaiseSetupDragAndDrop(recycledItem, dragAndDropController.GetSortedSelectedIds(), args);
		args.SetGenericData("__unity-drag-and-drop__source-view", targetView);
		return args;
	}

	protected internal override void UpdateDrag(Vector3 pointerPosition, EventModifiers modifiers)
	{
		DragPosition dragPosition = default(DragPosition);
		DragVisualMode visualMode = GetVisualMode(pointerPosition, modifiers, ref dragPosition);
		if (visualMode == DragVisualMode.Rejected)
		{
			ClearDragAndDropUI(dragCancelled: false);
		}
		else
		{
			HandleDragAndScroll(pointerPosition);
			ApplyDragAndDropUI(dragPosition);
		}
		base.dragAndDrop.SetVisualMode(visualMode);
		base.dragAndDrop.UpdateDrag(pointerPosition);
	}

	protected internal override void OnDrop(Vector3 pointerPosition, EventModifiers modifiers)
	{
		DragPosition dragPosition = default(DragPosition);
		if (!TryGetDragPosition(pointerPosition, ref dragPosition))
		{
			return;
		}
		DragAndDropArgs dragAndDropArgs = MakeDragAndDropArgs(dragPosition, modifiers);
		switch (targetView.RaiseDrop(pointerPosition, dragAndDropArgs))
		{
		default:
			base.dragAndDrop.AcceptDrag();
			break;
		case DragVisualMode.Rejected:
			dragAndDropController.DragCleanup();
			break;
		case DragVisualMode.None:
			if (!IsDraggingDisabled())
			{
				if (dragAndDropController.HandleDragAndDrop(dragAndDropArgs) != DragVisualMode.Rejected)
				{
					dragAndDropController.OnDrop(dragAndDropArgs);
					base.dragAndDrop.AcceptDrag();
				}
				else
				{
					dragAndDropController.DragCleanup();
				}
			}
			break;
		}
	}

	protected override void ClearDragAndDropUI(bool dragCancelled)
	{
		if (dragCancelled)
		{
			dragAndDropController.DragCleanup();
		}
		targetView.elementPanel.cursorManager.ResetCursor();
		m_LastDragPosition = default(DragPosition);
		foreach (RecycledItem value in targetView.m_IndexToItemDictionary.Values)
		{
			value.element.RemoveFromClassList(BaseVerticalCollectionView.itemDragHoverUssClassName);
		}
		if (m_DragHoverBar != null)
		{
			m_DragHoverBar.style.visibility = Visibility.Hidden;
		}
	}

	internal void HandleDragAndScroll(Vector2 pointerPosition)
	{
		double num = 0.0;
		if (pointerPosition.y < targetScrollView.worldBound.yMin + 10f)
		{
			num = targetScrollView.verticalScroller.value + -20.0;
			if (num <= (double)targetScrollView.worldBound.yMin)
			{
				num = 0.0;
			}
		}
		else
		{
			if (!(pointerPosition.y > targetScrollView.worldBound.yMax - 10f))
			{
				return;
			}
			float num2 = targetView.averageItemHeight * (float)targetView.itemsSource.Count;
			float height = targetScrollView.contentContainer.resolvedStyle.height;
			if (num2 > height)
			{
				double value = targetScrollView.verticalScroller.value;
				num = ((value + 20.0 > (double)(num2 - height)) ? value : (value + 20.0));
			}
		}
		targetView.UpdateVerticalScrollValue(num);
	}

	private DragVisualMode GetVisualMode(Vector3 pointerPosition, EventModifiers modifiers, ref DragPosition dragPosition)
	{
		if (dragAndDropController == null || !dragAndDropController.CanDrop())
		{
			return DragVisualMode.Rejected;
		}
		bool flag = TryGetDragPosition(pointerPosition, ref dragPosition);
		DragAndDropArgs dragAndDropArgs = MakeDragAndDropArgs(dragPosition, modifiers);
		DragVisualMode dragVisualMode = targetView.RaiseHandleDragAndDrop(pointerPosition, dragAndDropArgs);
		if (dragVisualMode != DragVisualMode.None)
		{
			return dragVisualMode;
		}
		return flag ? dragAndDropController.HandleDragAndDrop(dragAndDropArgs) : DragVisualMode.Rejected;
	}

	private VisualElement CreateDragHoverBar()
	{
		VisualElement visualElement = new VisualElement();
		visualElement.pickingMode = PickingMode.Ignore;
		visualElement.style.width = targetView.localBound.width;
		visualElement.style.visibility = Visibility.Hidden;
		VisualElement visualElement2 = visualElement;
		visualElement2.AddToClassList(BaseVerticalCollectionView.dragHoverBarUssClassName);
		targetView.RegisterCallback<GeometryChangedEvent>(delegate
		{
			m_DragHoverBar.style.width = targetView.localBound.width;
		});
		return visualElement2;
	}

	private void ApplyDragAndDropUI(DragPosition dragPosition)
	{
		if (m_LastDragPosition.Equals(dragPosition) || IsDraggingDisabled())
		{
			return;
		}
		if (m_DragHoverBar == null)
		{
			m_DragHoverBar = CreateDragHoverBar();
		}
		targetScrollView.viewport.Add(m_DragHoverBar);
		ClearDragAndDropUI(dragCancelled: false);
		m_LastDragPosition = dragPosition;
		switch (dragPosition.dropPosition)
		{
		case DragAndDropPosition.OverItem:
			dragPosition.recycledItem.element.AddToClassList(BaseVerticalCollectionView.itemDragHoverUssClassName);
			break;
		case DragAndDropPosition.BetweenItems:
		{
			if (dragPosition.insertAtIndex == 0)
			{
				PlaceHoverBarAt(0f);
				break;
			}
			VisualElement rootElementForIndex2 = targetView.GetRootElementForIndex(dragPosition.insertAtIndex - 1);
			PlaceHoverBarAtElement(rootElementForIndex2 ?? targetView.GetRootElementForIndex(dragPosition.insertAtIndex));
			break;
		}
		case DragAndDropPosition.OutsideItems:
		{
			VisualElement rootElementForIndex = targetView.GetRootElementForIndex(targetView.itemsSource.Count - 1);
			if (rootElementForIndex != null)
			{
				PlaceHoverBarAtElement(rootElementForIndex);
			}
			else
			{
				PlaceHoverBarAt(0f);
			}
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("dropPosition", dragPosition.dropPosition, "Unsupported dropPosition value");
		}
	}

	private bool TryGetDragPosition(Vector2 pointerPosition, ref DragPosition dragPosition)
	{
		RecycledItem recycledItem = GetRecycledItem(pointerPosition);
		if (recycledItem == null)
		{
			if (!targetView.worldBound.Contains(pointerPosition))
			{
				return false;
			}
			dragPosition.dropPosition = DragAndDropPosition.OutsideItems;
			if (pointerPosition.y <= targetScrollView.contentContainer.worldBound.yMin)
			{
				dragPosition.insertAtIndex = 0;
			}
			else
			{
				dragPosition.insertAtIndex = targetView.itemsSource.Count;
			}
			return true;
		}
		if (recycledItem.element.worldBound.yMax - pointerPosition.y < 5f)
		{
			dragPosition.insertAtIndex = recycledItem.index + 1;
			dragPosition.dropPosition = DragAndDropPosition.BetweenItems;
		}
		else if (pointerPosition.y - recycledItem.element.worldBound.yMin > 5f)
		{
			Vector2 containerOffset = targetScrollView.containerOffset;
			targetView.ScrollToItem(recycledItem.index);
			if (!Mathf.Approximately(containerOffset.x, targetScrollView.containerOffset.x) || !Mathf.Approximately(containerOffset.y, targetScrollView.containerOffset.y))
			{
				return TryGetDragPosition(pointerPosition, ref dragPosition);
			}
			dragPosition.recycledItem = recycledItem;
			dragPosition.insertAtIndex = recycledItem.index;
			dragPosition.dropPosition = DragAndDropPosition.OverItem;
		}
		else
		{
			dragPosition.insertAtIndex = recycledItem.index;
			dragPosition.dropPosition = DragAndDropPosition.BetweenItems;
		}
		return true;
	}

	private DragAndDropArgs MakeDragAndDropArgs(DragPosition dragPosition, EventModifiers modifiers)
	{
		object target = null;
		RecycledItem recycledItem = dragPosition.recycledItem;
		if (recycledItem != null)
		{
			target = targetView.itemsSource[recycledItem.index];
		}
		return new DragAndDropArgs
		{
			target = target,
			insertAtIndex = dragPosition.insertAtIndex,
			dragAndDropPosition = dragPosition.dropPosition,
			dragAndDropData = DragAndDropUtility.GetDragAndDrop(m_Target.panel).data,
			modifiers = modifiers
		};
	}

	private float GetHoverBarTopPosition(VisualElement item)
	{
		VisualElement viewport = targetScrollView.viewport;
		return Mathf.Min(viewport.WorldToLocal(item.worldBound).yMax, viewport.localBound.yMax - 2f);
	}

	private void PlaceHoverBarAtElement(VisualElement item)
	{
		PlaceHoverBarAt(GetHoverBarTopPosition(item));
	}

	private void PlaceHoverBarAt(float top)
	{
		m_DragHoverBar.style.top = top;
		m_DragHoverBar.style.visibility = Visibility.Visible;
		m_DragHoverBar.style.marginLeft = 0f;
		m_DragHoverBar.style.width = targetView.localBound.width;
	}

	private RecycledItem GetRecycledItem(Vector3 pointerPosition)
	{
		foreach (RecycledItem value in targetView.m_IndexToItemDictionary.Values)
		{
			if (value.element.worldBound.Contains(pointerPosition))
			{
				return value;
			}
		}
		return null;
	}

	private bool IsDraggingDisabled()
	{
		return targetView == base.dragAndDrop.data.source && !enabled;
	}
}
