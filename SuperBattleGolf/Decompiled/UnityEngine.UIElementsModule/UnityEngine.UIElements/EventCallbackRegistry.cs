using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

internal class EventCallbackRegistry
{
	internal struct DynamicCallbackList
	{
		private TrickleDown m_UseTrickleDown;

		[NotNull]
		private EventCallbackList m_Callbacks;

		[CanBeNull]
		private EventCallbackList m_TemporaryCallbacks;

		[CanBeNull]
		private List<EventCallbackFunctorBase> m_UnregisteredCallbacksDuringInvoke;

		private int m_IsInvoking;

		public int Count => m_Callbacks.Count;

		public static DynamicCallbackList Create(TrickleDown useTrickleDown)
		{
			return new DynamicCallbackList
			{
				m_UseTrickleDown = useTrickleDown,
				m_Callbacks = EventCallbackList.EmptyList,
				m_TemporaryCallbacks = null,
				m_UnregisteredCallbacksDuringInvoke = null,
				m_IsInvoking = 0
			};
		}

		[NotNull]
		public EventCallbackList GetCallbackListForWriting()
		{
			return (m_IsInvoking != 0) ? (m_TemporaryCallbacks ?? (m_TemporaryCallbacks = GetCallbackList(m_Callbacks))) : ((m_Callbacks != EventCallbackList.EmptyList) ? m_Callbacks : (m_Callbacks = GetCallbackList()));
		}

		[NotNull]
		public readonly EventCallbackList GetCallbackListForReading()
		{
			return m_TemporaryCallbacks ?? m_Callbacks;
		}

		public bool UnregisterCallback(long eventTypeId, [NotNull] Delegate callback)
		{
			EventCallbackList callbackListForWriting = GetCallbackListForWriting();
			if (!callbackListForWriting.Remove(eventTypeId, callback, out var removedFunctor))
			{
				return false;
			}
			if (m_IsInvoking > 0)
			{
				(m_UnregisteredCallbacksDuringInvoke ?? (m_UnregisteredCallbacksDuringInvoke = CollectionPool<List<EventCallbackFunctorBase>, EventCallbackFunctorBase>.Get())).Add(removedFunctor);
			}
			else
			{
				removedFunctor.Dispose();
			}
			return true;
		}

