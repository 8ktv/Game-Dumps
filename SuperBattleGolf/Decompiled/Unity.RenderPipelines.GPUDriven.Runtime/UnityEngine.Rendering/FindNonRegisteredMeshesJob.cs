using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering;

[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
internal struct FindNonRegisteredMeshesJob : IJobParallelForBatch
{
	public const int k_BatchSize = 128;

	[ReadOnly]
	public NativeArray<EntityId> instanceIDs;

	[ReadOnly]
	public NativeParallelHashMap<EntityId, BatchMeshID> hashMap;

	[WriteOnly]
	public NativeList<EntityId>.ParallelWriter outInstancesWriter;

	public unsafe void Execute(int startIndex, int count)
	{
		EntityId* ptr = stackalloc EntityId[128];
		UnsafeList<EntityId> unsafeList = new UnsafeList<EntityId>(ptr, 128);
		unsafeList.Length = 0;
		for (int i = startIndex; i < startIndex + count; i++)
		{
			EntityId entityId = instanceIDs[i];
			if (!hashMap.ContainsKey(entityId))
			{
				unsafeList.AddNoResize(entityId);
			}
		}
		outInstancesWriter.AddRangeNoResize(ptr, unsafeList.Length);
	}
}
