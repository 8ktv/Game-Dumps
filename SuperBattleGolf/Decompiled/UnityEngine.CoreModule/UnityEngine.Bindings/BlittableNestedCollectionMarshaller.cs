using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[VisibleToOtherModules]
internal struct BlittableNestedCollectionMarshaller<T> where T : unmanaged
{
	private static readonly int AlignOfT;

	static BlittableNestedCollectionMarshaller()
	{
		AlignOfT = UnsafeUtility.AlignOf<T>();
	}

	public unsafe static NestedCollectionData ConvertToUnmanaged(IList outerCollection)
	{
		if (outerCollection == null)
		{
			return default(NestedCollectionData);
		}
		int count = outerCollection.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			num += (outerCollection[i] as ICollection<T>)?.Count ?? 0;
		}
		if (num == 0)
		{
			return default(NestedCollectionData);
		}
		NestedCollectionData* ptr = (NestedCollectionData*)BindingsAllocator.Malloc(checked(outerCollection.Count * sizeof(NestedCollectionData) + AlignOfT + num * sizeof(T)));
		NestedCollectionData result = default(NestedCollectionData);
		result.Length = outerCollection.Count;
		result.Data = ptr;
		nuint num2 = (nuint)(ptr + result.Length);
		num2 += (nuint)((nint)AlignOfT - (nint)(num2 % (nuint)AlignOfT));
		T* ptr2 = (T*)num2;
		for (int j = 0; j < count; j++)
		{
			IList<T> list = (IList<T>)outerCollection[j];
			int num3 = (ptr->Length = list?.Count ?? 0);
			if (num3 == 0)
			{
				ptr->Data = null;
			}
			else
			{
				ptr->Data = ptr2;
				IList<T> list2 = list;
				IList<T> list3 = list2;
				if (!(list3 is T[] array))
				{
					if (list3 is List<T> list4)
					{
						NoAllocHelpers.CreateReadOnlySpan(list4).CopyTo(new Span<T>(ptr2, num3));
						ptr2 += num3;
					}
					else
					{
						for (int k = 0; k < num3; k++)
						{
							*(ptr2++) = list[k];
						}
					}
				}
				else
				{
					new Span<T>(array).CopyTo(new Span<T>(ptr2, num3));
					ptr2 += num3;
				}
			}
			ptr++;
		}
		return result;
	}
}
