using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SphereGeometry : IGeometry
{
	private int m_UnusedReserved;

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
		m_UnusedReserved = -1;
		m_Radius = radius;
	}
}
