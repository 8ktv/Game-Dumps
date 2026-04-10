using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
internal sealed class HierarchyViewColumnContextPool<TPooledObject> where TPooledObject : class
{
	private class ContextPoolImplementation
	{
		public ObjectPool<TPooledObject> Pool { get; private set; }

		public HashSet<TPooledObject> Active { get; private set; } = new HashSet<TPooledObject>();

		public ContextPoolImplementation(Func<TPooledObject> creator)
		{
			Pool = new ObjectPool<TPooledObject>(creator);
		}
	}

	private readonly Func<TPooledObject> m_ObjectCreator;

	private readonly Dictionary<int, ContextPoolImplementation> m_Pools = new Dictionary<int, ContextPoolImplementation>();

	public HierarchyViewColumnContextPool(Func<TPooledObject> objectCreator)
	{
		m_ObjectCreator = objectCreator;
	}

	public TPooledObject Get(int contextId)
	{
		ContextPoolImplementation poolForContext = GetPoolForContext(contextId);
		TPooledObject val = poolForContext.Pool.Get();
		poolForContext.Active.Add(val);
		return val;
	}

	public void Release(int contextId, TPooledObject obj)
	{
		ContextPoolImplementation poolForContext = GetPoolForContext(contextId);
		poolForContext.Pool.Release(obj);
		poolForContext.Active.Remove(obj);
	}

	public IReadOnlyCollection<TPooledObject> GetActiveObjects(int contextId)
	{
		if (m_Pools.TryGetValue(contextId, out var value))
		{
			return value.Active;
		}
		return (IReadOnlyCollection<TPooledObject>)(object)Array.Empty<TPooledObject>();
	}

	public void Clear(int contextId)
	{
		if (m_Pools.TryGetValue(contextId, out var value))
		{
			value.Pool.Dispose();
			value.Active.Clear();
			m_Pools.Remove(contextId);
		}
	}

	internal bool Exists(int contextId)
	{
		return m_Pools.ContainsKey(contextId);
	}

	private ContextPoolImplementation GetPoolForContext(int contextId)
	{
		if (!m_Pools.TryGetValue(contextId, out var value))
		{
			value = new ContextPoolImplementation(m_ObjectCreator);
			m_Pools[contextId] = value;
		}
		return value;
	}
}
