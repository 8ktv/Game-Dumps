using System;

namespace UnityEngine.LowLevelPhysics;

public struct TerrainGeometry : IGeometry
{
	private IntPtr m_TerrainData;

	private float m_HeightScale;

	private float m_RowScale;

	private float m_ColumnScale;

	private byte m_TerrainFlags;

	private byte pad1;

	private short pad2;

	private uint pad3;

	public GeometryType GeometryType => GeometryType.Terrain;
}
