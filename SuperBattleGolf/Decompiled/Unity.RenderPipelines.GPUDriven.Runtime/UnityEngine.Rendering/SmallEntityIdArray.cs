using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering;

internal struct SmallEntityIdArray : IDisposable
{
	private FixedList32Bytes<EntityId> m_FixedArray;

	private UnsafeList<EntityId> m_List;

	private readonly bool m_IsEmbedded;

	public readonly int Length;

	public bool Valid { get; private set; }

	public EntityId this[int index]
	{
		get
		{
			if (m_IsEmbedded)
			{
				return m_FixedArray[index];
			}
			return m_List[index];
		}
		set
		{
			if (m_IsEmbedded)
			{
				m_FixedArray[index] = value;
			}
			else
			{
				m_List[index] = value;
			}
		}
	}

	public SmallEntityIdArray(int length, Allocator allocator)
	{
		m_FixedArray = default(FixedList32Bytes<EntityId>);
		m_List = default(UnsafeList<EntityId>);
		Length = length;
		Valid = true;
		if (Length <= m_FixedArray.Capacity)
		{
			m_FixedArray = default(FixedList32Bytes<EntityId>);
			m_FixedArray.Length = Length;
			m_IsEmbedded = true;
		}
		else
		{
			m_List = new UnsafeList<EntityId>(Length, allocator);
			m_List.Resize(Length);
			m_IsEmbedded = false;
		}
	}

	public void Dispose()
	{
		if (Valid)
		{
			m_List.Dispose();
			Valid = false;
		}
	}
}
