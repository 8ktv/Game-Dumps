using System;
using System.Collections.Generic;
using Unity.IntegerTime;
using UnityEngine.InputForUI;

namespace UnityEngine.UIElements;

internal class DefaultEventSystem
{
	public enum UpdateMode
	{
		Always,
		IgnoreIfAppNotFocused
	}

	internal struct FocusBasedEventSequenceContext : IDisposable
	{
		private DefaultEventSystem es;

		public FocusBasedEventSequenceContext(DefaultEventSystem es)
		{
			this.es = es;
			es.m_PreviousFocusedPanel = es.focusedPanel;
			es.m_PreviousFocusedElement = es.focusedPanel?.focusController.GetLeafFocusedElement();
		}

		public void Dispose()
		{
			es.m_PreviousFocusedPanel = null;
			es.m_PreviousFocusedElement = null;
		}
	}

	private class InputForUIProcessor
	{
		private readonly DefaultEventSystem m_EventSystem;

		private DiscreteTime m_LastPointerTimestamp = DiscreteTime.Zero;

		private DiscreteTime m_NextPointerTimestamp = DiscreteTime.Zero;

		private readonly Queue<UnityEngine.InputForUI.Event> m_EventList = new Queue<UnityEngine.InputForUI.Event>();

		public InputForUIProcessor(DefaultEventSystem eventSystem)
		{
			m_EventSystem = eventSystem;
		}

		public void Reset()
		{
			m_LastPointerTimestamp = DiscreteTime.Zero;
			m_NextPointerTimestamp = DiscreteTime.Zero;
			m_EventList.Clear();
		}

		public bool OnEvent(in UnityEngine.InputForUI.Event ev)
		{
			m_EventList.Enqueue(ev);
			return true;
		}

		public void ProcessInputForUIEvents()
		{
			if (m_EventList.Count == 0)
			{
				return;
			}
			FocusBasedEventSequenceContext? focusBasedEventSequenceContext = null;
			while (m_EventList.Count > 0)
			{
				UnityEngine.InputForUI.Event obj = m_EventList.Dequeue();
				switch (obj.type)
				{
				case UnityEngine.InputForUI.Event.Type.PointerEvent:
					ProcessPointerEvent(obj.asPointerEvent);
					break;
				case UnityEngine.InputForUI.Event.Type.KeyEvent:
				{
					FocusBasedEventSequenceContext valueOrDefault = focusBasedEventSequenceContext.GetValueOrDefault();
					if (!focusBasedEventSequenceContext.HasValue)
					{
						valueOrDefault = m_EventSystem.FocusBasedEventSequence();
						focusBasedEventSequenceContext = valueOrDefault;
					}
					ProcessKeyEvent(obj.asKeyEvent);
					break;
				}
				case UnityEngine.InputForUI.Event.Type.TextInputEvent:
				{
					FocusBasedEventSequenceContext valueOrDefault = focusBasedEventSequenceContext.GetValueOrDefault();
					if (!focusBasedEventSequenceContext.HasValue)
					{
						valueOrDefault = m_EventSystem.FocusBasedEventSequence();
						focusBasedEventSequenceContext = valueOrDefault;
					}
					ProcessTextInputEvent(obj.asTextInputEvent);
					break;
				}
				case UnityEngine.InputForUI.Event.Type.IMECompositionEvent:
				{
					FocusBasedEventSequenceContext valueOrDefault = focusBasedEventSequenceContext.GetValueOrDefault();
					if (!focusBasedEventSequenceContext.HasValue)
					{
						valueOrDefault = m_EventSystem.FocusBasedEventSequence();
						focusBasedEventSequenceContext = valueOrDefault;
					}
					ProcessIMECompositionEvent(obj.asIMECompositionEvent);
					break;
				}
				case UnityEngine.InputForUI.Event.Type.CommandEvent:
				{
					FocusBasedEventSequenceContext valueOrDefault = focusBasedEventSequenceContext.GetValueOrDefault();
					if (!focusBasedEventSequenceContext.HasValue)
					{
						valueOrDefault = m_EventSystem.FocusBasedEventSequence();
						focusBasedEventSequenceContext = valueOrDefault;
					}
					ProcessCommandEvent(obj.asCommandEvent);
					break;
				}
				case UnityEngine.InputForUI.Event.Type.NavigationEvent:
				{
					FocusBasedEventSequenceContext valueOrDefault = focusBasedEventSequenceContext.GetValueOrDefault();
					if (!focusBasedEventSequenceContext.HasValue)
					{
						valueOrDefault = m_EventSystem.FocusBasedEventSequence();
						focusBasedEventSequenceContext = valueOrDefault;
					}
					ProcessNavigationEvent(obj.asNavigationEvent);
					break;
				}
				default:
					if (m_EventSystem.verbose)
					{
						DefaultEventSystem eventSystem = m_EventSystem;
						string text = ((int)obj.type).ToString();
						UnityEngine.InputForUI.Event obj2 = obj;
						eventSystem.Log("Unsupported event (" + text + "): " + obj2.ToString());
					}
					break;
				}
			}
			focusBasedEventSequenceContext?.Dispose();
			m_LastPointerTimestamp = m_NextPointerTimestamp;
		}

		private EventModifiers GetModifiers(UnityEngine.InputForUI.EventModifiers eventModifiers)
		{
			EventModifiers eventModifiers2 = EventModifiers.None;
			if (eventModifiers.isShiftPressed)
			{
				eventModifiers2 |= EventModifiers.Shift;
			}
			if (eventModifiers.isCtrlPressed)
			{
				eventModifiers2 |= EventModifiers.Control;
			}
			if (eventModifiers.isAltPressed)
			{
				eventModifiers2 |= EventModifiers.Alt;
			}
			if (eventModifiers.isMetaPressed)
			{
				eventModifiers2 |= EventModifiers.Command;
			}
			if (eventModifiers.isCapsLockEnabled)
			{
				eventModifiers2 |= EventModifiers.CapsLock;
			}
			if (eventModifiers.isNumericPressed)
			{
				eventModifiers2 |= EventModifiers.Numeric;
			}
			if (eventModifiers.isFunctionKeyPressed)
			{
				eventModifiers2 |= EventModifiers.FunctionKey;
			}
			return eventModifiers2;
		}

