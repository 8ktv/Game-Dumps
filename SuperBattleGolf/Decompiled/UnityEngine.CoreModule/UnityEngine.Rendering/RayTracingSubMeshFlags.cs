using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/RayTracing/RayTracingAccelerationStructure.h")]
[NativeHeader("Runtime/Export/Graphics/RayTracingAccelerationStructure.bindings.h")]
[MovedFrom("UnityEngine.Experimental.Rendering")]
[Flags]
public enum RayTracingSubMeshFlags
{
	Disabled = 0,
	Enabled = 1,
	ClosestHitOnly = 2,
	UniqueAnyHitCalls = 4
}
