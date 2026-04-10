using System;

namespace UnityEngine.Rendering.UnifiedRayTracing;

public sealed class RayTracingContext : IDisposable
{
	private readonly IRayTracingBackend m_Backend;

	private readonly ReferenceCounter m_AccelStructCounter = new ReferenceCounter();

	private readonly GraphicsBuffer m_DispatchBuffer;

	public RayTracingResources Resources { get; private set; }

	public RayTracingBackend BackendType { get; private set; }

	public RayTracingContext(RayTracingBackend backend, RayTracingResources resources)
	{
		if (!IsBackendSupported(backend))
		{
			throw new InvalidOperationException("Unsupported backend: " + backend);
		}
		BackendType = backend;
		switch (backend)
		{
		case RayTracingBackend.Hardware:
			m_Backend = new HardwareRayTracingBackend(resources);
			break;
		case RayTracingBackend.Compute:
			m_Backend = new ComputeRayTracingBackend(resources);
			break;
		}
		Resources = resources;
		m_DispatchBuffer = RayTracingHelper.CreateDispatchIndirectBuffer();
	}

	public RayTracingContext(RayTracingResources resources)
		: this((!IsBackendSupported(RayTracingBackend.Hardware)) ? RayTracingBackend.Compute : RayTracingBackend.Hardware, resources)
	{
	}

	public void Dispose()
	{
		if (m_AccelStructCounter.value != 0L)
		{
			Debug.LogError("Memory Leak. Please call .Dispose() on all the IAccelerationStructure resources that have been created with this context before calling RayTracingContext.Dispose()");
		}
		m_DispatchBuffer?.Release();
	}

	public static bool IsBackendSupported(RayTracingBackend backend)
	{
		return backend switch
		{
			RayTracingBackend.Hardware => SystemInfo.supportsRayTracing, 
			RayTracingBackend.Compute => SystemInfo.supportsComputeShaders, 
			_ => false, 
		};
	}

	public IRayTracingShader CreateRayTracingShader(Object shader)
	{
		return m_Backend.CreateRayTracingShader(shader, "MainRayGenShader", m_DispatchBuffer);
	}

	public IRayTracingShader LoadRayTracingShaderFromAssetBundle(AssetBundle assetBundle, string name)
	{
		Object shader = assetBundle.LoadAsset(name, BackendHelpers.GetTypeOfShader(BackendType));
		return CreateRayTracingShader(shader);
	}

	public IRayTracingAccelStruct CreateAccelerationStructure(AccelerationStructureOptions options)
	{
		return m_Backend.CreateAccelerationStructure(options, m_AccelStructCounter);
	}

	public ulong GetRequiredTraceScratchBufferSizeInBytes(uint width, uint height, uint depth)
	{
		return m_Backend.GetRequiredTraceScratchBufferSizeInBytes(width, height, depth);
	}

	public static uint GetScratchBufferStrideInBytes()
	{
		return 4u;
	}
}
