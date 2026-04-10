namespace UnityEngine.AdaptivePerformance;

public interface IPerformanceStatus
{
	PerformanceMetrics PerformanceMetrics { get; }

	FrameTiming FrameTiming { get; }

	PerformanceMode PerformanceMode { get; }

	event PerformanceBottleneckChangeHandler PerformanceBottleneckChangeEvent;

	event PerformanceLevelChangeHandler PerformanceLevelChangeEvent;

	event PerformanceBoostChangeHandler PerformanceBoostChangeEvent;
}
