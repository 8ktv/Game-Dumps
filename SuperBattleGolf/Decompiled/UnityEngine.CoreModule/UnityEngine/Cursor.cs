using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Input/Cursor.bindings.h")]
public class Cursor
{
	public static extern bool visible
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern CursorLockMode lockState
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	private static void SetCursor(Texture2D texture, CursorMode cursorMode)
	{
		SetCursor(texture, Vector2.zero, cursorMode);
	}

	public static void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
	{
		SetCursor_Injected(Object.MarshalledUnityObject.Marshal(texture), ref hotspot, cursorMode);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCursor_Injected(IntPtr texture, [In] ref Vector2 hotspot, CursorMode cursorMode);
}
