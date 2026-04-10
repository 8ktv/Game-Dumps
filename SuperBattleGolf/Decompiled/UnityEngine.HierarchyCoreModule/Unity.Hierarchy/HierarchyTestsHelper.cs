using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[NativeHeader("Modules/HierarchyCore/HierarchyTestsHelper.h")]
[NativeHeader("Modules/HierarchyCore/HierarchyTestsHelperBindings.h")]
internal static class HierarchyTestsHelper
{
	[NativeHeader("Modules/HierarchyCore/HierarchyTestsHelper.h")]
	internal enum SortOrder
	{
		Ascending,
		Descending
	}

	internal delegate void ForEachCallback(in HierarchyNode node, int index);

	internal static int GenerateNodesTree(Hierarchy hierarchy, in HierarchyNode root, int width, int depth, int maxCount = 0)
	{
		return GenerateNodesTreeHierarchy(hierarchy, in root, width, depth, maxCount);
	}

	internal static int GenerateNodesTree(HierarchyNodeTypeHandlerBase handler, in HierarchyNode root, int width, int depth, int maxCount = 0)
	{
		return GenerateNodesTreeHandler(handler, in root, width, depth, maxCount);
	}

	internal static void GenerateNodesCount(Hierarchy hierarchy, in HierarchyNode root, int count, int width, int depth)
	{
		GenerateNodesCountHierarchy(hierarchy, in root, count, width, depth);
	}

	internal static void GenerateNodesCount(HierarchyNodeTypeHandlerBase handler, in HierarchyNode root, int count, int width, int depth)
	{
		GenerateNodesCountHandler(handler, in root, count, width, depth);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	internal static void GenerateSortIndexRecursive(Hierarchy hierarchy, in HierarchyNode root, SortOrder order)
	{
		GenerateSortIndexRecursive_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in root, order);
	}

