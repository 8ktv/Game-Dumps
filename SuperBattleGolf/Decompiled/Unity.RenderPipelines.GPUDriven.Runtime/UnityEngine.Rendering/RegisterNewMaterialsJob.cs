using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering;

[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
internal struct RegisterNewMaterialsJob : IJobParallelFor
{
	public const int k_BatchSize = 128;

	[ReadOnly]
	public NativeArray<EntityId> instanceIDs;

	[ReadOnly]
	public NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas;

	[ReadOnly]
	public NativeArray<BatchMaterialID> batchIDs;

	[WriteOnly]
	public NativeParallelHashMap<EntityId, BatchMaterialID>.ParallelWriter batchMaterialHashMap;

	[WriteOnly]
	public NativeParallelHashMap<EntityId, GPUDrivenPackedMaterialData>.ParallelWriter packedMaterialHashMap;

	public void Execute(int index)
	{
		EntityId key = instanceIDs[index];
		batchMaterialHashMap.TryAdd(key, batchIDs[index]);
		packedMaterialHashMap.TryAdd(key, packedMaterialDatas[index]);
	}
}
