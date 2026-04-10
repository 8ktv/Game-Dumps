using System;

namespace UnityEngine.LowLevelPhysics2D;

public readonly struct PhysicsJoint : IPhysicsJoint, IEquatable<PhysicsJoint>
{
	public enum JointType
	{
		DistanceJoint,
		IgnoreJoint,
		RelativeJoint,
		SliderJoint,
		HingeJoint,
		FixedJoint,
		WheelJoint
	}

	private readonly int index1;

	private readonly ushort world0;

	private readonly ushort generation;

	public bool isValid => PhysicsLowLevelScripting2D.PhysicsJoint_IsValid(this);

	public PhysicsWorld world => PhysicsLowLevelScripting2D.PhysicsJoint_GetWorld(this);

	public JointType jointType => PhysicsLowLevelScripting2D.PhysicsJoint_GetJointType(this);

	public PhysicsBody bodyA => PhysicsLowLevelScripting2D.PhysicsJoint_GetBodyA(this);

	public PhysicsBody bodyB => PhysicsLowLevelScripting2D.PhysicsJoint_GetBodyB(this);

	public PhysicsTransform localAnchorA
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetLocalAnchorA(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetLocalAnchorA(this, value);
		}
	}

	public PhysicsTransform localAnchorB
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetLocalAnchorB(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetLocalAnchorB(this, value);
		}
	}

	public float forceThreshold
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetForceThreshold(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetForceThreshold(this, value);
		}
	}

	public float torqueThreshold
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetTorqueThreshold(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetTorqueThreshold(this, value);
		}
	}

	public bool collideConnected
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetCollideConnected(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetCollideConnected(this, value);
		}
	}

	public float tuningFrequency
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetTuningFrequency(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetTuningFrequency(this, value);
		}
	}

	public float tuningDamping
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetTuningDamping(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetTuningDamping(this, value);
		}
	}

	public float drawScale
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetDrawScale(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetDrawScale(this, value);
		}
	}

	public Vector2 currentConstraintForce => PhysicsLowLevelScripting2D.PhysicsJoint_GetCurrentConstraintForce(this);

	public float currentConstraintTorque => PhysicsLowLevelScripting2D.PhysicsJoint_GetCurrentConstraintTorque(this);

	public float currentLinearSeparationError => PhysicsLowLevelScripting2D.PhysicsJoint_GetCurrentLinearSeparation(this);

	public float currentAngularSeparationError => PhysicsLowLevelScripting2D.PhysicsJoint_GetCurrentAngularSeparation(this);

	public bool isOwned => PhysicsLowLevelScripting2D.PhysicsJoint_IsOwned(this);

	public object callbackTarget
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetCallbackTarget(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetCallbackTarget(this, value);
		}
	}

	public PhysicsUserData userData
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsJoint_GetUserData(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsJoint_SetUserData(this, value);
		}
	}

	public override string ToString()
	{
		return isValid ? $"type={jointType}, index={index1}, world={world0}, generation={generation}" : "<INVALID>";
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public bool Equals(PhysicsJoint other)
	{
		return index1 == other.index1 && world0 == other.world0 && generation == other.generation;
	}

	public static bool operator ==(PhysicsJoint lhs, PhysicsJoint rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PhysicsJoint lhs, PhysicsJoint rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(index1, world0, generation);
	}

	public bool Destroy(int ownerKey = 0)
	{
		return PhysicsLowLevelScripting2D.PhysicsJoint_Destroy(this, ownerKey);
	}

	public void WakeBodies()
	{
		PhysicsLowLevelScripting2D.PhysicsJoint_WakeBodies(this);
	}

	public int SetOwner(Object owner)
	{
		return PhysicsLowLevelScripting2D.PhysicsJoint_SetOwner(this, owner);
	}

	public Object GetOwner()
	{
		return PhysicsLowLevelScripting2D.PhysicsJoint_GetOwner(this);
	}

	public void Draw()
	{
		PhysicsLowLevelScripting2D.PhysicsJoint_Draw(this);
	}

	public static PhysicsDistanceJoint CreateJoint(PhysicsWorld world, PhysicsDistanceJointDefinition definition)
	{
		return PhysicsDistanceJoint.Create(world, definition);
	}

	public static PhysicsRelativeJoint CreateJoint(PhysicsWorld world, PhysicsRelativeJointDefinition definition)
	{
		return PhysicsRelativeJoint.Create(world, definition);
	}

	public static PhysicsIgnoreJoint CreateJoint(PhysicsWorld world, PhysicsIgnoreJointDefinition definition)
	{
		return PhysicsIgnoreJoint.Create(world, definition);
	}

	public static PhysicsSliderJoint CreateJoint(PhysicsWorld world, PhysicsSliderJointDefinition definition)
	{
		return PhysicsSliderJoint.Create(world, definition);
	}

	public static PhysicsHingeJoint CreateJoint(PhysicsWorld world, PhysicsHingeJointDefinition definition)
	{
		return PhysicsHingeJoint.Create(world, definition);
	}

	public static PhysicsFixedJoint CreateJoint(PhysicsWorld world, PhysicsFixedJointDefinition definition)
	{
		return PhysicsFixedJoint.Create(world, definition);
	}

	public static PhysicsWheelJoint CreateJoint(PhysicsWorld world, PhysicsWheelJointDefinition definition)
	{
		return PhysicsWheelJoint.Create(world, definition);
	}

	public static void DestroyBatch(ReadOnlySpan<PhysicsJoint> joints)
	{
		PhysicsLowLevelScripting2D.PhysicsJoint_DestroyBatch(joints);
	}
}
