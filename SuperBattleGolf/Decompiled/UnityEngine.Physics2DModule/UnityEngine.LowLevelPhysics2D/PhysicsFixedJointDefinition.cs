using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsFixedJointDefinition
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
	[Min(0f)]
	private float m_LinearFrequency;

	[Min(0f)]
	[SerializeField]
	private float m_LinearDamping;

	[SerializeField]
	[Min(0f)]
	private float m_AngularFrequency;

	[SerializeField]
	[Min(0f)]
	private float m_AngularDamping;

	[SerializeField]
	[Min(0f)]
	private float m_ForceThreshold;

	[SerializeField]
	[Min(0f)]
	private float m_TorqueThreshold;

	[SerializeField]
	[Range(0f, 1000f)]
	private float m_TuningFrequency;

	[SerializeField]
	[Range(0f, 10f)]
	private float m_TuningDamping;

	[Range(0.001f, 10f)]
	[SerializeField]
	private float m_DrawScale;

	[SerializeField]
	private bool m_CollideConnected;

	public static PhysicsFixedJointDefinition defaultDefinition => PhysicsLowLevelScripting2D.FixedJoint_GetDefaultDefinition(useSettings: true);

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

	public float linearFrequency
	{
		readonly get
		{
			return m_LinearFrequency;
		}
		set
		{
			m_LinearFrequency = Mathf.Max(0f, value);
		}
	}

	public float linearDamping
	{
		readonly get
		{
			return m_LinearDamping;
		}
		set
		{
			m_LinearDamping = Mathf.Max(0f, value);
		}
	}

	public float angularFrequency
	{
		readonly get
		{
			return m_AngularFrequency;
		}
		set
		{
			m_AngularFrequency = Mathf.Max(0f, value);
		}
	}

	public float angularDamping
	{
		readonly get
		{
			return m_AngularDamping;
		}
		set
		{
			m_AngularDamping = Mathf.Max(0f, value);
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

	public PhysicsFixedJointDefinition()
	{
		this = defaultDefinition;
	}

	public PhysicsFixedJointDefinition(bool useSettings)
	{
		this = PhysicsLowLevelScripting2D.FixedJoint_GetDefaultDefinition(useSettings);
	}
}
