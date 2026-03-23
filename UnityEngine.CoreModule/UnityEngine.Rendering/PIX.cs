using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

[NativeConditional("PLATFORM_WIN && ENABLE_PROFILER")]
[NativeHeader("PlatformDependent/Win/Profiler/PixBindings.h")]
public class PIX
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PIX::BeginGPUCapture")]
	public static extern void BeginGPUCapture();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PIX::EndGPUCapture")]
	public static extern void EndGPUCapture();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PIX::IsAttached")]
	public static extern bool IsAttached();
}
