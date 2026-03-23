#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.UIR;

internal class ImplicitPool<T> where T : class
{
	private readonly int m_StartCapacity;

	private readonly int m_MaxCapacity;

	private Func<T> m_CreateAction;

	private Action<T> m_ResetAction;

	private List<T> m_List;

	private int m_UsedCount;

	public ImplicitPool(Func<T> createAction, Action<T> resetAction, int startCapacity, int maxCapacity)
	{
		Debug.Assert(createAction != null);
		Debug.Assert(startCapacity > 0);
		Debug.Assert(startCapacity <= maxCapacity);
		Debug.Assert(maxCapacity > 0);
		m_List = new List<T>(0);
		m_StartCapacity = startCapacity;
		m_MaxCapacity = maxCapacity;
		m_CreateAction = createAction;
		m_ResetAction = resetAction;
	}

	public T Get()
	{
		if (m_UsedCount < m_List.Count)
		{
			return m_List[m_UsedCount++];
		}
		if (m_UsedCount < m_MaxCapacity)
		{
			int b = Mathf.Max(m_StartCapacity, m_UsedCount);
			int a = m_MaxCapacity - m_UsedCount;
			int num = Mathf.Min(a, b);
			m_List.Capacity = m_UsedCount + num;
			T val = m_CreateAction();
			m_List.Add(val);
			m_UsedCount++;
			for (int i = 1; i < num; i++)
			{
				m_List.Add(m_CreateAction());
			}
			return val;
		}
		return m_CreateAction();
	}

	public void ReturnAll()
	{
		Debug.Assert(m_List.Count <= m_MaxCapacity);
		if (m_ResetAction != null)
		{
			for (int i = 0; i < m_UsedCount; i++)
			{
				m_ResetAction(m_List[i]);
			}
		}
		m_UsedCount = 0;
	}
}
