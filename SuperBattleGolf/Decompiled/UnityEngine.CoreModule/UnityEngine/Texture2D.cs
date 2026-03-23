using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[ExcludeFromPreset]
[UsedByNativeCode]
[HelpURL("texture-type-default")]
[NativeHeader("Runtime/Graphics/Texture2D.h")]
[NativeHeader("Runtime/Graphics/GeneratedTextures.h")]
public sealed class Texture2D : Texture
{
	[Flags]
	public enum EXRFlags
	{
		None = 0,
		OutputAsFloat = 1,
		CompressZIP = 2,
		CompressRLE = 4,
		CompressPIZ = 8
	}

	internal const int streamingMipmapsPriorityMin = -128;

	internal const int streamingMipmapsPriorityMax = 127;

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

	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static Texture2D whiteTexture => Unmarshal.UnmarshalUnityObject<Texture2D>(get_whiteTexture_Injected());

	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static Texture2D blackTexture => Unmarshal.UnmarshalUnityObject<Texture2D>(get_blackTexture_Injected());

	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static Texture2D redTexture => Unmarshal.UnmarshalUnityObject<Texture2D>(get_redTexture_Injected());

	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static Texture2D grayTexture => Unmarshal.UnmarshalUnityObject<Texture2D>(get_grayTexture_Injected());

	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static Texture2D linearGrayTexture => Unmarshal.UnmarshalUnityObject<Texture2D>(get_linearGrayTexture_Injected());

	[StaticAccessor("builtintex", StaticAccessorType.DoubleColon)]
	public static Texture2D normalTexture => Unmarshal.UnmarshalUnityObject<Texture2D>(get_normalTexture_Injected());

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

	[NativeConditional("ENABLE_VIRTUALTEXTURING && UNITY_EDITOR")]
	[NativeName("VTOnly")]
	public bool vtOnly
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vtOnly_Injected(intPtr);
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

	public int minimumMipmapLevel
	{
		[FreeFunction(Name = "GetTextureStreamingManager().GetMinimumMipmapLevel", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_minimumMipmapLevel_Injected(intPtr);
		}
		[FreeFunction(Name = "GetTextureStreamingManager().SetMinimumMipmapLevel", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_minimumMipmapLevel_Injected(intPtr, value);
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

	public int calculatedMipmapLevel
	{
		[FreeFunction(Name = "GetTextureStreamingManager().GetCalculatedMipmapLevel", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_calculatedMipmapLevel_Injected(intPtr);
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

	public void Compress(bool highQuality)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Compress_Injected(intPtr, highQuality);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Texture2DScripting::CreateEmpty")]
	private static extern bool Internal_CreateEmptyImpl([Writable] Texture2D mono);

	[FreeFunction("Texture2DScripting::Create")]
	private unsafe static bool Internal_CreateImpl([Writable] Texture2D mono, int w, int h, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex, bool ignoreMipmapLimit, string mipmapLimitGroupName)
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
					return Internal_CreateImpl_Injected(mono, w, h, mipCount, format, colorSpace, flags, nativeTex, ignoreMipmapLimit, ref managedSpanWrapper);
				}
			}
			return Internal_CreateImpl_Injected(mono, w, h, mipCount, format, colorSpace, flags, nativeTex, ignoreMipmapLimit, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	private static void Internal_Create([Writable] Texture2D mono, int w, int h, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex, bool ignoreMipmapLimit, string mipmapLimitGroupName)
	{
		if (!Internal_CreateImpl(mono, w, h, mipCount, format, colorSpace, flags, nativeTex, ignoreMipmapLimit, mipmapLimitGroupName))
		{
			throw new UnityException("Failed to create texture because of invalid parameters.");
		}
	}

	[NativeName("Apply")]
	private void ApplyImpl(bool updateMipmaps, bool makeNoLongerReadable)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ApplyImpl_Injected(intPtr, updateMipmaps, makeNoLongerReadable);
	}

	[NativeName("Reinitialize")]
	private bool ReinitializeImpl(int width, int height)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ReinitializeImpl_Injected(intPtr, width, height);
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

	[NativeName("GetPixelBilinear")]
	private Color GetPixelBilinearImpl(int image, int mip, float u, float v)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPixelBilinearImpl_Injected(intPtr, image, mip, u, v, out var ret);
		return ret;
	}

	[FreeFunction(Name = "Texture2DScripting::ReinitializeWithFormat", HasExplicitThis = true)]
	private bool ReinitializeWithFormatImpl(int width, int height, GraphicsFormat format, bool hasMipMap)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ReinitializeWithFormatImpl_Injected(intPtr, width, height, format, hasMipMap);
	}

	[FreeFunction(Name = "Texture2DScripting::ReinitializeWithTextureFormat", HasExplicitThis = true)]
	private bool ReinitializeWithTextureFormatImpl(int width, int height, TextureFormat textureFormat, bool hasMipMap)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return ReinitializeWithTextureFormatImpl_Injected(intPtr, width, height, textureFormat, hasMipMap);
	}

	[FreeFunction(Name = "Texture2DScripting::ReadPixels", HasExplicitThis = true)]
	private void ReadPixelsImpl(Rect source, int destX, int destY, bool recalculateMipMaps)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadPixelsImpl_Injected(intPtr, ref source, destX, destY, recalculateMipMaps);
	}

	[FreeFunction(Name = "Texture2DScripting::SetPixels", HasExplicitThis = true, ThrowsException = true)]
	private unsafe void SetPixelsImpl(int x, int y, int w, int h, Color[] pixel, int miplevel, int frame)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Color> span = new Span<Color>(pixel);
		fixed (Color* begin = span)
		{
			ManagedSpanWrapper pixel2 = new ManagedSpanWrapper(begin, span.Length);
			SetPixelsImpl_Injected(intPtr, x, y, w, h, ref pixel2, miplevel, frame);
		}
	}

