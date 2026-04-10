using UnityEngine.Bindings;

namespace UnityEngine;

[NativeType("Runtime/Camera/LightProbeStructs.h")]
internal struct Tetrahedron
{
	internal unsafe fixed int indices[4];

	internal unsafe fixed int neighbors[4];

	internal Matrix3x4f matrix;

	internal bool isValid;
}
