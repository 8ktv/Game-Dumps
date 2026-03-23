using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Billboard/BillboardRenderer.h")]
public sealed class BillboardRenderer : Renderer
{
	public BillboardAsset billboard
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<BillboardAsset>(get_billboard_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_billboard_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_billboard_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_billboard_Injected(IntPtr _unity_self, IntPtr value);
}
