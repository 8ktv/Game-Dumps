using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Graphics/ScriptableRenderLoop/RendererList.h")]
[MovedFrom("UnityEngine.Rendering.RendererUtils")]
public struct RendererList
{
	internal UIntPtr context;

	internal uint index;

	internal uint frame;

	internal uint type;

	internal uint contextID;

	public static readonly RendererList nullRendererList = new RendererList(UIntPtr.Zero, uint.MaxValue);

	public extern bool isValid
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	internal RendererList(UIntPtr ctx, uint indx)
	{
		context = ctx;
		index = indx;
		frame = 0u;
		type = 0u;
		contextID = 0u;
	}
}
