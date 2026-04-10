using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(OutArray2D<>.BindingsMarshaller))]
internal ref struct OutArray2D<T> where T : unmanaged
{
	public static class BindingsMarshaller
	{
		public unsafe static OutArrayNativeData ConvertToUnmanaged(ref OutArray2D<T> marshalled)
		{
			return new OutArrayNativeData
			{
				createAndCallback = (IntPtr)(delegate*<IntPtr, IntPtr, int, int, delegate* unmanaged[Cdecl]<byte*, IntPtr, void>, IntPtr, void>)(&CreateAndFillCalbacks.CreateAndCallbackPinned2),
				arrayRef = (IntPtr)UnsafeUtility.AsPointer(ref marshalled.array),
				createArray = (IntPtr)(delegate*<int, int, Array>)(&OutArray2D<T>.CreateArray)
			};
		}

		public unsafe static OutArray2D<T> ConvertToManaged(in OutArrayNativeData unmanaged)
		{
			return new OutArray2D<T>
			{
				array = UnsafeUtility.ClassAsRef<T[,]>((void*)unmanaged.arrayRef)
			};
		}
	}

	[Ignore]
	private T[,] array;

	public T[,] Value => array;

	public static Array CreateArray(int length1, int lenght2)
	{
		return new T[length1, lenght2];
	}
}
