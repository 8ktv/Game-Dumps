using System;
using UnityEngine;

[Serializable]
public struct LevelHazardSettings
{
	public LevelHazardType type;

	public TerrainLayer effectiveTerrainLayer;

	public Texture swingPowerBarOverlayTexture;

	public Color swingPowerBarOverlayColor;
}
