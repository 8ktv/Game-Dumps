using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Content;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.IO.Archive;

[RequiredByNativeCode]
[StaticAccessor("GetManagedArchiveSystem()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/VirtualFileSystem/ArchiveFileSystem/ArchiveFileHandle.h")]
public static class ArchiveFileInterface
{
	public unsafe static ArchiveHandle MountAsync(ContentNamespace namespaceId, string filePath, string prefix)
	{
		//The blocks IL_002b, IL_0038, IL_0046, IL_0054, IL_0059 are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0059 are reachable both inside and outside the pinned region starting at IL_0046. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0059 are reachable both inside and outside the pinned region starting at IL_0046. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ArchiveHandle ret = default(ArchiveHandle);
		ArchiveHandle result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper filePath2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(filePath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					filePath2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(prefix);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							MountAsync_Injected(ref namespaceId, ref filePath2, ref managedSpanWrapper2, out ret);
						}
					}
					else
					{
						MountAsync_Injected(ref namespaceId, ref filePath2, ref managedSpanWrapper2, out ret);
					}
				}
			}
			else
			{
				filePath2 = ref managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					readOnlySpan2 = MemoryExtensions.AsSpan(prefix);
					fixed (char* begin2 = readOnlySpan2)
					{
						managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
						MountAsync_Injected(ref namespaceId, ref filePath2, ref managedSpanWrapper2, out ret);
					}
				}
				else
				{
					MountAsync_Injected(ref namespaceId, ref filePath2, ref managedSpanWrapper2, out ret);
				}
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	public static ArchiveHandle[] GetMountedArchives(ContentNamespace namespaceId)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		ArchiveHandle[] result;
		try
		{
			GetMountedArchives_Injected(ref namespaceId, out ret);
		}
		finally
		{
			ArchiveHandle[] array = default(ArchiveHandle[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	internal static ArchiveStatus Archive_GetStatus(ArchiveHandle handle)
	{
		return Archive_GetStatus_Injected(ref handle);
	}

	internal static JobHandle Archive_GetJobHandle(ArchiveHandle handle)
	{
		Archive_GetJobHandle_Injected(ref handle, out var ret);
		return ret;
	}

	internal static bool Archive_IsValid(ArchiveHandle handle)
	{
		return Archive_IsValid_Injected(ref handle);
	}

	internal static JobHandle Archive_UnmountAsync(ArchiveHandle handle)
	{
		Archive_UnmountAsync_Injected(ref handle, out var ret);
		return ret;
	}

	internal static string Archive_GetMountPath(ArchiveHandle handle)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			Archive_GetMountPath_Injected(ref handle, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	internal static CompressionType Archive_GetCompression(ArchiveHandle handle)
	{
		return Archive_GetCompression_Injected(ref handle);
	}

	internal static bool Archive_IsStreamed(ArchiveHandle handle)
	{
		return Archive_IsStreamed_Injected(ref handle);
	}

	internal static ArchiveFileInfo[] Archive_GetFileInfo(ArchiveHandle handle)
	{
		return Archive_GetFileInfo_Injected(ref handle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MountAsync_Injected([In] ref ContentNamespace namespaceId, ref ManagedSpanWrapper filePath, ref ManagedSpanWrapper prefix, out ArchiveHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMountedArchives_Injected([In] ref ContentNamespace namespaceId, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArchiveStatus Archive_GetStatus_Injected([In] ref ArchiveHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Archive_GetJobHandle_Injected([In] ref ArchiveHandle handle, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Archive_IsValid_Injected([In] ref ArchiveHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Archive_UnmountAsync_Injected([In] ref ArchiveHandle handle, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Archive_GetMountPath_Injected([In] ref ArchiveHandle handle, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CompressionType Archive_GetCompression_Injected([In] ref ArchiveHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Archive_IsStreamed_Injected([In] ref ArchiveHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArchiveFileInfo[] Archive_GetFileInfo_Injected([In] ref ArchiveHandle handle);
}
