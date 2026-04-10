using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering;

public struct RayTracingMultiGeometryInstanceConfig
{
	public GraphicsBuffer vertexBuffer { get; set; }

	public VertexAttributeDescriptor[] vertexAttributes { get; set; }

	public GraphicsBuffer indexBuffer { get; set; }

	public RayTracingMode rayTracingMode { get; set; }

	public Material[] materials { get; set; }

	public RayTracingSubGeometryDesc[] subGeometries { get; set; }

	public bool subGeometriesValidation { get; set; }

	public MaterialPropertyBlock materialProperties { get; set; }

	public bool enableTriangleCulling { get; set; }

	public bool frontTriangleCounterClockwise { get; set; }

	public int layer { get; set; }

	public uint renderingLayerMask { get; set; }

	public uint mask { get; set; }

	public MotionVectorGenerationMode motionVectorMode { get; set; }

	public RayTracingAccelerationStructureBuildFlags accelerationStructureBuildFlags { get; set; }

	public bool accelerationStructureBuildFlagsOverride { get; set; }

	public RayTracingMultiGeometryInstanceConfig()
	{
		vertexBuffer = null;
		indexBuffer = null;
		vertexAttributes = null;
		rayTracingMode = RayTracingMode.Static;
		materials = null;
		subGeometries = null;
		subGeometriesValidation = true;
		materialProperties = null;
		enableTriangleCulling = false;
		frontTriangleCounterClockwise = false;
		layer = 0;
		renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask;
		mask = 255u;
		motionVectorMode = MotionVectorGenerationMode.Camera;
		accelerationStructureBuildFlagsOverride = false;
		accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
	}
}
