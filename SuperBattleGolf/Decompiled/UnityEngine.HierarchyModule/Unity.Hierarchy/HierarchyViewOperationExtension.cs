using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
internal static class HierarchyViewOperationExtension
{
	public static void OnCut(this HierarchyView view)
	{
		foreach (HierarchyNodeTypeHandler item in view.Source.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler && hierarchyEditorNodeTypeHandler.CanCut(view))
			{
				hierarchyEditorNodeTypeHandler.OnCut(view);
			}
		}
	}

	public static void OnCopy(this HierarchyView view)
	{
		foreach (HierarchyNodeTypeHandler item in view.Source.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler && hierarchyEditorNodeTypeHandler.CanCopy(view))
			{
				hierarchyEditorNodeTypeHandler.OnCopy(view);
			}
		}
	}

	public static void OnPaste(this HierarchyView view)
	{
		foreach (HierarchyNodeTypeHandler item in view.Source.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler && hierarchyEditorNodeTypeHandler.CanPaste(view))
			{
				hierarchyEditorNodeTypeHandler.OnPaste(view);
			}
		}
	}

	public static void OnPasteAsChild(this HierarchyView view, bool keepWorldPos)
	{
		foreach (HierarchyNodeTypeHandler item in view.Source.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler && hierarchyEditorNodeTypeHandler.CanPasteAsChild(view))
			{
				hierarchyEditorNodeTypeHandler.OnPasteAsChild(view, keepWorldPos);
			}
		}
	}

	public static void OnDuplicate(this HierarchyView view)
	{
		foreach (HierarchyNodeTypeHandler item in view.Source.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler && hierarchyEditorNodeTypeHandler.CanDuplicate(view))
			{
				hierarchyEditorNodeTypeHandler.OnDuplicate(view);
			}
		}
	}

	public static void OnDelete(this HierarchyView view)
	{
		foreach (HierarchyNodeTypeHandler item in view.Source.EnumerateNodeTypeHandlers())
		{
			if (item is IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler && hierarchyEditorNodeTypeHandler.CanDelete(view))
			{
				hierarchyEditorNodeTypeHandler.OnDelete(view);
			}
		}
	}

	public static void OnSetName(this HierarchyView view, in HierarchyNode node)
	{
		view.BeginRename(in node);
	}

	public static HierarchyViewItem GetHierarchyViewItemForNode(this HierarchyView view, in HierarchyNode node)
	{
		if (node == HierarchyNode.Null)
		{
			return null;
		}
		int num = view.ViewModel.IndexOf(in node);
		if (num < 0)
		{
			return null;
		}
		return view.ListView.GetRootElementForIndex(num)?.Q<HierarchyViewItem>();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal static bool DoesSelectedNodesHaveChildren(this HierarchyView view)
	{
		HierarchyViewModel viewModel = view.ViewModel;
		HierarchyViewModelNodesEnumerable.Enumerator enumerator = viewModel.EnumerateNodesWithAllFlags(HierarchyNodeFlags.Selected).GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (viewModel.GetChildrenCount(in enumerator.Current) > 0)
			{
				return true;
			}
		}
		return false;
	}
}
