namespace UnityEngine.AdaptivePerformance;

public class AdaptivePhysics : AdaptivePerformanceScaler
{
	private float m_fixedDeltaTimeDefault;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptivePhysics);
		}
	}

	protected override void OnDisabled()
	{
		Time.fixedDeltaTime = m_fixedDeltaTimeDefault;
	}

	protected override void OnEnabled()
	{
		m_fixedDeltaTimeDefault = Time.fixedDeltaTime;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			Time.fixedDeltaTime = m_fixedDeltaTimeDefault / Scale;
		}
	}
}
