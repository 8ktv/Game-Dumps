using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Brimstone.BallDistanceJobs;

[BurstCompile]
public struct ProcessDistanceEstimationsJob : IJob
{
	public NativeArray<float> normalizedInitialSpeeds;

	public NativeArray<PlayerGolfer.SwingDistanceEstimation> estimatedDistances;

	public NativeReference<float> requiredNormalizedSpeed;

	public NativeList<PlayerGolfer.TerrainLayerNormalizedSwingPower> terrainLayerNormalizedSwingPowers;

	public float desiredDistance;

	public void Execute()
	{
		requiredNormalizedSpeed.Value = -1f;
		bool flag = ShouldSearchForDesiredDistance();
		float num = estimatedDistances[0].distance;
		int index = 0;
		PlayerGolfer.TerrainLayerNormalizedSwingPower terrainLayerNormalizedSwingPower = new PlayerGolfer.TerrainLayerNormalizedSwingPower
		{
			layer = estimatedDistances[0].layer,
			levelHazard = estimatedDistances[0].levelHazard,
			outOfBoundsHazard = estimatedDistances[0].outOfBoundsHazard,
			startNormalizedPower = normalizedInitialSpeeds[0]
		};
		for (int i = 1; i < estimatedDistances.Length; i++)
		{
			PlayerGolfer.SwingDistanceEstimation swingDistanceEstimation = estimatedDistances[i];
			float distance = swingDistanceEstimation.distance;
			if (swingDistanceEstimation.layer != terrainLayerNormalizedSwingPower.layer || swingDistanceEstimation.outOfBoundsHazard != terrainLayerNormalizedSwingPower.outOfBoundsHazard || swingDistanceEstimation.levelHazard != terrainLayerNormalizedSwingPower.levelHazard)
			{
				float startNormalizedPower = (terrainLayerNormalizedSwingPower.endNormalizedPower = (normalizedInitialSpeeds[i - 1] + normalizedInitialSpeeds[i]) / 2f);
				AddOrSetTerrainLayerNormalizedSwingPower(index++, terrainLayerNormalizedSwingPower);
				terrainLayerNormalizedSwingPower = new PlayerGolfer.TerrainLayerNormalizedSwingPower
				{
					layer = swingDistanceEstimation.layer,
					levelHazard = swingDistanceEstimation.levelHazard,
					outOfBoundsHazard = swingDistanceEstimation.outOfBoundsHazard,
					startNormalizedPower = startNormalizedPower
				};
			}
			if (flag && requiredNormalizedSpeed.Value < 0f && distance >= desiredDistance)
			{
				float t = BMath.InverseLerp(num, distance, desiredDistance);
				float value = math.lerp(normalizedInitialSpeeds[i - 1], normalizedInitialSpeeds[i], t);
				requiredNormalizedSpeed.Value = value;
			}
			num = distance;
		}
		ref NativeArray<float> reference = ref normalizedInitialSpeeds;
		terrainLayerNormalizedSwingPower.endNormalizedPower = reference[reference.Length - 1];
		AddOrSetTerrainLayerNormalizedSwingPower(index++, terrainLayerNormalizedSwingPower);
		AddOrSetTerrainLayerNormalizedSwingPower(index, PlayerGolfer.TerrainLayerNormalizedSwingPower.Invalid);
	}

	private bool ShouldSearchForDesiredDistance()
	{
		if (desiredDistance >= 0f && estimatedDistances[0].distance <= desiredDistance)
		{
			float num = desiredDistance;
			ref NativeArray<PlayerGolfer.SwingDistanceEstimation> reference = ref estimatedDistances;
			return num <= reference[reference.Length - 1].distance;
		}
		return false;
	}

	private void AddOrSetTerrainLayerNormalizedSwingPower(int index, PlayerGolfer.TerrainLayerNormalizedSwingPower terrainLayerNormalizedSwingPower)
	{
		if (index >= terrainLayerNormalizedSwingPowers.Length)
		{
			terrainLayerNormalizedSwingPowers.Add(in terrainLayerNormalizedSwingPower);
		}
		else
		{
			terrainLayerNormalizedSwingPowers[index] = terrainLayerNormalizedSwingPower;
		}
	}
}
