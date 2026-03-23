using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Animation/Motion.h")]
public class Motion : Object
{
	public float averageDuration
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_averageDuration_Injected(intPtr);
		}
	}

	public float averageAngularSpeed
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_averageAngularSpeed_Injected(intPtr);
		}
	}

	public Vector3 averageSpeed
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_averageSpeed_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public float apparentSpeed
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_apparentSpeed_Injected(intPtr);
		}
	}

	public bool isLooping
	{
		[NativeMethod("IsLooping")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isLooping_Injected(intPtr);
		}
	}

	public bool legacy
	{
		[NativeMethod("IsLegacy")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_legacy_Injected(intPtr);
		}
	}

	public bool isHumanMotion
	{
		[NativeMethod("IsHumanMotion")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isHumanMotion_Injected(intPtr);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("isAnimatorMotion is not supported anymore, please use !legacy instead.", true)]
	public bool isAnimatorMotion { get; }

	protected Motion()
	{
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ValidateIfRetargetable is not supported anymore, please use isHumanMotion instead.", true)]
	public bool ValidateIfRetargetable(bool val)
	{
		return false;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_averageDuration_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_averageAngularSpeed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_averageSpeed_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_apparentSpeed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isLooping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_legacy_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isHumanMotion_Injected(IntPtr _unity_self);
}
