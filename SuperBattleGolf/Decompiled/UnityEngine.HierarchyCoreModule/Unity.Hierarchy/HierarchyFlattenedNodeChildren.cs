using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy;

[Obsolete("HierarchyFlattenedNodeChildren is obsolete. It has been replaced by HierarchyFlattenedChildrenEnumerable.")]
[EditorBrowsable(EditorBrowsableState.Never)]
public readonly struct HierarchyFlattenedNodeChildren
{
	public struct Enumerator
	{
		private readonly HierarchyFlattenedNodeChildren m_Enumerable;

		private readonly HierarchyFlattened m_HierarchyFlattened;

		private readonly HierarchyNode m_Node;

		private int m_CurrentIndex;

		private int m_ChildrenIndex;

		private int m_ChildrenCount;

		public ref readonly HierarchyNode Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				m_Enumerable.ThrowIfVersionChanged();
				return ref HierarchyFlattenedNode.GetNodeByRef(in m_HierarchyFlattened[m_CurrentIndex]);
			}
		}

		internal Enumerator(HierarchyFlattenedNodeChildren enumerable, HierarchyNode node)
		{
			m_Enumerable = enumerable;
			m_HierarchyFlattened = enumerable.m_HierarchyFlattened;
			m_Node = node;
			m_CurrentIndex = -1;
			m_ChildrenIndex = 0;
			m_ChildrenCount = 0;
		}

		public bool MoveNext()
		{
			m_Enumerable.ThrowIfVersionChanged();
			if (m_CurrentIndex == -1)
			{
				int num = m_HierarchyFlattened.IndexOf(in m_Node);
				if (num == -1)
				{
					return false;
				}
				ref readonly HierarchyFlattenedNode reference = ref m_HierarchyFlattened[num];
				if (reference == HierarchyFlattenedNode.Null || reference.ChildrenCount <= 0)
				{
					return false;
				}
				if (num + 1 >= m_HierarchyFlattened.Count)
				{
					return false;
				}
				m_CurrentIndex = num + 1;
				m_ChildrenIndex = 0;
				m_ChildrenCount = reference.ChildrenCount;
				return true;
			}
			ref readonly HierarchyFlattenedNode reference2 = ref m_HierarchyFlattened[m_CurrentIndex];
			if (m_ChildrenIndex + 1 >= m_ChildrenCount || reference2.NextSiblingOffset <= 0)
			{
				return false;
			}
			m_CurrentIndex += reference2.NextSiblingOffset;
			m_ChildrenIndex++;
			return true;
		}
	}

	private readonly HierarchyFlattened m_HierarchyFlattened;

	private readonly HierarchyNode m_Node;

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

	public ref readonly HierarchyFlattenedNode this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (index < 0 || index >= m_Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			ThrowIfVersionChanged();
			return ref m_HierarchyFlattened[index];
		}
	}

	internal HierarchyFlattenedNodeChildren(HierarchyFlattened hierarchyFlattened, in HierarchyNode node)
	{
		if (hierarchyFlattened == null)
		{
			throw new ArgumentNullException("hierarchyFlattened");
		}
		if (node == HierarchyNode.Null)
		{
			throw new ArgumentNullException("node");
		}
		if (!hierarchyFlattened.Contains(in node))
		{
			throw new InvalidOperationException($"node {node.Id}:{node.Version} not found");
		}
		m_HierarchyFlattened = hierarchyFlattened;
		m_Node = node;
		m_Version = hierarchyFlattened.Version;
		m_Count = m_HierarchyFlattened.GetChildrenCount(in m_Node);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this, m_Node);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ThrowIfVersionChanged()
	{
		if (m_Version != m_HierarchyFlattened.Version)
		{
			throw new InvalidOperationException("HierarchyFlattened was modified.");
		}
	}
}
