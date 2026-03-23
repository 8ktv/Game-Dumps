using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.UIElements;

[AddComponentMenu("UI Toolkit/Panel Raycaster (UI Toolkit)")]
public class PanelRaycaster : BaseRaycaster, IRuntimePanelComponent
{
	private BaseRuntimePanel m_Panel;

	private static ScreenOverlayPanelPicker panelPicker = new ScreenOverlayPanelPicker();

	public IPanel panel
	{
		get
		{
			return m_Panel;
		}
		set
		{
			BaseRuntimePanel baseRuntimePanel = (BaseRuntimePanel)value;
			if (m_Panel != baseRuntimePanel)
			{
				UnregisterCallbacks();
				m_Panel = baseRuntimePanel;
				RegisterCallbacks();
			}
		}
	}

	private GameObject selectableGameObject => m_Panel?.selectableGameObject;

	public override int sortOrderPriority => Mathf.FloorToInt(m_Panel?.sortingPriority ?? 0f);

	public override int renderOrderPriority => int.MaxValue - (UIElementsRuntimeUtility.s_ResolvedSortingIndexMax - (m_Panel?.resolvedSortingIndex ?? 0));

	public override Camera eventCamera => null;

	private void RegisterCallbacks()
	{
		if (m_Panel != null)
		{
			m_Panel.destroyed += OnPanelDestroyed;
		}
	}

	private void UnregisterCallbacks()
	{
		if (m_Panel != null)
		{
			m_Panel.destroyed -= OnPanelDestroyed;
		}
	}

	private void OnPanelDestroyed()
	{
		panel = null;
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (m_Panel == null || !m_Panel.isFlat)
		{
			return;
		}
		int targetDisplay = m_Panel.targetDisplay;
		Vector3 relativeMousePositionForRaycast = MultipleDisplayUtilities.GetRelativeMousePositionForRaycast(eventData);
		Vector3 vector = relativeMousePositionForRaycast;
		Vector2 delta = eventData.delta;
		float num = Screen.height;
		if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
		{
			num = Display.displays[targetDisplay].systemHeight;
		}
		vector.y = num - vector.y;
		delta.y = 0f - delta.y;
		BaseInputModule currentInputModule = eventData.currentInputModule;
		if (!(currentInputModule == null))
		{
			int pointerId = currentInputModule.ConvertUIToolkitPointerId(eventData);
			if (panelPicker.TryPick((RuntimePanel)m_Panel, pointerId, vector, delta, (int)relativeMousePositionForRaycast.z, out var _))
			{
				resultAppendList.Add(new RaycastResult
				{
					gameObject = selectableGameObject,
					module = this,
					screenPosition = relativeMousePositionForRaycast,
					displayIndex = m_Panel.targetDisplay
				});
			}
		}
	}
}
