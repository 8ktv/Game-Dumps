using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;

[BurstCompile]
public struct UpdateLandmineTransformsJob : IJobParallelForTransform
{
	[NativeDisableParallelForRestriction]
	public NativeList<LandmineManager.LandmineInstance> landmines;

	public void Execute(int transformIndex, TransformAccess transform)
	{
		if (transform.isValid)
		{
			LandmineManager.LandmineInstance value = landmines[transformIndex];
			value.worldPosition = transform.position;
			landmines[transformIndex] = value;
		}
	}
}
