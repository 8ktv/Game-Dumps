using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal class ReadOnlyCollectionProxy<T> : ReadOnlyCollection<T>
{
	private readonly ICollection<T> m_target;

	public override int Count => m_target.Count;

	internal ReadOnlyCollectionProxy(ICollection<T> target)
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
}
