using System;

namespace UnityEngine.LowLevelPhysics;

public struct TerrainGeometry : IGeometry
{
	private int m_UnusedReserved;

	private IntPtr m_TerrainData;

	private float m_HeightScale;

	private float m_RowScale;

	private float m_ColumnScale;

	private byte m_TerrainFlags;

	private unsafe fixed byte m_TerrainFlagsPadding[3];

	public GeometryType GeometryType => GeometryType.Terrain;
}
