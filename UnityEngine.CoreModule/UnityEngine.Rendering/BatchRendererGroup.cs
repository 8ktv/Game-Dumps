using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
[NativeHeader("Runtime/Math/Matrix4x4.h")]
public class BatchRendererGroup : IDisposable
{
	public delegate JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext, BatchCullingOutput cullingOutput, IntPtr userContext);

	public delegate void OnFinishedCulling(IntPtr customCullingResult);

	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(BatchRendererGroup batchRendererGroup)
		{
			return batchRendererGroup.m_GroupHandle;
		}
	}

	private IntPtr m_GroupHandle = IntPtr.Zero;

	private OnPerformCulling m_PerformCulling;

	private OnFinishedCulling m_FinishedCulling;

	public static BatchBufferTarget BufferTarget => GetBufferTarget();

	public unsafe BatchRendererGroup(OnPerformCulling cullingCallback, IntPtr userContext)
	{
		m_PerformCulling = cullingCallback;
		m_GroupHandle = Create(this, (void*)userContext);
	}

	public unsafe BatchRendererGroup(BatchRendererGroupCreateInfo info)
	{
		m_PerformCulling = info.cullingCallback;
		m_GroupHandle = Create(this, (void*)info.userContext);
		m_FinishedCulling = info.finishedCullingCallback;
	}

	public void Dispose()
	{
		Destroy(m_GroupHandle);
		m_GroupHandle = IntPtr.Zero;
	}

	public ThreadedBatchContext GetThreadedBatchContext()
	{
		return new ThreadedBatchContext
		{
			batchRendererGroup = m_GroupHandle
		};
	}

	private BatchID AddDrawCommandBatch(IntPtr values, int count, GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddDrawCommandBatch_Injected(intPtr, values, count, ref buffer, bufferOffset, windowSize, out var ret);
		return ret;
	}

	public unsafe BatchID AddBatch(NativeArray<MetadataValue> batchMetadata, GraphicsBufferHandle buffer)
	{
		return AddDrawCommandBatch((IntPtr)batchMetadata.GetUnsafeReadOnlyPtr(), batchMetadata.Length, buffer, 0u, 0u);
	}

	public unsafe BatchID AddBatch(NativeArray<MetadataValue> batchMetadata, GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize)
	{
		return AddDrawCommandBatch((IntPtr)batchMetadata.GetUnsafeReadOnlyPtr(), batchMetadata.Length, buffer, bufferOffset, windowSize);
	}

	private void RemoveDrawCommandBatch(BatchID batchID)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RemoveDrawCommandBatch_Injected(intPtr, ref batchID);
	}

	public void RemoveBatch(BatchID batchID)
	{
		RemoveDrawCommandBatch(batchID);
	}

	private void SetDrawCommandBatchBuffer(BatchID batchID, GraphicsBufferHandle buffer)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDrawCommandBatchBuffer_Injected(intPtr, ref batchID, ref buffer);
	}

	public void SetBatchBuffer(BatchID batchID, GraphicsBufferHandle buffer)
	{
		SetDrawCommandBatchBuffer(batchID, buffer);
	}

	public BatchMaterialID RegisterMaterial(Material material)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RegisterMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal(material), out var ret);
		return ret;
	}

	internal unsafe void RegisterMaterials(ReadOnlySpan<int> materialID, Span<BatchMaterialID> batchMaterialID)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<int> readOnlySpan = materialID;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper materialID2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<BatchMaterialID> span = batchMaterialID;
			fixed (BatchMaterialID* begin2 = span)
			{
				ManagedSpanWrapper batchMaterialID2 = new ManagedSpanWrapper(begin2, span.Length);
				RegisterMaterials_Injected(intPtr, ref materialID2, ref batchMaterialID2);
			}
		}
	}

	public void UnregisterMaterial(BatchMaterialID material)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UnregisterMaterial_Injected(intPtr, ref material);
	}

	public Material GetRegisteredMaterial(BatchMaterialID material)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Material>(GetRegisteredMaterial_Injected(intPtr, ref material));
	}

	public BatchMeshID RegisterMesh(Mesh mesh)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RegisterMesh_Injected(intPtr, Object.MarshalledUnityObject.Marshal(mesh), out var ret);
		return ret;
	}

	internal unsafe void RegisterMeshes(ReadOnlySpan<int> meshID, Span<BatchMeshID> batchMeshID)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<int> readOnlySpan = meshID;
		fixed (int* begin = readOnlySpan)
		{
			ManagedSpanWrapper meshID2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Span<BatchMeshID> span = batchMeshID;
			fixed (BatchMeshID* begin2 = span)
			{
				ManagedSpanWrapper batchMeshID2 = new ManagedSpanWrapper(begin2, span.Length);
				RegisterMeshes_Injected(intPtr, ref meshID2, ref batchMeshID2);
			}
		}
	}

	public void UnregisterMesh(BatchMeshID mesh)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UnregisterMesh_Injected(intPtr, ref mesh);
	}

	public Mesh GetRegisteredMesh(BatchMeshID mesh)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Mesh>(GetRegisteredMesh_Injected(intPtr, ref mesh));
	}

	public void SetGlobalBounds(Bounds bounds)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetGlobalBounds_Injected(intPtr, ref bounds);
	}

	public void SetPickingMaterial(Material material)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPickingMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal(material));
	}

	public void SetErrorMaterial(Material material)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetErrorMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal(material));
	}

	public void SetLoadingMaterial(Material material)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLoadingMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal(material));
	}

	public unsafe void SetEnabledViewTypes(BatchCullingViewType[] viewTypes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<BatchCullingViewType> span = new Span<BatchCullingViewType>(viewTypes);
		fixed (BatchCullingViewType* begin = span)
		{
			ManagedSpanWrapper viewTypes2 = new ManagedSpanWrapper(begin, span.Length);
			SetEnabledViewTypes_Injected(intPtr, ref viewTypes2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern BatchBufferTarget GetBufferTarget();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int GetConstantBufferMaxWindowSize();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int GetConstantBufferOffsetAlignment();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern IntPtr Create([Unmarshalled] BatchRendererGroup group, void* userContext);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Destroy(IntPtr groupHandle);

	[RequiredByNativeCode]
	private unsafe static void InvokeOnPerformCulling(BatchRendererGroup group, ref BatchRendererCullingOutput context, ref LODParameters lodParameters, IntPtr userContext)
	{
		NativeArray<Plane> inCullingPlanes = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Plane>(context.cullingPlanes, context.cullingPlaneCount, Allocator.Invalid);
		NativeArray<CullingSplit> inCullingSplits = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<CullingSplit>(context.cullingSplits, context.cullingSplitCount, Allocator.Invalid);
		NativeArray<BatchCullingOutputDrawCommands> drawCommands = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BatchCullingOutputDrawCommands>(context.drawCommands, 1, Allocator.Invalid);
		try
		{
			BatchCullingOutput cullingOutput = new BatchCullingOutput
			{
				drawCommands = drawCommands,
				customCullingResult = new NativeArray<IntPtr>(1, Allocator.Temp)
			};
			context.cullingJobsFence = group.m_PerformCulling(group, new BatchCullingContext(inCullingPlanes, inCullingSplits, lodParameters, context.localToWorldMatrix, context.viewType, context.projectionType, context.cullingFlags, context.viewID, context.cullingLayerMask, context.sceneCullingMask, context.splitExclusionMask, context.receiverPlaneOffset, context.receiverPlaneCount, context.occlusionBuffer), cullingOutput, userContext);
			context.customCullingResult = cullingOutput.customCullingResult[0];
		}
		finally
		{
			JobHandle.ScheduleBatchedJobs();
		}
	}

	[RequiredByNativeCode]
	private static void InvokeOnFinishedCulling(BatchRendererGroup group, IntPtr customCullingResult)
	{
		try
		{
			if (group.m_FinishedCulling != null)
			{
				group.m_FinishedCulling(customCullingResult);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	[FreeFunction("BatchRendererGroup::OcclusionTestAABB", IsThreadSafe = true)]
	internal static bool OcclusionTestAABB(IntPtr occlusionBuffer, Bounds aabb)
	{
		return OcclusionTestAABB_Injected(occlusionBuffer, ref aabb);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddDrawCommandBatch_Injected(IntPtr _unity_self, IntPtr values, int count, [In] ref GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize, out BatchID ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RemoveDrawCommandBatch_Injected(IntPtr _unity_self, [In] ref BatchID batchID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDrawCommandBatchBuffer_Injected(IntPtr _unity_self, [In] ref BatchID batchID, [In] ref GraphicsBufferHandle buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RegisterMaterial_Injected(IntPtr _unity_self, IntPtr material, out BatchMaterialID ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RegisterMaterials_Injected(IntPtr _unity_self, ref ManagedSpanWrapper materialID, ref ManagedSpanWrapper batchMaterialID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UnregisterMaterial_Injected(IntPtr _unity_self, [In] ref BatchMaterialID material);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetRegisteredMaterial_Injected(IntPtr _unity_self, [In] ref BatchMaterialID material);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RegisterMesh_Injected(IntPtr _unity_self, IntPtr mesh, out BatchMeshID ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RegisterMeshes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper meshID, ref ManagedSpanWrapper batchMeshID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UnregisterMesh_Injected(IntPtr _unity_self, [In] ref BatchMeshID mesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetRegisteredMesh_Injected(IntPtr _unity_self, [In] ref BatchMeshID mesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalBounds_Injected(IntPtr _unity_self, [In] ref Bounds bounds);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPickingMaterial_Injected(IntPtr _unity_self, IntPtr material);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetErrorMaterial_Injected(IntPtr _unity_self, IntPtr material);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLoadingMaterial_Injected(IntPtr _unity_self, IntPtr material);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetEnabledViewTypes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper viewTypes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool OcclusionTestAABB_Injected(IntPtr occlusionBuffer, [In] ref Bounds aabb);
}
