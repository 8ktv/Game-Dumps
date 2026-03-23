using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/CustomRenderTexture.h")]
[UsedByNativeCode]
public sealed class CustomRenderTexture : RenderTexture
{
	public Material material
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(get_material_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_material_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Material initializationMaterial
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(get_initializationMaterial_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_initializationMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Texture initializationTexture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(get_initializationTexture_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_initializationTexture_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public CustomRenderTextureInitializationSource initializationSource
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_initializationSource_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_initializationSource_Injected(intPtr, value);
		}
	}

	public Color initializationColor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_initializationColor_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_initializationColor_Injected(intPtr, ref value);
		}
	}

	public CustomRenderTextureUpdateMode updateMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_updateMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_updateMode_Injected(intPtr, value);
		}
	}

	public CustomRenderTextureUpdateMode initializationMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_initializationMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_initializationMode_Injected(intPtr, value);
		}
	}

	public CustomRenderTextureUpdateZoneSpace updateZoneSpace
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_updateZoneSpace_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_updateZoneSpace_Injected(intPtr, value);
		}
	}

	public int shaderPass
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shaderPass_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shaderPass_Injected(intPtr, value);
		}
	}

	public uint cubemapFaceMask
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cubemapFaceMask_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cubemapFaceMask_Injected(intPtr, value);
		}
	}

	public bool doubleBuffered
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_doubleBuffered_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_doubleBuffered_Injected(intPtr, value);
		}
	}

	public bool wrapUpdateZones
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wrapUpdateZones_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wrapUpdateZones_Injected(intPtr, value);
		}
	}

	public float updatePeriod
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_updatePeriod_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_updatePeriod_Injected(intPtr, value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "CustomRenderTextureScripting::Create")]
	private static extern void Internal_CreateCustomRenderTexture([Writable] CustomRenderTexture rt);

	[NativeName("TriggerUpdate")]
	private void TriggerUpdate(int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TriggerUpdate_Injected(intPtr, count);
	}

	public void Update(int count)
	{
		CustomRenderTextureManager.InvokeTriggerUpdate(this, count);
		TriggerUpdate(count);
	}

	public void Update()
	{
		Update(1);
	}

	[NativeName("TriggerInitialization")]
	private void TriggerInitialization()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TriggerInitialization_Injected(intPtr);
	}

	public void Initialize()
	{
		TriggerInitialization();
		CustomRenderTextureManager.InvokeTriggerInitialize(this);
	}

	public void ClearUpdateZones()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearUpdateZones_Injected(intPtr);
	}

	[FreeFunction(Name = "CustomRenderTextureScripting::GetUpdateZonesInternal", HasExplicitThis = true)]
	internal void GetUpdateZonesInternal([NotNull] object updateZones)
	{
		if (updateZones == null)
		{
			ThrowHelper.ThrowArgumentNullException(updateZones, "updateZones");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetUpdateZonesInternal_Injected(intPtr, updateZones);
	}

	public void GetUpdateZones(List<CustomRenderTextureUpdateZone> updateZones)
	{
		GetUpdateZonesInternal(updateZones);
	}

	[FreeFunction(Name = "CustomRenderTextureScripting::SetUpdateZonesInternal", HasExplicitThis = true)]
	private unsafe void SetUpdateZonesInternal(CustomRenderTextureUpdateZone[] updateZones)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<CustomRenderTextureUpdateZone> span = new Span<CustomRenderTextureUpdateZone>(updateZones);
		fixed (CustomRenderTextureUpdateZone* begin = span)
		{
			ManagedSpanWrapper updateZones2 = new ManagedSpanWrapper(begin, span.Length);
			SetUpdateZonesInternal_Injected(intPtr, ref updateZones2);
		}
	}

	[FreeFunction(Name = "CustomRenderTextureScripting::GetDoubleBufferRenderTexture", HasExplicitThis = true)]
	public RenderTexture GetDoubleBufferRenderTexture()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<RenderTexture>(GetDoubleBufferRenderTexture_Injected(intPtr));
	}

	public void EnsureDoubleBufferConsistency()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EnsureDoubleBufferConsistency_Injected(intPtr);
	}

	public void SetUpdateZones(CustomRenderTextureUpdateZone[] updateZones)
	{
		if (updateZones == null)
		{
			throw new ArgumentNullException("updateZones");
		}
		SetUpdateZonesInternal(updateZones);
	}

	public CustomRenderTexture(int width, int height, RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
		: this(width, height, RenderTexture.GetCompatibleFormat(format, readWrite))
	{
		if (this != null)
		{
			SetShadowSamplingMode(RenderTexture.GetShadowSamplingModeForFormat(format));
		}
	}

	[ExcludeFromDocs]
	public CustomRenderTexture(int width, int height, RenderTextureFormat format)
		: this(width, height, format, RenderTextureReadWrite.Default)
	{
	}

	[ExcludeFromDocs]
	public CustomRenderTexture(int width, int height)
		: this(width, height, SystemInfo.GetGraphicsFormat(DefaultFormat.LDR))
	{
	}

	[ExcludeFromDocs]
	public CustomRenderTexture(int width, int height, [DefaultValue("DefaultFormat.LDR")] DefaultFormat defaultFormat)
		: this(width, height, RenderTexture.GetDefaultColorFormat(defaultFormat))
	{
		if (defaultFormat == DefaultFormat.DepthStencil || defaultFormat == DefaultFormat.Shadow)
		{
			base.depthStencilFormat = SystemInfo.GetGraphicsFormat(defaultFormat);
			SetShadowSamplingMode(RenderTexture.GetShadowSamplingModeForFormat(defaultFormat));
		}
	}

	[ExcludeFromDocs]
	public CustomRenderTexture(int width, int height, GraphicsFormat format)
	{
		if (format == GraphicsFormat.None || ValidateFormat(format, GraphicsFormatUsage.Render))
		{
			Internal_CreateCustomRenderTexture(this);
			this.width = width;
			this.height = height;
			base.graphicsFormat = format;
			SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(format));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TriggerUpdate_Injected(IntPtr _unity_self, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TriggerInitialization_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearUpdateZones_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_material_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_material_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_initializationMaterial_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_initializationMaterial_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_initializationTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_initializationTexture_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetUpdateZonesInternal_Injected(IntPtr _unity_self, object updateZones);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetUpdateZonesInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper updateZones);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetDoubleBufferRenderTexture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnsureDoubleBufferConsistency_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CustomRenderTextureInitializationSource get_initializationSource_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_initializationSource_Injected(IntPtr _unity_self, CustomRenderTextureInitializationSource value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_initializationColor_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_initializationColor_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CustomRenderTextureUpdateMode get_updateMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_updateMode_Injected(IntPtr _unity_self, CustomRenderTextureUpdateMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CustomRenderTextureUpdateMode get_initializationMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_initializationMode_Injected(IntPtr _unity_self, CustomRenderTextureUpdateMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CustomRenderTextureUpdateZoneSpace get_updateZoneSpace_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_updateZoneSpace_Injected(IntPtr _unity_self, CustomRenderTextureUpdateZoneSpace value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_shaderPass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shaderPass_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint get_cubemapFaceMask_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cubemapFaceMask_Injected(IntPtr _unity_self, uint value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_doubleBuffered_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_doubleBuffered_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_wrapUpdateZones_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wrapUpdateZones_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_updatePeriod_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_updatePeriod_Injected(IntPtr _unity_self, float value);
}
