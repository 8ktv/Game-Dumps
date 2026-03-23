using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Utilities/PropertyName.h")]
internal class PropertyNameUtils
{
	[FreeFunction("PropertyNameFromStringICall", IsThreadSafe = true)]
	public unsafe static PropertyName PropertyNameFromString(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		PropertyName ret = default(PropertyName);
		PropertyName result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					PropertyNameFromString_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				PropertyNameFromString_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PropertyNameFromString_Injected(ref ManagedSpanWrapper name, out PropertyName ret);
}