		private void ProcessPointerEvent(PointerEvent pointerEvent)
		{
			Vector2 position = pointerEvent.position;
			int displayIndex = pointerEvent.displayIndex;
			Vector2 deltaPosition = pointerEvent.deltaPosition;
			EventSource eventSource = pointerEvent.eventSource;
			if (1 == 0)
			{
			}
			(int, int) tuple = eventSource switch
			{
				EventSource.Mouse => (PointerId.mousePointerId, 1), 
				EventSource.Touch => (PointerId.touchPointerIdBase, PointerId.touchPointerCount), 
				EventSource.Pen => (PointerId.penPointerIdBase, PointerId.penPointerCount), 
				EventSource.TrackedDevice => (PointerId.trackedPointerIdBase, PointerId.trackedPointerCount), 
				_ => (PointerId.invalidPointerId, 1), 
			};
			if (1 == 0)
			{
			}
			var (num, num2) = tuple;
			if (num == PointerId.invalidPointerId)
			{
				if (m_EventSystem.verbose)
				{
					DefaultEventSystem eventSystem = m_EventSystem;
					string[] obj = new string[5] { "Pointer event source not supported: ", null, null, null, null };
					PointerEvent pointerEvent2 = pointerEvent;
					obj[1] = pointerEvent2.ToString();
					obj[2] = " (source=";
					obj[3] = pointerEvent.eventSource.ToString();
					obj[4] = ")";
					eventSystem.Log(string.Concat(obj));
				}
				return;
			}
			if (pointerEvent.pointerIndex < 0 || pointerEvent.pointerIndex >= num2)
			{
				if (m_EventSystem.verbose)
				{
					DefaultEventSystem eventSystem2 = m_EventSystem;
					string[] obj2 = new string[7] { "Pointer index out of range: ", null, null, null, null, null, null };
					PointerEvent pointerEvent2 = pointerEvent;
					obj2[1] = pointerEvent2.ToString();
					obj2[2] = " (index=";
					obj2[3] = pointerEvent.pointerIndex.ToString();
					obj2[4] = ", should have 0 <= index < ";
					obj2[5] = num2.ToString();
					obj2[6] = ")";
					eventSystem2.Log(string.Concat(obj2));
				}
				return;
			}
			int num3 = num + pointerEvent.pointerIndex;
			if (num3 < 0 || num3 >= PointerId.maxPointers)
			{
				if (m_EventSystem.verbose)
				{
					DefaultEventSystem eventSystem3 = m_EventSystem;
					string[] obj3 = new string[7] { "Pointer id out of range: ", null, null, null, null, null, null };
					PointerEvent pointerEvent2 = pointerEvent;
					obj3[1] = pointerEvent2.ToString();
					obj3[2] = " (id=";
					obj3[3] = num3.ToString();
					obj3[4] = ", should have 0 <= id < ";
					obj3[5] = PointerId.maxPointers.ToString();
					obj3[6] = ")";
					eventSystem3.Log(string.Concat(obj3));
				}
				return;
			}
			float item = ((m_LastPointerTimestamp != DiscreteTime.Zero) ? ((float)(pointerEvent.timestamp - m_LastPointerTimestamp)) : 0f);
			m_NextPointerTimestamp = pointerEvent.timestamp;
			bool deselectIfNoTarget = false;
			Func<Vector3, (PointerEvent, int, float), EventBase> evtFactory;
			if (pointerEvent.type == PointerEvent.Type.PointerMoved)
			{
				if (pointerEvent.eventSource != EventSource.TrackedDevice && Mathf.Approximately(deltaPosition.x, 0f) && Mathf.Approximately(deltaPosition.y, 0f))
				{
					return;
				}
				evtFactory = (Vector3 panelPosition, (PointerEvent pointerEvent, int pointerId, float deltaTime) t) => PointerEventBase<PointerMoveEvent>.GetPooled(t.pointerEvent, panelPosition, t.pointerId, t.deltaTime);
			}
			else if (pointerEvent.type == PointerEvent.Type.ButtonPressed)
			{
				evtFactory = (Vector3 panelPosition, (PointerEvent pointerEvent, int pointerId, float deltaTime) t) => PointerEventBase<PointerDownEvent>.GetPooled(t.pointerEvent, panelPosition, t.pointerId, t.deltaTime);
			}
			else if (pointerEvent.type == PointerEvent.Type.ButtonReleased)
			{
				evtFactory = (Vector3 panelPosition, (PointerEvent pointerEvent, int pointerId, float deltaTime) t) => PointerEventBase<PointerUpEvent>.GetPooled(t.pointerEvent, panelPosition, t.pointerId, t.deltaTime);
				deselectIfNoTarget = true;
			}
			else if (pointerEvent.type == PointerEvent.Type.TouchCanceled || pointerEvent.type == PointerEvent.Type.TouchCanceled)
			{
				evtFactory = (Vector3 panelPosition, (PointerEvent pointerEvent, int pointerId, float deltaTime) t) => PointerEventBase<PointerCancelEvent>.GetPooled(t.pointerEvent, panelPosition, t.pointerId, t.deltaTime);
			}
			else
			{
				if (pointerEvent.type != PointerEvent.Type.Scroll)
				{
					if (m_EventSystem.verbose)
					{
						DefaultEventSystem eventSystem4 = m_EventSystem;
						PointerEvent pointerEvent2 = pointerEvent;
						eventSystem4.Log("Unsupported event " + pointerEvent2.ToString());
					}
					return;
				}
				evtFactory = (Vector3 panelPosition, (PointerEvent pointerEvent, int pointerId, float deltaTime) t) => WheelEvent.GetPooled(t.pointerEvent.scroll, panelPosition, GetModifiers(t.pointerEvent.eventModifiers));
			}
			if (pointerEvent.eventSource == EventSource.TrackedDevice)
			{
				float maxDistance = ((pointerEvent.maxDistance > 0f) ? pointerEvent.maxDistance : float.PositiveInfinity);
				m_EventSystem.SendRayBasedEvent(pointerEvent.worldRay, maxDistance, num3, evtFactory, (pointerEvent, num3, item), deselectIfNoTarget);
			}
			else
			{
				m_EventSystem.SendPositionBasedEvent(position, deltaPosition, num3, displayIndex, evtFactory, (pointerEvent, num3, item), deselectIfNoTarget);
			}
		}

		private void ProcessNavigationEvent(NavigationEvent navigationEvent)
		{
			if (m_EventSystem.verbose)
			{
				m_EventSystem.Log(navigationEvent);
			}
			EventModifiers modifiers = GetModifiers(navigationEvent.eventModifiers);
			NavigationDeviceType navigationDeviceType = ((navigationEvent.eventSource == EventSource.Keyboard) ? NavigationDeviceType.Keyboard : ((navigationEvent.eventSource != EventSource.Unspecified) ? NavigationDeviceType.NonKeyboard : NavigationDeviceType.Unknown));
			if (navigationEvent.type == NavigationEvent.Type.Move)
			{
				Vector2 zero = Vector2.zero;
				if (navigationEvent.direction == NavigationEvent.Direction.Left)
				{
					zero.x = -1f;
				}
				else if (navigationEvent.direction == NavigationEvent.Direction.Right)
				{
					zero.x = 1f;
				}
				else if (navigationEvent.direction == NavigationEvent.Direction.Up)
				{
					zero.y = 1f;
				}
				else if (navigationEvent.direction == NavigationEvent.Direction.Down)
				{
					zero.y = -1f;
				}
				if (zero != Vector2.zero)
				{
					m_EventSystem.SendFocusBasedEvent(((Vector2 move, NavigationDeviceType deviceType, EventModifiers mod) t) => NavigationMoveEvent.GetPooled(t.move, t.deviceType, t.mod), (zero, navigationDeviceType, modifiers));
					return;
				}
				NavigationMoveEvent.Direction item = ((navigationEvent.direction == NavigationEvent.Direction.Previous) ? NavigationMoveEvent.Direction.Previous : NavigationMoveEvent.Direction.Next);
				m_EventSystem.SendFocusBasedEvent(((NavigationMoveEvent.Direction direction, NavigationDeviceType deviceType, EventModifiers mod) t) => NavigationMoveEvent.GetPooled(t.direction, t.deviceType, t.mod), (item, navigationDeviceType, modifiers));
			}
			else if (navigationEvent.type == NavigationEvent.Type.Submit)
			{
				m_EventSystem.SendFocusBasedEvent(((NavigationDeviceType deviceType, EventModifiers mod) t) => NavigationEventBase<NavigationSubmitEvent>.GetPooled(t.deviceType, t.mod), (navigationDeviceType, modifiers));
			}
			else if (navigationEvent.type == NavigationEvent.Type.Cancel)
			{
				m_EventSystem.SendFocusBasedEvent(((NavigationDeviceType deviceType, EventModifiers mod) t) => NavigationEventBase<NavigationCancelEvent>.GetPooled(t.deviceType, t.mod), (navigationDeviceType, modifiers));
			}
		}

