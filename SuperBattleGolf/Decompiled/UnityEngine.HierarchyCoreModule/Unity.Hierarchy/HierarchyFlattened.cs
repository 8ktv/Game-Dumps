using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/HierarchyCore/HierarchyFlattenedBindings.h")]
[NativeHeader("Modules/HierarchyCore/Public/HierarchyFlattened.h")]
[RequiredByNativeCode]
public sealed class HierarchyFlattened : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToUnmanaged(HierarchyFlattened hierarchyFlattened)
		{
			return hierarchyFlattened.m_Ptr;
		}
	}

	public struct Enumerator
	{
		private readonly HierarchyFlattened m_HierarchyFlattened;

		private readonly ReadOnlyNativeVector<HierarchyFlattenedNode> m_FlattenedNodes;

		private readonly int m_Version;

		private int m_Index;

		public ref readonly HierarchyFlattenedNode Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (m_Version != m_HierarchyFlattened.m_Version)
				{
					throw new InvalidOperationException("HierarchyFlattened was modified.");
				}
				return ref m_FlattenedNodes[m_Index];
			}
		}

		internal Enumerator(HierarchyFlattened hierarchyFlattened)
		{
			m_HierarchyFlattened = hierarchyFlattened;
			m_FlattenedNodes = hierarchyFlattened.m_FlattenedNodes;
			m_Version = hierarchyFlattened.m_Version;
			m_Index = -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			return ++m_Index < m_FlattenedNodes.Count;
		}
	}

	private IntPtr m_Ptr;

	internal readonly Hierarchy m_Hierarchy;

	private ReadOnlyNativeVector<HierarchyFlattenedNode> m_FlattenedNodes;

	private int m_Version;

	private readonly bool m_IsOwner;

	public bool IsCreated => m_Ptr != IntPtr.Zero;

	public int Count => m_FlattenedNodes.Count;

	public bool Updating
	{
		[NativeMethod("Updating", IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_Updating_Injected(intPtr);
		}
	}

	public bool UpdateNeeded
	{
		[NativeMethod("UpdateNeeded", IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_UpdateNeeded_Injected(intPtr);
		}
	}

	internal ReadOnlyNativeVector<HierarchyFlattenedNode> FlattenedNodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_FlattenedNodes;
		}
	}

	internal int Version
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_Version;
		}
	}

	public ref readonly HierarchyFlattenedNode this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ref m_FlattenedNodes[index];
		}
	}

	[Obsolete("The Hierarchy property will be removed in the future, remove its usage from your code.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Hierarchy Hierarchy => m_Hierarchy;

	public HierarchyFlattened(Hierarchy hierarchy)
	{
		m_Ptr = Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), hierarchy, out var nodesPtr, out var nodesCount, out var version);
		m_Hierarchy = hierarchy;
		m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(nodesPtr, nodesCount);
		m_Version = version;
		m_IsOwner = true;
	}

	private HierarchyFlattened(IntPtr nativePtr, Hierarchy hierarchy, IntPtr flattenedNodesPtr, int flattenedNodesCount, int version)
	{
		m_Ptr = nativePtr;
		m_Hierarchy = hierarchy;
		m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(flattenedNodesPtr, flattenedNodesCount);
		m_Version = version;
		m_IsOwner = false;
	}

	~HierarchyFlattened()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (m_Ptr != IntPtr.Zero)
		{
			if (m_IsOwner)
			{
				Destroy(m_Ptr);
			}
			m_Ptr = IntPtr.Zero;
		}
		m_FlattenedNodes = default(ReadOnlyNativeVector<HierarchyFlattenedNode>);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int IndexOf(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IndexOf_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public bool Contains(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Contains_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public HierarchyNode GetParent(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetParent_Injected(intPtr, in node, out var ret);
		return ret;
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public HierarchyNode GetNextSibling(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetNextSibling_Injected(intPtr, in node, out var ret);
		return ret;
	}

	public HierarchyFlattenedChildrenEnumerable EnumerateChildren(in HierarchyNode node)
	{
		return new HierarchyFlattenedChildrenEnumerable(this, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int GetChildrenCount(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetChildrenCount_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int GetChildrenCountRecursive(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetChildrenCountRecursive_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public HierarchyNode GetChild(in HierarchyNode node, int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetChild_Injected(intPtr, in node, index, out var ret);
		return ret;
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int GetChildIndex(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetChildIndex_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int GetDepth(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDepth_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true)]
	public void Update()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Update_Injected(intPtr);
	}

	[NativeMethod(IsThreadSafe = true)]
	public bool UpdateIncremental()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return UpdateIncremental_Injected(intPtr);
	}

	[NativeMethod(IsThreadSafe = true)]
	public bool UpdateIncrementalTimed(double milliseconds)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return UpdateIncrementalTimed_Injected(intPtr, milliseconds);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public ReadOnlySpan<HierarchyFlattenedNode> AsReadOnlySpan()
	{
		return m_FlattenedNodes.AsReadOnlySpan();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static HierarchyFlattened FromIntPtr(IntPtr handlePtr)
	{
		return (handlePtr != IntPtr.Zero) ? ((HierarchyFlattened)GCHandle.FromIntPtr(handlePtr).Target) : null;
	}

	[FreeFunction("HierarchyFlattenedBindings::Create", IsThreadSafe = true)]
	private static IntPtr Create(IntPtr handlePtr, Hierarchy hierarchy, out IntPtr nodesPtr, out int nodesCount, out int version)
	{
		return Create_Injected(handlePtr, (hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToUnmanaged(hierarchy), out nodesPtr, out nodesCount, out version);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("HierarchyFlattenedBindings::Destroy", IsThreadSafe = true)]
	private static extern void Destroy(IntPtr nativePtr);

	[RequiredByNativeCode]
	private static IntPtr CreateHierarchyFlattened(IntPtr nativePtr, IntPtr hierarchyPtr, IntPtr flattenedNodesPtr, int flattenedNodesCount, int version)
	{
		return GCHandle.ToIntPtr(GCHandle.Alloc(new HierarchyFlattened(nativePtr, Hierarchy.FromIntPtr(hierarchyPtr), flattenedNodesPtr, flattenedNodesCount, version)));
	}

	[RequiredByNativeCode]
	private static void UpdateHierarchyFlattened(IntPtr handlePtr, IntPtr flattenedNodesPtr, int flattenedNodesCount, int version)
	{
		HierarchyFlattened hierarchyFlattened = FromIntPtr(handlePtr);
		hierarchyFlattened.m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(flattenedNodesPtr, flattenedNodesCount);
		hierarchyFlattened.m_Version = version;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_Updating_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_UpdateNeeded_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int IndexOf_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Contains_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetParent_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNextSibling_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildrenCount_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildrenCountRecursive_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetChild_Injected(IntPtr _unity_self, in HierarchyNode node, int index, out HierarchyNode ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildIndex_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDepth_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Update_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UpdateIncremental_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UpdateIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create_Injected(IntPtr handlePtr, IntPtr hierarchy, out IntPtr nodesPtr, out int nodesCount, out int version);
}
