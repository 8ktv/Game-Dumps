using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainManager : SingletonBehaviour<TerrainManager>, ISerializationCallbackReceiver
{
	public struct JobsTerrain
	{
		public float3 worldMinCorner;

		public float3 size;

		public int2 layerDataPointCount;

		public int2 heightDataPointCount;

		public int layerCount;

		public float baseHeight;

		public float heightScale;

		public int layerWeightStartIndex;

		public int heightStartIndex;

		public float2 worldMinCorner2d;

		public float2 size2d;

		public float2 layerDataPointDistance;

		public float2 heightDataPointDistance;

		public JobsTerrain(Terrain terrain, int layerWeightStartIndex, int heightStartIndex)
		{
			TerrainData terrainData = terrain.terrainData;
			worldMinCorner = terrain.transform.position;
			size = terrainData.bounds.size;
			layerDataPointCount = new int2(terrainData.alphamapWidth, terrainData.alphamapHeight);
			heightDataPointCount = new int2(terrainData.heightmapResolution, terrainData.heightmapResolution);
			layerCount = terrainData.alphamapLayers;
			baseHeight = terrain.transform.position.y;
			heightScale = terrainData.size.y;
			this.layerWeightStartIndex = layerWeightStartIndex;
			this.heightStartIndex = heightStartIndex;
			worldMinCorner2d = new float2(worldMinCorner.x, worldMinCorner.z);
			size2d = new float2(size.x, size.z);
			layerDataPointDistance = size2d / (layerDataPointCount - 1);
			heightDataPointDistance = size2d / (heightDataPointCount - 1);
		}

		public readonly int GetDominantLayerIndexAt(float2 worldPoint2d, NativeArray<float> allTerrainLayerWeights)
		{
			float2 x = WorldPointToEffectiveLocalPoint2d(worldPoint2d) / layerDataPointDistance;
			float2 @float = math.frac(x);
			int2 @int = ClampLayerWeightPoint((int2)math.floor(x));
			int2 layerDataPoint = ClampLayerWeightPoint(@int + new int2(0, 1));
			int2 layerDataPoint2 = ClampLayerWeightPoint(@int + new int2(1, 1));
			int2 layerDataPoint3 = ClampLayerWeightPoint(@int + new int2(1, 0));
			int layerWeightPointLinearStartIndex = GetLayerWeightPointLinearStartIndex(@int);
			int layerWeightPointLinearStartIndex2 = GetLayerWeightPointLinearStartIndex(layerDataPoint);
			int layerWeightPointLinearStartIndex3 = GetLayerWeightPointLinearStartIndex(layerDataPoint2);
			int layerWeightPointLinearStartIndex4 = GetLayerWeightPointLinearStartIndex(layerDataPoint3);
			NativeArray<float> nativeArray = new NativeArray<float>(layerCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			NativeArray<float> nativeArray2 = new NativeArray<float>(layerCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			BlendPointWeights(layerWeightPointLinearStartIndex, layerWeightPointLinearStartIndex4, @float.x, allTerrainLayerWeights, nativeArray);
			BlendPointWeights(layerWeightPointLinearStartIndex2, layerWeightPointLinearStartIndex3, @float.x, allTerrainLayerWeights, nativeArray2);
			BlendWeights(nativeArray, nativeArray2, @float.y);
			int result = 0;
			float num = float.MinValue;
			for (int i = 0; i < layerCount; i++)
			{
				float num2 = nativeArray[i];
				if (!(num2 < num))
				{
					result = i;
					num = num2;
				}
			}
			return result;
		}

		public readonly float GetHeightAt(float2 worldPoint2d, NativeArray<float> allTerrainHeights)
		{
			float2 x = WorldPointToEffectiveLocalPoint2d(worldPoint2d) / heightDataPointDistance;
			float2 @float = math.frac(x);
			int2 @int = ClampHeightPoint((int2)math.floor(x));
			int2 heightDataPoint = ClampHeightPoint(@int + new int2(0, 1));
			int2 heightDataPoint2 = ClampHeightPoint(@int + new int2(1, 1));
			int2 heightDataPoint3 = ClampHeightPoint(@int + new int2(1, 0));
			int heightPointLinearStartIndex = GetHeightPointLinearStartIndex(@int);
			int heightPointLinearStartIndex2 = GetHeightPointLinearStartIndex(heightDataPoint);
			int heightPointLinearStartIndex3 = GetHeightPointLinearStartIndex(heightDataPoint2);
			int heightPointLinearStartIndex4 = GetHeightPointLinearStartIndex(heightDataPoint3);
			float start = math.lerp(allTerrainHeights[heightPointLinearStartIndex], allTerrainHeights[heightPointLinearStartIndex4], @float.x);
			float end = math.lerp(allTerrainHeights[heightPointLinearStartIndex2], allTerrainHeights[heightPointLinearStartIndex3], @float.x);
			return baseHeight + math.lerp(start, end, @float.y) * heightScale;
		}

		private readonly float2 WorldPointToEffectiveLocalPoint2d(float2 worldPoint2d)
		{
			return ClampLocalPoint2d(worldPoint2d - worldMinCorner2d);
		}

		private readonly float2 ClampLocalPoint2d(float2 localPoint2d)
		{
			return math.clamp(localPoint2d, float2.zero, size2d);
		}

		private readonly int2 ClampLayerWeightPoint(int2 layerDataPoint)
		{
			return math.clamp(layerDataPoint, int2.zero, layerDataPointCount);
		}

		private readonly int2 ClampHeightPoint(int2 heightDataPoint)
		{
			return math.clamp(heightDataPoint, int2.zero, heightDataPointCount);
		}

		private readonly int GetLayerWeightPointLinearStartIndex(int2 layerDataPoint)
		{
			return layerWeightStartIndex + (layerDataPoint.y * layerDataPointCount.x + layerDataPoint.x) * layerCount;
		}

		private readonly int GetHeightPointLinearStartIndex(int2 heightDataPoint)
		{
			return heightStartIndex + heightDataPoint.y * heightDataPointCount.x + heightDataPoint.x;
		}

		private readonly void BlendPointWeights(int point0LinearStartIndex, int point1LinearStartIndex, float t, NativeArray<float> allTerrainLayerWeights, NativeArray<float> result)
		{
			for (int i = 0; i < layerCount; i++)
			{
				result[i] = math.lerp(allTerrainLayerWeights[point0LinearStartIndex + i], allTerrainLayerWeights[point1LinearStartIndex + i], t);
			}
		}

		private readonly void BlendWeights(NativeArray<float> weights0, NativeArray<float> weights1, float t)
		{
			for (int i = 0; i < layerCount; i++)
			{
				weights0[i] = math.lerp(weights0[i], weights1[i], t);
			}
		}
	}

	[SerializeField]
	private TerrainSettings settings;

	private Terrain[] terrains;

	private Vector2 terrainSize;

	private UnityEngine.TerrainLayer[] layersInLevel;

	private NativeHashMap<int2, JobsTerrain> spatiallyHashedJobsTerrains;

	private NativeArray<float> allTerrainLayerWeights;

	private NativeArray<float> allTerrainHeights;

	private NativeHashMap<int, int> globalTerrainLayerIndicesPerLevelTerrainLayer;

	[SerializeField]
	[HideInInspector]
	private byte[] serializedAllTerrainLayerWeights;

	[SerializeField]
	[HideInInspector]
	private byte[] serializedAllTerrainHeights;

	public static TerrainSettings Settings
	{
		get
		{
			if (!SingletonBehaviour<TerrainManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<TerrainManager>.Instance.settings;
		}
	}

	public static Terrain[] Terrains
	{
		get
		{
			if (!SingletonBehaviour<TerrainManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<TerrainManager>.Instance.terrains;
		}
	}

	public static Vector2 TerrainSize
	{
		get
		{
			if (!SingletonBehaviour<TerrainManager>.HasInstance)
			{
				return default(Vector2);
			}
			return SingletonBehaviour<TerrainManager>.Instance.terrainSize;
		}
	}

	public static NativeHashMap<int2, JobsTerrain> SpatiallyHashedJobsTerrains
	{
		get
		{
			if (!SingletonBehaviour<TerrainManager>.HasInstance)
			{
				return default(NativeHashMap<int2, JobsTerrain>);
			}
			return SingletonBehaviour<TerrainManager>.Instance.spatiallyHashedJobsTerrains;
		}
	}

	public static NativeArray<float> AllTerrainLayerWeights
	{
		get
		{
			if (!SingletonBehaviour<TerrainManager>.HasInstance)
			{
				return default(NativeArray<float>);
			}
			return SingletonBehaviour<TerrainManager>.Instance.allTerrainLayerWeights;
		}
	}

	public static NativeArray<float> AllTerrainHeights
	{
		get
		{
			if (!SingletonBehaviour<TerrainManager>.HasInstance)
			{
				return default(NativeArray<float>);
			}
			return SingletonBehaviour<TerrainManager>.Instance.allTerrainHeights;
		}
	}

	public static NativeHashMap<int, int> GlobalTerrainLayerIndicesPerLevelTerrainLayer
	{
		get
		{
			if (!SingletonBehaviour<TerrainManager>.HasInstance)
			{
				return default(NativeHashMap<int, int>);
			}
			return SingletonBehaviour<TerrainManager>.Instance.globalTerrainLayerIndicesPerLevelTerrainLayer;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public unsafe void OnAfterDeserialize()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		if (serializedAllTerrainLayerWeights == null || serializedAllTerrainLayerWeights.Length == 0)
		{
			UnityEngine.Debug.LogError("No terrain data was serialized; falling back to runtime generation");
			return;
		}
		int num = serializedAllTerrainLayerWeights.Length;
		int length = num / 4;
		allTerrainLayerWeights = new NativeArray<float>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		int num2 = serializedAllTerrainHeights.Length;
		int length2 = num2 / 4;
		allTerrainHeights = new NativeArray<float>(length2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		fixed (byte* source = serializedAllTerrainLayerWeights)
		{
			UnsafeUtility.MemCpy(allTerrainLayerWeights.GetUnsafePtr(), source, num);
		}
		fixed (byte* source2 = serializedAllTerrainHeights)
		{
			UnsafeUtility.MemCpy(allTerrainHeights.GetUnsafePtr(), source2, num2);
		}
		serializedAllTerrainLayerWeights = null;
		serializedAllTerrainHeights = null;
		stopwatch.Stop();
		UnityEngine.Debug.Log($"Loaded terrain data in {stopwatch.Elapsed.Milliseconds}ms ({BGui.BytesToSize(num)} layers, {BGui.BytesToSize(num2)} heights)");
	}

	protected override void Awake()
	{
		base.Awake();
		terrains = Object.FindObjectsByType<Terrain>(FindObjectsSortMode.None);
		TerrainData terrainData = terrains[0].terrainData;
		terrainSize = new Vector2(terrainData.bounds.size.x, terrainData.bounds.size.z);
		layersInLevel = terrainData.terrainLayers;
		spatiallyHashedJobsTerrains = new NativeHashMap<int2, JobsTerrain>(terrains.Length, Allocator.Persistent);
		globalTerrainLayerIndicesPerLevelTerrainLayer = new NativeHashMap<int, int>(layersInLevel.Length, Allocator.Persistent);
		GenerateJobsData(terrains, spatiallyHashTerrains: true, !allTerrainLayerWeights.IsCreated);
		for (int i = 0; i < layersInLevel.Length; i++)
		{
			globalTerrainLayerIndicesPerLevelTerrainLayer.Add(i, (int)settings.LayersPerLayerAsset[layersInLevel[i]]);
		}
	}

	protected override void OnDestroy()
	{
		if (spatiallyHashedJobsTerrains.IsCreated)
		{
			spatiallyHashedJobsTerrains.Dispose();
		}
		if (allTerrainLayerWeights.IsCreated)
		{
			allTerrainLayerWeights.Dispose();
		}
		if (allTerrainHeights.IsCreated)
		{
			allTerrainHeights.Dispose();
		}
		if (globalTerrainLayerIndicesPerLevelTerrainLayer.IsCreated)
		{
			globalTerrainLayerIndicesPerLevelTerrainLayer.Dispose();
		}
		base.OnDestroy();
	}

	private void OnApplicationQuit()
	{
		settings.OnApplicationQuit();
	}

	public static Terrain GetTerrainAtPoint(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<TerrainManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<TerrainManager>.Instance.GetTerrainAtPointInternal(worldPoint);
	}

	public static int GetDominantLevelLayerIndexAtPoint(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<TerrainManager>.HasInstance)
		{
			return 0;
		}
		return SingletonBehaviour<TerrainManager>.Instance.GetDominantLevelLayerIndexAtPointInternal(worldPoint);
	}

	public static TerrainLayer GetDominantGlobalLayerAtPoint(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<TerrainManager>.HasInstance)
		{
			return TerrainLayer.Fairway;
		}
		return SingletonBehaviour<TerrainManager>.Instance.GetDominantGlobalLayerAtPointInternal(worldPoint);
	}

	public static TerrainLayerSettings GetLevelLayerSettings(int levelLayerIndex)
	{
		if (!SingletonBehaviour<TerrainManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<TerrainManager>.Instance.GetLevelLayerSettingsInternal(levelLayerIndex);
	}

	public static TerrainLayerSettings GetDominantLayerSettingsAtPoint(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<TerrainManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<TerrainManager>.Instance.GetDominantLayerSettingsAtPointInternal(worldPoint);
	}

	public static TerrainLayer LevelLayerToGlobal(int levelLayerIndex)
	{
		if (!SingletonBehaviour<TerrainManager>.HasInstance)
		{
			return TerrainLayer.Fairway;
		}
		return SingletonBehaviour<TerrainManager>.Instance.LevelLayerToGlobalInternal(levelLayerIndex);
	}

	public static float GetWorldHeightAtPoint(Vector3 worldPoint)
	{
		if (!SingletonBehaviour<TerrainManager>.HasInstance)
		{
			return 0f;
		}
		return SingletonBehaviour<TerrainManager>.Instance.GetWorldHeightAtPointInternal(worldPoint);
	}

	private Terrain GetTerrainAtPointInternal(Vector3 worldPoint)
	{
		Terrain[] array = terrains;
		foreach (Terrain terrain in array)
		{
			if (!(terrain.terrainData == null))
			{
				Bounds bounds = terrain.terrainData.bounds;
				Bounds bounds2 = new Bounds(terrain.transform.TransformPoint(bounds.center), bounds.size);
				if (!(worldPoint.x < bounds2.min.x) && !(worldPoint.x > bounds2.max.x) && !(worldPoint.z < bounds2.min.z) && !(worldPoint.z > bounds2.max.z))
				{
					return terrain;
				}
			}
		}
		return null;
	}

	private int GetDominantLevelLayerIndexAtPointInternal(Vector3 worldPoint)
	{
		Vector2 vector = worldPoint.AsHorizontal2();
		int2 spatialHash = GetSpatialHash(vector, terrainSize);
		if (!spatiallyHashedJobsTerrains.TryGetValue(spatialHash, out var item))
		{
			UnityEngine.Debug.LogError($"Could not find terrain for world point {worldPoint}", base.gameObject);
			return -1;
		}
		return item.GetDominantLayerIndexAt(vector, allTerrainLayerWeights);
	}

	private TerrainLayer GetDominantGlobalLayerAtPointInternal(Vector3 worldPoint)
	{
		int dominantLevelLayerIndexAtPointInternal = GetDominantLevelLayerIndexAtPointInternal(worldPoint);
		return LevelLayerToGlobalInternal(dominantLevelLayerIndexAtPointInternal);
	}

	private TerrainLayerSettings GetLevelLayerSettingsInternal(int levelLayerIndex)
	{
		TerrainLayer key = LevelLayerToGlobal(levelLayerIndex);
		return Settings.LayerSettings[key];
	}

	private TerrainLayerSettings GetDominantLayerSettingsAtPointInternal(Vector3 worldPoint)
	{
		return GetLevelLayerSettingsInternal(GetDominantLevelLayerIndexAtPointInternal(worldPoint));
	}

	private TerrainLayer LevelLayerToGlobalInternal(int levelLayerIndex)
	{
		if (!globalTerrainLayerIndicesPerLevelTerrainLayer.TryGetValue(levelLayerIndex, out var item))
		{
			UnityEngine.Debug.LogError($"Could not convert level terrain layer {levelLayerIndex} to global layer", base.gameObject);
			return TerrainLayer.Fairway;
		}
		return (TerrainLayer)item;
	}

	private float GetWorldHeightAtPointInternal(Vector3 worldPoint)
	{
		Terrain terrainAtPointInternal = GetTerrainAtPointInternal(worldPoint);
		if (terrainAtPointInternal == null)
		{
			return 0f;
		}
		return terrainAtPointInternal.SampleHeight(worldPoint) + terrainAtPointInternal.transform.position.y;
	}

	private void GenerateJobsData(Terrain[] terrains, bool spatiallyHashTerrains, bool generateLayerData)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		TerrainData terrainData = terrains[0].terrainData;
		Vector2 vector = new Vector2(terrainData.bounds.size.x, terrainData.bounds.size.z);
		UnityEngine.TerrainLayer[] layersInLevel = terrainData.terrainLayers;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < terrains.Length; i++)
		{
			TerrainData terrainData2 = terrains[i].terrainData;
			num += terrainData2.alphamapWidth * terrainData2.alphamapHeight * terrainData2.alphamapLayers;
			num2 += terrainData2.heightmapResolution * terrainData2.heightmapResolution;
		}
		if (generateLayerData)
		{
			allTerrainLayerWeights = new NativeArray<float>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			allTerrainHeights = new NativeArray<float>(num2, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
		int layerWeightStartIndex = 0;
		int heightStartIndex = 0;
		foreach (Terrain terrain in terrains)
		{
			TerrainData terrainData3 = terrain.terrainData;
			if (!terrainData3.size.x.Approximately(vector.x) || !terrainData3.size.z.Approximately(vector.y))
			{
				UnityEngine.Debug.LogError("Terrains of different sizes detected");
				continue;
			}
			if (!AreLayersValid(terrainData3))
			{
				UnityEngine.Debug.LogError("Terrains using different layers detected");
				continue;
			}
			if (spatiallyHashTerrains)
			{
				int2 key = (int2)math.round(terrain.transform.position.AsHorizontal2() / vector);
				if (!spatiallyHashedJobsTerrains.TryAdd(key, new JobsTerrain(terrain, layerWeightStartIndex, heightStartIndex)))
				{
					UnityEngine.Debug.LogError("Terrain spatial hash collision detected");
					continue;
				}
			}
			if (!generateLayerData)
			{
				continue;
			}
			float[,,] alphamaps = terrainData3.GetAlphamaps(0, 0, terrainData3.alphamapWidth, terrainData3.alphamapHeight);
			for (int k = 0; k < terrainData3.alphamapWidth; k++)
			{
				for (int l = 0; l < terrainData3.alphamapHeight; l++)
				{
					for (int m = 0; m < terrainData3.alphamapLayers; m++)
					{
						allTerrainLayerWeights[layerWeightStartIndex++] = alphamaps[k, l, m];
					}
				}
			}
			float[,] heights = terrainData3.GetHeights(0, 0, terrainData3.heightmapResolution, terrainData3.heightmapResolution);
			for (int n = 0; n < terrainData3.heightmapResolution; n++)
			{
				for (int num3 = 0; num3 < terrainData3.heightmapResolution; num3++)
				{
					allTerrainHeights[heightStartIndex++] = heights[n, num3];
				}
			}
		}
		stopwatch.Stop();
		if (generateLayerData)
		{
			UnityEngine.Debug.Log($"Generated terrain data for scene {SceneManager.GetActiveScene().name} in {stopwatch.Elapsed.Milliseconds}ms ({BGui.BytesToSize(allTerrainLayerWeights.Length * 4)} layers, {BGui.BytesToSize(allTerrainHeights.Length * 4)} heights)");
		}
		else
		{
			UnityEngine.Debug.Log($"Generated terrain data for scene {SceneManager.GetActiveScene().name} in {stopwatch.Elapsed.Milliseconds}ms");
		}
		bool AreLayersValid(TerrainData terrainData4)
		{
			UnityEngine.TerrainLayer[] terrainLayers = terrainData4.terrainLayers;
			if (terrainLayers.Length != layersInLevel.Length)
			{
				return false;
			}
			for (int num4 = 0; num4 < terrainLayers.Length; num4++)
			{
				if (terrainLayers[num4] != layersInLevel[num4])
				{
					return false;
				}
			}
			return true;
		}
	}

	public static int2 GetSpatialHash(float2 worldPoint2d, float2 terrainSize)
	{
		return (int2)math.floor(worldPoint2d / terrainSize);
	}
}
