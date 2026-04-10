using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Windows.WebCam;

[NativeHeader("PlatformDependent/Win/Webcam/PhotoCaptureFrame.h")]
[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
[MovedFrom("UnityEngine.XR.WSA.WebCam")]
public sealed class PhotoCaptureFrame : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(PhotoCaptureFrame photoCaptureFrame)
		{
			return photoCaptureFrame.m_NativePtr;
		}
	}

	private IntPtr m_NativePtr;

	public int dataLength { get; private set; }

	public bool hasLocationData { get; private set; }

	public CapturePixelFormat pixelFormat { get; private set; }

	[ThreadAndSerializationSafe]
	private int GetDataLength()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDataLength_Injected(intPtr);
	}

	[ThreadAndSerializationSafe]
	private bool GetHasLocationData()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetHasLocationData_Injected(intPtr);
	}

	[ThreadAndSerializationSafe]
	private CapturePixelFormat GetCapturePixelFormat()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCapturePixelFormat_Injected(intPtr);
	}

	public bool TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix)
	{
		cameraToWorldMatrix = Matrix4x4.identity;
		if (hasLocationData)
		{
			cameraToWorldMatrix = GetCameraToWorldMatrix();
			return true;
		}
		return false;
	}

	[ThreadAndSerializationSafe]
	[NativeName("GetCameraToWorld")]
	[NativeConditional("PLATFORM_WIN && !PLATFORM_XBOXONE", "Matrix4x4f()")]
	private Matrix4x4 GetCameraToWorldMatrix()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetCameraToWorldMatrix_Injected(intPtr, out var ret);
		return ret;
	}

	public bool TryGetProjectionMatrix(out Matrix4x4 projectionMatrix)
	{
		if (hasLocationData)
		{
			projectionMatrix = GetProjection();
			return true;
		}
		projectionMatrix = Matrix4x4.identity;
		return false;
	}

	public bool TryGetProjectionMatrix(float nearClipPlane, float farClipPlane, out Matrix4x4 projectionMatrix)
	{
		if (hasLocationData)
		{
			float num = 0.01f;
			if (nearClipPlane < num)
			{
				nearClipPlane = num;
			}
			if (farClipPlane < nearClipPlane + num)
			{
				farClipPlane = nearClipPlane + num;
			}
			projectionMatrix = GetProjection();
			float num2 = 1f / (farClipPlane - nearClipPlane);
			float m = (0f - (farClipPlane + nearClipPlane)) * num2;
			float m2 = (0f - 2f * farClipPlane * nearClipPlane) * num2;
			projectionMatrix.m22 = m;
			projectionMatrix.m23 = m2;
			return true;
		}
		projectionMatrix = Matrix4x4.identity;
		return false;
	}

	[NativeConditional("PLATFORM_WIN && !PLATFORM_XBOXONE", "Matrix4x4f()")]
	[ThreadAndSerializationSafe]
	private Matrix4x4 GetProjection()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetProjection_Injected(intPtr, out var ret);
		return ret;
	}

	public void UploadImageDataToTexture(Texture2D targetTexture)
	{
		if (targetTexture == null)
		{
			throw new ArgumentNullException("targetTexture");
		}
		if (pixelFormat != CapturePixelFormat.BGRA32)
		{
			throw new ArgumentException("Uploading PhotoCaptureFrame to a texture is only supported with BGRA32 CameraFrameFormat!");
		}
		UploadImageDataToTexture_Internal(targetTexture);
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("UploadImageDataToTexture")]
	[ThreadAndSerializationSafe]
	private void UploadImageDataToTexture_Internal(Texture2D targetTexture)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UploadImageDataToTexture_Internal_Injected(intPtr, Object.MarshalledUnityObject.Marshal(targetTexture));
	}

	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[ThreadAndSerializationSafe]
	public IntPtr GetUnsafePointerToBuffer()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetUnsafePointerToBuffer_Injected(intPtr);
	}

	public void CopyRawImageDataIntoBuffer(List<byte> byteBuffer)
	{
		if (byteBuffer == null)
		{
			throw new ArgumentNullException("byteBuffer");
		}
		byte[] array = new byte[dataLength];
		CopyRawImageDataIntoBuffer_Internal(array);
		if (byteBuffer.Capacity < array.Length)
		{
			byteBuffer.Capacity = array.Length;
		}
		byteBuffer.Clear();
		byteBuffer.AddRange(array);
	}

	[NativeName("CopyRawImageDataIntoBuffer")]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[ThreadAndSerializationSafe]
	internal unsafe void CopyRawImageDataIntoBuffer_Internal([Out] byte[] byteArray)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper byteArray2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (byteArray != null)
			{
				fixed (byte[] array = byteArray)
				{
					if (array.Length != 0)
					{
						byteArray2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					CopyRawImageDataIntoBuffer_Internal_Injected(intPtr, out byteArray2);
					return;
				}
			}
			CopyRawImageDataIntoBuffer_Internal_Injected(intPtr, out byteArray2);
		}
		finally
		{
			byteArray2.Unmarshal(ref array);
		}
	}

	internal PhotoCaptureFrame(IntPtr nativePtr)
	{
		m_NativePtr = nativePtr;
		dataLength = GetDataLength();
		hasLocationData = GetHasLocationData();
		pixelFormat = GetCapturePixelFormat();
		GC.AddMemoryPressure(dataLength);
	}

	private void Cleanup()
	{
		if (m_NativePtr != IntPtr.Zero)
		{
			GC.RemoveMemoryPressure(dataLength);
			Dispose_Internal();
			m_NativePtr = IntPtr.Zero;
		}
	}

	[ThreadAndSerializationSafe]
	[NativeConditional("(PLATFORM_WIN || PLATFORM_WINRT) && !PLATFORM_XBOXONE")]
	[NativeName("Dispose")]
	private void Dispose_Internal()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Dispose_Internal_Injected(intPtr);
	}

	public void Dispose()
	{
		Cleanup();
		GC.SuppressFinalize(this);
	}

	~PhotoCaptureFrame()
	{
		Cleanup();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDataLength_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetHasLocationData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CapturePixelFormat GetCapturePixelFormat_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCameraToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetProjection_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UploadImageDataToTexture_Internal_Injected(IntPtr _unity_self, IntPtr targetTexture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetUnsafePointerToBuffer_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyRawImageDataIntoBuffer_Internal_Injected(IntPtr _unity_self, out BlittableArrayWrapper byteArray);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Dispose_Internal_Injected(IntPtr _unity_self);
}
