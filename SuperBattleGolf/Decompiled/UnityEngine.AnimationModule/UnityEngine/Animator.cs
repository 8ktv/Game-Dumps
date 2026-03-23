using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[NativeHeader("Modules/Animation/ScriptBindings/AnimatorControllerParameter.bindings.h")]
[NativeHeader("Modules/Animation/Animator.h")]
[NativeHeader("Modules/Animation/ScriptBindings/Animator.bindings.h")]
public class Animator : Behaviour
{
	public bool isOptimizable
	{
		[NativeMethod("IsOptimizable")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isOptimizable_Injected(intPtr);
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

	public bool hasRootMotion
	{
		[NativeMethod("HasRootMotion")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasRootMotion_Injected(intPtr);
		}
	}

	internal bool isRootPositionOrRotationControlledByCurves
	{
		[NativeMethod("IsRootTranslationOrRotationControllerByCurves")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isRootPositionOrRotationControlledByCurves_Injected(intPtr);
		}
	}

	public float humanScale
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_humanScale_Injected(intPtr);
		}
	}

	public bool isInitialized
	{
		[NativeMethod("IsInitialized")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isInitialized_Injected(intPtr);
		}
	}

	public Vector3 deltaPosition
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_deltaPosition_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Quaternion deltaRotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_deltaRotation_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Vector3 velocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_velocity_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Vector3 angularVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_angularVelocity_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Vector3 rootPosition
	{
		[NativeMethod("GetAvatarPosition")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rootPosition_Injected(intPtr, out var ret);
			return ret;
		}
		[NativeMethod("SetAvatarPosition")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rootPosition_Injected(intPtr, ref value);
		}
	}

	public Quaternion rootRotation
	{
		[NativeMethod("GetAvatarRotation")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rootRotation_Injected(intPtr, out var ret);
			return ret;
		}
		[NativeMethod("SetAvatarRotation")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rootRotation_Injected(intPtr, ref value);
		}
	}

