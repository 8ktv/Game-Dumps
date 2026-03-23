using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode(GenerateProxy = true)]
internal class BatchRenderGroupUsageAnalytic : AnalyticsEventBase
{
	public int maxBRGInstance;

	public int maxMeshCount;

	public int maxMaterialCount;

	public int maxDrawCommandBatch;

	public BatchRenderGroupUsageAnalytic()
		: base("brgUsageEvent", 1)
	{
	}

	[RequiredByNativeCode]
	public static BatchRenderGroupUsageAnalytic CreateBatchRenderGroupUsageAnalytic()
	{
		return new BatchRenderGroupUsageAnalytic();
	}
}
