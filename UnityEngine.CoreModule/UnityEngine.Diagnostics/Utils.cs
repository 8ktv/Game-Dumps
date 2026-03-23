using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Diagnostics;

[NativeHeader("Runtime/Export/Diagnostics/DiagnosticsUtils.bindings.h")]
[NativeHeader("Runtime/Misc/GarbageCollectSharedAssets.h")]
public static class Utils
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("DiagnosticsUtils_Bindings::ForceCrash", IsThreadSafe = true, ThrowsException = true)]
	public static extern void ForceCrash(ForcedCrashCategory crashCategory);

	[FreeFunction("DiagnosticsUtils_Bindings::NativeAssert", IsThreadSafe = true)]
	public unsafe static void NativeAssert(string message)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(message);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					NativeAssert_Injected(ref managedSpanWrapper);
					return;
				}
			}
			NativeAssert_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("DiagnosticsUtils_Bindings::NativeError", IsThreadSafe = true)]
	public unsafe static void NativeError(string message)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(message);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					NativeError_Injected(ref managedSpanWrapper);
					return;
				}
			}
			NativeError_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("DiagnosticsUtils_Bindings::NativeWarning", IsThreadSafe = true)]
	public unsafe static void NativeWarning(string message)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(message);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					NativeWarning_Injected(ref managedSpanWrapper);
					return;
				}
			}
			NativeWarning_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ValidateHeap")]
	public static extern void ValidateHeap();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void NativeAssert_Injected(ref ManagedSpanWrapper message);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void NativeError_Injected(ref ManagedSpanWrapper message);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void NativeWarning_Injected(ref ManagedSpanWrapper message);
}
