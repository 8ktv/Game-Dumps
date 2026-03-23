using System;

namespace UnityEngine.Rendering;

[Flags]
public enum RTClearFlags
{
	None = 0,
	Color = 1,
	Depth = 2,
	Stencil = 4,
	All = 7,
	DepthStencil = 6,
	ColorDepth = 3,
	ColorStencil = 5,
	Color0 = 8,
	Color1 = 0x10,
	Color2 = 0x20,
	Color3 = 0x40,
	Color4 = 0x80,
	Color5 = 0x100,
	Color6 = 0x200,
	Color7 = 0x400
}
