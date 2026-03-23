namespace UnityEngine.UIElements;

internal class PointerDispatchState
{
	private IEventHandler[] m_PendingPointerCapture = new IEventHandler[PointerId.maxPointers];

	private IEventHandler[] m_PointerCapture = new IEventHandler[PointerId.maxPointers];

	private bool[] m_ShouldSendCompatibilityMouseEvents = new bool[PointerId.maxPointers];

	public PointerDispatchState()
	{
		Reset();
	}

	internal void Reset()
	{
		for (int i = 0; i < m_PointerCapture.Length; i++)
		{
			m_PendingPointerCapture[i] = null;
			m_PointerCapture[i] = null;
			m_ShouldSendCompatibilityMouseEvents[i] = true;
		}
	}

	public IEventHandler GetCapturingElement(int pointerId)
	{
		return m_PendingPointerCapture[pointerId];
	}

	public bool HasPointerCapture(IEventHandler handler, int pointerId)
	{
		return m_PendingPointerCapture[pointerId] == handler;
	}

	public void CapturePointer(IEventHandler handler, int pointerId)
	{
		IEventHandler eventHandler = m_PendingPointerCapture[pointerId];
		if (eventHandler != handler)
		{
			if (pointerId == PointerId.mousePointerId && GUIUtility.hotControl != 0)
			{
				GUIUtility.hotControl = 0;
			}
			m_PendingPointerCapture[pointerId] = handler;
			(eventHandler as VisualElement)?.UpdatePointerCaptureFlag();
			(handler as VisualElement)?.UpdatePointerCaptureFlag();
		}
	}

	public void ReleasePointer(int pointerId)
	{
		IEventHandler eventHandler = m_PendingPointerCapture[pointerId];
		if (eventHandler != null)
		{
			m_PendingPointerCapture[pointerId] = null;
			(eventHandler as VisualElement)?.UpdatePointerCaptureFlag();
		}
	}

	public void ReleasePointer(IEventHandler handler, int pointerId)
	{
		if (handler == m_PendingPointerCapture[pointerId])
		{
			ReleasePointer(pointerId);
		}
	}

	public void ProcessPointerCapture(int pointerId)
	{
		IEventHandler eventHandler = m_PointerCapture[pointerId];
		if (eventHandler == m_PendingPointerCapture[pointerId])
		{
			return;
		}
		if (eventHandler != null)
		{
			using (PointerCaptureOutEvent e = PointerCaptureEventBase<PointerCaptureOutEvent>.GetPooled(eventHandler, m_PendingPointerCapture[pointerId], pointerId))
			{
				eventHandler.SendEvent(e);
			}
			if (pointerId == PointerId.mousePointerId && m_PointerCapture[pointerId] == eventHandler)
			{
				using MouseCaptureOutEvent e2 = PointerCaptureEventBase<MouseCaptureOutEvent>.GetPooled(eventHandler, m_PendingPointerCapture[pointerId], pointerId);
				eventHandler.SendEvent(e2);
			}
		}
		IEventHandler eventHandler2 = m_PendingPointerCapture[pointerId];
		if (eventHandler2 != null)
		{
			using (PointerCaptureEvent e3 = PointerCaptureEventBase<PointerCaptureEvent>.GetPooled(eventHandler2, m_PointerCapture[pointerId], pointerId))
			{
				eventHandler2.SendEvent(e3);
			}
			if (pointerId == PointerId.mousePointerId && m_PendingPointerCapture[pointerId] == eventHandler2)
			{
				using MouseCaptureEvent e4 = PointerCaptureEventBase<MouseCaptureEvent>.GetPooled(eventHandler2, m_PointerCapture[pointerId], pointerId);
				eventHandler2.SendEvent(e4);
			}
		}
		m_PointerCapture[pointerId] = m_PendingPointerCapture[pointerId];
	}

	public void ActivateCompatibilityMouseEvents(int pointerId)
	{
		m_ShouldSendCompatibilityMouseEvents[pointerId] = true;
	}

	public void PreventCompatibilityMouseEvents(int pointerId)
	{
		m_ShouldSendCompatibilityMouseEvents[pointerId] = false;
	}

	public bool ShouldSendCompatibilityMouseEvents(IPointerEvent evt)
	{
		return evt.isPrimary && m_ShouldSendCompatibilityMouseEvents[evt.pointerId];
	}
}
