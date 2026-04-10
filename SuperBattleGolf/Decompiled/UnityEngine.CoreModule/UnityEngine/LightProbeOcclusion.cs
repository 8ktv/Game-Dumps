using UnityEngine.Bindings;

namespace UnityEngine;

[NativeType("Runtime/GI/SceneData.h")]
internal struct LightProbeOcclusion
{
	internal unsafe fixed int m_ProbeOcclusionLightIndex[4];

	internal unsafe fixed float m_Occlusion[4];

	internal unsafe fixed sbyte m_OcclusionMaskChannel[4];
}
