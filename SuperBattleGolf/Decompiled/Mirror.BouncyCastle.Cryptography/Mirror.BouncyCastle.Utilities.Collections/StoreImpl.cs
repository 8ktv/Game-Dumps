using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

internal sealed class StoreImpl<T> : IStore<T>
{
	private readonly List<T> m_contents;

	internal StoreImpl(IEnumerable<T> e)
	{
		m_contents = new List<T>(e);
	}

	IEnumerable<T> IStore<T>.EnumerateMatches(ISelector<T> selector)
	{
		foreach (T content in m_contents)
		{
			if (selector == null || selector.Match(content))
			{
				yield return content;
			}
		}
	}
}
