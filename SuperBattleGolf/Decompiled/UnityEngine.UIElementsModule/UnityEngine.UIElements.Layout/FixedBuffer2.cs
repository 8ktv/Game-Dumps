using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.Layout;

[Serializable]
internal struct FixedBuffer2<T> where T : unmanaged
{
	[SerializeField]
	private T __0;

	[SerializeField]
	private T __1;

	public const int Length = 2;

	public unsafe ref T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (index < 0 || index >= 2)
			{
				throw new IndexOutOfRangeException("index");
			}
			fixed (FixedBuffer2<T>* ptr = &this)
			{
				void* ptr2 = ptr;
				T* ptr3 = (T*)ptr2;
				return ref ptr3[index];
			}
		}
	}
}
