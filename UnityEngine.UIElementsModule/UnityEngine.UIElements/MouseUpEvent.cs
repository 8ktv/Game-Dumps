namespace UnityEngine.UIElements;

public class MouseUpEvent : MouseEventBase<MouseUpEvent>
{
	static MouseUpEvent()
	{
		EventBase<MouseUpEvent>.SetCreateFunction(() => new MouseUpEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown | EventPropagation.SkipDisabledElements;
		base.recomputeTopElementUnderMouse = true;
	}

	public MouseUpEvent()
	{
		LocalInit();
	}

	public new static MouseUpEvent GetPooled(Event systemEvent)
	{
		return MouseEventBase<MouseUpEvent>.GetPooled(systemEvent);
	}

	private static MouseUpEvent MakeFromPointerEvent(IPointerEvent pointerEvent)
	{
		return MouseEventBase<MouseUpEvent>.GetPooled(pointerEvent);
	}

	internal static MouseUpEvent GetPooled(PointerUpEvent pointerEvent)
	{
		return MakeFromPointerEvent(pointerEvent);
	}

	internal static MouseUpEvent GetPooled(PointerMoveEvent pointerEvent)
	{
		return MakeFromPointerEvent(pointerEvent);
	}

	internal static MouseUpEvent GetPooled(PointerCancelEvent pointerEvent)
	{
		return MakeFromPointerEvent(pointerEvent);
	}
}
