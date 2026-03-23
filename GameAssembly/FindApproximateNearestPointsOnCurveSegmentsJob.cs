using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Splines;

[BurstCompile]
public struct FindApproximateNearestPointsOnCurveSegmentsJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<BezierCurve> curves;

	[ReadOnly]
	public NativeReference<int> nearestCurveIndex;

	[WriteOnly]
	public NativeArray<BoundsManager.NearestPointOnCurve> nearestPointsPerSegment;

	public float approximateSpatialResolution;

	public int maxSubdivisionCount;

	public int segmentCount;

	public float3 point;

	public void Execute(int segmentIndex)
	{
		BezierCurve curve = curves[nearestCurveIndex.Value];
		float num = 1f / (float)segmentCount;
		float num2 = (float)segmentIndex * num;
		float num3 = num2 + num;
		float num4 = curve.GetBoundingLength() / (float)segmentCount;
		int num5 = math.min(maxSubdivisionCount, (int)math.round(num4 / approximateSpatialResolution));
		int num6 = num5 + 1;
		float num7 = (num3 - num2) / (float)num5;
		BoundsManager.NearestPointOnCurve value = new BoundsManager.NearestPointOnCurve
		{
			distance = float.MaxValue
		};
		float num8 = num2;
		for (int i = 0; i < num6; i++)
		{
			float3 @float = CurveUtility.EvaluatePosition(curve, num8);
			float num9 = math.lengthsq(@float - point);
			if (num9 < value.distance)
			{
				value = new BoundsManager.NearestPointOnCurve
				{
					point = @float,
					distance = num9,
					t = num8
				};
			}
			num8 += num7;
		}
		value.distance = math.sqrt(value.distance);
		nearestPointsPerSegment[segmentIndex] = value;
	}
}
