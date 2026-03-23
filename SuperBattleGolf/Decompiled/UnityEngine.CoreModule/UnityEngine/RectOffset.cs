using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/IMGUI/GUIStyle.h")]
[UsedByNativeCode]
public class RectOffset : IFormattable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(RectOffset rectOffset)
		{
			return rectOffset.m_Ptr;
		}
	}

	[NonSerialized]
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	internal IntPtr m_Ptr;

	private readonly object m_SourceStyle;

	[NativeProperty("left", false, TargetType.Field)]
	public int left
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_left_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_left_Injected(intPtr, value);
		}
	}

	[NativeProperty("right", false, TargetType.Field)]
	public int right
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_right_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_right_Injected(intPtr, value);
		}
	}

	[NativeProperty("top", false, TargetType.Field)]
	public int top
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_top_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_top_Injected(intPtr, value);
		}
	}

	[NativeProperty("bottom", false, TargetType.Field)]
	public int bottom
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bottom_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bottom_Injected(intPtr, value);
		}
	}

	public int horizontal
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_horizontal_Injected(intPtr);
		}
	}

	public int vertical
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vertical_Injected(intPtr);
		}
	}

	public RectOffset()
	{
		m_Ptr = InternalCreate();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	internal RectOffset(object sourceStyle, IntPtr source)
	{
		m_SourceStyle = sourceStyle;
		m_Ptr = source;
	}

	~RectOffset()
	{
		if (m_SourceStyle == null)
		{
			Destroy();
		}
	}

	public RectOffset(int left, int right, int top, int bottom)
	{
		m_Ptr = InternalCreate();
		this.left = left;
		this.right = right;
		this.top = top;
		this.bottom = bottom;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		return ToString(null, null);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string format)
	{
		return ToString(format, null);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string format, IFormatProvider formatProvider)
	{
		if (formatProvider == null)
		{
			formatProvider = CultureInfo.InvariantCulture.NumberFormat;
		}
		return $"RectOffset (l:{left.ToString(format, formatProvider)} r:{right.ToString(format, formatProvider)} t:{top.ToString(format, formatProvider)} b:{bottom.ToString(format, formatProvider)})";
	}

	private void Destroy()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			InternalDestroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	private static extern IntPtr InternalCreate();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadAndSerializationSafe]
	private static extern void InternalDestroy(IntPtr ptr);

	public Rect Add(Rect rect)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Add_Injected(intPtr, ref rect, out var ret);
		return ret;
	}

	public Rect Remove(Rect rect)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Remove_Injected(intPtr, ref rect, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_left_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_left_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_right_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_right_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_top_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_top_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_bottom_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bottom_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_horizontal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_vertical_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Add_Injected(IntPtr _unity_self, [In] ref Rect rect, out Rect ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Remove_Injected(IntPtr _unity_self, [In] ref Rect rect, out Rect ret);
}
