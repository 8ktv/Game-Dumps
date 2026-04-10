using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.RenderGraphModule;

internal abstract class RenderGraphDebugSession : IDisposable
{
	protected class DebugDataContainer
	{
		private readonly Dictionary<string, Dictionary<EntityId, RenderGraph.DebugData>> m_Container = new Dictionary<string, Dictionary<EntityId, RenderGraph.DebugData>>();

		public bool AddGraph(string graphName)
		{
			if (m_Container.ContainsKey(graphName))
			{
				return false;
			}
			m_Container.Add(graphName, new Dictionary<EntityId, RenderGraph.DebugData>());
			return true;
		}

		public bool RemoveGraph(string graphName)
		{
			return m_Container.Remove(graphName);
		}

		public bool AddExecution(string graphName, EntityId executionId, string executionName)
		{
			if (m_Container[graphName].ContainsKey(executionId))
			{
				return false;
			}
			m_Container[graphName][executionId] = new RenderGraph.DebugData(executionName);
			return true;
		}

		public List<string> GetRenderGraphs()
		{
			return new List<string>(m_Container.Keys);
		}

		public List<RenderGraph.DebugExecutionItem> GetExecutions(string graphName)
		{
			List<RenderGraph.DebugExecutionItem> list = new List<RenderGraph.DebugExecutionItem>();
			if (!string.IsNullOrEmpty(graphName) && m_Container.TryGetValue(graphName, out var value))
			{
				foreach (KeyValuePair<EntityId, RenderGraph.DebugData> item2 in value)
				{
					item2.Deconstruct(out var key, out var value2);
					EntityId id = key;
					RenderGraph.DebugData debugData = value2;
					RenderGraph.DebugExecutionItem item = new RenderGraph.DebugExecutionItem(id, debugData.executionName);
					list.Add(item);
				}
			}
			return list;
		}

		public RenderGraph.DebugData GetDebugData(string renderGraph, EntityId executionId)
		{
			if (!m_Container.TryGetValue(renderGraph, out var value))
			{
				throw new InvalidOperationException();
			}
			return value[executionId];
		}

		public void SetDebugData(string renderGraph, EntityId executionId, RenderGraph.DebugData data)
		{
			if (m_Container.TryGetValue(renderGraph, out var value))
			{
				value[executionId] = data;
			}
		}

		public void DeleteExecutionIds(string renderGraph, List<EntityId> executionIds)
		{
			if (!m_Container.TryGetValue(renderGraph, out var value))
			{
				return;
			}
			foreach (EntityId executionId in executionIds)
			{
				value.Remove(executionId);
			}
		}

		public void Clear()
		{
			m_Container.Clear();
		}

		public void Invalidate()
		{
			foreach (KeyValuePair<string, Dictionary<EntityId, RenderGraph.DebugData>> item in m_Container)
			{
				item.Deconstruct(out var _, out var value);
				foreach (KeyValuePair<EntityId, RenderGraph.DebugData> item2 in value)
				{
					item2.Deconstruct(out var _, out var value2);
					value2.Clear();
				}
			}
		}
	}

	private static RenderGraphDebugSession s_CurrentDebugSession;

	public static List<string> s_EmptyRegisteredGraphs = new List<string>();

	public static List<RenderGraph.DebugExecutionItem> s_EmptyExecutions = new List<RenderGraph.DebugExecutionItem>();

	public abstract bool isActive { get; }

	private DebugDataContainer debugDataContainer { get; }

	public static bool hasActiveDebugSession => s_CurrentDebugSession?.isActive ?? false;

	public static RenderGraphDebugSession currentDebugSession => s_CurrentDebugSession;

	public static event Action onRegisteredGraphsChanged;

	public static event Action<string, EntityId> onDebugDataUpdated;

	protected RenderGraphDebugSession()
	{
		debugDataContainer = new DebugDataContainer();
		RenderGraph.onGraphRegistered += RegisterGraph;
		RenderGraph.onGraphUnregistered += UnregisterGraph;
		RenderGraph.onExecutionRegistered += RegisterExecution;
	}

	protected void RegisterGraph(string graphName)
	{
		if (debugDataContainer.AddGraph(graphName))
		{
			RenderGraphDebugSession.onRegisteredGraphsChanged?.Invoke();
		}
	}

	protected void UnregisterGraph(string graphName)
	{
		if (debugDataContainer.RemoveGraph(graphName))
		{
			RenderGraphDebugSession.onRegisteredGraphsChanged?.Invoke();
		}
	}

	protected void RegisterExecution(string graphName, EntityId executionId, string executionName)
	{
		if (debugDataContainer.AddExecution(graphName, executionId, executionName))
		{
			RenderGraphDebugSession.onRegisteredGraphsChanged?.Invoke();
		}
	}

	public virtual void Dispose()
	{
		RenderGraph.onGraphRegistered -= RegisterGraph;
		RenderGraph.onGraphUnregistered -= UnregisterGraph;
		RenderGraph.onExecutionRegistered -= RegisterExecution;
		debugDataContainer.Clear();
	}

	protected void InvalidateData()
	{
		debugDataContainer.Invalidate();
	}

	public static void Create<TSession>() where TSession : RenderGraphDebugSession, new()
	{
		EndSession();
		s_CurrentDebugSession = new TSession();
	}

	public static void EndSession()
	{
		if (s_CurrentDebugSession != null)
		{
			s_CurrentDebugSession.Dispose();
			s_CurrentDebugSession = null;
		}
	}

	public static List<string> GetRegisteredGraphs()
	{
		if (s_CurrentDebugSession == null || s_CurrentDebugSession.debugDataContainer == null)
		{
			return s_EmptyRegisteredGraphs;
		}
		return s_CurrentDebugSession.debugDataContainer.GetRenderGraphs();
	}

	public static List<RenderGraph.DebugExecutionItem> GetExecutions(string graphName)
	{
		if (s_CurrentDebugSession == null || s_CurrentDebugSession.debugDataContainer == null)
		{
			return s_EmptyExecutions;
		}
		return s_CurrentDebugSession.debugDataContainer.GetExecutions(graphName);
	}

	public static RenderGraph.DebugData GetDebugData(string renderGraph, EntityId executionId)
	{
		return s_CurrentDebugSession.debugDataContainer.GetDebugData(renderGraph, executionId);
	}

	public static void SetDebugData(string renderGraph, EntityId executionId, RenderGraph.DebugData data)
	{
		s_CurrentDebugSession.debugDataContainer.SetDebugData(renderGraph, executionId, data);
		RenderGraphDebugSession.onDebugDataUpdated?.Invoke(renderGraph, executionId);
	}

	public static void DeleteExecutionIds(string renderGraph, List<EntityId> executionIds)
	{
		s_CurrentDebugSession.debugDataContainer.DeleteExecutionIds(renderGraph, executionIds);
		RenderGraphDebugSession.onRegisteredGraphsChanged?.Invoke();
	}

	protected void RegisterAllLocallyKnownGraphsAndExecutions()
	{
		foreach (var (renderGraph2, list2) in RenderGraph.GetRegisteredExecutions())
		{
			RegisterGraph(renderGraph2.name);
			foreach (RenderGraph.DebugExecutionItem item in list2)
			{
				RegisterExecution(renderGraph2.name, item.id, item.name);
			}
		}
	}
}
