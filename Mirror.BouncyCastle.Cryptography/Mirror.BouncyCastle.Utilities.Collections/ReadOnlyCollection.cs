using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal abstract class ReadOnlyCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	public bool IsReadOnly => true;

	public abstract int Count { get; }

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Add(T item)
	{
		throw new NotSupportedException();
	}

	public void Clear()
	{
		throw new NotSupportedException();
	}

	public bool Remove(T item)
	{
		throw new NotSupportedException();
	}

	public abstract bool Contains(T item);

	public abstract void CopyTo(T[] array, int arrayIndex);

	public abstract IEnumerator<T> GetEnumerator();
}
