using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Rendering;

[BurstCompile]
internal static class GPUResidentDrawerBurst
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void ClassifyMaterials_000000EA_0024PostfixBurstDelegate(in NativeArray<EntityId> materialIDs, in NativeParallelHashMap<EntityId, BatchMaterialID>.ReadOnly batchMaterialHash, ref NativeList<EntityId> supportedMaterialIDs, ref NativeList<EntityId> unsupportedMaterialIDs, ref NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas);

	internal static class ClassifyMaterials_000000EA_0024BurstDirectCall
	{
		private static IntPtr Pointer;

		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr P_0)
		{
			if (Pointer == (IntPtr)0)
			{
				Pointer = BurstCompiler.CompileFunctionPointer<ClassifyMaterials_000000EA_0024PostfixBurstDelegate>(ClassifyMaterials).Value;
			}
			P_0 = Pointer;
		}

		private static IntPtr GetFunctionPointer()
		{
			nint result = 0;
			GetFunctionPointerDiscard(ref result);
			return result;
		}

		public unsafe static void Invoke(in NativeArray<EntityId> materialIDs, in NativeParallelHashMap<EntityId, BatchMaterialID>.ReadOnly batchMaterialHash, ref NativeList<EntityId> supportedMaterialIDs, ref NativeList<EntityId> unsupportedMaterialIDs, ref NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = GetFunctionPointer();
				if (functionPointer != (IntPtr)0)
				{
					((delegate* unmanaged[Cdecl]<ref NativeArray<EntityId>, ref NativeParallelHashMap<EntityId, BatchMaterialID>.ReadOnly, ref NativeList<EntityId>, ref NativeList<EntityId>, ref NativeList<GPUDrivenPackedMaterialData>, void>)functionPointer)(ref materialIDs, ref batchMaterialHash, ref supportedMaterialIDs, ref unsupportedMaterialIDs, ref supportedPackedMaterialDatas);
					return;
				}
			}
			ClassifyMaterials_0024BurstManaged(in materialIDs, in batchMaterialHash, ref supportedMaterialIDs, ref unsupportedMaterialIDs, ref supportedPackedMaterialDatas);
		}
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void FindUnsupportedRenderers_000000EB_0024PostfixBurstDelegate(in NativeArray<EntityId> unsupportedMaterials, in NativeArray<SmallEntityIdArray>.ReadOnly materialIDArrays, in NativeArray<EntityId>.ReadOnly rendererGroups, ref NativeList<EntityId> unsupportedRenderers);

	internal static class FindUnsupportedRenderers_000000EB_0024BurstDirectCall
	{
		private static IntPtr Pointer;

		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr P_0)
		{
			if (Pointer == (IntPtr)0)
			{
				Pointer = BurstCompiler.CompileFunctionPointer<FindUnsupportedRenderers_000000EB_0024PostfixBurstDelegate>(FindUnsupportedRenderers).Value;
			}
			P_0 = Pointer;
		}

		private static IntPtr GetFunctionPointer()
		{
			nint result = 0;
			GetFunctionPointerDiscard(ref result);
			return result;
		}

		public unsafe static void Invoke(in NativeArray<EntityId> unsupportedMaterials, in NativeArray<SmallEntityIdArray>.ReadOnly materialIDArrays, in NativeArray<EntityId>.ReadOnly rendererGroups, ref NativeList<EntityId> unsupportedRenderers)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = GetFunctionPointer();
				if (functionPointer != (IntPtr)0)
				{
					((delegate* unmanaged[Cdecl]<ref NativeArray<EntityId>, ref NativeArray<SmallEntityIdArray>.ReadOnly, ref NativeArray<EntityId>.ReadOnly, ref NativeList<EntityId>, void>)functionPointer)(ref unsupportedMaterials, ref materialIDArrays, ref rendererGroups, ref unsupportedRenderers);
					return;
				}
			}
			FindUnsupportedRenderers_0024BurstManaged(in unsupportedMaterials, in materialIDArrays, in rendererGroups, ref unsupportedRenderers);
		}
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void GetMaterialsWithChangedPackedMaterial_000000EC_0024PostfixBurstDelegate(in NativeArray<EntityId> materialIDs, in NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, in NativeParallelHashMap<EntityId, GPUDrivenPackedMaterialData>.ReadOnly packedMaterialHash, ref NativeHashSet<EntityId> filteredMaterials);

	internal static class GetMaterialsWithChangedPackedMaterial_000000EC_0024BurstDirectCall
	{
		private static IntPtr Pointer;

		[BurstDiscard]
		private static void GetFunctionPointerDiscard(ref IntPtr P_0)
		{
			if (Pointer == (IntPtr)0)
			{
				Pointer = BurstCompiler.CompileFunctionPointer<GetMaterialsWithChangedPackedMaterial_000000EC_0024PostfixBurstDelegate>(GetMaterialsWithChangedPackedMaterial).Value;
			}
			P_0 = Pointer;
		}

		private static IntPtr GetFunctionPointer()
		{
			nint result = 0;
			GetFunctionPointerDiscard(ref result);
			return result;
		}

		public unsafe static void Invoke(in NativeArray<EntityId> materialIDs, in NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, in NativeParallelHashMap<EntityId, GPUDrivenPackedMaterialData>.ReadOnly packedMaterialHash, ref NativeHashSet<EntityId> filteredMaterials)
		{
			if (BurstCompiler.IsEnabled)
			{
				IntPtr functionPointer = GetFunctionPointer();
				if (functionPointer != (IntPtr)0)
				{
					((delegate* unmanaged[Cdecl]<ref NativeArray<EntityId>, ref NativeArray<GPUDrivenPackedMaterialData>, ref NativeParallelHashMap<EntityId, GPUDrivenPackedMaterialData>.ReadOnly, ref NativeHashSet<EntityId>, void>)functionPointer)(ref materialIDs, ref packedMaterialDatas, ref packedMaterialHash, ref filteredMaterials);
					return;
				}
			}
			GetMaterialsWithChangedPackedMaterial_0024BurstManaged(in materialIDs, in packedMaterialDatas, in packedMaterialHash, ref filteredMaterials);
		}
	}

	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	[MonoPInvokeCallback(typeof(UnityEngine_002ERendering_002EClassifyMaterials_000000EA_0024PostfixBurstDelegate))]
	public static void ClassifyMaterials(in NativeArray<EntityId> materialIDs, in NativeParallelHashMap<EntityId, BatchMaterialID>.ReadOnly batchMaterialHash, ref NativeList<EntityId> supportedMaterialIDs, ref NativeList<EntityId> unsupportedMaterialIDs, ref NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
	{
		ClassifyMaterials_000000EA_0024BurstDirectCall.Invoke(in materialIDs, in batchMaterialHash, ref supportedMaterialIDs, ref unsupportedMaterialIDs, ref supportedPackedMaterialDatas);
	}

	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	[MonoPInvokeCallback(typeof(UnityEngine_002ERendering_002EFindUnsupportedRenderers_000000EB_0024PostfixBurstDelegate))]
	public static void FindUnsupportedRenderers(in NativeArray<EntityId> unsupportedMaterials, in NativeArray<SmallEntityIdArray>.ReadOnly materialIDArrays, in NativeArray<EntityId>.ReadOnly rendererGroups, ref NativeList<EntityId> unsupportedRenderers)
	{
		FindUnsupportedRenderers_000000EB_0024BurstDirectCall.Invoke(in unsupportedMaterials, in materialIDArrays, in rendererGroups, ref unsupportedRenderers);
	}

	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	[MonoPInvokeCallback(typeof(UnityEngine_002ERendering_002EGetMaterialsWithChangedPackedMaterial_000000EC_0024PostfixBurstDelegate))]
	public static void GetMaterialsWithChangedPackedMaterial(in NativeArray<EntityId> materialIDs, in NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, in NativeParallelHashMap<EntityId, GPUDrivenPackedMaterialData>.ReadOnly packedMaterialHash, ref NativeHashSet<EntityId> filteredMaterials)
	{
		GetMaterialsWithChangedPackedMaterial_000000EC_0024BurstDirectCall.Invoke(in materialIDs, in packedMaterialDatas, in packedMaterialHash, ref filteredMaterials);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal static void ClassifyMaterials_0024BurstManaged(in NativeArray<EntityId> materialIDs, in NativeParallelHashMap<EntityId, BatchMaterialID>.ReadOnly batchMaterialHash, ref NativeList<EntityId> supportedMaterialIDs, ref NativeList<EntityId> unsupportedMaterialIDs, ref NativeList<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
	{
		NativeList<EntityId> nativeList = new NativeList<EntityId>(4, Allocator.Temp);
		foreach (EntityId materialID in materialIDs)
		{
			EntityId value = materialID;
			if (batchMaterialHash.ContainsKey(value))
			{
				nativeList.Add(in value);
			}
		}
		if (nativeList.IsEmpty)
		{
			nativeList.Dispose();
			return;
		}
		unsupportedMaterialIDs.Resize(nativeList.Length, NativeArrayOptions.UninitializedMemory);
		supportedMaterialIDs.Resize(nativeList.Length, NativeArrayOptions.UninitializedMemory);
		supportedPackedMaterialDatas.Resize(nativeList.Length, NativeArrayOptions.UninitializedMemory);
		int num = GPUDrivenProcessor.ClassifyMaterials(nativeList.AsArray(), unsupportedMaterialIDs.AsArray(), supportedMaterialIDs.AsArray(), supportedPackedMaterialDatas.AsArray());
		unsupportedMaterialIDs.Resize(num, NativeArrayOptions.ClearMemory);
		supportedMaterialIDs.Resize(nativeList.Length - num, NativeArrayOptions.ClearMemory);
		supportedPackedMaterialDatas.Resize(supportedMaterialIDs.Length, NativeArrayOptions.ClearMemory);
		nativeList.Dispose();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal static void FindUnsupportedRenderers_0024BurstManaged(in NativeArray<EntityId> unsupportedMaterials, in NativeArray<SmallEntityIdArray>.ReadOnly materialIDArrays, in NativeArray<EntityId>.ReadOnly rendererGroups, ref NativeList<EntityId> unsupportedRenderers)
	{
		for (int i = 0; i < materialIDArrays.Length; i++)
		{
			SmallEntityIdArray smallEntityIdArray = materialIDArrays[i];
			EntityId value = rendererGroups[i];
			for (int j = 0; j < smallEntityIdArray.Length; j++)
			{
				EntityId value2 = smallEntityIdArray[j];
				if (unsupportedMaterials.Contains(value2))
				{
					unsupportedRenderers.Add(in value);
					break;
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal static void GetMaterialsWithChangedPackedMaterial_0024BurstManaged(in NativeArray<EntityId> materialIDs, in NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas, in NativeParallelHashMap<EntityId, GPUDrivenPackedMaterialData>.ReadOnly packedMaterialHash, ref NativeHashSet<EntityId> filteredMaterials)
	{
		for (int i = 0; i < materialIDs.Length; i++)
		{
			EntityId entityId = materialIDs[i];
			GPUDrivenPackedMaterialData other = packedMaterialDatas[i];
			if (!packedMaterialHash.TryGetValue(entityId, out var item) || !item.Equals(other))
			{
				filteredMaterials.Add(entityId);
			}
		}
	}
}
