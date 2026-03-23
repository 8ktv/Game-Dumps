using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal sealed class EnumerableProxy<T> : IEnumerable<T>, IEnumerable
{
	private readonly IEnumerable<T> m_target;

	internal EnumerableProxy(IEnumerable<T> target)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		m_target = target;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return m_target.GetEnumerator();
	}

	public IEnumerator<T> GetEnumerator()
	{
		return m_target.GetEnumerator();
	}
}
