using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
internal sealed class HierarchyView : VisualElement, IDisposable
{
	private enum UpdateStage
	{
		UpdatingHierarchy = 0,
		UpdatingHierarchyFlattened = 1,
		UpdatingHierarchyViewModel = 2,
		UpdatingListView = 3,
		ExecutePostUpdateActions = 4,
		Count = 5,
		First = 0,
		Last = 4
	}

	private enum UpdateMode
	{
		Update,
		UpdateIncremental,
		UpdateIncrementalTimed
	}

	internal class TestHelper
	{
		public static int FirstUpdateStage => 0;

		public static int LastUpdateStage => 4;

		public static int HierarchyUpdateStage => 0;

		public static int CurrentUpdateStage(HierarchyView view)
		{
			return (int)view.m_UpdateStage;
		}

		public static bool ViewUpdateNeeded(HierarchyView view)
		{
			return view.UpdateNeeded;
		}

		public static bool HierarchyUpdateNeeded(HierarchyView view)
		{
			return view.m_Hierarchy.UpdateNeeded;
		}
	}

	public delegate void SourceHierarchyChangingEventHandler(Unity.Hierarchy.Hierarchy oldHierarchy, Unity.Hierarchy.Hierarchy newHierarchy, HierarchyNodeFlags defaultFlags);

	public delegate void SourceHierarchyChangedEventHandler(Unity.Hierarchy.Hierarchy hierarchy, HierarchyNodeFlags defaultFlags);

	public delegate void PopulateContextMenuEventHandler(HierarchyViewItem item, DropdownMenu menu);

	public delegate void GetTooltipEventHandler(HierarchyViewItem item, bool filtering, StringBuilder tooltip);

	internal const int k_ItemHeight = 20;

	private const string k_ListViewName = "unity-tree-view__list-view";

	private const string k_HierarchyViewRootStyleName = "hierarchy";

	private const string k_HierarchyViewStyleContainerStyleName = "hierarchy__container";

	private const int k_RenamingDelayMs = 500;

	internal const string k_HierarchyPingBase = "hierarchy - item__ping-base";

	private const string k_HierarchyPingRampIn_Style = "hierarchy-item__ping-ramp-in-style";

	private const string k_HierarchyPingRampIn_Start = "hierarchy-item__ping-ramp-in-start";

	private const string k_HierarchyPingRampOut_Style = "hierarchy-item__ping-ramp-out-style";

	private const string k_HierarchyPingRampOut_Start = "hierarchy-item__ping-ramp-out-start";

	private Unity.Hierarchy.Hierarchy m_Hierarchy;

	private HierarchyFlattened m_HierarchyFlattened;

	private HierarchyViewModel m_HierarchyViewModel;

	private int m_Version;

	private UpdateStage m_UpdateStage = UpdateStage.UpdatingHierarchy;

	private readonly Stopwatch m_UpdateTimer = new Stopwatch();

	private readonly CircularBuffer<Action> m_PostUpdateActionQueue = new CircularBuffer<Action>(16);

	private readonly MultiColumnListView m_MultiColumnListView;

	private readonly HierarchyViewItemColumn m_NameColumn;

	private readonly HierarchyViewDragHandler m_DragHandler;

	private readonly VisualElement m_ListViewContentContainer;

	private VisualElement m_StyleContainer;

	private IVisualElementScheduledItem m_ScheduledItem;

	private readonly List<int> m_SelectedIndices = new List<int>();

	private bool m_SelectedIndicesChangedFromPointerDown;

	private int m_LastMouseUpSelectionIndex;

	private HierarchyViewItem m_RenamingItem;

	internal int m_RenameDelayMs;

	internal bool m_IsRenamingItem => m_RenamingItem != null;

	public Unity.Hierarchy.Hierarchy Source => m_Hierarchy;

	public HierarchyFlattened Flattened => m_HierarchyFlattened;

	public HierarchyViewModel ViewModel => m_HierarchyViewModel;

	internal MultiColumnListView ListView => m_MultiColumnListView;

	public VisualElement StyleContainer => m_StyleContainer;

	public string Filter
	{
		get
		{
			return m_HierarchyViewModel.Query.ToString();
		}
		set
		{
			m_HierarchyViewModel.SetQuery(value);
		}
	}

	public bool Filtering => m_HierarchyViewModel.Filtering;

	public bool Updating
	{
		get
		{
			if (m_Hierarchy == null || !m_Hierarchy.IsCreated)
			{
				return false;
			}
			return m_UpdateStage != UpdateStage.UpdatingHierarchy || m_Hierarchy.Updating || m_HierarchyFlattened.Updating || m_HierarchyViewModel.Updating;
		}
	}

	public bool UpdateNeeded
	{
		get
		{
			if (m_Hierarchy == null || !m_Hierarchy.IsCreated)
			{
				return false;
			}
			return Updating || DataUpdateNeeded || DisplayUpdateNeeded || ExecutePostUpdateActionsNeeded;
		}
	}

	public float UpdateProgress
	{
		get
		{
			if (!Updating)
			{
				return 100f;
			}
			if (m_UpdateStage == UpdateStage.UpdatingHierarchyViewModel)
			{
				return m_HierarchyViewModel.UpdateProgress;
			}
			return 0f;
		}
	}

	internal bool DataUpdateNeeded => m_Hierarchy.UpdateNeeded || m_HierarchyFlattened.UpdateNeeded || m_HierarchyViewModel.UpdateNeeded;

	internal bool DisplayUpdateNeeded => m_Version != m_HierarchyViewModel.Version;

	internal bool ExecutePostUpdateActionsNeeded => m_PostUpdateActionQueue.Count > 0;

	internal HierarchyViewDragHandler DragHandler => m_DragHandler;

	internal HierarchyViewItemColumn NameColumn => m_NameColumn;

	public event SourceHierarchyChangingEventHandler SourceHierarchyChanging;

	public event SourceHierarchyChangedEventHandler SourceHierarchyChanged;

	public event Action<HierarchyViewItem> BindViewItem;

	public event Action<HierarchyViewItem> UnbindViewItem;

