using System;

namespace UnityEngine.UIElements;

[AddComponentMenu("UI Toolkit/Panel Input Configuration", 1)]
[ExecuteAlways]
[DisallowMultipleComponent]
[HelpURL("UIE-get-started-with-runtime-ui")]
public sealed class PanelInputConfiguration : MonoBehaviour
{
	public enum PanelInputRedirection
	{
		[InspectorName("Auto-switch (redirect from EventSystem if present)")]
		AutoSwitch,
		[InspectorName("No input redirection")]
		Never,
		[InspectorName("Always redirect from EventSystem (wait if unavailable)")]
		Always
	}

	[Serializable]
	internal struct Settings
	{
		private static Settings s_Default = new Settings
		{
			m_ProcessWorldSpaceInput = true,
			m_InteractionLayers = -5,
			m_MaxInteractionDistance = float.PositiveInfinity,
			m_DefaultEventCameraIsMainCamera = true,
			m_EventCameras = Array.Empty<Camera>(),
			m_PanelInputRedirection = PanelInputRedirection.AutoSwitch,
			m_AutoCreatePanelComponents = true
		};

		[Tooltip("Determines whether world space panels process input events. Disable this if you need UGUI support but do not require world space input to improve performance.")]
		[SerializeField]
		internal bool m_ProcessWorldSpaceInput;

		[Tooltip("Determines which layers can block input events on world space panels.")]
		[SerializeField]
		internal LayerMask m_InteractionLayers;

		[Tooltip("Sets how far away interactions with world-space UI are possible. Defaults to unlimited (infinity), but you can customize it for XR or performance needs. The distance uses GameObject units, consistent with transform positions and Camera clipping planes.")]
		[SerializeField]
		internal float m_MaxInteractionDistance;

		[SerializeField]
		[Tooltip("Defines whether the Main Camera is used as the Event Camera for world space panels. Disable to specify alternative Event Camera(s) for raycasting input.")]
		internal bool m_DefaultEventCameraIsMainCamera;

		[Tooltip("Defines the Event Camera(s) used for world space raycasting input.")]
		[SerializeField]
		internal Camera[] m_EventCameras;

		[Tooltip("Determines which input event system is used for UI interactions when combining UI Toolkit and UGUI.")]
		[SerializeField]
		internal PanelInputRedirection m_PanelInputRedirection;

		[Tooltip("Automatically adds UI Toolkit components under the EventSystem to handle input redirection between UI Toolkit and UGUI panels. Disable to manually assign these components through code.")]
		[SerializeField]
		internal bool m_AutoCreatePanelComponents;

		public static Settings Default => s_Default;

		public bool processWorldSpaceInput => m_ProcessWorldSpaceInput;

		public LayerMask interactionLayers => m_InteractionLayers;

		public float maxInteractionDistance => m_MaxInteractionDistance;

		public bool defaultEventCameraIsMainCamera => m_DefaultEventCameraIsMainCamera;

		public Camera[] eventCameras => m_EventCameras;

		public PanelInputRedirection panelInputRedirection => m_PanelInputRedirection;

		public bool autoCreatePanelComponents => m_AutoCreatePanelComponents;
	}

	internal static int s_ActiveInstances;

	internal static Action<PanelInputConfiguration> onApply;

	[SerializeField]
	private Settings m_Settings = Settings.Default;

	internal const string SettingsProperty = "m_Settings";

	internal static PanelInputConfiguration current { get; set; }

	internal Settings settings => m_Settings;

	public bool processWorldSpaceInput
	{
		get
		{
			return m_Settings.m_ProcessWorldSpaceInput;
		}
		set
		{
			if (m_Settings.m_ProcessWorldSpaceInput != value)
			{
				m_Settings.m_ProcessWorldSpaceInput = value;
				Apply(this);
			}
		}
	}

	public LayerMask interactionLayers
	{
		get
		{
			return m_Settings.m_InteractionLayers;
		}
		set
		{
			if ((int)m_Settings.m_InteractionLayers != (int)value)
			{
				m_Settings.m_InteractionLayers = value;
				Apply(this);
			}
		}
	}

	public float maxInteractionDistance
	{
		get
		{
			return m_Settings.m_MaxInteractionDistance;
		}
		set
		{
			if (m_Settings.m_MaxInteractionDistance != value)
			{
				m_Settings.m_MaxInteractionDistance = value;
				Apply(this);
			}
		}
	}

	public bool defaultEventCameraIsMainCamera
	{
		get
		{
			return m_Settings.m_DefaultEventCameraIsMainCamera;
		}
		set
		{
			if (m_Settings.m_DefaultEventCameraIsMainCamera != value)
			{
				m_Settings.m_DefaultEventCameraIsMainCamera = value;
				Apply(this);
			}
		}
	}

	public Camera[] eventCameras
	{
		get
		{
			return m_Settings.m_EventCameras;
		}
		set
		{
			if (m_Settings.m_EventCameras != value)
			{
				m_Settings.m_EventCameras = value;
				Apply(this);
			}
		}
	}

	public PanelInputRedirection panelInputRedirection
	{
		get
		{
			return m_Settings.m_PanelInputRedirection;
		}
		set
		{
			if (m_Settings.m_PanelInputRedirection != value)
			{
				m_Settings.m_PanelInputRedirection = value;
				Apply(this);
			}
		}
	}

	public bool autoCreatePanelComponents
	{
		get
		{
			return m_Settings.m_AutoCreatePanelComponents;
		}
		set
		{
			if (m_Settings.m_AutoCreatePanelComponents != value)
			{
				m_Settings.m_AutoCreatePanelComponents = value;
				Apply(this);
			}
		}
	}

	private void OnEnable()
	{
		s_ActiveInstances++;
		if (current != null)
		{
			if (Application.isPlaying)
			{
				Debug.LogWarning("Multiple Input Configuration components active. Only one will be considered, the rest will be disabled.\nEnabled: " + current?.ToString() + ". Disabled: " + this?.ToString() + ".");
				base.enabled = false;
			}
		}
		else
		{
			current = this;
			Apply(this);
		}
	}

	private void OnDisable()
	{
		s_ActiveInstances--;
		if (!(current != this))
		{
			current = null;
			Apply(null);
		}
	}

	private static void Apply(PanelInputConfiguration input)
	{
		Settings settings = ((input != null) ? input.settings : Settings.Default);
		PanelInputRedirection panelInputRedirection = settings.panelInputRedirection;
		if (1 == 0)
		{
		}
		bool? overrideUseDefaultEventSystem = panelInputRedirection switch
		{
			PanelInputRedirection.Never => true, 
			PanelInputRedirection.Always => false, 
			_ => null, 
		};
		if (1 == 0)
		{
		}
		UIElementsRuntimeUtility.overrideUseDefaultEventSystem = overrideUseDefaultEventSystem;
		UIElementsRuntimeUtility.defaultEventSystem.worldSpaceLayers = settings.interactionLayers;
		UIElementsRuntimeUtility.defaultEventSystem.worldSpaceMaxDistance = settings.maxInteractionDistance;
		UIElementsRuntimeUtility.defaultEventSystem.raycaster = ((!settings.processWorldSpaceInput) ? new CameraScreenRaycaster
		{
			cameras = Array.Empty<Camera>()
		} : (settings.defaultEventCameraIsMainCamera ? new MainCameraScreenRaycaster() : new CameraScreenRaycaster
		{
			cameras = (Camera[])settings.eventCameras.Clone()
		}));
		onApply?.Invoke(input);
	}
}
