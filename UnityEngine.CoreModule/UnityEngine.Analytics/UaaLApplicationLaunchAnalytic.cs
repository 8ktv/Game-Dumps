using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode(GenerateProxy = true)]
internal class UaaLApplicationLaunchAnalytic : AnalyticsEventBase
{
	public int launch_type;

	public int launch_process_type;

	public UaaLApplicationLaunchAnalytic()
		: base("UaaLApplicationLaunch", 1)
	{
	}

	[RequiredByNativeCode]
	public static UaaLApplicationLaunchAnalytic CreateUaaLApplicationLaunchAnalytic()
	{
		return new UaaLApplicationLaunchAnalytic();
	}
}
