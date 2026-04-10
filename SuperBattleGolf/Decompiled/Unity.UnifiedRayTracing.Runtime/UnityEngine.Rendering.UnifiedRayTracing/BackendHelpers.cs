using System;

namespace UnityEngine.Rendering.UnifiedRayTracing;

public static class BackendHelpers
{
	public static string GetFileNameOfShader(RayTracingBackend backend, string fileName)
	{
		return fileName + "." + backend switch
		{
			RayTracingBackend.Hardware => "raytrace", 
			RayTracingBackend.Compute => "compute", 
			_ => throw new ArgumentOutOfRangeException("backend", backend, null), 
		};
	}

	public static Type GetTypeOfShader(RayTracingBackend backend)
	{
		return backend switch
		{
			RayTracingBackend.Hardware => typeof(RayTracingShader), 
			RayTracingBackend.Compute => typeof(ComputeShader), 
			_ => throw new ArgumentOutOfRangeException("backend", backend, null), 
		};
	}
}
