namespace UnityEngine.LowLevelPhysics;

public struct BoxGeometry : IGeometry
{
	private Vector3 m_HalfExtents;

	public Vector3 HalfExtents
	{
		get
		{
			return m_HalfExtents;
		}
		set
		{
			m_HalfExtents = value;
		}
	}

	public GeometryType GeometryType => GeometryType.Box;

	public BoxGeometry(Vector3 halfExtents)
	{
		m_HalfExtents = halfExtents;
	}
}
