using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal class ReadOnlySetProxy<T> : ReadOnlySet<T>
{
	private readonly ISet<T> m_target;

	public override int Count => m_target.Count;

	internal ReadOnlySetProxy(ISet<T> target)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		m_target = target;
	}

	public override bool Contains(T item)
	{
		return m_target.Contains(item);
	}

	public override void CopyTo(T[] array, int arrayIndex)
	{
		m_target.CopyTo(array, arrayIndex);
	}

	public override IEnumerator<T> GetEnumerator()
	{
		return m_target.GetEnumerator();
	}

	public override bool IsProperSubsetOf(IEnumerable<T> other)
	{
		return m_target.IsProperSubsetOf(other);
	}

	public override bool IsProperSupersetOf(IEnumerable<T> other)
	{
		return m_target.IsProperSupersetOf(other);
	}

	public override bool IsSubsetOf(IEnumerable<T> other)
	{
		return m_target.IsSubsetOf(other);
	}

	public override bool IsSupersetOf(IEnumerable<T> other)
	{
		return m_target.IsSupersetOf(other);
	}

	public override bool Overlaps(IEnumerable<T> other)
	{
		return m_target.Overlaps(other);
	}
}
