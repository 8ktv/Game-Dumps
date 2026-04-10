using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Pool;

[VisibleToOtherModules]
internal readonly ref struct RentSpanUnmanaged<T> where T : unmanaged
{
	private readonly byte[] m_Array;

	public readonly Span<T> Span;

	public RentSpanUnmanaged(int length, bool clear = false)
	{
		int num = length * UnsafeUtility.SizeOf<T>();
		m_Array = ArrayPool<byte>.Shared.Rent(num);
		Span = MemoryMarshal.Cast<byte, T>(new Span<byte>(m_Array, 0, num));
		if (clear)
		{
			Span.Clear();
		}
	}

	public void Dispose()
	{
		ArrayPool<byte>.Shared.Return(m_Array);
	}

	public Span<T>.Enumerator GetEnumerator()
	{
		return Span.GetEnumerator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Span<T>(in RentSpanUnmanaged<T> rentSpan)
	{
		return rentSpan.Span;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ReadOnlySpan<T>(in RentSpanUnmanaged<T> rentSpan)
	{
		return rentSpan.Span;
	}
}
