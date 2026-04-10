namespace UnityEngine.AdaptivePerformance;

public class AdaptiveSorting : AdaptivePerformanceScaler
{
	private bool m_DefaultSorting;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveSorting);
		}
	}

	protected override void OnDisabled()
	{
		AdaptivePerformanceRenderSettings.SkipFrontToBackSorting = m_DefaultSorting;
	}

	protected override void OnEnabled()
	{
		m_DefaultSorting = AdaptivePerformanceRenderSettings.SkipFrontToBackSorting;
	}

	protected override void OnLevel()
	{
		if (ScaleChanged())
		{
			AdaptivePerformanceRenderSettings.SkipFrontToBackSorting = Scale < 1f;
		}
	}
}
