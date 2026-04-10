using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UIElements;

internal class UIToolkitInteroperabilityBridge
{
	[Flags]
	public enum EventHandlerTypes
	{
		ScreenOverlay = 1,
		WorldSpace = 2
	}

	private EventSystem m_EventSystem;

	private bool m_OverrideUIToolkitEvents = true;

	private EventHandlerTypes m_HandlerTypes = EventHandlerTypes.ScreenOverlay | EventHandlerTypes.WorldSpace;

	private LayerMask m_WorldPickingLayers = -5;

	private float m_WorldPickingMaxDistance = float.PositiveInfinity;

	private bool m_CreateDefaultPanelComponents = true;

	private bool m_Started;

	private bool m_Enabled;

	private bool m_IsTrackingPanels;

	private GameObject m_WorldSpaceGo;

	private readonly HashSet<BaseRuntimePanel> trackedPanels = new HashSet<BaseRuntimePanel>();

	private readonly Dictionary<BaseRuntimePanel, Action> destroyedActions = new Dictionary<BaseRuntimePanel, Action>();

	private PanelInputConfiguration.Settings m_InputSettings = PanelInputConfiguration.Settings.Default;

	private bool m_OldOverrideUIToolkitEvents = true;

	private EventHandlerTypes m_OldHandlerTypes = EventHandlerTypes.ScreenOverlay | EventHandlerTypes.WorldSpace;

	private bool m_OldCreateDefaultPanelComponents = true;

	private bool m_OldDefaultEventCameraIsMainCamera = true;

	private long m_OldEventCamerasHash;

	private List<BaseRuntimePanel> m_PanelsToRemove = new List<BaseRuntimePanel>();

	internal EventSystem eventSystem
	{
		get
		{
			return m_EventSystem;
		}
		set
		{
			if (!(m_EventSystem == value))
			{
				m_EventSystem = value;
			}
		}
	}

	public bool overrideUIToolkitEvents
	{
		get
		{
			return m_OverrideUIToolkitEvents;
		}
		internal set
		{
			m_OverrideUIToolkitEvents = value;
			ApplyOverrideUIToolkitEvents();
		}
	}

	public EventHandlerTypes handlerTypes
	{
		get
		{
			return m_HandlerTypes;
		}
		internal set
		{
			m_HandlerTypes = value;
			ApplyOtherProperties();
		}
	}

	public int worldPickingLayers
	{
		get
		{
			return m_WorldPickingLayers;
		}
		internal set
		{
			m_WorldPickingLayers = value;
		}
	}

	public float worldPickingMaxDistance
	{
		get
		{
			return m_WorldPickingMaxDistance;
		}
		internal set
		{
			m_WorldPickingMaxDistance = value;
		}
	}

	public bool createDefaultPanelComponents
	{
		get
		{
			return m_CreateDefaultPanelComponents;
		}
		internal set
		{
			m_CreateDefaultPanelComponents = value;
			ApplyOtherProperties();
		}
	}

	private bool shouldTrackPanels
	{
		get
		{
			if (overrideUIToolkitEvents && createDefaultPanelComponents && m_Started)
			{
				return m_Enabled;
			}
			return false;
		}
	}

	private void StartTrackingUIToolkitPanels()
	{
		if (m_IsTrackingPanels || !shouldTrackPanels)
		{
			return;
		}
		foreach (BaseRuntimePanel sortedPlayerPanel in UIElementsRuntimeUtility.GetSortedPlayerPanels())
		{
			StartTrackingPanel(sortedPlayerPanel);
		}
		UIElementsRuntimeUtility.onCreatePanel += StartTrackingPanel;
		m_IsTrackingPanels = true;
	}

	private void StartTrackingPanel(BaseRuntimePanel panel)
	{
		trackedPanels.Add(panel);
	}

