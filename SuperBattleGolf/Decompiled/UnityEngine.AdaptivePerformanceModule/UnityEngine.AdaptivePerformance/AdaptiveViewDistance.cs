namespace UnityEngine.AdaptivePerformance;

public class AdaptiveViewDistance : AdaptivePerformanceScaler
{
	private float m_DefaultFarClipPlane = -1f;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveViewDistance);
		}
	}

	protected override void OnDisabled()
	{
		if ((bool)Camera.main && m_DefaultFarClipPlane != -1f)
		{
			Camera.main.farClipPlane = m_DefaultFarClipPlane;
		}
	}

	protected override void OnEnabled()
	{
		if ((bool)Camera.main)
		{
			m_DefaultFarClipPlane = Camera.main.farClipPlane;
		}
	}

	protected override void OnLevel()
	{
		if ((bool)Camera.main)
		{
			if (m_DefaultFarClipPlane == -1f)
			{
				m_DefaultFarClipPlane = Camera.main.farClipPlane;
			}
			if (ScaleChanged())
			{
				Camera.main.farClipPlane = Scale;
			}
		}
	}
}
