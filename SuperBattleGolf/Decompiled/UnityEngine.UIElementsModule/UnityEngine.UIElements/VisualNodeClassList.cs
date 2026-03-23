using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal readonly struct VisualNodeClassList : IList<string>, ICollection<string>, IEnumerable<string>, IEnumerable
{
	public struct Enumerator : IEnumerator<string>, IEnumerator, IDisposable
	{
		private readonly VisualManager m_Manager;

		private readonly VisualNodeClassData m_Data;

		private int m_Position;

		public string Current => m_Manager.ClassNameStore.GetClassNameManaged(m_Data[m_Position]);

		object IEnumerator.Current => Current;

		internal Enumerator(VisualManager manager, in VisualNodeClassData data)
		{
			m_Manager = manager;
			m_Data = data;
			m_Position = -1;
		}

		public bool MoveNext()
		{
			return ++m_Position < m_Data.Count;
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

	public int Count => m_Manager.GetProperty<VisualNodeClassData>(m_Handle).Count;

	public string this[int index]
	{
		get
		{
			ref VisualNodeClassData property = ref m_Manager.GetProperty<VisualNodeClassData>(m_Handle);
			return m_Manager.ClassNameStore.GetClassNameManaged(property[index]);
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	bool ICollection<string>.IsReadOnly => false;

	public VisualNodeClassList(VisualManager store, VisualNodeHandle handle)
	{
		m_Manager = store;
		m_Handle = handle;
	}

	public void Add(string className)
	{
		m_Manager.AddToClassList(in m_Handle, className);
	}

	public bool Remove(string className)
	{
		return m_Manager.RemoveFromClassList(in m_Handle, className);
	}

	public bool Contains(string className)
	{
		return m_Manager.ClassListContains(in m_Handle, className);
	}

	public void Clear()
	{
		m_Manager.ClearClassList(in m_Handle);
	}

	void ICollection<string>.CopyTo(string[] array, int arrayIndex)
	{
		int num = 0;
		int num2 = arrayIndex;
		while (num < Count)
		{
			array[num2] = this[num];
			num++;
			num2++;
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(m_Manager, in m_Manager.GetProperty<VisualNodeClassData>(m_Handle));
	}

	IEnumerator<string> IEnumerable<string>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	int IList<string>.IndexOf(string item)
	{
		throw new NotImplementedException();
	}

	void IList<string>.Insert(int index, string item)
	{
		throw new NotImplementedException();
	}

	void IList<string>.RemoveAt(int index)
	{
		throw new NotImplementedException();
	}
}
