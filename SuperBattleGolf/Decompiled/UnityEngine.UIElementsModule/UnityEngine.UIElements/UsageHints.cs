using System;

namespace UnityEngine.UIElements;

[Flags]
public enum UsageHints
{
	None = 0,
	DynamicTransform = 1,
	GroupTransform = 2,
	MaskContainer = 4,
	DynamicColor = 8,
	DynamicPostProcessing = 0x10,
	LargePixelCoverage = 0x20
}
