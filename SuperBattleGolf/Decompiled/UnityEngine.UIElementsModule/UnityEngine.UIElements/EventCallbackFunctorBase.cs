using System;

namespace UnityEngine.UIElements;

internal abstract class EventCallbackFunctorBase : IDisposable
{
	public long eventTypeId;

	public InvokePolicy invokePolicy;

	public abstract void Invoke(EventBase evt);

	public abstract void UnregisterCallback(CallbackEventHandler target, TrickleDown useTrickleDown);

	public abstract void Dispose();

	public abstract bool IsEquivalentTo(long eventTypeId, Delegate callback);
}
