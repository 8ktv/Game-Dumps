namespace UnityEngine.Rendering;

public struct BatchCullingOutputDrawCommands
{
	public unsafe BatchDrawCommand* drawCommands;

	public unsafe BatchDrawCommandIndirect* indirectDrawCommands;

	public unsafe BatchDrawCommandProcedural* proceduralDrawCommands;

	public unsafe BatchDrawCommandProceduralIndirect* proceduralIndirectDrawCommands;

	public unsafe int* visibleInstances;

	public unsafe BatchDrawRange* drawRanges;

	public unsafe float* instanceSortingPositions;

	public unsafe int* drawCommandPickingInstanceIDs;

	public int drawCommandCount;

	public int indirectDrawCommandCount;

	public int proceduralDrawCommandCount;

	public int proceduralIndirectDrawCommandCount;

	public int visibleInstanceCount;

	public int drawRangeCount;

	public int instanceSortingPositionFloatCount;
}
