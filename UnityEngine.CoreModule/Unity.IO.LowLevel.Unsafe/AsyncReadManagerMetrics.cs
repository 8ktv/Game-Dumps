using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe;

[NativeConditional("ENABLE_PROFILER")]
public static class AsyncReadManagerMetrics
{
	[Flags]
	public enum Flags
	{
		None = 0,
		ClearOnRead = 1
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AreMetricsEnabled_Internal")]
	public static extern bool IsEnabled();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetAsyncReadManagerMetrics()->ClearMetrics")]
	[ThreadSafe]
	private static extern void ClearMetrics_Internal();

	public static void ClearCompletedMetrics()
	{
		ClearMetrics_Internal();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetMarshalledMetrics")]
	[ThreadSafe]
	internal static extern AsyncReadManagerRequestMetric[] GetMetrics_Internal(bool clear);

	[FreeFunction("GetAsyncReadManagerMetrics()->GetMetrics_NoAlloc")]
	[ThreadSafe]
	internal static void GetMetrics_NoAlloc_Internal([NotNull] List<AsyncReadManagerRequestMetric> metrics, bool clear)
	{
		if (metrics == null)
		{
			ThrowHelper.ThrowArgumentNullException(metrics, "metrics");
		}
		GetMetrics_NoAlloc_Internal_Injected(metrics, clear);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetMarshalledMetrics_Filtered_Managed")]
	internal static extern AsyncReadManagerRequestMetric[] GetMetrics_Filtered_Internal(AsyncReadManagerMetricsFilters filters, bool clear);

	[ThreadSafe]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetMetrics_NoAlloc_Filtered_Managed")]
	internal static void GetMetrics_NoAlloc_Filtered_Internal([NotNull] List<AsyncReadManagerRequestMetric> metrics, AsyncReadManagerMetricsFilters filters, bool clear)
	{
		if (metrics == null)
		{
			ThrowHelper.ThrowArgumentNullException(metrics, "metrics");
		}
		GetMetrics_NoAlloc_Filtered_Internal_Injected(metrics, filters, clear);
	}

	public static AsyncReadManagerRequestMetric[] GetMetrics(AsyncReadManagerMetricsFilters filters, Flags flags)
	{
		bool clear = (flags & Flags.ClearOnRead) == Flags.ClearOnRead;
		return GetMetrics_Filtered_Internal(filters, clear);
	}

	public static void GetMetrics(List<AsyncReadManagerRequestMetric> outMetrics, AsyncReadManagerMetricsFilters filters, Flags flags)
	{
		bool clear = (flags & Flags.ClearOnRead) == Flags.ClearOnRead;
		GetMetrics_NoAlloc_Filtered_Internal(outMetrics, filters, clear);
	}

	public static AsyncReadManagerRequestMetric[] GetMetrics(Flags flags)
	{
		bool clear = (flags & Flags.ClearOnRead) == Flags.ClearOnRead;
		return GetMetrics_Internal(clear);
	}

	public static void GetMetrics(List<AsyncReadManagerRequestMetric> outMetrics, Flags flags)
	{
		bool clear = (flags & Flags.ClearOnRead) == Flags.ClearOnRead;
		GetMetrics_NoAlloc_Internal(outMetrics, clear);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetAsyncReadManagerMetrics()->StartCollecting")]
	public static extern void StartCollectingMetrics();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetAsyncReadManagerMetrics()->StopCollecting")]
	public static extern void StopCollectingMetrics();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetCurrentSummaryMetrics")]
	internal static extern AsyncReadManagerSummaryMetrics GetSummaryMetrics_Internal(bool clear);

	public static AsyncReadManagerSummaryMetrics GetCurrentSummaryMetrics(Flags flags)
	{
		bool clear = (flags & Flags.ClearOnRead) == Flags.ClearOnRead;
		return GetSummaryMetrics_Internal(clear);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetCurrentSummaryMetricsWithFilters")]
	internal static extern AsyncReadManagerSummaryMetrics GetSummaryMetricsWithFilters_Internal(AsyncReadManagerMetricsFilters metricsFilters, bool clear);

	public static AsyncReadManagerSummaryMetrics GetCurrentSummaryMetrics(AsyncReadManagerMetricsFilters metricsFilters, Flags flags)
	{
		bool clear = (flags & Flags.ClearOnRead) == Flags.ClearOnRead;
		return GetSummaryMetricsWithFilters_Internal(metricsFilters, clear);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetSummaryOfMetrics_Managed")]
	internal static extern AsyncReadManagerSummaryMetrics GetSummaryOfMetrics_Internal(AsyncReadManagerRequestMetric[] metrics);

	public static AsyncReadManagerSummaryMetrics GetSummaryOfMetrics(AsyncReadManagerRequestMetric[] metrics)
	{
		return GetSummaryOfMetrics_Internal(metrics);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetSummaryOfMetrics_FromContainer_Managed", ThrowsException = true)]
	internal static extern AsyncReadManagerSummaryMetrics GetSummaryOfMetrics_FromContainer_Internal(List<AsyncReadManagerRequestMetric> metrics);

	public static AsyncReadManagerSummaryMetrics GetSummaryOfMetrics(List<AsyncReadManagerRequestMetric> metrics)
	{
		return GetSummaryOfMetrics_FromContainer_Internal(metrics);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetSummaryOfMetricsWithFilters_Managed")]
	[ThreadSafe]
	internal static extern AsyncReadManagerSummaryMetrics GetSummaryOfMetricsWithFilters_Internal(AsyncReadManagerRequestMetric[] metrics, AsyncReadManagerMetricsFilters metricsFilters);

	public static AsyncReadManagerSummaryMetrics GetSummaryOfMetrics(AsyncReadManagerRequestMetric[] metrics, AsyncReadManagerMetricsFilters metricsFilters)
	{
		return GetSummaryOfMetricsWithFilters_Internal(metrics, metricsFilters);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetSummaryOfMetricsWithFilters_FromContainer_Managed", ThrowsException = true)]
	[ThreadSafe]
	internal static extern AsyncReadManagerSummaryMetrics GetSummaryOfMetricsWithFilters_FromContainer_Internal(List<AsyncReadManagerRequestMetric> metrics, AsyncReadManagerMetricsFilters metricsFilters);

	public static AsyncReadManagerSummaryMetrics GetSummaryOfMetrics(List<AsyncReadManagerRequestMetric> metrics, AsyncReadManagerMetricsFilters metricsFilters)
	{
		return GetSummaryOfMetricsWithFilters_FromContainer_Internal(metrics, metricsFilters);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[FreeFunction("GetAsyncReadManagerMetrics()->GetTotalSizeNonASRMReadsBytes")]
	public static extern ulong GetTotalSizeOfNonASRMReadsBytes(bool emptyAfterRead);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMetrics_NoAlloc_Internal_Injected(List<AsyncReadManagerRequestMetric> metrics, bool clear);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMetrics_NoAlloc_Filtered_Internal_Injected(List<AsyncReadManagerRequestMetric> metrics, AsyncReadManagerMetricsFilters filters, bool clear);
}
