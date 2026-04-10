using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

internal class HierarchyViewDragHandler
{
	private struct HierarchyViewDragAndDropTargets : IEquatable<HierarchyViewDragAndDropTargets>
	{
		public int insertAtIndex;

		public int targetIndex;

		public int parentIndex;

		public int childIndex;

		public DragAndDropPosition dropPosition;

		public DragVisualMode dragVisualMode;

		[NoAutoStaticsCleanup]
		public static readonly HierarchyViewDragAndDropTargets Rejected = new HierarchyViewDragAndDropTargets(-1, -1, -1, -1, DragAndDropPosition.OverItem)
		{
			dragVisualMode = DragVisualMode.Rejected
		};

		public HierarchyViewDragAndDropTargets(int insertAtIndex, int targetIndex, int parentIndex, int childIndex, DragAndDropPosition dropPosition)
		{
			this.insertAtIndex = insertAtIndex;
			this.targetIndex = targetIndex;
			this.parentIndex = parentIndex;
			this.childIndex = childIndex;
			this.dropPosition = dropPosition;
			dragVisualMode = DragVisualMode.Move;
		}

		public bool Equals(HierarchyViewDragAndDropTargets other)
		{
			return parentIndex == other.parentIndex && childIndex == other.childIndex && dropPosition == other.dropPosition && targetIndex == other.targetIndex;
		}

		public override bool Equals(object obj)
		{
			return obj is HierarchyViewDragAndDropTargets other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(parentIndex, childIndex, (int)dropPosition);
		}
	}

	private class AutoExpansionData
	{
		public HierarchyNode[] expandedNodesBeforeDrag;

		public int lastItemIndex = -1;

		public float expandItemBeginTimerMs;

		public Vector2 expandItemBeginPosition;
	}

	internal const string DragHoverBarStyleName = "hierarchy__container__drag-hover-bar";

	internal const string DragHoverBarItemName = "HierarchyHoverBar";

	internal const string DragHoverItemMarkerItemName = "HierarchyHoverItemMarker";

	internal const string DragHoverSiblingMarkerItemName = "HierarchyHoverSiblingMarker";

	private readonly HierarchyView m_HierarchyView;

	private readonly MultiColumnListView m_MultiColumnListView;

	private HierarchyViewDragAndDropTargets m_LastDragPosition;

	private AutoExpansionData m_AutoExpansionData;

	private IVisualElementScheduledItem m_ExpandItemScheduledItem;

	private VisualElement m_DragHoverBar;

	private VisualElement m_DragHoverItemMarker;

	private VisualElement m_DragHoverSiblingMarker;

	private EventModifiers m_CurrentEventModifiers;

	private float m_LeftIndentation = -1f;

	private float m_SiblingBottom = -1f;

	private const int k_DragHoverBarHeight = 2;

	private const int k_InvalidIndex = -1;

	private const long k_ExpandUpdateIntervalMs = 10L;

	private const float k_DropExpandTimeoutMs = 700f;

	private const float k_DropDeltaPosition = 100f;

	private const float k_HalfDropBetweenHeight = 4f;

	private const float k_DefaultIndentWidth = 14f;

	private const float k_DragHoverBarPositionOffset = 4f;

	private Hierarchy Hierarchy => m_HierarchyView.Source;

	private HierarchyFlattened HierarchyFlattened => m_HierarchyView.Flattened;

	private HierarchyViewModel HierarchyViewModel => m_HierarchyView.ViewModel;

	private BaseVerticalCollectionView TargetView => m_MultiColumnListView;

	private ScrollView TargetScrollView => TargetView.Q<ScrollView>();

	public HierarchyViewDragHandler(HierarchyView hierarchyView)
	{
		m_HierarchyView = hierarchyView;
		m_MultiColumnListView = m_HierarchyView.ListView;
		m_AutoExpansionData = new AutoExpansionData();
		m_MultiColumnListView.canStartDrag += CanStartDrag;
		m_MultiColumnListView.setupDragAndDrop += SetupDragAndDrop;
		m_MultiColumnListView.dragAndDropUpdate += DragAndDropUpdate;
		m_MultiColumnListView.handleDrop += HandleDrop;
		m_MultiColumnListView.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
		m_MultiColumnListView.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
		m_MultiColumnListView.RegisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
		m_HierarchyView.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
	}

	private void OnPointerLeave(PointerLeaveEvent evt)
	{
		ClearDragAndDropUI();
	}

