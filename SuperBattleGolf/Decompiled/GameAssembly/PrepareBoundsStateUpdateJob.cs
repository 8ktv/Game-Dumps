using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct PrepareBoundsStateUpdateJob : IJob
{
	public NativeList<int> levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices;

	public NativeList<int> levelBoundsTrackerWithChangedStateIndices;

	public NativeList<int> levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices;

	public NativeList<int> greenBoundsTrackerWithChangedStateIndices;

	public void Execute()
	{
		levelBoundsTrackerWithChangedSecondaryOutOfBoundsHazardInstanceIdIndices.Clear();
		levelBoundsTrackerWithChangedStateIndices.Clear();
		levelBoundsTrackerWithChangedOutOfBoundsHazardHeightIndices.Clear();
		greenBoundsTrackerWithChangedStateIndices.Clear();
	}
}