	private void StopTrackingUIToolkitPanels()
	{
		if (!m_IsTrackingPanels)
		{
			return;
		}
		UIElementsRuntimeUtility.onCreatePanel -= StartTrackingPanel;
		m_IsTrackingPanels = false;
		foreach (BaseRuntimePanel trackedPanel in trackedPanels)
		{
			DestroyPanelGameObject(trackedPanel);
		}
		trackedPanels.Clear();
		DestroyWorldSpacePanelGameObject();
	}

	private void UpdatePanelGameObject(BaseRuntimePanel panel)
	{
		EventHandlerTypes eventHandlerTypes = (panel.isFlat ? EventHandlerTypes.ScreenOverlay : EventHandlerTypes.WorldSpace);
		if ((m_HandlerTypes & eventHandlerTypes) != 0)
		{
			CreatePanelGameObject(panel);
		}
		else
		{
			DestroyPanelGameObject(panel);
		}
	}

	private void CreatePanelGameObject(BaseRuntimePanel panel)
	{
		if (panel.selectableGameObject == null)
		{
			GameObject gameObject = new GameObject(panel.name, typeof(PanelEventHandler), typeof(PanelRaycaster));
			gameObject.transform.SetParent(m_EventSystem.transform);
			panel.selectableGameObject = gameObject;
			Action action = (destroyedActions[panel] = delegate
			{
				DestroyPanelGameObject(panel);
			});
			Action value = action;
			panel.destroyed += value;
		}
	}

	private void DestroyPanelGameObject(BaseRuntimePanel panel)
	{
		GameObject selectableGameObject = panel.selectableGameObject;
		if (selectableGameObject != null && destroyedActions.Remove(panel, out var value))
		{
			panel.destroyed -= value;
			panel.selectableGameObject = null;
			UIRUtility.Destroy(selectableGameObject);
		}
	}

	private void CreateWorldSpacePanelGameObject()
	{
		ApplyCameraProperties();
		if (!(m_WorldSpaceGo == null))
		{
			return;
		}
		GameObject gameObject = new GameObject("WorldDocumentRaycaster");
		gameObject.transform.SetParent(m_EventSystem.transform);
		if (m_InputSettings.defaultEventCameraIsMainCamera)
		{
			gameObject.AddComponent<WorldDocumentRaycaster>();
		}
		else
		{
			Camera[] eventCameras = m_InputSettings.eventCameras;
			foreach (Camera camera in eventCameras)
			{
				gameObject.AddComponent<WorldDocumentRaycaster>().camera = camera;
			}
		}
		m_WorldSpaceGo = gameObject;
	}

	private void DestroyWorldSpacePanelGameObject()
	{
		GameObject worldSpaceGo = m_WorldSpaceGo;
		m_WorldSpaceGo = null;
		UIRUtility.Destroy(worldSpaceGo);
	}

	public void Start()
	{
		m_Started = true;
		StartTrackingUIToolkitPanels();
	}

	public void OnEnable()
	{
		if (!m_Enabled)
		{
			m_Enabled = true;
			if (PanelInputConfiguration.current != null)
			{
				Apply(PanelInputConfiguration.current);
			}
			PanelInputConfiguration.onApply = (Action<PanelInputConfiguration>)Delegate.Combine(PanelInputConfiguration.onApply, new Action<PanelInputConfiguration>(Apply));
			if (m_Started)
			{
				StartTrackingUIToolkitPanels();
			}
			if (m_OverrideUIToolkitEvents)
			{
				UIElementsRuntimeUtility.RegisterEventSystem(m_EventSystem);
			}
		}
	}

	public void OnDisable()
	{
		if (m_Enabled)
		{
			m_Enabled = false;
			PanelInputConfiguration.onApply = (Action<PanelInputConfiguration>)Delegate.Remove(PanelInputConfiguration.onApply, new Action<PanelInputConfiguration>(Apply));
			StopTrackingUIToolkitPanels();
			UIElementsRuntimeUtility.UnregisterEventSystem(m_EventSystem);
		}
	}

	public void Update()
	{
		UpdatePanelGameObjects();
	}

