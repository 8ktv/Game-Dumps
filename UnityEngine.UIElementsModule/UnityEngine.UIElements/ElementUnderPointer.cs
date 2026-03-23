#define UNITY_ASSERTIONS
namespace UnityEngine.UIElements;

internal class ElementUnderPointer
{
	private VisualElement[] m_PendingTopElementUnderPointer = new VisualElement[PointerId.maxPointers];

	private VisualElement[] m_TopElementUnderPointer = new VisualElement[PointerId.maxPointers];

	private IPointerOrMouseEvent[] m_TriggerEvent = new IPointerOrMouseEvent[PointerId.maxPointers];

	private Vector2[] m_PickingPointerPositions = new Vector2[PointerId.maxPointers];

	private readonly bool[] m_IsPrimaryPointer;

	private bool[] m_IsPickingPointerTemporaries = new bool[PointerId.maxPointers];

	public ElementUnderPointer()
	{
		m_IsPrimaryPointer = new bool[PointerId.maxPointers];
		m_IsPrimaryPointer[PointerId.mousePointerId] = true;
		m_IsPrimaryPointer[PointerId.touchPointerIdBase] = true;
		for (int i = 0; i < PointerId.penPointerCount; i++)
		{
			m_IsPrimaryPointer[PointerId.penPointerIdBase + i] = true;
		}
	}

	internal VisualElement GetTopElementUnderPointer(int pointerId, out Vector2 pickPosition, out bool isTemporary)
	{
		pickPosition = m_PickingPointerPositions[pointerId];
		isTemporary = m_IsPickingPointerTemporaries[pointerId];
		return m_PendingTopElementUnderPointer[pointerId];
	}

	internal VisualElement GetTopElementUnderPointer(int pointerId)
	{
		return m_PendingTopElementUnderPointer[pointerId];
	}

	internal void RemoveElementUnderPointer(VisualElement elementToRemove)
	{
		for (int i = 0; i < m_TopElementUnderPointer.Length; i++)
		{
			VisualElement visualElement = m_TopElementUnderPointer[i];
			if (visualElement == elementToRemove)
			{
				SetElementUnderPointer(null, i, null);
			}
		}
	}

	internal void SetElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, Vector2 pointerPos)
	{
		Debug.Assert(pointerId >= 0, "SetElementUnderPointer expects pointerId >= 0");
		VisualElement visualElement = m_TopElementUnderPointer[pointerId];
		m_IsPickingPointerTemporaries[pointerId] = false;
		m_PickingPointerPositions[pointerId] = pointerPos;
		if (visualElement != newElementUnderPointer)
		{
			m_PendingTopElementUnderPointer[pointerId] = newElementUnderPointer;
			m_TriggerEvent[pointerId] = null;
		}
	}

	private Vector2 GetEventPointerPosition(EventBase triggerEvent)
	{
		if (triggerEvent is IPointerEvent pointerEvent)
		{
			return new Vector2(pointerEvent.position.x, pointerEvent.position.y);
		}
		if (!(triggerEvent is IMouseEvent { mousePosition: var mousePosition }))
		{
			return new Vector2(float.MinValue, float.MinValue);
		}
		return mousePosition;
	}

	internal void SetTemporaryElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, EventBase triggerEvent)
	{
		SetElementUnderPointer(newElementUnderPointer, pointerId, triggerEvent, temporary: true);
	}

	internal void SetElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, EventBase triggerEvent)
	{
		SetElementUnderPointer(newElementUnderPointer, pointerId, triggerEvent, temporary: false);
	}

	private void SetElementUnderPointer(VisualElement newElementUnderPointer, int pointerId, EventBase triggerEvent, bool temporary)
	{
		Debug.Assert(pointerId >= 0, "SetElementUnderPointer expects pointerId >= 0");
		m_IsPickingPointerTemporaries[pointerId] = temporary;
		m_PickingPointerPositions[pointerId] = GetEventPointerPosition(triggerEvent);
		m_PendingTopElementUnderPointer[pointerId] = newElementUnderPointer;
		VisualElement visualElement = m_TopElementUnderPointer[pointerId];
		if (visualElement != newElementUnderPointer && m_TriggerEvent[pointerId] == null && triggerEvent is IPointerOrMouseEvent pointerOrMouseEvent)
		{
			m_TriggerEvent[pointerId] = pointerOrMouseEvent;
			m_IsPrimaryPointer[pointerId] = !(pointerOrMouseEvent is IPointerEvent pointerEvent) || pointerEvent.isPrimary;
		}
	}

	internal bool CommitElementUnderPointers(EventDispatcher dispatcher, ContextType contextType)
	{
		bool result = false;
		bool flag = false;
		for (int i = 0; i < PointerId.maxPointers; i++)
		{
			IPointerOrMouseEvent pointerOrMouseEvent = m_TriggerEvent[i];
			VisualElement visualElement = m_TopElementUnderPointer[i];
			VisualElement visualElement2 = m_PendingTopElementUnderPointer[i];
			if (visualElement == visualElement2)
			{
				if (pointerOrMouseEvent != null)
				{
					m_PickingPointerPositions[i] = pointerOrMouseEvent.position;
				}
				continue;
			}
			result = true;
			m_TopElementUnderPointer[i] = visualElement2;
			Vector2 vector = pointerOrMouseEvent?.position ?? PointerDeviceState.GetPointerPosition(i, contextType);
			m_PickingPointerPositions[i] = vector;
			using (new EventDispatcherGate(dispatcher))
			{
				IPointerEvent triggerEvent = pointerOrMouseEvent as IPointerEvent;
				PointerEventsHelper.SendOverOut(visualElement, visualElement2, triggerEvent, vector, i);
				PointerEventsHelper.SendEnterLeave<PointerLeaveEvent, PointerEnterEvent>(visualElement, visualElement2, null, vector, i);
				IMouseEvent mouseEvent = (pointerOrMouseEvent as IMouseEvent) ?? (pointerOrMouseEvent as IPointerEventInternal)?.compatibilityMouseEvent;
				if ((mouseEvent != null || m_IsPrimaryPointer[i]) && !flag)
				{
					flag = true;
					MouseEventsHelper.SendMouseOverMouseOut(visualElement, visualElement2, mouseEvent, vector);
					MouseEventsHelper.SendEnterLeave<MouseLeaveEvent, MouseEnterEvent>(visualElement, visualElement2, mouseEvent, vector);
				}
			}
			m_TriggerEvent[i] = null;
		}
		return result;
	}
}
