namespace UnityEngine.AdaptivePerformance;

public class AdaptiveMSAA : AdaptivePerformanceScaler
{
	private int m_DefaultAA;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveMSAA);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.AntiAliasingQualityBias = m_DefaultAA;
	}

	protected override void OnEnabled()
	{
		m_DefaultAA = AdaptivePerformanceRenderSettings.AntiAliasingQualityBias;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.AntiAliasingQualityBias = (int)(2f * Scale);
		}
	}
}
