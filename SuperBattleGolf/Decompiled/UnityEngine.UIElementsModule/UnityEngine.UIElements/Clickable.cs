using System;

namespace UnityEngine.UIElements;

public class Clickable : PointerManipulator
{
	private readonly long m_Delay;

	private readonly long m_Interval;

	private int m_ActivePointerId = -1;

	private bool m_AcceptClicksIfDisabled;

	private IVisualElementScheduledItem m_Repeater;

	private IVisualElementScheduledItem m_PendingActivePseudoStateReset;

	protected bool active { get; set; }

	public Vector2 lastMousePosition { get; private set; }

	internal bool acceptClicksIfDisabled
	{
		get
		{
			return m_AcceptClicksIfDisabled;
		}
		set
		{
			if (m_AcceptClicksIfDisabled != value)
			{
				if (base.target != null)
				{
					UnregisterCallbacksFromTarget();
				}
				m_AcceptClicksIfDisabled = value;
				if (base.target != null)
				{
					RegisterCallbacksOnTarget();
				}
			}
		}
	}

	private InvokePolicy invokePolicy => acceptClicksIfDisabled ? InvokePolicy.IncludeDisabled : InvokePolicy.Default;

	public event Action<EventBase> clickedWithEventInfo;

	public event Action clicked;

	public Clickable(Action handler, long delay, long interval)
		: this(handler)
	{
		m_Delay = delay;
		m_Interval = interval;
		active = false;
	}

