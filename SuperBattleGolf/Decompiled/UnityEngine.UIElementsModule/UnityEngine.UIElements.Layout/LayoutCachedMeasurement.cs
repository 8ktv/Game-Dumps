namespace UnityEngine.UIElements.Layout;

internal struct LayoutCachedMeasurement
{
	public static LayoutCachedMeasurement Default = new LayoutCachedMeasurement
	{
		AvailableWidth = 0f,
		AvailableHeight = 0f,
		ParentWidth = 0f,
		ParentHeight = 0f,
		WidthMeasureMode = LayoutMeasureMode.Invalid,
		HeightMeasureMode = LayoutMeasureMode.Invalid,
		ComputedWidth = -1f,
		ComputedHeight = -1f,
		m_NextMeasurementCachePtr = null
	};

	public float AvailableWidth;

	public float AvailableHeight;

	public float ParentWidth;

	public float ParentHeight;

	public LayoutMeasureMode WidthMeasureMode;

	public LayoutMeasureMode HeightMeasureMode;

	public float ComputedWidth;

	public float ComputedHeight;

	private unsafe void* m_NextMeasurementCachePtr;

	public unsafe LayoutCachedMeasurement* NextMeasurementCache => (LayoutCachedMeasurement*)m_NextMeasurementCachePtr;

	public override readonly string ToString()
	{
		return $"Available: {AvailableWidth}/{AvailableHeight}   Parent: {ParentWidth}/{ParentHeight}   MeasureMode: {WidthMeasureMode}/{HeightMeasureMode},   Computed: {ComputedWidth}/{ComputedHeight}";
	}
}
