using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Camera/Light.h")]
[NativeHeader("Runtime/Export/Graphics/Light.bindings.h")]
public sealed class Light : Behaviour
{
	private int m_BakedIndex;

	[NativeProperty("LightType")]
	public LightType type
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

	[Obsolete("This property has been deprecated. Use Light.type instead.")]
	public LightShape shape { get; set; }

	public float spotAngle
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_spotAngle_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_spotAngle_Injected(intPtr, value);
		}
	}

	public float innerSpotAngle
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_innerSpotAngle_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_innerSpotAngle_Injected(intPtr, value);
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

	public float colorTemperature
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_colorTemperature_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_colorTemperature_Injected(intPtr, value);
		}
	}

	public bool useColorTemperature
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useColorTemperature_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useColorTemperature_Injected(intPtr, value);
		}
	}

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

	public float bounceIntensity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bounceIntensity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bounceIntensity_Injected(intPtr, value);
		}
	}

	public LightUnit lightUnit
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_lightUnit_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lightUnit_Injected(intPtr, value);
		}
	}

	public float luxAtDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_luxAtDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_luxAtDistance_Injected(intPtr, value);
		}
	}

	public bool enableSpotReflector
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enableSpotReflector_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enableSpotReflector_Injected(intPtr, value);
		}
	}

	public bool useBoundingSphereOverride
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useBoundingSphereOverride_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useBoundingSphereOverride_Injected(intPtr, value);
		}
	}

	public Vector4 boundingSphereOverride
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_boundingSphereOverride_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_boundingSphereOverride_Injected(intPtr, ref value);
		}
	}

	public bool useViewFrustumForShadowCasterCull
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useViewFrustumForShadowCasterCull_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useViewFrustumForShadowCasterCull_Injected(intPtr, value);
		}
	}

	public bool forceVisible
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_forceVisible_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_forceVisible_Injected(intPtr, value);
		}
	}

	public int shadowCustomResolution
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowCustomResolution_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowCustomResolution_Injected(intPtr, value);
		}
	}

	public float shadowBias
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowBias_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowBias_Injected(intPtr, value);
		}
	}

	public float shadowNormalBias
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowNormalBias_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowNormalBias_Injected(intPtr, value);
		}
	}

	public float shadowNearPlane
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowNearPlane_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowNearPlane_Injected(intPtr, value);
		}
	}

	public bool useShadowMatrixOverride
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useShadowMatrixOverride_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useShadowMatrixOverride_Injected(intPtr, value);
		}
	}

	public Matrix4x4 shadowMatrixOverride
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_shadowMatrixOverride_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowMatrixOverride_Injected(intPtr, ref value);
		}
	}

	public float range
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_range_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_range_Injected(intPtr, value);
		}
	}

	public float dilatedRange
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_dilatedRange_Injected(intPtr);
		}
	}

	public Flare flare
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Flare>(get_flare_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_flare_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public LightBakingOutput bakingOutput
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bakingOutput_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bakingOutput_Injected(intPtr, ref value);
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

	public int renderingLayerMask
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_renderingLayerMask_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_renderingLayerMask_Injected(intPtr, value);
		}
	}

	public LightShadowCasterMode lightShadowCasterMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_lightShadowCasterMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lightShadowCasterMode_Injected(intPtr, value);
		}
	}

	public LightShadows shadows
	{
		[NativeMethod("GetShadowType")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadows_Injected(intPtr);
		}
		[FreeFunction("Light_Bindings::SetShadowType", HasExplicitThis = true, ThrowsException = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadows_Injected(intPtr, value);
		}
	}

	public float shadowStrength
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowStrength_Injected(intPtr);
		}
		[FreeFunction("Light_Bindings::SetShadowStrength", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowStrength_Injected(intPtr, value);
		}
	}

	public LightShadowResolution shadowResolution
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowResolution_Injected(intPtr);
		}
		[FreeFunction("Light_Bindings::SetShadowResolution", HasExplicitThis = true, ThrowsException = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowResolution_Injected(intPtr, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Shadow softness is removed in Unity 5.0+", true)]
	public float shadowSoftness
	{
		get
		{
			return 4f;
		}
		set
		{
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Shadow softness is removed in Unity 5.0+", true)]
	public float shadowSoftnessFade
	{
		get
		{
			return 1f;
		}
		set
		{
		}
	}

	public unsafe float[] layerShadowCullDistances
	{
		[FreeFunction("Light_Bindings::GetLayerShadowCullDistances", HasExplicitThis = true, ThrowsException = false)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			float[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_layerShadowCullDistances_Injected(intPtr, out ret);
			}
			finally
			{
				float[] array = default(float[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction("Light_Bindings::SetLayerShadowCullDistances", HasExplicitThis = true, ThrowsException = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(value);
			fixed (float* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_layerShadowCullDistances_Injected(intPtr, ref value2);
			}
		}
	}

	public float cookieSize
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cookieSize_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cookieSize_Injected(intPtr, value);
		}
	}

	public Texture cookie
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(get_cookie_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cookie_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public LightRenderMode renderMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_renderMode_Injected(intPtr);
		}
		[FreeFunction("Light_Bindings::SetRenderMode", HasExplicitThis = true, ThrowsException = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_renderMode_Injected(intPtr, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("warning bakedIndex has been removed please use bakingOutput.isBaked instead.", true)]
	public int bakedIndex
	{
		get
		{
			return m_BakedIndex;
		}
		set
		{
			m_BakedIndex = value;
		}
	}

	public Vector2 areaSize
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_areaSize_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_areaSize_Injected(intPtr, ref value);
		}
	}

	public int commandBufferCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_commandBufferCount_Injected(intPtr);
		}
	}

	[Obsolete("Use QualitySettings.pixelLightCount instead.")]
	public static int pixelLightCount
	{
		get
		{
			return QualitySettings.pixelLightCount;
		}
		set
		{
			QualitySettings.pixelLightCount = value;
		}
	}

	[Obsolete("light.shadowConstantBias was removed, use light.shadowBias", true)]
	public float shadowConstantBias
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	[Obsolete("light.shadowObjectSizeBias was removed, use light.shadowBias", true)]
	public float shadowObjectSizeBias
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	[Obsolete("light.attenuate was removed; all lights always attenuate now", true)]
	public bool attenuate
	{
		get
		{
			return true;
		}
		set
		{
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

	public void AddCommandBuffer(LightEvent evt, CommandBuffer buffer)
	{
		AddCommandBuffer(evt, buffer, ShadowMapPass.All);
	}

	public void AddCommandBuffer(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask)
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Light.AddCommandBuffer only with the built-in renderer.");
		}
		AddCommandBufferInternal(evt, buffer, shadowPassMask);
	}

	[FreeFunction("Light_Bindings::AddCommandBuffer", HasExplicitThis = true)]
	internal void AddCommandBufferInternal(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddCommandBufferInternal_Injected(intPtr, evt, (buffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(buffer), shadowPassMask);
	}

	public void AddCommandBufferAsync(LightEvent evt, CommandBuffer buffer, ComputeQueueType queueType)
	{
		AddCommandBufferAsync(evt, buffer, ShadowMapPass.All, queueType);
	}

	public void AddCommandBufferAsync(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask, ComputeQueueType queueType)
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Light.AddCommandBufferAsync only with the built-in renderer.");
		}
		AddCommandBufferAsyncInternal(evt, buffer, shadowPassMask, queueType);
	}

	[FreeFunction("Light_Bindings::AddCommandBufferAsync", HasExplicitThis = true)]
	internal void AddCommandBufferAsyncInternal(LightEvent evt, CommandBuffer buffer, ShadowMapPass shadowPassMask, ComputeQueueType queueType)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddCommandBufferAsyncInternal_Injected(intPtr, evt, (buffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(buffer), shadowPassMask, queueType);
	}

	public void RemoveCommandBuffer(LightEvent evt, CommandBuffer buffer)
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Light.RemoveCommandBuffer only with the built-in renderer.");
		}
		RemoveCommandBufferInternal(evt, buffer);
	}

	[NativeMethod("RemoveCommandBuffer")]
	internal void RemoveCommandBufferInternal(LightEvent evt, CommandBuffer buffer)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveCommandBufferInternal_Injected(intPtr, evt, (buffer == null) ? ((IntPtr)0) : CommandBuffer.BindingsMarshaller.ConvertToNative(buffer));
	}

	public void RemoveCommandBuffers(LightEvent evt)
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Light.RemoveCommandBuffer only with the built-in renderer.");
		}
		RemoveCommandBuffersInternal(evt);
	}

	[NativeMethod("RemoveCommandBuffers")]
	internal void RemoveCommandBuffersInternal(LightEvent evt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveCommandBuffersInternal_Injected(intPtr, evt);
	}

	public void RemoveAllCommandBuffers()
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Light.RemoveAllCommandBuffers only with the built-in renderer.");
		}
		RemoveAllCommandBuffersInternal();
	}

	[NativeMethod("RemoveAllCommandBuffers")]
	internal void RemoveAllCommandBuffersInternal()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveAllCommandBuffersInternal_Injected(intPtr);
	}

	public CommandBuffer[] GetCommandBuffers(LightEvent evt)
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Light.GetCommandBuffers only with the built-in renderer.");
		}
		return GetCommandBuffersInternal(evt);
	}

	[FreeFunction("Light_Bindings::GetCommandBuffers", HasExplicitThis = true)]
	internal CommandBuffer[] GetCommandBuffersInternal(LightEvent evt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCommandBuffersInternal_Injected(intPtr, evt);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Obsolete("Light.GetLights has been deprecated, use FindObjectsOfType in combination with light.cullingmask/light.type", false)]
	[FreeFunction("Light_Bindings::GetLights")]
	public static extern Light[] GetLights(LightType type, int layer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LightType get_type_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_type_Injected(IntPtr _unity_self, LightType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_spotAngle_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_spotAngle_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_innerSpotAngle_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_innerSpotAngle_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_color_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_color_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_colorTemperature_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_colorTemperature_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useColorTemperature_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useColorTemperature_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_intensity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_intensity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_bounceIntensity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bounceIntensity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LightUnit get_lightUnit_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lightUnit_Injected(IntPtr _unity_self, LightUnit value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_luxAtDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_luxAtDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enableSpotReflector_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enableSpotReflector_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useBoundingSphereOverride_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useBoundingSphereOverride_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_boundingSphereOverride_Injected(IntPtr _unity_self, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_boundingSphereOverride_Injected(IntPtr _unity_self, [In] ref Vector4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useViewFrustumForShadowCasterCull_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useViewFrustumForShadowCasterCull_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_forceVisible_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_forceVisible_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_shadowCustomResolution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowCustomResolution_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_shadowBias_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowBias_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_shadowNormalBias_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowNormalBias_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_shadowNearPlane_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowNearPlane_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useShadowMatrixOverride_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useShadowMatrixOverride_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_shadowMatrixOverride_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowMatrixOverride_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_range_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_range_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_dilatedRange_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_flare_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_flare_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bakingOutput_Injected(IntPtr _unity_self, out LightBakingOutput ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bakingOutput_Injected(IntPtr _unity_self, [In] ref LightBakingOutput value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_cullingMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cullingMask_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_renderingLayerMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderingLayerMask_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LightShadowCasterMode get_lightShadowCasterMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lightShadowCasterMode_Injected(IntPtr _unity_self, LightShadowCasterMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Reset_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LightShadows get_shadows_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadows_Injected(IntPtr _unity_self, LightShadows value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_shadowStrength_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowStrength_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LightShadowResolution get_shadowResolution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowResolution_Injected(IntPtr _unity_self, LightShadowResolution value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_layerShadowCullDistances_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_layerShadowCullDistances_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_cookieSize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cookieSize_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_cookie_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cookie_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LightRenderMode get_renderMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderMode_Injected(IntPtr _unity_self, LightRenderMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_areaSize_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_areaSize_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddCommandBufferInternal_Injected(IntPtr _unity_self, LightEvent evt, IntPtr buffer, ShadowMapPass shadowPassMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddCommandBufferAsyncInternal_Injected(IntPtr _unity_self, LightEvent evt, IntPtr buffer, ShadowMapPass shadowPassMask, ComputeQueueType queueType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveCommandBufferInternal_Injected(IntPtr _unity_self, LightEvent evt, IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveCommandBuffersInternal_Injected(IntPtr _unity_self, LightEvent evt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveAllCommandBuffersInternal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CommandBuffer[] GetCommandBuffersInternal_Injected(IntPtr _unity_self, LightEvent evt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_commandBufferCount_Injected(IntPtr _unity_self);
}