		private void ProcessKeyEvent(KeyEvent keyEvent)
		{
			if (m_EventSystem.verbose)
			{
				m_EventSystem.Log(keyEvent);
			}
			if (keyEvent.type == KeyEvent.Type.KeyPressed || keyEvent.type == KeyEvent.Type.KeyRepeated)
			{
				m_EventSystem.SendFocusBasedEvent(((EventModifiers modifiers, KeyCode keyCode) t) => KeyboardEventBase<KeyDownEvent>.GetPooled('\0', t.keyCode, t.modifiers), (GetModifiers(keyEvent.eventModifiers), keyEvent.keyCode));
			}
			else if (keyEvent.type == KeyEvent.Type.KeyReleased)
			{
				m_EventSystem.SendFocusBasedEvent(((EventModifiers modifiers, KeyCode keyCode) t) => KeyboardEventBase<KeyUpEvent>.GetPooled('\0', t.keyCode, t.modifiers), (GetModifiers(keyEvent.eventModifiers), keyEvent.keyCode));
			}
		}

		private void ProcessTextInputEvent(TextInputEvent textInputEvent)
		{
			if (m_EventSystem.verbose)
			{
				m_EventSystem.Log(textInputEvent);
			}
			m_EventSystem.SendFocusBasedEvent(((EventModifiers modifiers, char character) t) => KeyboardEventBase<KeyDownEvent>.GetPooled(t.character, KeyCode.None, t.modifiers), (GetModifiers(textInputEvent.eventModifiers), textInputEvent.character));
		}

		private void ProcessCommandEvent(CommandEvent commandEvent)
		{
			if (m_EventSystem.verbose)
			{
				m_EventSystem.Log(commandEvent);
			}
		}

		private void ProcessIMECompositionEvent(IMECompositionEvent compositionEvent)
		{
			if (m_EventSystem.verbose)
			{
				m_EventSystem.Log(compositionEvent);
			}
			m_EventSystem.SendFocusBasedEvent((int _) => IMEEvent.GetPooled(compositionEvent.compositionString), 0);
		}
	}

	internal class LegacyInputProcessor
	{
		internal interface IInput
		{
			int penEventCount { get; }

			int touchCount { get; }

			bool mousePresent { get; }

			Vector3 mousePosition { get; }

			Vector2 mouseScrollDelta { get; }

			int mouseButtonCount { get; }

			bool anyKey { get; }

			float unscaledTime { get; }

			float doubleClickTime { get; }

			bool GetButtonDown(string button);

			float GetAxisRaw(string axis);

			void ResetPenEvents();

			void ClearLastPenContactEvent();

			PenData GetPenEvent(int index);

			PenData GetLastPenContactEvent();

			Touch GetTouch(int index);

			bool GetMouseButtonDown(int button);

			bool GetMouseButtonUp(int button);
		}

		private class Input : IInput
		{
			public int penEventCount => UnityEngine.Input.penEventCount;

			public int touchCount => UnityEngine.Input.touchCount;

			public bool mousePresent => UnityEngine.Input.mousePresent;

			public Vector3 mousePosition => UnityEngine.Input.mousePosition;

			public Vector2 mouseScrollDelta => UnityEngine.Input.mouseScrollDelta;

			public int mouseButtonCount => 3;

			public bool anyKey => UnityEngine.Input.anyKey;

			public float unscaledTime => Time.unscaledTime;

			public float doubleClickTime => (float)Event.GetDoubleClickTime() * 0.001f;

			public bool GetButtonDown(string button)
			{
				return UnityEngine.Input.GetButtonDown(button);
			}

			public float GetAxisRaw(string axis)
			{
				return UnityEngine.Input.GetAxis(axis);
			}

			public void ResetPenEvents()
			{
				UnityEngine.Input.ResetPenEvents();
			}

			public void ClearLastPenContactEvent()
			{
				UnityEngine.Input.ClearLastPenContactEvent();
			}

			public PenData GetPenEvent(int index)
			{
				return UnityEngine.Input.GetPenEvent(index);
			}

			public PenData GetLastPenContactEvent()
			{
				return UnityEngine.Input.GetLastPenContactEvent();
			}

			public Touch GetTouch(int index)
			{
				return UnityEngine.Input.GetTouch(index);
			}

			public bool GetMouseButtonDown(int button)
			{
				return UnityEngine.Input.GetMouseButtonDown(button);
			}

			public bool GetMouseButtonUp(int button)
			{
				return UnityEngine.Input.GetMouseButtonUp(button);
			}
		}

		private class NoInput : IInput
		{
			public int touchCount => 0;

			public int penEventCount => 0;

			public bool mousePresent => false;

			public Vector3 mousePosition => default(Vector3);

			public Vector2 mouseScrollDelta => default(Vector2);

			public int mouseButtonCount => 0;

			public bool anyKey => false;

			public float unscaledTime => 0f;

			public float doubleClickTime => float.PositiveInfinity;

			public bool GetButtonDown(string button)
			{
				return false;
			}

			public float GetAxisRaw(string axis)
			{
				return 0f;
			}

			public Touch GetTouch(int index)
			{
				return default(Touch);
			}

			public void ResetPenEvents()
			{
			}

			public void ClearLastPenContactEvent()
			{
			}

			public PenData GetPenEvent(int index)
			{
				return default(PenData);
			}

			public PenData GetLastPenContactEvent()
			{
				return default(PenData);
			}

			public bool GetMouseButtonDown(int button)
			{
				return false;
			}

			public bool GetMouseButtonUp(int button)
			{
				return false;
			}
		}

		private const string m_HorizontalAxis = "Horizontal";

		private const string m_VerticalAxis = "Vertical";

		private const string m_SubmitButton = "Submit";

		private const string m_CancelButton = "Cancel";

		private const float m_InputActionsPerSecond = 10f;