	internal unsafe static void ForEachRecursive(Hierarchy hierarchy, in HierarchyNode root, ForEachCallback func)
	{
		Stack<HierarchyNode> stack = new Stack<HierarchyNode>();
		stack.Push(root);
		using NativeArray<HierarchyNode> nativeArray = new NativeArray<HierarchyNode>(hierarchy.Count, Allocator.Temp);
		while (stack.Count > 0)
		{
			HierarchyNode node = stack.Pop();
			int childrenCount = hierarchy.GetChildrenCount(in node);
			Span<HierarchyNode> outChildren = new Span<HierarchyNode>(nativeArray.GetUnsafePtr(), childrenCount);
			int children = hierarchy.GetChildren(in node, outChildren);
			if (children != childrenCount)
			{
				throw new InvalidOperationException($"Expected GetChildren to return {childrenCount}, but was {children}.");
			}
			int i = 0;
			for (int length = outChildren.Length; i < length; i++)
			{
				HierarchyNode node2 = outChildren[i];
				func(in node2, i);
				stack.Push(node2);
			}
		}
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static byte[] GenerateInvalidViewModelState_BadIndices()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		byte[] result;
		try
		{
			GenerateInvalidViewModelState_BadIndices_Injected(out ret);
		}
		finally
		{
			byte[] array = default(byte[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static void SetNextHierarchyNodeId(Hierarchy hierarchy, int id)
	{
		SetNextHierarchyNodeId_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), id);
	}

	internal static int GetNodeType<T>() where T : HierarchyNodeTypeHandlerBase
	{
		return GetNodeType(typeof(T));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	internal static extern int GetNodeType(Type type);

	[NativeMethod(IsThreadSafe = true)]
	internal static int[] GetRegisteredNodeTypes(Hierarchy hierarchy)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		int[] result;
		try
		{
			GetRegisteredNodeTypes_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), out ret);
		}
		finally
		{
			int[] array = default(int[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static int GetCapacity(Hierarchy hierarchy)
	{
		return GetCapacity_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy));
	}

	internal static int GetVersion(Hierarchy hierarchy)
	{
		return GetHierarchyVersion(hierarchy);
	}

	internal static int GetVersion(Hierarchy hierarchy, in HierarchyNode node)
	{
		return GetHierarchyNodeVersion(hierarchy, in node);
	}

	internal static int GetVersion(HierarchyFlattened hierarchyFlattened)
	{
		return GetHierarchyFlattenedVersion(hierarchyFlattened);
	}

	internal static int GetVersion(HierarchyViewModel hierarchyViewModel)
	{
		return GetHierarchyViewModelVersion(hierarchyViewModel);
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static int GetChildrenCapacity(Hierarchy hierarchy, in HierarchyNode node)
	{
		return GetChildrenCapacity_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in node);
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static bool CompareNodeSortIndex(Hierarchy hierarchy, in HierarchyNode a, in HierarchyNode b)
	{
		return CompareNodeSortIndex_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in a, in b);
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static object GetHierarchyScriptingObject(Hierarchy hierarchy)
	{
		return GetHierarchyScriptingObject_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy));
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static object GetHierarchyFlattenedScriptingObject(HierarchyFlattened hierarchyFlattened)
	{
		return GetHierarchyFlattenedScriptingObject_Injected((hierarchyFlattened == null) ? ((IntPtr)0) : HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(hierarchyFlattened));
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static object GetHierarchyViewModelScriptingObject(HierarchyViewModel viewModel)
	{
		return GetHierarchyViewModelScriptingObject_Injected((viewModel == null) ? ((IntPtr)0) : HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(viewModel));
	}

	[NativeMethod(IsThreadSafe = true)]
	internal static object GetHierarchyCommandListScriptingObject(HierarchyCommandList cmdList)
	{
		return GetHierarchyCommandListScriptingObject_Injected((cmdList == null) ? ((IntPtr)0) : HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(cmdList));
	}

	[FreeFunction("HierarchyTestsHelperBindings::GenerateNodesTreeHierarchy", IsThreadSafe = true, ThrowsException = true)]
	private static int GenerateNodesTreeHierarchy(Hierarchy hierarchy, in HierarchyNode root, int width, int depth, int maxCount)
	{
		return GenerateNodesTreeHierarchy_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in root, width, depth, maxCount);
	}

	[FreeFunction("HierarchyTestsHelperBindings::GenerateNodesTreeHandler", IsThreadSafe = true, ThrowsException = true)]
	private static int GenerateNodesTreeHandler(HierarchyNodeTypeHandlerBase handler, in HierarchyNode root, int width, int depth, int maxCount)
	{
		return GenerateNodesTreeHandler_Injected((handler == null) ? ((IntPtr)0) : HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(handler), in root, width, depth, maxCount);
	}

	[FreeFunction("HierarchyTestsHelperBindings::GenerateNodesCountHierarchy", IsThreadSafe = true, ThrowsException = true)]
	private static void GenerateNodesCountHierarchy(Hierarchy hierarchy, in HierarchyNode root, int count, int width, int depth)
	{
		GenerateNodesCountHierarchy_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in root, count, width, depth);
	}

	[FreeFunction("HierarchyTestsHelperBindings::GenerateNodesCountHandler", IsThreadSafe = true, ThrowsException = true)]
	private static void GenerateNodesCountHandler(HierarchyNodeTypeHandlerBase handler, in HierarchyNode root, int count, int width, int depth)
	{
		GenerateNodesCountHandler_Injected((handler == null) ? ((IntPtr)0) : HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(handler), in root, count, width, depth);
	}

	[FreeFunction("HierarchyTestsHelperBindings::GetHierarchyVersion", IsThreadSafe = true)]
	private static int GetHierarchyVersion(Hierarchy hierarchy)
	{
		return GetHierarchyVersion_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy));
	}

	[FreeFunction("HierarchyTestsHelperBindings::GetHierarchyNodeVersion", IsThreadSafe = true)]
	private static int GetHierarchyNodeVersion(Hierarchy hierarchy, in HierarchyNode node)
	{
		return GetHierarchyNodeVersion_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), in node);
	}

	[FreeFunction("HierarchyTestsHelperBindings::GetHierarchyFlattenedVersion", IsThreadSafe = true)]
	private static int GetHierarchyFlattenedVersion(HierarchyFlattened hierarchyFlattened)
	{
		return GetHierarchyFlattenedVersion_Injected((hierarchyFlattened == null) ? ((IntPtr)0) : HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(hierarchyFlattened));
	}

	[FreeFunction("HierarchyTestsHelperBindings::GetHierarchyViewModelVersion", IsThreadSafe = true)]
	private static int GetHierarchyViewModelVersion(HierarchyViewModel hierarchyViewModel)
	{
		return GetHierarchyViewModelVersion_Injected((hierarchyViewModel == null) ? ((IntPtr)0) : HierarchyViewModel.BindingsMarshaller.ConvertToUnmanaged(hierarchyViewModel));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateSortIndexRecursive_Injected(IntPtr hierarchy, in HierarchyNode root, SortOrder order);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateInvalidViewModelState_BadIndices_Injected(out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetNextHierarchyNodeId_Injected(IntPtr hierarchy, int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRegisteredNodeTypes_Injected(IntPtr hierarchy, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCapacity_Injected(IntPtr hierarchy);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildrenCapacity_Injected(IntPtr hierarchy, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CompareNodeSortIndex_Injected(IntPtr hierarchy, in HierarchyNode a, in HierarchyNode b);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetHierarchyScriptingObject_Injected(IntPtr hierarchy);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetHierarchyFlattenedScriptingObject_Injected(IntPtr hierarchyFlattened);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetHierarchyViewModelScriptingObject_Injected(IntPtr viewModel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetHierarchyCommandListScriptingObject_Injected(IntPtr cmdList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GenerateNodesTreeHierarchy_Injected(IntPtr hierarchy, in HierarchyNode root, int width, int depth, int maxCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GenerateNodesTreeHandler_Injected(IntPtr handler, in HierarchyNode root, int width, int depth, int maxCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateNodesCountHierarchy_Injected(IntPtr hierarchy, in HierarchyNode root, int count, int width, int depth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateNodesCountHandler_Injected(IntPtr handler, in HierarchyNode root, int count, int width, int depth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetHierarchyVersion_Injected(IntPtr hierarchy);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetHierarchyNodeVersion_Injected(IntPtr hierarchy, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetHierarchyFlattenedVersion_Injected(IntPtr hierarchyFlattened);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetHierarchyViewModelVersion_Injected(IntPtr hierarchyViewModel);
}
