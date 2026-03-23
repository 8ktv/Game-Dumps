using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering;

[Serializable]
public struct GlobalDynamicResolutionSettings
{
	public bool enabled;

	public bool useMipBias;

	public List<AdvancedUpscalers> advancedUpscalersByPriority;

	public uint DLSSPerfQualitySetting;

	public DynamicResolutionHandler.UpsamplerScheduleType DLSSInjectionPoint;

	public DynamicResolutionHandler.UpsamplerScheduleType TAAUInjectionPoint;

	public DynamicResolutionHandler.UpsamplerScheduleType STPInjectionPoint;

	public DynamicResolutionHandler.UpsamplerScheduleType defaultInjectionPoint;

	public bool DLSSUseOptimalSettings;

	[Range(0f, 1f)]
	public float DLSSSharpness;

	public bool FSR2EnableSharpness;

	[Range(0f, 1f)]
	public float FSR2Sharpness;

	public bool FSR2UseOptimalSettings;

	public uint FSR2QualitySetting;

	public DynamicResolutionHandler.UpsamplerScheduleType FSR2InjectionPoint;

	public bool fsrOverrideSharpness;

	[Range(0f, 1f)]
	public float fsrSharpness;

	public float maxPercentage;

	public float minPercentage;

	public DynamicResolutionType dynResType;

	public DynamicResUpscaleFilter upsampleFilter;

	public bool forceResolution;

	public float forcedPercentage;

	public float lowResTransparencyMinimumThreshold;

	public float rayTracingHalfResThreshold;

	public float lowResSSGIMinimumThreshold;

	public float lowResVolumetricCloudsMinimumThreshold;

	[Obsolete("Obsolete, used only for data migration. Use the advancedUpscalersByPriority list instead to add the proper supported advanced upscaler by priority.")]
	public bool enableDLSS;

	public static GlobalDynamicResolutionSettings NewDefault()
	{
		return new GlobalDynamicResolutionSettings
		{
			useMipBias = false,
			maxPercentage = 100f,
			minPercentage = 100f,
			dynResType = DynamicResolutionType.Hardware,
			upsampleFilter = DynamicResUpscaleFilter.CatmullRom,
			forcedPercentage = 100f,
			lowResTransparencyMinimumThreshold = 0f,
			lowResVolumetricCloudsMinimumThreshold = 50f,
			rayTracingHalfResThreshold = 50f,
			DLSSUseOptimalSettings = true,
			DLSSPerfQualitySetting = 0u,
			DLSSSharpness = 0.5f,
			DLSSInjectionPoint = DynamicResolutionHandler.UpsamplerScheduleType.BeforePost,
			FSR2InjectionPoint = DynamicResolutionHandler.UpsamplerScheduleType.BeforePost,
			TAAUInjectionPoint = DynamicResolutionHandler.UpsamplerScheduleType.BeforePost,
			defaultInjectionPoint = DynamicResolutionHandler.UpsamplerScheduleType.AfterPost,
			advancedUpscalersByPriority = new List<AdvancedUpscalers> { AdvancedUpscalers.STP },
			fsrOverrideSharpness = false,
			fsrSharpness = 0.92f
		};
	}
}
