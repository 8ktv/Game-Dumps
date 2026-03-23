namespace UnityEngine.UIElements;

[EventCategory(EventCategory.PointerDown)]
public class MouseDownEvent : MouseEventBase<MouseDownEvent>
{
	static MouseDownEvent()
	{
		EventBase<MouseDownEvent>.SetCreateFunction(() => new MouseDownEvent());
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

	public MouseDownEvent()
	{
		LocalInit();
	}

	public new static MouseDownEvent GetPooled(Event systemEvent)
	{
		return MouseEventBase<MouseDownEvent>.GetPooled(systemEvent);
	}

	private static MouseDownEvent MakeFromPointerEvent(IPointerEvent pointerEvent)
	{
		return MouseEventBase<MouseDownEvent>.GetPooled(pointerEvent);
	}

	internal static MouseDownEvent GetPooled(PointerDownEvent pointerEvent)
	{
		return MakeFromPointerEvent(pointerEvent);
	}

	internal static MouseDownEvent GetPooled(PointerMoveEvent pointerEvent)
	{
		return MakeFromPointerEvent(pointerEvent);
	}
}
