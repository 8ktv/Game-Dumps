namespace UnityEngine.LowLevelPhysics;

public struct ImmediateContact
{
	private Vector3 m_Normal;

	private float m_Separation;

	private Vector3 m_Point;

	private float m_MaxImpulse;

	private Vector3 m_TargetVel;

	private float m_StaticFriction;

	private byte m_MaterialFlags;

	private byte m_Pad;

	private ushort m_InternalUse;

	private uint m_InternalFaceIndex1;

	private float m_DynamicFriction;

	private float m_Restitution;

	public Vector3 Normal
	{
		get
		{
			return m_Normal;
		}
		set
		{
			m_Normal = value;
		}
	}

	public float Separation
	{
		get
		{
			return m_Separation;
		}
		set
		{
			m_Separation = value;
		}
	}

	public Vector3 Point
	{
		get
		{
			return m_Point;
		}
		set
		{
			m_Point = value;
		}
	}
}
