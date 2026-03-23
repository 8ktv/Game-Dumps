using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
internal struct GPUDrivenMeshLodInfo
{
	public int levelCount;

	public float lodSlope;

	public float lodBias;

	public bool lodSelectionActive => levelCount > 1;
}
