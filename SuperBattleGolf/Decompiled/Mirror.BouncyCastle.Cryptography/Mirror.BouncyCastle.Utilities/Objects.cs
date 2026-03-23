using System;
using System.Threading;

namespace Mirror.BouncyCastle.Utilities;

public static class Objects
{
	public static int GetHashCode(object obj)
	{
		return obj?.GetHashCode() ?? 0;
	}

	internal static TValue EnsureSingletonInitialized<TValue, TArg>(ref TValue value, TArg arg, Func<TArg, TValue> initialize) where TValue : class
	{
		TValue val = Volatile.Read(ref value);
		if (val != null)
		{
			return val;
		}
		TValue val2 = initialize(arg);
		return Interlocked.CompareExchange(ref value, val2, null) ?? val2;
	}
}
