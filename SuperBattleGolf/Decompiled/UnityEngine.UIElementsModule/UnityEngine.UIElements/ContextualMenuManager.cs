namespace UnityEngine.UIElements;

public abstract class ContextualMenuManager
{
	internal bool displayMenuHandledOSX { get; set; }

	public abstract void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler);

	public void DisplayMenu(EventBase triggerEvent, IEventHandler target)
	{
		DropdownMenu menu = new DropdownMenu();
		DisplayMenu(triggerEvent, target, menu);
	}

	internal void DisplayMenu(EventBase triggerEvent, IEventHandler target, DropdownMenu menu)
	{
		int pointerId;
		using (ContextualMenuPopulateEvent contextualMenuPopulateEvent = ContextualMenuPopulateEvent.GetPooled(triggerEvent, menu, target, this))
		{
			pointerId = ((triggerEvent is IPointerEvent pointerEvent) ? pointerEvent.pointerId : PointerId.mousePointerId);
			int button = contextualMenuPopulateEvent.button;
			target?.SendEvent(contextualMenuPopulateEvent);
		}
		if (UIElementsUtility.isOSXContextualMenuPlatform)
		{
			displayMenuHandledOSX = true;
			ResetPointerDown(pointerId);
		}
	}

	protected internal abstract void DoDisplayMenu(DropdownMenu menu, EventBase triggerEvent);

	internal static void ResetPointerDown(int pointerId)
	{
		PointerDeviceState.ReleaseAllButtons(pointerId);
	}
}
