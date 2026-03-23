#define UNITY_ASSERTIONS
using System;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR;

internal class NativeList<T> : IDisposable where T : struct
{
	private NativeArray<T> m_NativeArray;

	private int m_Count;

	public int Count => m_Count;

	protected bool disposed { get; private set; }

	public NativeList(int initialCapacity)
	{
		Debug.Assert(initialCapacity > 0);
		m_NativeArray = new NativeArray<T>(initialCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
	}

	private void Expand(int newLength)
	{
		NativeArray<T> nativeArray = new NativeArray<T>(newLength, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		nativeArray.Slice(0, m_Count).CopyFrom(m_NativeArray);
		m_NativeArray.Dispose();
		m_NativeArray = nativeArray;
	}

	public void Add(ref T data)
	{
		if (m_Count == m_NativeArray.Length)
		{
			Expand(m_NativeArray.Length << 1);
		}
		m_NativeArray[m_Count++] = data;
	}

	public void Add(NativeSlice<T> src)
	{
		int num = m_Count + src.Length;
		if (m_NativeArray.Length < num)
		{
			Expand(num << 1);
		}
		m_NativeArray.Slice(m_Count, src.Length).CopyFrom(src);
		m_Count += src.Length;
	}

	public void Clear()
	{
		m_Count = 0;
	}

	public NativeSlice<T> GetSlice(int start, int length)
	{
		return m_NativeArray.Slice(start, length);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				m_NativeArray.Dispose();
			}
			disposed = true;
		}
	}
}
