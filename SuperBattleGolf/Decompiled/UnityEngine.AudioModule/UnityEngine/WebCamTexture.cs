using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("AudioScriptingClasses.h")]
[NativeHeader("Runtime/Video/ScriptBindings/WebCamTexture.bindings.h")]
[NativeHeader("Runtime/Video/BaseWebCamTexture.h")]
public sealed class WebCamTexture : Texture
{
	public static extern WebCamDevice[] devices
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[StaticAccessor("WebCamTextureBindings", StaticAccessorType.DoubleColon)]
		[NativeName("Internal_GetDevices")]
		get;
	}

	public bool isPlaying
	{
		[NativeName("IsPlaying")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isPlaying_Injected(intPtr);
		}
	}

	[NativeName("Device")]
	public unsafe string deviceName
	{
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
				get_deviceName_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
		set
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
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(value);
					fixed (char* begin = readOnlySpan)
					{
						managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
						set_deviceName_Injected(intPtr, ref managedSpanWrapper);
						return;
					}
				}
				set_deviceName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
			}
		}
	}

	public float requestedFPS
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_requestedFPS_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_requestedFPS_Injected(intPtr, value);
		}
	}

	public int requestedWidth
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_requestedWidth_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_requestedWidth_Injected(intPtr, value);
		}
	}

	public int requestedHeight
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_requestedHeight_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_requestedHeight_Injected(intPtr, value);
		}
	}

	public int videoRotationAngle
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_videoRotationAngle_Injected(intPtr);
		}
	}

	public bool videoVerticallyMirrored
	{
		[NativeName("IsVideoVerticallyMirrored")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_videoVerticallyMirrored_Injected(intPtr);
		}
	}

	public bool didUpdateThisFrame
	{
		[NativeName("DidUpdateThisFrame")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_didUpdateThisFrame_Injected(intPtr);
		}
	}

	public Vector2? autoFocusPoint
	{
		get
		{
			return (internalAutoFocusPoint.x < 0f) ? ((Vector2?)null) : new Vector2?(internalAutoFocusPoint);
		}
		set
		{
			internalAutoFocusPoint = ((!value.HasValue) ? new Vector2(-1f, -1f) : value.Value);
		}
	}

	internal Vector2 internalAutoFocusPoint
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_internalAutoFocusPoint_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_internalAutoFocusPoint_Injected(intPtr, ref value);
		}
	}

	public bool isDepth
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isDepth_Injected(intPtr);
		}
	}

	public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS)
	{
		Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, requestedFPS);
	}

	public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight)
	{
		Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, 0);
	}

	public WebCamTexture(string deviceName)
	{
		Internal_CreateWebCamTexture(this, deviceName, 0, 0, 0);
	}

	public WebCamTexture(int requestedWidth, int requestedHeight, int requestedFPS)
	{
		Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, requestedFPS);
	}

	public WebCamTexture(int requestedWidth, int requestedHeight)
	{
		Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, 0);
	}

	public WebCamTexture()
	{
		Internal_CreateWebCamTexture(this, "", 0, 0, 0);
	}

	public void Play()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Play_Injected(intPtr);
	}

	public void Pause()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Pause_Injected(intPtr);
	}

	public void Stop()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Stop_Injected(intPtr);
	}

	[FreeFunction("WebCamTextureBindings::Internal_GetPixel", HasExplicitThis = true)]
	public Color GetPixel(int x, int y)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPixel_Injected(intPtr, x, y, out var ret);
		return ret;
	}

	public Color[] GetPixels()
	{
		return GetPixels(0, 0, width, height);
	}

	[FreeFunction("WebCamTextureBindings::Internal_GetPixels", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels_Injected(intPtr, x, y, blockWidth, blockHeight);
	}

	[ExcludeFromDocs]
	public Color32[] GetPixels32()
	{
		return GetPixels32(null);
	}

	[FreeFunction("WebCamTextureBindings::Internal_GetPixels32", HasExplicitThis = true, ThrowsException = true)]
	[return: Unmarshalled]
	public Color32[] GetPixels32([DefaultValue("null")][Unmarshalled] Color32[] colors)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPixels32_Injected(intPtr, colors);
	}

	[StaticAccessor("WebCamTextureBindings", StaticAccessorType.DoubleColon)]
	private unsafe static void Internal_CreateWebCamTexture([Writable] WebCamTexture self, string scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(scriptingDevice, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(scriptingDevice);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Internal_CreateWebCamTexture_Injected(self, ref managedSpanWrapper, requestedWidth, requestedHeight, maxFramerate);
					return;
				}
			}
			Internal_CreateWebCamTexture_Injected(self, ref managedSpanWrapper, requestedWidth, requestedHeight, maxFramerate);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Play_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Pause_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Stop_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_deviceName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_deviceName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_requestedFPS_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_requestedFPS_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_requestedWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_requestedWidth_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_requestedHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_requestedHeight_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_videoRotationAngle_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_videoVerticallyMirrored_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_didUpdateThisFrame_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPixel_Injected(IntPtr _unity_self, int x, int y, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color[] GetPixels_Injected(IntPtr _unity_self, int x, int y, int blockWidth, int blockHeight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, [DefaultValue("null")] Color32[] colors);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_internalAutoFocusPoint_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_internalAutoFocusPoint_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isDepth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CreateWebCamTexture_Injected([Writable] WebCamTexture self, ref ManagedSpanWrapper scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate);
}
