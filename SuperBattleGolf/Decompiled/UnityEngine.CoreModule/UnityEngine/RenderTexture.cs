using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Camera/Camera.h")]
[NativeHeader("Runtime/Graphics/RenderBufferManager.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
[NativeHeader("Runtime/Graphics/RenderTexture.h")]
public class RenderTexture : Texture
{
	public override int width
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_width_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_width_Injected(intPtr, value);
		}
	}

	public override int height
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_height_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_height_Injected(intPtr, value);
		}
	}

	public override TextureDimension dimension
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_dimension_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_dimension_Injected(intPtr, value);
		}
	}

	public new GraphicsFormat graphicsFormat
	{
		get
		{
			return GetColorFormat(suppressWarnings: true);
		}
		set
		{
			SetColorFormat(value);
		}
	}

	[NativeProperty("MipMap")]
	public bool useMipMap
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useMipMap_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useMipMap_Injected(intPtr, value);
		}
	}

	[NativeProperty("SRGBReadWrite")]
	public bool sRGB
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sRGB_Injected(intPtr);
		}
	}

	[NativeProperty("VRUsage")]
	public VRTextureUsage vrUsage
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vrUsage_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_vrUsage_Injected(intPtr, value);
		}
	}

	[NativeProperty("Memoryless")]
	public RenderTextureMemoryless memorylessMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_memorylessMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_memorylessMode_Injected(intPtr, value);
		}
	}

	public RenderTextureFormat format
	{
		get
		{
			if (graphicsFormat != GraphicsFormat.None)
			{
				return GraphicsFormatUtility.GetRenderTextureFormat(graphicsFormat);
			}
			return (GetDescriptor().shadowSamplingMode == ShadowSamplingMode.None) ? RenderTextureFormat.Depth : RenderTextureFormat.Shadowmap;
		}
		set
		{
			if (value == RenderTextureFormat.Depth || value == RenderTextureFormat.Shadowmap)
			{
				if (depthStencilFormat == GraphicsFormat.None)
				{
					WarnAboutFallbackTo16BitsDepth(value);
					depthStencilFormat = GraphicsFormat.D16_UNorm;
				}
				if (value == RenderTextureFormat.Shadowmap)
				{
					SetShadowSamplingMode(ShadowSamplingMode.CompareDepths);
				}
			}
			graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(value, sRGB);
		}
	}

	public GraphicsFormat stencilFormat
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stencilFormat_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_stencilFormat_Injected(intPtr, value);
		}
	}

	public GraphicsFormat depthStencilFormat
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_depthStencilFormat_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_depthStencilFormat_Injected(intPtr, value);
		}
	}

	public bool autoGenerateMips
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_autoGenerateMips_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_autoGenerateMips_Injected(intPtr, value);
		}
	}

	public int volumeDepth
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_volumeDepth_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_volumeDepth_Injected(intPtr, value);
		}
	}

	public int antiAliasing
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_antiAliasing_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_antiAliasing_Injected(intPtr, value);
		}
	}

	public bool bindTextureMS
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bindTextureMS_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bindTextureMS_Injected(intPtr, value);
		}
	}

	public bool enableRandomWrite
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enableRandomWrite_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enableRandomWrite_Injected(intPtr, value);
		}
	}

	public bool useDynamicScale
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useDynamicScale_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useDynamicScale_Injected(intPtr, value);
		}
	}

	public bool useDynamicScaleExplicit
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useDynamicScaleExplicit_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useDynamicScaleExplicit_Injected(intPtr, value);
		}
	}

	public bool enableShadingRate
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enableShadingRate_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enableShadingRate_Injected(intPtr, value);
		}
	}

	public bool isPowerOfTwo
	{
		get
		{
			return GetIsPowerOfTwo();
		}
		set
		{
		}
	}

	public static RenderTexture active
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

	public RenderBuffer colorBuffer => GetColorBuffer();

	public RenderBuffer depthBuffer => GetDepthBuffer();

	public int depth
	{
		[FreeFunction("RenderTextureScripting::GetDepth", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_depth_Injected(intPtr);
		}
		[FreeFunction("RenderTextureScripting::SetDepth", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_depth_Injected(intPtr, value);
		}
	}

	public RenderTextureDescriptor descriptor
	{
		get
		{
			return GetDescriptor();
		}
		set
		{
			ValidateRenderTextureDesc(ref value);
			SetRenderTextureDescriptor(value);
		}
	}

	[Obsolete("Use RenderTexture.dimension instead.", false)]
	public bool isCubemap
	{
		get
		{
			return dimension == TextureDimension.Cube;
		}
		set
		{
			dimension = (value ? TextureDimension.Cube : TextureDimension.Tex2D);
		}
	}

	[Obsolete("Use RenderTexture.dimension instead.", false)]
	public bool isVolume
	{
		get
		{
			return dimension == TextureDimension.Tex3D;
		}
		set
		{
			dimension = (value ? TextureDimension.Tex3D : TextureDimension.Tex2D);
		}
	}

	[Obsolete("RenderTexture.enabled is always now, no need to use it.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool enabled
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	[NativeName("GetColorFormat")]
	private GraphicsFormat GetColorFormat(bool suppressWarnings)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetColorFormat_Injected(intPtr, suppressWarnings);
	}

	[NativeName("SetColorFormat")]
	private void SetColorFormat(GraphicsFormat format)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetColorFormat_Injected(intPtr, format);
	}

	public void ApplyDynamicScale()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ApplyDynamicScale_Injected(intPtr);
	}

	private bool GetIsPowerOfTwo()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetIsPowerOfTwo_Injected(intPtr);
	}

	[FreeFunction("RenderTexture::GetActiveAsRenderTexture")]
	private static RenderTexture GetActive()
	{
		return Unmarshal.UnmarshalUnityObject<RenderTexture>(GetActive_Injected());
	}

	[FreeFunction("RenderTextureScripting::SetActive")]
	private static void SetActive(RenderTexture rt)
	{
		SetActive_Injected(MarshalledUnityObject.Marshal(rt));
	}

	[FreeFunction(Name = "RenderTextureScripting::GetColorBuffer", HasExplicitThis = true)]
	private RenderBuffer GetColorBuffer()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetColorBuffer_Injected(intPtr, out var ret);
		return ret;
	}

	[FreeFunction(Name = "RenderTextureScripting::GetDepthBuffer", HasExplicitThis = true)]
	private RenderBuffer GetDepthBuffer()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetDepthBuffer_Injected(intPtr, out var ret);
		return ret;
	}

	private void SetMipMapCount(int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMipMapCount_Injected(intPtr, count);
	}

	internal void SetShadowSamplingMode(ShadowSamplingMode samplingMode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetShadowSamplingMode_Injected(intPtr, samplingMode);
	}

	public IntPtr GetNativeDepthBufferPtr()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNativeDepthBufferPtr_Injected(intPtr);
	}

	public void DiscardContents(bool discardColor, bool discardDepth)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		DiscardContents_Injected(intPtr, discardColor, discardDepth);
	}

	[Obsolete("This function has no effect.", false)]
	public void MarkRestoreExpected()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MarkRestoreExpected_Injected(intPtr);
	}

	public void DiscardContents()
	{
		DiscardContents(discardColor: true, discardDepth: true);
	}

	[NativeName("ResolveAntiAliasedSurface")]
	private void ResolveAA()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResolveAA_Injected(intPtr);
	}

	[NativeName("ResolveAntiAliasedSurface")]
	private void ResolveAATo(RenderTexture rt)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResolveAATo_Injected(intPtr, MarshalledUnityObject.Marshal(rt));
	}

	public void ResolveAntiAliasedSurface()
	{
		ResolveAA();
	}

	public void ResolveAntiAliasedSurface(RenderTexture target)
	{
		ResolveAATo(target);
	}

	[FreeFunction(Name = "RenderTextureScripting::SetGlobalShaderProperty", HasExplicitThis = true)]
	public unsafe void SetGlobalShaderProperty(string propertyName)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(propertyName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetGlobalShaderProperty_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			SetGlobalShaderProperty_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public bool Create()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Create_Injected(intPtr);
	}

	public void Release()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Release_Injected(intPtr);
	}

	public bool IsCreated()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsCreated_Injected(intPtr);
	}

	public void GenerateMips()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GenerateMips_Injected(intPtr);
	}

	[NativeThrows]
	public void ConvertToEquirect(RenderTexture equirect, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ConvertToEquirect_Injected(intPtr, MarshalledUnityObject.Marshal(equirect), eye);
	}

	internal void SetSRGBReadWrite(bool srgb)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetSRGBReadWrite_Injected(intPtr, srgb);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("RenderTextureScripting::Create")]
	private static extern void Internal_Create([Writable] RenderTexture rt);

	[FreeFunction("RenderTextureSupportsStencil")]
	public static bool SupportsStencil(RenderTexture rt)
	{
		return SupportsStencil_Injected(MarshalledUnityObject.Marshal(rt));
	}

	[NativeName("SetRenderTextureDescFromScript")]
	private void SetRenderTextureDescriptor(RenderTextureDescriptor desc)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetRenderTextureDescriptor_Injected(intPtr, ref desc);
	}

	[NativeName("GetRenderTextureDesc")]
	private RenderTextureDescriptor GetDescriptor()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetDescriptor_Injected(intPtr, out var ret);
		return ret;
	}

	[FreeFunction("GetRenderBufferManager().GetTextures().GetTempBuffer")]
	private static RenderTexture GetTemporary_Internal(RenderTextureDescriptor desc)
	{
		return Unmarshal.UnmarshalUnityObject<RenderTexture>(GetTemporary_Internal_Injected(ref desc));
	}

	[FreeFunction("GetRenderBufferManager().GetTextures().ReleaseTempBuffer")]
	public static void ReleaseTemporary(RenderTexture temp)
	{
		ReleaseTemporary_Injected(MarshalledUnityObject.Marshal(temp));
	}

	[RequiredByNativeCode]
	protected internal RenderTexture()
	{
	}

	public RenderTexture(RenderTextureDescriptor desc)
	{
		ValidateRenderTextureDesc(ref desc);
		Internal_Create(this);
		SetRenderTextureDescriptor(desc);
	}

	public RenderTexture(RenderTexture textureToCopy)
	{
		if (textureToCopy == null)
		{
			throw new ArgumentNullException("textureToCopy");
		}
		RenderTextureDescriptor desc = textureToCopy.descriptor;
		ValidateRenderTextureDesc(ref desc);
		Internal_Create(this);
		SetRenderTextureDescriptor(desc);
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, DefaultFormat format)
		: this(width, height, GetDefaultColorFormat(format), GetDefaultDepthStencilFormat(format, depth), Texture.GenerateAllMips)
	{
		if (this != null)
		{
			SetShadowSamplingMode(GetShadowSamplingModeForFormat(format));
		}
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, GraphicsFormat format)
		: this(width, height, depth, format, Texture.GenerateAllMips)
	{
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, GraphicsFormat format, int mipCount)
	{
		if (format == GraphicsFormat.None || ValidateFormat(format, GraphicsFormatUsage.Render))
		{
			Internal_Create(this);
			depthStencilFormat = GetDepthStencilFormatLegacy(depth, format);
			this.width = width;
			this.height = height;
			graphicsFormat = format;
			SetMipMapCount(mipCount);
			SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(format));
		}
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int mipCount)
	{
		if (colorFormat == GraphicsFormat.None || ValidateFormat(colorFormat, GraphicsFormatUsage.Render))
		{
			Internal_Create(this);
			this.width = width;
			this.height = height;
			this.depthStencilFormat = depthStencilFormat;
			graphicsFormat = colorFormat;
			SetMipMapCount(mipCount);
			SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(colorFormat));
		}
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat)
		: this(width, height, colorFormat, depthStencilFormat, Texture.GenerateAllMips)
	{
	}

	public RenderTexture(int width, int height, int depth, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
	{
		Initialize(width, height, depth, format, readWrite, Texture.GenerateAllMips);
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
		: this(width, height, depth, format, Texture.GenerateAllMips)
	{
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth)
		: this(width, height, depth, RenderTextureFormat.Default)
	{
	}

	[ExcludeFromDocs]
	public RenderTexture(int width, int height, int depth, RenderTextureFormat format, int mipCount)
	{
		Initialize(width, height, depth, format, RenderTextureReadWrite.Default, mipCount);
	}

	private void Initialize(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite, int mipCount)
	{
		GraphicsFormat compatibleFormat = GetCompatibleFormat(format, readWrite);
		GraphicsFormat depthStencilFormatLegacy = GetDepthStencilFormatLegacy(depth, format);
		if (compatibleFormat == GraphicsFormat.None || ValidateFormat(compatibleFormat, GraphicsFormatUsage.Render))
		{
			Internal_Create(this);
			this.width = width;
			this.height = height;
			depthStencilFormat = depthStencilFormatLegacy;
			graphicsFormat = compatibleFormat;
			SetMipMapCount(mipCount);
			SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(compatibleFormat));
			SetShadowSamplingMode(GetShadowSamplingModeForFormat(format));
		}
	}

	internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, GraphicsFormat colorFormat)
	{
		return GetDepthStencilFormatLegacy(depthBits, requestedShadowMap: false);
	}

	internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, RenderTextureFormat format, bool disableFallback = false)
	{
		if (!disableFallback && (format == RenderTextureFormat.Depth || format == RenderTextureFormat.Shadowmap) && depthBits < 16)
		{
			WarnAboutFallbackTo16BitsDepth(format);
			depthBits = 16;
		}
		return GetDepthStencilFormatLegacy(depthBits, format == RenderTextureFormat.Shadowmap);
	}

	internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, DefaultFormat format)
	{
		return GetDepthStencilFormatLegacy(depthBits, format == DefaultFormat.Shadow);
	}

	internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, ShadowSamplingMode shadowSamplingMode)
	{
		return GetDepthStencilFormatLegacy(depthBits, shadowSamplingMode != ShadowSamplingMode.None);
	}

	internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, bool requestedShadowMap)
	{
		GraphicsFormat graphicsFormat = (requestedShadowMap ? GraphicsFormatUtility.GetDepthStencilFormat(depthBits, 0) : GraphicsFormatUtility.GetDepthStencilFormat(depthBits));
		if (depthBits > 16 && graphicsFormat == GraphicsFormat.None && requestedShadowMap)
		{
			Debug.LogWarning($"No compatible shadow map depth format with {depthBits} or more depth bits has been found. Changing to a 16 bit depth buffer.");
			return GraphicsFormat.D16_UNorm;
		}
		return graphicsFormat;
	}

	private static void ValidateRenderTextureDesc(ref RenderTextureDescriptor desc)
	{
		if (desc.graphicsFormat == GraphicsFormat.None && desc.depthStencilFormat == GraphicsFormat.None)
		{
			WarnAboutFallbackTo16BitsDepth(desc.colorFormat);
			desc.depthStencilFormat = GraphicsFormat.D16_UNorm;
		}
		if (desc.graphicsFormat != GraphicsFormat.None && !SystemInfo.IsFormatSupported(desc.graphicsFormat, GraphicsFormatUsage.Render))
		{
			throw new ArgumentException("RenderTextureDesc graphicsFormat must be a supported GraphicsFormat. " + desc.graphicsFormat.ToString() + " is not supported on this platform.", "desc.graphicsFormat");
		}
		if (desc.depthStencilFormat != GraphicsFormat.None && !GraphicsFormatUtility.IsDepthStencilFormat(desc.depthStencilFormat))
		{
			throw new ArgumentException("RenderTextureDesc depthStencilFormat must be a supported depth/stencil GraphicsFormat. " + desc.depthStencilFormat.ToString() + " is not supported on this platform.", "desc.depthStencilFormat");
		}
		if (desc.width <= 0)
		{
			throw new ArgumentException("RenderTextureDesc width must be greater than zero.", "desc.width");
		}
		if (desc.height <= 0)
		{
			throw new ArgumentException("RenderTextureDesc height must be greater than zero.", "desc.height");
		}
		if (desc.volumeDepth <= 0)
		{
			throw new ArgumentException("RenderTextureDesc volumeDepth must be greater than zero.", "desc.volumeDepth");
		}
		if (desc.msaaSamples != 1 && desc.msaaSamples != 2 && desc.msaaSamples != 4 && desc.msaaSamples != 8)
		{
			throw new ArgumentException("RenderTextureDesc msaaSamples must be 1, 2, 4, or 8.", "desc.msaaSamples");
		}
		if (desc.dimension == TextureDimension.CubeArray && desc.volumeDepth % 6 != 0)
		{
			throw new ArgumentException("RenderTextureDesc volumeDepth must be a multiple of 6 when dimension is CubeArray", "desc.volumeDepth");
		}
		if (GraphicsFormatUtility.IsDepthStencilFormat(desc.graphicsFormat))
		{
			throw new ArgumentException("RenderTextureDesc graphicsFormat must not be a depth/stencil format. " + desc.graphicsFormat.ToString() + " is not supported.", "desc.graphicsFormat");
		}
	}

	internal static GraphicsFormat GetDefaultColorFormat(DefaultFormat format)
	{
		if ((uint)(format - 2) <= 1u)
		{
			return GraphicsFormat.None;
		}
		return SystemInfo.GetGraphicsFormat(format);
	}

	internal static GraphicsFormat GetDefaultDepthStencilFormat(DefaultFormat format, int depth)
	{
		if ((uint)(format - 2) <= 1u)
		{
			return SystemInfo.GetGraphicsFormat(format);
		}
		return GetDepthStencilFormatLegacy(depth, format);
	}

	internal static ShadowSamplingMode GetShadowSamplingModeForFormat(RenderTextureFormat format)
	{
		return (format != RenderTextureFormat.Shadowmap) ? ShadowSamplingMode.None : ShadowSamplingMode.CompareDepths;
	}

	internal static ShadowSamplingMode GetShadowSamplingModeForFormat(DefaultFormat format)
	{
		return (format != DefaultFormat.Shadow) ? ShadowSamplingMode.None : ShadowSamplingMode.CompareDepths;
	}

	internal static void WarnAboutFallbackTo16BitsDepth(RenderTextureFormat format)
	{
		Debug.LogWarning($"{format} RenderTexture requested without a depth buffer. Changing to a 16 bit depth buffer. To resolve this warning, please specify the desired number of depth bits when creating the render texture.");
	}

	internal static GraphicsFormat GetCompatibleFormat(RenderTextureFormat renderTextureFormat, RenderTextureReadWrite readWrite)
	{
		GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(renderTextureFormat, readWrite);
		GraphicsFormat compatibleFormat = SystemInfo.GetCompatibleFormat(graphicsFormat, GraphicsFormatUsage.Render);
		if (graphicsFormat == compatibleFormat)
		{
			return graphicsFormat;
		}
		Debug.LogWarning($"'{graphicsFormat.ToString()}' is not supported. RenderTexture::GetTemporary fallbacks to {compatibleFormat.ToString()} format on this platform. Use 'SystemInfo.IsFormatSupported' C# API to check format support.");
		return compatibleFormat;
	}

	public static RenderTexture GetTemporary(RenderTextureDescriptor desc)
	{
		ValidateRenderTextureDesc(ref desc);
		desc.createdFromScript = true;
		return GetTemporary_Internal(desc);
	}

	private static RenderTexture GetTemporaryImpl(int width, int height, GraphicsFormat depthStencilFormat, GraphicsFormat colorFormat, int antiAliasing = 1, RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, bool useDynamicScale = false, ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None)
	{
		RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height, colorFormat, depthStencilFormat);
		desc.msaaSamples = antiAliasing;
		desc.memoryless = memorylessMode;
		desc.vrUsage = vrUsage;
		desc.useDynamicScale = useDynamicScale;
		desc.shadowSamplingMode = shadowSamplingMode;
		return GetTemporary(desc);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, [UnityEngine.Internal.DefaultValue("1")] int antiAliasing, [UnityEngine.Internal.DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [UnityEngine.Internal.DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [UnityEngine.Internal.DefaultValue("false")] bool useDynamicScale)
	{
		ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None;
		return GetTemporaryImpl(width, height, GetDepthStencilFormatLegacy(depthBuffer, shadowSamplingMode), format, antiAliasing, memorylessMode, vrUsage, useDynamicScale);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
	{
		return GetTemporary(width, height, depthBuffer, format, antiAliasing, memorylessMode, vrUsage, useDynamicScale: false);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing, RenderTextureMemoryless memorylessMode)
	{
		return GetTemporary(width, height, depthBuffer, format, antiAliasing, memorylessMode, VRTextureUsage.None);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing)
	{
		return GetTemporary(width, height, depthBuffer, format, antiAliasing, RenderTextureMemoryless.None);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format)
	{
		return GetTemporary(width, height, depthBuffer, format, 1);
	}

	public static RenderTexture GetTemporary(int width, int height, [UnityEngine.Internal.DefaultValue("0")] int depthBuffer, [UnityEngine.Internal.DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [UnityEngine.Internal.DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [UnityEngine.Internal.DefaultValue("1")] int antiAliasing, [UnityEngine.Internal.DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [UnityEngine.Internal.DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [UnityEngine.Internal.DefaultValue("false")] bool useDynamicScale)
	{
		GraphicsFormat compatibleFormat = GetCompatibleFormat(format, readWrite);
		GraphicsFormat depthStencilFormatLegacy = GetDepthStencilFormatLegacy(depthBuffer, format);
		ShadowSamplingMode shadowSamplingModeForFormat = GetShadowSamplingModeForFormat(format);
		return GetTemporaryImpl(width, height, depthStencilFormatLegacy, compatibleFormat, antiAliasing, memorylessMode, vrUsage, useDynamicScale, shadowSamplingModeForFormat);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
	{
		return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, useDynamicScale: false);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode)
	{
		return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, VRTextureUsage.None);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
	{
		return GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, RenderTextureMemoryless.None);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
	{
		return GetTemporary(width, height, depthBuffer, format, readWrite, 1);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
	{
		return GetTemporary(width, height, depthBuffer, format, RenderTextureReadWrite.Default);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
	{
		return GetTemporary(width, height, depthBuffer, RenderTextureFormat.Default);
	}

	[ExcludeFromDocs]
	public static RenderTexture GetTemporary(int width, int height)
	{
		return GetTemporary(width, height, 0);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("GetTexelOffset always returns zero now, no point in using it.", false)]
	public Vector2 GetTexelOffset()
	{
		return Vector2.zero;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_width_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_width_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_height_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_height_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureDimension get_dimension_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_dimension_Injected(IntPtr _unity_self, TextureDimension value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GraphicsFormat GetColorFormat_Injected(IntPtr _unity_self, bool suppressWarnings);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetColorFormat_Injected(IntPtr _unity_self, GraphicsFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useMipMap_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useMipMap_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_sRGB_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern VRTextureUsage get_vrUsage_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_vrUsage_Injected(IntPtr _unity_self, VRTextureUsage value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RenderTextureMemoryless get_memorylessMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_memorylessMode_Injected(IntPtr _unity_self, RenderTextureMemoryless value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GraphicsFormat get_stencilFormat_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_stencilFormat_Injected(IntPtr _unity_self, GraphicsFormat value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GraphicsFormat get_depthStencilFormat_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_depthStencilFormat_Injected(IntPtr _unity_self, GraphicsFormat value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_autoGenerateMips_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_autoGenerateMips_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_volumeDepth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_volumeDepth_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_antiAliasing_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_antiAliasing_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_bindTextureMS_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bindTextureMS_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enableRandomWrite_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enableRandomWrite_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useDynamicScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useDynamicScale_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useDynamicScaleExplicit_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useDynamicScaleExplicit_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enableShadingRate_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enableShadingRate_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ApplyDynamicScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIsPowerOfTwo_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetActive_Injected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetActive_Injected(IntPtr rt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetColorBuffer_Injected(IntPtr _unity_self, out RenderBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDepthBuffer_Injected(IntPtr _unity_self, out RenderBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMipMapCount_Injected(IntPtr _unity_self, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetShadowSamplingMode_Injected(IntPtr _unity_self, ShadowSamplingMode samplingMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNativeDepthBufferPtr_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DiscardContents_Injected(IntPtr _unity_self, bool discardColor, bool discardDepth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MarkRestoreExpected_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResolveAA_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResolveAATo_Injected(IntPtr _unity_self, IntPtr rt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalShaderProperty_Injected(IntPtr _unity_self, ref ManagedSpanWrapper propertyName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Create_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Release_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsCreated_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateMips_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ConvertToEquirect_Injected(IntPtr _unity_self, IntPtr equirect, Camera.MonoOrStereoscopicEye eye);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSRGBReadWrite_Injected(IntPtr _unity_self, bool srgb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SupportsStencil_Injected(IntPtr rt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRenderTextureDescriptor_Injected(IntPtr _unity_self, [In] ref RenderTextureDescriptor desc);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDescriptor_Injected(IntPtr _unity_self, out RenderTextureDescriptor ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetTemporary_Internal_Injected([In] ref RenderTextureDescriptor desc);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ReleaseTemporary_Injected(IntPtr temp);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_depth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_depth_Injected(IntPtr _unity_self, int value);
}
