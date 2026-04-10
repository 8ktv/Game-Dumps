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

	[Serializable]
	public class FuzzySettings
	{
		[Range(0f, 1f)]
		public float fuzzyDirectDarken;

		[Range(0f, 1f)]
		public float fuzzyEdgeLighten = 1f;

		public float fuzzyPower = 1f;
	}

	[Serializable]
	public struct RimlightSettings
	{
		public Color rimlightColor;

		public float rimlightPower;
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

	[HideInInspector]
	public Vector4[] fuzzySettings;

	[HideInInspector]
	public Vector4[] rimlightSettings;

	[HideInInspector]
	public Vector4[] rimlightColors;

	[HideInInspector]
	public bool materialFuzzy;

	[HideInInspector]
	public bool materialRimlight;
}
