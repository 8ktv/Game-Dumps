using UnityEngine.Scripting;

namespace UnityEngine.AdaptivePerformance;

[RequireDerived]
public abstract class AdaptivePerformanceScaler : ScriptableObject
{
	private AdaptivePerformanceIndexer m_Indexer;

	private int m_OverrideLevel = -1;

	[SerializeField]
	private AdaptivePerformanceScalerSettingsBase m_defaultSetting = new AdaptivePerformanceScalerSettingsBase();

	protected IAdaptivePerformanceSettings m_Settings;

	public virtual string Name
	{
		get
		{
			return m_defaultSetting.name;
		}
		set
		{
			if (!(m_defaultSetting.name == value))
			{
				m_defaultSetting.name = value;
			}
		}
	}

	public virtual bool Enabled
	{
		get
		{
			return m_defaultSetting.enabled;
		}
		set
		{
			if (m_defaultSetting.enabled != value)
			{
				m_defaultSetting.enabled = value;
			}
		}
	}

	public virtual float Scale
	{
		get
		{
			return m_defaultSetting.scale;
		}
		set
		{
			if (m_defaultSetting.scale != value)
			{
				m_defaultSetting.scale = value;
			}
		}
	}

	public virtual ScalerVisualImpact VisualImpact
	{
		get
		{
			return m_defaultSetting.visualImpact;
		}
		set
		{
			if (m_defaultSetting.visualImpact != value)
			{
				m_defaultSetting.visualImpact = value;
			}
		}
	}

	public virtual ScalerTarget Target
	{
		get
		{
			return m_defaultSetting.target;
		}
		set
		{
			if (m_defaultSetting.target != value)
			{
				m_defaultSetting.target = value;
			}
		}
	}

	public virtual int MaxLevel
	{
		get
		{
			return m_defaultSetting.maxLevel;
		}
		set
		{
			if (m_defaultSetting.maxLevel != value)
			{
				m_defaultSetting.maxLevel = value;
			}
		}
	}

	public virtual float MinBound
	{
		get
		{
			return m_defaultSetting.minBound;
		}
		set
		{
			if (m_defaultSetting.minBound != value)
			{
				m_defaultSetting.minBound = value;
			}
		}
	}

	public virtual float MaxBound
	{
		get
		{
			return m_defaultSetting.maxBound;
		}
		set
		{
			if (m_defaultSetting.maxBound != value)
			{
				m_defaultSetting.maxBound = value;
			}
		}
	}

	public int CurrentLevel { get; private set; }

	public bool IsMaxLevel => CurrentLevel == MaxLevel;

	public bool NotLeveled => CurrentLevel == 0;

	public int GpuImpact { get; internal set; }

	public int CpuImpact { get; internal set; }

	public AdaptivePerformanceScalerSettingsBase DefaultSetting
	{
		get
		{
			return m_defaultSetting;
		}
		set
		{
			m_defaultSetting = value;
		}
	}

	public int OverrideLevel
	{
		get
		{
			return m_OverrideLevel;
		}
		set
		{
			m_OverrideLevel = value;
			m_Indexer.UpdateOverrideLevel(this);
		}
	}

	public int CalculateCost()
	{
		PerformanceBottleneck performanceBottleneck = Holder.Instance.PerformanceStatus.PerformanceMetrics.PerformanceBottleneck;
		int num = 0;
		switch (VisualImpact)
		{
		case ScalerVisualImpact.Low:
			num += CurrentLevel;
			break;
		case ScalerVisualImpact.Medium:
			num += CurrentLevel * 2;
			break;
		case ScalerVisualImpact.High:
			num += CurrentLevel * 3;
			break;
		}
		if (performanceBottleneck == PerformanceBottleneck.CPU && (Target & ScalerTarget.CPU) == 0)
		{
			num = 6;
		}
		if (performanceBottleneck == PerformanceBottleneck.GPU && (Target & ScalerTarget.GPU) == 0)
		{
			num = 6;
		}
		if (performanceBottleneck == PerformanceBottleneck.TargetFrameRate && (Target & ScalerTarget.FillRate) == 0)
		{
			num = 6;
		}
		return num;
	}

	protected virtual void Awake()
	{
		if (Holder.Instance != null)
		{
			m_Settings = Holder.Instance.Settings;
			m_Indexer = Holder.Instance.Indexer;
		}
	}

	internal void InitializeScaler()
	{
		if (Holder.Instance != null)
		{
			m_Settings = Holder.Instance.Settings;
			m_Indexer = Holder.Instance.Indexer;
			EnableScaler();
		}
	}

	private void OnEnable()
	{
		EnableScaler();
	}

	internal void EnableScaler()
	{
		if (m_Indexer != null)
		{
			m_Indexer.AddScaler(this);
			OnEnabled();
		}
	}

	internal void RemoveScaler()
	{
		if (m_Indexer != null && m_Indexer.RemoveScaler(this))
		{
			OnDisabled();
		}
	}

	private void OnDisable()
	{
		RemoveScaler();
	}

	internal void IncreaseLevel()
	{
		if (IsMaxLevel)
		{
			Debug.LogError("Cannot increase scaler level as it is already max.");
			return;
		}
		CurrentLevel++;
		OnLevelIncrease();
		OnLevel();
	}

	internal void DecreaseLevel()
	{
		if (NotLeveled)
		{
			Debug.LogError("Cannot decrease scaler level as it is already 0.");
			return;
		}
		CurrentLevel--;
		OnLevelDecrease();
		OnLevel();
	}

	internal void Activate()
	{
		OnEnabled();
	}

	internal void Deactivate()
	{
		OnDisabled();
	}

	public void ApplyDefaultSetting(AdaptivePerformanceScalerSettingsBase defaultSetting)
	{
		m_defaultSetting = defaultSetting;
	}

	protected bool ScaleChanged()
	{
		float scale = Scale;
		float num = (MaxBound - MinBound) / (float)MaxLevel;
		Scale = num * (float)(MaxLevel - CurrentLevel) + MinBound;
		return Scale != scale;
	}

	protected virtual void OnLevelIncrease()
	{
	}

	protected virtual void OnLevelDecrease()
	{
	}

	protected virtual void OnLevel()
	{
	}

	protected virtual void OnEnabled()
	{
	}

	protected virtual void OnDisabled()
	{
	}
}
