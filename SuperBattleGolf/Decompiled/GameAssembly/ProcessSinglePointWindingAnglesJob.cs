using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct ProcessSinglePointWindingAnglesJob : IJob
{
	[ReadOnly]
	public NativeArray<float> windingAnglesRad;

	[WriteOnly]
	public NativeReference<bool> isPointInside;

	public void Execute()
	{
		float num = 0f;
		foreach (float item in windingAnglesRad)
		{
			num += item;
		}
		int num2 = (int)math.round(num / (MathF.PI * 2f));
		isPointInside.Value = num2 != 0;
	}
}
