namespace UnityEngine.AdaptivePerformance;

public class AdaptiveShadowQuality : AdaptivePerformanceScaler
{
	private int m_DefaultShadowQualityBias;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveShadowQuality);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.ShadowQualityBias = m_DefaultShadowQualityBias;
	}

	protected override void OnEnabled()
	{
		m_DefaultShadowQualityBias = AdaptivePerformanceRenderSettings.ShadowQualityBias;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.ShadowQualityBias = (int)(3f - 3f * Scale);
		}
	}
}
