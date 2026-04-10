using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy;

[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
[NativeHeader("Modules/HierarchyCore/Public/HierarchyNodeTypeHandlerBase.h")]
[NativeHeader("Modules/HierarchyCore/Public/Hierarchy.h")]
[NativeHeader("Modules/HierarchyCore/HierarchyBindings.h")]
public sealed class Hierarchy : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToUnmanaged(Hierarchy hierarchy)
		{
			return hierarchy.m_Ptr;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	internal delegate void HandlerCreatedEventHandler(HierarchyNodeTypeHandlerBase handler);

	private IntPtr m_Ptr;

	private readonly IntPtr m_RootPtr;

	private readonly IntPtr m_VersionPtr;

	private readonly bool m_IsOwner;

	public bool IsCreated => m_Ptr != IntPtr.Zero;

	public unsafe ref readonly HierarchyNode Root
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ref *(HierarchyNode*)(void*)m_RootPtr;
		}
	}

	public int Count
	{
		[NativeMethod("Count", IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_Count_Injected(intPtr);
		}
	}

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

	internal unsafe int Version
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return *(int*)(void*)m_VersionPtr;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	internal event HandlerCreatedEventHandler HandlerCreated;

	public Hierarchy()
	{
		m_Ptr = Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), out var rootPtr, out var versionPtr);
		m_RootPtr = rootPtr;
		m_VersionPtr = versionPtr;
		m_IsOwner = true;
	}

	private Hierarchy(IntPtr nativePtr, IntPtr rootPtr, IntPtr versionPtr)
	{
		m_Ptr = nativePtr;
		m_RootPtr = rootPtr;
		m_VersionPtr = versionPtr;
		m_IsOwner = false;
	}

	~Hierarchy()
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
	}

	public T GetOrCreateNodeTypeHandler<T>() where T : HierarchyNodeTypeHandlerBase
	{
		return (T)HierarchyNodeTypeHandlerBase.FromIntPtr(GetOrCreateNodeTypeHandler(typeof(T)));
	}

	public T GetNodeTypeHandlerBase<T>() where T : HierarchyNodeTypeHandlerBase
	{
		return (T)HierarchyNodeTypeHandlerBase.FromIntPtr(GetNodeTypeHandlerFromType(typeof(T)));
	}

	public HierarchyNodeTypeHandlerBase GetNodeTypeHandlerBase(in HierarchyNode node)
	{
		return HierarchyNodeTypeHandlerBase.FromIntPtr(GetNodeTypeHandlerFromNode(in node));
	}

	public HierarchyNodeTypeHandlerBase GetNodeTypeHandlerBase(string nodeTypeName)
	{
		return HierarchyNodeTypeHandlerBase.FromIntPtr(GetNodeTypeHandlerFromName(nodeTypeName));
	}

	public HierarchyNodeTypeHandlerBaseEnumerable EnumerateNodeTypeHandlersBase()
	{
		return new HierarchyNodeTypeHandlerBaseEnumerable(this);
	}

	public HierarchyNodeType GetNodeType<T>() where T : HierarchyNodeTypeHandlerBase
	{
		return GetNodeTypeFromType(typeof(T));
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public HierarchyNodeType GetNodeType(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetNodeType_Injected(intPtr, in node, out var ret);
		return ret;
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public void Reserve(int count)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Reserve_Injected(intPtr, count);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public void ReserveChildren(in HierarchyNode node, int count)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReserveChildren_Injected(intPtr, in node, count);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public bool Exists(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Exists_Injected(intPtr, in node);
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

	public HierarchyNode Add(in HierarchyNode parent)
	{
		return AddNode(in parent);
	}

	public HierarchyNode[] Add(in HierarchyNode parent, int count)
	{
		if (count < 0)
		{
			throw new ArgumentException(string.Format("{0} must be positive, but was {1}", "count", count));
		}
		HierarchyNode[] array = new HierarchyNode[count];
		AddNodeSpan(in parent, array);
		return array;
	}

	public void Add(in HierarchyNode parent, Span<HierarchyNode> outNodes)
	{
		AddNodeSpan(in parent, outNodes);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public bool Remove(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Remove_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public void RemoveChildren(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveChildren_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true)]
	public void Clear()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Clear_Injected(intPtr);
	}

	public void SetParent(in HierarchyNode node, in HierarchyNode parent)
	{
		SetNodeParent(in node, in parent);
	}

	public void SetParent(in HierarchyNode node, in HierarchyNode parent, int index)
	{
		SetNodeParentAt(in node, in parent, index);
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
	public HierarchyNode[] GetChildren(in HierarchyNode node)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		HierarchyNode[] result;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetChildren_Injected(intPtr, in node, out ret);
		}
		finally
		{
			HierarchyNode[] array = default(HierarchyNode[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public int GetChildren(in HierarchyNode node, Span<HierarchyNode> outChildren)
	{
		return GetNodeChildrenSpan(in node, outChildren);
	}

	public HierarchyNodeChildren EnumerateChildren(in HierarchyNode node)
	{
		return new HierarchyNodeChildren(this, EnumerateChildrenPtr(in node));
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
	public void SetSortIndex(in HierarchyNode node, int sortIndex)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetSortIndex_Injected(intPtr, in node, sortIndex);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int GetSortIndex(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSortIndex_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public void SortChildren(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SortChildren_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public void SortChildrenRecursive(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SortChildrenRecursive_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public void SetChildrenNeedsSorting(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetChildrenNeedsSorting_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public bool DoesChildrenNeedsSorting(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return DoesChildrenNeedsSorting_Injected(intPtr, in node);
	}

	public HierarchyPropertyUnmanaged<T> GetOrCreatePropertyUnmanaged<T>(string name, HierarchyPropertyStorageType type = HierarchyPropertyStorageType.Dense) where T : unmanaged
	{
		HierarchyPropertyDescriptor descriptor = new HierarchyPropertyDescriptor
		{
			Size = UnsafeUtility.SizeOf<T>(),
			Type = type
		};
		return new HierarchyPropertyUnmanaged<T>(this, GetOrCreateProperty(name, in descriptor));
	}

	public HierarchyPropertyString GetOrCreatePropertyString(string name)
	{
		HierarchyPropertyDescriptor descriptor = new HierarchyPropertyDescriptor
		{
			Size = 0,
			Type = HierarchyPropertyStorageType.Blob
		};
		return new HierarchyPropertyString(this, GetOrCreateProperty(name, in descriptor));
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public unsafe void SetName(in HierarchyNode node, string name)
	{
		//The blocks IL_003a are reachable both inside and outside the pinned region starting at IL_0029. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetName_Injected(intPtr, in node, ref managedSpanWrapper);
					return;
				}
			}
			SetName_Injected(intPtr, in node, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public string GetName(in HierarchyNode node)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetName_Injected(intPtr, in node, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public string GetPath(in HierarchyNode node)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetPath_Injected(intPtr, in node, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int GetHashCode(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetHashCode_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true)]
	public void SetDirty()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDirty_Injected(intPtr);
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Hierarchy FromIntPtr(IntPtr handlePtr)
	{
		return (handlePtr != IntPtr.Zero) ? ((Hierarchy)GCHandle.FromIntPtr(handlePtr).Target) : null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("HierarchyBindings::Create", IsThreadSafe = true)]
	private static extern IntPtr Create(IntPtr handlePtr, out IntPtr rootPtr, out IntPtr versionPtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("HierarchyBindings::Destroy", IsThreadSafe = true)]
	private static extern void Destroy(IntPtr nativePtr);

	[FreeFunction("HierarchyBindings::GetOrCreateNodeTypeHandler", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private IntPtr GetOrCreateNodeTypeHandler(Type type)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetOrCreateNodeTypeHandler_Injected(intPtr, type);
	}

	[FreeFunction("HierarchyBindings::GetNodeTypeHandlerFromType", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private IntPtr GetNodeTypeHandlerFromType(Type type)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNodeTypeHandlerFromType_Injected(intPtr, type);
	}

	[FreeFunction("HierarchyBindings::GetNodeTypeHandlerFromNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private IntPtr GetNodeTypeHandlerFromNode(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNodeTypeHandlerFromNode_Injected(intPtr, in node);
	}

	[FreeFunction("HierarchyBindings::GetNodeTypeHandlerFromName", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe IntPtr GetNodeTypeHandlerFromName(string nodeTypeName)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(nodeTypeName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(nodeTypeName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetNodeTypeHandlerFromName_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetNodeTypeHandlerFromName_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	[FreeFunction("HierarchyBindings::GetNodeTypeHandlersBaseCount", HasExplicitThis = true, IsThreadSafe = true)]
	internal int GetNodeTypeHandlersBaseCount()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNodeTypeHandlersBaseCount_Injected(intPtr);
	}

	[FreeFunction("HierarchyBindings::GetNodeTypeHandlersBaseSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	internal unsafe int GetNodeTypeHandlersBaseSpan(Span<IntPtr> outHandlers)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<IntPtr> span = outHandlers;
		int nodeTypeHandlersBaseSpan_Injected;
		fixed (IntPtr* begin = span)
		{
			ManagedSpanWrapper outHandlers2 = new ManagedSpanWrapper(begin, span.Length);
			nodeTypeHandlersBaseSpan_Injected = GetNodeTypeHandlersBaseSpan_Injected(intPtr, ref outHandlers2);
		}
		return nodeTypeHandlersBaseSpan_Injected;
	}

	[FreeFunction("HierarchyBindings::GetNodeTypeFromType", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private HierarchyNodeType GetNodeTypeFromType(Type type)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetNodeTypeFromType_Injected(intPtr, type, out var ret);
		return ret;
	}

	[FreeFunction("HierarchyBindings::AddNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private HierarchyNode AddNode(in HierarchyNode parent)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddNode_Injected(intPtr, in parent, out var ret);
		return ret;
	}

	[FreeFunction("HierarchyBindings::AddNodeSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe void AddNodeSpan(in HierarchyNode parent, Span<HierarchyNode> nodes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<HierarchyNode> span = nodes;
		fixed (HierarchyNode* begin = span)
		{
			ManagedSpanWrapper nodes2 = new ManagedSpanWrapper(begin, span.Length);
			AddNodeSpan_Injected(intPtr, in parent, ref nodes2);
		}
	}

	[FreeFunction("HierarchyBindings::SetNodeParent", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void SetNodeParent(in HierarchyNode node, in HierarchyNode parent)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetNodeParent_Injected(intPtr, in node, in parent);
	}

	[FreeFunction("HierarchyBindings::SetNodeParentAt", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void SetNodeParentAt(in HierarchyNode node, in HierarchyNode parent, int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetNodeParentAt_Injected(intPtr, in node, in parent, index);
	}

	[FreeFunction("HierarchyBindings::GetNodeChildrenSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetNodeChildrenSpan(in HierarchyNode node, Span<HierarchyNode> outChildren)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<HierarchyNode> span = outChildren;
		int nodeChildrenSpan_Injected;
		fixed (HierarchyNode* begin = span)
		{
			ManagedSpanWrapper outChildren2 = new ManagedSpanWrapper(begin, span.Length);
			nodeChildrenSpan_Injected = GetNodeChildrenSpan_Injected(intPtr, in node, ref outChildren2);
		}
		return nodeChildrenSpan_Injected;
	}

	[FreeFunction("HierarchyBindings::EnumerateChildrenPtr", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private IntPtr EnumerateChildrenPtr(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return EnumerateChildrenPtr_Injected(intPtr, in node);
	}

	[FreeFunction("HierarchyBindings::GetOrCreateProperty", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe HierarchyPropertyId GetOrCreateProperty(string name, in HierarchyPropertyDescriptor descriptor)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		HierarchyPropertyId ret = default(HierarchyPropertyId);
		HierarchyPropertyId result;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetOrCreateProperty_Injected(intPtr, ref managedSpanWrapper, in descriptor, out ret);
				}
			}
			else
			{
				GetOrCreateProperty_Injected(intPtr, ref managedSpanWrapper, in descriptor, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	[FreeFunction("HierarchyBindings::SetPropertyRaw", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	internal unsafe void SetPropertyRaw(in HierarchyPropertyId property, in HierarchyNode node, void* ptr, int size)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPropertyRaw_Injected(intPtr, in property, in node, ptr, size);
	}

	[FreeFunction("HierarchyBindings::GetPropertyRaw", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	internal unsafe void* GetPropertyRaw(in HierarchyPropertyId property, in HierarchyNode node, out int size)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPropertyRaw_Injected(intPtr, in property, in node, out size);
	}

	[FreeFunction("HierarchyBindings::SetPropertyString", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	internal unsafe void SetPropertyString(in HierarchyPropertyId property, in HierarchyNode node, string value)
	{
		//The blocks IL_003b are reachable both inside and outside the pinned region starting at IL_002a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(value);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetPropertyString_Injected(intPtr, in property, in node, ref managedSpanWrapper);
					return;
				}
			}
			SetPropertyString_Injected(intPtr, in property, in node, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("HierarchyBindings::GetPropertyString", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	internal string GetPropertyString(in HierarchyPropertyId property, in HierarchyNode node)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetPropertyString_Injected(intPtr, in property, in node, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("HierarchyBindings::ClearProperty", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	internal void ClearProperty(in HierarchyPropertyId property, in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearProperty_Injected(intPtr, in property, in node);
	}

	[RequiredByNativeCode]
	private static IntPtr CreateHierarchy(IntPtr nativePtr, IntPtr rootPtr, IntPtr versionPtr)
	{
		return GCHandle.ToIntPtr(GCHandle.Alloc(new Hierarchy(nativePtr, rootPtr, versionPtr)));
	}

	[RequiredByNativeCode]
	private static void InvokeHandlerCreated(IntPtr hierarchyPtr, IntPtr handlerPtr)
	{
		Hierarchy hierarchy = FromIntPtr(hierarchyPtr);
		HierarchyNodeTypeHandlerBase handler = HierarchyNodeTypeHandlerBase.FromIntPtr(handlerPtr);
		hierarchy.HandlerCreated?.Invoke(handler);
	}

	[Obsolete("RegisterNodeTypeHandler has been renamed GetOrCreateNodeTypeHandler (UnityUpgradable) -> GetOrCreateNodeTypeHandler<T>()")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public T RegisterNodeTypeHandler<T>() where T : HierarchyNodeTypeHandlerBase
	{
		return (T)HierarchyNodeTypeHandlerBase.FromIntPtr(GetOrCreateNodeTypeHandler(typeof(T)));
	}

	[Obsolete("UnregisterNodeTypeHandler no longer has any effect and will be removed in a future release.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void UnregisterNodeTypeHandler<T>() where T : HierarchyNodeTypeHandlerBase
	{
	}

	[Obsolete("GetAllNodeTypeHandlersBaseCount is obsolete, please use EnumerateNodeTypeHandlersBase instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int GetAllNodeTypeHandlersBaseCount()
	{
		return GetNodeTypeHandlersBaseCount();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("GetAllNodeTypeHandlersBase is obsolete, please use EnumerateNodeTypeHandlersBase instead.")]
	public void GetAllNodeTypeHandlersBase(List<HierarchyNodeTypeHandlerBase> handlers)
	{
		handlers.Clear();
		foreach (HierarchyNodeTypeHandlerBase item in EnumerateNodeTypeHandlersBase())
		{
			handlers.Add(item);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("SortChildren(node, recurse) with a bool parameter is obsolete, please use SortChildren(node) or SortChildrenRecursive(node) instead.")]
	public void SortChildren(in HierarchyNode node, bool recurse)
	{
		if (recurse)
		{
			SortChildrenRecursive(in node);
		}
		else
		{
			SortChildren(in node);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_Count_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_Updating_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_UpdateNeeded_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNodeType_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNodeType ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Reserve_Injected(IntPtr _unity_self, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ReserveChildren_Injected(IntPtr _unity_self, in HierarchyNode node, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Exists_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNextSibling_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDepth_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Remove_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveChildren_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Clear_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetParent_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetChild_Injected(IntPtr _unity_self, in HierarchyNode node, int index, out HierarchyNode ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildIndex_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetChildren_Injected(IntPtr _unity_self, in HierarchyNode node, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildrenCount_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetChildrenCountRecursive_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSortIndex_Injected(IntPtr _unity_self, in HierarchyNode node, int sortIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSortIndex_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SortChildren_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SortChildrenRecursive_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetChildrenNeedsSorting_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DoesChildrenNeedsSorting_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetName_Injected(IntPtr _unity_self, in HierarchyNode node, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetName_Injected(IntPtr _unity_self, in HierarchyNode node, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPath_Injected(IntPtr _unity_self, in HierarchyNode node, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetHashCode_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDirty_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Update_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UpdateIncremental_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UpdateIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetOrCreateNodeTypeHandler_Injected(IntPtr _unity_self, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNodeTypeHandlerFromType_Injected(IntPtr _unity_self, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNodeTypeHandlerFromNode_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNodeTypeHandlerFromName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodeTypeName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetNodeTypeHandlersBaseCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetNodeTypeHandlersBaseSpan_Injected(IntPtr _unity_self, ref ManagedSpanWrapper outHandlers);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNodeTypeFromType_Injected(IntPtr _unity_self, Type type, out HierarchyNodeType ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddNode_Injected(IntPtr _unity_self, in HierarchyNode parent, out HierarchyNode ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddNodeSpan_Injected(IntPtr _unity_self, in HierarchyNode parent, ref ManagedSpanWrapper nodes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetNodeParent_Injected(IntPtr _unity_self, in HierarchyNode node, in HierarchyNode parent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetNodeParentAt_Injected(IntPtr _unity_self, in HierarchyNode node, in HierarchyNode parent, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetNodeChildrenSpan_Injected(IntPtr _unity_self, in HierarchyNode node, ref ManagedSpanWrapper outChildren);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr EnumerateChildrenPtr_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetOrCreateProperty_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, in HierarchyPropertyDescriptor descriptor, out HierarchyPropertyId ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void SetPropertyRaw_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, void* ptr, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* GetPropertyRaw_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, out int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPropertyString_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPropertyString_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearProperty_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node);
}
