namespace UnityEngine.UIElements;

internal class PhysicsDocumentPicker
{
	private void Pick(Ray worldRay, float maxDistance, int layerMask, out Collider collider, out UIDocument document, out VisualElement pickedElement, out float distance)
	{
		WorldSpaceInput.PickResult pickResult = WorldSpaceInput.PickDocument3D(worldRay, maxDistance, layerMask);
		collider = pickResult.collider;
		document = pickResult.document;
		pickedElement = pickResult.pickedElement;
		distance = pickResult.distance;
	}

	public bool TryPickWithCapture(int pointerId, Ray worldRay, float maxDistance, int layerMask, out Collider collider, out UIDocument document, out VisualElement elementUnderPointer, out float distance, out bool captured)
	{
		captured = GetCapturingDocument(pointerId, out var capturingDocument);
		if (!captured)
		{
			Pick(worldRay, maxDistance, layerMask, out collider, out document, out elementUnderPointer, out distance);
			return !float.IsPositiveInfinity(distance);
		}
		if (capturingDocument != null && ((1 << capturingDocument.gameObject.layer) & layerMask) != 0)
		{
			collider = null;
			document = capturingDocument;
			elementUnderPointer = WorldSpaceInput.Pick3D(document, worldRay, out distance);
			return true;
		}
		collider = null;
		document = null;
		elementUnderPointer = null;
		distance = 0f;
		return false;
	}

	private bool GetCapturingDocument(int pointerId, out UIDocument capturingDocument)
	{
		IEventHandler capturingElement = RuntimePanel.s_EventDispatcher.pointerState.GetCapturingElement(pointerId);
		if (capturingElement is VisualElement { elementPanel: { isFlat: false } } visualElement)
		{
			capturingDocument = UIDocument.FindRootUIDocument(visualElement);
			return true;
		}
		RuntimePanel playerPanelWithSoftPointerCapture = PointerDeviceState.GetPlayerPanelWithSoftPointerCapture(pointerId);
		if (playerPanelWithSoftPointerCapture != null && !playerPanelWithSoftPointerCapture.isFlat)
		{
			capturingDocument = PointerDeviceState.GetWorldSpaceDocumentWithSoftPointerCapture(pointerId);
			return true;
		}
		capturingDocument = null;
		return false;
	}
}
