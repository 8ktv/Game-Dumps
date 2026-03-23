using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations;

[RequiredByNativeCode]
[NativeHeader("Modules/Animation/Director/AnimationHumanStream.h")]
[MovedFrom("UnityEngine.Experimental.Animations")]
[NativeHeader("Modules/Animation/ScriptBindings/AnimationHumanStream.bindings.h")]
public struct AnimationHumanStream
{
	private IntPtr stream;

	public bool isValid => stream != IntPtr.Zero;

	public float humanScale
	{
		get
		{
			ThrowIfInvalid();
			return GetHumanScale();
		}
	}

	public float leftFootHeight
	{
		get
		{
			ThrowIfInvalid();
			return GetFootHeight(left: true);
		}
	}

	public float rightFootHeight
	{
		get
		{
			ThrowIfInvalid();
			return GetFootHeight(left: false);
		}
	}

	public Vector3 bodyLocalPosition
	{
		get
		{
			ThrowIfInvalid();
			return InternalGetBodyLocalPosition();
		}
		set
		{
			ThrowIfInvalid();
			InternalSetBodyLocalPosition(value);
		}
	}

	public Quaternion bodyLocalRotation
	{
		get
		{
			ThrowIfInvalid();
			return InternalGetBodyLocalRotation();
		}
		set
		{
			ThrowIfInvalid();
			InternalSetBodyLocalRotation(value);
		}
	}

	public Vector3 bodyPosition
	{
		get
		{
			ThrowIfInvalid();
			return InternalGetBodyPosition();
		}
		set
		{
			ThrowIfInvalid();
			InternalSetBodyPosition(value);
		}
	}

	public Quaternion bodyRotation
	{
		get
		{
			ThrowIfInvalid();
			return InternalGetBodyRotation();
		}
		set
		{
			ThrowIfInvalid();
			InternalSetBodyRotation(value);
		}
	}

	public Vector3 leftFootVelocity
	{
		get
		{
			ThrowIfInvalid();
			return GetLeftFootVelocity();
		}
	}

	public Vector3 rightFootVelocity
	{
		get
		{
			ThrowIfInvalid();
			return GetRightFootVelocity();
		}
	}

	private void ThrowIfInvalid()
	{
		if (!isValid)
		{
			throw new InvalidOperationException("The AnimationHumanStream is invalid.");
		}
	}

	public float GetMuscle(MuscleHandle muscle)
	{
		ThrowIfInvalid();
		return InternalGetMuscle(muscle);
	}

	public void SetMuscle(MuscleHandle muscle, float value)
	{
		ThrowIfInvalid();
		InternalSetMuscle(muscle, value);
	}

	public void ResetToStancePose()
	{
		ThrowIfInvalid();
		InternalResetToStancePose();
	}

	public Vector3 GetGoalPositionFromPose(AvatarIKGoal index)
	{
		ThrowIfInvalid();
		return InternalGetGoalPositionFromPose(index);
	}

	public Quaternion GetGoalRotationFromPose(AvatarIKGoal index)
	{
		ThrowIfInvalid();
		return InternalGetGoalRotationFromPose(index);
	}

	public Vector3 GetGoalLocalPosition(AvatarIKGoal index)
	{
		ThrowIfInvalid();
		return InternalGetGoalLocalPosition(index);
	}

	public void SetGoalLocalPosition(AvatarIKGoal index, Vector3 pos)
	{
		ThrowIfInvalid();
		InternalSetGoalLocalPosition(index, pos);
	}

	public Quaternion GetGoalLocalRotation(AvatarIKGoal index)
	{
		ThrowIfInvalid();
		return InternalGetGoalLocalRotation(index);
	}

	public void SetGoalLocalRotation(AvatarIKGoal index, Quaternion rot)
	{
		ThrowIfInvalid();
		InternalSetGoalLocalRotation(index, rot);
	}

	public Vector3 GetGoalPosition(AvatarIKGoal index)
	{
		ThrowIfInvalid();
		return InternalGetGoalPosition(index);
	}

	public void SetGoalPosition(AvatarIKGoal index, Vector3 pos)
	{
		ThrowIfInvalid();
		InternalSetGoalPosition(index, pos);
	}

	public Quaternion GetGoalRotation(AvatarIKGoal index)
	{
		ThrowIfInvalid();
		return InternalGetGoalRotation(index);
	}

