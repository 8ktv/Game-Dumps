using System;

namespace UnityEngine.UIElements.UIR;

[Flags]
internal enum RenderDataFlags
{
	IsGroupTransform = 1,
	IsIgnoringDynamicColorHint = 2,
	HasExtraData = 4,
	HasExtraMeshes = 8,
	IsSubTreeQuad = 0x10,
	IsNestedRenderTreeRoot = 0x20,
	IsClippingRectDirty = 0x40
}
