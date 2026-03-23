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

[NativeHeader("Runtime/Shaders/GraphicsBuffer.h")]
[NativeHeader("Runtime/Export/Graphics/GraphicsBuffer.bindings.h")]
[UsedByNativeCode]
public sealed class GraphicsBuffer : IDisposable
{
	[Flags]
	public enum Target
	{
		Vertex = 1,
		Index = 2,
		CopySource = 4,
		CopyDestination = 8,
		Structured = 0x10,
		Raw = 0x20,
		Append = 0x40,
		Counter = 0x80,
		IndirectArguments = 0x100,
		Constant = 0x200
	}

	[Flags]
	public enum UsageFlags
	{
		None = 0,
		LockBufferForWrite = 1
	}

	public struct IndirectDrawArgs
	{
		public const int size = 16;

		public uint vertexCountPerInstance { get; set; }

		public uint instanceCount { get; set; }

		public uint startVertex { get; set; }

		public uint startInstance { get; set; }
	}

	public struct IndirectDrawIndexedArgs
	{
		public const int size = 20;

		public uint indexCountPerInstance { get; set; }

		public uint instanceCount { get; set; }

		public uint startIndex { get; set; }

		public uint baseVertexIndex { get; set; }

		public uint startInstance { get; set; }
	}

	internal static class BindingsMarshaller
	{
		public static GraphicsBuffer ConvertToManaged(IntPtr ptr)
		{
			return new GraphicsBuffer(ptr);
		}

		public static IntPtr ConvertToNative(GraphicsBuffer graphicsBuffer)
		{
			return graphicsBuffer.m_Ptr;
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

	public Target target
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_target_Injected(intPtr);
		}
	}

	public UsageFlags usageFlags => GetUsageFlags();

	public GraphicsBufferHandle bufferHandle
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bufferHandle_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public string name
	{
		set
		{
			SetName(value);
		}
	}

