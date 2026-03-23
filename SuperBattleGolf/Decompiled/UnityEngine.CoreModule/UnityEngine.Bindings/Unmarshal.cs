using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[VisibleToOtherModules]
internal struct Unmarshal
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T UnmarshalUnityObject<T>(IntPtr gcHandlePtr) where T : Object
	{
		if (gcHandlePtr == IntPtr.Zero)
		{
			return null;
		}
		return (T)FromIntPtrUnsafe(gcHandlePtr).Target;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GCHandle FromIntPtrUnsafe(IntPtr gcHandle)
	{
		return UnsafeUtility.As<IntPtr, GCHandle>(ref gcHandle);
	}
}
