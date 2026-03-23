using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Mirror;

public class ConcurrentPool<T>
{
	private readonly ConcurrentBag<T> objects = new ConcurrentBag<T>();

	private readonly Func<T> objectGenerator;

	public int Count => objects.Count;

	public ConcurrentPool(Func<T> objectGenerator, int initialCapacity)
	{
		this.objectGenerator = objectGenerator;
		for (int i = 0; i < initialCapacity; i++)
		{
			objects.Add(objectGenerator());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T Get()
	{
		if (!objects.TryTake(out var result))
		{
			return objectGenerator();
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Return(T item)
	{
		objects.Add(item);
	}
}
