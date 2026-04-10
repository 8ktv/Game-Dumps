using System;

namespace UnityEngine.AdaptivePerformance.Basic;

[Serializable]
[AdaptivePerformanceConfigurationData("Basic", "com.unity.adaptivePerformance.basic.provider_settings")]
public class BasicProviderSettings : IAdaptivePerformanceSettings
{
	private static BasicProviderSettings m_Instance;

	private void Awake()
	{
		m_Instance = this;
	}

	internal static BasicProviderSettings GetSettings()
	{
		return m_Instance;
	}
}
