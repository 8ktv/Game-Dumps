using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
internal ref struct BlittableListWrapper
{
	private BlittableArrayWrapper arrayWrapper;

	private int listSize;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BlittableListWrapper(BlittableArrayWrapper arrayWrapper, int listSize)
	{
		this.arrayWrapper = arrayWrapper;
		this.listSize = listSize;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe void Unmarshal<T>(List<T> list) where T : unmanaged
	{
		if (list != null)
		{
			switch (arrayWrapper.updateFlags)
			{
			case BlittableArrayWrapper.UpdateFlags.NoUpdateNeeded:
				break;
			case BlittableArrayWrapper.UpdateFlags.SizeChanged:
			case BlittableArrayWrapper.UpdateFlags.DataIsEmpty:
			case BlittableArrayWrapper.UpdateFlags.DataIsNull:
				NoAllocHelpers.ResetListSize(list, listSize);
				break;
			case BlittableArrayWrapper.UpdateFlags.DataIsNativePointer:
				NoAllocHelpers.ResetListContents(list, new ReadOnlySpan<T>(arrayWrapper.data, arrayWrapper.size));
				break;
			case BlittableArrayWrapper.UpdateFlags.DataIsNativeOwnedMemory:
				NoAllocHelpers.ResetListContents(list, new ReadOnlySpan<T>(BindingsAllocator.GetNativeOwnedDataPointer(arrayWrapper.data), arrayWrapper.size));
				BindingsAllocator.FreeNativeOwnedMemory(arrayWrapper.data);
				break;
			}
		}
	}
}
