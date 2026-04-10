using System;
using Unity.Collections;

namespace UnityEngine;

internal struct TypeDispatchData : IDisposable
{
	public Object[] changed;

	public NativeArray<EntityId> changedID;

	public NativeArray<EntityId> destroyedID;

	public void Dispose()
	{
		changed = null;
		changedID.Dispose();
		destroyedID.Dispose();
	}
}
