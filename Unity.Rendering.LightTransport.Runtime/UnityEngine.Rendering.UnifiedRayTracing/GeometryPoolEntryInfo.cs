namespace UnityEngine.Rendering.UnifiedRayTracing;

internal struct GeometryPoolEntryInfo
{
	public bool valid;

	public uint refCount;

	public static GeometryPoolEntryInfo NewDefault()
	{
		return new GeometryPoolEntryInfo
		{
			valid = false,
			refCount = 0u
		};
	}
}
