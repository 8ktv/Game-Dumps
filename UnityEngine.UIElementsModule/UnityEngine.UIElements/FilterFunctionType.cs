using System;

namespace UnityEngine.UIElements;

[Serializable]
internal enum FilterFunctionType
{
	None,
	Custom,
	Tint,
	Opacity,
	Invert,
	Grayscale,
	Sepia,
	Blur,
	Count
}
