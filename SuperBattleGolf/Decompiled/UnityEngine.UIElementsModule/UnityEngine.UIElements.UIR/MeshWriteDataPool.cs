using System;

namespace UnityEngine.UIElements.UIR;

internal class MeshWriteDataPool : ImplicitPool<MeshWriteData>
{
	private static readonly Func<MeshWriteData> k_CreateAction = () => new MeshWriteData();

	public MeshWriteDataPool()
		: base(k_CreateAction, (Action<MeshWriteData>)null, 100, 1000)
	{
	}
}
