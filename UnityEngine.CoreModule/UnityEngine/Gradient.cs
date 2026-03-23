using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Runtime/Export/Math/Gradient.bindings.h")]
[RequiredByNativeCode]
public class Gradient : IEquatable<Gradient>
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(Gradient graident)
		{
			return graident.m_Ptr;
		}

		public static Gradient ConvertToManaged(IntPtr ptr)
		{
			return new Gradient(ptr);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
	internal IntPtr m_Ptr;

	private bool m_RequiresNativeCleanup;

	public unsafe GradientColorKey[] colorKeys
	{
		[FreeFunction("Gradient_Bindings::GetColorKeysArray", IsThreadSafe = true, HasExplicitThis = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			GradientColorKey[] result;
			try
			{
				IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_colorKeys_Injected(intPtr, out ret);
			}
			finally
			{
				GradientColorKey[] array = default(GradientColorKey[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction("Gradient_Bindings::SetColorKeysWithSpan", IsThreadSafe = true, HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<GradientColorKey> span = new Span<GradientColorKey>(value);
			fixed (GradientColorKey* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_colorKeys_Injected(intPtr, ref value2);
			}
		}
	}

	public unsafe GradientAlphaKey[] alphaKeys
	{
		[FreeFunction("Gradient_Bindings::GetAlphaKeysArray", IsThreadSafe = true, HasExplicitThis = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			GradientAlphaKey[] result;
			try
			{
				IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_alphaKeys_Injected(intPtr, out ret);
			}
			finally
			{
				GradientAlphaKey[] array = default(GradientAlphaKey[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction("Gradient_Bindings::SetAlphaKeysWithSpan", IsThreadSafe = true, HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<GradientAlphaKey> span = new Span<GradientAlphaKey>(value);
			fixed (GradientAlphaKey* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_alphaKeys_Injected(intPtr, ref value2);
			}
		}
	}

	public int colorKeyCount
	{
		[FreeFunction("Gradient_Bindings::GetColorKeyCount", IsThreadSafe = true, HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_colorKeyCount_Injected(intPtr);
		}
	}

	public int alphaKeyCount
	{
		[FreeFunction("Gradient_Bindings::GetAlphaKeyCount", IsThreadSafe = true, HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_alphaKeyCount_Injected(intPtr);
		}
	}

	[NativeProperty(IsThreadSafe = true)]
	public GradientMode mode
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_mode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_mode_Injected(intPtr, value);
		}
	}

	[NativeProperty(IsThreadSafe = true)]
	public ColorSpace colorSpace
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_colorSpace_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_colorSpace_Injected(intPtr, value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "Gradient_Bindings::Init", IsThreadSafe = true)]
	private static extern IntPtr Init();

	[FreeFunction(Name = "Gradient_Bindings::Cleanup", IsThreadSafe = true, HasExplicitThis = true)]
	private void Cleanup()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Cleanup_Injected(intPtr);
	}

	[FreeFunction("Gradient_Bindings::Internal_Equals", IsThreadSafe = true, HasExplicitThis = true)]
	private bool Internal_Equals(IntPtr other)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_Equals_Injected(intPtr, other);
	}

	[RequiredByNativeCode]
	public Gradient()
	{
		m_Ptr = Init();
		m_RequiresNativeCleanup = true;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
	internal Gradient(IntPtr ptr)
	{
		m_Ptr = ptr;
		m_RequiresNativeCleanup = false;
	}

	~Gradient()
	{
		if (m_RequiresNativeCleanup)
		{
			Cleanup();
		}
	}

	[FreeFunction(Name = "Gradient_Bindings::Evaluate", IsThreadSafe = true, HasExplicitThis = true)]
	public Color Evaluate(float time)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Evaluate_Injected(intPtr, time, out var ret);
		return ret;
	}

	public void GetColorKeys(Span<GradientColorKey> keys)
	{
		if (colorKeyCount > keys.Length)
		{
			throw new ArgumentException("Destination array must be large enough to store the keys", "keys");
		}
		GetColorKeysWithSpan(keys);
	}

	public void GetAlphaKeys(Span<GradientAlphaKey> keys)
	{
		if (alphaKeyCount > keys.Length)
		{
			throw new ArgumentException("Destination array must be large enough to store the keys", "keys");
		}
		GetAlphaKeysWithSpan(keys);
	}

	[FreeFunction(Name = "Gradient_Bindings::SetColorKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
	public unsafe void SetColorKeys(ReadOnlySpan<GradientColorKey> keys)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<GradientColorKey> readOnlySpan = keys;
		fixed (GradientColorKey* begin = readOnlySpan)
		{
			ManagedSpanWrapper keys2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			SetColorKeys_Injected(intPtr, ref keys2);
		}
	}

	[FreeFunction(Name = "Gradient_Bindings::SetAlphaKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
	public unsafe void SetAlphaKeys(ReadOnlySpan<GradientAlphaKey> keys)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<GradientAlphaKey> readOnlySpan = keys;
		fixed (GradientAlphaKey* begin = readOnlySpan)
		{
			ManagedSpanWrapper keys2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			SetAlphaKeys_Injected(intPtr, ref keys2);
		}
	}

	[SecurityCritical]
	[FreeFunction(Name = "Gradient_Bindings::GetColorKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe void GetColorKeysWithSpan(Span<GradientColorKey> keys)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<GradientColorKey> span = keys;
		fixed (GradientColorKey* begin = span)
		{
			ManagedSpanWrapper keys2 = new ManagedSpanWrapper(begin, span.Length);
			GetColorKeysWithSpan_Injected(intPtr, ref keys2);
		}
	}

	[SecurityCritical]
	[FreeFunction(Name = "Gradient_Bindings::GetAlphaKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe void GetAlphaKeysWithSpan(Span<GradientAlphaKey> keys)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<GradientAlphaKey> span = keys;
		fixed (GradientAlphaKey* begin = span)
		{
			ManagedSpanWrapper keys2 = new ManagedSpanWrapper(begin, span.Length);
			GetAlphaKeysWithSpan_Injected(intPtr, ref keys2);
		}
	}

	public void SetKeys(GradientColorKey[] colorKeys, GradientAlphaKey[] alphaKeys)
	{
		SetKeys(MemoryExtensions.AsSpan(colorKeys), MemoryExtensions.AsSpan(alphaKeys));
	}

	[FreeFunction(Name = "Gradient_Bindings::SetKeysWithSpans", HasExplicitThis = true, IsThreadSafe = true)]
	public unsafe void SetKeys(ReadOnlySpan<GradientColorKey> colorKeys, ReadOnlySpan<GradientAlphaKey> alphaKeys)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<GradientColorKey> readOnlySpan = colorKeys;
		fixed (GradientColorKey* begin = readOnlySpan)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			ReadOnlySpan<GradientAlphaKey> readOnlySpan2 = alphaKeys;
			fixed (GradientAlphaKey* begin2 = readOnlySpan2)
			{
				ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
				SetKeys_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
		}
	}

	public override bool Equals(object o)
	{
		if (o == null)
		{
			return false;
		}
		if (this == o)
		{
			return true;
		}
		if (o.GetType() != GetType())
		{
			return false;
		}
		return Equals((Gradient)o);
	}

	public bool Equals(Gradient other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (m_Ptr.Equals(other.m_Ptr))
		{
			return true;
		}
		return Internal_Equals(other.m_Ptr);
	}

	public override int GetHashCode()
	{
		return m_Ptr.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Cleanup_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_Equals_Injected(IntPtr _unity_self, IntPtr other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Evaluate_Injected(IntPtr _unity_self, float time, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_colorKeys_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_colorKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_alphaKeys_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_alphaKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_colorKeyCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_alphaKeyCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetColorKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAlphaKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetColorKeysWithSpan_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAlphaKeysWithSpan_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GradientMode get_mode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mode_Injected(IntPtr _unity_self, GradientMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ColorSpace get_colorSpace_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_colorSpace_Injected(IntPtr _unity_self, ColorSpace value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colorKeys, ref ManagedSpanWrapper alphaKeys);
}
