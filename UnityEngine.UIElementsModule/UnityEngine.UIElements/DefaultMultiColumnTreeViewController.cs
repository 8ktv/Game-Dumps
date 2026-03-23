using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;

namespace UnityEngine.UIElements;

public class DefaultMultiColumnTreeViewController<T> : MultiColumnTreeViewController, IDefaultTreeViewController<T>
{
	private TreeDataController<T> m_TreeDataController;

	private TreeDataController<T> treeDataController => m_TreeDataController ?? (m_TreeDataController = new TreeDataController<T>());

	public override IList itemsSource
	{
		get
		{
			return base.itemsSource;
		}
		set
		{
			if (value == null)
			{
				SetRootItems(null);
			}
			else if (value is IList<TreeViewItemData<T>> rootItems)
			{
				SetRootItems(rootItems);
			}
			else
			{
				Debug.LogError($"Type does not match this tree view controller's data type ({typeof(T)}).");
			}
		}
	}

	public DefaultMultiColumnTreeViewController(Columns columns, SortColumnDescriptions sortDescriptions, List<SortColumnDescription> sortedColumns)
		: base(columns, sortDescriptions, sortedColumns)
	{
	}

	public void SetRootItems(IList<TreeViewItemData<T>> items)
	{
		if (items == base.itemsSource)
		{
			return;
		}
		if (m_Hierarchy.IsCreated)
		{
			ClearIdToNodeDictionary();
			treeDataController.ClearNodeToDataDictionary();
			base.hierarchy = new Hierarchy();
		}
		if (items != null)
		{
			treeDataController.ConvertTreeViewItemDataToHierarchy(items, (HierarchyNode node) => CreateNode(in node), delegate(int id, HierarchyNode node)
			{
				UpdateIdToNodeDictionary(id, in node);
			});
			UpdateHierarchy();
			if (base.baseTreeView.autoExpand)
			{
				m_HierarchyViewModel.SetFlags(HierarchyNodeFlags.Expanded);
				UpdateHierarchy();
			}
			if (IsViewDataKeyEnabled())
			{
				OnViewDataReadyUpdateNodes();
			}
		}
		SetHierarchyViewModelWithoutNotify(m_HierarchyViewModel);
		RaiseItemsSourceChanged();
	}

	public virtual void AddItem(in TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree = true)
	{
		HierarchyNode node;
		if (parentId == BaseTreeView.invalidId)
		{
			node = CreateNode(in HierarchyNode.Null);
		}
		else
		{
			HierarchyNode parent = GetHierarchyNodeById(parentId);
			node = CreateNode(in parent);
			TreeViewItemData<T> treeItemDataForNode = treeDataController.GetTreeItemDataForNode(parent);
			if (treeItemDataForNode.data != null)
			{
				treeItemDataForNode.InsertChild(item, childIndex);
			}
		}
		treeDataController.AddItem(in item, node);
		UpdateIdToNodeDictionary(item.id, in node);
		if (item.children.GetCount() > 0)
		{
			HierarchyNode parentNode = GetHierarchyNodeById(item.id);
			treeDataController.ConvertTreeViewItemDataToHierarchy(item.children, (HierarchyNode itemNode) => CreateNode((itemNode == HierarchyNode.Null) ? parentNode : itemNode), delegate(int id, HierarchyNode newNode)
			{
				UpdateIdToNodeDictionary(id, in newNode);
			});
		}
		if (base.baseTreeView.autoExpand)
		{
			ExpandAncestorNodes(in node);
		}
		if (childIndex != -1)
		{
			UpdateSortOrder(m_Hierarchy.GetParent(in node), in node, childIndex);
		}
		if (rebuildTree)
		{
			base.baseTreeView.RefreshItems();
		}
	}

	public virtual TreeViewItemData<T> GetTreeViewItemDataForId(int id)
	{
		return treeDataController.GetTreeItemDataForNode(GetHierarchyNodeById(id));
	}

	public virtual TreeViewItemData<T> GetTreeViewItemDataForIndex(int index)
	{
		int idForIndex = GetIdForIndex(index);
		return treeDataController.GetTreeItemDataForNode(GetHierarchyNodeById(idForIndex));
	}

	public override bool TryRemoveItem(int id, bool rebuildTree = true)
	{
		HierarchyNode node = GetHierarchyNodeById(id);
		if (node != HierarchyNode.Null)
		{
			int parentId = GetParentId(id);
			if (parentId != BaseTreeView.invalidId)
			{
				TreeViewItemData<T> treeItemDataForNode = treeDataController.GetTreeItemDataForNode(GetHierarchyNodeById(parentId));
				if (treeItemDataForNode.data != null)
				{
					treeItemDataForNode.RemoveChild(id);
				}
			}
			RemoveAllChildrenItemsFromCollections(in node, delegate(HierarchyNode hierarchyNode, int itemId)
			{
				treeDataController.RemoveItem(hierarchyNode);
				UpdateIdToNodeDictionary(itemId, in node, isAdd: false);
			});
			treeDataController.RemoveItem(node);
			UpdateIdToNodeDictionary(id, in node, isAdd: false);
			m_Hierarchy.Remove(in node);
			if (rebuildTree)
			{
				base.baseTreeView.RefreshItems();
			}
			return true;
		}
		return false;
	}

	public override object GetItemForId(int id)
	{
		return treeDataController.GetTreeItemDataForNode(GetHierarchyNodeById(id)).data;
	}

	public T GetDataForId(int id)
	{
		return treeDataController.GetDataForNode(GetHierarchyNodeById(id));
	}

	public T GetDataForIndex(int index)
	{
		return treeDataController.GetDataForNode(GetHierarchyNodeByIndex(index));
	}

	public override object GetItemForIndex(int index)
	{
		return treeDataController.GetDataForNode(GetHierarchyNodeByIndex(index));
	}
}
