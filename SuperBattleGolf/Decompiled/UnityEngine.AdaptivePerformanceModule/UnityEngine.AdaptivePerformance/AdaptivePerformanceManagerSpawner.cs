using System;

namespace UnityEngine.AdaptivePerformance;

internal class AdaptivePerformanceManagerSpawner : ScriptableObject
{
	public const string AdaptivePerformanceManagerObjectName = "AdaptivePerformanceManager";

	private GameObject m_ManagerGameObject;

	public GameObject ManagerGameObject => m_ManagerGameObject;

	private void OnEnable()
	{
		if (!(m_ManagerGameObject != null))
		{
			m_ManagerGameObject = GameObject.Find("AdaptivePerformanceManager");
		}
	}

	public void Initialize(bool isCheckingProvider)
	{
		if (m_ManagerGameObject != null)
		{
			return;
		}
		m_ManagerGameObject = new GameObject("AdaptivePerformanceManager");
		AdaptivePerformanceManager adaptivePerformanceManager = m_ManagerGameObject.AddComponent<AdaptivePerformanceManager>();
		if (isCheckingProvider && adaptivePerformanceManager.Indexer == null)
		{
			Deinitialize();
			return;
		}
		Holder.Instance = adaptivePerformanceManager;
		Object.DontDestroyOnLoad(m_ManagerGameObject);
		IAdaptivePerformanceSettings settings = adaptivePerformanceManager.Settings;
		if (!(settings == null))
		{
			string[] availableScalerProfiles = settings.GetAvailableScalerProfiles();
			if (availableScalerProfiles.Length == 0)
			{
				APLog.Debug("No Scaler Profiles available. Did you remove all profiles manually from the provider Settings?");
				return;
			}
			settings.LoadScalerProfile(availableScalerProfiles[settings.defaultScalerProfilerIndex]);
			InstallScalers(settings.ScalerProfiles[settings.defaultScalerProfilerIndex], settings);
		}
	}

	public void Deinitialize()
	{
		if (!(m_ManagerGameObject == null))
		{
			Object.DestroyImmediate(m_ManagerGameObject);
			m_ManagerGameObject = null;
		}
	}

	private void InstallScalers(AdaptivePerformanceScalerProfile profile, IAdaptivePerformanceSettings settings)
	{
		foreach (Type k_DefaultScalerName in AdaptivePerformanceScalerSettings.k_DefaultScalerNames)
		{
			ScriptableObject.CreateInstance(k_DefaultScalerName);
		}
		if (profile.AddedScalers != null && profile.AddedScalers.Count > 0)
		{
			profile.EnableAddedScalers();
		}
		else if (settings.AddedScalerViaScan != null && settings.AddedScalerViaScan.Count > 0)
		{
			for (int i = 0; i < settings.AddedScalerViaScan.Count; i++)
			{
				settings.AddedScalerViaScan[i].InitializeScaler();
			}
		}
	}
}
