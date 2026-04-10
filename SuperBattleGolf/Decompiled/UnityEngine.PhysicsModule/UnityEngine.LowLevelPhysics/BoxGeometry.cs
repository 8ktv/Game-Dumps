using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BoxGeometry : IGeometry
{
	private int m_UnusedReserved;

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
		m_UnusedReserved = -1;
		m_HalfExtents = halfExtents;
	}
}
