using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct FindNearestCurveJob : IJob
{
	public NativeArray<BoundsManager.NearestPointOnCurve> nearestPointsPerCurve;

	public NativeReference<int> nearestCurveIndex;

	public void Execute()
	{
		int value = -1;
		float num = float.MaxValue;
		for (int i = 0; i < nearestPointsPerCurve.Length; i++)
		{
			BoundsManager.NearestPointOnCurve nearestPointOnCurve = nearestPointsPerCurve[i];
			if (nearestPointOnCurve.distance < num)
			{
				value = i;
				num = nearestPointOnCurve.distance;
			}
		}
		nearestCurveIndex.Value = value;
	}
}
