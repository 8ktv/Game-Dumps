using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[NativeHeader("Runtime/Graphics/Mesh/StaticBatching.h")]
internal struct StaticBatchingHelper
{
	[FreeFunction("StaticBatching::CombineMeshesForStaticBatching")]
	internal static void CombineMeshes(GameObject[] gos, GameObject staticBatchRoot)
	{
		CombineMeshes_Injected(gos, Object.MarshalledUnityObject.Marshal(staticBatchRoot));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CombineMeshes_Injected(GameObject[] gos, IntPtr staticBatchRoot);
}
