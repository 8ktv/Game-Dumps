using System;

namespace UnityEngine.AdaptivePerformance;

[Serializable]
public class AdaptivePerformanceScalerSettingsBase
{
	[SerializeField]
	[Tooltip("Name of the scaler.")]
	private string m_Name = "Base Scaler";

	[SerializeField]
	[Tooltip("Active")]
	private bool m_Enabled = false;

	[SerializeField]
	[Tooltip("Scale to control the quality impact for the scaler. No quality change when 1, improved quality when >1, and lowered quality when <1.")]
	private float m_Scale = -1f;

	[SerializeField]
	[Tooltip("Visual impact the scaler has on the application. The higher the value, the more impact the scaler has on the visuals.")]
	private ScalerVisualImpact m_VisualImpact = ScalerVisualImpact.Low;

	[SerializeField]
	[Tooltip("Application bottleneck that the scaler targets. The target selected has the most impact on the quality control of this scaler.")]
	private ScalerTarget m_Target = ScalerTarget.CPU;

	[Tooltip("Maximum level for the scaler. This is tied to the implementation of the scaler to divide the levels into concrete steps.")]
	[SerializeField]
	private int m_MaxLevel = 1;

	[SerializeField]
	[Tooltip("Minimum value for the scale boundary.")]
	private float m_MinBound = -1f;

	[SerializeField]
	[Tooltip("Maximum value for the scale boundary.")]
	private float m_MaxBound = -1f;

	public string name
	{
		get
		{
			return m_Name;
		}
		set
		{
			m_Name = value;
		}
	}

	public bool enabled
	{
		get
		{
			return m_Enabled;
		}
		set
		{
			m_Enabled = value;
		}
	}

	public float scale
	{
		get
		{
			return m_Scale;
		}
		set
		{
			m_Scale = value;
		}
	}

	public ScalerVisualImpact visualImpact
	{
		get
		{
			return m_VisualImpact;
		}
		set
		{
			m_VisualImpact = value;
		}
	}

	public ScalerTarget target
	{
		get
		{
			return m_Target;
		}
		set
		{
			m_Target = value;
		}
	}

	public int maxLevel
	{
		get
		{
			return m_MaxLevel;
		}
		set
		{
			m_MaxLevel = value;
		}
	}

	public float minBound
	{
		get
		{
			return m_MinBound;
		}
		set
		{
			m_MinBound = value;
		}
	}

	public float maxBound
	{
		get
		{
			return m_MaxBound;
		}
		set
		{
			m_MaxBound = value;
		}
	}
}
