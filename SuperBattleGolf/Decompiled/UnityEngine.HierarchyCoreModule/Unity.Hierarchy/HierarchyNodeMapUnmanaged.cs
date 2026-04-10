using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace Unity.Hierarchy;

public struct HierarchyNodeMapUnmanaged<T> : IDisposable where T : unmanaged
{
	private NativeSparseArray<HierarchyNode, T> m_Values;

	public bool IsCreated
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_Values.IsCreated;
		}
	}

	public int Capacity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_Values.Capacity;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Values.Capacity = value;
		}
	}

	public int Count => m_Values.Count;

	public T this[in HierarchyNode node]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_Values[in node];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Values[in node] = value;
		}
	}

	public HierarchyNodeMapUnmanaged(Allocator allocator)
	{
		m_Values = new NativeSparseArray<HierarchyNode, T>(KeyIndex, KeyEqual, allocator);
	}

	public HierarchyNodeMapUnmanaged(in T initValue, Allocator allocator)
	{
		m_Values = new NativeSparseArray<HierarchyNode, T>(in initValue, KeyIndex, KeyEqual, allocator);
	}

	public void Dispose()
	{
		m_Values.Dispose();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reserve(int capacity)
	{
		m_Values.Reserve(capacity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ContainsKey(in HierarchyNode node)
	{
		return m_Values.ContainsKey(in node);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(in HierarchyNode node, in T value)
	{
		m_Values.Add(in node, in value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddNoResize(in HierarchyNode node, in T value)
	{
		m_Values.AddNoResize(in node, in value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryAdd(in HierarchyNode node, in T value)
	{
		return m_Values.TryAdd(in node, in value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryAddNoResize(in HierarchyNode node, in T value)
	{
		return m_Values.TryAddNoResize(in node, in value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetValue(in HierarchyNode node, out T value)
	{
		return m_Values.TryGetValue(in node, out value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Remove(in HierarchyNode node)
	{
		return m_Values.Remove(in node);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		m_Values.Clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int KeyIndex(in HierarchyNode node)
	{
		return node.Id - 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool KeyEqual(in HierarchyNode lhs, in HierarchyNode rhs)
	{
		return lhs.Version == rhs.Version;
	}
}
