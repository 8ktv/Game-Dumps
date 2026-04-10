using System;

namespace UnityEngine.UIElements;

[Flags]
internal enum RenderHints
{
	None = 0,
	GroupTransform = 1,
	BoneTransform = 2,
	ClipWithScissors = 4,
	MaskContainer = 8,
	DynamicColor = 0x10,
	DynamicPostProcessing = 0x20,
	LargePixelCoverage = 0x40,
	DirtyOffset = 7,
	DirtyGroupTransform = 0x80,
	DirtyBoneTransform = 0x100,
	DirtyClipWithScissors = 0x200,
	DirtyMaskContainer = 0x400,
	DirtyDynamicColor = 0x800,
	DirtyDynamicPostProcessing = 0x1000,
	DirtyLargePixelCoverage = 0x2000,
	DirtyAll = 0x3F80
}
