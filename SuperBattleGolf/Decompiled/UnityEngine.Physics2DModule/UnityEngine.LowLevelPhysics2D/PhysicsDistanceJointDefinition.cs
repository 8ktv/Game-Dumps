using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsDistanceJointDefinition
{
	[SerializeField]
	private PhysicsBody m_BodyA;

	[SerializeField]
	private PhysicsBody m_BodyB;

	[SerializeField]
	private PhysicsTransform m_LocalAnchorA;

	[SerializeField]
	private PhysicsTransform m_LocalAnchorB;

	[Min(float.Epsilon)]
	[SerializeField]
	private float m_Distance;

	[SerializeField]
	private bool m_EnableSpring;

	[SerializeField]
	[Min(0f)]
	private float m_SpringFrequency;

	[SerializeField]
	[Min(0f)]
	private float m_SpringDamping;

	[SerializeField]
	private float m_SpringLowerForce;

	[SerializeField]
	private float m_SpringUpperForce;

	[SerializeField]
	private bool m_EnableMotor;

	[SerializeField]
	private float m_MotorSpeed;

	[SerializeField]
	private float m_MaxMotorForce;

	[SerializeField]
	private bool m_EnableLimit;

	[SerializeField]
	[Min(0f)]
	private float m_MinDistanceLimit;

	[Min(0f)]
	[SerializeField]
	private float m_MaxDistanceLimit;

	[Min(0f)]
	[SerializeField]
	private float m_ForceThreshold;

	[SerializeField]
	[Min(0f)]
	private float m_TorqueThreshold;

	[Range(0f, 1000f)]
	[SerializeField]
	private float m_TuningFrequency;

	[SerializeField]
	[Range(0f, 10f)]
	private float m_TuningDamping;

	[Range(0.001f, 10f)]
	[SerializeField]
	private float m_DrawScale;

	[SerializeField]
	private bool m_CollideConnected;

	public static PhysicsDistanceJointDefinition defaultDefinition => PhysicsLowLevelScripting2D.DistanceJoint_GetDefaultDefinition(useSettings: true);

	public PhysicsBody bodyA
	{
		readonly get
		{
			return m_BodyA;
		}
		set
		{
			m_BodyA = value;
		}
	}

	public PhysicsBody bodyB
	{
		readonly get
		{
			return m_BodyB;
		}
		set
		{
			m_BodyB = value;
		}
	}

	public PhysicsTransform localAnchorA
	{
		readonly get
		{
			return m_LocalAnchorA;
		}
		set
		{
			m_LocalAnchorA = value;
		}
	}

	public PhysicsTransform localAnchorB
	{
		readonly get
		{
			return m_LocalAnchorB;
		}
		set
		{
			m_LocalAnchorB = value;
		}
	}

	public float distance
	{
		readonly get
		{
			return m_Distance;
		}
		set
		{
			m_Distance = Math.Max(float.Epsilon, value);
		}
	}

	public bool enableSpring
	{
		readonly get
		{
			return m_EnableSpring;
		}
		set
		{
			m_EnableSpring = value;
		}
	}

	public float springFrequency
	{
		readonly get
		{
			return m_SpringFrequency;
		}
		set
		{
			m_SpringFrequency = Mathf.Max(0f, value);
		}
	}

	public float springDamping
	{
		readonly get
		{
			return m_SpringDamping;
		}
		set
		{
			m_SpringDamping = Mathf.Max(0f, value);
		}
	}

	public float springLowerForce
	{
		readonly get
		{
			return m_SpringLowerForce;
		}
		set
		{
			m_SpringLowerForce = value;
		}
	}

	public float springUpperForce
	{
		readonly get
		{
			return m_SpringUpperForce;
		}
		set
		{
			m_SpringUpperForce = value;
		}
	}

	public bool enableMotor
	{
		readonly get
		{
			return m_EnableMotor;
		}
		set
		{
			m_EnableMotor = value;
		}
	}

	public float motorSpeed
	{
		readonly get
		{
			return m_MotorSpeed;
		}
		set
		{
			m_MotorSpeed = value;
		}
	}

	public float maxMotorForce
	{
		readonly get
		{
			return m_MaxMotorForce;
		}
		set
		{
			m_MaxMotorForce = value;
		}
	}

	public bool enableLimit
	{
		readonly get
		{
			return m_EnableLimit;
		}
		set
		{
			m_EnableLimit = value;
		}
	}

	public float minDistanceLimit
	{
		readonly get
		{
			return m_MinDistanceLimit;
		}
		set
		{
			m_MinDistanceLimit = Mathf.Max(0f, value);
		}
	}

	public float maxDistanceLimit
	{
		readonly get
		{
			return m_MaxDistanceLimit;
		}
		set
		{
			m_MaxDistanceLimit = Mathf.Max(0f, value);
		}
	}

	public float forceThreshold
	{
		readonly get
		{
			return m_ForceThreshold;
		}
		set
		{
			m_ForceThreshold = Mathf.Max(0f, value);
		}
	}

	public float torqueThreshold
	{
		readonly get
		{
			return m_TorqueThreshold;
		}
		set
		{
			m_TorqueThreshold = Mathf.Max(0f, value);
		}
	}

	public float tuningFrequency
	{
		readonly get
		{
			return m_TuningFrequency;
		}
		set
		{
			m_TuningFrequency = Mathf.Clamp(value, 0f, 1000f);
		}
	}

	public float tuningDamping
	{
		readonly get
		{
			return m_TuningDamping;
		}
		set
		{
			m_TuningDamping = Mathf.Clamp(value, 0f, 10f);
		}
	}

	public float drawScale
	{
		readonly get
		{
			return m_DrawScale;
		}
		set
		{
			m_DrawScale = Mathf.Clamp(value, 0.001f, 10f);
		}
	}

	public bool collideConnected
	{
		readonly get
		{
			return m_CollideConnected;
		}
		set
		{
			m_CollideConnected = value;
		}
	}

	public PhysicsDistanceJointDefinition()
	{
		this = defaultDefinition;
	}

	public PhysicsDistanceJointDefinition(bool useSettings)
	{
		this = PhysicsLowLevelScripting2D.DistanceJoint_GetDefaultDefinition(useSettings);
	}
}
