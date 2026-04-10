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
[StaticAccessor("VideoCaptureBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("PlatformDependent/Win/Webcam/VideoCaptureBindings.h")]
[MovedFrom("UnityEngine.XR.WSA.WebCam")]
public class VideoCapture : IDisposable
{
	public enum CaptureResultType
	{
		Success,
		UnknownError
	}

	public enum AudioState
	{
		MicAudio,
		ApplicationAudio,
		ApplicationAndMicAudio,
		None
	}

	public struct VideoCaptureResult
	{
		public CaptureResultType resultType;

		public long hResult;

		public bool success => resultType == CaptureResultType.Success;
	}

	public delegate void OnVideoCaptureResourceCreatedCallback(VideoCapture captureObject);

	public delegate void OnVideoModeStartedCallback(VideoCaptureResult result);

	public delegate void OnVideoModeStoppedCallback(VideoCaptureResult result);

	public delegate void OnStartedRecordingVideoCallback(VideoCaptureResult result);

	public delegate void OnStoppedRecordingVideoCallback(VideoCaptureResult result);

	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(VideoCapture videoCapture)
		{
			return videoCapture.m_NativePtr;
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

	public bool IsRecording
	{
		[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
		[NativeMethod("VideoCaptureBindings::IsRecording", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_IsRecording_Injected(intPtr);
		}
	}

	private static VideoCaptureResult MakeCaptureResult(CaptureResultType resultType, long hResult)
	{
		return new VideoCaptureResult
		{
			resultType = resultType,
			hResult = hResult
		};
	}

	private static VideoCaptureResult MakeCaptureResult(long hResult)
	{
		VideoCaptureResult result = default(VideoCaptureResult);
		CaptureResultType resultType = ((hResult != HR_SUCCESS) ? CaptureResultType.UnknownError : CaptureResultType.Success);
		result.resultType = resultType;
		result.hResult = hResult;
		return result;
	}

	[NativeName("GetSupportedResolutions")]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
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

	public static IEnumerable<float> GetSupportedFrameRatesForResolution(Resolution resolution)
	{
		float[] array = null;
		return GetSupportedFrameRatesForResolution_Internal(resolution.width, resolution.height);
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("GetSupportedFrameRatesForResolution")]
	private static float[] GetSupportedFrameRatesForResolution_Internal(int resolutionWidth, int resolutionHeight)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		float[] result;
		try
		{
			GetSupportedFrameRatesForResolution_Internal_Injected(resolutionWidth, resolutionHeight, out ret);
		}
		finally
		{
			float[] array = default(float[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public static void CreateAsync(bool showHolograms, OnVideoCaptureResourceCreatedCallback onCreatedCallback)
	{
		if (onCreatedCallback == null)
		{
			throw new ArgumentNullException("onCreatedCallback");
		}
		Instantiate_Internal(showHolograms, onCreatedCallback);
	}

	public static void CreateAsync(OnVideoCaptureResourceCreatedCallback onCreatedCallback)
	{
		if (onCreatedCallback == null)
		{
			throw new ArgumentNullException("onCreatedCallback");
		}
		Instantiate_Internal(showHolograms: false, onCreatedCallback);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("Instantiate")]
	private static extern void Instantiate_Internal(bool showHolograms, OnVideoCaptureResourceCreatedCallback onCreatedCallback);

	[RequiredByNativeCode]
	private static void InvokeOnCreatedVideoCaptureResourceDelegate(OnVideoCaptureResourceCreatedCallback callback, IntPtr nativePtr)
	{
		if (nativePtr == IntPtr.Zero)
		{
			callback(null);
		}
		else
		{
			callback(new VideoCapture(nativePtr));
		}
	}

	private VideoCapture(IntPtr nativeCaptureObject)
	{
		m_NativePtr = nativeCaptureObject;
	}

	public void StartVideoModeAsync(CameraParameters setupParams, AudioState audioState, OnVideoModeStartedCallback onVideoModeStartedCallback)
	{
		if (onVideoModeStartedCallback == null)
		{
			throw new ArgumentNullException("onVideoModeStartedCallback");
		}
		if (setupParams.cameraResolutionWidth == 0 || setupParams.cameraResolutionHeight == 0)
		{
			throw new ArgumentOutOfRangeException("setupParams", "The camera resolution must be set to a supported resolution.");
		}
		if (setupParams.frameRate == 0f)
		{
			throw new ArgumentOutOfRangeException("setupParams", "The camera frame rate must be set to a supported recording frame rate.");
		}
		StartVideoMode_Internal(setupParams, audioState, onVideoModeStartedCallback);
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeMethod("VideoCaptureBindings::StartVideoMode", HasExplicitThis = true)]
	private void StartVideoMode_Internal(CameraParameters cameraParameters, AudioState audioState, OnVideoModeStartedCallback onVideoModeStartedCallback)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StartVideoMode_Internal_Injected(intPtr, ref cameraParameters, audioState, onVideoModeStartedCallback);
	}

	[RequiredByNativeCode]
	private static void InvokeOnVideoModeStartedDelegate(OnVideoModeStartedCallback callback, long hResult)
	{
		callback(MakeCaptureResult(hResult));
	}

	[NativeMethod("VideoCaptureBindings::StopVideoMode", HasExplicitThis = true)]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	public void StopVideoModeAsync([NotNull] OnVideoModeStoppedCallback onVideoModeStoppedCallback)
	{
		if (onVideoModeStoppedCallback == null)
		{
			ThrowHelper.ThrowArgumentNullException(onVideoModeStoppedCallback, "onVideoModeStoppedCallback");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StopVideoModeAsync_Injected(intPtr, onVideoModeStoppedCallback);
	}

	[RequiredByNativeCode]
	private static void InvokeOnVideoModeStoppedDelegate(OnVideoModeStoppedCallback callback, long hResult)
	{
		callback(MakeCaptureResult(hResult));
	}

	public void StartRecordingAsync(string filename, OnStartedRecordingVideoCallback onStartedRecordingVideoCallback)
	{
		if (onStartedRecordingVideoCallback == null)
		{
			throw new ArgumentNullException("onStartedRecordingVideoCallback");
		}
		if (string.IsNullOrEmpty(filename))
		{
			throw new ArgumentNullException("filename");
		}
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
		StartRecordingVideoToDisk_Internal(fileInfo.FullName, onStartedRecordingVideoCallback);
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeMethod("VideoCaptureBindings::StartRecordingVideoToDisk", HasExplicitThis = true)]
	private unsafe void StartRecordingVideoToDisk_Internal(string filename, OnStartedRecordingVideoCallback onStartedRecordingVideoCallback)
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
					StartRecordingVideoToDisk_Internal_Injected(intPtr, ref managedSpanWrapper, onStartedRecordingVideoCallback);
					return;
				}
			}
			StartRecordingVideoToDisk_Internal_Injected(intPtr, ref managedSpanWrapper, onStartedRecordingVideoCallback);
		}
		finally
		{
		}
	}

	[RequiredByNativeCode]
	private static void InvokeOnStartedRecordingVideoToDiskDelegate(OnStartedRecordingVideoCallback callback, long hResult)
	{
		callback(MakeCaptureResult(hResult));
	}

	[NativeMethod("VideoCaptureBindings::StopRecordingVideoToDisk", HasExplicitThis = true)]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	public void StopRecordingAsync([NotNull] OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback)
	{
		if (onStoppedRecordingVideoCallback == null)
		{
			ThrowHelper.ThrowArgumentNullException(onStoppedRecordingVideoCallback, "onStoppedRecordingVideoCallback");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StopRecordingAsync_Injected(intPtr, onStoppedRecordingVideoCallback);
	}

	[RequiredByNativeCode]
	private static void InvokeOnStoppedRecordingVideoToDiskDelegate(OnStoppedRecordingVideoCallback callback, long hResult)
	{
		callback(MakeCaptureResult(hResult));
	}

	[NativeMethod("VideoCaptureBindings::GetUnsafePointerToVideoDeviceController", HasExplicitThis = true)]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[ThreadAndSerializationSafe]
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

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeMethod("VideoCaptureBindings::Dispose", HasExplicitThis = true)]
	private void Dispose_Internal()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Dispose_Internal_Injected(intPtr);
	}

	~VideoCapture()
	{
		if (m_NativePtr != IntPtr.Zero)
		{
			DisposeThreaded_Internal();
			m_NativePtr = IntPtr.Zero;
		}
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeMethod("VideoCaptureBindings::DisposeThreaded", HasExplicitThis = true)]
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
	private static extern void GetSupportedFrameRatesForResolution_Internal_Injected(int resolutionWidth, int resolutionHeight, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_IsRecording_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StartVideoMode_Internal_Injected(IntPtr _unity_self, [In] ref CameraParameters cameraParameters, AudioState audioState, OnVideoModeStartedCallback onVideoModeStartedCallback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopVideoModeAsync_Injected(IntPtr _unity_self, OnVideoModeStoppedCallback onVideoModeStoppedCallback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StartRecordingVideoToDisk_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper filename, OnStartedRecordingVideoCallback onStartedRecordingVideoCallback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopRecordingAsync_Injected(IntPtr _unity_self, OnStoppedRecordingVideoCallback onStoppedRecordingVideoCallback);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetUnsafePointerToVideoDeviceController_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Dispose_Internal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisposeThreaded_Internal_Injected(IntPtr _unity_self);
}
