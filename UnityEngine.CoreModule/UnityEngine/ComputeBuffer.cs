using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeClass("GraphicsBuffer")]
[NativeHeader("Runtime/Export/Graphics/GraphicsBuffer.bindings.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Shaders/GraphicsBuffer.h")]
public sealed class ComputeBuffer : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(ComputeBuffer computeBuffer)
		{
			return computeBuffer.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	public int count
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_count_Injected(intPtr);
		}
	}

	public int stride
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stride_Injected(intPtr);
		}
	}

	private ComputeBufferMode usage
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_usage_Injected(intPtr);
		}
	}

	public string name
	{
		set
		{
			SetName(value);
		}
	}

	~ComputeBuffer()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			DestroyBuffer(this);
		}
		else if (m_Ptr != IntPtr.Zero)
		{
			Debug.LogWarning("GarbageCollector disposing of ComputeBuffer. Please use ComputeBuffer.Release() or .Dispose() to manually release the buffer.");
		}
		m_Ptr = IntPtr.Zero;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsBuffer_Bindings::InitComputeBuffer")]
	private static extern IntPtr InitBuffer(int count, int stride, ComputeBufferType type, ComputeBufferMode usage);

	[FreeFunction("GraphicsBuffer_Bindings::DestroyComputeBuffer")]
	private static void DestroyBuffer(ComputeBuffer buf)
	{
		DestroyBuffer_Injected((buf == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(buf));
	}

	public ComputeBuffer(int count, int stride)
		: this(count, stride, ComputeBufferType.Default, ComputeBufferMode.Immutable, 3)
	{
	}

	public ComputeBuffer(int count, int stride, ComputeBufferType type)
		: this(count, stride, type, ComputeBufferMode.Immutable, 3)
	{
	}

	public ComputeBuffer(int count, int stride, ComputeBufferType type, ComputeBufferMode usage)
		: this(count, stride, type, usage, 3)
	{
	}

	private ComputeBuffer(int count, int stride, ComputeBufferType type, ComputeBufferMode usage, int stackDepth)
	{
		if (count <= 0)
		{
			throw new ArgumentException("Attempting to create a zero length compute buffer", "count");
		}
		if (stride <= 0)
		{
			throw new ArgumentException("Attempting to create a compute buffer with a negative or null stride", "stride");
		}
		long num = (long)count * (long)stride;
		long maxGraphicsBufferSize = SystemInfo.maxGraphicsBufferSize;
		if (num > maxGraphicsBufferSize)
		{
			throw new ArgumentException($"The total size of the compute buffer ({num} bytes) exceeds the maximum buffer size. Maximum supported buffer size: {maxGraphicsBufferSize} bytes.");
		}
		m_Ptr = InitBuffer(count, stride, type, usage);
	}

	public void Release()
	{
		Dispose();
	}

	[FreeFunction("GraphicsBuffer_Bindings::IsValidBuffer")]
	private static bool IsValidBuffer(ComputeBuffer buf)
	{
		return IsValidBuffer_Injected((buf == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(buf));
	}

	public bool IsValid()
	{
		return m_Ptr != IntPtr.Zero && IsValidBuffer(this);
	}

	[SecuritySafeCritical]
	public void SetData(Array data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to ComputeBuffer.SetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		InternalSetData(data, 0, 0, data.Length, UnsafeUtility.SizeOf(data.GetType().GetElementType()));
	}

	[SecuritySafeCritical]
	public void SetData<T>(List<T> data) where T : struct
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsGenericListBlittable<T>())
		{
			throw new ArgumentException($"List<{typeof(T)}> passed to ComputeBuffer.SetData(List<>) must be blittable.\n{UnsafeUtility.GetReasonForGenericListNonBlittable<T>()}");
		}
		InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), 0, 0, NoAllocHelpers.SafeLength(data), Marshal.SizeOf(typeof(T)));
	}

	[SecuritySafeCritical]
	public unsafe void SetData<T>(NativeArray<T> data) where T : struct
	{
		InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
	}

	[SecuritySafeCritical]
	public void SetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to ComputeBuffer.SetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (managedBufferStartIndex:{managedBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalSetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[SecuritySafeCritical]
	public void SetData<T>(List<T> data, int managedBufferStartIndex, int computeBufferStartIndex, int count) where T : struct
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsGenericListBlittable<T>())
		{
			throw new ArgumentException($"List<{typeof(T)}> passed to ComputeBuffer.SetData(List<>) must be blittable.\n{UnsafeUtility.GetReasonForGenericListNonBlittable<T>()}");
		}
		if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (managedBufferStartIndex:{managedBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
	}

	[SecuritySafeCritical]
	public unsafe void SetData<T>(NativeArray<T> data, int nativeBufferStartIndex, int computeBufferStartIndex, int count) where T : struct
	{
		if (nativeBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (nativeBufferStartIndex:{nativeBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr(), nativeBufferStartIndex, computeBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetNativeData", HasExplicitThis = true, ThrowsException = true)]
	private void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int computeBufferStartIndex, int count, int elemSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetNativeData_Injected(intPtr, data, nativeBufferStartIndex, computeBufferStartIndex, count, elemSize);
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetData", HasExplicitThis = true, ThrowsException = true)]
	private void InternalSetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetData_Injected(intPtr, data, managedBufferStartIndex, computeBufferStartIndex, count, elemSize);
	}

	[SecurityCritical]
	public void GetData(Array data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to ComputeBuffer.GetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		InternalGetData(data, 0, 0, data.Length, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[SecurityCritical]
	public void GetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to ComputeBuffer.GetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count argument (managedBufferStartIndex:{managedBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalGetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetData", HasExplicitThis = true, ThrowsException = true)]
	private void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalGetData_Injected(intPtr, data, managedBufferStartIndex, computeBufferStartIndex, count, elemSize);
	}

	private unsafe void* BeginBufferWrite(int offset = 0, int size = 0)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return BeginBufferWrite_Injected(intPtr, offset, size);
	}

	public unsafe NativeArray<T> BeginWrite<T>(int computeBufferStartIndex, int count) where T : struct
	{
		if (!IsValid())
		{
			throw new InvalidOperationException("BeginWrite requires a valid ComputeBuffer");
		}
		if (usage != ComputeBufferMode.SubUpdates)
		{
			throw new ArgumentException("ComputeBuffer must be created with usage mode ComputeBufferMode.SubUpdates to be able to be mapped with BeginWrite");
		}
		int num = UnsafeUtility.SizeOf<T>();
		if (computeBufferStartIndex < 0 || count < 0 || (computeBufferStartIndex + count) * num > this.count * stride)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (computeBufferStartIndex:{computeBufferStartIndex} count:{count} elementSize:{num}, this.count:{this.count}, this.stride{stride})");
		}
		void* dataPointer = BeginBufferWrite(computeBufferStartIndex * num, count * num);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(dataPointer, count, Allocator.Invalid);
	}

	private void EndBufferWrite(int bytesWritten = 0)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EndBufferWrite_Injected(intPtr, bytesWritten);
	}

	public void EndWrite<T>(int countWritten) where T : struct
	{
		if (countWritten < 0)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (countWritten:{countWritten})");
		}
		int num = UnsafeUtility.SizeOf<T>();
		EndBufferWrite(countWritten * num);
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::SetName", HasExplicitThis = true)]
	private unsafe void SetName(string name)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetName_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			SetName_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public void SetCounterValue(uint counterValue)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetCounterValue_Injected(intPtr, counterValue);
	}

	public static void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes)
	{
		CopyCount_Injected((src == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
	}

	public IntPtr GetNativeBufferPtr()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNativeBufferPtr_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DestroyBuffer_Injected(IntPtr buf);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValidBuffer_Injected(IntPtr buf);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_count_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_stride_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ComputeBufferMode get_usage_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetNativeData_Injected(IntPtr _unity_self, IntPtr data, int nativeBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetData_Injected(IntPtr _unity_self, Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetData_Injected(IntPtr _unity_self, Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* BeginBufferWrite_Injected(IntPtr _unity_self, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EndBufferWrite_Injected(IntPtr _unity_self, int bytesWritten);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCounterValue_Injected(IntPtr _unity_self, uint counterValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyCount_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNativeBufferPtr_Injected(IntPtr _unity_self);
}
