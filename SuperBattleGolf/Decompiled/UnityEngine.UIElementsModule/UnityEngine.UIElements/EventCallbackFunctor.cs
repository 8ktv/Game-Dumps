using System;
using UnityEngine.Pool;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements;

internal class EventCallbackFunctor<TEventType> : EventCallbackFunctorBase where TEventType : EventBase<TEventType>, new()
{
	private EventCallback<TEventType> m_Callback;

	public static EventCallbackFunctor<TEventType> GetPooled(long eventTypeId, EventCallback<TEventType> callback, InvokePolicy invokePolicy = InvokePolicy.Default)
	{
		EventCallbackFunctor<TEventType> eventCallbackFunctor = GenericPool<EventCallbackFunctor<TEventType>>.Get();
		eventCallbackFunctor.eventTypeId = eventTypeId;
		eventCallbackFunctor.invokePolicy = invokePolicy;
		eventCallbackFunctor.m_Callback = callback;
		return eventCallbackFunctor;
	}

	public override void Dispose()
	{
		eventTypeId = 0L;
		invokePolicy = InvokePolicy.Default;
		m_Callback = null;
		GenericPool<EventCallbackFunctor<TEventType>>.Release(this);
	}

	public override void Invoke(EventBase evt)
	{
		using (new EventDebuggerLogCall(m_Callback, evt))
		{
			m_Callback(evt as TEventType);
		}
	}

	public override void UnregisterCallback(CallbackEventHandler target, TrickleDown useTrickleDown)
	{
		target.UnregisterCallback(m_Callback, useTrickleDown);
	}

	public override bool IsEquivalentTo(long eventTypeId, Delegate callback)
	{
		return base.eventTypeId == eventTypeId && m_Callback == callback;
	}
}
internal class EventCallbackFunctor<TEventType, TCallbackArgs> : EventCallbackFunctorBase where TEventType : EventBase<TEventType>, new()
{
	private EventCallback<TEventType, TCallbackArgs> m_Callback;

	internal TCallbackArgs userArgs { get; set; }

	public static EventCallbackFunctor<TEventType, TCallbackArgs> GetPooled(long eventTypeId, EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, InvokePolicy invokePolicy = InvokePolicy.Default)
	{
		EventCallbackFunctor<TEventType, TCallbackArgs> eventCallbackFunctor = GenericPool<EventCallbackFunctor<TEventType, TCallbackArgs>>.Get();
		eventCallbackFunctor.eventTypeId = eventTypeId;
		eventCallbackFunctor.invokePolicy = invokePolicy;
		eventCallbackFunctor.userArgs = userArgs;
		eventCallbackFunctor.m_Callback = callback;
		return eventCallbackFunctor;
	}

	public override void Dispose()
	{
		eventTypeId = 0L;
		invokePolicy = InvokePolicy.Default;
		userArgs = default(TCallbackArgs);
		m_Callback = null;
		GenericPool<EventCallbackFunctor<TEventType, TCallbackArgs>>.Release(this);
	}

	public override void Invoke(EventBase evt)
	{
		using (new EventDebuggerLogCall(m_Callback, evt))
		{
			m_Callback(evt as TEventType, userArgs);
		}
	}

	public override void UnregisterCallback(CallbackEventHandler target, TrickleDown useTrickleDown)
	{
		target.UnregisterCallback(m_Callback, useTrickleDown);
	}

	public override bool IsEquivalentTo(long eventTypeId, Delegate callback)
	{
		return base.eventTypeId == eventTypeId && m_Callback == callback;
	}
}
