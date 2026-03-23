namespace UnityEngine.UIElements;

[EventCategory(EventCategory.EnterLeave)]
public class MouseLeaveEvent : MouseEventBase<MouseLeaveEvent>
{
	static MouseLeaveEvent()
	{
		EventBase<MouseLeaveEvent>.SetCreateFunction(() => new MouseLeaveEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.TricklesDown;
	}

	public MouseLeaveEvent()
	{
		LocalInit();
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToAssignedTarget(this, panel);
	}
}
