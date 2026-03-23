using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal class ReadOnlyListProxy<T> : ReadOnlyList<T>
{
	private readonly IList<T> m_target;

	public override int Count => m_target.Count;

	internal ReadOnlyListProxy(IList<T> target)
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

	public override int IndexOf(T item)
	{
		return m_target.IndexOf(item);
	}

	protected override T Lookup(int index)
	{
		return m_target[index];
	}
}
