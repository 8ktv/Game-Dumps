using System.Collections.Generic;

namespace Mirror.BouncyCastle.Utilities.Collections;

public interface IStore<out T>
{
	IEnumerable<T> EnumerateMatches(ISelector<T> selector);
}
