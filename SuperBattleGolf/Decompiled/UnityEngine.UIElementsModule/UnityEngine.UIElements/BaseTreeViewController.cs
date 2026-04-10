using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;
using Unity.Profiling;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public abstract class BaseTreeViewController : CollectionViewController
{
	private protected Hierarchy m_Hierarchy;

	private protected HierarchyFlattened m_HierarchyFlattened;

	private protected HierarchyViewModel m_HierarchyViewModel;

	private protected Dictionary<int, HierarchyNode> m_IdToNodeDictionary = new Dictionary<int, HierarchyNode>();

	private const string k_HierarchyPropertyName = "TreeViewDataProperty";

	private IHierarchyProperty<int> m_TreeViewDataProperty;

	private bool m_HierarchyHasPendingChanged;

	private static readonly ProfilerMarker K_ExpandItemByIndex = new ProfilerMarker(ProfilerCategory.Scripts, "BaseTreeViewController.ExpandItemByIndex");

	protected BaseTreeView baseTreeView => base.view as BaseTreeView;

	private protected Hierarchy hierarchy
	{
		get
		{
			return m_Hierarchy;
		}
		set
		{
			if (hierarchy != value)
			{
				DisposeHierarchy();
				if (value != null)
				{
					m_Hierarchy = value;
					m_HierarchyFlattened = new HierarchyFlattened(m_Hierarchy);
					m_HierarchyViewModel = new HierarchyViewModel(m_HierarchyFlattened);
					m_TreeViewDataProperty = m_Hierarchy.GetOrCreatePropertyUnmanaged<int>("TreeViewDataProperty");
				}
			}
		}
	}

	public override IList itemsSource
	{
		get
		{
			return base.itemsSource;
		}
		set
		{
			throw new InvalidOperationException("Can't set itemsSource directly. Override this controller to manage tree data.");
		}
	}

	internal event Action<TreeViewExpansionChangedArgs> itemExpandedChanged;

	protected BaseTreeViewController()
	{
		hierarchy = new Hierarchy();
	}

	~BaseTreeViewController()
	{
		DisposeHierarchy();
	}

	internal void DisposeHierarchy()
	{
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
		if (m_Hierarchy != null)
		{
			if (m_Hierarchy.IsCreated)
			{
				m_Hierarchy.Dispose();
			}
			m_Hierarchy = null;
		}
	}

	[Obsolete("RebuildTree is no longer supported and will be removed.", false)]
	public void RebuildTree()
	{
	}

	public IEnumerable<int> GetRootItemIds()
	{
		HierarchyNodeChildren.Enumerator enumerator = m_Hierarchy.EnumerateChildren(in m_Hierarchy.Root).GetEnumerator();
		while (enumerator.MoveNext())
		{
			HierarchyNode node = enumerator.Current;
			yield return m_TreeViewDataProperty.GetValue(in node);
		}
	}

	public virtual IEnumerable<int> GetAllItemIds(IEnumerable<int> rootIds = null)
	{
		if (rootIds == null)
		{
			HierarchyFlattened.Enumerator enumerator = m_HierarchyFlattened.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HierarchyFlattenedNode flattenedNode = enumerator.Current;
				HierarchyNode node = flattenedNode.Node;
				if (!(node == m_Hierarchy.Root) && m_Hierarchy.Exists(in node))
				{
					yield return m_TreeViewDataProperty.GetValue(in node);
				}
			}
			yield break;
		}
		foreach (int id in rootIds)
		{
			HierarchyNode parentNode = m_IdToNodeDictionary[id];
			if (m_Hierarchy.Exists(in parentNode))
			{
				HierarchyFlattenedChildrenEnumerable.Enumerator enumerator3 = m_HierarchyFlattened.EnumerateChildren(in parentNode).GetEnumerator();
				while (enumerator3.MoveNext())
				{
					HierarchyFlattenedNode flattenedNode2 = enumerator3.Current;
					yield return m_TreeViewDataProperty.GetValue(flattenedNode2.Node);
				}
				yield return id;
			}
		}
	}

	public virtual int GetParentId(int id)
	{
		HierarchyNode lhs = GetHierarchyNodeById(id);
		if (lhs == HierarchyNode.Null || !m_Hierarchy.Exists(in lhs))
		{
			return BaseTreeView.invalidId;
		}
		HierarchyNode lhs2 = m_Hierarchy.GetParent(in lhs);
		if (lhs2 == m_Hierarchy.Root)
		{
			return BaseTreeView.invalidId;
		}
		return m_TreeViewDataProperty.GetValue(in lhs2);
	}

	public virtual IEnumerable<int> GetChildrenIds(int id)
	{
		HierarchyNode nodeById = GetHierarchyNodeById(id);
		if (!(nodeById == HierarchyNode.Null) && m_Hierarchy.Exists(in nodeById))
		{
			HierarchyNodeChildren.Enumerator enumerator = m_Hierarchy.EnumerateChildren(in nodeById).GetEnumerator();
			while (enumerator.MoveNext())
			{
				HierarchyNode node = enumerator.Current;
				yield return m_TreeViewDataProperty.GetValue(in node);
			}
		}
	}

	public virtual void Move(int id, int newParentId, int childIndex = -1, bool rebuildTree = true)
	{
		if (id == newParentId || IsChildOf(newParentId, id) || !m_IdToNodeDictionary.TryGetValue(id, out var insertedNode))
		{
			return;
		}
		HierarchyNode rhs = ((newParentId == BaseTreeView.invalidId) ? m_Hierarchy.Root : GetHierarchyNodeById(newParentId));
		if (m_Hierarchy.GetParent(in insertedNode) == rhs)
		{
			int childIndexForId = GetChildIndexForId(id);
			if (childIndexForId < childIndex)
			{
				childIndex--;
			}
		}
		else
		{
			m_Hierarchy.SetParent(in insertedNode, in rhs);
		}
		UpdateSortOrder(in rhs, in insertedNode, childIndex);
		if (rebuildTree)
		{
			RaiseItemParentChanged(id, newParentId);
		}
	}

	public abstract bool TryRemoveItem(int id, bool rebuildTree = true);

	internal override void InvokeMakeItem(ReusableCollectionItem reusableItem)
	{
		if (reusableItem is ReusableTreeViewItem reusableTreeViewItem)
		{
			reusableTreeViewItem.Init(MakeItem());
			PostInitRegistration(reusableTreeViewItem);
		}
	}

	internal override void InvokeBindItem(ReusableCollectionItem reusableItem, int index)
	{
		if (reusableItem is ReusableTreeViewItem reusableTreeViewItem)
		{
			reusableTreeViewItem.customIndentWidth = baseTreeView.customIdent;
			reusableTreeViewItem.Indent(GetIndentationDepthByIndex(index));
			reusableTreeViewItem.SetExpandedWithoutNotify(IsExpandedByIndex(index));
			reusableTreeViewItem.SetToggleVisibility(HasChildrenByIndex(index));
		}
		base.InvokeBindItem(reusableItem, index);
	}

	internal override void InvokeDestroyItem(ReusableCollectionItem reusableItem)
	{
		if (reusableItem is ReusableTreeViewItem reusableTreeViewItem)
		{
			reusableTreeViewItem.onPointerUp -= OnItemPointerUp;
			reusableTreeViewItem.onToggleValueChanged -= OnToggleValueChanged;
		}
		base.InvokeDestroyItem(reusableItem);
	}

	internal void PostInitRegistration(ReusableTreeViewItem treeItem)
	{
		treeItem.onPointerUp += OnItemPointerUp;
		treeItem.onToggleValueChanged += OnToggleValueChanged;
	}

	private void OnItemPointerUp(PointerUpEvent evt)
	{
		if ((evt.modifiers & EventModifiers.Alt) == 0)
		{
			return;
		}
		VisualElement e = evt.currentTarget as VisualElement;
		Toggle toggle = e.Q<Toggle>(BaseTreeView.itemToggleUssClassName);
		int index = ((ReusableTreeViewItem)toggle.userData).index;
		if (!HasChildrenByIndex(index))
		{
			return;
		}
		bool flag = IsExpandedByIndex(index);
		if (IsViewDataKeyEnabled())
		{
			int idForIndex = GetIdForIndex(index);
			HashSet<int> hashSet = new HashSet<int>(baseTreeView.expandedItemIds);
			if (flag)
			{
				hashSet.Remove(idForIndex);
			}
			else
			{
				hashSet.Add(idForIndex);
			}
			IEnumerable<int> childrenIdsByIndex = GetChildrenIdsByIndex(index);
			foreach (int allItemId in GetAllItemIds(childrenIdsByIndex))
			{
				if (HasChildren(allItemId))
				{
					if (flag)
					{
						hashSet.Remove(allItemId);
					}
					else
					{
						hashSet.Add(allItemId);
					}
				}
			}
			baseTreeView.expandedItemIds = new List<int>(hashSet);
		}
		if (flag)
		{
			m_HierarchyViewModel.ClearFlagsRecursive(GetHierarchyNodeByIndex(index), HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Children);
		}
		else
		{
			m_HierarchyViewModel.SetFlagsRecursive(GetHierarchyNodeByIndex(index), HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Children);
		}
		UpdateHierarchy();
		baseTreeView.RefreshItems();
		RaiseItemExpandedChanged(GetIdForIndex(index), !flag, isAppliedToAllChildren: true);
		evt.StopPropagation();
	}

	private void RaiseItemExpandedChanged(int id, bool isExpanded, bool isAppliedToAllChildren)
	{
		this.itemExpandedChanged?.Invoke(new TreeViewExpansionChangedArgs
		{
			id = id,
			isExpanded = isExpanded,
			isAppliedToAllChildren = isAppliedToAllChildren
		});
	}

	private void OnToggleValueChanged(ChangeEvent<bool> evt)
	{
		Toggle toggle = evt.target as Toggle;
		int index = ((ReusableTreeViewItem)toggle.userData).index;
		if (IsExpandedByIndex(index))
		{
			CollapseItemByIndex(index, collapseAllChildren: false);
		}
		else
		{
			ExpandItemByIndex(index, expandAllChildren: false);
		}
		baseTreeView.scrollView.contentContainer.Focus();
	}

	public virtual int GetTreeItemsCount()
	{
		return m_Hierarchy.Count;
	}

	public override int GetIndexForId(int id)
	{
		HierarchyNode value;
		return m_IdToNodeDictionary.TryGetValue(id, out value) ? m_HierarchyViewModel.IndexOf(in value) : BaseTreeView.invalidId;
	}

	public override int GetIdForIndex(int index)
	{
		int count = m_HierarchyViewModel.Count;
		if (index == count && count > 0)
		{
			IHierarchyProperty<int> treeViewDataProperty = m_TreeViewDataProperty;
			HierarchyViewModel hierarchyViewModel = m_HierarchyViewModel;
			return treeViewDataProperty.GetValue(in hierarchyViewModel[hierarchyViewModel.Count - 1]);
		}
		return (!IsIndexValid(index)) ? BaseTreeView.invalidId : m_TreeViewDataProperty.GetValue(in m_HierarchyViewModel[index]);
	}

	public virtual bool HasChildren(int id)
	{
		if (m_IdToNodeDictionary.TryGetValue(id, out var value) && m_Hierarchy.Exists(in value))
		{
			return m_Hierarchy.GetChildrenCount(in value) > 0;
		}
		return false;
	}

	public bool Exists(int id)
	{
		return m_IdToNodeDictionary.ContainsKey(id);
	}

	public bool HasChildrenByIndex(int index)
	{
		if (!IsIndexValid(index))
		{
			return false;
		}
		return m_HierarchyViewModel.GetChildrenCount(in m_HierarchyViewModel[index]) > 0;
	}

	public IEnumerable<int> GetChildrenIdsByIndex(int index)
	{
		if (IsIndexValid(index))
		{
			HierarchyNodeChildren.Enumerator enumerator = m_Hierarchy.EnumerateChildren(in m_HierarchyViewModel[index]).GetEnumerator();
			while (enumerator.MoveNext())
			{
				HierarchyNode node = enumerator.Current;
				yield return m_TreeViewDataProperty.GetValue(in node);
			}
		}
	}

	public int GetChildIndexForId(int id)
	{
		if (m_IdToNodeDictionary.TryGetValue(id, out var value))
		{
			HierarchyNode lhs = m_Hierarchy.GetParent(in value);
			if (lhs == HierarchyNode.Null)
			{
				return BaseTreeView.invalidId;
			}
			HierarchyNodeChildren hierarchyNodeChildren = m_Hierarchy.EnumerateChildren(in lhs);
			int num = 0;
			HierarchyNodeChildren.Enumerator enumerator = hierarchyNodeChildren.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HierarchyNode lhs2 = enumerator.Current;
				if (lhs2 == value)
				{
					break;
				}
				num++;
			}
			return num;
		}
		return BaseTreeView.invalidId;
	}

	public int GetIndentationDepth(int id)
	{
		int num = 0;
		int parentId = GetParentId(id);
		while (parentId != BaseTreeView.invalidId)
		{
			parentId = GetParentId(parentId);
			num++;
		}
		return num;
	}

	public int GetIndentationDepthByIndex(int index)
	{
		int idForIndex = GetIdForIndex(index);
		return GetIndentationDepth(idForIndex);
	}

	public virtual bool CanChangeExpandedState(int id)
	{
		return true;
	}

	public bool IsExpanded(int id)
	{
		return m_IdToNodeDictionary.ContainsKey(id) && m_Hierarchy.Exists(m_IdToNodeDictionary[id]) && m_HierarchyViewModel.HasAllFlags(m_IdToNodeDictionary[id], HierarchyNodeFlags.Expanded);
	}

	public bool IsExpandedByIndex(int index)
	{
		if (!IsIndexValid(index))
		{
			return false;
		}
		return IsExpanded(GetIdForIndex(index));
	}

	public void ExpandItemByIndex(int index, bool expandAllChildren, bool refresh = true)
	{
		using (K_ExpandItemByIndex.Auto())
		{
			if (HasChildrenByIndex(index))
			{
				ExpandItemByNode(GetHierarchyNodeById(GetIdForIndex(index)), expandAllChildren, refresh);
			}
		}
	}

	public void ExpandItem(int id, bool expandAllChildren, bool refresh = true)
	{
		if (HasChildren(id) && CanChangeExpandedState(id) && m_IdToNodeDictionary.TryGetValue(id, out var value))
		{
			ExpandItemByNode(in value, expandAllChildren, refresh);
		}
	}

	public void CollapseItemByIndex(int index, bool collapseAllChildren, bool refresh = true)
	{
		if (HasChildrenByIndex(index))
		{
			CollapseItemByNode(GetHierarchyNodeById(GetIdForIndex(index)), collapseAllChildren, refresh);
		}
	}

	public void CollapseItem(int id, bool collapseAllChildren, bool refresh = true)
	{
		if (HasChildren(id) && CanChangeExpandedState(id) && m_IdToNodeDictionary.TryGetValue(id, out var value))
		{
			CollapseItemByNode(in value, collapseAllChildren, refresh);
		}
	}

	public void ExpandAll()
	{
		m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Expanded);
		UpdateHierarchy();
		if (IsViewDataKeyEnabled())
		{
			baseTreeView.expandedItemIds.Clear();
			HierarchyViewModelNodesEnumerable.Enumerator enumerator = m_HierarchyViewModel.EnumerateNodesWithAllFlags(HierarchyNodeFlags.Expanded).GetEnumerator();
			while (enumerator.MoveNext())
			{
				HierarchyNode node = enumerator.Current;
				baseTreeView.expandedItemIds.Add(m_TreeViewDataProperty.GetValue(in node));
			}
			baseTreeView.SaveViewData();
		}
		baseTreeView.RefreshItems();
		RaiseItemExpandedChanged(-1, isExpanded: true, isAppliedToAllChildren: true);
	}

	public void CollapseAll()
	{
		m_HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Expanded);
		UpdateHierarchy();
		if (IsViewDataKeyEnabled())
		{
			baseTreeView.expandedItemIds.Clear();
			baseTreeView.SaveViewData();
		}
		baseTreeView.RefreshItems();
		RaiseItemExpandedChanged(-1, isExpanded: false, isAppliedToAllChildren: true);
	}

	private void ExpandItemByNode(in HierarchyNode node, bool expandAllChildren, bool refresh)
	{
		int value = m_TreeViewDataProperty.GetValue(in node);
		if (!CanChangeExpandedState(value))
		{
			return;
		}
		if (expandAllChildren)
		{
			m_HierarchyViewModel.SetFlagsRecursive(in node, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Children);
		}
		else
		{
			m_HierarchyViewModel.SetFlags(in node, HierarchyNodeFlags.Expanded);
		}
		m_HierarchyHasPendingChanged = true;
		if (IsViewDataKeyEnabled())
		{
			HashSet<int> hashSet = new HashSet<int>(baseTreeView.expandedItemIds) { value };
			if (expandAllChildren)
			{
				UpdateHierarchy();
				IEnumerable<int> childrenIds = GetChildrenIds(value);
				foreach (int allItemId in GetAllItemIds(childrenIds))
				{
					hashSet.Add(allItemId);
				}
			}
			baseTreeView.expandedItemIds.Clear();
			baseTreeView.expandedItemIds.AddRange(hashSet);
			baseTreeView.SaveViewData();
		}
		if (refresh)
		{
			baseTreeView.RefreshItems();
		}
		RaiseItemExpandedChanged(value, isExpanded: true, expandAllChildren);
	}

	private void CollapseItemByNode(in HierarchyNode node, bool collapseAllChildren, bool refresh)
	{
		int value = m_TreeViewDataProperty.GetValue(in node);
		if (!CanChangeExpandedState(value))
		{
			return;
		}
		if (IsViewDataKeyEnabled())
		{
			if (collapseAllChildren)
			{
				IEnumerable<int> childrenIds = GetChildrenIds(value);
				foreach (int allItemId in GetAllItemIds(childrenIds))
				{
					baseTreeView.expandedItemIds.Remove(allItemId);
				}
			}
			baseTreeView.expandedItemIds.Remove(value);
			baseTreeView.SaveViewData();
		}
		if (collapseAllChildren)
		{
			m_HierarchyViewModel.ClearFlagsRecursive(in node, HierarchyNodeFlags.Expanded, HierarchyTraversalDirection.Children);
		}
		else
		{
			m_HierarchyViewModel.ClearFlags(in node, HierarchyNodeFlags.Expanded);
		}
		m_HierarchyHasPendingChanged = true;
		if (refresh)
		{
			baseTreeView.RefreshItems();
		}
		RaiseItemExpandedChanged(value, isExpanded: false, collapseAllChildren);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void GetExpandedItemIds(List<int> list)
	{
		if (list.Count > 0)
		{
			list.Clear();
		}
		if (IsViewDataKeyEnabled())
		{
			list.AddRange(baseTreeView.expandedItemIds);
		}
		HierarchyViewModelNodesEnumerable.Enumerator enumerator = m_HierarchyViewModel.EnumerateNodesWithAllFlags(HierarchyNodeFlags.Expanded).GetEnumerator();
		while (enumerator.MoveNext())
		{
			HierarchyNode node = enumerator.Current;
			list.Add(m_TreeViewDataProperty.GetValue(in node));
		}
	}

	internal bool IsViewDataKeyEnabled()
	{
		return baseTreeView.enableViewDataPersistence && !string.IsNullOrEmpty(baseTreeView.viewDataKey);
	}

	internal void ExpandAncestorNodes(in HierarchyNode node)
	{
		HierarchyNode lhs = m_Hierarchy.GetParent(in node);
		while (lhs != m_Hierarchy.Root)
		{
			HierarchyNode lhs2 = (lhs = GetHierarchyNodeById(m_TreeViewDataProperty.GetValue(in lhs)));
			if (!(lhs2 != m_Hierarchy.Root))
			{
				break;
			}
			int value = m_TreeViewDataProperty.GetValue(in lhs);
			if (!m_HierarchyViewModel.HasAllFlags(in lhs, HierarchyNodeFlags.Expanded) && CanChangeExpandedState(value))
			{
				if (IsViewDataKeyEnabled())
				{
					baseTreeView.expandedItemIds.Add(value);
				}
				m_HierarchyViewModel.SetFlags(in lhs, HierarchyNodeFlags.Expanded);
				m_HierarchyViewModel.Update();
			}
			lhs = m_Hierarchy.GetParent(in lhs);
		}
	}

	internal override void PreRefresh()
	{
		if (m_HierarchyHasPendingChanged)
		{
			UpdateHierarchy();
		}
	}

	private bool IsIndexValid(int index)
	{
		return index >= 0 && index < m_HierarchyViewModel.Count;
	}

	private bool IsChildOf(int childId, int id)
	{
		if (childId == BaseTreeView.invalidId || id == BaseTreeView.invalidId)
		{
			return false;
		}
		HierarchyNode rhs = GetHierarchyNodeById(childId);
		HierarchyNode lhs = GetHierarchyNodeById(id);
		if (lhs == rhs)
		{
			return true;
		}
		while (true)
		{
			HierarchyNode rhs2;
			HierarchyNode lhs2 = (rhs2 = m_Hierarchy.GetParent(in rhs));
			if (!(lhs2 != m_Hierarchy.Root))
			{
				break;
			}
			if (lhs == rhs2)
			{
				return true;
			}
			rhs = rhs2;
		}
		return false;
	}

	internal void RaiseItemParentChanged(int id, int newParentId)
	{
		RaiseItemIndexChanged(id, newParentId);
	}

	internal HierarchyNode CreateNode(in HierarchyNode parent)
	{
		return m_Hierarchy.Add((parent == HierarchyNode.Null) ? m_Hierarchy.Root : parent);
	}

	internal void UpdateIdToNodeDictionary(int id, in HierarchyNode node, bool isAdd = true)
	{
		m_HierarchyHasPendingChanged = true;
		if (isAdd)
		{
			m_TreeViewDataProperty.SetValue(in node, id);
			m_IdToNodeDictionary[id] = node;
		}
		else
		{
			m_IdToNodeDictionary.Remove(id);
		}
	}

	internal void RemoveAllChildrenItemsFromCollections(in HierarchyNode node, Action<HierarchyNode, int> removeCallback)
	{
		if (node == HierarchyNode.Null)
		{
			return;
		}
		int num = m_HierarchyFlattened.IndexOf(in node);
		if (num != -1)
		{
			int num2 = num + 1;
			int childrenCountRecursive = m_HierarchyFlattened.GetChildrenCountRecursive(in node);
			for (int i = num2; i < num2 + childrenCountRecursive; i++)
			{
				HierarchyFlattenedNode hierarchyFlattenedNode = m_HierarchyFlattened[i];
				removeCallback(hierarchyFlattenedNode.Node, m_TreeViewDataProperty.GetValue(hierarchyFlattenedNode.Node));
			}
		}
	}

	internal void ClearIdToNodeDictionary()
	{
		m_IdToNodeDictionary.Clear();
	}

	internal void UpdateSortOrder(in HierarchyNode newParent, in HierarchyNode insertedNode, int insertedIndex)
	{
		Span<HierarchyNode> span = m_Hierarchy.GetChildren(in newParent);
		if (insertedIndex == -1)
		{
			insertedIndex = span.Length;
		}
		int num = 0;
		for (int i = 0; i < insertedIndex && i < span.Length; i++)
		{
			if (!(insertedNode == span[i]))
			{
				m_Hierarchy.SetSortIndex(in span[i], num++);
			}
		}
		m_Hierarchy.SetSortIndex(in insertedNode, insertedIndex);
		if (insertedIndex == num)
		{
			num++;
		}
		for (int j = insertedIndex; j < span.Length; j++)
		{
			if (!(insertedNode == span[j]))
			{
				m_Hierarchy.SetSortIndex(in span[j], num++);
			}
		}
		m_Hierarchy.SortChildren(in newParent);
		UpdateHierarchy();
		Span<HierarchyNode> span2 = m_Hierarchy.GetChildren(in newParent);
		Span<HierarchyNode> span3 = span2;
		for (int k = 0; k < span3.Length; k++)
		{
			HierarchyNode node = span3[k];
			m_Hierarchy.SetSortIndex(in node, 0);
		}
	}

	internal void OnViewDataReadyUpdateNodes()
	{
		foreach (int expandedItemId in baseTreeView.expandedItemIds)
		{
			if (m_IdToNodeDictionary.TryGetValue(expandedItemId, out var value))
			{
				m_HierarchyViewModel.SetFlags(in value, HierarchyNodeFlags.Expanded);
			}
		}
		UpdateHierarchy();
	}

	internal void UpdateHierarchy()
	{
		if (m_Hierarchy.UpdateNeeded)
		{
			m_Hierarchy.Update();
		}
		if (m_HierarchyFlattened.UpdateNeeded)
		{
			m_HierarchyFlattened.Update();
		}
		if (m_HierarchyViewModel.UpdateNeeded)
		{
			m_HierarchyViewModel.Update();
		}
		m_HierarchyHasPendingChanged = false;
	}

	internal HierarchyNode GetHierarchyNodeById(int id)
	{
		HierarchyNode value;
		return (m_IdToNodeDictionary.TryGetValue(id, out value) && m_Hierarchy.Exists(in value)) ? value : HierarchyNode.Null;
	}

	internal HierarchyNode GetHierarchyNodeByIndex(int index)
	{
		if (!IsIndexValid(index))
		{
			return HierarchyNode.Null;
		}
		return m_HierarchyViewModel[index];
	}
}