	public Clickable(Action<EventBase> handler)
	{
		this.clickedWithEventInfo = handler;
		base.activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.LeftMouse
		});
	}

	public Clickable(Action handler)
	{
		this.clicked = handler;
		base.activators.Add(new ManipulatorActivationFilter
		{
			button = MouseButton.LeftMouse
		});
		active = false;
	}

	private void OnTimer(TimerState timerState)
	{
		if ((this.clicked != null || this.clickedWithEventInfo != null) && IsRepeatable())
		{
			if (ContainsPointer(m_ActivePointerId) && (base.target.enabledInHierarchy || acceptClicksIfDisabled))
			{
				Invoke(null);
				base.target.pseudoStates |= PseudoStates.Active;
			}
			else
			{
				base.target.pseudoStates &= ~PseudoStates.Active;
			}
		}
	}

	private bool IsRepeatable()
	{
		return m_Delay > 0 || m_Interval > 0;
	}

	protected override void RegisterCallbacksOnTarget()
	{
		base.target.RegisterCallback<PointerDownEvent>(OnPointerDown, invokePolicy);
		base.target.RegisterCallback<PointerMoveEvent>(OnPointerMove, invokePolicy);
		base.target.RegisterCallback<PointerUpEvent>(OnPointerUp, InvokePolicy.IncludeDisabled);
		base.target.RegisterCallback<PointerCancelEvent>(OnPointerCancel, InvokePolicy.IncludeDisabled);
		base.target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut, InvokePolicy.IncludeDisabled);
	}

	protected override void UnregisterCallbacksFromTarget()
	{
		base.target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
		base.target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
		base.target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
		base.target.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
		base.target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
		ResetActivePseudoState();
	}

	[Obsolete("OnMouseDown has been removed and replaced by its pointer-based equivalent. Please use OnPointerDown.", false)]
	protected void OnMouseDown(MouseDownEvent evt)
	{
		if (!active && CanStartManipulation(evt))
		{
			ProcessDownEvent(evt, evt.localMousePosition, PointerId.mousePointerId);
		}
	}

	[Obsolete("OnMouseMove has been removed and replaced by its pointer-based equivalent. Please use OnPointerMove.", false)]
	protected void OnMouseMove(MouseMoveEvent evt)
	{
		if (active)
		{
			ProcessMoveEvent(evt, evt.localMousePosition);
		}
	}

	[Obsolete("OnMouseUp has been removed and replaced by its pointer-based equivalent. Please use OnPointerUp.", false)]
	protected void OnMouseUp(MouseUpEvent evt)
	{
		if (active && CanStopManipulation(evt))
		{
			ProcessUpEvent(evt, evt.localMousePosition, PointerId.mousePointerId);
		}
	}

	protected void OnPointerDown(PointerDownEvent evt)
	{
		if (!active && CanStartManipulation(evt))
		{
			ProcessDownEvent(evt, evt.localPosition, evt.pointerId);
		}
	}

	protected void OnPointerMove(PointerMoveEvent evt)
	{
		if (active)
		{
			ProcessMoveEvent(evt, evt.localPosition);
		}
	}

	protected void OnPointerUp(PointerUpEvent evt)
	{
		if (active && CanStopManipulation(evt))
		{
			ProcessUpEvent(evt, evt.localPosition, evt.pointerId);
		}
	}

	private void OnPointerCancel(PointerCancelEvent evt)
	{
		if (active && CanStopManipulation(evt))
		{
			ProcessCancelEvent(evt, evt.pointerId);
		}
	}

	private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
	{
		if (active)
		{
			ProcessCancelEvent(evt, evt.pointerId);
		}
	}

	private bool ContainsPointer(int pointerId)
	{
		VisualElement topElementUnderPointer = base.target.elementPanel.GetTopElementUnderPointer(pointerId);
		return base.target == topElementUnderPointer || base.target.Contains(topElementUnderPointer);
	}

	protected void Invoke(EventBase evt)
	{
		this.clicked?.Invoke();
		this.clickedWithEventInfo?.Invoke(evt);
	}

	internal void SimulateSingleClick(EventBase evt, int delayMs = 100)
	{
		base.target.pseudoStates |= PseudoStates.Active;
		m_PendingActivePseudoStateReset = base.target.schedule.Execute(ResetActivePseudoState);
		m_PendingActivePseudoStateReset.ExecuteLater(delayMs);
		Invoke(evt);
	}

	private void ResetActivePseudoState()
	{
		if (m_PendingActivePseudoStateReset != null)
		{
			base.target.pseudoStates &= ~PseudoStates.Active;
			m_PendingActivePseudoStateReset = null;
		}
	}

	protected virtual void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
	{
		active = true;
		m_ActivePointerId = pointerId;
		base.target.CapturePointer(pointerId);
		if (!(evt is IPointerEvent))
		{
			base.target.panel.ProcessPointerCapture(pointerId);
		}
		lastMousePosition = localPosition;
		if (IsRepeatable())
		{
			if (ContainsPointer(pointerId) && (base.target.enabledInHierarchy || acceptClicksIfDisabled))
			{
				Invoke(evt);
			}
			if (m_Repeater == null)
			{
				m_Repeater = base.target.schedule.Execute(OnTimer).Every(m_Interval).StartingIn(m_Delay);
			}
			else
			{
				m_Repeater.ExecuteLater(m_Delay);
			}
		}
		base.target.pseudoStates |= PseudoStates.Active;
		evt.StopImmediatePropagation();
	}

	protected virtual void ProcessMoveEvent(EventBase evt, Vector2 localPosition)
	{
		lastMousePosition = localPosition;
		if (ContainsPointer(m_ActivePointerId))
		{
			base.target.pseudoStates |= PseudoStates.Active;
		}
		else
		{
			base.target.pseudoStates &= ~PseudoStates.Active;
		}
		evt.StopPropagation();
	}

	protected virtual void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
	{
		active = false;
		m_ActivePointerId = -1;
		base.target.ReleasePointer(pointerId);
		if (!(evt is IPointerEvent))
		{
			base.target.panel.ProcessPointerCapture(pointerId);
		}
		base.target.pseudoStates &= ~PseudoStates.Active;
		if (IsRepeatable())
		{
			m_Repeater?.Pause();
		}
		else if (ContainsPointer(pointerId) && (base.target.enabledInHierarchy || acceptClicksIfDisabled))
		{
			Invoke(evt);
		}
		evt.StopPropagation();
	}

	protected virtual void ProcessCancelEvent(EventBase evt, int pointerId)
	{
		active = false;
		m_ActivePointerId = -1;
		base.target.ReleasePointer(pointerId);
		if (!(evt is IPointerEvent))
		{
			base.target.panel.ProcessPointerCapture(pointerId);
		}
		base.target.pseudoStates &= ~PseudoStates.Active;
		if (IsRepeatable())
		{
			m_Repeater?.Pause();
		}
		evt.StopPropagation();
	}
}
