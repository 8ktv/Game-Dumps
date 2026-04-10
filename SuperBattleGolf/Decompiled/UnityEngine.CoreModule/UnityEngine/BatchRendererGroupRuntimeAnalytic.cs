using System;
using System.Runtime.InteropServices;
using UnityEngine.Analytics;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode(GenerateProxy = true)]
[ExcludeFromDocs]
internal class BatchRendererGroupRuntimeAnalytic : AnalyticsEventBase
{
	private int brgRuntimeStatus;

	private BatchRendererGroupRuntimeAnalytic()
		: base("brgPlayerUsage", 1)
	{
	}

	[RequiredByNativeCode]
	public static BatchRendererGroupRuntimeAnalytic CreateBatchRendererGroupRuntimeAnalytic()
	{
		return new BatchRendererGroupRuntimeAnalytic();
	}
}
