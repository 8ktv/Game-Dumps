using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
public static class RendererExtensions
{
	public static void UpdateGIMaterials(this Renderer renderer)
	{
		UpdateGIMaterialsForRenderer(renderer);
	}

	[FreeFunction("RendererScripting::UpdateGIMaterialsForRenderer")]
	internal static void UpdateGIMaterialsForRenderer(Renderer renderer)
	{
		UpdateGIMaterialsForRenderer_Injected(Object.MarshalledUnityObject.Marshal(renderer));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateGIMaterialsForRenderer_Injected(IntPtr renderer);
}
