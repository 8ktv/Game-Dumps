using UnityEngine.AdaptivePerformance.Provider;

namespace UnityEngine.AdaptivePerformance;

internal class DevicePerformanceControlImpl : IDevicePerformanceControl
{
	private IDevicePerformanceLevelControl m_PerformanceLevelControl;

	public bool AutomaticPerformanceControl
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public PerformanceControlMode PerformanceControlMode { get; set; }

	public int MaxCpuPerformanceLevel => (m_PerformanceLevelControl != null) ? m_PerformanceLevelControl.MaxCpuPerformanceLevel : (-1);

	public int MaxGpuPerformanceLevel => (m_PerformanceLevelControl != null) ? m_PerformanceLevelControl.MaxGpuPerformanceLevel : (-1);

	public int CpuLevel { get; set; }

	public int GpuLevel { get; set; }

	public int CurrentCpuLevel { get; set; }

	public int CurrentGpuLevel { get; set; }

	public bool CpuPerformanceBoost { get; set; }

	public bool GpuPerformanceBoost { get; set; }

	public DevicePerformanceControlImpl(IDevicePerformanceLevelControl performanceLevelControl)
	{
		m_PerformanceLevelControl = performanceLevelControl;
		PerformanceControlMode = PerformanceControlMode.Automatic;
		CurrentCpuLevel = -1;
		CurrentGpuLevel = -1;
		CpuLevel = -1;
		GpuLevel = -1;
	}

	public bool Update(out PerformanceLevelChangeEventArgs changeArgs)
	{
		changeArgs = default(PerformanceLevelChangeEventArgs);
		changeArgs.PerformanceControlMode = PerformanceControlMode;
		if (PerformanceControlMode == PerformanceControlMode.System)
		{
			bool flag = CurrentCpuLevel != -1 || CurrentGpuLevel != -1;
			CurrentCpuLevel = -1;
			CurrentGpuLevel = -1;
			if (flag)
			{
				changeArgs.CpuLevel = CurrentCpuLevel;
				changeArgs.GpuLevel = CurrentGpuLevel;
				changeArgs.CpuLevelDelta = 0;
				changeArgs.GpuLevelDelta = 0;
			}
			return flag;
		}
		if ((CpuLevel != -1 || GpuLevel != -1) && (CpuLevel != CurrentCpuLevel || GpuLevel != CurrentGpuLevel))
		{
			int cpu = CpuLevel;
			int gpu = GpuLevel;
			if (m_PerformanceLevelControl.SetPerformanceLevel(ref cpu, ref gpu))
			{
				changeArgs.CpuLevelDelta = ComputeDelta(CurrentCpuLevel, cpu);
				changeArgs.GpuLevelDelta = ComputeDelta(CurrentGpuLevel, gpu);
				if (cpu != CpuLevel || gpu != GpuLevel)
				{
					Debug.Log($"Requested CPU level {CpuLevel} and GPU level {GpuLevel} was overriden by System with CPU level {cpu} and GPU level {gpu}");
				}
				CurrentCpuLevel = CpuLevel;
				CurrentGpuLevel = GpuLevel;
				changeArgs.CpuLevel = CurrentCpuLevel;
				changeArgs.GpuLevel = CurrentGpuLevel;
				return true;
			}
			changeArgs.CpuLevelDelta = 0;
			changeArgs.GpuLevelDelta = 0;
			CurrentCpuLevel = -1;
			CurrentGpuLevel = -1;
			CpuLevel = -1;
			GpuLevel = -1;
			return false;
		}
		return false;
	}

	private int ComputeDelta(int oldLevel, int newLevel)
	{
		if (oldLevel < 0 || newLevel < 0)
		{
			return 0;
		}
		return newLevel - oldLevel;
	}
}
