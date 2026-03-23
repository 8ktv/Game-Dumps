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
	DirtyOffset = 6,
	DirtyGroupTransform = 0x40,
	DirtyBoneTransform = 0x80,
	DirtyClipWithScissors = 0x100,
	DirtyMaskContainer = 0x200,
	DirtyDynamicColor = 0x400,
	DirtyDynamicPostProcessing = 0x800,
	DirtyAll = 0xFC0
}
