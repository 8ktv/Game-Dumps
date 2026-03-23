using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[BurstCompile]
public struct GetBezierWindingAnglesJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<BezierCurve> curves;

	[ReadOnly]
	public NativeList<BoundsManager.BoundsTrackerInstance> boundsTrackers;

	[NativeDisableParallelForRestriction]
	public NativeList<float> windingAnglesRad;

	public void Execute(int index)
	{
		int num = index / curves.Length;
		if (num < 0 || num >= boundsTrackers.Length)
		{
			Debug.LogError($"Tracker index {num} outside of range of {boundsTrackers.Length} registered trackers");
			return;
		}
		int num2 = index % curves.Length;
		if (num2 < 0 || num2 >= curves.Length)
		{
			Debug.LogError($"Curve index {num2} outside of range of {curves.Length} registered curves");
			return;
		}
		int num3 = num * curves.Length + num2;
		if (num3 < 0 || num3 >= windingAnglesRad.Length)
		{
			Debug.LogError($"Winding angle index {num3} outside of range of {windingAnglesRad.Length} winding angles (tracker index {num} of {boundsTrackers.Length} trackers, curve index {num2} of {curves.Length} curves)");
		}
		else
		{
			float3 worldPosition = boundsTrackers[num].worldPosition;
			float bezierTotalWindingAngleRad = BoundsJobHelper.GetBezierTotalWindingAngleRad(curves[num2], worldPosition);
			windingAnglesRad[num3] = bezierTotalWindingAngleRad;
		}
	}
}