	~GraphicsBuffer()
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
			Debug.LogWarning("GarbageCollector disposing of GraphicsBuffer. Please use GraphicsBuffer.Release() or .Dispose() to manually release the buffer.");
		}
		m_Ptr = IntPtr.Zero;
	}

	private static bool RequiresCompute(Target target)
	{
		Target target2 = Target.Structured | Target.Raw | Target.Append | Target.Counter | Target.IndirectArguments;
		return (target & target2) != 0;
	}

	private static bool IsVertexIndexOrCopyOnly(Target target)
	{
		Target target2 = Target.Vertex | Target.Index | Target.CopySource | Target.CopyDestination;
		return (target & target2) == target;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GraphicsBuffer_Bindings::InitBuffer")]
	private static extern IntPtr InitBuffer(Target target, UsageFlags usageFlags, int count, int stride);

	[FreeFunction("GraphicsBuffer_Bindings::DestroyBuffer")]
	private static void DestroyBuffer(GraphicsBuffer buf)
	{
		DestroyBuffer_Injected((buf == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(buf));
	}

	private GraphicsBuffer(IntPtr ptr)
	{
		m_Ptr = ptr;
	}

	public GraphicsBuffer(Target target, int count, int stride)
	{
		InternalInitialization(target, ((target & (Target.Vertex | Target.Index)) == target) ? UsageFlags.LockBufferForWrite : UsageFlags.None, count, stride);
	}

	public GraphicsBuffer(Target target, UsageFlags usageFlags, int count, int stride)
	{
		InternalInitialization(target, usageFlags, count, stride);
	}

	private void InternalInitialization(Target target, UsageFlags usageFlags, int count, int stride)
	{
		if (RequiresCompute(target) && !SystemInfo.supportsComputeShaders)
		{
			throw new ArgumentException("Attempting to create a graphics buffer that requires compute shader support, but compute shaders are not supported on this platform. Target: " + target);
		}
		if (count <= 0)
		{
			throw new ArgumentException("Attempting to create a zero length graphics buffer", "count");
		}
		if (stride <= 0)
		{
			throw new ArgumentException("Attempting to create a graphics buffer with a negative or null stride", "stride");
		}
		if ((target & Target.Index) != 0 && stride != 2 && stride != 4)
		{
			throw new ArgumentException("Attempting to create an index buffer with an invalid stride: " + stride, "stride");
		}
		if (!IsVertexIndexOrCopyOnly(target) && stride % 4 != 0)
		{
			throw new ArgumentException("Stride must be a multiple of 4 unless the buffer is only used as a vertex buffer and/or index buffer ", "stride");
		}
		long num = (long)count * (long)stride;
		long maxGraphicsBufferSize = SystemInfo.maxGraphicsBufferSize;
		if (num > maxGraphicsBufferSize)
		{
			throw new ArgumentException($"The total size of the graphics buffer ({num} bytes) exceeds the maximum buffer size. Maximum supported buffer size: {maxGraphicsBufferSize} bytes.");
		}
		if ((usageFlags & UsageFlags.LockBufferForWrite) != UsageFlags.None && (target & Target.CopyDestination) != 0)
		{
			throw new ArgumentException("Attempting to create a LockBufferForWrite capable buffer that can be copied into. LockBufferForWrite buffers are read-only on the GPU.");
		}
		m_Ptr = InitBuffer(target, usageFlags, count, stride);
	}

	public void Release()
	{
		Dispose();
	}

	[FreeFunction("GraphicsBuffer_Bindings::IsValidBuffer")]
	private static bool IsValidBuffer(GraphicsBuffer buf)
	{
		return IsValidBuffer_Injected((buf == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(buf));
	}

	public bool IsValid()
	{
		return m_Ptr != IntPtr.Zero && IsValidBuffer(this);
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::GetUsageFlags", HasExplicitThis = true)]
	private UsageFlags GetUsageFlags()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetUsageFlags_Injected(intPtr);
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
			throw new ArgumentException($"Array passed to GraphicsBuffer.SetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
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
			throw new ArgumentException($"List<{typeof(T)}> passed to GraphicsBuffer.SetData(List<>) must be blittable.\n{UnsafeUtility.GetReasonForGenericListNonBlittable<T>()}");
		}
		InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), 0, 0, NoAllocHelpers.SafeLength(data), Marshal.SizeOf(typeof(T)));
	}

	[SecuritySafeCritical]
	public unsafe void SetData<T>(NativeArray<T> data) where T : struct
	{
		InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
	}

	[SecuritySafeCritical]
	public void SetData(Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException($"Array passed to GraphicsBuffer.SetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		if (managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (managedBufferStartIndex:{managedBufferStartIndex} graphicsBufferStartIndex:{graphicsBufferStartIndex} count:{count})");
		}
		InternalSetData(data, managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[SecuritySafeCritical]
	public void SetData<T>(List<T> data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (!UnsafeUtility.IsGenericListBlittable<T>())
		{
			throw new ArgumentException($"List<{typeof(T)}> passed to GraphicsBuffer.SetData(List<>) must be blittable.\n{UnsafeUtility.GetReasonForGenericListNonBlittable<T>()}");
		}
		if (managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (managedBufferStartIndex:{managedBufferStartIndex} graphicsBufferStartIndex:{graphicsBufferStartIndex} count:{count})");
		}
		InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
	}

	[SecuritySafeCritical]
	public unsafe void SetData<T>(NativeArray<T> data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
	{
		if (nativeBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (nativeBufferStartIndex:{nativeBufferStartIndex} graphicsBufferStartIndex:{graphicsBufferStartIndex} count:{count})");
		}
		InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr(), nativeBufferStartIndex, graphicsBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
	}

	[SecurityCritical]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetNativeData", HasExplicitThis = true, ThrowsException = true)]
	private void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetNativeData_Injected(intPtr, data, nativeBufferStartIndex, graphicsBufferStartIndex, count, elemSize);
	}

	[SecurityCritical]
	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetData", HasExplicitThis = true, ThrowsException = true)]
	private void InternalSetData(Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetData_Injected(intPtr, data, managedBufferStartIndex, graphicsBufferStartIndex, count, elemSize);
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
			throw new ArgumentException($"Array passed to GraphicsBuffer.GetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
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
			throw new ArgumentException($"Array passed to GraphicsBuffer.GetData(array) must be blittable.\n{UnsafeUtility.GetReasonForArrayNonBlittable(data)}");
		}
		if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count argument (managedBufferStartIndex:{managedBufferStartIndex} computeBufferStartIndex:{computeBufferStartIndex} count:{count})");
		}
		InternalGetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetData", HasExplicitThis = true, ThrowsException = true)]
	[SecurityCritical]
	private void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalGetData_Injected(intPtr, data, managedBufferStartIndex, computeBufferStartIndex, count, elemSize);
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetNativeBufferPtr", HasExplicitThis = true)]
	public IntPtr GetNativeBufferPtr()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNativeBufferPtr_Injected(intPtr);
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

	public unsafe NativeArray<T> LockBufferForWrite<T>(int bufferStartIndex, int count) where T : struct
	{
		if (!IsValid())
		{
			throw new InvalidOperationException("LockBufferForWrite requires a valid GraphicsBuffer");
		}
		if ((usageFlags & UsageFlags.LockBufferForWrite) == 0)
		{
			throw new InvalidOperationException("GraphicsBuffer must be created with usage mode UsageFlage.LockBufferForWrite to use LockBufferForWrite");
		}
		int num = UnsafeUtility.SizeOf<T>();
		if (bufferStartIndex < 0 || count < 0 || (bufferStartIndex + count) * num > this.count * stride)
		{
			throw new ArgumentOutOfRangeException($"Bad indices/count arguments (bufferStartIndex:{bufferStartIndex} count:{count} elementSize:{num}, this.count:{this.count}, this.stride{stride})");
		}
		void* dataPointer = BeginBufferWrite(bufferStartIndex * num, count * num);
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

	public void UnlockBufferAfterWrite<T>(int countWritten) where T : struct
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

	[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
	private static void CopyCountCC(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes)
	{
		CopyCountCC_Injected((src == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
	private static void CopyCountGC(GraphicsBuffer src, ComputeBuffer dst, int dstOffsetBytes)
	{
		CopyCountGC_Injected((src == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
	private static void CopyCountCG(ComputeBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
	{
		CopyCountCG_Injected((src == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
	}

	[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
	private static void CopyCountGG(GraphicsBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
	{
		CopyCountGG_Injected((src == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
	}

	public static void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes)
	{
		CopyCountCC(src, dst, dstOffsetBytes);
	}

	public static void CopyCount(GraphicsBuffer src, ComputeBuffer dst, int dstOffsetBytes)
	{
		CopyCountGC(src, dst, dstOffsetBytes);
	}

	public static void CopyCount(ComputeBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
	{
		CopyCountCG(src, dst, dstOffsetBytes);
	}

	public static void CopyCount(GraphicsBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
	{
		CopyCountGG(src, dst, dstOffsetBytes);
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
	private static extern Target get_target_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern UsageFlags GetUsageFlags_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bufferHandle_Injected(IntPtr _unity_self, out GraphicsBufferHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetNativeData_Injected(IntPtr _unity_self, IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetData_Injected(IntPtr _unity_self, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetData_Injected(IntPtr _unity_self, Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNativeBufferPtr_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* BeginBufferWrite_Injected(IntPtr _unity_self, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EndBufferWrite_Injected(IntPtr _unity_self, int bytesWritten);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCounterValue_Injected(IntPtr _unity_self, uint counterValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyCountCC_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyCountGC_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyCountCG_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyCountGG_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);
}
