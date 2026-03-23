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
		ComputedHeight = -1f
	};

	public float AvailableWidth;

	public float AvailableHeight;

	public float ParentWidth;

	public float ParentHeight;

	public LayoutMeasureMode WidthMeasureMode;

	public LayoutMeasureMode HeightMeasureMode;

	public float ComputedWidth;

	public float ComputedHeight;
}
