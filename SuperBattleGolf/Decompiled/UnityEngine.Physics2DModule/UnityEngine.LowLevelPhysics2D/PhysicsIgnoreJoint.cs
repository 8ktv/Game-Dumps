using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D;

public readonly struct PhysicsIgnoreJoint : IPhysicsJoint, IEquatable<PhysicsIgnoreJoint>
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

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PhysicsJoint(PhysicsIgnoreJoint joint)
	{
		return joint.m_Id;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PhysicsIgnoreJoint(PhysicsJoint joint)
	{
		return new PhysicsIgnoreJoint(joint);
	}

	public PhysicsIgnoreJoint(PhysicsJoint physicsJoint)
	{
		if (physicsJoint.jointType != PhysicsJoint.JointType.IgnoreJoint)
		{
			throw new InvalidCastException(string.Format("The joint must be of type {0} but is of type {1}.", "IgnoreJoint", physicsJoint.jointType));
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

	public bool Equals(PhysicsIgnoreJoint other)
	{
		return m_Id.Equals(other);
	}

	public static bool operator ==(PhysicsIgnoreJoint lhs, PhysicsIgnoreJoint rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PhysicsIgnoreJoint lhs, PhysicsIgnoreJoint rhs)
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

	public static PhysicsIgnoreJoint Create(PhysicsWorld world, PhysicsIgnoreJointDefinition definition)
	{
		return PhysicsLowLevelScripting2D.IgnorePhysicsJoint_Create(world, definition);
	}

	public static void DestroyBatch(ReadOnlySpan<PhysicsJoint> joints)
	{
		PhysicsJoint.DestroyBatch(joints);
	}
}
