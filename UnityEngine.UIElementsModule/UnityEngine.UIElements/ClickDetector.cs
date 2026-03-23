using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal class ClickDetector
{
	private class ButtonClickStatus
	{
		public VisualElement m_Target;

		public Vector3 m_PointerDownPosition;

		public long m_LastPointerDownTime;

		public int m_ClickCount;

		public void Reset()
		{
			m_Target = null;
			m_ClickCount = 0;
			m_LastPointerDownTime = 0L;
			m_PointerDownPosition = Vector3.zero;
		}
	}

	private List<ButtonClickStatus> m_ClickStatus;

	internal static int s_DoubleClickTime { get; set; } = -1;

	public ClickDetector()
	{
		m_ClickStatus = new List<ButtonClickStatus>(PointerId.maxPointers);
		for (int i = 0; i < PointerId.maxPointers; i++)
		{
			m_ClickStatus.Add(new ButtonClickStatus());
		}
		if (s_DoubleClickTime == -1)
		{
			s_DoubleClickTime = Event.GetDoubleClickTime();
		}
	}

	private void StartClickTracking(EventBase evt)
	{
		if (evt is IPointerEvent pointerEvent)
		{
			ButtonClickStatus buttonClickStatus = m_ClickStatus[pointerEvent.pointerId];
			VisualElement elementTarget = evt.elementTarget;
			if (elementTarget != buttonClickStatus.m_Target)
			{
				buttonClickStatus.Reset();
			}
			buttonClickStatus.m_Target = elementTarget;
			if (evt.timestamp - buttonClickStatus.m_LastPointerDownTime > s_DoubleClickTime)
			{
				buttonClickStatus.m_ClickCount = 1;
			}
			else
			{
				buttonClickStatus.m_ClickCount++;
			}
			buttonClickStatus.m_LastPointerDownTime = evt.timestamp;
			buttonClickStatus.m_PointerDownPosition = pointerEvent.position;
		}
	}

	private void SendClickEvent(EventBase evt)
	{
		if (!(evt is IPointerEvent pointerEvent))
		{
			return;
		}
		ButtonClickStatus buttonClickStatus = m_ClickStatus[pointerEvent.pointerId];
		VisualElement elementTarget = evt.elementTarget;
		if (elementTarget == null || !ContainsPointer(elementTarget, pointerEvent) || buttonClickStatus.m_Target == null || buttonClickStatus.m_ClickCount <= 0)
		{
			return;
		}
		VisualElement visualElement = buttonClickStatus.m_Target.FindCommonAncestor(evt.elementTarget);
		if (visualElement == null)
		{
			return;
		}
		using ClickEvent clickEvent = ClickEvent.GetPooled(pointerEvent, buttonClickStatus.m_ClickCount);
		clickEvent.elementTarget = visualElement;
		visualElement.SendEvent(clickEvent, DispatchMode.Immediate);
	}

	private void CancelClickTracking(EventBase evt)
	{
		if (evt is IPointerEvent pointerEvent)
		{
			ButtonClickStatus buttonClickStatus = m_ClickStatus[pointerEvent.pointerId];
			buttonClickStatus.Reset();
		}
	}

	public void ProcessEvent<TEvent>(PointerEventBase<TEvent> evt) where TEvent : PointerEventBase<TEvent>, new()
	{
		if (evt.eventTypeId == EventBase<PointerDownEvent>.TypeId() && evt.button == 0)
		{
			StartClickTracking(evt);
		}
		else if (evt.eventTypeId == EventBase<PointerMoveEvent>.TypeId())
		{
			if (evt.button == 0 && (evt.pressedButtons & 1) == 1)
			{
				StartClickTracking(evt);
				return;
			}
			if (evt.button == 0 && (evt.pressedButtons & 1) == 0)
			{
				SendClickEvent(evt);
				return;
			}
			ButtonClickStatus buttonClickStatus = m_ClickStatus[evt.pointerId];
			if (buttonClickStatus.m_Target != null)
			{
				buttonClickStatus.m_LastPointerDownTime = 0L;
			}
		}
		else if (evt.eventTypeId == EventBase<PointerCancelEvent>.TypeId())
		{
			CancelClickTracking(evt);
		}
		else if (evt.eventTypeId == EventBase<PointerUpEvent>.TypeId() && evt.button == 0)
		{
			SendClickEvent(evt);
		}
	}

	private static bool ContainsPointer(VisualElement element, IPointerEvent pe)
	{
		if (!element.worldBound.Contains(pe.position) || element.panel == null)
		{
			return false;
		}
		if (element.elementPanel.isFlat)
		{
			VisualElement topElementUnderPointer = element.elementPanel.GetTopElementUnderPointer(pe.pointerId);
			return element == topElementUnderPointer || element.Contains(topElementUnderPointer);
		}
		return true;
	}

	internal void Cleanup(List<VisualElement> elements)
	{
		foreach (ButtonClickStatus item in m_ClickStatus)
		{
			if (item.m_Target != null && elements.Contains(item.m_Target))
			{
				item.Reset();
			}
		}
	}
}
