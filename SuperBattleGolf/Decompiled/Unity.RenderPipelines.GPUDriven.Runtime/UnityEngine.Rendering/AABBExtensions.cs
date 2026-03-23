namespace UnityEngine.Rendering;

internal static class AABBExtensions
{
	public static AABB ToAABB(this Bounds bounds)
	{
		return new AABB
		{
			center = bounds.center,
			extents = bounds.extents
		};
	}

	public static Bounds ToBounds(this AABB aabb)
	{
		return new Bounds
		{
			center = aabb.center,
			extents = aabb.extents
		};
	}
}
