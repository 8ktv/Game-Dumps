namespace UnityEngine.AdaptivePerformance;

internal class AutoPerformanceLevelController
{
	private IDevicePerformanceControl m_PerfControl;

	private IPerformanceStatus m_PerfStats;

	private IThermalStatus m_ThermalStats;

	private float m_LastChangeTimeStamp = 0f;

	private float m_LastGpuLevelRaiseTimeStamp = 0f;

	private float m_LastCpuLevelRaiseTimeStamp = 0f;

	private float m_TargetFrameRateHitTimestamp = 0f;

	private float m_BottleneckUnknownTimestamp = 0f;

	private bool m_TriedToResolveUnknownBottleneck = false;

	private bool m_Enabled = false;

	private string m_FeatureName = "Auto Performance Control";

	public float TargetFrameTime { get; set; }

	public float AllowedCpuActiveTimeRatio { get; set; }

	public float AllowedGpuActiveTimeRatio { get; set; }

	public float GpuLevelBounceAvoidanceThreshold { get; set; }

	public float CpuLevelBounceAvoidanceThreshold { get; set; }

	public float UpdateInterval { get; set; }

	public float MinTargetFrameRateHitTime { get; set; }

	public float MaxTemperatureLevel { get; set; }

	public bool Enabled
	{
		get
		{
			return m_Enabled;
		}
		set
		{
			if (m_Enabled != value)
			{
				m_Enabled = value;
			}
		}
	}

	public AutoPerformanceLevelController(IDevicePerformanceControl perfControl, IPerformanceStatus perfStat, IThermalStatus thermalStat)
	{
		UpdateInterval = 5f;
		TargetFrameTime = -1f;
		AllowedCpuActiveTimeRatio = 0.8f;
		AllowedGpuActiveTimeRatio = 0.9f;
		GpuLevelBounceAvoidanceThreshold = 10f;
		CpuLevelBounceAvoidanceThreshold = 10f;
		MinTargetFrameRateHitTime = 10f;
		MaxTemperatureLevel = 0.9f;
		m_PerfStats = perfStat;
		m_PerfControl = perfControl;
		m_ThermalStats = thermalStat;
		perfStat.PerformanceBottleneckChangeEvent += delegate(PerformanceBottleneckChangeEventArgs ev)
		{
			OnBottleneckChange(ev);
		};
	}

	public void Update()
	{
		if (m_Enabled)
		{
			UpdateImpl(Time.time);
		}
	}

	public void Override(int requestedCpuLevel, int requestedGpuLevel)
	{
		m_LastChangeTimeStamp = Time.time;
		if (requestedCpuLevel > m_PerfControl.CpuLevel)
		{
			m_LastCpuLevelRaiseTimeStamp = m_LastChangeTimeStamp;
		}
		if (requestedGpuLevel > m_PerfControl.GpuLevel)
		{
			m_LastCpuLevelRaiseTimeStamp = m_LastChangeTimeStamp;
		}
		m_PerfControl.CpuLevel = requestedCpuLevel;
		m_PerfControl.GpuLevel = requestedGpuLevel;
	}

	private void UpdateImpl(float timestamp)
	{
		if (timestamp - m_LastChangeTimeStamp < UpdateInterval)
		{
			return;
		}
		switch (m_PerfStats.PerformanceMetrics.PerformanceBottleneck)
		{
		case PerformanceBottleneck.GPU:
			if (AllowRaiseGpuLevel())
			{
				RaiseGpuLevel(timestamp);
			}
			break;
		case PerformanceBottleneck.CPU:
			if (AllowRaiseCpuLevel())
			{
				RaiseCpuLevel(timestamp);
			}
			break;
		case PerformanceBottleneck.TargetFrameRate:
			if (timestamp - m_TargetFrameRateHitTimestamp > MinTargetFrameRateHitTime)
			{
				if (AllowLowerCpuLevel(timestamp))
				{
					LowerCpuLevel(timestamp);
				}
				if (AllowLowerGpuLevel(timestamp))
				{
					LowerGpuLevel(timestamp);
				}
			}
			break;
		case PerformanceBottleneck.Unknown:
			if (!m_TriedToResolveUnknownBottleneck && timestamp - m_BottleneckUnknownTimestamp > 10f)
			{
				if (AllowRaiseCpuLevel())
				{
					RaiseCpuLevel(timestamp);
					m_TriedToResolveUnknownBottleneck = true;
				}
				else if (AllowRaiseGpuLevel())
				{
					RaiseGpuLevel(timestamp);
					m_TriedToResolveUnknownBottleneck = true;
				}
			}
			break;
		}
	}

