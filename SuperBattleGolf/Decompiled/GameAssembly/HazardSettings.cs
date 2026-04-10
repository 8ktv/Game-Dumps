using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Hazard settings", menuName = "Settings/Gameplay/Hazards")]
public class HazardSettings : ScriptableObject
{
	public readonly Dictionary<LevelHazardType, LevelHazardSettings> levelHazardsByType = new Dictionary<LevelHazardType, LevelHazardSettings>();

	public NativeHashMap<int, int> globalTerrainLayerIndicesPerLevelHazard;

	[field: SerializeField]
	[field: DynamicElementName("type")]
	public LevelHazardSettings[] LevelHazardSettings { get; private set; }

	[field: SerializeField]
	public Texture SwingPowerBarOutOfBoundsHazardOVerlayTexture { get; private set; }

	[field: SerializeField]
	public Material BreakableIcePristineMaterial { get; private set; }

	[field: SerializeField]
	public Material BreakableIceCrackedMaterial { get; private set; }

	[field: SerializeField]
	public float BreakableIceHitBreakChance { get; private set; }

	[field: SerializeField]
	public float JumpPadCooldown { get; private set; }

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		levelHazardsByType.Clear();
		if (globalTerrainLayerIndicesPerLevelHazard.IsCreated)
		{
			globalTerrainLayerIndicesPerLevelHazard.Dispose();
		}
		globalTerrainLayerIndicesPerLevelHazard = new NativeHashMap<int, int>(LevelHazardSettings.Length, Allocator.Persistent);
		LevelHazardSettings[] levelHazardSettings = LevelHazardSettings;
		for (int i = 0; i < levelHazardSettings.Length; i++)
		{
			LevelHazardSettings value = levelHazardSettings[i];
			if (!levelHazardsByType.TryAdd(value.type, value))
			{
				Debug.LogError($"Duplicate level hazard of type {value.type} found");
			}
			globalTerrainLayerIndicesPerLevelHazard.Add((int)value.type, (int)value.effectiveTerrainLayer);
		}
	}
}
