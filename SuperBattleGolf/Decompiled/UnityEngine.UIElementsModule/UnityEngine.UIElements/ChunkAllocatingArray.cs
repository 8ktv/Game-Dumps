using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal class ChunkAllocatingArray<T>
{
	private const int k_ChunkSize = 2048;

	private readonly List<T[]> m_Chunks;

	public T this[int index]
	{
		get
		{
			int num = index / 2048;
			int num2 = index % 2048;
			if (num >= m_Chunks.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return m_Chunks[num][num2];
		}
		set
		{
			int num = index / 2048;
			int num2 = index % 2048;
			while (num >= m_Chunks.Count)
			{
				m_Chunks.Add(new T[2048]);
			}
			m_Chunks[num][num2] = value;
		}
	}

	public ChunkAllocatingArray()
	{
		m_Chunks = new List<T[]> { new T[2048] };
	}
}
