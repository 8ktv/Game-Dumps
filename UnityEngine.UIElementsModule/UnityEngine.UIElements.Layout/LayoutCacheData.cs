namespace UnityEngine.UIElements.Layout;

internal struct LayoutCacheData
{
	public static LayoutCacheData Default = new LayoutCacheData
	{
		NextCachedMeasurementsIndex = 0u,
		CachedLayout = LayoutCachedMeasurement.Default
	};

	public uint NextCachedMeasurementsIndex;

	public FixedBuffer16<LayoutCachedMeasurement> cachedMeasurements;

	public LayoutCachedMeasurement CachedLayout;
}
