using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
internal struct GPUDrivenPackedMaterialData
{
	private uint data;

	public bool isTransparent => (data & 1) != 0;

	public bool isMotionVectorsPassEnabled => (data & 2) != 0;

	public bool isIndirectSupported => (data & 4) != 0;

	public bool supportsCrossFade => (data & 8) != 0;

	public GPUDrivenPackedMaterialData()
	{
		data = 0u;
	}

	public GPUDrivenPackedMaterialData(bool isTransparent, bool isMotionVectorsPassEnabled, bool isIndirectSupported, bool supportsCrossFade)
	{
		data = (isTransparent ? 1u : 0u);
		data |= (uint)(isMotionVectorsPassEnabled ? 2 : 0);
		data |= (uint)(isIndirectSupported ? 4 : 0);
		data |= (uint)(supportsCrossFade ? 8 : 0);
	}

	public bool Equals(GPUDrivenPackedMaterialData other)
	{
		return (other.data & 7) == (data & 7);
	}
}
