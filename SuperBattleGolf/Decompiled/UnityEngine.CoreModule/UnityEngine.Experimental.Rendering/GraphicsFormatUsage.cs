using System;

namespace UnityEngine.Experimental.Rendering;

[Flags]
public enum GraphicsFormatUsage
{
	None = 0,
	Sample = 1,
	Linear = 2,
	Sparse = 4,
	Render = 0x10,
	Blend = 0x20,
	GetPixels = 0x40,
	SetPixels = 0x80,
	SetPixels32 = 0x100,
	ReadPixels = 0x200,
	LoadStore = 0x400,
	MSAA2x = 0x800,
	MSAA4x = 0x1000,
	MSAA8x = 0x2000,
	StencilSampling = 0x10000
}
