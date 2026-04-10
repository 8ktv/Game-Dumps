namespace UnityEngine.AdaptivePerformance;

public class AdaptiveShadowDistance : AdaptivePerformanceScaler
{
	private float m_DefaultShadowDistance;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveShadowDistance);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = m_DefaultShadowDistance;
	}

	protected override void OnEnabled()
	{
		m_DefaultShadowDistance = AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.MaxShadowDistanceMultiplier = 1f * Scale;
		}
	}
}
