namespace UnityEngine.Rendering;

public struct RayTracingAABBsInstanceConfig
{
	public GraphicsBuffer aabbBuffer { get; set; }

	public int aabbCount { get; set; }

	public uint aabbOffset { get; set; }

	public bool dynamicGeometry { get; set; }

	public bool opaqueMaterial { get; set; }

	public Material material { get; set; }

	public MaterialPropertyBlock materialProperties { get; set; }

	public int layer { get; set; }

	public uint mask { get; set; }

	public RayTracingAccelerationStructureBuildFlags accelerationStructureBuildFlags { get; set; }

	public bool accelerationStructureBuildFlagsOverride { get; set; }

	public RayTracingAABBsInstanceConfig()
	{
		aabbBuffer = null;
		aabbCount = 0;
		material = null;
		dynamicGeometry = false;
		opaqueMaterial = true;
		aabbOffset = 0u;
		materialProperties = null;
		layer = 0;
		mask = 255u;
		accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
		accelerationStructureBuildFlagsOverride = false;
	}

	public RayTracingAABBsInstanceConfig(GraphicsBuffer aabbBuffer, int aabbCount, bool dynamicGeometry, Material material)
	{
		this.aabbBuffer = aabbBuffer;
		this.aabbCount = aabbCount;
		this.material = material;
		this.dynamicGeometry = dynamicGeometry;
		opaqueMaterial = true;
		aabbOffset = 0u;
		materialProperties = null;
		layer = 0;
		mask = 255u;
		accelerationStructureBuildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
		accelerationStructureBuildFlagsOverride = false;
	}
}
