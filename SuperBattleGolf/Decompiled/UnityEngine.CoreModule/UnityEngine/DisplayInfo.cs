using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeType("Runtime/Graphics/DisplayInfo.h")]
public struct DisplayInfo : IEquatable<DisplayInfo>
{
	[UnityEngine.Scripting.RequiredMember]
	internal ulong handle;

	[UnityEngine.Scripting.RequiredMember]
	public int width;

	[UnityEngine.Scripting.RequiredMember]
	public int height;

	[UnityEngine.Scripting.RequiredMember]
	public RefreshRate refreshRate;

	[UnityEngine.Scripting.RequiredMember]
	public RectInt workArea;

	[UnityEngine.Scripting.RequiredMember]
	public string name;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(DisplayInfo other)
	{
		return handle == other.handle && width == other.width && height == other.height && refreshRate.Equals(other.refreshRate) && workArea.Equals(other.workArea) && name == other.name;
	}
}
