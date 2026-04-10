using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings;

internal static class CreateAndFillCalbacks
{
	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	public unsafe static void CreateAndCallbackPinned1(IntPtr arrayPointer, IntPtr createArrayCb, int size, delegate* unmanaged[Cdecl]<byte*, IntPtr, void> callback, IntPtr arg)
	{
		ref Array reference = ref UnsafeUtility.ClassAsRef<Array>((void*)arrayPointer);
		reference = ((delegate*<int, Array>)(void*)createArrayCb)(size);
		fixed (byte* ptr = UnsafeUtility.As<byte[]>(reference))
		{
			callback(ptr, arg);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	public unsafe static void CreateAndCallbackPinned2(IntPtr arrayPointer, IntPtr createArrayCb, int size1, int size2, delegate* unmanaged[Cdecl]<byte*, IntPtr, void> callback, IntPtr arg)
	{
		ref Array reference = ref UnsafeUtility.ClassAsRef<Array>((void*)arrayPointer);
		reference = ((delegate*<int, int, Array>)(void*)createArrayCb)(size1, size2);
		fixed (byte* ptr = UnsafeUtility.As<byte[,]>(reference))
		{
			callback(ptr, arg);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	public unsafe static void CreateAndCallbackPinned3(IntPtr arrayPointer, IntPtr createArrayCb, int size1, int size2, int size3, delegate* unmanaged[Cdecl]<byte*, IntPtr, void> callback, IntPtr arg)
	{
		ref Array reference = ref UnsafeUtility.ClassAsRef<Array>((void*)arrayPointer);
		reference = ((delegate*<int, int, int, Array>)(void*)createArrayCb)(size1, size2, size2);
		fixed (byte* ptr = UnsafeUtility.As<byte[,,]>(reference))
		{
			callback(ptr, arg);
		}
	}
}
