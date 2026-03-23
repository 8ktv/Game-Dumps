using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements;

[StructLayout(LayoutKind.Explicit, Size = 32)]
internal readonly struct VisualNodeChildrenAlloc
{
	private const int k_VisualNodeChildrenIsAllocBit = int.MinValue;

	[FieldOffset(0)]
	private readonly IntPtr m_Ptr;

	[FieldOffset(8)]
	private readonly int m_Size;

	[FieldOffset(12)]
	private readonly int m_Capacity;

	[FieldOffset(16)]
	private readonly int m_Reserved;

	public bool IsCreated => (m_Reserved & int.MinValue) != 0;

	public int Count => m_Size;

	public unsafe VisualNodeHandle this[int index]
	{
		get
		{
			if ((uint)index >= m_Size)
			{
				throw new IndexOutOfRangeException("index");
			}
			return GetUnsafePtr()[index];
		}
	}

	public unsafe VisualNodeHandle* GetUnsafePtr()
	{
		return (VisualNodeHandle*)m_Ptr.ToPointer();
	}
}
