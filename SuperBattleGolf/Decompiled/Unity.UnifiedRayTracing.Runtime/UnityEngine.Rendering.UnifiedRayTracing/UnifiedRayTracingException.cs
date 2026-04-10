using System;

namespace UnityEngine.Rendering.UnifiedRayTracing;

public class UnifiedRayTracingException : Exception
{
	public UnifiedRayTracingError errorCode { get; private set; }

	public UnifiedRayTracingException(string message, UnifiedRayTracingError errorCode)
		: base(message)
	{
		this.errorCode = errorCode;
	}
}
