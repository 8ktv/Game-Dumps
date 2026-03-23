using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
public struct RenderBuffer
{
	internal int m_RenderTextureInstanceID;

	internal IntPtr m_BufferPtr;

	internal RenderBufferLoadAction loadAction
	{
		get
		{
			return GetLoadAction();
		}
		set
		{
			SetLoadAction(value);
		}
	}

	internal RenderBufferStoreAction storeAction
	{
		get
		{
			return GetStoreAction();
		}
		set
		{
			SetStoreAction(value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RenderBufferScripting::SetLoadAction", HasExplicitThis = true)]
	internal extern void SetLoadAction(RenderBufferLoadAction action);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RenderBufferScripting::SetStoreAction", HasExplicitThis = true)]
	internal extern void SetStoreAction(RenderBufferStoreAction action);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RenderBufferScripting::GetLoadAction", HasExplicitThis = true)]
	internal extern RenderBufferLoadAction GetLoadAction();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RenderBufferScripting::GetStoreAction", HasExplicitThis = true)]
	internal extern RenderBufferStoreAction GetStoreAction();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "RenderBufferScripting::GetNativeRenderBufferPtr", HasExplicitThis = true)]
	public extern IntPtr GetNativeRenderBufferPtr();
}
