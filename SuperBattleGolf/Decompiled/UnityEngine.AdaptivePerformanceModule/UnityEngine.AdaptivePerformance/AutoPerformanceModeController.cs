namespace UnityEngine.AdaptivePerformance;

internal class AutoPerformanceModeController
{
	private string m_FeatureName = "Auto Performance Mode Control";

	public AutoPerformanceModeController(IPerformanceModeStatus perfModeStat)
	{
		perfModeStat.PerformanceModeEvent += delegate(PerformanceMode mode)
		{
			OnPerformanceModeChange(mode);
		};
	}

	private void OnPerformanceModeChange(PerformanceMode performanceMode)
	{
		switch (performanceMode)
		{
		case PerformanceMode.Battery:
			Application.targetFrameRate = 30;
			break;
		case PerformanceMode.Optimize:
			Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
			break;
		default:
			Application.targetFrameRate = -1;
			break;
		}
		APLog.Debug($"[AutoPerformanceModeController] Performance Mode: {performanceMode}, fps: {Application.targetFrameRate}");
	}
}
