namespace UnityEngine.UIElements;

[EventCategory(EventCategory.Keyboard)]
internal class IMEEvent : EventBase<IMEEvent>
{
	public string compositionString { get; protected set; }

	static IMEEvent()
	{
		EventBase<IMEEvent>.SetCreateFunction(() => new IMEEvent());
	}

	public static IMEEvent GetPooled(string compositionString)
	{
		IMEEvent iMEEvent = EventBase<IMEEvent>.GetPooled();
		iMEEvent.compositionString = compositionString;
		return iMEEvent;
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown | EventPropagation.SkipDisabledElements;
		compositionString = null;
	}

	public IMEEvent()
	{
		LocalInit();
	}
}
