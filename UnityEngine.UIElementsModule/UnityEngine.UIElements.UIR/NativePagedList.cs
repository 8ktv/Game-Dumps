#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR;

internal class NativePagedList<T> : IDisposable where T : struct
{
	public struct Enumerator
	{
		private NativePagedList<T> m_NativePagedList;

		private NativeArray<T> m_CurrentPage;

		private int m_IndexInCurrentPage;

		private int m_IndexOfCurrentPage;

		private int m_CountInCurrentPage;

		public Enumerator(NativePagedList<T> nativePagedList, int offset)
		{
			m_IndexInCurrentPage = 0;
			m_IndexOfCurrentPage = 0;
			m_CountInCurrentPage = 0;
			m_NativePagedList = nativePagedList;
			for (int i = 0; i < m_NativePagedList.m_Pages.Count - 1; i++)
			{
				m_CountInCurrentPage = m_NativePagedList.m_Pages[i].Length;
				if (offset >= m_CountInCurrentPage)
				{
					offset -= m_CountInCurrentPage;
					continue;
				}
				m_IndexInCurrentPage = offset;
				m_IndexOfCurrentPage = i;
				m_CurrentPage = m_NativePagedList.m_Pages[m_IndexOfCurrentPage];
				return;
			}
			m_IndexOfCurrentPage = m_NativePagedList.m_Pages.Count - 1;
			m_CountInCurrentPage = m_NativePagedList.m_CountInLastPage;
			m_IndexInCurrentPage = offset;
			m_CurrentPage = m_NativePagedList.m_LastPage;
		}

		public bool HasNext()
		{
			return m_IndexInCurrentPage < m_CountInCurrentPage;
		}

		public T GetNext()
		{
			if (!HasNext())
			{
				throw new InvalidOperationException("No more elements");
			}
			T result = m_CurrentPage[m_IndexInCurrentPage];
			m_IndexInCurrentPage++;
			if (m_IndexInCurrentPage == m_CountInCurrentPage)
			{
				m_IndexInCurrentPage = 0;
				m_IndexOfCurrentPage++;
				int count = m_NativePagedList.m_Pages.Count;
				if (m_IndexOfCurrentPage < count)
				{
					if (m_IndexOfCurrentPage < count - 1)
					{
						m_CountInCurrentPage = m_NativePagedList.m_Pages[m_IndexOfCurrentPage].Length;
					}
					else
					{
						m_CountInCurrentPage = m_NativePagedList.m_CountInLastPage;
					}
				}
				else
				{
					m_IndexOfCurrentPage = count - 1;
					m_CountInCurrentPage = m_NativePagedList.m_CountInLastPage;
					m_IndexInCurrentPage = m_CountInCurrentPage;
				}
				m_CurrentPage = m_NativePagedList.m_Pages[m_IndexOfCurrentPage];
			}
			return result;
		}
	}

	private readonly int k_PoolCapacity;

	private List<NativeArray<T>> m_Pages = new List<NativeArray<T>>(8);

	private NativeArray<T> m_LastPage;

	private int m_CountInLastPage;

	private Allocator m_FirstPageAllocator;

	private Allocator m_OtherPagesAllocator;

	private List<NativeSlice<T>> m_Enumerator = new List<NativeSlice<T>>(8);

	protected bool disposed { get; private set; }

	public NativePagedList(int poolCapacity, Allocator firstPageAllocator = Allocator.Persistent, Allocator otherPagesAllocator = Allocator.Persistent)
	{
		Debug.Assert(poolCapacity > 0);
		k_PoolCapacity = Mathf.NextPowerOfTwo(poolCapacity);
		m_FirstPageAllocator = firstPageAllocator;
		m_OtherPagesAllocator = otherPagesAllocator;
	}

	public void Add(ref T data)
	{
		if (m_CountInLastPage < m_LastPage.Length)
		{
			m_LastPage[m_CountInLastPage++] = data;
			return;
		}
		int length = ((m_Pages.Count > 0) ? (m_LastPage.Length << 1) : k_PoolCapacity);
		Allocator allocator = ((m_Pages.Count == 0) ? m_FirstPageAllocator : m_OtherPagesAllocator);
		m_LastPage = new NativeArray<T>(length, allocator, NativeArrayOptions.UninitializedMemory);
		m_Pages.Add(m_LastPage);
		m_LastPage[0] = data;
		m_CountInLastPage = 1;
	}

	public void Add(T data)
	{
		Add(ref data);
	}

	public List<NativeSlice<T>> GetPages()
	{
		m_Enumerator.Clear();
		if (m_Pages.Count > 0)
		{
			int num = m_Pages.Count - 1;
			for (int i = 0; i < num; i++)
			{
				m_Enumerator.Add(m_Pages[i]);
			}
			if (m_CountInLastPage > 0)
			{
				m_Enumerator.Add(m_LastPage.Slice(0, m_CountInLastPage));
			}
		}
		return m_Enumerator;
	}

	public int GetCount()
	{
		int num = m_CountInLastPage;
		for (int i = 0; i < m_Pages.Count - 1; i++)
		{
			num += m_Pages[i].Length;
		}
		return num;
	}

	public void Reset()
	{
		if (m_Pages.Count > 1)
		{
			m_LastPage = m_Pages[0];
			for (int i = 1; i < m_Pages.Count; i++)
			{
				m_Pages[i].Dispose();
			}
			m_Pages.Clear();
			m_Pages.Add(m_LastPage);
		}
		m_CountInLastPage = 0;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			for (int i = 0; i < m_Pages.Count; i++)
			{
				m_Pages[i].Dispose();
			}
			m_Pages.Clear();
			m_CountInLastPage = 0;
		}
		disposed = true;
	}
}
