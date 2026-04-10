using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D;

public readonly struct PhysicsRelativeJoint : IPhysicsJoint, IEquatable<PhysicsRelativeJoint>
{
	private readonly PhysicsJoint m_Id;

	public bool isValid => m_Id.isValid;

	public PhysicsWorld world => m_Id.world;

	public PhysicsJoint.JointType jointType => m_Id.jointType;

	public PhysicsBody bodyA => m_Id.bodyA;

	public PhysicsBody bodyB => m_Id.bodyB;

	public PhysicsTransform localAnchorA
	{
		get
		{
			return m_Id.localAnchorA;
		}
		set
		{
			m_Id.localAnchorA = value;
		}
	}

	public PhysicsTransform localAnchorB
	{
		get
		{
			return m_Id.localAnchorB;
		}
		set
		{
			m_Id.localAnchorB = value;
		}
	}

	public float forceThreshold
	{
		get
		{
			return m_Id.forceThreshold;
		}
		set
		{
			m_Id.forceThreshold = value;
		}
	}

	public float torqueThreshold
	{
		get
		{
			return m_Id.torqueThreshold;
		}
		set
		{
			m_Id.torqueThreshold = value;
		}
	}

	public bool collideConnected
	{
		get
		{
			return m_Id.collideConnected;
		}
		set
		{
			m_Id.collideConnected = value;
		}
	}

	public float tuningFrequency
	{
		get
		{
			return m_Id.tuningFrequency;
		}
		set
		{
			m_Id.tuningFrequency = value;
		}
	}

	public float tuningDamping
	{
		get
		{
			return m_Id.tuningDamping;
		}
		set
		{
			m_Id.tuningDamping = value;
		}
	}

	public float drawScale
	{
		get
		{
			return m_Id.drawScale;
		}
		set
		{
			m_Id.drawScale = value;
		}
	}

	public Vector2 currentConstraintForce => m_Id.currentConstraintForce;

	public float currentConstraintTorque => m_Id.currentConstraintTorque;

	public float currentLinearSeparationError => m_Id.currentLinearSeparationError;

	public float currentAngularSeparationError => m_Id.currentAngularSeparationError;

	public bool isOwned => m_Id.isOwned;

	public object callbackTarget
	{
		get
		{
			return m_Id.callbackTarget;
		}
		set
		{
			m_Id.callbackTarget = value;
		}
	}

	public PhysicsUserData userData
	{
		get
		{
			return m_Id.userData;
		}
		set
		{
			m_Id.userData = value;
		}
	}

	public Vector2 linearVelocity
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetLinearVelocity(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetLinearVelocity(this, value);
		}
	}

	public float angularVelocity
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetAngularVelocity(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetAngularVelocity(this, value);
		}
	}

	public float maxForce
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetMaxForce(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetMaxForce(this, value);
		}
	}

	public float maxTorque
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetMaxTorque(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetMaxTorque(this, value);
		}
	}

	public float springLinearFrequency
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringLinearFrequency(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetSpringLinearFrequency(this, value);
		}
	}

	public float springAngularFrequency
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringAngularFrequency(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetSpringAngularFrequency(this, value);
		}
	}

	public float springLinearDamping
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringLinearDamping(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetSpringLinearDamping(this, value);
		}
	}

	public float springAngularDamping
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringAngularDamping(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetSpringAngularDamping(this, value);
		}
	}

	public float springMaxForce
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringMaxForce(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetSpringMaxForce(this, value);
		}
	}

	public float springMaxTorque
	{
		get
		{
			return PhysicsLowLevelScripting2D.RelativeJoint_GetSpringMaxTorque(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.RelativeJoint_SetSpringMaxTorque(this, value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PhysicsJoint(PhysicsRelativeJoint joint)
	{
		return joint.m_Id;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PhysicsRelativeJoint(PhysicsJoint joint)
	{
		return new PhysicsRelativeJoint(joint);
	}

	public PhysicsRelativeJoint(PhysicsJoint physicsJoint)
	{
		if (physicsJoint.jointType != PhysicsJoint.JointType.RelativeJoint)
		{
			throw new InvalidCastException(string.Format("The joint must be of type {0} but is of type {1}.", "RelativeJoint", physicsJoint.jointType));
		}
		m_Id = physicsJoint;
	}

	public override string ToString()
	{
		return m_Id.ToString();
	}

	public override bool Equals(object obj)
	{
		return m_Id.Equals(obj);
	}

	public bool Equals(PhysicsRelativeJoint other)
	{
		return m_Id.Equals(other);
	}

	public static bool operator ==(PhysicsRelativeJoint lhs, PhysicsRelativeJoint rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PhysicsRelativeJoint lhs, PhysicsRelativeJoint rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return m_Id.GetHashCode();
	}

	public bool Destroy(int ownerKey = 0)
	{
		return m_Id.Destroy(ownerKey);
	}

	public void WakeBodies()
	{
		m_Id.WakeBodies();
	}

	public int SetOwner(Object owner)
	{
		return m_Id.SetOwner(owner);
	}

	public Object GetOwner()
	{
		return m_Id.GetOwner();
	}

	public void Draw()
	{
		m_Id.Draw();
	}

	public static PhysicsRelativeJoint Create(PhysicsWorld world, PhysicsRelativeJointDefinition definition)
	{
		return PhysicsLowLevelScripting2D.RelativeJoint_Create(world, definition);
	}

	public static void DestroyBatch(ReadOnlySpan<PhysicsJoint> joints)
	{
		PhysicsJoint.DestroyBatch(joints);
	}
}
