using System;
using System.Collections.Generic;

namespace UnityEngine.AdaptivePerformance;

[Serializable]
public class AdaptivePerformanceScalerSettings
{
	[Tooltip("Settings for a scaler used by the Indexer to adjust the application update rate using Application.TargetFramerate")]
	[SerializeField]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveFramerate = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Framerate",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.High,
		target = (ScalerTarget.CPU | ScalerTarget.GPU | ScalerTarget.FillRate),
		minBound = 15f,
		maxBound = 60f,
		maxLevel = 45
	};

	[Tooltip("Settings for a scaler used by the Indexer to adjust the resolution of all render targets that allow dynamic resolution.")]
	[SerializeField]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveResolution = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Resolution",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Low,
		target = (ScalerTarget.GPU | ScalerTarget.FillRate),
		maxLevel = 9,
		minBound = 0.5f,
		maxBound = 1f
	};

	[Tooltip("Settings for a scaler used by the Indexer to control if dynamic batching is enabled.")]
	[SerializeField]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveBatching = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Batching",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Medium,
		target = ScalerTarget.CPU,
		maxLevel = 1,
		minBound = 0f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer for adjusting at what distance LODs are switched.")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveLOD = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive LOD",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.High,
		target = ScalerTarget.GPU,
		maxLevel = 3,
		minBound = 0.4f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to adjust the size of the palette used for color grading in URP.")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveLut = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Lut",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Medium,
		target = (ScalerTarget.CPU | ScalerTarget.GPU),
		maxLevel = 1,
		minBound = 0f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to adjust the level of antialiasing.")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveMSAA = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive MSAA",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Medium,
		target = (ScalerTarget.GPU | ScalerTarget.FillRate),
		maxLevel = 2,
		minBound = 0f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to adjust the number of shadow cascades to be used.")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowCascade = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Shadow Cascade",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Medium,
		target = (ScalerTarget.CPU | ScalerTarget.GPU),
		maxLevel = 2,
		minBound = 0f,
		maxBound = 1f
	};

	private const string obsoleteMsg = "AdaptiveShadowCascades has been renamed. Please use AdaptiveShadowCascade. (UnityUpgradable) -> AdaptiveShadowCascade";

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to change the distance at which shadows are rendered.")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowDistance = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Shadow Distance",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Low,
		target = ScalerTarget.GPU,
		maxLevel = 3,
		minBound = 0.15f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to adjust the resolution of shadow maps.")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowmapResolution = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Shadowmap Resolution",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Low,
		target = ScalerTarget.GPU,
		maxLevel = 3,
		minBound = 0.15f,
		maxBound = 1f
	};

	[Tooltip("Settings for a scaler used by the Indexer to adjust the quality of shadows.")]
	[SerializeField]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveShadowQuality = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Shadow Quality",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.High,
		target = (ScalerTarget.CPU | ScalerTarget.GPU),
		maxLevel = 3,
		minBound = 0f,
		maxBound = 1f
	};

	[Tooltip("Settings for a scaler used by the Indexer to change if objects in the scene are sorted by depth before rendering to reduce overdraw.")]
	[SerializeField]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveSorting = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Sorting",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Medium,
		target = ScalerTarget.CPU,
		maxLevel = 1,
		minBound = 0f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to disable transparent objects rendering")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveTransparency = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Transparency",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.High,
		target = ScalerTarget.GPU,
		maxLevel = 1,
		minBound = 0f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to change the view distance")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveViewDistance = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive View Distance",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.High,
		target = ScalerTarget.GPU,
		maxLevel = 40,
		minBound = 50f,
		maxBound = 1000f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to change physics properties")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptivePhysics = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Physics",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Low,
		target = ScalerTarget.CPU,
		maxLevel = 5,
		minBound = 0.5f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to change decal properties")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveDecals = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Decals",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Medium,
		target = ScalerTarget.GPU,
		maxLevel = 20,
		minBound = 0.01f,
		maxBound = 1f
	};

	[SerializeField]
	[Tooltip("Settings for a scaler used by the Indexer to change the layer culling distance")]
	private AdaptivePerformanceScalerSettingsBase m_AdaptiveLayerCulling = new AdaptivePerformanceScalerSettingsBase
	{
		name = "Adaptive Layer Culling",
		enabled = false,
		scale = 1f,
		visualImpact = ScalerVisualImpact.Medium,
		target = ScalerTarget.CPU,
		maxLevel = 40,
		minBound = 0.01f,
		maxBound = 1f
	};

	private List<AdaptivePerformanceScalerSettingsBase> m_DefaultScalerSettings = null;

	internal static readonly List<Type> k_DefaultScalerNames = new List<Type>
	{
		typeof(AdaptiveFramerate),
		typeof(AdaptiveBatching),
		typeof(AdaptiveLOD),
		typeof(AdaptiveLut),
		typeof(AdaptiveMSAA),
		typeof(AdaptiveResolution),
		typeof(AdaptiveShadowCascade),
		typeof(AdaptiveShadowDistance),
		typeof(AdaptiveShadowmapResolution),
		typeof(AdaptiveShadowQuality),
		typeof(AdaptiveTransparency),
		typeof(AdaptiveSorting),
		typeof(AdaptiveViewDistance),
		typeof(AdaptivePhysics),
		typeof(AdaptiveLayerCulling),
		typeof(AdaptiveDecals)
	};

	public AdaptivePerformanceScalerSettingsBase AdaptiveFramerate
	{
		get
		{
			return m_AdaptiveFramerate;
		}
		set
		{
			m_AdaptiveFramerate = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveResolution
	{
		get
		{
			return m_AdaptiveResolution;
		}
		set
		{
			m_AdaptiveResolution = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveBatching
	{
		get
		{
			return m_AdaptiveBatching;
		}
		set
		{
			m_AdaptiveBatching = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveLOD
	{
		get
		{
			return m_AdaptiveLOD;
		}
		set
		{
			m_AdaptiveLOD = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveLut
	{
		get
		{
			return m_AdaptiveLut;
		}
		set
		{
			m_AdaptiveLut = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveMSAA
	{
		get
		{
			return m_AdaptiveMSAA;
		}
		set
		{
			m_AdaptiveMSAA = value;
		}
	}

	[Obsolete("AdaptiveShadowCascades has been renamed. Please use AdaptiveShadowCascade. (UnityUpgradable) -> AdaptiveShadowCascade", false)]
	public AdaptivePerformanceScalerSettingsBase AdaptiveShadowCascades => AdaptiveShadowCascade;

	public AdaptivePerformanceScalerSettingsBase AdaptiveShadowCascade
	{
		get
		{
			return m_AdaptiveShadowCascade;
		}
		set
		{
			m_AdaptiveShadowCascade = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveShadowDistance
	{
		get
		{
			return m_AdaptiveShadowDistance;
		}
		set
		{
			m_AdaptiveShadowDistance = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveShadowmapResolution
	{
		get
		{
			return m_AdaptiveShadowmapResolution;
		}
		set
		{
			m_AdaptiveShadowmapResolution = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveShadowQuality
	{
		get
		{
			return m_AdaptiveShadowQuality;
		}
		set
		{
			m_AdaptiveShadowQuality = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveSorting
	{
		get
		{
			return m_AdaptiveSorting;
		}
		set
		{
			m_AdaptiveSorting = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveTransparency
	{
		get
		{
			return m_AdaptiveTransparency;
		}
		set
		{
			m_AdaptiveTransparency = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveViewDistance
	{
		get
		{
			return m_AdaptiveViewDistance;
		}
		set
		{
			m_AdaptiveViewDistance = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptivePhysics
	{
		get
		{
			return m_AdaptivePhysics;
		}
		set
		{
			m_AdaptivePhysics = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveDecals
	{
		get
		{
			return m_AdaptiveDecals;
		}
		set
		{
			m_AdaptiveDecals = value;
		}
	}

	public AdaptivePerformanceScalerSettingsBase AdaptiveLayerCulling
	{
		get
		{
			return m_AdaptiveLayerCulling;
		}
		set
		{
			m_AdaptiveLayerCulling = value;
		}
	}

	public List<AdaptivePerformanceScalerSettingsBase> DefaultScalerSettings
	{
		get
		{
			if (m_DefaultScalerSettings == null || m_DefaultScalerSettings.Count == 0)
			{
				m_DefaultScalerSettings = new List<AdaptivePerformanceScalerSettingsBase>
				{
					AdaptiveFramerate, AdaptiveBatching, AdaptiveLOD, AdaptiveLut, AdaptiveMSAA, AdaptiveResolution, AdaptiveShadowCascade, AdaptiveShadowDistance, AdaptiveShadowmapResolution, AdaptiveShadowQuality,
					AdaptiveTransparency, AdaptiveSorting, AdaptiveViewDistance, AdaptivePhysics, AdaptiveLayerCulling, AdaptiveDecals
				};
			}
			return m_DefaultScalerSettings;
		}
	}

	public void ApplySettings(AdaptivePerformanceScalerSettings settings)
	{
		if (settings != null)
		{
			ApplySettingsBase(AdaptiveFramerate, settings.AdaptiveFramerate);
			ApplySettingsBase(AdaptiveBatching, settings.AdaptiveBatching);
			ApplySettingsBase(AdaptiveLOD, settings.AdaptiveLOD);
			ApplySettingsBase(AdaptiveLut, settings.AdaptiveLut);
			ApplySettingsBase(AdaptiveMSAA, settings.AdaptiveMSAA);
			ApplySettingsBase(AdaptiveResolution, settings.AdaptiveResolution);
			ApplySettingsBase(AdaptiveShadowCascade, settings.AdaptiveShadowCascade);
			ApplySettingsBase(AdaptiveShadowDistance, settings.AdaptiveShadowDistance);
			ApplySettingsBase(AdaptiveShadowmapResolution, settings.AdaptiveShadowmapResolution);
			ApplySettingsBase(AdaptiveShadowQuality, settings.AdaptiveShadowQuality);
			ApplySettingsBase(AdaptiveTransparency, settings.AdaptiveTransparency);
			ApplySettingsBase(AdaptiveSorting, settings.AdaptiveSorting);
			ApplySettingsBase(AdaptiveViewDistance, settings.AdaptiveViewDistance);
			ApplySettingsBase(AdaptivePhysics, settings.AdaptivePhysics);
			ApplySettingsBase(AdaptiveLayerCulling, settings.AdaptiveLayerCulling);
			ApplySettingsBase(AdaptiveDecals, settings.AdaptiveDecals);
		}
	}

	private void ApplySettingsBase(AdaptivePerformanceScalerSettingsBase destination, AdaptivePerformanceScalerSettingsBase sources)
	{
		destination.enabled = sources.enabled;
		destination.scale = sources.scale;
		destination.visualImpact = sources.visualImpact;
		destination.target = sources.target;
		destination.minBound = sources.minBound;
		destination.maxBound = sources.maxBound;
		destination.maxLevel = sources.maxLevel;
	}
}
