using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements.UIR;

[NativeHeader("Modules/UIElements/Core/Native/Renderer/UIRendererUtility.h")]
[VisibleToOtherModules(new string[] { "Unity.UIElements" })]
internal class Utility
{
	internal enum GPUBufferType
	{
		Vertex,
		Index
	}

	public class GPUBuffer<T> : IDisposable where T : struct
	{
		private IntPtr buffer;

		private int elemCount;

		private int elemStride;

		public int ElementStride => elemStride;

		public int Count => elemCount;

		internal IntPtr BufferPointer => buffer;

		public GPUBuffer(int elementCount, GPUBufferType type)
		{
			elemCount = elementCount;
			elemStride = UnsafeUtility.SizeOf<T>();
			buffer = AllocateBuffer(elementCount, elemStride, type == GPUBufferType.Vertex);
		}

		public void Dispose()
		{
			FreeBuffer(buffer);
		}

		public unsafe void UpdateRanges(NativeSlice<GfxUpdateBufferRange> ranges, int rangesMin, int rangesMax)
		{
			UpdateBufferRanges(buffer, new IntPtr(ranges.GetUnsafePtr()), ranges.Length, rangesMin, rangesMax);
		}
	}

	private static ProfilerMarker s_MarkerRaiseEngineUpdate = new ProfilerMarker("UIR.RaiseEngineUpdate");

	public static event Action<bool> GraphicsResourcesRecreate;

	public static event Action EngineUpdate;

	public static event Action FlushPendingResources;

	public unsafe static void SetVectorArray<T>(MaterialPropertyBlock props, int name, NativeSlice<T> vector4s) where T : struct
	{
		int count = vector4s.Length * vector4s.Stride / 16;
		SetVectorArray(props, name, new IntPtr(vector4s.GetUnsafePtr()), count);
	}

	[RequiredByNativeCode]
	internal static void RaiseGraphicsResourcesRecreate(bool recreate)
	{
		Utility.GraphicsResourcesRecreate?.Invoke(recreate);
	}

	[RequiredByNativeCode]
	internal static void RaiseEngineUpdate()
	{
		if (Utility.EngineUpdate != null)
		{
			Utility.EngineUpdate();
		}
	}

	[RequiredByNativeCode]
	internal static void RaiseFlushPendingResources()
	{
		Utility.FlushPendingResources?.Invoke();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private static extern IntPtr AllocateBuffer(int elementCount, int elementStride, bool vertexBuffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private static extern void FreeBuffer(IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private static extern void UpdateBufferRanges(IntPtr buffer, IntPtr ranges, int rangeCount, int writeRangeStart, int writeRangeEnd);

	[ThreadSafe]
	private static void SetVectorArray(MaterialPropertyBlock props, int name, IntPtr vector4s, int count)
	{
		SetVectorArray_Injected((props == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(props), name, vector4s, count);
	}

	[ThreadSafe]
	public unsafe static IntPtr GetVertexDeclaration(VertexAttributeDescriptor[] vertexAttributes)
	{
		Span<VertexAttributeDescriptor> span = new Span<VertexAttributeDescriptor>(vertexAttributes);
		IntPtr vertexDeclaration_Injected;
		fixed (VertexAttributeDescriptor* begin = span)
		{
			ManagedSpanWrapper vertexAttributes2 = new ManagedSpanWrapper(begin, span.Length);
			vertexDeclaration_Injected = GetVertexDeclaration_Injected(ref vertexAttributes2);
		}
		return vertexDeclaration_Injected;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void DrawRanges(IntPtr ib, IntPtr* vertexStreams, int streamCount, IntPtr ranges, int rangeCount, IntPtr vertexDecl);

	[ThreadSafe]
	public static void SetPropertyBlock(MaterialPropertyBlock props)
	{
		SetPropertyBlock_Injected((props == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(props));
	}

	[ThreadSafe]
	public static void SetScissorRect(RectInt scissorRect)
	{
		SetScissorRect_Injected(ref scissorRect);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void DisableScissor();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool IsScissorEnabled();

	[ThreadSafe]
	public static IntPtr CreateStencilState(StencilState stencilState)
	{
		return CreateStencilState_Injected(ref stencilState);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStencilState(IntPtr stencilState, int stencilRef);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool HasMappedBufferRange();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern uint InsertCPUFence();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool CPUFencePassed(uint fence);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void WaitForCPUFencePassed(uint fence);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SyncRenderThread();

	[ThreadSafe]
	public static RectInt GetActiveViewport()
	{
		GetActiveViewport_Injected(out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void ProfileDrawChainBegin();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void ProfileDrawChainEnd();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void NotifyOfUIREvents(bool subscribe);

	[ThreadSafe]
	public static Matrix4x4 GetUnityProjectionMatrix()
	{
		GetUnityProjectionMatrix_Injected(out var ret);
		return ret;
	}

	[ThreadSafe]
	public static Matrix4x4 GetDeviceProjectionMatrix()
	{
		GetDeviceProjectionMatrix_Injected(out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool DebugIsMainThread();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVectorArray_Injected(IntPtr props, int name, IntPtr vector4s, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetVertexDeclaration_Injected(ref ManagedSpanWrapper vertexAttributes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPropertyBlock_Injected(IntPtr props);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetScissorRect_Injected([In] ref RectInt scissorRect);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateStencilState_Injected([In] ref StencilState stencilState);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetActiveViewport_Injected(out RectInt ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetUnityProjectionMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDeviceProjectionMatrix_Injected(out Matrix4x4 ret);
}
