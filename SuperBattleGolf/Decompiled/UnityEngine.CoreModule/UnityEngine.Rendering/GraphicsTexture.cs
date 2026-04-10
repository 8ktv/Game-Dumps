using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[NativeType("Runtime/Graphics/Texture/GraphicsTexture.h")]
[NativeHeader("Runtime/Export/Graphics/GraphicsTexture.bindings.h")]
[UsedByNativeCode]
public class GraphicsTexture : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(GraphicsTexture graphicsTexture)
		{
			return graphicsTexture.m_Ptr;
		}

		public static GraphicsTexture ConvertToManaged(IntPtr ptr)
		{
			return new GraphicsTexture(ptr);
		}
	}

	internal IntPtr m_Ptr;

	public GraphicsTextureDescriptor descriptor
	{
		[FreeFunction("GraphicsTexture_Bindings::GetDescriptor", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_descriptor_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public GraphicsTextureState state
	{
		[FreeFunction("GraphicsTexture_Bindings::GetState", HasExplicitThis = true, IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_state_Injected(intPtr);
		}
	}

	public static GraphicsTexture active
	{
		get
		{
			return GetActive();
		}
		set
		{
			SetActive(value);
		}
	}

	private GraphicsTexture(IntPtr ptr)
	{
		m_Ptr = ptr;
	}

	~GraphicsTexture()
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
			ReleaseBuffer();
		}
		else
		{
			ReleaseBufferOnMain();
		}
		m_Ptr = IntPtr.Zero;
	}

	public GraphicsTexture(GraphicsTextureDescriptor desc)
	{
		m_Ptr = InitBuffer(desc);
	}

	internal void UploadData(IntPtr data, int size)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("GraphicsTexture");
		}
		if (data == IntPtr.Zero || size == 0)
		{
			Debug.LogError("No texture data provided to GraphicsTexture.UploadData");
		}
		else
		{
			UploadBuffer(data, (ulong)size);
		}
	}

	internal void UploadData(byte[] data)
	{
		if (m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("GraphicsTexture");
		}
		if (data == null || data.Length == 0)
		{
			Debug.LogError("No texture data provided to GraphicsTexture.UploadData");
		}
		else
		{
			UploadBuffer_Array(data);
		}
	}

	[FreeFunction("GraphicsTexture_Bindings::GetActive")]
	private static GraphicsTexture GetActive()
	{
		IntPtr active_Injected = GetActive_Injected();
		return (active_Injected == (IntPtr)0) ? null : BindingsMarshaller.ConvertToManaged(active_Injected);
	}

	[FreeFunction("RenderTextureScripting::SetActive")]
	private static void SetActive(GraphicsTexture target)
	{
		SetActive_Injected((target == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(target));
	}

	[FreeFunction("GraphicsTexture_Bindings::InitBuffer", ThrowsException = true)]
	private static IntPtr InitBuffer(GraphicsTextureDescriptor desc)
	{
		return InitBuffer_Injected(ref desc);
	}

	[FreeFunction("GraphicsTexture_Bindings::ReleaseBuffer", HasExplicitThis = true, IsThreadSafe = true)]
	private void ReleaseBuffer()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReleaseBuffer_Injected(intPtr);
	}

	[FreeFunction("GraphicsTexture_Bindings::ReleaseBufferOnMain", HasExplicitThis = true, IsThreadSafe = true)]
	private void ReleaseBufferOnMain()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReleaseBufferOnMain_Injected(intPtr);
	}

	[FreeFunction("GraphicsTexture_Bindings::UploadBuffer", HasExplicitThis = true, ThrowsException = true)]
	private bool UploadBuffer(IntPtr data, ulong size)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return UploadBuffer_Injected(intPtr, data, size);
	}

	[FreeFunction("GraphicsTexture_Bindings::UploadBuffer", HasExplicitThis = true, ThrowsException = true)]
	private unsafe bool UploadBuffer_Array(byte[] data)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<byte> span = new Span<byte>(data);
		bool result;
		fixed (byte* begin = span)
		{
			ManagedSpanWrapper data2 = new ManagedSpanWrapper(begin, span.Length);
			result = UploadBuffer_Array_Injected(intPtr, ref data2);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_descriptor_Injected(IntPtr _unity_self, out GraphicsTextureDescriptor ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GraphicsTextureState get_state_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetActive_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetActive_Injected(IntPtr target);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr InitBuffer_Injected([In] ref GraphicsTextureDescriptor desc);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ReleaseBuffer_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ReleaseBufferOnMain_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UploadBuffer_Injected(IntPtr _unity_self, IntPtr data, ulong size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool UploadBuffer_Array_Injected(IntPtr _unity_self, ref ManagedSpanWrapper data);
}
