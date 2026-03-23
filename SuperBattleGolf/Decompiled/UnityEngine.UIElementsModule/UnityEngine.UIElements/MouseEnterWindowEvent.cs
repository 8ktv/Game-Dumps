namespace UnityEngine.UIElements;

[EventCategory(EventCategory.EnterLeaveWindow)]
public class MouseEnterWindowEvent : MouseEventBase<MouseEnterWindowEvent>
{
	static MouseEnterWindowEvent()
	{
		EventBase<MouseEnterWindowEvent>.SetCreateFunction(() => new MouseEnterWindowEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.Bubbles;
		base.recomputeTopElementUnderMouse = true;
	}

	public MouseEnterWindowEvent()
	{
		LocalInit();
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToElementUnderPointerOrPanelRoot(this, panel, PointerId.mousePointerId, base.mousePosition);
	}
}
