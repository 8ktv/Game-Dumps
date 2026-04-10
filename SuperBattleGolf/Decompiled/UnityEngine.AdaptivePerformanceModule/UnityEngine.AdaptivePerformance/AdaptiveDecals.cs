namespace UnityEngine.AdaptivePerformance;

public class AdaptiveDecals : AdaptivePerformanceScaler
{
	private float m_DefaultDecalsDistance;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveDecals);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.DecalsDrawDistance = m_DefaultDecalsDistance;
	}

	protected override void OnEnabled()
	{
		m_DefaultDecalsDistance = AdaptivePerformanceRenderSettings.DecalsDrawDistance;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.DecalsDrawDistance = (int)(m_DefaultDecalsDistance * Scale);
		}
	}
}
