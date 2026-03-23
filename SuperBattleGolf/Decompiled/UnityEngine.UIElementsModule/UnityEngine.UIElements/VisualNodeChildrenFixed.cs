using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements;

[StructLayout(LayoutKind.Explicit, Size = 32)]
internal readonly struct VisualNodeChildrenFixed
{
	private const int k_VisualNodeChildrenFixedCapacity = 4;

	[FieldOffset(0)]
	private readonly VisualNodeHandle __Child0;

	[FieldOffset(8)]
	private readonly VisualNodeHandle __Child1;

	[FieldOffset(16)]
	private readonly VisualNodeHandle __Child2;

	[FieldOffset(24)]
	private readonly VisualNodeHandle __Child3;

	public unsafe int Count
	{
		get
		{
			fixed (VisualNodeHandle* _Child = &__Child0)
			{
				int i;
				for (i = 0; i < 4; i++)
				{
					if (_Child[i].Id == 0)
					{
						return i;
					}
				}
				return i;
			}
		}
	}

	public unsafe VisualNodeHandle this[int index]
	{
		get
		{
			if ((uint)index >= 4u)
			{
				throw new IndexOutOfRangeException("index");
			}
			fixed (VisualNodeHandle* _Child = &__Child0)
			{
				return _Child[index];
			}
		}
	}

	public unsafe VisualNodeHandle* GetUnsafePtr()
	{
		fixed (VisualNodeHandle* _Child = &__Child0)
		{
			return _Child;
		}
	}
}
