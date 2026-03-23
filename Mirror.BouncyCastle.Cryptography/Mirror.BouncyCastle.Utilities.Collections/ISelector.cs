using System;

namespace Mirror.BouncyCastle.Utilities.Collections;

public interface ISelector<in T> : ICloneable
{
	bool Match(T candidate);
}
