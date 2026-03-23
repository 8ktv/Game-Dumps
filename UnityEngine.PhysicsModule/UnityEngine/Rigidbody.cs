using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Modules/Physics/Rigidbody.h")]
[RequireComponent(typeof(Transform))]
public class Rigidbody : Component
{
	public Vector3 linearVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_linearVelocity_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_linearVelocity_Injected(intPtr, ref value);
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
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_angularVelocity_Injected(intPtr, ref value);
		}
	}

	public float linearDamping
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_linearDamping_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_linearDamping_Injected(intPtr, value);
		}
	}

	public float angularDamping
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_angularDamping_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_angularDamping_Injected(intPtr, value);
		}
	}

	public float mass
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_mass_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_mass_Injected(intPtr, value);
		}
	}

	public bool useGravity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useGravity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useGravity_Injected(intPtr, value);
		}
	}

	public float maxDepenetrationVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxDepenetrationVelocity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maxDepenetrationVelocity_Injected(intPtr, value);
		}
	}

	public bool isKinematic
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isKinematic_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_isKinematic_Injected(intPtr, value);
		}
	}

	public bool freezeRotation
	{
		get
		{
			return constraints.HasFlag(RigidbodyConstraints.FreezeRotation);
		}
		set
		{
			if (value)
			{
				constraints |= RigidbodyConstraints.FreezeRotation;
			}
			else
			{
				constraints &= RigidbodyConstraints.FreezePosition;
			}
		}
	}

	public RigidbodyConstraints constraints
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_constraints_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_constraints_Injected(intPtr, value);
		}
	}

	public CollisionDetectionMode collisionDetectionMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_collisionDetectionMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_collisionDetectionMode_Injected(intPtr, value);
		}
	}

	public bool automaticCenterOfMass
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_automaticCenterOfMass_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_automaticCenterOfMass_Injected(intPtr, value);
		}
	}

	public Vector3 centerOfMass
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_centerOfMass_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_centerOfMass_Injected(intPtr, ref value);
		}
	}

	public Vector3 worldCenterOfMass
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_worldCenterOfMass_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public bool automaticInertiaTensor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_automaticInertiaTensor_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_automaticInertiaTensor_Injected(intPtr, value);
		}
	}

	public Quaternion inertiaTensorRotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_inertiaTensorRotation_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_inertiaTensorRotation_Injected(intPtr, ref value);
		}
	}

	public Vector3 inertiaTensor
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_inertiaTensor_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_inertiaTensor_Injected(intPtr, ref value);
		}
	}

	internal Matrix4x4 worldInertiaTensorMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_worldInertiaTensorMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public bool detectCollisions
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_detectCollisions_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_detectCollisions_Injected(intPtr, value);
		}
	}

	public Vector3 position
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_position_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_position_Injected(intPtr, ref value);
		}
	}

	public Quaternion rotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_rotation_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rotation_Injected(intPtr, ref value);
		}
	}

	public RigidbodyInterpolation interpolation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_interpolation_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_interpolation_Injected(intPtr, value);
		}
	}

	public int solverIterations
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_solverIterations_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_solverIterations_Injected(intPtr, value);
		}
	}

	public float sleepThreshold
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sleepThreshold_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sleepThreshold_Injected(intPtr, value);
		}
	}

	public float maxAngularVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxAngularVelocity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maxAngularVelocity_Injected(intPtr, value);
		}
	}

	public float maxLinearVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxLinearVelocity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maxLinearVelocity_Injected(intPtr, value);
		}
	}

	public int solverVelocityIterations
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_solverVelocityIterations_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_solverVelocityIterations_Injected(intPtr, value);
		}
	}

	public LayerMask excludeLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_excludeLayers_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_excludeLayers_Injected(intPtr, ref value);
		}
	}

	public LayerMask includeLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_includeLayers_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_includeLayers_Injected(intPtr, ref value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Please use Rigidbody.linearDamping instead. (UnityUpgradable) -> linearDamping")]
	public float drag
	{
		get
		{
			return linearDamping;
		}
		set
		{
			linearDamping = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Please use Rigidbody.angularDamping instead. (UnityUpgradable) -> angularDamping")]
	public float angularDrag
	{
		get
		{
			return angularDamping;
		}
		set
		{
			angularDamping = value;
		}
	}

	[Obsolete("Please use Rigidbody.linearVelocity instead. (UnityUpgradable) -> linearVelocity")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Vector3 velocity
	{
		get
		{
			return linearVelocity;
		}
		set
		{
			linearVelocity = value;
		}
	}

	public void MovePosition(Vector3 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MovePosition_Injected(intPtr, ref position);
	}

	public void MoveRotation(Quaternion rot)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MoveRotation_Injected(intPtr, ref rot);
	}

	public void Move(Vector3 position, Quaternion rotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Move_Injected(intPtr, ref position, ref rotation);
	}

	public void Sleep()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Sleep_Injected(intPtr);
	}

	public bool IsSleeping()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsSleeping_Injected(intPtr);
	}

	public void WakeUp()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WakeUp_Injected(intPtr);
	}

	public void ResetCenterOfMass()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetCenterOfMass_Injected(intPtr);
	}

	public void ResetInertiaTensor()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetInertiaTensor_Injected(intPtr);
	}

	public Vector3 GetRelativePointVelocity(Vector3 relativePoint)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetRelativePointVelocity_Injected(intPtr, ref relativePoint, out var ret);
		return ret;
	}

	public Vector3 GetPointVelocity(Vector3 worldPoint)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPointVelocity_Injected(intPtr, ref worldPoint, out var ret);
		return ret;
	}

	public void PublishTransform()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		PublishTransform_Injected(intPtr);
	}

	public Vector3 GetAccumulatedForce([UnityEngine.Internal.DefaultValue("Time.fixedDeltaTime")] float step)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetAccumulatedForce_Injected(intPtr, step, out var ret);
		return ret;
	}

	[ExcludeFromDocs]
	public Vector3 GetAccumulatedForce()
	{
		return GetAccumulatedForce(Time.fixedDeltaTime);
	}

	public Vector3 GetAccumulatedTorque([UnityEngine.Internal.DefaultValue("Time.fixedDeltaTime")] float step)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetAccumulatedTorque_Injected(intPtr, step, out var ret);
		return ret;
	}

	[ExcludeFromDocs]
	public Vector3 GetAccumulatedTorque()
	{
		return GetAccumulatedTorque(Time.fixedDeltaTime);
	}

	public void AddForce(Vector3 force, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddForce_Injected(intPtr, ref force, mode);
	}

	[ExcludeFromDocs]
	public void AddForce(Vector3 force)
	{
		AddForce(force, ForceMode.Force);
	}

	public void AddForce(float x, float y, float z, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		AddForce(new Vector3(x, y, z), mode);
	}

	[ExcludeFromDocs]
	public void AddForce(float x, float y, float z)
	{
		AddForce(new Vector3(x, y, z), ForceMode.Force);
	}

	public void AddRelativeForce(Vector3 force, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddRelativeForce_Injected(intPtr, ref force, mode);
	}

	[ExcludeFromDocs]
	public void AddRelativeForce(Vector3 force)
	{
		AddRelativeForce(force, ForceMode.Force);
	}

	public void AddRelativeForce(float x, float y, float z, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		AddRelativeForce(new Vector3(x, y, z), mode);
	}

	[ExcludeFromDocs]
	public void AddRelativeForce(float x, float y, float z)
	{
		AddRelativeForce(new Vector3(x, y, z), ForceMode.Force);
	}

	public void AddTorque(Vector3 torque, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddTorque_Injected(intPtr, ref torque, mode);
	}

	[ExcludeFromDocs]
	public void AddTorque(Vector3 torque)
	{
		AddTorque(torque, ForceMode.Force);
	}

	public void AddTorque(float x, float y, float z, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		AddTorque(new Vector3(x, y, z), mode);
	}

	[ExcludeFromDocs]
	public void AddTorque(float x, float y, float z)
	{
		AddTorque(new Vector3(x, y, z), ForceMode.Force);
	}

	public void AddRelativeTorque(Vector3 torque, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddRelativeTorque_Injected(intPtr, ref torque, mode);
	}

	[ExcludeFromDocs]
	public void AddRelativeTorque(Vector3 torque)
	{
		AddRelativeTorque(torque, ForceMode.Force);
	}

	public void AddRelativeTorque(float x, float y, float z, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		AddRelativeTorque(new Vector3(x, y, z), mode);
	}

	[ExcludeFromDocs]
	public void AddRelativeTorque(float x, float y, float z)
	{
		AddRelativeTorque(x, y, z, ForceMode.Force);
	}

	public void AddForceAtPosition(Vector3 force, Vector3 position, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddForceAtPosition_Injected(intPtr, ref force, ref position, mode);
	}

	[ExcludeFromDocs]
	public void AddForceAtPosition(Vector3 force, Vector3 position)
	{
		AddForceAtPosition(force, position, ForceMode.Force);
	}

	public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, [UnityEngine.Internal.DefaultValue("0.0f")] float upwardsModifier, [UnityEngine.Internal.DefaultValue("ForceMode.Force)")] ForceMode mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddExplosionForce_Injected(intPtr, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, mode);
	}

	[ExcludeFromDocs]
	public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
	{
		AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Force);
	}

	[ExcludeFromDocs]
	public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
	{
		AddExplosionForce(explosionForce, explosionPosition, explosionRadius, 0f, ForceMode.Force);
	}

	[NativeName("ClosestPointOnBounds")]
	private void Internal_ClosestPointOnBounds(Vector3 point, ref Vector3 outPos, ref float distance)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_ClosestPointOnBounds_Injected(intPtr, ref point, ref outPos, ref distance);
	}

	public Vector3 ClosestPointOnBounds(Vector3 position)
	{
		float distance = 0f;
		Vector3 outPos = Vector3.zero;
		Internal_ClosestPointOnBounds(position, ref outPos, ref distance);
		return outPos;
	}

	private RaycastHit SweepTest(Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction, ref bool hasHit)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SweepTest_Injected(intPtr, ref direction, maxDistance, queryTriggerInteraction, ref hasHit, out var ret);
		return ret;
	}

	public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		float magnitude = direction.magnitude;
		if (magnitude > float.Epsilon)
		{
			Vector3 direction2 = direction / magnitude;
			bool hasHit = false;
			hitInfo = SweepTest(direction2, maxDistance, queryTriggerInteraction, ref hasHit);
			return hasHit;
		}
		hitInfo = default(RaycastHit);
		return false;
	}

	[ExcludeFromDocs]
	public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, float maxDistance)
	{
		return SweepTest(direction, out hitInfo, maxDistance, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public bool SweepTest(Vector3 direction, out RaycastHit hitInfo)
	{
		return SweepTest(direction, out hitInfo, float.PositiveInfinity, QueryTriggerInteraction.UseGlobal);
	}

	[NativeName("SweepTestAll")]
	private RaycastHit[] Internal_SweepTestAll(Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Internal_SweepTestAll_Injected(intPtr, ref direction, maxDistance, queryTriggerInteraction, out ret);
		}
		finally
		{
			RaycastHit[] array = default(RaycastHit[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public RaycastHit[] SweepTestAll(Vector3 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		float magnitude = direction.magnitude;
		if (magnitude > float.Epsilon)
		{
			Vector3 direction2 = direction / magnitude;
			return Internal_SweepTestAll(direction2, maxDistance, queryTriggerInteraction);
		}
		return new RaycastHit[0];
	}

	[ExcludeFromDocs]
	public RaycastHit[] SweepTestAll(Vector3 direction, float maxDistance)
	{
		return SweepTestAll(direction, maxDistance, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public RaycastHit[] SweepTestAll(Vector3 direction)
	{
		return SweepTestAll(direction, float.PositiveInfinity, QueryTriggerInteraction.UseGlobal);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Please use Rigidbody.mass instead. Setting density on a Rigidbody no longer has any effect.", false)]
	public void SetDensity(float density)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_linearVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_angularVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_angularVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_linearDamping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearDamping_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_angularDamping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_angularDamping_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_mass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mass_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useGravity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useGravity_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxDepenetrationVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxDepenetrationVelocity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isKinematic_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_isKinematic_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RigidbodyConstraints get_constraints_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_constraints_Injected(IntPtr _unity_self, RigidbodyConstraints value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CollisionDetectionMode get_collisionDetectionMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_collisionDetectionMode_Injected(IntPtr _unity_self, CollisionDetectionMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_automaticCenterOfMass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_automaticCenterOfMass_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_centerOfMass_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_centerOfMass_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_worldCenterOfMass_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_automaticInertiaTensor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_automaticInertiaTensor_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_inertiaTensorRotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_inertiaTensorRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_inertiaTensor_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_inertiaTensor_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_worldInertiaTensorMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_detectCollisions_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_detectCollisions_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_position_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_position_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RigidbodyInterpolation get_interpolation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_interpolation_Injected(IntPtr _unity_self, RigidbodyInterpolation value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_solverIterations_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_solverIterations_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_sleepThreshold_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sleepThreshold_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxAngularVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxAngularVelocity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxLinearVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxLinearVelocity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MovePosition_Injected(IntPtr _unity_self, [In] ref Vector3 position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveRotation_Injected(IntPtr _unity_self, [In] ref Quaternion rot);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Move_Injected(IntPtr _unity_self, [In] ref Vector3 position, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Sleep_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsSleeping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WakeUp_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetCenterOfMass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetInertiaTensor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRelativePointVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 relativePoint, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPointVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 worldPoint, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_solverVelocityIterations_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_solverVelocityIterations_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PublishTransform_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_excludeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_excludeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_includeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_includeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAccumulatedForce_Injected(IntPtr _unity_self, [UnityEngine.Internal.DefaultValue("Time.fixedDeltaTime")] float step, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAccumulatedTorque_Injected(IntPtr _unity_self, [UnityEngine.Internal.DefaultValue("Time.fixedDeltaTime")] float step, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddForce_Injected(IntPtr _unity_self, [In] ref Vector3 force, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddRelativeForce_Injected(IntPtr _unity_self, [In] ref Vector3 force, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddTorque_Injected(IntPtr _unity_self, [In] ref Vector3 torque, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddRelativeTorque_Injected(IntPtr _unity_self, [In] ref Vector3 torque, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddForceAtPosition_Injected(IntPtr _unity_self, [In] ref Vector3 force, [In] ref Vector3 position, [UnityEngine.Internal.DefaultValue("ForceMode.Force")] ForceMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddExplosionForce_Injected(IntPtr _unity_self, float explosionForce, [In] ref Vector3 explosionPosition, float explosionRadius, [UnityEngine.Internal.DefaultValue("0.0f")] float upwardsModifier, [UnityEngine.Internal.DefaultValue("ForceMode.Force)")] ForceMode mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_ClosestPointOnBounds_Injected(IntPtr _unity_self, [In] ref Vector3 point, ref Vector3 outPos, ref float distance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SweepTest_Injected(IntPtr _unity_self, [In] ref Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction, ref bool hasHit, out RaycastHit ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SweepTestAll_Injected(IntPtr _unity_self, [In] ref Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);
}
