using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering;

[MovedFrom("UnityEngine.Experimental.Rendering")]
public sealed class RayTracingAccelerationStructure : IDisposable
{
	[Flags]
	public enum RayTracingModeMask
	{
		Nothing = 0,
		Static = 2,
		DynamicTransform = 4,
		DynamicGeometry = 8,
		DynamicGeometryManualUpdate = 0x10,
		Everything = 0x1E
	}

	public enum ManagementMode
	{
		Manual,
		Automatic
	}

	public struct BuildSettings
	{
		public RayTracingAccelerationStructureBuildFlags buildFlags { get; set; }

		public Vector3 relativeOrigin { get; set; }

		public BuildSettings()
		{
			buildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			relativeOrigin = Vector3.zero;
		}

		public BuildSettings(RayTracingAccelerationStructureBuildFlags buildFlags, Vector3 relativeOrigin)
		{
			this.buildFlags = buildFlags;
			this.relativeOrigin = relativeOrigin;
		}
	}

	[Obsolete("RayTracingAccelerationStructure.RASSettings is deprecated. Use RayTracingAccelerationStructure.Settings instead. (UnityUpgradable) -> RayTracingAccelerationStructure/Settings", false)]
	public struct RASSettings
	{
		public ManagementMode managementMode;

		public RayTracingModeMask rayTracingModeMask;

		public int layerMask;

		public RASSettings(ManagementMode sceneManagementMode, RayTracingModeMask rayTracingModeMask, int layerMask)
		{
			managementMode = sceneManagementMode;
			this.rayTracingModeMask = rayTracingModeMask;
			this.layerMask = layerMask;
		}
	}

	public struct Settings
	{
		public ManagementMode managementMode;

		public RayTracingModeMask rayTracingModeMask;

		public int layerMask;

		public RayTracingAccelerationStructureBuildFlags buildFlagsStaticGeometries { get; set; }

		public RayTracingAccelerationStructureBuildFlags buildFlagsDynamicGeometries { get; set; }

		public bool enableCompaction { get; set; }

		public Settings()
		{
			managementMode = ManagementMode.Manual;
			rayTracingModeMask = RayTracingModeMask.Everything;
			layerMask = -1;
			buildFlagsStaticGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			buildFlagsDynamicGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			enableCompaction = true;
		}

		public Settings(ManagementMode sceneManagementMode, RayTracingModeMask rayTracingModeMask, int layerMask)
		{
			managementMode = sceneManagementMode;
			this.rayTracingModeMask = rayTracingModeMask;
			this.layerMask = layerMask;
			buildFlagsStaticGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			buildFlagsDynamicGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace;
			enableCompaction = true;
		}

		public Settings(ManagementMode sceneManagementMode, RayTracingModeMask rayTracingModeMask, int layerMask, RayTracingAccelerationStructureBuildFlags buildFlagsStaticGeometries, RayTracingAccelerationStructureBuildFlags buildFlagsDynamicGeometries)
		{
			managementMode = sceneManagementMode;
			this.rayTracingModeMask = rayTracingModeMask;
			this.layerMask = layerMask;
			this.buildFlagsStaticGeometries = buildFlagsStaticGeometries;
			this.buildFlagsDynamicGeometries = buildFlagsDynamicGeometries;
			enableCompaction = true;
		}
	}

	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			return rayTracingAccelerationStructure.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	private const string obsoleteBuildMsg1 = "Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build()";

	private const string obsoleteBuildMsg2 = "Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build(*)";

	private const string obsoleteRendererMsg = "This AddInstance method is deprecated and will be removed and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding Renderers to the acceleration structure.";

	private const string obsoleteAABBMsg1 = "This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.";

	private const string obsoleteAABBMsg2 = "This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.";

