using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/IMGUI/GUIStyle.bindings.h")]
public sealed class GUIStyleState
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(GUIStyleState guiStyleState)
		{
			return guiStyleState.m_Ptr;
		}
	}

	[NonSerialized]
	internal IntPtr m_Ptr;

	private readonly GUIStyle m_SourceStyle;

	[NativeProperty("Background", false, TargetType.Function)]
	public Texture2D background
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(get_background_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_background_Injected(intPtr, Object.MarshalledUnityObject.Marshal(value));
		}
	}

	[NativeProperty("textColor", false, TargetType.Field)]
	public Color textColor
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_textColor_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_textColor_Injected(intPtr, ref value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GUIStyleState_Bindings::Init", IsThreadSafe = true)]
	private static extern IntPtr Init();

	[FreeFunction(Name = "GUIStyleState_Bindings::Cleanup", IsThreadSafe = true, HasExplicitThis = true)]
	private void Cleanup()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Cleanup_Injected(intPtr);
	}

	public GUIStyleState()
	{
		m_Ptr = Init();
	}

	private GUIStyleState(GUIStyle sourceStyle, IntPtr source)
	{
		m_SourceStyle = sourceStyle;
		m_Ptr = source;
	}

	internal static GUIStyleState ProduceGUIStyleStateFromDeserialization(GUIStyle sourceStyle, IntPtr source)
	{
		return new GUIStyleState(sourceStyle, source);
	}

	internal static GUIStyleState GetGUIStyleState(GUIStyle sourceStyle, IntPtr source)
	{
		return new GUIStyleState(sourceStyle, source);
	}

	~GUIStyleState()
	{
		if (m_SourceStyle == null)
		{
			Cleanup();
			m_Ptr = IntPtr.Zero;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_background_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_background_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_textColor_Injected(IntPtr _unity_self, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_textColor_Injected(IntPtr _unity_self, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Cleanup_Injected(IntPtr _unity_self);
}
