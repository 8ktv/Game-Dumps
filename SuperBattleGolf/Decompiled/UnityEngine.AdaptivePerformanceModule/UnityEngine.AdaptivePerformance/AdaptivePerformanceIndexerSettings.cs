using System;

namespace UnityEngine.AdaptivePerformance;

[Serializable]
public class AdaptivePerformanceIndexerSettings
{
	private const string m_FeatureName = "Indexer";

	[SerializeField]
	[Tooltip("Active")]
	private bool m_Active = true;

	[Tooltip("Thermal Action Delay")]
	[SerializeField]
	private float m_ThermalActionDelay = 10f;

	[Tooltip("Performance Action Delay")]
	[SerializeField]
	private float m_PerformanceActionDelay = 4f;

	public bool active
	{
		get
		{
			return m_Active;
		}
		set
		{
			if (m_Active != value)
			{
				m_Active = value;
			}
		}
	}

	public float thermalActionDelay
	{
		get
		{
			return m_ThermalActionDelay;
		}
		set
		{
			m_ThermalActionDelay = value;
		}
	}

	public float performanceActionDelay
	{
		get
		{
			return m_PerformanceActionDelay;
		}
		set
		{
			m_PerformanceActionDelay = value;
		}
	}
}
