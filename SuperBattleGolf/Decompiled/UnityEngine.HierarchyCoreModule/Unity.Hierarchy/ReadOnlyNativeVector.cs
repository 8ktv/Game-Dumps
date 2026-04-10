using System;
using System.Runtime.CompilerServices;

namespace Unity.Hierarchy;

internal readonly struct ReadOnlyNativeVector<T> where T : unmanaged
{
	private readonly IntPtr m_Ptr;

	private readonly int m_Count;

	public int Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return m_Count;
		}
	}

	public unsafe ref readonly T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (index < 0 || index >= m_Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return ref *(T*)((byte*)(void*)m_Ptr + (nint)index * (nint)sizeof(T));
		}
	}

	public ReadOnlyNativeVector(IntPtr ptr, int size)
	{
		m_Ptr = ptr;
		m_Count = size;
	}

	public unsafe ReadOnlySpan<T> AsReadOnlySpan()
	{
		return new ReadOnlySpan<T>((void*)m_Ptr, m_Count);
	}

	public static implicit operator ReadOnlySpan<T>(ReadOnlyNativeVector<T> vector)
	{
		return vector.AsReadOnlySpan();
	}
}
