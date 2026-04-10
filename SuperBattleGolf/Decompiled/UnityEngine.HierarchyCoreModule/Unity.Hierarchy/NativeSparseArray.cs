using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Hierarchy;

internal struct NativeSparseArray<TKey, TValue> : IDisposable where TKey : unmanaged, IEquatable<TKey> where TValue : unmanaged
{
	private readonly struct Pair
	{
		public readonly TKey Key;

		public readonly TValue Value;

		public Pair(in TKey key, in TValue value)
		{
			Key = key;
			Value = value;
		}
	}

	public delegate int KeyIndex(in TKey key);

	public delegate bool KeyEqual(in TKey lhs, in TKey rhs);

	private unsafe Pair* m_Ptr;

	private int m_Capacity;

	private int m_Count;

	private readonly Allocator m_Allocator;

	private readonly Pair m_InitValue;

	private readonly KeyIndex m_KeyIndex;

	private readonly KeyEqual m_KeyEqual;

	public unsafe bool IsCreated
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_Ptr != null;
		}
	}

	public int Capacity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_Capacity;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Allocate(value);
		}
	}

	public int Count => m_Count;

	public unsafe TValue this[in TKey key]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			int num = m_KeyIndex(in key);
			ThrowIfIndexOutOfRange(num);
			ref Pair reference = ref m_Ptr[num];
			if (!m_KeyEqual(in reference.Key, in key))
			{
				throw new KeyNotFoundException(key.ToString());
			}
			return reference.Value;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			int num = m_KeyIndex(in key);
			ThrowIfIndexIsNegative(num);
			EnsureCapacity(num + 1, NativeSparseArrayResizePolicy.ExactSize);
			ref Pair reference = ref m_Ptr[num];
			if (m_KeyEqual(in reference.Key, default(TKey)))
			{
				m_Count++;
			}
			m_Ptr[num] = new Pair(in key, in value);
		}
	}

	public unsafe NativeSparseArray(KeyIndex keyIndex, Allocator allocator)
	{
		m_Ptr = null;
		m_Capacity = 0;
		m_Count = 0;
		m_Allocator = allocator;
		m_InitValue = default(Pair);
		m_KeyIndex = keyIndex;
		m_KeyEqual = delegate(in TKey lhs, in TKey rhs)
		{
			return lhs.Equals(rhs);
		};
	}

	public unsafe NativeSparseArray(KeyIndex keyIndex, KeyEqual keyEqual, Allocator allocator)
	{
		m_Ptr = null;
		m_Capacity = 0;
		m_Count = 0;
		m_Allocator = allocator;
		m_InitValue = default(Pair);
		m_KeyIndex = keyIndex;
		m_KeyEqual = keyEqual;
	}

	public unsafe NativeSparseArray(in TValue initValue, KeyIndex keyIndex, Allocator allocator)
	{
		m_Ptr = null;
		m_Capacity = 0;
		m_Count = 0;
		m_Allocator = allocator;
		m_InitValue = new Pair(default(TKey), in initValue);
		m_KeyIndex = keyIndex;
		m_KeyEqual = delegate(in TKey lhs, in TKey rhs)
		{
			return lhs.Equals(rhs);
		};
	}

	public unsafe NativeSparseArray(in TValue initValue, KeyIndex keyIndex, KeyEqual keyEqual, Allocator allocator)
	{
		m_Ptr = null;
		m_Capacity = 0;
		m_Count = 0;
		m_Allocator = allocator;
		m_InitValue = new Pair(default(TKey), in initValue);
		m_KeyIndex = keyIndex;
		m_KeyEqual = keyEqual;
	}

	public void Dispose()
	{
		Deallocate();
	}

	public void Reserve(int capacity)
	{
		EnsureCapacity(capacity, NativeSparseArrayResizePolicy.ExactSize);
	}

	public unsafe bool ContainsKey(in TKey key)
	{
		int num = m_KeyIndex(in key);
		ThrowIfIndexOutOfRange(num);
		ref Pair reference = ref m_Ptr[num];
		return m_KeyEqual(in reference.Key, in key);
	}

	public unsafe void Add(in TKey key, in TValue value, NativeSparseArrayResizePolicy policy = NativeSparseArrayResizePolicy.ExactSize)
	{
		int num = m_KeyIndex(in key);
		ThrowIfIndexIsNegative(num);
		EnsureCapacity(num + 1, policy);
		ref Pair reference = ref m_Ptr[num];
		if (m_KeyEqual(in reference.Key, in key))
		{
			throw new ArgumentException($"an element with the same key [{key}] already exists");
		}
		if (m_KeyEqual(in reference.Key, default(TKey)))
		{
			m_Count++;
		}
		m_Ptr[num] = new Pair(in key, in value);
	}

	public unsafe void AddNoResize(in TKey key, in TValue value)
	{
		int num = m_KeyIndex(in key);
		ThrowIfIndexOutOfRange(num);
		ref Pair reference = ref m_Ptr[num];
		if (m_KeyEqual(in reference.Key, in key))
		{
			throw new ArgumentException($"an element with the same key [{key}] already exists");
		}
		if (m_KeyEqual(in reference.Key, default(TKey)))
		{
			m_Count++;
		}
		m_Ptr[num] = new Pair(in key, in value);
	}

	public unsafe bool TryAdd(in TKey key, in TValue value, NativeSparseArrayResizePolicy policy = NativeSparseArrayResizePolicy.ExactSize)
	{
		int num = m_KeyIndex(in key);
		ThrowIfIndexIsNegative(num);
		EnsureCapacity(num + 1, policy);
		ref Pair reference = ref m_Ptr[num];
		if (m_KeyEqual(in reference.Key, in key))
		{
			return false;
		}
		if (m_KeyEqual(in reference.Key, default(TKey)))
		{
			m_Count++;
		}
		m_Ptr[num] = new Pair(in key, in value);
		return true;
	}

	public unsafe bool TryAddNoResize(in TKey key, in TValue value)
	{
		int num = m_KeyIndex(in key);
		ThrowIfIndexOutOfRange(num);
		ref Pair reference = ref m_Ptr[num];
		if (m_KeyEqual(in reference.Key, in key))
		{
			return false;
		}
		if (m_KeyEqual(in reference.Key, default(TKey)))
		{
			m_Count++;
		}
		m_Ptr[num] = new Pair(in key, in value);
		return true;
	}

	public unsafe bool TryGetValue(in TKey key, out TValue value)
	{
		int num = m_KeyIndex(in key);
		ThrowIfIndexOutOfRange(num);
		ref Pair reference = ref m_Ptr[num];
		if (m_KeyEqual(in reference.Key, in key))
		{
			value = reference.Value;
			return true;
		}
		value = default(TValue);
		return false;
	}

	public unsafe bool Remove(in TKey key)
	{
		int num = m_KeyIndex(in key);
		ThrowIfIndexOutOfRange(num);
		ref Pair reference = ref m_Ptr[num];
		if (!m_KeyEqual(in reference.Key, in key))
		{
			return false;
		}
		m_Ptr[num] = m_InitValue;
		m_Count--;
		return true;
	}

	public unsafe void Clear()
	{
		if (m_Ptr != null)
		{
			fixed (Pair* initValue = &m_InitValue)
			{
				void* source = initValue;
				UnsafeUtility.MemCpyReplicate(m_Ptr, source, sizeof(Pair), m_Capacity);
			}
		}
		m_Count = 0;
	}

	private unsafe void Allocate(int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentException($"capacity [{capacity}] cannot be negative");
		}
		int num = sizeof(Pair);
		int alignment = UnsafeUtility.AlignOf<Pair>();
		if (m_Ptr == null)
		{
			m_Ptr = (Pair*)UnsafeUtility.Malloc(capacity * num, alignment, m_Allocator);
			fixed (Pair* initValue = &m_InitValue)
			{
				UnsafeUtility.MemCpyReplicate(m_Ptr, initValue, num, capacity);
			}
		}
		else
		{
			m_Ptr = (Pair*)Realloc(m_Ptr, capacity * num, alignment, m_Allocator);
			if (capacity > m_Capacity)
			{
				fixed (Pair* initValue2 = &m_InitValue)
				{
					UnsafeUtility.MemCpyReplicate(m_Ptr + m_Capacity, initValue2, num, capacity - m_Capacity);
				}
			}
		}
		m_Capacity = capacity;
	}

	private unsafe void Deallocate()
	{
		if (m_Ptr != null)
		{
			UnsafeUtility.Free(m_Ptr, m_Allocator);
			m_Ptr = null;
		}
		m_Capacity = 0;
		m_Count = 0;
	}

	private void EnsureCapacity(int capacity, NativeSparseArrayResizePolicy policy)
	{
		if (capacity > m_Capacity)
		{
			switch (policy)
			{
			case NativeSparseArrayResizePolicy.ExactSize:
				Allocate(capacity);
				break;
			case NativeSparseArrayResizePolicy.DoubleSize:
				Allocate(Math.Max(capacity, m_Capacity * 2));
				break;
			default:
				throw new NotImplementedException(policy.ToString());
			}
		}
	}

	private void ThrowIfIndexIsNegative(int index)
	{
		if (index < 0)
		{
			throw new InvalidOperationException($"key index [{index}] cannot be negative");
		}
	}

	private void ThrowIfIndexOutOfRange(int index)
	{
		ThrowIfIndexIsNegative(index);
		if (index >= m_Capacity)
		{
			throw new InvalidOperationException($"key index [{index}] is out of range [0, {m_Capacity}]");
		}
	}

	private unsafe static void* Realloc(void* ptr, long size, int alignment, Allocator allocator)
	{
		if (ptr == null)
		{
			return UnsafeUtility.Malloc(size, alignment, allocator);
		}
		void* ptr2 = UnsafeUtility.Malloc(size, alignment, allocator);
		UnsafeUtility.MemCpy(ptr2, ptr, size);
		UnsafeUtility.Free(ptr, allocator);
		return ptr2;
	}
}
