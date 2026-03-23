namespace UnityEngine.UIElements;

public sealed class PointerUpEvent : PointerEventBase<PointerUpEvent>
{
	static PointerUpEvent()
	{
		EventBase<PointerUpEvent>.SetCreateFunction(() => new PointerUpEvent());
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

	public PointerUpEvent()
	{
		LocalInit();
	}

	internal override IMouseEvent GetPooledCompatibilityMouseEvent()
	{
		return MouseUpEvent.GetPooled(this);
	}

	protected internal override void PreDispatch(IPanel panel)
	{
		base.PreDispatch(panel);
	}

	protected internal override void PostDispatch(IPanel panel)
	{
		panel.dispatcher.m_ClickDetector.ProcessEvent(this);
		if (PointerType.IsDirectManipulationDevice(base.pointerType))
		{
			panel.ReleasePointer(base.pointerId);
			if (panel is BaseVisualElementPanel baseVisualElementPanel)
			{
				baseVisualElementPanel.ClearCachedElementUnderPointer(base.pointerId, this);
			}
		}
		base.PostDispatch(panel);
		panel.ActivateCompatibilityMouseEvents(base.pointerId);
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, base.pointerId, base.position);
	}
}
