using System;
using JetBrains.Annotations;

namespace UnityEngine.UIElements;

internal class EventCallbackList
{
	public static readonly EventCallbackList EmptyList = new EventCallbackList();

	private static readonly EventCallbackFunctorBase[] EmptyArray = new EventCallbackFunctorBase[0];

	private EventCallbackFunctorBase[] m_Array;

	private int m_Count;

	public int Count => m_Count;

	public Span<EventCallbackFunctorBase> Span => new Span<EventCallbackFunctorBase>(m_Array, 0, m_Count);

	public EventCallbackFunctorBase this[int i]
	{
		get
		{
			return m_Array[i];
		}
		set
		{
			m_Array[i] = value;
		}
	}

	public EventCallbackList()
	{
		m_Array = EmptyArray;
	}

	public EventCallbackList(EventCallbackList source)
	{
		m_Count = source.m_Count;
		m_Array = new EventCallbackFunctorBase[m_Count];
		Array.Copy(source.m_Array, m_Array, m_Count);
	}

	public bool Contains(long eventTypeId, [NotNull] Delegate callback)
	{
		return Find(eventTypeId, callback) != null;
	}

	public EventCallbackFunctorBase Find(long eventTypeId, [NotNull] Delegate callback)
	{
		for (int i = 0; i < m_Count; i++)
		{
			if (m_Array[i].IsEquivalentTo(eventTypeId, callback))
			{
				return m_Array[i];
			}
		}
		return null;
	}

	public bool Remove(long eventTypeId, [NotNull] Delegate callback, out EventCallbackFunctorBase removedFunctor)
	{
		for (int i = 0; i < m_Count; i++)
		{
			if (m_Array[i].IsEquivalentTo(eventTypeId, callback))
			{
				removedFunctor = m_Array[i];
				m_Count--;
				Array.Copy(m_Array, i + 1, m_Array, i, m_Count - i);
				m_Array[m_Count] = null;
				return true;
			}
		}
		removedFunctor = null;
		return false;
	}

	public void Add(EventCallbackFunctorBase item)
	{
		if (m_Count >= m_Array.Length)
		{
			Array.Resize(ref m_Array, Mathf.NextPowerOfTwo(m_Count + 4));
		}
		m_Array[m_Count++] = item;
	}

	public void AddRange(EventCallbackList list)
	{
		if (m_Count + list.m_Count > m_Array.Length)
		{
			Array.Resize(ref m_Array, Mathf.NextPowerOfTwo(m_Count + list.m_Count));
		}
		Array.Copy(list.m_Array, 0, m_Array, m_Count, list.m_Count);
		m_Count += list.m_Count;
	}

	public void Clear()
	{
		Array.Clear(m_Array, 0, m_Count);
		m_Count = 0;
	}
}
