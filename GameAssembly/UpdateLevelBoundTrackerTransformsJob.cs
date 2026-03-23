using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

[BurstCompile]
public struct UpdateLevelBoundTrackerTransformsJob : IJobParallelForTransform
{
	[NativeDisableParallelForRestriction]
	public NativeList<BoundsManager.BoundsTrackerInstance> levelBoundsTrackers;

	public void Execute(int transformIndex, TransformAccess transform)
	{
		if (transform.isValid)
		{
			BoundsManager.BoundsTrackerInstance value = levelBoundsTrackers[transformIndex];
			float3 xyz = math.mul(transform.localToWorldMatrix, new float4(value.outOfBoundsHazardSubmersionLocalPosition, 1f)).xyz;
			xyz.y += value.outOfBoundsHazardSubmersionWorldVerticalOffset;
			value.worldPosition = transform.position;
			value.outOfBoundsHazardSubmersionWorldPosition = xyz;
			levelBoundsTrackers[transformIndex] = value;
		}
	}
}
