using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct SetUpLandmineOverlapChecksJob : IJobParallelFor
{
	[ReadOnly]
	public NativeList<LandmineManager.LandmineInstance> landmines;

	[NativeDisableParallelForRestriction]
	public NativeArray<OverlapSphereCommand> overlapSphereCommands;

	public float detectionRadius;

	public QueryParameters queryParameters;

	public void Execute(int landmineIndex)
	{
		if (landmineIndex < 0 || landmineIndex >= landmines.Length)
		{
			Debug.LogError($"Landmine index {landmineIndex} is out of range of {landmines.Length} landmines");
			return;
		}
		LandmineManager.LandmineInstance landmineInstance = landmines[landmineIndex];
		overlapSphereCommands[landmineIndex] = new OverlapSphereCommand(landmineInstance.worldPosition, detectionRadius, queryParameters);
	}
}
