using System;

namespace UnityEngine.Rendering;

[Flags]
public enum SubPassFlags
{
	None = 0,
	ReadOnlyDepth = 2,
	ReadOnlyStencil = 4,
	ReadOnlyDepthStencil = 6,
	UseShadingRateImage = 8,
	TileProperties = 0x10,
	MultiviewRenderRegionsCompatible = 0x20
}
