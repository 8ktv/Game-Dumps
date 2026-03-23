using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements;

[StructLayout(LayoutKind.Explicit, Size = 32)]
internal readonly struct VisualNodeClassData : IEnumerable<int>, IEnumerable
{
	public struct Enumerator : IEnumerator<int>, IEnumerator, IDisposable
	{
		private unsafe int* m_Ptr;

		private int m_Count;

		private int m_Index;

		public unsafe int Current => m_Ptr[m_Index];

		object IEnumerator.Current => Current;

		public unsafe Enumerator(int* ptr, int count)
		{
			m_Ptr = ptr;
			m_Count = count;
			m_Index = -1;
		}

		public bool MoveNext()
		{
			return ++m_Index < m_Count;
		}

		public void Reset()
		{
			m_Index = -1;
		}

		public unsafe void Dispose()
		{
			m_Ptr = null;
		}
	}

	[FieldOffset(0)]
	private readonly VisualNodeClassDataFixed m_Fixed;

	[FieldOffset(0)]
	private readonly VisualNodeClassDataAlloc m_Alloc;

	public int Count => m_Alloc.IsCreated ? m_Alloc.Count : m_Fixed.Count;

	public int this[int index] => m_Alloc.IsCreated ? m_Alloc[index] : m_Fixed[index];

	public unsafe int* GetUnsafePtr()
	{
		fixed (VisualNodeClassData* result = &this)
		{
			return (int*)result;
		}
	}

	public unsafe Enumerator GetEnumerator()
	{
		return new Enumerator(GetUnsafePtr(), Count);
	}

	unsafe IEnumerator<int> IEnumerable<int>.GetEnumerator()
	{
		return new Enumerator(GetUnsafePtr(), Count);
	}

	unsafe IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(GetUnsafePtr(), Count);
	}
}
