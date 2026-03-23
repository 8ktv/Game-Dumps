using System;
using System.Collections.Generic;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR;

internal class EntryPool
{
	private const int k_StackSize = 128;

	private Stack<Entry>[] m_ThreadEntries;

	private ImplicitPool<Entry> m_SharedPool;

	private static readonly Func<Entry> k_CreateAction = () => new Entry();

	private static readonly Action<Entry> k_ResetAction = delegate(Entry e)
	{
		e.Reset();
	};

	public EntryPool(int maxCapacity = 1024)
	{
		m_ThreadEntries = new Stack<Entry>[JobsUtility.ThreadIndexCount];
		int i = 0;
		for (int threadIndexCount = JobsUtility.ThreadIndexCount; i < threadIndexCount; i++)
		{
			m_ThreadEntries[i] = new Stack<Entry>(128);
		}
		m_SharedPool = new ImplicitPool<Entry>(k_CreateAction, k_ResetAction, 128, maxCapacity);
	}

	public Entry Get()
	{
		Stack<Entry> stack = m_ThreadEntries[UIRUtility.GetThreadIndex()];
		if (stack.Count == 0)
		{
			lock (m_SharedPool)
			{
				for (int i = 0; i < 128; i++)
				{
					stack.Push(m_SharedPool.Get());
				}
			}
		}
		return stack.Pop();
	}

	public void ReturnAll()
	{
		int i = 0;
		for (int num = m_ThreadEntries.Length; i < num; i++)
		{
			m_ThreadEntries[i].Clear();
		}
		m_SharedPool.ReturnAll();
	}
}
