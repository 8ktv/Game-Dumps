using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.HierarchyV2;

internal class CollectionView : VisualElement
{
	private enum RangeSelectionDirection
	{
		Up = -1,
		None,
		Down
	}

	internal static readonly BindingId itemsSourceProperty = "itemsSource";

	internal static readonly BindingId selectionTypeProperty = "selectionType";

	internal static readonly BindingId selectedIndexProperty = "selectedIndex";

	internal static readonly BindingId reorderableProperty = "reorderable";

	internal static readonly BindingId reorderModeProperty = "reorderMode";

	internal static readonly BindingId showBorderProperty = "showBorder";

	internal static readonly BindingId showAlternatingRowBackgroundsProperty = "showAlternatingRowBackgrounds";

	internal static readonly BindingId fixedItemHeightProperty = "fixedItemHeight";

	internal static readonly BindingId selectedIndicesProperty = "selectedIndices";

	private VisualElement m_Container;

	private ScrollContainer m_ScrollView;

	private CollectionViewScroller m_VerticalScroller;

	private CollectionViewScroller m_HorizontalScroller;

	private CollectionViewDragger m_Dragger;

	private CollectionViewLayoutConfiguration m_Configuration;

	private IList m_ItemsSource;

	private LinkedList<RecycledItem> m_RefreshList = new LinkedList<RecycledItem>();

	private LinkedList<RecycledItem> m_DisplayedList = new LinkedList<RecycledItem>();

	private KeyboardNavigationManipulator m_NavigationManipulator;

	private IVisualElementScheduledItem m_RebuildScheduled;

	private IVisualElementScheduledItem m_ScrollScheduledItem;

	private List<int> m_LastFocusedElementTreeChildIndexes = new List<int>();

	private readonly LinkedList<RecycledItem> m_FreeList = new LinkedList<RecycledItem>();

	private readonly CollectionViewSelection m_Selection = new CollectionViewSelection();

	private bool m_IsChangingScrollingParameters;

	private double m_DelayedScrolledVerticalValue = 0.0;

	private double m_ScrollValue;

	private float m_FixedItemHeight = -1f;

	private float m_ComputedAverageHeight = -1f;

	private float m_LastHeight = -1f;

	private int m_FirstVisibleItemIndex;

	private int m_LastFocusedElementIndex = -1;

	private Vector3 m_TouchDownPosition;

	private AlternatingRowBackground m_ShowAlternatingRowBackgrounds = AlternatingRowBackground.None;

	private RangeSelectionDirection m_RangeSelectionDirection = RangeSelectionDirection.None;

	private SelectionType m_SelectionType;

	private ListViewReorderMode m_ReorderMode;

	private const float k_DefaultItemHeight = 22f;

	private const float k_ScrollThresholdSquared = 100f;

	private const float k_DefaultScrollSize = 10f;

	private const float k_Buffer = 1f;

	internal readonly Dictionary<int, RecycledItem> m_IndexToItemDictionary = new Dictionary<int, RecycledItem>();

	public static readonly string verticalScrollerVisibleUssClassName = BaseVerticalCollectionView.ussClassName + "--vertical-scroller-visible";

	internal CollectionViewDragger dragger => m_Dragger;

	internal bool isRebuildScheduled => m_RebuildScheduled?.isActive ?? false;

