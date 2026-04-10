using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[ExcludeFromPreset]
[NativeHeader("Modules/Terrain/Public/SpeedTreeWind.h")]
public class SpeedTreeWindAsset : Object
{
	public int Version
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_Version_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_Version_Injected(intPtr, value);
		}
	}

	internal SpeedTreeWindAsset(int version, SpeedTreeWindConfig9 config)
	{
		Internal_Create(this, version, SpeedTreeWindConfig9.Serialize(config));
	}

	[NativeThrows]
	private unsafe static void Internal_Create([Writable] SpeedTreeWindAsset notSelf, int version, byte[] data)
	{
		Span<byte> span = new Span<byte>(data);
		fixed (byte* begin = span)
		{
			ManagedSpanWrapper data2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_Create_Injected(notSelf, version, ref data2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_Version_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_Version_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create_Injected([Writable] SpeedTreeWindAsset notSelf, int version, ref ManagedSpanWrapper data);
}
