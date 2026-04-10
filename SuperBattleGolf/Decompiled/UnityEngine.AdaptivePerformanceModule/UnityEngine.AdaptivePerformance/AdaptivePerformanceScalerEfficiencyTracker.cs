#define UNITY_ASSERTIONS
namespace UnityEngine.AdaptivePerformance;

internal class AdaptivePerformanceScalerEfficiencyTracker
{
	private AdaptivePerformanceScaler m_Scaler;

	private float m_LastAverageGpuFrameTime;

	private float m_LastAverageCpuFrameTime;

	private bool m_IsApplied;

	public bool IsRunning => m_Scaler != null;

	public void Start(AdaptivePerformanceScaler scaler, bool isApply)
	{
		Debug.Assert(!IsRunning, "AdaptivePerformanceScalerEfficiencyTracker is already running");
		m_Scaler = scaler;
		m_LastAverageGpuFrameTime = Holder.Instance.PerformanceStatus.FrameTiming.AverageGpuFrameTime;
		m_LastAverageCpuFrameTime = Holder.Instance.PerformanceStatus.FrameTiming.AverageCpuFrameTime;
		m_IsApplied = true;
	}

	public void Stop()
	{
		float num = Holder.Instance.PerformanceStatus.FrameTiming.AverageGpuFrameTime - m_LastAverageGpuFrameTime;
		float num2 = Holder.Instance.PerformanceStatus.FrameTiming.AverageCpuFrameTime - m_LastAverageCpuFrameTime;
		int num3 = (m_IsApplied ? 1 : (-1));
		m_Scaler.GpuImpact = num3 * (int)(num * 1000f);
		m_Scaler.CpuImpact = num3 * (int)(num2 * 1000f);
		m_Scaler = null;
	}
}
