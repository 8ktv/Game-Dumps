using System.Collections.Generic;

namespace UnityEngine.UIElements.HierarchyV2;

internal sealed class CollectionViewSelection
{
	private readonly HashSet<int> m_IndexLookup = new HashSet<int>();

	private int m_MinIndex = -1;

	private int m_MaxIndex = -1;

	public readonly List<int> indices = new List<int>();

	public int indexCount => indices.Count;

	public int minIndex
	{
		get
		{
			if (m_MinIndex == -1)
			{
				m_MinIndex = int.MaxValue;
				foreach (int index in indices)
				{
					if (index < m_MinIndex)
					{
						m_MinIndex = index;
					}
				}
			}
			return m_MinIndex;
		}
	}

	public int maxIndex
	{
		get
		{
			if (m_MaxIndex == -1)
			{
				foreach (int index in indices)
				{
					if (index > m_MaxIndex)
					{
						m_MaxIndex = index;
					}
				}
			}
			return m_MaxIndex;
		}
	}

	public int capacity
	{
		get
		{
			return indices.Capacity;
		}
		set
		{
			indices.Capacity = value;
		}
	}

	public int FirstIndex()
	{
		return (indices.Count > 0) ? indices[0] : (-1);
	}

	public bool ContainsIndex(int index)
	{
		return m_IndexLookup.Contains(index);
	}

	public void AddIndex(int index)
	{
		m_IndexLookup.Add(index);
		indices.Add(index);
		if (index < m_MinIndex)
		{
			m_MinIndex = index;
		}
		if (index > m_MaxIndex)
		{
			m_MaxIndex = index;
		}
	}

	public bool TryRemove(int index)
	{
		if (!m_IndexLookup.Remove(index))
		{
			return false;
		}
		int num = indices.IndexOf(index);
		if (num >= 0)
		{
			indices.RemoveAt(num);
			if (index == m_MinIndex)
			{
				m_MinIndex = -1;
			}
			if (index == m_MaxIndex)
			{
				m_MaxIndex = -1;
			}
		}
		return true;
	}

	public void ClearIndices()
	{
		m_IndexLookup.Clear();
		indices.Clear();
		m_MinIndex = -1;
		m_MaxIndex = -1;
	}
}
