namespace UnityEngine.UIElements;

[EventCategory(EventCategory.PointerMove)]
public sealed class PointerMoveEvent : PointerEventBase<PointerMoveEvent>
{
	internal bool isHandledByDraggable { get; set; }

	internal bool isPointerUpDown => base.button >= 0;

	internal bool isPointerDown => base.button >= 0 && (base.pressedButtons & (1 << base.button)) != 0;

	internal bool isPointerUp => base.button >= 0 && (base.pressedButtons & (1 << base.button)) == 0;

	static PointerMoveEvent()
	{
		EventBase<PointerMoveEvent>.SetCreateFunction(() => new PointerMoveEvent());
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown;
		base.recomputeTopElementUnderPointer = true;
		isHandledByDraggable = false;
	}

	public PointerMoveEvent()
	{
		LocalInit();
	}

	internal override IMouseEvent GetPooledCompatibilityMouseEvent()
	{
		if (base.imguiEvent != null && base.imguiEvent.rawType == EventType.MouseDown)
		{
			return MouseDownEvent.GetPooled(this);
		}
		if (base.imguiEvent != null && base.imguiEvent.rawType == EventType.MouseUp)
		{
			return MouseUpEvent.GetPooled(this);
		}
		return MouseMoveEvent.GetPooled(this);
	}

	protected internal override void PreDispatch(IPanel panel)
	{
		base.PreDispatch(panel);
	}

	protected internal override void PostDispatch(IPanel panel)
	{
		panel.dispatcher.m_ClickDetector.ProcessEvent(this);
		base.PostDispatch(panel);
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, base.pointerId, base.position);
	}
}
