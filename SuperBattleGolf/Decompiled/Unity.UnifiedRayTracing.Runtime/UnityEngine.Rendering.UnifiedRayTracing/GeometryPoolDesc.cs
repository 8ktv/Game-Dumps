namespace UnityEngine.Rendering.UnifiedRayTracing;

internal struct GeometryPoolDesc
{
	public int vertexPoolByteSize;

	public int indexPoolByteSize;

	public int meshChunkTablesByteSize;

	public static GeometryPoolDesc NewDefault()
	{
		return new GeometryPoolDesc
		{
			vertexPoolByteSize = 268435456,
			indexPoolByteSize = 33554432,
			meshChunkTablesByteSize = 4194304
		};
	}
}