		private const float m_RepeatDelay = 0.5f;

		private bool m_SendingTouchEvents;

		private bool m_SendingPenEvent;

		private EventModifiers m_CurrentModifiers;

		private int m_LastMousePressButton = -1;

		private float m_NextMousePressTime = 0f;

		private int m_LastMouseClickCount = 0;

		private Vector2 m_LastMousePosition = Vector2.zero;

		private bool m_MouseProcessedAtLeastOnce;

		private Dictionary<int, int> m_TouchFingerIdToFingerIndex = new Dictionary<int, int>();

		private int m_TouchNextFingerIndex = 0;

		private IInput m_Input;

		private readonly Event m_Event = new Event();

		private readonly DefaultEventSystem m_EventSystem;

		private int m_ConsecutiveMoveCount;

		private Vector2 m_LastMoveVector;

		private float m_PrevActionTime;

		private bool m_IsMoveFromKeyboard;

		private EventModifiers m_CurrentPointerModifiers => m_CurrentModifiers & (EventModifiers.Shift | EventModifiers.Control | EventModifiers.Alt | EventModifiers.Command);

		public IInput input
		{
			get
			{
				return m_Input ?? (m_Input = GetDefaultInput());
			}
			set
			{
				m_Input = value;
			}
		}

		public LegacyInputProcessor(DefaultEventSystem eventSystem)
		{
			m_EventSystem = eventSystem;
		}

		public IInput GetDefaultInput()
		{
			IInput input = new Input();
			try
			{
				input.GetAxisRaw("Horizontal");
			}
			catch (InvalidOperationException)
			{
				input = new NoInput();
				m_EventSystem.LogWarning("UI Toolkit is currently relying on the legacy Input Manager for its active input source, but the legacy Input Manager is not available using your current Project Settings. Some UI Toolkit functionality might be missing or not working properly as a result. To fix this problem, you can enable \"Input Manager (old)\" or \"Both\" in the Active Input Source setting of the Player section. UI Toolkit is using its internal default event system to process input. Alternatively, you may activate new Input System support with UI Toolkit by adding an EventSystem component to your active scene.");
			}
			return input;
		}

		public void Reset()
		{
			m_SendingTouchEvents = false;
			m_SendingPenEvent = false;
			m_CurrentModifiers = EventModifiers.None;
			m_LastMousePressButton = -1;
			m_NextMousePressTime = 0f;
			m_LastMouseClickCount = 0;
			m_LastMousePosition = Vector2.zero;
			m_MouseProcessedAtLeastOnce = false;
			m_ConsecutiveMoveCount = 0;
			m_IsMoveFromKeyboard = false;
			m_TouchFingerIdToFingerIndex.Clear();
			m_TouchNextFingerIndex = 0;
		}

		public void ProcessLegacyInputEvents()
		{
			m_SendingPenEvent = ProcessPenEvents();
			if (!m_SendingPenEvent)
			{
				m_SendingTouchEvents = ProcessTouchEvents();
			}
			if (!m_SendingPenEvent && !m_SendingTouchEvents)
			{
				ProcessMouseEvents();
			}
			else
			{
				m_MouseProcessedAtLeastOnce = false;
			}
			using (m_EventSystem.FocusBasedEventSequence())
			{
				SendIMGUIEvents();
				SendInputEvents();
			}
		}

		private void SendIMGUIEvents()
		{
			bool flag = true;
			while (Event.PopEvent(m_Event))
			{
				if (m_Event.type == EventType.Ignore || m_Event.type == EventType.Repaint || m_Event.type == EventType.Layout)
				{
					continue;
				}
				m_CurrentModifiers = (flag ? m_Event.modifiers : (m_CurrentModifiers | m_Event.modifiers));
				flag = false;
				if (m_Event.type == EventType.KeyUp || m_Event.type == EventType.KeyDown)
				{
					m_EventSystem.SendFocusBasedEvent((Event e) => UIElementsRuntimeUtility.CreateEvent(e), m_Event);
					ProcessTabEvent(m_Event, m_CurrentModifiers);
				}
				else if (m_Event.type == EventType.ScrollWheel)
				{
					int? targetDisplay;
					Vector2 vector = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(input.mousePosition, out targetDisplay);
					Vector2 vector2 = vector - m_LastMousePosition;
					Vector2 delta = m_Event.delta;
					m_EventSystem.SendPositionBasedEvent(vector, vector2, PointerId.mousePointerId, targetDisplay, (Vector3 panelPosition, Vector3 _, (EventModifiers modifiers, Vector2 scrollDelta) t) => WheelEvent.GetPooled(t.scrollDelta, panelPosition, t.modifiers), (m_CurrentPointerModifiers, delta));
				}
				else if ((!m_SendingTouchEvents && !m_SendingPenEvent && m_Event.pointerType != UnityEngine.PointerType.Mouse) || m_Event.type == EventType.MouseEnterWindow || m_Event.type == EventType.MouseLeaveWindow)
				{
					int pointerId = ((m_Event.pointerType == UnityEngine.PointerType.Mouse) ? PointerId.mousePointerId : ((m_Event.pointerType == UnityEngine.PointerType.Touch) ? PointerId.touchPointerIdBase : PointerId.penPointerIdBase));
					int? targetDisplay2;
					Vector3 mousePosition = UIElementsRuntimeUtility.MultiDisplayToLocalScreenPosition(m_Event.mousePosition, out targetDisplay2);
					Vector2 delta2 = m_Event.delta;
					m_EventSystem.SendPositionBasedEvent(mousePosition, delta2, pointerId, targetDisplay2, delegate(Vector3 panelPosition, Vector3 panelDelta, Event evt)
					{
						evt.mousePosition = panelPosition;
						evt.delta = panelDelta;
						return UIElementsRuntimeUtility.CreateEvent(evt);
					}, m_Event, m_Event.type == EventType.MouseDown || m_Event.type == EventType.TouchDown);
				}
			}
		}

