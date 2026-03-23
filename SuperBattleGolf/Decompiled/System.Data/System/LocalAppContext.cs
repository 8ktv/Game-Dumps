using System.Runtime.CompilerServices;
using System.Threading;

namespace System;

internal class LocalAppContext
{
	private static bool s_isDisableCachingInitialized;

	private static bool s_disableCaching;

	private static object s_syncObject;

	private static bool DisableCaching => LazyInitializer.EnsureInitialized(ref s_disableCaching, ref s_isDisableCachingInitialized, ref s_syncObject, delegate
	{
		AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out var isEnabled);
		return isEnabled;
	});

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool GetCachedSwitchValue(string switchName, ref int switchValue)
	{
		if (switchValue < 0)
		{
			return false;
		}
		if (switchValue > 0)
		{
			return true;
		}
		return GetCachedSwitchValueInternal(switchName, ref switchValue);
	}

	private static bool GetCachedSwitchValueInternal(string switchName, ref int switchValue)
	{
		AppContext.TryGetSwitch(switchName, out var isEnabled);
		if (DisableCaching)
		{
			return isEnabled;
		}
		switchValue = (isEnabled ? 1 : (-1));
		return isEnabled;
	}
}
