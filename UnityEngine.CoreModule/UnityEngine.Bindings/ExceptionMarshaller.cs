using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Bindings;

[VisibleToOtherModules]
internal static class ExceptionMarshaller
{
	[ThreadStatic]
	private static Exception s_pendingException;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CheckPendingException()
	{
		Exception ex = s_pendingException;
		if (ex != null)
		{
			s_pendingException = null;
			throw ex;
		}
	}

	[RequiredByNativeCode]
	private static void SetPendingException(Exception ex)
	{
		s_pendingException = ex;
	}
}
