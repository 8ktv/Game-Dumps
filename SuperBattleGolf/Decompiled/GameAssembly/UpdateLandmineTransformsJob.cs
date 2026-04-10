using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

[BurstCompile]
public struct UpdateLandmineTransformsJob : IJobParallelForTransform
{
	[NativeDisableParallelForRestriction]
	public NativeList<LandmineManager.LandmineInstance> landmines;

	[NativeDisableParallelForRestriction]
	public float3 localCenter;

	public void Execute(int transformIndex, TransformAccess transform)
	{
		if (transform.isValid)
		{
			LandmineManager.LandmineInstance value = landmines[transformIndex];
			value.worldPosition = math.mul(transform.localToWorldMatrix, new float4(localCenter, 1f)).xyz;
			landmines[transformIndex] = value;
		}
	}
}
