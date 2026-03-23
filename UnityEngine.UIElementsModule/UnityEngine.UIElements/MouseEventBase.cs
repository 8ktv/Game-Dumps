#define UNITY_ASSERTIONS
namespace UnityEngine.UIElements;

[EventCategory(EventCategory.Pointer)]
public abstract class MouseEventBase<T> : EventBase<T>, IMouseEvent, IMouseEventInternal, IPointerOrMouseEvent where T : MouseEventBase<T>, new()
{
	public EventModifiers modifiers { get; protected set; }

	public Vector2 mousePosition { get; protected set; }

	public Vector2 localMousePosition { get; internal set; }

	public Vector2 mouseDelta { get; protected set; }

	public int clickCount { get; protected set; }

	public int button { get; protected set; }

	public int pressedButtons { get; protected set; }

	public bool shiftKey => (modifiers & EventModifiers.Shift) != 0;

	public bool ctrlKey => (modifiers & EventModifiers.Control) != 0;

	public bool commandKey => (modifiers & EventModifiers.Command) != 0;

	public bool altKey => (modifiers & EventModifiers.Alt) != 0;

	public bool actionKey
	{
		get
		{
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
			{
				return commandKey;
			}
			return ctrlKey;
		}
	}

	internal IPointerEvent sourcePointerEvent { get; set; }

	internal bool recomputeTopElementUnderMouse { get; set; }

	IPointerEvent IMouseEventInternal.sourcePointerEvent => sourcePointerEvent;

	bool IMouseEventInternal.recomputeTopElementUnderMouse => recomputeTopElementUnderMouse;

	int IPointerOrMouseEvent.pointerId => PointerId.mousePointerId;

	Vector3 IPointerOrMouseEvent.position => mousePosition;

	Vector3 IPointerOrMouseEvent.deltaPosition
	{
		get
		{
			return mouseDelta;
		}
		set
		{
			mouseDelta = value;
		}
	}

	public override IEventHandler currentTarget
	{
		get
		{
			return base.currentTarget;
		}
		internal set
		{
			base.currentTarget = value;
			if (currentTarget is VisualElement ele)
			{
				localMousePosition = ele.WorldToLocal3D(mousePosition);
			}
			else
			{
				localMousePosition = mousePosition;
			}
		}
	}

	protected override void Init()
	{
		base.Init();
		LocalInit();
	}

	private void LocalInit()
	{
		base.propagation = EventPropagation.BubblesOrTricklesDown;
		modifiers = EventModifiers.None;
		mousePosition = Vector2.zero;
		localMousePosition = Vector2.zero;
		mouseDelta = Vector2.zero;
		clickCount = 0;
		button = 0;
		pressedButtons = 0;
		sourcePointerEvent = null;
		recomputeTopElementUnderMouse = false;
	}

	protected internal override void PreDispatch(IPanel panel)
	{
		base.PreDispatch(panel);
		if (recomputeTopElementUnderMouse)
		{
			if (sourcePointerEvent == null)
			{
				PointerDeviceState.SavePointerPosition(PointerId.mousePointerId, mousePosition, panel, panel.contextType);
				((BaseVisualElementPanel)panel).RecomputeTopElementUnderPointer(PointerId.mousePointerId, mousePosition, this);
			}
			else if (sourcePointerEvent.pointerId != PointerId.mousePointerId)
			{
				Vector2 s_OutsidePanelCoordinates = BaseVisualElementPanel.s_OutsidePanelCoordinates;
				PointerDeviceState.SavePointerPosition(PointerId.mousePointerId, s_OutsidePanelCoordinates, null, panel.contextType);
				((BaseVisualElementPanel)panel).SetTopElementUnderPointer(PointerId.mousePointerId, null, s_OutsidePanelCoordinates);
			}
		}
	}

	protected internal override void PostDispatch(IPanel panel)
	{
		if (sourcePointerEvent is EventBase eventBase)
		{
			Debug.Assert(!eventBase.processed, "!pointerEvent.processed");
			if (base.isPropagationStopped)
			{
				eventBase.StopPropagation();
			}
			if (base.isImmediatePropagationStopped)
			{
				eventBase.StopImmediatePropagation();
			}
			eventBase.processedByFocusController |= base.processedByFocusController;
		}
		else if (recomputeTopElementUnderMouse)
		{
			(panel as BaseVisualElementPanel)?.CommitElementUnderPointers();
		}
		base.PostDispatch(panel);
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, PointerId.mousePointerId, mousePosition);
	}

	public static T GetPooled(Event systemEvent)
	{
		T val = EventBase<T>.GetPooled();
		val.imguiEvent = systemEvent;
		if (systemEvent != null)
		{
			val.modifiers = systemEvent.modifiers;
			val.mousePosition = systemEvent.mousePosition;
			val.localMousePosition = systemEvent.mousePosition;
			val.mouseDelta = systemEvent.delta;
			val.button = systemEvent.button;
			val.pressedButtons = PointerDeviceState.GetPressedButtons(PointerId.mousePointerId);
			val.clickCount = systemEvent.clickCount;
		}
		return val;
	}

	public static T GetPooled(Vector2 position, int button, int clickCount, Vector2 delta, EventModifiers modifiers = EventModifiers.None)
	{
		T val = EventBase<T>.GetPooled();
		val.modifiers = modifiers;
		val.mousePosition = position;
		val.localMousePosition = position;
		val.mouseDelta = delta;
		val.button = button;
		val.pressedButtons = PointerDeviceState.GetPressedButtons(PointerId.mousePointerId);
		val.clickCount = clickCount;
		return val;
	}

	internal static T GetPooled(IMouseEvent triggerEvent, Vector2 mousePosition)
	{
		if (triggerEvent != null)
		{
			return GetPooled(triggerEvent);
		}
		T val = EventBase<T>.GetPooled();
		val.mousePosition = mousePosition;
		val.localMousePosition = mousePosition;
		return val;
	}

	public static T GetPooled(IMouseEvent triggerEvent)
	{
		T val = EventBase<T>.GetPooled(triggerEvent as EventBase);
		if (triggerEvent != null)
		{
			val.modifiers = triggerEvent.modifiers;
			val.mousePosition = triggerEvent.mousePosition;
			val.localMousePosition = triggerEvent.mousePosition;
			val.mouseDelta = triggerEvent.mouseDelta;
			val.button = triggerEvent.button;
			val.pressedButtons = triggerEvent.pressedButtons;
			val.clickCount = triggerEvent.clickCount;
		}
		return val;
	}

	protected static T GetPooled(IPointerEvent pointerEvent)
	{
		T val = EventBase<T>.GetPooled();
		val.elementTarget = (pointerEvent as EventBase)?.elementTarget;
		val.imguiEvent = (pointerEvent as EventBase)?.imguiEvent;
		val.modifiers = pointerEvent.modifiers;
		val.mousePosition = pointerEvent.position;
		val.localMousePosition = pointerEvent.position;
		val.mouseDelta = pointerEvent.deltaPosition;
		val.button = ((pointerEvent.button != -1) ? pointerEvent.button : 0);
		val.pressedButtons = pointerEvent.pressedButtons;
		val.clickCount = pointerEvent.clickCount;
		if (pointerEvent is IPointerEventInternal)
		{
			val.sourcePointerEvent = pointerEvent;
		}
		return val;
	}

	protected MouseEventBase()
	{
		LocalInit();
	}
}
