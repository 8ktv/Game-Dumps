using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Graphics/DrawSplashScreenAndWatermarks.h")]
public class Watermark
{
	[NativeProperty("s_ShowDeveloperWatermark", false, TargetType.Field)]
	public static extern bool showDeveloperWatermark
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("IsAnyWatermarkVisible")]
	public static extern bool IsVisible();
}
