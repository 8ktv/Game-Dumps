using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(OutArray<>.BindingsMarshaller))]
internal ref struct OutArray<T> where T : unmanaged
{
	public static class BindingsMarshaller
	{
		public unsafe static OutArrayNativeData ConvertToUnmanaged(ref OutArray<T> marshalled)
		{
			return new OutArrayNativeData
			{
				createAndCallback = (IntPtr)(delegate*<IntPtr, IntPtr, int, delegate* unmanaged[Cdecl]<byte*, IntPtr, void>, IntPtr, void>)(&CreateAndFillCalbacks.CreateAndCallbackPinned1),
				arrayRef = (IntPtr)UnsafeUtility.AsPointer(ref marshalled.array),
				createArray = (IntPtr)(delegate*<int, Array>)(&OutArray<T>.CreateArray)
			};
		}

		public unsafe static OutArray<T> ConvertToManaged(in OutArrayNativeData unmanaged)
		{
			return new OutArray<T>
			{
				array = UnsafeUtility.ClassAsRef<T[]>((void*)unmanaged.arrayRef)
			};
		}
	}

	[Ignore]
	private T[] array;

	public T[] Value => array;

	public static Array CreateArray(int length)
	{
		return new T[length];
	}
}
