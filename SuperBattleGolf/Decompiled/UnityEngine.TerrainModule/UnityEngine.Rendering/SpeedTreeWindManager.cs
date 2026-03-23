using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

[StaticAccessor("GetSpeedTreeWindManager()", StaticAccessorType.Dot)]
[NativeHeader("Modules/Terrain/Public/SpeedTreeWindManager.h")]
internal static class SpeedTreeWindManager
{
	public unsafe static void UpdateWindAndWriteBufferWindParams(ReadOnlySpan<int> renderersID, SpeedTreeWindParamsBufferIterator windParams, bool history)
	{
		ReadOnlySpan<int> readOnlySpan = renderersID;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper renderersID2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			UpdateWindAndWriteBufferWindParams_Injected(ref renderersID2, ref windParams, history);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateWindAndWriteBufferWindParams_Injected(ref ManagedSpanWrapper renderersID, [In] ref SpeedTreeWindParamsBufferIterator windParams, bool history);
}
