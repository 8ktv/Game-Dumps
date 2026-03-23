using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct ProcessLevelBoundsUpdateResultsJob : IJobParallelFor
{
	[ReadOnly]
	public NativeList<int> levelBoundsWindingNumbers;

	[ReadOnly]
	public NativeList<BoundsManager.TrackerOutOfBoundsHazardState> trackerOutOfBoundsHazardStates;

	[NativeDisableParallelForRestriction]
	public NativeList<BoundsManager.BoundsTrackerInstance> levelBoundsTrackers;

	public int levelBoundsSplineCount;

	public void Execute(int trackerIndex)
	{
		BoundsManager.BoundsTrackerInstance tracker = levelBoundsTrackers[trackerIndex];
		bool num = tracker.levelBoundsTrackingType.HasType(LevelBoundsTrackingType.Bounds);
		bool flag = false;
		if (num)
		{
			for (int i = 0; i < levelBoundsSplineCount; i++)
			{
				if (levelBoundsWindingNumbers[trackerIndex * levelBoundsSplineCount + i] != 0)
				{
					flag = true;
					break;
				}
			}
		}
		BoundsState boundsState = tracker.boundsState;
		ApplyState(BoundsState.OutOfBounds, !flag);
		BoundsManager.TrackerOutOfBoundsHazardState trackerOutOfBoundsHazardState = trackerOutOfBoundsHazardStates[trackerIndex];
		ApplyState(BoundsState.InMainOutOfBoundsHazard, trackerOutOfBoundsHazardState.hazardState.HasState(BoundsState.InMainOutOfBoundsHazard));
		ApplyState(BoundsState.InSecondaryOutOfBoundsHazard, trackerOutOfBoundsHazardState.hazardState.HasState(BoundsState.InSecondaryOutOfBoundsHazard));
		ApplyState(BoundsState.OverSecondaryOutOfBoundsHazard, trackerOutOfBoundsHazardState.hazardState.HasState(BoundsState.OverSecondaryOutOfBoundsHazard));
		float outOfBoundsHazardHeight = tracker.outOfBoundsHazardHeight;
		tracker.outOfBoundsHazardHeight = trackerOutOfBoundsHazardState.hazardHeight;
		int secondaryOutOfBoundsHazardInstanceId = tracker.secondaryOutOfBoundsHazardInstanceId;
		tracker.secondaryOutOfBoundsHazardInstanceId = trackerOutOfBoundsHazardState.secondaryHazardInstanceId;
		tracker.secondaryOutOfBoundsHazardInstanceIdChanged = !tracker.isInitialized || tracker.secondaryOutOfBoundsHazardInstanceId != secondaryOutOfBoundsHazardInstanceId;
		tracker.levelBoundsStateChanged = !tracker.isInitialized || tracker.boundsState != boundsState;
		tracker.outOfBoundsHazardHeightChanged = !tracker.isInitialized || tracker.outOfBoundsHazardHeight != outOfBoundsHazardHeight;
		tracker.isInitialized = true;
		levelBoundsTrackers[trackerIndex] = tracker;
		void ApplyState(BoundsState state, bool enabled)
		{
			if (enabled)
			{
				tracker.boundsState |= state;
			}
			else
			{
				tracker.boundsState &= (BoundsState)(byte)(~(int)state);
			}
		}
	}
}
