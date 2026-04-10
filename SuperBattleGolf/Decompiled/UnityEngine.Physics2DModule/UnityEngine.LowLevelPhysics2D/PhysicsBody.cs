using System;
using System.ComponentModel;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D;

public readonly struct PhysicsBody : IEquatable<PhysicsBody>
{
	public enum BodyType
	{
		Dynamic,
		Kinematic,
		Static
	}

	[Flags]
	public enum BodyConstraints
	{
		None = 0,
		PositionX = 1,
		PositionY = 2,
		Rotation = 4,
		Position = 3,
		All = 7
	}

	public enum TransformWriteMode
	{
		Current,
		Interpolate,
		Extrapolate,
		Off
	}

	public struct TransformWriteTween
	{
		private PhysicsBody m_Body;

		private TransformWriteMode m_TransformWriteMode;

		private PhysicsTransform m_PhysicsTransform;

		private Vector2 m_LinearVelocity;

		private float m_AngularVelocity;

		private Vector3 m_PositionFrom;

		private Quaternion m_RotationFrom;

		public PhysicsBody body
		{
			readonly get
			{
				return m_Body;
			}
			set
			{
				m_Body = value;
			}
		}

		public TransformWriteMode transformWriteMode
		{
			readonly get
			{
				return m_TransformWriteMode;
			}
			set
			{
				m_TransformWriteMode = value;
			}
		}

		public PhysicsTransform physicsTransform
		{
			readonly get
			{
				return m_PhysicsTransform;
			}
			set
			{
				m_PhysicsTransform = value;
			}
		}

		public Vector2 linearVelocity
		{
			readonly get
			{
				return m_LinearVelocity;
			}
			set
			{
				m_LinearVelocity = value;
			}
		}

		public float angularVelocity
		{
			readonly get
			{
				return m_AngularVelocity;
			}
			set
			{
				m_AngularVelocity = value;
			}
		}

		public Vector3 positionFrom
		{
			readonly get
			{
				return m_PositionFrom;
			}
			set
			{
				m_PositionFrom = value;
			}
		}

		public Quaternion rotationFrom
		{
			readonly get
			{
				return m_RotationFrom;
			}
			set
			{
				m_RotationFrom = value;
			}
		}
	}

	[Serializable]
	public struct MassConfiguration
	{
		[SerializeField]
		private float m_Mass;

		[SerializeField]
		private Vector2 m_Center;

		[SerializeField]
		private float m_RotationalInertia;

		public float mass
		{
			readonly get
			{
				return m_Mass;
			}
			set
			{
				m_Mass = value;
			}
		}

		public Vector2 center
		{
			readonly get
			{
				return m_Center;
			}
			set
			{
				m_Center = value;
			}
		}

		public float rotationalInertia
		{
			readonly get
			{
				return m_RotationalInertia;
			}
			set
			{
				m_RotationalInertia = value;
			}
		}
	}

	public struct BatchVelocity
	{
		private PhysicsBody m_PhysicsBody;

		private Vector2 m_LinearVelocity;

		private float m_AngularVelocity;

		private bool m_UseLinearVelocity;

		private bool m_UseAngularVelocity;

		public PhysicsBody physicsBody
		{
			readonly get
			{
				return m_PhysicsBody;
			}
			set
			{
				m_PhysicsBody = value;
			}
		}

		public Vector2 linearVelocity
		{
			readonly get
			{
				return m_LinearVelocity;
			}
			set
			{
				m_LinearVelocity = value;
				m_UseLinearVelocity = true;
			}
		}

		public float angularVelocity
		{
			readonly get
			{
				return m_AngularVelocity;
			}
			set
			{
				m_AngularVelocity = value;
				m_UseAngularVelocity = true;
			}
		}

		public BatchVelocity(PhysicsBody physicsBody)
		{
			m_PhysicsBody = physicsBody;
			m_LinearVelocity = default(Vector2);
			m_AngularVelocity = 0f;
			m_UseLinearVelocity = false;
			m_UseAngularVelocity = false;
		}
	}

	public struct BatchForce
	{
		private PhysicsBody m_PhysicsBody;

		private Vector2 m_LinearForce;

		private Vector2 m_LinearForcePosition;

		private float m_Torque;

		private bool m_WakeBody;

		private bool m_UseLinearForce;

		private bool m_UseLinearForcePosition;

		private bool m_UseTorque;

		public PhysicsBody physicsBody
		{
			readonly get
			{
				return m_PhysicsBody;
			}
			set
			{
				m_PhysicsBody = value;
			}
		}

		public BatchForce(PhysicsBody physicsBody)
		{
			m_PhysicsBody = physicsBody;
			m_LinearForce = default(Vector2);
			m_LinearForcePosition = default(Vector2);
			m_Torque = 0f;
			m_WakeBody = false;
			m_UseLinearForce = false;
			m_UseLinearForcePosition = false;
			m_UseTorque = false;
		}

		public void ApplyForce(Vector2 force, Vector2 point, bool wake = true)
		{
			m_LinearForce = force;
			m_LinearForcePosition = point;
			m_WakeBody = wake;
			m_UseLinearForce = true;
			m_UseLinearForcePosition = true;
		}

		public void ApplyForceToCenter(Vector2 force, bool wake = true)
		{
			m_LinearForce = force;
			m_WakeBody = wake;
			m_UseLinearForce = true;
			m_UseLinearForcePosition = false;
		}

		public void ApplyTorque(float torque, bool wake = true)
		{
			m_Torque = torque;
			m_WakeBody = wake;
			m_UseTorque = true;
		}
	}

	public struct BatchImpulse
	{
		private PhysicsBody m_PhysicsBody;

		private Vector2 m_LinearImpulse;

		private Vector2 m_LinearImpulsePosition;

		private float m_AngularImpulse;

		private bool m_WakeBody;

		private bool m_UseLinearImpulse;

		private bool m_UseLinearImpulsePosition;

		private bool m_UseAngularImpulse;

		public PhysicsBody physicsBody
		{
			readonly get
			{
				return m_PhysicsBody;
			}
			set
			{
				m_PhysicsBody = value;
			}
		}

		public BatchImpulse(PhysicsBody physicsBody)
		{
			m_PhysicsBody = physicsBody;
			m_LinearImpulse = default(Vector2);
			m_LinearImpulsePosition = default(Vector2);
			m_AngularImpulse = 0f;
			m_WakeBody = false;
			m_UseLinearImpulse = false;
			m_UseLinearImpulsePosition = false;
			m_UseAngularImpulse = false;
		}

		public void ApplyLinearImpulse(Vector2 impulse, Vector2 point, bool wake = true)
		{
			m_LinearImpulse = impulse;
			m_LinearImpulsePosition = point;
			m_WakeBody = wake;
			m_UseLinearImpulse = true;
			m_UseLinearImpulsePosition = true;
		}

		public void ApplyLinearImpulseToCenter(Vector2 impulse, bool wake = true)
		{
			m_LinearImpulse = impulse;
			m_WakeBody = wake;
			m_UseLinearImpulse = true;
			m_UseLinearImpulsePosition = false;
		}

		public void ApplyAngularImpulse(float impulse, bool wake = true)
		{
			m_AngularImpulse = impulse;
			m_WakeBody = true;
			m_UseAngularImpulse = true;
		}
	}

	public struct BatchTransform
	{
		private PhysicsBody m_PhysicsBody;

		private PhysicsTransform m_PhysicsTransform;

		private bool m_UsePosition;

		private bool m_UseRotation;

		public PhysicsBody physicsBody
		{
			readonly get
			{
				return m_PhysicsBody;
			}
			set
			{
				m_PhysicsBody = value;
			}
		}

		public Vector2 position
		{
			readonly get
			{
				return m_PhysicsTransform.position;
			}
			set
			{
				m_PhysicsTransform.position = value;
				m_UsePosition = true;
			}
		}

		public PhysicsRotate rotation
		{
			readonly get
			{
				return m_PhysicsTransform.rotation;
			}
			set
			{
				m_PhysicsTransform.rotation = value;
				m_UseRotation = true;
			}
		}

		public PhysicsTransform transform
		{
			readonly get
			{
				return m_PhysicsTransform;
			}
			set
			{
				m_PhysicsTransform = value;
				m_UsePosition = (m_UseRotation = true);
			}
		}

		public BatchTransform(PhysicsBody physicsBody)
		{
			m_PhysicsBody = physicsBody;
			m_PhysicsTransform = default(PhysicsTransform);
			m_UsePosition = false;
			m_UseRotation = false;
		}
	}

	private readonly int m_Index1;

	private readonly ushort m_World0;

	private readonly ushort m_Generation;

	public PhysicsBodyDefinition definition
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_ReadDefinition(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_WriteDefinition(this, value, onlyExtendedProperties: false);
		}
	}

	public bool isValid => PhysicsLowLevelScripting2D.PhysicsBody_IsValid(this);

	public PhysicsWorld world => PhysicsLowLevelScripting2D.PhysicsBody_GetWorld(this);

	public BodyType type
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetBodyType(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetBodyType(this, value);
		}
	}

	public BodyConstraints constraints
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetBodyConstraints(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetBodyConstraints(this, value);
		}
	}

	public Vector2 position
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetPosition(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetPosition(this, value);
		}
	}

	public PhysicsRotate rotation
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetRotation(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetRotation(this, value);
		}
	}

	public PhysicsTransform transform
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetTransform(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetTransform(this, value);
		}
	}

	public Vector2 linearVelocity
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetLinearVelocity(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetLinearVelocity(this, value);
		}
	}

	public float angularVelocity
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetAngularVelocity(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetAngularVelocity(this, value);
		}
	}

	public float mass => PhysicsLowLevelScripting2D.PhysicsBody_GetMass(this);

	public float rotationalInertia => PhysicsLowLevelScripting2D.PhysicsBody_GetRotationalInertia(this);

	public Vector2 localCenterOfMass => PhysicsLowLevelScripting2D.PhysicsBody_GetLocalCenterOfMass(this);

	public Vector2 worldCenterOfMass => PhysicsLowLevelScripting2D.PhysicsBody_GetWorldCenterOfMass(this);

	public MassConfiguration massConfiguration
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetMassConfiguration(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetMassConfiguration(this, value);
		}
	}

	public float linearDamping
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetLinearDamping(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetLinearDamping(this, value);
		}
	}

	public float angularDamping
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetAngularDamping(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetAngularDamping(this, value);
		}
	}

	public float gravityScale
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetGravityScale(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetGravityScale(this, value);
		}
	}

	public bool awake
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetAwake(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetAwake(this, value);
		}
	}

	public bool sleepingAllowed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetSleepingAllowed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetSleepingAllowed(this, value);
		}
	}

	public float sleepThreshold
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetSleepThreshold(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetSleepThreshold(this, value);
		}
	}

	public bool enabled
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetEnabled(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetEnabled(this, value);
		}
	}

	public bool fastRotationAllowed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetFastRotationAllowed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetFastRotationAllowed(this, value);
		}
	}

	public bool fastCollisionsAllowed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetFastCollisionsAllowed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetFastCollisionsAllowed(this, value);
		}
	}

	public bool isOwned => PhysicsLowLevelScripting2D.PhysicsBody_IsOwned(this);

	public object callbackTarget
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetCallbackTarget(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetCallbackTarget(this, value);
		}
	}

	public PhysicsUserData userData
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetUserData(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetUserData(this, value);
		}
	}

	public Transform transformObject
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetTransformObject(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetTransformObject(this, value);
		}
	}

	public TransformWriteMode transformWriteMode
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsBody_GetTransformWriteMode(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsBody_SetTransformWriteMode(this, value);
		}
	}

	public int shapeCount => PhysicsLowLevelScripting2D.PhysicsBody_GetShapeCount(this);

	public int jointCount => PhysicsLowLevelScripting2D.PhysicsBody_GetJointCount(this);

	[Obsolete("PhysicsBody.bodyType has been deprecated. Please use PhysicsBody.type instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public RigidbodyType2D bodyType
	{
		get
		{
			return (RigidbodyType2D)type;
		}
		set
		{
			type = (BodyType)value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("PhysicsBody.bodyConstraints has been deprecated. Please use PhysicsBody.constraints instead.", false)]
	public RigidbodyConstraints2D bodyConstraints
	{
		get
		{
			return (RigidbodyConstraints2D)constraints;
		}
		set
		{
			constraints = (BodyConstraints)value;
		}
	}

	public override string ToString()
	{
		return isValid ? $"type={type}, index={m_Index1}, world={m_World0}, generation={m_Generation}" : "<INVALID>";
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public bool Equals(PhysicsBody other)
	{
		return m_Index1 == other.m_Index1 && m_World0 == other.m_World0 && m_Generation == other.m_Generation;
	}

	public static bool operator ==(PhysicsBody lhs, PhysicsBody rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PhysicsBody lhs, PhysicsBody rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(m_Index1, m_World0, m_Generation);
	}

	public static PhysicsBody Create(PhysicsWorld world)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_Create(world, PhysicsBodyDefinition.defaultDefinition);
	}

	public static PhysicsBody Create(PhysicsWorld world, PhysicsBodyDefinition definition)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_Create(world, definition);
	}

	public unsafe static NativeArray<PhysicsBody> CreateBatch(PhysicsWorld world, PhysicsBodyDefinition definition, int bodyCount, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_CreateBatch(world, new ReadOnlySpan<PhysicsBodyDefinition>(&definition, 1), bodyCount, allocator).ToNativeArray<PhysicsBody>();
	}

	public static NativeArray<PhysicsBody> CreateBatch(PhysicsWorld world, ReadOnlySpan<PhysicsBodyDefinition> definitions, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_CreateBatch(world, definitions, definitions.Length, allocator).ToNativeArray<PhysicsBody>();
	}

	public bool Destroy(int ownerKey = 0)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_Destroy(this, ownerKey);
	}

	public static void DestroyBatch(ReadOnlySpan<PhysicsBody> bodies)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_DestroyBatch(bodies);
	}

	public static void SetBatchVelocity(ReadOnlySpan<BatchVelocity> batch)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_SetBatchVelocity(batch);
	}

	public static void SetBatchForce(ReadOnlySpan<BatchForce> batch)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_SetBatchForce(batch);
	}

	public static void SetBatchImpulse(ReadOnlySpan<BatchImpulse> batch)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_SetBatchImpulse(batch);
	}

	public static void SetBatchTransform(ReadOnlySpan<BatchTransform> batch)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_SetBatchTransform(batch);
	}

	public void SetTransformTarget(PhysicsTransform transform, float deltaTime)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_SetTransformTarget(this, transform, deltaTime);
	}

	public void GetPositionAndRotation3D(Transform transform, PhysicsWorld.TransformWriteMode transformWriteMode, PhysicsWorld.TransformPlane transformPlane, out Vector3 position, out Quaternion rotation)
	{
		if (transform == null)
		{
			throw new ArgumentNullException("transform", "Transform cannot be NULL.");
		}
		PhysicsTransform physicsTransform = this.transform;
		PhysicsWorld physicsWorld = world;
		switch (transformWriteMode)
		{
		case PhysicsWorld.TransformWriteMode.Fast2D:
			position = PhysicsMath.ToPosition3D(physicsTransform.position, transform.position, transformPlane);
			rotation = PhysicsMath.ToRotationFast3D(physicsTransform.rotation.angle, transformPlane);
			break;
		case PhysicsWorld.TransformWriteMode.Slow3D:
		{
			transform.GetPositionAndRotation(out var reference, out var reference2);
			position = PhysicsMath.ToPosition3D(physicsTransform.position, reference, transformPlane);
			rotation = PhysicsMath.ToRotationSlow3D(physicsTransform.rotation.angle, reference2, transformPlane);
			break;
		}
		default:
			throw new InvalidOperationException("Invalid Transform Write Mode.");
		}
	}

	public bool SetAndWriteTransform(PhysicsTransform transform)
	{
		this.transform = transform;
		Transform transform2 = transformObject;
		if (transform2 == null)
		{
			return false;
		}
		PhysicsWorld physicsWorld = world;
		PhysicsWorld.TransformWriteMode transformWriteMode = physicsWorld.transformWriteMode;
		PhysicsWorld.TransformPlane transformPlane = physicsWorld.transformPlane;
		switch (transformWriteMode)
		{
		case PhysicsWorld.TransformWriteMode.Off:
			return false;
		case PhysicsWorld.TransformWriteMode.Fast2D:
		case PhysicsWorld.TransformWriteMode.Slow3D:
		{
			GetPositionAndRotation3D(transformObject, transformWriteMode, transformPlane, out var vector, out var quaternion);
			transform2.SetPositionAndRotation(vector, quaternion);
			return true;
		}
		default:
			throw new InvalidOperationException("Invalid Transform Write Mode.");
		}
	}

	public Vector2 GetLocalPoint(Vector2 worldPoint)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetLocalPoint(this, worldPoint);
	}

	public Vector2 GetWorldPoint(Vector2 localPoint)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetWorldPoint(this, localPoint);
	}

	public Vector2 GetLocalVector(Vector2 worldVector)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetLocalVector(this, worldVector);
	}

	public Vector2 GetWorldVector(Vector2 localVector)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetWorldVector(this, localVector);
	}

	public Vector2 GetLocalPointVelocity(Vector2 localPoint)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetLocalPointVelocity(this, localPoint);
	}

	public Vector2 GetWorldPointVelocity(Vector2 worldPoint)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetWorldPointVelocity(this, worldPoint);
	}

	public void ApplyMassFromShapes()
	{
		PhysicsLowLevelScripting2D.PhysicsBody_ApplyMassFromShapes(this);
	}

	public void ApplyForce(Vector2 force, Vector2 point, bool wake = true)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_ApplyForce(this, force, point, wake);
	}

	public void ApplyForceToCenter(Vector2 force, bool wake = true)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_ApplyForceToCenter(this, force, wake);
	}

	public void ApplyTorque(float torque, bool wake = true)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_ApplyTorque(this, torque, wake);
	}

	public void ApplyLinearImpulse(Vector2 impulse, Vector2 point, bool wake = true)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_ApplyLinearImpulse(this, impulse, point, wake);
	}

	public void ApplyLinearImpulseToCenter(Vector2 impulse, bool wake = true)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_ApplyLinearImpulseToCenter(this, impulse, wake);
	}

	public void ApplyAngularImpulse(float impulse, bool wake = true)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_ApplyAngularImpulse(this, impulse, wake);
	}

	public void ClearForces()
	{
		PhysicsLowLevelScripting2D.PhysicsBody_ClearForces(this);
	}

	public void WakeTouching()
	{
		PhysicsLowLevelScripting2D.PhysicsBody_WakeTouching(this);
	}

	public void SetContactEvents(bool contactEvents)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_SetContactEvents(this, contactEvents);
	}

	public void SetHitEvents(bool hitEvents)
	{
		PhysicsLowLevelScripting2D.PhysicsBody_SetHitEvents(this, hitEvents);
	}

	public int SetOwner(Object owner)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_SetOwner(this, owner);
	}

	public Object GetOwner()
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetOwner(this);
	}

	public NativeArray<PhysicsShape> GetShapes(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetShapes(this, allocator).ToNativeArray<PhysicsShape>();
	}

	public NativeArray<PhysicsJoint> GetJoints(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetJoints(this, allocator).ToNativeArray<PhysicsJoint>();
	}

	public NativeArray<PhysicsShape.Contact> GetContacts(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_GetContacts(this, allocator).ToNativeArray<PhysicsShape.Contact>();
	}

	public PhysicsAABB GetAABB()
	{
		return PhysicsLowLevelScripting2D.PhysicsBody_CalculateAABB(this);
	}

	public PhysicsShape CreateShape(CircleGeometry geometry)
	{
		return PhysicsShape.CreateShape(this, geometry);
	}

	public PhysicsShape CreateShape(CircleGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsShape.CreateShape(this, geometry, definition);
	}

	public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<CircleGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
	}

	public PhysicsShape CreateShape(PolygonGeometry geometry)
	{
		return PhysicsShape.CreateShape(this, geometry);
	}

	public PhysicsShape CreateShape(PolygonGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsShape.CreateShape(this, geometry, definition);
	}

	public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<PolygonGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
	}

	public PhysicsShape CreateShape(CapsuleGeometry geometry)
	{
		return PhysicsShape.CreateShape(this, geometry);
	}

	public PhysicsShape CreateShape(CapsuleGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsShape.CreateShape(this, geometry, definition);
	}

	public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<CapsuleGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
	}

	public PhysicsShape CreateShape(SegmentGeometry geometry)
	{
		return PhysicsShape.CreateShape(this, geometry);
	}

	public PhysicsShape CreateShape(SegmentGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsShape.CreateShape(this, geometry, definition);
	}

	public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<SegmentGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
	}

	public PhysicsShape CreateShape(ChainSegmentGeometry geometry)
	{
		return PhysicsShape.CreateShape(this, geometry);
	}

	public PhysicsShape CreateShape(ChainSegmentGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsShape.CreateShape(this, geometry, definition);
	}

	public NativeArray<PhysicsShape> CreateShapeBatch(ReadOnlySpan<ChainSegmentGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsShape.CreateShapeBatch(this, geometry, definition, allocator);
	}

	public PhysicsChain CreateChain(ChainGeometry geometry, PhysicsChainDefinition definition)
	{
		return PhysicsChain.Create(this, geometry, definition);
	}

	public void Draw()
	{
		PhysicsLowLevelScripting2D.PhysicsBody_Draw(this);
	}
}
