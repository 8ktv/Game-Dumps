#define UNITY_ASSERTIONS
using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements.Layout;

internal struct LayoutDataStore : IDisposable
{
	private struct Chunk
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe byte* Buffer;
	}

	internal struct ComponentDataStore : IDisposable
	{
		public readonly MemoryLabel MemoryLabel;

		public int Size;

		public int ComponentCountPerChunk;

		public int ChunkCount;

		[NativeDisableUnsafePtrRestriction]
		private unsafe Chunk* m_Chunks;

		public unsafe ComponentDataStore(int size, MemoryLabel allocLabel)
		{
			Size = size;
			ComponentCountPerChunk = 32768 / size;
			ChunkCount = 0;
			MemoryLabel = allocLabel;
			m_Chunks = null;
		}

		public unsafe void Dispose()
		{
			if (null != m_Chunks)
			{
				for (int i = 0; i < ChunkCount; i++)
				{
					UnsafeUtility.Free(m_Chunks[i].Buffer, MemoryLabel);
				}
				UnsafeUtility.Free(m_Chunks, MemoryLabel);
				ChunkCount = 0;
				m_Chunks = null;
			}
		}

		public unsafe byte* GetComponentDataPtr(int index)
		{
			int num = index / ComponentCountPerChunk;
			int num2 = index % ComponentCountPerChunk;
			return m_Chunks[num].Buffer + num2 * Size;
		}

		public unsafe void ResizeCapacity(int capacity)
		{
			int num = capacity / ComponentCountPerChunk + 1;
			if (num > ChunkCount)
			{
				m_Chunks = (Chunk*)ResizeArray(m_Chunks, ChunkCount, num, UnsafeUtility.SizeOf<Chunk>(), UnsafeUtility.AlignOf<Chunk>(), MemoryLabel);
				for (int i = ChunkCount; i < num; i++)
				{
					m_Chunks[i] = new Chunk
					{
						Buffer = (byte*)UnsafeUtility.Malloc(32768L, 4, MemoryLabel)
					};
				}
			}
			else if (num < ChunkCount)
			{
				for (int num2 = ChunkCount - 1; num2 >= num; num2--)
				{
					UnsafeUtility.Free(m_Chunks[num2].Buffer, MemoryLabel);
				}
				m_Chunks = (Chunk*)ResizeArray(m_Chunks, ChunkCount, num, UnsafeUtility.SizeOf<Chunk>(), UnsafeUtility.AlignOf<Chunk>(), MemoryLabel);
			}
			ChunkCount = num;
		}
	}

	private struct Data
	{
		public int Capacity;

		public int NextFreeIndex;

		public int ComponentCount;

		[NativeDisableUnsafePtrRestriction]
		public unsafe int* Versions;

		[NativeDisableUnsafePtrRestriction]
		public unsafe ComponentDataStore* Components;
	}

	private const int k_ChunkSize = 32768;

	private readonly MemoryLabel m_MemoryLabel;

	[NativeDisableUnsafePtrRestriction]
	private unsafe Data* m_Data;

	public unsafe bool IsValid => null != m_Data;

	public unsafe int Capacity => m_Data->Capacity;

	public unsafe LayoutDataStore(ComponentType[] components, ReadOnlySpan<MemoryLabel> labels, int initialCapacity, Allocator allocator)
	{
		Assert.IsTrue(components.Length != 0, "LayoutDataStore requires at least one component size.");
		Assert.IsTrue(components[0].Size >= 4, string.Format("{0} requires a minimum element size of {1} to alias", "LayoutDataStore", 4));
		Assert.AreEqual(components.Length, labels.Length, "Expected a matching number of component names and components.");
		m_MemoryLabel = new MemoryLabel("UIElements", "Layout.LayoutDataStore", allocator);
		m_Data = (Data*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Data>(), UnsafeUtility.AlignOf<Data>(), m_MemoryLabel);
		UnsafeUtility.MemClear(m_Data, UnsafeUtility.SizeOf<Data>());
		m_Data->ComponentCount = components.Length;
		m_Data->Components = (ComponentDataStore*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<ComponentDataStore>() * components.Length, UnsafeUtility.AlignOf<ComponentDataStore>(), m_MemoryLabel);
		for (int i = 0; i < components.Length; i++)
		{
			m_Data->Components[i] = new ComponentDataStore(components[i].Size, labels[i]);
		}
		ResizeCapacity(initialCapacity);
		m_Data->NextFreeIndex = 0;
	}

	public unsafe void Dispose()
	{
		for (int i = 0; i < m_Data->ComponentCount; i++)
		{
			m_Data->Components[i].Dispose();
		}
		UnsafeUtility.Free(m_Data->Versions, m_MemoryLabel);
		UnsafeUtility.Free(m_Data->Components, m_MemoryLabel);
		UnsafeUtility.Free(m_Data, m_MemoryLabel);
		m_Data = null;
	}

	public unsafe bool Exists(in LayoutHandle handle)
	{
		if ((uint)handle.Index >= m_Data->Capacity)
		{
			return false;
		}
		return m_Data->Versions[handle.Index] == handle.Version;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe readonly void* GetComponentDataPtr(int index, int componentIndex)
	{
		return m_Data->Components[componentIndex].GetComponentDataPtr(index);
	}

	private unsafe LayoutHandle Allocate(byte** data, int count)
	{
		int nextFreeIndex = m_Data->NextFreeIndex;
		int nextFreeIndex2 = GetNextFreeIndex(m_Data->Components, nextFreeIndex);
		if (nextFreeIndex2 == -1)
		{
			IncreaseCapacity();
			nextFreeIndex2 = GetNextFreeIndex(m_Data->Components, nextFreeIndex);
		}
		int version = m_Data->Versions[nextFreeIndex];
		m_Data->NextFreeIndex = nextFreeIndex2;
		Debug.Assert(m_Data->ComponentCount == count, "All components must be initialized");
		Debug.Assert(data != null);
		for (int i = 0; i < count; i++)
		{
			Debug.Assert(data[i] != null);
			byte* componentDataPtr = m_Data->Components[i].GetComponentDataPtr(nextFreeIndex);
			UnsafeUtility.MemCpy(componentDataPtr, data[i], m_Data->Components[i].Size);
		}
		return new LayoutHandle(nextFreeIndex, version);
	}

	public unsafe void Free(in LayoutHandle handle)
	{
		if (!Exists(in handle))
		{
			throw new InvalidOperationException($"Failed to Free handle with Index={handle.Index} Version={handle.Version}");
		}
		m_Data->Versions[handle.Index]++;
		SetNextFreeIndex(m_Data->Components, handle.Index, m_Data->NextFreeIndex);
		m_Data->NextFreeIndex = handle.Index;
	}

	private unsafe static void SetNextFreeIndex(ComponentDataStore* ptr, int index, int value)
	{
		*(int*)ptr->GetComponentDataPtr(index) = value;
	}

	private unsafe static int GetNextFreeIndex(ComponentDataStore* ptr, int index)
	{
		return *(int*)ptr->GetComponentDataPtr(index);
	}

	private unsafe void IncreaseCapacity()
	{
		ResizeCapacity((int)((float)m_Data->Capacity * 1.5f));
	}

	private unsafe void ResizeCapacity(int capacity)
	{
		Assert.IsTrue(capacity > 0);
		m_Data->Versions = (int*)ResizeArray(m_Data->Versions, m_Data->Capacity, capacity, 4L, 4, m_MemoryLabel);
		for (int i = 0; i < m_Data->ComponentCount; i++)
		{
			m_Data->Components[i].ResizeCapacity(capacity);
		}
		int num = ((m_Data->Capacity > 0) ? (m_Data->Capacity - 1) : 0);
		for (int j = num; j < capacity; j++)
		{
			m_Data->Versions[j] = 1;
			SetNextFreeIndex(m_Data->Components, j, j + 1);
		}
		SetNextFreeIndex(m_Data->Components, capacity - 1, -1);
		m_Data->Capacity = capacity;
	}

	private unsafe static void* ResizeArray(void* fromPtr, long fromCount, long toCount, long size, int align, MemoryLabel label)
	{
		Assert.IsTrue(toCount > 0);
		void* ptr = UnsafeUtility.Malloc(size * toCount, align, label);
		Assert.IsTrue(ptr != null);
		if (fromCount <= 0)
		{
			return ptr;
		}
		long num = ((toCount < fromCount) ? toCount : fromCount);
		long size2 = num * size;
		UnsafeUtility.MemCpy(ptr, fromPtr, size2);
		UnsafeUtility.Free(fromPtr, label);
		return ptr;
	}

	public unsafe LayoutHandle Allocate<T0>(in T0 component0) where T0 : unmanaged
	{
		fixed (T0* ptr = &component0)
		{
			byte** ptr2 = stackalloc byte*[1];
			*ptr2 = (byte*)ptr;
			return Allocate(ptr2, 1);
		}
	}

	public unsafe LayoutHandle Allocate<T0, T1, T2>(in T0 component0, in T1 component1, in T2 component2) where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged
	{
		fixed (T0* ptr = &component0)
		{
			fixed (T1* ptr2 = &component1)
			{
				fixed (T2* ptr3 = &component2)
				{
					byte** ptr4 = stackalloc byte*[3];
					*ptr4 = (byte*)ptr;
					ptr4[1] = (byte*)ptr2;
					ptr4[2] = (byte*)ptr3;
					return Allocate(ptr4, 3);
				}
			}
		}
	}

	public unsafe LayoutHandle Allocate<T0, T1, T2, T3>(in T0 component0, in T1 component1, in T2 component2, in T3 component3) where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged
	{
		fixed (T0* ptr = &component0)
		{
			fixed (T1* ptr2 = &component1)
			{
				fixed (T2* ptr3 = &component2)
				{
					fixed (T3* ptr4 = &component3)
					{
						byte** ptr5 = stackalloc byte*[4];
						*ptr5 = (byte*)ptr;
						ptr5[1] = (byte*)ptr2;
						ptr5[2] = (byte*)ptr3;
						ptr5[3] = (byte*)ptr4;
						return Allocate(ptr5, 4);
					}
				}
			}
		}
	}

	public unsafe LayoutHandle Allocate<T0, T1, T2, T3, T4>(in T0 component0, in T1 component1, in T2 component2, in T3 component3, in T4 component4) where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged
	{
		fixed (T0* ptr = &component0)
		{
			fixed (T1* ptr2 = &component1)
			{
				fixed (T2* ptr3 = &component2)
				{
					fixed (T3* ptr4 = &component3)
					{
						fixed (T4* ptr5 = &component4)
						{
							byte** ptr6 = stackalloc byte*[5];
							*ptr6 = (byte*)ptr;
							ptr6[1] = (byte*)ptr2;
							ptr6[2] = (byte*)ptr3;
							ptr6[3] = (byte*)ptr4;
							ptr6[4] = (byte*)ptr5;
							return Allocate(ptr6, 5);
						}
					}
				}
			}
		}
	}

	public unsafe LayoutHandle Allocate<T0, T1, T2, T3, T4, T5>(in T0 component0, in T1 component1, in T2 component2, in T3 component3, in T4 component4, in T5 component5) where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged
	{
		fixed (T0* ptr = &component0)
		{
			fixed (T1* ptr2 = &component1)
			{
				fixed (T2* ptr3 = &component2)
				{
					fixed (T3* ptr4 = &component3)
					{
						fixed (T4* ptr5 = &component4)
						{
							fixed (T5* ptr6 = &component5)
							{
								byte** ptr7 = stackalloc byte*[6];
								*ptr7 = (byte*)ptr;
								ptr7[1] = (byte*)ptr2;
								ptr7[2] = (byte*)ptr3;
								ptr7[3] = (byte*)ptr4;
								ptr7[4] = (byte*)ptr5;
								ptr7[5] = (byte*)ptr6;
								return Allocate(ptr7, 6);
							}
						}
					}
				}
			}
		}
	}

	public unsafe LayoutHandle Allocate<T0, T1, T2, T3, T4, T5, T6>(in T0 component0, in T1 component1, in T2 component2, in T3 component3, in T4 component4, in T5 component5, in T6 component6) where T0 : unmanaged where T1 : unmanaged where T2 : unmanaged where T3 : unmanaged where T4 : unmanaged where T5 : unmanaged where T6 : unmanaged
	{
		fixed (T0* ptr = &component0)
		{
			fixed (T1* ptr2 = &component1)
			{
				fixed (T2* ptr3 = &component2)
				{
					fixed (T3* ptr4 = &component3)
					{
						fixed (T4* ptr5 = &component4)
						{
							fixed (T5* ptr6 = &component5)
							{
								fixed (T6* ptr7 = &component6)
								{
									byte** ptr8 = stackalloc byte*[7];
									*ptr8 = (byte*)ptr;
									ptr8[1] = (byte*)ptr2;
									ptr8[2] = (byte*)ptr3;
									ptr8[3] = (byte*)ptr4;
									ptr8[4] = (byte*)ptr5;
									ptr8[5] = (byte*)ptr6;
									ptr8[6] = (byte*)ptr7;
									return Allocate(ptr8, 7);
								}
							}
						}
					}
				}
			}
		}
	}
}
