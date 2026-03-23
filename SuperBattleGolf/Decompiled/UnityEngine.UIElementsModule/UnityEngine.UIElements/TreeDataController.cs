using System;
using System.Collections.Generic;
using Unity.Hierarchy;

namespace UnityEngine.UIElements;

internal sealed class TreeDataController<T>
{
	private Dictionary<HierarchyNode, TreeViewItemData<T>> m_NodeToItemDataDictionary = new Dictionary<HierarchyNode, TreeViewItemData<T>>();

	private Stack<IEnumerator<TreeViewItemData<T>>> m_ItemStack = new Stack<IEnumerator<TreeViewItemData<T>>>();

	private Stack<HierarchyNode> m_NodeStack = new Stack<HierarchyNode>();

	public void AddItem(in TreeViewItemData<T> item, HierarchyNode node)
	{
		m_NodeToItemDataDictionary.TryAdd(node, item);
	}

	public void RemoveItem(HierarchyNode node)
	{
		m_NodeToItemDataDictionary.Remove(node);
	}

	public TreeViewItemData<T> GetTreeItemDataForNode(HierarchyNode node)
	{
		if (m_NodeToItemDataDictionary.TryGetValue(node, out var value))
		{
			return value;
		}
		return default(TreeViewItemData<T>);
	}

	public T GetDataForNode(HierarchyNode node)
	{
		if (m_NodeToItemDataDictionary.TryGetValue(node, out var value))
		{
			return value.data;
		}
		return default(T);
	}

	internal void ConvertTreeViewItemDataToHierarchy(IEnumerable<TreeViewItemData<T>> list, Func<HierarchyNode, HierarchyNode> createNode, Action<int, HierarchyNode> updateDictionary)
	{
		if (list == null)
		{
			return;
		}
		m_ItemStack.Clear();
		m_NodeStack.Clear();
		IEnumerator<TreeViewItemData<T>> enumerator = list.GetEnumerator();
		HierarchyNode hierarchyNode = HierarchyNode.Null;
		while (true)
		{
			if (!enumerator.MoveNext())
			{
				if (m_ItemStack.Count > 0)
				{
					hierarchyNode = m_NodeStack.Pop();
					enumerator = m_ItemStack.Pop();
					continue;
				}
				break;
			}
			TreeViewItemData<T> current = enumerator.Current;
			HierarchyNode hierarchyNode2 = createNode(hierarchyNode);
			UpdateNodeToDataDictionary(hierarchyNode2, current);
			updateDictionary(current.id, hierarchyNode2);
			if (current.children != null && ((IList<TreeViewItemData<T>>)current.children).Count > 0)
			{
				m_NodeStack.Push(hierarchyNode);
				hierarchyNode = hierarchyNode2;
				m_ItemStack.Push(enumerator);
				enumerator = current.children.GetEnumerator();
			}
		}
	}

	internal void UpdateNodeToDataDictionary(HierarchyNode node, TreeViewItemData<T> item)
	{
		m_NodeToItemDataDictionary.TryAdd(node, item);
	}

	internal void ClearNodeToDataDictionary()
	{
		m_NodeToItemDataDictionary.Clear();
	}
}
