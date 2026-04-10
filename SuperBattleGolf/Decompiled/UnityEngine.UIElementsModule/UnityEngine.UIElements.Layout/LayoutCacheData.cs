using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.Layout;

[NativeHeader("Modules/UIElements/Core/Layout/Native/LayoutNative.h")]
internal struct LayoutCacheData
{
	public static LayoutCacheData Default = new LayoutCacheData
	{
		CachedLayout = LayoutCachedMeasurement.Default
	};

	public LayoutCachedMeasurement CachedLayout;

	public override readonly string ToString()
	{
		return $"CacheCount: {MeasurementCacheCount()}\n" + $"CachedLayout: {CachedLayout}";
	}

	public unsafe readonly int MeasurementCacheCount()
	{
		int num = 0;
		for (LayoutCachedMeasurement* nextMeasurementCache = CachedLayout.NextMeasurementCache; nextMeasurementCache != null; nextMeasurementCache = nextMeasurementCache->NextMeasurementCache)
		{
			num++;
		}
		return num;
	}

	public unsafe void ClearCachedMeasurements()
	{
		if (CachedLayout.NextMeasurementCache != null)
		{
			fixed (LayoutCachedMeasurement* cachedLayout = &CachedLayout)
			{
				void* layoutCacheData = cachedLayout;
				ClearCachedMeasurements(layoutCacheData);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void ClearCachedMeasurements(void* LayoutCacheData);
}
