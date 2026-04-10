using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Burst.LowLevel;

[StaticAccessor("BurstCompilerService::Get()", StaticAccessorType.Arrow)]
[NativeHeader("Runtime/Burst/Burst.h")]
[NativeHeader("Runtime/Burst/BurstDelegateCache.h")]
internal static class BurstCompilerService
{
	public delegate bool ExtractCompilerFlags(Type jobType, out string flags);

	public enum BurstLogType
	{
		Info,
		Warning,
		Error
	}

	public static extern bool IsInitialized
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeMethod("Initialize")]
	private unsafe static string InitializeInternal(string path, ExtractCompilerFlags extractCompilerFlags)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					InitializeInternal_Injected(ref managedSpanWrapper, extractCompilerFlags, out ret);
				}
			}
			else
			{
				InitializeInternal_Injected(ref managedSpanWrapper, extractCompilerFlags, out ret);
			}
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[ThreadSafe]
	public unsafe static string GetDisassembly(MethodInfo m, string compilerOptions)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(compilerOptions, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(compilerOptions);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetDisassembly_Injected(m, ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				GetDisassembly_Injected(m, ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction(IsThreadSafe = true)]
	public unsafe static int CompileAsyncDelegateMethod(object delegateMethod, string compilerOptions)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(compilerOptions, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(compilerOptions);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return CompileAsyncDelegateMethod_Injected(delegateMethod, ref managedSpanWrapper);
				}
			}
			return CompileAsyncDelegateMethod_Injected(delegateMethod, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(IsThreadSafe = true)]
	public unsafe static extern void* GetAsyncCompiledAsyncDelegateMethod(int userID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void* GetOrCreateSharedMemory(ref Hash128 key, uint size_of, uint alignment);

	[ThreadSafe]
	public static string GetMethodSignature(MethodInfo method)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetMethodSignature_Injected(method, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetCurrentExecutionMode(uint environment);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern uint GetCurrentExecutionMode();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("DefaultBurstLogCallback", true)]
	public unsafe static extern void Log(void* userData, BurstLogType logType, byte* message, byte* filename, int lineNumber);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("DefaultBurstRuntimeLogCallback", true)]
	public unsafe static extern void RuntimeLog(void* userData, BurstLogType logType, byte* message, byte* filename, int lineNumber);

	public unsafe static bool LoadBurstLibrary(string fullPathToLibBurstGenerated)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(fullPathToLibBurstGenerated, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(fullPathToLibBurstGenerated);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return LoadBurstLibrary_Injected(ref managedSpanWrapper);
				}
			}
			return LoadBurstLibrary_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public static void Initialize(string folderRuntime, ExtractCompilerFlags extractCompilerFlags)
	{
		if (folderRuntime == null)
		{
			throw new ArgumentNullException("folderRuntime");
		}
		if (extractCompilerFlags == null)
		{
			throw new ArgumentNullException("extractCompilerFlags");
		}
		if (!Directory.Exists(folderRuntime))
		{
			Debug.LogError("Unable to initialize the burst JIT compiler. The folder `" + folderRuntime + "` does not exist");
			return;
		}
		string text = InitializeInternal(folderRuntime, extractCompilerFlags);
		if (!string.IsNullOrEmpty(text))
		{
			Debug.LogError("Unexpected error while trying to initialize the burst JIT compiler: " + text);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InitializeInternal_Injected(ref ManagedSpanWrapper path, ExtractCompilerFlags extractCompilerFlags, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDisassembly_Injected(MethodInfo m, ref ManagedSpanWrapper compilerOptions, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CompileAsyncDelegateMethod_Injected(object delegateMethod, ref ManagedSpanWrapper compilerOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMethodSignature_Injected(MethodInfo method, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool LoadBurstLibrary_Injected(ref ManagedSpanWrapper fullPathToLibBurstGenerated);
}
