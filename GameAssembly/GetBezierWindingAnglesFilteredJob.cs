using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[BurstCompile]
public struct GetBezierWindingAnglesFilteredJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<BezierCurve> curves;

	[ReadOnly]
	public NativeList<BoundsManager.BoundsTrackerInstance> boundsTrackers;

	[ReadOnly]
	public NativeList<int> boundsTrackerIndices;

	[NativeDisableParallelForRestriction]
	public NativeList<float> windingAnglesRad;

	public void Execute(int index)
	{
		int num = index / curves.Length;
		if (num < 0 || num >= boundsTrackerIndices.Length)
		{
			Debug.LogError($"Tracker index index {num} outside of range of {boundsTrackerIndices.Length} registered tracker indices");
			return;
		}
		int num2 = boundsTrackerIndices[num];
		if (num2 < 0 || num2 >= boundsTrackers.Length)
		{
			Debug.LogError($"Tracker index {num2} outside of range of {boundsTrackers.Length} registered trackers");
			return;
		}
		BoundsManager.BoundsTrackerInstance boundsTrackerInstance = boundsTrackers[num2];
		if (!boundsTrackerInstance.levelBoundsTrackingType.HasType(LevelBoundsTrackingType.Bounds))
		{
			return;
		}
		int num3 = index % curves.Length;
		if (num3 < 0 || num3 >= curves.Length)
		{
			Debug.LogError($"Curve index {num3} outside of range of {curves.Length} registered curves");
			return;
		}
		int num4 = num * curves.Length + num3;
		if (num4 < 0 || num4 >= windingAnglesRad.Length)
		{
			Debug.LogError($"Winding angle index {num4} outside of range of {windingAnglesRad.Length} winding angles (tracker index index {num} of {boundsTrackerIndices.Length} tracker indices, curve index {num3} of {curves.Length} curves)");
		}
		else
		{
			float3 worldPosition = boundsTrackerInstance.worldPosition;
			float bezierTotalWindingAngleRad = BoundsJobHelper.GetBezierTotalWindingAngleRad(curves[num3], worldPosition);
			windingAnglesRad[num4] = bezierTotalWindingAngleRad;
		}
	}
}
