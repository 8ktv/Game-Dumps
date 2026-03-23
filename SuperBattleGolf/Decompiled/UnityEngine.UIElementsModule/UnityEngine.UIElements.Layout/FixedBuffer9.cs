using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.Layout;

[Serializable]
internal struct FixedBuffer9<T> where T : unmanaged
{
	[SerializeField]
	private T __0;

	[SerializeField]
	private T __1;

	[SerializeField]
	private T __2;

	[SerializeField]
	private T __3;

	[SerializeField]
	private T __4;

	[SerializeField]
	private T __5;

	[SerializeField]
	private T __6;

	[SerializeField]
	private T __7;

	[SerializeField]
	private T __8;

	public const int Length = 9;

	public unsafe ref T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (index < 0 || index >= 9)
			{
				throw new IndexOutOfRangeException("index");
			}
			fixed (FixedBuffer9<T>* ptr = &this)
			{
				void* ptr2 = ptr;
				T* ptr3 = (T*)ptr2;
				return ref ptr3[index];
			}
		}
	}
}
