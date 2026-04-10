using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsIgnoreJointDefinition
{
	[SerializeField]
	private PhysicsBody m_BodyA;

	[SerializeField]
	private PhysicsBody m_BodyB;

	public static PhysicsIgnoreJointDefinition defaultDefinition => PhysicsLowLevelScripting2D.IgnorePhysicsJoint_GetDefaultDefinition();

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

	public PhysicsIgnoreJointDefinition()
	{
		this = PhysicsLowLevelScripting2D.IgnorePhysicsJoint_GetDefaultDefinition();
	}
}
