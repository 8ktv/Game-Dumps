namespace UnityEngine.UIElements;

[EventCategory(EventCategory.PointerDown)]
public sealed class PointerDownEvent : PointerEventBase<PointerDownEvent>
{
	static PointerDownEvent()
	{
		EventBase<PointerDownEvent>.SetCreateFunction(() => new PointerDownEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown | EventPropagation.SkipDisabledElements;
		base.recomputeTopElementUnderPointer = true;
	}

	public PointerDownEvent()
	{
		LocalInit();
	}

	internal override IMouseEvent GetPooledCompatibilityMouseEvent()
	{
		return MouseDownEvent.GetPooled(this);
	}

	protected internal override void PreDispatch(IPanel panel)
	{
		base.PreDispatch(panel);
	}

	protected internal override void PostDispatch(IPanel panel)
	{
		panel.focusController.SwitchFocusOnEvent(panel.focusController.GetLeafFocusedElement(), this);
		panel.dispatcher.m_ClickDetector.ProcessEvent(this);
		base.PostDispatch(panel);
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, base.pointerId, base.position);
	}
}
