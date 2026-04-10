using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings;

[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(OutArray3D<>.BindingsMarshaller))]
[VisibleToOtherModules]
internal ref struct OutArray3D<T> where T : unmanaged
{
	public static class BindingsMarshaller
	{
		public unsafe static OutArrayNativeData ConvertToUnmanaged(ref OutArray3D<T> marshalled)
		{
			return new OutArrayNativeData
			{
				createAndCallback = (IntPtr)(delegate*<IntPtr, IntPtr, int, int, int, delegate* unmanaged[Cdecl]<byte*, IntPtr, void>, IntPtr, void>)(&CreateAndFillCalbacks.CreateAndCallbackPinned3),
				arrayRef = (IntPtr)UnsafeUtility.AsPointer(ref marshalled.array),
				createArray = (IntPtr)(delegate*<int, int, int, Array>)(&OutArray3D<T>.CreateArray)
			};
		}

		public unsafe static OutArray3D<T> ConvertToManaged(in OutArrayNativeData unmanaged)
		{
			return new OutArray3D<T>
			{
				array = UnsafeUtility.ClassAsRef<T[,,]>((void*)unmanaged.arrayRef)
			};
		}
	}

	[Ignore]
	private T[,,] array;

	public T[,,] Value => array;

	public static Array CreateArray(int length1, int lenght2, int length3)
	{
		return new T[length1, lenght2, length3];
	}
}
