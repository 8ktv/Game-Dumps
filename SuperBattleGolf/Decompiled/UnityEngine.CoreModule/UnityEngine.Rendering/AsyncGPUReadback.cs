using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering;

[StaticAccessor("AsyncGPUReadbackManager::GetInstance()", StaticAccessorType.Dot)]
public static class AsyncGPUReadback
{
	internal static void ValidateFormat(Texture src, GraphicsFormat dstformat)
	{
		GraphicsFormat format = GraphicsFormatUtility.GetFormat(src);
		if (!SystemInfo.IsFormatSupported(format, GraphicsFormatUsage.ReadPixels))
		{
			Debug.LogError($"'{format}' doesn't support ReadPixels usage on this platform. Async GPU readback failed.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void WaitAllRequests();

	public unsafe static AsyncGPUReadbackRequest Request(ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback = null)
	{
		AsyncGPUReadbackRequest result = Request_Internal_ComputeBuffer_1(src, null);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest Request(ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback = null)
	{
		AsyncGPUReadbackRequest result = Request_Internal_ComputeBuffer_2(src, size, offset, null);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest Request(GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback = null)
	{
		AsyncGPUReadbackRequest result = Request_Internal_GraphicsBuffer_1(src, null);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest Request(GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback = null)
	{
		AsyncGPUReadbackRequest result = Request_Internal_GraphicsBuffer_2(src, size, offset, null);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest Request(Texture src, int mipIndex = 0, Action<AsyncGPUReadbackRequest> callback = null)
	{
		AsyncGPUReadbackRequest result = Request_Internal_Texture_1(src, mipIndex, null);
		result.SetScriptingCallback(callback);
		return result;
	}

	public static AsyncGPUReadbackRequest Request(Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null)
	{
		return Request(src, mipIndex, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback);
	}

	public unsafe static AsyncGPUReadbackRequest Request(Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null)
	{
		ValidateFormat(src, dstFormat);
		AsyncGPUReadbackRequest result = Request_Internal_Texture_2(src, mipIndex, dstFormat, null);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest Request(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback = null)
	{
		AsyncGPUReadbackRequest result = Request_Internal_Texture_3(src, mipIndex, x, width, y, height, z, depth, null);
		result.SetScriptingCallback(callback);
		return result;
	}

	public static AsyncGPUReadbackRequest Request(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null)
	{
		return Request(src, mipIndex, x, width, y, height, z, depth, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback);
	}

	public unsafe static AsyncGPUReadbackRequest Request(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null)
	{
		ValidateFormat(src, dstFormat);
		AsyncGPUReadbackRequest result = Request_Internal_Texture_4(src, mipIndex, x, width, y, height, z, depth, dstFormat, null);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_ComputeBuffer_1(src, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_ComputeBuffer_2(src, size, offset, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_GraphicsBuffer_1(src, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_GraphicsBuffer_2(src, size, offset, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex = 0, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_Texture_1(src, mipIndex, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		return RequestIntoNativeArray(ref output, src, mipIndex, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback);
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		ValidateFormat(src, dstFormat);
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_Texture_2(src, mipIndex, dstFormat, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		return RequestIntoNativeArray(ref output, src, mipIndex, x, width, y, height, z, depth, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback);
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		ValidateFormat(src, dstFormat);
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_Texture_4(src, mipIndex, x, width, y, height, z, depth, dstFormat, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_ComputeBuffer_1(src, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_ComputeBuffer_2(src, size, offset, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_GraphicsBuffer_1(src, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_GraphicsBuffer_2(src, size, offset, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex = 0, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_Texture_1(src, mipIndex, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		return RequestIntoNativeSlice(ref output, src, mipIndex, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback);
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		ValidateFormat(src, dstFormat);
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_Texture_2(src, mipIndex, dstFormat, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	public static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		return RequestIntoNativeSlice(ref output, src, mipIndex, x, width, y, height, z, depth, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback);
	}

	public unsafe static AsyncGPUReadbackRequest RequestIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback = null) where T : struct
	{
		ValidateFormat(src, dstFormat);
		AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess(output);
		AsyncGPUReadbackRequest result = Request_Internal_Texture_4(src, mipIndex, x, width, y, height, z, depth, dstFormat, &asyncRequestNativeArrayData);
		result.SetScriptingCallback(callback);
		return result;
	}

	[NativeMethod("Request")]
	private unsafe static AsyncGPUReadbackRequest Request_Internal_ComputeBuffer_1([NotNull] ComputeBuffer buffer, AsyncRequestNativeArrayData* data)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		Request_Internal_ComputeBuffer_1_Injected(intPtr, data, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private unsafe static AsyncGPUReadbackRequest Request_Internal_ComputeBuffer_2([NotNull] ComputeBuffer src, int size, int offset, AsyncRequestNativeArrayData* data)
	{
		if (src == null)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(src);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		Request_Internal_ComputeBuffer_2_Injected(intPtr, size, offset, data, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private unsafe static AsyncGPUReadbackRequest Request_Internal_GraphicsBuffer_1([NotNull] GraphicsBuffer buffer, AsyncRequestNativeArrayData* data)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		Request_Internal_GraphicsBuffer_1_Injected(intPtr, data, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private unsafe static AsyncGPUReadbackRequest Request_Internal_GraphicsBuffer_2([NotNull] GraphicsBuffer src, int size, int offset, AsyncRequestNativeArrayData* data)
	{
		if (src == null)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(src);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		Request_Internal_GraphicsBuffer_2_Injected(intPtr, size, offset, data, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private unsafe static AsyncGPUReadbackRequest Request_Internal_Texture_1([NotNull] Texture src, int mipIndex, AsyncRequestNativeArrayData* data)
	{
		if ((object)src == null)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(src);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		Request_Internal_Texture_1_Injected(intPtr, mipIndex, data, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private unsafe static AsyncGPUReadbackRequest Request_Internal_Texture_2([NotNull] Texture src, int mipIndex, GraphicsFormat dstFormat, AsyncRequestNativeArrayData* data)
	{
		if ((object)src == null)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(src);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		Request_Internal_Texture_2_Injected(intPtr, mipIndex, dstFormat, data, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private unsafe static AsyncGPUReadbackRequest Request_Internal_Texture_3([NotNull] Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, AsyncRequestNativeArrayData* data)
	{
		if ((object)src == null)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(src);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		Request_Internal_Texture_3_Injected(intPtr, mipIndex, x, width, y, height, z, depth, data, out var ret);
		return ret;
	}

	[NativeMethod("Request")]
	private unsafe static AsyncGPUReadbackRequest Request_Internal_Texture_4([NotNull] Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, AsyncRequestNativeArrayData* data)
	{
		if ((object)src == null)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(src);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(src, "src");
		}
		Request_Internal_Texture_4_Injected(intPtr, mipIndex, x, width, y, height, z, depth, dstFormat, data, out var ret);
		return ret;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(ComputeBuffer src)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(ComputeBuffer src, int size, int offset)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, size, offset, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(GraphicsBuffer src)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(GraphicsBuffer src, int size, int offset)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, size, offset, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(Texture src, int mipIndex = 0)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, mipIndex, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(Texture src, int mipIndex, TextureFormat dstFormat)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, mipIndex, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(Texture src, int mipIndex, GraphicsFormat dstFormat)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, mipIndex, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, mipIndex, x, width, y, height, z, depth, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, mipIndex, x, width, y, height, z, depth, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestAsync(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat)
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		Request(src, mipIndex, x, width, y, height, z, depth, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, ComputeBuffer src) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, ComputeBuffer src) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, ComputeBuffer src, int size, int offset) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, size, offset, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, ComputeBuffer src, int size, int offset) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, size, offset, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, GraphicsBuffer src) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, GraphicsBuffer src) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, GraphicsBuffer src, int size, int offset) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, size, offset, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, GraphicsBuffer src, int size, int offset) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, size, offset, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, Texture src, int mipIndex = 0) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, mipIndex, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, Texture src, int mipIndex = 0) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, mipIndex, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, Texture src, int mipIndex, TextureFormat dstFormat) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, mipIndex, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, Texture src, int mipIndex, TextureFormat dstFormat) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, mipIndex, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, Texture src, int mipIndex, GraphicsFormat dstFormat) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, mipIndex, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, Texture src, int mipIndex, GraphicsFormat dstFormat) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, mipIndex, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, mipIndex, x, width, y, height, z, depth, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, mipIndex, x, width, y, height, z, depth, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeArrayAsync<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeArray(ref output, src, mipIndex, x, width, y, height, z, depth, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	public static Awaitable<AsyncGPUReadbackRequest> RequestIntoNativeSliceAsync<T>(ref NativeSlice<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat) where T : struct
	{
		Awaitable<AsyncGPUReadbackRequest> managed = Awaitable<AsyncGPUReadbackRequest>.GetManaged();
		RequestIntoNativeSlice(ref output, src, mipIndex, x, width, y, height, z, depth, dstFormat, managed.SetResultAndRaiseContinuation);
		return managed;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Request_Internal_ComputeBuffer_1_Injected(IntPtr buffer, AsyncRequestNativeArrayData* data, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Request_Internal_ComputeBuffer_2_Injected(IntPtr src, int size, int offset, AsyncRequestNativeArrayData* data, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Request_Internal_GraphicsBuffer_1_Injected(IntPtr buffer, AsyncRequestNativeArrayData* data, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Request_Internal_GraphicsBuffer_2_Injected(IntPtr src, int size, int offset, AsyncRequestNativeArrayData* data, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Request_Internal_Texture_1_Injected(IntPtr src, int mipIndex, AsyncRequestNativeArrayData* data, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Request_Internal_Texture_2_Injected(IntPtr src, int mipIndex, GraphicsFormat dstFormat, AsyncRequestNativeArrayData* data, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Request_Internal_Texture_3_Injected(IntPtr src, int mipIndex, int x, int width, int y, int height, int z, int depth, AsyncRequestNativeArrayData* data, out AsyncGPUReadbackRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Request_Internal_Texture_4_Injected(IntPtr src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, AsyncRequestNativeArrayData* data, out AsyncGPUReadbackRequest ret);
}
