namespace UnityEngine.Rendering;

public struct RayTracingSubGeometryDesc
{
	public RayTracingSubMeshFlags flags { get; set; }

	public int id { get; set; }

	public int indexStart { get; set; }

	public int indexCount { get; set; }

	public int vertexStart { get; set; }

	public int vertexCount { get; set; }

	public RayTracingSubGeometryDesc()
	{
		flags = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly;
		id = 0;
		indexStart = 0;
		indexCount = 0;
		vertexStart = 0;
		vertexCount = 0;
	}

	public RayTracingSubGeometryDesc(int indexStart, int indexCount, int id = 0, RayTracingSubMeshFlags flags = RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly)
	{
		vertexStart = 0;
		vertexCount = 0;
		this.indexStart = indexStart;
		this.indexCount = indexCount;
		this.id = id;
		this.flags = flags;
	}
}
