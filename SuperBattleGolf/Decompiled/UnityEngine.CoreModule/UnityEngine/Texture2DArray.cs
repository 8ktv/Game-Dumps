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

[NativeHeader("Runtime/Graphics/Texture2DArray.h")]
[ExcludeFromPreset]
public sealed class Texture2DArray : Texture
{
	public static extern int allSlices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetAllTextureLayersIdentifier")]
		get;
	}

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

	public string mipmapLimitGroup
	{
		[NativeName("GetMipmapLimitGroupName")]
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_mipmapLimitGroup_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
	}

	public int activeMipmapLimit
	{
		[NativeName("GetMipmapLimit")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_activeMipmapLimit_Injected(intPtr);
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

	public bool ignoreMipmapLimit
	{
		get
		{
			return IgnoreMipmapLimit();
		}
		set
		{
			if (!isReadable)
			{
				throw IgnoreMipmapLimitCannotBeToggledException(this);
			}
			SetIgnoreMipmapLimitAndReload(value);
		}
	}

	private bool IgnoreMipmapLimit()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IgnoreMipmapLimit_Injected(intPtr);
	}

	private void SetIgnoreMipmapLimitAndReload(bool value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIgnoreMipmapLimitAndReload_Injected(intPtr, value);
	}

	[FreeFunction("Texture2DArrayScripting::Create")]
	private unsafe static bool Internal_CreateImpl([Writable] Texture2DArray mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, bool ignoreMipmapLimit, string mipmapLimitGroupName)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(mipmapLimitGroupName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(mipmapLimitGroupName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return Internal_CreateImpl_Injected(mono, w, h, d, mipCount, format, colorSpace, flags, ignoreMipmapLimit, ref managedSpanWrapper);
				}
			}
			return Internal_CreateImpl_Injected(mono, w, h, d, mipCount, format, colorSpace, flags, ignoreMipmapLimit, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	private static void Internal_Create([Writable] Texture2DArray mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, bool ignoreMipmapLimit, string mipmapLimitGroupName)
	{
		if (!Internal_CreateImpl(mono, w, h, d, mipCount, format, colorSpace, flags, ignoreMipmapLimit, mipmapLimitGroupName))
		{
			throw new UnityException("Failed to create 2D array texture because of invalid parameters.");
		}
	}

	[FreeFunction(Name = "Texture2DArrayScripting::Apply", HasExplicitThis = true)]
	private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
	}

	[FreeFunction(Name = "Texture2DArrayScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public Color[] GetPixels(int arrayElement, int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels_Injected(intPtr, arrayElement, miplevel);
	}

	public Color[] GetPixels(int arrayElement)
	{
		return GetPixels(arrayElement, 0);
	}

	[FreeFunction(Name = "Texture2DArrayScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImplArray(Array data, int mipLevel, int element, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImplArray_Injected(intPtr, data, mipLevel, element, elementSize, dataArraySize, sourceDataStartIndex);
	}

	[FreeFunction(Name = "Texture2DArrayScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImpl(IntPtr data, int mipLevel, int element, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImpl_Injected(intPtr, data, mipLevel, element, elementSize, dataArraySize, sourceDataStartIndex);
	}

	[FreeFunction(Name = "Texture2DArrayScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public Color32[] GetPixels32(int arrayElement, int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels32_Injected(intPtr, arrayElement, miplevel);
	}

	public Color32[] GetPixels32(int arrayElement)
	{
		return GetPixels32(arrayElement, 0);
	}

	[FreeFunction(Name = "Texture2DArrayScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetPixels(Color[] colors, int arrayElement, int miplevel)
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
			SetPixels_Injected(intPtr, ref colors2, arrayElement, miplevel);
		}
	}

	public void SetPixels(Color[] colors, int arrayElement)
	{
		SetPixels(colors, arrayElement, 0);
	}

	[FreeFunction(Name = "Texture2DArrayScripting::SetPixels32", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetPixels32(Color32[] colors, int arrayElement, int miplevel)
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
			SetPixels32_Injected(intPtr, ref colors2, arrayElement, miplevel);
		}
	}

	public void SetPixels32(Color32[] colors, int arrayElement)
	{
		SetPixels32(colors, arrayElement, 0);
	}

	[FreeFunction(Name = "Texture2DArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Full(Texture src)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Full_Injected(intPtr, MarshalledUnityObject.Marshal(src));
	}

	[FreeFunction(Name = "Texture2DArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstElement, int dstMip)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Slice_Injected(intPtr, MarshalledUnityObject.Marshal(src), srcElement, srcMip, dstElement, dstMip);
	}

	[FreeFunction(Name = "Texture2DArrayScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
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

	internal bool ValidateFormat(TextureFormat format, int width, int height)
	{
		bool flag = ValidateFormat(format);
		if (flag && TextureFormat.PVRTC_RGB2 <= format && format <= TextureFormat.PVRTC_RGBA4 && (width != height || !Mathf.IsPowerOfTwo(width)))
		{
			throw new UnityException($"'{format.ToString()}' demands texture to be square and have power-of-two dimensions");
		}
		return flag;
	}

	internal bool ValidateFormat(GraphicsFormat format, int width, int height)
	{
		bool flag = ValidateFormat(format, GraphicsFormatUsage.Sample);
		if (flag && GraphicsFormatUtility.IsPVRTCFormat(format) && (width != height || !Mathf.IsPowerOfTwo(width)))
		{
			throw new UnityException($"'{format.ToString()}' demands texture to be square and have power-of-two dimensions");
		}
		return flag;
	}

	[ExcludeFromDocs]
	public Texture2DArray(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags)
		: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags)
	{
	}

	[ExcludeFromDocs]
	public Texture2DArray(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags, int mipCount)
		: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags, mipCount)
	{
	}

	[ExcludeFromDocs]
	public Texture2DArray(int width, int height, int depth, DefaultFormat format, TextureCreationFlags flags, int mipCount, MipmapLimitDescriptor mipmapLimitDescriptor)
		: this(width, height, depth, SystemInfo.GetGraphicsFormat(format), flags, mipCount, mipmapLimitDescriptor)
	{
	}

	[RequiredByNativeCode]
	public Texture2DArray(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags)
		: this(width, height, depth, format, flags, Texture.GenerateAllMips, default(MipmapLimitDescriptor))
	{
	}

	[ExcludeFromDocs]
	public Texture2DArray(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags, int mipCount)
		: this(width, height, depth, format, flags, mipCount, default(MipmapLimitDescriptor))
	{
	}

	[ExcludeFromDocs]
	public Texture2DArray(int width, int height, int depth, GraphicsFormat format, TextureCreationFlags flags, int mipCount, MipmapLimitDescriptor mipmapLimitDescriptor)
	{
		if (ValidateFormat(format, width, height))
		{
			ValidateIsNotCrunched(flags);
			Internal_Create(this, width, height, depth, mipCount, format, GetTextureColorSpace(format), flags, !mipmapLimitDescriptor.useMipmapLimit, mipmapLimitDescriptor.groupName);
		}
	}

	public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, int mipCount, bool linear, bool createUninitialized, MipmapLimitDescriptor mipmapLimitDescriptor)
	{
		if (ValidateFormat(textureFormat, width, height))
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
			Internal_Create(this, width, height, depth, mipCount, graphicsFormat, GetTextureColorSpace(linear), textureCreationFlags, !mipmapLimitDescriptor.useMipmapLimit, mipmapLimitDescriptor.groupName);
		}
	}

	public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, int mipCount, bool linear, bool createUninitialized)
		: this(width, height, depth, textureFormat, mipCount, linear, createUninitialized, default(MipmapLimitDescriptor))
	{
	}

	public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, int mipCount, bool linear)
		: this(width, height, depth, textureFormat, mipCount, linear, createUninitialized: false, default(MipmapLimitDescriptor))
	{
	}

	public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
		: this(width, height, depth, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear, createUninitialized, default(MipmapLimitDescriptor))
	{
	}

	public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain, [DefaultValue("false")] bool linear)
		: this(width, height, depth, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear)
	{
	}

	[ExcludeFromDocs]
	public Texture2DArray(int width, int height, int depth, TextureFormat textureFormat, bool mipChain)
		: this(width, height, depth, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear: false)
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

	public void SetPixelData<T>(T[] data, int mipLevel, int element, [DefaultValue("0")] int sourceDataStartIndex = 0)
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
		SetPixelDataImplArray(data, mipLevel, element, Marshal.SizeOf(data[0]), data.Length, sourceDataStartIndex);
	}

	public unsafe void SetPixelData<T>(NativeArray<T> data, int mipLevel, int element, [DefaultValue("0")] int sourceDataStartIndex = 0) where T : struct
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
		SetPixelDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr(), mipLevel, element, UnsafeUtility.SizeOf<T>(), data.Length, sourceDataStartIndex);
	}

	public unsafe NativeArray<T> GetPixelData<T>(int mipLevel, int element) where T : struct
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (mipLevel < 0 || mipLevel >= base.mipmapCount)
		{
			throw new ArgumentException("The passed in miplevel " + mipLevel + " is invalid. The valid range is 0 through " + (base.mipmapCount - 1));
		}
		if (element < 0 || element >= depth)
		{
			throw new ArgumentException("The passed in element " + element + " is invalid. The valid range is 0 through " + (depth - 1));
		}
		ulong pixelDataOffset = GetPixelDataOffset(base.mipmapCount, element);
		ulong pixelDataOffset2 = GetPixelDataOffset(mipLevel, element);
		ulong pixelDataSize = GetPixelDataSize(mipLevel, element);
		int num = UnsafeUtility.SizeOf<T>();
		ulong num2 = pixelDataSize / (ulong)num;
		if (num2 > int.MaxValue)
		{
			throw CreateNativeArrayLengthOverflowException();
		}
		IntPtr intPtr = new IntPtr((long)GetImageData() + ((long)pixelDataOffset * (long)element + (long)pixelDataOffset2));
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
			throw new ArgumentException("Crunched Texture2DArray is not supported.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_depth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureFormat get_format_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IgnoreMipmapLimit_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIgnoreMipmapLimitAndReload_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_mipmapLimitGroup_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_activeMipmapLimit_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isReadable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_CreateImpl_Injected([Writable] Texture2DArray mono, int w, int h, int d, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, bool ignoreMipmapLimit, ref ManagedSpanWrapper mipmapLimitGroupName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color[] GetPixels_Injected(IntPtr _unity_self, int arrayElement, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int element, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int element, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, int arrayElement, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixels_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, int arrayElement, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixels32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, int arrayElement, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Full_Injected(IntPtr _unity_self, IntPtr src);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Slice_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int dstElement, int dstMip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Region_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstElement, int dstMip, int dstX, int dstY);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetImageData_Injected(IntPtr _unity_self);
}
