using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Windows.WebCam;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("PlatformDependent/Win/Webcam/PhotoCapture.h")]
[MovedFrom("UnityEngine.XR.WSA.WebCam")]
[StaticAccessor("PhotoCapture", StaticAccessorType.DoubleColon)]
public class PhotoCapture : IDisposable
{
	public enum CaptureResultType
	{
		Success,
		UnknownError
	}

	public struct PhotoCaptureResult
	{
		public CaptureResultType resultType;

		public long hResult;

		public bool success => resultType == CaptureResultType.Success;
	}

	public delegate void OnCaptureResourceCreatedCallback(PhotoCapture captureObject);

	public delegate void OnPhotoModeStartedCallback(PhotoCaptureResult result);

	public delegate void OnPhotoModeStoppedCallback(PhotoCaptureResult result);

	public delegate void OnCapturedToDiskCallback(PhotoCaptureResult result);

	public delegate void OnCapturedToMemoryCallback(PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame);

	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(PhotoCapture photoCapture)
		{
			return photoCapture.m_NativePtr;
		}
	}

	internal IntPtr m_NativePtr;

	private static Resolution[] s_SupportedResolutions;

	private static readonly long HR_SUCCESS;

	public static IEnumerable<Resolution> SupportedResolutions
	{
		get
		{
			if (s_SupportedResolutions == null)
			{
				s_SupportedResolutions = GetSupportedResolutions_Internal();
			}
			return s_SupportedResolutions;
		}
	}

	private static PhotoCaptureResult MakeCaptureResult(CaptureResultType resultType, long hResult)
	{
		return new PhotoCaptureResult
		{
			resultType = resultType,
			hResult = hResult
		};
	}

	private static PhotoCaptureResult MakeCaptureResult(long hResult)
	{
		PhotoCaptureResult result = default(PhotoCaptureResult);
		CaptureResultType resultType = ((hResult != HR_SUCCESS) ? CaptureResultType.UnknownError : CaptureResultType.Success);
		result.resultType = resultType;
		result.hResult = hResult;
		return result;
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("GetSupportedResolutions")]
	private static Resolution[] GetSupportedResolutions_Internal()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Resolution[] result;
		try
		{
			GetSupportedResolutions_Internal_Injected(out ret);
		}
		finally
		{
			Resolution[] array = default(Resolution[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public static void CreateAsync(bool showHolograms, OnCaptureResourceCreatedCallback onCreatedCallback)
	{
		if (onCreatedCallback == null)
		{
			throw new ArgumentNullException("onCreatedCallback");
		}
		Instantiate_Internal(showHolograms, onCreatedCallback);
	}

	public static void CreateAsync(OnCaptureResourceCreatedCallback onCreatedCallback)
	{
		if (onCreatedCallback == null)
		{
			throw new ArgumentNullException("onCreatedCallback");
		}
		Instantiate_Internal(showHolograms: false, onCreatedCallback);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("Instantiate")]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	private static extern IntPtr Instantiate_Internal(bool showHolograms, OnCaptureResourceCreatedCallback onCreatedCallback);

	[RequiredByNativeCode]
	private static void InvokeOnCreatedResourceDelegate(OnCaptureResourceCreatedCallback callback, IntPtr nativePtr)
	{
		if (nativePtr == IntPtr.Zero)
		{
			callback(null);
		}
		else
		{
			callback(new PhotoCapture(nativePtr));
		}
	}

	private PhotoCapture(IntPtr nativeCaptureObject)
	{
		m_NativePtr = nativeCaptureObject;
	}

	public void StartPhotoModeAsync(CameraParameters setupParams, OnPhotoModeStartedCallback onPhotoModeStartedCallback)
	{
		if (onPhotoModeStartedCallback == null)
		{
			throw new ArgumentException("onPhotoModeStartedCallback");
		}
		if (setupParams.cameraResolutionWidth == 0 || setupParams.cameraResolutionHeight == 0)
		{
			throw new ArgumentOutOfRangeException("setupParams", "The camera resolution must be set to a supported resolution.");
		}
		StartPhotoMode_Internal(setupParams, onPhotoModeStartedCallback);
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("StartPhotoMode")]
	private void StartPhotoMode_Internal(CameraParameters setupParams, OnPhotoModeStartedCallback onPhotoModeStartedCallback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StartPhotoMode_Internal_Injected(intPtr, ref setupParams, onPhotoModeStartedCallback);
	}

	[RequiredByNativeCode]
	private static void InvokeOnPhotoModeStartedDelegate(OnPhotoModeStartedCallback callback, long hResult)
	{
		callback(MakeCaptureResult(hResult));
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("StopPhotoMode")]
	public void StopPhotoModeAsync(OnPhotoModeStoppedCallback onPhotoModeStoppedCallback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StopPhotoModeAsync_Injected(intPtr, onPhotoModeStoppedCallback);
	}

	[RequiredByNativeCode]
	private static void InvokeOnPhotoModeStoppedDelegate(OnPhotoModeStoppedCallback callback, long hResult)
	{
		callback(MakeCaptureResult(hResult));
	}

	public void TakePhotoAsync(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
	{
		if (onCapturedPhotoToDiskCallback == null)
		{
			throw new ArgumentNullException("onCapturedPhotoToDiskCallback");
		}
		if (string.IsNullOrEmpty(filename))
		{
			throw new ArgumentNullException("filename");
		}
		filename = filename.Replace("/", "\\");
		string directoryName = Path.GetDirectoryName(filename);
		if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
		{
			throw new ArgumentException("The specified directory does not exist.", "filename");
		}
		FileInfo fileInfo = new FileInfo(filename);
		if (fileInfo.Exists && fileInfo.IsReadOnly)
		{
			throw new ArgumentException("Cannot write to the file because it is read-only.", "filename");
		}
		CapturePhotoToDisk_Internal(filename, fileOutputFormat, onCapturedPhotoToDiskCallback);
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("CapturePhotoToDisk")]
	private unsafe void CapturePhotoToDisk_Internal(string filename, PhotoCaptureFileOutputFormat fileOutputFormat, OnCapturedToDiskCallback onCapturedPhotoToDiskCallback)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(filename);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					CapturePhotoToDisk_Internal_Injected(intPtr, ref managedSpanWrapper, fileOutputFormat, onCapturedPhotoToDiskCallback);
					return;
				}
			}
			CapturePhotoToDisk_Internal_Injected(intPtr, ref managedSpanWrapper, fileOutputFormat, onCapturedPhotoToDiskCallback);
		}
		finally
		{
		}
	}

	[RequiredByNativeCode]
	private static void InvokeOnCapturedPhotoToDiskDelegate(OnCapturedToDiskCallback callback, long hResult)
	{
		callback(MakeCaptureResult(hResult));
	}

	public void TakePhotoAsync(OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
	{
		if (onCapturedPhotoToMemoryCallback == null)
		{
			throw new ArgumentNullException("onCapturedPhotoToMemoryCallback");
		}
		CapturePhotoToMemory_Internal(onCapturedPhotoToMemoryCallback);
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("CapturePhotoToMemory")]
	private void CapturePhotoToMemory_Internal(OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CapturePhotoToMemory_Internal_Injected(intPtr, onCapturedPhotoToMemoryCallback);
	}

	[RequiredByNativeCode]
	private static void InvokeOnCapturedPhotoToMemoryDelegate(OnCapturedToMemoryCallback callback, long hResult, IntPtr photoCaptureFramePtr)
	{
		PhotoCaptureFrame photoCaptureFrame = null;
		if (photoCaptureFramePtr != IntPtr.Zero)
		{
			photoCaptureFrame = new PhotoCaptureFrame(photoCaptureFramePtr);
		}
		callback(MakeCaptureResult(hResult), photoCaptureFrame);
	}

	[ThreadAndSerializationSafe]
	[NativeName("GetUnsafePointerToVideoDeviceController")]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	public IntPtr GetUnsafePointerToVideoDeviceController()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetUnsafePointerToVideoDeviceController_Injected(intPtr);
	}

	public void Dispose()
	{
		if (m_NativePtr != IntPtr.Zero)
		{
			Dispose_Internal();
			m_NativePtr = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	[NativeName("Dispose")]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	private void Dispose_Internal()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Dispose_Internal_Injected(intPtr);
	}

	~PhotoCapture()
	{
		if (m_NativePtr != IntPtr.Zero)
		{
			DisposeThreaded_Internal();
			m_NativePtr = IntPtr.Zero;
		}
	}

	[NativeName("DisposeThreaded")]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[ThreadAndSerializationSafe]
	private void DisposeThreaded_Internal()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		DisposeThreaded_Internal_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSupportedResolutions_Internal_Injected(out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StartPhotoMode_Internal_Injected(IntPtr _unity_self, [In] ref CameraParameters setupParams, OnPhotoModeStartedCallback onPhotoModeStartedCallback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopPhotoModeAsync_Injected(IntPtr _unity_self, OnPhotoModeStoppedCallback onPhotoModeStoppedCallback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapturePhotoToDisk_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filename, PhotoCaptureFileOutputFormat fileOutputFormat, OnCapturedToDiskCallback onCapturedPhotoToDiskCallback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapturePhotoToMemory_Internal_Injected(IntPtr _unity_self, OnCapturedToMemoryCallback onCapturedPhotoToMemoryCallback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetUnsafePointerToVideoDeviceController_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Dispose_Internal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisposeThreaded_Internal_Injected(IntPtr _unity_self);
}
