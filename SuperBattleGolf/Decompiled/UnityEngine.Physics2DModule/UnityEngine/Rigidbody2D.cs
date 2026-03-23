using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Modules/Physics2D/Public/Rigidbody2D.h")]
[RequireComponent(typeof(Transform))]
public sealed class Rigidbody2D : Component
{
	[Serializable]
	[NativeHeader(Header = "Modules/Physics2D/Public/Rigidbody2D.h")]
	public struct SlideMovement
	{
		[field: SerializeField]
		public int maxIterations { get; set; }

		[field: SerializeField]
		public float surfaceSlideAngle { get; set; }

		[field: SerializeField]
		public float gravitySlipAngle { get; set; }

		[field: SerializeField]
		public Vector2 surfaceUp { get; set; }

		[field: SerializeField]
		public Vector2 surfaceAnchor { get; set; }

		[field: SerializeField]
		public Vector2 gravity { get; set; }

		[field: SerializeField]
		public Vector2 startPosition { get; set; }

		[field: SerializeField]
		public Collider2D selectedCollider { get; set; }

		[field: SerializeField]
		public LayerMask layerMask { get; set; }

		[field: SerializeField]
		public bool useLayerMask { get; set; }

		[field: SerializeField]
		public bool useStartPosition { get; set; }

		[field: SerializeField]
		public bool useNoMove { get; set; }

		[field: SerializeField]
		public bool useSimulationMove { get; set; }

		[field: SerializeField]
		public bool useAttachedTriggers { get; set; }

		public SlideMovement()
		{
			maxIterations = 3;
			surfaceSlideAngle = 90f;
			gravitySlipAngle = 90f;
			surfaceUp = Vector2.up;
			surfaceAnchor = Vector2.down;
			gravity = new Vector2(0f, -9.81f);
			startPosition = Vector2.zero;
			selectedCollider = null;
			useStartPosition = false;
			useNoMove = false;
			useSimulationMove = false;
			useAttachedTriggers = false;
			useLayerMask = false;
			layerMask = -1;
		}

		public void SetLayerMask(LayerMask mask)
		{
			layerMask = mask;
			useLayerMask = true;
		}

		public void SetStartPosition(Vector2 position)
		{
			startPosition = position;
			useStartPosition = true;
		}
	}

	[Serializable]
	[NativeHeader(Header = "Modules/Physics2D/Public/Rigidbody2D.h")]
	public struct SlideResults
	{
		[field: SerializeField]
		public Vector2 remainingVelocity { get; set; }

		[field: SerializeField]
		public Vector2 position { get; set; }

		[field: SerializeField]
		public int iterationsUsed { get; set; }

		[field: SerializeField]
		public RaycastHit2D slideHit { get; set; }

		[field: SerializeField]
		public RaycastHit2D surfaceHit { get; set; }
	}

	public Vector2 position
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

