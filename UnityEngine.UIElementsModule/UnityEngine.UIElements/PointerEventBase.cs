#define UNITY_ASSERTIONS
using System;
using UnityEngine.InputForUI;

namespace UnityEngine.UIElements;

[EventCategory(EventCategory.Pointer)]
public abstract class PointerEventBase<T> : EventBase<T>, IPointerEvent, IPointerEventInternal, IPointerOrMouseEvent where T : PointerEventBase<T>, new()
{
	private const float k_DefaultButtonPressure = 0.5f;

	private bool m_AltitudeNeedsConversion = true;

	private bool m_AzimuthNeedsConversion = true;

	private float m_AltitudeAngle = 0f;

	private float m_AzimuthAngle = 0f;

	private bool m_TiltNeeded = true;

	private Vector2 m_Tilt = new Vector2(0f, 0f);

	public int pointerId { get; protected set; }

	public string pointerType { get; protected set; }

	public bool isPrimary { get; protected set; }

	public int button { get; protected set; }

	public int pressedButtons { get; protected set; }

	public Vector3 position { get; protected set; }

	public Vector3 localPosition { get; protected set; }

	public Vector3 deltaPosition { get; protected set; }

	public float deltaTime { get; protected set; }

	public int clickCount { get; protected set; }

	public float pressure { get; protected set; }

	public float tangentialPressure { get; protected set; }

	public float altitudeAngle
	{
		get
		{
			if (m_AltitudeNeedsConversion)
			{
				m_AltitudeAngle = TiltToAltitude(tilt);
				m_AltitudeNeedsConversion = false;
			}
			return m_AltitudeAngle;
		}
		protected set
		{
			m_AltitudeNeedsConversion = true;
			m_AltitudeAngle = value;
		}
	}

	public float azimuthAngle
	{
		get
		{
			if (m_AzimuthNeedsConversion)
			{
				m_AzimuthAngle = TiltToAzimuth(tilt);
				m_AzimuthNeedsConversion = false;
			}
			return m_AzimuthAngle;
		}
		protected set
		{
			m_AzimuthNeedsConversion = true;
			m_AzimuthAngle = value;
		}
	}

	public float twist { get; protected set; }

