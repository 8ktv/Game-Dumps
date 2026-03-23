namespace UnityEngine.UIElements;

internal class ScreenOverlayPanelPicker
{
	public bool TryPick(BaseRuntimePanel panel, int pointerId, Vector2 screenPosition, Vector2 delta, int? targetDisplay, out bool captured)
	{
		if (targetDisplay.HasValue && targetDisplay != panel.targetDisplay)
		{
			captured = false;
			return false;
		}
		captured = GetCapturingPanel(pointerId, out var capturingPanel);
		Vector3 panelPosition;
		if (captured)
		{
			if (capturingPanel == panel)
			{
				return true;
			}
		}
		else if (panel.ScreenToPanel(screenPosition, delta, out panelPosition))
		{
			VisualElement visualElement = panel.Pick(panelPosition, pointerId);
			if (visualElement != null)
			{
				return true;
			}
		}
		return false;
	}

	private bool GetCapturingPanel(int pointerId, out BaseVisualElementPanel capturingPanel)
	{
		IEventHandler capturingElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId);
		if (capturingElement is VisualElement visualElement)
		{
			capturingPanel = visualElement.elementPanel;
		}
		else
		{
			capturingPanel = PointerDeviceState.GetPlayerPanelWithSoftPointerCapture(pointerId);
		}
		return capturingPanel != null;
	}
}
