using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy;

public readonly struct HierarchyFlattenedChildrenEnumerable
{
	public struct Enumerator
	{
		private readonly HierarchyFlattenedChildrenEnumerable m_Enumerable;

		private readonly int m_End;

		private readonly int m_Depth;

		private readonly int m_Version;

		private int m_Current;

		public ref readonly HierarchyFlattenedNode Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				ThrowIfVersionChanged();
				return ref m_Enumerable.m_HierarchyFlattened[m_Current];
			}
		}

		internal Enumerator(HierarchyFlattenedChildrenEnumerable enumerable)
		{
			m_Enumerable = enumerable;
			m_End = m_Enumerable.m_ParentIndex + m_Enumerable.m_ParentNode.NextSiblingOffset;
			m_Depth = m_Enumerable.m_ParentNode.Depth + 1;
			m_Version = m_Enumerable.m_HierarchyFlattened.Version;
			m_Current = m_Enumerable.m_ParentIndex;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			ThrowIfVersionChanged();
			if (m_Current == m_Enumerable.m_ParentIndex)
			{
				m_Current++;
			}
			else
			{
				m_Current += m_Enumerable.m_HierarchyFlattened[m_Current].NextSiblingOffset;
			}
			return m_Current < m_End;
		}

		public void Reset()
		{
			ThrowIfVersionChanged();
			m_Current = m_Enumerable.m_ParentIndex;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfVersionChanged()
		{
			if (m_Version != m_Enumerable.m_HierarchyFlattened.Version)
			{
				throw new InvalidOperationException("HierarchyFlattened was modified during enumeration.");
			}
		}
	}

	private readonly HierarchyFlattened m_HierarchyFlattened;

	private readonly HierarchyFlattenedNode m_ParentNode;

	private readonly int m_ParentIndex;

	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_ParentNode.ChildrenCount;
		}
	}

	internal HierarchyFlattenedChildrenEnumerable(HierarchyFlattened hierarchyFlattened, in HierarchyNode node)
	{
		if (hierarchyFlattened == null || !hierarchyFlattened.IsCreated)
		{
			throw new ArgumentNullException("hierarchyFlattened");
		}
		if (node == HierarchyNode.Null)
		{
			throw new ArgumentNullException("node");
		}
		if (!hierarchyFlattened.Contains(in node))
		{
			throw new InvalidOperationException($"{node} not found");
		}
		m_HierarchyFlattened = hierarchyFlattened;
		m_ParentIndex = m_HierarchyFlattened.IndexOf(in node);
		m_ParentNode = m_HierarchyFlattened[m_ParentIndex];
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
}
