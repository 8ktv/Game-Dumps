using System;

namespace UnityEngine.AdaptivePerformance.Provider;

[Flags]
public enum Feature
{
	None = 0,
	WarningLevel = 1,
	TemperatureLevel = 2,
	TemperatureTrend = 4,
	CpuPerformanceLevel = 8,
	GpuPerformanceLevel = 0x10,
	PerformanceLevelControl = 0x20,
	GpuFrameTime = 0x40,
	CpuFrameTime = 0x80,
	OverallFrameTime = 0x100,
	CpuPerformanceBoost = 0x200,
	GpuPerformanceBoost = 0x400,
	ClusterInfo = 0x800,
	PerformanceMode = 0x1000
}
