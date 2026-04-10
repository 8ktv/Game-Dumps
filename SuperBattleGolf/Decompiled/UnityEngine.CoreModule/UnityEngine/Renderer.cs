using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/Renderer.h")]
public class Renderer : Component
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use shadowCastingMode instead.", false)]
	public bool castShadows
	{
		get
		{
			return shadowCastingMode != ShadowCastingMode.Off;
		}
		set
		{
			shadowCastingMode = (value ? ShadowCastingMode.On : ShadowCastingMode.Off);
		}
	}

	[Obsolete("Use motionVectorGenerationMode instead.", false)]
	public bool motionVectors
	{
		get
		{
			return motionVectorGenerationMode == MotionVectorGenerationMode.Object;
		}
		set
		{
			motionVectorGenerationMode = (value ? MotionVectorGenerationMode.Object : MotionVectorGenerationMode.Camera);
		}
	}

	[Obsolete("Use lightProbeUsage instead.", false)]
	public bool useLightProbes
	{
		get
		{
			return lightProbeUsage != LightProbeUsage.Off;
		}
		set
		{
			lightProbeUsage = (value ? LightProbeUsage.BlendProbes : LightProbeUsage.Off);
		}
	}

	public Bounds bounds
	{
		[FreeFunction(Name = "RendererScripting::GetWorldBounds", HasExplicitThis = true)]
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
		[NativeName("SetWorldAABB")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bounds_Injected(intPtr, ref value);
		}
	}

	public Bounds localBounds
	{
		[FreeFunction(Name = "RendererScripting::GetLocalBounds", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localBounds_Injected(intPtr, out var ret);
			return ret;
		}
		[NativeName("SetLocalAABB")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_localBounds_Injected(intPtr, ref value);
		}
	}

	public bool enabled
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enabled_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enabled_Injected(intPtr, value);
		}
	}

	public bool isVisible
	{
		[NativeName("IsVisibleInScene")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isVisible_Injected(intPtr);
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

	public bool receiveShadows
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_receiveShadows_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_receiveShadows_Injected(intPtr, value);
		}
	}

	public bool forceRenderingOff
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_forceRenderingOff_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_forceRenderingOff_Injected(intPtr, value);
		}
	}

	internal bool allowGPUDrivenRendering
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowGPUDrivenRendering_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowGPUDrivenRendering_Injected(intPtr, value);
		}
	}

	internal bool smallMeshCulling
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_smallMeshCulling_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_smallMeshCulling_Injected(intPtr, value);
		}
	}

	public bool staticShadowCaster
	{
		get
		{
			return GetIsStaticShadowCaster();
		}
		set
		{
			SetIsStaticShadowCaster(value);
		}
	}

	public MotionVectorGenerationMode motionVectorGenerationMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_motionVectorGenerationMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_motionVectorGenerationMode_Injected(intPtr, value);
		}
	}

	public LightProbeUsage lightProbeUsage
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_lightProbeUsage_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lightProbeUsage_Injected(intPtr, value);
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

	public int rendererPriority
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rendererPriority_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rendererPriority_Injected(intPtr, value);
		}
	}

	public RayTracingMode rayTracingMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rayTracingMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rayTracingMode_Injected(intPtr, value);
		}
	}

	public RayTracingAccelerationStructureBuildFlags rayTracingAccelerationStructureBuildFlags
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rayTracingAccelerationStructureBuildFlags_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rayTracingAccelerationStructureBuildFlags_Injected(intPtr, value);
		}
	}

	public bool rayTracingAccelerationStructureBuildFlagsOverride
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rayTracingAccelerationStructureBuildFlagsOverride_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rayTracingAccelerationStructureBuildFlagsOverride_Injected(intPtr, value);
		}
	}

	public unsafe string sortingLayerName
	{
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_sortingLayerName_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
		set
		{
			//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(value);
					fixed (char* begin = readOnlySpan)
					{
						managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
						set_sortingLayerName_Injected(intPtr, ref managedSpanWrapper);
						return;
					}
				}
				set_sortingLayerName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
			}
		}
	}

	public int sortingLayerID
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sortingLayerID_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sortingLayerID_Injected(intPtr, value);
		}
	}

	public int sortingOrder
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sortingOrder_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sortingOrder_Injected(intPtr, value);
		}
	}

	internal uint sortingKey
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sortingKey_Injected(intPtr);
		}
	}

	internal int sortingGroupID
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sortingGroupID_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sortingGroupID_Injected(intPtr, value);
		}
	}

	internal int sortingGroupOrder
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sortingGroupOrder_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sortingGroupOrder_Injected(intPtr, value);
		}
	}

	internal uint sortingGroupKey
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sortingGroupKey_Injected(intPtr);
		}
	}

	public bool isLOD0
	{
		[NativeName("IsLOD0")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isLOD0_Injected(intPtr);
		}
	}

	[NativeProperty("IsDynamicOccludee")]
	public bool allowOcclusionWhenDynamic
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowOcclusionWhenDynamic_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowOcclusionWhenDynamic_Injected(intPtr, value);
		}
	}

	[NativeProperty("ForceMeshLod")]
	public short forceMeshLod
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_forceMeshLod_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_forceMeshLod_Injected(intPtr, value);
		}
	}

	[NativeProperty("MeshLodSelectionBias")]
	public float meshLodSelectionBias
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_meshLodSelectionBias_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_meshLodSelectionBias_Injected(intPtr, value);
		}
	}

	[NativeProperty("StaticBatchRoot")]
	internal Transform staticBatchRootTransform
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(get_staticBatchRootTransform_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_staticBatchRootTransform_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	internal int staticBatchIndex
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_staticBatchIndex_Injected(intPtr);
		}
	}

	public bool isPartOfStaticBatch
	{
		[NativeName("IsPartOfStaticBatch")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isPartOfStaticBatch_Injected(intPtr);
		}
	}

	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_worldToLocalMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localToWorldMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public GameObject lightProbeProxyVolumeOverride
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<GameObject>(get_lightProbeProxyVolumeOverride_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lightProbeProxyVolumeOverride_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Transform probeAnchor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(get_probeAnchor_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_probeAnchor_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public int lightmapIndex
	{
		get
		{
			return GetLightmapIndex(LightmapType.StaticLightmap);
		}
		set
		{
			SetLightmapIndex(value, LightmapType.StaticLightmap);
		}
	}

	public int realtimeLightmapIndex
	{
		get
		{
			return GetLightmapIndex(LightmapType.DynamicLightmap);
		}
		set
		{
			SetLightmapIndex(value, LightmapType.DynamicLightmap);
		}
	}

	public Vector4 lightmapScaleOffset
	{
		get
		{
			return GetLightmapST(LightmapType.StaticLightmap);
		}
		set
		{
			SetStaticLightmapST(value);
		}
	}

	public Vector4 realtimeLightmapScaleOffset
	{
		get
		{
			return GetLightmapST(LightmapType.DynamicLightmap);
		}
		set
		{
			SetLightmapST(value, LightmapType.DynamicLightmap);
		}
	}

	public Material[] materials
	{
		get
		{
			return GetMaterialArray();
		}
		set
		{
			SetMaterialArray(value);
		}
	}

	public Material material
	{
		get
		{
			return GetMaterial();
		}
		set
		{
			SetMaterial(value);
		}
	}

	public Material sharedMaterial
	{
		get
		{
			return GetSharedMaterial();
		}
		set
		{
			SetMaterial(value);
		}
	}

	public Material[] sharedMaterials
	{
		get
		{
			return GetSharedMaterialArray();
		}
		set
		{
			SetMaterialArray(value);
		}
	}

	public LODGroup LODGroup
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<LODGroup>(get_LODGroup_Injected(intPtr));
		}
	}

	[NativeName("ResetWorldAABB")]
	public void ResetBounds()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetBounds_Injected(intPtr);
	}

	[NativeName("ResetLocalAABB")]
	public void ResetLocalBounds()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetLocalBounds_Injected(intPtr);
	}

	[NativeName("HasCustomWorldAABB")]
	internal bool Internal_HasCustomBounds()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_HasCustomBounds_Injected(intPtr);
	}

	[NativeName("HasCustomLocalAABB")]
	internal bool Internal_HasCustomLocalBounds()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_HasCustomLocalBounds_Injected(intPtr);
	}

	[FreeFunction(Name = "RendererScripting::SetStaticLightmapST", HasExplicitThis = true)]
	private void SetStaticLightmapST(Vector4 st)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetStaticLightmapST_Injected(intPtr, ref st);
	}

	[FreeFunction(Name = "RendererScripting::GetMaterial", HasExplicitThis = true)]
	private Material GetMaterial()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Material>(GetMaterial_Injected(intPtr));
	}

	[FreeFunction(Name = "RendererScripting::GetSharedMaterial", HasExplicitThis = true)]
	private Material GetSharedMaterial()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Material>(GetSharedMaterial_Injected(intPtr));
	}

	[FreeFunction(Name = "RendererScripting::SetMaterial", HasExplicitThis = true)]
	private void SetMaterial(Material m)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(m));
	}

	[FreeFunction(Name = "RendererScripting::GetMaterialArray", HasExplicitThis = true)]
	private Material[] GetMaterialArray()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetMaterialArray_Injected(intPtr);
	}

	[FreeFunction(Name = "RendererScripting::GetMaterialArray", HasExplicitThis = true)]
	private void CopyMaterialArray([Out] Material[] m)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyMaterialArray_Injected(intPtr, m);
	}

	[FreeFunction(Name = "RendererScripting::GetSharedMaterialArray", HasExplicitThis = true)]
	private void CopySharedMaterialArray([Out] Material[] m)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopySharedMaterialArray_Injected(intPtr, m);
	}

	[FreeFunction(Name = "RendererScripting::SetMaterialArray", HasExplicitThis = true)]
	private void SetMaterialArray([NotNull] Material[] m, int length)
	{
		if (m == null)
		{
			ThrowHelper.ThrowArgumentNullException(m, "m");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMaterialArray_Injected(intPtr, m, length);
	}

	private void SetMaterialArray(Material[] m)
	{
		SetMaterialArray(m, (m != null) ? m.Length : 0);
	}

	[FreeFunction(Name = "RendererScripting::SetPropertyBlock", HasExplicitThis = true)]
	internal void Internal_SetPropertyBlock(MaterialPropertyBlock properties)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_SetPropertyBlock_Injected(intPtr, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
	}

	[FreeFunction(Name = "RendererScripting::GetPropertyBlock", HasExplicitThis = true)]
	internal void Internal_GetPropertyBlock([NotNull] MaterialPropertyBlock dest)
	{
		if (dest == null)
		{
			ThrowHelper.ThrowArgumentNullException(dest, "dest");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(dest);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(dest, "dest");
		}
		Internal_GetPropertyBlock_Injected(intPtr, intPtr2);
	}

	[FreeFunction(Name = "RendererScripting::SetPropertyBlockMaterialIndex", HasExplicitThis = true)]
	internal void Internal_SetPropertyBlockMaterialIndex(MaterialPropertyBlock properties, int materialIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_SetPropertyBlockMaterialIndex_Injected(intPtr, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), materialIndex);
	}

	[FreeFunction(Name = "RendererScripting::GetPropertyBlockMaterialIndex", HasExplicitThis = true)]
	internal void Internal_GetPropertyBlockMaterialIndex([NotNull] MaterialPropertyBlock dest, int materialIndex)
	{
		if (dest == null)
		{
			ThrowHelper.ThrowArgumentNullException(dest, "dest");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(dest);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(dest, "dest");
		}
		Internal_GetPropertyBlockMaterialIndex_Injected(intPtr, intPtr2, materialIndex);
	}

	[FreeFunction(Name = "RendererScripting::HasPropertyBlock", HasExplicitThis = true)]
	public bool HasPropertyBlock()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasPropertyBlock_Injected(intPtr);
	}

	public void SetPropertyBlock(MaterialPropertyBlock properties)
	{
		Internal_SetPropertyBlock(properties);
	}

	public void SetPropertyBlock(MaterialPropertyBlock properties, int materialIndex)
	{
		Internal_SetPropertyBlockMaterialIndex(properties, materialIndex);
	}

	public void GetPropertyBlock(MaterialPropertyBlock properties)
	{
		Internal_GetPropertyBlock(properties);
	}

	public void GetPropertyBlock(MaterialPropertyBlock properties, int materialIndex)
	{
		Internal_GetPropertyBlockMaterialIndex(properties, materialIndex);
	}

	[FreeFunction(Name = "RendererScripting::GetClosestReflectionProbes", HasExplicitThis = true)]
	private void GetClosestReflectionProbesInternal(object result)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetClosestReflectionProbesInternal_Injected(intPtr, result);
	}

	[NativeName("Renderer::GetMaskInteraction")]
	internal SpriteMaskInteraction Internal_GetSpriteMaskInteraction()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetSpriteMaskInteraction_Injected(intPtr);
	}

	[NativeName("Renderer::SetMaskInteraction")]
	internal void Internal_SetSpriteMaskInteraction(SpriteMaskInteraction maskInteraction)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_SetSpriteMaskInteraction_Injected(intPtr, maskInteraction);
	}

	[NativeName("GetIsStaticShadowCaster")]
	private bool GetIsStaticShadowCaster()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetIsStaticShadowCaster_Injected(intPtr);
	}

	[NativeName("SetIsStaticShadowCaster")]
	private void SetIsStaticShadowCaster(bool value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIsStaticShadowCaster_Injected(intPtr, value);
	}

	internal void SetStaticBatchInfo(int firstSubMesh, int subMeshCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetStaticBatchInfo_Injected(intPtr, firstSubMesh, subMeshCount);
	}

	[NativeName("GetLightmapIndexInt")]
	private int GetLightmapIndex(LightmapType lt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetLightmapIndex_Injected(intPtr, lt);
	}

	[NativeName("SetLightmapIndexInt")]
	private void SetLightmapIndex(int index, LightmapType lt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLightmapIndex_Injected(intPtr, index, lt);
	}

	[NativeName("GetLightmapST")]
	private Vector4 GetLightmapST(LightmapType lt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetLightmapST_Injected(intPtr, lt, out var ret);
		return ret;
	}

	[NativeName("SetLightmapST")]
	private void SetLightmapST(Vector4 st, LightmapType lt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLightmapST_Injected(intPtr, ref st, lt);
	}

	private int GetMaterialCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetMaterialCount_Injected(intPtr);
	}

	[NativeName("GetMaterialArray")]
	private Material[] GetSharedMaterialArray()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSharedMaterialArray_Injected(intPtr);
	}

	public void GetMaterials(List<Material> m)
	{
		if (m == null)
		{
			throw new ArgumentNullException("The result material list cannot be null.", "m");
		}
		NoAllocHelpers.EnsureListElemCount(m, GetMaterialCount());
		CopyMaterialArray(NoAllocHelpers.ExtractArrayFromList(m));
	}

	public void SetSharedMaterials(List<Material> materials)
	{
		if (materials == null)
		{
			throw new ArgumentNullException("The material list to set cannot be null.", "materials");
		}
		SetMaterialArray(NoAllocHelpers.ExtractArrayFromList(materials), materials.Count);
	}

	public void SetMaterials(List<Material> materials)
	{
		if (materials == null)
		{
			throw new ArgumentNullException("The material list to set cannot be null.", "materials");
		}
		SetMaterialArray(NoAllocHelpers.ExtractArrayFromList(materials), materials.Count);
	}

	public void GetSharedMaterials(List<Material> m)
	{
		if (m == null)
		{
			throw new ArgumentNullException("The result material list cannot be null.", "m");
		}
		NoAllocHelpers.EnsureListElemCount(m, GetMaterialCount());
		CopySharedMaterialArray(NoAllocHelpers.ExtractArrayFromList(m));
	}

	public void GetClosestReflectionProbes(List<ReflectionProbeBlendInfo> result)
	{
		GetClosestReflectionProbesInternal(result);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localBounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_localBounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetBounds_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetLocalBounds_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_HasCustomBounds_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_HasCustomLocalBounds_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStaticLightmapST_Injected(IntPtr _unity_self, [In] ref Vector4 st);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetMaterial_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetSharedMaterial_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMaterial_Injected(IntPtr _unity_self, IntPtr m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Material[] GetMaterialArray_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyMaterialArray_Injected(IntPtr _unity_self, [Out] Material[] m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopySharedMaterialArray_Injected(IntPtr _unity_self, [Out] Material[] m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMaterialArray_Injected(IntPtr _unity_self, Material[] m, int length);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetPropertyBlock_Injected(IntPtr _unity_self, IntPtr properties);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetPropertyBlock_Injected(IntPtr _unity_self, IntPtr dest);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetPropertyBlockMaterialIndex_Injected(IntPtr _unity_self, IntPtr properties, int materialIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetPropertyBlockMaterialIndex_Injected(IntPtr _unity_self, IntPtr dest, int materialIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasPropertyBlock_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetClosestReflectionProbesInternal_Injected(IntPtr _unity_self, object result);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SpriteMaskInteraction Internal_GetSpriteMaskInteraction_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetSpriteMaskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction maskInteraction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enabled_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isVisible_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ShadowCastingMode get_shadowCastingMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowCastingMode_Injected(IntPtr _unity_self, ShadowCastingMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_receiveShadows_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_receiveShadows_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_forceRenderingOff_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_forceRenderingOff_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowGPUDrivenRendering_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowGPUDrivenRendering_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_smallMeshCulling_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_smallMeshCulling_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIsStaticShadowCaster_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIsStaticShadowCaster_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern MotionVectorGenerationMode get_motionVectorGenerationMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_motionVectorGenerationMode_Injected(IntPtr _unity_self, MotionVectorGenerationMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LightProbeUsage get_lightProbeUsage_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lightProbeUsage_Injected(IntPtr _unity_self, LightProbeUsage value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ReflectionProbeUsage get_reflectionProbeUsage_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_reflectionProbeUsage_Injected(IntPtr _unity_self, ReflectionProbeUsage value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint get_renderingLayerMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderingLayerMask_Injected(IntPtr _unity_self, uint value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_rendererPriority_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rendererPriority_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RayTracingMode get_rayTracingMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rayTracingMode_Injected(IntPtr _unity_self, RayTracingMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RayTracingAccelerationStructureBuildFlags get_rayTracingAccelerationStructureBuildFlags_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rayTracingAccelerationStructureBuildFlags_Injected(IntPtr _unity_self, RayTracingAccelerationStructureBuildFlags value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_rayTracingAccelerationStructureBuildFlagsOverride_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rayTracingAccelerationStructureBuildFlagsOverride_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_sortingLayerName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sortingLayerName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_sortingLayerID_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sortingLayerID_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_sortingOrder_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sortingOrder_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint get_sortingKey_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_sortingGroupID_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sortingGroupID_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_sortingGroupOrder_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sortingGroupOrder_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint get_sortingGroupKey_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isLOD0_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowOcclusionWhenDynamic_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowOcclusionWhenDynamic_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern short get_forceMeshLod_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_forceMeshLod_Injected(IntPtr _unity_self, short value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_meshLodSelectionBias_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_meshLodSelectionBias_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_staticBatchRootTransform_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_staticBatchRootTransform_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_staticBatchIndex_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStaticBatchInfo_Injected(IntPtr _unity_self, int firstSubMesh, int subMeshCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isPartOfStaticBatch_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_worldToLocalMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_lightProbeProxyVolumeOverride_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lightProbeProxyVolumeOverride_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_probeAnchor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_probeAnchor_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetLightmapIndex_Injected(IntPtr _unity_self, LightmapType lt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLightmapIndex_Injected(IntPtr _unity_self, int index, LightmapType lt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLightmapST_Injected(IntPtr _unity_self, LightmapType lt, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLightmapST_Injected(IntPtr _unity_self, [In] ref Vector4 st, LightmapType lt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMaterialCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Material[] GetSharedMaterialArray_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_LODGroup_Injected(IntPtr _unity_self);
}
