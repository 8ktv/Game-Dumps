using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
internal struct GPUDrivenPackedRendererData
{
	private uint data;

	public bool receiveShadows => (data & 1) != 0;

	public bool staticShadowCaster => (data & 2) != 0;

	public byte lodMask => (byte)((data >> 2) & 0xFF);

	public ShadowCastingMode shadowCastingMode => (ShadowCastingMode)((data >> 10) & 3);

	public LightProbeUsage lightProbeUsage => (LightProbeUsage)((data >> 12) & 7);

	public MotionVectorGenerationMode motionVecGenMode => (MotionVectorGenerationMode)((data >> 15) & 3);

	public bool isPartOfStaticBatch => (data & 0x20000) != 0;

	public bool movedCurrentFrame => (data & 0x40000) != 0;

	public bool hasTree => (data & 0x80000) != 0;

	public bool smallMeshCulling => (data & 0x100000) != 0;

	public bool supportsIndirect => (data & 0x200000) != 0;

	public GPUDrivenPackedRendererData()
	{
		data = 0u;
	}

	public GPUDrivenPackedRendererData(bool receiveShadows, bool staticShadowCaster, byte lodMask, ShadowCastingMode shadowCastingMode, LightProbeUsage lightProbeUsage, MotionVectorGenerationMode motionVecGenMode, bool isPartOfStaticBatch, bool movedCurrentFrame, bool hasTree, bool smallMeshCulling, bool supportsIndirect)
	{
		data = (receiveShadows ? 1u : 0u);
		data |= (uint)(staticShadowCaster ? 2 : 0);
		data |= (uint)(lodMask << 2);
		data |= (uint)((int)shadowCastingMode << 10);
		data |= (uint)((int)lightProbeUsage << 12);
		data |= (uint)((int)motionVecGenMode << 15);
		data |= (uint)(isPartOfStaticBatch ? 131072 : 0);
		data |= (uint)(movedCurrentFrame ? 262144 : 0);
		data |= (uint)(hasTree ? 524288 : 0);
		data |= (uint)(smallMeshCulling ? 1048576 : 0);
		data |= (uint)(supportsIndirect ? 2097152 : 0);
	}
}
