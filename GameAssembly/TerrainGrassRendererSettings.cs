using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainGrassRendererSettings", menuName = "Scriptable Objects/TerrainGrassRendererSettings")]
public class TerrainGrassRendererSettings : ScriptableObject
{
	[Serializable]
	public class LodData
	{
		public int bladeSegments = 3;

		public int bladeDensity = 12;

		public float thicknessScale = 1f;

		public float nearDist;

		public float farDist;
	}

	public LodData[] lodData;

	public float tileSize = 4f;

	public float bladeThickness = 0.1f;

	public float bladeHeight = 0.5f;

	public float bladeDroop = 0.25f;

	public AnimationCurve grassShape;

	public AnimationCurve quadSpacing;

	public Material grassMaterial;

	public Vector4 grassHeights;
}
