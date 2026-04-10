using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Profiling;
using UnityEngine.Profiling;

namespace UnityEngine.AdaptivePerformance;

public static class AdaptivePerformanceProfilerStats
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct CustomProfilerMarker<T> where T : unmanaged
	{
		public CustomProfilerMarker(string name, ProfilerMarkerDataUnit dataUnit)
		{
		}

		public void Sample(T value)
		{
		}

		private static byte GetProfilerMarkerDataType()
		{
			return Type.GetTypeCode(typeof(T)) switch
			{
				TypeCode.Int32 => 2, 
				TypeCode.UInt32 => 3, 
				TypeCode.Int64 => 4, 
				TypeCode.UInt64 => 5, 
				TypeCode.Single => 6, 
				TypeCode.Double => 7, 
				TypeCode.String => 9, 
				_ => throw new ArgumentException($"Type {typeof(T)} is unsupported by ProfilerCounter."), 
			};
		}
	}

	public struct ScalerInfo
	{
		public unsafe fixed byte scalerName[320];

		public uint enabled;

		public int overrideLevel;

		public int currentLevel;

		public int maxLevel;

		public float scale;

		public uint applied;
	}

	public static readonly ProfilerCategory AdaptivePerformanceProfilerCategory = ProfilerCategory.Scripts;

	public static CustomProfilerMarker<float> CurrentCPUMarker = new CustomProfilerMarker<float>("CPU frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

	public static CustomProfilerMarker<float> AvgCPUMarker = new CustomProfilerMarker<float>("CPU avg frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

	public static CustomProfilerMarker<float> CurrentGPUMarker = new CustomProfilerMarker<float>("GPU frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

	public static CustomProfilerMarker<float> AvgGPUMarker = new CustomProfilerMarker<float>("GPU avg frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

	public static CustomProfilerMarker<int> CurrentCPULevelMarker = new CustomProfilerMarker<int>("CPU performance level", ProfilerMarkerDataUnit.Count);

	public static CustomProfilerMarker<int> CurrentGPULevelMarker = new CustomProfilerMarker<int>("GPU performance level", ProfilerMarkerDataUnit.Count);

	public static CustomProfilerMarker<float> CurrentFrametimeMarker = new CustomProfilerMarker<float>("Frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

	public static CustomProfilerMarker<float> AvgFrametimeMarker = new CustomProfilerMarker<float>("Avg frametime", ProfilerMarkerDataUnit.TimeNanoseconds);

	public static CustomProfilerMarker<int> WarningLevelMarker = new CustomProfilerMarker<int>("Thermal Warning Level", ProfilerMarkerDataUnit.Count);

	public static CustomProfilerMarker<float> TemperatureLevelMarker = new CustomProfilerMarker<float>("Temperature Level", ProfilerMarkerDataUnit.Count);

	public static CustomProfilerMarker<float> TemperatureTrendMarker = new CustomProfilerMarker<float>("Temperature Trend", ProfilerMarkerDataUnit.Count);

	public static CustomProfilerMarker<int> BottleneckMarker = new CustomProfilerMarker<int>("Bottleneck", ProfilerMarkerDataUnit.Count);

	public static CustomProfilerMarker<int> PerformanceModeMarker = new CustomProfilerMarker<int>("Performance Mode", ProfilerMarkerDataUnit.Count);

	public static readonly Guid kAdaptivePerformanceProfilerModuleGuid = new Guid("42c5aeb7-fb77-4172-a384-34063f1bd332");

	public static readonly int kScalerDataTag = 0;

	private static Dictionary<string, ScalerInfo> scalerInfos = new Dictionary<string, ScalerInfo>();

	[Conditional("ENABLE_PROFILER")]
	public unsafe static void EmitScalerDataToProfilerStream(string scalerName, bool enabled, int overrideLevel, int currentLevel, float scale, bool applied, int maxLevel)
	{
		if (Profiler.enabled && scalerName.Length != 0)
		{
			ScalerInfo value;
			bool flag = scalerInfos.TryGetValue(scalerName, out value);
			if (!flag)
			{
				value = default(ScalerInfo);
			}
			value.enabled = (enabled ? 1u : 0u);
			value.overrideLevel = overrideLevel;
			value.currentLevel = currentLevel;
			value.scale = scale;
			value.maxLevel = maxLevel;
			value.applied = (applied ? 1u : 0u);
			Encoding.ASCII.GetBytes(MemoryExtensions.AsSpan(scalerName), new Span<byte>(value.scalerName, 320));
			if (!flag)
			{
				scalerInfos.Add(scalerName, value);
			}
			else
			{
				scalerInfos[scalerName] = value;
			}
		}
	}

	public static void FlushScalerDataToProfilerStream()
	{
		ScalerInfo[] array = new ScalerInfo[scalerInfos.Count];
		scalerInfos.Values.CopyTo(array, 0);
	}
}
