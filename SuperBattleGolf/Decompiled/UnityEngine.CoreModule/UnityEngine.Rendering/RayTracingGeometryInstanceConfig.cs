using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering;

public struct RayTracingGeometryInstanceConfig
{
	public GraphicsBuffer vertexBuffer { get; set; }

	public VertexAttributeDescriptor[] vertexAttributes { get; set; }

	public uint vertexStart { get; set; }

	public int vertexCount { get; set; }

	public GraphicsBuffer indexBuffer { get; set; }

	public uint indexStart { get; set; }

	public int indexCount { get; set; }

	public RayTracingSubMeshFlags subMeshFlags { get; set; }

	public RayTracingMode rayTracingMode { get; set; }

	[Obsolete("dynamicGeometry has been deprecated and will be removed in the future. Use rayTracingMode instead.", false)]
	public bool dynamicGeometry { get; set; }

	public Material material { get; set; }

	public MaterialPropertyBlock materialProperties { get; set; }

	public bool enableTriangleCulling { get; set; }

	public bool frontTriangleCounterClockwise { get; set; }

	public int layer { get; set; }

	public uint renderingLayerMask { get; set; }

	public uint mask { get; set; }

	public MotionVectorGenerationMode motionVectorMode { get; set; }

	public LightProbeUsage lightProbeUsage { get; set; }

	public LightProbeProxyVolume lightProbeProxyVolume { get; set; }

	public RayTracingAccelerationStructureBuildFlags accelerationStructureBuildFlags { get; set; }

	public bool accelerationStructureBuildFlagsOverride { get; set; }

	public RayTracingGeometryInstanceConfig()
	{
		material = null;
		vertexBuffer = null;
		indexBuffer = null;
		vertexAttributes = null;
		vertexStart = 0u;
		indexStart = 0u;
		vertexCount = -1;
		indexCount = -1;
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
		accelerationStructureBuildFlagsOverride = false;
		accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
	}

	public RayTracingGeometryInstanceConfig(GraphicsBuffer vertexBuffer, VertexAttributeDescriptor[] vertexAttributes, GraphicsBuffer indexBuffer, Material material)
	{
		this.material = material;
		this.vertexBuffer = vertexBuffer;
		this.indexBuffer = indexBuffer;
		this.vertexAttributes = vertexAttributes;
		vertexStart = 0u;
		indexStart = 0u;
		vertexCount = -1;
		indexCount = -1;
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
		accelerationStructureBuildFlagsOverride = false;
		accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
	}
}
