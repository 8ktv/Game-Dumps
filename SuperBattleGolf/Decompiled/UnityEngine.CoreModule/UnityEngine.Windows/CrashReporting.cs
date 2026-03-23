using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Windows;

public static class CrashReporting
{
	public static string crashReportFolder
	{
		[ThreadSafe]
		[NativeHeader("PlatformDependent/WinPlayer/Bindings/CrashReportingBindings.h")]
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				get_crashReportFolder_Injected(out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_crashReportFolder_Injected(out ManagedSpanWrapper ret);
}
