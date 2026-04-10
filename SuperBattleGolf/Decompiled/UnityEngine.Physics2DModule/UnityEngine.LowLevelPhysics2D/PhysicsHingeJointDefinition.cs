using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsHingeJointDefinition
{
	[SerializeField]
	private PhysicsBody m_BodyA;

	[SerializeField]
	private PhysicsBody m_BodyB;

	[SerializeField]
	private PhysicsTransform m_LocalAnchorA;

	[SerializeField]
	private PhysicsTransform m_LocalAnchorB;

	[SerializeField]
	private bool m_EnableSpring;

	[SerializeField]
	private float m_SpringTargetAngle;

	[SerializeField]
	[Min(0f)]
	private float m_SpringFrequency;

	[Min(0f)]
	[SerializeField]
	private float m_SpringDamping;

	[SerializeField]
	private bool m_EnableMotor;

	[SerializeField]
	private float m_MotorSpeed;

	[Min(0f)]
	[SerializeField]
	private float m_MaxMotorTorque;

	[SerializeField]
	private bool m_EnableLimit;

	[Range(-178f, 178f)]
	[SerializeField]
	private float m_LowerAngleLimit;

	[Range(-178f, 178f)]
	[SerializeField]
	private float m_UpperAngleLimit;

	[SerializeField]
	[Min(0f)]
	private float m_ForceThreshold;

	[Min(0f)]
	[SerializeField]
	private float m_TorqueThreshold;

	[Range(0f, 1000f)]
	[SerializeField]
	private float m_TuningFrequency;

	[Range(0f, 10f)]
	[SerializeField]
	private float m_TuningDamping;

	[SerializeField]
	[Range(0.001f, 10f)]
	private float m_DrawScale;

	[SerializeField]
	private bool m_CollideConnected;

	public static PhysicsHingeJointDefinition defaultDefinition => PhysicsLowLevelScripting2D.HingeJoint_GetDefaultDefinition(useSettings: true);

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

	public float springTargetAngle
	{
		readonly get
		{
			return m_SpringTargetAngle;
		}
		set
		{
			m_SpringTargetAngle = value;
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

	public float maxMotorTorque
	{
		readonly get
		{
			return m_MaxMotorTorque;
		}
		set
		{
			m_MaxMotorTorque = Mathf.Max(0f, value);
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

	public float lowerAngleLimit
	{
		readonly get
		{
			return m_LowerAngleLimit;
		}
		set
		{
			m_LowerAngleLimit = Mathf.Clamp(value, -178f, 178f);
		}
	}

	public float upperAngleLimit
	{
		readonly get
		{
			return m_UpperAngleLimit;
		}
		set
		{
			m_UpperAngleLimit = Mathf.Clamp(value, -178f, 178f);
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

	public PhysicsHingeJointDefinition()
	{
		this = defaultDefinition;
	}

	public PhysicsHingeJointDefinition(bool useSettings)
	{
		this = PhysicsLowLevelScripting2D.HingeJoint_GetDefaultDefinition(useSettings);
	}
}
