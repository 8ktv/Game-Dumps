using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling.LowLevel;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Scripting;

namespace Unity.Profiling;

[UsedByNativeCode]
[IgnoredByDeepProfiler]
public struct ProfilerMarker
{
	[UsedByNativeCode]
	[IgnoredByDeepProfiler]
	public struct AutoScope : IDisposable
	{
		[NativeDisableUnsafePtrRestriction]
		internal readonly IntPtr m_Ptr;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal AutoScope(IntPtr markerPtr)
		{
			m_Ptr = markerPtr;
			if (m_Ptr != IntPtr.Zero)
			{
				ProfilerUnsafeUtility.BeginSample(markerPtr);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe AutoScope(IntPtr markerPtr, string metadata)
		{
			m_Ptr = markerPtr;
			if (!(m_Ptr != IntPtr.Zero))
			{
				return;
			}
			if (string.IsNullOrEmpty(metadata))
			{
				ProfilerUnsafeUtility.BeginSample(markerPtr);
				return;
			}
			ProfilerMarkerData profilerMarkerData = new ProfilerMarkerData
			{
				Type = 9
			};
			fixed (char* ptr = metadata)
			{
				profilerMarkerData.Size = (uint)((metadata.Length + 1) * 2);
				profilerMarkerData.Ptr = ptr;
				ProfilerUnsafeUtility.BeginSampleWithMetadata(markerPtr, 1, &profilerMarkerData);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			if (m_Ptr != IntPtr.Zero)
			{
				ProfilerUnsafeUtility.EndSample(m_Ptr);
			}
		}
	}

	[NonSerialized]
	[NativeDisableUnsafePtrRestriction]
	internal readonly IntPtr m_Ptr;

	public IntPtr Handle => m_Ptr;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ProfilerMarker(string name)
	{
		m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, 1, MarkerFlags.Default, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ProfilerMarker(char* name, int nameLen)
	{
		m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, nameLen, 1, MarkerFlags.Default, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ProfilerMarker(ProfilerCategory category, string name)
	{
		m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, category, MarkerFlags.Default, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ProfilerMarker(ProfilerCategory category, char* name, int nameLen)
	{
		m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, nameLen, category, MarkerFlags.Default, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ProfilerMarker(string name, MarkerFlags flags)
	{
		m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, 1, flags, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ProfilerMarker(char* name, int nameLen, MarkerFlags flags)
	{
		m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, nameLen, 1, flags, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ProfilerMarker(ProfilerCategory category, string name, MarkerFlags flags)
	{
		m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, category, flags, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe ProfilerMarker(ProfilerCategory category, char* name, int nameLen, MarkerFlags flags)
	{
		m_Ptr = ProfilerUnsafeUtility.CreateMarker(name, nameLen, category, flags, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("ENABLE_PROFILER")]
	[Pure]
	public void Begin()
	{
		ProfilerUnsafeUtility.BeginSample(m_Ptr);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("ENABLE_PROFILER")]
	public void Begin(UnityEngine.Object contextUnityObject)
	{
		ProfilerUnsafeUtility.Internal_BeginWithObject(m_Ptr, contextUnityObject);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("ENABLE_PROFILER")]
	[Pure]
	public void End()
	{
		ProfilerUnsafeUtility.EndSample(m_Ptr);
	}

	[Conditional("ENABLE_PROFILER")]
	internal void GetName(ref string name)
	{
		name = ProfilerUnsafeUtility.Internal_GetName(m_Ptr);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Pure]
	public AutoScope Auto()
	{
		return new AutoScope(m_Ptr);
	}
}
