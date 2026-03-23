using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct GetWindingNumbersFromAnglesJob : IJobParallelFor
{
	[ReadOnly]
	public NativeList<float> windingAnglesRad;

	[ReadOnly]
	public NativeArray<int> splineCurveStartIndices;

	[NativeDisableParallelForRestriction]
	public NativeList<int> windingNumbers;

	public int splineCount;

	public int curveCount;

	public void Execute(int index)
	{
		int num = index / splineCount;
		int num2 = index % splineCount;
		if (num2 < 0 || num2 >= splineCurveStartIndices.Length)
		{
			Debug.LogError($"Spline index {num2} outside of range of {splineCurveStartIndices.Length} registered splines");
			return;
		}
		int num3 = num * splineCount + num2;
		if (num3 < 0 || num3 >= windingNumbers.Length)
		{
			Debug.LogError($"Winding number index {num3} outside of range of {windingNumbers.Length} available winding numbers");
			return;
		}
		int num4 = num * curveCount;
		int num5 = num4 + splineCurveStartIndices[num2];
		int num6 = num4 + ((num2 >= splineCount - 1) ? curveCount : splineCurveStartIndices[num2 + 1]);
		float num7 = 0f;
		for (int i = num5; i < num6; i++)
		{
			num7 += windingAnglesRad[i];
		}
		windingNumbers[num3] = (int)math.round(num7 / (MathF.PI * 2f));
	}
}
