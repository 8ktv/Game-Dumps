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

[NativeHeader("Runtime/Graphics/CubemapArrayTexture.h")]
[ExcludeFromPreset]
public sealed class CubemapArray : Texture
{
	public int cubemapCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cubemapCount_Injected(intPtr);
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

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CubemapArrayScripting::Create")]
	private static extern bool Internal_CreateImpl([Writable] CubemapArray mono, int ext, int count, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags);

	private static void Internal_Create([Writable] CubemapArray mono, int ext, int count, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags)
	{
		if (!Internal_CreateImpl(mono, ext, count, mipCount, format, colorSpace, flags))
		{
			throw new UnityException("Failed to create cubemap array texture because of invalid parameters.");
		}
	}

	[FreeFunction(Name = "CubemapArrayScripting::Apply", HasExplicitThis = true)]
	private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
	}

	[FreeFunction(Name = "CubemapArrayScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color[] GetPixels(CubemapFace face, int arrayElement, int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels_Injected(intPtr, face, arrayElement, miplevel);
	}

	public Color[] GetPixels(CubemapFace face, int arrayElement)
	{
		return GetPixels(face, arrayElement, 0);
	}

	[FreeFunction(Name = "CubemapArrayScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color32[] GetPixels32(CubemapFace face, int arrayElement, int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels32_Injected(intPtr, face, arrayElement, miplevel);
	}

	public Color32[] GetPixels32(CubemapFace face, int arrayElement)
	{
		return GetPixels32(face, arrayElement, 0);
	}

	[FreeFunction(Name = "CubemapArrayScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetPixels(Color[] colors, CubemapFace face, int arrayElement, int miplevel)
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
			SetPixels_Injected(intPtr, ref colors2, face, arrayElement, miplevel);
		}
	}

	public void SetPixels(Color[] colors, CubemapFace face, int arrayElement)
	{
		SetPixels(colors, face, arrayElement, 0);
	}

	[FreeFunction(Name = "CubemapArrayScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement, int miplevel)
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
			SetPixels32_Injected(intPtr, ref colors2, face, arrayElement, miplevel);
		}
	}

	public void SetPixels32(Color32[] colors, CubemapFace face, int arrayElement)
	{
		SetPixels32(colors, face, arrayElement, 0);
	}

	[FreeFunction(Name = "CubemapArrayScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImplArray(Array data, int mipLevel, int face, int element, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImplArray_Injected(intPtr, data, mipLevel, face, element, elementSize, dataArraySize, sourceDataStartIndex);
	}

	[FreeFunction(Name = "CubemapArrayScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImpl(IntPtr data, int mipLevel, int face, int element, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImpl_Injected(intPtr, data, mipLevel, face, element, elementSize, dataArraySize, sourceDataStartIndex);
	}

	[FreeFunction(Name = "CubemapArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Full(Texture src)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Full_Injected(intPtr, MarshalledUnityObject.Marshal(src));
	}

	[FreeFunction(Name = "CubemapArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstElement, int dstMip)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Slice_Injected(intPtr, MarshalledUnityObject.Marshal(src), srcElement, srcMip, dstElement, dstMip);
	}

	[FreeFunction(Name = "CubemapArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
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
	public CubemapArray(int width, int cubemapCount, DefaultFormat format, TextureCreationFlags flags)
		: this(width, cubemapCount, SystemInfo.GetGraphicsFormat(format), flags)
	{
	}

	[ExcludeFromDocs]
	public CubemapArray(int width, int cubemapCount, DefaultFormat format, TextureCreationFlags flags, [DefaultValue("Texture.GenerateAllMips")] int mipCount)
		: this(width, cubemapCount, SystemInfo.GetGraphicsFormat(format), flags, mipCount)
	{
	}

	[RequiredByNativeCode]
	public CubemapArray(int width, int cubemapCount, GraphicsFormat format, TextureCreationFlags flags)
		: this(width, cubemapCount, format, flags, Texture.GenerateAllMips)
	{
	}

	[ExcludeFromDocs]
	public CubemapArray(int width, int cubemapCount, GraphicsFormat format, TextureCreationFlags flags, [DefaultValue("Texture.GenerateAllMips")] int mipCount)
	{
		if (ValidateFormat(format, GraphicsFormatUsage.Sample))
		{
			ValidateIsNotCrunched(flags);
			Internal_Create(this, width, cubemapCount, mipCount, format, GetTextureColorSpace(format), flags);
		}
	}

	public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, int mipCount, bool linear, [DefaultValue("false")] bool createUninitialized)
	{
		if (ValidateFormat(textureFormat))
		{
			GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(textureFormat, !linear);
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
			Internal_Create(this, width, cubemapCount, mipCount, graphicsFormat, GetTextureColorSpace(linear), textureCreationFlags);
		}
	}

	public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, int mipCount, bool linear)
		: this(width, cubemapCount, textureFormat, mipCount, linear, createUninitialized: false)
	{
	}

	public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
		: this(width, cubemapCount, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear, createUninitialized)
	{
	}

	[ExcludeFromDocs]
	public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear)
		: this(width, cubemapCount, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear)
	{
	}

	public CubemapArray(int width, int cubemapCount, TextureFormat textureFormat, bool mipChain)
		: this(width, cubemapCount, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear: false)
	{
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

	public void SetPixelData<T>(T[] data, int mipLevel, CubemapFace face, int element, [DefaultValue("0")] int sourceDataStartIndex = 0)
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
		SetPixelDataImplArray(data, mipLevel, (int)face, element, Marshal.SizeOf(data[0]), data.Length, sourceDataStartIndex);
	}

	public unsafe void SetPixelData<T>(NativeArray<T> data, int mipLevel, CubemapFace face, int element, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
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
		SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr(), mipLevel, (int)face, element, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
	}

	public unsafe NativeArray<T> GetPixelData<T>(int mipLevel, CubemapFace face, int element) where T : struct
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
			throw new ArgumentException("The passed in face " + face.ToString() + " is invalid.  The valid range is 0 through 5");
		}
		if (element < 0 || element >= cubemapCount)
		{
			throw new ArgumentException("The passed in element " + element + " is invalid. The valid range is 0 through " + (cubemapCount - 1));
		}
		int num = (int)(element * 6 + face);
		ulong pixelDataOffset = GetPixelDataOffset(base.mipmapCount, num);
		ulong pixelDataOffset2 = GetPixelDataOffset(mipLevel, num);
		ulong pixelDataSize = GetPixelDataSize(mipLevel, num);
		int num2 = UnsafeUtility.SizeOf<T>();
		ulong num3 = pixelDataSize / (ulong)num2;
		if (num3 > int.MaxValue)
		{
			throw CreateNativeArrayLengthOverflowException();
		}
		IntPtr intPtr = new IntPtr((long)GetImageData() + ((long)pixelDataOffset * (long)num + (long)pixelDataOffset2));
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num3, Allocator.None);
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
			throw new ArgumentException("Crunched TextureCubeArray is not supported.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_cubemapCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureFormat get_format_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isReadable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color[] GetPixels_Injected(IntPtr _unity_self, CubemapFace face, int arrayElement, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, CubemapFace face, int arrayElement, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixels_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, CubemapFace face, int arrayElement, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixels32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, CubemapFace face, int arrayElement, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int face, int element, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int face, int element, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Full_Injected(IntPtr _unity_self, IntPtr src);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Slice_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int dstElement, int dstMip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Region_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetImageData_Injected(IntPtr _unity_self);
}
