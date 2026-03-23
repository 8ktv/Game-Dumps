using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Splines;

[BurstCompile]
public struct GetBezierWindingAnglesForSinglePointJob : IJobParallelFor
{
	[ReadOnly]
	public NativeArray<BezierCurve> curves;

	[NativeDisableParallelForRestriction]
	public NativeArray<float> windingAnglesRad;

	public float3 point;

	public void Execute(int curveIndex)
	{
		NativeQueue<BezierCurve> nativeQueue = new NativeQueue<BezierCurve>(Allocator.Temp);
		nativeQueue.Enqueue(curves[curveIndex]);
		float num = 0f;
		int num2 = 0;
		BezierCurve item;
		while (nativeQueue.TryDequeue(out item))
		{
			if (BoundsJobHelper.DoesCurveRequiresSplittingForWindingAngle(point, item))
			{
				CurveUtility.Split(item, 0.5f, out var left, out var right);
				nativeQueue.Enqueue(left);
				nativeQueue.Enqueue(right);
				continue;
			}
			num += BoundsJobHelper.GetCurveWindingAngleRad(point, item.P0, item.P3);
			if (num2++ > 100)
			{
				break;
			}
		}
		windingAnglesRad[curveIndex] = num;
	}
}
