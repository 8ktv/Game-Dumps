using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrain settings", menuName = "Settings/Gameplay/Terrain")]
public class TerrainSettings : ScriptableObject
{
	public readonly Dictionary<TerrainLayer, TerrainLayerSettings> LayerSettings = new Dictionary<TerrainLayer, TerrainLayerSettings>();

	public readonly Dictionary<OutOfBoundsHazard, OutOfBoundsHazardSettings> OutOfBoundsHazardSettings = new Dictionary<OutOfBoundsHazard, OutOfBoundsHazardSettings>();

	public readonly Dictionary<UnityEngine.TerrainLayer, TerrainLayer> LayersPerLayerAsset = new Dictionary<UnityEngine.TerrainLayer, TerrainLayer>();

	private static NativeHashMap<int, TerrainLayerSettings.JobsPhysicsData> jobsBallTerrainCollisionMaterialPerTerrainLayer;

	[field: SerializeField]
	[field: DynamicElementName("Layer")]
	public TerrainLayerSettings[] Layers { get; private set; }

	[field: SerializeField]
	[field: DynamicElementName("Hazard")]
	public OutOfBoundsHazardSettings[] OutOfBoundsHazards { get; private set; }

	public static NativeHashMap<int, TerrainLayerSettings.JobsPhysicsData> JobsBallTerrainCollisionMaterialPerTerrainLayer => jobsBallTerrainCollisionMaterialPerTerrainLayer;

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	public void OnApplicationQuit()
	{
		if (jobsBallTerrainCollisionMaterialPerTerrainLayer.IsCreated)
		{
			jobsBallTerrainCollisionMaterialPerTerrainLayer.Dispose();
		}
	}

	private void Initialize()
	{
		if (jobsBallTerrainCollisionMaterialPerTerrainLayer.IsCreated)
		{
			jobsBallTerrainCollisionMaterialPerTerrainLayer.Dispose();
		}
		jobsBallTerrainCollisionMaterialPerTerrainLayer = new NativeHashMap<int, TerrainLayerSettings.JobsPhysicsData>(Layers.Length, Allocator.Persistent);
		LayerSettings.Clear();
		LayersPerLayerAsset.Clear();
		for (int i = 0; i < Layers.Length; i++)
		{
			TerrainLayerSettings terrainLayerSettings = Layers[i];
			LayerSettings.Add(terrainLayerSettings.Layer, terrainLayerSettings);
			LayersPerLayerAsset.Add(terrainLayerSettings.LayerAsset, terrainLayerSettings.Layer);
			if (jobsBallTerrainCollisionMaterialPerTerrainLayer.IsCreated)
			{
				jobsBallTerrainCollisionMaterialPerTerrainLayer.Add((int)terrainLayerSettings.Layer, new TerrainLayerSettings.JobsPhysicsData(terrainLayerSettings));
			}
		}
		OutOfBoundsHazardSettings.Clear();
		for (int j = 0; j < OutOfBoundsHazards.Length; j++)
		{
			OutOfBoundsHazardSettings outOfBoundsHazardSettings = OutOfBoundsHazards[j];
			OutOfBoundsHazardSettings.Add(outOfBoundsHazardSettings.Type, outOfBoundsHazardSettings);
		}
	}
}
