using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Pool;

[VisibleToOtherModules]
internal readonly ref struct RentSpan<T> where T : class
{
	private readonly T[] m_Array;

	public readonly Span<T> Span;

	public RentSpan(int length, bool clear = false)
	{
		m_Array = ArrayPool<T>.Shared.Rent(length);
		Span = MemoryExtensions.AsSpan(m_Array, 0, length);
		if (clear)
		{
			Span.Clear();
		}
	}

	public void Dispose()
	{
		ArrayPool<T>.Shared.Return(m_Array);
	}

	public Span<T>.Enumerator GetEnumerator()
	{
		return Span.GetEnumerator();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Span<T>(in RentSpan<T> rentSpan)
	{
		return rentSpan.Span;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ReadOnlySpan<T>(in RentSpan<T> rentSpan)
	{
		return rentSpan.Span;
	}

	public Memory<T> AsMemory()
	{
		return new Memory<T>(m_Array, 0, Span.Length);
	}
}
