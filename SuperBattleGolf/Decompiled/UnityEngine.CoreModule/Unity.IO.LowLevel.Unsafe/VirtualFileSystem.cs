using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe;

[NativeHeader("Runtime/VirtualFileSystem/VirtualFileSystem.h")]
[StaticAccessor("GetFileSystem()", StaticAccessorType.Dot)]
public static class VirtualFileSystem
{
	[FreeFunction(IsThreadSafe = true)]
	public unsafe static bool GetLocalFileSystemName(string vfsFileName, out string localFileName, out ulong localFileOffset, out ulong localFileSize)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper localFileName2 = default(ManagedSpanWrapper);
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(vfsFileName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(vfsFileName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetLocalFileSystemName_Injected(ref managedSpanWrapper, out localFileName2, out localFileOffset, out localFileSize);
				}
			}
			return GetLocalFileSystemName_Injected(ref managedSpanWrapper, out localFileName2, out localFileOffset, out localFileSize);
		}
		finally
		{
			localFileName = OutStringMarshaller.GetStringAndDispose(localFileName2);
		}
	}

	internal unsafe static string ToLogicalPath(string physicalPath)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(physicalPath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(physicalPath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					ToLogicalPath_Injected(ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				ToLogicalPath_Injected(ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetLocalFileSystemName_Injected(ref ManagedSpanWrapper vfsFileName, out ManagedSpanWrapper localFileName, out ulong localFileOffset, out ulong localFileSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ToLogicalPath_Injected(ref ManagedSpanWrapper physicalPath, out ManagedSpanWrapper ret);
}
