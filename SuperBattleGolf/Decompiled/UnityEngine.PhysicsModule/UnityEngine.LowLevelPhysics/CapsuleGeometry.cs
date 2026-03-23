namespace UnityEngine.LowLevelPhysics;

public struct CapsuleGeometry : IGeometry
{
	private float m_Radius;

	private float m_HalfLength;

	public float Radius
	{
		get
		{
			return m_Radius;
		}
		set
		{
			m_Radius = value;
		}
	}

	public float HalfLength
	{
		get
		{
			return m_HalfLength;
		}
		set
		{
			m_HalfLength = value;
		}
	}

	public GeometryType GeometryType => GeometryType.Capsule;

	public CapsuleGeometry(float radius, float halfLength)
	{
		m_Radius = radius;
		m_HalfLength = halfLength;
	}
}
