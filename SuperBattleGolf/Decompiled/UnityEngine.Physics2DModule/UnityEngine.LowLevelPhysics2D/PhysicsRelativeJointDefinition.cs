using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsRelativeJointDefinition
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
	private Vector2 m_LinearVelocity;

	[SerializeField]
	private float m_AngularVelocity;

	[SerializeField]
	[Min(0f)]
	private float m_MaxForce;

	[Min(0f)]
	[SerializeField]
	private float m_MaxTorque;

	[Min(0f)]
	[SerializeField]
	private float m_SpringLinearFrequency;

	[SerializeField]
	[Min(0f)]
	private float m_SpringAngularFrequency;

	[SerializeField]
	[Min(0f)]
	private float m_SpringLinearDamping;

	[Min(0f)]
	[SerializeField]
	private float m_SpringAngularDamping;

	[SerializeField]
	[Min(0f)]
	private float m_SpringMaxForce;

	[Min(0f)]
	[SerializeField]
	private float m_SpringMaxTorque;

	[Min(0f)]
	[SerializeField]
	private float m_ForceThreshold;

	[Min(0f)]
	[SerializeField]
	private float m_TorqueThreshold;

	[SerializeField]
	[Range(0f, 1000f)]
	private float m_TuningFrequency;

	[Range(0f, 10f)]
	[SerializeField]
	private float m_TuningDamping;

	[SerializeField]
	[Range(0.001f, 10f)]
	private float m_DrawScale;

	[SerializeField]
	private bool m_CollideConnected;

	public static PhysicsRelativeJointDefinition defaultDefinition => PhysicsLowLevelScripting2D.RelativeJoint_GetDefaultDefinition(useSettings: true);

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

	public float maxForce
	{
		readonly get
		{
			return m_MaxForce;
		}
		set
		{
			m_MaxForce = Mathf.Max(0f, value);
		}
	}

	public float maxTorque
	{
		readonly get
		{
			return m_MaxTorque;
		}
		set
		{
			m_MaxTorque = Mathf.Max(0f, value);
		}
	}

	public float springLinearFrequency
	{
		readonly get
		{
			return m_SpringLinearFrequency;
		}
		set
		{
			m_SpringLinearFrequency = Mathf.Max(0f, value);
		}
	}

	public float springAngularFrequency
	{
		readonly get
		{
			return m_SpringAngularFrequency;
		}
		set
		{
			m_SpringAngularFrequency = Mathf.Max(0f, value);
		}
	}

	public float springLinearDamping
	{
		readonly get
		{
			return m_SpringLinearDamping;
		}
		set
		{
			m_SpringLinearDamping = Mathf.Max(0f, value);
		}
	}

	public float springAngularDamping
	{
		readonly get
		{
			return m_SpringAngularDamping;
		}
		set
		{
			m_SpringAngularDamping = Mathf.Max(0f, value);
		}
	}

	public float springMaxForce
	{
		readonly get
		{
			return m_SpringMaxForce;
		}
		set
		{
			m_SpringMaxForce = Mathf.Max(0f, value);
		}
	}

	public float springMaxTorque
	{
		readonly get
		{
			return m_SpringMaxTorque;
		}
		set
		{
			m_SpringMaxTorque = Mathf.Max(0f, value);
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

	public PhysicsRelativeJointDefinition()
	{
		this = defaultDefinition;
	}

	public PhysicsRelativeJointDefinition(bool useSettings)
	{
		this = PhysicsLowLevelScripting2D.RelativeJoint_GetDefaultDefinition(useSettings);
	}
}
