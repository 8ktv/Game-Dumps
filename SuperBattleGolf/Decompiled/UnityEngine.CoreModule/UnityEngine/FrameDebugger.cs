using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Profiler/PerformanceTools/FrameDebugger.h")]
[StaticAccessor("FrameDebugger", StaticAccessorType.DoubleColon)]
public static class FrameDebugger
{
	public static bool enabled => IsLocalEnabled() || IsRemoteEnabled();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsLocalEnabled();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsRemoteEnabled();
}
