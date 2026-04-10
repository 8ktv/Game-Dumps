using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeType("Runtime/Graphics/DisplayInfo.h")]
[UsedByNativeCode]
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

	[UnityEngine.Scripting.RequiredMember]
	[NativeName("dpi")]
	public float physicalDpi;

	public Resolution[] resolutions
	{
		get
		{
			throw new NotSupportedException("DisplayInfo.resolutions is currently not supported on this platform.");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(DisplayInfo other)
	{
		return handle == other.handle && width == other.width && height == other.height && refreshRate.Equals(other.refreshRate) && workArea.Equals(other.workArea) && name == other.name && physicalDpi == other.physicalDpi;
	}

	public static void GetLayout(List<DisplayInfo> displayLayout)
	{
		Screen.GetDisplayLayout(displayLayout);
	}

	private static Resolution[] GetResolutions(DisplayInfo displayInfo)
	{
		throw new NotSupportedException("DisplayInfo.GetResolutions() is not supported on this platform.");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("DisplayInfoScripting::GetLayout")]
	[NativeConditional("PLATFORM_SUPPORTS_DISPLAYINFO_API")]
	private static extern void GetLayoutImpl(List<DisplayInfo> displayLayout);

	[FreeFunction("DisplayInfoScripting::GetResolutions")]
	[NativeConditional("PLATFORM_SUPPORTS_DISPLAYINFO_API")]
	private static Resolution[] GetResolutionsImpl(ulong handle)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Resolution[] result;
		try
		{
			GetResolutionsImpl_Injected(handle, out ret);
		}
		finally
		{
			Resolution[] array = default(Resolution[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetResolutionsImpl_Injected(ulong handle, out BlittableArrayWrapper ret);
}
