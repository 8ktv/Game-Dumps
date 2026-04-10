using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.Scripting;

namespace Unity.Profiling.Memory;

[NativeHeader("Runtime/Profiler/Runtime/MemorySnapshotManager.h")]
public static class MemoryProfiler
{
	private static event Action<string, bool> m_SnapshotFinished;

	private static event Action<string, bool, DebugScreenCapture> m_SaveScreenshotToDisk;

	public static event Action<MemorySnapshotMetadata> CreatingMetadata;

	[NativeMethod("StartOperation")]
	[StaticAccessor("profiling::memory::GetMemorySnapshotManager()", StaticAccessorType.Dot)]
	[NativeConditional("ENABLE_PROFILER")]
	private unsafe static void StartOperation(uint captureFlags, bool requestScreenshot, string path, bool isRemote)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					StartOperation_Injected(captureFlags, requestScreenshot, ref managedSpanWrapper, isRemote);
					return;
				}
			}
			StartOperation_Injected(captureFlags, requestScreenshot, ref managedSpanWrapper, isRemote);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("ENABLE_PROFILER")]
	[NativeMethod("RequestEditorTakeSnapshotOfPlayer")]
	[StaticAccessor("profiling::memory::GetMemorySnapshotManager()", StaticAccessorType.Dot)]
	private static extern void RequestEditorTakeSnapshotOfPlayer(uint captureFlags, bool requestScreenshot);

	public static void TakeSnapshot(string path, Action<string, bool> finishCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
	{
		TakeSnapshot(path, finishCallback, null, captureFlags);
	}

	public static void TakeSnapshot(string path, Action<string, bool> finishCallback, Action<string, bool, DebugScreenCapture> screenshotCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
	{
		if (MemoryProfiler.m_SnapshotFinished != null)
		{
			Debug.LogWarning("Canceling snapshot, there is another snapshot in progress.");
			finishCallback(path, arg2: false);
			return;
		}
		m_SnapshotFinished += finishCallback;
		m_SaveScreenshotToDisk += screenshotCallback;
		if (string.IsNullOrEmpty(path) && PlayerConnection.instance.isConnected)
		{
			RequestEditorTakeSnapshotOfPlayer((uint)captureFlags, MemoryProfiler.m_SaveScreenshotToDisk != null);
		}
		else
		{
			StartOperation((uint)captureFlags, MemoryProfiler.m_SaveScreenshotToDisk != null, path, isRemote: false);
		}
	}

	public static void TakeTempSnapshot(Action<string, bool> finishCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
	{
		string text = Application.dataPath.Split('/')[^2];
		string path = Application.temporaryCachePath + "/" + text + ".snap";
		TakeSnapshot(path, finishCallback, captureFlags);
	}

	[RequiredByNativeCode]
	private unsafe static byte[] PrepareMetadata()
	{
		if (MemoryProfiler.CreatingMetadata == null)
		{
			return new byte[0];
		}
		MemorySnapshotMetadata memorySnapshotMetadata = new MemorySnapshotMetadata();
		memorySnapshotMetadata.Description = string.Empty;
		MemoryProfiler.CreatingMetadata(memorySnapshotMetadata);
		if (memorySnapshotMetadata.Description == null)
		{
			memorySnapshotMetadata.Description = "";
		}
		int num = 2 * memorySnapshotMetadata.Description.Length;
		int num2 = ((memorySnapshotMetadata.Data != null) ? memorySnapshotMetadata.Data.Length : 0);
		int num3 = num + num2 + 12;
		byte[] array = new byte[num3];
		int offset = 0;
		offset = WriteIntToByteArray(array, offset, memorySnapshotMetadata.Description.Length);
		offset = WriteStringToByteArray(array, offset, memorySnapshotMetadata.Description);
		offset = WriteIntToByteArray(array, offset, num2);
		fixed (byte* data = memorySnapshotMetadata.Data)
		{
			fixed (byte* ptr = array)
			{
				byte* destination = ptr + offset;
				UnsafeUtility.MemCpy(destination, data, num2);
			}
		}
		return array;
	}

	internal unsafe static int WriteIntToByteArray(byte[] array, int offset, int value)
	{
		byte* ptr = (byte*)(&value);
		array[offset++] = *ptr;
		array[offset++] = ptr[1];
		array[offset++] = ptr[2];
		array[offset++] = ptr[3];
		return offset;
	}

	internal unsafe static int WriteStringToByteArray(byte[] array, int offset, string value)
	{
		if (value.Length != 0)
		{
			fixed (char* ptr = value)
			{
				char* ptr2 = ptr;
				for (char* ptr3 = ptr + value.Length; ptr2 != ptr3; ptr2++)
				{
					for (int i = 0; i < 2; i++)
					{
						array[offset++] = ((byte*)ptr2)[i];
					}
				}
			}
		}
		return offset;
	}

	[RequiredByNativeCode]
	private static void FinalizeSnapshot(string path, bool result)
	{
		if (MemoryProfiler.m_SnapshotFinished != null)
		{
			Action<string, bool> snapshotFinished = MemoryProfiler.m_SnapshotFinished;
			MemoryProfiler.m_SnapshotFinished = null;
			snapshotFinished(path, result);
		}
	}

	[RequiredByNativeCode]
	private unsafe static void SaveScreenshotToDisk(string path, bool result, IntPtr pixelsPtr, int pixelsCount, TextureFormat format, int width, int height)
	{
		if (MemoryProfiler.m_SaveScreenshotToDisk != null)
		{
			Action<string, bool, DebugScreenCapture> saveScreenshotToDisk = MemoryProfiler.m_SaveScreenshotToDisk;
			MemoryProfiler.m_SaveScreenshotToDisk = null;
			DebugScreenCapture arg = default(DebugScreenCapture);
			if (result)
			{
				NativeArray<byte> rawImageDataReference = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(pixelsPtr.ToPointer(), pixelsCount, Allocator.Persistent);
				arg.RawImageDataReference = rawImageDataReference;
				arg.Height = height;
				arg.Width = width;
				arg.ImageFormat = format;
			}
			saveScreenshotToDisk(path, result, arg);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StartOperation_Injected(uint captureFlags, bool requestScreenshot, ref ManagedSpanWrapper path, bool isRemote);
}