	public event HierarchyViewModel.FlagsChangedEventHandler FlagsChanged;

	public event PopulateContextMenuEventHandler PopulateContextMenu;

	public event GetTooltipEventHandler GetTooltip;

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal event Action Initializing;

	public HierarchyView()
	{
		AddToClassList("hierarchy");
		this.AddManipulator(new ContextualMenuManipulator(InvokePopulateContextMenu));
		MultiColumnListView multiColumnListView = new MultiColumnListView();
		multiColumnListView.name = "unity-tree-view__list-view";
		multiColumnListView.fixedItemHeight = 20f;
		multiColumnListView.selectionType = SelectionType.Multiple;
		multiColumnListView.reorderMode = ListViewReorderMode.Simple;
		multiColumnListView.reorderable = true;
		multiColumnListView.itemsSource = null;
		multiColumnListView.columns.stretchMode = Columns.StretchMode.Grow;
		m_MultiColumnListView = multiColumnListView;
		m_NameColumn = new HierarchyViewItemColumn(this);
		m_DragHandler = new HierarchyViewDragHandler(this);
		m_MultiColumnListView.selectedIndicesChanged += OnSelectedIndicesChanged;
		m_MultiColumnListView.AddToClassList("unity-tree-view__list-view");
		m_MultiColumnListView.RegisterCallback<PointerUpEvent>(OnPointerUp);
		m_MultiColumnListView.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
		m_MultiColumnListView.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
		m_MultiColumnListView.Q(null, ScrollView.contentAndVerticalScrollUssClassName).RegisterCallback<ClickEvent>(OnListViewClick);
		m_MultiColumnListView.columns.Add(m_NameColumn);
		m_NameColumn.stretchable = true;
		m_NameColumn.OnBindItem += OnBindItem;
		m_NameColumn.OnUnbindItem += OnUnbindItem;
		ScrollView scrollView = m_MultiColumnListView.Q<ScrollView>();
		m_ListViewContentContainer = scrollView.contentContainer;
		scrollView.mode = ScrollViewMode.VerticalAndHorizontal;
		m_ListViewContentContainer.RegisterCallback<ClickEvent>(OnClickEvent);
		m_ListViewContentContainer.RegisterCallback<NavigationCancelEvent>(OnNavigationCancel);
		m_StyleContainer = new VisualElement();
		m_StyleContainer.AddToClassList("hierarchy__container");
		m_StyleContainer.Add(m_MultiColumnListView);
		Add(m_StyleContainer);
		m_LastMouseUpSelectionIndex = -1;
		SetRenamingItem(null);
		m_RenameDelayMs = 500;
	}

	public void Dispose()
	{
		SetSourceHierarchy(null);
		this.BindViewItem = null;
		this.UnbindViewItem = null;
		this.PopulateContextMenu = null;
		this.GetTooltip = null;
	}

	public void SetSourceHierarchy(Unity.Hierarchy.Hierarchy hierarchy, HierarchyNodeFlags defaultFlags = HierarchyNodeFlags.None)
	{
		if (m_Hierarchy == hierarchy)
		{
			return;
		}
		if (m_Hierarchy != null)
		{
			m_Hierarchy.HandlerCreated -= OnHandlerCreated;
		}
		if (m_HierarchyViewModel != null)
		{
			m_HierarchyViewModel.FlagsChanged -= this.FlagsChanged;
		}
		this.SourceHierarchyChanging?.Invoke(m_Hierarchy, hierarchy, defaultFlags);
		ClearColumns();
		Reset();
		SetRenamingItem(null);
		m_LastMouseUpSelectionIndex = -1;
		m_SelectedIndicesChangedFromPointerDown = false;
		m_SelectedIndices.Clear();
		m_ScheduledItem = null;
		m_MultiColumnListView.itemsSource = null;
		m_PostUpdateActionQueue.Clear();
		m_UpdateStage = UpdateStage.UpdatingHierarchy;
		m_Version = 0;
		if (m_HierarchyViewModel != null)
		{
			if (m_HierarchyViewModel.IsCreated)
			{
				m_HierarchyViewModel.Dispose();
			}
			m_HierarchyViewModel = null;
		}
		if (m_HierarchyFlattened != null)
		{
			if (m_HierarchyFlattened.IsCreated)
			{
				m_HierarchyFlattened.Dispose();
			}
			m_HierarchyFlattened = null;
		}
		m_Hierarchy = null;
		if (hierarchy != null)
		{
			m_Hierarchy = hierarchy;
			m_HierarchyFlattened = new HierarchyFlattened(m_Hierarchy);
			m_HierarchyViewModel = new HierarchyViewModel(m_HierarchyFlattened, defaultFlags);
			m_Hierarchy.Update();
			m_HierarchyFlattened.Update();
			m_HierarchyViewModel.Update();
			m_MultiColumnListView.itemsSource = m_HierarchyViewModel.AsReadOnlyList();
			BindColumns();
			Initialize();
			this.SourceHierarchyChanged?.Invoke(hierarchy, defaultFlags);
			m_Hierarchy.HandlerCreated += OnHandlerCreated;
			m_HierarchyViewModel.FlagsChanged += this.FlagsChanged;
		}
	}

	public void Update()
	{
		while (DoUpdate(UpdateMode.Update))
		{
		}
	}

	public bool UpdateIncremental()
	{
		return DoUpdate(UpdateMode.UpdateIncremental);
	}

	public bool UpdateIncrementalTimed(double milliseconds)
	{
		do
		{
			m_UpdateTimer.Restart();
			if (!DoUpdate(UpdateMode.UpdateIncrementalTimed, milliseconds))
			{
				return false;
			}
			milliseconds -= m_UpdateTimer.ElapsedMillisecondsPrecise();
		}
		while (!(milliseconds <= 0.0));
		return true;
	}