	public Vector2 tilt
	{
		get
		{
			if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsPlayer && pointerType == PointerType.touch && m_TiltNeeded)
			{
				m_Tilt = AzimuthAndAlitutudeToTilt(m_AltitudeAngle, m_AzimuthAngle);
				m_TiltNeeded = false;
			}
			return m_Tilt;
		}
		protected set
		{
			m_TiltNeeded = true;
			m_Tilt = value;
		}
	}

	public PenStatus penStatus { get; protected set; }

	public Vector2 radius { get; protected set; }

	public Vector2 radiusVariance { get; protected set; }

	public EventModifiers modifiers { get; protected set; }

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

	internal IMouseEvent compatibilityMouseEvent { get; set; }

	internal int displayIndex { get; set; }

	internal bool recomputeTopElementUnderPointer { get; set; }

	IMouseEvent IPointerEventInternal.compatibilityMouseEvent => compatibilityMouseEvent;

	int IPointerEventInternal.displayIndex => displayIndex;

	bool IPointerEventInternal.recomputeTopElementUnderPointer => recomputeTopElementUnderPointer;

	Vector3 IPointerOrMouseEvent.deltaPosition
	{
		get
		{
			return deltaPosition;
		}
		set
		{
			deltaPosition = value;
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
				localPosition = ele.WorldToLocal3D(position);
			}
			else
			{
				localPosition = position;
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
		pointerId = 0;
		pointerType = PointerType.unknown;
		isPrimary = false;
		button = -1;
		pressedButtons = 0;
		position = Vector3.zero;
		localPosition = Vector3.zero;
		deltaPosition = Vector3.zero;
		deltaTime = 0f;
		clickCount = 0;
		pressure = 0f;
		tangentialPressure = 0f;
		altitudeAngle = 0f;
		azimuthAngle = 0f;
		tilt = new Vector2(0f, 0f);
		twist = 0f;
		penStatus = PenStatus.None;
		radius = Vector2.zero;
		radiusVariance = Vector2.zero;
		modifiers = EventModifiers.None;
		((IDisposable)compatibilityMouseEvent)?.Dispose();
		compatibilityMouseEvent = null;
		displayIndex = 0;
		recomputeTopElementUnderPointer = false;
	}

	private static bool IsMouse(Event systemEvent)
	{
		EventType rawType = systemEvent.rawType;
		return rawType == EventType.MouseMove || rawType == EventType.MouseDown || rawType == EventType.MouseUp || rawType == EventType.MouseDrag || rawType == EventType.ContextClick || rawType == EventType.MouseEnterWindow || rawType == EventType.MouseLeaveWindow;
	}

	private static bool IsTouch(Event systemEvent)
	{
		EventType rawType = systemEvent.rawType;
		return rawType == EventType.TouchMove || rawType == EventType.TouchDown || rawType == EventType.TouchUp || rawType == EventType.TouchStationary || rawType == EventType.TouchEnter || rawType == EventType.TouchLeave;
	}

	private static float TiltToAzimuth(Vector2 tilt)
	{
		float result = 0f;
		if (tilt.x != 0f)
		{
			result = MathF.PI / 2f - Mathf.Atan2((0f - Mathf.Cos(tilt.x)) * Mathf.Sin(tilt.y), Mathf.Cos(tilt.y) * Mathf.Sin(tilt.x));
			if (result < 0f)
			{
				result += MathF.PI * 2f;
			}
			result = ((!(result >= MathF.PI / 2f)) ? (result + 4.712389f) : (result - MathF.PI / 2f));
		}
		return result;
	}

	private static Vector2 AzimuthAndAlitutudeToTilt(float altitude, float azimuth)
	{
		Vector2 result = new Vector2(0f, 0f);
		result.x = Mathf.Atan(Mathf.Cos(azimuth) * Mathf.Cos(altitude) / Mathf.Sin(azimuth));
		result.y = Mathf.Atan(Mathf.Cos(azimuth) * Mathf.Sin(altitude) / Mathf.Sin(azimuth));
		return result;
	}

	private static float TiltToAltitude(Vector2 tilt)
	{
		return MathF.PI / 2f - Mathf.Acos(Mathf.Cos(tilt.x) * Mathf.Cos(tilt.y));
	}

	public static T GetPooled(Event systemEvent)
	{
		T val = EventBase<T>.GetPooled();
		if (!IsMouse(systemEvent) && !IsTouch(systemEvent) && systemEvent.rawType != EventType.DragUpdated)
		{
			Debug.Assert(condition: false, "Unexpected event type: " + systemEvent.rawType.ToString() + " (" + systemEvent.type.ToString() + ")");
		}
		switch (systemEvent.pointerType)
		{
		default:
			val.pointerType = PointerType.mouse;
			val.pointerId = PointerId.mousePointerId;
			break;
		case UnityEngine.PointerType.Touch:
			val.pointerType = PointerType.touch;
			val.pointerId = PointerId.touchPointerIdBase;
			break;
		case UnityEngine.PointerType.Pen:
			val.pointerType = PointerType.pen;
			val.pointerId = PointerId.penPointerIdBase;
			if (systemEvent.penStatus == PenStatus.Barrel)
			{
				PointerDeviceState.PressButton(val.pointerId, 1);
			}
			else
			{
				PointerDeviceState.ReleaseButton(val.pointerId, 1);
			}
			if (systemEvent.penStatus == PenStatus.Eraser)
			{
				PointerDeviceState.PressButton(val.pointerId, 5);
			}
			else
			{
				PointerDeviceState.ReleaseButton(val.pointerId, 5);
			}
			break;
		}
		val.isPrimary = true;
		val.altitudeAngle = 0f;
		val.azimuthAngle = 0f;
		val.radius = Vector2.zero;
		val.radiusVariance = Vector2.zero;
		val.imguiEvent = systemEvent;
		if (systemEvent.rawType == EventType.MouseDown || systemEvent.rawType == EventType.TouchDown)
		{
			PointerDeviceState.PressButton(val.pointerId, systemEvent.button);
			val.button = systemEvent.button;
		}
		else if (systemEvent.rawType == EventType.MouseUp || systemEvent.rawType == EventType.TouchUp)
		{
			PointerDeviceState.ReleaseButton(val.pointerId, systemEvent.button);
			val.button = systemEvent.button;
		}
		else if (systemEvent.rawType == EventType.MouseMove || systemEvent.rawType == EventType.TouchMove)
		{
			val.button = -1;
		}
		val.pressedButtons = PointerDeviceState.GetPressedButtons(val.pointerId);
		val.position = systemEvent.mousePosition;
		val.localPosition = systemEvent.mousePosition;
		val.deltaPosition = systemEvent.delta;
		val.clickCount = systemEvent.clickCount;
		val.modifiers = systemEvent.modifiers;
		val.tilt = systemEvent.tilt;
		val.penStatus = systemEvent.penStatus;
		val.twist = systemEvent.twist;
		switch (systemEvent.pointerType)
		{
		default:
			val.pressure = ((val.pressedButtons == 0) ? 0f : 0.5f);
			break;
		case UnityEngine.PointerType.Touch:
			val.pressure = systemEvent.pressure;
			break;
		case UnityEngine.PointerType.Pen:
			val.pressure = systemEvent.pressure;
			break;
		}
		val.tangentialPressure = 0f;
		return val;
	}

	internal static T GetPooled(EventType eventType, Vector3 mousePosition, Vector2 delta, int button, int clickCount, EventModifiers modifiers, int displayIndex)
	{
		T val = EventBase<T>.GetPooled();
		val.pointerId = PointerId.mousePointerId;
		val.pointerType = PointerType.mouse;
		val.isPrimary = true;
		val.displayIndex = displayIndex;
		switch (eventType)
		{
		case EventType.MouseDown:
			PointerDeviceState.PressButton(val.pointerId, button);
			val.button = button;
			break;
		case EventType.MouseUp:
			PointerDeviceState.ReleaseButton(val.pointerId, button);
			val.button = button;
			break;
		default:
			val.button = -1;
			break;
		}
		val.pressedButtons = PointerDeviceState.GetPressedButtons(val.pointerId);
		val.position = mousePosition;
		val.localPosition = mousePosition;
		val.deltaPosition = delta;
		val.clickCount = clickCount;
		val.modifiers = modifiers;
		val.pressure = ((val.pressedButtons == 0) ? 0f : 0.5f);
		return val;
	}

	public static T GetPooled(Touch touch, EventModifiers modifiers = EventModifiers.None)
	{
		return GetPooled(touch, touch.fingerId + PointerId.touchPointerIdBase, modifiers, 0);
	}

	internal static T GetPooled(Touch touch, int pointerId, EventModifiers modifiers, int displayIndex)
	{
		T val = EventBase<T>.GetPooled();
		val.pointerId = pointerId;
		val.pointerType = PointerType.touch;
		val.displayIndex = displayIndex;
		val.isPrimary = pointerId == PointerId.touchPointerIdBase;
		if (touch.phase == TouchPhase.Began)
		{
			PointerDeviceState.PressButton(val.pointerId, 0);
			val.button = 0;
		}
		else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
		{
			PointerDeviceState.ReleaseButton(val.pointerId, 0);
			val.button = 0;
		}
		else
		{
			val.button = -1;
		}
		val.pressedButtons = PointerDeviceState.GetPressedButtons(val.pointerId);
		val.position = touch.position;
		val.localPosition = touch.position;
		val.deltaPosition = touch.deltaPosition;
		val.deltaTime = touch.deltaTime;
		val.clickCount = touch.tapCount;
		val.pressure = ((Mathf.Abs(touch.maximumPossiblePressure) > 1E-30f) ? (touch.pressure / touch.maximumPossiblePressure) : 1f);
		val.tangentialPressure = 0f;
		val.altitudeAngle = touch.altitudeAngle;
		val.azimuthAngle = touch.azimuthAngle;
		val.twist = 0f;
		val.tilt = new Vector2(0f, 0f);
		val.penStatus = PenStatus.None;
		val.radius = new Vector2(touch.radius, touch.radius);
		val.radiusVariance = new Vector2(touch.radiusVariance, touch.radiusVariance);
		val.modifiers = modifiers;
		return val;
	}

	public static T GetPooled(PenData pen, EventModifiers modifiers = EventModifiers.None)
	{
		return GetPooled(pen, modifiers, 0);
	}

	internal static T GetPooled(PenData pen, EventModifiers modifiers, int displayIndex)
	{
		T val = EventBase<T>.GetPooled();
		val.pointerId = PointerId.penPointerIdBase;
		val.pointerType = PointerType.pen;
		val.displayIndex = displayIndex;
		val.isPrimary = true;
		if (pen.contactType == PenEventType.PenDown)
		{
			PointerDeviceState.PressButton(val.pointerId, 0);
			val.button = 0;
		}
		else if (pen.contactType == PenEventType.PenUp)
		{
			PointerDeviceState.ReleaseButton(val.pointerId, 0);
			val.button = 0;
		}
		else
		{
			val.button = -1;
		}
		if (pen.penStatus == PenStatus.Barrel)
		{
			PointerDeviceState.PressButton(val.pointerId, 1);
		}
		else
		{
			PointerDeviceState.ReleaseButton(val.pointerId, 1);
		}
		if (pen.penStatus == PenStatus.Eraser)
		{
			PointerDeviceState.PressButton(val.pointerId, 5);
		}
		else
		{
			PointerDeviceState.ReleaseButton(val.pointerId, 5);
		}
		val.pressedButtons = PointerDeviceState.GetPressedButtons(val.pointerId);
		val.position = pen.position;
		val.localPosition = pen.position;
		val.deltaPosition = pen.deltaPos;
		val.clickCount = 0;
		val.pressure = pen.pressure;
		val.tangentialPressure = 0f;
		val.twist = pen.twist;
		val.tilt = pen.tilt;
		val.penStatus = pen.penStatus;
		val.radius = Vector2.zero;
		val.radiusVariance = Vector2.zero;
		val.modifiers = modifiers;
		return val;
	}

	internal static T GetPooled(PointerEvent pointerEvent, Vector3 position, int pointerId, float deltaTime)
	{
		T val = EventBase<T>.GetPooled();
		val.position = position;
		val.localPosition = position;
		val.deltaPosition = PointerDeviceState.GetPointerDeltaPosition(pointerId, ContextType.Player, position);
		val.pointerId = pointerId;
		val.deltaTime = deltaTime;
		val.displayIndex = pointerEvent.displayIndex;
		val.isPrimary = pointerEvent.isPrimaryPointer;
		val.button = -1;
		if (pointerEvent.eventSource == EventSource.Mouse)
		{
			val.pointerType = PointerType.mouse;
			Debug.Assert(pointerEvent.isPrimaryPointer, "PointerEvent from Mouse source is expected to be a primary pointer.");
			Debug.Assert(pointerId == PointerId.mousePointerId, "PointerEvent from Mouse source is expected to have mouse pointer id.");
			if (pointerEvent.button == PointerEvent.Button.Primary)
			{
				val.button = 0;
			}
			else if (pointerEvent.button == PointerEvent.Button.PenEraserInTouch)
			{
				val.button = 1;
			}
			else if (pointerEvent.button == PointerEvent.Button.PenBarrelButton)
			{
				val.button = 2;
			}
		}
		else if (pointerEvent.eventSource == EventSource.Touch)
		{
			val.pointerType = PointerType.touch;
			Debug.Assert(val.pointerId >= PointerId.touchPointerIdBase && val.pointerId < PointerId.touchPointerIdBase + PointerId.touchPointerCount, "PointerEvent from Touch source is expected to have touch-based pointer id.");
			if (pointerEvent.button == PointerEvent.Button.Primary)
			{
				val.button = 0;
			}
		}
		else if (pointerEvent.eventSource == EventSource.Pen)
		{
			val.pointerType = PointerType.pen;
			Debug.Assert(val.pointerId >= PointerId.penPointerIdBase && val.pointerId < PointerId.penPointerIdBase + PointerId.penPointerCount, "PointerEvent from Pen source is expected to have pen-based pointer id.");
			if (pointerEvent.button == PointerEvent.Button.Primary)
			{
				val.button = 0;
			}
			else if (pointerEvent.button == PointerEvent.Button.PenBarrelButton)
			{
				val.button = 1;
			}
			else if (pointerEvent.button == PointerEvent.Button.PenEraserInTouch)
			{
				val.button = 5;
			}
		}
		else
		{
			if (pointerEvent.eventSource != EventSource.TrackedDevice)
			{
				throw new ArgumentOutOfRangeException("pointerEvent", "Unsupported EventSource for pointer event");
			}
			val.pointerType = PointerType.tracked;
			Debug.Assert(val.pointerId >= PointerId.trackedPointerIdBase && val.pointerId < PointerId.trackedPointerIdBase + PointerId.trackedPointerCount, "PointerEvent from TrackedDevice source is expected to have tracked-based pointer id.");
			if (pointerEvent.button == PointerEvent.Button.Primary)
			{
				val.button = 0;
			}
		}
		if (pointerEvent.type == PointerEvent.Type.ButtonPressed)
		{
			Debug.Assert(val.button != -1, "PointerEvent of type ButtonPressed is expected to have button != -1.");
			PointerDeviceState.PressButton(val.pointerId, val.button);
		}
		else if (pointerEvent.type == PointerEvent.Type.ButtonReleased)
		{
			Debug.Assert(val.button != -1, "PointerEvent of type ButtonReleased is expected to have button != -1.");
			PointerDeviceState.ReleaseButton(val.pointerId, val.button);
		}
		else if (pointerEvent.type != PointerEvent.Type.TouchCanceled)
		{
			Debug.Assert(val.button == -1, "PointerEvent of type other than ButtonPressed, ButtonReleased, or TouchCanceled is expected to have button set to none.");
		}
		val.pressedButtons = PointerDeviceState.GetPressedButtons(val.pointerId);
		if (pointerEvent.eventSource == EventSource.Pen)
		{
			val.penStatus = PenStatus.None;
			if ((val.pressedButtons & 1) != 0)
			{
				ref T reference = ref val;
				ref T reference2 = ref reference;
				PenStatus num = reference.penStatus | PenStatus.Contact;
				reference2.penStatus = num;
			}
			if ((val.pressedButtons & 2) != 0)
			{
				ref T reference = ref val;
				ref T reference3 = ref reference;
				PenStatus num2 = reference.penStatus | PenStatus.Barrel;
				reference3.penStatus = num2;
			}
			if ((val.pressedButtons & 0x20) != 0)
			{
				ref T reference = ref val;
				ref T reference4 = ref reference;
				PenStatus num3 = reference.penStatus | PenStatus.Eraser;
				reference4.penStatus = num3;
			}
			if (pointerEvent.isInverted)
			{
				ref T reference = ref val;
				ref T reference5 = ref reference;
				PenStatus num4 = reference.penStatus | PenStatus.Inverted;
				reference5.penStatus = num4;
			}
		}
		val.clickCount = pointerEvent.clickCount;
		val.pressure = pointerEvent.pressure;
		val.altitudeAngle = pointerEvent.altitude;
		val.azimuthAngle = pointerEvent.azimuth;
		val.twist = pointerEvent.twist;
		val.tilt = pointerEvent.tilt;
		EventModifiers eventModifiers = EventModifiers.None;
		if (pointerEvent.eventModifiers.isShiftPressed)
		{
			eventModifiers |= EventModifiers.Shift;
		}
		if (pointerEvent.eventModifiers.isCtrlPressed)
		{
			eventModifiers |= EventModifiers.Control;
		}
		if (pointerEvent.eventModifiers.isAltPressed)
		{
			eventModifiers |= EventModifiers.Alt;
		}
		if (pointerEvent.eventModifiers.isMetaPressed)
		{
			eventModifiers |= EventModifiers.Command;
		}
		val.modifiers = eventModifiers;
		return val;
	}

	internal static T GetPooled(IPointerEvent triggerEvent, Vector2 position, int pointerId)
	{
		if (triggerEvent != null)
		{
			return GetPooled(triggerEvent);
		}
		T val = EventBase<T>.GetPooled();
		val.position = position;
		val.localPosition = position;
		val.pointerId = pointerId;
		val.pointerType = PointerType.GetPointerType(pointerId);
		return val;
	}

	public static T GetPooled(IPointerEvent triggerEvent)
	{
		T val = EventBase<T>.GetPooled();
		if (triggerEvent != null)
		{
			val.pointerId = triggerEvent.pointerId;
			val.pointerType = triggerEvent.pointerType;
			val.isPrimary = triggerEvent.isPrimary;
			val.button = triggerEvent.button;
			val.pressedButtons = triggerEvent.pressedButtons;
			val.position = triggerEvent.position;
			val.localPosition = triggerEvent.localPosition;
			val.deltaPosition = triggerEvent.deltaPosition;
			val.deltaTime = triggerEvent.deltaTime;
			val.clickCount = triggerEvent.clickCount;
			val.pressure = triggerEvent.pressure;
			val.tangentialPressure = triggerEvent.tangentialPressure;
			val.altitudeAngle = triggerEvent.altitudeAngle;
			val.azimuthAngle = triggerEvent.azimuthAngle;
			val.twist = triggerEvent.twist;
			val.tilt = triggerEvent.tilt;
			val.penStatus = triggerEvent.penStatus;
			val.radius = triggerEvent.radius;
			val.radiusVariance = triggerEvent.radiusVariance;
			val.modifiers = triggerEvent.modifiers;
		}
		return val;
	}

	internal virtual IMouseEvent GetPooledCompatibilityMouseEvent()
	{
		return null;
	}

	protected internal override void PreDispatch(IPanel panel)
	{
		base.PreDispatch(panel);
		if (panel.ShouldSendCompatibilityMouseEvents(this))
		{
			compatibilityMouseEvent = GetPooledCompatibilityMouseEvent();
		}
		if (recomputeTopElementUnderPointer)
		{
			PointerDeviceState.SavePointerPosition(pointerId, position, panel, panel.contextType);
			((BaseVisualElementPanel)panel).RecomputeTopElementUnderPointer(pointerId, position, this);
		}
		((EventBase)compatibilityMouseEvent)?.PreDispatch(panel);
	}

	protected internal override void PostDispatch(IPanel panel)
	{
		for (int i = 0; i < PointerId.maxPointers; i++)
		{
			panel.ProcessPointerCapture(i);
		}
		((EventBase)compatibilityMouseEvent)?.PostDispatch(panel);
		if (recomputeTopElementUnderPointer)
		{
			(panel as BaseVisualElementPanel)?.CommitElementUnderPointers();
		}
		base.PostDispatch(panel);
	}

	internal override void Dispatch(BaseVisualElementPanel panel)
	{
		EventDispatchUtilities.DispatchToCapturingElementOrElementUnderPointer(this, panel, pointerId, position);
	}

	protected PointerEventBase()
	{
		LocalInit();
	}
}
