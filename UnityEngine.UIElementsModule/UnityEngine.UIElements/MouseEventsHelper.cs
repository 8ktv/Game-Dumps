using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal static class MouseEventsHelper
{
	internal static void SendEnterLeave<TLeaveEvent, TEnterEvent>(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, IMouseEvent triggerEvent, Vector2 mousePosition) where TLeaveEvent : MouseEventBase<TLeaveEvent>, new() where TEnterEvent : MouseEventBase<TEnterEvent>, new()
	{
		if (previousTopElementUnderMouse != null && previousTopElementUnderMouse.panel == null)
		{
			previousTopElementUnderMouse = null;
		}
		int num = 0;
		VisualElement visualElement;
		for (visualElement = previousTopElementUnderMouse; visualElement != null; visualElement = visualElement.hierarchy.parent)
		{
			num++;
		}
		int num2 = 0;
		VisualElement visualElement2;
		for (visualElement2 = currentTopElementUnderMouse; visualElement2 != null; visualElement2 = visualElement2.hierarchy.parent)
		{
			num2++;
		}
		visualElement = previousTopElementUnderMouse;
		visualElement2 = currentTopElementUnderMouse;
		while (num > num2)
		{
			using (TLeaveEvent val = MouseEventBase<TLeaveEvent>.GetPooled(triggerEvent, mousePosition))
			{
				val.elementTarget = visualElement;
				visualElement.SendEvent(val);
			}
			num--;
			visualElement = visualElement.hierarchy.parent;
		}
		List<VisualElement> list = VisualElementListPool.Get(num2);
		while (num2 > num)
		{
			list.Add(visualElement2);
			num2--;
			visualElement2 = visualElement2.hierarchy.parent;
		}
		while (visualElement != visualElement2)
		{
			using (TLeaveEvent val2 = MouseEventBase<TLeaveEvent>.GetPooled(triggerEvent, mousePosition))
			{
				val2.elementTarget = visualElement;
				visualElement.SendEvent(val2);
			}
			list.Add(visualElement2);
			visualElement = visualElement.hierarchy.parent;
			visualElement2 = visualElement2.hierarchy.parent;
		}
		for (int num3 = list.Count - 1; num3 >= 0; num3--)
		{
			using TEnterEvent val3 = MouseEventBase<TEnterEvent>.GetPooled(triggerEvent, mousePosition);
			val3.elementTarget = list[num3];
			list[num3].SendEvent(val3);
		}
		VisualElementListPool.Release(list);
	}

	internal static void SendMouseOverMouseOut(VisualElement previousTopElementUnderMouse, VisualElement currentTopElementUnderMouse, IMouseEvent triggerEvent, Vector2 mousePosition)
	{
		if (previousTopElementUnderMouse != null && previousTopElementUnderMouse.panel != null)
		{
			using MouseOutEvent mouseOutEvent = MouseEventBase<MouseOutEvent>.GetPooled(triggerEvent, mousePosition);
			mouseOutEvent.elementTarget = previousTopElementUnderMouse;
			previousTopElementUnderMouse.SendEvent(mouseOutEvent);
		}
		if (currentTopElementUnderMouse != null)
		{
			using (MouseOverEvent mouseOverEvent = MouseEventBase<MouseOverEvent>.GetPooled(triggerEvent, mousePosition))
			{
				mouseOverEvent.elementTarget = currentTopElementUnderMouse;
				currentTopElementUnderMouse.SendEvent(mouseOverEvent);
			}
		}
	}
}
