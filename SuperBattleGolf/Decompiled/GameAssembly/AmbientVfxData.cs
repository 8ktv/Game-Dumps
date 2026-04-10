using System;

[Serializable]
public class AmbientVfxData
{
	public AmbientVfxType vfxType;

	public bool hasSpawnChance;

	[DisplayIf("hasSpawnChance", true)]
	public float spawnChance = 0.5f;
}