	public float rotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rotation_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rotation_Injected(intPtr, value);
		}
	}

	public Vector2 linearVelocity
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

	public float linearVelocityX
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_linearVelocityX_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_linearVelocityX_Injected(intPtr, value);
		}
	}

	public float linearVelocityY
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_linearVelocityY_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_linearVelocityY_Injected(intPtr, value);
		}
	}

	public float angularVelocity
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_angularVelocity_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_angularVelocity_Injected(intPtr, value);
		}
	}

	public bool useAutoMass
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useAutoMass_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useAutoMass_Injected(intPtr, value);
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

	[NativeMethod("Material")]
	public PhysicsMaterial2D sharedMaterial
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<PhysicsMaterial2D>(get_sharedMaterial_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sharedMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Vector2 centerOfMass
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

	public Vector2 worldCenterOfMass
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

	public float inertia
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_inertia_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_inertia_Injected(intPtr, value);
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

	public float gravityScale
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_gravityScale_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_gravityScale_Injected(intPtr, value);
		}
	}

	public RigidbodyType2D bodyType
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bodyType_Injected(intPtr);
		}
		[NativeMethod("SetBodyType_Binding")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bodyType_Injected(intPtr, value);
		}
	}

	public bool useFullKinematicContacts
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useFullKinematicContacts_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useFullKinematicContacts_Injected(intPtr, value);
		}
	}

	public bool freezeRotation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_freezeRotation_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_freezeRotation_Injected(intPtr, value);
		}
	}

	public RigidbodyConstraints2D constraints
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

	public bool simulated
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_simulated_Injected(intPtr);
		}
		[NativeMethod("SetSimulated_Binding")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_simulated_Injected(intPtr, value);
		}
	}

	public RigidbodyInterpolation2D interpolation
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

	public RigidbodySleepMode2D sleepMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sleepMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sleepMode_Injected(intPtr, value);
		}
	}

	public CollisionDetectionMode2D collisionDetectionMode
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

	public int attachedColliderCount => GetAttachedColliderCount_Internal(findTriggers: true);

	public Vector2 totalForce
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_totalForce_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_totalForce_Injected(intPtr, ref value);
		}
	}

	public float totalTorque
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_totalTorque_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_totalTorque_Injected(intPtr, value);
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

	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localToWorldMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Rigidbody2D.fixedAngle is obsolete. Use Rigidbody2D.constraints instead.", true)]
	public bool fixedAngle
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("isKinematic has been deprecated. Please use bodyType.", false)]
	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool isKinematic
	{
		get
		{
			return bodyType == RigidbodyType2D.Kinematic;
		}
		set
		{
			bodyType = (value ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("drag has been deprecated. Please use linearDamping. (UnityUpgradable) -> linearDamping", false)]
	[ExcludeFromDocs]
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
	[ExcludeFromDocs]
	[Obsolete("angularDrag has been deprecated. Please use angularDamping. (UnityUpgradable) -> angularDamping", false)]
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

	[Obsolete("velocity has been deprecated. Please use linearVelocity. (UnityUpgradable) -> linearVelocity", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public Vector2 velocity
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

	[ExcludeFromDocs]
	[Obsolete("velocityX has been deprecated. Please use linearVelocityX. (UnityUpgradable) -> linearVelocityX", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public float velocityX
	{
		get
		{
			return linearVelocityX;
		}
		set
		{
			linearVelocityX = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("velocityY has been deprecated. Please use linearVelocityY (UnityUpgradable) -> linearVelocityY", false)]
	[ExcludeFromDocs]
	public float velocityY
	{
		get
		{
			return linearVelocityY;
		}
		set
		{
			linearVelocityY = value;
		}
	}

	public void SetRotation(float angle)
	{
		SetRotation_Angle(angle);
	}

	[NativeMethod("SetRotation")]
	private void SetRotation_Angle(float angle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetRotation_Angle_Injected(intPtr, angle);
	}

	public void SetRotation(Quaternion rotation)
	{
		SetRotation_Quaternion(rotation);
	}

	[NativeMethod("SetRotation")]
	private void SetRotation_Quaternion(Quaternion rotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetRotation_Quaternion_Injected(intPtr, ref rotation);
	}

	public void MovePosition(Vector2 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MovePosition_Injected(intPtr, ref position);
	}

	public void MoveRotation(float angle)
	{
		MoveRotation_Angle(angle);
	}

	[NativeMethod("MoveRotation")]
	private void MoveRotation_Angle(float angle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MoveRotation_Angle_Injected(intPtr, angle);
	}

	public void MoveRotation(Quaternion rotation)
	{
		MoveRotation_Quaternion(rotation);
	}

	[NativeMethod("MoveRotation")]
	private void MoveRotation_Quaternion(Quaternion rotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MoveRotation_Quaternion_Injected(intPtr, ref rotation);
	}

	[NativeMethod("MovePositionAndRotation")]
	public void MovePositionAndRotation(Vector2 position, float angle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MovePositionAndRotation_Injected(intPtr, ref position, angle);
	}

	public void MovePositionAndRotation(Vector2 position, Quaternion rotation)
	{
		MovePositionAndRotation_Quaternion(position, rotation);
	}

	[NativeMethod("MovePositionAndRotation")]
	private void MovePositionAndRotation_Quaternion(Vector2 position, Quaternion rotation)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MovePositionAndRotation_Quaternion_Injected(intPtr, ref position, ref rotation);
	}

	public SlideResults Slide(Vector2 velocity, float deltaTime, SlideMovement slideMovement)
	{
		if (deltaTime < 0f)
		{
			throw new ArgumentException($"Time cannot be negative. It is {deltaTime}.", "deltaTime");
		}
		if (Mathf.Approximately(deltaTime, 0f))
		{
			return new SlideResults
			{
				position = (slideMovement.useStartPosition ? slideMovement.startPosition : position),
				remainingVelocity = velocity
			};
		}
		if (slideMovement.useSimulationMove && bodyType == RigidbodyType2D.Static)
		{
			throw new ArgumentException($"Cannot use simulation move when the body type is Static. It is {slideMovement.useSimulationMove}.", "SlideMovement.useSimulationMove");
		}
		if (slideMovement.useNoMove && slideMovement.useSimulationMove)
		{
			throw new ArgumentException($"Cannot use no move and simulation move at the same time; the two are conflicting options. It is {slideMovement.useNoMove}.", "SlideMovement.useNoMove");
		}
		if (slideMovement.maxIterations < 1)
		{
			throw new ArgumentException($"Maximum Iterations must be greater than zero. It is {slideMovement.maxIterations}.", "SlideMovement.maxIterations");
		}
		if (!float.IsFinite(slideMovement.surfaceSlideAngle) || slideMovement.surfaceSlideAngle < 0f || slideMovement.surfaceSlideAngle > 90f)
		{
			throw new ArgumentException($"Surface Slide Angle must be in the range of 0 to 90 degrees. It is {slideMovement.surfaceSlideAngle}.", "SlideMovement.surfaceSlideAngle");
		}
		if (!float.IsFinite(slideMovement.gravitySlipAngle) || slideMovement.gravitySlipAngle < 0f || slideMovement.gravitySlipAngle > 90f)
		{
			throw new ArgumentException($"Gravity Slip Angle must be in the range of 0 to 90 degrees. It is {slideMovement.gravitySlipAngle}.", "SlideMovement.gravitySlipAngle");
		}
		if (!float.IsFinite(slideMovement.surfaceUp.x) || !float.IsFinite(slideMovement.surfaceUp.y))
		{
			throw new ArgumentException($"Surface Up is invalid. It is {slideMovement.surfaceUp}.", "SlideMovement.surfaceUp");
		}
		if (!float.IsFinite(slideMovement.surfaceAnchor.x) || !float.IsFinite(slideMovement.surfaceAnchor.y))
		{
			throw new ArgumentException($"Surface Anchor is invalid. It is {slideMovement.surfaceAnchor}.", "SlideMovement.surfaceAnchor");
		}
		if (!float.IsFinite(slideMovement.gravity.x) || !float.IsFinite(slideMovement.gravity.y))
		{
			throw new ArgumentException($"Gravity is invalid. It is {slideMovement.gravity}.", "SlideMovement.gravity");
		}
		if (!float.IsFinite(slideMovement.startPosition.x) || !float.IsFinite(slideMovement.startPosition.y))
		{
			throw new ArgumentException($"Start Position is invalid. It is {slideMovement.gravity}.", "SlideMovement.startPosition");
		}
		if ((bool)slideMovement.selectedCollider && slideMovement.selectedCollider.attachedRigidbody != this)
		{
			throw new ArgumentException($"Selected Collider must be attached to the Slide Rigidbody2D. It is {slideMovement.selectedCollider}.", "SlideMovement.selectedCollider");
		}
		return Slide_Internal(velocity, deltaTime, slideMovement);
	}

	[NativeMethod("Slide")]
	private SlideResults Slide_Internal(Vector2 velocity, float deltaTime, SlideMovement slideMovement)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Slide_Internal_Injected(intPtr, ref velocity, deltaTime, ref slideMovement, out var ret);
		return ret;
	}

	internal void SetDragBehaviour(bool dragged)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDragBehaviour_Injected(intPtr, dragged);
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

	public bool IsAwake()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsAwake_Injected(intPtr);
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

	[NativeMethod("Wake")]
	public void WakeUp()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WakeUp_Injected(intPtr);
	}

	[NativeMethod("GetAttachedColliderCount")]
	private int GetAttachedColliderCount_Internal(bool findTriggers)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAttachedColliderCount_Internal_Injected(intPtr, findTriggers);
	}

	public bool IsTouching([NotNull] Collider2D collider)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return IsTouching_Injected(intPtr, intPtr2);
	}

	public bool IsTouching(Collider2D collider, ContactFilter2D contactFilter)
	{
		return IsTouching_OtherColliderWithFilter_Internal(collider, contactFilter);
	}

	[NativeMethod("IsTouching")]
	private bool IsTouching_OtherColliderWithFilter_Internal([NotNull] Collider2D collider, ContactFilter2D contactFilter)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return IsTouching_OtherColliderWithFilter_Internal_Injected(intPtr, intPtr2, ref contactFilter);
	}

	public bool IsTouching(ContactFilter2D contactFilter)
	{
		return IsTouching_AnyColliderWithFilter_Internal(contactFilter);
	}

	[NativeMethod("IsTouching")]
	private bool IsTouching_AnyColliderWithFilter_Internal(ContactFilter2D contactFilter)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsTouching_AnyColliderWithFilter_Internal_Injected(intPtr, ref contactFilter);
	}

	[ExcludeFromDocs]
	public bool IsTouchingLayers()
	{
		return IsTouchingLayers(-1);
	}

	public bool IsTouchingLayers([UnityEngine.Internal.DefaultValue("Physics2D.AllLayers")] int layerMask = -1)
	{
		ContactFilter2D contactFilter = default(ContactFilter2D);
		contactFilter.SetLayerMask(layerMask);
		contactFilter.useTriggers = Physics2D.queriesHitTriggers;
		return IsTouching(contactFilter);
	}

	public bool OverlapPoint(Vector2 point)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return OverlapPoint_Injected(intPtr, ref point);
	}

	public ColliderDistance2D Distance(Collider2D collider)
	{
		if (collider == null)
		{
			throw new ArgumentNullException("Collider cannot be null.");
		}
		if (collider.attachedRigidbody == this)
		{
			throw new ArgumentException("The collider cannot be attached to the Rigidbody2D being searched.");
		}
		return Distance_Internal(collider);
	}

	public ColliderDistance2D Distance(Vector2 thisPosition, float thisAngle, Collider2D collider, Vector2 position, float angle)
	{
		if (!collider.attachedRigidbody)
		{
			throw new InvalidOperationException("Cannot perform a Collider Distance at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
		}
		return DistanceFrom_Internal(thisPosition, thisAngle, collider, position, angle);
	}

	[NativeMethod("Distance")]
	private ColliderDistance2D Distance_Internal([NotNull] Collider2D collider)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		Distance_Internal_Injected(intPtr, intPtr2, out var ret);
		return ret;
	}

	[NativeMethod("DistanceFrom")]
	private ColliderDistance2D DistanceFrom_Internal(Vector2 thisPosition, float thisAngle, [NotNull] Collider2D collider, Vector2 position, float angle)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		DistanceFrom_Internal_Injected(intPtr, ref thisPosition, thisAngle, intPtr2, ref position, angle, out var ret);
		return ret;
	}

	public Vector2 ClosestPoint(Vector2 position)
	{
		return Physics2D.ClosestPoint(position, this);
	}

	[ExcludeFromDocs]
	public void AddForce(Vector2 force)
	{
		AddForce_Internal(force, ForceMode2D.Force);
	}

	public void AddForce(Vector2 force, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
	{
		AddForce_Internal(force, mode);
	}

	public void AddForceX(float force, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
	{
		AddForce_Internal(new Vector2(force, 0f), mode);
	}

	public void AddForceY(float force, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
	{
		AddForce_Internal(new Vector2(0f, force), mode);
	}

	[NativeMethod("AddForce")]
	private void AddForce_Internal(Vector2 force, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddForce_Internal_Injected(intPtr, ref force, mode);
	}

	[ExcludeFromDocs]
	public void AddRelativeForce(Vector2 relativeForce)
	{
		AddRelativeForce_Internal(relativeForce, ForceMode2D.Force);
	}

	public void AddRelativeForce(Vector2 relativeForce, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
	{
		AddRelativeForce_Internal(relativeForce, mode);
	}

	public void AddRelativeForceX(float force, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
	{
		AddRelativeForce_Internal(new Vector2(force, 0f), mode);
	}

	public void AddRelativeForceY(float force, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode = ForceMode2D.Force)
	{
		AddRelativeForce_Internal(new Vector2(0f, force), mode);
	}

	[NativeMethod("AddRelativeForce")]
	private void AddRelativeForce_Internal(Vector2 relativeForce, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddRelativeForce_Internal_Injected(intPtr, ref relativeForce, mode);
	}

	[ExcludeFromDocs]
	public void AddForceAtPosition(Vector2 force, Vector2 position)
	{
		AddForceAtPosition(force, position, ForceMode2D.Force);
	}

	public void AddForceAtPosition(Vector2 force, Vector2 position, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddForceAtPosition_Injected(intPtr, ref force, ref position, mode);
	}

	[ExcludeFromDocs]
	public void AddTorque(float torque)
	{
		AddTorque(torque, ForceMode2D.Force);
	}

	public void AddTorque(float torque, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddTorque_Injected(intPtr, torque, mode);
	}

	public Vector2 GetPoint(Vector2 point)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPoint_Injected(intPtr, ref point, out var ret);
		return ret;
	}

	public Vector2 GetRelativePoint(Vector2 relativePoint)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetRelativePoint_Injected(intPtr, ref relativePoint, out var ret);
		return ret;
	}

	public Vector2 GetVector(Vector2 vector)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetVector_Injected(intPtr, ref vector, out var ret);
		return ret;
	}

	public Vector2 GetRelativeVector(Vector2 relativeVector)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetRelativeVector_Injected(intPtr, ref relativeVector, out var ret);
		return ret;
	}

	public Vector2 GetPointVelocity(Vector2 point)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPointVelocity_Injected(intPtr, ref point, out var ret);
		return ret;
	}

	public Vector2 GetRelativePointVelocity(Vector2 relativePoint)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetRelativePointVelocity_Injected(intPtr, ref relativePoint, out var ret);
		return ret;
	}

	public int GetContacts(ContactPoint2D[] contacts)
	{
		return Physics2D.GetContacts(this, ContactFilter2D.noFilter, contacts);
	}

	public int GetContacts(List<ContactPoint2D> contacts)
	{
		return Physics2D.GetContacts(this, ContactFilter2D.noFilter, contacts);
	}

	public int GetContacts(ContactFilter2D contactFilter, ContactPoint2D[] contacts)
	{
		return Physics2D.GetContacts(this, contactFilter, contacts);
	}

	public int GetContacts(ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
	{
		return Physics2D.GetContacts(this, contactFilter, contacts);
	}

	public int GetContacts(Collider2D[] colliders)
	{
		return Physics2D.GetContacts(this, ContactFilter2D.noFilter, colliders);
	}

	public int GetContacts(List<Collider2D> colliders)
	{
		return Physics2D.GetContacts(this, ContactFilter2D.noFilter, colliders);
	}

	public int GetContacts(ContactFilter2D contactFilter, Collider2D[] colliders)
	{
		return Physics2D.GetContacts(this, contactFilter, colliders);
	}

	public int GetContacts(ContactFilter2D contactFilter, List<Collider2D> colliders)
	{
		return Physics2D.GetContacts(this, contactFilter, colliders);
	}

	[ExcludeFromDocs]
	public int GetAttachedColliders([Out] Collider2D[] results)
	{
		return GetAttachedCollidersArray_Internal(results, findTriggers: true);
	}

	[ExcludeFromDocs]
	public int GetAttachedColliders(List<Collider2D> results)
	{
		return GetAttachedCollidersList_Internal(results, findTriggers: true);
	}

	public int GetAttachedColliders([Out] Collider2D[] results, [UnityEngine.Internal.DefaultValue("true")] bool findTriggers = true)
	{
		return GetAttachedCollidersArray_Internal(results, findTriggers);
	}

	public int GetAttachedColliders(List<Collider2D> results, [UnityEngine.Internal.DefaultValue("true")] bool findTriggers = true)
	{
		return GetAttachedCollidersList_Internal(results, findTriggers);
	}

	public int GetShapes(PhysicsShapeGroup2D physicsShapeGroup)
	{
		return GetShapes_Internal(ref physicsShapeGroup.m_GroupState);
	}

	[ExcludeFromDocs]
	public int Cast(Vector2 direction, RaycastHit2D[] results)
	{
		return CastArray_Internal(direction, float.PositiveInfinity, results);
	}

	public int Cast(Vector2 direction, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance)
	{
		return CastArray_Internal(direction, distance, results);
	}

	public int Cast(Vector2 direction, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return CastList_Internal(direction, distance, results);
	}

	[ExcludeFromDocs]
	public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return CastFilteredArray_Internal(direction, float.PositiveInfinity, contactFilter, results);
	}

	public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return CastFilteredArray_Internal(direction, distance, contactFilter, results);
	}

	public int Cast(Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return CastFilteredList_Internal(direction, distance, contactFilter, results);
	}

	public int Cast(Vector2 position, float angle, Vector2 direction, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return CastFrom_Internal(position, angle, direction, distance, results);
	}

	public int Cast(Vector2 position, float angle, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return CastFromFiltered_Internal(position, angle, direction, distance, contactFilter, results);
	}

	public int Overlap(ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return OverlapArray_Internal(contactFilter, results);
	}

	public int Overlap(List<Collider2D> results)
	{
		return OverlapList_Internal(results);
	}

	public int Overlap(ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return OverlapFilteredList_Internal(contactFilter, results);
	}

	public int Overlap(Vector2 position, float angle, List<Collider2D> results)
	{
		return OverlapFromList_Internal(position, angle, results);
	}

	public int Overlap(Vector2 position, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return OverlapFromFilteredList_Internal(position, angle, contactFilter, results);
	}

	[NativeMethod("GetAttachedCollidersArray_Binding")]
	private int GetAttachedCollidersArray_Internal([Unmarshalled][NotNull] Collider2D[] results, bool findTriggers)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAttachedCollidersArray_Internal_Injected(intPtr, results, findTriggers);
	}

	[NativeMethod("GetAttachedCollidersList_Binding")]
	private int GetAttachedCollidersList_Internal([NotNull] List<Collider2D> results, bool findTriggers)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAttachedCollidersList_Internal_Injected(intPtr, results, findTriggers);
	}

	[NativeMethod("GetShapes_Binding")]
	private int GetShapes_Internal(ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetShapes_Internal_Injected(intPtr, ref physicsShapeGroupState);
	}

	[NativeMethod("CastArray_Binding")]
	private unsafe int CastArray_Internal(Vector2 direction, float distance, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = CastArray_Internal_Injected(intPtr, ref direction, distance, ref results2);
		}
		return result;
	}

	[NativeMethod("CastList_Binding")]
	private unsafe int CastList_Internal(Vector2 direction, float distance, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CastList_Internal_Injected(intPtr, ref direction, distance, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[NativeMethod("CastFilteredArray_Binding")]
	private unsafe int CastFilteredArray_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = CastFilteredArray_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ref results2);
		}
		return result;
	}

	[NativeMethod("CastFilteredList_Binding")]
	private unsafe int CastFilteredList_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CastFilteredList_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[NativeMethod("CastFrom_Binding")]
	private unsafe int CastFrom_Internal(Vector2 position, float angle, Vector2 direction, float distance, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CastFrom_Internal_Injected(intPtr, ref position, angle, ref direction, distance, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[NativeMethod("CastFromFiltered_Binding")]
	private unsafe int CastFromFiltered_Internal(Vector2 position, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CastFromFiltered_Internal_Injected(intPtr, ref position, angle, ref direction, distance, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[NativeMethod("OverlapArray_Binding")]
	private int OverlapArray_Internal(ContactFilter2D contactFilter, [NotNull][Unmarshalled] Collider2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return OverlapArray_Internal_Injected(intPtr, ref contactFilter, results);
	}

	[NativeMethod("OverlapList_Binding")]
	private int OverlapList_Internal([NotNull] List<Collider2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return OverlapList_Internal_Injected(intPtr, results);
	}

	[NativeMethod("OverlapFilteredList_Binding")]
	private int OverlapFilteredList_Internal(ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return OverlapFilteredList_Internal_Injected(intPtr, ref contactFilter, results);
	}

	[NativeMethod("OverlapFromList_Binding")]
	private int OverlapFromList_Internal(Vector2 position, float angle, [NotNull] List<Collider2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return OverlapFromList_Internal_Injected(intPtr, ref position, angle, results);
	}

	[NativeMethod("OverlapFromFilteredList_Binding")]
	private int OverlapFromFilteredList_Internal(Vector2 position, float angle, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return OverlapFromFilteredList_Internal_Injected(intPtr, ref position, angle, ref contactFilter, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapCollider has been deprecated. Please use Overlap (UnityUpgradable) -> Overlap(*)", false)]
	[ExcludeFromDocs]
	public int OverlapCollider(ContactFilter2D contactFilter, [Out] Collider2D[] results)
	{
		return Overlap(contactFilter, results);
	}

	[ExcludeFromDocs]
	[Obsolete("OverlapCollider has been deprecated. Please use Overlap (UnityUpgradable) -> Overlap(*)", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int OverlapCollider(ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return Overlap(contactFilter, results);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_position_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_position_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_rotation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rotation_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRotation_Angle_Injected(IntPtr _unity_self, float angle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRotation_Quaternion_Injected(IntPtr _unity_self, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MovePosition_Injected(IntPtr _unity_self, [In] ref Vector2 position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveRotation_Angle_Injected(IntPtr _unity_self, float angle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MoveRotation_Quaternion_Injected(IntPtr _unity_self, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MovePositionAndRotation_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MovePositionAndRotation_Quaternion_Injected(IntPtr _unity_self, [In] ref Vector2 position, [In] ref Quaternion rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Slide_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 velocity, float deltaTime, [In] ref SlideMovement slideMovement, out SlideResults ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_linearVelocity_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearVelocity_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_linearVelocityX_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearVelocityX_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_linearVelocityY_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearVelocityY_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_angularVelocity_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_angularVelocity_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useAutoMass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useAutoMass_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_mass_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mass_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_sharedMaterial_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sharedMaterial_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_centerOfMass_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_centerOfMass_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_worldCenterOfMass_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_inertia_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_inertia_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_linearDamping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_linearDamping_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_angularDamping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_angularDamping_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_gravityScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_gravityScale_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RigidbodyType2D get_bodyType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bodyType_Injected(IntPtr _unity_self, RigidbodyType2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDragBehaviour_Injected(IntPtr _unity_self, bool dragged);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useFullKinematicContacts_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useFullKinematicContacts_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_freezeRotation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_freezeRotation_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RigidbodyConstraints2D get_constraints_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_constraints_Injected(IntPtr _unity_self, RigidbodyConstraints2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsSleeping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsAwake_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Sleep_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WakeUp_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_simulated_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_simulated_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RigidbodyInterpolation2D get_interpolation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_interpolation_Injected(IntPtr _unity_self, RigidbodyInterpolation2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RigidbodySleepMode2D get_sleepMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sleepMode_Injected(IntPtr _unity_self, RigidbodySleepMode2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CollisionDetectionMode2D get_collisionDetectionMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_collisionDetectionMode_Injected(IntPtr _unity_self, CollisionDetectionMode2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetAttachedColliderCount_Internal_Injected(IntPtr _unity_self, bool findTriggers);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_totalForce_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_totalForce_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_totalTorque_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_totalTorque_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_excludeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_excludeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_includeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_includeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_Injected(IntPtr _unity_self, IntPtr collider);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_OtherColliderWithFilter_Internal_Injected(IntPtr _unity_self, IntPtr collider, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_AnyColliderWithFilter_Internal_Injected(IntPtr _unity_self, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool OverlapPoint_Injected(IntPtr _unity_self, [In] ref Vector2 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Distance_Internal_Injected(IntPtr _unity_self, IntPtr collider, out ColliderDistance2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceFrom_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 thisPosition, float thisAngle, IntPtr collider, [In] ref Vector2 position, float angle, out ColliderDistance2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddForce_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 force, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddRelativeForce_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 relativeForce, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddForceAtPosition_Injected(IntPtr _unity_self, [In] ref Vector2 force, [In] ref Vector2 position, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddTorque_Injected(IntPtr _unity_self, float torque, [UnityEngine.Internal.DefaultValue("ForceMode2D.Force")] ForceMode2D mode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPoint_Injected(IntPtr _unity_self, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRelativePoint_Injected(IntPtr _unity_self, [In] ref Vector2 relativePoint, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVector_Injected(IntPtr _unity_self, [In] ref Vector2 vector, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRelativeVector_Injected(IntPtr _unity_self, [In] ref Vector2 relativeVector, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPointVelocity_Injected(IntPtr _unity_self, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRelativePointVelocity_Injected(IntPtr _unity_self, [In] ref Vector2 relativePoint, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetAttachedCollidersArray_Internal_Injected(IntPtr _unity_self, Collider2D[] results, bool findTriggers);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetAttachedCollidersList_Internal_Injected(IntPtr _unity_self, List<Collider2D> results, bool findTriggers);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetShapes_Internal_Injected(IntPtr _unity_self, ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastArray_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastFilteredArray_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastFilteredList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastFrom_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref Vector2 direction, float distance, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastFromFiltered_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapArray_Internal_Injected(IntPtr _unity_self, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapList_Internal_Injected(IntPtr _unity_self, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapFilteredList_Internal_Injected(IntPtr _unity_self, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapFromList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapFromFilteredList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);
}
