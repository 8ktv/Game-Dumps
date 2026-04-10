using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Hierarchy;

[DebuggerTypeProxy(typeof(UnsafeCircularBufferTDebugView<>))]
[DebuggerDisplay("Count = {Count}, Capacity = {Capacity}, IsEmpty = {IsEmpty}")]
internal class CircularBuffer<T>
{
	public struct Enumerator
	{
		private readonly CircularBuffer<T> m_Buffer;

		private int m_Index;

		public T Current => m_Buffer[m_Index];

		internal Enumerator(CircularBuffer<T> buffer)
		{
			m_Buffer = buffer;
			m_Index = -1;
		}

		public bool MoveNext()
		{
			return ++m_Index < m_Buffer.m_Count;
		}

		public void Reset()
		{
			m_Index = -1;
		}
	}

	private T[] m_Buffer;

	private int m_Front;

	private int m_Back;

	private int m_Capacity;

	private int m_Count;

	private bool m_Locked;

	public int Capacity
	{
		get
		{
			return m_Capacity;
		}
		set
		{
			EnsureCapacity(value);
		}
	}

	public int Count => m_Count;

	public bool IsEmpty => m_Count == 0;

	public int FrontIndex => m_Front;

	public int BackIndex => m_Back;

	public bool Locked
	{
		get
		{
			return m_Locked;
		}
		set
		{
			m_Locked = value;
		}
	}

	public T this[int index]
	{
		get
		{
			return m_Buffer[GetIndex(index)];
		}
		set
		{
			m_Buffer[GetIndex(index)] = value;
		}
	}

	public CircularBuffer()
	{
		m_Buffer = Array.Empty<T>();
	}

	public CircularBuffer(int initialCapacity)
	{
		EnsureCapacity(initialCapacity);
	}

	public CircularBuffer(T[] items)
	{
		EnsureCapacity(items.Length);
		Array.Copy(items, m_Buffer, items.Length);
		m_Count = items.Length;
	}

	public T Front()
	{
		ThrowIfEmpty();
		return m_Buffer[m_Front];
	}

	public T Back()
	{
		ThrowIfEmpty();
		return m_Buffer[((m_Back == 0) ? m_Capacity : m_Back) - 1];
	}

	public void PushFront(in T item)
	{
		ThrowIfLocked();
		EnsureCapacity(m_Count + 1);
		m_Front = Modulo(m_Front - 1, m_Capacity);
		m_Buffer[m_Front] = item;
		m_Count++;
	}

	public void PushBack(in T item)
	{
		ThrowIfLocked();
		EnsureCapacity(m_Count + 1);
		m_Buffer[m_Back] = item;
		m_Back = Modulo(m_Back + 1, m_Capacity);
		m_Count++;
	}

	public void PopFront()
	{
		PopFront(1);
	}

	private void PopFront(int count)
	{
		ThrowIfLocked();
		ThrowIfEmpty();
		count = Math.Min(count, m_Count);
		if (m_Buffer[m_Front] is IDisposable disposable)
		{
			disposable.Dispose();
		}
		m_Buffer[m_Front] = default(T);
		m_Front = Modulo(m_Front + count, m_Capacity);
		m_Count -= count;
	}

	public void PopBack()
	{
		PopBack(1);
	}

	private void PopBack(int count)
	{
		ThrowIfLocked();
		ThrowIfEmpty();
		count = Math.Min(count, m_Count);
		if (m_Buffer[m_Back] is IDisposable disposable)
		{
			disposable.Dispose();
		}
		m_Buffer[m_Back] = default(T);
		m_Back = Modulo(m_Back - count, m_Capacity);
		m_Count -= count;
	}

	public void Clear()
	{
		ThrowIfLocked();
		for (int i = 0; i < m_Buffer.Length; i++)
		{
			if (m_Buffer[i] is IDisposable disposable)
			{
				disposable.Dispose();
			}
			m_Buffer[i] = default(T);
		}
		m_Count = 0;
		m_Back = 0;
		m_Front = 0;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	public T[] ToArray()
	{
		T[] array = new T[m_Count];
		for (int i = 0; i < m_Count; i++)
		{
			array[i] = m_Buffer[GetIndex(i)];
		}
		return array;
	}

	private void Allocate(int capacity)
	{
		T[] array = new T[capacity];
		if (m_Count > 0)
		{
			for (int i = 0; i < m_Count; i++)
			{
				array[i] = m_Buffer[GetIndex(i)];
			}
			m_Front = 0;
			m_Back = m_Count;
		}
		m_Buffer = array;
		m_Capacity = capacity;
	}

	private void EnsureCapacity(int capacity)
	{
		if (capacity <= 0)
		{
			throw new ArgumentException("capacity must be greater than zero.");
		}
		int num = Mathf.NextPowerOfTwo(capacity);
		if (num > m_Capacity)
		{
			Allocate(num);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetIndex(int index)
	{
		ThrowIfIndexOutOfRange(index);
		return m_Front + ((index < m_Capacity - m_Front) ? index : (index - m_Capacity));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int Modulo(int x, int y)
	{
		int num = x % y;
		return (num < 0) ? (num + y) : num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ThrowIfEmpty()
	{
		if (IsEmpty)
		{
			throw new InvalidOperationException("Buffer is empty.");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ThrowIfIndexOutOfRange(int index)
	{
		if (IsEmpty)
		{
			throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer is empty.");
		}
		if (index < 0)
		{
			throw new IndexOutOfRangeException($"Cannot access index {index}.");
		}
		if (index >= m_Count)
		{
			throw new IndexOutOfRangeException($"Cannot access index {index}. Buffer count is {m_Count}.");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ThrowIfLocked()
	{
		if (m_Locked)
		{
			throw new InvalidOperationException("Buffer is locked.");
		}
	}
}
