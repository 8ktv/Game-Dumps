namespace UnityEngine.AdaptivePerformance;

public class AdaptiveTransparency : AdaptivePerformanceScaler
{
	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveTransparency);
		}
	}

	protected override void OnDisabled()
	{
		OnDestroy();
	}

	private void OnDestroy()
	{
		AdaptivePerformanceRenderSettings.SkipTransparentObjects = false;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.SkipTransparentObjects = Scale < 1f;
		}
	}
}