	internal bool processingPointerDownEvent
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		get;
		private set; }

	[CreateProperty]
	public IList itemsSource
	{
		get
		{
			return m_ItemsSource;
		}
		set
		{
			if (value != itemsSource)
			{
				m_ItemsSource = value;
				RefreshItems();
				NotifyPropertyChanged(in itemsSourceProperty);
			}
		}
	}

	public CollectionViewLayoutConfiguration layoutConfiguration
	{
		get
		{
			return m_Configuration;
		}
		set
		{
			if (value != null && value != m_Configuration)
			{
				m_Configuration = value;
				m_Configuration.m_View = this;
				if (value is MultiColumnLayoutConfiguration multiColumnLayoutConfiguration)
				{
					Insert(0, multiColumnLayoutConfiguration.CreateMultiColumnHeader());
				}
				UnbindAllItems();
				ResetAverageHeight();
				RefreshItems();
			}
		}
	}

	public ScrollContainer scrollView
	{
		get
		{
			return m_ScrollView;
		}
		private set
		{
			m_ScrollView = value;
		}
	}

	[CreateProperty]
	public float fixedItemHeight
	{
		get
		{
			return m_FixedItemHeight;
		}
		set
		{
			if (Math.Abs(m_FixedItemHeight - value) > float.Epsilon)
			{
				m_FixedItemHeight = value;
				NotifyPropertyChanged(in fixedItemHeightProperty);
			}
		}
	}

	[CreateProperty]
	public SelectionType selectionType
	{
		get
		{
			return m_SelectionType;
		}
		set
		{
			SelectionType selectionType = m_SelectionType;
			m_SelectionType = value;
			if (m_SelectionType == SelectionType.None)
			{
				ClearSelection();
			}
			else if (m_SelectionType == SelectionType.Single && m_Selection.indexCount > 1)
			{
				SetSelection(m_Selection.FirstIndex());
			}
			if (selectionType != m_SelectionType)
			{
				NotifyPropertyChanged(in selectionTypeProperty);
			}
		}
	}

	internal float averageItemHeight
	{
		get
		{
			if (fixedItemHeight > 0f)
			{
				return fixedItemHeight;
			}
			if (m_ComputedAverageHeight > 0f)
			{
				return m_ComputedAverageHeight;
			}
			return 22f;
		}
	}

	[CreateProperty]
	public bool showBorder
	{
		get
		{
			return m_ScrollView.contentContainer.ClassListContains(BaseVerticalCollectionView.borderUssClassName);
		}
		set
		{
			bool flag = showBorder;
			m_ScrollView.contentContainer.EnableInClassList(BaseVerticalCollectionView.borderUssClassName, value);
			if (flag != showBorder)
			{
				NotifyPropertyChanged(in showBorderProperty);
			}
		}
	}

	[CreateProperty]
	public AlternatingRowBackground showAlternatingRowBackgrounds
	{
		get
		{
			return m_ShowAlternatingRowBackgrounds;
		}
		set
		{
			if (m_ShowAlternatingRowBackgrounds != value)
			{
				m_ShowAlternatingRowBackgrounds = value;
				RefreshItems();
				NotifyPropertyChanged(in showAlternatingRowBackgroundsProperty);
			}
		}
	}

	[CreateProperty]
	public bool reorderable
	{
		get
		{
			return m_Dragger?.dragAndDropController?.enableReordering == true;
		}
		set
		{
			if (value != reorderable)
			{
				bool flag = reorderable;
				ICollectionDragAndDropController dragAndDropController = m_Dragger.dragAndDropController;
				if (dragAndDropController != null && dragAndDropController.enableReordering != value)
				{
					dragAndDropController.enableReordering = value;
					Rebuild();
				}
				if (flag != reorderable)
				{
					NotifyPropertyChanged(in reorderableProperty);
				}
			}
		}
	}

	[CreateProperty]
	public ListViewReorderMode reorderMode
	{
		get
		{
			return m_ReorderMode;
		}
		set
		{
			if (value != m_ReorderMode)
			{
				m_ReorderMode = value;
				InitializeDragAndDropController(reorderable);
				this.reorderModeChanged?.Invoke();
				Rebuild();
				NotifyPropertyChanged(in reorderModeProperty);
			}
		}
	}

	[CreateProperty]
	public int selectedIndex
	{
		get
		{
			return m_Selection.FirstIndex();
		}
		set
		{
			SetSelection(value);
		}
	}

	[CreateProperty(ReadOnly = true)]
	public IEnumerable<int> selectedIndices => m_Selection.indices;

	public bool hasSelection => m_Selection.indices.Count > 0;

	internal event Action reorderModeChanged;

	public event Func<CanStartDragArgs, bool> canStartDrag;

	public event Func<SetupDragAndDropArgs, StartDragArgs> setupDragAndDrop;

	public event Func<HandleDragAndDropArgs, DragVisualMode> dragAndDropUpdate;

	public event Func<HandleDragAndDropArgs, DragVisualMode> handleDrop;

	public event Action selectedIndicesChanged;

	private ICollectionDragAndDropController CreateDragAndDropController()
	{
		return new ReorderableDragAndDropController(this);
	}

	public void SetItemsSourceWithoutNotify(IList source)
	{
		m_ItemsSource = source;
	}

	public CollectionView()
	{
		focusable = true;
		base.isCompositeRoot = true;
		base.delegatesFocus = true;
		selectionType = SelectionType.Single;
		AddToClassList(BaseVerticalCollectionView.ussClassName);
		m_ScrollView = new ScrollContainer
		{
			focusable = true
		};
		m_Container = m_ScrollView.contentContainer;
		m_VerticalScroller = m_ScrollView.verticalScroller;
		m_VerticalScroller.RegisterValueChangedCallback(OnVerticalScrollingChangeEvent);
		m_HorizontalScroller = m_ScrollView.horizontalScroller;
		m_HorizontalScroller.RegisterValueChangedCallback(OnHorizontalScrollerChangeEvent);
		Add(m_ScrollView);
		InitializeDragAndDropController(reorderable);
		RegisterCallback<AttachToPanelEvent>(OnAttachToPanelEvent);
		RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEvent);
		RegisterCallback(delegate(GeometryChangedEvent evt)
		{
			ContainerSizeChanged(evt.newRect.height, evt.newRect.width);
		});
	}

	private void OnAttachToPanelEvent(AttachToPanelEvent evt)
	{
		if (evt.destinationPanel != null)
		{
			this.AddManipulator(m_NavigationManipulator = new KeyboardNavigationManipulator(Apply));
			RegisterCallback<PointerDownEvent>(OnPointerDown);
			RegisterCallback<PointerUpEvent>(OnPointerUp);
			RegisterCallback<PointerMoveEvent>(OnPointerMove);
			RegisterCallback<PointerCancelEvent>(OnPointerCancel);
		}
	}

	private void OnDetachFromPanelEvent(DetachFromPanelEvent evt)
	{
		if (evt.originPanel != null)
		{
			this.RemoveManipulator(m_NavigationManipulator);
			UnregisterCallback<PointerDownEvent>(OnPointerDown);
			UnregisterCallback<PointerUpEvent>(OnPointerUp);
			UnregisterCallback<PointerMoveEvent>(OnPointerMove);
			UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
			IVisualElementScheduledItem scrollScheduledItem = m_ScrollScheduledItem;
			if (scrollScheduledItem != null && scrollScheduledItem.isActive)
			{
				m_ScrollScheduledItem.Pause();
				m_ScrollScheduledItem = null;
			}
		}
	}

	private void OnVerticalScrollingChangeEvent(ChangeEvent<double> evt)
	{
		if (!m_IsChangingScrollingParameters)
		{
			m_DelayedScrolledVerticalValue = m_VerticalScroller.value;
			ScheduleScroll();
			evt.StopImmediatePropagation();
		}
	}

	private void OnHorizontalScrollerChangeEvent(ChangeEvent<double> evt)
	{
		if (layoutConfiguration is MultiColumnLayoutConfiguration multiColumnLayoutConfiguration)
		{
			multiColumnLayoutConfiguration.header.ScrollHorizontally((float)evt.newValue);
		}
	}

	internal void UpdateVerticalScrollValue(double value)
	{
		if (!m_VerticalScroller.Approximately(value, m_ScrollValue))
		{
			m_VerticalScroller.value = (m_ScrollValue = value);
			BindVisibleItems();
		}
	}

	private void SetScrollingParameters(double currentScrollOffset, double maxScrollRange)
	{
		bool isChangingScrollingParameters = m_IsChangingScrollingParameters;
		m_IsChangingScrollingParameters = true;
		currentScrollOffset = Math.Min(currentScrollOffset, maxScrollRange);
		try
		{
			using (new EventDispatcherGate(base.panel.dispatcher))
			{
				m_VerticalScroller.highValue = maxScrollRange;
				m_VerticalScroller.value = (m_ScrollValue = currentScrollOffset);
			}
		}
		finally
		{
			m_IsChangingScrollingParameters = isChangingScrollingParameters;
		}
	}

	private void ScheduleScroll()
	{
		if (m_ScrollScheduledItem == null)
		{
			m_ScrollScheduledItem = base.schedule.Execute(OnDelayedScroll);
		}
		else if (!m_ScrollScheduledItem.isActive)
		{
			m_ScrollScheduledItem.Resume();
		}
	}

	private void OnDelayedScroll()
	{
		UpdateVerticalScrollValue(m_DelayedScrolledVerticalValue);
		m_DelayedScrolledVerticalValue = 0.0;
	}

	private void ContainerSizeChanged(float height, float width)
	{
		if (!Mathf.Approximately(m_LastHeight, height))
		{
			m_LastHeight = height;
			RefreshItems();
		}
	}

	private void UnbindItem(RecycledItem item)
	{
		if (item != null)
		{
			int index = item.index;
			item.index = -1;
			m_IndexToItemDictionary.Remove(index);
			layoutConfiguration.unbindCell?.Invoke(item.element, index);
		}
	}

	internal void OnDestroyItem(RecycledItem item)
	{
		layoutConfiguration.destroyCell?.Invoke(item.element);
	}

	private void BindItem(RecycledItem item, int index)
	{
		int index2 = item.index;
		if (m_IndexToItemDictionary.ContainsKey(item.index))
		{
			UnbindItem(item);
		}
		bool enable = showAlternatingRowBackgrounds != AlternatingRowBackground.None && index % 2 == 1;
		item.element.EnableInClassList(BaseVerticalCollectionView.itemAlternativeBackgroundUssClassName, enable);
		item.isLastItem = index == itemsSource.Count - 1;
		item.SetSelected(m_Selection.ContainsIndex(index));
		item.element.style.height = averageItemHeight;
		item.index = index;
		m_IndexToItemDictionary.Add(index, item);
		if (index >= 0 && index < itemsSource.Count)
		{
			layoutConfiguration.bindCell?.Invoke(item.element, index);
		}
		HandleFocus(item, index2);
	}

	public void Rebuild()
	{
		m_RebuildScheduled?.Pause();
		ClearAllItems();
		ResetAverageHeight();
		RefreshItems();
	}

	internal void ScheduleRebuild()
	{
		if (m_RebuildScheduled == null)
		{
			m_RebuildScheduled = base.schedule.Execute(Rebuild);
		}
		else if (!m_RebuildScheduled.isActive)
		{
			m_RebuildScheduled.Resume();
		}
	}

	public void RefreshItems()
	{
		if (itemsSource == null || layoutConfiguration?.makeCell == null || itemsSource.Count == 0)
		{
			m_VerticalScroller.style.display = DisplayStyle.None;
			return;
		}
		IVisualElementScheduledItem rebuildScheduled = m_RebuildScheduled;
		if (rebuildScheduled != null && rebuildScheduled.isActive)
		{
			Rebuild();
			return;
		}
		m_VerticalScroller.style.display = DisplayStyle.Flex;
		float height = m_Container.resolvedStyle.height;
		if (float.IsNaN(height))
		{
			return;
		}
		m_LastHeight = height;
		int num = (int)(m_Container.layout.height / averageItemHeight);
		if (itemsSource.Count - 1 < num)
		{
			m_ScrollValue = 0.0;
		}
		double num2 = (double)averageItemHeight * (double)itemsSource.Count;
		m_VerticalScroller.style.display = ((!(num2 > (double)height)) ? DisplayStyle.None : DisplayStyle.Flex);
		EnableInClassList(verticalScrollerVisibleUssClassName, num2 > (double)height);
		SetScrollingParameters(m_ScrollValue, Math.Abs(num2 - (double)height));
		BindVisibleItems();
		if (m_IndexToItemDictionary.Count > m_DisplayedList.Count)
		{
			for (int num3 = m_IndexToItemDictionary.Count - 1; num3 >= m_DisplayedList.Count; num3--)
			{
				UnbindItem(m_IndexToItemDictionary[num3]);
			}
		}
	}

	private void ClearAllItems()
	{
		while (m_DisplayedList.Count > 0)
		{
			ClearItem(m_DisplayedList.First);
		}
		while (m_FreeList.Count > 0)
		{
			ClearItem(m_FreeList.First);
		}
		RecycledItem.ClearItemPool();
	}

	[EventInterest(new Type[]
	{
		typeof(PointerUpEvent),
		typeof(FocusInEvent),
		typeof(FocusOutEvent),
		typeof(NavigationSubmitEvent)
	})]
	protected override void HandleEventBubbleUp(EventBase evt)
	{
		base.HandleEventBubbleUp(evt);
		if (evt.eventTypeId == EventBase<PointerUpEvent>.TypeId())
		{
			m_Dragger?.OnPointerUpEvent((PointerUpEvent)evt);
		}
		else if (evt.eventTypeId == EventBase<FocusInEvent>.TypeId())
		{
			OnFocusIn(evt.elementTarget);
		}
		else if (evt.eventTypeId == EventBase<FocusOutEvent>.TypeId())
		{
			OnFocusOut(((FocusOutEvent)evt).relatedTarget as VisualElement);
		}
		else if (evt.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId() && evt.target == this)
		{
			m_ScrollView.Focus();
		}
	}

	private void OnFocusIn(VisualElement leafTarget)
	{
		if (leafTarget == m_ScrollView)
		{
			return;
		}
		m_LastFocusedElementTreeChildIndexes.Clear();
		if (m_ScrollView.contentContainer.FindElementInTree(leafTarget, m_LastFocusedElementTreeChildIndexes))
		{
			VisualElement visualElement = m_ScrollView.contentContainer[m_LastFocusedElementTreeChildIndexes[0]];
			foreach (RecycledItem value in m_IndexToItemDictionary.Values)
			{
				if (value.element == visualElement)
				{
					m_LastFocusedElementIndex = value.index;
					break;
				}
			}
			m_LastFocusedElementTreeChildIndexes.RemoveAt(0);
		}
		else
		{
			m_LastFocusedElementIndex = -1;
		}
	}

	private void OnFocusOut(VisualElement willFocus)
	{
		if (willFocus == null || willFocus != m_ScrollView)
		{
			m_LastFocusedElementTreeChildIndexes.Clear();
			m_LastFocusedElementIndex = -1;
		}
	}

	private void HandleFocus(RecycledItem recycledItem, int previousIndex)
	{
		if (m_LastFocusedElementIndex != -1)
		{
			if (m_LastFocusedElementIndex == recycledItem.index)
			{
				recycledItem.element.ElementAtTreePath(m_LastFocusedElementTreeChildIndexes)?.Focus();
			}
			else if (m_LastFocusedElementIndex != previousIndex)
			{
				recycledItem.element.ElementAtTreePath(m_LastFocusedElementTreeChildIndexes)?.Blur();
			}
			else
			{
				m_ScrollView.Focus();
			}
		}
	}

	private void ClearItem(LinkedListNode<RecycledItem> item)
	{
		item.List.Remove(item);
		UnbindItem(item.Value);
		RecycledItem.Recycle(item.Value);
	}

	private void UnbindAllItems()
	{
		foreach (var (_, item) in m_IndexToItemDictionary)
		{
			UnbindItem(item);
		}
	}

	private void UpdateVisibleRange()
	{
		int num = 0;
		int num2 = -1;
		if (m_DisplayedList.Count > 0)
		{
			num = m_DisplayedList.First.Value.index;
			LinkedListNode<RecycledItem> linkedListNode = m_DisplayedList.First;
			if (linkedListNode.Value.renderedHeight < 0f)
			{
				return;
			}
			m_VerticalScroller.scrollSize = 10f * averageItemHeight / linkedListNode.Value.renderedHeight;
			LinkedListNode<RecycledItem> linkedListNode2 = m_DisplayedList.First;
			float num3 = m_LastHeight - m_ScrollView.containerOffset.y;
			while (linkedListNode != null && linkedListNode.Value.verticalOffset + linkedListNode.Value.renderedHeight < num3)
			{
				linkedListNode2 = linkedListNode;
				linkedListNode = linkedListNode.Next;
			}
			if (linkedListNode2 != null)
			{
				num2 = linkedListNode2.Value.index;
			}
		}
		int num4 = num2 - num + 1;
		if (num4 > 0 && itemsSource != null)
		{
			float num5 = Mathf.Ceil(m_LastHeight / averageItemHeight);
			float factor = num5 / (float)itemsSource.Count;
			m_VerticalScroller.Adjust(factor);
		}
	}

	private void ResetAverageHeight()
	{
		m_ComputedAverageHeight = -1f;
	}

	private void BindVisibleItems()
	{
		float lastHeight = m_LastHeight;
		int num = (int)Mathf.Ceil(lastHeight / averageItemHeight) + 3;
		m_FirstVisibleItemIndex = (int)(m_ScrollValue / (double)averageItemHeight);
		LinkedList<RecycledItem> refreshList = m_RefreshList;
		LinkedList<RecycledItem> displayedList = m_DisplayedList;
		m_DisplayedList = refreshList;
		m_RefreshList = displayedList;
		for (int i = 0; i < num; i++)
		{
			int num2 = m_FirstVisibleItemIndex + i;
			if (num2 >= 0 && num2 <= itemsSource.Count - 1 && m_IndexToItemDictionary.TryGetValue(num2, out var value) && value.node.List == m_RefreshList)
			{
				value.node.List.Remove(value.node);
			}
		}
		while (m_RefreshList.Count > 0)
		{
			LinkedListNode<RecycledItem> first = m_RefreshList.First;
			if (first != null)
			{
				m_RefreshList.RemoveFirst();
				first.Value.element.style.display = DisplayStyle.None;
				m_FreeList.AddLast(first);
			}
		}
		AddElementsFromIndex(m_FirstVisibleItemIndex, num);
	}

	private void AddElementsFromIndex(int firstIndex, int itemCount)
	{
		int num = firstIndex + itemCount;
		for (int i = firstIndex; i < num; i++)
		{
			if (i >= 0 && i <= itemsSource.Count - 1)
			{
				if (m_IndexToItemDictionary.TryGetValue(i, out var value))
				{
					value.node.List?.Remove(value.node);
				}
				else if (m_FreeList.Count > 0)
				{
					value = m_FreeList.First.Value;
					m_FreeList.RemoveFirst();
				}
				else
				{
					VisualElement visualElement = layoutConfiguration.makeCell?.Invoke();
					value = RecycledItem.AllocateItem(visualElement, this);
					m_Container.Add(visualElement);
				}
				BindItem(value, i);
				value.element.style.display = DisplayStyle.Flex;
				value.element.style.position = Position.Absolute;
				value.element.style.top = 0f;
				value.element.style.left = 0f;
				value.element.style.right = 0f;
				m_DisplayedList.AddLast(value.node);
			}
		}
		UpdateContainerOffset();
		if (m_DisplayedList.Count > 0)
		{
			RecycledItem.UpdatePositions(m_DisplayedList.First.Value);
		}
	}

	private void UpdateContainerOffset()
	{
		float renderedHeight = averageItemHeight;
		Vector2 containerOffset = m_ScrollView.containerOffset;
		if (m_DisplayedList.Count > 0 && m_DisplayedList.First.Value.renderedHeight > 0f)
		{
			renderedHeight = m_DisplayedList.First.Value.renderedHeight;
		}
		float num = (float)(m_ScrollValue % (double)averageItemHeight);
		num *= renderedHeight / averageItemHeight;
		containerOffset.y = num;
		containerOffset.x = 0f;
		m_ScrollView.containerOffset = containerOffset;
	}

	private void UpdateScrollingRangeAfterLayout()
	{
		float num = 0f;
		if (layoutConfiguration is MultiColumnLayoutConfiguration multiColumnLayoutConfiguration)
		{
			num = multiColumnLayoutConfiguration.header.worldBoundingBox.width;
		}
		else
		{
			float num2 = ((m_VerticalScroller.style.display == DisplayStyle.None) ? 0f : m_VerticalScroller.worldBound.width);
			foreach (RecycledItem displayed in m_DisplayedList)
			{
				num = Mathf.Max(num, displayed.element.worldBoundingBox.width - num2);
			}
		}
		m_HorizontalScroller.SetEnabled(num > m_Container.worldBound.width);
		m_HorizontalScroller.style.display = ((!(num > m_Container.rect.width)) ? DisplayStyle.None : DisplayStyle.Flex);
		m_HorizontalScroller.lowValue = 0.0;
		m_HorizontalScroller.highValue = num - m_Container.rect.width;
		m_HorizontalScroller.scrollSize = 10f * m_Container.rect.width;
		float factor = ((num > 1E-30f) ? (m_Container.worldBound.width / num) : 1f);
		m_HorizontalScroller.Adjust(factor);
		LinkedListNode<RecycledItem> last = m_DisplayedList.Last;
		if (last == null)
		{
			return;
		}
		RecycledItem value = last.Value;
		float y = m_ScrollView.containerOffset.y;
		float num3 = value.verticalOffset + value.renderedHeight;
		float height = m_Container.resolvedStyle.height;
		if (value.isLastItem)
		{
			LinkedListNode<RecycledItem> linkedListNode = last;
			float num4 = height;
			float num5 = num4 - value.renderedHeight;
			while (num5 > 0f && linkedListNode.Previous != null)
			{
				linkedListNode = linkedListNode.Previous;
				num5 -= linkedListNode.Value.renderedHeight;
			}
			if (num5 <= 0f)
			{
				double num6 = (0f - num5) / linkedListNode.Value.renderedHeight;
				int index = linkedListNode.Value.index;
				double num7 = ((double)index + num6) * (double)averageItemHeight;
				SetScrollingParameters(Math.Min(m_ScrollValue, num7), num7);
			}
		}
		else if (num3 + y < height - 1f)
		{
			float num8 = height - (num3 - y);
			int value2 = Mathf.CeilToInt(num8 / averageItemHeight);
			value2 = Math.Clamp(value2, 0, itemsSource.Count - value.index - 1);
			if (value2 > 0)
			{
				AddElementsFromIndex(value.index + 1, value2);
				return;
			}
		}
		UpdateVisibleRange();
	}

	internal void ItemPositionUpdated(RecycledItem item)
	{
		UpdateScrollingRangeAfterLayout();
	}

	public int GetIndexFromPosition(Vector2 position)
	{
		float num = AlignmentUtils.RoundToPixelGrid(averageItemHeight, base.scaledPixelsPerPoint);
		double num2 = m_ScrollValue + (double)position.y;
		return (int)(num2 / (double)num);
	}

	public void ScrollToItem(int index)
	{
		if (index < -1 || index > itemsSource.Count)
		{
			return;
		}
		if (index == -1)
		{
			index = itemsSource.Count - 1;
		}
		if (m_FirstVisibleItemIndex >= index)
		{
			UpdateVerticalScrollValue(averageItemHeight * (float)index);
			return;
		}
		int num = (int)(m_Container.layout.height / averageItemHeight);
		if (index >= m_FirstVisibleItemIndex + num)
		{
			float num2 = averageItemHeight - (m_Container.layout.height - (float)num * averageItemHeight);
			float num3 = averageItemHeight * (float)(index - num) + num2;
			UpdateVerticalScrollValue(num3);
		}
	}

	public VisualElement GetRootElementForIndex(int index)
	{
		if (m_DisplayedList == null || index < 0 || index >= m_DisplayedList.Count)
		{
			return null;
		}
		LinkedListNode<RecycledItem> linkedListNode = m_DisplayedList.First;
		for (int i = 0; i < index; i++)
		{
			linkedListNode = linkedListNode.Next;
		}
		return linkedListNode.Value.element;
	}

	public bool IsSelected(int index)
	{
		return m_Selection.ContainsIndex(index);
	}

	private void NotifyOfSelectionChange()
	{
		this.selectedIndicesChanged?.Invoke();
	}

	private void OnPointerUp(IPointerEvent evt)
	{
		if (!evt.isPrimary || (evt.button != 0 && evt.button != 1))
		{
			return;
		}
		if (evt.pointerType != PointerType.mouse)
		{
			if ((evt.position - m_TouchDownPosition).sqrMagnitude <= 100f)
			{
				DoSelect(evt.localPosition, evt.actionKey, evt.shiftKey);
			}
			return;
		}
		Vector2 vector = default(Vector2);
		if (layoutConfiguration is MultiColumnLayoutConfiguration multiColumnLayoutConfiguration)
		{
			vector = new Vector2(0f, multiColumnLayoutConfiguration.headerContainer.rect.height);
		}
		int indexFromPosition = GetIndexFromPosition((Vector2)evt.localPosition - vector);
		if (selectionType == SelectionType.Multiple && evt.button == 0 && !evt.shiftKey && !evt.actionKey && m_Selection.indexCount > 1 && m_Selection.ContainsIndex(indexFromPosition))
		{
			SetSelection(indexFromPosition);
		}
	}

	private void OnPointerCancel(PointerCancelEvent evt)
	{
		if (evt.isPrimary)
		{
			ClearSelection();
		}
	}

	private void OnPointerMove(PointerMoveEvent evt)
	{
		if (evt.button == 0)
		{
			if ((evt.pressedButtons & 1) == 0)
			{
				OnPointerDown(evt);
			}
			else
			{
				OnPointerUp(evt);
			}
		}
	}

	private void OnPointerDown(IPointerEvent evt)
	{
		processingPointerDownEvent = true;
		try
		{
			if (evt.isPrimary && (evt.button == 0 || evt.button == 1))
			{
				if (evt.pointerType != PointerType.mouse)
				{
					m_TouchDownPosition = evt.position;
				}
				else
				{
					DoSelect(evt.localPosition, evt.actionKey, evt.shiftKey);
				}
			}
		}
		finally
		{
			processingPointerDownEvent = false;
		}
	}

	private void DoSelect(Vector2 localPosition, bool actionKey, bool shiftKey)
	{
		Vector2 vector = default(Vector2);
		if (layoutConfiguration is MultiColumnLayoutConfiguration multiColumnLayoutConfiguration)
		{
			vector = new Vector2(0f, multiColumnLayoutConfiguration.headerContainer.rect.height);
		}
		int indexFromPosition = GetIndexFromPosition(localPosition - vector);
		if (indexFromPosition > itemsSource.Count - 1)
		{
			return;
		}
		m_RangeSelectionDirection = RangeSelectionDirection.None;
		switch (selectionType)
		{
		case SelectionType.None:
			return;
		case SelectionType.Multiple:
			if (actionKey)
			{
				if (m_Selection.ContainsIndex(indexFromPosition))
				{
					RemoveFromSelection(indexFromPosition);
				}
				else
				{
					AddToSelection(indexFromPosition);
				}
				return;
			}
			if (shiftKey)
			{
				if (m_Selection.indexCount == 0)
				{
					SetSelection(indexFromPosition);
				}
				else
				{
					DoRangeSelection(indexFromPosition);
				}
				return;
			}
			if (!m_Selection.ContainsIndex(indexFromPosition))
			{
				break;
			}
			return;
		case SelectionType.Single:
			if (!m_Selection.ContainsIndex(indexFromPosition))
			{
				break;
			}
			return;
		}
		SetSelection(indexFromPosition);
	}

	private void DoRangeSelection(int rangeSelectionFinalIndex)
	{
		if (rangeSelectionFinalIndex < 0 || rangeSelectionFinalIndex >= itemsSource.Count)
		{
			return;
		}
		int num = m_Selection.minIndex;
		int num2 = m_Selection.maxIndex;
		switch (m_RangeSelectionDirection)
		{
		case RangeSelectionDirection.Up:
			num = rangeSelectionFinalIndex;
			break;
		case RangeSelectionDirection.Down:
			num2 = rangeSelectionFinalIndex;
			break;
		default:
			num = Mathf.Min(num, rangeSelectionFinalIndex);
			num2 = Mathf.Max(num2, rangeSelectionFinalIndex);
			break;
		}
		if (num == num2)
		{
			m_RangeSelectionDirection = RangeSelectionDirection.None;
		}
		int num3 = num2 - num + 1;
		if (num3 <= 0)
		{
			return;
		}
		int[] array = ArrayPool<int>.Shared.Rent(num3);
		try
		{
			for (int i = 0; i < num3; i++)
			{
				array[i] = num + i;
			}
			ClearSelectionWithoutValidation();
			AddToSelection(MemoryExtensions.AsSpan(array, 0, num3));
		}
		finally
		{
			ArrayPool<int>.Shared.Return(array);
		}
	}

	private void AddToSelection(ReadOnlySpan<int> indices)
	{
		if (indices.Length != 0)
		{
			ReadOnlySpan<int> readOnlySpan = indices;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				int index = readOnlySpan[i];
				AddToSelectionWithoutValidation(index);
			}
			NotifyOfSelectionChange();
			SaveViewData();
		}
	}

	private void AddToSelectionWithoutValidation(int index)
	{
		if (!m_Selection.ContainsIndex(index))
		{
			if (m_IndexToItemDictionary.TryGetValue(index, out var value))
			{
				value.SetSelected(selected: true);
			}
			m_Selection.AddIndex(index);
		}
	}

	public void AddToSelection(int index)
	{
		Span<int> span = stackalloc int[1] { index };
		AddToSelection(span);
	}

	public void RemoveFromSelection(int index)
	{
		if (m_Selection.TryRemove(index))
		{
			if (m_IndexToItemDictionary.TryGetValue(index, out var value))
			{
				value.SetSelected(selected: false);
			}
			m_Selection.TryRemove(index);
			NotifyOfSelectionChange();
			SaveViewData();
		}
	}

	public void SetSelection(IReadOnlyList<int> indices)
	{
		SetSelectionInternal(indices, sendNotification: true);
	}

	public void SetSelection(ReadOnlySpan<int> indices)
	{
		SetSelectionInternal(indices, sendNotification: true);
	}

	public void SetSelection(int index)
	{
		if (index < 0)
		{
			ClearSelection();
			return;
		}
		Span<int> span = stackalloc int[1] { index };
		SetSelection(span);
	}

	public void SetSelectionWithoutNotify(IReadOnlyList<int> indices)
	{
		SetSelectionInternal(indices, sendNotification: false);
	}

	public void SetSelectionWithoutNotify(ReadOnlySpan<int> indices)
	{
		SetSelectionInternal(indices, sendNotification: false);
	}

	private void SetSelectionInternal(IReadOnlyList<int> indices, bool sendNotification)
	{
		if (indices == null)
		{
			return;
		}
		int count = indices.Count;
		Span<int> span;
		if (count == 0)
		{
			SetSelectionInternal(ReadOnlySpan<int>.Empty, sendNotification);
		}
		else if (count < 16)
		{
			span = stackalloc int[count];
			Span<int> span2 = span;
			int num = 0;
			foreach (int index in indices)
			{
				span2[num++] = index;
			}
			SetSelectionInternal(span2, sendNotification);
		}
		else
		{
			byte[] array = ArrayPool<byte>.Shared.Rent(count * 4);
			try
			{
				Span<int> span3 = MemoryMarshal.Cast<byte, int>(array);
				int length = 0;
				foreach (int index2 in indices)
				{
					span3[length++] = index2;
				}
				span = span3;
				span3 = span.Slice(0, length);
				SetSelectionInternal(span3, sendNotification);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
		SaveViewData();
	}

	private void SetSelectionInternal(ReadOnlySpan<int> indices, bool sendNotification)
	{
		if (MatchesExistingSelection(indices))
		{
			return;
		}
		int num = selectedIndex;
		ClearSelectionWithoutValidation();
		if (m_Selection.capacity < indices.Length)
		{
			m_Selection.capacity = indices.Length;
		}
		ReadOnlySpan<int> readOnlySpan = indices;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			int index = readOnlySpan[i];
			AddToSelectionWithoutValidation(index);
		}
		if (sendNotification)
		{
			if (num != selectedIndex)
			{
				NotifyPropertyChanged(in selectedIndexProperty);
			}
			NotifyOfSelectionChange();
		}
		SaveViewData();
	}

	private bool MatchesExistingSelection(ReadOnlySpan<int> indices)
	{
		if (indices.Length != m_Selection.indexCount)
		{
			return false;
		}
		ReadOnlySpan<int> span = NoAllocHelpers.CreateReadOnlySpan(m_Selection.indices);
		return span.SequenceEqual(indices);
	}

	public void ClearSelection()
	{
		if (m_Selection.indices.Count != 0)
		{
			ClearSelectionWithoutValidation();
			NotifyOfSelectionChange();
		}
	}

	private void ClearSelectionWithoutValidation()
	{
		foreach (var (_, recycledItem2) in m_IndexToItemDictionary)
		{
			recycledItem2.SetSelected(selected: false);
		}
		m_Selection.ClearIndices();
	}

	internal virtual CollectionViewDragger CreateDragger()
	{
		return new CollectionViewDragger(this);
	}

	private void InitializeDragAndDropController(bool enableReordering)
	{
		if (m_Dragger != null)
		{
			m_Dragger.UnregisterCallbacksFromTarget(unregisterPanelEvents: true);
			m_Dragger.dragAndDropController = null;
			m_Dragger = null;
		}
		m_Dragger = CreateDragger();
		m_Dragger.dragAndDropController = CreateDragAndDropController();
		if (m_Dragger.dragAndDropController != null)
		{
			m_Dragger.dragAndDropController.enableReordering = enableReordering;
		}
	}

	internal void SetDragAndDropController(ICollectionDragAndDropController dragAndDropController)
	{
		if (m_Dragger == null)
		{
			m_Dragger = CreateDragger();
		}
		m_Dragger.dragAndDropController = dragAndDropController;
	}

	internal bool HasCanStartDrag()
	{
		return this.canStartDrag != null;
	}

	internal bool RaiseCanStartDrag(RecycledItem item, IEnumerable<int> indices, EventModifiers modifiers)
	{
		return this.canStartDrag?.Invoke(new CanStartDragArgs(item.element, item.index, indices, modifiers)) ?? true;
	}

	internal StartDragArgs RaiseSetupDragAndDrop(RecycledItem item, IEnumerable<int> indices, StartDragArgs args)
	{
		return this.setupDragAndDrop?.Invoke(new SetupDragAndDropArgs(item.element, indices, args)) ?? args;
	}

	internal DragVisualMode RaiseHandleDragAndDrop(Vector2 pointerPosition, DragAndDropArgs dragAndDropArgs)
	{
		return this.dragAndDropUpdate?.Invoke(new HandleDragAndDropArgs(pointerPosition, dragAndDropArgs)) ?? DragVisualMode.None;
	}

	internal DragVisualMode RaiseDrop(Vector2 pointerPosition, DragAndDropArgs dragAndDropArgs)
	{
		return this.handleDrop?.Invoke(new HandleDragAndDropArgs(pointerPosition, dragAndDropArgs)) ?? DragVisualMode.None;
	}

	internal void Move(int index, int newIndex)
	{
		if (itemsSource == null || index == newIndex)
		{
			return;
		}
		int num = Mathf.Min(index, newIndex);
		int num2 = Mathf.Max(index, newIndex);
		if (num < 0 || num2 >= itemsSource.Count)
		{
			return;
		}
		int num3 = ((newIndex < index) ? 1 : (-1));
		while (num < num2)
		{
			Swap(index, newIndex);
			newIndex += num3;
			if (index < newIndex)
			{
				num = index;
				num2 = newIndex;
			}
			else
			{
				num2 = index;
				num = newIndex;
			}
		}
	}

	private void Swap(int lhs, int rhs)
	{
		IList list = itemsSource;
		IList list2 = itemsSource;
		object obj = itemsSource[rhs];
		object obj2 = itemsSource[lhs];
		object obj3 = (list[lhs] = obj);
		obj3 = (list2[rhs] = obj2);
	}

	private void SelectAll()
	{
		if (selectionType != SelectionType.Multiple)
		{
			return;
		}
		for (int i = 0; i < itemsSource.Count; i++)
		{
			m_Selection.AddIndex(i);
		}
		foreach (RecycledItem value in m_IndexToItemDictionary.Values)
		{
			value.SetSelected(selected: true);
		}
		NotifyOfSelectionChange();
		SaveViewData();
	}

	private bool Apply(KeyboardNavigationOperation op, bool shiftKey)
	{
		if (selectionType == SelectionType.None)
		{
			return false;
		}
		switch (op)
		{
		case KeyboardNavigationOperation.SelectAll:
			SelectAll();
			return true;
		case KeyboardNavigationOperation.Cancel:
			ClearSelection();
			return true;
		case KeyboardNavigationOperation.Submit:
			ScrollToItem(selectedIndex);
			return true;
		case KeyboardNavigationOperation.Previous:
		{
			int num2 = ((m_Selection.indexCount == 0) ? (-1) : ((m_RangeSelectionDirection != RangeSelectionDirection.Down) ? m_Selection.minIndex : m_Selection.maxIndex)) - 1;
			if (num2 >= 0)
			{
				if (m_RangeSelectionDirection == RangeSelectionDirection.None)
				{
					m_RangeSelectionDirection = RangeSelectionDirection.Up;
				}
				HandleSelectionAndScroll(num2);
				return true;
			}
			break;
		}
		case KeyboardNavigationOperation.Next:
		{
			int num3 = ((m_Selection.indexCount == 0) ? (-1) : ((m_RangeSelectionDirection != RangeSelectionDirection.Up) ? m_Selection.maxIndex : m_Selection.minIndex)) + 1;
			if (num3 < itemsSource.Count)
			{
				if (m_RangeSelectionDirection == RangeSelectionDirection.None)
				{
					m_RangeSelectionDirection = RangeSelectionDirection.Down;
				}
				HandleSelectionAndScroll(num3);
				return true;
			}
			break;
		}
		case KeyboardNavigationOperation.Begin:
			HandleSelectionAndScroll(0);
			return true;
		case KeyboardNavigationOperation.End:
			HandleSelectionAndScroll(itemsSource.Count - 1);
			return true;
		case KeyboardNavigationOperation.PageDown:
			if (m_Selection.indexCount > 0)
			{
				if (m_RangeSelectionDirection == RangeSelectionDirection.None)
				{
					m_RangeSelectionDirection = RangeSelectionDirection.Down;
				}
				int num4 = ((m_RangeSelectionDirection == RangeSelectionDirection.Up) ? m_Selection.minIndex : m_Selection.maxIndex);
				HandleSelectionAndScroll(Mathf.Min(itemsSource.Count - 1, num4 + (m_DisplayedList.Count - 1)));
			}
			return true;
		case KeyboardNavigationOperation.PageUp:
			if (m_Selection.indexCount > 0)
			{
				if (m_RangeSelectionDirection == RangeSelectionDirection.None)
				{
					m_RangeSelectionDirection = RangeSelectionDirection.Up;
				}
				int num = ((m_RangeSelectionDirection == RangeSelectionDirection.Up) ? m_Selection.minIndex : m_Selection.maxIndex);
				HandleSelectionAndScroll(Mathf.Max(0, num - (m_DisplayedList.Count - 1)));
			}
			return true;
		default:
			throw new ArgumentOutOfRangeException("op", op, null);
		case KeyboardNavigationOperation.MoveRight:
		case KeyboardNavigationOperation.MoveLeft:
			break;
		}
		return false;
		void HandleSelectionAndScroll(int index)
		{
			if (index >= 0 && index < itemsSource.Count)
			{
				if (selectionType == SelectionType.Multiple && shiftKey && m_Selection.indexCount != 0)
				{
					DoRangeSelection(index);
				}
				else
				{
					m_RangeSelectionDirection = RangeSelectionDirection.None;
					selectedIndex = index;
				}
				ScrollToItem(index);
			}
		}
	}

	private void Apply(KeyboardNavigationOperation op, EventBase sourceEvent)
	{
		bool flag = ((sourceEvent is KeyDownEvent { shiftKey: not false } || sourceEvent is INavigationEvent { shiftKey: not false }) ? true : false);
		bool shiftKey = flag;
		if (Apply(op, shiftKey))
		{
			sourceEvent.StopPropagation();
		}
		focusController?.IgnoreEvent(sourceEvent);
	}
}
