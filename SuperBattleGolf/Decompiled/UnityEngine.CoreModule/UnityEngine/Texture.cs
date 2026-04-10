using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Streaming/TextureStreamingManager.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/Texture.h")]
public class Texture : Object
{
	public static readonly int GenerateAllMips = -1;

	[Obsolete("masterTextureLimit has been deprecated. Use globalMipmapLimit instead (UnityUpgradable) -> globalMipmapLimit", false)]
	[NativeProperty("ActiveGlobalMipmapLimit")]
	public static extern int masterTextureLimit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("ActiveGlobalMipmapLimit")]
	[Obsolete("globalMipmapLimit is not supported. Use QualitySettings.globalTextureMipmapLimit or Mipmap Limit Groups instead.", false)]
	public static extern int globalMipmapLimit
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public int mipmapCount
	{
		[NativeName("GetMipmapCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_mipmapCount_Injected(intPtr);
		}
	}

	[NativeProperty("AnisoLimit")]
	public static extern AnisotropicFiltering anisotropicFiltering
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public virtual GraphicsFormat graphicsFormat => GraphicsFormatUtility.GetFormat(this);

	public virtual int width
	{
		get
		{
			return GetDataWidth();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public virtual int height
	{
		get
		{
			return GetDataHeight();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public virtual TextureDimension dimension
	{
		get
		{
			return GetDimension();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	internal bool isNativeTexture
	{
		[NativeName("IsNativeTexture")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isNativeTexture_Injected(intPtr);
		}
	}

	public virtual bool isReadable
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

	public TextureWrapMode wrapMode
	{
		[NativeName("GetWrapModeU")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wrapMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wrapMode_Injected(intPtr, value);
		}
	}

	public TextureWrapMode wrapModeU
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wrapModeU_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wrapModeU_Injected(intPtr, value);
		}
	}

	public TextureWrapMode wrapModeV
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wrapModeV_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wrapModeV_Injected(intPtr, value);
		}
	}

	public TextureWrapMode wrapModeW
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wrapModeW_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wrapModeW_Injected(intPtr, value);
		}
	}

	public FilterMode filterMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_filterMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_filterMode_Injected(intPtr, value);
		}
	}

	public int anisoLevel
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_anisoLevel_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_anisoLevel_Injected(intPtr, value);
		}
	}

	public float mipMapBias
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_mipMapBias_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_mipMapBias_Injected(intPtr, value);
		}
	}

	public Vector2 texelSize
	{
		[NativeName("GetTexelSize")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_texelSize_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public uint updateCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_updateCount_Injected(intPtr);
		}
	}

	internal ColorSpace activeTextureColorSpace
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "Unity.UIElements" })]
		get
		{
			return (Internal_GetActiveTextureColorSpace() == 0) ? ColorSpace.Linear : ColorSpace.Gamma;
		}
	}

	public bool isDataSRGB => Internal_GetStoredColorSpace() == TextureColorSpace.sRGB;

	public static extern ulong totalTextureMemory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetTotalTextureMemory")]
		get;
	}

	public static extern ulong desiredTextureMemory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetDesiredTextureMemory")]
		get;
	}

	public static extern ulong targetTextureMemory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetTargetTextureMemory")]
		get;
	}

	public static extern ulong currentTextureMemory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetCurrentTextureMemory")]
		get;
	}

	public static extern ulong nonStreamingTextureMemory
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetNonStreamingTextureMemory")]
		get;
	}

	public static extern ulong streamingMipmapUploadCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetStreamingMipmapUploadCount")]
		get;
	}

	public static extern ulong streamingRendererCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetStreamingRendererCount")]
		get;
	}

	public static extern ulong streamingTextureCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetStreamingTextureCount")]
		get;
	}

	public static extern ulong nonStreamingTextureCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetNonStreamingTextureCount")]
		get;
	}

	public static extern ulong streamingTexturePendingLoadCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetStreamingTexturePendingLoadCount")]
		get;
	}

	public static extern ulong streamingTextureLoadingCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("GetTextureStreamingManager().GetStreamingTextureLoadingCount")]
		get;
	}

	public static extern bool streamingTextureForceLoadAll
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "GetTextureStreamingManager().GetForceLoadAll")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "GetTextureStreamingManager().SetForceLoadAll")]
		set;
	}

	public static extern bool streamingTextureDiscardUnusedMips
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "GetTextureStreamingManager().GetDiscardUnusedMips")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "GetTextureStreamingManager().SetDiscardUnusedMips")]
		set;
	}

	public static extern bool allowThreadedTextureCreation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "Texture2DScripting::IsCreateTextureThreadedEnabled")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction(Name = "Texture2DScripting::EnableCreateTextureThreaded")]
		set;
	}

	public GraphicsTexture graphicsTexture
	{
		[FreeFunction(Name = "Texture2DScripting::GetCurrentGraphicsTexture", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = get_graphicsTexture_Injected(intPtr);
			return (intPtr2 == (IntPtr)0) ? null : GraphicsTexture.BindingsMarshaller.ConvertToManaged(intPtr2);
		}
	}

	protected Texture()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("SetGlobalAnisoLimits")]
	public static extern void SetGlobalAnisotropicFilteringLimits(int forcedMin, int globalMax);

	[ThreadSafe]
	private int GetDataWidth()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDataWidth_Injected(intPtr);
	}

	[ThreadSafe]
	private int GetDataHeight()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDataHeight_Injected(intPtr);
	}

	[ThreadSafe]
	private TextureDimension GetDimension()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDimension_Injected(intPtr);
	}

	public IntPtr GetNativeTexturePtr()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNativeTexturePtr_Injected(intPtr);
	}

	[Obsolete("Use GetNativeTexturePtr instead.", false)]
	public int GetNativeTextureID()
	{
		return (int)GetNativeTexturePtr();
	}

	public void IncrementUpdateCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IncrementUpdateCount_Injected(intPtr);
	}

	[NativeMethod("GetActiveTextureColorSpace")]
	private int Internal_GetActiveTextureColorSpace()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetActiveTextureColorSpace_Injected(intPtr);
	}

	[NativeMethod("GetStoredColorSpace")]
	private TextureColorSpace Internal_GetStoredColorSpace()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetStoredColorSpace_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetTextureStreamingManager().SetStreamingTextureMaterialDebugProperties")]
	public static extern void SetStreamingTextureMaterialDebugProperties();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetTextureStreamingManager().SetStreamingTextureMaterialDebugPropertiesWithSlot")]
	private static extern void SetStreamingTextureMaterialDebugPropertiesWithSlot(int materialTextureSlot);

	public static void SetStreamingTextureMaterialDebugProperties(int materialTextureSlot)
	{
		SetStreamingTextureMaterialDebugPropertiesWithSlot(materialTextureSlot);
	}

	internal ulong GetPixelDataSize(int mipLevel, int element = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixelDataSize_Injected(intPtr, mipLevel, element);
	}

	internal ulong GetPixelDataOffset(int mipLevel, int element = 0)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixelDataOffset_Injected(intPtr, mipLevel, element);
	}

	internal TextureColorSpace GetTextureColorSpace(bool linear)
	{
		return (!linear) ? TextureColorSpace.sRGB : TextureColorSpace.Linear;
	}

	internal TextureColorSpace GetTextureColorSpace(GraphicsFormat format)
	{
		return GetTextureColorSpace(!GraphicsFormatUtility.IsSRGBFormat(format));
	}

	internal bool ValidateFormat(RenderTextureFormat format)
	{
		if (SystemInfo.SupportsRenderTextureFormat(format))
		{
			return true;
		}
		Debug.LogError($"RenderTexture creation failed. '{format.ToString()}' is not supported on this platform. Use 'SystemInfo.SupportsRenderTextureFormat' C# API to check format support.", this);
		return false;
	}

	internal bool ValidateFormat(TextureFormat format)
	{
		if (SystemInfo.SupportsTextureFormat(format))
		{
			return true;
		}
		if (GraphicsFormatUtility.IsCompressedFormat(format) && GraphicsFormatUtility.CanDecompressFormat(GraphicsFormatUtility.GetGraphicsFormat(format, isSRGB: false)))
		{
			Debug.LogWarning($"'{format.ToString()}' is not supported on this platform. Decompressing texture. Use 'SystemInfo.SupportsTextureFormat' C# API to check format support.", this);
			return true;
		}
		Debug.LogError($"Texture creation failed. '{format.ToString()}' is not supported on this platform. Use 'SystemInfo.SupportsTextureFormat' C# API to check format support.", this);
		return false;
	}

	internal bool ValidateFormat(GraphicsFormat format, GraphicsFormatUsage usage)
	{
		if (SystemInfo.IsFormatSupported(format, usage))
		{
			return true;
		}
		Debug.LogError($"Texture creation failed. '{format.ToString()}' is not supported for {usage.ToString()} usage on this platform. Use 'SystemInfo.IsFormatSupported' C# API to check format support.", this);
		return false;
	}

	internal UnityException CreateNonReadableException(Texture t)
	{
		return new UnityException($"Texture '{t.name}' is not readable, the texture memory can not be accessed from scripts. You can make the texture readable in the Texture Import Settings.");
	}

	internal UnityException IgnoreMipmapLimitCannotBeToggledException(Texture t)
	{
		return new UnityException($"Failed to toggle ignoreMipmapLimit, Texture '{t.name}' is not readable. You can make the texture readable in the Texture Import Settings.");
	}

	internal UnityException CreateNativeArrayLengthOverflowException()
	{
		return new UnityException("Failed to create NativeArray, length exceeds the allowed maximum of Int32.MaxValue. Use a larger type as template argument to reduce the array length.");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_mipmapCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDataWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDataHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureDimension GetDimension_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isNativeTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isReadable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureWrapMode get_wrapMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wrapMode_Injected(IntPtr _unity_self, TextureWrapMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureWrapMode get_wrapModeU_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wrapModeU_Injected(IntPtr _unity_self, TextureWrapMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureWrapMode get_wrapModeV_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wrapModeV_Injected(IntPtr _unity_self, TextureWrapMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureWrapMode get_wrapModeW_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wrapModeW_Injected(IntPtr _unity_self, TextureWrapMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern FilterMode get_filterMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_filterMode_Injected(IntPtr _unity_self, FilterMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_anisoLevel_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_anisoLevel_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_mipMapBias_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mipMapBias_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_texelSize_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNativeTexturePtr_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint get_updateCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void IncrementUpdateCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Internal_GetActiveTextureColorSpace_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureColorSpace Internal_GetStoredColorSpace_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ulong GetPixelDataSize_Injected(IntPtr _unity_self, int mipLevel, int element);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ulong GetPixelDataOffset_Injected(IntPtr _unity_self, int mipLevel, int element);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_graphicsTexture_Injected(IntPtr _unity_self);
}
