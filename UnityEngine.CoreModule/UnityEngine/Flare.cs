using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Camera/Flare.h")]
public sealed class Flare : Object
{
	[Serializable]
	[UsedByNativeCode]
	internal struct FlareElement
	{
		[NativeName("m_ImageIndex")]
		public uint imageIndex;

		[NativeName("m_Position")]
		public float position;

		[NativeName("m_Size")]
		public float size;

		[NativeName("m_Color")]
		public Color color;

		[NativeName("m_UseLightColor")]
		public bool useLightColor;

		[NativeName("m_Rotate")]
		public bool rotate;

		[NativeName("m_Zoom")]
		public bool zoom;

		[NativeName("m_Fade")]
		public bool fade;
	}

	[Serializable]
	internal enum FlareLayout
	{
		LayoutLargeRestSmall,
		LayoutMixed,
		Layout1x1,
		Layout2x2,
		Layout3x3,
		Layout4x4
	}

	internal Texture texture
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture>(get_texture_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_texture_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	internal unsafe FlareElement[] elements
	{
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			FlareElement[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_elements_Injected(intPtr, out ret);
			}
			finally
			{
				FlareElement[] array = default(FlareElement[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<FlareElement> span = new Span<FlareElement>(value);
			fixed (FlareElement* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_elements_Injected(intPtr, ref value2);
			}
		}
	}

	internal bool useFog
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useFog_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useFog_Injected(intPtr, value);
		}
	}

	internal int textureLayout
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_textureLayout_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_textureLayout_Injected(intPtr, value);
		}
	}

	public Flare()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Create([Writable] Flare self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_texture_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_texture_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_elements_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_elements_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useFog_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useFog_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_textureLayout_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_textureLayout_Injected(IntPtr _unity_self, int value);
}
