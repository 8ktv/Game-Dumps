using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct ProcessGreenBoundsUpdateResultsJob : IJobParallelFor
{
	[ReadOnly]
	public NativeList<int> greenBoundsWindingNumbers;

	[ReadOnly]
	public NativeList<int> greenBoundsTrackerIndices;

	[NativeDisableParallelForRestriction]
	public NativeList<BoundsManager.BoundsTrackerInstance> levelBoundsTrackers;

	public int greenBoundsSplineCount;

	public void Execute(int trackerIndexIndex)
	{
		int num = greenBoundsTrackerIndices[trackerIndexIndex];
		if (num < 0 || num >= levelBoundsTrackers.Length)
		{
			Debug.LogError($"Tracker index {num} outside of range of {levelBoundsTrackers.Length} registered trackers");
			return;
		}
		BoundsManager.BoundsTrackerInstance value = levelBoundsTrackers[num];
		bool isOnGreen = false;
		for (int i = 0; i < greenBoundsSplineCount; i++)
		{
			if (greenBoundsWindingNumbers[trackerIndexIndex * greenBoundsSplineCount + i] != 0)
			{
				isOnGreen = true;
				break;
			}
		}
		bool isOnGreen2 = value.isOnGreen;
		value.isOnGreen = isOnGreen;
		value.greenBoundsStateChanged = !value.isInitialized || value.isOnGreen != isOnGreen2;
		levelBoundsTrackers[num] = value;
	}
}