	private void Apply(PanelInputConfiguration input)
	{
		m_InputSettings = ((input != null) ? input.settings : PanelInputConfiguration.Settings.Default);
		m_OverrideUIToolkitEvents = m_InputSettings.panelInputRedirection != PanelInputConfiguration.PanelInputRedirection.Never;
		m_HandlerTypes = (EventHandlerTypes)(1 | (m_InputSettings.processWorldSpaceInput ? 2 : 0));
		m_WorldPickingLayers = m_InputSettings.interactionLayers;
		m_WorldPickingMaxDistance = m_InputSettings.maxInteractionDistance;
		m_CreateDefaultPanelComponents = m_InputSettings.autoCreatePanelComponents;
		ApplyOverrideUIToolkitEvents();
		ApplyCameraProperties();
		ApplyOtherProperties();
	}

	private void ApplyOverrideUIToolkitEvents()
	{
		if (m_OldOverrideUIToolkitEvents == m_OverrideUIToolkitEvents)
		{
			return;
		}
		m_OldOverrideUIToolkitEvents = m_OverrideUIToolkitEvents;
		if (m_Enabled)
		{
			if (m_OverrideUIToolkitEvents)
			{
				UIElementsRuntimeUtility.RegisterEventSystem(m_EventSystem);
			}
			else
			{
				UIElementsRuntimeUtility.UnregisterEventSystem(m_EventSystem);
			}
			UpdatePanelTracking();
		}
	}

	private void ApplyCameraProperties()
	{
		bool flag = false;
		if (m_OldDefaultEventCameraIsMainCamera != m_InputSettings.defaultEventCameraIsMainCamera)
		{
			m_OldDefaultEventCameraIsMainCamera = m_InputSettings.defaultEventCameraIsMainCamera;
			flag = true;
		}
		if (!m_InputSettings.defaultEventCameraIsMainCamera)
		{
			int num = 0;
			Camera[] eventCameras = m_InputSettings.eventCameras;
			foreach (Camera camera in eventCameras)
			{
				num = (num * 397) ^ camera.GetHashCode();
			}
			if (m_OldEventCamerasHash != num)
			{
				m_OldEventCamerasHash = num;
				flag = true;
			}
		}
		else
		{
			m_OldEventCamerasHash = 0L;
		}
		if (flag)
		{
			DestroyWorldSpacePanelGameObject();
		}
	}

	private void ApplyOtherProperties()
	{
		bool flag = false;
		if (m_OldHandlerTypes != m_HandlerTypes)
		{
			m_OldHandlerTypes = m_HandlerTypes;
			flag = true;
		}
		if (m_OldCreateDefaultPanelComponents != m_CreateDefaultPanelComponents)
		{
			m_OldCreateDefaultPanelComponents = m_CreateDefaultPanelComponents;
			flag = true;
		}
		if (flag)
		{
			UpdatePanelTracking();
		}
	}

	private void UpdatePanelTracking()
	{
		if (shouldTrackPanels)
		{
			StartTrackingUIToolkitPanels();
		}
		else
		{
			StopTrackingUIToolkitPanels();
		}
	}

	private void UpdatePanelGameObjects()
	{
		if (!m_IsTrackingPanels)
		{
			return;
		}
		bool flag = false;
		foreach (BaseRuntimePanel trackedPanel in trackedPanels)
		{
			if (trackedPanel.disposed)
			{
				m_PanelsToRemove.Add(trackedPanel);
				continue;
			}
			UpdatePanelGameObject(trackedPanel);
			flag |= !trackedPanel.isFlat;
		}
		foreach (BaseRuntimePanel item in m_PanelsToRemove)
		{
			trackedPanels.Remove(item);
		}
		m_PanelsToRemove.Clear();
		if (flag && (m_HandlerTypes & EventHandlerTypes.WorldSpace) != 0)
		{
			CreateWorldSpacePanelGameObject();
		}
		else
		{
			DestroyWorldSpacePanelGameObject();
		}
	}
}
