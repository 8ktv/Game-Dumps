using System;
using System.ComponentModel;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsBodyDefinition
{
	[SerializeField]
	private PhysicsBody.BodyType m_BodyType;

	[SerializeField]
	private PhysicsBody.BodyConstraints m_BodyConstraints;

	[SerializeField]
	private PhysicsBody.TransformWriteMode m_TransformWriteMode;

	[SerializeField]
	private Vector2 m_Position;

	[SerializeField]
	private PhysicsRotate m_Rotation;

	[SerializeField]
	private Vector2 m_LinearVelocity;

	[SerializeField]
	private float m_AngularVelocity;

	[SerializeField]
	[Min(0f)]
	private float m_LinearDamping;

	[SerializeField]
	[Min(0f)]
	private float m_AngularDamping;

	[SerializeField]
	private float m_GravityScale;

	[SerializeField]
	[Min(0f)]
	private float m_SleepThreshold;

	[SerializeField]
	private bool m_FastRotationAllowed;

	[SerializeField]
	private bool m_FastCollisionsAllowed;

	[SerializeField]
	private bool m_SleepingAllowed;

	[SerializeField]
	private bool m_Awake;

	[SerializeField]
	private bool m_Enabled;

	public static PhysicsBodyDefinition defaultDefinition => PhysicsLowLevelScripting2D.PhysicsBody_GetDefaultDefinition(useSettings: true);

	public PhysicsBody.BodyType type
	{
		readonly get
		{
			return m_BodyType;
		}
		set
		{
			m_BodyType = value;
		}
	}

	public PhysicsBody.BodyConstraints constraints
	{
		readonly get
		{
			return m_BodyConstraints;
		}
		set
		{
			m_BodyConstraints = value;
		}
	}

	public PhysicsBody.TransformWriteMode transformWriteMode
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

	public Vector2 position
	{
		readonly get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
		}
	}

	public PhysicsRotate rotation
	{
		readonly get
		{
			return m_Rotation;
		}
		set
		{
			m_Rotation = value;
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

	public float gravityScale
	{
		readonly get
		{
			return m_GravityScale;
		}
		set
		{
			m_GravityScale = value;
		}
	}

	public float sleepThreshold
	{
		readonly get
		{
			return m_SleepThreshold;
		}
		set
		{
			m_SleepThreshold = Mathf.Max(0f, value);
		}
	}

	public bool fastRotationAllowed
	{
		readonly get
		{
			return m_FastRotationAllowed;
		}
		set
		{
			m_FastRotationAllowed = value;
		}
	}

	public bool fastCollisionsAllowed
	{
		readonly get
		{
			return m_FastCollisionsAllowed;
		}
		set
		{
			m_FastCollisionsAllowed = value;
		}
	}

	public bool sleepingAllowed
	{
		readonly get
		{
			return m_SleepingAllowed;
		}
		set
		{
			m_SleepingAllowed = value;
		}
	}

	public bool awake
	{
		readonly get
		{
			return m_Awake;
		}
		set
		{
			m_Awake = value;
		}
	}

	public bool enabled
	{
		readonly get
		{
			return m_Enabled;
		}
		set
		{
			m_Enabled = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("PhysicsBodyDefinition.bodyType has been deprecated. Please use PhysicsBodyDefinition.type instead.", false)]
	public RigidbodyType2D bodyType
	{
		readonly get
		{
			return (RigidbodyType2D)m_BodyType;
		}
		set
		{
			m_BodyType = (PhysicsBody.BodyType)value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("PhysicsBodyDefinition.bodyConstraints has been deprecated. Please use PhysicsBodyDefinition.constraints instead.", false)]
	public RigidbodyConstraints2D bodyConstraints
	{
		readonly get
		{
			return (RigidbodyConstraints2D)m_BodyConstraints;
		}
		set
		{
			m_BodyConstraints = (PhysicsBody.BodyConstraints)value;
		}
	}

	public PhysicsBodyDefinition()
	{
		this = defaultDefinition;
	}

	public PhysicsBodyDefinition(bool useSettings)
	{
		this = PhysicsLowLevelScripting2D.PhysicsBody_GetDefaultDefinition(useSettings);
	}
}
