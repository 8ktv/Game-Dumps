using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.AdaptivePerformance;

public class AdaptivePerformanceGeneralSettings : ScriptableObject
{
	public static string k_SettingsKey = "com.unity.adaptiveperformance.loader_settings";

	internal static AdaptivePerformanceGeneralSettings s_RuntimeSettingsInstance = null;

	[SerializeField]
	internal AdaptivePerformanceManagerSettings m_LoaderManagerInstance = null;

	[SerializeField]
	[Tooltip("Enable this to automatically start up Adaptive Performance at runtime.")]
	internal bool m_InitManagerOnStart = true;

	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEditor.AdaptivePerformanceModule" })]
	internal string m_LastSelectedProvider = "";

	private AdaptivePerformanceManagerSettings m_AdaptivePerformanceManager = null;

	private bool m_ProviderIntialized = false;

	private bool m_ProviderStarted = false;

	public AdaptivePerformanceManagerSettings Manager
	{
		get
		{
			return m_LoaderManagerInstance;
		}
		set
		{
			m_LoaderManagerInstance = value;
		}
	}

	public bool IsProviderInitialized => m_ProviderIntialized;

	public bool IsProviderStarted => m_ProviderStarted;

	public static AdaptivePerformanceGeneralSettings Instance
	{
		get
		{
			return s_RuntimeSettingsInstance;
		}
		set
		{
			s_RuntimeSettingsInstance = value;
		}
	}

	public AdaptivePerformanceManagerSettings AssignedSettings
	{
		get
		{
			return m_LoaderManagerInstance;
		}
		set
		{
			m_LoaderManagerInstance = value;
		}
	}

	public bool InitManagerOnStart
	{
		get
		{
			return m_InitManagerOnStart;
		}
		set
		{
			m_InitManagerOnStart = value;
		}
	}

	private void Awake()
	{
		s_RuntimeSettingsInstance = this;
		Application.quitting += Quit;
		Object.DontDestroyOnLoad(s_RuntimeSettingsInstance);
	}

	private static void Quit()
	{
		AdaptivePerformanceGeneralSettings instance = Instance;
		if (!(instance == null))
		{
			instance.DeInitAdaptivePerformance();
		}
	}

	private void OnDestroy()
	{
		DeInitAdaptivePerformance();
		s_RuntimeSettingsInstance = null;
	}

	[RequiredByNativeCode(true)]
	internal static void AttemptInitializeAdaptivePerformanceGeneralSettingsOnLoad()
	{
		AdaptivePerformanceGeneralSettings instance = Instance;
		if (!(instance == null) && instance.InitManagerOnStart)
		{
			instance.InitAdaptivePerformance();
		}
	}

	[RequiredByNativeCode(true)]
	internal static void AttemptStartAdaptivePerformanceGeneralSettingsOnBeforeSplashScreen()
	{
		AdaptivePerformanceGeneralSettings instance = Instance;
		if (!(instance == null) && instance.InitManagerOnStart)
		{
			instance.StartAdaptivePerformance();
		}
	}

	internal void InitAdaptivePerformance()
	{
		if (m_ProviderIntialized || Instance == null)
		{
			return;
		}
		m_AdaptivePerformanceManager = Instance.m_LoaderManagerInstance;
		if (m_AdaptivePerformanceManager == null)
		{
			Debug.LogError("Assigned GameObject for Adaptive Performance Management loading is invalid. No Adaptive Performance Providers will be automatically loaded.");
			return;
		}
		m_AdaptivePerformanceManager.automaticLoading = false;
		m_AdaptivePerformanceManager.automaticRunning = false;
		m_AdaptivePerformanceManager.InitializeLoaderSync();
		if (!(m_AdaptivePerformanceManager.activeLoader == null))
		{
			m_ProviderIntialized = true;
		}
	}

	internal void StartAdaptivePerformance()
	{
		if (m_ProviderIntialized && !m_ProviderStarted && !(m_AdaptivePerformanceManager == null) && !(m_AdaptivePerformanceManager.activeLoader == null))
		{
			m_AdaptivePerformanceManager.StartSubsystems();
			m_ProviderStarted = true;
		}
	}

	internal void StopAdaptivePerformance()
	{
		if (m_ProviderIntialized && m_ProviderStarted && !(m_AdaptivePerformanceManager == null) && !(m_AdaptivePerformanceManager.activeLoader == null))
		{
			m_AdaptivePerformanceManager.StopSubsystems();
			m_ProviderStarted = false;
		}
	}

	internal void DeInitAdaptivePerformance()
	{
		if (m_ProviderIntialized)
		{
			if (m_ProviderStarted)
			{
				StopAdaptivePerformance();
			}
			if (m_AdaptivePerformanceManager != null)
			{
				m_AdaptivePerformanceManager.DeinitializeLoader();
				m_AdaptivePerformanceManager = null;
			}
			m_ProviderIntialized = false;
		}
	}
}
