using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeHeader("Modules/Animation/Avatar.h")]
public class Avatar : Object
{
	public bool isValid
	{
		[NativeMethod("IsValid")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isValid_Injected(intPtr);
		}
	}

	public bool isHuman
	{
		[NativeMethod("IsHuman")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isHuman_Injected(intPtr);
		}
	}

	public HumanDescription humanDescription
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_humanDescription_Injected(intPtr, out var ret);
			return ret;
		}
	}

	private Avatar()
	{
	}

	internal void SetMuscleMinMax(int muscleId, float min, float max)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMuscleMinMax_Injected(intPtr, muscleId, min, max);
	}

	internal void SetParameter(int parameterId, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetParameter_Injected(intPtr, parameterId, value);
	}

	internal float GetAxisLength(int humanId)
	{
		return Internal_GetAxisLength(HumanTrait.GetBoneIndexFromMono(humanId));
	}

	internal Quaternion GetPreRotation(int humanId)
	{
		return Internal_GetPreRotation(HumanTrait.GetBoneIndexFromMono(humanId));
	}

	internal Quaternion GetPostRotation(int humanId)
	{
		return Internal_GetPostRotation(HumanTrait.GetBoneIndexFromMono(humanId));
	}

	internal Quaternion GetZYPostQ(int humanId, Quaternion parentQ, Quaternion q)
	{
		return Internal_GetZYPostQ(HumanTrait.GetBoneIndexFromMono(humanId), parentQ, q);
	}

	internal Quaternion GetZYRoll(int humanId, Vector3 uvw)
	{
		return Internal_GetZYRoll(HumanTrait.GetBoneIndexFromMono(humanId), uvw);
	}

	internal Vector3 GetLimitSign(int humanId)
	{
		return Internal_GetLimitSign(HumanTrait.GetBoneIndexFromMono(humanId));
	}

	[NativeMethod("GetAxisLength")]
	internal float Internal_GetAxisLength(int humanId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetAxisLength_Injected(intPtr, humanId);
	}

	[NativeMethod("GetPreRotation")]
	internal Quaternion Internal_GetPreRotation(int humanId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetPreRotation_Injected(intPtr, humanId, out var ret);
		return ret;
	}

	[NativeMethod("GetPostRotation")]
	internal Quaternion Internal_GetPostRotation(int humanId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetPostRotation_Injected(intPtr, humanId, out var ret);
		return ret;
	}

	[NativeMethod("GetZYPostQ")]
	internal Quaternion Internal_GetZYPostQ(int humanId, Quaternion parentQ, Quaternion q)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetZYPostQ_Injected(intPtr, humanId, ref parentQ, ref q, out var ret);
		return ret;
	}

	[NativeMethod("GetZYRoll")]
	internal Quaternion Internal_GetZYRoll(int humanId, Vector3 uvw)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetZYRoll_Injected(intPtr, humanId, ref uvw, out var ret);
		return ret;
	}

	[NativeMethod("GetLimitSign")]
	internal Vector3 Internal_GetLimitSign(int humanId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetLimitSign_Injected(intPtr, humanId, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isValid_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isHuman_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_humanDescription_Injected(IntPtr _unity_self, out HumanDescription ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMuscleMinMax_Injected(IntPtr _unity_self, int muscleId, float min, float max);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetParameter_Injected(IntPtr _unity_self, int parameterId, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float Internal_GetAxisLength_Injected(IntPtr _unity_self, int humanId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetPreRotation_Injected(IntPtr _unity_self, int humanId, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetPostRotation_Injected(IntPtr _unity_self, int humanId, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetZYPostQ_Injected(IntPtr _unity_self, int humanId, [In] ref Quaternion parentQ, [In] ref Quaternion q, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetZYRoll_Injected(IntPtr _unity_self, int humanId, [In] ref Vector3 uvw, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetLimitSign_Injected(IntPtr _unity_self, int humanId, out Vector3 ret);
}