	~RayTracingAccelerationStructure()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			Destroy(this);
		}
		m_Ptr = IntPtr.Zero;
	}

	public RayTracingAccelerationStructure(Settings settings)
	{
		m_Ptr = Create(settings);
	}

	public RayTracingAccelerationStructure()
	{
		Settings desc = new Settings
		{
			rayTracingModeMask = RayTracingModeMask.Everything,
			managementMode = ManagementMode.Manual,
			layerMask = -1,
			buildFlagsStaticGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace,
			buildFlagsDynamicGeometries = RayTracingAccelerationStructureBuildFlags.PreferFastTrace
		};
		m_Ptr = Create(desc);
	}

	public void Release()
	{
		Dispose();
	}

	public void Build()
	{
		Build(new BuildSettings());
	}

	public void Build(Vector3 relativeOrigin)
	{
		BuildSettings buildSettings = new BuildSettings();
		buildSettings.relativeOrigin = relativeOrigin;
		BuildSettings buildSettings2 = buildSettings;
		Build(buildSettings2);
	}

	public int AddInstance(Renderer targetRenderer, RayTracingSubMeshFlags[] subMeshFlags, bool enableTriangleCulling = true, bool frontTriangleCounterClockwise = false, uint mask = 255u, uint id = uint.MaxValue)
	{
		return AddInstanceSubMeshFlagsArray(targetRenderer, subMeshFlags, enableTriangleCulling, frontTriangleCounterClockwise, mask, id);
	}

	public int AddInstance(RayTracingAABBsInstanceConfig config, Matrix4x4 matrix, uint id = uint.MaxValue)
	{
		if (config.aabbBuffer == null)
		{
			throw new ArgumentNullException("config.aabbBuffer.");
		}
		if (config.aabbBuffer.target != GraphicsBuffer.Target.Structured)
		{
			throw new ArgumentException("config.aabbBuffer.target must be GraphicsBuffer.Target.Structured.");
		}
		if (config.aabbBuffer.stride != 24)
		{
			throw new ArgumentException("config.aabbBuffer.stride must be 6 floats.");
		}
		if (config.aabbCount == 0)
		{
			throw new ArgumentException("config.aabbCount cannot be 0.");
		}
		return AddAABBsInstance(config, matrix, id);
	}

	public unsafe int AddInstance(in RayTracingMeshInstanceConfig config, Matrix4x4 matrix, [DefaultValue("null")] Matrix4x4? prevMatrix = null, uint id = uint.MaxValue)
	{
		if (config.mesh == null)
		{
			throw new ArgumentNullException("config.mesh");
		}
		if (config.subMeshIndex >= config.mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
		}
		if (config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null)
		{
			throw new ArgumentException("config.lightProbeProxyVolume must not be null if config.lightProbeUsage is set to UseProxyVolume.");
		}
		if (config.meshLod > 0 && config.meshLod >= config.mesh.lodCount)
		{
			throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
		}
		if (prevMatrix.HasValue)
		{
			Matrix4x4 value = prevMatrix.Value;
			return AddMeshInstance(config, matrix, &value, id);
		}
		return AddMeshInstance(config, matrix, null, id);
	}

	public unsafe int AddInstance(in RayTracingGeometryInstanceConfig config, Matrix4x4 matrix, [DefaultValue("null")] Matrix4x4? prevMatrix = null, uint id = uint.MaxValue)
	{
		if (config.vertexBuffer == null)
		{
			throw new ArgumentException("config.vertexBuffer must not be null.");
		}
		if (config.vertexCount == -1 && config.vertexStart >= config.vertexBuffer.count)
		{
			throw new ArgumentOutOfRangeException("config.vertexStart", $"config.vertexStart ({config.vertexStart}) is out of range. Not enough vertices in the vertex buffer ({config.vertexBuffer.count}).");
		}
		if (config.vertexCount != -1 && config.vertexStart + config.vertexCount > config.vertexBuffer.count)
		{
			throw new ArgumentOutOfRangeException("config.vertexStart", $"config.vertexStart ({config.vertexStart}) + config.vertexCount ({config.vertexCount}) is out of range. Not enough vertices in the vertex buffer ({config.vertexBuffer.count}).");
		}
		int num = ((config.vertexCount < 0) ? config.vertexBuffer.count : config.vertexCount);
		if (num == 0)
		{
			throw new ArgumentOutOfRangeException("config.vertexCount", "The amount of vertices used must be greater than 0.");
		}
		if (config.indexBuffer != null)
		{
			if (config.indexBuffer.count < 3)
			{
				throw new ArgumentOutOfRangeException("config.indexBuffer", "config.indexBuffer must contain at least 3 indices.");
			}
			if (config.indexCount == -1 && config.indexStart >= config.indexBuffer.count)
			{
				throw new ArgumentOutOfRangeException("config.indexStart", $"config.indexStart ({config.indexStart}) is out of range. Not enough indices in the index buffer ({config.indexBuffer.count}).");
			}
			if (config.indexCount != -1 && config.indexStart + config.indexCount > config.indexBuffer.count)
			{
				throw new ArgumentOutOfRangeException("config.indexStart", $"config.indexStart ({config.indexStart}) + config.indexCount ({config.indexCount}) is out of range. Not enough indices in the index buffer ({config.indexBuffer.count}).");
			}
			int num2 = ((config.indexCount < 0) ? config.indexBuffer.count : config.indexCount);
			if (num2 % 3 != 0)
			{
				throw new ArgumentOutOfRangeException("config.indexBuffer", $"The amount of indices used must be a multiple of 3. Only triangle geometries are supported. Currently using {num2} indices.");
			}
		}
		else if (num % 3 != 0)
		{
			throw new ArgumentOutOfRangeException("config.vertexBuffer", $"When config.indexBuffer is null, the amount of vertices used must be a multiple of 3. Only triangle geometries are supported. Currently using {num} vertices.");
		}
		if (config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null)
		{
			throw new ArgumentException("config.lightProbeProxyVolume must not be null if config.lightProbeUsage is set to UseProxyVolume.");
		}
		if (config.vertexAttributes == null)
		{
			throw new ArgumentNullException("config.vertexAttributes");
		}
		if (config.vertexAttributes.Length == 0)
		{
			throw new ArgumentException("config.vertexAttributes must contain at least one entry.");
		}
		if (prevMatrix.HasValue)
		{
			Matrix4x4 value = prevMatrix.Value;
			return AddGeometryInstance(in config, matrix, &value, id);
		}
		return AddGeometryInstance(in config, matrix, null, id);
	}

	public unsafe int AddInstances<T>(in RayTracingMeshInstanceConfig config, T[] instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0, uint id = uint.MaxValue) where T : unmanaged
	{
		if (instanceData == null)
		{
			throw new ArgumentNullException("instanceData");
		}
		if (config.material != null && !CheckMaterialEnablesInstancing(config.material))
		{
			throw new InvalidOperationException("config.material (" + config.material.name + ") needs to enable GPU Instancing for use with AddInstances.");
		}
		if (config.mesh == null)
		{
			throw new ArgumentNullException("config.mesh");
		}
		if (config.subMeshIndex >= config.mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
		}
		if (config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null)
		{
			throw new ArgumentException("config.lightProbeProxyVolume argument must not be null if config.lightProbeUsage is set to UseProxyVolume.");
		}
		if (config.meshLod > 0 && config.meshLod >= config.mesh.lodCount)
		{
			throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
		}
		RenderInstancedDataLayout cachedRenderInstancedDataLayout = Graphics.GetCachedRenderInstancedDataLayout(typeof(T));
		instanceCount = ((instanceCount == -1) ? instanceData.Length : instanceCount);
		startInstance = Math.Clamp(startInstance, 0, Math.Max(0, instanceData.Length - 1));
		instanceCount = Math.Clamp(instanceCount, 0, Math.Max(0, instanceData.Length - startInstance));
		if (instanceCount > Graphics.kMaxDrawMeshInstanceCount)
		{
			throw new InvalidOperationException($"Instance count cannot exceed {Graphics.kMaxDrawMeshInstanceCount}.");
		}
		fixed (T* ptr = instanceData)
		{
			return AddMeshInstances(config, (IntPtr)(ptr + startInstance), cachedRenderInstancedDataLayout, (uint)instanceCount, id);
		}
	}

	public unsafe int AddInstances<T>(in RayTracingMeshInstanceConfig config, List<T> instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0, uint id = uint.MaxValue) where T : unmanaged
	{
		if (instanceData == null)
		{
			throw new ArgumentNullException("instanceData");
		}
		if (config.material != null && !CheckMaterialEnablesInstancing(config.material))
		{
			throw new InvalidOperationException("config.material (" + config.material.name + ") needs to enable GPU Instancing for use with AddInstances.");
		}
		if (config.mesh == null)
		{
			throw new ArgumentNullException("config.mesh");
		}
		if (config.subMeshIndex >= config.mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
		}
		if (config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null)
		{
			throw new ArgumentException("config.lightProbeProxyVolume argument must not be null if config.lightProbeUsage is set to UseProxyVolume.");
		}
		if (config.meshLod > 0 && config.meshLod >= config.mesh.lodCount)
		{
			throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
		}
		RenderInstancedDataLayout cachedRenderInstancedDataLayout = Graphics.GetCachedRenderInstancedDataLayout(typeof(T));
		instanceCount = ((instanceCount == -1) ? instanceData.Count : instanceCount);
		startInstance = Math.Clamp(startInstance, 0, Math.Max(0, instanceData.Count - 1));
		instanceCount = Math.Clamp(instanceCount, 0, Math.Max(0, instanceData.Count - startInstance));
		if (instanceCount > Graphics.kMaxDrawMeshInstanceCount)
		{
			throw new InvalidOperationException($"Instance count cannot exceed {Graphics.kMaxDrawMeshInstanceCount}.");
		}
		fixed (T* ptr = NoAllocHelpers.ExtractArrayFromList(instanceData))
		{
			return AddMeshInstances(config, (IntPtr)(ptr + startInstance), cachedRenderInstancedDataLayout, (uint)instanceCount, id);
		}
	}

	public unsafe int AddInstances<T>(in RayTracingMeshInstanceConfig config, NativeArray<T> instanceData, [DefaultValue("-1")] int instanceCount = -1, [DefaultValue("0")] int startInstance = 0, uint id = uint.MaxValue) where T : unmanaged
	{
		if (config.material != null && !CheckMaterialEnablesInstancing(config.material))
		{
			throw new InvalidOperationException("config.material (" + config.material.name + ") needs to enable GPU Instancing for use with AddInstances.");
		}
		if (config.mesh == null)
		{
			throw new ArgumentNullException("config.mesh");
		}
		if (config.subMeshIndex >= config.mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
		}
		if (config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null)
		{
			throw new ArgumentException("config.lightProbeProxyVolume argument must not be null if config.lightProbeUsage is set to UseProxyVolume.");
		}
		if (config.meshLod > 0 && config.meshLod >= config.mesh.lodCount)
		{
			throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
		}
		RenderInstancedDataLayout cachedRenderInstancedDataLayout = Graphics.GetCachedRenderInstancedDataLayout(typeof(T));
		instanceCount = ((instanceCount == -1) ? instanceData.Length : instanceCount);
		startInstance = Math.Clamp(startInstance, 0, Math.Max(0, instanceData.Length - 1));
		instanceCount = Math.Clamp(instanceCount, 0, Math.Max(0, instanceData.Length - startInstance));
		if (instanceCount > Graphics.kMaxDrawMeshInstanceCount)
		{
			throw new InvalidOperationException($"Instance count cannot exceed {Graphics.kMaxDrawMeshInstanceCount}.");
		}
		return AddMeshInstances(config, (IntPtr)((byte*)instanceData.GetUnsafeReadOnlyPtr() + (nint)startInstance * (nint)sizeof(T)), cachedRenderInstancedDataLayout, (uint)instanceCount, id);
	}

	public int AddInstancesIndirect(in RayTracingMeshInstanceConfig config, GraphicsBuffer instanceMatrices, int maxInstanceCount, GraphicsBuffer argsBuffer, [DefaultValue("0")] uint argsOffset = 0u, uint id = uint.MaxValue)
	{
		if (config.mesh == null)
		{
			throw new ArgumentNullException("config.mesh");
		}
		if (instanceMatrices == null)
		{
			throw new ArgumentNullException("instanceMatrices");
		}
		if (argsBuffer == null)
		{
			throw new ArgumentNullException("argsBuffer");
		}
		if (config.material != null && !CheckMaterialEnablesInstancing(config.material))
		{
			throw new InvalidOperationException("config.material needs to enable instancing for use with AddInstancesIndirect.");
		}
		if (config.subMeshIndex >= config.mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("config.subMeshIndex", $"The Mesh contains only {config.mesh.subMeshCount} sub-meshes.");
		}
		if (config.lightProbeUsage != LightProbeUsage.Off)
		{
			throw new ArgumentException("config.lightProbeUsage must be LightProbeUsage.Off. This method doesn't support light probe setup.");
		}
		if (config.lightProbeProxyVolume != null)
		{
			throw new ArgumentException("config.lightProbeProxyVolume must be null. This method doesn't support Light Probe Proxy Volume.");
		}
		if (instanceMatrices.stride != 64)
		{
			throw new ArgumentException(string.Format("{0} ({1}) must be 64 bytes.", "stride", instanceMatrices.stride));
		}
		if ((instanceMatrices.target & GraphicsBuffer.Target.Structured) == 0 && (instanceMatrices.target & GraphicsBuffer.Target.Append) == 0)
		{
			throw new ArgumentException("target must use GraphicsBuffer.Target.Structured or GraphicsBuffer.Target.Append flag.");
		}
		if (maxInstanceCount > instanceMatrices.count)
		{
			throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, $"The value cannot exceed {instanceMatrices.count}.");
		}
		if (maxInstanceCount < -1 || maxInstanceCount == 0)
		{
			throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, "The parameter must be either -1 or a positive value.");
		}
		if (argsBuffer.target != GraphicsBuffer.Target.Raw)
		{
			throw new ArgumentException("argsBuffer buffer must use GraphicsBuffer.Target.Raw flag.");
		}
		if (argsBuffer.count * argsBuffer.stride < 8)
		{
			throw new ArgumentException(string.Format("{0} buffer must contain at least 2 uints at the {1} byte offset. The current size of the buffer is {2}.", "argsBuffer", argsOffset, argsBuffer.count * argsBuffer.stride));
		}
		if (maxInstanceCount == -1)
		{
			maxInstanceCount = instanceMatrices.count;
		}
		return AddMeshInstancesIndirect(in config, instanceMatrices, (uint)maxInstanceCount, argsBuffer, argsOffset, id);
	}

	public int AddInstancesIndirect(in RayTracingGeometryInstanceConfig config, GraphicsBuffer instanceMatrices, int maxInstanceCount, GraphicsBuffer argsBuffer, [DefaultValue("0")] uint argsOffset = 0u, uint id = uint.MaxValue)
	{
		if (instanceMatrices == null)
		{
			throw new ArgumentNullException("instanceMatrices");
		}
		if (argsBuffer == null)
		{
			throw new ArgumentNullException("argsBuffer");
		}
		if (config.material != null && !CheckMaterialEnablesInstancing(config.material))
		{
			throw new InvalidOperationException("config.material needs to enable instancing for use with AddInstancesIndirect.");
		}
		if (config.vertexBuffer == null)
		{
			throw new ArgumentNullException("config.vertexBuffer");
		}
		if (config.vertexCount == -1 && config.vertexStart >= config.vertexBuffer.count)
		{
			throw new ArgumentOutOfRangeException("config.vertexStart", config.vertexStart, $"Addressing vertices outside of the vertex buffer ({config.vertexBuffer.count}).");
		}
		if (config.vertexCount != -1 && config.vertexStart + config.vertexCount > config.vertexBuffer.count)
		{
			throw new ArgumentOutOfRangeException("config.vertexStart", $"config.vertexStart ({config.vertexStart}) is too large and is causing invalid vertices to be used. The vertex buffer contains {config.vertexBuffer.count} vertices.");
		}
		int num = ((config.vertexCount < 0) ? config.vertexBuffer.count : config.vertexCount);
		if (num == 0)
		{
			throw new ArgumentException("The amount of vertices used must be greater than 0.", "config.vertexCount");
		}
		if (config.indexBuffer != null)
		{
			if (config.indexBuffer.count < 3)
			{
				throw new ArgumentException("config.indexBuffer", $"The index buffer must contain at least 3 indices. Currently using {config.indexBuffer.count} indices.");
			}
			if (config.indexCount == -1 && config.indexStart >= config.indexBuffer.count)
			{
				throw new ArgumentOutOfRangeException("config.indexStart", config.indexStart, $"The value exceeds the amount of indices ({config.indexBuffer.count}) in the index buffer.");
			}
			if (config.indexCount != -1 && config.indexStart + config.indexCount > config.indexBuffer.count)
			{
				if (config.indexStart == 0)
				{
					throw new ArgumentOutOfRangeException("config.indexCount", $"The value exceeds the amount of indices ({config.indexBuffer.count}) in the index buffer.");
				}
				throw new ArgumentOutOfRangeException("config.indexStart", string.Format("{0}.{1} ({2}) + {3}.{4} ({5}) is out of range. The result exceeds the amount of indices ({6}) in the index buffer.", "config", "indexStart", config.indexStart, "config", "indexCount", config.indexCount, config.indexBuffer.count));
			}
			int num2 = ((config.indexCount < 0) ? config.indexBuffer.count : config.indexCount);
			if (num2 % 3 != 0)
			{
				throw new ArgumentException("config.indexBuffer", $"The amount of indices used must be a multiple of 3. Only triangle geometries are supported. Currently using {num2} indices.");
			}
		}
		else
		{
			if (num < 3)
			{
				throw new ArgumentException($"The amount of vertices used must be at least 3. Only triangle geometries are supported. Currently using {num} vertices.");
			}
			if (num % 3 != 0)
			{
				throw new ArgumentException("config.vertexBuffer", string.Format("When {0}.{1} is null, the amount of vertices used must be a multiple of 3. Only triangle geometries are supported. Currently using {2} vertices.", "config", "indexBuffer", num));
			}
		}
		if (config.lightProbeUsage != LightProbeUsage.Off)
		{
			throw new ArgumentException("config.lightProbeUsage must be LightProbeUsage.Off. This method doesn't support light probe setup.");
		}
		if (config.lightProbeProxyVolume != null)
		{
			throw new ArgumentException("config.lightProbeProxyVolume must be null. This method doesn't support Light Probe Proxy Volume.");
		}
		if (config.vertexAttributes == null)
		{
			throw new ArgumentNullException("config.vertexAttributes");
		}
		if (config.vertexAttributes.Length == 0)
		{
			throw new ArgumentException("config.vertexAttributes must contain at least one entry.");
		}
		if (instanceMatrices.stride != 64)
		{
			throw new ArgumentException(string.Format("{0} ({1}) must be 64 bytes.", "stride", instanceMatrices.stride));
		}
		if ((instanceMatrices.target & GraphicsBuffer.Target.Structured) == 0 && (instanceMatrices.target & GraphicsBuffer.Target.Append) == 0)
		{
			throw new ArgumentException("target must use GraphicsBuffer.Target.Structured or GraphicsBuffer.Target.Append flag.");
		}
		if (maxInstanceCount > instanceMatrices.count)
		{
			throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, $"The value cannot exceed {instanceMatrices.count}.");
		}
		if (maxInstanceCount < -1 || maxInstanceCount == 0)
		{
			throw new ArgumentOutOfRangeException("maxInstanceCount", maxInstanceCount, "The parameter must be either -1 or a positive value.");
		}
		if (maxInstanceCount == -1)
		{
			maxInstanceCount = instanceMatrices.count;
		}
		if (argsBuffer.target != GraphicsBuffer.Target.Raw)
		{
			throw new ArgumentException("argsBuffer buffer must use GraphicsBuffer.Target.Raw flag.");
		}
		if (argsBuffer.count * argsBuffer.stride < 8)
		{
			throw new ArgumentException(string.Format("{0} buffer must contain at least 2 uints at the {1} byte offset. The current size of the buffer is {2}.", "argsBuffer", argsOffset, argsBuffer.count * argsBuffer.stride));
		}
		return AddGeometryInstancesIndirect(in config, instanceMatrices, (uint)maxInstanceCount, argsBuffer, argsOffset, id);
	}

	public int AddInstancesIndirect(in RayTracingMultiGeometryInstanceConfig config, GraphicsBuffer instanceData, Type instanceType, GraphicsBuffer instanceIndices, int maxInstanceCount, GraphicsBuffer argsBuffer, [DefaultValue("0")] uint argsOffset = 0u, uint id = uint.MaxValue)
	{
		if (instanceData == null)
		{
			throw new ArgumentNullException("instanceData");
		}
		if (instanceIndices != null)
		{
			if ((instanceIndices.target & GraphicsBuffer.Target.Structured) == 0 && (instanceIndices.target & GraphicsBuffer.Target.Append) == 0)
			{
				throw new ArgumentException("instanceIndices must use GraphicsBuffer.Target.Structured or GraphicsBuffer.Target.Append flag.");
			}
			if (instanceIndices.stride != 4)
			{
				throw new ArgumentException(string.Format("When using instance indices, the element type for the {0} buffer must be int (4 bytes stride). The current stride is {1} bytes.", "instanceIndices", instanceIndices.stride));
			}
		}
		if (argsBuffer == null)
		{
			throw new ArgumentNullException("argsBuffer");
		}
		if (config.materials == null)
		{
			throw new ArgumentNullException("config.materials");
		}
		if (config.materials.Length == 0)
		{
			throw new ArgumentException("config.materials needs to be contain at least one entry.");
		}
		for (int i = 0; i < config.materials.Length; i++)
		{
			if (config.materials[i] != null && !CheckMaterialEnablesInstancing(config.materials[i]))
			{
				throw new InvalidOperationException("config.material (" + config.materials[i].name + ") needs to enable GPU Instancing for use with AddInstancesIndirect.");
			}
		}
		if (config.vertexBuffer == null)
		{
			throw new ArgumentNullException("config.vertexBuffer");
		}
		if (config.subGeometries == null)
		{
			throw new ArgumentNullException("config.subGeometries");
		}
		if (config.subGeometries.Length == 0)
		{
			throw new ArgumentException("config.subGeometries array needs to be contain at least one entry.");
		}
		if (config.indexBuffer != null && config.indexBuffer.count < 3)
		{
			throw new ArgumentException("config.indexBuffer", $"The index buffer must contain at least 3 indices. Currently using {config.indexBuffer.count} indices.");
		}
		if (config.subGeometriesValidation)
		{
			for (int j = 0; j < config.subGeometries.Length; j++)
			{
				ref RayTracingSubGeometryDesc reference = ref config.subGeometries[j];
				int num = ((reference.vertexCount <= 0) ? config.vertexBuffer.count : reference.vertexCount);
				if (reference.vertexStart >= config.vertexBuffer.count)
				{
					throw new ArgumentOutOfRangeException($"config.subGeometries[{j}].vertexStart", reference.vertexStart, $"Addressing vertices outside of the vertex buffer ({config.vertexBuffer.count}).");
				}
				if (reference.vertexStart + num > config.vertexBuffer.count)
				{
					throw new ArgumentOutOfRangeException($"config.subGeometries[{j}].vertexStart", $"config.subGeometries[{j}].vertexStart ({reference.vertexStart}) is too large and is causing invalid vertices to be used. Currently using {num} vertices starting from vertex {reference.vertexStart} but the vertex buffer contains only {config.vertexBuffer.count} vertices.");
				}
				if (config.indexBuffer != null)
				{
					if (reference.indexStart >= config.indexBuffer.count)
					{
						throw new ArgumentOutOfRangeException($"config.subGeometries[{j}].indexStart", reference.indexStart, $"The value exceeds the amount of indices ({config.indexBuffer.count}) in the index buffer.");
					}
					if (reference.indexCount > config.indexBuffer.count)
					{
						throw new ArgumentOutOfRangeException($"config.subGeometries[{j}].indexCount", reference.indexCount, $"The value exceeds the amount of indices ({config.indexBuffer.count}) in the index buffer.");
					}
					if (reference.indexStart + reference.indexCount > config.indexBuffer.count)
					{
						if (reference.indexStart == 0)
						{
							throw new ArgumentOutOfRangeException($"config.subGeometries[{j}].indexCount", $"The value exceeds the amount of indices ({config.indexBuffer.count}) in the index buffer.");
						}
						throw new ArgumentOutOfRangeException($"config.subGeometries[{j}].indexStart", "The value is too large and causes out of range indices to be used.");
					}
					if (reference.indexCount % 3 != 0)
					{
						throw new ArgumentException($"config.subGeometries[{j}].indexCount", "The amount of indices used must be a multiple of 3. Only triangle geometries are supported.");
					}
				}
				else
				{
					if (num < 3)
					{
						throw new ArgumentException($"config.subGeometries[{j}].vertexCount", $"The amount of vertices used must be at least 3. Only triangle geometries are supported. Currently using {num} vertices.");
					}
					if (num % 3 != 0)
					{
						throw new ArgumentException("config.vertexBuffer", string.Format("When {0}.{1} is null, the amount of vertices used must be a multiple of 3. Only triangle geometries are supported. Currently using {2} vertices.", "config", "indexBuffer", num));
					}
				}
			}
		}
		if (config.vertexAttributes == null)
		{
			throw new ArgumentNullException("config.vertexAttributes");
		}
		if (config.vertexAttributes.Length == 0)
		{
			throw new ArgumentException("config.vertexAttributes must contain at least one entry.");
		}
		if ((instanceData.target & GraphicsBuffer.Target.Raw) == 0)
		{
			throw new ArgumentException("target must use GraphicsBuffer.Target.Raw flag.");
		}
		int num2 = Marshal.SizeOf(instanceType);
		int num3 = instanceData.stride * instanceData.count / num2;
		if (maxInstanceCount < -1 || maxInstanceCount == 0)
		{
			throw new ArgumentException("maxInstanceCount", $"The value must be either -1 or a positive value. Currently using {maxInstanceCount} instances.");
		}
		if (maxInstanceCount == -1)
		{
			maxInstanceCount = num3;
		}
		if (instanceIndices != null && maxInstanceCount > instanceIndices.count)
		{
			maxInstanceCount = instanceIndices.count;
		}
		if (argsBuffer.target != GraphicsBuffer.Target.Raw)
		{
			throw new ArgumentException("argsBuffer buffer must use GraphicsBuffer.Target.Raw flag.");
		}
		if (argsBuffer.count * argsBuffer.stride < 8)
		{
			throw new ArgumentException(string.Format("{0} buffer must contain at least 2 unsigned integer values at the {1} byte offset. The current size of the buffer is {2}.", "argsBuffer", argsOffset, argsBuffer.count * argsBuffer.stride));
		}
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		try
		{
			num4 = Marshal.OffsetOf(instanceType, "objectToWorld").ToInt32();
		}
		catch (ArgumentException)
		{
			num4 = -1;
		}
		try
		{
			num5 = Marshal.OffsetOf(instanceType, "materialIndex").ToInt32();
		}
		catch (ArgumentException)
		{
			num5 = -1;
		}
		try
		{
			num6 = Marshal.OffsetOf(instanceType, "geometryIndex").ToInt32();
		}
		catch (ArgumentException)
		{
			num6 = -1;
		}
		if (num4 == -1)
		{
			throw new ArgumentException("T template structure must contain a field named objectToWorld.");
		}
		if (num4 % 4 != 0)
		{
			throw new ArgumentException("objectToWorld field offset must be a multiple of 4.");
		}
		if (num5 == -1)
		{
			throw new ArgumentException("T template structure must contain a field named materialIndex.");
		}
		if (num5 % 4 != 0)
		{
			throw new ArgumentException("materialIndex field offset must be a multiple of 4.");
		}
		if (num6 == -1)
		{
			throw new ArgumentException("T template structure must contain a field named geometryIndex.");
		}
		if (num6 % 4 != 0)
		{
			throw new ArgumentException("geometryIndex field offset must be a multiple of 4.");
		}
		return AddMultiGeometryInstancesIndirect(in config, instanceData, instanceIndices, num2, num4, num5, num6, (uint)maxInstanceCount, argsBuffer, argsOffset, id);
	}

	public int AddInstancesIndirect<T>(in RayTracingMultiGeometryInstanceConfig config, GraphicsBuffer instanceData, GraphicsBuffer instanceIndices, int maxInstanceCount, GraphicsBuffer argsBuffer, [DefaultValue("0")] uint argsOffset = 0u, uint id = uint.MaxValue)
	{
		return AddInstancesIndirect(in config, instanceData, typeof(T), instanceIndices, maxInstanceCount, argsBuffer, argsOffset, id);
	}

	public unsafe int AddInstances<T>(in RayTracingMeshInstanceConfig config, NativeSlice<T> instanceData, uint id = uint.MaxValue) where T : unmanaged
	{
		if (config.material == null)
		{
			throw new ArgumentNullException("config.material");
		}
		if (!CheckMaterialEnablesInstancing(config.material))
		{
			throw new InvalidOperationException("config.material (" + config.material.name + ") needs to enable GPU Instancing for use with AddInstances.");
		}
		if (config.mesh == null)
		{
			throw new ArgumentNullException("config.mesh");
		}
		if (config.subMeshIndex >= config.mesh.subMeshCount)
		{
			throw new ArgumentOutOfRangeException("config.subMeshIndex", "config.subMeshIndex is out of range.");
		}
		if (config.lightProbeUsage == LightProbeUsage.UseProxyVolume && config.lightProbeProxyVolume == null)
		{
			throw new ArgumentException("config.lightProbeProxyVolume argument must not be null if config.lightProbeUsage is set to UseProxyVolume.");
		}
		if (config.meshLod > 0 && config.meshLod >= config.mesh.lodCount)
		{
			throw new ArgumentOutOfRangeException("config.meshLod", "config.meshLod is out of range");
		}
		RenderInstancedDataLayout cachedRenderInstancedDataLayout = Graphics.GetCachedRenderInstancedDataLayout(typeof(T));
		if (instanceData.Length > Graphics.kMaxDrawMeshInstanceCount)
		{
			throw new InvalidOperationException($"Instance count cannot exceed {Graphics.kMaxDrawMeshInstanceCount}.");
		}
		return AddMeshInstances(config, (IntPtr)instanceData.GetUnsafeReadOnlyPtr(), cachedRenderInstancedDataLayout, (uint)instanceData.Length, id);
	}

	public void RemoveInstance(Renderer targetRenderer)
	{
		RemoveInstance_Renderer(targetRenderer);
	}

	public void RemoveInstance(int handle)
	{
		RemoveInstance_InstanceID(handle);
	}

	public void UpdateInstanceGeometry(Renderer renderer)
	{
		UpdateInstanceGeometry_Renderer(renderer);
	}

	public void UpdateInstanceGeometry(int handle)
	{
		UpdateInstanceGeometry_Handle(handle);
	}

	public void UpdateInstanceTransform(Renderer renderer)
	{
		UpdateInstanceTransform_Renderer(renderer);
	}

	public void UpdateInstanceTransform(int handle, Matrix4x4 matrix)
	{
		UpdateInstanceTransform_Handle(handle, matrix);
	}

	public void UpdateInstanceID(Renderer renderer, uint instanceID)
	{
		UpdateInstanceID_Renderer(renderer, instanceID);
	}

	public void UpdateInstanceID(int handle, uint instanceID)
	{
		UpdateInstanceID_Handle(handle, instanceID);
	}

	public void UpdateInstanceMask(Renderer renderer, uint mask)
	{
		UpdateInstanceMask_Renderer(renderer, mask);
	}

	public void UpdateInstanceMask(int handle, uint mask)
	{
		UpdateInstanceMask_Handle(handle, mask);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::Build", HasExplicitThis = true)]
	public void Build(BuildSettings buildSettings)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Build_Injected(intPtr, ref buildSettings);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::AddVFXInstances", HasExplicitThis = true)]
	public unsafe void AddVFXInstances([NotNull] Renderer targetRenderer, uint[] vfxSystemMasks)
	{
		if ((object)targetRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(targetRenderer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
		}
		Span<uint> span = new Span<uint>(vfxSystemMasks);
		fixed (uint* begin = span)
		{
			ManagedSpanWrapper vfxSystemMasks2 = new ManagedSpanWrapper(begin, span.Length);
			AddVFXInstances_Injected(intPtr, intPtr2, ref vfxSystemMasks2);
		}
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::RemoveVFXInstances", HasExplicitThis = true)]
	public void RemoveVFXInstances([NotNull] Renderer targetRenderer)
	{
		if ((object)targetRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(targetRenderer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
		}
		RemoveVFXInstances_Injected(intPtr, intPtr2);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstancePropertyBlock", HasExplicitThis = true)]
	public void UpdateInstancePropertyBlock(int handle, MaterialPropertyBlock properties)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpdateInstancePropertyBlock_Injected(intPtr, handle, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::GetSize", HasExplicitThis = true)]
	public ulong GetSize()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetSize_Injected(intPtr);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::GetInstanceCount", HasExplicitThis = true)]
	public uint GetInstanceCount()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetInstanceCount_Injected(intPtr);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::ClearInstances", HasExplicitThis = true)]
	public void ClearInstances()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearInstances_Injected(intPtr);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::RemoveInstances", HasExplicitThis = true)]
	public void RemoveInstances(int layerMask, RayTracingModeMask rayTracingModeMask)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveInstances_Injected(intPtr, layerMask, rayTracingModeMask);
	}

	public RayTracingInstanceCullingResults CullInstances(ref RayTracingInstanceCullingConfig cullingConfig)
	{
		return Internal_CullInstances(in cullingConfig);
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::Create")]
	private static IntPtr Create(Settings desc)
	{
		return Create_Injected(ref desc);
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::Destroy")]
	private static void Destroy(RayTracingAccelerationStructure accelStruct)
	{
		Destroy_Injected((accelStruct == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(accelStruct));
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::RemoveInstance", HasExplicitThis = true)]
	private void RemoveInstance_Renderer([NotNull] Renderer targetRenderer)
	{
		if ((object)targetRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(targetRenderer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
		}
		RemoveInstance_Renderer_Injected(intPtr, intPtr2);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::RemoveInstance", HasExplicitThis = true)]
	private void RemoveInstance_InstanceID(int instanceID)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveInstance_InstanceID_Injected(intPtr, instanceID);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceTransform", HasExplicitThis = true)]
	private void UpdateInstanceTransform_Renderer([NotNull] Renderer renderer)
	{
		if ((object)renderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(renderer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		UpdateInstanceTransform_Renderer_Injected(intPtr, intPtr2);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceTransform", HasExplicitThis = true)]
	private void UpdateInstanceTransform_Handle(int handle, Matrix4x4 matrix)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpdateInstanceTransform_Handle_Injected(intPtr, handle, ref matrix);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceGeometry", HasExplicitThis = true)]
	private void UpdateInstanceGeometry_Renderer([NotNull] Renderer renderer)
	{
		if ((object)renderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(renderer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		UpdateInstanceGeometry_Renderer_Injected(intPtr, intPtr2);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceGeometry", HasExplicitThis = true)]
	private void UpdateInstanceGeometry_Handle(int handle)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpdateInstanceGeometry_Handle_Injected(intPtr, handle);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceMask", HasExplicitThis = true)]
	private void UpdateInstanceMask_Renderer([NotNull] Renderer renderer, uint mask)
	{
		if ((object)renderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(renderer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		UpdateInstanceMask_Renderer_Injected(intPtr, intPtr2, mask);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceMask", HasExplicitThis = true)]
	private void UpdateInstanceMask_Handle(int handle, uint mask)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpdateInstanceMask_Handle_Injected(intPtr, handle, mask);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceID", HasExplicitThis = true)]
	private void UpdateInstanceID_Renderer([NotNull] Renderer renderer, uint id)
	{
		if ((object)renderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(renderer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
		}
		UpdateInstanceID_Renderer_Injected(intPtr, intPtr2, id);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::UpdateInstanceID", HasExplicitThis = true)]
	private void UpdateInstanceID_Handle(int handle, uint id)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UpdateInstanceID_Handle_Injected(intPtr, handle, id);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::AddInstanceSubMeshFlagsArray", HasExplicitThis = true)]
	private unsafe int AddInstanceSubMeshFlagsArray([NotNull] Renderer targetRenderer, RayTracingSubMeshFlags[] subMeshFlags, bool enableTriangleCulling = true, bool frontTriangleCounterClockwise = false, uint mask = 255u, uint id = uint.MaxValue)
	{
		if ((object)targetRenderer == null)
		{
			ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(targetRenderer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(targetRenderer, "targetRenderer");
		}
		Span<RayTracingSubMeshFlags> span = new Span<RayTracingSubMeshFlags>(subMeshFlags);
		int result;
		fixed (RayTracingSubMeshFlags* begin = span)
		{
			ManagedSpanWrapper subMeshFlags2 = new ManagedSpanWrapper(begin, span.Length);
			result = AddInstanceSubMeshFlagsArray_Injected(intPtr, intPtr2, ref subMeshFlags2, enableTriangleCulling, frontTriangleCounterClockwise, mask, id);
		}
		return result;
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::AddMeshInstance", HasExplicitThis = true)]
	private unsafe int AddMeshInstance(RayTracingMeshInstanceConfig config, Matrix4x4 matrix, Matrix4x4* prevMatrix, uint id = uint.MaxValue)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddMeshInstance_Injected(intPtr, ref config, ref matrix, prevMatrix, id);
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::AddGeometryInstance", HasExplicitThis = true)]
	private unsafe int AddGeometryInstance(in RayTracingGeometryInstanceConfig config, Matrix4x4 matrix, Matrix4x4* prevMatrix, uint id = uint.MaxValue)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddGeometryInstance_Injected(intPtr, in config, ref matrix, prevMatrix, id);
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::AddMeshInstances", HasExplicitThis = true)]
	private int AddMeshInstances(RayTracingMeshInstanceConfig config, IntPtr instancedData, RenderInstancedDataLayout layout, uint instanceCount, uint id = uint.MaxValue)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddMeshInstances_Injected(intPtr, ref config, instancedData, ref layout, instanceCount, id);
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::AddMeshInstancesIndirect", HasExplicitThis = true)]
	private int AddMeshInstancesIndirect(in RayTracingMeshInstanceConfig config, GraphicsBuffer instanceMatrices, uint maxInstanceCount, GraphicsBuffer argsBuffer, uint argsOffset, uint id = uint.MaxValue)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddMeshInstancesIndirect_Injected(intPtr, in config, (instanceMatrices == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(instanceMatrices), maxInstanceCount, (argsBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer), argsOffset, id);
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::AddGeometryInstancesIndirect", HasExplicitThis = true)]
	private int AddGeometryInstancesIndirect(in RayTracingGeometryInstanceConfig config, GraphicsBuffer instanceMatrices, uint maxInstanceCount, GraphicsBuffer argsBuffer, uint argsOffset, uint id = uint.MaxValue)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddGeometryInstancesIndirect_Injected(intPtr, in config, (instanceMatrices == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(instanceMatrices), maxInstanceCount, (argsBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer), argsOffset, id);
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::AddMultiGeometryInstancesIndirect", HasExplicitThis = true)]
	private int AddMultiGeometryInstancesIndirect(in RayTracingMultiGeometryInstanceConfig config, GraphicsBuffer instanceData, GraphicsBuffer instanceIndices, int instanceSize, int objectToWorldOffset, int materialIndexOffset, int geometryIndexOffset, uint maxInstanceCount, GraphicsBuffer argsBuffer, uint argsOffset, uint id = uint.MaxValue)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddMultiGeometryInstancesIndirect_Injected(intPtr, in config, (instanceData == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(instanceData), (instanceIndices == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(instanceIndices), instanceSize, objectToWorldOffset, materialIndexOffset, geometryIndexOffset, maxInstanceCount, (argsBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer), argsOffset, id);
	}

	[FreeFunction("RayTracingAccelerationStructure_Bindings::AddAABBsInstance", HasExplicitThis = true)]
	private int AddAABBsInstance(RayTracingAABBsInstanceConfig config, Matrix4x4 matrix, uint id = uint.MaxValue)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return AddAABBsInstance_Injected(intPtr, ref config, ref matrix, id);
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::CullInstances", HasExplicitThis = true)]
	private RayTracingInstanceCullingResults Internal_CullInstances(in RayTracingInstanceCullingConfig cullingConfig)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_CullInstances_Injected(intPtr, in cullingConfig, out var ret);
		return ret;
	}

	[FreeFunction(Name = "RayTracingAccelerationStructure_Bindings::CheckMaterialEnablesInstancing")]
	private static bool CheckMaterialEnablesInstancing(Material material)
	{
		return CheckMaterialEnablesInstancing_Injected(Object.MarshalledUnityObject.Marshal(material));
	}

	[Obsolete("Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build()", true)]
	public void Update()
	{
		new NotSupportedException("Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build()");
	}

	[Obsolete("Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build(*)", true)]
	public void Update(Vector3 relativeOrigin)
	{
		new NotSupportedException("Method Update is deprecated and it will be removed in Unity 2024.1. Use Build instead (UnityUpgradable) -> Build(*)");
	}

	[Obsolete("This AddInstance method is deprecated and will be removed and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding Renderers to the acceleration structure.", true)]
	public void AddInstance(Renderer targetRenderer, bool[] subMeshMask = null, bool[] subMeshTransparencyFlags = null, bool enableTriangleCulling = true, bool frontTriangleCounterClockwise = false, uint mask = 255u, uint id = uint.MaxValue)
	{
		new NotSupportedException("This AddInstance method is deprecated and will be removed and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding Renderers to the acceleration structure.");
	}

	[Obsolete("This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.", true)]
	public void AddInstance(GraphicsBuffer aabbBuffer, uint numElements, Material material, bool isCutOff, bool enableTriangleCulling = true, bool frontTriangleCounterClockwise = false, uint mask = 255u, bool reuseBounds = false, uint id = uint.MaxValue)
	{
		new NotSupportedException("This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.");
	}

	[Obsolete("This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.", true)]
	public int AddInstance(GraphicsBuffer aabbBuffer, uint aabbCount, bool dynamicData, Matrix4x4 matrix, Material material, bool opaqueMaterial, MaterialPropertyBlock properties, uint mask = 255u, uint id = uint.MaxValue)
	{
		throw new NotSupportedException("This AddInstance method is deprecated and it will be removed in Unity 2024.1. Please use the alternative AddInstance method for adding procedural geometry (AABBs) to the acceleration structure.");
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Build_Injected(IntPtr _unity_self, [In] ref BuildSettings buildSettings);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddVFXInstances_Injected(IntPtr _unity_self, IntPtr targetRenderer, ref ManagedSpanWrapper vfxSystemMasks);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveVFXInstances_Injected(IntPtr _unity_self, IntPtr targetRenderer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstancePropertyBlock_Injected(IntPtr _unity_self, int handle, IntPtr properties);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ulong GetSize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetInstanceCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearInstances_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveInstances_Injected(IntPtr _unity_self, int layerMask, RayTracingModeMask rayTracingModeMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Create_Injected([In] ref Settings desc);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Destroy_Injected(IntPtr accelStruct);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveInstance_Renderer_Injected(IntPtr _unity_self, IntPtr targetRenderer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveInstance_InstanceID_Injected(IntPtr _unity_self, int instanceID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstanceTransform_Renderer_Injected(IntPtr _unity_self, IntPtr renderer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstanceTransform_Handle_Injected(IntPtr _unity_self, int handle, [In] ref Matrix4x4 matrix);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstanceGeometry_Renderer_Injected(IntPtr _unity_self, IntPtr renderer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstanceGeometry_Handle_Injected(IntPtr _unity_self, int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstanceMask_Renderer_Injected(IntPtr _unity_self, IntPtr renderer, uint mask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstanceMask_Handle_Injected(IntPtr _unity_self, int handle, uint mask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstanceID_Renderer_Injected(IntPtr _unity_self, IntPtr renderer, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UpdateInstanceID_Handle_Injected(IntPtr _unity_self, int handle, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddInstanceSubMeshFlagsArray_Injected(IntPtr _unity_self, IntPtr targetRenderer, ref ManagedSpanWrapper subMeshFlags, bool enableTriangleCulling, bool frontTriangleCounterClockwise, uint mask, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int AddMeshInstance_Injected(IntPtr _unity_self, [In] ref RayTracingMeshInstanceConfig config, [In] ref Matrix4x4 matrix, Matrix4x4* prevMatrix, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int AddGeometryInstance_Injected(IntPtr _unity_self, in RayTracingGeometryInstanceConfig config, [In] ref Matrix4x4 matrix, Matrix4x4* prevMatrix, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddMeshInstances_Injected(IntPtr _unity_self, [In] ref RayTracingMeshInstanceConfig config, IntPtr instancedData, [In] ref RenderInstancedDataLayout layout, uint instanceCount, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddMeshInstancesIndirect_Injected(IntPtr _unity_self, in RayTracingMeshInstanceConfig config, IntPtr instanceMatrices, uint maxInstanceCount, IntPtr argsBuffer, uint argsOffset, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddGeometryInstancesIndirect_Injected(IntPtr _unity_self, in RayTracingGeometryInstanceConfig config, IntPtr instanceMatrices, uint maxInstanceCount, IntPtr argsBuffer, uint argsOffset, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddMultiGeometryInstancesIndirect_Injected(IntPtr _unity_self, in RayTracingMultiGeometryInstanceConfig config, IntPtr instanceData, IntPtr instanceIndices, int instanceSize, int objectToWorldOffset, int materialIndexOffset, int geometryIndexOffset, uint maxInstanceCount, IntPtr argsBuffer, uint argsOffset, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int AddAABBsInstance_Injected(IntPtr _unity_self, [In] ref RayTracingAABBsInstanceConfig config, [In] ref Matrix4x4 matrix, uint id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CullInstances_Injected(IntPtr _unity_self, in RayTracingInstanceCullingConfig cullingConfig, out RayTracingInstanceCullingResults ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CheckMaterialEnablesInstancing_Injected(IntPtr material);
}
