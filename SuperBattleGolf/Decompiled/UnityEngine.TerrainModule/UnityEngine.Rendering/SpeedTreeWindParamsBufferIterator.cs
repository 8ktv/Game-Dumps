using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
[NativeHeader("Modules/Terrain/Public/SpeedTreeWind.h")]
internal struct SpeedTreeWindParamsBufferIterator
{
	public IntPtr bufferPtr;

	public unsafe fixed int uintParamOffsets[16];

	public int uintStride;

	public int elementOffset;

	public int elementsCount;
}
