using System;

namespace UnityEngine.LowLevelPhysics;

public struct ConvexMeshGeometry : IGeometry
{
	private int m_UnusedReserved;

	private Vector3 m_Scale;

	private Quaternion m_Rotation;

	private IntPtr m_ConvexMesh;

	private byte m_MeshFlags;

	private unsafe fixed byte m_MeshFlagsPadding[3];

	public Vector3 Scale
	{
		get
		{
			return m_Scale;
		}
		set
		{
			m_Scale = value;
		}
	}

	public Quaternion ScaleAxisRotation
	{
		get
		{
			return m_Rotation;
		}
		set
		{
			m_Rotation = value;
		}
	}

	public GeometryType GeometryType => GeometryType.ConvexMesh;
}