	public void SetGoalRotation(AvatarIKGoal index, Quaternion rot)
	{
		ThrowIfInvalid();
		InternalSetGoalRotation(index, rot);
	}

	public void SetGoalWeightPosition(AvatarIKGoal index, float value)
	{
		ThrowIfInvalid();
		InternalSetGoalWeightPosition(index, value);
	}

	public void SetGoalWeightRotation(AvatarIKGoal index, float value)
	{
		ThrowIfInvalid();
		InternalSetGoalWeightRotation(index, value);
	}

	public float GetGoalWeightPosition(AvatarIKGoal index)
	{
		ThrowIfInvalid();
		return InternalGetGoalWeightPosition(index);
	}

	public float GetGoalWeightRotation(AvatarIKGoal index)
	{
		ThrowIfInvalid();
		return InternalGetGoalWeightRotation(index);
	}

	public Vector3 GetHintPosition(AvatarIKHint index)
	{
		ThrowIfInvalid();
		return InternalGetHintPosition(index);
	}

	public void SetHintPosition(AvatarIKHint index, Vector3 pos)
	{
		ThrowIfInvalid();
		InternalSetHintPosition(index, pos);
	}

	public void SetHintWeightPosition(AvatarIKHint index, float value)
	{
		ThrowIfInvalid();
		InternalSetHintWeightPosition(index, value);
	}

	public float GetHintWeightPosition(AvatarIKHint index)
	{
		ThrowIfInvalid();
		return InternalGetHintWeightPosition(index);
	}

	public void SetLookAtPosition(Vector3 lookAtPosition)
	{
		ThrowIfInvalid();
		InternalSetLookAtPosition(lookAtPosition);
	}

	public void SetLookAtClampWeight(float weight)
	{
		ThrowIfInvalid();
		InternalSetLookAtClampWeight(weight);
	}

	public void SetLookAtBodyWeight(float weight)
	{
		ThrowIfInvalid();
		InternalSetLookAtBodyWeight(weight);
	}

	public void SetLookAtHeadWeight(float weight)
	{
		ThrowIfInvalid();
		InternalSetLookAtHeadWeight(weight);
	}

	public void SetLookAtEyesWeight(float weight)
	{
		ThrowIfInvalid();
		InternalSetLookAtEyesWeight(weight);
	}

