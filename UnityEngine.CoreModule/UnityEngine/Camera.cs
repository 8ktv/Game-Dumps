using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Shaders/Shader.h")]
[NativeHeader("Runtime/GfxDevice/GfxDeviceTypes.h")]
[NativeHeader("Runtime/Graphics/RenderTexture.h")]
[UsedByNativeCode]
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Camera/RenderManager.h")]
[NativeHeader("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
[NativeHeader("Runtime/Camera/Camera.h")]
[NativeHeader("Runtime/Misc/GameObjectUtility.h")]
public sealed class Camera : Behaviour
{
	internal enum ProjectionMatrixMode
	{
		Explicit,
		Implicit,
		PhysicalPropertiesBased
	}

	public enum GateFitMode
	{
		Vertical = 1,
		Horizontal = 2,
		Fill = 3,
		Overscan = 4,
		None = 0
	}

	public enum FieldOfViewAxis
	{
		Vertical,
		Horizontal
	}

	public struct GateFitParameters
	{
		public GateFitMode mode { get; set; }

		public float aspect { get; set; }

		public GateFitParameters(GateFitMode mode, float aspect)
		{
			this.mode = mode;
			this.aspect = aspect;
		}
	}

	public enum StereoscopicEye
	{
		Left,
		Right
	}

	public enum MonoOrStereoscopicEye
	{
		Left,
		Right,
		Mono
	}

	public enum SceneViewFilterMode
	{
		Off,
		ShowFiltered
	}

	public delegate void CameraCallback(Camera cam);

	public const float kMinAperture = 0.7f;

	public const float kMaxAperture = 32f;

	public const int kMinBladeCount = 3;

	public const int kMaxBladeCount = 11;

	internal uint m_NonSerializedVersion;

	public static CameraCallback onPreCull;

	public static CameraCallback onPreRender;

	public static CameraCallback onPostRender;

	[NativeProperty("Near")]
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

	[NativeProperty("Far")]
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

	[NativeProperty("VerticalFieldOfView")]
	public float fieldOfView
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_fieldOfView_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_fieldOfView_Injected(intPtr, value);
		}
	}

	public RenderingPath renderingPath
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_renderingPath_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_renderingPath_Injected(intPtr, value);
		}
	}

	public RenderingPath actualRenderingPath
	{
		[NativeName("CalculateRenderingPath")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_actualRenderingPath_Injected(intPtr);
		}
	}

	public bool allowHDR
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowHDR_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowHDR_Injected(intPtr, value);
		}
	}

	public bool allowMSAA
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowMSAA_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowMSAA_Injected(intPtr, value);
		}
	}

	public bool allowDynamicResolution
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowDynamicResolution_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowDynamicResolution_Injected(intPtr, value);
		}
	}

	[NativeProperty("ForceIntoRT")]
	public bool forceIntoRenderTexture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_forceIntoRenderTexture_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_forceIntoRenderTexture_Injected(intPtr, value);
		}
	}

	public float orthographicSize
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_orthographicSize_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_orthographicSize_Injected(intPtr, value);
		}
	}

	public bool orthographic
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_orthographic_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_orthographic_Injected(intPtr, value);
		}
	}

	public OpaqueSortMode opaqueSortMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_opaqueSortMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_opaqueSortMode_Injected(intPtr, value);
		}
	}

	public TransparencySortMode transparencySortMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_transparencySortMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_transparencySortMode_Injected(intPtr, value);
		}
	}

	public Vector3 transparencySortAxis
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_transparencySortAxis_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_transparencySortAxis_Injected(intPtr, ref value);
		}
	}

	public float depth
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_depth_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_depth_Injected(intPtr, value);
		}
	}

	public float aspect
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_aspect_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_aspect_Injected(intPtr, value);
		}
	}

	public Vector3 velocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_velocity_Injected(intPtr, out var ret);
			return ret;
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

	public int eventMask
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_eventMask_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_eventMask_Injected(intPtr, value);
		}
	}

	public bool layerCullSpherical
	{
		get
		{
			return layerCullSphericalInternal;
		}
		set
		{
			if (GraphicsSettings.currentRenderPipeline != null)
			{
				Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.layerCullSpherical only with the built-in renderer.");
			}
			layerCullSphericalInternal = value;
		}
	}

	[NativeProperty("LayerCullSpherical")]
	internal bool layerCullSphericalInternal
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_layerCullSphericalInternal_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_layerCullSphericalInternal_Injected(intPtr, value);
		}
	}

	public CameraType cameraType
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cameraType_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cameraType_Injected(intPtr, value);
		}
	}

	internal Material skyboxMaterial
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(get_skyboxMaterial_Injected(intPtr));
		}
	}

	[NativeConditional("UNITY_EDITOR")]
	public ulong overrideSceneCullingMask
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_overrideSceneCullingMask_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_overrideSceneCullingMask_Injected(intPtr, value);
		}
	}

	[NativeConditional("UNITY_EDITOR")]
	internal ulong sceneCullingMask
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sceneCullingMask_Injected(intPtr);
		}
	}

	[NativeConditional("UNITY_EDITOR")]
	internal bool useInteractiveLightBakingData
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useInteractiveLightBakingData_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useInteractiveLightBakingData_Injected(intPtr, value);
		}
	}

	public float[] layerCullDistances
	{
		get
		{
			return GetLayerCullDistances();
		}
		set
		{
			if (value.Length != 32)
			{
				throw new UnityException("Array needs to contain exactly 32 floats for layerCullDistances.");
			}
			SetLayerCullDistances(value);
		}
	}

	[Obsolete("PreviewCullingLayer is obsolete. Use scene culling masks instead.", false)]
	internal static int PreviewCullingLayer => 31;

	public bool useOcclusionCulling
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useOcclusionCulling_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useOcclusionCulling_Injected(intPtr, value);
		}
	}

	public Matrix4x4 cullingMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_cullingMatrix_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cullingMatrix_Injected(intPtr, ref value);
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

	public CameraClearFlags clearFlags
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

	public DepthTextureMode depthTextureMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_depthTextureMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_depthTextureMode_Injected(intPtr, value);
		}
	}

	public bool clearStencilAfterLightingPass
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_clearStencilAfterLightingPass_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_clearStencilAfterLightingPass_Injected(intPtr, value);
		}
	}

	internal ProjectionMatrixMode projectionMatrixMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_projectionMatrixMode_Injected(intPtr);
		}
	}

	public bool usePhysicalProperties
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_usePhysicalProperties_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_usePhysicalProperties_Injected(intPtr, value);
		}
	}

	public int iso
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_iso_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_iso_Injected(intPtr, value);
		}
	}

	public float shutterSpeed
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shutterSpeed_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shutterSpeed_Injected(intPtr, value);
		}
	}

	public float aperture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_aperture_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_aperture_Injected(intPtr, value);
		}
	}

	public float focusDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_focusDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_focusDistance_Injected(intPtr, value);
		}
	}

	public float focalLength
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_focalLength_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_focalLength_Injected(intPtr, value);
		}
	}

	public int bladeCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bladeCount_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bladeCount_Injected(intPtr, value);
		}
	}

	public Vector2 curvature
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_curvature_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_curvature_Injected(intPtr, ref value);
		}
	}

	public float barrelClipping
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_barrelClipping_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_barrelClipping_Injected(intPtr, value);
		}
	}

	public float anamorphism
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_anamorphism_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_anamorphism_Injected(intPtr, value);
		}
	}

	public Vector2 sensorSize
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_sensorSize_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sensorSize_Injected(intPtr, ref value);
		}
	}

	public Vector2 lensShift
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_lensShift_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lensShift_Injected(intPtr, ref value);
		}
	}

	public GateFitMode gateFit
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_gateFit_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_gateFit_Injected(intPtr, value);
		}
	}

	[NativeProperty("NormalizedViewportRect")]
	public Rect rect
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rect_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rect_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("ScreenViewportRect")]
	public Rect pixelRect
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_pixelRect_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_pixelRect_Injected(intPtr, ref value);
		}
	}

	public int pixelWidth
	{
		[FreeFunction("CameraScripting::GetPixelWidth", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_pixelWidth_Injected(intPtr);
		}
	}

	public int pixelHeight
	{
		[FreeFunction("CameraScripting::GetPixelHeight", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_pixelHeight_Injected(intPtr);
		}
	}

	public int scaledPixelWidth
	{
		[FreeFunction("CameraScripting::GetScaledPixelWidth", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_scaledPixelWidth_Injected(intPtr);
		}
	}

	public int scaledPixelHeight
	{
		[FreeFunction("CameraScripting::GetScaledPixelHeight", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_scaledPixelHeight_Injected(intPtr);
		}
	}

	public RenderTexture targetTexture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(get_targetTexture_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_targetTexture_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public RenderTexture activeTexture
	{
		[NativeName("GetCurrentTargetTexture")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(get_activeTexture_Injected(intPtr));
		}
	}

	public int targetDisplay
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_targetDisplay_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_targetDisplay_Injected(intPtr, value);
		}
	}

	public Matrix4x4 cameraToWorldMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_cameraToWorldMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Matrix4x4 worldToCameraMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_worldToCameraMatrix_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_worldToCameraMatrix_Injected(intPtr, ref value);
		}
	}

	public Matrix4x4 projectionMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_projectionMatrix_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_projectionMatrix_Injected(intPtr, ref value);
		}
	}

	public Matrix4x4 nonJitteredProjectionMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_nonJitteredProjectionMatrix_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_nonJitteredProjectionMatrix_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("UseJitteredProjectionMatrixForTransparent")]
	public bool useJitteredProjectionMatrixForTransparentRendering
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useJitteredProjectionMatrixForTransparentRendering_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useJitteredProjectionMatrixForTransparentRendering_Injected(intPtr, value);
		}
	}

	public Matrix4x4 previousViewProjectionMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_previousViewProjectionMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public static Camera main
	{
		[FreeFunction("FindMainCamera")]
		get
		{
			return Unmarshal.UnmarshalUnityObject<Camera>(get_main_Injected());
		}
	}

	public static Camera current => currentInternal;

	private static Camera currentInternal
	{
		[FreeFunction("GetCurrentCameraPPtr")]
		get
		{
			return Unmarshal.UnmarshalUnityObject<Camera>(get_currentInternal_Injected());
		}
	}

	public Scene scene
	{
		[FreeFunction("CameraScripting::GetScene", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_scene_Injected(intPtr, out var ret);
			return ret;
		}
		[FreeFunction("CameraScripting::SetScene", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_scene_Injected(intPtr, ref value);
		}
	}

	public bool stereoEnabled
	{
		[NativeMethod("GetStereoEnabledForBuiltInOrSRP")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stereoEnabled_Injected(intPtr);
		}
	}

	public float stereoSeparation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stereoSeparation_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_stereoSeparation_Injected(intPtr, value);
		}
	}

	public float stereoConvergence
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stereoConvergence_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_stereoConvergence_Injected(intPtr, value);
		}
	}

	public bool areVRStereoViewMatricesWithinSingleCullTolerance
	{
		[NativeName("AreVRStereoViewMatricesWithinSingleCullTolerance")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_areVRStereoViewMatricesWithinSingleCullTolerance_Injected(intPtr);
		}
	}

	public StereoTargetEyeMask stereoTargetEye
	{
		get
		{
			return stereoTargetEyeInternal;
		}
		set
		{
			if (GraphicsSettings.currentRenderPipeline != null)
			{
				Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.stereoTargetEye only with the built-in renderer.");
			}
			stereoTargetEyeInternal = value;
		}
	}

	[NativeProperty("StereoTargetEye")]
	internal StereoTargetEyeMask stereoTargetEyeInternal
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stereoTargetEyeInternal_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_stereoTargetEyeInternal_Injected(intPtr, value);
		}
	}

	public MonoOrStereoscopicEye stereoActiveEye
	{
		[FreeFunction("CameraScripting::GetStereoActiveEye", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stereoActiveEye_Injected(intPtr);
		}
	}

	public static int allCamerasCount => GetAllCamerasCount();

	public static Camera[] allCameras
	{
		get
		{
			Camera[] array = new Camera[allCamerasCount];
			GetAllCamerasImpl(array);
			return array;
		}
	}

	[NativeConditional("UNITY_EDITOR")]
	public SceneViewFilterMode sceneViewFilterMode => (SceneViewFilterMode)GetFilterMode();

	[NativeConditional("UNITY_EDITOR")]
	public bool renderCloudsInSceneView
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_renderCloudsInSceneView_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_renderCloudsInSceneView_Injected(intPtr, value);
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

	public void Reset()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Reset_Injected(intPtr);
	}

	public void ResetTransparencySortSettings()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetTransparencySortSettings_Injected(intPtr);
	}

	public void ResetAspect()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetAspect_Injected(intPtr);
	}

	[FreeFunction("CameraScripting::GetLayerCullDistances", HasExplicitThis = true)]
	private float[] GetLayerCullDistances()
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
			GetLayerCullDistances_Injected(intPtr, out ret);
		}
		finally
		{
			float[] array = default(float[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("CameraScripting::SetLayerCullDistances", HasExplicitThis = true)]
	private unsafe void SetLayerCullDistances([NotNull] float[] d)
	{
		if (d == null)
		{
			ThrowHelper.ThrowArgumentNullException(d, "d");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(d);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper d2 = new ManagedSpanWrapper(begin, span.Length);
			SetLayerCullDistances_Injected(intPtr, ref d2);
		}
	}

	public void ResetCullingMatrix()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetCullingMatrix_Injected(intPtr);
	}

	public unsafe void SetReplacementShader(Shader shader, string replacementTag)
	{
		//The blocks IL_003f are reachable both inside and outside the pinned region starting at IL_002e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr shader2 = MarshalledUnityObject.Marshal(shader);
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(replacementTag, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(replacementTag);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetReplacementShader_Injected(intPtr, shader2, ref managedSpanWrapper);
					return;
				}
			}
			SetReplacementShader_Injected(intPtr, shader2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public void ResetReplacementShader()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetReplacementShader_Injected(intPtr);
	}

	public float GetGateFittedFieldOfView()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetGateFittedFieldOfView_Injected(intPtr);
	}

	public Vector2 GetGateFittedLensShift()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetGateFittedLensShift_Injected(intPtr, out var ret);
		return ret;
	}

	internal Vector3 GetLocalSpaceAim()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetLocalSpaceAim_Injected(intPtr, out var ret);
		return ret;
	}

	[FreeFunction("CameraScripting::SetTargetBuffers", HasExplicitThis = true)]
	private void SetTargetBuffersImpl(RenderBuffer color, RenderBuffer depth)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTargetBuffersImpl_Injected(intPtr, ref color, ref depth);
	}

	public void SetTargetBuffers(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
	{
		SetTargetBuffersImpl(colorBuffer, depthBuffer);
	}

	[FreeFunction("CameraScripting::SetTargetBuffers", HasExplicitThis = true)]
	private unsafe void SetTargetBuffersMRTImpl(RenderBuffer[] color, RenderBuffer depth)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<RenderBuffer> span = new Span<RenderBuffer>(color);
		fixed (RenderBuffer* begin = span)
		{
			ManagedSpanWrapper color2 = new ManagedSpanWrapper(begin, span.Length);
			SetTargetBuffersMRTImpl_Injected(intPtr, ref color2, ref depth);
		}
	}

	public void SetTargetBuffers(RenderBuffer[] colorBuffer, RenderBuffer depthBuffer)
	{
		SetTargetBuffersMRTImpl(colorBuffer, depthBuffer);
	}

	internal string[] GetCameraBufferWarnings()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCameraBufferWarnings_Injected(intPtr);
	}

	public void ResetWorldToCameraMatrix()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetWorldToCameraMatrix_Injected(intPtr);
	}

	public void ResetProjectionMatrix()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetProjectionMatrix_Injected(intPtr);
	}

	[FreeFunction("CameraScripting::CalculateObliqueMatrix", HasExplicitThis = true)]
	public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CalculateObliqueMatrix_Injected(intPtr, ref clipPlane, out var ret);
		return ret;
	}

	public Vector3 WorldToScreenPoint(Vector3 position, MonoOrStereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WorldToScreenPoint_Injected(intPtr, ref position, eye, out var ret);
		return ret;
	}

	public Vector3 WorldToViewportPoint(Vector3 position, MonoOrStereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WorldToViewportPoint_Injected(intPtr, ref position, eye, out var ret);
		return ret;
	}

	public Vector3 ViewportToWorldPoint(Vector3 position, MonoOrStereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ViewportToWorldPoint_Injected(intPtr, ref position, eye, out var ret);
		return ret;
	}

	public Vector3 ScreenToWorldPoint(Vector3 position, MonoOrStereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ScreenToWorldPoint_Injected(intPtr, ref position, eye, out var ret);
		return ret;
	}

	public Vector3 WorldToScreenPoint(Vector3 position)
	{
		return WorldToScreenPoint(position, MonoOrStereoscopicEye.Mono);
	}

	public Vector3 WorldToViewportPoint(Vector3 position)
	{
		return WorldToViewportPoint(position, MonoOrStereoscopicEye.Mono);
	}

	public Vector3 ViewportToWorldPoint(Vector3 position)
	{
		return ViewportToWorldPoint(position, MonoOrStereoscopicEye.Mono);
	}

	public Vector3 ScreenToWorldPoint(Vector3 position)
	{
		return ScreenToWorldPoint(position, MonoOrStereoscopicEye.Mono);
	}

	public Vector3 ScreenToViewportPoint(Vector3 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ScreenToViewportPoint_Injected(intPtr, ref position, out var ret);
		return ret;
	}

	public Vector3 ViewportToScreenPoint(Vector3 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ViewportToScreenPoint_Injected(intPtr, ref position, out var ret);
		return ret;
	}

	internal Vector2 GetFrustumPlaneSizeAt(float distance)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetFrustumPlaneSizeAt_Injected(intPtr, distance, out var ret);
		return ret;
	}

	private Ray ViewportPointToRay(Vector2 pos, MonoOrStereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ViewportPointToRay_Injected(intPtr, ref pos, eye, out var ret);
		return ret;
	}

	public Ray ViewportPointToRay(Vector3 pos, MonoOrStereoscopicEye eye)
	{
		return ViewportPointToRay((Vector2)pos, eye);
	}

	public Ray ViewportPointToRay(Vector3 pos)
	{
		return ViewportPointToRay(pos, MonoOrStereoscopicEye.Mono);
	}

	private Ray ScreenPointToRay(Vector2 pos, MonoOrStereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ScreenPointToRay_Injected(intPtr, ref pos, eye, out var ret);
		return ret;
	}

	public Ray ScreenPointToRay(Vector3 pos, MonoOrStereoscopicEye eye)
	{
		return ScreenPointToRay((Vector2)pos, eye);
	}

	public Ray ScreenPointToRay(Vector3 pos)
	{
		return ScreenPointToRay(pos, MonoOrStereoscopicEye.Mono);
	}

	[FreeFunction("CameraScripting::CalculateViewportRayVectors", HasExplicitThis = true)]
	private unsafe void CalculateFrustumCornersInternal(Rect viewport, float z, MonoOrStereoscopicEye eye, [Out] Vector3[] outCorners)
	{
		//The blocks IL_0031 are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper outCorners2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (outCorners != null)
			{
				fixed (Vector3[] array = outCorners)
				{
					if (array.Length != 0)
					{
						outCorners2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					CalculateFrustumCornersInternal_Injected(intPtr, ref viewport, z, eye, out outCorners2);
					return;
				}
			}
			CalculateFrustumCornersInternal_Injected(intPtr, ref viewport, z, eye, out outCorners2);
		}
		finally
		{
			outCorners2.Unmarshal(ref array);
		}
	}

	public void CalculateFrustumCorners(Rect viewport, float z, MonoOrStereoscopicEye eye, Vector3[] outCorners)
	{
		if (outCorners == null)
		{
			throw new ArgumentNullException("outCorners");
		}
		if (outCorners.Length < 4)
		{
			throw new ArgumentException("outCorners minimum size is 4", "outCorners");
		}
		CalculateFrustumCornersInternal(viewport, z, eye, outCorners);
	}

	[NativeName("CalculateProjectionMatrixFromPhysicalProperties")]
	private static void CalculateProjectionMatrixFromPhysicalPropertiesInternal(out Matrix4x4 output, float focalLength, Vector2 sensorSize, Vector2 lensShift, float nearClip, float farClip, float gateAspect, GateFitMode gateFitMode)
	{
		CalculateProjectionMatrixFromPhysicalPropertiesInternal_Injected(out output, focalLength, ref sensorSize, ref lensShift, nearClip, farClip, gateAspect, gateFitMode);
	}

	public static void CalculateProjectionMatrixFromPhysicalProperties(out Matrix4x4 output, float focalLength, Vector2 sensorSize, Vector2 lensShift, float nearClip, float farClip, GateFitParameters gateFitParameters = default(GateFitParameters))
	{
		CalculateProjectionMatrixFromPhysicalPropertiesInternal(out output, focalLength, sensorSize, lensShift, nearClip, farClip, gateFitParameters.aspect, gateFitParameters.mode);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("FocalLengthToFieldOfView_Safe")]
	public static extern float FocalLengthToFieldOfView(float focalLength, float sensorSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("FieldOfViewToFocalLength_Safe")]
	public static extern float FieldOfViewToFocalLength(float fieldOfView, float sensorSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("HorizontalToVerticalFieldOfView_Safe")]
	public static extern float HorizontalToVerticalFieldOfView(float horizontalFieldOfView, float aspectRatio);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern float VerticalToHorizontalFieldOfView(float verticalFieldOfView, float aspectRatio);

	public Matrix4x4 GetStereoNonJitteredProjectionMatrix(StereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetStereoNonJitteredProjectionMatrix_Injected(intPtr, eye, out var ret);
		return ret;
	}

	[FreeFunction("CameraScripting::GetStereoViewMatrix", HasExplicitThis = true)]
	public Matrix4x4 GetStereoViewMatrix(StereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetStereoViewMatrix_Injected(intPtr, eye, out var ret);
		return ret;
	}

	public void CopyStereoDeviceProjectionMatrixToNonJittered(StereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyStereoDeviceProjectionMatrixToNonJittered_Injected(intPtr, eye);
	}

	[FreeFunction("CameraScripting::GetStereoProjectionMatrix", HasExplicitThis = true)]
	public Matrix4x4 GetStereoProjectionMatrix(StereoscopicEye eye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetStereoProjectionMatrix_Injected(intPtr, eye, out var ret);
		return ret;
	}

	public void SetStereoProjectionMatrix(StereoscopicEye eye, Matrix4x4 matrix)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetStereoProjectionMatrix_Injected(intPtr, eye, ref matrix);
	}

	public void ResetStereoProjectionMatrices()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetStereoProjectionMatrices_Injected(intPtr);
	}

	public void SetStereoViewMatrix(StereoscopicEye eye, Matrix4x4 matrix)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetStereoViewMatrix_Injected(intPtr, eye, ref matrix);
	}

	public void ResetStereoViewMatrices()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetStereoViewMatrices_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CameraScripting::GetAllCamerasCount")]
	private static extern int GetAllCamerasCount();

	[FreeFunction("CameraScripting::GetAllCameras")]
	private static int GetAllCamerasImpl([Out][NotNull] Camera[] cam)
	{
		if (cam == null)
		{
			ThrowHelper.ThrowArgumentNullException(cam, "cam");
		}
		return GetAllCamerasImpl_Injected(cam);
	}

	public static int GetAllCameras(Camera[] cameras)
	{
		if (cameras == null)
		{
			throw new NullReferenceException();
		}
		if (cameras.Length < allCamerasCount)
		{
			throw new ArgumentException("Passed in array to fill with cameras is to small to hold the number of cameras. Use Camera.allCamerasCount to get the needed size.");
		}
		return GetAllCamerasImpl(cameras);
	}

	[FreeFunction("CameraScripting::RenderToCubemap", HasExplicitThis = true)]
	private bool RenderToCubemapImpl(Texture tex, [DefaultValue("63")] int faceMask)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RenderToCubemapImpl_Injected(intPtr, MarshalledUnityObject.Marshal(tex), faceMask);
	}

	public bool RenderToCubemap(Cubemap cubemap, int faceMask)
	{
		return RenderToCubemapImpl(cubemap, faceMask);
	}

	public bool RenderToCubemap(Cubemap cubemap)
	{
		return RenderToCubemapImpl(cubemap, 63);
	}

	public bool RenderToCubemap(RenderTexture cubemap, int faceMask)
	{
		return RenderToCubemapImpl(cubemap, faceMask);
	}

	public bool RenderToCubemap(RenderTexture cubemap)
	{
		return RenderToCubemapImpl(cubemap, 63);
	}

	[NativeConditional("UNITY_EDITOR")]
	private int GetFilterMode()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetFilterMode_Injected(intPtr);
	}

	[NativeName("RenderToCubemap")]
	private bool RenderToCubemapEyeImpl(RenderTexture cubemap, int faceMask, MonoOrStereoscopicEye stereoEye)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return RenderToCubemapEyeImpl_Injected(intPtr, MarshalledUnityObject.Marshal(cubemap), faceMask, stereoEye);
	}

	public bool RenderToCubemap(RenderTexture cubemap, int faceMask, MonoOrStereoscopicEye stereoEye)
	{
		return RenderToCubemapEyeImpl(cubemap, faceMask, stereoEye);
	}

	[FreeFunction("CameraScripting::Render", HasExplicitThis = true)]
	public void Render()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Render_Injected(intPtr);
	}

	[FreeFunction("CameraScripting::RenderWithShader", HasExplicitThis = true)]
	public unsafe void RenderWithShader(Shader shader, string replacementTag)
	{
		//The blocks IL_003f are reachable both inside and outside the pinned region starting at IL_002e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr shader2 = MarshalledUnityObject.Marshal(shader);
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(replacementTag, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(replacementTag);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					RenderWithShader_Injected(intPtr, shader2, ref managedSpanWrapper);
					return;
				}
			}
			RenderWithShader_Injected(intPtr, shader2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("CameraScripting::RenderDontRestore", HasExplicitThis = true)]
	public void RenderDontRestore()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RenderDontRestore_Injected(intPtr);
	}

	public void SubmitRenderRequest<RequestData>(RequestData renderRequest)
	{
		if (renderRequest == null)
		{
			throw new ArgumentException("SubmitRenderRequest is invoked with invalid renderRequests");
		}
		if (renderRequest is ObjectIdRequest objectIdRequest)
		{
			if (objectIdRequest.destination.depthStencilFormat == GraphicsFormat.None)
			{
				Debug.LogWarning("ObjectId Render Request submitted without a depth stencil, which can produce results that are not depth tested correctly");
			}
			if (GraphicsSettings.currentRenderPipeline == null || !RenderPipelineManager.currentPipeline.IsRenderRequestSupported(this, objectIdRequest))
			{
				throw new ArgumentException((GraphicsSettings.currentRenderPipeline == null) ? "The Built-In Render Pipeline does not support ObjectIdRequest outside of the editor." : "The current render pipeline does not support ObjectIdRequest, and the fallback implementation of the Built-In Render Pipeline is not available outside of the editor.");
			}
		}
		if (GraphicsSettings.currentRenderPipeline == null)
		{
			Debug.LogWarning("Trying to invoke 'SubmitRenderRequest' when no SRP is set. A scriptable render pipeline is needed for this function call");
		}
		else
		{
			SubmitRenderRequestsInternal(renderRequest);
		}
	}

	[FreeFunction("CameraScripting::SubmitRenderRequests", HasExplicitThis = true)]
	private void SubmitRenderRequestsInternal(object requests)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SubmitRenderRequestsInternal_Injected(intPtr, requests);
	}

	[NativeConditional("UNITY_EDITOR")]
	[FreeFunction("CameraScripting::SubmitBuiltInObjectIDRenderRequest", HasExplicitThis = true)]
	[return: Unmarshalled]
	private Object[] SubmitBuiltInObjectIDRenderRequest(RenderTexture target, int mipLevel, CubemapFace cubemapFace, int depthSlice)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SubmitBuiltInObjectIDRenderRequest_Injected(intPtr, MarshalledUnityObject.Marshal(target), mipLevel, cubemapFace, depthSlice);
	}

	[FreeFunction("CameraScripting::SetupCurrent")]
	public static void SetupCurrent(Camera cur)
	{
		SetupCurrent_Injected(MarshalledUnityObject.Marshal(cur));
	}

	[FreeFunction("CameraScripting::CopyFrom", HasExplicitThis = true)]
	public void CopyFrom(Camera other)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyFrom_Injected(intPtr, MarshalledUnityObject.Marshal(other));
	}

	[NativeName("RemoveCommandBuffers")]
	private void RemoveCommandBuffersImpl(CameraEvent evt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveCommandBuffersImpl_Injected(intPtr, evt);
	}

	[NativeName("RemoveAllCommandBuffers")]
	private void RemoveAllCommandBuffersImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveAllCommandBuffersImpl_Injected(intPtr);
	}

	public void RemoveCommandBuffers(CameraEvent evt)
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.RemoveCommandBuffers only with the built-in renderer.");
			return;
		}
		m_NonSerializedVersion++;
		RemoveCommandBuffersImpl(evt);
	}

	public void RemoveAllCommandBuffers()
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.RemoveAllCommandBuffers only with the built-in renderer.");
			return;
		}
		m_NonSerializedVersion++;
		RemoveAllCommandBuffersImpl();
	}

	[NativeName("AddCommandBuffer")]
	private void AddCommandBufferImpl(CameraEvent evt, [NotNull] CommandBuffer buffer)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = CommandBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		AddCommandBufferImpl_Injected(intPtr, evt, intPtr2);
	}

	[NativeName("AddCommandBufferAsync")]
	private void AddCommandBufferAsyncImpl(CameraEvent evt, [NotNull] CommandBuffer buffer, ComputeQueueType queueType)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = CommandBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		AddCommandBufferAsyncImpl_Injected(intPtr, evt, intPtr2, queueType);
	}

	[NativeName("RemoveCommandBuffer")]
	private void RemoveCommandBufferImpl(CameraEvent evt, [NotNull] CommandBuffer buffer)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = CommandBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		RemoveCommandBufferImpl_Injected(intPtr, evt, intPtr2);
	}

	public void AddCommandBuffer(CameraEvent evt, CommandBuffer buffer)
	{
		if (!CameraEventUtils.IsValid(evt))
		{
			throw new ArgumentException($"Invalid CameraEvent value \"{(int)evt}\".", "evt");
		}
		if (buffer == null)
		{
			throw new NullReferenceException("buffer is null");
		}
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.AddCommandBuffer only with the built-in renderer.");
			return;
		}
		AddCommandBufferImpl(evt, buffer);
		m_NonSerializedVersion++;
	}

	public void AddCommandBufferAsync(CameraEvent evt, CommandBuffer buffer, ComputeQueueType queueType)
	{
		if (!CameraEventUtils.IsValid(evt))
		{
			throw new ArgumentException($"Invalid CameraEvent value \"{(int)evt}\".", "evt");
		}
		if (buffer == null)
		{
			throw new NullReferenceException("buffer is null");
		}
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.AddCommandBufferAsync only with the built-in renderer.");
			return;
		}
		AddCommandBufferAsyncImpl(evt, buffer, queueType);
		m_NonSerializedVersion++;
	}

	public void RemoveCommandBuffer(CameraEvent evt, CommandBuffer buffer)
	{
		if (!CameraEventUtils.IsValid(evt))
		{
			throw new ArgumentException($"Invalid CameraEvent value \"{(int)evt}\".", "evt");
		}
		if (buffer == null)
		{
			throw new NullReferenceException("buffer is null");
		}
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.RemoveCommandBuffer only with the built-in renderer.");
			return;
		}
		RemoveCommandBufferImpl(evt, buffer);
		m_NonSerializedVersion++;
	}

	public CommandBuffer[] GetCommandBuffers(CameraEvent evt)
	{
		if (RenderPipelineManager.currentPipeline != null)
		{
			Debug.LogWarning("Your project uses a scriptable render pipeline. You can use Camera.GetCommandBuffers only with the built-in renderer.");
		}
		return GetCommandBuffersImpl(evt);
	}

	[FreeFunction("CameraScripting::GetCommandBuffers", HasExplicitThis = true)]
	internal CommandBuffer[] GetCommandBuffersImpl(CameraEvent evt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCommandBuffersImpl_Injected(intPtr, evt);
	}

	[RequiredByNativeCode]
	private static void FireOnPreCull(Camera cam)
	{
		if (onPreCull != null)
		{
			onPreCull(cam);
		}
	}

	[RequiredByNativeCode]
	private static void FireOnPreRender(Camera cam)
	{
		if (onPreRender != null)
		{
			onPreRender(cam);
		}
	}

	[RequiredByNativeCode]
	private static void FireOnPostRender(Camera cam)
	{
		if (onPostRender != null)
		{
			onPostRender(cam);
		}
	}

	[RequiredByNativeCode]
	private static void BumpNonSerializedVersion(Camera cam)
	{
		cam.m_NonSerializedVersion++;
	}

	internal void OnlyUsedForTesting1()
	{
	}

	internal void OnlyUsedForTesting2()
	{
	}

	public unsafe bool TryGetCullingParameters(out ScriptableCullingParameters cullingParameters)
	{
		return GetCullingParameters_Internal(this, stereoAware: false, out cullingParameters, sizeof(ScriptableCullingParameters));
	}

	public unsafe bool TryGetCullingParameters(bool stereoAware, out ScriptableCullingParameters cullingParameters)
	{
		return GetCullingParameters_Internal(this, stereoAware, out cullingParameters, sizeof(ScriptableCullingParameters));
	}

	[FreeFunction("ScriptableRenderPipeline_Bindings::GetCullingParameters_Internal")]
	[NativeHeader("Runtime/Export/RenderPipeline/ScriptableRenderPipeline.bindings.h")]
	private static bool GetCullingParameters_Internal(Camera camera, bool stereoAware, out ScriptableCullingParameters cullingParameters, int managedCullingParametersSize)
	{
		return GetCullingParameters_Internal_Injected(MarshalledUnityObject.Marshal(camera), stereoAware, out cullingParameters, managedCullingParametersSize);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_nearClipPlane_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_nearClipPlane_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_farClipPlane_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_farClipPlane_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_fieldOfView_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_fieldOfView_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RenderingPath get_renderingPath_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderingPath_Injected(IntPtr _unity_self, RenderingPath value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RenderingPath get_actualRenderingPath_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Reset_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowHDR_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowHDR_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowMSAA_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowMSAA_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowDynamicResolution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowDynamicResolution_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_forceIntoRenderTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_forceIntoRenderTexture_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_orthographicSize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_orthographicSize_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_orthographic_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_orthographic_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern OpaqueSortMode get_opaqueSortMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_opaqueSortMode_Injected(IntPtr _unity_self, OpaqueSortMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TransparencySortMode get_transparencySortMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_transparencySortMode_Injected(IntPtr _unity_self, TransparencySortMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_transparencySortAxis_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_transparencySortAxis_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetTransparencySortSettings_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_depth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_depth_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_aspect_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_aspect_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetAspect_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_velocity_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_cullingMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cullingMask_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_eventMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_eventMask_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_layerCullSphericalInternal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_layerCullSphericalInternal_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CameraType get_cameraType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cameraType_Injected(IntPtr _unity_self, CameraType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_skyboxMaterial_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ulong get_overrideSceneCullingMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_overrideSceneCullingMask_Injected(IntPtr _unity_self, ulong value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ulong get_sceneCullingMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useInteractiveLightBakingData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useInteractiveLightBakingData_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLayerCullDistances_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLayerCullDistances_Injected(IntPtr _unity_self, ref ManagedSpanWrapper d);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useOcclusionCulling_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useOcclusionCulling_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_cullingMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cullingMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetCullingMatrix_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_backgroundColor_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_backgroundColor_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CameraClearFlags get_clearFlags_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_clearFlags_Injected(IntPtr _unity_self, CameraClearFlags value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern DepthTextureMode get_depthTextureMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_depthTextureMode_Injected(IntPtr _unity_self, DepthTextureMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_clearStencilAfterLightingPass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_clearStencilAfterLightingPass_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetReplacementShader_Injected(IntPtr _unity_self, IntPtr shader, ref ManagedSpanWrapper replacementTag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetReplacementShader_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ProjectionMatrixMode get_projectionMatrixMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_usePhysicalProperties_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_usePhysicalProperties_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_iso_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_iso_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_shutterSpeed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shutterSpeed_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_aperture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_aperture_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_focusDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_focusDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_focalLength_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_focalLength_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_bladeCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bladeCount_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_curvature_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_curvature_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_barrelClipping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_barrelClipping_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_anamorphism_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_anamorphism_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_sensorSize_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sensorSize_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_lensShift_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lensShift_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GateFitMode get_gateFit_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_gateFit_Injected(IntPtr _unity_self, GateFitMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetGateFittedFieldOfView_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGateFittedLensShift_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLocalSpaceAim_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rect_Injected(IntPtr _unity_self, out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rect_Injected(IntPtr _unity_self, [In] ref Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_pixelRect_Injected(IntPtr _unity_self, out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_pixelRect_Injected(IntPtr _unity_self, [In] ref Rect value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_pixelWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_pixelHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_scaledPixelWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_scaledPixelHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_targetTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_targetTexture_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_activeTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_targetDisplay_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_targetDisplay_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTargetBuffersImpl_Injected(IntPtr _unity_self, [In] ref RenderBuffer color, [In] ref RenderBuffer depth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTargetBuffersMRTImpl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper color, [In] ref RenderBuffer depth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string[] GetCameraBufferWarnings_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_cameraToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_worldToCameraMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_worldToCameraMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_projectionMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_projectionMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_nonJitteredProjectionMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_nonJitteredProjectionMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useJitteredProjectionMatrixForTransparentRendering_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useJitteredProjectionMatrixForTransparentRendering_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_previousViewProjectionMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetWorldToCameraMatrix_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetProjectionMatrix_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CalculateObliqueMatrix_Injected(IntPtr _unity_self, [In] ref Vector4 clipPlane, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WorldToScreenPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, MonoOrStereoscopicEye eye, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WorldToViewportPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, MonoOrStereoscopicEye eye, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ViewportToWorldPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, MonoOrStereoscopicEye eye, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ScreenToWorldPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, MonoOrStereoscopicEye eye, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ScreenToViewportPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ViewportToScreenPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetFrustumPlaneSizeAt_Injected(IntPtr _unity_self, float distance, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ViewportPointToRay_Injected(IntPtr _unity_self, [In] ref Vector2 pos, MonoOrStereoscopicEye eye, out Ray ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ScreenPointToRay_Injected(IntPtr _unity_self, [In] ref Vector2 pos, MonoOrStereoscopicEye eye, out Ray ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CalculateFrustumCornersInternal_Injected(IntPtr _unity_self, [In] ref Rect viewport, float z, MonoOrStereoscopicEye eye, out BlittableArrayWrapper outCorners);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CalculateProjectionMatrixFromPhysicalPropertiesInternal_Injected(out Matrix4x4 output, float focalLength, [In] ref Vector2 sensorSize, [In] ref Vector2 lensShift, float nearClip, float farClip, float gateAspect, GateFitMode gateFitMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_main_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_currentInternal_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_scene_Injected(IntPtr _unity_self, out Scene ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_scene_Injected(IntPtr _unity_self, [In] ref Scene value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_stereoEnabled_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_stereoSeparation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_stereoSeparation_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_stereoConvergence_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_stereoConvergence_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_areVRStereoViewMatricesWithinSingleCullTolerance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern StereoTargetEyeMask get_stereoTargetEyeInternal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_stereoTargetEyeInternal_Injected(IntPtr _unity_self, StereoTargetEyeMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern MonoOrStereoscopicEye get_stereoActiveEye_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetStereoNonJitteredProjectionMatrix_Injected(IntPtr _unity_self, StereoscopicEye eye, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetStereoViewMatrix_Injected(IntPtr _unity_self, StereoscopicEye eye, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyStereoDeviceProjectionMatrixToNonJittered_Injected(IntPtr _unity_self, StereoscopicEye eye);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetStereoProjectionMatrix_Injected(IntPtr _unity_self, StereoscopicEye eye, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStereoProjectionMatrix_Injected(IntPtr _unity_self, StereoscopicEye eye, [In] ref Matrix4x4 matrix);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetStereoProjectionMatrices_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStereoViewMatrix_Injected(IntPtr _unity_self, StereoscopicEye eye, [In] ref Matrix4x4 matrix);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetStereoViewMatrices_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetAllCamerasImpl_Injected([Out] Camera[] cam);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RenderToCubemapImpl_Injected(IntPtr _unity_self, IntPtr tex, [DefaultValue("63")] int faceMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetFilterMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_renderCloudsInSceneView_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderCloudsInSceneView_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool RenderToCubemapEyeImpl_Injected(IntPtr _unity_self, IntPtr cubemap, int faceMask, MonoOrStereoscopicEye stereoEye);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Render_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RenderWithShader_Injected(IntPtr _unity_self, IntPtr shader, ref ManagedSpanWrapper replacementTag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RenderDontRestore_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SubmitRenderRequestsInternal_Injected(IntPtr _unity_self, object requests);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Object[] SubmitBuiltInObjectIDRenderRequest_Injected(IntPtr _unity_self, IntPtr target, int mipLevel, CubemapFace cubemapFace, int depthSlice);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetupCurrent_Injected(IntPtr cur);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyFrom_Injected(IntPtr _unity_self, IntPtr other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_commandBufferCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveCommandBuffersImpl_Injected(IntPtr _unity_self, CameraEvent evt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveAllCommandBuffersImpl_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddCommandBufferImpl_Injected(IntPtr _unity_self, CameraEvent evt, IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddCommandBufferAsyncImpl_Injected(IntPtr _unity_self, CameraEvent evt, IntPtr buffer, ComputeQueueType queueType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveCommandBufferImpl_Injected(IntPtr _unity_self, CameraEvent evt, IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CommandBuffer[] GetCommandBuffersImpl_Injected(IntPtr _unity_self, CameraEvent evt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetCullingParameters_Internal_Injected(IntPtr camera, bool stereoAware, out ScriptableCullingParameters cullingParameters, int managedCullingParametersSize);
}