	public void Select(in HierarchyNode node)
	{
		m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Selected);
		Update();
	}

	public void Select(ReadOnlySpan<HierarchyNode> nodes)
	{
		m_HierarchyViewModel.SetFlags(nodes, HierarchyNodeFlags.Selected);
		Update();
	}

	public void SelectRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.SetFlagsRecursive(in node, HierarchyNodeFlags.Selected, direction);
		Update();
	}

	public void SelectRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.SetFlagsRecursive(nodes, HierarchyNodeFlags.Selected, direction);
		Update();
	}

	public void SelectAll(bool exposedOnly)
	{
		if (exposedOnly)
		{
			m_HierarchyViewModel.SetFlags(m_HierarchyViewModel.AsReadOnlySpan(), HierarchyNodeFlags.Selected);
		}
		else
		{
			m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Selected);
		}
		Update();
	}

	public void SetSelection(in HierarchyNode node)
	{
		using (new HierarchyViewModelFlagsChangeScope(m_HierarchyViewModel))
		{
			m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
			m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Selected);
		}
		Update();
	}

	public void SetSelection(ReadOnlySpan<HierarchyNode> nodes)
	{
		using (new HierarchyViewModelFlagsChangeScope(m_HierarchyViewModel))
		{
			m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
			m_HierarchyViewModel.SetFlags(nodes, HierarchyNodeFlags.Selected);
		}
		Update();
	}

	public bool IsSelected(in HierarchyNode node)
	{
		return m_HierarchyViewModel.HasAllFlags(in node, HierarchyNodeFlags.Selected);
	}

	public bool IsSelectedOrAnyAncestorSelected(in HierarchyNode node)
	{
		HierarchyNode lhs = node;
		while (true)
		{
			if (lhs == m_Hierarchy.Root)
			{
				return false;
			}
			if (IsSelected(in lhs))
			{
				break;
			}
			lhs = m_HierarchyViewModel.GetParent(in lhs);
		}
		return true;
	}

	public void ToggleSelected(in HierarchyNode node)
	{
		m_HierarchyViewModel.ToggleFlags(in node, HierarchyNodeFlags.Selected);
		Update();
	}

	public void ToggleSelected(ReadOnlySpan<HierarchyNode> nodes)
	{
		m_HierarchyViewModel.ToggleFlags(nodes, HierarchyNodeFlags.Selected);
		Update();
	}

	public void ToggleSelectedRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.ToggleFlagsRecursive(in node, HierarchyNodeFlags.Selected, direction);
		Update();
	}

	public void ToggleSelectedRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.ToggleFlagsRecursive(nodes, HierarchyNodeFlags.Selected, direction);
		Update();
	}

	public void ToggleSelection()
	{
		m_HierarchyViewModel.ToggleFlags(HierarchyNodeFlags.Selected);
		Update();
	}

	public void Deselect(in HierarchyNode node)
	{
		m_HierarchyViewModel.ClearFlags(in node, HierarchyNodeFlags.Selected);
		Update();
	}

	public void Deselect(ReadOnlySpan<HierarchyNode> nodes)
	{
		m_HierarchyViewModel.ClearFlags(nodes, HierarchyNodeFlags.Selected);
		Update();
	}

	public void DeselectRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.ClearFlagsRecursive(in node, HierarchyNodeFlags.Selected, direction);
		Update();
	}

	public void DeselectRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.ClearFlagsRecursive(nodes, HierarchyNodeFlags.Selected, direction);
		Update();
	}

	public void DeselectAll()
	{
		m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
		Update();
	}

	public void Expand(in HierarchyNode node)
	{
		m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Expanded);
		Update();
	}

	public void Expand(ReadOnlySpan<HierarchyNode> nodes)
	{
		m_HierarchyViewModel.SetFlags(nodes, HierarchyNodeFlags.Expanded);
		Update();
	}

	public void ExpandRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.SetFlagsRecursive(in node, HierarchyNodeFlags.Expanded, direction);
		Update();
	}

	public void ExpandRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.SetFlagsRecursive(nodes, HierarchyNodeFlags.Expanded, direction);
		Update();
	}

	public void ExpandAll()
	{
		m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Expanded);
		Update();
	}

	public bool IsExpanded(in HierarchyNode node)
	{
		return m_HierarchyViewModel.HasAllFlags(in node, HierarchyNodeFlags.Expanded);
	}

	public void Collapse(in HierarchyNode node)
	{
		m_HierarchyViewModel.ClearFlags(in node, HierarchyNodeFlags.Expanded);
		Update();
	}

	public void Collapse(ReadOnlySpan<HierarchyNode> nodes)
	{
		m_HierarchyViewModel.ClearFlags(nodes, HierarchyNodeFlags.Expanded);
		Update();
	}

	public void CollapseRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.ClearFlagsRecursive(in node, HierarchyNodeFlags.Expanded, direction);
		Update();
	}

	public void CollapseRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.ClearFlagsRecursive(nodes, HierarchyNodeFlags.Expanded, direction);
		Update();
	}

	public void CollapseAll()
	{
		m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Expanded);
		Update();
	}

	public bool IsCollapsed(in HierarchyNode node)
	{
		return m_HierarchyViewModel.DoesNotHaveAllFlags(in node, HierarchyNodeFlags.Expanded);
	}

	public void Show(in HierarchyNode node)
	{
		m_HierarchyViewModel.ClearFlags(in node, HierarchyNodeFlags.Hidden);
		Update();
	}

	public void Show(ReadOnlySpan<HierarchyNode> nodes)
	{
		m_HierarchyViewModel.ClearFlags(nodes, HierarchyNodeFlags.Hidden);
		Update();
	}

	public void ShowRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.ClearFlagsRecursive(in node, HierarchyNodeFlags.Hidden, direction);
		Update();
	}

	public void ShowRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.ClearFlagsRecursive(nodes, HierarchyNodeFlags.Hidden, direction);
		Update();
	}

	public void ShowAll()
	{
		m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Hidden);
		Update();
	}

	public bool IsShown(in HierarchyNode node)
	{
		return m_HierarchyViewModel.DoesNotHaveAllFlags(in node, HierarchyNodeFlags.Hidden);
	}

	public void Hide(in HierarchyNode node)
	{
		m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Hidden);
		Update();
	}

	public void Hide(ReadOnlySpan<HierarchyNode> nodes)
	{
		m_HierarchyViewModel.SetFlags(nodes, HierarchyNodeFlags.Hidden);
		Update();
	}

	public void HideRecursive(in HierarchyNode node, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.SetFlagsRecursive(in node, HierarchyNodeFlags.Hidden, direction);
		Update();
	}

	public void HideRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyTraversalDirection direction = HierarchyTraversalDirection.Children)
	{
		m_HierarchyViewModel.SetFlagsRecursive(nodes, HierarchyNodeFlags.Hidden, direction);
		Update();
	}

	public void HideAll()
	{
		m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Hidden);
		Update();
	}

	public bool IsHidden(in HierarchyNode node)
	{
		return m_HierarchyViewModel.HasAllFlags(in node, HierarchyNodeFlags.Hidden);
	}

	public void Frame(in HierarchyNode node)
	{
		if (!(node == HierarchyNode.Null) && !(node == m_Hierarchy.Root))
		{
			ExpandParents(in node);
			m_HierarchyViewModel.Update();
			UpdateListView();
			ScrollToNode(in node);
		}
	}

	public void Frame(ReadOnlySpan<HierarchyNode> nodes)
	{
		if (nodes.Length != 0)
		{
			ExpandParents(nodes);
			m_HierarchyViewModel.Update();
			UpdateListView();
			ScrollToNode(in nodes[0]);
		}
	}

	public void SetColumns(List<Column> columns, HierarchyViewState state = null)
	{
		if (state != null && state.Columns != null && state.Columns.Length != 0)
		{
			HierarchyViewColumnState[] columns2 = state.Columns;
			foreach (HierarchyViewColumnState hierarchyViewColumnState in columns2)
			{
				Column columnWithId = HierarchyViewColumnUtility.GetColumnWithId(columns, hierarchyViewColumnState.ColumnId);
				if (columnWithId != null)
				{
					columnWithId.visible = hierarchyViewColumnState.Visible;
					HierarchyViewColumn.SetWidth(columnWithId, hierarchyViewColumnState.Width);
				}
			}
			columns.Sort(delegate(Column c1, Column c2)
			{
				int visibleIndex = HierarchyViewColumnUtility.GetVisibleIndex(state, c1);
				int visibleIndex2 = HierarchyViewColumnUtility.GetVisibleIndex(state, c2);
				return visibleIndex - visibleIndex2;
			});
		}
		else
		{
			foreach (Column column in columns)
			{
				if (column is HierarchyViewColumn hierarchyViewColumn)
				{
					hierarchyViewColumn.ApplyDefaultColumnProperties();
				}
				else if (column is HierarchyViewItemColumn hierarchyViewItemColumn)
				{
					hierarchyViewItemColumn.ApplyDefaultColumnProperties();
				}
			}
		}
		m_MultiColumnListView.columns.Clear();
		foreach (Column column2 in columns)
		{
			m_MultiColumnListView.columns.Add(column2);
		}
		BindColumns();
	}

	public void SetColumnDescriptors(IEnumerable<HierarchyViewColumnDescriptor> columnDescriptors, IEnumerable<HierarchyViewCellDescriptor> cellDescriptors, HierarchyViewState state = null)
	{
		List<Column> list = new List<Column> { NameColumn };
		foreach (HierarchyViewColumnDescriptor columnDescriptor in columnDescriptors)
		{
			HierarchyViewColumn hierarchyViewColumn = new HierarchyViewColumn(this, columnDescriptor);
			foreach (HierarchyViewCellDescriptor cellDescriptor in cellDescriptors)
			{
				if (cellDescriptor.ValidForColumn(columnDescriptor))
				{
					hierarchyViewColumn.AddCell(cellDescriptor);
				}
			}
			list.Add(hierarchyViewColumn);
		}
		list.Sort(delegate(Column c1, Column c2)
		{
			int columnDefaultPriority = HierarchyViewColumnUtility.GetColumnDefaultPriority(c1);
			int columnDefaultPriority2 = HierarchyViewColumnUtility.GetColumnDefaultPriority(c2);
			return columnDefaultPriority - columnDefaultPriority2;
		});
		SetColumns(list, state);
	}

	public void SetState(HierarchyViewState viewState)
	{
		if ((viewState.ValidContent & (HierarchyViewState.Content.ViewModelState | HierarchyViewState.Content.SearchText | HierarchyViewState.Content.Columns)) != HierarchyViewState.Content.Invalid)
		{
			EnqueuePostUpdateAction(delegate
			{
				if (viewState.ValidContent.HasFlag(HierarchyViewState.Content.Columns))
				{
					SetColumnState(viewState);
				}
				if (viewState.ValidContent.HasFlag(HierarchyViewState.Content.SearchText))
				{
					Filter = viewState.SearchText;
				}
				if (viewState.ValidContent.HasFlag(HierarchyViewState.Content.ViewModelState))
				{
					m_HierarchyViewModel.SetState(viewState.ViewModelState);
				}
			});
		}
		if (viewState.ValidContent.HasFlag(HierarchyViewState.Content.ScrollPosition))
		{
			m_MultiColumnListView.scrollView.scrollOffset = new Vector2(viewState.ScrollPositionX, viewState.ScrollPositionY);
		}
	}

	public HierarchyViewState GetState(HierarchyViewState.Content content = HierarchyViewState.Content.All)
	{
		HierarchyViewState hierarchyViewState = new HierarchyViewState(content);
		if (hierarchyViewState.ValidContent.HasFlag(HierarchyViewState.Content.ViewModelState))
		{
			hierarchyViewState.ViewModelState = m_HierarchyViewModel.GetState();
		}
		if (hierarchyViewState.ValidContent.HasFlag(HierarchyViewState.Content.SearchText))
		{
			hierarchyViewState.SearchText = Filter;
		}
		if (hierarchyViewState.ValidContent.HasFlag(HierarchyViewState.Content.ScrollPosition))
		{
			Vector2 vector = m_MultiColumnListView.Q<ScrollView>()?.scrollOffset ?? new Vector2(-1f, -1f);
			hierarchyViewState.ScrollPositionX = vector.x;
			hierarchyViewState.ScrollPositionY = vector.y;
		}
		if (hierarchyViewState.ValidContent.HasFlag(HierarchyViewState.Content.Columns))
		{
			hierarchyViewState.Columns = new HierarchyViewColumnState[m_MultiColumnListView.columns.Count];
			List<VisualElement> list = m_MultiColumnListView.Query<VisualElement>(null, "unity-multi-column-header__column").ToList();
			int num = 0;
			foreach (Column column in m_MultiColumnListView.columns)
			{
				string columnId = HierarchyViewColumnUtility.GetColumnId(column);
				int num2 = list.FindIndex((VisualElement header) => header.name == columnId);
				hierarchyViewState.Columns[num] = new HierarchyViewColumnState
				{
					ColumnId = columnId,
					Width = column.width.value,
					Visible = column.visible,
					Index = ((num2 != -1) ? num2 : num)
				};
				num++;
			}
			Array.Sort(hierarchyViewState.Columns, (HierarchyViewColumnState c1, HierarchyViewColumnState c2) => c1.Index - c2.Index);
		}
		return hierarchyViewState;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal void Initialize()
	{
		BindHandlers();
		try
		{
			this.Initializing?.Invoke();
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	internal void Reset()
	{
		UnbindHandlers();
		m_StyleContainer.Remove(m_MultiColumnListView);
		m_StyleContainer.RemoveFromHierarchy();
		m_StyleContainer = new VisualElement();
		m_StyleContainer.AddToClassList("hierarchy__container");
		m_StyleContainer.Add(m_MultiColumnListView);
		Add(m_StyleContainer);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal void EnqueuePostUpdateAction(Action action)
	{
		if (m_PostUpdateActionQueue.Locked)
		{
			throw new InvalidOperationException("Cannot enqueue post update action while processing post update actions.");
		}
		m_PostUpdateActionQueue.PushBack(in action);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal void BeginRename(in HierarchyNode node)
	{
		int num = m_HierarchyViewModel.IndexOf(in node);
		if (num >= 0)
		{
			GetHierarchyViewItemFromIndex(num)?.BeginRename();
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal void OnLostFocus()
	{
		m_LastMouseUpSelectionIndex = -1;
	}

	internal int GetIndexFromLocalPosition(Vector2 pos)
	{
		return m_MultiColumnListView.virtualizationController.GetIndexFromPosition(pos);
	}

	internal int GetIndexFromWorldPosition(Vector2 worldPos, float offset = 0f)
	{
		Vector2 pos = VisualElementExtensions.WorldToLocal(p: new Vector3(worldPos.x, worldPos.y - offset, 0f), ele: m_ListViewContentContainer);
		return GetIndexFromLocalPosition(pos);
	}

	internal void InvokeBindViewItem(HierarchyViewItem item)
	{
		item.Handler?.Internal_BindItem(item);
		this.BindViewItem?.Invoke(item);
	}

	internal void InvokeUnbindViewItem(HierarchyViewItem item)
	{
		item.Handler?.Internal_UnbindItem(item);
		this.UnbindViewItem?.Invoke(item);
	}

	internal void InvokePopulateContextMenu(ContextualMenuPopulateEvent evt)
	{
		if (!(evt.target is HierarchyView src))
		{
			return;
		}
		if (m_IsRenamingItem)
		{
			m_RenamingItem.Q<HierarchyViewItemName>()?.CancelRename();
			SetRenamingItem(null);
		}
		evt.StopImmediatePropagation();
		Vector2 pos = src.ChangeCoordinatesTo(m_ListViewContentContainer, evt.localMousePosition);
		int indexFromLocalPosition = GetIndexFromLocalPosition(pos);
		HierarchyViewItem hierarchyViewItemFromIndex = GetHierarchyViewItemFromIndex(indexFromLocalPosition);
		if (hierarchyViewItemFromIndex == null)
		{
			m_MultiColumnListView.ClearSelection();
			foreach (HierarchyNodeTypeHandler item in m_Hierarchy.EnumerateNodeTypeHandlers())
			{
				if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler)
				{
					hierarchyEditorNodeTypeHandler.PopulateContextMenu(this, null, evt.menu);
				}
			}
		}
		else if (hierarchyViewItemFromIndex.Handler is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler2)
		{
			hierarchyEditorNodeTypeHandler2.PopulateContextMenu(this, hierarchyViewItemFromIndex, evt.menu);
		}
		this.PopulateContextMenu?.Invoke(hierarchyViewItemFromIndex, evt.menu);
	}

	internal void InvokeGetTooltip(HierarchyViewItem item, bool filtering, StringBuilder tooltip)
	{
		if (item.Handler is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler)
		{
			hierarchyEditorNodeTypeHandler.GetTooltip(item, filtering, tooltip);
		}
		this.GetTooltip?.Invoke(item, filtering, tooltip);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal void PingNode(HierarchyNode node)
	{
		if (node == HierarchyNode.Null || node == m_Hierarchy.Root || !m_Hierarchy.Exists(in node))
		{
			return;
		}
		ExpandParents(in node);
		Update();
		int num = m_HierarchyViewModel.IndexOf(in node);
		if (num < 0)
		{
			return;
		}
		m_MultiColumnListView.ScrollToItem(num);
		EnqueuePostUpdateAction(delegate
		{
			base.schedule.Execute((Action)delegate
			{
				DoPingAnimation(node);
			});
		});
	}

	private void DoPingAnimation(HierarchyNode node)
	{
		int num = m_HierarchyViewModel.IndexOf(in node);
		if (num < 0)
		{
			return;
		}
		HierarchyViewItem hierarchyViewItemFromIndex = GetHierarchyViewItemFromIndex(num);
		if (hierarchyViewItemFromIndex == null)
		{
			return;
		}
		VisualElement rowContainer = hierarchyViewItemFromIndex.RowContainer;
		if (rowContainer == null || rowContainer.ClassListContains("hierarchy - item__ping-base"))
		{
			return;
		}
		rowContainer.AddToClassList("hierarchy - item__ping-base");
		rowContainer.schedule.Execute((Action)delegate
		{
			rowContainer.AddToClassList("hierarchy-item__ping-ramp-in-style");
			rowContainer.AddToClassList("hierarchy-item__ping-ramp-in-start");
			rowContainer.RegisterCallbackOnce<TransitionEndEvent>(delegate
			{
				rowContainer.RemoveFromClassList("hierarchy-item__ping-ramp-in-start");
				rowContainer.RemoveFromClassList("hierarchy-item__ping-ramp-in-style");
				rowContainer.AddToClassList("hierarchy-item__ping-ramp-out-start");
				rowContainer.AddToClassList("hierarchy-item__ping-ramp-out-style");
				rowContainer.RegisterCallbackOnce<TransitionEndEvent>(delegate
				{
					rowContainer.RemoveFromClassList("hierarchy - item__ping-base");
					rowContainer.RemoveFromClassList("hierarchy-item__ping-ramp-out-start");
					rowContainer.RemoveFromClassList("hierarchy-item__ping-ramp-out-style");
				});
			});
		});
	}

	internal void ScrollToNode(in HierarchyNode node)
	{
		if (!(node == HierarchyNode.Null) && !(node == m_Hierarchy.Root))
		{
			int num = m_HierarchyViewModel.IndexOf(in node);
			if (num >= 0)
			{
				m_MultiColumnListView.ScrollToItem(num);
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal void ExpandParents(in HierarchyNode node)
	{
		if (!(node == HierarchyNode.Null) && !(node == m_Hierarchy.Root))
		{
			HierarchyNode lhs = m_Hierarchy.GetParent(in node);
			if (!(lhs == HierarchyNode.Null) && !(lhs == m_Hierarchy.Root))
			{
				m_HierarchyViewModel.SetFlagsRecursive(in lhs, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Parents);
			}
		}
	}

	internal void ExpandParents(ReadOnlySpan<HierarchyNode> nodes)
	{
		RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(nodes.Length, clear: true);
		try
		{
			int i = 0;
			for (int length = nodes.Length; i < length; i++)
			{
				ref readonly HierarchyNode reference = ref nodes[i];
				if (!(reference == HierarchyNode.Null) && !(reference == m_Hierarchy.Root))
				{
					rentSpanUnmanaged.Span[i] = m_Hierarchy.GetParent(in reference);
				}
			}
			m_HierarchyViewModel.SetFlagsRecursive(rentSpanUnmanaged.Span, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Parents);
		}
		finally
		{
			rentSpanUnmanaged.Dispose();
		}
	}

	internal void SelectChildrenAndExpandRecursive()
	{
		int num = m_HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected);
		if (num == 0)
		{
			return;
		}
		RentSpanUnmanaged<HierarchyNode> rentSpan = new RentSpanUnmanaged<HierarchyNode>(num);
		try
		{
			m_HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, rentSpan);
			m_HierarchyViewModel.SetFlagsRecursive(rentSpan, HierarchyNodeFlags.Expanded | HierarchyNodeFlags.Selected, HierarchyTraversalDirection.Children);
			Update();
		}
		finally
		{
			rentSpan.Dispose();
		}
	}

	internal void SetRenamingItem(HierarchyViewItem item)
	{
		m_RenamingItem = item;
	}

	private void BindHandlers()
	{
		if (m_Hierarchy == null || !m_Hierarchy.IsCreated)
		{
			return;
		}
		foreach (HierarchyNodeTypeHandler item in m_Hierarchy.EnumerateNodeTypeHandlers())
		{
			item.Internal_BindView(this);
		}
	}

	private void UnbindHandlers()
	{
		if (m_Hierarchy == null || !m_Hierarchy.IsCreated)
		{
			return;
		}
		foreach (HierarchyNodeTypeHandler item in m_Hierarchy.EnumerateNodeTypeHandlers())
		{
			item.Internal_UnbindView(this);
		}
	}

	private void OnClickEvent(ClickEvent evt)
	{
		m_ScheduledItem?.Pause();
		if (evt.button != 0)
		{
			return;
		}
		int indexFromLocalPosition = GetIndexFromLocalPosition(evt.localPosition);
		HierarchyViewItem item = GetHierarchyViewItemFromIndex(indexFromLocalPosition);
		if (item == null)
		{
			return;
		}
		Vector3 position = evt.position;
		HierarchyViewItemName hierarchyViewItemName = item.Q<HierarchyViewItemName>();
		if (indexFromLocalPosition == m_LastMouseUpSelectionIndex && evt.clickCount == 1 && hierarchyViewItemName != null && hierarchyViewItemName.worldBound.Contains(position))
		{
			if (m_RenameDelayMs == 0)
			{
				item.BeginRename();
			}
			else
			{
				m_ScheduledItem = base.schedule.Execute((Action)delegate
				{
					item.BeginRename();
					m_ScheduledItem = null;
				}).StartingIn(m_RenameDelayMs);
			}
		}
		else if (evt.clickCount == 2)
		{
			ref readonly HierarchyNode node = ref m_HierarchyViewModel[indexFromLocalPosition];
			HierarchyNodeTypeHandler nodeTypeHandler = m_Hierarchy.GetNodeTypeHandler(in node);
			if (nodeTypeHandler is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler)
			{
				hierarchyEditorNodeTypeHandler.OnDoubleClick(this, in node);
			}
		}
		m_LastMouseUpSelectionIndex = indexFromLocalPosition;
	}

	private void OnPointerUp(PointerUpEvent evt)
	{
		if (m_SelectedIndicesChangedFromPointerDown)
		{
			this.FlagsChanged?.Invoke(HierarchyNodeFlags.Selected);
			m_SelectedIndicesChangedFromPointerDown = false;
		}
	}

	private void OnKeyDown(KeyDownEvent evt)
	{
		if (!m_IsRenamingItem)
		{
			bool flag = true;
			switch (evt.keyCode)
			{
			case KeyCode.Home:
			case KeyCode.PageUp:
				m_MultiColumnListView.SetSelection(0);
				break;
			case KeyCode.End:
			case KeyCode.PageDown:
				m_MultiColumnListView.SetSelection(m_MultiColumnListView.itemsSource.Count - 1);
				break;
			case KeyCode.Escape:
				m_SelectedIndices.Clear();
				m_SelectedIndicesChangedFromPointerDown = false;
				break;
			default:
				flag = false;
				break;
			}
			m_ListViewContentContainer.Focus();
			if (flag)
			{
				evt.StopPropagation();
			}
		}
	}

	private void OnNavigationMove(NavigationMoveEvent evt)
	{
		if (m_IsRenamingItem)
		{
			return;
		}
		bool flag = true;
		int selectedIndex = m_MultiColumnListView.selectedIndex;
		if (selectedIndex == -1)
		{
			NavigationMoveEvent.Direction direction = evt.direction;
			NavigationMoveEvent.Direction direction2 = direction;
			if (direction2 == NavigationMoveEvent.Direction.Up || direction2 == NavigationMoveEvent.Direction.Down)
			{
				m_MultiColumnListView.SetSelection(0);
			}
			else
			{
				flag = false;
			}
			m_ListViewContentContainer.Focus();
		}
		else
		{
			NavigationMoveEvent.Direction direction3 = evt.direction;
			NavigationMoveEvent.Direction direction4 = direction3;
			if (direction4 == NavigationMoveEvent.Direction.Left || direction4 == NavigationMoveEvent.Direction.Right)
			{
				int length = m_HierarchyViewModel.HasAnyFlagsCount(HierarchyNodeFlags.Selected);
				RentSpanUnmanaged<HierarchyNode> rentSpan = new RentSpanUnmanaged<HierarchyNode>(length);
				try
				{
					m_HierarchyViewModel.GetNodesWithAnyFlags(HierarchyNodeFlags.Selected, rentSpan);
					SetExpandedState(rentSpan, evt.direction == NavigationMoveEvent.Direction.Right, evt.altKey);
				}
				finally
				{
					rentSpan.Dispose();
				}
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			evt.StopPropagation();
		}
	}

	private void OnNavigationCancel(NavigationCancelEvent evt)
	{
		m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Cut);
		Update();
		evt.StopImmediatePropagation();
	}

	private void OnListViewClick(ClickEvent evt)
	{
		VisualElement visualElement = evt.target as VisualElement;
		if (visualElement == m_MultiColumnListView.Q(null, ScrollView.contentAndVerticalScrollUssClassName))
		{
			m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
			Update();
			m_LastMouseUpSelectionIndex = -1;
			evt.StopImmediatePropagation();
		}
	}

	private void OnUnbindItem(HierarchyViewItem element)
	{
		element.ExpandedStateChanged -= SetExpandedState;
	}

	private void OnHandlerCreated(HierarchyNodeTypeHandlerBase handler)
	{
		Reset();
		Initialize();
	}

	private HierarchyViewItem GetHierarchyViewItemFromIndex(int index)
	{
		if (index == -1)
		{
			return null;
		}
		return m_MultiColumnListView.GetRootElementForIndex(index)?.Q<HierarchyViewItem>();
	}

	private void OnSelectedIndicesChanged(IEnumerable<int> indices)
	{
		bool flag = false;
		m_SelectedIndices.Clear();
		foreach (int index in indices)
		{
			if (index >= 0)
			{
				if (index == m_LastMouseUpSelectionIndex)
				{
					flag = true;
				}
				m_SelectedIndices.Add(index);
			}
		}
		if (!flag)
		{
			m_LastMouseUpSelectionIndex = -1;
		}
		RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(m_SelectedIndices.Count, clear: true);
		try
		{
			for (int i = 0; i < m_SelectedIndices.Count; i++)
			{
				int num = m_SelectedIndices[i];
				if (num >= 0 && num < m_HierarchyViewModel.Count)
				{
					rentSpanUnmanaged.Span[i] = m_HierarchyViewModel[num];
				}
			}
			m_SelectedIndices.Clear();
			using (new HierarchyViewModelFlagsChangeScope(m_HierarchyViewModel, notify: false))
			{
				m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Selected);
				m_HierarchyViewModel.SetFlags(rentSpanUnmanaged.Span, HierarchyNodeFlags.Selected);
			}
			if (m_MultiColumnListView.pointerProcessingState == BaseVerticalCollectionView.pointerProcessingStateEnum.PointerDown && m_MultiColumnListView.currentPointerButton != 1)
			{
				m_SelectedIndicesChangedFromPointerDown = true;
			}
			else
			{
				this.FlagsChanged?.Invoke(HierarchyNodeFlags.Selected);
			}
		}
		finally
		{
			rentSpanUnmanaged.Dispose();
		}
	}

	private void OnBindItem(HierarchyViewItem item)
	{
		item.ExpandedStateChanged += SetExpandedState;
	}

	private void SetExpandedState(in HierarchyNode node, bool isExpanded, bool recurse)
	{
		if (isExpanded)
		{
			if (recurse)
			{
				ExpandRecursive(in node);
			}
			else
			{
				Expand(in node);
			}
		}
		else if (recurse)
		{
			CollapseRecursive(in node);
		}
		else
		{
			Collapse(in node);
		}
	}

	private void SetExpandedState(ReadOnlySpan<HierarchyNode> nodes, bool isExpanded, bool recurse)
	{
		if (isExpanded)
		{
			if (recurse)
			{
				ExpandRecursive(nodes);
			}
			else
			{
				Expand(nodes);
			}
		}
		else if (recurse)
		{
			CollapseRecursive(nodes);
		}
		else
		{
			Collapse(nodes);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal bool UpdateListView()
	{
		if (m_Version == m_HierarchyViewModel.Version)
		{
			return false;
		}
		m_MultiColumnListView.RefreshItems();
		int num = m_HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected);
		if (num == 0)
		{
			SetListViewSelectionWithoutNotify(Array.Empty<int>());
		}
		else
		{
			RentSpanUnmanaged<int> rentSpan = new RentSpanUnmanaged<int>(num);
			try
			{
				m_HierarchyViewModel.GetIndicesWithAllFlags(HierarchyNodeFlags.Selected, rentSpan);
				SetListViewSelectionWithoutNotify(rentSpan);
			}
			finally
			{
				rentSpan.Dispose();
			}
		}
		m_Version = m_HierarchyViewModel.Version;
		return false;
	}

	private void SetListViewSelectionWithoutNotify(Span<int> selection)
	{
		RentSpanUnmanaged<int> rentSpanUnmanaged = new RentSpanUnmanaged<int>(selection.Length);
		try
		{
			int length = 0;
			for (int i = 0; i < selection.Length; i++)
			{
				int num = selection[i];
				if (num >= 0 && num < m_HierarchyViewModel.Count)
				{
					rentSpanUnmanaged.Span[length++] = num;
				}
			}
			MultiColumnListView multiColumnListView = m_MultiColumnListView;
			Span<int> span = rentSpanUnmanaged.Span;
			multiColumnListView.SetSelectionWithoutNotify(span.Slice(0, length));
		}
		finally
		{
			rentSpanUnmanaged.Dispose();
		}
	}

	private void SetColumnState(HierarchyViewState state)
	{
		List<Column> list = CollectionPool<List<Column>, Column>.Get();
		foreach (Column column in m_MultiColumnListView.columns)
		{
			list.Add(column);
		}
		SetColumns(list, state);
	}

	private void ClearColumns()
	{
		List<VisualElement> list = m_MultiColumnListView.Query<VisualElement>("unity-multi-column-view__row-container").ToList();
		foreach (VisualElement item in list)
		{
			List<HierarchyViewCell> list2 = item.Query<HierarchyViewCell>("HierarchyViewCell").ToList();
			foreach (HierarchyViewCell item2 in list2)
			{
				if (item2.Descriptor != null)
				{
					item2.UnbindCell();
				}
			}
		}
		foreach (Column column in m_MultiColumnListView.columns)
		{
			if (column is HierarchyViewColumn hierarchyViewColumn)
			{
				hierarchyViewColumn.UnbindColumn(this);
			}
		}
	}

	private void BindColumns()
	{
		foreach (Column column in m_MultiColumnListView.columns)
		{
			if (column is HierarchyViewColumn hierarchyViewColumn)
			{
				hierarchyViewColumn.BindColumn(this);
			}
		}
	}

	private bool DoUpdate(UpdateMode mode, double milliseconds = 0.0)
	{
		bool flag = DoUpdateStage(mode, milliseconds);
		if (!flag)
		{
			IncrementUpdateStage();
		}
		return flag || UpdateNeeded;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool DoUpdateStage(UpdateMode mode2, double milliseconds2)
		{
			switch (m_UpdateStage)
			{
			case UpdateStage.UpdatingHierarchy:
				return UpdateHierarchy(mode2, milliseconds2);
			case UpdateStage.UpdatingHierarchyFlattened:
				return UpdateHierarchyFlattened(mode2, milliseconds2);
			case UpdateStage.UpdatingHierarchyViewModel:
				return UpdateHierarchyViewModel(mode2, milliseconds2);
			case UpdateStage.UpdatingListView:
				return UpdateListView();
			case UpdateStage.ExecutePostUpdateActions:
				ExecuteActions(m_PostUpdateActionQueue);
				return false;
			default:
				throw new NotImplementedException(m_UpdateStage.ToString());
			}
		}
		static void ExecuteActions(CircularBuffer<Action> actions)
		{
			if (!actions.IsEmpty)
			{
			}
			while (!actions.IsEmpty)
			{
				Action action = actions.Front();
				try
				{
					actions.Locked = true;
					action?.Invoke();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
				finally
				{
					actions.Locked = false;
					actions.PopFront();
				}
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IncrementUpdateStage()
		{
			m_UpdateStage = (UpdateStage)((int)(m_UpdateStage + 1) % 5);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool UpdateHierarchy(UpdateMode updateMode, double milliseconds2)
		{
			if (m_Hierarchy != null && m_Hierarchy.IsCreated && m_Hierarchy.UpdateNeeded)
			{
				switch (updateMode)
				{
				case UpdateMode.Update:
					m_Hierarchy.Update();
					return false;
				case UpdateMode.UpdateIncremental:
					return m_Hierarchy.UpdateIncremental();
				case UpdateMode.UpdateIncrementalTimed:
					return m_Hierarchy.UpdateIncrementalTimed(milliseconds2);
				default:
					throw new NotImplementedException(updateMode.ToString());
				}
			}
			return false;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool UpdateHierarchyFlattened(UpdateMode updateMode, double milliseconds2)
		{
			if (m_HierarchyFlattened != null && m_HierarchyFlattened.IsCreated && m_HierarchyFlattened.UpdateNeeded)
			{
				switch (updateMode)
				{
				case UpdateMode.Update:
					m_HierarchyFlattened.Update();
					return false;
				case UpdateMode.UpdateIncremental:
					return m_HierarchyFlattened.UpdateIncremental();
				case UpdateMode.UpdateIncrementalTimed:
					return m_HierarchyFlattened.UpdateIncrementalTimed(milliseconds2);
				default:
					throw new NotImplementedException(updateMode.ToString());
				}
			}
			return false;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool UpdateHierarchyViewModel(UpdateMode updateMode, double milliseconds2)
		{
			if (m_HierarchyViewModel != null && m_HierarchyViewModel.IsCreated && m_HierarchyViewModel.UpdateNeeded)
			{
				switch (updateMode)
				{
				case UpdateMode.Update:
					m_HierarchyViewModel.Update();
					return false;
				case UpdateMode.UpdateIncremental:
					return m_HierarchyViewModel.UpdateIncremental();
				case UpdateMode.UpdateIncrementalTimed:
					return m_HierarchyViewModel.UpdateIncrementalTimed(milliseconds2);
				default:
					throw new NotImplementedException(updateMode.ToString());
				}
			}
			return false;
		}
	}
}