	public bool applyRootMotion
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_applyRootMotion_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_applyRootMotion_Injected(intPtr, value);
		}
	}

	[Obsolete("Animator.linearVelocityBlending is no longer used and has been deprecated.")]
	public bool linearVelocityBlending
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_linearVelocityBlending_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_linearVelocityBlending_Injected(intPtr, value);
		}
	}

	public bool animatePhysics
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_animatePhysics_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_animatePhysics_Injected(intPtr, value);
		}
	}

	public AnimatorUpdateMode updateMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_updateMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_updateMode_Injected(intPtr, value);
		}
	}

	public bool hasTransformHierarchy
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasTransformHierarchy_Injected(intPtr);
		}
	}

	internal bool allowConstantClipSamplingOptimization
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowConstantClipSamplingOptimization_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowConstantClipSamplingOptimization_Injected(intPtr, value);
		}
	}

	public float gravityWeight
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_gravityWeight_Injected(intPtr);
		}
	}

	public Vector3 bodyPosition
	{
		get
		{
			CheckIfInIKPass();
			return bodyPositionInternal;
		}
		set
		{
			CheckIfInIKPass();
			bodyPositionInternal = value;
		}
	}

	internal Vector3 bodyPositionInternal
	{
		[NativeMethod("GetBodyPosition")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bodyPositionInternal_Injected(intPtr, out var ret);
			return ret;
		}
		[NativeMethod("SetBodyPosition")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bodyPositionInternal_Injected(intPtr, ref value);
		}
	}

	public Quaternion bodyRotation
	{
		get
		{
			CheckIfInIKPass();
			return bodyRotationInternal;
		}
		set
		{
			CheckIfInIKPass();
			bodyRotationInternal = value;
		}
	}

	internal Quaternion bodyRotationInternal
	{
		[NativeMethod("GetBodyRotation")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bodyRotationInternal_Injected(intPtr, out var ret);
			return ret;
		}
		[NativeMethod("SetBodyRotation")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bodyRotationInternal_Injected(intPtr, ref value);
		}
	}

	public bool stabilizeFeet
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stabilizeFeet_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_stabilizeFeet_Injected(intPtr, value);
		}
	}

	public int layerCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_layerCount_Injected(intPtr);
		}
	}

	public AnimatorControllerParameter[] parameters
	{
		[FreeFunction(Name = "AnimatorBindings::GetParameters", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_parameters_Injected(intPtr);
		}
	}

	public int parameterCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_parameterCount_Injected(intPtr);
		}
	}

	public float feetPivotActive
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_feetPivotActive_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_feetPivotActive_Injected(intPtr, value);
		}
	}

	public float pivotWeight
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_pivotWeight_Injected(intPtr);
		}
	}

	public Vector3 pivotPosition
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_pivotPosition_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public bool isMatchingTarget
	{
		[NativeMethod("IsMatchingTarget")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isMatchingTarget_Injected(intPtr);
		}
	}

	public float speed
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_speed_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_speed_Injected(intPtr, value);
		}
	}

	public Vector3 targetPosition
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_targetPosition_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Quaternion targetRotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_targetRotation_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public Transform avatarRoot
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(get_avatarRoot_Injected(intPtr));
		}
	}

	public AnimatorCullingMode cullingMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cullingMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cullingMode_Injected(intPtr, value);
		}
	}

	public float playbackTime
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_playbackTime_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_playbackTime_Injected(intPtr, value);
		}
	}

	public float recorderStartTime
	{
		get
		{
			return GetRecorderStartTime();
		}
		set
		{
		}
	}

	public float recorderStopTime
	{
		get
		{
			return GetRecorderStopTime();
		}
		set
		{
		}
	}

	public AnimatorRecorderMode recorderMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_recorderMode_Injected(intPtr);
		}
	}

	public RuntimeAnimatorController runtimeAnimatorController
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<RuntimeAnimatorController>(get_runtimeAnimatorController_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_runtimeAnimatorController_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public bool hasBoundPlayables
	{
		[NativeMethod("HasBoundPlayables")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasBoundPlayables_Injected(intPtr);
		}
	}

	public Avatar avatar
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Avatar>(get_avatar_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_avatar_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public PlayableGraph playableGraph
	{
		get
		{
			PlayableGraph graph = default(PlayableGraph);
			GetCurrentGraph(ref graph);
			return graph;
		}
	}

	public bool layersAffectMassCenter
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_layersAffectMassCenter_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_layersAffectMassCenter_Injected(intPtr, value);
		}
	}

	public float leftFeetBottomHeight
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_leftFeetBottomHeight_Injected(intPtr);
		}
	}

	public float rightFeetBottomHeight
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rightFeetBottomHeight_Injected(intPtr);
		}
	}

	[NativeConditional("UNITY_EDITOR")]
	internal bool supportsOnAnimatorMove
	{
		[NativeMethod("SupportsOnAnimatorMove")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_supportsOnAnimatorMove_Injected(intPtr);
		}
	}

	public bool logWarnings
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_logWarnings_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_logWarnings_Injected(intPtr, value);
		}
	}

	public bool fireEvents
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_fireEvents_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_fireEvents_Injected(intPtr, value);
		}
	}

	[Obsolete("keepAnimatorControllerStateOnDisable is deprecated, use keepAnimatorStateOnDisable instead. (UnityUpgradable) -> keepAnimatorStateOnDisable", false)]
	public bool keepAnimatorControllerStateOnDisable
	{
		get
		{
			return keepAnimatorStateOnDisable;
		}
		set
		{
			keepAnimatorStateOnDisable = value;
		}
	}

	public bool keepAnimatorStateOnDisable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_keepAnimatorStateOnDisable_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_keepAnimatorStateOnDisable_Injected(intPtr, value);
		}
	}

	public bool writeDefaultValuesOnDisable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_writeDefaultValuesOnDisable_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_writeDefaultValuesOnDisable_Injected(intPtr, value);
		}
	}

	public float GetFloat(string name)
	{
		return GetFloatString(name);
	}

	public float GetFloat(int id)
	{
		return GetFloatID(id);
	}

	public void SetFloat(string name, float value)
	{
		SetFloatString(name, value);
	}

	public void SetFloat(string name, float value, float dampTime, float deltaTime)
	{
		SetFloatStringDamp(name, value, dampTime, deltaTime);
	}

	public void SetFloat(int id, float value)
	{
		SetFloatID(id, value);
	}

	public void SetFloat(int id, float value, float dampTime, float deltaTime)
	{
		SetFloatIDDamp(id, value, dampTime, deltaTime);
	}

	public bool GetBool(string name)
	{
		return GetBoolString(name);
	}

	public bool GetBool(int id)
	{
		return GetBoolID(id);
	}

	public void SetBool(string name, bool value)
	{
		SetBoolString(name, value);
	}

	public void SetBool(int id, bool value)
	{
		SetBoolID(id, value);
	}

	public int GetInteger(string name)
	{
		return GetIntegerString(name);
	}

	public int GetInteger(int id)
	{
		return GetIntegerID(id);
	}

	public void SetInteger(string name, int value)
	{
		SetIntegerString(name, value);
	}

	public void SetInteger(int id, int value)
	{
		SetIntegerID(id, value);
	}

	public void SetTrigger(string name)
	{
		SetTriggerString(name);
	}

	public void SetTrigger(int id)
	{
		SetTriggerID(id);
	}

	public void ResetTrigger(string name)
	{
		ResetTriggerString(name);
	}

	public void ResetTrigger(int id)
	{
		ResetTriggerID(id);
	}

	public bool IsParameterControlledByCurve(string name)
	{
		return IsParameterControlledByCurveString(name);
	}

	public bool IsParameterControlledByCurve(int id)
	{
		return IsParameterControlledByCurveID(id);
	}

	public Vector3 GetIKPosition(AvatarIKGoal goal)
	{
		CheckIfInIKPass();
		return GetGoalPosition(goal);
	}

	private Vector3 GetGoalPosition(AvatarIKGoal goal)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetGoalPosition_Injected(intPtr, goal, out var ret);
		return ret;
	}

	public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
	{
		CheckIfInIKPass();
		SetGoalPosition(goal, goalPosition);
	}

	private void SetGoalPosition(AvatarIKGoal goal, Vector3 goalPosition)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetGoalPosition_Injected(intPtr, goal, ref goalPosition);
	}

	public Quaternion GetIKRotation(AvatarIKGoal goal)
	{
		CheckIfInIKPass();
		return GetGoalRotation(goal);
	}

	private Quaternion GetGoalRotation(AvatarIKGoal goal)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetGoalRotation_Injected(intPtr, goal, out var ret);
		return ret;
	}

	public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
	{
		CheckIfInIKPass();
		SetGoalRotation(goal, goalRotation);
	}

	private void SetGoalRotation(AvatarIKGoal goal, Quaternion goalRotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetGoalRotation_Injected(intPtr, goal, ref goalRotation);
	}

	public float GetIKPositionWeight(AvatarIKGoal goal)
	{
		CheckIfInIKPass();
		return GetGoalWeightPosition(goal);
	}

	private float GetGoalWeightPosition(AvatarIKGoal goal)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetGoalWeightPosition_Injected(intPtr, goal);
	}

	public void SetIKPositionWeight(AvatarIKGoal goal, float value)
	{
		CheckIfInIKPass();
		SetGoalWeightPosition(goal, value);
	}

	private void SetGoalWeightPosition(AvatarIKGoal goal, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetGoalWeightPosition_Injected(intPtr, goal, value);
	}

	public float GetIKRotationWeight(AvatarIKGoal goal)
	{
		CheckIfInIKPass();
		return GetGoalWeightRotation(goal);
	}

	private float GetGoalWeightRotation(AvatarIKGoal goal)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetGoalWeightRotation_Injected(intPtr, goal);
	}

	public void SetIKRotationWeight(AvatarIKGoal goal, float value)
	{
		CheckIfInIKPass();
		SetGoalWeightRotation(goal, value);
	}

	private void SetGoalWeightRotation(AvatarIKGoal goal, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetGoalWeightRotation_Injected(intPtr, goal, value);
	}

	public Vector3 GetIKHintPosition(AvatarIKHint hint)
	{
		CheckIfInIKPass();
		return GetHintPosition(hint);
	}

	private Vector3 GetHintPosition(AvatarIKHint hint)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetHintPosition_Injected(intPtr, hint, out var ret);
		return ret;
	}

	public void SetIKHintPosition(AvatarIKHint hint, Vector3 hintPosition)
	{
		CheckIfInIKPass();
		SetHintPosition(hint, hintPosition);
	}

	private void SetHintPosition(AvatarIKHint hint, Vector3 hintPosition)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetHintPosition_Injected(intPtr, hint, ref hintPosition);
	}

	public float GetIKHintPositionWeight(AvatarIKHint hint)
	{
		CheckIfInIKPass();
		return GetHintWeightPosition(hint);
	}

	private float GetHintWeightPosition(AvatarIKHint hint)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetHintWeightPosition_Injected(intPtr, hint);
	}

	public void SetIKHintPositionWeight(AvatarIKHint hint, float value)
	{
		CheckIfInIKPass();
		SetHintWeightPosition(hint, value);
	}

	private void SetHintWeightPosition(AvatarIKHint hint, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetHintWeightPosition_Injected(intPtr, hint, value);
	}

	public void SetLookAtPosition(Vector3 lookAtPosition)
	{
		CheckIfInIKPass();
		SetLookAtPositionInternal(lookAtPosition);
	}

	[NativeMethod("SetLookAtPosition")]
	private void SetLookAtPositionInternal(Vector3 lookAtPosition)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLookAtPositionInternal_Injected(intPtr, ref lookAtPosition);
	}

	public void SetLookAtWeight(float weight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, 0f, 1f, 0f, 0.5f);
	}

	public void SetLookAtWeight(float weight, float bodyWeight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, bodyWeight, 1f, 0f, 0.5f);
	}

	public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, bodyWeight, headWeight, 0f, 0.5f);
	}

	public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, 0.5f);
	}

	public void SetLookAtWeight(float weight, [UnityEngine.Internal.DefaultValue("0.0f")] float bodyWeight, [UnityEngine.Internal.DefaultValue("1.0f")] float headWeight, [UnityEngine.Internal.DefaultValue("0.0f")] float eyesWeight, [UnityEngine.Internal.DefaultValue("0.5f")] float clampWeight)
	{
		CheckIfInIKPass();
		SetLookAtWeightInternal(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
	}

	[NativeMethod("SetLookAtWeight")]
	private void SetLookAtWeightInternal(float weight, float bodyWeight, float headWeight, float eyesWeight, float clampWeight)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLookAtWeightInternal_Injected(intPtr, weight, bodyWeight, headWeight, eyesWeight, clampWeight);
	}

	public void SetBoneLocalRotation(HumanBodyBones humanBoneId, Quaternion rotation)
	{
		CheckIfInIKPass();
		SetBoneLocalRotationInternal(HumanTrait.GetBoneIndexFromMono((int)humanBoneId), rotation);
	}

	[NativeMethod("SetBoneLocalRotation")]
	private void SetBoneLocalRotationInternal(int humanBoneId, Quaternion rotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBoneLocalRotationInternal_Injected(intPtr, humanBoneId, ref rotation);
	}

	private ScriptableObject GetBehaviour([NotNull] Type type)
	{
		if ((object)type == null)
		{
			ThrowHelper.ThrowArgumentNullException(type, "type");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<ScriptableObject>(GetBehaviour_Injected(intPtr, type));
	}

	public T GetBehaviour<T>() where T : StateMachineBehaviour
	{
		return GetBehaviour(typeof(T)) as T;
	}

	private static T[] ConvertStateMachineBehaviour<T>(ScriptableObject[] rawObjects) where T : StateMachineBehaviour
	{
		if (rawObjects == null)
		{
			return null;
		}
		T[] array = new T[rawObjects.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (T)rawObjects[i];
		}
		return array;
	}

	public T[] GetBehaviours<T>() where T : StateMachineBehaviour
	{
		return ConvertStateMachineBehaviour<T>(InternalGetBehaviours(typeof(T)));
	}

	[FreeFunction(Name = "AnimatorBindings::InternalGetBehaviours", HasExplicitThis = true)]
	internal ScriptableObject[] InternalGetBehaviours([NotNull] Type type)
	{
		if ((object)type == null)
		{
			ThrowHelper.ThrowArgumentNullException(type, "type");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return InternalGetBehaviours_Injected(intPtr, type);
	}

	public StateMachineBehaviour[] GetBehaviours(int fullPathHash, int layerIndex)
	{
		return InternalGetBehavioursByKey(fullPathHash, layerIndex, typeof(StateMachineBehaviour)) as StateMachineBehaviour[];
	}

	[FreeFunction(Name = "AnimatorBindings::InternalGetBehavioursByKey", HasExplicitThis = true)]
	internal ScriptableObject[] InternalGetBehavioursByKey(int fullPathHash, int layerIndex, [NotNull] Type type)
	{
		if ((object)type == null)
		{
			ThrowHelper.ThrowArgumentNullException(type, "type");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return InternalGetBehavioursByKey_Injected(intPtr, fullPathHash, layerIndex, type);
	}

	public string GetLayerName(int layerIndex)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetLayerName_Injected(intPtr, layerIndex, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	public unsafe int GetLayerIndex(string layerName)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(layerName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(layerName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetLayerIndex_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetLayerIndex_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public float GetLayerWeight(int layerIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetLayerWeight_Injected(intPtr, layerIndex);
	}

	public void SetLayerWeight(int layerIndex, float weight)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLayerWeight_Injected(intPtr, layerIndex, weight);
	}

	private void GetAnimatorStateInfo(int layerIndex, StateInfoIndex stateInfoIndex, out AnimatorStateInfo info)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetAnimatorStateInfo_Injected(intPtr, layerIndex, stateInfoIndex, out info);
	}

	public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
	{
		GetAnimatorStateInfo(layerIndex, StateInfoIndex.CurrentState, out var info);
		return info;
	}

	public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex)
	{
		GetAnimatorStateInfo(layerIndex, StateInfoIndex.NextState, out var info);
		return info;
	}

	private void GetAnimatorTransitionInfo(int layerIndex, out AnimatorTransitionInfo info)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetAnimatorTransitionInfo_Injected(intPtr, layerIndex, out info);
	}

	public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
	{
		GetAnimatorTransitionInfo(layerIndex, out var info);
		return info;
	}

	internal int GetAnimatorClipInfoCount(int layerIndex, bool current)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAnimatorClipInfoCount_Injected(intPtr, layerIndex, current);
	}

	public int GetCurrentAnimatorClipInfoCount(int layerIndex)
	{
		return GetAnimatorClipInfoCount(layerIndex, current: true);
	}

	public int GetNextAnimatorClipInfoCount(int layerIndex)
	{
		return GetAnimatorClipInfoCount(layerIndex, current: false);
	}

	[FreeFunction(Name = "AnimatorBindings::GetCurrentAnimatorClipInfo", HasExplicitThis = true)]
	[return: Unmarshalled]
	public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCurrentAnimatorClipInfo_Injected(intPtr, layerIndex);
	}

	[FreeFunction(Name = "AnimatorBindings::GetNextAnimatorClipInfo", HasExplicitThis = true)]
	[return: Unmarshalled]
	public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNextAnimatorClipInfo_Injected(intPtr, layerIndex);
	}

	public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
	{
		if (clips == null)
		{
			throw new ArgumentNullException("clips");
		}
		GetAnimatorClipInfoInternal(layerIndex, isCurrent: true, clips);
	}

	[FreeFunction(Name = "AnimatorBindings::GetAnimatorClipInfoInternal", HasExplicitThis = true)]
	private void GetAnimatorClipInfoInternal(int layerIndex, bool isCurrent, object clips)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetAnimatorClipInfoInternal_Injected(intPtr, layerIndex, isCurrent, clips);
	}

	public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
	{
		if (clips == null)
		{
			throw new ArgumentNullException("clips");
		}
		GetAnimatorClipInfoInternal(layerIndex, isCurrent: false, clips);
	}

	public bool IsInTransition(int layerIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsInTransition_Injected(intPtr, layerIndex);
	}

	[FreeFunction(Name = "AnimatorBindings::GetParameterInternal", HasExplicitThis = true)]
	private AnimatorControllerParameter GetParameterInternal(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetParameterInternal_Injected(intPtr, index);
	}

	public AnimatorControllerParameter GetParameter(int index)
	{
		AnimatorControllerParameter parameterInternal = GetParameterInternal(index);
		if (parameterInternal.m_Type == (AnimatorControllerParameterType)0)
		{
			throw new IndexOutOfRangeException("Index must be between 0 and " + parameterCount);
		}
		return parameterInternal;
	}

	private void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, int targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime, bool completeMatch)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MatchTarget_Injected(intPtr, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime, completeMatch);
	}

	public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
	{
		MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, 1f, completeMatch: true);
	}

	public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [UnityEngine.Internal.DefaultValue("1")] float targetNormalizedTime)
	{
		MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, targetNormalizedTime, completeMatch: true);
	}

	public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [UnityEngine.Internal.DefaultValue("1")] float targetNormalizedTime, [UnityEngine.Internal.DefaultValue("true")] bool completeMatch)
	{
		MatchTarget(matchPosition, matchRotation, (int)targetBodyPart, weightMask, startNormalizedTime, targetNormalizedTime, completeMatch);
	}

	public void InterruptMatchTarget()
	{
		InterruptMatchTarget(completeMatch: true);
	}

	public void InterruptMatchTarget([UnityEngine.Internal.DefaultValue("true")] bool completeMatch)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InterruptMatchTarget_Injected(intPtr, completeMatch);
	}

	[Obsolete("ForceStateNormalizedTime is deprecated. Please use Play or CrossFade instead.")]
	public void ForceStateNormalizedTime(float normalizedTime)
	{
		Play(0, 0, normalizedTime);
	}

	public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration)
	{
		float normalizedTransitionTime = 0f;
		float fixedTimeOffset = 0f;
		int layer = -1;
		CrossFadeInFixedTime(StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, int layer)
	{
		float normalizedTransitionTime = 0f;
		float fixedTimeOffset = 0f;
		CrossFadeInFixedTime(StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, int layer, float fixedTimeOffset)
	{
		float normalizedTransitionTime = 0f;
		CrossFadeInFixedTime(StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(string stateName, float fixedTransitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float fixedTimeOffset, [UnityEngine.Internal.DefaultValue("0.0f")] float normalizedTransitionTime)
	{
		CrossFadeInFixedTime(StringToHash(stateName), fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, int layer, float fixedTimeOffset)
	{
		float normalizedTransitionTime = 0f;
		CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, int layer)
	{
		float normalizedTransitionTime = 0f;
		float fixedTimeOffset = 0f;
		CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration)
	{
		float normalizedTransitionTime = 0f;
		float fixedTimeOffset = 0f;
		int layer = -1;
		CrossFadeInFixedTime(stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	[FreeFunction(Name = "AnimatorBindings::CrossFadeInFixedTime", HasExplicitThis = true)]
	public void CrossFadeInFixedTime(int stateHashName, float fixedTransitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float fixedTimeOffset, [UnityEngine.Internal.DefaultValue("0.0f")] float normalizedTransitionTime)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CrossFadeInFixedTime_Injected(intPtr, stateHashName, fixedTransitionDuration, layer, fixedTimeOffset, normalizedTransitionTime);
	}

	[FreeFunction(Name = "AnimatorBindings::WriteDefaultValues", HasExplicitThis = true)]
	public void WriteDefaultValues()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WriteDefaultValues_Injected(intPtr);
	}

	public void CrossFade(string stateName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
	{
		float normalizedTransitionTime = 0f;
		CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(string stateName, float normalizedTransitionDuration, int layer)
	{
		float normalizedTransitionTime = 0f;
		float normalizedTimeOffset = float.NegativeInfinity;
		CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(string stateName, float normalizedTransitionDuration)
	{
		float normalizedTransitionTime = 0f;
		float normalizedTimeOffset = float.NegativeInfinity;
		int layer = -1;
		CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(string stateName, float normalizedTransitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTimeOffset, [UnityEngine.Internal.DefaultValue("0.0f")] float normalizedTransitionTime)
	{
		CrossFade(StringToHash(stateName), normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	[FreeFunction(Name = "AnimatorBindings::CrossFade", HasExplicitThis = true)]
	public void CrossFade(int stateHashName, float normalizedTransitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float normalizedTimeOffset, [UnityEngine.Internal.DefaultValue("0.0f")] float normalizedTransitionTime)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CrossFade_Injected(intPtr, stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(int stateHashName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset)
	{
		float normalizedTransitionTime = 0f;
		CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(int stateHashName, float normalizedTransitionDuration, int layer)
	{
		float normalizedTransitionTime = 0f;
		float normalizedTimeOffset = float.NegativeInfinity;
		CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void CrossFade(int stateHashName, float normalizedTransitionDuration)
	{
		float normalizedTransitionTime = 0f;
		float normalizedTimeOffset = float.NegativeInfinity;
		int layer = -1;
		CrossFade(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
	}

	public void PlayInFixedTime(string stateName, int layer)
	{
		float fixedTime = float.NegativeInfinity;
		PlayInFixedTime(stateName, layer, fixedTime);
	}

	public void PlayInFixedTime(string stateName)
	{
		float fixedTime = float.NegativeInfinity;
		int layer = -1;
		PlayInFixedTime(stateName, layer, fixedTime);
	}

	public void PlayInFixedTime(string stateName, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float fixedTime)
	{
		PlayInFixedTime(StringToHash(stateName), layer, fixedTime);
	}

	[FreeFunction(Name = "AnimatorBindings::PlayInFixedTime", HasExplicitThis = true)]
	public void PlayInFixedTime(int stateNameHash, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float fixedTime)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		PlayInFixedTime_Injected(intPtr, stateNameHash, layer, fixedTime);
	}

	public void PlayInFixedTime(int stateNameHash, int layer)
	{
		float fixedTime = float.NegativeInfinity;
		PlayInFixedTime(stateNameHash, layer, fixedTime);
	}

	public void PlayInFixedTime(int stateNameHash)
	{
		float fixedTime = float.NegativeInfinity;
		int layer = -1;
		PlayInFixedTime(stateNameHash, layer, fixedTime);
	}

	public void Play(string stateName, int layer)
	{
		float normalizedTime = float.NegativeInfinity;
		Play(stateName, layer, normalizedTime);
	}

	public void Play(string stateName)
	{
		float normalizedTime = float.NegativeInfinity;
		int layer = -1;
		Play(stateName, layer, normalizedTime);
	}

	public void Play(string stateName, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime)
	{
		Play(StringToHash(stateName), layer, normalizedTime);
	}

	[FreeFunction(Name = "AnimatorBindings::Play", HasExplicitThis = true)]
	public void Play(int stateNameHash, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Play_Injected(intPtr, stateNameHash, layer, normalizedTime);
	}

	public void Play(int stateNameHash, int layer)
	{
		float normalizedTime = float.NegativeInfinity;
		Play(stateNameHash, layer, normalizedTime);
	}

	public void Play(int stateNameHash)
	{
		float normalizedTime = float.NegativeInfinity;
		int layer = -1;
		Play(stateNameHash, layer, normalizedTime);
	}

	public void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTarget_Injected(intPtr, targetIndex, targetNormalizedTime);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use mask and layers to control subset of transfroms in a skeleton.", true)]
	public bool IsControlled(Transform transform)
	{
		return false;
	}

	internal bool IsBoneTransform(Transform transform)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsBoneTransform_Injected(intPtr, MarshalledUnityObject.Marshal(transform));
	}

	public Transform GetBoneTransform(HumanBodyBones humanBoneId)
	{
		if (avatar == null)
		{
			throw new InvalidOperationException("Avatar is null.");
		}
		if (!avatar.isValid)
		{
			throw new InvalidOperationException("Avatar is not valid.");
		}
		if (!avatar.isHuman)
		{
			throw new InvalidOperationException("Avatar is not of type humanoid.");
		}
		if (humanBoneId < HumanBodyBones.Hips || humanBoneId >= HumanBodyBones.LastBone)
		{
			throw new IndexOutOfRangeException("humanBoneId must be between 0 and " + HumanBodyBones.LastBone);
		}
		return GetBoneTransformInternal(HumanTrait.GetBoneIndexFromMono((int)humanBoneId));
	}

	[NativeMethod("GetBoneTransform")]
	internal Transform GetBoneTransformInternal(int humanBoneId)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Transform>(GetBoneTransformInternal_Injected(intPtr, humanBoneId));
	}

	public void StartPlayback()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StartPlayback_Injected(intPtr);
	}

	public void StopPlayback()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StopPlayback_Injected(intPtr);
	}

	public void StartRecording(int frameCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StartRecording_Injected(intPtr, frameCount);
	}

	public void StopRecording()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		StopRecording_Injected(intPtr);
	}

	private float GetRecorderStartTime()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetRecorderStartTime_Injected(intPtr);
	}

	private float GetRecorderStopTime()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetRecorderStopTime_Injected(intPtr);
	}

	internal void ClearInternalControllerPlayable()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearInternalControllerPlayable_Injected(intPtr);
	}

	public bool HasState(int layerIndex, int stateID)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasState_Injected(intPtr, layerIndex, stateID);
	}

	[NativeMethod(Name = "ScriptingStringToCRC32", IsThreadSafe = true)]
	public unsafe static int StringToHash(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return StringToHash_Injected(ref managedSpanWrapper);
				}
			}
			return StringToHash_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	internal string GetStats()
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetStats_Injected(intPtr, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction(Name = "AnimatorBindings::GetCurrentGraph", HasExplicitThis = true)]
	private void GetCurrentGraph(ref PlayableGraph graph)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetCurrentGraph_Injected(intPtr, ref graph);
	}

	private void CheckIfInIKPass()
	{
		if (logWarnings && !IsInIKPass())
		{
			Debug.LogWarning("Setting and getting Body Position/Rotation, IK Goals, Lookat and BoneLocalRotation should only be done in OnAnimatorIK or OnStateIK");
		}
	}

	private bool IsInIKPass()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsInIKPass_Injected(intPtr);
	}

	[FreeFunction(Name = "AnimatorBindings::SetFloatString", HasExplicitThis = true)]
	private unsafe void SetFloatString(string name, float value)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetFloatString_Injected(intPtr, ref managedSpanWrapper, value);
					return;
				}
			}
			SetFloatString_Injected(intPtr, ref managedSpanWrapper, value);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::SetFloatID", HasExplicitThis = true)]
	private void SetFloatID(int id, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFloatID_Injected(intPtr, id, value);
	}

	[FreeFunction(Name = "AnimatorBindings::GetFloatString", HasExplicitThis = true)]
	private unsafe float GetFloatString(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetFloatString_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetFloatString_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::GetFloatID", HasExplicitThis = true)]
	private float GetFloatID(int id)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetFloatID_Injected(intPtr, id);
	}

	[FreeFunction(Name = "AnimatorBindings::SetBoolString", HasExplicitThis = true)]
	private unsafe void SetBoolString(string name, bool value)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetBoolString_Injected(intPtr, ref managedSpanWrapper, value);
					return;
				}
			}
			SetBoolString_Injected(intPtr, ref managedSpanWrapper, value);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::SetBoolID", HasExplicitThis = true)]
	private void SetBoolID(int id, bool value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBoolID_Injected(intPtr, id, value);
	}

	[FreeFunction(Name = "AnimatorBindings::GetBoolString", HasExplicitThis = true)]
	private unsafe bool GetBoolString(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetBoolString_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetBoolString_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::GetBoolID", HasExplicitThis = true)]
	private bool GetBoolID(int id)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetBoolID_Injected(intPtr, id);
	}

	[FreeFunction(Name = "AnimatorBindings::SetIntegerString", HasExplicitThis = true)]
	private unsafe void SetIntegerString(string name, int value)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetIntegerString_Injected(intPtr, ref managedSpanWrapper, value);
					return;
				}
			}
			SetIntegerString_Injected(intPtr, ref managedSpanWrapper, value);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::SetIntegerID", HasExplicitThis = true)]
	private void SetIntegerID(int id, int value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIntegerID_Injected(intPtr, id, value);
	}

	[FreeFunction(Name = "AnimatorBindings::GetIntegerString", HasExplicitThis = true)]
	private unsafe int GetIntegerString(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetIntegerString_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetIntegerString_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::GetIntegerID", HasExplicitThis = true)]
	private int GetIntegerID(int id)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetIntegerID_Injected(intPtr, id);
	}

	[FreeFunction(Name = "AnimatorBindings::SetTriggerString", HasExplicitThis = true)]
	private unsafe void SetTriggerString(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetTriggerString_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			SetTriggerString_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::SetTriggerID", HasExplicitThis = true)]
	private void SetTriggerID(int id)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTriggerID_Injected(intPtr, id);
	}

	[FreeFunction(Name = "AnimatorBindings::ResetTriggerString", HasExplicitThis = true)]
	private unsafe void ResetTriggerString(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					ResetTriggerString_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			ResetTriggerString_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::ResetTriggerID", HasExplicitThis = true)]
	private void ResetTriggerID(int id)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetTriggerID_Injected(intPtr, id);
	}

	[FreeFunction(Name = "AnimatorBindings::IsParameterControlledByCurveString", HasExplicitThis = true)]
	private unsafe bool IsParameterControlledByCurveString(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return IsParameterControlledByCurveString_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return IsParameterControlledByCurveString_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::IsParameterControlledByCurveID", HasExplicitThis = true)]
	private bool IsParameterControlledByCurveID(int id)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsParameterControlledByCurveID_Injected(intPtr, id);
	}

	[FreeFunction(Name = "AnimatorBindings::SetFloatStringDamp", HasExplicitThis = true)]
	private unsafe void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetFloatStringDamp_Injected(intPtr, ref managedSpanWrapper, value, dampTime, deltaTime);
					return;
				}
			}
			SetFloatStringDamp_Injected(intPtr, ref managedSpanWrapper, value, dampTime, deltaTime);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "AnimatorBindings::SetFloatIDDamp", HasExplicitThis = true)]
	private void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFloatIDDamp_Injected(intPtr, id, value, dampTime, deltaTime);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal void OnUpdateModeChanged()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		OnUpdateModeChanged_Injected(intPtr);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal void OnCullingModeChanged()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		OnCullingModeChanged_Injected(intPtr);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal void WriteDefaultPose()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WriteDefaultPose_Injected(intPtr);
	}

	[NativeMethod("UpdateWithDelta")]
	public void Update(float deltaTime)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Update_Injected(intPtr, deltaTime);
	}

	public void Rebind()
	{
		Rebind(writeDefaultValues: true);
	}

	private void Rebind(bool writeDefaultValues)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Rebind_Injected(intPtr, writeDefaultValues);
	}

	public void ApplyBuiltinRootMotion()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ApplyBuiltinRootMotion_Injected(intPtr);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal void EvaluateController()
	{
		EvaluateController(0f);
	}

	private void EvaluateController(float deltaTime)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EvaluateController_Injected(intPtr, deltaTime);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal string GetCurrentStateName(int layerIndex)
	{
		return GetAnimatorStateName(layerIndex, current: true);
	}

	[NativeConditional("UNITY_EDITOR")]
	internal string GetNextStateName(int layerIndex)
	{
		return GetAnimatorStateName(layerIndex, current: false);
	}

	[NativeConditional("UNITY_EDITOR")]
	private string GetAnimatorStateName(int layerIndex, bool current)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetAnimatorStateName_Injected(intPtr, layerIndex, current, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	internal string ResolveHash(int hash)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ResolveHash_Injected(intPtr, hash, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[Obsolete("GetVector is deprecated.")]
	public Vector3 GetVector(string name)
	{
		return Vector3.zero;
	}

	[Obsolete("GetVector is deprecated.")]
	public Vector3 GetVector(int id)
	{
		return Vector3.zero;
	}

	[Obsolete("SetVector is deprecated.")]
	public void SetVector(string name, Vector3 value)
	{
	}

	[Obsolete("SetVector is deprecated.")]
	public void SetVector(int id, Vector3 value)
	{
	}

	[Obsolete("GetQuaternion is deprecated.")]
	public Quaternion GetQuaternion(string name)
	{
		return Quaternion.identity;
	}

	[Obsolete("GetQuaternion is deprecated.")]
	public Quaternion GetQuaternion(int id)
	{
		return Quaternion.identity;
	}

	[Obsolete("SetQuaternion is deprecated.")]
	public void SetQuaternion(string name, Quaternion value)
	{
	}

	[Obsolete("SetQuaternion is deprecated.")]
	public void SetQuaternion(int id, Quaternion value)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isOptimizable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isHuman_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasRootMotion_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isRootPositionOrRotationControlledByCurves_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_humanScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isInitialized_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_deltaPosition_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_deltaRotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_velocity_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_angularVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rootPosition_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rootPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rootRotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rootRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_applyRootMotion_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_applyRootMotion_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_linearVelocityBlending_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearVelocityBlending_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_animatePhysics_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_animatePhysics_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimatorUpdateMode get_updateMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_updateMode_Injected(IntPtr _unity_self, AnimatorUpdateMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasTransformHierarchy_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowConstantClipSamplingOptimization_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowConstantClipSamplingOptimization_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_gravityWeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bodyPositionInternal_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bodyPositionInternal_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bodyRotationInternal_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bodyRotationInternal_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGoalPosition_Injected(IntPtr _unity_self, AvatarIKGoal goal, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGoalPosition_Injected(IntPtr _unity_self, AvatarIKGoal goal, [In] ref Vector3 goalPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGoalRotation_Injected(IntPtr _unity_self, AvatarIKGoal goal, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGoalRotation_Injected(IntPtr _unity_self, AvatarIKGoal goal, [In] ref Quaternion goalRotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetGoalWeightPosition_Injected(IntPtr _unity_self, AvatarIKGoal goal);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGoalWeightPosition_Injected(IntPtr _unity_self, AvatarIKGoal goal, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetGoalWeightRotation_Injected(IntPtr _unity_self, AvatarIKGoal goal);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGoalWeightRotation_Injected(IntPtr _unity_self, AvatarIKGoal goal, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetHintPosition_Injected(IntPtr _unity_self, AvatarIKHint hint, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetHintPosition_Injected(IntPtr _unity_self, AvatarIKHint hint, [In] ref Vector3 hintPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetHintWeightPosition_Injected(IntPtr _unity_self, AvatarIKHint hint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetHintWeightPosition_Injected(IntPtr _unity_self, AvatarIKHint hint, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLookAtPositionInternal_Injected(IntPtr _unity_self, [In] ref Vector3 lookAtPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLookAtWeightInternal_Injected(IntPtr _unity_self, float weight, float bodyWeight, float headWeight, float eyesWeight, float clampWeight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBoneLocalRotationInternal_Injected(IntPtr _unity_self, int humanBoneId, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetBehaviour_Injected(IntPtr _unity_self, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ScriptableObject[] InternalGetBehaviours_Injected(IntPtr _unity_self, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ScriptableObject[] InternalGetBehavioursByKey_Injected(IntPtr _unity_self, int fullPathHash, int layerIndex, Type type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_stabilizeFeet_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_stabilizeFeet_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_layerCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLayerName_Injected(IntPtr _unity_self, int layerIndex, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetLayerIndex_Injected(IntPtr _unity_self, ref ManagedSpanWrapper layerName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetLayerWeight_Injected(IntPtr _unity_self, int layerIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLayerWeight_Injected(IntPtr _unity_self, int layerIndex, float weight);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAnimatorStateInfo_Injected(IntPtr _unity_self, int layerIndex, StateInfoIndex stateInfoIndex, out AnimatorStateInfo info);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAnimatorTransitionInfo_Injected(IntPtr _unity_self, int layerIndex, out AnimatorTransitionInfo info);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetAnimatorClipInfoCount_Injected(IntPtr _unity_self, int layerIndex, bool current);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimatorClipInfo[] GetCurrentAnimatorClipInfo_Injected(IntPtr _unity_self, int layerIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimatorClipInfo[] GetNextAnimatorClipInfo_Injected(IntPtr _unity_self, int layerIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAnimatorClipInfoInternal_Injected(IntPtr _unity_self, int layerIndex, bool isCurrent, object clips);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsInTransition_Injected(IntPtr _unity_self, int layerIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimatorControllerParameter[] get_parameters_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_parameterCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimatorControllerParameter GetParameterInternal_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_feetPivotActive_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_feetPivotActive_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_pivotWeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_pivotPosition_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MatchTarget_Injected(IntPtr _unity_self, [In] ref Vector3 matchPosition, [In] ref Quaternion matchRotation, int targetBodyPart, [In] ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime, bool completeMatch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InterruptMatchTarget_Injected(IntPtr _unity_self, [UnityEngine.Internal.DefaultValue("true")] bool completeMatch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isMatchingTarget_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_speed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_speed_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CrossFadeInFixedTime_Injected(IntPtr _unity_self, int stateHashName, float fixedTransitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float fixedTimeOffset, [UnityEngine.Internal.DefaultValue("0.0f")] float normalizedTransitionTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WriteDefaultValues_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CrossFade_Injected(IntPtr _unity_self, int stateHashName, float normalizedTransitionDuration, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("0.0f")] float normalizedTimeOffset, [UnityEngine.Internal.DefaultValue("0.0f")] float normalizedTransitionTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PlayInFixedTime_Injected(IntPtr _unity_self, int stateNameHash, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float fixedTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Play_Injected(IntPtr _unity_self, int stateNameHash, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTarget_Injected(IntPtr _unity_self, AvatarTarget targetIndex, float targetNormalizedTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_targetPosition_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_targetRotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsBoneTransform_Injected(IntPtr _unity_self, IntPtr transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_avatarRoot_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetBoneTransformInternal_Injected(IntPtr _unity_self, int humanBoneId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimatorCullingMode get_cullingMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cullingMode_Injected(IntPtr _unity_self, AnimatorCullingMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StartPlayback_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopPlayback_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_playbackTime_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_playbackTime_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StartRecording_Injected(IntPtr _unity_self, int frameCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void StopRecording_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetRecorderStartTime_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetRecorderStopTime_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimatorRecorderMode get_recorderMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_runtimeAnimatorController_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_runtimeAnimatorController_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasBoundPlayables_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearInternalControllerPlayable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasState_Injected(IntPtr _unity_self, int layerIndex, int stateID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int StringToHash_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_avatar_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_avatar_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetStats_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCurrentGraph_Injected(IntPtr _unity_self, ref PlayableGraph graph);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsInIKPass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatID_Injected(IntPtr _unity_self, int id, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetFloatString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetFloatID_Injected(IntPtr _unity_self, int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBoolString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBoolID_Injected(IntPtr _unity_self, int id, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetBoolString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetBoolID_Injected(IntPtr _unity_self, int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIntegerString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIntegerID_Injected(IntPtr _unity_self, int id, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetIntegerString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetIntegerID_Injected(IntPtr _unity_self, int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTriggerString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTriggerID_Injected(IntPtr _unity_self, int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetTriggerString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetTriggerID_Injected(IntPtr _unity_self, int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsParameterControlledByCurveString_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsParameterControlledByCurveID_Injected(IntPtr _unity_self, int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatStringDamp_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, float value, float dampTime, float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatIDDamp_Injected(IntPtr _unity_self, int id, float value, float dampTime, float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_layersAffectMassCenter_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_layersAffectMassCenter_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_leftFeetBottomHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_rightFeetBottomHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_supportsOnAnimatorMove_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OnUpdateModeChanged_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OnCullingModeChanged_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WriteDefaultPose_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Update_Injected(IntPtr _unity_self, float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Rebind_Injected(IntPtr _unity_self, bool writeDefaultValues);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ApplyBuiltinRootMotion_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EvaluateController_Injected(IntPtr _unity_self, float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAnimatorStateName_Injected(IntPtr _unity_self, int layerIndex, bool current, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResolveHash_Injected(IntPtr _unity_self, int hash, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_logWarnings_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_logWarnings_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_fireEvents_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_fireEvents_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_keepAnimatorStateOnDisable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_keepAnimatorStateOnDisable_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_writeDefaultValuesOnDisable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_writeDefaultValuesOnDisable_Injected(IntPtr _unity_self, bool value);
}
