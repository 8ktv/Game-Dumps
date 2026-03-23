namespace UnityEngine.UIElements;

public class WheelEvent : MouseEventBase<WheelEvent>
{
	public const float scrollDeltaPerTick = 3f;

	public Vector3 delta { get; private set; }

	static WheelEvent()
	{
		EventBase<WheelEvent>.SetCreateFunction(() => new WheelEvent());
	}

	public new static WheelEvent GetPooled(Event systemEvent)
	{
		WheelEvent wheelEvent = MouseEventBase<WheelEvent>.GetPooled(systemEvent);
		if (systemEvent != null)
		{
			wheelEvent.delta = systemEvent.delta;
		}
		return wheelEvent;
	}

	internal static WheelEvent GetPooled(Vector3 delta, Vector3 mousePosition, EventModifiers modifiers = EventModifiers.None)
	{
		WheelEvent wheelEvent = EventBase<WheelEvent>.GetPooled();
		wheelEvent.delta = delta;
		wheelEvent.mousePosition = mousePosition;
		wheelEvent.modifiers = modifiers;
		return wheelEvent;
	}

	internal static WheelEvent GetPooled(Vector3 delta, IPointerEvent pointerEvent)
	{
		WheelEvent wheelEvent = MouseEventBase<WheelEvent>.GetPooled(pointerEvent);
		wheelEvent.delta = delta;
		return wheelEvent;
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown | EventPropagation.SkipDisabledElements;
		delta = Vector3.zero;
		base.recomputeTopElementUnderMouse = true;
	}

	public WheelEvent()
	{
		LocalInit();
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToElementUnderPointerOrPanelRoot(this, panel, PointerId.mousePointerId, base.mousePosition);
	}
}
