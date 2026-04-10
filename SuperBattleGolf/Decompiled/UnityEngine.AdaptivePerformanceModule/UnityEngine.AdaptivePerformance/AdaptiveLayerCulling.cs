namespace UnityEngine.AdaptivePerformance;

public class AdaptiveLayerCulling : AdaptivePerformanceScaler
{
	private float[] m_defaultDistances = new float[32];

	private float[] m_scaledDistances = new float[32];

	private bool init = false;

	private Camera m_cachedCamera;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveLayerCulling);
		}
	}

	protected override void OnDisabled()
	{
		init = false;
		if ((bool)Camera.main && m_defaultDistances != null)
		{
			Camera.main.layerCullDistances = m_defaultDistances;
		}
	}

	protected override void OnEnabled()
	{
		AsignDefaultValues();
	}

	protected override void OnLevel()
	{
		if (!Camera.main)
		{
			return;
		}
		AsignDefaultValues();
		if (!ScaleChanged())
		{
			return;
		}
		for (int num = 31; num >= 0; num--)
		{
			if (m_defaultDistances[num] != 0f)
			{
				m_scaledDistances[num] = m_defaultDistances[num] * Scale;
			}
		}
		Camera.main.layerCullDistances = m_scaledDistances;
	}

	private void AsignDefaultValues()
	{
		if (m_cachedCamera == null || m_cachedCamera != Camera.main)
		{
			m_cachedCamera = Camera.main;
			init = false;
		}
		if (!init && (bool)Camera.main)
		{
			m_defaultDistances = Camera.main.layerCullDistances;
			init = true;
		}
	}
}