		public void Invoke(EventBase evt, BaseVisualElementPanel panel, VisualElement target)
		{
			BeginInvoke();
			try
			{
				bool flag = !evt.skipDisabledElements || target.enabledInHierarchy;
				long eventTypeId = evt.eventTypeId;
				Span<EventCallbackFunctorBase> span = m_Callbacks.Span;
				for (int i = 0; i < span.Length; i++)
				{
					EventCallbackFunctorBase eventCallbackFunctorBase = span[i];
					if (eventCallbackFunctorBase.eventTypeId == eventTypeId && target.elementPanel == panel && (flag || (eventCallbackFunctorBase.invokePolicy & InvokePolicy.IncludeDisabled) != InvokePolicy.Default))
					{
						if ((eventCallbackFunctorBase.invokePolicy & InvokePolicy.Once) != InvokePolicy.Default)
						{
							eventCallbackFunctorBase.UnregisterCallback(target, m_UseTrickleDown);
						}
						eventCallbackFunctorBase.Invoke(evt);
						if (evt.isImmediatePropagationStopped)
						{
							break;
						}
					}
				}
			}
			finally
			{
				EndInvoke();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void BeginInvoke()
		{
			m_IsInvoking++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EndInvoke()
		{
			m_IsInvoking--;
			if (m_IsInvoking != 0 || m_TemporaryCallbacks == null)
			{
				return;
			}
			if (m_Callbacks != EventCallbackList.EmptyList)
			{
				ReleaseCallbackList(m_Callbacks);
			}
			m_Callbacks = GetCallbackList(m_TemporaryCallbacks);
			ReleaseCallbackList(m_TemporaryCallbacks);
			m_TemporaryCallbacks = null;
			if (m_UnregisteredCallbacksDuringInvoke == null)
			{
				return;
			}
			foreach (EventCallbackFunctorBase item in m_UnregisteredCallbacksDuringInvoke)
			{
				item.Dispose();
			}
			CollectionPool<List<EventCallbackFunctorBase>, EventCallbackFunctorBase>.Release(m_UnregisteredCallbacksDuringInvoke);
			m_UnregisteredCallbacksDuringInvoke = null;
		}
	}

	private static readonly EventCallbackListPool s_ListPool = new EventCallbackListPool();

	internal DynamicCallbackList m_TrickleDownCallbacks = DynamicCallbackList.Create(TrickleDown.TrickleDown);

	internal DynamicCallbackList m_BubbleUpCallbacks = DynamicCallbackList.Create(TrickleDown.NoTrickleDown);

	private static EventCallbackList GetCallbackList(EventCallbackList initializer = null)
	{
		return s_ListPool.Get(initializer);
	}

	private static void ReleaseCallbackList(EventCallbackList toRelease)
	{
		s_ListPool.Release(toRelease);
	}

	private ref DynamicCallbackList GetDynamicCallbackList(TrickleDown useTrickleDown)
	{
		return ref useTrickleDown == TrickleDown.TrickleDown ? ref m_TrickleDownCallbacks : ref m_BubbleUpCallbacks;
	}

	public void RegisterCallback<TEventType>([NotNull] EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown, InvokePolicy invokePolicy = InvokePolicy.Default) where TEventType : EventBase<TEventType>, new()
	{
		long eventTypeId = EventBase<TEventType>.TypeId();
		ref DynamicCallbackList dynamicCallbackList = ref GetDynamicCallbackList(useTrickleDown);
		EventCallbackList callbackListForReading = dynamicCallbackList.GetCallbackListForReading();
		if (callbackListForReading.Find(eventTypeId, callback) is EventCallbackFunctor<TEventType> eventCallbackFunctor)
		{
			eventCallbackFunctor.invokePolicy = invokePolicy;
			return;
		}
		callbackListForReading = dynamicCallbackList.GetCallbackListForWriting();
		callbackListForReading.Add(EventCallbackFunctor<TEventType>.GetPooled(eventTypeId, callback, invokePolicy));
	}

	public void RegisterCallback<TEventType, TCallbackArgs>([NotNull] EventCallback<TEventType, TCallbackArgs> callback, TCallbackArgs userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown, InvokePolicy invokePolicy = InvokePolicy.Default) where TEventType : EventBase<TEventType>, new()
	{
		long eventTypeId = EventBase<TEventType>.TypeId();
		ref DynamicCallbackList dynamicCallbackList = ref GetDynamicCallbackList(useTrickleDown);
		EventCallbackList callbackListForReading = dynamicCallbackList.GetCallbackListForReading();
		if (callbackListForReading.Find(eventTypeId, callback) is EventCallbackFunctor<TEventType, TCallbackArgs> eventCallbackFunctor)
		{
			eventCallbackFunctor.invokePolicy = invokePolicy;
			eventCallbackFunctor.userArgs = userArgs;
		}
		else
		{
			callbackListForReading = dynamicCallbackList.GetCallbackListForWriting();
			callbackListForReading.Add(EventCallbackFunctor<TEventType, TCallbackArgs>.GetPooled(eventTypeId, callback, userArgs, invokePolicy));
		}
	}

	public bool UnregisterCallback<TEventType>([NotNull] EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		return GetDynamicCallbackList(useTrickleDown).UnregisterCallback(EventBase<TEventType>.TypeId(), callback);
	}

	public bool UnregisterCallback<TEventType, TCallbackArgs>([NotNull] EventCallback<TEventType, TCallbackArgs> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		return GetDynamicCallbackList(useTrickleDown).UnregisterCallback(EventBase<TEventType>.TypeId(), callback);
	}

	internal void InvokeCallbacks(EventBase evt, PropagationPhase propagationPhase)
	{
		VisualElement visualElement = (VisualElement)evt.currentTarget;
		BaseVisualElementPanel elementPanel = visualElement.elementPanel;
		switch (propagationPhase)
		{
		case PropagationPhase.TrickleDown:
			GetDynamicCallbackList(TrickleDown.TrickleDown).Invoke(evt, elementPanel, visualElement);
			break;
		case PropagationPhase.BubbleUp:
			GetDynamicCallbackList(TrickleDown.NoTrickleDown).Invoke(evt, elementPanel, visualElement);
			break;
		default:
			throw new ArgumentOutOfRangeException("propagationPhase", "Propagation phases other than TrickleDown and BubbleUp are not supported");
		}
	}

	public bool HasTrickleDownHandlers()
	{
		return m_TrickleDownCallbacks.Count > 0;
	}

	public bool HasBubbleHandlers()
	{
		return m_BubbleUpCallbacks.Count > 0;
	}
}
