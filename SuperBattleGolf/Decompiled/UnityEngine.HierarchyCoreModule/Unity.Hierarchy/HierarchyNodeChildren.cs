using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy;

public readonly struct HierarchyNodeChildren
{
	public struct Enumerator
	{
		private readonly HierarchyNodeChildren m_Enumerable;

		private int m_Index;

		public unsafe ref readonly HierarchyNode Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				m_Enumerable.ThrowIfVersionChanged();
				return ref m_Enumerable.m_Ptr[m_Index];
			}
		}

		internal Enumerator(in HierarchyNodeChildren enumerable)
		{
			m_Enumerable = enumerable;
			m_Index = -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			return ++m_Index < m_Enumerable.m_Count;
		}
	}

	private const int k_HierarchyNodeChildrenIsAllocBit = int.MinValue;

	private readonly Hierarchy m_Hierarchy;

	private unsafe readonly HierarchyNode* m_Ptr;

	private readonly int m_Version;

	private readonly int m_Count;

	public int Count
	{
		get
		{
			ThrowIfVersionChanged();
			return m_Count;
		}
	}

	public unsafe ref readonly HierarchyNode this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (index < 0 || index >= m_Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			ThrowIfVersionChanged();
			return ref m_Ptr[index];
		}
	}

	internal unsafe HierarchyNodeChildren(Hierarchy hierarchy, IntPtr nodeChildrenPtr)
	{
		if (hierarchy == null)
		{
			throw new ArgumentNullException("hierarchy");
		}
		if (nodeChildrenPtr == IntPtr.Zero)
		{
			throw new ArgumentNullException("nodeChildrenPtr");
		}
		m_Hierarchy = hierarchy;
		m_Version = hierarchy.Version;
		ref HierarchyNodeChildrenAlloc reference = ref *(HierarchyNodeChildrenAlloc*)(void*)nodeChildrenPtr;
		if ((reference.ControlBit & int.MinValue) == int.MinValue)
		{
			m_Ptr = reference.Ptr;
			m_Count = reference.Size;
			return;
		}
		m_Ptr = (HierarchyNode*)(void*)nodeChildrenPtr;
		m_Count = 0;
		for (int i = 0; i < 4 && m_Ptr[i] != HierarchyNode.Null; i++)
		{
			m_Count++;
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(in this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ThrowIfVersionChanged()
	{
		if (m_Version != m_Hierarchy.Version)
		{
			throw new InvalidOperationException("Hierarchy was modified.");
		}
	}
}
