using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering;

[MovedFrom("UnityEngine.Experimental.Rendering")]
public struct RayTracingMeshInstanceConfig
{
	public Mesh mesh;

	public uint subMeshIndex;

	public RayTracingSubMeshFlags subMeshFlags;

	public Material material;

	public MaterialPropertyBlock materialProperties;

	public bool enableTriangleCulling;

	public bool frontTriangleCounterClockwise;

	public int layer;

	public uint renderingLayerMask;

	public uint mask;

	public MotionVectorGenerationMode motionVectorMode;

	public LightProbeUsage lightProbeUsage;

	public LightProbeProxyVolume lightProbeProxyVolume;

	public int meshLod;

	public RayTracingMode rayTracingMode { get; set; }

	[Obsolete("dynamicGeometry has been deprecated and will be removed in the future. Use rayTracingMode instead.", false)]
	public bool dynamicGeometry { get; set; }

	public RayTracingAccelerationStructureBuildFlags accelerationStructureBuildFlags { get; set; }

	public bool accelerationStructureBuildFlagsOverride { get; set; }

	public RayTracingMeshInstanceConfig()
	{
		mesh = null;
		subMeshIndex = 0u;
		material = null;
		subMeshFlags = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
		rayTracingMode = RayTracingMode.Static;
		dynamicGeometry = false;
		materialProperties = null;
		enableTriangleCulling = true;
		frontTriangleCounterClockwise = false;
		layer = 0;
		renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask;
		mask = 255u;
		motionVectorMode = MotionVectorGenerationMode.Camera;
		lightProbeUsage = LightProbeUsage.Off;
		lightProbeProxyVolume = null;
		accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
		accelerationStructureBuildFlagsOverride = false;
		meshLod = 0;
	}

	public RayTracingMeshInstanceConfig(Mesh mesh, uint subMeshIndex, Material material)
	{
		this.mesh = mesh;
		this.subMeshIndex = subMeshIndex;
		this.material = material;
		subMeshFlags = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
		rayTracingMode = RayTracingMode.Static;
		dynamicGeometry = false;
		materialProperties = null;
		enableTriangleCulling = true;
		frontTriangleCounterClockwise = false;
		layer = 0;
		renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask;
		mask = 255u;
		motionVectorMode = MotionVectorGenerationMode.Camera;
		lightProbeUsage = LightProbeUsage.Off;
		lightProbeProxyVolume = null;
		accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
		accelerationStructureBuildFlagsOverride = false;
		meshLod = 0;
	}
}
