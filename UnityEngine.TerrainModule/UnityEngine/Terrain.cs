using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[StaticAccessor("GetITerrainManager()", StaticAccessorType.Arrow)]
[UsedByNativeCode]
[NativeHeader("Modules/Terrain/Public/Terrain.h")]
[NativeHeader("Runtime/Interfaces/ITerrainManager.h")]
[NativeHeader("TerrainScriptingClasses.h")]
public sealed class Terrain : Behaviour
{
	[Obsolete("Enum type MaterialType is not used any more.", false)]
	public enum MaterialType
	{
		BuiltInStandard,
		BuiltInLegacyDiffuse,
		BuiltInLegacySpecular,
		Custom
	}

	public TerrainData terrainData
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<TerrainData>(get_terrainData_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_terrainData_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public float treeDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_treeDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_treeDistance_Injected(intPtr, value);
		}
	}

	public float treeBillboardDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_treeBillboardDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_treeBillboardDistance_Injected(intPtr, value);
		}
	}

	public float treeCrossFadeLength
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_treeCrossFadeLength_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_treeCrossFadeLength_Injected(intPtr, value);
		}
	}

	public int treeMaximumFullLODCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_treeMaximumFullLODCount_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_treeMaximumFullLODCount_Injected(intPtr, value);
		}
	}

	public float detailObjectDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailObjectDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_detailObjectDistance_Injected(intPtr, value);
		}
	}

	public float detailObjectDensity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detailObjectDensity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_detailObjectDensity_Injected(intPtr, value);
		}
	}

	public float heightmapPixelError
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_heightmapPixelError_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_heightmapPixelError_Injected(intPtr, value);
		}
	}

	public int heightmapMaximumLOD
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_heightmapMaximumLOD_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_heightmapMaximumLOD_Injected(intPtr, value);
		}
	}

	public int heightmapMinimumLODSimplification
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_heightmapMinimumLODSimplification_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_heightmapMinimumLODSimplification_Injected(intPtr, value);
		}
	}

	public float basemapDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_basemapDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_basemapDistance_Injected(intPtr, value);
		}
	}

	[NativeProperty("StaticLightmapIndexInt")]
	public int lightmapIndex
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_lightmapIndex_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lightmapIndex_Injected(intPtr, value);
		}
	}

	[NativeProperty("DynamicLightmapIndexInt")]
	public int realtimeLightmapIndex
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_realtimeLightmapIndex_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_realtimeLightmapIndex_Injected(intPtr, value);
		}
	}

	[NativeProperty("StaticLightmapST")]
	public Vector4 lightmapScaleOffset
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_lightmapScaleOffset_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lightmapScaleOffset_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("DynamicLightmapST")]
	public Vector4 realtimeLightmapScaleOffset
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_realtimeLightmapScaleOffset_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_realtimeLightmapScaleOffset_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("FreeUnusedRenderingResourcesObsolete")]
	[Obsolete("Terrain.freeUnusedRenderingResources is obsolete; use keepUnusedRenderingResources instead.")]
	public bool freeUnusedRenderingResources
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_freeUnusedRenderingResources_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_freeUnusedRenderingResources_Injected(intPtr, value);
		}
	}

	[NativeProperty("KeepUnusedRenderingResources")]
	public bool keepUnusedRenderingResources
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_keepUnusedRenderingResources_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_keepUnusedRenderingResources_Injected(intPtr, value);
		}
	}

	public ShadowCastingMode shadowCastingMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowCastingMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowCastingMode_Injected(intPtr, value);
		}
	}

	public ReflectionProbeUsage reflectionProbeUsage
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_reflectionProbeUsage_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_reflectionProbeUsage_Injected(intPtr, value);
		}
	}

	public Material materialTemplate
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(get_materialTemplate_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_materialTemplate_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public bool drawHeightmap
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_drawHeightmap_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_drawHeightmap_Injected(intPtr, value);
		}
	}

	public bool allowAutoConnect
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowAutoConnect_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowAutoConnect_Injected(intPtr, value);
		}
	}

	public int groupingID
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_groupingID_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_groupingID_Injected(intPtr, value);
		}
	}

	public bool drawInstanced
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_drawInstanced_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_drawInstanced_Injected(intPtr, value);
		}
	}

	public bool enableHeightmapRayTracing
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enableHeightmapRayTracing_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enableHeightmapRayTracing_Injected(intPtr, value);
		}
	}

	public RenderTexture normalmapTexture
	{
		[NativeMethod("TryGetNormalMapTexture")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(get_normalmapTexture_Injected(intPtr));
		}
	}

	public bool drawTreesAndFoliage
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_drawTreesAndFoliage_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_drawTreesAndFoliage_Injected(intPtr, value);
		}
	}

	public Vector3 patchBoundsMultiplier
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_patchBoundsMultiplier_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_patchBoundsMultiplier_Injected(intPtr, ref value);
		}
	}

	public float treeLODBiasMultiplier
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_treeLODBiasMultiplier_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_treeLODBiasMultiplier_Injected(intPtr, value);
		}
	}

	public bool collectDetailPatches
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_collectDetailPatches_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_collectDetailPatches_Injected(intPtr, value);
		}
	}

	public bool ignoreQualitySettings
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_ignoreQualitySettings_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_ignoreQualitySettings_Injected(intPtr, value);
		}
	}

	public TerrainRenderFlags editorRenderFlags
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_editorRenderFlags_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_editorRenderFlags_Injected(intPtr, value);
		}
	}

	public TreeMotionVectorModeOverride treeMotionVectorModeOverride
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_treeMotionVectorModeOverride_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_treeMotionVectorModeOverride_Injected(intPtr, value);
		}
	}

	public bool preserveTreePrototypeLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_preserveTreePrototypeLayers_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_preserveTreePrototypeLayers_Injected(intPtr, value);
		}
	}

	[StaticAccessor("Terrain", StaticAccessorType.DoubleColon)]
	public static extern GraphicsFormat heightmapFormat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static TextureFormat heightmapTextureFormat => GraphicsFormatUtility.GetTextureFormat(heightmapFormat);

	public static RenderTextureFormat heightmapRenderTextureFormat => GraphicsFormatUtility.GetRenderTextureFormat(heightmapFormat);

	[StaticAccessor("Terrain", StaticAccessorType.DoubleColon)]
	public static extern GraphicsFormat normalmapFormat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static TextureFormat normalmapTextureFormat => GraphicsFormatUtility.GetTextureFormat(normalmapFormat);

	public static RenderTextureFormat normalmapRenderTextureFormat => GraphicsFormatUtility.GetRenderTextureFormat(normalmapFormat);

	[StaticAccessor("Terrain", StaticAccessorType.DoubleColon)]
	public static extern GraphicsFormat holesFormat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static RenderTextureFormat holesRenderTextureFormat => GraphicsFormatUtility.GetRenderTextureFormat(holesFormat);

	[StaticAccessor("Terrain", StaticAccessorType.DoubleColon)]
	public static extern GraphicsFormat compressedHolesFormat
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static TextureFormat compressedHolesTextureFormat => GraphicsFormatUtility.GetTextureFormat(compressedHolesFormat);

	public static Terrain activeTerrain => Unmarshal.UnmarshalUnityObject<Terrain>(get_activeTerrain_Injected());

	[NativeProperty("ActiveTerrainsScriptingArray")]
	public static extern Terrain[] activeTerrains
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		get;
	}

	public Terrain leftNeighbor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Terrain>(get_leftNeighbor_Injected(intPtr));
		}
	}

	public Terrain rightNeighbor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Terrain>(get_rightNeighbor_Injected(intPtr));
		}
	}

	public Terrain topNeighbor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Terrain>(get_topNeighbor_Injected(intPtr));
		}
	}

	public Terrain bottomNeighbor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Terrain>(get_bottomNeighbor_Injected(intPtr));
		}
	}

	public uint renderingLayerMask
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

	[Obsolete("splatmapDistance is deprecated, please use basemapDistance instead. (UnityUpgradable) -> basemapDistance", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public float splatmapDistance
	{
		get
		{
			return basemapDistance;
		}
		set
		{
			basemapDistance = value;
		}
	}

	[Obsolete("castShadows is deprecated, please use shadowCastingMode instead.")]
	public bool castShadows
	{
		get
		{
			return shadowCastingMode != ShadowCastingMode.Off;
		}
		set
		{
			shadowCastingMode = (value ? ShadowCastingMode.TwoSided : ShadowCastingMode.Off);
		}
	}

	[Obsolete("Property materialType is not used any more. Set materialTemplate directly.", false)]
	public MaterialType materialType
	{
		get
		{
			return MaterialType.Custom;
		}
		set
		{
		}
	}

	[Obsolete("Property legacySpecular is not used any more. Set materialTemplate directly.", false)]
	public Color legacySpecular
	{
		get
		{
			return Color.gray;
		}
		set
		{
		}
	}

	[Obsolete("Property legacyShininess is not used any more. Set materialTemplate directly.", false)]
	public float legacyShininess
	{
		get
		{
			return 5f / 64f;
		}
		set
		{
		}
	}

	public bool GetKeepUnusedCameraRenderingResources(int cameraInstanceID)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetKeepUnusedCameraRenderingResources_Injected(intPtr, cameraInstanceID);
	}

	public void SetKeepUnusedCameraRenderingResources(int cameraInstanceID, bool keepUnused)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetKeepUnusedCameraRenderingResources_Injected(intPtr, cameraInstanceID, keepUnused);
	}

	public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetClosestReflectionProbes_Injected(intPtr, result);
	}

	public float SampleHeight(Vector3 worldPosition)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SampleHeight_Injected(intPtr, ref worldPosition);
	}

	public void AddTreeInstance(TreeInstance instance)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddTreeInstance_Injected(intPtr, ref instance);
	}

	public void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetNeighbors_Injected(intPtr, MarshalledUnityObject.Marshal(left), MarshalledUnityObject.Marshal(top), MarshalledUnityObject.Marshal(right), MarshalledUnityObject.Marshal(bottom));
	}

	public Vector3 GetPosition()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPosition_Injected(intPtr, out var ret);
		return ret;
	}

	public void Flush()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Flush_Injected(intPtr);
	}

	internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveTrees_Injected(intPtr, ref position, radius, prototypeIndex);
	}

	[NativeMethod("CopySplatMaterialCustomProps")]
	public void SetSplatMaterialPropertyBlock(MaterialPropertyBlock properties)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetSplatMaterialPropertyBlock_Injected(intPtr, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
	}

	public void GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest)
	{
		if (dest == null)
		{
			throw new ArgumentNullException("dest");
		}
		Internal_GetSplatMaterialPropertyBlock(dest);
	}

	[NativeMethod("GetSplatMaterialCustomProps")]
	private void Internal_GetSplatMaterialPropertyBlock(MaterialPropertyBlock dest)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetSplatMaterialPropertyBlock_Injected(intPtr, (dest == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(dest));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void SetConnectivityDirty();

	public static void GetActiveTerrains(List<Terrain> terrainList)
	{
		Internal_FillActiveTerrainList(terrainList);
	}

	private static void Internal_FillActiveTerrainList([NotNull] object terrainList)
	{
		if (terrainList == null)
		{
			ThrowHelper.ThrowArgumentNullException(terrainList, "terrainList");
		}
		Internal_FillActiveTerrainList_Injected(terrainList);
	}

	[UsedByNativeCode]
	public static GameObject CreateTerrainGameObject(TerrainData assignTerrain)
	{
		return Unmarshal.UnmarshalUnityObject<GameObject>(CreateTerrainGameObject_Injected(MarshalledUnityObject.Marshal(assignTerrain)));
	}

	[Obsolete("Use TerrainData.SyncHeightmap to notify all Terrain instances using the TerrainData.", false)]
	public void ApplyDelayedHeightmapModification()
	{
		terrainData?.SyncHeightmap();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_terrainData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_terrainData_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_treeDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_treeDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_treeBillboardDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_treeBillboardDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_treeCrossFadeLength_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_treeCrossFadeLength_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_treeMaximumFullLODCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_treeMaximumFullLODCount_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_detailObjectDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_detailObjectDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_detailObjectDensity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_detailObjectDensity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_heightmapPixelError_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_heightmapPixelError_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_heightmapMaximumLOD_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_heightmapMaximumLOD_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_heightmapMinimumLODSimplification_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_heightmapMinimumLODSimplification_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_basemapDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_basemapDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_lightmapIndex_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lightmapIndex_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_realtimeLightmapIndex_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_realtimeLightmapIndex_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_lightmapScaleOffset_Injected(IntPtr _unity_self, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lightmapScaleOffset_Injected(IntPtr _unity_self, [In] ref Vector4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_realtimeLightmapScaleOffset_Injected(IntPtr _unity_self, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_realtimeLightmapScaleOffset_Injected(IntPtr _unity_self, [In] ref Vector4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_freeUnusedRenderingResources_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_freeUnusedRenderingResources_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_keepUnusedRenderingResources_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_keepUnusedRenderingResources_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetKeepUnusedCameraRenderingResources_Injected(IntPtr _unity_self, int cameraInstanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetKeepUnusedCameraRenderingResources_Injected(IntPtr _unity_self, int cameraInstanceID, bool keepUnused);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ShadowCastingMode get_shadowCastingMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowCastingMode_Injected(IntPtr _unity_self, ShadowCastingMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ReflectionProbeUsage get_reflectionProbeUsage_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_reflectionProbeUsage_Injected(IntPtr _unity_self, ReflectionProbeUsage value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetClosestReflectionProbes_Injected(IntPtr _unity_self, List<ReflectionProbeBlendInfo> result);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_materialTemplate_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_materialTemplate_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_drawHeightmap_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_drawHeightmap_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowAutoConnect_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowAutoConnect_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_groupingID_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_groupingID_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_drawInstanced_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_drawInstanced_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enableHeightmapRayTracing_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enableHeightmapRayTracing_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_normalmapTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_drawTreesAndFoliage_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_drawTreesAndFoliage_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_patchBoundsMultiplier_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_patchBoundsMultiplier_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SampleHeight_Injected(IntPtr _unity_self, [In] ref Vector3 worldPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddTreeInstance_Injected(IntPtr _unity_self, [In] ref TreeInstance instance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetNeighbors_Injected(IntPtr _unity_self, IntPtr left, IntPtr top, IntPtr right, IntPtr bottom);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_treeLODBiasMultiplier_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_treeLODBiasMultiplier_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_collectDetailPatches_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_collectDetailPatches_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_ignoreQualitySettings_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_ignoreQualitySettings_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TerrainRenderFlags get_editorRenderFlags_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_editorRenderFlags_Injected(IntPtr _unity_self, TerrainRenderFlags value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPosition_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Flush_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveTrees_Injected(IntPtr _unity_self, [In] ref Vector2 position, float radius, int prototypeIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSplatMaterialPropertyBlock_Injected(IntPtr _unity_self, IntPtr properties);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetSplatMaterialPropertyBlock_Injected(IntPtr _unity_self, IntPtr dest);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TreeMotionVectorModeOverride get_treeMotionVectorModeOverride_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_treeMotionVectorModeOverride_Injected(IntPtr _unity_self, TreeMotionVectorModeOverride value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_preserveTreePrototypeLayers_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_preserveTreePrototypeLayers_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_activeTerrain_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_FillActiveTerrainList_Injected(object terrainList);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateTerrainGameObject_Injected(IntPtr assignTerrain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_leftNeighbor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_rightNeighbor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_topNeighbor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_bottomNeighbor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint get_renderingLayerMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderingLayerMask_Injected(IntPtr _unity_self, uint value);
}
