using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/QualitySettingsTypes.h")]
[NativeHeader("Runtime/Camera/RenderSettings.h")]
[StaticAccessor("GetRenderSettings()", StaticAccessorType.Dot)]
public sealed class RenderSettings : Object
{
	[Obsolete("Use RenderSettings.ambientIntensity instead (UnityUpgradable) -> ambientIntensity", false)]
	public static float ambientSkyboxAmount
	{
		get
		{
			return ambientIntensity;
		}
		set
		{
			ambientIntensity = value;
		}
	}

	[NativeProperty("UseFog")]
	public static extern bool fog
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("LinearFogStart")]
	public static extern float fogStartDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("LinearFogEnd")]
	public static extern float fogEndDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern FogMode fogMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static Color fogColor
	{
		get
		{
			get_fogColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_fogColor_Injected(ref value);
		}
	}

	public static extern float fogDensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern AmbientMode ambientMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static Color ambientSkyColor
	{
		get
		{
			get_ambientSkyColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientSkyColor_Injected(ref value);
		}
	}

	public static Color ambientEquatorColor
	{
		get
		{
			get_ambientEquatorColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientEquatorColor_Injected(ref value);
		}
	}

	public static Color ambientGroundColor
	{
		get
		{
			get_ambientGroundColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientGroundColor_Injected(ref value);
		}
	}

	public static extern float ambientIntensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("AmbientSkyColor")]
	public static Color ambientLight
	{
		get
		{
			get_ambientLight_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientLight_Injected(ref value);
		}
	}

	public static Color subtractiveShadowColor
	{
		get
		{
			get_subtractiveShadowColor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_subtractiveShadowColor_Injected(ref value);
		}
	}

	[NativeProperty("SkyboxMaterial")]
	public static Material skybox
	{
		get
		{
			return Unmarshal.UnmarshalUnityObject<Material>(get_skybox_Injected());
		}
		set
		{
			set_skybox_Injected(MarshalledUnityObject.Marshal(value));
		}
	}

	public static Light sun
	{
		get
		{
			return Unmarshal.UnmarshalUnityObject<Light>(get_sun_Injected());
		}
		set
		{
			set_sun_Injected(MarshalledUnityObject.Marshal(value));
		}
	}

	public static SphericalHarmonicsL2 ambientProbe
	{
		[NativeMethod("GetFinalAmbientProbe")]
		get
		{
			get_ambientProbe_Injected(out var ret);
			return ret;
		}
		set
		{
			set_ambientProbe_Injected(ref value);
		}
	}

	[Obsolete("RenderSettings.customReflection has been deprecated in favor of RenderSettings.customReflectionTexture.", false)]
	public static Cubemap customReflection
	{
		get
		{
			if (!(customReflectionTexture is Cubemap result))
			{
				throw new ArgumentException("RenderSettings.customReflection is currently not referencing a cubemap.");
			}
			return result;
		}
		[NativeThrows]
		set
		{
			customReflectionTexture = value;
		}
	}

	[NativeProperty("CustomReflection")]
	public static Texture customReflectionTexture
	{
		get
		{
			return Unmarshal.UnmarshalUnityObject<Texture>(get_customReflectionTexture_Injected());
		}
		[NativeThrows]
		set
		{
			set_customReflectionTexture_Injected(MarshalledUnityObject.Marshal(value));
		}
	}

	public static extern float reflectionIntensity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern int reflectionBounces
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("GeneratedSkyboxReflection")]
	internal static Cubemap defaultReflection => Unmarshal.UnmarshalUnityObject<Cubemap>(get_defaultReflection_Injected());

	public static extern DefaultReflectionMode defaultReflectionMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern int defaultReflectionResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern float haloStrength
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern float flareStrength
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern float flareFadeSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	private RenderSettings()
	{
	}

	[FreeFunction("GetRenderSettings")]
	internal static Object GetRenderSettings()
	{
		return Unmarshal.UnmarshalUnityObject<Object>(GetRenderSettings_Injected());
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("RenderSettingsScripting", StaticAccessorType.DoubleColon)]
	internal static extern void Reset();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_fogColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_fogColor_Injected([In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_ambientSkyColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_ambientSkyColor_Injected([In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_ambientEquatorColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_ambientEquatorColor_Injected([In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_ambientGroundColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_ambientGroundColor_Injected([In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_ambientLight_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_ambientLight_Injected([In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_subtractiveShadowColor_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_subtractiveShadowColor_Injected([In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_skybox_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_skybox_Injected(IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_sun_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sun_Injected(IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_ambientProbe_Injected(out SphericalHarmonicsL2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_ambientProbe_Injected([In] ref SphericalHarmonicsL2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_customReflectionTexture_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_customReflectionTexture_Injected(IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_defaultReflection_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetRenderSettings_Injected();
}
