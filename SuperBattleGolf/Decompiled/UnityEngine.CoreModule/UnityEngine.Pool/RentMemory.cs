using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Pool;

[VisibleToOtherModules]
internal readonly struct RentMemory<T> : IDisposable where T : class
{
	private readonly T[] m_Array;

	public readonly Memory<T> Memory;

	public RentMemory(int length, bool clear = false)
	{
		m_Array = ArrayPool<T>.Shared.Rent(length);
		Memory = MemoryExtensions.AsMemory(m_Array, 0, length);
		if (clear)
		{
			Memory.Span.Clear();
		}
	}

	public Span<T>.Enumerator GetEnumerator()
	{
		return Memory.Span.GetEnumerator();
	}

	public void Dispose()
	{
		ArrayPool<T>.Shared.Return(m_Array);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Memory<T>(in RentMemory<T> rentMemory)
	{
		return rentMemory.Memory;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator ReadOnlyMemory<T>(in RentMemory<T> rentMemory)
	{
		return rentMemory.Memory;
	}
}
