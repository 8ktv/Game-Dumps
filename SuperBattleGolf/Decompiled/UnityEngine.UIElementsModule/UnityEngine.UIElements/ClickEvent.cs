namespace UnityEngine.UIElements;

public sealed class ClickEvent : PointerEventBase<ClickEvent>
{
	static ClickEvent()
	{
		EventBase<ClickEvent>.SetCreateFunction(() => new ClickEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown | EventPropagation.SkipDisabledElements;
	}

	public ClickEvent()
	{
		LocalInit();
	}

	internal static ClickEvent GetPooled(IPointerEvent pointerEvent, int clickCount)
	{
		ClickEvent clickEvent = PointerEventBase<ClickEvent>.GetPooled(pointerEvent);
		clickEvent.clickCount = clickCount;
		return clickEvent;
	}
}
