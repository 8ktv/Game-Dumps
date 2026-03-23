using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Physics2D/WheelJoint2D.h")]
public sealed class WheelJoint2D : AnchoredJoint2D
{
	public JointSuspension2D suspension
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_suspension_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_suspension_Injected(intPtr, ref value);
		}
	}

	public bool useMotor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useMotor_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useMotor_Injected(intPtr, value);
		}
	}

	public JointMotor2D motor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_motor_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_motor_Injected(intPtr, ref value);
		}
	}

	public float jointTranslation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_jointTranslation_Injected(intPtr);
		}
	}

	public float jointLinearSpeed
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_jointLinearSpeed_Injected(intPtr);
		}
	}

	public float jointSpeed
	{
		[NativeMethod("GetJointAngularSpeed")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_jointSpeed_Injected(intPtr);
		}
	}

	public float jointAngle
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_jointAngle_Injected(intPtr);
		}
	}

	public float GetMotorTorque(float timeStep)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetMotorTorque_Injected(intPtr, timeStep);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_suspension_Injected(IntPtr _unity_self, out JointSuspension2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_suspension_Injected(IntPtr _unity_self, [In] ref JointSuspension2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useMotor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useMotor_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_motor_Injected(IntPtr _unity_self, out JointMotor2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_motor_Injected(IntPtr _unity_self, [In] ref JointMotor2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_jointTranslation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_jointLinearSpeed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_jointSpeed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_jointAngle_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetMotorTorque_Injected(IntPtr _unity_self, float timeStep);
}
