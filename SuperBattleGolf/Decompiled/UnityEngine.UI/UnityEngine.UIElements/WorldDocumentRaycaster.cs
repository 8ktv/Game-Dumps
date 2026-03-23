using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UIElements;

[AddComponentMenu("UI Toolkit/World Document Raycaster (UI Toolkit)")]
public class WorldDocumentRaycaster : BaseRaycaster
{
	[SerializeField]
	private Camera m_EventCamera;

	private static PhysicsDocumentPicker worldPicker = new PhysicsDocumentPicker();

	public override Camera eventCamera => m_EventCamera;

	public Camera camera
	{
		get
		{
			return m_EventCamera;
		}
		set
		{
			m_EventCamera = value;
		}
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		BaseInputModule baseInputModule = ((EventSystem.current != null) ? EventSystem.current.currentInputModule : null);
		if (baseInputModule == null || !GetWorldRay(eventData, out var worldRay, out var maxDistance, out var layerMask))
		{
			return;
		}
		maxDistance = Mathf.Min(maxDistance, EventSystem.current.uiToolkitInterop.worldPickingMaxDistance);
		layerMask &= EventSystem.current.uiToolkitInterop.worldPickingLayers;
		int pointerId = baseInputModule.ConvertUIToolkitPointerId(eventData);
		Camera cameraWithSoftPointerCapture = PointerDeviceState.GetCameraWithSoftPointerCapture(pointerId);
		if (cameraWithSoftPointerCapture != null)
		{
			Camera camera = ((m_EventCamera != null) ? m_EventCamera : Camera.main);
			if (cameraWithSoftPointerCapture != camera)
			{
				return;
			}
		}
		if (worldPicker.TryPickWithCapture(pointerId, worldRay, maxDistance, layerMask, out var _, out var document, out var elementUnderPointer, out var distance, out var captured))
		{
			resultAppendList.Add(new RaycastResult
			{
				gameObject = ((document == null) ? base.gameObject : document.containerPanel.selectableGameObject),
				origin = worldRay.origin,
				worldPosition = worldRay.origin + distance * worldRay.direction,
				document = document,
				element = elementUnderPointer,
				module = this,
				distance = distance,
				sortingOrder = (captured ? int.MaxValue : 0)
			});
		}
	}

	protected virtual bool GetWorldRay(PointerEventData eventData, out Ray worldRay, out float maxDistance, out int layerMask)
	{
		Camera camera = ((m_EventCamera != null) ? m_EventCamera : Camera.main);
		if (camera == null)
		{
			worldRay = default(Ray);
			maxDistance = 0f;
			layerMask = 0;
			return false;
		}
		maxDistance = camera.farClipPlane;
		layerMask = camera.cullingMask;
		Vector3 relativeMousePositionForRaycast = MultipleDisplayUtilities.GetRelativeMousePositionForRaycast(eventData);
		if ((int)relativeMousePositionForRaycast.z != camera.targetDisplay)
		{
			worldRay = default(Ray);
			return false;
		}
		worldRay = camera.ScreenPointToRay(relativeMousePositionForRaycast);
		return true;
	}
}
