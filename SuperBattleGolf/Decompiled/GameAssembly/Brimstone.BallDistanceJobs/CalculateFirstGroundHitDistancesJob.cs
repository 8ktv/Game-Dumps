using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Brimstone.BallDistanceJobs;

[BurstCompile]
public struct CalculateFirstGroundHitDistancesJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<float> normalizedInitialSpeeds;

	[ReadOnly]
	public NativeHashMap<int2, TerrainManager.JobsTerrain> spatiallyHashedTerrains;

	[ReadOnly]
	public NativeHashMap<int, int> globalTerrainLayerIndicesPerLevelTerrainLayer;

	[ReadOnly]
	public NativeArray<float> allTerrainLayerWeights;

	[ReadOnly]
	public NativeArray<float> allTerrainHeights;

	[ReadOnly]
	public NativeList<BoundsManager.SecondaryOutOfBoundsHazardInstance> secondaryOutOfBoundsHazards;

	[WriteOnly]
	public NativeArray<PlayerGolfer.SwingDistanceEstimation> estimatedDistances;

	public float mainOutOfBoundsHazardHeight;

	public OutOfBoundsHazard mainOutOfBoundsHazardType;

	public float2 terrainSize;

	public float2 initialWorldPosition2d;

	public float yawRad;

	public float fullInitialSpeed;

	public float verticalGravity;

	public float pitchRad;

	public float airDragCoefficient;

	public float deltaTime;

	public void Execute(int initialSpeedIndex)
	{
		float num = normalizedInitialSpeeds[initialSpeedIndex] * fullInitialSpeed;
		float2 @float = new float2(0f, verticalGravity);
		float2 float2 = Vector2.zero;
		float2 zero = float2.zero;
		float2 float3 = new float2(math.cos(pitchRad), math.sin(pitchRad)) * num;
		while (zero.y >= 0f)
		{
			float3 += @float * deltaTime;
			float3 *= math.max(0f, 1f - airDragCoefficient * math.lengthsq(float3) * deltaTime);
			float2 = zero;
			zero += float3 * deltaTime;
		}
		float t = BMath.InverseLerp(float2.y, zero.y, 0f);
		float distance = math.lerp(float2.x, zero.x, t);
		math.sincos(yawRad, out var s, out var c);
		float2 worldPoint2d = initialWorldPosition2d + zero.x * new float2(s, c);
		int2 spatialHash = TerrainManager.GetSpatialHash(worldPoint2d, terrainSize);
		TerrainLayer layer = (TerrainLayer)(-1);
		OutOfBoundsHazard outOfBoundsHazard = (OutOfBoundsHazard)(-1);
		if (spatiallyHashedTerrains.TryGetValue(spatialHash, out var item))
		{
			float heightAt = item.GetHeightAt(worldPoint2d, allTerrainHeights);
			BoundsJobHelper.IsInOutOfBoundsHazard(new float3(worldPoint2d.x, heightAt, worldPoint2d.y), secondaryOutOfBoundsHazards, mainOutOfBoundsHazardHeight, mainOutOfBoundsHazardType, out var _, out var hazardType, out var _);
			if (hazardType >= OutOfBoundsHazard.Water)
			{
				outOfBoundsHazard = hazardType;
			}
			else
			{
				int dominantLayerIndexAt = item.GetDominantLayerIndexAt(worldPoint2d, allTerrainLayerWeights);
				if (globalTerrainLayerIndicesPerLevelTerrainLayer.TryGetValue(dominantLayerIndexAt, out var item2))
				{
					layer = (TerrainLayer)item2;
				}
				else
				{
					Debug.LogError($"Level terrain layer {dominantLayerIndexAt} cannot be mapped to a global terrain layer");
				}
			}
		}
		estimatedDistances[initialSpeedIndex] = new PlayerGolfer.SwingDistanceEstimation(distance, layer, outOfBoundsHazard);
	}
}
