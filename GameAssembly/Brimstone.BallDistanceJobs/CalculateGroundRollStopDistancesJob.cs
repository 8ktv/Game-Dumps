using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Brimstone.BallDistanceJobs;

[BurstCompile]
public struct CalculateGroundRollStopDistancesJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<float> normalizedInitialSpeeds;

	[ReadOnly]
	public NativeHashMap<int, int> globalTerrainLayerIndicesPerLevelTerrainLayer;

	[ReadOnly]
	public NativeHashMap<int, TerrainLayerSettings.JobsPhysicsData> ballTerrainPhysicsSettingsPerTerrainLayer;

	[ReadOnly]
	public NativeHashMap<int2, TerrainManager.JobsTerrain> spatiallyHashedTerrains;

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

	public float2 fullInitialSpeed;

	public float movementDirectionRightInitialAngularSpeed;

	public float2 initialWorldPosition2d;

	public float2 terrainSize;

	public float absoluteVerticalGravity;

	public float ballRadius;

	public float ballMass;

	public float ballMaxAngularSpeed;

	public float ballFullStopMinDampingSpeed;

	public float ballFullStopMaxDampingSpeed;

	public float ballFullStopLinearDamping;

	public float verticalGravityMagnitude;

	public float deltaTime;

	public void Execute(int initialSpeedIndex)
	{
		float num = 1f / ballMass;
		float num2 = 0.4f * ballMass * ballRadius * ballRadius;
		float num3 = 0.2857143f * ballMass;
		float num4 = ballMass * absoluteVerticalGravity * deltaTime;
		float2 @float = normalizedInitialSpeeds[initialSpeedIndex] * fullInitialSpeed;
		float2 float2 = initialWorldPosition2d;
		float2 float3 = @float;
		float num5 = math.min(movementDirectionRightInitialAngularSpeed, ballMaxAngularSpeed);
		TerrainLayer layer = (TerrainLayer)(-1);
		OutOfBoundsHazard outOfBoundsHazard = (OutOfBoundsHazard)(-1);
		while (math.lengthsq(float3) > 0.001f && math.dot(float3, @float) > 0f)
		{
			int2 spatialHash = TerrainManager.GetSpatialHash(float2, terrainSize);
			if (!spatiallyHashedTerrains.TryGetValue(spatialHash, out var item))
			{
				break;
			}
			float heightAt = item.GetHeightAt(float2, allTerrainHeights);
			BoundsJobHelper.IsInOutOfBoundsHazard(new float3(float2.x, heightAt, float2.y), secondaryOutOfBoundsHazards, mainOutOfBoundsHazardHeight, mainOutOfBoundsHazardType, out var _, out var hazardType, out var _);
			if (hazardType >= OutOfBoundsHazard.Water)
			{
				layer = (TerrainLayer)(-1);
				outOfBoundsHazard = hazardType;
				break;
			}
			int dominantLayerIndexAt = item.GetDominantLayerIndexAt(float2, allTerrainLayerWeights);
			if (!globalTerrainLayerIndicesPerLevelTerrainLayer.TryGetValue(dominantLayerIndexAt, out var item2))
			{
				Debug.LogError($"Level terrain layer {dominantLayerIndexAt} cannot be mapped to a global terrain layer");
				break;
			}
			layer = (TerrainLayer)item2;
			TerrainLayerSettings.JobsPhysicsData jobsPhysicsData = ballTerrainPhysicsSettingsPerTerrainLayer[item2];
			float num6 = math.length(float3);
			float num7 = num6 - num5 * ballRadius;
			if (math.abs(num7) > 1E-05f)
			{
				float num8 = jobsPhysicsData.dynamicFriction * num4;
				float num9 = math.clamp((0f - num3) * num7, 0f - num8, num8);
				float num10 = num9 * num;
				num6 += num10;
				float3 = math.normalizesafe(float3) * num6;
				float num11 = (0f - num9) * ballRadius / num2;
				num5 = math.min(num5 + num11, ballMaxAngularSpeed);
			}
			float num12 = jobsPhysicsData.linearDamping;
			if (num6 < ballFullStopMaxDampingSpeed)
			{
				num12 = ballFullStopLinearDamping;
			}
			else if (num6 < ballFullStopMinDampingSpeed)
			{
				float t = BMath.InverseLerpClamped(ballFullStopMinDampingSpeed, ballFullStopMaxDampingSpeed, num6);
				num12 = math.lerp(num12, ballFullStopLinearDamping, t);
			}
			float3 *= math.max(0f, 1f - num12 * deltaTime);
			float2 += float3 * deltaTime;
		}
		float distance = math.length(float2 - initialWorldPosition2d);
		estimatedDistances[initialSpeedIndex] = new PlayerGolfer.SwingDistanceEstimation(distance, layer, outOfBoundsHazard);
	}
}