		private void ProcessMouseEvents()
		{
			if (!input.mousePresent)
			{
				return;
			}
			int? targetDisplay;
			Vector2 vector = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(input.mousePosition, out targetDisplay);
			Vector2 vector2 = vector - m_LastMousePosition;
			if (!m_MouseProcessedAtLeastOnce)
			{
				vector2 = Vector2.zero;
				m_LastMousePosition = vector;
				m_MouseProcessedAtLeastOnce = true;
			}
			else if (!Mathf.Approximately(vector2.x, 0f) || !Mathf.Approximately(vector2.y, 0f))
			{
				m_LastMousePosition = vector;
				m_EventSystem.SendPositionBasedEvent(vector, vector2, PointerId.mousePointerId, targetDisplay, (Vector3 panelPosition, Vector3 panelDelta, (EventModifiers modifiers, int? targetDisplay) t) => PointerEventBase<PointerMoveEvent>.GetPooled(EventType.MouseMove, panelPosition, panelDelta, -1, 0, t.modifiers, t.targetDisplay.GetValueOrDefault()), (m_CurrentPointerModifiers, targetDisplay));
			}
			int mouseButtonCount = input.mouseButtonCount;
			for (int num = 0; num < mouseButtonCount; num++)
			{
				if (input.GetMouseButtonDown(num))
				{
					if (m_LastMousePressButton != num || input.unscaledTime >= m_NextMousePressTime)
					{
						m_LastMousePressButton = num;
						m_LastMouseClickCount = 0;
					}
					int item = ++m_LastMouseClickCount;
					m_NextMousePressTime = input.unscaledTime + input.doubleClickTime;
					m_EventSystem.SendPositionBasedEvent(vector, vector2, PointerId.mousePointerId, targetDisplay, (Vector3 panelPosition, Vector3 panelDelta, (int button, int clickCount, EventModifiers modifiers, int? targetDisplay) t) => PointerEventHelper.GetPooled(EventType.MouseDown, panelPosition, panelDelta, t.button, t.clickCount, t.modifiers, t.targetDisplay.GetValueOrDefault()), (num, item, m_CurrentPointerModifiers, targetDisplay), deselectIfNoTarget: true);
				}
				if (input.GetMouseButtonUp(num))
				{
					int lastMouseClickCount = m_LastMouseClickCount;
					m_EventSystem.SendPositionBasedEvent(vector, vector2, PointerId.mousePointerId, targetDisplay, (Vector3 panelPosition, Vector3 panelDelta, (int button, int clickCount, EventModifiers modifiers, int? targetDisplay) t) => PointerEventHelper.GetPooled(EventType.MouseUp, panelPosition, panelDelta, t.button, t.clickCount, t.modifiers, t.targetDisplay.GetValueOrDefault()), (num, lastMouseClickCount, m_CurrentPointerModifiers, targetDisplay));
				}
			}
		}

