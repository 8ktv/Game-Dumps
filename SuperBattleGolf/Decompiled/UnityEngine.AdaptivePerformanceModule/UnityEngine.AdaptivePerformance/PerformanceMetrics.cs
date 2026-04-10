namespace UnityEngine.AdaptivePerformance;

public struct PerformanceMetrics
{
	public int CurrentCpuLevel { get; set; }

	public int CurrentGpuLevel { get; set; }

	public PerformanceBottleneck PerformanceBottleneck { get; set; }

	public bool CpuPerformanceBoost { get; set; }

	public bool GpuPerformanceBoost { get; set; }

	public ClusterInfo ClusterInfo { get; set; }
}
