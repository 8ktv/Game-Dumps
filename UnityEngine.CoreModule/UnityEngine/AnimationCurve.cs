using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Runtime/Math/AnimationCurve.bindings.h")]
[RequiredByNativeCode]
public class AnimationCurve : IEquatable<AnimationCurve>
{
	internal static class BindingsMarshaller
	{
		public static AnimationCurve ConvertToManaged(IntPtr ptr)
		{
			return new AnimationCurve(ptr, ownMemory: true);
		}

		public static IntPtr ConvertToNative(AnimationCurve animationCurve)
		{
			return animationCurve.m_Ptr;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
	internal IntPtr m_Ptr;

	private bool m_RequiresNativeCleanup;

	public unsafe Keyframe[] keys
	{
		[FreeFunction("AnimationCurveBindings::GetKeysArray", HasExplicitThis = true, IsThreadSafe = true)]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			Keyframe[] result;
			try
			{
				IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_keys_Injected(intPtr, out ret);
			}
			finally
			{
				Keyframe[] array = default(Keyframe[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[FreeFunction("AnimationCurveBindings::SetKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Keyframe> span = new Span<Keyframe>(value);
			fixed (Keyframe* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_keys_Injected(intPtr, ref value2);
			}
		}
	}

	public Keyframe this[int index] => GetKey(index);

	public int length
	{
		[NativeMethod("GetKeyCount", IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_length_Injected(intPtr);
		}
	}

	public WrapMode preWrapMode
	{
		[NativeMethod("GetPreInfinity", IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_preWrapMode_Injected(intPtr);
		}
		[NativeMethod("SetPreInfinity", IsThreadSafe = true)]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_preWrapMode_Injected(intPtr, value);
		}
	}

	public WrapMode postWrapMode
	{
		[NativeMethod("GetPostInfinity", IsThreadSafe = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_postWrapMode_Injected(intPtr);
		}
		[NativeMethod("SetPostInfinity", IsThreadSafe = true)]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_postWrapMode_Injected(intPtr, value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("AnimationCurveBindings::Internal_Destroy", IsThreadSafe = true)]
	private static extern void Internal_Destroy(IntPtr ptr);

	[FreeFunction("AnimationCurveBindings::Internal_Create", IsThreadSafe = true)]
	private unsafe static IntPtr Internal_Create(Keyframe[] keys)
	{
		Span<Keyframe> span = new Span<Keyframe>(keys);
		IntPtr result;
		fixed (Keyframe* begin = span)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, span.Length);
			result = Internal_Create_Injected(ref managedSpanWrapper);
		}
		return result;
	}

	[FreeFunction("AnimationCurveBindings::Internal_Equals", HasExplicitThis = true, IsThreadSafe = true)]
	private bool Internal_Equals(IntPtr other)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_Equals_Injected(intPtr, other);
	}

	[FreeFunction("AnimationCurveBindings::Internal_CopyFrom", HasExplicitThis = true, IsThreadSafe = true)]
	private void Internal_CopyFrom(IntPtr other)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_CopyFrom_Injected(intPtr, other);
	}

	~AnimationCurve()
	{
		if (m_RequiresNativeCleanup)
		{
			Internal_Destroy(m_Ptr);
		}
	}

	[ThreadSafe]
	public float Evaluate(float time)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Evaluate_Injected(intPtr, time);
	}

	[FreeFunction("AnimationCurveBindings::AddKeySmoothTangents", HasExplicitThis = true, IsThreadSafe = true)]
	public int AddKey(float time, float value)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddKey_Injected(intPtr, time, value);
	}

	public int AddKey(Keyframe key)
	{
		return AddKey_Internal(key);
	}

	[NativeMethod("AddKey", IsThreadSafe = true)]
	private int AddKey_Internal(Keyframe key)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddKey_Internal_Injected(intPtr, ref key);
	}

	[NativeThrows]
	[FreeFunction("AnimationCurveBindings::MoveKey", HasExplicitThis = true, IsThreadSafe = true)]
	public int MoveKey(int index, Keyframe key)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return MoveKey_Injected(intPtr, index, ref key);
	}

	[FreeFunction("AnimationCurveBindings::ClearKeys", HasExplicitThis = true, IsThreadSafe = true)]
	public void ClearKeys()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearKeys_Injected(intPtr);
	}

	[NativeThrows]
	[FreeFunction("AnimationCurveBindings::RemoveKey", HasExplicitThis = true, IsThreadSafe = true)]
	public void RemoveKey(int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveKey_Injected(intPtr, index);
	}

	[NativeThrows]
	[FreeFunction("AnimationCurveBindings::GetKey", HasExplicitThis = true, IsThreadSafe = true)]
	private Keyframe GetKey(int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetKey_Injected(intPtr, index, out var ret);
		return ret;
	}

