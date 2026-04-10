namespace UnityEngine.AdaptivePerformance;

public class AdaptiveShadowCascade : AdaptivePerformanceScaler
{
	private int m_DefaultCascadeCount;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveShadowCascade);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = m_DefaultCascadeCount;
	}

	protected override void OnEnabled()
	{
		m_DefaultCascadeCount = AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.MainLightShadowCascadesCountBias = (int)(2f * Scale);
		}
	}
}
