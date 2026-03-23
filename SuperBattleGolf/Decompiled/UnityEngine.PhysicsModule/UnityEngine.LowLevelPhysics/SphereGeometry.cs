namespace UnityEngine.LowLevelPhysics;

public struct SphereGeometry : IGeometry
{
	private float m_Radius;

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

	public GeometryType GeometryType => GeometryType.Sphere;

	public SphereGeometry(float radius)
	{
		m_Radius = radius;
	}
}