	[FreeFunction("AnimationCurveBindings::GetKeysArray", HasExplicitThis = true, IsThreadSafe = true)]
	private Keyframe[] GetKeysArray()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Keyframe[] result;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetKeysArray_Injected(intPtr, out ret);
		}
		finally
		{
			Keyframe[] array = default(Keyframe[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public void GetKeys(Span<Keyframe> keys)
	{
		int num = length;
		if (num > keys.Length)
		{
			throw new ArgumentException("Destination array must be large enough to store the keys", "keys");
		}
		GetKeysWithSpan(keys);
	}

	[SecurityCritical]
	[FreeFunction(Name = "AnimationCurveBindings::GetKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
	private unsafe void GetKeysWithSpan(Span<Keyframe> keys)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Keyframe> span = keys;
		fixed (Keyframe* begin = span)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, span.Length);
			GetKeysWithSpan_Injected(intPtr, ref managedSpanWrapper);
		}
	}

	[FreeFunction("AnimationCurveBindings::SetKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
	public unsafe void SetKeys(ReadOnlySpan<Keyframe> keys)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<Keyframe> readOnlySpan = keys;
		fixed (Keyframe* begin = readOnlySpan)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			SetKeys_Injected(intPtr, ref managedSpanWrapper);
		}
	}

	[FreeFunction("AnimationCurveBindings::GetHashCode", HasExplicitThis = true, IsThreadSafe = true)]
	public override int GetHashCode()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetHashCode_Injected(intPtr);
	}

	[FreeFunction("AnimationCurveBindings::SmoothTangents", HasExplicitThis = true, IsThreadSafe = true)]
	[NativeThrows]
	public void SmoothTangents(int index, float weight)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SmoothTangents_Injected(intPtr, index, weight);
	}

	public static AnimationCurve Constant(float timeStart, float timeEnd, float value)
	{
		return Linear(timeStart, value, timeEnd, value);
	}

	public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
	{
		if (timeStart == timeEnd)
		{
			Keyframe keyframe = new Keyframe(timeStart, valueStart);
			return new AnimationCurve(keyframe);
		}
		float num = (valueEnd - valueStart) / (timeEnd - timeStart);
		Keyframe[] array = new Keyframe[2]
		{
			new Keyframe(timeStart, valueStart, 0f, num),
			new Keyframe(timeEnd, valueEnd, num, 0f)
		};
		return new AnimationCurve(array);
	}

	public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
	{
		if (timeStart == timeEnd)
		{
			Keyframe keyframe = new Keyframe(timeStart, valueStart);
			return new AnimationCurve(keyframe);
		}
		Keyframe[] array = new Keyframe[2]
		{
			new Keyframe(timeStart, valueStart, 0f, 0f),
			new Keyframe(timeEnd, valueEnd, 0f, 0f)
		};
		return new AnimationCurve(array);
	}

	public AnimationCurve(params Keyframe[] keys)
	{
		m_Ptr = Internal_Create(keys);
		m_RequiresNativeCleanup = true;
	}

	[RequiredByNativeCode]
	public AnimationCurve()
	{
		m_Ptr = Internal_Create(null);
		m_RequiresNativeCleanup = true;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
	internal AnimationCurve(IntPtr ptr, bool ownMemory)
	{
		m_Ptr = ptr;
		m_RequiresNativeCleanup = ownMemory;
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
		return o.GetType() == GetType() && Equals((AnimationCurve)o);
	}

	public bool Equals(AnimationCurve other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return m_Ptr.Equals(other.m_Ptr) || Internal_Equals(other.m_Ptr);
	}

	public void CopyFrom(AnimationCurve other)
	{
		Internal_CopyFrom(other.m_Ptr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_Create_Injected(ref ManagedSpanWrapper keys);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_Equals_Injected(IntPtr _unity_self, IntPtr other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CopyFrom_Injected(IntPtr _unity_self, IntPtr other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float Evaluate_Injected(IntPtr _unity_self, float time);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_keys_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_keys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddKey_Injected(IntPtr _unity_self, float time, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddKey_Internal_Injected(IntPtr _unity_self, [In] ref Keyframe key);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int MoveKey_Injected(IntPtr _unity_self, int index, [In] ref Keyframe key);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearKeys_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveKey_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_length_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetKey_Injected(IntPtr _unity_self, int index, out Keyframe ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetKeysArray_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetKeysWithSpan_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetHashCode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SmoothTangents_Injected(IntPtr _unity_self, int index, float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern WrapMode get_preWrapMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_preWrapMode_Injected(IntPtr _unity_self, WrapMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern WrapMode get_postWrapMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_postWrapMode_Injected(IntPtr _unity_self, WrapMode value);
}
