using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Camera/HaloManager.h")]
internal sealed class Halo : Behaviour
{
	public float size
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_size_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_size_Injected(intPtr, value);
		}
	}

	public Color color
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_color_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_color_Injected(intPtr, ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_size_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_size_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_color_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_color_Injected(IntPtr _unity_self, [In] ref Color value);
}
