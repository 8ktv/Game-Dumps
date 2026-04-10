namespace UnityEngine.AdaptivePerformance;

public class AdaptiveBatching : AdaptivePerformanceScaler
{
	private bool m_DefaultState;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveBatching);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.SkipDynamicBatching = m_DefaultState;
	}

	protected override void OnEnabled()
	{
		m_DefaultState = AdaptivePerformanceRenderSettings.SkipDynamicBatching;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.SkipDynamicBatching = Scale < 1f;
		}
	}
}
