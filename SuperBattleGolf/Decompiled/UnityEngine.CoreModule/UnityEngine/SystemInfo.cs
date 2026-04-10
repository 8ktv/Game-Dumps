using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace UnityEngine;

[NativeHeader("Runtime/Misc/SystemInfo.h")]
[NativeHeader("Runtime/Graphics/GraphicsFormatUtility.bindings.h")]
[NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
[NativeHeader("Runtime/Misc/SystemInfoAudio.h")]
[NativeHeader("Runtime/Camera/RenderLoops/MotionVectorRenderLoop.h")]
[NativeHeader("Runtime/Misc/SystemInfoMemory.h")]
[NativeHeader("Runtime/Shaders/GraphicsCapsScriptBindings.h")]
[NativeHeader("Runtime/Misc/SystemInfoRendering.h")]
[NativeHeader("Runtime/Input/GetInput.h")]
public sealed class SystemInfo
{
	public const string unsupportedIdentifier = "n/a";

	[NativeProperty]
	public static float batteryLevel => GetBatteryLevel();

	public static BatteryStatus batteryStatus => GetBatteryStatus();

	public static string operatingSystem => GetOperatingSystem();

	public static OperatingSystemFamily operatingSystemFamily => GetOperatingSystemFamily();

	public static string processorType => GetProcessorType();

	public static string processorModel => GetProcessorModel();

	public static string processorManufacturer => GetProcessorManufacturer();

	public static int processorFrequency => GetProcessorFrequencyMHz();

	public static int processorCount => GetProcessorCount();

	public static int systemMemorySize => GetPhysicalMemoryMB();

	public static string deviceUniqueIdentifier => GetDeviceUniqueIdentifier();

	public static string deviceName => GetDeviceName();

	public static string deviceModel => GetDeviceModel();

	public static bool supportsAccelerometer => SupportsAccelerometer();

	public static bool supportsGyroscope => IsGyroAvailable();

	public static bool supportsLocationService => SupportsLocationService();

	public static bool supportsVibration => SupportsVibration();

	public static bool supportsAudio => SupportsAudio();

	public static bool supportsRendering => SupportsRendering();

	public static DeviceType deviceType => GetDeviceType();

	public static int graphicsMemorySize => GetGraphicsMemorySize();

	public static string graphicsDeviceName => GetGraphicsDeviceName();

	public static string graphicsDeviceVendor => GetGraphicsDeviceVendor();

	public static int graphicsDeviceID => GetGraphicsDeviceID();

	public static int graphicsDeviceVendorID => GetGraphicsDeviceVendorID();

	public static GraphicsDeviceType graphicsDeviceType => GetGraphicsDeviceType();

	public static bool graphicsUVStartsAtTop => GetGraphicsUVStartsAtTop();

	public static string graphicsDeviceVersion => GetGraphicsDeviceVersion();

	public static int graphicsShaderLevel => GetGraphicsShaderLevel();

	public static bool graphicsMultiThreaded => GetGraphicsMultiThreaded();

	public static RenderingThreadingMode renderingThreadingMode => GetRenderingThreadingMode();

	public static FoveatedRenderingCaps foveatedRenderingCaps => GetFoveatedRenderingCaps();

	public static bool hasTiledGPU => HasTiledGPU();

	public static bool hasHiddenSurfaceRemovalOnGPU => HasHiddenSurfaceRemovalOnGPU();

	public static bool hasDynamicUniformArrayIndexingInFragmentShaders => HasDynamicUniformArrayIndexingInFragmentShaders();

	public static bool supportsShadows => SupportsShadows();

	public static bool supportsRawShadowDepthSampling => SupportsRawShadowDepthSampling();

	[Obsolete("supportsRenderTextures always returns true, no need to call it")]
	public static bool supportsRenderTextures => true;

	public static bool supportsMotionVectors => SupportsMotionVectors();

	[Obsolete("supportsRenderToCubemap always returns true, no need to call it")]
	public static bool supportsRenderToCubemap => true;

	[Obsolete("supportsImageEffects always returns true, no need to call it")]
	public static bool supportsImageEffects => true;

	public static bool supports3DTextures => Supports3DTextures();

	public static bool supportsCompressed3DTextures => SupportsCompressed3DTextures();

	public static bool supports2DArrayTextures => Supports2DArrayTextures();

	public static bool supports3DRenderTextures => Supports3DRenderTextures();

	public static bool supportsCubemapArrayTextures => SupportsCubemapArrayTextures();

	public static bool supportsAnisotropicFilter => SupportsAnisotropicFilter();

	public static CopyTextureSupport copyTextureSupport => GetCopyTextureSupport();

	public static bool supportsComputeShaders => SupportsComputeShaders();

	public static bool supportsGeometryShaders => SupportsGeometryShaders();

	public static bool supportsTessellationShaders => SupportsTessellationShaders();

	public static bool supportsRenderTargetArrayIndexFromVertexShader => SupportsRenderTargetArrayIndexFromVertexShader();

	public static bool supportsInstancing => SupportsInstancing();

	public static bool supportsHardwareQuadTopology => SupportsHardwareQuadTopology();

	public static bool supports32bitsIndexBuffer => Supports32bitsIndexBuffer();

	public static bool supportsSparseTextures => SupportsSparseTextures();

	public static int supportedRenderTargetCount => SupportedRenderTargetCount();

	public static bool supportsSeparatedRenderTargetsBlend => SupportsSeparatedRenderTargetsBlend();

	public static int supportedRandomWriteTargetCount => SupportedRandomWriteTargetCount();

	public static int supportsMultisampledTextures => SupportsMultisampledTextures();

	public static bool supportsMultisampled2DArrayTextures => SupportsMultisampled2DArrayTextures();

	public static bool supportsMultisampledBackBuffer => SupportsMultisampledBackBuffer();

	public static bool supportsMemorylessTextures => SupportsMemorylessTextures();

	public static bool supportsMultisampleAutoResolve => SupportsMultisampleAutoResolve();

	public static bool supportsMultisampledShaderResolve => SupportsMultisampledShaderResolve();

	public static int supportsTextureWrapMirrorOnce => SupportsTextureWrapMirrorOnce();

	public static bool usesReversedZBuffer => UsesReversedZBuffer();

	[Obsolete("supportsStencil always returns true, no need to call it", true)]
	public static int supportsStencil => 1;

	public static bool supportsVariableRateShading => SupportsVariableRateShading();

	public static int maxTiledPixelStorageSize => MaxTiledPixelStorageSize();

	public static bool supportsDynamicResolution => SupportsDynamicResolution();

	public static NPOTSupport npotSupport => GetNPOTSupport();

	public static int maxTextureSize => GetMaxTextureSize();

	public static int maxTexture3DSize => GetMaxTexture3DSize();

	public static int maxTextureArraySlices => GetMaxTextureArraySlices();

	public static int maxCubemapSize => GetMaxCubemapSize();

	public static int maxAnisotropyLevel => GetMaxAnisotropyLevel();

	internal static int maxRenderTextureSize
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		get
		{
			return GetMaxRenderTextureSize();
		}
	}

	public static int maxComputeBufferInputsVertex => MaxComputeBufferInputsVertex();

	public static int maxComputeBufferInputsFragment => MaxComputeBufferInputsFragment();

	public static int maxComputeBufferInputsGeometry => MaxComputeBufferInputsGeometry();

	public static int maxComputeBufferInputsDomain => MaxComputeBufferInputsDomain();

	public static int maxComputeBufferInputsHull => MaxComputeBufferInputsHull();

	public static int maxComputeBufferInputsCompute => MaxComputeBufferInputsCompute();

	public static int maxComputeWorkGroupSize => GetMaxComputeWorkGroupSize();

	public static int maxComputeWorkGroupSizeX => GetMaxComputeWorkGroupSizeX();

	public static int maxComputeWorkGroupSizeY => GetMaxComputeWorkGroupSizeY();

	public static int maxComputeWorkGroupSizeZ => GetMaxComputeWorkGroupSizeZ();

	public static int computeSubGroupSize => GetComputeSubGroupSize();

	public static bool supportsAsyncCompute => SupportsAsyncCompute();

	public static bool supportsGpuRecorder => SupportsGpuRecorder();

	public static bool supportsGraphicsFence => SupportsGPUFence();

	public static bool supportsAsyncGPUReadback => SupportsAsyncGPUReadback();

	public static bool supportsParallelPSOCreation => SupportsParallelPSOCreation();

	public static bool supportsRayTracingShaders => SupportsRayTracingShaders();

	public static bool supportsRayTracing => SupportsRayTracing();

	public static bool supportsInlineRayTracing => SupportsInlineRayTracing();

	public static bool supportsIndirectDispatchRays => SupportsIndirectDispatchRays();

	public static bool supportsMachineLearning => SupportsMachineLearning();

	public static bool supportsSetConstantBuffer => SupportsSetConstantBuffer();

	public static int constantBufferOffsetAlignment => MinConstantBufferOffsetAlignment();

	public static int maxConstantBufferSize => MaxConstantBufferSize();

	public static long maxGraphicsBufferSize => MaxGraphicsBufferSize();

	[Obsolete("Use SystemInfo.constantBufferOffsetAlignment instead.", true)]
	public static bool minConstantBufferOffsetAlignment => false;

	public static bool hasMipMaxLevel => HasMipMaxLevel();

	public static bool supportsMipStreaming => SupportsMipStreaming();

	[Obsolete("graphicsPixelFillrate is no longer supported in Unity 5.0+.")]
	public static int graphicsPixelFillrate => -1;

	public static bool usesLoadStoreActions => UsesLoadStoreActions();

	public static HDRDisplaySupportFlags hdrDisplaySupportFlags => GetHDRDisplaySupportFlags();

	public static bool supportsConservativeRaster => SupportsConservativeRaster();

	public static bool supportsMultiview => SupportsMultiview();

	public static bool supportsStoreAndResolveAction => SupportsStoreAndResolveAction();

	public static bool supportsMultisampleResolveDepth => SupportsMultisampleResolveDepth();

	public static bool supportsMultisampleResolveStencil => SupportsMultisampleResolveStencil();

	public static bool supportsIndirectArgumentsBuffer => SupportsIndirectArgumentsBuffer();

	public static bool supportsDepthFetchInRenderPass => SupportsDepthFetchInRenderPass();

	[Obsolete("Vertex program support is required in Unity 5.0+", true)]
	public static bool supportsVertexPrograms => true;

	[Obsolete("SystemInfo.supportsGPUFence has been deprecated, use SystemInfo.supportsGraphicsFence instead (UnityUpgradable) ->  supportsGraphicsFence", true)]
	public static bool supportsGPUFence => false;

	private static bool IsValidEnumValue(Enum value)
	{
		if (!Enum.IsDefined(value.GetType(), value))
		{
			return false;
		}
		return true;
	}

	public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
	{
		if (!IsValidEnumValue(format))
		{
			throw new ArgumentException("Failed SupportsRenderTextureFormat; format is not a valid RenderTextureFormat");
		}
		return HasRenderTextureNative(format);
	}

	public static bool SupportsBlendingOnRenderTextureFormat(RenderTextureFormat format)
	{
		if (!IsValidEnumValue(format))
		{
			throw new ArgumentException("Failed SupportsBlendingOnRenderTextureFormat; format is not a valid RenderTextureFormat");
		}
		return SupportsBlendingOnRenderTextureFormatNative(format);
	}

	public static bool SupportsRandomWriteOnRenderTextureFormat(RenderTextureFormat format)
	{
		if (!IsValidEnumValue(format))
		{
			throw new ArgumentException("Failed SupportsRandomWriteOnRenderTextureFormat; format is not a valid RenderTextureFormat");
		}
		return SupportsRandomWriteOnRenderTextureFormatNative(format);
	}

	public static bool SupportsTextureFormat(TextureFormat format)
	{
		if (!IsValidEnumValue(format))
		{
			throw new ArgumentException("Failed SupportsTextureFormat; format is not a valid TextureFormat");
		}
		return SupportsTextureFormatNative(format);
	}

	public static bool SupportsVertexAttributeFormat(VertexAttributeFormat format, int dimension)
	{
		if (!IsValidEnumValue(format))
		{
			throw new ArgumentException("Failed SupportsVertexAttributeFormat; format is not a valid VertexAttributeFormat");
		}
		if (dimension < 1 || dimension > 4)
		{
			throw new ArgumentException("Failed SupportsVertexAttributeFormat; dimension must be in 1..4 range");
		}
		return SupportsVertexAttributeFormatNative(format, dimension);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetBatteryLevel")]
	private static extern float GetBatteryLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetBatteryStatus")]
	private static extern BatteryStatus GetBatteryStatus();

	[FreeFunction("systeminfo::GetOperatingSystem")]
	private static string GetOperatingSystem()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetOperatingSystem_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetOperatingSystemFamily")]
	private static extern OperatingSystemFamily GetOperatingSystemFamily();

	[FreeFunction("systeminfo::GetProcessorType")]
	private static string GetProcessorType()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetProcessorType_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("systeminfo::GetProcessorModel")]
	private static string GetProcessorModel()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetProcessorModel_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("systeminfo::GetProcessorManufacturer")]
	private static string GetProcessorManufacturer()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetProcessorManufacturer_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetProcessorFrequencyMHz")]
	private static extern int GetProcessorFrequencyMHz();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetProcessorCount")]
	private static extern int GetProcessorCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetPhysicalMemoryMB")]
	private static extern int GetPhysicalMemoryMB();

	[FreeFunction("systeminfo::GetDeviceUniqueIdentifier")]
	private static string GetDeviceUniqueIdentifier()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetDeviceUniqueIdentifier_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("systeminfo::GetDeviceName")]
	private static string GetDeviceName()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetDeviceName_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("systeminfo::GetDeviceModel")]
	private static string GetDeviceModel()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetDeviceModel_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsAccelerometer")]
	private static extern bool SupportsAccelerometer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	private static extern bool IsGyroAvailable();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsLocationService")]
	private static extern bool SupportsLocationService();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsVibration")]
	private static extern bool SupportsVibration();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsAudio")]
	private static extern bool SupportsAudio();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::SupportsRendering")]
	private static extern bool SupportsRendering();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("systeminfo::GetDeviceType")]
	private static extern DeviceType GetDeviceType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsMemorySize")]
	private static extern int GetGraphicsMemorySize();

	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceName")]
	private static string GetGraphicsDeviceName()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetGraphicsDeviceName_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVendor")]
	private static string GetGraphicsDeviceVendor()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetGraphicsDeviceVendor_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceID")]
	private static extern int GetGraphicsDeviceID();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVendorID")]
	private static extern int GetGraphicsDeviceVendorID();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceType")]
	private static extern GraphicsDeviceType GetGraphicsDeviceType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsUVStartsAtTop")]
	private static extern bool GetGraphicsUVStartsAtTop();

	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsDeviceVersion")]
	private static string GetGraphicsDeviceVersion()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetGraphicsDeviceVersion_Injected(out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsShaderLevel")]
	private static extern int GetGraphicsShaderLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsMultiThreaded")]
	private static extern bool GetGraphicsMultiThreaded();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::HasTiledGPU")]
	private static extern bool HasTiledGPU();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetRenderingThreadingMode")]
	private static extern RenderingThreadingMode GetRenderingThreadingMode();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetFoveatedRenderingCaps")]
	private static extern FoveatedRenderingCaps GetFoveatedRenderingCaps();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::HasHiddenSurfaceRemovalOnGPU")]
	private static extern bool HasHiddenSurfaceRemovalOnGPU();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::HasDynamicUniformArrayIndexingInFragmentShaders")]
	private static extern bool HasDynamicUniformArrayIndexingInFragmentShaders();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsShadows")]
	private static extern bool SupportsShadows();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsRawShadowDepthSampling")]
	private static extern bool SupportsRawShadowDepthSampling();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("SupportsMotionVectors")]
	private static extern bool SupportsMotionVectors();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::Supports3DTextures")]
	private static extern bool Supports3DTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsCompressed3DTextures")]
	private static extern bool SupportsCompressed3DTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::Supports2DArrayTextures")]
	private static extern bool Supports2DArrayTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::Supports3DRenderTextures")]
	private static extern bool Supports3DRenderTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsCubemapArrayTextures")]
	private static extern bool SupportsCubemapArrayTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsAnisotropicFilter")]
	private static extern bool SupportsAnisotropicFilter();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetCopyTextureSupport")]
	private static extern CopyTextureSupport GetCopyTextureSupport();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsComputeShaders")]
	private static extern bool SupportsComputeShaders();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsGeometryShaders")]
	private static extern bool SupportsGeometryShaders();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsTessellationShaders")]
	private static extern bool SupportsTessellationShaders();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsRenderTargetArrayIndexFromVertexShader")]
	private static extern bool SupportsRenderTargetArrayIndexFromVertexShader();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsInstancing")]
	private static extern bool SupportsInstancing();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsHardwareQuadTopology")]
	private static extern bool SupportsHardwareQuadTopology();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::Supports32bitsIndexBuffer")]
	private static extern bool Supports32bitsIndexBuffer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsSparseTextures")]
	private static extern bool SupportsSparseTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportedRenderTargetCount")]
	private static extern int SupportedRenderTargetCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsSeparatedRenderTargetsBlend")]
	private static extern bool SupportsSeparatedRenderTargetsBlend();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportedRandomWriteTargetCount")]
	private static extern int SupportedRandomWriteTargetCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxComputeBufferInputsVertex")]
	private static extern int MaxComputeBufferInputsVertex();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxComputeBufferInputsFragment")]
	private static extern int MaxComputeBufferInputsFragment();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxComputeBufferInputsGeometry")]
	private static extern int MaxComputeBufferInputsGeometry();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxComputeBufferInputsDomain")]
	private static extern int MaxComputeBufferInputsDomain();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxComputeBufferInputsHull")]
	private static extern int MaxComputeBufferInputsHull();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxComputeBufferInputsCompute")]
	private static extern int MaxComputeBufferInputsCompute();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampledTextures")]
	private static extern int SupportsMultisampledTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampled2DArrayTextures")]
	private static extern bool SupportsMultisampled2DArrayTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampledBackBuffer")]
	private static extern bool SupportsMultisampledBackBuffer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMemorylessTextures")]
	private static extern bool SupportsMemorylessTextures();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampleAutoResolve")]
	private static extern bool SupportsMultisampleAutoResolve();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampledShaderResolve")]
	private static extern bool SupportsMultisampledShaderResolve();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsTextureWrapMirrorOnce")]
	private static extern int SupportsTextureWrapMirrorOnce();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::UsesReversedZBuffer")]
	private static extern bool UsesReversedZBuffer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::HasRenderTexture")]
	private static extern bool HasRenderTextureNative(RenderTextureFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsBlendingOnRenderTextureFormat")]
	private static extern bool SupportsBlendingOnRenderTextureFormatNative(RenderTextureFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsRandomWriteOnRenderTextureFormat")]
	private static extern bool SupportsRandomWriteOnRenderTextureFormatNative(RenderTextureFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsTextureFormat")]
	private static extern bool SupportsTextureFormatNative(TextureFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsVertexAttributeFormat")]
	private static extern bool SupportsVertexAttributeFormatNative(VertexAttributeFormat format, int dimension);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetNPOTSupport")]
	private static extern NPOTSupport GetNPOTSupport();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxTextureSize")]
	private static extern int GetMaxTextureSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxTexture3DSize")]
	private static extern int GetMaxTexture3DSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxTextureArraySlices")]
	private static extern int GetMaxTextureArraySlices();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxCubemapSize")]
	private static extern int GetMaxCubemapSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxAnisotropyLevel")]
	private static extern int GetMaxAnisotropyLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxRenderTextureSize")]
	private static extern int GetMaxRenderTextureSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxComputeWorkGroupSize")]
	private static extern int GetMaxComputeWorkGroupSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxComputeWorkGroupSizeX")]
	private static extern int GetMaxComputeWorkGroupSizeX();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxComputeWorkGroupSizeY")]
	private static extern int GetMaxComputeWorkGroupSizeY();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetMaxComputeWorkGroupSizeZ")]
	private static extern int GetMaxComputeWorkGroupSizeZ();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetComputeSubGroupSize")]
	private static extern int GetComputeSubGroupSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsAsyncCompute")]
	private static extern bool SupportsAsyncCompute();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsGpuRecorder")]
	private static extern bool SupportsGpuRecorder();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsGPUFence")]
	private static extern bool SupportsGPUFence();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsAsyncGPUReadback")]
	private static extern bool SupportsAsyncGPUReadback();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsParallelPSOCreation")]
	private static extern bool SupportsParallelPSOCreation();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsRayTracing")]
	private static extern bool SupportsRayTracing();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsRayTracingShaders")]
	private static extern bool SupportsRayTracingShaders();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsInlineRayTracing")]
	private static extern bool SupportsInlineRayTracing();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsIndirectDispatchRays")]
	private static extern bool SupportsIndirectDispatchRays();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMachineLearning")]
	private static extern bool SupportsMachineLearning();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsSetConstantBuffer")]
	private static extern bool SupportsSetConstantBuffer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MinConstantBufferOffsetAlignment")]
	private static extern int MinConstantBufferOffsetAlignment();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxConstantBufferSize")]
	private static extern int MaxConstantBufferSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxGraphicsBufferSize")]
	private static extern long MaxGraphicsBufferSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::HasMipMaxLevel")]
	private static extern bool HasMipMaxLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMipStreaming")]
	private static extern bool SupportsMipStreaming();

	[Obsolete("Use overload with a GraphicsFormatUsage parameter instead", false)]
	public static bool IsFormatSupported(GraphicsFormat format, FormatUsage usage)
	{
		GraphicsFormatUsage usage2 = (GraphicsFormatUsage)(1 << (int)usage);
		return IsFormatSupported(format, usage2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::IsFormatSupported")]
	public static extern bool IsFormatSupported(GraphicsFormat format, GraphicsFormatUsage usage);

	[Obsolete("Use overload with a GraphicsFormatUsage parameter instead", false)]
	public static GraphicsFormat GetCompatibleFormat(GraphicsFormat format, FormatUsage usage)
	{
		GraphicsFormatUsage usage2 = (GraphicsFormatUsage)(1 << (int)usage);
		return GetCompatibleFormat(format, usage2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetCompatibleFormat")]
	public static extern GraphicsFormat GetCompatibleFormat(GraphicsFormat format, GraphicsFormatUsage usage);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetGraphicsFormat")]
	public static extern GraphicsFormat GetGraphicsFormat(DefaultFormat format);

	[FreeFunction("ScriptingGraphicsCaps::GetRenderTextureSupportedMSAASampleCount")]
	public static int GetRenderTextureSupportedMSAASampleCount(RenderTextureDescriptor desc)
	{
		return GetRenderTextureSupportedMSAASampleCount_Injected(ref desc);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetTiledRenderTargetStorageSize")]
	public static extern int GetTiledRenderTargetStorageSize(GraphicsFormat format, int sampleCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::UsesLoadStoreActions")]
	private static extern bool UsesLoadStoreActions();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::GetHDRDisplaySupportFlags")]
	private static extern HDRDisplaySupportFlags GetHDRDisplaySupportFlags();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsConservativeRaster")]
	private static extern bool SupportsConservativeRaster();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultiview")]
	private static extern bool SupportsMultiview();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsStoreAndResolveAction")]
	private static extern bool SupportsStoreAndResolveAction();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampleResolveDepth")]
	private static extern bool SupportsMultisampleResolveDepth();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsMultisampleResolveStencil")]
	private static extern bool SupportsMultisampleResolveStencil();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsIndirectArgumentsBuffer")]
	private static extern bool SupportsIndirectArgumentsBuffer();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsDepthFetchInRenderPass")]
	private static extern bool SupportsDepthFetchInRenderPass();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsVariableRateShading")]
	private static extern bool SupportsVariableRateShading();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::MaxTiledPixelStorageSize")]
	private static extern int MaxTiledPixelStorageSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ScriptingGraphicsCaps::SupportsDynamicResolution")]
	private static extern bool SupportsDynamicResolution();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetOperatingSystem_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetProcessorType_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetProcessorModel_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetProcessorManufacturer_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDeviceUniqueIdentifier_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDeviceName_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDeviceModel_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGraphicsDeviceName_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGraphicsDeviceVendor_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGraphicsDeviceVersion_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRenderTextureSupportedMSAASampleCount_Injected([In] ref RenderTextureDescriptor desc);
}
