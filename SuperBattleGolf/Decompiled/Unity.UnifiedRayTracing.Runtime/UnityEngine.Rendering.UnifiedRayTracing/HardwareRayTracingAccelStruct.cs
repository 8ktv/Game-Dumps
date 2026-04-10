using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.Rendering.UnifiedRayTracing;

internal sealed class HardwareRayTracingAccelStruct : IRayTracingAccelStruct, IDisposable
{
	private readonly RayTracingAccelerationStructureBuildFlags m_BuildFlags;

	private readonly Dictionary<int, Mesh> m_Meshes = new Dictionary<int, Mesh>();

	private readonly ReferenceCounter m_Counter;

	public RayTracingAccelerationStructure accelStruct { get; }

	internal HardwareRayTracingAccelStruct(AccelerationStructureOptions options, ReferenceCounter counter)
	{
		m_BuildFlags = (RayTracingAccelerationStructureBuildFlags)options.buildFlags;
		accelStruct = new RayTracingAccelerationStructure(new RayTracingAccelerationStructure.Settings
		{
			rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything,
			managementMode = RayTracingAccelerationStructure.ManagementMode.Manual,
			enableCompaction = false,
			layerMask = 255,
			buildFlagsStaticGeometries = m_BuildFlags
		});
		m_Counter = counter;
		m_Counter.Inc();
	}

	public void Dispose()
	{
		m_Counter.Dec();
		accelStruct?.Dispose();
	}

	public int AddInstance(MeshInstanceDesc meshInstance)
	{
		RayTracingMeshInstanceConfig config = new RayTracingMeshInstanceConfig(meshInstance.mesh, (uint)meshInstance.subMeshIndex, null);
		config.mask = meshInstance.mask;
		config.enableTriangleCulling = meshInstance.enableTriangleCulling;
		config.frontTriangleCounterClockwise = meshInstance.frontTriangleCounterClockwise;
		config.subMeshFlags = (meshInstance.opaqueGeometry ? (RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.ClosestHitOnly) : (RayTracingSubMeshFlags.Enabled | RayTracingSubMeshFlags.UniqueAnyHitCalls));
		int num = accelStruct.AddInstance(in config, meshInstance.localToWorldMatrix, null, meshInstance.instanceID);
		if (meshInstance.instanceID == uint.MaxValue)
		{
			accelStruct.UpdateInstanceID(num, (uint)num);
		}
		m_Meshes.Add(num, meshInstance.mesh);
		return num;
	}

	public void RemoveInstance(int instanceHandle)
	{
		m_Meshes.Remove(instanceHandle);
		accelStruct.RemoveInstance(instanceHandle);
	}

	public void ClearInstances()
	{
		m_Meshes.Clear();
		accelStruct.ClearInstances();
	}

	public void UpdateInstanceTransform(int instanceHandle, Matrix4x4 localToWorldMatrix)
	{
		accelStruct.UpdateInstanceTransform(instanceHandle, localToWorldMatrix);
	}

	public void UpdateInstanceID(int instanceHandle, uint instanceID)
	{
		accelStruct.UpdateInstanceID(instanceHandle, instanceID);
	}

	public void UpdateInstanceMask(int instanceHandle, uint mask)
	{
		accelStruct.UpdateInstanceMask(instanceHandle, mask);
	}

	public void Build(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
	{
		RayTracingAccelerationStructure.BuildSettings buildSettings = new RayTracingAccelerationStructure.BuildSettings();
		buildSettings.buildFlags = m_BuildFlags;
		buildSettings.relativeOrigin = Vector3.zero;
		RayTracingAccelerationStructure.BuildSettings buildSettings2 = buildSettings;
		cmd.BuildRayTracingAccelerationStructure(accelStruct, buildSettings2);
	}

	public ulong GetBuildScratchBufferRequiredSizeInBytes()
	{
		return 0uL;
	}

	[Conditional("UNITY_ASSERTIONS")]
	private void CheckInstanceHandleIsValid(int instanceHandle)
	{
	}
}
