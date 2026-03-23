using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal abstract class ReadOnlySet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public bool IsReadOnly => true;

	public abstract int Count { get; }

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection<T>.Add(T item)
	{
		throw new NotSupportedException();
	}

	public bool Add(T item)
	{
		throw new NotSupportedException();
	}

	public void Clear()
	{
		throw new NotSupportedException();
	}

	public void ExceptWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	public bool Remove(T item)
	{
		throw new NotSupportedException();
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	public void UnionWith(IEnumerable<T> other)
	{
		throw new NotSupportedException();
	}

	public abstract bool Contains(T item);

	public abstract void CopyTo(T[] array, int arrayIndex);

	public abstract IEnumerator<T> GetEnumerator();

	public abstract bool IsProperSubsetOf(IEnumerable<T> other);

	public abstract bool IsProperSupersetOf(IEnumerable<T> other);

	public abstract bool IsSubsetOf(IEnumerable<T> other);

	public abstract bool IsSupersetOf(IEnumerable<T> other);

	public abstract bool Overlaps(IEnumerable<T> other);
}