	public void SolveIK()
	{
		ThrowIfInvalid();
		InternalSolveIK();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern float GetHumanScale();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern float GetFootHeight(bool left);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "ResetToStancePose", IsThreadSafe = true)]
	private extern void InternalResetToStancePose();

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalPositionFromPose", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 InternalGetGoalPositionFromPose(AvatarIKGoal index)
	{
		InternalGetGoalPositionFromPose_Injected(ref this, index, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalRotationFromPose", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Quaternion InternalGetGoalRotationFromPose(AvatarIKGoal index)
	{
		InternalGetGoalRotationFromPose_Injected(ref this, index, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 InternalGetBodyLocalPosition()
	{
		InternalGetBodyLocalPosition_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetBodyLocalPosition(Vector3 value)
	{
		InternalSetBodyLocalPosition_Injected(ref this, ref value);
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Quaternion InternalGetBodyLocalRotation()
	{
		InternalGetBodyLocalRotation_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetBodyLocalRotation(Quaternion value)
	{
		InternalSetBodyLocalRotation_Injected(ref this, ref value);
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 InternalGetBodyPosition()
	{
		InternalGetBodyPosition_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetBodyPosition(Vector3 value)
	{
		InternalSetBodyPosition_Injected(ref this, ref value);
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Quaternion InternalGetBodyRotation()
	{
		InternalGetBodyRotation_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetBodyRotation(Quaternion value)
	{
		InternalSetBodyRotation_Injected(ref this, ref value);
	}

	[NativeMethod(Name = "GetMuscle", IsThreadSafe = true)]
	private float InternalGetMuscle(MuscleHandle muscle)
	{
		return InternalGetMuscle_Injected(ref this, ref muscle);
	}

	[NativeMethod(Name = "SetMuscle", IsThreadSafe = true)]
	private void InternalSetMuscle(MuscleHandle muscle, float value)
	{
		InternalSetMuscle_Injected(ref this, ref muscle, value);
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetLeftFootVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 GetLeftFootVelocity()
	{
		GetLeftFootVelocity_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetRightFootVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 GetRightFootVelocity()
	{
		GetRightFootVelocity_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 InternalGetGoalLocalPosition(AvatarIKGoal index)
	{
		InternalGetGoalLocalPosition_Injected(ref this, index, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetGoalLocalPosition(AvatarIKGoal index, Vector3 pos)
	{
		InternalSetGoalLocalPosition_Injected(ref this, index, ref pos);
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Quaternion InternalGetGoalLocalRotation(AvatarIKGoal index)
	{
		InternalGetGoalLocalRotation_Injected(ref this, index, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetGoalLocalRotation(AvatarIKGoal index, Quaternion rot)
	{
		InternalSetGoalLocalRotation_Injected(ref this, index, ref rot);
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 InternalGetGoalPosition(AvatarIKGoal index)
	{
		InternalGetGoalPosition_Injected(ref this, index, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetGoalPosition(AvatarIKGoal index, Vector3 pos)
	{
		InternalSetGoalPosition_Injected(ref this, index, ref pos);
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Quaternion InternalGetGoalRotation(AvatarIKGoal index)
	{
		InternalGetGoalRotation_Injected(ref this, index, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetGoalRotation(AvatarIKGoal index, Quaternion rot)
	{
		InternalSetGoalRotation_Injected(ref this, index, ref rot);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetGoalWeightPosition", IsThreadSafe = true)]
	private extern void InternalSetGoalWeightPosition(AvatarIKGoal index, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetGoalWeightRotation", IsThreadSafe = true)]
	private extern void InternalSetGoalWeightRotation(AvatarIKGoal index, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetGoalWeightPosition", IsThreadSafe = true)]
	private extern float InternalGetGoalWeightPosition(AvatarIKGoal index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetGoalWeightRotation", IsThreadSafe = true)]
	private extern float InternalGetGoalWeightRotation(AvatarIKGoal index);

	[NativeMethod(Name = "AnimationHumanStreamBindings::GetHintPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 InternalGetHintPosition(AvatarIKHint index)
	{
		InternalGetHintPosition_Injected(ref this, index, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetHintPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetHintPosition(AvatarIKHint index, Vector3 pos)
	{
		InternalSetHintPosition_Injected(ref this, index, ref pos);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetHintWeightPosition", IsThreadSafe = true)]
	private extern void InternalSetHintWeightPosition(AvatarIKHint index, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetHintWeightPosition", IsThreadSafe = true)]
	private extern float InternalGetHintWeightPosition(AvatarIKHint index);

	[NativeMethod(Name = "AnimationHumanStreamBindings::SetLookAtPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void InternalSetLookAtPosition(Vector3 lookAtPosition)
	{
		InternalSetLookAtPosition_Injected(ref this, ref lookAtPosition);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetLookAtClampWeight", IsThreadSafe = true)]
	private extern void InternalSetLookAtClampWeight(float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetLookAtBodyWeight", IsThreadSafe = true)]
	private extern void InternalSetLookAtBodyWeight(float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetLookAtHeadWeight", IsThreadSafe = true)]
	private extern void InternalSetLookAtHeadWeight(float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetLookAtEyesWeight", IsThreadSafe = true)]
	private extern void InternalSetLookAtEyesWeight(float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SolveIK", IsThreadSafe = true)]
	private extern void InternalSolveIK();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetGoalPositionFromPose_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetGoalRotationFromPose_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetBodyLocalPosition_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetBodyLocalPosition_Injected(ref AnimationHumanStream _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetBodyLocalRotation_Injected(ref AnimationHumanStream _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetBodyLocalRotation_Injected(ref AnimationHumanStream _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetBodyPosition_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetBodyPosition_Injected(ref AnimationHumanStream _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetBodyRotation_Injected(ref AnimationHumanStream _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetBodyRotation_Injected(ref AnimationHumanStream _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float InternalGetMuscle_Injected(ref AnimationHumanStream _unity_self, [In] ref MuscleHandle muscle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetMuscle_Injected(ref AnimationHumanStream _unity_self, [In] ref MuscleHandle muscle, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLeftFootVelocity_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRightFootVelocity_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetGoalLocalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetGoalLocalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, [In] ref Vector3 pos);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetGoalLocalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetGoalLocalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, [In] ref Quaternion rot);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetGoalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetGoalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, [In] ref Vector3 pos);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetGoalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetGoalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, [In] ref Quaternion rot);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetHintPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKHint index, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetHintPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKHint index, [In] ref Vector3 pos);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetLookAtPosition_Injected(ref AnimationHumanStream _unity_self, [In] ref Vector3 lookAtPosition);
}