	private void OnPointerDown(PointerDownEvent evt)
	{
		m_CurrentEventModifiers = evt.modifiers;
	}

	private void OnPointerMove(PointerMoveEvent evt)
	{
		m_CurrentEventModifiers = evt.modifiers;
	}

	private void OnPointerUp(PointerUpEvent evt)
	{
		m_CurrentEventModifiers = evt.modifiers;
	}

	private bool IsSearchActive()
	{
		return m_HierarchyView.Filtering;
	}

	private bool CanStartDrag(CanStartDragArgs args)
	{
		if (IsSearchActive())
		{
			return false;
		}
		RentSpanUnmanaged<HierarchyNode> rentSpan = new RentSpanUnmanaged<HierarchyNode>(HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected));
		try
		{
			HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, rentSpan);
			foreach (HierarchyNodeTypeHandler item in Hierarchy.EnumerateNodeTypeHandlers())
			{
				if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler && !hierarchyEditorNodeTypeHandler.CanStartDrag(m_HierarchyView, rentSpan))
				{
					return false;
				}
			}
			return true;
		}
		finally
		{
			rentSpan.Dispose();
		}
	}

	private StartDragArgs SetupDragAndDrop(SetupDragAndDropArgs args)
	{
		int length = HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected);
		RentSpanUnmanaged<HierarchyNode> rentSpan = new RentSpanUnmanaged<HierarchyNode>(length);
		HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, rentSpan);
		List<EntityId> entityIds = new List<EntityId>();
		List<string> value;
		using (CollectionPool<List<string>, string>.Get(out value))
		{
			Dictionary<string, object> value2;
			using (CollectionPool<Dictionary<string, object>, KeyValuePair<string, object>>.Get(out value2))
			{
				HierarchyViewDragAndDropSetupData data = new HierarchyViewDragAndDropSetupData(rentSpan, entityIds, value, m_HierarchyView, value2);
				foreach (HierarchyNodeTypeHandler item in Hierarchy.EnumerateNodeTypeHandlers())
				{
					if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler)
					{
						hierarchyEditorNodeTypeHandler.OnStartDrag(in data);
					}
				}
				StartDragArgs result = new StartDragArgs(args.startDragArgs.title, args.startDragArgs.visualMode);
				result.SetEntityIds(entityIds);
				result.SetPaths(value.ToArray());
				foreach (KeyValuePair<string, object> item2 in value2)
				{
					result.SetGenericData(item2.Key, item2.Value);
				}
				return result;
			}
		}
	}

	private DragVisualMode DragAndDropUpdate(HandleDragAndDropArgs args)
	{
		HierarchyViewDragAndDropTargets visualMode = GetVisualMode(in args);
		if (visualMode.dragVisualMode == DragVisualMode.Rejected)
		{
			ClearDragAndDropUI();
		}
		else
		{
			HandleAutoExpansion(visualMode, args.position);
			ApplyDragAndDropUI(visualMode);
		}
		return visualMode.dragVisualMode;
	}

	private HierarchyViewDragAndDropTargets GetVisualMode(in HandleDragAndDropArgs args)
	{
		if (args.insertAtIndex < 0)
		{
			return HierarchyViewDragAndDropTargets.Rejected;
		}
		HierarchyViewDragAndDropTargets dragAndDropTargets = GetDragAndDropTargets(in args);
		HierarchyNode parentNode = ((dragAndDropTargets.parentIndex == -1) ? Hierarchy.Root : HierarchyViewModel[dragAndDropTargets.parentIndex]);
		dragAndDropTargets = HandleNodeHandlersDrop(dragAndDropTargets, args.dragAndDropData, in parentNode, perform: false);
		if (dragAndDropTargets.dragVisualMode != DragVisualMode.None)
		{
			return dragAndDropTargets;
		}
		return HandleDefaultCanDrop(in args, dragAndDropTargets, in parentNode);
	}

	private HierarchyViewDragAndDropTargets HandleDefaultCanDrop(in HandleDragAndDropArgs args, HierarchyViewDragAndDropTargets dragAndDropTargets, in HierarchyNode parentNode)
	{
		if (!DragSourceIsCurrentListView(in args))
		{
			return HierarchyViewDragAndDropTargets.Rejected;
		}
		RentSpanUnmanaged<HierarchyNode> rentSpan = new RentSpanUnmanaged<HierarchyNode>(HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected));
		try
		{
			HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, rentSpan);
			IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = ((parentNode == Hierarchy.Root) ? null : (Hierarchy.GetNodeTypeHandler(in parentNode) as IHierarchyEditorNodeTypeHandler));
			for (int i = 0; i < rentSpan.Span.Length; i++)
			{
				HierarchyNode target = rentSpan.Span[i];
				if (IsDescendant(in parentNode, in target))
				{
					return HierarchyViewDragAndDropTargets.Rejected;
				}
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler2 = Hierarchy.GetNodeTypeHandler(in target) as IHierarchyEditorNodeTypeHandler;
				if (hierarchyEditorNodeTypeHandler != null && !hierarchyEditorNodeTypeHandler.AcceptChild(m_HierarchyView, in target))
				{
					return HierarchyViewDragAndDropTargets.Rejected;
				}
				if (hierarchyEditorNodeTypeHandler2 != null && !hierarchyEditorNodeTypeHandler2.AcceptParent(m_HierarchyView, in parentNode))
				{
					return HierarchyViewDragAndDropTargets.Rejected;
				}
			}
			dragAndDropTargets.dragVisualMode = DragVisualMode.Move;
			return dragAndDropTargets;
		}
		finally
		{
			rentSpan.Dispose();
		}
	}

	private bool IsDescendant(in HierarchyNode possibleDescendant, in HierarchyNode target)
	{
		if (Hierarchy.GetDepth(in possibleDescendant) <= Hierarchy.GetDepth(in target))
		{
			return false;
		}
		HierarchyNode lhs = Hierarchy.GetParent(in possibleDescendant);
		while (lhs != HierarchyNode.Null)
		{
			if (lhs == target)
			{
				return true;
			}
			lhs = Hierarchy.GetParent(in lhs);
		}
		return false;
	}

	private HierarchyViewDragAndDropTargets HandleNodeHandlersDrop(HierarchyViewDragAndDropTargets dragAndDropTargets, DragAndDropData dragAndDropData, in HierarchyNode parentNode, bool perform)
	{
		HierarchyViewDragAndDropHandlingData data = new HierarchyViewDragAndDropHandlingData(in parentNode, (dragAndDropTargets.targetIndex == -1 || dragAndDropTargets.targetIndex >= HierarchyViewModel.Count) ? HierarchyNode.Null : HierarchyViewModel[dragAndDropTargets.targetIndex], dragAndDropTargets.insertAtIndex, dragAndDropTargets.dropPosition, dragAndDropData, m_HierarchyView, m_CurrentEventModifiers);
		foreach (HierarchyNodeTypeHandler item in Hierarchy.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler)
			{
				DragVisualMode dragVisualMode = (perform ? hierarchyEditorNodeTypeHandler.OnDrop(in data) : hierarchyEditorNodeTypeHandler.CanDrop(in data));
				if (dragVisualMode != DragVisualMode.None)
				{
					dragAndDropTargets.dragVisualMode = dragVisualMode;
					return dragAndDropTargets;
				}
			}
		}
		dragAndDropTargets.dragVisualMode = DragVisualMode.None;
		return dragAndDropTargets;
	}

	private DragVisualMode HandleDrop(HandleDragAndDropArgs args)
	{
		ClearDragAndDropUI();
		if (args.insertAtIndex < 0)
		{
			return DragVisualMode.Rejected;
		}
		HierarchyViewDragAndDropTargets dragAndDropTargets = GetDragAndDropTargets(in args);
		HierarchyNode parentNode = ((dragAndDropTargets.parentIndex == -1) ? Hierarchy.Root : HierarchyViewModel[dragAndDropTargets.parentIndex]);
		int version = HierarchyViewModel.Version;
		dragAndDropTargets = HandleNodeHandlersDrop(dragAndDropTargets, args.dragAndDropData, in parentNode, perform: true);
		if (HierarchyViewModel.Version != version)
		{
			return DragVisualMode.Rejected;
		}
		DragVisualMode dragVisualMode = dragAndDropTargets.dragVisualMode;
		if (dragVisualMode == DragVisualMode.None)
		{
			dragVisualMode = HandleDefaultDrop(in args, dragAndDropTargets, in parentNode);
		}
		if (parentNode != Hierarchy.Root)
		{
			HierarchyViewModel.SetFlags(in parentNode, HierarchyNodeFlags.Expanded);
		}
		ClearAutoExpansionData(restoreState: false);
		if (dragVisualMode != DragVisualMode.Rejected && dragVisualMode != DragVisualMode.None)
		{
			m_HierarchyView.EnqueuePostUpdateAction(delegate
			{
				HierarchyViewModelNodesEnumerable.Enumerator enumerator = m_HierarchyView.ViewModel.EnumerateNodesWithAllFlags(HierarchyNodeFlags.Selected).GetEnumerator();
				while (enumerator.MoveNext())
				{
					ref readonly HierarchyNode current = ref enumerator.Current;
					if (!(current == HierarchyNode.Null) && !(current == m_HierarchyView.Source.Root))
					{
						m_HierarchyView.Frame(in current);
						break;
					}
				}
			});
		}
		return dragVisualMode;
	}

	private DragVisualMode HandleDefaultDrop(in HandleDragAndDropArgs args, HierarchyViewDragAndDropTargets dragAndDropTargets, in HierarchyNode parentNode)
	{
		if (!DragSourceIsCurrentListView(in args))
		{
			return DragVisualMode.Rejected;
		}
		RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected));
		try
		{
			Span<HierarchyNode> outNodes = rentSpanUnmanaged.Span;
			HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, outNodes);
			HierarchyNode[] children = Hierarchy.GetChildren(in parentNode);
			int num = dragAndDropTargets.childIndex;
			for (int i = 0; i < outNodes.Length; i++)
			{
				if (outNodes[i] == parentNode)
				{
					for (int j = i; j < outNodes.Length - 1; j++)
					{
						outNodes[j] = outNodes[j + 1];
					}
					outNodes = outNodes.Slice(0, outNodes.Length - 1);
					break;
				}
			}
			List<HierarchyNode> list = new List<HierarchyNode>(outNodes.Length);
			List<int> list2 = new List<int>(outNodes.Length);
			int num2 = 0;
			for (int k = 0; k < outNodes.Length; k++)
			{
				HierarchyNode lhs = Hierarchy.GetParent(in outNodes[k]);
				if (!(lhs == parentNode) && !list.Contains(lhs))
				{
					list.Add(lhs);
					int childrenCount = Hierarchy.GetChildrenCount(in lhs);
					list2.Add(childrenCount);
					num2 = Math.Max(num2, childrenCount);
				}
			}
			int num4;
			if (list.Count > 0)
			{
				HierarchyNode[] array = new HierarchyNode[num2];
				for (int l = 0; l < list.Count; l++)
				{
					HierarchyNode node = list[l];
					Hierarchy.GetChildren(in node, array);
					int num3 = list2[l];
					num4 = 0;
					for (int m = 0; m < num3; m++)
					{
						HierarchyNode node2 = array[m];
						if (!HierarchyViewModel.HasAllFlags(in node2, HierarchyNodeFlags.Selected))
						{
							Hierarchy.SetSortIndex(in node2, num4++);
						}
					}
				}
			}
			for (int n = 0; n < outNodes.Length; n++)
			{
				Hierarchy.SetParent(in outNodes[n], in parentNode);
			}
			if (num == -1)
			{
				num = children.Length;
			}
			num4 = 0;
			for (int num5 = 0; num5 < num && num5 < children.Length; num5++)
			{
				HierarchyNode node3 = children[num5];
				if (!HierarchyViewModel.HasAllFlags(in node3, HierarchyNodeFlags.Selected))
				{
					Hierarchy.SetSortIndex(in node3, num4++);
				}
			}
			for (int num6 = 0; num6 < outNodes.Length; num6++)
			{
				Hierarchy.SetSortIndex(in outNodes[num6], num4++);
			}
			for (int num7 = num; num7 < children.Length; num7++)
			{
				HierarchyNode node4 = children[num7];
				if (!HierarchyViewModel.HasAllFlags(in node4, HierarchyNodeFlags.Selected))
				{
					Hierarchy.SetSortIndex(in node4, num4++);
				}
			}
			Hierarchy.SortChildren(in parentNode);
			return DragVisualMode.Move;
		}
		finally
		{
			rentSpanUnmanaged.Dispose();
		}
	}

	private HierarchyViewDragAndDropTargets GetDragAndDropTargets(in HandleDragAndDropArgs args)
	{
		ReadOnlySpan<int> draggedIndices;
		if (DragSourceIsCurrentListView(in args))
		{
			int length = HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected);
			RentSpanUnmanaged<int> rentSpan = new RentSpanUnmanaged<int>(length);
			try
			{
				HierarchyViewModel.GetIndicesWithAllFlags(HierarchyNodeFlags.Selected, rentSpan);
				draggedIndices = rentSpan;
				return HandleTreePosition(in args, in draggedIndices);
			}
			finally
			{
				rentSpan.Dispose();
			}
		}
		draggedIndices = Array.Empty<int>();
		return HandleTreePosition(in args, in draggedIndices);
	}

	private bool DragSourceIsCurrentListView(in HandleDragAndDropArgs args)
	{
		return args.dragAndDropData.source == TargetView;
	}

	private HierarchyViewDragAndDropTargets HandleTreePosition(in HandleDragAndDropArgs dnDArgs, in ReadOnlySpan<int> draggedIndices)
	{
		m_LeftIndentation = -1f;
		m_SiblingBottom = -1f;
		if (dnDArgs.insertAtIndex < 0)
		{
			return new HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, -1, -1, -1, DragAndDropPosition.OutsideItems);
		}
		if (dnDArgs.dropPosition == DragAndDropPosition.OverItem)
		{
			return new HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, dnDArgs.insertAtIndex, dnDArgs.insertAtIndex, -1, DragAndDropPosition.OverItem);
		}
		if (dnDArgs.insertAtIndex <= 0)
		{
			return new HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, 0, -1, 0, DragAndDropPosition.BetweenItems);
		}
		int indexFromWorldPosition = m_HierarchyView.GetIndexFromWorldPosition(dnDArgs.position, 4f);
		if (indexFromWorldPosition >= m_HierarchyView.ViewModel.Count)
		{
			return new HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, 0, -1, -1, DragAndDropPosition.OutsideItems);
		}
		return HandleSiblingInsertionAtAvailableDepthsAndChangeTargetIfNeeded(in dnDArgs, dnDArgs.position, in draggedIndices);
	}

	private HierarchyViewDragAndDropTargets HandleSiblingInsertionAtAvailableDepthsAndChangeTargetIfNeeded(in HandleDragAndDropArgs dnDArgs, in Vector2 pointerPosition, in ReadOnlySpan<int> draggedIndices)
	{
		int insertAtIndex = dnDArgs.insertAtIndex;
		GetPreviousAndNextIndexesIgnoringDraggedItems(dnDArgs.insertAtIndex, out var previousNodeIndex, out var nextNodeIndex, in draggedIndices);
		HierarchyViewDragAndDropTargets result = new HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, dnDArgs.insertAtIndex, -1, -1, DragAndDropPosition.BetweenItems);
		if (previousNodeIndex == -1)
		{
			return result;
		}
		HierarchyNode node = HierarchyViewModel[previousNodeIndex];
		HierarchyNode node2 = ((nextNodeIndex == -1) ? HierarchyNode.Null : HierarchyViewModel[nextNodeIndex]);
		bool flag = HierarchyFlattened.GetChildrenCount(in node) > 0 && HierarchyViewModel.HasAllFlags(in node, HierarchyNodeFlags.Expanded);
		int depth = HierarchyFlattened.GetDepth(in node);
		int num = ((nextNodeIndex != -1) ? HierarchyFlattened.GetDepth(in node2) : 0);
		int num2 = num;
		int num3 = depth + (flag ? 1 : 0);
		int num4 = previousNodeIndex;
		result.targetIndex = previousNodeIndex;
		HierarchyNode node3 = node;
		int num5 = depth;
		float num6 = 0f;
		float num7 = 14f;
		VisualElement visualElement = null;
		if (depth > 0)
		{
			visualElement = TargetView.GetRootElementForIndex(previousNodeIndex);
		}
		else
		{
			HierarchyNode lhs = ((dnDArgs.insertAtIndex == -1 || dnDArgs.insertAtIndex >= HierarchyViewModel.Count) ? HierarchyNode.Null : HierarchyViewModel[dnDArgs.insertAtIndex]);
			int num8 = ((!(lhs == HierarchyNode.Null)) ? HierarchyFlattened.GetDepth(in lhs) : 0);
			if (num8 > 0)
			{
				visualElement = TargetView.GetRootElementForIndex(dnDArgs.insertAtIndex);
			}
		}
		HierarchyViewItem hierarchyViewItem = visualElement?.Q<HierarchyViewItem>();
		if (hierarchyViewItem != null)
		{
			num6 = hierarchyViewItem.Toggle.layout.width;
			num7 = ((depth > 0) ? ((hierarchyViewItem.LeftContainer.style.translate.value.x.value + 4f) / (float)depth) : num7);
		}
		VisualElement nameColumn = GetNameColumn();
		bool flag2 = false;
		Vector2 vector = Vector2.zero;
		if (nameColumn != null)
		{
			vector = nameColumn.WorldToLocal(pointerPosition);
			flag2 = vector.x >= 0f && vector.x < nameColumn.layout.width;
		}
		if (num3 <= num2)
		{
			m_LeftIndentation = num6 + num7 * (float)num2;
			if (flag)
			{
				result.parentIndex = previousNodeIndex;
				result.childIndex = 0;
			}
			else
			{
				HierarchyNode node4 = HierarchyFlattened.GetParent(in node);
				result.parentIndex = GetNodeIndex(in node4);
				result.childIndex = ((nextNodeIndex == -1) ? HierarchyFlattened.GetChildrenCount(in node4) : GetChildIndex(in node2));
			}
			return result;
		}
		int num9 = (flag2 ? Mathf.FloorToInt((vector.x - num6) / num7) : num3);
		if (num9 >= num3)
		{
			m_LeftIndentation = num6 + num7 * (float)num3;
			if (flag)
			{
				result.parentIndex = previousNodeIndex;
				result.childIndex = 0;
			}
			else
			{
				result.parentIndex = GetNodeIndex(HierarchyFlattened.GetParent(in node));
				result.childIndex = GetChildIndex(in node) + 1;
			}
			return result;
		}
		while (num5 > num2 && num5 != num9)
		{
			node3 = HierarchyFlattened.GetParent(in node3);
			num4 = GetNodeIndex(in node3);
			num5--;
		}
		if (num4 != insertAtIndex)
		{
			VisualElement rootElementForIndex = TargetView.GetRootElementForIndex(num4);
			if (rootElementForIndex != null)
			{
				VisualElement contentViewport = TargetScrollView.contentViewport;
				Rect rect = contentViewport.WorldToLocal(rootElementForIndex.worldBound);
				if (contentViewport.localBound.yMin < rect.yMax && rect.yMax < contentViewport.localBound.yMax)
				{
					m_SiblingBottom = rect.yMax;
				}
			}
		}
		result.parentIndex = GetNodeIndex(HierarchyFlattened.GetParent(in node3));
		result.targetIndex = num4;
		result.childIndex = GetChildIndex(in node3) + 1;
		m_LeftIndentation = num6 + num7 * (float)num5;
		return result;
	}

	private void GetPreviousAndNextIndexesIgnoringDraggedItems(int insertAtIndex, out int previousNodeIndex, out int nextNodeIndex, in ReadOnlySpan<int> draggedIndices)
	{
		previousNodeIndex = (nextNodeIndex = -1);
		int num = insertAtIndex - 1;
		int i = insertAtIndex;
		while (num >= 0)
		{
			if (!draggedIndices.Contains(num))
			{
				previousNodeIndex = num;
				break;
			}
			num--;
		}
		for (int count = HierarchyViewModel.Count; i < count; i++)
		{
			if (!draggedIndices.Contains(i))
			{
				nextNodeIndex = i;
				break;
			}
		}
	}

	private int GetChildIndex(in HierarchyNode childNode)
	{
		return HierarchyFlattened.GetChildIndex(in childNode);
	}

	private int GetNodeIndex(in HierarchyNode node)
	{
		return HierarchyViewModel.IndexOf(in node);
	}

	private void ApplyDragAndDropUI(HierarchyViewDragAndDropTargets dragTargets)
	{
		if (m_LastDragPosition.Equals(dragTargets))
		{
			return;
		}
		ScrollView targetScrollView = TargetScrollView;
		if (m_DragHoverBar == null)
		{
			m_DragHoverBar = new VisualElement
			{
				name = "HierarchyHoverBar"
			};
			m_DragHoverBar.AddToClassList(BaseVerticalCollectionView.dragHoverBarUssClassName);
			m_DragHoverBar.AddToClassList("hierarchy__container__drag-hover-bar");
			m_DragHoverBar.style.width = TargetView.localBound.width;
			m_DragHoverBar.style.visibility = Visibility.Hidden;
			m_DragHoverBar.pickingMode = PickingMode.Ignore;
			TargetView.RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
			targetScrollView.contentViewport.Add(m_DragHoverBar);
		}
		if (m_DragHoverItemMarker == null)
		{
			m_DragHoverItemMarker = new VisualElement
			{
				name = "HierarchyHoverItemMarker"
			};
			m_DragHoverItemMarker.AddToClassList(BaseVerticalCollectionView.dragHoverMarkerUssClassName);
			m_DragHoverItemMarker.style.visibility = Visibility.Hidden;
			m_DragHoverItemMarker.pickingMode = PickingMode.Ignore;
			m_DragHoverBar.Add(m_DragHoverItemMarker);
			m_DragHoverSiblingMarker = new VisualElement
			{
				name = "HierarchyHoverSiblingMarker"
			};
			m_DragHoverSiblingMarker.AddToClassList(BaseVerticalCollectionView.dragHoverMarkerUssClassName);
			m_DragHoverSiblingMarker.style.visibility = Visibility.Hidden;
			m_DragHoverSiblingMarker.pickingMode = PickingMode.Ignore;
			targetScrollView.contentViewport.Add(m_DragHoverSiblingMarker);
		}
		ClearDragAndDropUI();
		m_LastDragPosition = dragTargets;
		switch (dragTargets.dropPosition)
		{
		case DragAndDropPosition.OverItem:
			break;
		case DragAndDropPosition.BetweenItems:
		{
			if (dragTargets.insertAtIndex == 0)
			{
				PlaceHoverBarAt(0f);
				break;
			}
			VisualElement rootElementForIndex2 = TargetView.GetRootElementForIndex(dragTargets.insertAtIndex - 1);
			VisualElement rootElementForIndex3 = TargetView.GetRootElementForIndex(dragTargets.insertAtIndex);
			PlaceHoverBarAtElement(rootElementForIndex2 ?? rootElementForIndex3);
			break;
		}
		case DragAndDropPosition.OutsideItems:
		{
			VisualElement rootElementForIndex = TargetView.GetRootElementForIndex(TargetView.itemsSource.Count - 1);
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
			throw new ArgumentOutOfRangeException("dropPosition", dragTargets.dropPosition, "Unsupported dropPosition value");
		}
		void GeometryChangedCallback(GeometryChangedEvent e)
		{
			m_DragHoverBar.style.width = TargetView.localBound.width;
		}
	}

	private void ClearDragAndDropUI()
	{
		m_LastDragPosition = default(HierarchyViewDragAndDropTargets);
		if (m_DragHoverBar != null)
		{
			m_DragHoverBar.style.visibility = Visibility.Hidden;
		}
		if (m_DragHoverItemMarker != null)
		{
			m_DragHoverItemMarker.style.visibility = Visibility.Hidden;
		}
		if (m_DragHoverSiblingMarker != null)
		{
			m_DragHoverSiblingMarker.style.visibility = Visibility.Hidden;
		}
	}

	private void ClearDragAndDrop()
	{
		m_CurrentEventModifiers = EventModifiers.None;
		ClearDragAndDropUI();
		ClearAutoExpansionData();
	}

	private void ClearAutoExpansionData(bool restoreState = true)
	{
		if (restoreState && m_AutoExpansionData?.expandedNodesBeforeDrag != null)
		{
			RestoreExpanded(m_AutoExpansionData.expandedNodesBeforeDrag);
		}
		m_AutoExpansionData = new AutoExpansionData();
		m_ExpandItemScheduledItem?.Pause();
	}

	private void RestoreExpanded(ReadOnlySpan<HierarchyNode> expandedNodes)
	{
		using (new HierarchyViewModelFlagsChangeScope(HierarchyViewModel))
		{
			HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Expanded);
			HierarchyViewModel.SetFlags(expandedNodes, HierarchyNodeFlags.Expanded);
		}
	}

	private float GetHoverBarTopPosition(VisualElement item)
	{
		VisualElement contentViewport = TargetScrollView.contentViewport;
		return Mathf.Min(contentViewport.WorldToLocal(item.worldBound).yMax, contentViewport.localBound.yMax - 2f);
	}

	private void PlaceHoverBarAtElement(VisualElement item)
	{
		PlaceHoverBarAt(GetHoverBarTopPosition(item), m_LeftIndentation, m_SiblingBottom);
	}

	private void PlaceHoverBarAt(float top, float indentationPadding = -1f, float siblingBottom = -1f)
	{
		m_DragHoverBar.style.top = top;
		m_DragHoverBar.style.visibility = Visibility.Visible;
		Rect nameColumnLayout = GetNameColumnLayout();
		float xMin = nameColumnLayout.xMin;
		float width = TargetView.localBound.width;
		if (nameColumnLayout.width > 0f)
		{
			width = nameColumnLayout.width;
		}
		else
		{
			indentationPadding = -1f;
		}
		if (m_DragHoverItemMarker != null)
		{
			m_DragHoverItemMarker.style.visibility = Visibility.Visible;
		}
		if (indentationPadding >= 0f)
		{
			m_DragHoverBar.style.marginLeft = xMin + indentationPadding;
			m_DragHoverBar.style.width = width - indentationPadding;
			if (siblingBottom > 0f && m_DragHoverSiblingMarker != null)
			{
				m_DragHoverSiblingMarker.style.top = siblingBottom;
				m_DragHoverSiblingMarker.style.visibility = Visibility.Visible;
				m_DragHoverSiblingMarker.style.marginLeft = xMin + indentationPadding;
			}
		}
		else
		{
			m_DragHoverBar.style.marginLeft = xMin;
			m_DragHoverBar.style.width = width;
		}
	}

	private VisualElement GetNameColumn()
	{
		return TargetView.Q("HierarchyViewColumn Name");
	}

	private Rect GetNameColumnLayout()
	{
		return GetNameColumn()?.layout ?? Rect.zero;
	}

	private void HandleAutoExpansion(HierarchyViewDragAndDropTargets dropTargets, Vector2 pointerPosition)
	{
		if (dropTargets.dropPosition == DragAndDropPosition.OverItem)
		{
			int parentIndex = dropTargets.parentIndex;
			VisualElement rootElementForIndex = m_MultiColumnListView.GetRootElementForIndex(parentIndex);
			if (rootElementForIndex != null)
			{
				HandleAutoExpansion(rootElementForIndex, parentIndex, pointerPosition);
			}
		}
	}

	private void HandleAutoExpansion(VisualElement item, int itemIndex, Vector2 pointerPosition)
	{
		Rect worldBound = item.worldBound;
		bool flag = new Rect(worldBound.x, worldBound.y + 4f, worldBound.width, worldBound.height - 8f).Contains(pointerPosition);
		Vector2 vector = m_AutoExpansionData.expandItemBeginPosition - pointerPosition;
		if (itemIndex != m_AutoExpansionData.lastItemIndex || !flag || vector.sqrMagnitude >= 100f)
		{
			m_AutoExpansionData.lastItemIndex = itemIndex;
			m_AutoExpansionData.expandItemBeginTimerMs = 0f;
			m_AutoExpansionData.expandItemBeginPosition = pointerPosition;
			DelayExpandItem();
		}
	}

	private void DelayExpandItem()
	{
		if (m_ExpandItemScheduledItem == null)
		{
			m_ExpandItemScheduledItem = m_MultiColumnListView.schedule.Execute(ExpandItem).Every(10L);
			return;
		}
		m_ExpandItemScheduledItem.Pause();
		m_ExpandItemScheduledItem.Resume();
	}

	internal void ExpandItem(TimerState state)
	{
		m_AutoExpansionData.expandItemBeginTimerMs = (float)state.deltaTime + m_AutoExpansionData.expandItemBeginTimerMs;
		bool flag = m_AutoExpansionData.expandItemBeginTimerMs > 700f;
		int lastItemIndex = m_AutoExpansionData.lastItemIndex;
		if (!flag || lastItemIndex < 0 || lastItemIndex >= HierarchyViewModel.Count)
		{
			return;
		}
		HierarchyNode node = HierarchyViewModel[lastItemIndex];
		bool flag2 = HierarchyViewModel.GetChildrenCount(in node) > 0;
		bool flag3 = HierarchyViewModel.HasAllFlags(in node, HierarchyNodeFlags.Expanded);
		if (!(!flag2 || flag3))
		{
			HierarchyNode[] nodesWithAllFlags = HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Expanded);
			AutoExpansionData autoExpansionData = m_AutoExpansionData;
			if (autoExpansionData.expandedNodesBeforeDrag == null)
			{
				autoExpansionData.expandedNodesBeforeDrag = nodesWithAllFlags;
			}
			m_AutoExpansionData.expandItemBeginTimerMs = 0f;
			m_AutoExpansionData.lastItemIndex = -1;
			HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Expanded);
		}
	}
}
