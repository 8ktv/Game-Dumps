using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements;

internal readonly struct VisualNodeChildren : IEnumerable<VisualNode>, IEnumerable
{
	public struct Enumerator : IEnumerator<VisualNode>, IEnumerator, IDisposable
	{
		private readonly VisualManager m_Manager;

		private readonly VisualNodeChildrenData m_Children;

		private int m_Position;

		public VisualNode Current => new VisualNode(m_Manager, m_Children[m_Position]);

		object IEnumerator.Current => Current;

		internal Enumerator(VisualManager manager, in VisualNodeChildrenData children)
		{
			m_Manager = manager;
			m_Children = children;
			m_Position = -1;
		}

		public bool MoveNext()
		{
			return ++m_Position < m_Children.Count;
		}

		public void Reset()
		{
			m_Position = -1;
		}

		public void Dispose()
		{
		}
	}

	private readonly VisualManager m_Manager;

	private readonly VisualNodeHandle m_Handle;

	public int Count => m_Manager.GetChildrenCount(in m_Handle);

	public unsafe VisualNode this[int index]
	{
		get
		{
			VisualNodeChildrenData* dataPtr = GetDataPtr();
			if ((uint)index >= dataPtr->Count)
			{
				throw new IndexOutOfRangeException();
			}
			return new VisualNode(m_Manager, dataPtr->ElementAt(index));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe VisualNodeChildrenData* GetDataPtr()
	{
		return (VisualNodeChildrenData*)m_Manager.GetChildrenPtr(in m_Handle).ToPointer();
	}

	public VisualNodeChildren(VisualManager manager, VisualNodeHandle handle)
	{
		m_Manager = manager;
		m_Handle = handle;
	}

	public void Add(in VisualNode child)
	{
		m_Manager.AddChild(in m_Handle, child.Handle);
	}

	public bool Remove(in VisualNode child)
	{
		return m_Manager.RemoveChild(in m_Handle, child.Handle);
	}

	public unsafe Enumerator GetEnumerator()
	{
		return new Enumerator(m_Manager, in *GetDataPtr());
	}

	IEnumerator<VisualNode> IEnumerable<VisualNode>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
