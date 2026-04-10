namespace UnityEngine.AdaptivePerformance;

public struct FrameTiming
{
	public float CurrentFrameTime { get; set; }

	public float AverageFrameTime { get; set; }

	public float CurrentGpuFrameTime { get; set; }

	public float AverageGpuFrameTime { get; set; }

	public float CurrentCpuFrameTime { get; set; }

	public float AverageCpuFrameTime { get; set; }
}
