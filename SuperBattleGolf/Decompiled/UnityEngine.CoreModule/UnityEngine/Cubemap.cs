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

[NativeHeader("Runtime/Graphics/CubemapTexture.h")]
[ExcludeFromPreset]
public sealed class Cubemap : Texture
{
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

	internal bool isPreProcessed
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isPreProcessed_Injected(intPtr);
		}
	}

	public bool streamingMipmaps
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_streamingMipmaps_Injected(intPtr);
		}
	}

	public int streamingMipmapsPriority
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_streamingMipmapsPriority_Injected(intPtr);
		}
	}

	public int requestedMipmapLevel
	{
		[FreeFunction(Name = "GetTextureStreamingManager().GetRequestedMipmapLevel", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_requestedMipmapLevel_Injected(intPtr);
		}
		[FreeFunction(Name = "GetTextureStreamingManager().SetRequestedMipmapLevel", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_requestedMipmapLevel_Injected(intPtr, value);
		}
	}

	internal bool loadAllMips
	{
		[FreeFunction(Name = "GetTextureStreamingManager().GetLoadAllMips", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_loadAllMips_Injected(intPtr);
		}
		[FreeFunction(Name = "GetTextureStreamingManager().SetLoadAllMips", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_loadAllMips_Injected(intPtr, value);
		}
	}

	public int desiredMipmapLevel
	{
		[FreeFunction(Name = "GetTextureStreamingManager().GetDesiredMipmapLevel", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_desiredMipmapLevel_Injected(intPtr);
		}
	}

	public int loadingMipmapLevel
	{
		[FreeFunction(Name = "GetTextureStreamingManager().GetLoadingMipmapLevel", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_loadingMipmapLevel_Injected(intPtr);
		}
	}

	public int loadedMipmapLevel
	{
		[FreeFunction(Name = "GetTextureStreamingManager().GetLoadedMipmapLevel", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_loadedMipmapLevel_Injected(intPtr);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CubemapScripting::Create")]
	private static extern bool Internal_CreateImpl([Writable] Cubemap mono, int ext, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex);

	private static void Internal_Create([Writable] Cubemap mono, int ext, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex)
	{
		if (!Internal_CreateImpl(mono, ext, mipCount, format, colorSpace, flags, nativeTex))
		{
			throw new UnityException("Failed to create texture because of invalid parameters.");
		}
	}

	[FreeFunction(Name = "CubemapScripting::Apply", HasExplicitThis = true)]
	private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
	}

	[FreeFunction("CubemapScripting::UpdateExternalTexture", HasExplicitThis = true)]
	public void UpdateExternalTexture(IntPtr nativeTexture)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpdateExternalTexture_Injected(intPtr, nativeTexture);
	}

	[NativeName("SetPixel")]
	private void SetPixelImpl(int image, int mip, int x, int y, Color color)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPixelImpl_Injected(intPtr, image, mip, x, y, ref color);
	}

	[NativeName("GetPixel")]
	private Color GetPixelImpl(int image, int mip, int x, int y)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPixelImpl_Injected(intPtr, image, mip, x, y, out var ret);
		return ret;
	}

	[NativeName("FixupEdges")]
	public void SmoothEdges([DefaultValue("1")] int smoothRegionWidthInPixels)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SmoothEdges_Injected(intPtr, smoothRegionWidthInPixels);
	}

	public void SmoothEdges()
	{
		SmoothEdges(1);
	}

	[FreeFunction(Name = "CubemapScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color[] GetPixels(CubemapFace face, int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels_Injected(intPtr, face, miplevel);
	}

	public Color[] GetPixels(CubemapFace face)
	{
		return GetPixels(face, 0);
	}

	[FreeFunction(Name = "CubemapScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetPixels(Color[] colors, CubemapFace face, int miplevel)
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
			SetPixels_Injected(intPtr, ref colors2, face, miplevel);
		}
	}

	[FreeFunction(Name = "CubemapScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImplArray(Array data, int mipLevel, int face, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImplArray_Injected(intPtr, data, mipLevel, face, elementSize, dataArraySize, sourceDataStartIndex);
	}

	[FreeFunction(Name = "CubemapScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImpl(IntPtr data, int mipLevel, int face, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImpl_Injected(intPtr, data, mipLevel, face, elementSize, dataArraySize, sourceDataStartIndex);
	}

	public void SetPixels(Color[] colors, CubemapFace face)
	{
		SetPixels(colors, face, 0);
	}

	[FreeFunction(Name = "CubemapScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Full(Texture src)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Full_Injected(intPtr, MarshalledUnityObject.Marshal(src));
	}

	[FreeFunction(Name = "CubemapScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstFace, int dstMip)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Slice_Injected(intPtr, MarshalledUnityObject.Marshal(src), srcElement, srcMip, dstFace, dstMip);
	}

	[FreeFunction(Name = "CubemapScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstFace, int dstMip, int dstX, int dstY)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Region_Injected(intPtr, MarshalledUnityObject.Marshal(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstFace, dstMip, dstX, dstY);
	}

	private IntPtr GetWritableImageData(int frame)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetWritableImageData_Injected(intPtr, frame);
	}

	[FreeFunction(Name = "GetTextureStreamingManager().ClearRequestedMipmapLevel", HasExplicitThis = true)]
	public void ClearRequestedMipmapLevel()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearRequestedMipmapLevel_Injected(intPtr);
	}

	[FreeFunction(Name = "GetTextureStreamingManager().IsRequestedMipmapLevelLoaded", HasExplicitThis = true)]
	public bool IsRequestedMipmapLevelLoaded()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsRequestedMipmapLevelLoaded_Injected(intPtr);
	}

	internal bool ValidateFormat(TextureFormat format, int width)
	{
		bool flag = ValidateFormat(format);
		if (flag && TextureFormat.PVRTC_RGB2 <= format && format <= TextureFormat.PVRTC_RGBA4 && !Mathf.IsPowerOfTwo(width))
		{
			throw new UnityException($"'{format.ToString()}' demands texture to have power-of-two dimensions");
		}
		return flag;
	}

	internal bool ValidateFormat(GraphicsFormat format, int width)
	{
		bool flag = ValidateFormat(format, GraphicsFormatUsage.Sample);
		if (flag && GraphicsFormatUtility.IsPVRTCFormat(format) && !Mathf.IsPowerOfTwo(width))
		{
			throw new UnityException($"'{format.ToString()}' demands texture to have power-of-two dimensions");
		}
		return flag;
	}

	[ExcludeFromDocs]
	public Cubemap(int width, DefaultFormat format, TextureCreationFlags flags)
		: this(width, SystemInfo.GetGraphicsFormat(format), flags)
	{
	}

	[ExcludeFromDocs]
	public Cubemap(int width, DefaultFormat format, TextureCreationFlags flags, int mipCount)
		: this(width, SystemInfo.GetGraphicsFormat(format), flags, mipCount)
	{
	}

	[RequiredByNativeCode]
	[ExcludeFromDocs]
	public Cubemap(int width, GraphicsFormat format, TextureCreationFlags flags)
		: this(width, format, flags, Texture.GenerateAllMips)
	{
	}

	[ExcludeFromDocs]
	public Cubemap(int width, GraphicsFormat format, TextureCreationFlags flags, int mipCount)
	{
		if (ValidateFormat(format, width))
		{
			ValidateIsNotCrunched(flags);
			Internal_Create(this, width, mipCount, format, GetTextureColorSpace(format), flags, IntPtr.Zero);
		}
	}

	internal Cubemap(int width, TextureFormat textureFormat, int mipCount, IntPtr nativeTex, bool createUninitialized)
	{
		if (ValidateFormat(textureFormat, width))
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
			Internal_Create(this, width, mipCount, graphicsFormat, GetTextureColorSpace(linear: true), textureCreationFlags, nativeTex);
		}
	}

	public Cubemap(int width, TextureFormat textureFormat, bool mipChain)
		: this(width, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, IntPtr.Zero, createUninitialized: false)
	{
	}

	public Cubemap(int width, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool createUninitialized)
		: this(width, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, IntPtr.Zero, createUninitialized)
	{
	}

	public Cubemap(int width, TextureFormat format, int mipCount)
		: this(width, format, mipCount, IntPtr.Zero, createUninitialized: false)
	{
	}

	public Cubemap(int width, TextureFormat format, int mipCount, [DefaultValue("false")] bool createUninitialized)
		: this(width, format, mipCount, IntPtr.Zero, createUninitialized)
	{
	}

	public static Cubemap CreateExternalTexture(int width, TextureFormat format, bool mipmap, IntPtr nativeTex)
	{
		if (nativeTex == IntPtr.Zero)
		{
			throw new ArgumentException("nativeTex can not be null");
		}
		return new Cubemap(width, format, (!mipmap) ? 1 : Texture.GenerateAllMips, nativeTex, createUninitialized: false);
	}

	public void SetPixelData<T>(T[] data, int mipLevel, CubemapFace face, [DefaultValue("0")] int sourceDataStartIndex = 0)
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
		SetPixelDataImplArray(data, mipLevel, (int)face, Marshal.SizeOf(data[0]), data.Length, sourceDataStartIndex);
	}

	public unsafe void SetPixelData<T>(NativeArray<T> data, int mipLevel, CubemapFace face, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
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
		SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr(), mipLevel, (int)face, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
	}

	public unsafe NativeArray<T> GetPixelData<T>(int mipLevel, CubemapFace face) where T : struct
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (mipLevel < 0 || mipLevel >= base.mipmapCount)
		{
			throw new ArgumentException("The passed in miplevel " + mipLevel + " is invalid. The valid range is 0 through " + (base.mipmapCount - 1));
		}
		if (face < CubemapFace.PositiveX || face >= (CubemapFace)6)
		{
			throw new ArgumentException("The passed in face " + face.ToString() + " is invalid. The valid range is 0 through 5.");
		}
		if (GetWritableImageData(0).ToInt64() == 0)
		{
			throw new UnityException("Texture '" + base.name + "' has no data.");
		}
		ulong pixelDataOffset = GetPixelDataOffset(base.mipmapCount, (int)face);
		ulong pixelDataOffset2 = GetPixelDataOffset(mipLevel, (int)face);
		ulong pixelDataSize = GetPixelDataSize(mipLevel, (int)face);
		int num = UnsafeUtility.SizeOf<T>();
		ulong num2 = pixelDataSize / (ulong)num;
		if (num2 > int.MaxValue)
		{
			throw CreateNativeArrayLengthOverflowException();
		}
		IntPtr intPtr = new IntPtr((long)GetWritableImageData(0) + ((long)pixelDataOffset * (long)face + (long)pixelDataOffset2));
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num2, Allocator.None);
	}

	[ExcludeFromDocs]
	public void SetPixel(CubemapFace face, int x, int y, Color color)
	{
		SetPixel(face, x, y, color, 0);
	}

	public void SetPixel(CubemapFace face, int x, int y, Color color, [DefaultValue("0")] int mip)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		SetPixelImpl((int)face, mip, x, y, color);
	}

	[ExcludeFromDocs]
	public Color GetPixel(CubemapFace face, int x, int y)
	{
		return GetPixel(face, x, y, 0);
	}

	public Color GetPixel(CubemapFace face, int x, int y, [DefaultValue("0")] int mip)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelImpl((int)face, mip, x, y);
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

	public void CopyPixels(Texture src, int srcElement, int srcMip, CubemapFace dstFace, int dstMip)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!src.isReadable)
		{
			throw CreateNonReadableException(src);
		}
		CopyPixels_Slice(src, srcElement, srcMip, (int)dstFace, dstMip);
	}

	public void CopyPixels(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, CubemapFace dstFace, int dstMip, int dstX, int dstY)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!src.isReadable)
		{
			throw CreateNonReadableException(src);
		}
		CopyPixels_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, (int)dstFace, dstMip, dstX, dstY);
	}

	private static void ValidateIsNotCrunched(TextureCreationFlags flags)
	{
		if ((flags &= TextureCreationFlags.Crunch) != TextureCreationFlags.None)
		{
			throw new ArgumentException("Crunched Cubemap is not supported for textures created from script.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureFormat get_format_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateExternalTexture_Injected(IntPtr _unity_self, IntPtr nativeTexture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isReadable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixelImpl_Injected(IntPtr _unity_self, int image, int mip, int x, int y, [In] ref Color color);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPixelImpl_Injected(IntPtr _unity_self, int image, int mip, int x, int y, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SmoothEdges_Injected(IntPtr _unity_self, [DefaultValue("1")] int smoothRegionWidthInPixels);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color[] GetPixels_Injected(IntPtr _unity_self, CubemapFace face, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixels_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, CubemapFace face, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int face, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int face, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Full_Injected(IntPtr _unity_self, IntPtr src);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Slice_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int dstFace, int dstMip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Region_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstFace, int dstMip, int dstX, int dstY);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetWritableImageData_Injected(IntPtr _unity_self, int frame);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isPreProcessed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_streamingMipmaps_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_streamingMipmapsPriority_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_requestedMipmapLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_requestedMipmapLevel_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_loadAllMips_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_loadAllMips_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_desiredMipmapLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_loadingMipmapLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_loadedMipmapLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearRequestedMipmapLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsRequestedMipmapLevelLoaded_Injected(IntPtr _unity_self);
}
