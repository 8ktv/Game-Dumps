using System;
using System.Runtime.InteropServices;
using AOT;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal readonly struct ArrayHandleOnStack
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal unsafe delegate void* CreateArrayDelegate(void* targetRef, int size);
}
[VisibleToOtherModules]
internal readonly struct ArrayHandleOnStack<TT> where TT : unmanaged
{
	private unsafe readonly void* _arrayRefPtr;

	private readonly IntPtr _allocArrayCallbackPtr;

	private static ArrayHandleOnStack.CreateArrayDelegate s_createArrayDelegate;

	private static IntPtr s_createArrayFcnPtr;

	unsafe static ArrayHandleOnStack()
	{
		s_createArrayDelegate = AllocArrayManagedCallback;
		s_createArrayFcnPtr = Marshal.GetFunctionPointerForDelegate(s_createArrayDelegate);
	}

	public unsafe ArrayHandleOnStack(void* arrayRefPtr)
	{
		_arrayRefPtr = arrayRefPtr;
		_allocArrayCallbackPtr = s_createArrayFcnPtr;
	}

	public unsafe ArrayHandleOnStack(void* arrayRefPtr, IntPtr allocArrayCallbackPtr)
	{
		_arrayRefPtr = arrayRefPtr;
		_allocArrayCallbackPtr = allocArrayCallbackPtr;
	}

	[MonoPInvokeCallback(typeof(ArrayHandleOnStack.CreateArrayDelegate))]
	public unsafe static void* AllocArrayManagedCallback(void* targetRef, int size)
	{
		TT[] array = new TT[size];
		UnsafeUtility.ClassAsRef<TT[]>(targetRef) = array;
		if (size < 1)
		{
			return null;
		}
		fixed (TT* result = array)
		{
			return result;
		}
	}
}
