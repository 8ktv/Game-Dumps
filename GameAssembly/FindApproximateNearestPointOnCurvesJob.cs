using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Splines;

[BurstCompile]
public struct FindApproximateNearestPointOnCurvesJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<BezierCurve> curves;

	[WriteOnly]
	public NativeArray<BoundsManager.NearestPointOnCurve> nearestPointsPerCurve;

	public float approximateSpatialResolution;

	public int maxSubdivisionCount;

	public float3 point;

	public void Execute(int curveIndex)
	{
		BezierCurve curve = curves[curveIndex];
		float boundingLength = curve.GetBoundingLength();
		int num = math.min(maxSubdivisionCount, (int)math.round(boundingLength / approximateSpatialResolution));
		int num2 = num + 1;
		float num3 = 1f / (float)num;
		math.lengthsq(curve.P0 - point);
		math.lengthsq(curve.P3 - point);
		BoundsManager.NearestPointOnCurve value = new BoundsManager.NearestPointOnCurve
		{
			distance = float.MaxValue
		};
		float num4 = 0f;
		for (int i = 0; i < num2; i++)
		{
			float3 @float = CurveUtility.EvaluatePosition(curve, num4);
			float num5 = math.lengthsq(@float - point);
			if (num5 < value.distance)
			{
				value = new BoundsManager.NearestPointOnCurve
				{
					point = @float,
					distance = num5,
					t = num4
				};
			}
			num4 += num3;
		}
		value.distance = math.sqrt(value.distance);
		nearestPointsPerCurve[curveIndex] = value;
	}
}
