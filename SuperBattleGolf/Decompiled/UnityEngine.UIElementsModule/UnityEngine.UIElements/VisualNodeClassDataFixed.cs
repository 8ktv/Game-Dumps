using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements;

[StructLayout(LayoutKind.Explicit, Size = 32)]
internal readonly struct VisualNodeClassDataFixed
{
	private const int k_VisualNodeClassDataFixedCapacity = 8;

	[FieldOffset(0)]
	private readonly int __Child0;

	[FieldOffset(4)]
	private readonly int __Child1;

	[FieldOffset(8)]
	private readonly int __Child2;

	[FieldOffset(12)]
	private readonly int __Child3;

	[FieldOffset(16)]
	private readonly int __Child4;

	[FieldOffset(20)]
	private readonly int __Child5;

	[FieldOffset(24)]
	private readonly int __Child6;

	[FieldOffset(28)]
	private readonly int __Child7;

	public unsafe int Count
	{
		get
		{
			fixed (int* _Child = &__Child0)
			{
				int i;
				for (i = 0; i < 8; i++)
				{
					if (_Child[i] == 0)
					{
						return i;
					}
				}
				return i;
			}
		}
	}

	public unsafe int this[int index]
	{
		get
		{
			if ((uint)index >= 8u)
			{
				throw new IndexOutOfRangeException("index");
			}
			fixed (int* _Child = &__Child0)
			{
				return _Child[index];
			}
		}
	}

	public unsafe int* GetUnsafePtr()
	{
		fixed (int* _Child = &__Child0)
		{
			return _Child;
		}
	}
}
