using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public abstract class CallbackEventHandler : IEventHandler
{
	internal bool isIMGUIContainer = false;

	internal EventCallbackRegistry m_CallbackRegistry;

	internal const string HandleEventBubbleUpName = "HandleEventBubbleUp";

	internal const string HandleEventTrickleDownName = "HandleEventTrickleDown";

	internal const string ExecuteDefaultActionName = "ExecuteDefaultAction";

	internal const string ExecuteDefaultActionAtTargetName = "ExecuteDefaultActionAtTarget";

	public void RegisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		if (callback == null)
		{
			throw new ArgumentException("callback parameter is null");
		}
		(m_CallbackRegistry ?? (m_CallbackRegistry = new EventCallbackRegistry())).RegisterCallback(callback, useTrickleDown);
		AddEventCategories<TEventType>(useTrickleDown);
	}

	public void RegisterCallbackOnce<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		if (callback == null)
		{
			throw new ArgumentException("callback parameter is null");
		}
		(m_CallbackRegistry ?? (m_CallbackRegistry = new EventCallbackRegistry())).RegisterCallback(callback, useTrickleDown, InvokePolicy.Once);
		AddEventCategories<TEventType>(useTrickleDown);
	}

	private void AddEventCategories<TEventType>(TrickleDown useTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		if (this is VisualElement visualElement)
		{
			visualElement.AddEventCallbackCategories(1 << (int)EventBase<TEventType>.EventCategory, useTrickleDown);
		}
	}

	public void RegisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		if (callback == null)
		{
			throw new ArgumentException("callback parameter is null");
		}
		(m_CallbackRegistry ?? (m_CallbackRegistry = new EventCallbackRegistry())).RegisterCallback(callback, userArgs, useTrickleDown);
		AddEventCategories<TEventType>(useTrickleDown);
	}

	public void RegisterCallbackOnce<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TUserArgsType userArgs, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		if (callback == null)
		{
			throw new ArgumentException("callback parameter is null");
		}
		(m_CallbackRegistry ?? (m_CallbackRegistry = new EventCallbackRegistry())).RegisterCallback(callback, userArgs, useTrickleDown, InvokePolicy.Once);
		AddEventCategories<TEventType>(useTrickleDown);
	}

	internal void RegisterCallback<TEventType>(EventCallback<TEventType> callback, InvokePolicy invokePolicy, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		(m_CallbackRegistry ?? (m_CallbackRegistry = new EventCallbackRegistry())).RegisterCallback(callback, useTrickleDown, invokePolicy);
		AddEventCategories<TEventType>(useTrickleDown);
	}

	public void UnregisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		if (callback == null)
		{
			throw new ArgumentException("callback parameter is null");
		}
		m_CallbackRegistry?.UnregisterCallback(callback, useTrickleDown);
	}

	public void UnregisterCallback<TEventType, TUserArgsType>(EventCallback<TEventType, TUserArgsType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEventType : EventBase<TEventType>, new()
	{
		if (callback == null)
		{
			throw new ArgumentException("callback parameter is null");
		}
		m_CallbackRegistry?.UnregisterCallback(callback, useTrickleDown);
	}

	public abstract void SendEvent(EventBase e);

	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
	internal abstract void SendEvent(EventBase e, DispatchMode dispatchMode);

	internal abstract void HandleEvent(EventBase e);

	void IEventHandler.HandleEvent(EventBase evt)
	{
		if (evt != null)
		{
			HandleEvent(evt);
		}
	}

	public bool HasTrickleDownHandlers()
	{
		return m_CallbackRegistry != null && m_CallbackRegistry.HasTrickleDownHandlers();
	}

	public bool HasBubbleUpHandlers()
	{
		return m_CallbackRegistry != null && m_CallbackRegistry.HasBubbleHandlers();
	}

	[EventInterest(EventInterestOptions.Inherit)]
	[Obsolete("Use HandleEventBubbleUp. Before proceeding, make sure you understand the latest changes to UIToolkit event propagation rules by visiting Unity's manual page https://docs.unity3d.com/Manual/UIE-Events-Dispatching.html")]
	protected virtual void ExecuteDefaultActionAtTarget(EventBase evt)
	{
	}

	[EventInterest(EventInterestOptions.Inherit)]
	protected virtual void HandleEventBubbleUp(EventBase evt)
	{
	}

	[EventInterest(EventInterestOptions.Inherit)]
	internal virtual void HandleEventBubbleUpDisabled(EventBase evt)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void HandleEventBubbleUpInternal(EventBase evt)
	{
		HandleEventBubbleUp(evt);
	}

	[EventInterest(EventInterestOptions.Inherit)]
	protected virtual void HandleEventTrickleDown(EventBase evt)
	{
	}

	[EventInterest(EventInterestOptions.Inherit)]
	internal virtual void HandleEventTrickleDownDisabled(EventBase evt)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void HandleEventTrickleDownInternal(EventBase evt)
	{
		HandleEventTrickleDown(evt);
	}

	[Obsolete("Use HandleEventBubbleUp. Before proceeding, make sure you understand the latest changes to UIToolkit event propagation rules by visiting Unity's manual page https://docs.unity3d.com/Manual/UIE-Events-Dispatching.html")]
	[EventInterest(EventInterestOptions.Inherit)]
	protected virtual void ExecuteDefaultAction(EventBase evt)
	{
	}

	[Obsolete("Use HandleEventBubbleUpDisabled.")]
	[EventInterest(EventInterestOptions.Inherit)]
	internal virtual void ExecuteDefaultActionDisabledAtTarget(EventBase evt)
	{
	}

	[Obsolete("Use HandleEventBubbleUpDisabled.")]
	[EventInterest(EventInterestOptions.Inherit)]
	internal virtual void ExecuteDefaultActionDisabled(EventBase evt)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ExecuteDefaultActionInternal(EventBase evt)
	{
		ExecuteDefaultAction(evt);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ExecuteDefaultActionDisabledInternal(EventBase evt)
	{
		ExecuteDefaultActionDisabled(evt);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ExecuteDefaultActionAtTargetInternal(EventBase evt)
	{
		ExecuteDefaultActionAtTarget(evt);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ExecuteDefaultActionDisabledAtTargetInternal(EventBase evt)
	{
		ExecuteDefaultActionDisabledAtTarget(evt);
	}

	protected void NotifyPropertyChanged(in BindingId property)
	{
		if (((this is VisualElement visualElement) ? visualElement.elementPanel : null) == null)
		{
			return;
		}
		using PropertyChangedEvent propertyChangedEvent = PropertyChangedEvent.GetPooled(in property);
		propertyChangedEvent.target = this;
		SendEvent(propertyChangedEvent);
	}
}
