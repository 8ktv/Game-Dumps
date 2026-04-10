using System;
using System.Diagnostics;
using Unity.Profiling;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Profiling;

[NativeHeader("Runtime/Profiler/ScriptBindings/Sampler.bindings.h")]
[NativeHeader("Runtime/Profiler/Marker.h")]
[UsedByNativeCode]
public sealed class CustomSampler : Sampler
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(CustomSampler customSampler)
		{
			return customSampler.m_Ptr;
		}
	}

	internal static CustomSampler s_InvalidCustomSampler = new CustomSampler();

	internal CustomSampler()
	{
	}

	private CustomSampler(IntPtr ptr)
		: base(ptr)
	{
	}

	public static CustomSampler Create(string name, bool collectGpuData = false)
	{
		IntPtr intPtr = ProfilerUnsafeUtility.CreateMarker(name, 1, (MarkerFlags)(8 | (collectGpuData ? 256 : 0)), 0);
		if (intPtr == IntPtr.Zero)
		{
			return s_InvalidCustomSampler;
		}
		return new CustomSampler(intPtr);
	}

	[Conditional("ENABLE_PROFILER")]
	[IgnoredByDeepProfiler]
	public void Begin()
	{
		ProfilerUnsafeUtility.BeginSample(m_Ptr);
	}

	[Conditional("ENABLE_PROFILER")]
	[IgnoredByDeepProfiler]
	public void Begin(Object targetObject)
	{
		ProfilerUnsafeUtility.Internal_BeginWithObject(m_Ptr, targetObject);
	}

	[Conditional("ENABLE_PROFILER")]
	[IgnoredByDeepProfiler]
	public void End()
	{
		ProfilerUnsafeUtility.EndSample(m_Ptr);
	}
}
