using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Texture3D.h")]
[ExcludeFromPreset]
public sealed class Texture3D : Texture
{
	public int depth
	{
		[NativeName("GetTextureLayerCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_depth_Injected(intPtr);
		}
	}

	public TextureFormat format
	{
		[NativeName("GetTextureFormat")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_format_Injected(intPtr);
		}
	}

	public override bool isReadable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isReadable_Injected(intPtr);
		}
	}

	[NativeName("SetPixel")]
	private void SetPixelImpl(int mip, int x, int y, int z, Color color)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPixelImpl_Injected(intPtr, mip, x, y, z, ref color);
	}

	[NativeName("GetPixel")]
	private Color GetPixelImpl(int mip, int x, int y, int z)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPixelImpl_Injected(intPtr, mip, x, y, z, out var ret);
		return ret;
	}

	[NativeName("GetPixelBilinear")]
	private Color GetPixelBilinearImpl(int mip, float u, float v, float w)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPixelBilinearImpl_Injected(intPtr, mip, u, v, w, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Texture3DScripting::Create")]
	private static extern bool Internal_CreateImpl([Writable] Texture3D mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex);

	private static void Internal_Create([Writable] Texture3D mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex)
	{
		if (!Internal_CreateImpl(mono, w, h, d, mipCount, format, colorSpace, flags, nativeTex))
		{
			throw new UnityException("Failed to create texture because of invalid parameters.");
		}
	}

	[FreeFunction("Texture3DScripting::UpdateExternalTexture", HasExplicitThis = true)]
	public void UpdateExternalTexture(IntPtr nativeTex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpdateExternalTexture_Injected(intPtr, nativeTex);
	}

	[FreeFunction(Name = "Texture3DScripting::Apply", HasExplicitThis = true)]
	private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
	}

	[FreeFunction(Name = "Texture3DScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color[] GetPixels(int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels_Injected(intPtr, miplevel);
	}

	public Color[] GetPixels()
	{
		return GetPixels(0);
	}

	[FreeFunction(Name = "Texture3DScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color32[] GetPixels32(int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels32_Injected(intPtr, miplevel);
	}

	public Color32[] GetPixels32()
	{
		return GetPixels32(0);
	}

	[FreeFunction(Name = "Texture3DScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetPixels(Color[] colors, int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Color> span = new Span<Color>(colors);
		fixed (Color* begin = span)
		{
			ManagedSpanWrapper colors2 = new ManagedSpanWrapper(begin, span.Length);
			SetPixels_Injected(intPtr, ref colors2, miplevel);
		}
	}

	public void SetPixels(Color[] colors)
	{
		SetPixels(colors, 0);
	}

	[FreeFunction(Name = "Texture3DScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetPixels32(Color32[] colors, int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Color32> span = new Span<Color32>(colors);
		fixed (Color32* begin = span)
		{
			ManagedSpanWrapper colors2 = new ManagedSpanWrapper(begin, span.Length);
			SetPixels32_Injected(intPtr, ref colors2, miplevel);
		}
	}

	public void SetPixels32(Color32[] colors)
	{
		SetPixels32(colors, 0);
	}

	[FreeFunction(Name = "Texture3DScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImplArray(Array data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImplArray_Injected(intPtr, data, mipLevel, elementSize, dataArraySize, sourceDataStartIndex);
	}

	[FreeFunction(Name = "Texture3DScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImpl(IntPtr data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImpl_Injected(intPtr, data, mipLevel, elementSize, dataArraySize, sourceDataStartIndex);
	}

	[FreeFunction(Name = "Texture3DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Full(Texture src)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Full_Injected(intPtr, MarshalledUnityObject.Marshal(src));
	}

	[FreeFunction(Name = "Texture3DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstElement, int dstMip)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Slice_Injected(intPtr, MarshalledUnityObject.Marshal(src), srcElement, srcMip, dstElement, dstMip);
	}

	[FreeFunction(Name = "Texture3DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Region_Injected(intPtr, MarshalledUnityObject.Marshal(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstElement, dstMip, dstX, dstY);
	}

	private IntPtr GetImageData()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetImageData_Injected(intPtr);
	}

	[ExcludeFromDocs]
	public Texture3D(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags)
		: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags)
	{
	}

	[ExcludeFromDocs]
	public Texture3D(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags, int mipCount)
		: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags, mipCount)
	{
	}

	[RequiredByNativeCode]
	[ExcludeFromDocs]
	public Texture3D(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags)
		: this(width, height, depth, format, flags, Texture.GenerateAllMips)
	{
	}

	[ExcludeFromDocs]
	public Texture3D(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags, [DefaultValue("Texture.GenerateAllMips")] int mipCount)
	{
		if (ValidateFormat(format, GraphicsFormatUsage.Sample))
		{
			ValidateIsNotCrunched(flags);
			Internal_Create(this, width, height, depth, mipCount, format, GetTextureColorSpace(format), flags, IntPtr.Zero);
		}
	}

	[ExcludeFromDocs]
	public Texture3D(int width, int height, int depth, TextureFormat textureFormat, int mipCount)
		: this(width, height, depth, textureFormat, mipCount, IntPtr.Zero)
	{
	}

	public Texture3D(int width, int height, int depth, TextureFormat textureFormat, int mipCount, [DefaultValue("IntPtr.Zero")] IntPtr nativeTex)
		: this(width, height, depth, textureFormat, mipCount, nativeTex, createUninitialized: false)
	{
	}

	public Texture3D(int width, int height, int depth, TextureFormat textureFormat, int mipCount, [DefaultValue("IntPtr.Zero")] IntPtr nativeTex, [DefaultValue("false")] bool createUninitialized)
	{
		if (ValidateFormat(textureFormat))
		{
			GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, isSRGB: false);
			TextureCreationFlags textureCreationFlags = ((mipCount != 1) ? TextureCreationFlags.MipChain : TextureCreationFlags.None);
			if (GraphicsFormatUtility.IsCrunchFormat(textureFormat))
			{
				textureCreationFlags |= TextureCreationFlags.Crunch;
			}
			if (createUninitialized)
			{
				textureCreationFlags |= TextureCreationFlags.DontInitializePixels | TextureCreationFlags.DontUploadUponCreate;
			}
			ValidateIsNotCrunched(textureCreationFlags);
			Internal_Create(this, width, height, depth, mipCount, graphicsFormat, GetTextureColorSpace(linear: true), textureCreationFlags, nativeTex);
		}
	}

	[ExcludeFromDocs]
	public Texture3D(int width, int height, int depth, TextureFormat textureFormat, bool mipChain)
		: this(width, height, depth, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips)
	{
	}

	public Texture3D(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool createUninitialized)
		: this(width, height, depth, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, IntPtr.Zero, createUninitialized)
	{
	}

	public Texture3D(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("IntPtr.Zero")] IntPtr nativeTex)
		: this(width, height, depth, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, nativeTex)
	{
	}

	public static Texture3D CreateExternalTexture(int width, int height, int depth, TextureFormat format, bool mipChain, IntPtr nativeTex)
	{
		if (nativeTex == IntPtr.Zero)
		{
			throw new ArgumentException("nativeTex may not be zero");
		}
		return new Texture3D(width, height, depth, format, (!mipChain) ? 1 : (-1), nativeTex, createUninitialized: false);
	}

	public void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		ApplyImpl(updateMipmaps, makeNoLongerReadable);
	}

	[ExcludeFromDocs]
	public void Apply(bool updateMipmaps)
	{
		Apply(updateMipmaps, makeNoLongerReadable: false);
	}

	[ExcludeFromDocs]
	public void Apply()
	{
		Apply(updateMipmaps: true, makeNoLongerReadable: false);
	}

	[ExcludeFromDocs]
	public void SetPixel(int x, int y, int z, Color color)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		SetPixelImpl(0, x, y, z, color);
	}

	public void SetPixel(int x, int y, int z, Color color, [DefaultValue("0")] int mipLevel)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		SetPixelImpl(mipLevel, x, y, z, color);
	}

	[ExcludeFromDocs]
	public Color GetPixel(int x, int y, int z)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelImpl(0, x, y, z);
	}

	public Color GetPixel(int x, int y, int z, [DefaultValue("0")] int mipLevel)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelImpl(mipLevel, x, y, z);
	}

	[ExcludeFromDocs]
	public Color GetPixelBilinear(float u, float v, float w)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelBilinearImpl(0, u, v, w);
	}

	public Color GetPixelBilinear(float u, float v, float w, [DefaultValue("0")] int mipLevel)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelBilinearImpl(mipLevel, u, v, w);
	}

	public void SetPixelData<T>(T[] data, int mipLevel, [DefaultValue("0")] int sourceDataStartIndex = 0)
	{
		if (sourceDataStartIndex < 0)
		{
			throw new UnityException("SetPixelData: sourceDataStartIndex cannot be less than 0.");
		}
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (data == null || data.Length == 0)
		{
			throw new UnityException("No texture data provided to SetPixelData.");
		}
		SetPixelDataImplArray(data, mipLevel, Marshal.SizeOf(data[0]), data.Length, sourceDataStartIndex);
	}

	public unsafe void SetPixelData<T>(NativeArray<T> data, int mipLevel, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
	{
		if (sourceDataStartIndex < 0)
		{
			throw new UnityException("SetPixelData: sourceDataStartIndex cannot be less than 0.");
		}
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!data.IsCreated || data.Length == 0)
		{
			throw new UnityException("No texture data provided to SetPixelData.");
		}
		SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr(), mipLevel, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
	}

	public unsafe NativeArray<T> GetPixelData<T>(int mipLevel) where T : struct
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (mipLevel < 0 || mipLevel >= base.mipmapCount)
		{
			throw new ArgumentException("The passed in miplevel " + mipLevel + " is invalid. The valid range is 0 through  " + (base.mipmapCount - 1));
		}
		if (GetImageData().ToInt64() == 0)
		{
			throw new UnityException("Texture '" + base.name + "' has no data.");
		}
		ulong pixelDataOffset = GetPixelDataOffset(mipLevel);
		ulong pixelDataSize = GetPixelDataSize(mipLevel);
		int num = UnsafeUtility.SizeOf<T>();
		ulong num2 = pixelDataSize / (ulong)num;
		if (num2 > int.MaxValue)
		{
			throw CreateNativeArrayLengthOverflowException();
		}
		IntPtr intPtr = new IntPtr((long)GetImageData() + (long)pixelDataOffset);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num2, Allocator.None);
	}

	public void CopyPixels(Texture src)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!src.isReadable)
		{
			throw CreateNonReadableException(src);
		}
		CopyPixels_Full(src);
	}

	public void CopyPixels(Texture src, int srcElement, int srcMip, int dstElement, int dstMip)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!src.isReadable)
		{
			throw CreateNonReadableException(src);
		}
		CopyPixels_Slice(src, srcElement, srcMip, dstElement, dstMip);
	}

	public void CopyPixels(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!src.isReadable)
		{
			throw CreateNonReadableException(src);
		}
		CopyPixels_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstElement, dstMip, dstX, dstY);
	}

	private static void ValidateIsNotCrunched(TextureCreationFlags flags)
	{
		if ((flags &= TextureCreationFlags.Crunch) != TextureCreationFlags.None)
		{
			throw new ArgumentException("Crunched Texture3D is not supported.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_depth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureFormat get_format_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isReadable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixelImpl_Injected(IntPtr _unity_self, int mip, int x, int y, int z, [In] ref Color color);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPixelImpl_Injected(IntPtr _unity_self, int mip, int x, int y, int z, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPixelBilinearImpl_Injected(IntPtr _unity_self, int mip, float u, float v, float w, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateExternalTexture_Injected(IntPtr _unity_self, IntPtr nativeTex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color[] GetPixels_Injected(IntPtr _unity_self, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixels_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixels32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Full_Injected(IntPtr _unity_self, IntPtr src);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Slice_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int dstElement, int dstMip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Region_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetImageData_Injected(IntPtr _unity_self);
}
