using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Android;

[StaticAccessor("AndroidInsets", StaticAccessorType.DoubleColon)]
[RequiredByNativeCode]
[NativeHeader("Modules/AndroidJNI/Public/AndroidInsets.bindings.h")]
internal class AndroidInsets
{
	[Flags]
	internal enum AndroidInsetsType
	{
		StatusBars = 1,
		NavigationBars = 2,
		CaptionBar = 4,
		IME = 8,
		SystemGestures = 0x10,
		MandatorySystemGestures = 0x20,
		TappableElement = 0x40,
		DisplayCutout = 0x80
	}

	private IntPtr m_NativeHandle;

	internal AndroidInsets()
	{
	}

	[RequiredByNativeCode(GenerateProxy = true)]
	private void SetNativeHandle(IntPtr ptr)
	{
		m_NativeHandle = ptr;
	}

	private static Rect InternalGetAndroidInsets(IntPtr handle, AndroidInsetsType type)
	{
		InternalGetAndroidInsets_Injected(handle, type, out var ret);
		return ret;
	}

	internal Rect GetInsets(AndroidInsetsType type)
	{
		if (m_NativeHandle == IntPtr.Zero)
		{
			throw new Exception("You can only query insets from within AndroidApplication.$onInsetsChanged");
		}
		return InternalGetAndroidInsets(m_NativeHandle, type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetAndroidInsets_Injected(IntPtr handle, AndroidInsetsType type, out Rect ret);
}
