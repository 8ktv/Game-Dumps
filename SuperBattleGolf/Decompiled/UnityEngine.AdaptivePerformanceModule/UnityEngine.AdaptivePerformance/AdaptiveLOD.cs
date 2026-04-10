namespace UnityEngine.AdaptivePerformance;

public class AdaptiveLOD : AdaptivePerformanceScaler
{
	private float m_DefaultLodBias;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveLOD);
		}
	}

	protected override void OnDisabled()
	{
		QualitySettings.lodBias = m_DefaultLodBias;
	}

	protected override void OnEnabled()
	{
		m_DefaultLodBias = QualitySettings.lodBias;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			QualitySettings.lodBias = m_DefaultLodBias * Scale;
		}
	}
}