	[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
	private bool LoadRawTextureDataImpl(IntPtr data, ulong size)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return LoadRawTextureDataImpl_Injected(intPtr, data, size);
	}

	[FreeFunction(Name = "Texture2DScripting::LoadRawData", HasExplicitThis = true)]
	private unsafe bool LoadRawTextureDataImplArray(byte[] data)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<byte> span = new Span<byte>(data);
		bool result;
		fixed (byte* begin = span)
		{
			ManagedSpanWrapper data2 = new ManagedSpanWrapper(begin, span.Length);
			result = LoadRawTextureDataImplArray_Injected(intPtr, ref data2);
		}
		return result;
	}

	[FreeFunction(Name = "Texture2DScripting::SetPixelDataArray", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImplArray(Array data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImplArray_Injected(intPtr, data, mipLevel, elementSize, dataArraySize, sourceDataStartIndex);
	}

	[FreeFunction(Name = "Texture2DScripting::SetPixelData", HasExplicitThis = true, ThrowsException = true)]
	private bool SetPixelDataImpl(IntPtr data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return SetPixelDataImpl_Injected(intPtr, data, mipLevel, elementSize, dataArraySize, sourceDataStartIndex);
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

	private ulong GetImageDataSize()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetImageDataSize_Injected(intPtr);
	}

	[FreeFunction("Texture2DScripting::GenerateAtlas")]
	private unsafe static void GenerateAtlasImpl(Vector2[] sizes, int padding, int atlasSize, [Out] Rect[] rect)
	{
		//The blocks IL_0044 are reachable both inside and outside the pinned region starting at IL_0029. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper rect2 = default(BlittableArrayWrapper);
		try
		{
			Span<Vector2> span = new Span<Vector2>(sizes);
			fixed (Vector2* begin = span)
			{
				ManagedSpanWrapper sizes2 = new ManagedSpanWrapper(begin, span.Length);
				if (rect != null)
				{
					fixed (Rect[] array = rect)
					{
						if (array.Length != 0)
						{
							rect2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
						}
						GenerateAtlasImpl_Injected(ref sizes2, padding, atlasSize, out rect2);
						return;
					}
				}
				GenerateAtlasImpl_Injected(ref sizes2, padding, atlasSize, out rect2);
			}
		}
		finally
		{
			rect2.Unmarshal(ref array);
		}
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

	[FreeFunction(Name = "GetTextureStreamingManager().ClearMinimumMipmapLevel", HasExplicitThis = true)]
	public void ClearMinimumMipmapLevel()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearMinimumMipmapLevel_Injected(intPtr);
	}

	[FreeFunction("Texture2DScripting::UpdateExternalTexture", HasExplicitThis = true)]
	public void UpdateExternalTexture(IntPtr nativeTex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpdateExternalTexture_Injected(intPtr, nativeTex);
	}

	[FreeFunction("Texture2DScripting::SetAllPixels32", HasExplicitThis = true, ThrowsException = true)]
	private unsafe void SetAllPixels32(Color32[] colors, int miplevel)
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
			SetAllPixels32_Injected(intPtr, ref colors2, miplevel);
		}
	}

	[FreeFunction("Texture2DScripting::SetBlockOfPixels32", HasExplicitThis = true, ThrowsException = true)]
	private unsafe void SetBlockOfPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, int miplevel)
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
			SetBlockOfPixels32_Injected(intPtr, x, y, blockWidth, blockHeight, ref colors2, miplevel);
		}
	}

	[FreeFunction("Texture2DScripting::GetRawTextureData", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public byte[] GetRawTextureData()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetRawTextureData_Injected(intPtr);
	}

	[FreeFunction("Texture2DScripting::GetPixels", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels_Injected(intPtr, x, y, blockWidth, blockHeight, miplevel);
	}

	[ExcludeFromDocs]
	public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
	{
		return GetPixels(x, y, blockWidth, blockHeight, 0);
	}

	[FreeFunction("Texture2DScripting::GetPixels32", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color32[] GetPixels32([DefaultValue("0")] int miplevel)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels32_Injected(intPtr, miplevel);
	}

	[ExcludeFromDocs]
	public Color32[] GetPixels32()
	{
		return GetPixels32(0);
	}

	[FreeFunction("Texture2DScripting::PackTextures", HasExplicitThis = true)]
	[return: Unmarshalled]
	public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize, bool makeNoLongerReadable)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return PackTextures_Injected(intPtr, textures, padding, maximumAtlasSize, makeNoLongerReadable);
	}

	public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
	{
		return PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable: false);
	}

	public Rect[] PackTextures(Texture2D[] textures, int padding)
	{
		return PackTextures(textures, padding, 2048);
	}

	[FreeFunction(Name = "Texture2DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Full(Texture src)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Full_Injected(intPtr, MarshalledUnityObject.Marshal(src));
	}

	[FreeFunction(Name = "Texture2DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Slice(Texture src, int srcElement, int srcMip, int dstMip)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Slice_Injected(intPtr, MarshalledUnityObject.Marshal(src), srcElement, srcMip, dstMip);
	}

	[FreeFunction(Name = "Texture2DScripting::CopyPixels", HasExplicitThis = true, ThrowsException = true)]
	private void CopyPixels_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstMip, int dstX, int dstY)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyPixels_Region_Injected(intPtr, MarshalledUnityObject.Marshal(src), srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstMip, dstX, dstY);
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

	internal Texture2D(int width, int height, GraphicsFormat format, TextureCreationFlags flags, int mipCount, IntPtr nativeTex, MipmapLimitDescriptor mipmapLimitDescriptor)
	{
		bool flag = mipmapLimitDescriptor.useMipmapLimit;
		string groupName = mipmapLimitDescriptor.groupName;
		if ((flags & TextureCreationFlags.IgnoreMipmapLimit) != TextureCreationFlags.None)
		{
			flag = false;
		}
		if (ValidateFormat(format, width, height))
		{
			Internal_Create(this, width, height, mipCount, format, GetTextureColorSpace(format), flags, nativeTex, !flag, groupName);
		}
	}

	[ExcludeFromDocs]
	public Texture2D(int width, int height, DefaultFormat format, TextureCreationFlags flags)
		: this(width, height, SystemInfo.GetGraphicsFormat(format), flags)
	{
	}

	[ExcludeFromDocs]
	public Texture2D(int width, int height, DefaultFormat format, int mipCount, TextureCreationFlags flags)
		: this(width, height, SystemInfo.GetGraphicsFormat(format), flags, mipCount, IntPtr.Zero, default(MipmapLimitDescriptor))
	{
	}

	[ExcludeFromDocs]
	[Obsolete("Please provide mipmap limit information using a MipmapLimitDescriptor argument", false)]
	public Texture2D(int width, int height, DefaultFormat format, int mipCount, string mipmapLimitGroupName, TextureCreationFlags flags)
		: this(width, height, SystemInfo.GetGraphicsFormat(format), flags, mipCount, IntPtr.Zero, new MipmapLimitDescriptor(useMipmapLimit: true, mipmapLimitGroupName))
	{
	}

	[ExcludeFromDocs]
	public Texture2D(int width, int height, DefaultFormat format, int mipCount, TextureCreationFlags flags, MipmapLimitDescriptor mipmapLimitDescriptor)
		: this(width, height, SystemInfo.GetGraphicsFormat(format), flags, mipCount, IntPtr.Zero, mipmapLimitDescriptor)
	{
	}

	[ExcludeFromDocs]
	public Texture2D(int width, int height, GraphicsFormat format, TextureCreationFlags flags)
		: this(width, height, format, flags, Texture.GenerateAllMips, IntPtr.Zero, default(MipmapLimitDescriptor))
	{
	}

	[ExcludeFromDocs]
	public Texture2D(int width, int height, GraphicsFormat format, int mipCount, TextureCreationFlags flags)
		: this(width, height, format, flags, mipCount, IntPtr.Zero, default(MipmapLimitDescriptor))
	{
	}

	[ExcludeFromDocs]
	[Obsolete("Please provide mipmap limit information using a MipmapLimitDescriptor argument", false)]
	public Texture2D(int width, int height, GraphicsFormat format, int mipCount, string mipmapLimitGroupName, TextureCreationFlags flags)
		: this(width, height, format, flags, mipCount, IntPtr.Zero, new MipmapLimitDescriptor(useMipmapLimit: true, mipmapLimitGroupName))
	{
	}

	[ExcludeFromDocs]
	public Texture2D(int width, int height, GraphicsFormat format, int mipCount, TextureCreationFlags flags, MipmapLimitDescriptor mipmapLimitDescriptor)
		: this(width, height, format, flags, mipCount, IntPtr.Zero, mipmapLimitDescriptor)
	{
	}

	internal Texture2D(int width, int height, TextureFormat textureFormat, int mipCount, bool linear, IntPtr nativeTex, bool createUninitialized, MipmapLimitDescriptor mipmapLimitDescriptor)
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
			Internal_Create(this, width, height, mipCount, graphicsFormat, GetTextureColorSpace(linear), textureCreationFlags, nativeTex, !mipmapLimitDescriptor.useMipmapLimit, mipmapLimitDescriptor.groupName);
		}
	}

	public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear)
		: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized: false, default(MipmapLimitDescriptor))
	{
	}

	public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
		: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized, default(MipmapLimitDescriptor))
	{
	}

	public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized, MipmapLimitDescriptor mipmapLimitDescriptor)
		: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized, mipmapLimitDescriptor)
	{
	}

	[Obsolete("Please provide mipmap limit information using a MipmapLimitDescriptor argument", false)]
	public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("-1")] int mipCount, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized, [DefaultValue("true")] bool ignoreMipmapLimit, [DefaultValue("null")] string mipmapLimitGroupName)
		: this(width, height, textureFormat, mipCount, linear, IntPtr.Zero, createUninitialized, new MipmapLimitDescriptor(!ignoreMipmapLimit, mipmapLimitGroupName))
	{
	}

	public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("true")] bool mipChain, [DefaultValue("false")] bool linear)
		: this(width, height, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear, IntPtr.Zero, createUninitialized: false, default(MipmapLimitDescriptor))
	{
	}

	public Texture2D(int width, int height, [DefaultValue("TextureFormat.RGBA32")] TextureFormat textureFormat, [DefaultValue("true")] bool mipChain, [DefaultValue("false")] bool linear, [DefaultValue("false")] bool createUninitialized)
		: this(width, height, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear, IntPtr.Zero, createUninitialized, default(MipmapLimitDescriptor))
	{
	}

	public Texture2D(int width, int height, TextureFormat textureFormat, bool mipChain)
		: this(width, height, textureFormat, (!mipChain) ? 1 : Texture.GenerateAllMips, linear: false, IntPtr.Zero, createUninitialized: false, default(MipmapLimitDescriptor))
	{
	}

	public Texture2D(int width, int height)
	{
		TextureFormat textureFormat = TextureFormat.RGBA32;
		if (width == 0 && height == 0)
		{
			Internal_CreateEmptyImpl(this);
		}
		else if (ValidateFormat(textureFormat, width, height))
		{
			Internal_Create(this, width, height, Texture.GenerateAllMips, GraphicsFormatUtility.GetGraphicsFormat(textureFormat, isSRGB: true), GetTextureColorSpace(linear: false), TextureCreationFlags.MipChain, IntPtr.Zero, ignoreMipmapLimit: true, null);
		}
	}

	public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipChain, bool linear, IntPtr nativeTex)
	{
		if (nativeTex == IntPtr.Zero)
		{
			throw new ArgumentException("nativeTex can not be null");
		}
		return new Texture2D(width, height, format, (!mipChain) ? 1 : (-1), linear, nativeTex, createUninitialized: false, default(MipmapLimitDescriptor));
	}

	[ExcludeFromDocs]
	public void SetPixel(int x, int y, Color color)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		SetPixelImpl(0, 0, x, y, color);
	}

	public void SetPixel(int x, int y, Color color, [DefaultValue("0")] int mipLevel)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		SetPixelImpl(0, mipLevel, x, y, color);
	}

	public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		SetPixelsImpl(x, y, blockWidth, blockHeight, colors, miplevel, 0);
	}

	[ExcludeFromDocs]
	public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
	{
		SetPixels(x, y, blockWidth, blockHeight, colors, 0);
	}

	public void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel)
	{
		int num = width >> miplevel;
		if (num < 1)
		{
			num = 1;
		}
		int num2 = height >> miplevel;
		if (num2 < 1)
		{
			num2 = 1;
		}
		SetPixels(0, 0, num, num2, colors, miplevel);
	}

	[ExcludeFromDocs]
	public void SetPixels(Color[] colors)
	{
		SetPixels(0, 0, width, height, colors, 0);
	}

	[ExcludeFromDocs]
	public Color GetPixel(int x, int y)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelImpl(0, 0, x, y);
	}

	public Color GetPixel(int x, int y, [DefaultValue("0")] int mipLevel)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelImpl(0, mipLevel, x, y);
	}

	[ExcludeFromDocs]
	public Color GetPixelBilinear(float u, float v)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelBilinearImpl(0, 0, u, v);
	}

	public Color GetPixelBilinear(float u, float v, [DefaultValue("0")] int mipLevel)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return GetPixelBilinearImpl(0, mipLevel, u, v);
	}

	public void LoadRawTextureData(IntPtr data, int size)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (data == IntPtr.Zero || size == 0)
		{
			Debug.LogError("No texture data provided to LoadRawTextureData", this);
		}
		else if (!LoadRawTextureDataImpl(data, (ulong)size))
		{
			throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
		}
	}

	public void LoadRawTextureData(byte[] data)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (data == null || data.Length == 0)
		{
			Debug.LogError("No texture data provided to LoadRawTextureData", this);
		}
		else if (!LoadRawTextureDataImplArray(data))
		{
			throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
		}
	}

	public unsafe void LoadRawTextureData<T>(NativeArray<T> data) where T : struct
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!data.IsCreated || data.Length == 0)
		{
			throw new UnityException("No texture data provided to LoadRawTextureData");
		}
		if (!LoadRawTextureDataImpl((IntPtr)data.GetUnsafeReadOnlyPtr(), (ulong)data.Length * (ulong)UnsafeUtility.SizeOf<T>()))
		{
			throw new UnityException("LoadRawTextureData: not enough data provided (will result in overread).");
		}
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
			throw new ArgumentException("The passed in miplevel " + mipLevel + " is invalid. It needs to be in the range 0 and " + (base.mipmapCount - 1));
		}
		if (GetWritableImageData(0).ToInt64() == 0)
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
		IntPtr intPtr = new IntPtr((long)GetWritableImageData(0) + (long)pixelDataOffset);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)intPtr, (int)num2, Allocator.None);
	}

	public unsafe NativeArray<T> GetRawTextureData<T>() where T : struct
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		int num = UnsafeUtility.SizeOf<T>();
		ulong num2 = GetImageDataSize() / (ulong)num;
		if (num2 > int.MaxValue)
		{
			throw CreateNativeArrayLengthOverflowException();
		}
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)GetWritableImageData(0), (int)num2, Allocator.None);
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

	public bool Reinitialize(int width, int height)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return ReinitializeImpl(width, height);
	}

	public bool Reinitialize(int width, int height, TextureFormat format, bool hasMipMap)
	{
		return ReinitializeWithTextureFormatImpl(width, height, format, hasMipMap);
	}

	public bool Reinitialize(int width, int height, GraphicsFormat format, bool hasMipMap)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		return ReinitializeWithFormatImpl(width, height, format, hasMipMap);
	}

	[Obsolete("Texture2D.Resize(int, int) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32)", false)]
	public bool Resize(int width, int height)
	{
		return Reinitialize(width, height);
	}

	[Obsolete("Texture2D.Resize(int, int, TextureFormat, bool) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int, TextureFormat, bool) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32, UnityEngine.TextureFormat, [*] System.Boolean)", false)]
	public bool Resize(int width, int height, TextureFormat format, bool hasMipMap)
	{
		return Reinitialize(width, height, format, hasMipMap);
	}

	[Obsolete("Texture2D.Resize(int, int, GraphicsFormat, bool) has been deprecated because it actually reinitializes the texture. Use Texture2D.Reinitialize(int, int, GraphicsFormat, bool) instead (UnityUpgradable) -> Reinitialize([*] System.Int32, [*] System.Int32, UnityEngine.Experimental.Rendering.GraphicsFormat, [*] System.Boolean)", false)]
	public bool Resize(int width, int height, GraphicsFormat format, bool hasMipMap)
	{
		return Reinitialize(width, height, format, hasMipMap);
	}

	public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		ReadPixelsImpl(source, destX, destY, recalculateMipMaps);
	}

	[ExcludeFromDocs]
	public void ReadPixels(Rect source, int destX, int destY)
	{
		ReadPixels(source, destX, destY, recalculateMipMaps: true);
	}

	public static bool GenerateAtlas(Vector2[] sizes, int padding, int atlasSize, List<Rect> results)
	{
		if (sizes == null)
		{
			throw new ArgumentException("sizes array can not be null");
		}
		if (results == null)
		{
			throw new ArgumentException("results list cannot be null");
		}
		if (padding < 0)
		{
			throw new ArgumentException("padding can not be negative");
		}
		if (atlasSize <= 0)
		{
			throw new ArgumentException("atlas size must be positive");
		}
		results.Clear();
		if (sizes.Length == 0)
		{
			return true;
		}
		NoAllocHelpers.EnsureListElemCount(results, sizes.Length);
		GenerateAtlasImpl(sizes, padding, atlasSize, NoAllocHelpers.ExtractArrayFromList(results));
		return results.Count != 0;
	}

	public void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel)
	{
		SetAllPixels32(colors, miplevel);
	}

	[ExcludeFromDocs]
	public void SetPixels32(Color32[] colors)
	{
		SetPixels32(colors, 0);
	}

	public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors, [DefaultValue("0")] int miplevel)
	{
		SetBlockOfPixels32(x, y, blockWidth, blockHeight, colors, miplevel);
	}

	[ExcludeFromDocs]
	public void SetPixels32(int x, int y, int blockWidth, int blockHeight, Color32[] colors)
	{
		SetPixels32(x, y, blockWidth, blockHeight, colors, 0);
	}

	public Color[] GetPixels([DefaultValue("0")] int miplevel)
	{
		int num = width >> miplevel;
		if (num < 1)
		{
			num = 1;
		}
		int num2 = height >> miplevel;
		if (num2 < 1)
		{
			num2 = 1;
		}
		return GetPixels(0, 0, num, num2, miplevel);
	}

	[ExcludeFromDocs]
	public Color[] GetPixels()
	{
		return GetPixels(0);
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

	public void CopyPixels(Texture src, int srcElement, int srcMip, int dstMip)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!src.isReadable)
		{
			throw CreateNonReadableException(src);
		}
		CopyPixels_Slice(src, srcElement, srcMip, dstMip);
	}

	public void CopyPixels(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstMip, int dstX, int dstY)
	{
		if (!isReadable)
		{
			throw CreateNonReadableException(this);
		}
		if (!src.isReadable)
		{
			throw CreateNonReadableException(src);
		}
		CopyPixels_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dstMip, dstX, dstY);
	}

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
	private static extern IntPtr get_whiteTexture_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_blackTexture_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_redTexture_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_grayTexture_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_linearGrayTexture_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_normalTexture_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Compress_Injected(IntPtr _unity_self, bool highQuality);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_CreateImpl_Injected([Writable] Texture2D mono, int w, int h, int mipCount, GraphicsFormat format, TextureColorSpace colorSpace, TextureCreationFlags flags, IntPtr nativeTex, bool ignoreMipmapLimit, ref ManagedSpanWrapper mipmapLimitGroupName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isReadable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_vtOnly_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ApplyImpl_Injected(IntPtr _unity_self, bool updateMipmaps, bool makeNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ReinitializeImpl_Injected(IntPtr _unity_self, int width, int height);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixelImpl_Injected(IntPtr _unity_self, int image, int mip, int x, int y, [In] ref Color color);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPixelImpl_Injected(IntPtr _unity_self, int image, int mip, int x, int y, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPixelBilinearImpl_Injected(IntPtr _unity_self, int image, int mip, float u, float v, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ReinitializeWithFormatImpl_Injected(IntPtr _unity_self, int width, int height, GraphicsFormat format, bool hasMipMap);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ReinitializeWithTextureFormatImpl_Injected(IntPtr _unity_self, int width, int height, TextureFormat textureFormat, bool hasMipMap);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ReadPixelsImpl_Injected(IntPtr _unity_self, [In] ref Rect source, int destX, int destY, bool recalculateMipMaps);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPixelsImpl_Injected(IntPtr _unity_self, int x, int y, int w, int h, ref ManagedSpanWrapper pixel, int miplevel, int frame);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool LoadRawTextureDataImpl_Injected(IntPtr _unity_self, IntPtr data, ulong size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool LoadRawTextureDataImplArray_Injected(IntPtr _unity_self, ref ManagedSpanWrapper data);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImplArray_Injected(IntPtr _unity_self, Array data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPixelDataImpl_Injected(IntPtr _unity_self, IntPtr data, int mipLevel, int elementSize, int dataArraySize, int sourceDataStartIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetWritableImageData_Injected(IntPtr _unity_self, int frame);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ulong GetImageDataSize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateAtlasImpl_Injected(ref ManagedSpanWrapper sizes, int padding, int atlasSize, out BlittableArrayWrapper rect);

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
	private static extern int get_minimumMipmapLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_minimumMipmapLevel_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_loadAllMips_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_loadAllMips_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_calculatedMipmapLevel_Injected(IntPtr _unity_self);

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

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearMinimumMipmapLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateExternalTexture_Injected(IntPtr _unity_self, IntPtr nativeTex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAllPixels32_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBlockOfPixels32_Injected(IntPtr _unity_self, int x, int y, int blockWidth, int blockHeight, ref ManagedSpanWrapper colors, int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern byte[] GetRawTextureData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color[] GetPixels_Injected(IntPtr _unity_self, int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, [DefaultValue("0")] int miplevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Rect[] PackTextures_Injected(IntPtr _unity_self, Texture2D[] textures, int padding, int maximumAtlasSize, bool makeNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Full_Injected(IntPtr _unity_self, IntPtr src);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Slice_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int dstMip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyPixels_Region_Injected(IntPtr _unity_self, IntPtr src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, int dstMip, int dstX, int dstY);
}