		private void SendInputEvents()
		{
			if (ShouldSendMoveFromInput())
			{
				m_EventSystem.SendFocusBasedEvent((LegacyInputProcessor self) => NavigationMoveEvent.GetPooled(self.GetRawMoveVector(), self.m_IsMoveFromKeyboard ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
			}
			if (input.GetButtonDown("Submit"))
			{
				m_EventSystem.SendFocusBasedEvent((LegacyInputProcessor self) => NavigationEventBase<NavigationSubmitEvent>.GetPooled(self.input.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
			}
			if (input.GetButtonDown("Cancel"))
			{
				m_EventSystem.SendFocusBasedEvent((LegacyInputProcessor self) => NavigationEventBase<NavigationCancelEvent>.GetPooled(self.input.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, self.m_CurrentModifiers), this);
			}
		}

		private bool ProcessTouchEvents()
		{
			bool flag = true;
			for (int i = 0; i < input.touchCount; i++)
			{
				Touch touch = input.GetTouch(i);
				if (touch.type != TouchType.Indirect && touch.phase != TouchPhase.Stationary)
				{
					if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
					{
						flag = false;
					}
					touch.position = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(touch.position, out var targetDisplay);
					touch.rawPosition = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(touch.rawPosition, out var _);
					touch.deltaPosition = UIElementsRuntimeUtility.ScreenBottomLeftToPanelDelta(touch.deltaPosition);
					if (!m_TouchFingerIdToFingerIndex.TryGetValue(touch.fingerId, out var value))
					{
						value = m_TouchNextFingerIndex++;
						m_TouchFingerIdToFingerIndex.Add(touch.fingerId, value);
					}
					int num = PointerId.touchPointerIdBase + value;
					m_EventSystem.SendPositionBasedEvent(touch.position, touch.deltaPosition, num, targetDisplay, delegate(Vector3 panelPosition, Vector3 panelDelta, (Touch touch, int pointerId, int? targetDisplay) t)
					{
						t.touch.position = panelPosition;
						t.touch.deltaPosition = panelDelta;
						return MakeTouchEvent(t.touch, t.pointerId, EventModifiers.None, t.targetDisplay.GetValueOrDefault());
					}, (touch, num, targetDisplay));
				}
			}
			if (flag)
			{
				m_TouchNextFingerIndex = 0;
				m_TouchFingerIdToFingerIndex.Clear();
			}
			return input.touchCount > 0;
		}

		private bool ProcessPenEvents()
		{
			PenData lastPenContactEvent = input.GetLastPenContactEvent();
			if (lastPenContactEvent.contactType == PenEventType.NoContact)
			{
				return false;
			}
			m_EventSystem.SendPositionBasedEvent(lastPenContactEvent.position, lastPenContactEvent.deltaPos, PointerId.penPointerIdBase, null, delegate(Vector3 panelPosition, Vector3 panelDelta, PenData _pen)
			{
				_pen.position = panelPosition;
				_pen.deltaPos = panelDelta;
				return MakePenEvent(_pen, EventModifiers.None, 0);
			}, lastPenContactEvent);
			input.ClearLastPenContactEvent();
			return true;
		}

		private Vector2 GetRawMoveVector()
		{
			Vector2 zero = Vector2.zero;
			zero.x = input.GetAxisRaw("Horizontal");
			zero.y = input.GetAxisRaw("Vertical");
			if (input.GetButtonDown("Horizontal"))
			{
				if (zero.x < 0f)
				{
					zero.x = -1f;
				}
				if (zero.x > 0f)
				{
					zero.x = 1f;
				}
			}
			if (input.GetButtonDown("Vertical"))
			{
				if (zero.y < 0f)
				{
					zero.y = -1f;
				}
				if (zero.y > 0f)
				{
					zero.y = 1f;
				}
			}
			return zero;
		}

		private bool ShouldSendMoveFromInput()
		{
			float unscaledTime = input.unscaledTime;
			Vector2 rawMoveVector = GetRawMoveVector();
			if (Mathf.Approximately(rawMoveVector.x, 0f) && Mathf.Approximately(rawMoveVector.y, 0f))
			{
				m_ConsecutiveMoveCount = 0;
				m_IsMoveFromKeyboard = false;
				return false;
			}
			bool flag = input.GetButtonDown("Horizontal") || input.GetButtonDown("Vertical");
			bool flag2 = Vector2.Dot(rawMoveVector, m_LastMoveVector) > 0f;
			if (!flag)
			{
				flag = ((!flag2 || m_ConsecutiveMoveCount != 1) ? (unscaledTime > m_PrevActionTime + 0.1f) : (unscaledTime > m_PrevActionTime + 0.5f));
			}
			if (!flag)
			{
				return false;
			}
			NavigationMoveEvent.Direction direction = NavigationMoveEvent.DetermineMoveDirection(rawMoveVector.x, rawMoveVector.y);
			if (direction != NavigationMoveEvent.Direction.None)
			{
				if (!flag2)
				{
					m_ConsecutiveMoveCount = 0;
				}
				m_ConsecutiveMoveCount++;
				m_PrevActionTime = unscaledTime;
				m_LastMoveVector = rawMoveVector;
				m_IsMoveFromKeyboard |= input.anyKey;
			}
			else
			{
				m_ConsecutiveMoveCount = 0;
				m_IsMoveFromKeyboard = false;
			}
			return direction != NavigationMoveEvent.Direction.None;
		}

		private void ProcessTabEvent(Event e, EventModifiers modifiers)
		{
			if (e.ShouldSendNavigationMoveEventRuntime())
			{
				NavigationMoveEvent.Direction item = (e.shift ? NavigationMoveEvent.Direction.Previous : NavigationMoveEvent.Direction.Next);
				m_EventSystem.SendFocusBasedEvent(((NavigationMoveEvent.Direction direction, EventModifiers modifiers, IInput input) t) => NavigationMoveEvent.GetPooled(t.direction, t.input.anyKey ? NavigationDeviceType.Keyboard : NavigationDeviceType.NonKeyboard, t.modifiers), (item, modifiers, input));
			}
		}

		private static EventBase MakeTouchEvent(Touch touch, int pointerId, EventModifiers modifiers, int targetDisplay)
		{
			return touch.phase switch
			{
				TouchPhase.Began => PointerEventBase<PointerDownEvent>.GetPooled(touch, pointerId, modifiers, targetDisplay), 
				TouchPhase.Moved => PointerEventBase<PointerMoveEvent>.GetPooled(touch, pointerId, modifiers, targetDisplay), 
				TouchPhase.Ended => PointerEventBase<PointerUpEvent>.GetPooled(touch, pointerId, modifiers, targetDisplay), 
				TouchPhase.Canceled => PointerEventBase<PointerCancelEvent>.GetPooled(touch, pointerId, modifiers, targetDisplay), 
				_ => null, 
			};
		}

		private static EventBase MakePenEvent(PenData pen, EventModifiers modifiers, int targetDisplay)
		{
			return pen.contactType switch
			{
				PenEventType.PenDown => PointerEventBase<PointerDownEvent>.GetPooled(pen, modifiers, targetDisplay), 
				PenEventType.PenUp => PointerEventBase<PointerUpEvent>.GetPooled(pen, modifiers, targetDisplay), 
				PenEventType.NoContact => PointerEventBase<PointerMoveEvent>.GetPooled(pen, modifiers, targetDisplay), 
				_ => null, 
			};
		}
	}

	internal static Func<bool> IsEditorRemoteConnected = () => false;

	private RuntimePanel m_FocusedPanel;

	private RuntimePanel m_PreviousFocusedPanel;

	private Focusable m_PreviousFocusedElement;

	private int m_UpdateFrameCount = 0;

	private LegacyInputProcessor m_LegacyInputProcessor;

	private InputForUIProcessor m_InputForUIProcessor;

	private bool m_IsInputReady = false;

	private bool m_UseInputForUI = true;

	private bool m_IsInputForUIActive = false;

	private IScreenRaycaster m_Raycaster;

	private readonly PhysicsDocumentPicker m_WorldSpacePicker = new PhysicsDocumentPicker();

	private readonly ScreenOverlayPanelPicker m_ScreenOverlayPicker = new ScreenOverlayPanelPicker();

	public float worldSpaceMaxDistance = float.PositiveInfinity;

	public int worldSpaceLayers = -5;

	private static readonly Vector3 s_InvalidPanelCoordinates = new Vector3(float.NaN, float.NaN, float.NaN);

	internal bool verbose = false;

	internal bool logToGameScreen = false;

	private Label m_LogLabel;

	private List<string> m_LogLines = new List<string>();

	private bool isAppFocused => Application.isFocused;

	public RuntimePanel focusedPanel
	{
		get
		{
			return m_FocusedPanel;
		}
		set
		{
			if (m_FocusedPanel != value)
			{
				m_FocusedPanel?.Blur();
				m_FocusedPanel = value;
				m_FocusedPanel?.Focus();
			}
		}
	}

	internal LegacyInputProcessor legacyInputProcessor => m_LegacyInputProcessor ?? (m_LegacyInputProcessor = new LegacyInputProcessor(this));

	private InputForUIProcessor inputForUIProcessor => m_InputForUIProcessor ?? (m_InputForUIProcessor = new InputForUIProcessor(this));

	internal bool isInputReady
	{
		get
		{
			return m_IsInputReady;
		}
		set
		{
			if (m_IsInputReady != value)
			{
				m_IsInputReady = value;
				if (m_IsInputReady)
				{
					InitInputProcessor();
				}
				else
				{
					RemoveInputProcessor();
				}
			}
		}
	}

	internal bool useInputForUI
	{
		get
		{
			return m_UseInputForUI;
		}
		set
		{
			if (m_UseInputForUI != value)
			{
				if (m_IsInputReady)
				{
					RemoveInputProcessor();
					m_UseInputForUI = value;
					InitInputProcessor();
				}
				else
				{
					m_UseInputForUI = value;
				}
			}
		}
	}

	public IScreenRaycaster raycaster
	{
		get
		{
			return m_Raycaster ?? (m_Raycaster = new MainCameraScreenRaycaster());
		}
		set
		{
			m_Raycaster = value;
		}
	}

	private bool ShouldIgnoreEventsOnAppNotFocused()
	{
		OperatingSystemFamily operatingSystemFamily = SystemInfo.operatingSystemFamily;
		OperatingSystemFamily operatingSystemFamily2 = operatingSystemFamily;
		if ((uint)(operatingSystemFamily2 - 1) <= 2u)
		{
			return true;
		}
		return false;
	}

	public void Reset()
	{
		m_LegacyInputProcessor?.Reset();
		m_InputForUIProcessor?.Reset();
		m_FocusedPanel = null;
	}

	public void Update(UpdateMode updateMode = UpdateMode.Always)
	{
		if (isAppFocused || !ShouldIgnoreEventsOnAppNotFocused() || updateMode != UpdateMode.IgnoreIfAppNotFocused)
		{
			m_UpdateFrameCount++;
			m_Raycaster?.Update();
			if (m_IsInputForUIActive)
			{
				inputForUIProcessor.ProcessInputForUIEvents();
			}
			else
			{
				legacyInputProcessor.ProcessLegacyInputEvents();
			}
			UpdateWorldSpacePointers();
		}
	}

	internal FocusBasedEventSequenceContext FocusBasedEventSequence()
	{
		return new FocusBasedEventSequenceContext(this);
	}

	private void RemoveInputProcessor()
	{
		if (m_IsInputForUIActive)
		{
			EventProvider.Unsubscribe(inputForUIProcessor.OnEvent);
			EventProvider.SetEnabled(enable: false);
			m_IsInputForUIActive = false;
		}
	}

	private void InitInputProcessor()
	{
		if (m_UseInputForUI)
		{
			m_IsInputForUIActive = true;
			EventProvider.SetEnabled(enable: true);
			EventProvider.Subscribe(inputForUIProcessor.OnEvent, 0, null);
			m_InputForUIProcessor.Reset();
		}
	}

	internal void OnFocusEvent(RuntimePanel panel, FocusEvent evt)
	{
		focusedPanel = panel;
	}

	internal void SendFocusBasedEvent<TArg>(Func<TArg, EventBase> evtFactory, TArg arg)
	{
		if (m_PreviousFocusedPanel != null)
		{
			using (EventBase eventBase = evtFactory(arg))
			{
				eventBase.elementTarget = ((VisualElement)m_PreviousFocusedElement) ?? m_PreviousFocusedPanel.visualTree;
				m_PreviousFocusedPanel.visualTree.SendEvent(eventBase);
				UpdateFocusedPanel(m_PreviousFocusedPanel);
				return;
			}
		}
		List<BaseRuntimePanel> sortedPlayerPanels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
		for (int num = sortedPlayerPanels.Count - 1; num >= 0; num--)
		{
			BaseRuntimePanel baseRuntimePanel = sortedPlayerPanels[num];
			if (baseRuntimePanel is RuntimePanel runtimePanel && !baseRuntimePanel.drawsInCameras)
			{
				using EventBase eventBase2 = evtFactory(arg);
				eventBase2.elementTarget = runtimePanel.visualTree;
				runtimePanel.visualTree.SendEvent(eventBase2);
				if (runtimePanel.focusController.focusedElement != null)
				{
					focusedPanel = runtimePanel;
					break;
				}
				if (eventBase2.isPropagationStopped)
				{
					break;
				}
			}
		}
	}

	internal void SendPositionBasedEvent<TArg>(Vector3 mousePosition, Vector3 delta, int pointerId, int? targetDisplay, Func<Vector3, Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
	{
		SendPositionBasedEvent(mousePosition, delta, pointerId, targetDisplay, delegate(Vector3 p, (Func<Vector3, Vector3, TArg, EventBase> evtFactory, Vector3 delta, TArg arg) t)
		{
			EventBase eventBase = t.evtFactory(p, t.delta, t.arg);
			if (eventBase is IPointerOrMouseEvent pointerOrMouseEvent)
			{
				pointerOrMouseEvent.deltaPosition = t.delta;
			}
			return eventBase;
		}, (evtFactory, delta, arg), deselectIfNoTarget);
	}

	internal void SendPositionBasedEvent<TArg>(Vector3 mousePosition, Vector3 delta, int pointerId, int? targetDisplay, Func<Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
	{
		if (focusedPanel != null)
		{
			UpdateFocusedPanel(focusedPanel);
		}
		FindTargetAtPosition(mousePosition, delta, pointerId, targetDisplay, out var target, out var targetPanel, out var targetPanelPosition, out var elementUnderPointer, out var camera);
		RuntimePanel runtimePanel = PointerDeviceState.GetPanel(pointerId, ContextType.Player) as RuntimePanel;
		if (runtimePanel != targetPanel)
		{
			runtimePanel?.PointerLeavesPanel(pointerId);
			targetPanel?.PointerEntersPanel(pointerId, targetPanelPosition);
		}
		if (targetPanel != null)
		{
			using (EventBase eventBase = evtFactory(targetPanelPosition, arg))
			{
				if (!targetPanel.isFlat)
				{
					targetPanel.SetTopElementUnderPointer(pointerId, elementUnderPointer, eventBase);
				}
				eventBase.elementTarget = target;
				targetPanel.visualTree.SendEvent(eventBase);
				if (eventBase.processedByFocusController)
				{
					UpdateFocusedPanel(targetPanel);
				}
				if (eventBase.eventTypeId == EventBase<PointerDownEvent>.TypeId())
				{
					PointerDeviceState.SetElementWithSoftPointerCapture(pointerId, target ?? targetPanel.visualTree, camera);
				}
				else if (eventBase.eventTypeId == EventBase<PointerUpEvent>.TypeId() && ((PointerUpEvent)eventBase).pressedButtons == 0)
				{
					PointerDeviceState.SetElementWithSoftPointerCapture(pointerId, null, null);
				}
				return;
			}
		}
		if (deselectIfNoTarget)
		{
			focusedPanel = null;
		}
	}

	internal void SendRayBasedEvent<TArg>(Ray worldRay, float maxDistance, int pointerId, Func<Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
	{
		if (focusedPanel != null)
		{
			UpdateFocusedPanel(focusedPanel);
		}
		FindTargetAtRay(worldRay, maxDistance, pointerId, out var target, out var targetPanel, out var targetPanelPosition, out var elementUnderPointer);
		RuntimePanel runtimePanel = PointerDeviceState.GetPanel(pointerId, ContextType.Player) as RuntimePanel;
		if (runtimePanel != targetPanel)
		{
			runtimePanel?.PointerLeavesPanel(pointerId);
			targetPanel?.PointerEntersPanel(pointerId, targetPanelPosition);
		}
		if (targetPanel != null)
		{
			using (EventBase eventBase = evtFactory(targetPanelPosition, arg))
			{
				if (!targetPanel.isFlat)
				{
					targetPanel.SetTopElementUnderPointer(pointerId, elementUnderPointer, eventBase);
				}
				eventBase.elementTarget = target;
				targetPanel.visualTree.SendEvent(eventBase);
				if (eventBase.processedByFocusController)
				{
					UpdateFocusedPanel(targetPanel);
				}
				if (eventBase.eventTypeId == EventBase<PointerDownEvent>.TypeId())
				{
					PointerDeviceState.SetElementWithSoftPointerCapture(pointerId, target ?? targetPanel.visualTree, null);
				}
				else if (eventBase.eventTypeId == EventBase<PointerUpEvent>.TypeId() && ((PointerUpEvent)eventBase).pressedButtons == 0)
				{
					PointerDeviceState.SetElementWithSoftPointerCapture(pointerId, null, null);
				}
				return;
			}
		}
		if (deselectIfNoTarget)
		{
			focusedPanel = null;
		}
	}

	internal void FindTargetAtPosition(Vector2 mousePosition, Vector2 delta, int pointerId, int? targetDisplay, out VisualElement target, out RuntimePanel targetPanel, out Vector3 targetPanelPosition, out VisualElement elementUnderPointer, out Camera camera)
	{
		PointerDeviceState.ScreenPointerState screenPointerState = PointerDeviceState.GetScreenPointerState(pointerId, createIfNull: true);
		screenPointerState.Reset();
		screenPointerState.mousePosition = mousePosition;
		screenPointerState.targetDisplay = targetDisplay;
		screenPointerState.updateFrameCount = m_UpdateFrameCount;
		List<BaseRuntimePanel> sortedScreenOverlayPlayerPanels = UIElementsRuntimeUtility.GetSortedScreenOverlayPlayerPanels();
		for (int num = sortedScreenOverlayPlayerPanels.Count - 1; num >= 0; num--)
		{
			if (m_ScreenOverlayPicker.TryPick(sortedScreenOverlayPlayerPanels[num], pointerId, mousePosition, delta, targetDisplay, out var _))
			{
				target = (elementUnderPointer = null);
				targetPanel = (RuntimePanel)sortedScreenOverlayPlayerPanels[num];
				targetPanel.ScreenToPanel(mousePosition, delta, out targetPanelPosition, allowOutside: true);
				camera = null;
				return;
			}
		}
		foreach (var item in raycaster.MakeRay(mousePosition, pointerId, targetDisplay))
		{
			int layerMask = item.camera.cullingMask & worldSpaceLayers;
			if (m_WorldSpacePicker.TryPickWithCapture(pointerId, item.ray, worldSpaceMaxDistance, layerMask, out var collider, out var document, out elementUnderPointer, out var distance, out var captured2) && (item.isInsideCameraRect || captured2))
			{
				screenPointerState.hit = new PointerDeviceState.RuntimePointerState.RaycastHit
				{
					collider = collider,
					document = document,
					distance = distance,
					element = elementUnderPointer
				};
				if (document == null)
				{
					break;
				}
				VisualElement visualElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId) as VisualElement;
				target = visualElement ?? elementUnderPointer ?? document.rootVisualElement;
				targetPanel = document.containerPanel;
				targetPanelPosition = GetPanelPosition(target, document, item.ray);
				camera = item.camera;
				return;
			}
		}
		target = (elementUnderPointer = null);
		targetPanel = null;
		targetPanelPosition = s_InvalidPanelCoordinates;
		camera = null;
	}

	internal void FindTargetAtRay(Ray worldRay, float maxDistance, int pointerId, out VisualElement target, out RuntimePanel targetPanel, out Vector3 targetPanelPosition, out VisualElement elementUnderPointer)
	{
		maxDistance = Mathf.Min(maxDistance, worldSpaceMaxDistance);
		Collider collider;
		UIDocument document;
		float distance;
		bool captured;
		bool flag = m_WorldSpacePicker.TryPickWithCapture(pointerId, worldRay, maxDistance, worldSpaceLayers, out collider, out document, out elementUnderPointer, out distance, out captured);
		PointerDeviceState.TrackedPointerState trackedState = PointerDeviceState.GetTrackedState(pointerId, createIfNull: true);
		trackedState.Reset();
		trackedState.worldPosition = worldRay.origin;
		trackedState.worldOrientation = Quaternion.FromToRotation(Vector3.forward, worldRay.direction);
		trackedState.maxDistance = maxDistance;
		trackedState.hit = new PointerDeviceState.RuntimePointerState.RaycastHit
		{
			collider = collider,
			document = document,
			distance = distance,
			element = elementUnderPointer
		};
		trackedState.updateFrameCount = m_UpdateFrameCount;
		if (flag && document != null)
		{
			VisualElement visualElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId) as VisualElement;
			target = visualElement ?? elementUnderPointer ?? document.rootVisualElement;
			targetPanel = document.containerPanel;
			targetPanelPosition = GetPanelPosition(target, document, worldRay);
		}
		else
		{
			target = (elementUnderPointer = null);
			targetPanel = null;
			targetPanelPosition = s_InvalidPanelCoordinates;
		}
	}

	private Vector3 GetPanelPosition(VisualElement pickedElement, UIDocument document, Ray worldRay)
	{
		Ray worldRay2 = document.transform.worldToLocalMatrix.TransformRay(worldRay);
		pickedElement.IntersectWorldRay(worldRay2, out var distance, out var _);
		return worldRay2.origin + worldRay2.direction * distance;
	}

	private void UpdateFocusedPanel(RuntimePanel runtimePanel)
	{
		if (runtimePanel.focusController.focusedElement != null)
		{
			focusedPanel = runtimePanel;
		}
		else if (focusedPanel == runtimePanel)
		{
			focusedPanel = null;
		}
	}

	private void UpdateWorldSpacePointers()
	{
		if (UIElementsRuntimeUtility.GetWorldSpacePlayerPanels().Count == 0)
		{
			return;
		}
		int[] screenHoveringPointers = PointerId.screenHoveringPointers;
		VisualElement target;
		foreach (int pointerId in screenHoveringPointers)
		{
			RuntimePanel runtimePanel = PointerDeviceState.GetPanel(pointerId, ContextType.Player) as RuntimePanel;
			if (runtimePanel != null && runtimePanel.isFlat)
			{
				continue;
			}
			PointerDeviceState.ScreenPointerState screenPointerState = PointerDeviceState.GetScreenPointerState(pointerId);
			if (screenPointerState != null && screenPointerState.updateFrameCount != m_UpdateFrameCount)
			{
				FindTargetAtPosition(screenPointerState.mousePosition, Vector2.zero, pointerId, screenPointerState.targetDisplay, out target, out var targetPanel, out var targetPanelPosition, out var elementUnderPointer, out var _);
				if (runtimePanel != targetPanel)
				{
					runtimePanel?.PointerLeavesPanel(pointerId);
					targetPanel?.PointerEntersPanel(pointerId, targetPanelPosition);
				}
				if (targetPanel != null && !targetPanel.isFlat)
				{
					targetPanel.SetTopElementUnderPointer(pointerId, elementUnderPointer, targetPanelPosition);
					targetPanel.CommitElementUnderPointers();
				}
			}
		}
		for (int j = 0; j < PointerId.trackedPointerCount; j++)
		{
			int pointerId2 = PointerId.trackedPointerIdBase + j;
			PointerDeviceState.TrackedPointerState trackedState = PointerDeviceState.GetTrackedState(pointerId2);
			if (trackedState != null && trackedState.updateFrameCount != m_UpdateFrameCount)
			{
				FindTargetAtRay(trackedState.worldRay, trackedState.maxDistance, pointerId2, out target, out var targetPanel2, out var targetPanelPosition2, out var elementUnderPointer2);
				RuntimePanel runtimePanel2 = PointerDeviceState.GetPanel(pointerId2, ContextType.Player) as RuntimePanel;
				if (runtimePanel2 != targetPanel2)
				{
					runtimePanel2?.PointerLeavesPanel(pointerId2);
					targetPanel2?.PointerEntersPanel(pointerId2, targetPanelPosition2);
				}
				if (targetPanel2 != null)
				{
					targetPanel2.SetTopElementUnderPointer(pointerId2, elementUnderPointer2, targetPanelPosition2);
					targetPanel2.CommitElementUnderPointers();
				}
			}
		}
	}

	private void Log(object o)
	{
		Debug.Log(o);
		if (logToGameScreen)
		{
			LogToGameScreen(o?.ToString() ?? "");
		}
	}

	private void LogWarning(object o)
	{
		Debug.LogWarning(o);
		if (logToGameScreen)
		{
			LogToGameScreen("Warning! " + o);
		}
	}

	private void LogToGameScreen(string s)
	{
		if (m_LogLabel == null)
		{
			Label label = new Label();
			label.style.position = Position.Absolute;
			label.style.bottom = 0f;
			label.style.color = Color.white;
			m_LogLabel = label;
			Object.FindFirstObjectByType<UIDocument>().rootVisualElement.Add(m_LogLabel);
		}
		m_LogLines.Add(s + "\n");
		if (m_LogLines.Count > 10)
		{
			m_LogLines.RemoveAt(0);
		}
		m_LogLabel.text = string.Concat(m_LogLines);
	}
}
