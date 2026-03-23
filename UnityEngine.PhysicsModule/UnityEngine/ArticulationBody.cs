using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeClass("Physics::ArticulationBody")]
[NativeHeader("Modules/Physics/ArticulationBody.h")]
public class ArticulationBody : Behaviour
{
	public ArticulationJointType jointType
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_jointType_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_jointType_Injected(intPtr, value);
		}
	}

	public Vector3 anchorPosition
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_anchorPosition_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_anchorPosition_Injected(intPtr, ref value);
		}
	}

	public Vector3 parentAnchorPosition
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_parentAnchorPosition_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_parentAnchorPosition_Injected(intPtr, ref value);
		}
	}

	public Quaternion anchorRotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_anchorRotation_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_anchorRotation_Injected(intPtr, ref value);
		}
	}

	public Quaternion parentAnchorRotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_parentAnchorRotation_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_parentAnchorRotation_Injected(intPtr, ref value);
		}
	}

	public bool isRoot
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isRoot_Injected(intPtr);
		}
	}

	public bool matchAnchors
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_matchAnchors_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_matchAnchors_Injected(intPtr, value);
		}
	}

	public ArticulationDofLock linearLockX
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_linearLockX_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_linearLockX_Injected(intPtr, value);
		}
	}

	public ArticulationDofLock linearLockY
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_linearLockY_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_linearLockY_Injected(intPtr, value);
		}
	}

	public ArticulationDofLock linearLockZ
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_linearLockZ_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_linearLockZ_Injected(intPtr, value);
		}
	}

	public ArticulationDofLock swingYLock
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_swingYLock_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_swingYLock_Injected(intPtr, value);
		}
	}

	public ArticulationDofLock swingZLock
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_swingZLock_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_swingZLock_Injected(intPtr, value);
		}
	}

	public ArticulationDofLock twistLock
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_twistLock_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_twistLock_Injected(intPtr, value);
		}
	}

	public ArticulationDrive xDrive
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_xDrive_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_xDrive_Injected(intPtr, ref value);
		}
	}

	public ArticulationDrive yDrive
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_yDrive_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_yDrive_Injected(intPtr, ref value);
		}
	}

	public ArticulationDrive zDrive
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_zDrive_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_zDrive_Injected(intPtr, ref value);
		}
	}

	public bool immovable
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_immovable_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_immovable_Injected(intPtr, value);
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

	public float jointFriction
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_jointFriction_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_jointFriction_Injected(intPtr, value);
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

	public float maxJointVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxJointVelocity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maxJointVelocity_Injected(intPtr, value);
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

	public ArticulationReducedSpace jointPosition
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_jointPosition_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_jointPosition_Injected(intPtr, ref value);
		}
	}

	public ArticulationReducedSpace jointVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_jointVelocity_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_jointVelocity_Injected(intPtr, ref value);
		}
	}

	public ArticulationReducedSpace jointAcceleration
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_jointAcceleration_Injected(intPtr, out var ret);
			return ret;
		}
		[Obsolete("Setting joint accelerations is not supported in forward kinematics. To have inverse dynamics take acceleration into account, use GetJointForcesForAcceleration instead", true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_jointAcceleration_Injected(intPtr, ref value);
		}
	}

	public ArticulationReducedSpace jointForce
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_jointForce_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_jointForce_Injected(intPtr, ref value);
		}
	}

	public ArticulationReducedSpace driveForce
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_driveForce_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public int dofCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_dofCount_Injected(intPtr);
		}
	}

	public int index
	{
		[NativeMethod("GetBodyIndex")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_index_Injected(intPtr);
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

	[Obsolete("Please use ArticulationBody.linearVelocity instead. (UnityUpgradable) -> linearVelocity")]
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

	[Obsolete("computeParentAnchor has been renamed to matchAnchors (UnityUpgradable) -> matchAnchors")]
	public bool computeParentAnchor
	{
		get
		{
			return matchAnchors;
		}
		set
		{
			matchAnchors = value;
		}
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

	public void TeleportRoot(Vector3 position, Quaternion rotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TeleportRoot_Injected(intPtr, ref position, ref rotation);
	}

	public Vector3 GetClosestPoint(Vector3 point)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetClosestPoint_Injected(intPtr, ref point, out var ret);
		return ret;
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

	[NativeMethod("GetDenseJacobian")]
	private int GetDenseJacobian_Internal(ref ArticulationJacobian jacobian)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDenseJacobian_Internal_Injected(intPtr, ref jacobian);
	}

	public int GetDenseJacobian(ref ArticulationJacobian jacobian)
	{
		if (jacobian.elements == null)
		{
			jacobian.elements = new List<float>();
		}
		return GetDenseJacobian_Internal(ref jacobian);
	}

	public unsafe int GetJointPositions(List<float> positions)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper positions2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = positions;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					positions2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetJointPositions_Injected(intPtr, ref positions2);
				}
			}
			return GetJointPositions_Injected(intPtr, ref positions2);
		}
		finally
		{
			positions2.Unmarshal(list);
		}
	}

	public unsafe void SetJointPositions(List<float> positions)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper positions2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = positions;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					positions2 = new BlittableListWrapper(arrayWrapper, list.Count);
					SetJointPositions_Injected(intPtr, ref positions2);
					return;
				}
			}
			SetJointPositions_Injected(intPtr, ref positions2);
		}
		finally
		{
			positions2.Unmarshal(list);
		}
	}

	public unsafe int GetJointVelocities(List<float> velocities)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper velocities2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = velocities;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					velocities2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetJointVelocities_Injected(intPtr, ref velocities2);
				}
			}
			return GetJointVelocities_Injected(intPtr, ref velocities2);
		}
		finally
		{
			velocities2.Unmarshal(list);
		}
	}

	public unsafe void SetJointVelocities(List<float> velocities)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper velocities2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = velocities;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					velocities2 = new BlittableListWrapper(arrayWrapper, list.Count);
					SetJointVelocities_Injected(intPtr, ref velocities2);
					return;
				}
			}
			SetJointVelocities_Injected(intPtr, ref velocities2);
		}
		finally
		{
			velocities2.Unmarshal(list);
		}
	}

	public unsafe int GetJointAccelerations(List<float> accelerations)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper accelerations2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = accelerations;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					accelerations2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetJointAccelerations_Injected(intPtr, ref accelerations2);
				}
			}
			return GetJointAccelerations_Injected(intPtr, ref accelerations2);
		}
		finally
		{
			accelerations2.Unmarshal(list);
		}
	}

	public unsafe int GetJointForces(List<float> forces)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper forces2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = forces;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					forces2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetJointForces_Injected(intPtr, ref forces2);
				}
			}
			return GetJointForces_Injected(intPtr, ref forces2);
		}
		finally
		{
			forces2.Unmarshal(list);
		}
	}

	public unsafe void SetJointForces(List<float> forces)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper forces2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = forces;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					forces2 = new BlittableListWrapper(arrayWrapper, list.Count);
					SetJointForces_Injected(intPtr, ref forces2);
					return;
				}
			}
			SetJointForces_Injected(intPtr, ref forces2);
		}
		finally
		{
			forces2.Unmarshal(list);
		}
	}

	public ArticulationReducedSpace GetJointForcesForAcceleration(ArticulationReducedSpace acceleration)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetJointForcesForAcceleration_Injected(intPtr, ref acceleration, out var ret);
		return ret;
	}

	public unsafe int GetDriveForces(List<float> forces)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper forces2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = forces;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					forces2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetDriveForces_Injected(intPtr, ref forces2);
				}
			}
			return GetDriveForces_Injected(intPtr, ref forces2);
		}
		finally
		{
			forces2.Unmarshal(list);
		}
	}

	public unsafe int GetJointGravityForces(List<float> forces)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper forces2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = forces;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					forces2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetJointGravityForces_Injected(intPtr, ref forces2);
				}
			}
			return GetJointGravityForces_Injected(intPtr, ref forces2);
		}
		finally
		{
			forces2.Unmarshal(list);
		}
	}

	public unsafe int GetJointCoriolisCentrifugalForces(List<float> forces)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper forces2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = forces;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					forces2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetJointCoriolisCentrifugalForces_Injected(intPtr, ref forces2);
				}
			}
			return GetJointCoriolisCentrifugalForces_Injected(intPtr, ref forces2);
		}
		finally
		{
			forces2.Unmarshal(list);
		}
	}

	public unsafe int GetJointExternalForces(List<float> forces, float step)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper forces2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = forces;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					forces2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetJointExternalForces_Injected(intPtr, ref forces2, step);
				}
			}
			return GetJointExternalForces_Injected(intPtr, ref forces2, step);
		}
		finally
		{
			forces2.Unmarshal(list);
		}
	}

	public unsafe int GetDriveTargets(List<float> targets)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper targets2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = targets;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					targets2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetDriveTargets_Injected(intPtr, ref targets2);
				}
			}
			return GetDriveTargets_Injected(intPtr, ref targets2);
		}
		finally
		{
			targets2.Unmarshal(list);
		}
	}

	public unsafe void SetDriveTargets(List<float> targets)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper targets2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = targets;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					targets2 = new BlittableListWrapper(arrayWrapper, list.Count);
					SetDriveTargets_Injected(intPtr, ref targets2);
					return;
				}
			}
			SetDriveTargets_Injected(intPtr, ref targets2);
		}
		finally
		{
			targets2.Unmarshal(list);
		}
	}

	public unsafe int GetDriveTargetVelocities(List<float> targetVelocities)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper targetVelocities2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = targetVelocities;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					targetVelocities2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetDriveTargetVelocities_Injected(intPtr, ref targetVelocities2);
				}
			}
			return GetDriveTargetVelocities_Injected(intPtr, ref targetVelocities2);
		}
		finally
		{
			targetVelocities2.Unmarshal(list);
		}
	}

	public unsafe void SetDriveTargetVelocities(List<float> targetVelocities)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper targetVelocities2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = targetVelocities;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					targetVelocities2 = new BlittableListWrapper(arrayWrapper, list.Count);
					SetDriveTargetVelocities_Injected(intPtr, ref targetVelocities2);
					return;
				}
			}
			SetDriveTargetVelocities_Injected(intPtr, ref targetVelocities2);
		}
		finally
		{
			targetVelocities2.Unmarshal(list);
		}
	}

	public unsafe int GetDofStartIndices(List<int> dofStartIndices)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<int> list = default(List<int>);
		BlittableListWrapper dofStartIndices2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = dofStartIndices;
			if (list != null)
			{
				fixed (int[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					dofStartIndices2 = new BlittableListWrapper(arrayWrapper, list.Count);
					return GetDofStartIndices_Injected(intPtr, ref dofStartIndices2);
				}
			}
			return GetDofStartIndices_Injected(intPtr, ref dofStartIndices2);
		}
		finally
		{
			dofStartIndices2.Unmarshal(list);
		}
	}

	public void SetDriveTarget(ArticulationDriveAxis axis, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDriveTarget_Injected(intPtr, axis, value);
	}

	public void SetDriveTargetVelocity(ArticulationDriveAxis axis, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDriveTargetVelocity_Injected(intPtr, axis, value);
	}

	public void SetDriveLimits(ArticulationDriveAxis axis, float lower, float upper)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDriveLimits_Injected(intPtr, axis, lower, upper);
	}

	public void SetDriveStiffness(ArticulationDriveAxis axis, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDriveStiffness_Injected(intPtr, axis, value);
	}

	public void SetDriveDamping(ArticulationDriveAxis axis, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDriveDamping_Injected(intPtr, axis, value);
	}

	public void SetDriveForceLimit(ArticulationDriveAxis axis, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDriveForceLimit_Injected(intPtr, axis, value);
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

	public void SnapAnchorToClosestContact()
	{
		if ((bool)base.transform.parent)
		{
			ArticulationBody componentInParent = base.transform.parent.GetComponentInParent<ArticulationBody>();
			while ((bool)componentInParent && !componentInParent.enabled)
			{
				componentInParent = componentInParent.transform.parent.GetComponentInParent<ArticulationBody>();
			}
			if ((bool)componentInParent)
			{
				Vector3 vector = componentInParent.worldCenterOfMass;
				Vector3 closestPoint = GetClosestPoint(vector);
				anchorPosition = base.transform.InverseTransformPoint(closestPoint);
				anchorRotation = Quaternion.FromToRotation(Vector3.right, base.transform.InverseTransformDirection(vector - closestPoint).normalized);
			}
		}
	}

	[Obsolete("Setting joint accelerations is not supported in forward kinematics. To have inverse dynamics take acceleration into account, use GetJointForcesForAcceleration instead", true)]
	public unsafe void SetJointAccelerations(List<float> accelerations)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<float> list = default(List<float>);
		BlittableListWrapper accelerations2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = accelerations;
			if (list != null)
			{
				fixed (float[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					accelerations2 = new BlittableListWrapper(arrayWrapper, list.Count);
					SetJointAccelerations_Injected(intPtr, ref accelerations2);
					return;
				}
			}
			SetJointAccelerations_Injected(intPtr, ref accelerations2);
		}
		finally
		{
			accelerations2.Unmarshal(list);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArticulationJointType get_jointType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_jointType_Injected(IntPtr _unity_self, ArticulationJointType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_anchorPosition_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_anchorPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_parentAnchorPosition_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_parentAnchorPosition_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_anchorRotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_anchorRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_parentAnchorRotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_parentAnchorRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isRoot_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_matchAnchors_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_matchAnchors_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArticulationDofLock get_linearLockX_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearLockX_Injected(IntPtr _unity_self, ArticulationDofLock value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArticulationDofLock get_linearLockY_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearLockY_Injected(IntPtr _unity_self, ArticulationDofLock value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArticulationDofLock get_linearLockZ_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearLockZ_Injected(IntPtr _unity_self, ArticulationDofLock value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArticulationDofLock get_swingYLock_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_swingYLock_Injected(IntPtr _unity_self, ArticulationDofLock value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArticulationDofLock get_swingZLock_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_swingZLock_Injected(IntPtr _unity_self, ArticulationDofLock value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ArticulationDofLock get_twistLock_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_twistLock_Injected(IntPtr _unity_self, ArticulationDofLock value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_xDrive_Injected(IntPtr _unity_self, out ArticulationDrive ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_xDrive_Injected(IntPtr _unity_self, [In] ref ArticulationDrive value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_yDrive_Injected(IntPtr _unity_self, out ArticulationDrive ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_yDrive_Injected(IntPtr _unity_self, [In] ref ArticulationDrive value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_zDrive_Injected(IntPtr _unity_self, out ArticulationDrive ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_zDrive_Injected(IntPtr _unity_self, [In] ref ArticulationDrive value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_immovable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_immovable_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useGravity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useGravity_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_linearDamping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearDamping_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_angularDamping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_angularDamping_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_jointFriction_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_jointFriction_Injected(IntPtr _unity_self, float value);

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
	private static extern void get_linearVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_angularVelocity_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_angularVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_mass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mass_Injected(IntPtr _unity_self, float value);

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
	private static extern void get_inertiaTensor_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_inertiaTensor_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_worldInertiaTensorMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_inertiaTensorRotation_Injected(IntPtr _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_inertiaTensorRotation_Injected(IntPtr _unity_self, [In] ref Quaternion value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetCenterOfMass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetInertiaTensor_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Sleep_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsSleeping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WakeUp_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_sleepThreshold_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sleepThreshold_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_solverIterations_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_solverIterations_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_solverVelocityIterations_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_solverVelocityIterations_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxAngularVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxAngularVelocity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxLinearVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxLinearVelocity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxJointVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxJointVelocity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxDepenetrationVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxDepenetrationVelocity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_jointPosition_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_jointPosition_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_jointVelocity_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_jointVelocity_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_jointAcceleration_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_jointAcceleration_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_jointForce_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_jointForce_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_driveForce_Injected(IntPtr _unity_self, out ArticulationReducedSpace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_dofCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_index_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TeleportRoot_Injected(IntPtr _unity_self, [In] ref Vector3 position, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetClosestPoint_Injected(IntPtr _unity_self, [In] ref Vector3 point, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRelativePointVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 relativePoint, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPointVelocity_Injected(IntPtr _unity_self, [In] ref Vector3 worldPoint, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDenseJacobian_Internal_Injected(IntPtr _unity_self, ref ArticulationJacobian jacobian);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetJointPositions_Injected(IntPtr _unity_self, ref BlittableListWrapper positions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetJointPositions_Injected(IntPtr _unity_self, ref BlittableListWrapper positions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetJointVelocities_Injected(IntPtr _unity_self, ref BlittableListWrapper velocities);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetJointVelocities_Injected(IntPtr _unity_self, ref BlittableListWrapper velocities);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetJointAccelerations_Injected(IntPtr _unity_self, ref BlittableListWrapper accelerations);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetJointForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetJointForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetJointForcesForAcceleration_Injected(IntPtr _unity_self, [In] ref ArticulationReducedSpace acceleration, out ArticulationReducedSpace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDriveForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetJointGravityForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetJointCoriolisCentrifugalForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetJointExternalForces_Injected(IntPtr _unity_self, ref BlittableListWrapper forces, float step);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDriveTargets_Injected(IntPtr _unity_self, ref BlittableListWrapper targets);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDriveTargets_Injected(IntPtr _unity_self, ref BlittableListWrapper targets);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDriveTargetVelocities_Injected(IntPtr _unity_self, ref BlittableListWrapper targetVelocities);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDriveTargetVelocities_Injected(IntPtr _unity_self, ref BlittableListWrapper targetVelocities);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDofStartIndices_Injected(IntPtr _unity_self, ref BlittableListWrapper dofStartIndices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDriveTarget_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDriveTargetVelocity_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDriveLimits_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float lower, float upper);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDriveStiffness_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDriveDamping_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDriveForceLimit_Injected(IntPtr _unity_self, ArticulationDriveAxis axis, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CollisionDetectionMode get_collisionDetectionMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_collisionDetectionMode_Injected(IntPtr _unity_self, CollisionDetectionMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PublishTransform_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetJointAccelerations_Injected(IntPtr _unity_self, ref BlittableListWrapper accelerations);
}
