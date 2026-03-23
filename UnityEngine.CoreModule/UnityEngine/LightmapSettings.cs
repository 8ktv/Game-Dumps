using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/LightmapSettings.h")]
[StaticAccessor("GetLightmapSettings()")]
public sealed class LightmapSettings : Object
{
	public static extern LightmapData[] lightmaps
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction]
		[return: Unmarshalled]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(ThrowsException = true)]
		[param: Unmarshalled]
		set;
	}

	public static extern LightmapsMode lightmapsMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(ThrowsException = true)]
		set;
	}

	public static LightProbes lightProbes
	{
		get
		{
			return Unmarshal.UnmarshalUnityObject<LightProbes>(get_lightProbes_Injected());
		}
		[NativeName("SetLightProbes")]
		[FreeFunction]
		set
		{
			set_lightProbes_Injected(MarshalledUnityObject.Marshal(value));
		}
	}

	[Obsolete("Use lightmapsMode instead.", false)]
	public static LightmapsModeLegacy lightmapsModeLegacy
	{
		get
		{
			return LightmapsModeLegacy.Single;
		}
		set
		{
		}
	}

	[Obsolete("Use QualitySettings.desiredColorSpace instead.", false)]
	public static ColorSpace bakedColorSpace
	{
		get
		{
			return QualitySettings.desiredColorSpace;
		}
		set
		{
		}
	}

	private LightmapSettings()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("ResetAndAwakeFromLoad")]
	internal static extern void Reset();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_lightProbes_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lightProbes_Injected(IntPtr value);
}
