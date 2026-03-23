using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering;

[Flags]
[MovedFrom("UnityEngine.Experimental.Rendering")]
public enum RayTracingInstanceCullingFlags
{
	None = 0,
	EnableSphereCulling = 1,
	EnablePlaneCulling = 2,
	EnableLODCulling = 4,
	ComputeMaterialsCRC = 8,
	IgnoreReflectionProbes = 0x10,
	EnableSolidAngleCulling = 0x20,
	EnableMeshLOD = 0x40
}
