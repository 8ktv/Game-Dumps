using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal abstract class ReadOnlyList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public T this[int index]
	{
		get
		{
			return Lookup(index);
		}
		set
		{
			throw new NotSupportedException();
		}
	}

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

	public void Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	public bool Remove(T item)
	{
		throw new NotSupportedException();
	}

	public void RemoveAt(int index)
	{
		throw new NotSupportedException();
	}

	public abstract bool Contains(T item);

	public abstract void CopyTo(T[] array, int arrayIndex);

	public abstract IEnumerator<T> GetEnumerator();

	public abstract int IndexOf(T item);

	protected abstract T Lookup(int index);
}
