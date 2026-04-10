using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe;

[NativeHeader("Runtime/File/AsyncReadManagerManagedApi.h")]
public static class AsyncReadManager
{
	[ThreadAndSerializationSafe]
	[FreeFunction("AsyncReadManagerManaged::Read", IsThreadSafe = true)]
	private unsafe static ReadHandle ReadInternal(string filename, void* cmds, uint cmdCount, string assetName, ulong typeID, AssetLoadingSubsystem subsystem)
	{
		//The blocks IL_0029, IL_0038, IL_0046, IL_0054, IL_0059 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0059 are reachable both inside and outside the pinned region starting at IL_0046. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0059 are reachable both inside and outside the pinned region starting at IL_0046. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ReadHandle ret = default(ReadHandle);
		ReadHandle result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper filename2;
			void* cmds2;
			uint cmdCount2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(filename);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					filename2 = ref managedSpanWrapper;
					cmds2 = cmds;
					cmdCount2 = cmdCount;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(assetName, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(assetName);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							ReadInternal_Injected(ref filename2, cmds2, cmdCount2, ref managedSpanWrapper2, typeID, subsystem, out ret);
						}
					}
					else
					{
						ReadInternal_Injected(ref filename2, cmds2, cmdCount2, ref managedSpanWrapper2, typeID, subsystem, out ret);
					}
				}
			}
			else
			{
				filename2 = ref managedSpanWrapper;
				cmds2 = cmds;
				cmdCount2 = cmdCount;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetName, ref managedSpanWrapper2))
				{
					readOnlySpan2 = MemoryExtensions.AsSpan(assetName);
					fixed (char* begin2 = readOnlySpan2)
					{
						managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
						ReadInternal_Injected(ref filename2, cmds2, cmdCount2, ref managedSpanWrapper2, typeID, subsystem, out ret);
					}
				}
				else
				{
					ReadInternal_Injected(ref filename2, cmds2, cmdCount2, ref managedSpanWrapper2, typeID, subsystem, out ret);
				}
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	public unsafe static ReadHandle Read(string filename, ReadCommand* readCmds, uint readCmdCount, string assetName = "", ulong typeID = 0uL, AssetLoadingSubsystem subsystem = AssetLoadingSubsystem.Scripts)
	{
		return ReadInternal(filename, readCmds, readCmdCount, assetName, typeID, subsystem);
	}

	[ThreadAndSerializationSafe]
	[FreeFunction("AsyncReadManagerManaged::GetFileInfo", IsThreadSafe = true)]
	private unsafe static ReadHandle GetFileInfoInternal(string filename, void* cmd)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ReadHandle ret = default(ReadHandle);
		ReadHandle result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(filename);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetFileInfoInternal_Injected(ref managedSpanWrapper, cmd, out ret);
				}
			}
			else
			{
				GetFileInfoInternal_Injected(ref managedSpanWrapper, cmd, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	public unsafe static ReadHandle GetFileInfo(string filename, FileInfoResult* result)
	{
		if (result == null)
		{
			throw new NullReferenceException("GetFileInfo must have a valid FileInfoResult to write into.");
		}
		return GetFileInfoInternal(filename, result);
	}

	[ThreadAndSerializationSafe]
	[FreeFunction("AsyncReadManagerManaged::ReadWithHandles_NativePtr", IsThreadSafe = true)]
	private unsafe static ReadHandle ReadWithHandlesInternal_NativePtr(in FileHandle fileHandle, void* readCmdArray, JobHandle dependency)
	{
		ReadWithHandlesInternal_NativePtr_Injected(in fileHandle, readCmdArray, ref dependency, out var ret);
		return ret;
	}

	[FreeFunction("AsyncReadManagerManaged::ReadWithHandles_NativeCopy", IsThreadSafe = true)]
	[ThreadAndSerializationSafe]
	private unsafe static ReadHandle ReadWithHandlesInternal_NativeCopy(in FileHandle fileHandle, void* readCmdArray)
	{
		ReadWithHandlesInternal_NativeCopy_Injected(in fileHandle, readCmdArray, out var ret);
		return ret;
	}

	public unsafe static ReadHandle ReadDeferred(in FileHandle fileHandle, ReadCommandArray* readCmdArray, JobHandle dependency)
	{
		if (!fileHandle.IsValid())
		{
			throw new InvalidOperationException("FileHandle is invalid and may not be read from.");
		}
		return ReadWithHandlesInternal_NativePtr(in fileHandle, readCmdArray, dependency);
	}

	public unsafe static ReadHandle Read(in FileHandle fileHandle, ReadCommandArray readCmdArray)
	{
		if (!fileHandle.IsValid())
		{
			throw new InvalidOperationException("FileHandle is invalid and may not be read from.");
		}
		return ReadWithHandlesInternal_NativeCopy(in fileHandle, UnsafeUtility.AddressOf(ref readCmdArray));
	}

	[FreeFunction("AsyncReadManagerManaged::ScheduleOpenRequest", IsThreadSafe = true)]
	[ThreadAndSerializationSafe]
	private unsafe static FileHandle OpenFileAsync_Internal(string fileName)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		FileHandle ret = default(FileHandle);
		FileHandle result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(fileName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					OpenFileAsync_Internal_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				OpenFileAsync_Internal_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	public static FileHandle OpenFileAsync(string fileName)
	{
		if (fileName.Length == 0)
		{
			throw new InvalidOperationException("FileName is empty");
		}
		return OpenFileAsync_Internal(fileName);
	}

	[ThreadAndSerializationSafe]
	[FreeFunction("AsyncReadManagerManaged::ScheduleCloseRequest", IsThreadSafe = true)]
	internal static JobHandle CloseFileAsync(in FileHandle fileHandle, JobHandle dependency)
	{
		CloseFileAsync_Injected(in fileHandle, ref dependency, out var ret);
		return ret;
	}

	[ThreadAndSerializationSafe]
	[FreeFunction("AsyncReadManagerManaged::ScheduleCloseCachedFileRequest", IsThreadSafe = true)]
	public unsafe static JobHandle CloseCachedFileAsync(string fileName, JobHandle dependency = default(JobHandle))
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		JobHandle ret = default(JobHandle);
		JobHandle result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(fileName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					CloseCachedFileAsync_Injected(ref managedSpanWrapper, ref dependency, out ret);
				}
			}
			else
			{
				CloseCachedFileAsync_Injected(ref managedSpanWrapper, ref dependency, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void ReadInternal_Injected(ref ManagedSpanWrapper filename, void* cmds, uint cmdCount, ref ManagedSpanWrapper assetName, ulong typeID, AssetLoadingSubsystem subsystem, out ReadHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void GetFileInfoInternal_Injected(ref ManagedSpanWrapper filename, void* cmd, out ReadHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void ReadWithHandlesInternal_NativePtr_Injected(in FileHandle fileHandle, void* readCmdArray, [In] ref JobHandle dependency, out ReadHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void ReadWithHandlesInternal_NativeCopy_Injected(in FileHandle fileHandle, void* readCmdArray, out ReadHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OpenFileAsync_Internal_Injected(ref ManagedSpanWrapper fileName, out FileHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CloseFileAsync_Injected(in FileHandle fileHandle, [In] ref JobHandle dependency, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CloseCachedFileAsync_Injected(ref ManagedSpanWrapper fileName, [In] ref JobHandle dependency, out JobHandle ret);
}
