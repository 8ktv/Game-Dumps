#define UNITY_ASSERTIONS
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements.Layout;

internal struct LayoutList<T> : IDisposable where T : unmanaged
{
	private struct Data
	{
		public int Capacity;

		public int Count;

		public unsafe T* Values;
	}

	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private LayoutList<T> m_List;

		private int m_Index;

		private T m_Current;

		public T Current => m_Current;

		object IEnumerator.Current => m_Current;

		public Enumerator(LayoutList<T> list)
		{
			m_List = list;
			m_Index = 0;
			m_Current = default(T);
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (!m_List.IsCreated)
			{
				m_Current = default(T);
				return false;
			}
			if ((uint)m_Index >= m_List.Count)
			{
				m_Current = default(T);
				return false;
			}
			m_Current = m_List[m_Index];
			m_Index++;
			return true;
		}

		public void Reset()
		{
			m_Index = 0;
		}
	}

	private readonly Allocator m_Allocator;

	private unsafe Data* m_Data;

	public unsafe int Count => m_Data->Count;

	public unsafe bool IsCreated => null != m_Data;

	public unsafe ref T this[int index]
	{
		get
		{
			if ((uint)index > m_Data->Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			return ref m_Data->Values[index];
		}
	}

	public unsafe LayoutList()
	{
		m_Data = null;
		m_Allocator = Allocator.Invalid;
	}

	public unsafe LayoutList(int initialCapacity, Allocator allocator)
	{
		m_Allocator = allocator;
		m_Data = (Data*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Data>(), 16, allocator);
		Assert.IsTrue(m_Data != null);
		UnsafeUtility.MemClear(m_Data, UnsafeUtility.SizeOf<Data>());
		ResizeCapacity(initialCapacity);
	}

	public unsafe void Dispose()
	{
		if (null != m_Data)
		{
			if (m_Data->Values != null)
			{
				UnsafeUtility.Free(m_Data->Values, m_Allocator);
			}
			UnsafeUtility.Free(m_Data, m_Allocator);
			m_Data = null;
		}
	}

	public unsafe void Insert(int index, T value)
	{
		if ((uint)index > m_Data->Count)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (m_Data->Capacity == m_Data->Count)
		{
			IncreaseCapacity();
		}
		if (index < m_Data->Count)
		{
			UnsafeUtility.MemMove(m_Data->Values + index + 1, m_Data->Values + index, UnsafeUtility.SizeOf<T>() * (m_Data->Count - index));
		}
		m_Data->Values[index] = value;
		m_Data->Count++;
	}

	public unsafe int IndexOf(T value)
	{
		int count = m_Data->Count;
		T* ptr = &value;
		T* ptr2 = m_Data->Values;
		int num = UnsafeUtility.SizeOf<T>();
		int num2 = 0;
		while (num2 < count)
		{
			if (UnsafeUtility.MemCmp(ptr2, ptr, num) == 0)
			{
				return num2;
			}
			num2++;
			ptr2++;
		}
		return -1;
	}

	public unsafe void RemoveAt(int index)
	{
		if ((uint)index >= m_Data->Count)
		{
			throw new ArgumentOutOfRangeException();
		}
		m_Data->Count--;
		UnsafeUtility.MemMove(m_Data->Values + index, m_Data->Values + index + 1, UnsafeUtility.SizeOf<T>() * (m_Data->Count - index));
		m_Data->Values[m_Data->Count] = default(T);
	}

	public unsafe void Clear()
	{
		m_Data->Count = 0;
	}

	private unsafe void IncreaseCapacity()
	{
		EnsureCapacity(m_Data->Capacity * 2);
	}

	private unsafe void EnsureCapacity(int capacity)
	{
		if (capacity > m_Data->Capacity)
		{
			ResizeCapacity(capacity);
		}
	}

	private unsafe void ResizeCapacity(int capacity)
	{
		Assert.IsTrue(capacity > 0);
		m_Data->Values = (T*)ResizeArray(m_Data->Values, m_Data->Capacity, capacity, UnsafeUtility.SizeOf<T>(), 16, m_Allocator);
		m_Data->Capacity = capacity;
	}

	private unsafe static void* ResizeArray(void* fromPtr, long fromCount, long toCount, long size, int align, Allocator allocator)
	{
		Assert.IsTrue(toCount > 0);
		void* ptr = UnsafeUtility.Malloc(size * toCount, align, allocator);
		Assert.IsTrue(ptr != null);
		if (fromCount <= 0)
		{
			return ptr;
		}
		long num = ((toCount < fromCount) ? toCount : fromCount);
		long size2 = num * size;
		UnsafeUtility.MemCpy(ptr, fromPtr, size2);
		UnsafeUtility.Free(fromPtr, allocator);
		return ptr;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
}
