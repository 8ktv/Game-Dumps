using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct ProcessBoundsCheckResultsJob : IJob
{
	[ReadOnly]
	public NativeList<BoundsManager.BoundsTrackerInstance> levelBoundsTrackers;

	public NativeList<int> levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices;

	public NativeList<int> levelBoundsTrackerWithChangedStateIndices;

	public NativeList<int> levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices;

	public NativeList<int> greenBoundsTrackerWithChangedStateIndices;

	public void Execute()
	{
		for (int i = 0; i < levelBoundsTrackers.Length; i++)
		{
			BoundsManager.BoundsTrackerInstance boundsTrackerInstance = levelBoundsTrackers[i];
			if (boundsTrackerInstance.secondaryOutOfBoundsHazardInstanceIdChanged)
			{
				levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices.Add(in i);
			}
			if (boundsTrackerInstance.levelBoundsStateChanged)
			{
				levelBoundsTrackerWithChangedStateIndices.Add(in i);
			}
			if (boundsTrackerInstance.outOfBoundsHazardHeightChanged)
			{
				levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices.Add(in i);
			}
			if (boundsTrackerInstance.greenBoundsStateChanged)
			{
				greenBoundsTrackerWithChangedStateIndices.Add(in i);
			}
		}
	}
}
