using System;
using Unity.Collections;

namespace UnityEngine;

internal struct TransformDispatchData : IDisposable
{
	public NativeArray<EntityId> transformedID;

	public NativeArray<EntityId> parentID;

	public NativeArray<Matrix4x4> localToWorldMatrices;

	public NativeArray<Vector3> positions;

	public NativeArray<Quaternion> rotations;

	public NativeArray<Vector3> scales;

	public void Dispose()
	{
		transformedID.Dispose();
		parentID.Dispose();
		localToWorldMatrices.Dispose();
		positions.Dispose();
		rotations.Dispose();
		scales.Dispose();
	}
}
