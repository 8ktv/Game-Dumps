namespace UnityEngine.UIElements;

internal static class PointerEventHelper
{
	public static EventBase GetPooled(EventType eventType, Vector3 mousePosition, Vector2 delta, int button, int clickCount, EventModifiers modifiers, int displayIndex)
	{
		if (eventType == EventType.MouseDown && !PointerDeviceState.HasAdditionalPressedButtons(PointerId.mousePointerId, button))
		{
			return PointerEventBase<PointerDownEvent>.GetPooled(eventType, mousePosition, delta, button, clickCount, modifiers, displayIndex);
		}
		if (eventType == EventType.MouseUp && !PointerDeviceState.HasAdditionalPressedButtons(PointerId.mousePointerId, button))
		{
			return PointerEventBase<PointerUpEvent>.GetPooled(eventType, mousePosition, delta, button, clickCount, modifiers, displayIndex);
		}
		return PointerEventBase<PointerMoveEvent>.GetPooled(eventType, mousePosition, delta, button, clickCount, modifiers, displayIndex);
	}
}
