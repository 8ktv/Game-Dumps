using System.Collections.Generic;

namespace UnityEngine.UIElements.Layout;

internal class ManagedObjectStore<T>
{
	private const int k_ChunkSize = 2048;

	private readonly int m_ChunkSize;

	private int m_Length;

	private readonly List<T[]> m_Chunks;

	private readonly Queue<int> m_Free;

	public ManagedObjectStore(int chunkSize = 2048)
	{
		m_ChunkSize = chunkSize;
		m_Chunks = new List<T[]> { new T[m_ChunkSize] };
		m_Length = 1;
		m_Free = new Queue<int>();
	}

	public T GetValue(int index)
	{
		if (index == 0)
		{
			return default(T);
		}
		int index2 = index / m_ChunkSize;
		int num = index % m_ChunkSize;
		return m_Chunks[index2][num];
	}

	public void UpdateValue(ref int index, T value)
	{
		if (index != 0)
		{
			if (value != null)
			{
				int index2 = index / m_ChunkSize;
				int num = index % m_ChunkSize;
				m_Chunks[index2][num] = value;
			}
			else
			{
				m_Free.Enqueue(index);
				int index3 = index / m_ChunkSize;
				int num2 = index % m_ChunkSize;
				m_Chunks[index3][num2] = default(T);
				index = 0;
			}
		}
		else
		{
			if (value == null)
			{
				return;
			}
			if (m_Free.Count > 0)
			{
				index = m_Free.Dequeue();
				int index4 = index / m_ChunkSize;
				int num3 = index % m_ChunkSize;
				m_Chunks[index4][num3] = value;
				return;
			}
			index = m_Length++;
			if (index >= m_Chunks.Count * m_ChunkSize)
			{
				m_Chunks.Add(new T[m_ChunkSize]);
			}
			int index5 = index / m_ChunkSize;
			int num4 = index % m_ChunkSize;
			m_Chunks[index5][num4] = value;
		}
	}
}
