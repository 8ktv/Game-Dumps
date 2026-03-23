using System;
using Unity.Collections;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
[UsedByNativeCode]
public struct BatchCullingContext
{
	public readonly NativeArray<Plane> cullingPlanes;

	public readonly NativeArray<CullingSplit> cullingSplits;

	public readonly LODParameters lodParameters;

	public readonly Matrix4x4 localToWorldMatrix;

	public readonly BatchCullingViewType viewType;

	public readonly BatchCullingProjectionType projectionType;

	public readonly BatchCullingFlags cullingFlags;

	public readonly BatchPackedCullingViewID viewID;

	public readonly uint cullingLayerMask;

	public readonly ulong sceneCullingMask;

	public readonly ushort splitExclusionMask;

	[Obsolete("BatchCullingContext.isOrthographic is deprecated. Use BatchCullingContext.projectionType instead.")]
	public readonly byte isOrthographic;

	public readonly int receiverPlaneOffset;

	public readonly int receiverPlaneCount;

	internal readonly IntPtr occlusionBuffer;

	internal BatchCullingContext(NativeArray<Plane> inCullingPlanes, NativeArray<CullingSplit> inCullingSplits, LODParameters inLodParameters, Matrix4x4 inLocalToWorldMatrix, BatchCullingViewType inViewType, BatchCullingProjectionType inProjectionType, BatchCullingFlags inBatchCullingFlags, ulong inViewID, uint inCullingLayerMask, ulong inSceneCullingMask, byte inExclusionSplitMask, int inReceiverPlaneOffset, int inReceiverPlaneCount, IntPtr inOcclusionBuffer)
	{
		cullingPlanes = inCullingPlanes;
		cullingSplits = inCullingSplits;
		lodParameters = inLodParameters;
		localToWorldMatrix = inLocalToWorldMatrix;
		viewType = inViewType;
		projectionType = inProjectionType;
		cullingFlags = inBatchCullingFlags;
		viewID = new BatchPackedCullingViewID
		{
			handle = inViewID
		};
		cullingLayerMask = inCullingLayerMask;
		sceneCullingMask = inSceneCullingMask;
		splitExclusionMask = inExclusionSplitMask;
		receiverPlaneOffset = inReceiverPlaneOffset;
		receiverPlaneCount = inReceiverPlaneCount;
		isOrthographic = 0;
		occlusionBuffer = inOcclusionBuffer;
	}
}
