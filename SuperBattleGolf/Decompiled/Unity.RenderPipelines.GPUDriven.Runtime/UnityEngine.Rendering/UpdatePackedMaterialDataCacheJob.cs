using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering;

[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
internal struct UpdatePackedMaterialDataCacheJob : IJob
{
	[ReadOnly]
	public NativeArray<EntityId>.ReadOnly materialIDs;

	[ReadOnly]
	public NativeArray<GPUDrivenPackedMaterialData>.ReadOnly packedMaterialDatas;

	public NativeParallelHashMap<EntityId, GPUDrivenPackedMaterialData> packedMaterialHash;

	private void ProcessMaterial(int i)
	{
		EntityId entityId = materialIDs[i];
		GPUDrivenPackedMaterialData value = packedMaterialDatas[i];
		if (!(entityId == 0))
		{
			packedMaterialHash[entityId] = value;
		}
	}

	public void Execute()
	{
		for (int i = 0; i < materialIDs.Length; i++)
		{
			ProcessMaterial(i);
		}
	}
}
