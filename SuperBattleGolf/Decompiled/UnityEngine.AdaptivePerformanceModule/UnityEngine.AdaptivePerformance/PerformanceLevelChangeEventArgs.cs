namespace UnityEngine.AdaptivePerformance;

public struct PerformanceLevelChangeEventArgs
{
	public int CpuLevel { get; set; }

	public int CpuLevelDelta { get; set; }

	public int GpuLevel { get; set; }

	public int GpuLevelDelta { get; set; }

	public PerformanceControlMode PerformanceControlMode { get; set; }

	public bool ManualOverride { get; set; }
}
