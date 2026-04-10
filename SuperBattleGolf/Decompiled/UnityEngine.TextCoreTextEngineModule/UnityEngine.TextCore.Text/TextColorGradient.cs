using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[Serializable]
[NativeHeader("Modules/TextCoreTextEngine/TextColorGradient.h")]
[ExcludeFromObjectFactory]
[ExcludeFromPreset]
public class TextColorGradient : ScriptableObject
{
	public ColorGradientMode colorMode = ColorGradientMode.FourCornersGradient;

	public Color topLeft;

	public Color topRight;

	public Color bottomLeft;

	public Color bottomRight;

	private const ColorGradientMode k_DefaultColorMode = ColorGradientMode.FourCornersGradient;

	private static readonly Color k_DefaultColor = Color.white;

	private IntPtr m_NativeInstance = IntPtr.Zero;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal IntPtr nativeInstance
	{
		get
		{
			if (m_NativeInstance == IntPtr.Zero)
			{
				m_NativeInstance = CreateNative(topLeft, topRight, bottomLeft, bottomRight, MarshalledUnityObject.MarshalNotNull(this));
			}
			return m_NativeInstance;
		}
	}

	private void OnValidate()
	{
		MarkNativeDirty();
	}

	private void OnDisable()
	{
		if (m_NativeInstance != IntPtr.Zero)
		{
			DestroyNative(m_NativeInstance, MarshalledUnityObject.MarshalNotNull(this));
			m_NativeInstance = IntPtr.Zero;
		}
	}

	public TextColorGradient()
	{
		colorMode = ColorGradientMode.FourCornersGradient;
		topLeft = k_DefaultColor;
		topRight = k_DefaultColor;
		bottomLeft = k_DefaultColor;
		bottomRight = k_DefaultColor;
	}

	public TextColorGradient(Color color)
	{
		colorMode = ColorGradientMode.FourCornersGradient;
		topLeft = color;
		topRight = color;
		bottomLeft = color;
		bottomRight = color;
	}

	public TextColorGradient(Color color0, Color color1, Color color2, Color color3)
	{
		colorMode = ColorGradientMode.FourCornersGradient;
		topLeft = color0;
		topRight = color1;
		bottomLeft = color2;
		bottomRight = color3;
	}

	internal void MarkNativeDirty()
	{
		if (m_NativeInstance != IntPtr.Zero)
		{
			UpdateNative(m_NativeInstance, topLeft, topRight, bottomLeft, bottomRight);
		}
	}

	private static IntPtr CreateNative(Color32 tl, Color32 tr, Color32 bl, Color32 br, IntPtr managedObject)
	{
		return CreateNative_Injected(ref tl, ref tr, ref bl, ref br, managedObject);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DestroyNative(IntPtr nativeInstance, IntPtr managedObject);

	private static void UpdateNative(IntPtr instance, Color32 tl, Color32 tr, Color32 bl, Color32 br)
	{
		UpdateNative_Injected(instance, ref tl, ref tr, ref bl, ref br);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateNative_Injected([In] ref Color32 tl, [In] ref Color32 tr, [In] ref Color32 bl, [In] ref Color32 br, IntPtr managedObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateNative_Injected(IntPtr instance, [In] ref Color32 tl, [In] ref Color32 tr, [In] ref Color32 bl, [In] ref Color32 br);
}
