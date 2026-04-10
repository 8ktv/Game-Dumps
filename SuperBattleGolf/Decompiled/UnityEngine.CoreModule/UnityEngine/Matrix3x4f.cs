using UnityEngine.Bindings;

namespace UnityEngine;

[NativeType("Runtime/Camera/LightProbeStructs.h")]
internal struct Matrix3x4f
{
	internal unsafe fixed float m_Data[12];
}
