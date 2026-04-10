namespace UnityEngine.AdaptivePerformance;

public class AdaptiveLut : AdaptivePerformanceScaler
{
	private float m_DefaultLutBias;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveLut);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.LutBias = m_DefaultLutBias;
	}

	protected override void OnEnabled()
	{
		m_DefaultLutBias = AdaptivePerformanceRenderSettings.LutBias;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.LutBias = Scale;
		}
	}
}
