#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace UnityEngine.UIElements;

public sealed class EventDispatcher
{
	private struct EventRecord
	{
		public EventBase m_Event;

		public BaseVisualElementPanel m_Panel;
	}

	private struct DispatchContext
	{
		public uint m_GateCount;

		public Queue<EventRecord> m_Queue;
	}

	internal ClickDetector m_ClickDetector = new ClickDetector();

	private static readonly ObjectPool<Queue<EventRecord>> k_EventQueuePool = new ObjectPool<Queue<EventRecord>>(() => new Queue<EventRecord>());

	private Queue<EventRecord> m_Queue;

	private uint m_GateCount;

	private uint m_GateDepth = 0u;

	internal const int k_MaxGateDepth = 500;

	internal const int k_NumberOfEventsWithStackInfo = 10;

	internal const int k_NumberOfEventsWithEventInfo = 100;

	private int m_DispatchStackFrame = 0;

	private EventBase m_CurrentEvent;

	private Stack<DispatchContext> m_DispatchContexts = new Stack<DispatchContext>();

	private bool m_Immediate = false;

	internal PointerDispatchState pointerState { get; } = new PointerDispatchState();

	internal uint GateDepth => m_GateDepth;

	private bool dispatchImmediately => m_Immediate || m_GateCount == 0;

	internal bool processingEvents { get; private set; }

	internal static EventDispatcher CreateDefault()
	{
		return new EventDispatcher();
	}

	[Obsolete("Please use EventDispatcher.CreateDefault().")]
	internal EventDispatcher()
	{
		m_Queue = k_EventQueuePool.Get();
	}

	internal void Dispatch(EventBase evt, [NotNull] BaseVisualElementPanel panel, DispatchMode dispatchMode)
	{
		evt.MarkReceivedByDispatcher();
		if (evt.eventTypeId == EventBase<IMGUIEvent>.TypeId())
		{
			Event imguiEvent = evt.imguiEvent;
			if (imguiEvent.rawType == EventType.Repaint)
			{
				return;
			}
		}
		if (dispatchImmediately || dispatchMode == DispatchMode.Immediate)
		{
			ProcessEvent(evt, panel);
		}
		else if (!HandleRecursiveState(evt))
		{
			evt.Acquire();
			m_Queue.Enqueue(new EventRecord
			{
				m_Event = evt,
				m_Panel = panel
			});
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool HandleRecursiveState(EventBase evt)
	{
		if (m_GateDepth <= 400)
		{
			return false;
		}
		if (m_DispatchStackFrame != 0)
		{
			StackTrace stackTrace = new StackTrace(1, fNeedFileInfo: true);
			StringBuilder stringBuilder = new StringBuilder();
			int num = stackTrace.FrameCount - m_DispatchStackFrame;
			stringBuilder.AppendLine($"Recursively dispatching event {evt} from another event {m_CurrentEvent} (depth = {m_GateDepth})");
			for (int i = 0; i < num; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				stringBuilder.Append(frame.GetMethod()).AppendFormat("({0}:{1}", frame.GetFileName(), frame.GetFileLineNumber()).AppendLine(")");
			}
			Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, stringBuilder.ToString());
		}
		else
		{
			Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, $"Recursively dispatching event {evt} from another event {m_CurrentEvent} (depth = {m_GateDepth})");
		}
		if (m_GateDepth > 500)
		{
			Debug.LogErrorFormat("Ignoring event {0}: too many events dispatched recurively", evt);
			return true;
		}
		return false;
	}

	internal void PushDispatcherContext()
	{
		ProcessEventQueue();
		m_DispatchContexts.Push(new DispatchContext
		{
			m_GateCount = m_GateCount,
			m_Queue = m_Queue
		});
		m_GateCount = 0u;
		m_Queue = k_EventQueuePool.Get();
	}

	internal void PopDispatcherContext()
	{
		Debug.Assert(m_GateCount == 0, "All gates should have been opened before popping dispatch context.");
		Debug.Assert(m_Queue.Count == 0, "Queue should be empty when popping dispatch context.");
		k_EventQueuePool.Release(m_Queue);
		m_GateCount = m_DispatchContexts.Peek().m_GateCount;
		m_Queue = m_DispatchContexts.Peek().m_Queue;
		m_DispatchContexts.Pop();
	}

	internal void CloseGate()
	{
		m_GateCount++;
		m_GateDepth++;
	}

	internal void OpenGate()
	{
		Debug.Assert(m_GateCount != 0, "m_GateCount > 0");
		if (m_GateCount != 0)
		{
			m_GateCount--;
		}
		try
		{
			if (m_GateCount == 0)
			{
				ProcessEventQueue();
			}
		}
		finally
		{
			Debug.Assert(m_GateDepth != 0, "m_GateDepth > 0");
			if (m_GateDepth != 0)
			{
				m_GateDepth--;
			}
		}
	}

	private void ProcessEventQueue()
	{
		Queue<EventRecord> queue = m_Queue;
		m_Queue = k_EventQueuePool.Get();
		ExitGUIException ex = null;
		try
		{
			processingEvents = true;
			while (queue.Count > 0)
			{
				EventRecord eventRecord = queue.Dequeue();
				EventBase eventBase = eventRecord.m_Event;
				BaseVisualElementPanel panel = eventRecord.m_Panel;
				try
				{
					ProcessEvent(eventBase, panel);
				}
				catch (ExitGUIException ex2)
				{
					Debug.Assert(ex == null);
					ex = ex2;
				}
				finally
				{
					eventBase.Dispose();
				}
			}
		}
		finally
		{
			processingEvents = false;
			k_EventQueuePool.Release(queue);
		}
		if (ex != null)
		{
			throw ex;
		}
	}

	private void ProcessEvent(EventBase evt, [NotNull] BaseVisualElementPanel panel)
	{
		if (panel.disposed)
		{
			return;
		}
		Event imguiEvent = evt.imguiEvent;
		bool flag = imguiEvent != null && imguiEvent.rawType == EventType.Used;
		using (new EventDispatcherGate(this))
		{
			evt.PreDispatch(panel);
			try
			{
				m_CurrentEvent = evt;
				m_DispatchStackFrame = ((m_GateDepth > 490) ? new StackTrace().FrameCount : 0);
				evt.Dispatch(panel);
			}
			finally
			{
				m_CurrentEvent = null;
			}
			evt.PostDispatch(panel);
			Debug.Assert(flag || evt.isPropagationStopped || imguiEvent == null || imguiEvent.rawType != EventType.Used, "Event is used but not stopped.");
		}
	}
}
