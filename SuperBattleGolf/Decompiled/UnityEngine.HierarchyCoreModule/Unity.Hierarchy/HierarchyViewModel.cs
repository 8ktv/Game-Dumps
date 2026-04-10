using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/HierarchyCore/Public/HierarchyViewModel.h")]
[NativeHeader("Modules/HierarchyCore/HierarchyViewModelBindings.h")]
[RequiredByNativeCode]
public sealed class HierarchyViewModel : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToUnmanaged(HierarchyViewModel viewModel)
		{
			return viewModel.m_Ptr;
		}
	}

	public delegate void FlagsChangedEventHandler(HierarchyNodeFlags flags);

	public struct Enumerator
	{
		private readonly HierarchyViewModel m_ViewModel;

		private readonly ReadOnlyNativeVector<HierarchyNode> m_Nodes;

		private readonly int m_Version;

		private int m_Index;

		public ref readonly HierarchyNode Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (m_Version != m_ViewModel.m_Version)
				{
					throw new InvalidOperationException("HierarchyViewModel was modified.");
				}
				return ref m_Nodes[m_Index];
			}
		}

		internal Enumerator(HierarchyViewModel hierarchyViewModel)
		{
			m_ViewModel = hierarchyViewModel;
			m_Nodes = hierarchyViewModel.m_Nodes;
			m_Version = hierarchyViewModel.m_Version;
			m_Index = -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			return ++m_Index < m_Nodes.Count;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	internal class ReadOnlyList : IList, ICollection, IEnumerable
	{
		private readonly HierarchyViewModel m_ViewModel;

		public bool IsFixedSize => true;

		public bool IsReadOnly => true;

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (!m_ViewModel.IsCreated)
				{
					throw new NullReferenceException("HierarchyViewModel has been disposed.");
				}
				return m_ViewModel.Count;
			}
		}

		public object this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (!m_ViewModel.IsCreated)
				{
					throw new NullReferenceException("HierarchyViewModel has been disposed.");
				}
				return m_ViewModel[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal ReadOnlyList(HierarchyViewModel viewModel)
		{
			m_ViewModel = viewModel;
		}

		public bool Contains(object value)
		{
			if (value is HierarchyNode node)
			{
				if (!m_ViewModel.IsCreated)
				{
					throw new NullReferenceException("HierarchyViewModel has been disposed.");
				}
				return m_ViewModel.Contains(in node);
			}
			return false;
		}

		public int IndexOf(object value)
		{
			if (value is HierarchyNode node)
			{
				if (!m_ViewModel.IsCreated)
				{
					throw new NullReferenceException("HierarchyViewModel has been disposed.");
				}
				return m_ViewModel.IndexOf(in node);
			}
			return -1;
		}

		public void CopyTo(Array array, int index)
		{
			for (int i = index; i < m_ViewModel.Count; i++)
			{
				array.SetValue(m_ViewModel[i], i - index);
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(m_ViewModel);
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException();
		}
	}

	private IntPtr m_Ptr;

	internal readonly Hierarchy m_Hierarchy;

	internal readonly HierarchyFlattened m_HierarchyFlattened;

	private ReadOnlyNativeVector<HierarchyFlattenedNode> m_FlattenedNodes;

	private ReadOnlyNativeVector<HierarchyNode> m_Nodes;

	private int m_Version;

	private readonly bool m_IsOwner;

	public bool IsCreated => m_Ptr != IntPtr.Zero;

	public int Count => m_Nodes.Count;

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

	public bool Filtering
	{
		[NativeMethod("Filtering", IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_Filtering_Injected(intPtr);
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

	internal ReadOnlyNativeVector<HierarchyNode> Nodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_Nodes;
		}
	}

	internal int Version
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		get
		{
			return m_Version;
		}
	}

	internal float UpdateProgress
	{
		[NativeMethod("UpdateProgress", IsThreadSafe = true)]
		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_UpdateProgress_Injected(intPtr);
		}
	}

	internal IHierarchySearchQueryParser QueryParser
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		get;
		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		set;
	}

	internal HierarchySearchQueryDescriptor Query
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		[NativeMethod(IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_Query_Injected(intPtr);
		}
		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		[NativeMethod(IsThreadSafe = true)]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_Query_Injected(intPtr, value);
		}
	}

	public ref readonly HierarchyNode this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ref m_Nodes[index];
		}
	}

	[Obsolete("The Hierarchy property will be removed in the future, remove its usage from your code.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Hierarchy Hierarchy => m_Hierarchy;

	[Obsolete("The HierarchyFlattened property will be removed in the future, remove its usage from your code.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public HierarchyFlattened HierarchyFlattned => m_HierarchyFlattened;

	public event FlagsChangedEventHandler FlagsChanged;

	public HierarchyViewModel(HierarchyFlattened hierarchyFlattened, HierarchyNodeFlags defaultFlags = HierarchyNodeFlags.None)
	{
		m_Ptr = Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), hierarchyFlattened, defaultFlags, out var nodesPtr, out var nodesCount, out var indicesPtr, out var indicesCount, out var version);
		m_Hierarchy = hierarchyFlattened.m_Hierarchy;
		m_HierarchyFlattened = hierarchyFlattened;
		m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(nodesPtr, nodesCount);
		m_Nodes = new ReadOnlyNativeVector<HierarchyNode>(indicesPtr, indicesCount);
		m_Version = version;
		m_IsOwner = true;
		QueryParser = new DefaultHierarchySearchQueryParser();
	}

	private HierarchyViewModel(IntPtr nativePtr, HierarchyFlattened hierarchyFlattened, IntPtr flattenedNodesPtr, int flattenedNodesCount, IntPtr nodesPtr, int nodesCount, int version)
	{
		m_Ptr = nativePtr;
		m_Hierarchy = hierarchyFlattened.m_Hierarchy;
		m_HierarchyFlattened = hierarchyFlattened;
		m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(flattenedNodesPtr, flattenedNodesCount);
		m_Nodes = new ReadOnlyNativeVector<HierarchyNode>(nodesPtr, nodesCount);
		m_Version = version;
		m_IsOwner = false;
		QueryParser = new DefaultHierarchySearchQueryParser();
	}

	~HierarchyViewModel()
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
		m_Nodes = default(ReadOnlyNativeVector<HierarchyNode>);
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
	public void SetRoot(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetRoot_Injected(intPtr, in node);
	}

	[NativeMethod(IsThreadSafe = true)]
	public HierarchyNode GetRoot()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetRoot_Injected(intPtr, out var ret);
		return ret;
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

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public HierarchyNodeFlags GetFlags(in HierarchyNode node)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetFlags_Injected(intPtr, in node);
	}

	public void SetFlags(HierarchyNodeFlags flags)
	{
		SetFlagsAll(flags);
	}

	public void SetFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		SetFlagsNode(in node, flags);
	}

	public int SetFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
	{
		return SetFlagsNodes(nodes, flags);
	}

	public int SetFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
	{
		return SetFlagsIndices(indices, flags);
	}

	public void SetFlagsRecursive(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		SetFlagsRecursiveNode(in node, flags, direction);
	}

	public void SetFlagsRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		SetFlagsRecursiveNodes(nodes, flags, direction);
	}

	public bool HasAllFlags(HierarchyNodeFlags flags)
	{
		return HasAllFlagsAny(flags);
	}

	public bool HasAnyFlags(HierarchyNodeFlags flags)
	{
		return HasAnyFlagsAny(flags);
	}

	public bool HasAllFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		return HasAllFlagsNode(in node, flags);
	}

	public bool HasAnyFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		return HasAnyFlagsNode(in node, flags);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int HasAllFlagsCount(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasAllFlagsCount_Injected(intPtr, flags);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int HasAnyFlagsCount(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasAnyFlagsCount_Injected(intPtr, flags);
	}

	public bool DoesNotHaveAllFlags(HierarchyNodeFlags flags)
	{
		return DoesNotHaveAllFlagsAny(flags);
	}

	public bool DoesNotHaveAnyFlags(HierarchyNodeFlags flags)
	{
		return DoesNotHaveAnyFlagsAny(flags);
	}

	public bool DoesNotHaveAllFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		return DoesNotHaveAllFlagsNode(in node, flags);
	}

	public bool DoesNotHaveAnyFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		return DoesNotHaveAnyFlagsNode(in node, flags);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int DoesNotHaveAllFlagsCount(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return DoesNotHaveAllFlagsCount_Injected(intPtr, flags);
	}

	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	public int DoesNotHaveAnyFlagsCount(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return DoesNotHaveAnyFlagsCount_Injected(intPtr, flags);
	}

	public void ClearFlags(HierarchyNodeFlags flags)
	{
		ClearFlagsAll(flags);
	}

	public void ClearFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		ClearFlagsNode(in node, flags);
	}

	public int ClearFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
	{
		return ClearFlagsNodes(nodes, flags);
	}

	public int ClearFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
	{
		return ClearFlagsIndices(indices, flags);
	}

	public void ClearFlagsRecursive(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		ClearFlagsRecursiveNode(in node, flags, direction);
	}

	public void ClearFlagsRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		ClearFlagsRecursiveNodes(nodes, flags, direction);
	}

	public void ToggleFlags(HierarchyNodeFlags flags)
	{
		ToggleFlagsAll(flags);
	}

	public void ToggleFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		ToggleFlagsNode(in node, flags);
	}

	public int ToggleFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
	{
		return ToggleFlagsNodes(nodes, flags);
	}

	public int ToggleFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
	{
		return ToggleFlagsIndices(indices, flags);
	}

	public void ToggleFlagsRecursive(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		ToggleFlagsRecursiveNode(in node, flags, direction);
	}

	public void ToggleFlagsRecursive(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		ToggleFlagsRecursiveNodes(nodes, flags, direction);
	}

	[NativeMethod(IsThreadSafe = true)]
	public void BeginFlagsChange()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		BeginFlagsChange_Injected(intPtr);
	}

	public HierarchyNodeFlags EndFlagsChange()
	{
		return EndFlagsChange(notify: true);
	}

	public HierarchyNodeFlags EndFlagsChangeWithoutNotify()
	{
		return EndFlagsChange(notify: false);
	}

	public int GetNodesWithAllFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		return GetNodesWithAllFlagsSpan(flags, outNodes);
	}

	public int GetNodesWithAnyFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		return GetNodesWithAnyFlagsSpan(flags, outNodes);
	}

	public HierarchyNode[] GetNodesWithAllFlags(HierarchyNodeFlags flags)
	{
		int num = HasAllFlagsCount(flags);
		if (num == 0)
		{
			return Array.Empty<HierarchyNode>();
		}
		HierarchyNode[] array = new HierarchyNode[num];
		GetNodesWithAllFlagsSpan(flags, array);
		return array;
	}

	public HierarchyNode[] GetNodesWithAnyFlags(HierarchyNodeFlags flags)
	{
		int num = HasAnyFlagsCount(flags);
		if (num == 0)
		{
			return Array.Empty<HierarchyNode>();
		}
		HierarchyNode[] array = new HierarchyNode[num];
		GetNodesWithAnyFlagsSpan(flags, array);
		return array;
	}

	public HierarchyViewModelNodesEnumerable EnumerateNodesWithAllFlags(HierarchyNodeFlags flags)
	{
		return new HierarchyViewModelNodesEnumerable(this, flags, HasAllFlags);
	}

	public HierarchyViewModelNodesEnumerable EnumerateNodesWithAnyFlags(HierarchyNodeFlags flags)
	{
		return new HierarchyViewModelNodesEnumerable(this, flags, HasAnyFlags);
	}

	public int GetIndicesWithAllFlags(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		return GetIndicesWithAllFlagsSpan(flags, outIndices);
	}

	public int GetIndicesWithAnyFlags(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		return GetIndicesWithAnyFlagsSpan(flags, outIndices);
	}

	public int[] GetIndicesWithAllFlags(HierarchyNodeFlags flags)
	{
		int num = HasAllFlagsCount(flags);
		if (num == 0)
		{
			return Array.Empty<int>();
		}
		int[] array = new int[num];
		GetIndicesWithAllFlagsSpan(flags, array);
		return array;
	}

	public int[] GetIndicesWithAnyFlags(HierarchyNodeFlags flags)
	{
		int num = HasAnyFlagsCount(flags);
		if (num == 0)
		{
			return Array.Empty<int>();
		}
		int[] array = new int[num];
		GetIndicesWithAnyFlagsSpan(flags, array);
		return array;
	}

	public int GetNodesWithoutAllFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		return GetNodesWithoutAllFlagsSpan(flags, outNodes);
	}

	public int GetNodesWithoutAnyFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		return GetNodesWithoutAnyFlagsSpan(flags, outNodes);
	}

	public HierarchyNode[] GetNodesWithoutAllFlags(HierarchyNodeFlags flags)
	{
		int num = DoesNotHaveAllFlagsCount(flags);
		if (num == 0)
		{
			return Array.Empty<HierarchyNode>();
		}
		HierarchyNode[] array = new HierarchyNode[num];
		GetNodesWithoutAllFlagsSpan(flags, array);
		return array;
	}

	public HierarchyNode[] GetNodesWithoutAnyFlags(HierarchyNodeFlags flags)
	{
		int num = DoesNotHaveAnyFlagsCount(flags);
		if (num == 0)
		{
			return Array.Empty<HierarchyNode>();
		}
		HierarchyNode[] array = new HierarchyNode[num];
		GetNodesWithoutAnyFlagsSpan(flags, array);
		return array;
	}

	public HierarchyViewModelNodesEnumerable EnumerateNodesWithoutAllFlags(HierarchyNodeFlags flags)
	{
		return new HierarchyViewModelNodesEnumerable(this, flags, DoesNotHaveAllFlags);
	}

	public HierarchyViewModelNodesEnumerable EnumerateNodesWithoutAnyFlags(HierarchyNodeFlags flags)
	{
		return new HierarchyViewModelNodesEnumerable(this, flags, DoesNotHaveAnyFlags);
	}

	public int GetIndicesWithoutAllFlags(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		return GetIndicesWithoutAllFlagsSpan(flags, outIndices);
	}

	public int GetIndicesWithoutAnyFlags(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		return GetIndicesWithoutAnyFlagsSpan(flags, outIndices);
	}

	public int[] GetIndicesWithoutAllFlags(HierarchyNodeFlags flags)
	{
		int num = DoesNotHaveAllFlagsCount(flags);
		if (num == 0)
		{
			return Array.Empty<int>();
		}
		int[] array = new int[num];
		GetIndicesWithoutAllFlagsSpan(flags, array);
		return array;
	}

	public int[] GetIndicesWithoutAnyFlags(HierarchyNodeFlags flags)
	{
		int num = DoesNotHaveAnyFlagsCount(flags);
		if (num == 0)
		{
			return Array.Empty<int>();
		}
		int[] array = new int[num];
		GetIndicesWithoutAnyFlagsSpan(flags, array);
		return array;
	}

	public void SetQuery(string query)
	{
		HierarchySearchQueryDescriptor hierarchySearchQueryDescriptor = QueryParser.ParseQuery(query);
		if (hierarchySearchQueryDescriptor != Query)
		{
			Query = hierarchySearchQueryDescriptor;
		}
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

	public ReadOnlySpan<HierarchyNode> AsReadOnlySpan()
	{
		return m_Nodes.AsReadOnlySpan();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	internal ReadOnlyList AsReadOnlyList()
	{
		return new ReadOnlyList(this);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	[FreeFunction("HierarchyViewModelBindings::GetState", HasExplicitThis = true, IsThreadSafe = true)]
	internal byte[] GetState()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		byte[] result;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetState_Injected(intPtr, out ret);
		}
		finally
		{
			byte[] array = default(byte[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	[FreeFunction("HierarchyViewModelBindings::SetState", HasExplicitThis = true, IsThreadSafe = true)]
	internal unsafe void SetState(ReadOnlySpan<byte> bytes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<byte> readOnlySpan = bytes;
		fixed (byte* begin = readOnlySpan)
		{
			ManagedSpanWrapper bytes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			SetState_Injected(intPtr, ref bytes2);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static HierarchyViewModel FromIntPtr(IntPtr handlePtr)
	{
		return (handlePtr != IntPtr.Zero) ? ((HierarchyViewModel)GCHandle.FromIntPtr(handlePtr).Target) : null;
	}

	[FreeFunction("HierarchyViewModelBindings::Create", IsThreadSafe = true)]
	private static IntPtr Create(IntPtr handlePtr, HierarchyFlattened hierarchyFlattened, HierarchyNodeFlags defaultFlags, out IntPtr nodesPtr, out int nodesCount, out IntPtr indicesPtr, out int indicesCount, out int version)
	{
		return Create_Injected(handlePtr, (hierarchyFlattened == null) ? ((IntPtr)0) : HierarchyFlattened.BindingsMarshaller.ConvertToUnmanaged(hierarchyFlattened), defaultFlags, out nodesPtr, out nodesCount, out indicesPtr, out indicesCount, out version);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("HierarchyViewModelBindings::Destroy", IsThreadSafe = true)]
	private static extern void Destroy(IntPtr nativePtr);

	[FreeFunction("HierarchyViewModelBindings::SetFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void SetFlagsAll(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFlagsAll_Injected(intPtr, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::SetFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void SetFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFlagsNode_Injected(intPtr, in node, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::SetFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe int SetFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
		int result;
		fixed (HierarchyNode* begin = readOnlySpan)
		{
			ManagedSpanWrapper nodes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			result = SetFlagsNodes_Injected(intPtr, ref nodes2, flags);
		}
		return result;
	}

	[FreeFunction("HierarchyViewModelBindings::SetFlagsRecursiveNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void SetFlagsRecursiveNode(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFlagsRecursiveNode_Injected(intPtr, in node, flags, direction);
	}

	[FreeFunction("HierarchyViewModelBindings::SetFlagsRecursiveNodes", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe void SetFlagsRecursiveNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
		fixed (HierarchyNode* begin = readOnlySpan)
		{
			ManagedSpanWrapper nodes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			SetFlagsRecursiveNodes_Injected(intPtr, ref nodes2, flags, direction);
		}
	}

	[FreeFunction("HierarchyViewModelBindings::SetFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe int SetFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<int> readOnlySpan = indices;
		int result;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper indices2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			result = SetFlagsIndices_Injected(intPtr, ref indices2, flags);
		}
		return result;
	}

	[FreeFunction("HierarchyViewModelBindings::HasAllFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private bool HasAllFlagsAny(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasAllFlagsAny_Injected(intPtr, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::HasAnyFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private bool HasAnyFlagsAny(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasAnyFlagsAny_Injected(intPtr, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::HasAllFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private bool HasAllFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasAllFlagsNode_Injected(intPtr, in node, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::HasAnyFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private bool HasAnyFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasAnyFlagsNode_Injected(intPtr, in node, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAllFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private bool DoesNotHaveAllFlagsAny(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return DoesNotHaveAllFlagsAny_Injected(intPtr, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAnyFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private bool DoesNotHaveAnyFlagsAny(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return DoesNotHaveAnyFlagsAny_Injected(intPtr, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAllFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private bool DoesNotHaveAllFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return DoesNotHaveAllFlagsNode_Injected(intPtr, in node, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAnyFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private bool DoesNotHaveAnyFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return DoesNotHaveAnyFlagsNode_Injected(intPtr, in node, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::ClearFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void ClearFlagsAll(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearFlagsAll_Injected(intPtr, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::ClearFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void ClearFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearFlagsNode_Injected(intPtr, in node, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::ClearFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe int ClearFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
		int result;
		fixed (HierarchyNode* begin = readOnlySpan)
		{
			ManagedSpanWrapper nodes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			result = ClearFlagsNodes_Injected(intPtr, ref nodes2, flags);
		}
		return result;
	}

	[FreeFunction("HierarchyViewModelBindings::ClearFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe int ClearFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<int> readOnlySpan = indices;
		int result;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper indices2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			result = ClearFlagsIndices_Injected(intPtr, ref indices2, flags);
		}
		return result;
	}

	[FreeFunction("HierarchyViewModelBindings::ClearFlagsRecursiveNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void ClearFlagsRecursiveNode(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearFlagsRecursiveNode_Injected(intPtr, in node, flags, direction);
	}

	[FreeFunction("HierarchyViewModelBindings::ClearFlagsRecursiveNodes", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe void ClearFlagsRecursiveNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
		fixed (HierarchyNode* begin = readOnlySpan)
		{
			ManagedSpanWrapper nodes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			ClearFlagsRecursiveNodes_Injected(intPtr, ref nodes2, flags, direction);
		}
	}

	[FreeFunction("HierarchyViewModelBindings::ToggleFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void ToggleFlagsAll(HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ToggleFlagsAll_Injected(intPtr, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::ToggleFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void ToggleFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ToggleFlagsNode_Injected(intPtr, in node, flags);
	}

	[FreeFunction("HierarchyViewModelBindings::ToggleFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe int ToggleFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
		int result;
		fixed (HierarchyNode* begin = readOnlySpan)
		{
			ManagedSpanWrapper nodes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			result = ToggleFlagsNodes_Injected(intPtr, ref nodes2, flags);
		}
		return result;
	}

	[FreeFunction("HierarchyViewModelBindings::ToggleFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe int ToggleFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<int> readOnlySpan = indices;
		int result;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper indices2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			result = ToggleFlagsIndices_Injected(intPtr, ref indices2, flags);
		}
		return result;
	}

	[FreeFunction("HierarchyViewModelBindings::ToggleFlagsRecursiveNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private void ToggleFlagsRecursiveNode(in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ToggleFlagsRecursiveNode_Injected(intPtr, in node, flags, direction);
	}

	[FreeFunction("HierarchyViewModelBindings::ToggleFlagsRecursiveNodes", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe void ToggleFlagsRecursiveNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
		fixed (HierarchyNode* begin = readOnlySpan)
		{
			ManagedSpanWrapper nodes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			ToggleFlagsRecursiveNodes_Injected(intPtr, ref nodes2, flags, direction);
		}
	}

	[FreeFunction("HierarchyViewModelBindings::EndFlagsChange", HasExplicitThis = true, IsThreadSafe = true)]
	private HierarchyNodeFlags EndFlagsChange(bool notify)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return EndFlagsChange_Injected(intPtr, notify);
	}

	[FreeFunction("HierarchyViewModelBindings::GetNodesWithAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetNodesWithAllFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<HierarchyNode> span = outNodes;
		int nodesWithAllFlagsSpan_Injected;
		fixed (HierarchyNode* begin = span)
		{
			ManagedSpanWrapper outNodes2 = new ManagedSpanWrapper(begin, span.Length);
			nodesWithAllFlagsSpan_Injected = GetNodesWithAllFlagsSpan_Injected(intPtr, flags, ref outNodes2);
		}
		return nodesWithAllFlagsSpan_Injected;
	}

	[FreeFunction("HierarchyViewModelBindings::GetNodesWithAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetNodesWithAnyFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<HierarchyNode> span = outNodes;
		int nodesWithAnyFlagsSpan_Injected;
		fixed (HierarchyNode* begin = span)
		{
			ManagedSpanWrapper outNodes2 = new ManagedSpanWrapper(begin, span.Length);
			nodesWithAnyFlagsSpan_Injected = GetNodesWithAnyFlagsSpan_Injected(intPtr, flags, ref outNodes2);
		}
		return nodesWithAnyFlagsSpan_Injected;
	}

	[FreeFunction("HierarchyViewModelBindings::GetIndicesWithAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetIndicesWithAllFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<int> span = outIndices;
		int indicesWithAllFlagsSpan_Injected;
		fixed (int* begin = span)
		{
			ManagedSpanWrapper outIndices2 = new ManagedSpanWrapper(begin, span.Length);
			indicesWithAllFlagsSpan_Injected = GetIndicesWithAllFlagsSpan_Injected(intPtr, flags, ref outIndices2);
		}
		return indicesWithAllFlagsSpan_Injected;
	}

	[FreeFunction("HierarchyViewModelBindings::GetIndicesWithAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetIndicesWithAnyFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<int> span = outIndices;
		int indicesWithAnyFlagsSpan_Injected;
		fixed (int* begin = span)
		{
			ManagedSpanWrapper outIndices2 = new ManagedSpanWrapper(begin, span.Length);
			indicesWithAnyFlagsSpan_Injected = GetIndicesWithAnyFlagsSpan_Injected(intPtr, flags, ref outIndices2);
		}
		return indicesWithAnyFlagsSpan_Injected;
	}

	[FreeFunction("HierarchyViewModelBindings::GetNodesWithoutAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetNodesWithoutAllFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<HierarchyNode> span = outNodes;
		int nodesWithoutAllFlagsSpan_Injected;
		fixed (HierarchyNode* begin = span)
		{
			ManagedSpanWrapper outNodes2 = new ManagedSpanWrapper(begin, span.Length);
			nodesWithoutAllFlagsSpan_Injected = GetNodesWithoutAllFlagsSpan_Injected(intPtr, flags, ref outNodes2);
		}
		return nodesWithoutAllFlagsSpan_Injected;
	}

	[FreeFunction("HierarchyViewModelBindings::GetNodesWithoutAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetNodesWithoutAnyFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<HierarchyNode> span = outNodes;
		int nodesWithoutAnyFlagsSpan_Injected;
		fixed (HierarchyNode* begin = span)
		{
			ManagedSpanWrapper outNodes2 = new ManagedSpanWrapper(begin, span.Length);
			nodesWithoutAnyFlagsSpan_Injected = GetNodesWithoutAnyFlagsSpan_Injected(intPtr, flags, ref outNodes2);
		}
		return nodesWithoutAnyFlagsSpan_Injected;
	}

	[FreeFunction("HierarchyViewModelBindings::GetIndicesWithoutAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetIndicesWithoutAllFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<int> span = outIndices;
		int indicesWithoutAllFlagsSpan_Injected;
		fixed (int* begin = span)
		{
			ManagedSpanWrapper outIndices2 = new ManagedSpanWrapper(begin, span.Length);
			indicesWithoutAllFlagsSpan_Injected = GetIndicesWithoutAllFlagsSpan_Injected(intPtr, flags, ref outIndices2);
		}
		return indicesWithoutAllFlagsSpan_Injected;
	}

	[FreeFunction("HierarchyViewModelBindings::GetIndicesWithoutAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
	private unsafe int GetIndicesWithoutAnyFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToUnmanaged(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<int> span = outIndices;
		int indicesWithoutAnyFlagsSpan_Injected;
		fixed (int* begin = span)
		{
			ManagedSpanWrapper outIndices2 = new ManagedSpanWrapper(begin, span.Length);
			indicesWithoutAnyFlagsSpan_Injected = GetIndicesWithoutAnyFlagsSpan_Injected(intPtr, flags, ref outIndices2);
		}
		return indicesWithoutAnyFlagsSpan_Injected;
	}

	[RequiredByNativeCode]
	private static IntPtr CreateHierarchyViewModel(IntPtr nativePtr, IntPtr flattenedPtr, IntPtr flattenedNodesPtr, int flattenedNodesCount, IntPtr nodesPtr, int nodesCount, int version)
	{
		return GCHandle.ToIntPtr(GCHandle.Alloc(new HierarchyViewModel(nativePtr, HierarchyFlattened.FromIntPtr(flattenedPtr), flattenedNodesPtr, flattenedNodesCount, nodesPtr, nodesCount, version)));
	}

	[RequiredByNativeCode]
	private static void UpdateHierarchyViewModel(IntPtr handlePtr, IntPtr flattenedNodesPtr, int flattenedNodesCount, IntPtr nodesPtr, int nodesCount, int version)
	{
		HierarchyViewModel hierarchyViewModel = FromIntPtr(handlePtr);
		hierarchyViewModel.m_FlattenedNodes = new ReadOnlyNativeVector<HierarchyFlattenedNode>(flattenedNodesPtr, flattenedNodesCount);
		hierarchyViewModel.m_Nodes = new ReadOnlyNativeVector<HierarchyNode>(nodesPtr, nodesCount);
		hierarchyViewModel.m_Version = version;
	}

	[RequiredByNativeCode]
	private static void InvokeFlagsChanged(IntPtr handlePtr, HierarchyNodeFlags flags)
	{
		HierarchyViewModel hierarchyViewModel = FromIntPtr(handlePtr);
		hierarchyViewModel.FlagsChanged?.Invoke(flags);
	}

	[RequiredByNativeCode]
	private static void SearchBegin(IntPtr handlePtr)
	{
		HierarchyViewModel hierarchyViewModel = FromIntPtr(handlePtr);
		foreach (HierarchyNodeTypeHandlerBase item in hierarchyViewModel.m_Hierarchy.EnumerateNodeTypeHandlersBase())
		{
			item.Internal_SearchBegin(hierarchyViewModel.Query);
		}
	}

	[Obsolete("SetFlags(node, flags, recurse) with a bool parameter is obsolete, please use SetFlags(node, flags) or SetFlags(node, flags, direction) instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse)
	{
		if (recurse)
		{
			SetFlagsRecursiveNode(in node, flags, HierarchyTraversalDirection.Children);
		}
		else
		{
			SetFlagsNode(in node, flags);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ClearFlags(node, flags, recurse) with a bool parameter is obsolete, please use ClearFlags(node, flags) or ClearFlags(node, flags, direction) instead.", false)]
	public void ClearFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse)
	{
		if (recurse)
		{
			ClearFlagsRecursiveNode(in node, flags, HierarchyTraversalDirection.Children);
		}
		else
		{
			ClearFlagsNode(in node, flags);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ToggleFlags(node, flags, recurse) with a bool parameter is obsolete, please use ToggleFlags(node, flags) or ToggleFlags(node, flags, direction) instead.", false)]
	public void ToggleFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse)
	{
		if (recurse)
		{
			ToggleFlagsRecursiveNode(in node, flags, HierarchyTraversalDirection.Children);
		}
		else
		{
			ToggleFlagsNode(in node, flags);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("HasFlags is obsolete, please use HasAllFlags or HasAnyFlags instead.", false)]
	public bool HasFlags(HierarchyNodeFlags flags)
	{
		return HasAllFlagsAny(flags);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("HasFlags is obsolete, please use HasAllFlags or HasAnyFlags instead.", false)]
	public bool HasFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		return HasAllFlagsNode(in node, flags);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("HasFlagsCount is obsolete, please use HasAllFlagsCount or HasAnyFlagsCount instead.", false)]
	public int HasFlagsCount(HierarchyNodeFlags flags)
	{
		return HasAllFlagsCount(flags);
	}

	[Obsolete("DoesNotHaveFlags is obsolete, please use DoesNotHaveAllFlags or DoesNotHaveAnyFlags instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool DoesNotHaveFlags(HierarchyNodeFlags flags)
	{
		return DoesNotHaveAllFlagsAny(flags);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("DoesNotHaveFlags is obsolete, please use DoesNotHaveAllFlags or DoesNotHaveAnyFlags instead.", false)]
	public bool DoesNotHaveFlags(in HierarchyNode node, HierarchyNodeFlags flags)
	{
		return DoesNotHaveAllFlagsNode(in node, flags);
	}

	[Obsolete("DoesNotHaveFlagsCount is obsolete, please use DoesNotHaveAllFlagsCount or DoesNotHaveAnyFlagsCount instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int DoesNotHaveFlagsCount(HierarchyNodeFlags flags)
	{
		return DoesNotHaveAllFlagsCount(flags);
	}

	[Obsolete("GetNodesWithFlags is obsolete, please use GetNodesWithAllFlags or GetNodesWithAnyFlags instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int GetNodesWithFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		return GetNodesWithAllFlagsSpan(flags, outNodes);
	}

	[Obsolete("GetNodesWithFlags is obsolete, please use GetNodesWithAllFlags or GetNodesWithAnyFlags instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public HierarchyNode[] GetNodesWithFlags(HierarchyNodeFlags flags)
	{
		return GetNodesWithAllFlags(flags);
	}

	[Obsolete("EnumerateNodesWithFlags is obsolete, please use EnumerateNodesWithAllFlags or EnumerateNodesWithAnyFlags instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public HierarchyViewModelNodesEnumerable EnumerateNodesWithFlags(HierarchyNodeFlags flags)
	{
		return EnumerateNodesWithAllFlags(flags);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("GetIndicesWithFlags is obsolete, please use GetIndicesWithAllFlags or GetIndicesWithAnyFlags instead.", false)]
	public int GetIndicesWithFlags(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		return GetIndicesWithAllFlagsSpan(flags, outIndices);
	}

	[Obsolete("GetIndicesWithFlags is obsolete, please use GetIndicesWithAllFlags or GetIndicesWithAnyFlags instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int[] GetIndicesWithFlags(HierarchyNodeFlags flags)
	{
		return GetIndicesWithAllFlags(flags);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("GetNodesWithoutFlags is obsolete, please use GetNodesWithoutAllFlags or GetNodesWithoutAnyFlags instead.", false)]
	public int GetNodesWithoutFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
	{
		return GetNodesWithoutAllFlagsSpan(flags, outNodes);
	}

	[Obsolete("GetNodesWithoutFlags is obsolete, please use GetNodesWithoutAllFlags or GetNodesWithoutAnyFlags instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public HierarchyNode[] GetNodesWithoutFlags(HierarchyNodeFlags flags)
	{
		return GetNodesWithoutAllFlags(flags);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("EnumerateNodesWithoutFlags is obsolete, please use EnumerateNodesWithoutAllFlags or EnumerateNodesWithoutAnyFlags instead.", false)]
	public HierarchyViewModelNodesEnumerable EnumerateNodesWithoutFlags(HierarchyNodeFlags flags)
	{
		return EnumerateNodesWithoutAllFlags(flags);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("GetIndicesWithoutFlags is obsolete, please use GetIndicesWithoutAllFlags or GetIndicesWithoutAnyFlags instead.", false)]
	public int GetIndicesWithoutFlags(HierarchyNodeFlags flags, Span<int> outIndices)
	{
		return GetIndicesWithoutAllFlagsSpan(flags, outIndices);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("GetIndicesWithoutFlags is obsolete, please use GetIndicesWithoutAllFlags or GetIndicesWithoutAnyFlags instead.", false)]
	public int[] GetIndicesWithoutFlags(HierarchyNodeFlags flags)
	{
		return GetIndicesWithoutAllFlags(flags);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_Updating_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_UpdateNeeded_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_Filtering_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_UpdateProgress_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern HierarchySearchQueryDescriptor get_Query_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_Query_Injected(IntPtr _unity_self, HierarchySearchQueryDescriptor value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int IndexOf_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Contains_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRoot_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRoot_Injected(IntPtr _unity_self, out HierarchyNode ret);

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
	private static extern HierarchyNodeFlags GetFlags_Injected(IntPtr _unity_self, in HierarchyNode node);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int HasAllFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int HasAnyFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int DoesNotHaveAllFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int DoesNotHaveAnyFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BeginFlagsChange_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Update_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UpdateIncremental_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UpdateIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetState_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetState_Injected(IntPtr _unity_self, ref ManagedSpanWrapper bytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create_Injected(IntPtr handlePtr, IntPtr hierarchyFlattened, HierarchyNodeFlags defaultFlags, out IntPtr nodesPtr, out int nodesCount, out IntPtr indicesPtr, out int indicesCount, out int version);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int SetFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFlagsRecursiveNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFlagsRecursiveNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int SetFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasAllFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasAnyFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasAllFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasAnyFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DoesNotHaveAllFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DoesNotHaveAnyFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DoesNotHaveAllFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DoesNotHaveAnyFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ClearFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ClearFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearFlagsRecursiveNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearFlagsRecursiveNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ToggleFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ToggleFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ToggleFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ToggleFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ToggleFlagsRecursiveNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ToggleFlagsRecursiveNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags, HierarchyTraversalDirection direction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern HierarchyNodeFlags EndFlagsChange_Injected(IntPtr _unity_self, bool notify);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetNodesWithAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetNodesWithAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetIndicesWithAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetIndicesWithAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetNodesWithoutAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetNodesWithoutAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetIndicesWithoutAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetIndicesWithoutAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);
}
