#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Camera/GPUDrivenProcessor.h")]
[RequiredByNativeCode]
internal class GPUDrivenProcessor
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(GPUDrivenProcessor obj)
		{
			return obj.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	private unsafe static GPUDrivenRendererDataNativeCallback s_NativeRendererCallback = delegate(in GPUDrivenRendererGroupDataNative nativeData, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback callback)
	{
		NativeArray<int> rendererGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.rendererGroupID, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<Bounds> localBounds = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Bounds>(nativeData.localBounds, (nativeData.localBounds != null) ? nativeData.rendererGroupCount : 0, Allocator.Invalid);
		NativeArray<Vector4> lightmapScaleOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(nativeData.lightmapScaleOffset, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<int> gameObjectLayer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.gameObjectLayer, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<uint> renderingLayerMask = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<uint>(nativeData.renderingLayerMask, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<int> lodGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.lodGroupID, (nativeData.lodGroupID != null) ? nativeData.rendererGroupCount : 0, Allocator.Invalid);
		NativeArray<int> lightmapIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.motionVecGenMode, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<GPUDrivenPackedRendererData> packedRendererData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenPackedRendererData>(nativeData.packedRendererData, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<int> rendererPriority = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.rendererPriority, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<int> meshIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.meshIndex, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<short> subMeshStartIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.subMeshStartIndex, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<int> materialsOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.materialsOffset, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<short> materialsCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.materialsCount, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<int> instancesOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
		NativeArray<int> instancesCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
		NativeArray<GPUDrivenRendererEditorData> editorData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenRendererEditorData>(nativeData.editorData, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<GPUDrivenRendererMeshLodData> meshLodData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenRendererMeshLodData>(nativeData.meshLodData, nativeData.rendererGroupCount, Allocator.Invalid);
		NativeArray<int> invalidRendererGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.invalidRendererGroupID, nativeData.invalidRendererGroupIDCount, Allocator.Invalid);
		NativeArray<Matrix4x4> localToWorldMatrix = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(nativeData.localToWorldMatrix, (nativeData.localToWorldMatrix != null) ? nativeData.rendererGroupCount : 0, Allocator.Invalid);
		NativeArray<Matrix4x4> prevLocalToWorldMatrix = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(nativeData.prevLocalToWorldMatrix, (nativeData.prevLocalToWorldMatrix != null) ? nativeData.rendererGroupCount : 0, Allocator.Invalid);
		NativeArray<int> rendererGroupIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
		NativeArray<int> meshID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.meshID, nativeData.meshCount, Allocator.Invalid);
		NativeArray<GPUDrivenMeshLodInfo> meshLodInfo = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenMeshLodInfo>(nativeData.meshLodInfo, nativeData.meshCount, Allocator.Invalid);
		NativeArray<short> subMeshCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.subMeshCount, nativeData.meshCount, Allocator.Invalid);
		NativeArray<int> subMeshDescOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.subMeshDescOffset, nativeData.meshCount, Allocator.Invalid);
		NativeArray<SubMeshDescriptor> subMeshDesc = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<SubMeshDescriptor>(nativeData.subMeshDesc, nativeData.subMeshDescCount, Allocator.Invalid);
		NativeArray<int> materialIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.materialIndex, nativeData.materialIndexCount, Allocator.Invalid);
		NativeArray<int> materialID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.materialID, nativeData.materialCount, Allocator.Invalid);
		NativeArray<GPUDrivenPackedMaterialData> packedMaterialData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenPackedMaterialData>(nativeData.packedMaterialData, (nativeData.packedMaterialData != null) ? nativeData.materialCount : 0, Allocator.Invalid);
		NativeArray<int> materialFilterFlags = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.materialFilterFlags, (nativeData.packedMaterialData != null) ? nativeData.materialCount : 0, Allocator.Invalid);
		callback(new GPUDrivenRendererGroupData
		{
			rendererGroupID = rendererGroupID,
			localBounds = localBounds,
			lightmapScaleOffset = lightmapScaleOffset,
			gameObjectLayer = gameObjectLayer,
			renderingLayerMask = renderingLayerMask,
			lodGroupID = lodGroupID,
			lightmapIndex = lightmapIndex,
			packedRendererData = packedRendererData,
			rendererPriority = rendererPriority,
			meshIndex = meshIndex,
			subMeshStartIndex = subMeshStartIndex,
			materialsOffset = materialsOffset,
			materialsCount = materialsCount,
			instancesOffset = instancesOffset,
			instancesCount = instancesCount,
			editorData = editorData,
			invalidRendererGroupID = invalidRendererGroupID,
			meshLodData = meshLodData,
			localToWorldMatrix = localToWorldMatrix,
			prevLocalToWorldMatrix = prevLocalToWorldMatrix,
			rendererGroupIndex = rendererGroupIndex,
			meshID = meshID,
			meshLodInfo = meshLodInfo,
			subMeshCount = subMeshCount,
			subMeshDescOffset = subMeshDescOffset,
			subMeshDesc = subMeshDesc,
			materialIndex = materialIndex,
			materialID = materialID,
			packedMaterialData = packedMaterialData,
			materialFilterFlags = materialFilterFlags
		}, meshes, materials);
	};

	private unsafe static GPUDrivenLODGroupDataNativeCallback s_NativeLODGroupCallback = delegate(in GPUDrivenLODGroupDataNative nativeData, GPUDrivenLODGroupDataCallback callback)
	{
		NativeArray<int> lodGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.lodGroupID, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<int> lodOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.lodOffset, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<int> lodCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.lodCount, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<LODFadeMode> fadeMode = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<LODFadeMode>(nativeData.fadeMode, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<Vector3> worldSpaceReferencePoint = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(nativeData.worldSpaceReferencePoint, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<float> worldSpaceSize = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(nativeData.worldSpaceSize, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<short> renderersCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.renderersCount, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<bool> lastLODIsBillboard = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<bool>(nativeData.lastLODIsBillboard, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<byte> forceLODMask = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(nativeData.forceLODMask, nativeData.lodGroupCount, Allocator.Invalid);
		NativeArray<int> invalidLODGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(nativeData.invalidLODGroupID, nativeData.invalidLODGroupCount, Allocator.Invalid);
		NativeArray<short> lodRenderersCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>(nativeData.lodRenderersCount, nativeData.lodDataCount, Allocator.Invalid);
		NativeArray<float> lodScreenRelativeTransitionHeight = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(nativeData.lodScreenRelativeTransitionHeight, nativeData.lodDataCount, Allocator.Invalid);
		NativeArray<float> lodFadeTransitionWidth = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(nativeData.lodFadeTransitionWidth, nativeData.lodDataCount, Allocator.Invalid);
		GPUDrivenLODGroupData lodGroupData = new GPUDrivenLODGroupData
		{
			lodGroupID = lodGroupID,
			lodOffset = lodOffset,
			lodCount = lodCount,
			fadeMode = fadeMode,
			worldSpaceReferencePoint = worldSpaceReferencePoint,
			worldSpaceSize = worldSpaceSize,
			renderersCount = renderersCount,
			lastLODIsBillboard = lastLODIsBillboard,
			forceLODMask = forceLODMask,
			invalidLODGroupID = invalidLODGroupID,
			lodRenderersCount = lodRenderersCount,
			lodScreenRelativeTransitionHeight = lodScreenRelativeTransitionHeight,
			lodFadeTransitionWidth = lodFadeTransitionWidth
		};
		callback(in lodGroupData);
	};

	internal List<Mesh> scratchMeshes { get; private set; }

	internal List<Material> scratchMaterials { get; private set; }

	public bool enablePartialRendering
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enablePartialRendering_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enablePartialRendering_Injected(intPtr, value);
		}
	}

	public bool enableMaterialFilters
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enableMaterialFilters_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enableMaterialFilters_Injected(intPtr, value);
		}
	}

	public GPUDrivenProcessor()
	{
		m_Ptr = Internal_Create();
		scratchMeshes = new List<Mesh>();
		scratchMaterials = new List<Material>();
	}

	~GPUDrivenProcessor()
	{
		Destroy();
	}

	public void Dispose()
	{
		scratchMeshes = null;
		scratchMaterials = null;
		Destroy();
		GC.SuppressFinalize(this);
	}

	private void Destroy()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_Create();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Destroy(IntPtr ptr);

	private unsafe void EnableGPUDrivenRenderingAndDispatchRendererData(ReadOnlySpan<int> renderersID, GPUDrivenRendererDataNativeCallback callback, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback param, bool materialUpdateOnly)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<int> readOnlySpan = renderersID;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper renderersID2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			EnableGPUDrivenRenderingAndDispatchRendererData_Injected(intPtr, ref renderersID2, callback, meshes, materials, param, materialUpdateOnly);
		}
	}

	public void EnableGPUDrivenRenderingAndDispatchRendererData(ReadOnlySpan<int> renderersID, GPUDrivenRendererDataCallback callback, bool materialUpdateOnly = false)
	{
		scratchMeshes.Clear();
		scratchMaterials.Clear();
		EnableGPUDrivenRenderingAndDispatchRendererData(renderersID, s_NativeRendererCallback, scratchMeshes, scratchMaterials, callback, materialUpdateOnly);
	}

	public unsafe void DisableGPUDrivenRendering(ReadOnlySpan<int> renderersID)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<int> readOnlySpan = renderersID;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper renderersID2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			DisableGPUDrivenRendering_Injected(intPtr, ref renderersID2);
		}
	}

	private unsafe void DispatchLODGroupData(ReadOnlySpan<int> lodGroupID, GPUDrivenLODGroupDataNativeCallback callback, GPUDrivenLODGroupDataCallback param)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<int> readOnlySpan = lodGroupID;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper lodGroupID2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			DispatchLODGroupData_Injected(intPtr, ref lodGroupID2, callback, param);
		}
	}

	public void DispatchLODGroupData(ReadOnlySpan<int> lodGroupID, GPUDrivenLODGroupDataCallback callback)
	{
		DispatchLODGroupData(lodGroupID, s_NativeLODGroupCallback, callback);
	}

	public void AddMaterialFilters([NotNull] GPUDrivenMaterialFilterEntry[] filters)
	{
		if (filters == null)
		{
			ThrowHelper.ThrowArgumentNullException(filters, "filters");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddMaterialFilters_Injected(intPtr, filters);
	}

	public void ClearMaterialFilters()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearMaterialFilters_Injected(intPtr);
	}

	public int GetMaterialFilterFlags(Material material)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetMaterialFilterFlags_Injected(intPtr, Object.MarshalledUnityObject.Marshal(material));
	}

	[FreeFunction("GPUDrivenProcessor::ClassifyMaterials", IsThreadSafe = true)]
	private unsafe static int ClassifyMaterialsImpl(ReadOnlySpan<EntityId> materialIDs, Span<EntityId> unsupportedMaterialIDs, Span<EntityId> supportedMaterialIDs, Span<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
	{
		ReadOnlySpan<EntityId> readOnlySpan = materialIDs;
		int result;
		fixed (EntityId* begin = readOnlySpan)
		{
			ManagedSpanWrapper materialIDs2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<EntityId> span = unsupportedMaterialIDs;
			fixed (EntityId* begin2 = span)
			{
				ManagedSpanWrapper unsupportedMaterialIDs2 = new ManagedSpanWrapper(begin2, span.Length);
				Span<EntityId> span2 = supportedMaterialIDs;
				fixed (EntityId* begin3 = span2)
				{
					ManagedSpanWrapper supportedMaterialIDs2 = new ManagedSpanWrapper(begin3, span2.Length);
					Span<GPUDrivenPackedMaterialData> span3 = supportedPackedMaterialDatas;
					fixed (GPUDrivenPackedMaterialData* begin4 = span3)
					{
						ManagedSpanWrapper supportedPackedMaterialDatas2 = new ManagedSpanWrapper(begin4, span3.Length);
						result = ClassifyMaterialsImpl_Injected(ref materialIDs2, ref unsupportedMaterialIDs2, ref supportedMaterialIDs2, ref supportedPackedMaterialDatas2);
					}
				}
			}
		}
		return result;
	}

	public static int ClassifyMaterials(NativeArray<EntityId> materialIDs, NativeArray<EntityId> unsupportedMaterialIDs, NativeArray<EntityId> supportedMaterialIDs, NativeArray<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
	{
		return ClassifyMaterialsImpl(materialIDs, unsupportedMaterialIDs, supportedMaterialIDs, supportedPackedMaterialDatas);
	}

	public unsafe static int ClassifyMaterials(NativeArray<int> materialIDs, NativeArray<int> unsupportedMaterialIDs, NativeArray<int> supportedMaterialIDs, NativeArray<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
	{
		Assert.IsTrue(4 == sizeof(EntityId));
		return ClassifyMaterialsImpl(materialIDs.Reinterpret<EntityId>(), unsupportedMaterialIDs.Reinterpret<EntityId>(), supportedMaterialIDs.Reinterpret<EntityId>(), supportedPackedMaterialDatas);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableGPUDrivenRenderingAndDispatchRendererData_Injected(IntPtr _unity_self, ref ManagedSpanWrapper renderersID, GPUDrivenRendererDataNativeCallback callback, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback param, bool materialUpdateOnly);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableGPUDrivenRendering_Injected(IntPtr _unity_self, ref ManagedSpanWrapper renderersID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DispatchLODGroupData_Injected(IntPtr _unity_self, ref ManagedSpanWrapper lodGroupID, GPUDrivenLODGroupDataNativeCallback callback, GPUDrivenLODGroupDataCallback param);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enablePartialRendering_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enablePartialRendering_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enableMaterialFilters_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enableMaterialFilters_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddMaterialFilters_Injected(IntPtr _unity_self, GPUDrivenMaterialFilterEntry[] filters);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearMaterialFilters_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMaterialFilterFlags_Injected(IntPtr _unity_self, IntPtr material);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ClassifyMaterialsImpl_Injected(ref ManagedSpanWrapper materialIDs, ref ManagedSpanWrapper unsupportedMaterialIDs, ref ManagedSpanWrapper supportedMaterialIDs, ref ManagedSpanWrapper supportedPackedMaterialDatas);
}
