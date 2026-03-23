using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
internal static class StringMarshaller
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool TryMarshalEmptyOrNullString(string s, ref ManagedSpanWrapper managedSpanWrapper)
	{
		if (s == null)
		{
			managedSpanWrapper = default(ManagedSpanWrapper);
			return true;
		}
		if (s.Length == 0)
		{
			managedSpanWrapper = new ManagedSpanWrapper((void*)(UIntPtr)1uL, 0);
			return true;
		}
		return false;
	}
}
