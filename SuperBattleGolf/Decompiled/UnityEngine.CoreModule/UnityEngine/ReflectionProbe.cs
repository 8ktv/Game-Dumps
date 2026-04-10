using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Camera/ReflectionProbes.h")]
public sealed class ReflectionProbe : Behaviour
{
	public enum ReflectionProbeEvent
	{
		ReflectionProbeAdded,
		ReflectionProbeRemoved
	}

	private static Dictionary<int, Action<Texture>> registeredDefaultReflectionSetActions = new Dictionary<int, Action<Texture>>();

	private static List<Action<Texture>> registeredDefaultReflectionTextureActions = new List<Action<Texture>>();

	[NativeName("ProbeType")]
	[Obsolete("type property has been deprecated. Starting with Unity 5.4, the only supported reflection probe type is Cube.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public ReflectionProbeType type
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_type_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_type_Injected(intPtr, value);
		}
	}

	[NativeName("BoxSize")]
	public Vector3 size
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_size_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_size_Injected(intPtr, ref value);
		}
	}

	[NativeName("BoxOffset")]
	public Vector3 center
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_center_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_center_Injected(intPtr, ref value);
		}
	}

	[NativeName("Near")]
	public float nearClipPlane
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_nearClipPlane_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_nearClipPlane_Injected(intPtr, value);
		}
	}

	[NativeName("Far")]
	public float farClipPlane
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_farClipPlane_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_farClipPlane_Injected(intPtr, value);
		}
	}

	[NativeName("IntensityMultiplier")]
	public float intensity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_intensity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_intensity_Injected(intPtr, value);
		}
	}

	[NativeName("GlobalAABB")]
	public Bounds bounds
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bounds_Injected(intPtr, out var ret);
			return ret;
		}
	}

	[NativeName("HDR")]
	public bool hdr
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hdr_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_hdr_Injected(intPtr, value);
		}
	}

	[NativeName("RenderDynamicObjects")]
	public bool renderDynamicObjects
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_renderDynamicObjects_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_renderDynamicObjects_Injected(intPtr, value);
		}
	}

	public float shadowDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowDistance_Injected(intPtr, value);
		}
	}

	public int resolution
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_resolution_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_resolution_Injected(intPtr, value);
		}
	}

	public int cullingMask
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cullingMask_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cullingMask_Injected(intPtr, value);
		}
	}

	public ReflectionProbeClearFlags clearFlags
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_clearFlags_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_clearFlags_Injected(intPtr, value);
		}
	}

	public Color backgroundColor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_backgroundColor_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_backgroundColor_Injected(intPtr, ref value);
		}
	}

	public float blendDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_blendDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_blendDistance_Injected(intPtr, value);
		}
	}

	public bool boxProjection
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_boxProjection_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_boxProjection_Injected(intPtr, value);
		}
	}

	public ReflectionProbeMode mode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_mode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_mode_Injected(intPtr, value);
		}
	}

	public int importance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_importance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_importance_Injected(intPtr, value);
		}
	}

	public ReflectionProbeRefreshMode refreshMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_refreshMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_refreshMode_Injected(intPtr, value);
		}
	}

	public ReflectionProbeTimeSlicingMode timeSlicingMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_timeSlicingMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_timeSlicingMode_Injected(intPtr, value);
		}
	}

	public Texture bakedTexture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(get_bakedTexture_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bakedTexture_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Texture customBakedTexture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(get_customBakedTexture_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_customBakedTexture_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public RenderTexture realtimeTexture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(get_realtimeTexture_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_realtimeTexture_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Texture texture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(get_texture_Injected(intPtr));
		}
	}

	public Vector4 textureHDRDecodeValues
	{
		[NativeName("CalculateHDRDecodeValues")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_textureHDRDecodeValues_Injected(intPtr, out var ret);
			return ret;
		}
	}

	[StaticAccessor("GetReflectionProbes()")]
	public static extern int minBakedCubemapResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[StaticAccessor("GetReflectionProbes()")]
	public static extern int maxBakedCubemapResolution
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[StaticAccessor("GetReflectionProbes()")]
	public static Vector4 defaultTextureHDRDecodeValues
	{
		get
		{
			get_defaultTextureHDRDecodeValues_Injected(out var ret);
			return ret;
		}
	}

	[StaticAccessor("GetReflectionProbes()")]
	public static Texture defaultTexture => Unmarshal.UnmarshalUnityObject<Texture>(get_defaultTexture_Injected());

	public static event Action<ReflectionProbe, ReflectionProbeEvent> reflectionProbeChanged;

	[Obsolete("ReflectionProbe.defaultReflectionSet has been deprecated. Use ReflectionProbe.defaultReflectionTexture. (UnityUpgradable) -> UnityEngine.ReflectionProbe.defaultReflectionTexture", false)]
	public static event Action<Cubemap> defaultReflectionSet
	{
		add
		{
			if (registeredDefaultReflectionTextureActions.Any((Action<Texture> h) => h.Method == value.Method))
			{
				return;
			}
			Action<Texture> value2 = delegate(Texture b)
			{
				if (b is Cubemap obj)
				{
					value(obj);
				}
			};
			defaultReflectionTexture += value2;
			registeredDefaultReflectionSetActions[value.Method.GetHashCode()] = value2;
		}
		remove
		{
			if (registeredDefaultReflectionSetActions.TryGetValue(value.Method.GetHashCode(), out var value2))
			{
				defaultReflectionTexture -= value2;
				registeredDefaultReflectionSetActions.Remove(value.Method.GetHashCode());
			}
		}
	}

	public static event Action<Texture> defaultReflectionTexture
	{
		add
		{
			if (!registeredDefaultReflectionTextureActions.Any((Action<Texture> h) => h.Method == value.Method) && !registeredDefaultReflectionSetActions.ContainsKey(value.Method.GetHashCode()))
			{
				registeredDefaultReflectionTextureActions.Add(value);
			}
		}
		remove
		{
			registeredDefaultReflectionTextureActions.Remove(value);
		}
	}

	public void Reset()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Reset_Injected(intPtr);
	}

	public int RenderProbe()
	{
		return RenderProbe(null);
	}

	public int RenderProbe([UnityEngine.Internal.DefaultValue("null")] RenderTexture targetTexture)
	{
		return ScheduleRender(timeSlicingMode, targetTexture);
	}

	public bool IsFinishedRendering(int renderId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsFinishedRendering_Injected(intPtr, renderId);
	}

	private int ScheduleRender(ReflectionProbeTimeSlicingMode timeSlicingMode, RenderTexture targetTexture)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ScheduleRender_Injected(intPtr, timeSlicingMode, MarshalledUnityObject.Marshal(targetTexture));
	}

	[FreeFunction("CubemapGPUBlend")]
	[NativeHeader("Runtime/Camera/CubemapGPUUtility.h")]
	public static bool BlendCubemap(Texture src, Texture dst, float blend, RenderTexture target)
	{
		return BlendCubemap_Injected(MarshalledUnityObject.Marshal(src), MarshalledUnityObject.Marshal(dst), blend, MarshalledUnityObject.Marshal(target));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("UpdateSampleData")]
	[StaticAccessor("GetReflectionProbes()")]
	public static extern void UpdateCachedState();

	[RequiredByNativeCode]
	private static void CallReflectionProbeEvent(ReflectionProbe probe, ReflectionProbeEvent probeEvent)
	{
		ReflectionProbe.reflectionProbeChanged?.Invoke(probe, probeEvent);
	}

	[RequiredByNativeCode]
	private static void CallSetDefaultReflection(Texture defaultReflectionCubemap)
	{
		foreach (Action<Texture> registeredDefaultReflectionTextureAction in registeredDefaultReflectionTextureActions)
		{
			registeredDefaultReflectionTextureAction(defaultReflectionCubemap);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ReflectionProbeType get_type_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_type_Injected(IntPtr _unity_self, ReflectionProbeType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_size_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_size_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_center_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_center_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_nearClipPlane_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_nearClipPlane_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_farClipPlane_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_farClipPlane_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_intensity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_intensity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hdr_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_hdr_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_renderDynamicObjects_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderDynamicObjects_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_shadowDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_resolution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_resolution_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_cullingMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cullingMask_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ReflectionProbeClearFlags get_clearFlags_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_clearFlags_Injected(IntPtr _unity_self, ReflectionProbeClearFlags value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_backgroundColor_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_backgroundColor_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_blendDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_blendDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_boxProjection_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_boxProjection_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ReflectionProbeMode get_mode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mode_Injected(IntPtr _unity_self, ReflectionProbeMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_importance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_importance_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ReflectionProbeRefreshMode get_refreshMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_refreshMode_Injected(IntPtr _unity_self, ReflectionProbeRefreshMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ReflectionProbeTimeSlicingMode get_timeSlicingMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_timeSlicingMode_Injected(IntPtr _unity_self, ReflectionProbeTimeSlicingMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_bakedTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bakedTexture_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_customBakedTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_customBakedTexture_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_realtimeTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_realtimeTexture_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_texture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_textureHDRDecodeValues_Injected(IntPtr _unity_self, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Reset_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsFinishedRendering_Injected(IntPtr _unity_self, int renderId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ScheduleRender_Injected(IntPtr _unity_self, ReflectionProbeTimeSlicingMode timeSlicingMode, IntPtr targetTexture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool BlendCubemap_Injected(IntPtr src, IntPtr dst, float blend, IntPtr target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_defaultTextureHDRDecodeValues_Injected(out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_defaultTexture_Injected();
}
