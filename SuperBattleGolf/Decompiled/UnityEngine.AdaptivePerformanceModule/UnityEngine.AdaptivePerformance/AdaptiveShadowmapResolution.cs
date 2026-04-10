namespace UnityEngine.AdaptivePerformance;

public class AdaptiveShadowmapResolution : AdaptivePerformanceScaler
{
	private float m_DefaultShadowmapResolution;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveShadowmapResolution);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = m_DefaultShadowmapResolution;
	}

	protected override void OnEnabled()
	{
		m_DefaultShadowmapResolution = AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.MainLightShadowmapResolutionMultiplier = 1f * Scale;
		}
	}
}
