namespace UnityEngine.UIElements;

[EventCategory(EventCategory.EnterLeaveWindow)]
public class MouseLeaveWindowEvent : MouseEventBase<MouseLeaveWindowEvent>
{
	static MouseLeaveWindowEvent()
	{
		EventBase<MouseLeaveWindowEvent>.SetCreateFunction(() => new MouseLeaveWindowEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.Bubbles;
		base.recomputeTopElementUnderMouse = false;
	}

	public MouseLeaveWindowEvent()
	{
		LocalInit();
	}

	public new static MouseLeaveWindowEvent GetPooled(Event systemEvent)
	{
		if (systemEvent != null)
		{
			PointerDeviceState.ReleaseAllButtons(PointerId.mousePointerId);
		}
		return MouseEventBase<MouseLeaveWindowEvent>.GetPooled(systemEvent);
	}

	protected internal override void PostDispatch(IPanel panel)
	{
		if (base.pressedButtons == 0 && panel is BaseVisualElementPanel baseVisualElementPanel)
		{
			baseVisualElementPanel.ClearCachedElementUnderPointer(PointerId.mousePointerId, this);
			baseVisualElementPanel.CommitElementUnderPointers();
		}
		base.PostDispatch(panel);
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToElementUnderPointerOrPanelRoot(this, panel, PointerId.mousePointerId, base.mousePosition);
	}
}
