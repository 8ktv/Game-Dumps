using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct UpdateOutOfBoundsHazardStateJob : IJobParallelFor
{
	[ReadOnly]
	public NativeList<BoundsManager.BoundsTrackerInstance> levelBoundsTrackers;

	[ReadOnly]
	public NativeList<BoundsManager.SecondaryOutOfBoundsHazardInstance> secondaryOutOfBoundsHazards;

	[NativeDisableParallelForRestriction]
	public NativeList<BoundsManager.TrackerOutOfBoundsHazardState> trackerOutOfBoundsHazardStates;

	public float mainOutOfBoundsHazardHeight;

	public void Execute(int trackerIndex)
	{
		BoundsManager.BoundsTrackerInstance boundsTrackerInstance = levelBoundsTrackers[trackerIndex];
		if (!boundsTrackerInstance.levelBoundsTrackingType.HasType(LevelBoundsTrackingType.OutOfBoundsHazards))
		{
			trackerOutOfBoundsHazardStates[trackerIndex] = new BoundsManager.TrackerOutOfBoundsHazardState(BoundsState.InBounds, mainOutOfBoundsHazardHeight, 0);
			return;
		}
		float secondaryHazardHeight;
		OutOfBoundsHazard hazardType;
		int secondaryHazardInstanceId;
		BoundsState hazardState = BoundsJobHelper.IsInOutOfBoundsHazard(boundsTrackerInstance.outOfBoundsHazardSubmersionWorldPosition, secondaryOutOfBoundsHazards, mainOutOfBoundsHazardHeight, OutOfBoundsHazard.Water, out secondaryHazardHeight, out hazardType, out secondaryHazardInstanceId);
		trackerOutOfBoundsHazardStates[trackerIndex] = new BoundsManager.TrackerOutOfBoundsHazardState(hazardState, secondaryHazardHeight, secondaryHazardInstanceId);
	}
}
