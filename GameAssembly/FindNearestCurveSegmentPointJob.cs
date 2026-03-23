using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct FindNearestCurveSegmentPointJob : IJob
{
	public NativeArray<BoundsManager.NearestPointOnCurve> nearestPointsPerSegment;

	public void Execute()
	{
		BoundsManager.NearestPointOnCurve value = new BoundsManager.NearestPointOnCurve
		{
			distance = float.MaxValue
		};
		for (int i = 0; i < nearestPointsPerSegment.Length; i++)
		{
			BoundsManager.NearestPointOnCurve nearestPointOnCurve = nearestPointsPerSegment[i];
			if (nearestPointOnCurve.distance < value.distance)
			{
				value = nearestPointOnCurve;
			}
		}
		nearestPointsPerSegment[0] = value;
	}
}