	private void OnBottleneckChange(PerformanceBottleneckChangeEventArgs ev)
	{
		if (ev.PerformanceBottleneck == PerformanceBottleneck.TargetFrameRate)
		{
			m_TargetFrameRateHitTimestamp = Time.time;
		}
		if (ev.PerformanceBottleneck == PerformanceBottleneck.Unknown)
		{
			m_BottleneckUnknownTimestamp = Time.time;
		}
		else
		{
			m_TriedToResolveUnknownBottleneck = false;
		}
	}

	private void RaiseGpuLevel(float timestamp)
	{
		IDevicePerformanceControl perfControl = m_PerfControl;
		int gpuLevel = perfControl.GpuLevel + 1;
		perfControl.GpuLevel = gpuLevel;
		m_LastChangeTimeStamp = timestamp;
		m_LastGpuLevelRaiseTimeStamp = timestamp;
		APLog.Debug("Auto Perf Level: raise GPU level to {0}", m_PerfControl.GpuLevel);
	}

	private void RaiseCpuLevel(float timestamp)
	{
		IDevicePerformanceControl perfControl = m_PerfControl;
		int cpuLevel = perfControl.CpuLevel + 1;
		perfControl.CpuLevel = cpuLevel;
		m_LastChangeTimeStamp = timestamp;
		m_LastCpuLevelRaiseTimeStamp = timestamp;
		APLog.Debug("Auto Perf Level: raise CPU level to {0}", m_PerfControl.CpuLevel);
	}

	private void LowerCpuLevel(float timestamp)
	{
		IDevicePerformanceControl perfControl = m_PerfControl;
		int cpuLevel = perfControl.CpuLevel - 1;
		perfControl.CpuLevel = cpuLevel;
		m_LastChangeTimeStamp = timestamp;
		APLog.Debug("Auto Perf Level: lower CPU level to {0}", m_PerfControl.CpuLevel);
	}

	private void LowerGpuLevel(float timestamp)
	{
		IDevicePerformanceControl perfControl = m_PerfControl;
		int gpuLevel = perfControl.GpuLevel - 1;
		perfControl.GpuLevel = gpuLevel;
		m_LastChangeTimeStamp = timestamp;
		APLog.Debug("Auto Perf Level: lower GPU level to {0}", m_PerfControl.GpuLevel);
	}

	private bool AllowLowerCpuLevel(float timestamp)
	{
		if (m_PerfControl.CpuLevel > 0 && timestamp - m_LastCpuLevelRaiseTimeStamp > CpuLevelBounceAvoidanceThreshold)
		{
			if (TargetFrameTime <= 0f)
			{
				return true;
			}
			FrameTiming frameTiming = m_PerfStats.FrameTiming;
			if (frameTiming.AverageCpuFrameTime <= 0f)
			{
				return true;
			}
			if (frameTiming.AverageCpuFrameTime < AllowedCpuActiveTimeRatio * TargetFrameTime)
			{
				return true;
			}
		}
		return false;
	}

	private bool AllowLowerGpuLevel(float timestamp)
	{
		if (m_PerfControl.GpuLevel > 0 && timestamp - m_LastGpuLevelRaiseTimeStamp > GpuLevelBounceAvoidanceThreshold)
		{
			if (TargetFrameTime <= 0f)
			{
				return true;
			}
			FrameTiming frameTiming = m_PerfStats.FrameTiming;
			if (frameTiming.AverageGpuFrameTime <= 0f)
			{
				return true;
			}
			if (frameTiming.AverageGpuFrameTime < AllowedGpuActiveTimeRatio * TargetFrameTime)
			{
				return true;
			}
		}
		return false;
	}

	private bool AllowRaiseLevels()
	{
		float temperatureLevel = m_ThermalStats.ThermalMetrics.TemperatureLevel;
		if (temperatureLevel < 0f)
		{
			return true;
		}
		if (temperatureLevel < MaxTemperatureLevel)
		{
			return true;
		}
		APLog.Debug("Auto Perf Level: cannot raise performance level, current temperature level ({0}) exceeds {1}", temperatureLevel, MaxTemperatureLevel);
		return false;
	}

	private bool AllowRaiseCpuLevel()
	{
		if (m_PerfControl.CpuLevel >= m_PerfControl.MaxCpuPerformanceLevel)
		{
			return false;
		}
		return AllowRaiseLevels();
	}

	private bool AllowRaiseGpuLevel()
	{
		if (m_PerfControl.GpuLevel >= m_PerfControl.MaxGpuPerformanceLevel)
		{
			return false;
		}
		return AllowRaiseLevels();
	}
}
