using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

internal class NativePassCompiler : IDisposable
{
	internal struct RenderGraphInputInfo
	{
		public RenderGraphResourceRegistry m_ResourcesForDebugOnly;

		public List<RenderGraphPass> m_RenderPasses;

		public string debugName;

		public bool disablePassCulling;

		public bool disablePassMerging;

		public RenderTextureUVOriginStrategy renderTextureUVOriginStrategy;
	}

	internal enum NativeCompilerProfileId
	{
		NRPRGComp_PrepareNativePass,
		NRPRGComp_SetupContextData,
		NRPRGComp_BuildGraph,
		NRPRGComp_CullNodes,
		NRPRGComp_TryMergeNativePasses,
		NRPRGComp_FindResourceUsageRanges,
		NRPRGComp_DetectMemorylessResources,
		NRPRGComp_PropagateTextureUVOrigin,
		NRPRGComp_ExecuteInitializeResources,
		NRPRGComp_ExecuteBeginRenderpassCommand,
		NRPRGComp_ExecuteDestroyResources
	}

	internal RenderGraphInputInfo graph;

	internal CompilerContextData contextData;

	internal CompilerContextData defaultContextData;

	internal CommandBuffer previousCommandBuffer;

	private Stack<int> m_HasSideEffectPassIdCullingStack;

	private List<Stack<ResourceHandle>> m_UnusedVersionedResourceIdCullingStacks;

	private Dictionary<int, List<ResourceHandle>> m_DelayedLastUseListPerPassMap;

	private RenderGraphCompilationCache m_CompilationCache;

	private RenderTargetIdentifier[][] m_TempMRTArrays;

	internal const int k_EstimatedPassCount = 100;

	internal const int k_MaxSubpass = 8;

	private NativeList<AttachmentDescriptor> m_BeginRenderPassAttachments;

	internal static bool s_ForceGenerateAuditsForTests;

	private const int ArbitraryMaxNbMergedPasses = 16;

	private DynamicArray<Name> graphPassNamesForDebug = new DynamicArray<Name>(16);

	public NativePassCompiler(RenderGraphCompilationCache cache)
	{
		m_CompilationCache = cache;
		defaultContextData = new CompilerContextData();
		m_HasSideEffectPassIdCullingStack = new Stack<int>(100);
		m_UnusedVersionedResourceIdCullingStacks = new List<Stack<ResourceHandle>>();
		for (int i = 0; i < 3; i++)
		{
			m_UnusedVersionedResourceIdCullingStacks.Add(new Stack<ResourceHandle>());
		}
		m_DelayedLastUseListPerPassMap = new Dictionary<int, List<ResourceHandle>>(100);
		for (int j = 0; j < 100; j++)
		{
			m_DelayedLastUseListPerPassMap.Add(j, new List<ResourceHandle>());
		}
		m_TempMRTArrays = new RenderTargetIdentifier[RenderGraph.kMaxMRTCount][];
		for (int k = 0; k < RenderGraph.kMaxMRTCount; k++)
		{
			m_TempMRTArrays[k] = new RenderTargetIdentifier[k + 1];
		}
	}

	~NativePassCompiler()
	{
		Cleanup();
	}

	public void Dispose()
	{
		Cleanup();
		GC.SuppressFinalize(this);
	}

	public void Cleanup()
	{
		contextData?.Dispose();
		defaultContextData?.Dispose();
		if (m_BeginRenderPassAttachments.IsCreated)
		{
			m_BeginRenderPassAttachments.Dispose();
		}
	}

	public bool Initialize(RenderGraphResourceRegistry resources, List<RenderGraphPass> renderPasses, RenderGraphDebugParams debugParams, string debugName, bool useCompilationCaching, int graphHash, int frameIndex, RenderTextureUVOriginStrategy renderTextureUVOriginStrategy)
	{
		bool result = false;
		if (!useCompilationCaching)
		{
			contextData = defaultContextData;
		}
		else
		{
			result = m_CompilationCache.GetCompilationCache(graphHash, frameIndex, out contextData);
		}
		graph.m_ResourcesForDebugOnly = resources;
		graph.m_RenderPasses = renderPasses;
		graph.disablePassCulling = debugParams.disablePassCulling;
		graph.disablePassMerging = debugParams.disablePassMerging;
		graph.debugName = debugName;
		graph.renderTextureUVOriginStrategy = renderTextureUVOriginStrategy;
		Clear(!useCompilationCaching);
		return result;
	}

	private void HandleExtendedFeatureFlags()
	{
		for (int i = 0; i < contextData.nativePassData.Length; i++)
		{
			int firstNativeSubPass = contextData.nativePassData[i].firstNativeSubPass;
			if (firstNativeSubPass < 0)
			{
				continue;
			}
			int firstGraphPass = contextData.nativePassData[i].firstGraphPass;
			int j = 0;
			for (int k = 0; k < contextData.nativePassData[i].numNativeSubPasses; k++)
			{
				SubPassFlags subPassFlags = SubPassFlags.MultiviewRenderRegionsCompatible;
				for (; j < contextData.nativePassData[i].numGraphPasses && contextData.passData[j + firstGraphPass].nativeSubPassIndex == k; j++)
				{
					if (contextData.passData[j + firstGraphPass].extendedFeatureFlags.HasFlag(ExtendedFeatureFlags.TileProperties))
					{
						subPassFlags |= SubPassFlags.TileProperties;
					}
					if (!contextData.passData[j + firstGraphPass].extendedFeatureFlags.HasFlag(ExtendedFeatureFlags.MultiviewRenderRegionsCompatible))
					{
						subPassFlags &= ~SubPassFlags.MultiviewRenderRegionsCompatible;
					}
				}
				contextData.nativeSubPassData.ElementAt(firstNativeSubPass + k).flags |= subPassFlags;
			}
		}
	}

	public void Compile(RenderGraphResourceRegistry resources)
	{
		SetupContextData(resources);
		BuildGraph();
		CullUnusedRenderGraphPasses();
		TryMergeNativePasses();
		HandleExtendedFeatureFlags();
		FindResourceUsageRangeAndSynchronization();
		DetectMemoryLessResources();
		PrepareNativeRenderPasses();
		if (graph.renderTextureUVOriginStrategy == RenderTextureUVOriginStrategy.PropagateAttachmentOrientation)
		{
			PropagateTextureUVOrigin();
		}
	}

	public void Clear(bool clearContextData)
	{
		if (clearContextData)
		{
			contextData.Clear();
		}
		m_HasSideEffectPassIdCullingStack.Clear();
		for (int i = 0; i < 3; i++)
		{
			m_UnusedVersionedResourceIdCullingStacks[i].Clear();
		}
		foreach (KeyValuePair<int, List<ResourceHandle>> item in m_DelayedLastUseListPerPassMap)
		{
			item.Value.Clear();
		}
		m_DelayedLastUseListPerPassMap.Clear();
	}

	private void SetPassStatesForNativePass(int nativePassId)
	{
		NativePassData.SetPassStatesForNativePass(contextData, nativePassId);
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void ValidatePasses()
	{
		if (!RenderGraph.enableValidityChecks)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < graph.m_RenderPasses.Count; i++)
		{
			if (graph.m_RenderPasses[i].extendedFeatureFlags.HasFlag(ExtendedFeatureFlags.TileProperties))
			{
				if (num > -1)
				{
					throw new Exception("ExtendedFeatureFlags.TileProperties can only be set once per render graph (render graph " + graph.debugName + ", pass " + graph.m_RenderPasses[i].name + "), previously set at (pass " + graph.m_RenderPasses[num].name + ").");
				}
				num = i;
			}
		}
	}

	private void SetupContextData(RenderGraphResourceRegistry resources)
	{
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_SetupContextData)))
		{
			contextData.Initialize(resources, 100);
		}
	}

	private bool TrySetupRasterFragmentList(ref PassData ctxPass, ref RenderGraphPass inputPass, out string errorMessage)
	{
		errorMessage = null;
		CompilerContextData compilerContextData = contextData;
		ctxPass.firstFragment = compilerContextData.fragmentData.Length;
		if (inputPass.depthAccess.textureHandle.handle.IsValid())
		{
			ctxPass.fragmentInfoHasDepth = true;
			if (compilerContextData.TryAddToFragmentList(inputPass.depthAccess, ctxPass.firstFragment, ctxPass.numFragments, out errorMessage))
			{
				TextureAccess depthAccess = inputPass.depthAccess;
				ctxPass.TryAddFragment(in depthAccess.textureHandle.handle, compilerContextData, out errorMessage);
			}
			if (errorMessage != null)
			{
				errorMessage = $"when trying to add depth attachment of type {inputPass.depthAccess.textureHandle.handle.type} at index {inputPass.depthAccess.textureHandle.handle.index} - {errorMessage}";
				return false;
			}
		}
		for (int i = 0; i < inputPass.colorBufferMaxIndex + 1; i++)
		{
			if (inputPass.colorBufferAccess[i].textureHandle.handle.IsValid())
			{
				if (compilerContextData.TryAddToFragmentList(in inputPass.colorBufferAccess[i], ctxPass.firstFragment, ctxPass.numFragments, out errorMessage))
				{
					ctxPass.TryAddFragment(in inputPass.colorBufferAccess[i].textureHandle.handle, compilerContextData, out errorMessage);
				}
				if (errorMessage != null)
				{
					errorMessage = $"when trying to add render attachment of type {inputPass.colorBufferAccess[i].textureHandle.handle.type} at index {inputPass.colorBufferAccess[i].textureHandle.handle.index} - {errorMessage}";
					return false;
				}
			}
		}
		if (inputPass.hasShadingRateImage && inputPass.shadingRateAccess.textureHandle.handle.IsValid())
		{
			if (compilerContextData.TryAddToFragmentList(inputPass.shadingRateAccess, ctxPass.firstFragment, ctxPass.numFragments, out errorMessage))
			{
				ctxPass.shadingRateImageIndex = compilerContextData.fragmentData.Length - 1;
			}
			if (errorMessage != null)
			{
				errorMessage = $"when trying to add VRS attachment of type {inputPass.shadingRateAccess.textureHandle.handle.type} at index {inputPass.shadingRateAccess.textureHandle.handle.index} - {errorMessage}";
				return false;
			}
		}
		ctxPass.firstFragmentInput = compilerContextData.fragmentData.Length;
		for (int j = 0; j < inputPass.fragmentInputMaxIndex + 1; j++)
		{
			if (inputPass.fragmentInputAccess[j].textureHandle.IsValid())
			{
				if (compilerContextData.TryAddToFragmentList(in inputPass.fragmentInputAccess[j], ctxPass.firstFragmentInput, ctxPass.numFragmentInputs, out errorMessage))
				{
					ctxPass.TryAddFragmentInput(in inputPass.fragmentInputAccess[j].textureHandle.handle, compilerContextData, out errorMessage);
				}
				if (errorMessage != null)
				{
					errorMessage = $"when trying to add input attachment of type {inputPass.fragmentInputAccess[j].textureHandle.handle.type} at index {inputPass.fragmentInputAccess[j].textureHandle.handle.index} - {errorMessage}";
					return false;
				}
			}
		}
		ctxPass.firstRandomAccessResource = compilerContextData.randomAccessResourceData.Length;
		for (int k = 0; k < inputPass.randomAccessResourceMaxIndex + 1; k++)
		{
			ref RenderGraphPass.RandomWriteResourceInfo reference = ref inputPass.randomAccessResource[k];
			if (reference.h.IsValid())
			{
				if (compilerContextData.TryAddToRandomAccessResourceList(in reference.h, k, reference.preserveCounterValue, ctxPass.firstRandomAccessResource, ctxPass.numRandomAccessResources, out errorMessage))
				{
					ctxPass.AddRandomAccessResource();
				}
				if (errorMessage != null)
				{
					errorMessage = $"when trying to add random access attachment of type {reference.h.type} at index {reference.h.index} - {errorMessage}";
					return false;
				}
			}
		}
		_ = ctxPass.numFragments;
		return true;
	}

	private void BuildGraph()
	{
		CompilerContextData compilerContextData = contextData;
		List<RenderGraphPass> renderPasses = graph.m_RenderPasses;
		compilerContextData.passData.ResizeUninitialized(renderPasses.Count);
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_BuildGraph)))
		{
			for (int i = 0; i < renderPasses.Count; i++)
			{
				RenderGraphPass pass = renderPasses[i];
				ref PassData reference = ref compilerContextData.passData.ElementAt(i);
				reference.ResetAndInitialize(in pass, i);
				compilerContextData.passNames.Add(new Name(pass.name, computeUTF8ByteCount: true));
				if (reference.hasSideEffects)
				{
					m_HasSideEffectPassIdCullingStack.Push(i);
				}
				if (reference.type == RenderGraphPassType.Raster && !TrySetupRasterFragmentList(ref reference, ref pass, out var errorMessage))
				{
					throw new Exception("In pass '" + pass.name + "', " + errorMessage);
				}
				reference.firstInput = compilerContextData.inputData.Length;
				reference.firstOutput = compilerContextData.outputData.Length;
				for (int j = 0; j < 3; j++)
				{
					List<ResourceHandle> list = pass.resourceWriteLists[j];
					int count = list.Count;
					for (int k = 0; k < count; k++)
					{
						ResourceHandle h = list[k];
						if (compilerContextData.UnversionedResourceData(in h).isImported && !reference.hasSideEffects)
						{
							reference.hasSideEffects = true;
							m_HasSideEffectPassIdCullingStack.Push(i);
						}
						compilerContextData.resources[h].SetWritingPass(compilerContextData, in h, i);
						compilerContextData.outputData.Add(new PassOutputData(in h));
						reference.numOutputs++;
					}
					List<ResourceHandle> list2 = pass.resourceReadLists[j];
					int count2 = list2.Count;
					for (int l = 0; l < count2; l++)
					{
						ResourceHandle h2 = list2[l];
						compilerContextData.resources[h2].RegisterReadingPass(compilerContextData, in h2, i, reference.numInputs);
						compilerContextData.inputData.Add(new PassInputData(in h2));
						reference.numInputs++;
					}
					List<ResourceHandle> list3 = pass.transientResourceList[j];
					int count3 = list3.Count;
					for (int m = 0; m < count3; m++)
					{
						ResourceHandle h3 = list3[m];
						compilerContextData.resources[h3].RegisterReadingPass(compilerContextData, in h3, i, reference.numInputs);
						compilerContextData.inputData.Add(new PassInputData(in h3));
						reference.numInputs++;
						compilerContextData.resources[h3].SetWritingPass(compilerContextData, in h3, i);
						compilerContextData.outputData.Add(new PassOutputData(in h3));
						reference.numOutputs++;
					}
					if (j != 0 || reference.type != RenderGraphPassType.Raster)
					{
						continue;
					}
					reference.firstSampledOnlyRaster = compilerContextData.sampledData.Length;
					ReadOnlySpan<PassInputData> readOnlySpan = reference.Inputs(compilerContextData);
					for (int n = 0; n < readOnlySpan.Length; n++)
					{
						ref readonly PassInputData reference2 = ref readOnlySpan[n];
						if (!reference.IsUsedAsFragment(in reference2.resource, compilerContextData))
						{
							compilerContextData.sampledData.Add(in reference2.resource);
							reference.numSampledOnlyRaster++;
						}
					}
				}
			}
		}
	}

	private void CullUnusedRenderGraphPasses()
	{
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_CullNodes)))
		{
			if (graph.disablePassCulling)
			{
				return;
			}
			CompilerContextData compilerContextData = contextData;
			compilerContextData.CullAllPasses(isCulled: true);
			while (m_HasSideEffectPassIdCullingStack.Count != 0)
			{
				int index = m_HasSideEffectPassIdCullingStack.Pop();
				ref PassData reference = ref compilerContextData.passData.ElementAt(index);
				if (!reference.culled)
				{
					continue;
				}
				ReadOnlySpan<PassInputData> readOnlySpan = reference.Inputs(compilerContextData);
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					ref readonly PassInputData reference2 = ref readOnlySpan[i];
					ref ResourceVersionedData reference3 = ref compilerContextData.resources[reference2.resource];
					if (reference3.written)
					{
						m_HasSideEffectPassIdCullingStack.Push(reference3.writePassId);
					}
				}
				reference.culled = false;
			}
			for (int num = compilerContextData.passData.Length - 1; num >= 0; num--)
			{
				ref PassData reference4 = ref compilerContextData.passData.ElementAt(num);
				if (reference4.culled)
				{
					PassData passData = reference4;
					passData.DisconnectFromResources(compilerContextData);
				}
			}
		}
	}

	private void CullRenderGraphPassesWritingOnlyUnusedResources()
	{
		CompilerContextData compilerContextData = contextData;
		int length = compilerContextData.passData.Length;
		for (int i = 0; i < length; i++)
		{
			ref PassData reference = ref compilerContextData.passData.ElementAt(i);
			reference.tag = reference.numOutputs;
			ReadOnlySpan<PassOutputData> readOnlySpan = reference.Outputs(compilerContextData);
			for (int j = 0; j < readOnlySpan.Length; j++)
			{
				ref readonly ResourceHandle resource = ref readOnlySpan[j].resource;
				if (compilerContextData.resources[resource].numReaders == 0)
				{
					m_UnusedVersionedResourceIdCullingStacks[resource.iType].Push(resource);
				}
			}
		}
		for (int k = 0; k < 3; k++)
		{
			Stack<ResourceHandle> stack = m_UnusedVersionedResourceIdCullingStacks[k];
			while (stack.Count != 0)
			{
				ResourceHandle h = stack.Pop();
				if (compilerContextData.resources.unversionedData[k].ElementAt(h.index).isImported)
				{
					continue;
				}
				ref ResourceVersionedData reference2 = ref compilerContextData.resources[h];
				ref PassData reference3 = ref compilerContextData.passData.ElementAt(reference2.writePassId);
				if (reference3.culled)
				{
					continue;
				}
				reference3.tag--;
				if (reference3.tag == 0 && !reference3.hasSideEffects)
				{
					reference3.culled = true;
					reference3.DisconnectFromResources(compilerContextData, stack, k);
					continue;
				}
				ResourceHandle h2 = new ResourceHandle(in h, h.version - 1);
				if (graph.m_RenderPasses[reference3.passId].implicitReadsList.Contains(h2))
				{
					ref ResourceVersionedData reference4 = ref compilerContextData.resources[h2];
					reference4.RemoveReadingPass(compilerContextData, in h2, reference3.passId);
					if (reference4.written && reference4.numReaders == 0)
					{
						stack.Push(h2);
					}
				}
			}
		}
	}

	private void TryMergeNativePasses()
	{
		CompilerContextData compilerContextData = contextData;
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_TryMergeNativePasses)))
		{
			int num = -1;
			for (int i = 0; i < compilerContextData.passData.Length; i++)
			{
				ref PassData reference = ref compilerContextData.passData.ElementAt(i);
				if (reference.culled)
				{
					continue;
				}
				if (num == -1)
				{
					if (reference.type == RenderGraphPassType.Raster)
					{
						compilerContextData.nativePassData.Add(new NativePassData(ref reference, compilerContextData));
						reference.nativePassIndex = NativeListExtensions.LastIndex(ref compilerContextData.nativePassData);
						num = reference.nativePassIndex;
					}
					continue;
				}
				PassBreakAudit passBreakAudit = (graph.disablePassMerging ? new PassBreakAudit(PassBreakReason.PassMergingDisabled, i) : NativePassData.TryMerge(contextData, num, i));
				if (passBreakAudit.reason != PassBreakReason.Merged)
				{
					SetPassStatesForNativePass(num);
					if (passBreakAudit.reason == PassBreakReason.NonRasterPass)
					{
						num = -1;
						continue;
					}
					compilerContextData.nativePassData.Add(new NativePassData(ref reference, compilerContextData));
					reference.nativePassIndex = NativeListExtensions.LastIndex(ref compilerContextData.nativePassData);
					num = reference.nativePassIndex;
				}
			}
			if (num >= 0)
			{
				SetPassStatesForNativePass(num);
			}
		}
	}

	private bool FindFirstPassIdOnGraphicsQueueAwaitingFenceGoingForward(ref PassData startAsyncPass, out int firstPassIdAwaiting)
	{
		CompilerContextData compilerContextData = contextData;
		firstPassIdAwaiting = startAsyncPass.awaitingMyGraphicsFencePassId;
		if (firstPassIdAwaiting == -1)
		{
			int num = startAsyncPass.passId + 1;
			int num2 = compilerContextData.passData.Length - 1;
			while (firstPassIdAwaiting == -1 && num <= num2)
			{
				ref PassData reference = ref compilerContextData.passData.ElementAt(num);
				if (reference.asyncCompute && !reference.culled)
				{
					firstPassIdAwaiting = reference.awaitingMyGraphicsFencePassId;
				}
				num++;
			}
			if (num > num2)
			{
				firstPassIdAwaiting = num2;
				return false;
			}
		}
		return true;
	}

	private int FindFirstNonCulledPassIdGoingBackward(int startPassId, bool startPassIsIncluded)
	{
		CompilerContextData compilerContextData = contextData;
		int num = (startPassIsIncluded ? startPassId : Math.Max(0, startPassId - 1));
		ref PassData reference = ref compilerContextData.passData.ElementAt(num);
		while (reference.culled && num > 0)
		{
			reference = ref compilerContextData.passData.ElementAt(--num);
		}
		return reference.passId;
	}

	private void FindResourceUsageRangeAndSynchronization()
	{
		CompilerContextData compilerContextData = contextData;
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_FindResourceUsageRanges)))
		{
			for (int i = 0; i < compilerContextData.passData.Length; i++)
			{
				ref PassData reference = ref compilerContextData.passData.ElementAt(i);
				if (reference.culled)
				{
					continue;
				}
				ClearDelayedLastUseListAtPass(i);
				reference.waitOnGraphicsFencePassId = -1;
				reference.awaitingMyGraphicsFencePassId = -1;
				reference.insertGraphicsFence = false;
				ReadOnlySpan<PassInputData> readOnlySpan = reference.Inputs(compilerContextData);
				for (int j = 0; j < readOnlySpan.Length; j++)
				{
					ResourceHandle h = readOnlySpan[j].resource;
					ref ResourceUnversionedData reference2 = ref compilerContextData.UnversionedResourceData(in h);
					ref ResourceVersionedData reference3 = ref compilerContextData.VersionedResourceData(in h);
					reference2.lastUsePassID = -1;
					if (reference2.firstUsePassID < 0)
					{
						reference2.firstUsePassID = reference.passId;
						reference.AddFirstUse(in h, compilerContextData);
					}
					if (reference2.latestVersionNumber == h.version)
					{
						reference2.tag++;
					}
					if (reference3.written)
					{
						ref PassData reference4 = ref compilerContextData.passData.ElementAt(reference3.writePassId);
						if (reference4.asyncCompute != reference.asyncCompute)
						{
							int waitOnGraphicsFencePassId = reference.waitOnGraphicsFencePassId;
							reference.waitOnGraphicsFencePassId = Math.Max(reference4.passId, waitOnGraphicsFencePassId);
						}
					}
				}
				ReadOnlySpan<PassOutputData> readOnlySpan2 = reference.Outputs(compilerContextData);
				for (int j = 0; j < readOnlySpan2.Length; j++)
				{
					ResourceHandle h2 = readOnlySpan2[j].resource;
					ref ResourceUnversionedData reference5 = ref compilerContextData.UnversionedResourceData(in h2);
					ref ResourceVersionedData reference6 = ref compilerContextData.VersionedResourceData(in h2);
					if (reference5.firstUsePassID < 0)
					{
						reference5.firstUsePassID = reference.passId;
						reference.AddFirstUse(in h2, compilerContextData);
					}
					if (reference5.latestVersionNumber == h2.version)
					{
						reference5.lastWritePassID = reference.passId;
					}
					int numReaders = reference6.numReaders;
					for (int k = 0; k < numReaders; k++)
					{
						int index = compilerContextData.resources.IndexReader(in h2, k);
						ref ResourceReaderData reference7 = ref compilerContextData.resources.readerData[h2.iType].ElementAt(index);
						ref PassData reference8 = ref compilerContextData.passData.ElementAt(reference7.passId);
						if (reference.asyncCompute != reference8.asyncCompute)
						{
							reference.insertGraphicsFence = true;
							int awaitingMyGraphicsFencePassId = reference.awaitingMyGraphicsFencePassId;
							reference.awaitingMyGraphicsFencePassId = ((awaitingMyGraphicsFencePassId == -1) ? reference7.passId : Math.Min(awaitingMyGraphicsFencePassId, reference7.passId));
						}
					}
				}
			}
			for (int l = 0; l < compilerContextData.passData.Length; l++)
			{
				ref PassData reference9 = ref compilerContextData.passData.ElementAt(l);
				if (reference9.culled)
				{
					continue;
				}
				bool asyncCompute = reference9.asyncCompute;
				ReadOnlySpan<PassInputData> readOnlySpan = reference9.Inputs(compilerContextData);
				for (int j = 0; j < readOnlySpan.Length; j++)
				{
					ResourceHandle h3 = readOnlySpan[j].resource;
					ref ResourceUnversionedData reference10 = ref compilerContextData.UnversionedResourceData(in h3);
					if (reference10.latestVersionNumber != h3.version)
					{
						continue;
					}
					int num = reference10.tag - 1;
					if (num == 0)
					{
						if (asyncCompute)
						{
							int firstPassIdAwaiting;
							bool flag = FindFirstPassIdOnGraphicsQueueAwaitingFenceGoingForward(ref reference9, out firstPassIdAwaiting);
							AddDelayedLastUseToPass(in h3, reference10.lastUsePassID = FindFirstNonCulledPassIdGoingBackward(firstPassIdAwaiting, !flag));
						}
						else
						{
							reference10.lastUsePassID = reference9.passId;
							reference9.AddLastUse(in h3, compilerContextData);
						}
					}
					reference10.tag = num;
				}
				ReadOnlySpan<PassOutputData> readOnlySpan2 = reference9.Outputs(compilerContextData);
				for (int j = 0; j < readOnlySpan2.Length; j++)
				{
					ResourceHandle h4 = readOnlySpan2[j].resource;
					ref ResourceUnversionedData reference11 = ref compilerContextData.UnversionedResourceData(in h4);
					ref ResourceVersionedData reference12 = ref compilerContextData.VersionedResourceData(in h4);
					if (reference11.latestVersionNumber == h4.version && reference12.numReaders == 0)
					{
						if (asyncCompute)
						{
							int firstPassIdAwaiting2;
							bool flag2 = FindFirstPassIdOnGraphicsQueueAwaitingFenceGoingForward(ref reference9, out firstPassIdAwaiting2);
							AddDelayedLastUseToPass(in h4, reference11.lastUsePassID = FindFirstNonCulledPassIdGoingBackward(firstPassIdAwaiting2, !flag2));
						}
						else
						{
							reference11.lastUsePassID = reference9.passId;
							reference9.AddLastUse(in h4, compilerContextData);
						}
					}
				}
				AddLastUseFromDelayedList(ref reference9);
			}
		}
	}

	private void ClearDelayedLastUseListAtPass(int passId)
	{
		if (m_DelayedLastUseListPerPassMap.TryGetValue(passId, out var value))
		{
			value.Clear();
		}
	}

	private void AddDelayedLastUseToPass(in ResourceHandle releaseResource, int passId)
	{
		if (!m_DelayedLastUseListPerPassMap.TryGetValue(passId, out var value))
		{
			value = new List<ResourceHandle>();
			m_DelayedLastUseListPerPassMap.Add(passId, value);
		}
		value.Add(releaseResource);
	}

	public void AddLastUseFromDelayedList(ref PassData passData)
	{
		if (!m_DelayedLastUseListPerPassMap.TryGetValue(passData.passId, out var value))
		{
			return;
		}
		foreach (ResourceHandle item in value)
		{
			passData.AddLastUse(item, contextData);
		}
		value.Clear();
	}

	private void PrepareNativeRenderPasses()
	{
		for (int i = 0; i < contextData.nativePassData.Length; i++)
		{
			DetermineLoadStoreActions(ref contextData.nativePassData.ElementAt(i));
		}
	}

	private void PropagateTextureUVOrigin()
	{
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_PropagateTextureUVOrigin)))
		{
			for (int num = contextData.nativePassData.Length - 1; num >= 0; num--)
			{
				ref NativePassData reference = ref contextData.nativePassData.ElementAt(num);
				int size = reference.attachments.size;
				int index = 0;
				TextureUVOriginSelection textureUVOriginSelection = TextureUVOriginSelection.Unknown;
				for (int i = 0; i < size; i++)
				{
					ref NativePassAttachment reference2 = ref reference.attachments[i];
					if (reference2.storeAction != RenderBufferStoreAction.DontCare && reference2.handle.type == RenderGraphResourceType.Texture)
					{
						textureUVOriginSelection = contextData.UnversionedResourceData(in reference2.handle).textureUVOrigin;
						index = i;
						break;
					}
				}
				for (int j = 0; j < size; j++)
				{
					ref NativePassAttachment reference3 = ref reference.attachments[j];
					if (reference3.handle.type == RenderGraphResourceType.Texture)
					{
						ref ResourceUnversionedData reference4 = ref contextData.UnversionedResourceData(in reference3.handle);
						if (textureUVOriginSelection != TextureUVOriginSelection.Unknown && reference4.textureUVOrigin != TextureUVOriginSelection.Unknown && reference4.textureUVOrigin != textureUVOriginSelection)
						{
							ref NativePassAttachment reference5 = ref reference.attachments[index];
							string renderGraphResourceName = graph.m_ResourcesForDebugOnly.GetRenderGraphResourceName(in reference5.handle);
							string renderGraphResourceName2 = graph.m_ResourcesForDebugOnly.GetRenderGraphResourceName(in reference3.handle);
							throw new InvalidOperationException($"From pass '{contextData.passNames[reference.firstGraphPass]}' to pass '{contextData.passNames[reference.lastGraphPass]}' when trying to store resource '{renderGraphResourceName2}' of type {reference3.handle.type} at index {reference3.handle.index} - " + RenderGraph.RenderGraphExceptionMessages.IncompatibleTextureUVOriginStore(renderGraphResourceName, textureUVOriginSelection, renderGraphResourceName2, reference4.textureUVOrigin));
						}
						reference4.textureUVOrigin = textureUVOriginSelection;
					}
				}
			}
		}
	}

	private static bool IsGlobalTextureInPass(RenderGraphPass pass, in ResourceHandle handle)
	{
		foreach (var setGlobals in pass.setGlobalsList)
		{
			if (setGlobals.Item1.handle.index == handle.index)
			{
				return true;
			}
		}
		return false;
	}

	private void DetectMemoryLessResources()
	{
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_DetectMemorylessResources)))
		{
			if (!SystemInfo.supportsMemorylessTextures)
			{
				return;
			}
			CompilerContextData.NativePassIterator enumerator = contextData.NativePasses.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ref readonly NativePassData current = ref enumerator.Current;
				NativeArray<PassData> actualPasses;
				ReadOnlySpan<PassData> readOnlySpan = current.GraphPasses(contextData, out actualPasses);
				ReadOnlySpan<PassData> readOnlySpan2 = readOnlySpan;
				for (int i = 0; i < readOnlySpan2.Length; i++)
				{
					ref readonly PassData reference = ref readOnlySpan2[i];
					ReadOnlySpan<ResourceHandle> readOnlySpan3 = reference.FirstUsedResources(contextData);
					for (int j = 0; j < readOnlySpan3.Length; j++)
					{
						ref readonly ResourceHandle reference2 = ref readOnlySpan3[j];
						ref ResourceUnversionedData reference3 = ref contextData.UnversionedResourceData(in reference2);
						if (reference2.type != RenderGraphResourceType.Texture || reference3.isImported)
						{
							continue;
						}
						bool flag = IsGlobalTextureInPass(graph.m_RenderPasses[reference.passId], in reference2);
						ReadOnlySpan<PassData> readOnlySpan4 = readOnlySpan;
						for (int k = 0; k < readOnlySpan4.Length; k++)
						{
							ref readonly PassData reference4 = ref readOnlySpan4[k];
							ReadOnlySpan<ResourceHandle> readOnlySpan5 = reference4.LastUsedResources(contextData);
							for (int l = 0; l < readOnlySpan5.Length; l++)
							{
								ref readonly ResourceHandle reference5 = ref readOnlySpan5[l];
								ref ResourceUnversionedData reference6 = ref contextData.UnversionedResourceData(in reference5);
								if (reference5.type == RenderGraphResourceType.Texture && !reference6.isImported && reference2.index == reference5.index && !flag && (current.numNativeSubPasses > 1 || reference4.IsUsedAsFragment(in reference2, contextData)))
								{
									reference3.memoryLess = true;
									reference6.memoryLess = true;
								}
							}
						}
					}
				}
				if (actualPasses.IsCreated)
				{
					actualPasses.Dispose();
				}
			}
		}
	}

	internal static bool IsSameNativeSubPass(ref SubPassDescriptor a, ref SubPassDescriptor b)
	{
		SubPassFlags num = a.flags & ~(SubPassFlags.TileProperties | SubPassFlags.MultiviewRenderRegionsCompatible);
		SubPassFlags subPassFlags = b.flags & ~(SubPassFlags.TileProperties | SubPassFlags.MultiviewRenderRegionsCompatible);
		if (num != subPassFlags || a.colorOutputs.Length != b.colorOutputs.Length || a.inputs.Length != b.inputs.Length)
		{
			return false;
		}
		for (int i = 0; i < a.colorOutputs.Length; i++)
		{
			if (a.colorOutputs[i] != b.colorOutputs[i])
			{
				return false;
			}
		}
		for (int j = 0; j < a.inputs.Length; j++)
		{
			if (a.inputs[j] != b.inputs[j])
			{
				return false;
			}
		}
		return true;
	}

	private bool ExecuteInitializeResource(InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, in PassData pass)
	{
		bool flag = false;
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_ExecuteInitializeResources)))
		{
			resources.forceManualClearOfResource = true;
			ReadOnlySpan<ResourceHandle> readOnlySpan2;
			if (pass.type == RenderGraphPassType.Raster && pass.nativePassIndex >= 0)
			{
				if (pass.mergeState == PassMergeState.Begin || pass.mergeState == PassMergeState.None)
				{
					NativeArray<PassData> actualPasses;
					ReadOnlySpan<PassData> readOnlySpan = contextData.nativePassData.ElementAt(pass.nativePassIndex).GraphPasses(contextData, out actualPasses);
					for (int i = 0; i < readOnlySpan.Length; i++)
					{
						ref readonly PassData reference = ref readOnlySpan[i];
						readOnlySpan2 = reference.FirstUsedResources(contextData);
						for (int j = 0; j < readOnlySpan2.Length; j++)
						{
							ref readonly ResourceHandle reference2 = ref readOnlySpan2[j];
							ref ResourceUnversionedData reference3 = ref contextData.UnversionedResourceData(in reference2);
							bool flag2 = reference.IsUsedAsFragment(in reference2, contextData);
							resources.forceManualClearOfResource = !flag2;
							if (!reference3.isImported)
							{
								if (reference3.memoryLess)
								{
									resources.SetTextureAsMemoryLess(in reference2);
								}
								flag |= resources.CreatePooledResource(rgContext, reference2.iType, reference2.index);
							}
							else if (reference3.clear && !reference3.memoryLess && resources.forceManualClearOfResource)
							{
								flag |= resources.ClearResource(rgContext, reference2.iType, reference2.index);
							}
						}
					}
					if (actualPasses.IsCreated)
					{
						actualPasses.Dispose();
					}
				}
			}
			else
			{
				readOnlySpan2 = pass.FirstUsedResources(contextData);
				for (int i = 0; i < readOnlySpan2.Length; i++)
				{
					ref readonly ResourceHandle reference4 = ref readOnlySpan2[i];
					ref ResourceUnversionedData reference5 = ref contextData.UnversionedResourceData(in reference4);
					if (!reference5.isImported)
					{
						flag |= resources.CreatePooledResource(rgContext, reference4.iType, reference4.index);
					}
					else if (reference5.clear)
					{
						flag |= resources.ClearResource(rgContext, reference4.iType, reference4.index);
					}
				}
			}
			resources.forceManualClearOfResource = true;
			return flag;
		}
	}

	private void DetermineLoadStoreActions(ref NativePassData nativePass)
	{
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_PrepareNativePass)))
		{
			contextData.passData.ElementAt(nativePass.firstGraphPass);
			contextData.passData.ElementAt(nativePass.lastGraphPass);
			if (nativePass.fragments.size <= 0)
			{
				return;
			}
			ref FixedAttachmentArray<PassFragmentData> fragments = ref nativePass.fragments;
			int num = 0;
			while (true)
			{
				int num2 = num;
				FixedAttachmentArray<PassFragmentData> fixedAttachmentArray = fragments;
				if (num2 >= fixedAttachmentArray.size)
				{
					break;
				}
				fixedAttachmentArray = fragments;
				ref PassFragmentData reference = ref fixedAttachmentArray[num];
				ResourceHandle handle = reference.resource;
				bool memoryless = false;
				int mipLevel = reference.mipLevel;
				int depthSlice = reference.depthSlice;
				RenderBufferLoadAction loadAction = RenderBufferLoadAction.DontCare;
				RenderBufferStoreAction storeAction = RenderBufferStoreAction.DontCare;
				bool flag = reference.accessFlags.HasFlag(AccessFlags.Write) && !reference.accessFlags.HasFlag(AccessFlags.Discard);
				ref ResourceUnversionedData reference2 = ref contextData.UnversionedResourceData(in reference.resource);
				bool isImported = reference2.isImported;
				int lastUsePassID = reference2.lastUsePassID;
				bool flag2 = lastUsePassID >= nativePass.lastGraphPass + 1;
				if (reference.accessFlags.HasFlag(AccessFlags.Read) || flag)
				{
					if (reference2.firstUsePassID >= nativePass.firstGraphPass)
					{
						loadAction = ((!isImported) ? RenderBufferLoadAction.Clear : (reference2.clear ? RenderBufferLoadAction.Clear : RenderBufferLoadAction.Load));
					}
					else
					{
						loadAction = RenderBufferLoadAction.Load;
						if (flag2)
						{
							storeAction = RenderBufferStoreAction.Store;
						}
					}
				}
				if (reference.accessFlags.HasFlag(AccessFlags.Write))
				{
					if (nativePass.samples <= 1)
					{
						storeAction = ((!flag2) ? ((!isImported) ? RenderBufferStoreAction.DontCare : (reference2.discard ? RenderBufferStoreAction.DontCare : RenderBufferStoreAction.Store)) : RenderBufferStoreAction.Store);
					}
					else
					{
						storeAction = RenderBufferStoreAction.DontCare;
						bool flag3 = reference2.latestVersionNumber == reference.resource.version;
						bool flag4 = isImported && flag3;
						if (lastUsePassID >= nativePass.firstGraphPass + nativePass.numGraphPasses)
						{
							bool flag5 = flag4 && !reference2.discard;
							bool flag6 = flag4 && !reference2.bindMS;
							ReadOnlySpan<ResourceReaderData> readOnlySpan = contextData.Readers(in reference.resource);
							for (int i = 0; i < readOnlySpan.Length; i++)
							{
								ref readonly ResourceReaderData reference3 = ref readOnlySpan[i];
								ref PassData reference4 = ref contextData.passData.ElementAt(reference3.passId);
								bool flag7 = reference4.IsUsedAsFragment(in reference.resource, contextData);
								if (reference4.type == RenderGraphPassType.Unsafe)
								{
									flag5 = true;
									flag6 = !reference2.bindMS;
									break;
								}
								if (flag7)
								{
									flag5 = true;
								}
								else if (reference2.bindMS)
								{
									flag5 = true;
								}
								else
								{
									flag6 = true;
								}
							}
							if (flag5 && flag6)
							{
								storeAction = RenderBufferStoreAction.StoreAndResolve;
							}
							else if (flag6)
							{
								storeAction = RenderBufferStoreAction.Resolve;
							}
							else if (flag5)
							{
								storeAction = RenderBufferStoreAction.Store;
							}
						}
						else if (flag4)
						{
							storeAction = (reference2.bindMS ? (reference2.discard ? RenderBufferStoreAction.DontCare : RenderBufferStoreAction.Store) : ((!reference2.discard) ? RenderBufferStoreAction.StoreAndResolve : ((!nativePass.hasDepth || nativePass.attachments.size != 0) ? RenderBufferStoreAction.Resolve : RenderBufferStoreAction.DontCare)));
						}
					}
				}
				if (reference2.memoryLess)
				{
					memoryless = true;
				}
				NativePassAttachment data = new NativePassAttachment(in handle, loadAction, storeAction, memoryless, mipLevel, depthSlice);
				nativePass.attachments.Add(in data);
				num++;
			}
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void ValidateNativePass(in NativePassData nativePass, int width, int height, int depth, int samples, int attachmentCount)
	{
		if (RenderGraph.enableValidityChecks)
		{
			if (nativePass.attachments.size == 0 || nativePass.numNativeSubPasses == 0)
			{
				throw new Exception("Empty render pass");
			}
			if (width == 0 || height == 0 || depth == 0 || samples == 0 || nativePass.numNativeSubPasses == 0 || attachmentCount == 0)
			{
				throw new Exception("Invalid render pass properties. One or more properties are zero.");
			}
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void ValidateAttachment(in RenderTargetInfo attRenderTargetInfo, RenderGraphResourceRegistry resources, int nativePassWidth, int nativePassHeight, int nativePassMSAASamples, bool isVrs, bool isShaderResolve)
	{
		if (!RenderGraph.enableValidityChecks)
		{
			return;
		}
		if (isVrs)
		{
			Vector2Int allocTileSize = ShadingRateImage.GetAllocTileSize(nativePassWidth, nativePassHeight);
			if (attRenderTargetInfo.width != allocTileSize.x || attRenderTargetInfo.height != allocTileSize.y || attRenderTargetInfo.msaaSamples != 1)
			{
				throw new Exception("Low level rendergraph error: Shading rate image attachment in renderpass does not match.");
			}
		}
		else if (attRenderTargetInfo.width != nativePassWidth || attRenderTargetInfo.height != nativePassHeight || (attRenderTargetInfo.msaaSamples != nativePassMSAASamples && !isShaderResolve))
		{
			throw new Exception("Low level rendergraph error: Attachments in renderpass do not match.");
		}
	}

	internal unsafe void ExecuteBeginRenderPass(InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, ref NativePassData nativePass)
	{
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_ExecuteBeginRenderpassCommand)))
		{
			ref FixedAttachmentArray<NativePassAttachment> attachments = ref nativePass.attachments;
			int size = attachments.size;
			int width = nativePass.width;
			int height = nativePass.height;
			int volumeDepth = nativePass.volumeDepth;
			int samples = nativePass.samples;
			nativePass.extendedFeatureFlags.HasFlag(ExtendedFeatureFlags.MultisampledShaderResolve);
			NativeArray<SubPassDescriptor> subPasses = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<SubPassDescriptor>(contextData.nativeSubPassData.GetUnsafeReadOnlyPtr() + nativePass.firstNativeSubPass, nativePass.numNativeSubPasses, Allocator.None);
			if (nativePass.hasFoveatedRasterization)
			{
				rgContext.cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
			}
			if (nativePass.hasShadingRateStates)
			{
				rgContext.cmd.SetShadingRateFragmentSize(nativePass.shadingRateFragmentSize);
				rgContext.cmd.SetShadingRateCombiner(ShadingRateCombinerStage.Primitive, nativePass.primitiveShadingRateCombiner);
				rgContext.cmd.SetShadingRateCombiner(ShadingRateCombinerStage.Fragment, nativePass.fragmentShadingRateCombiner);
			}
			if (!m_BeginRenderPassAttachments.IsCreated)
			{
				m_BeginRenderPassAttachments = new NativeList<AttachmentDescriptor>(8, Allocator.Persistent);
			}
			m_BeginRenderPassAttachments.Resize(size, NativeArrayOptions.UninitializedMemory);
			for (int i = 0; i < size; i++)
			{
				ref readonly ResourceHandle handle = ref attachments[i].handle;
				resources.GetRenderTargetInfo(in handle, out var outInfo);
				ref AttachmentDescriptor reference = ref m_BeginRenderPassAttachments.ElementAt(i);
				reference = new AttachmentDescriptor(outInfo.format);
				RTHandle texture = resources.GetTexture(handle.index);
				RenderTargetIdentifier renderTargetIdentifier = texture;
				reference.loadStoreTarget = new RenderTargetIdentifier(renderTargetIdentifier, attachments[i].mipLevel, CubemapFace.Unknown, attachments[i].depthSlice);
				if (attachments[i].storeAction == RenderBufferStoreAction.Resolve || attachments[i].storeAction == RenderBufferStoreAction.StoreAndResolve)
				{
					reference.resolveTarget = texture;
				}
				reference.loadAction = attachments[i].loadAction;
				reference.storeAction = attachments[i].storeAction;
				if (attachments[i].loadAction == RenderBufferLoadAction.Clear)
				{
					reference.clearColor = Color.red;
					reference.clearDepth = 1f;
					reference.clearStencil = 0u;
					ref readonly TextureDesc textureResourceDesc = ref resources.GetTextureResourceDesc(in handle, noThrowOnInvalidDesc: true);
					if (i == 0 && nativePass.hasDepth)
					{
						reference.clearDepth = 1f;
					}
					else
					{
						reference.clearColor = textureResourceDesc.clearColor;
					}
				}
			}
			if (nativePass.extendedFeatureFlags.HasFlag(ExtendedFeatureFlags.MultisampledShaderResolve))
			{
				SubPassDescriptor subPassDescriptor = subPasses[subPasses.Length - 1];
				for (int j = 0; j < subPassDescriptor.inputs.Length; j++)
				{
					int index = subPassDescriptor.inputs[j];
					if (m_BeginRenderPassAttachments.ElementAt(index).storeAction != RenderBufferStoreAction.DontCare)
					{
						throw new Exception("Low level rendergraph error: last subpass with shader resolve must have all input attachments as memoryless attachments.");
					}
				}
				if (subPassDescriptor.colorOutputs.Length != 1)
				{
					throw new Exception("Low level rendergraph error: last subpass with shader resolve must have one color attachment.");
				}
				if (SystemInfo.supportsMultisampledShaderResolve)
				{
					int index2 = subPassDescriptor.colorOutputs[0];
					ref AttachmentDescriptor reference2 = ref m_BeginRenderPassAttachments.ElementAt(index2);
					reference2.resolveTarget = reference2.loadStoreTarget;
					reference2.loadStoreTarget = new RenderTargetIdentifier(BuiltinRenderTextureType.None);
					reference2.storeAction = RenderBufferStoreAction.Store;
				}
			}
			NativeArray<AttachmentDescriptor> attachments2 = m_BeginRenderPassAttachments.AsArray();
			int depthAttachmentIndex = ((!nativePass.hasDepth) ? (-1) : 0);
			ReadOnlySpan<byte> empty = ReadOnlySpan<byte>.Empty;
			rgContext.cmd.BeginRenderPass(width, height, volumeDepth, samples, attachments2, depthAttachmentIndex, nativePass.shadingRateImageIndex, subPasses, empty);
			CommandBuffer.ThrowOnSetRenderTarget = true;
		}
	}

	private void ExecuteDestroyResource(InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, ref PassData pass)
	{
		using (new ProfilingScope(ProfilingSampler.Get(NativeCompilerProfileId.NRPRGComp_ExecuteDestroyResources)))
		{
			rgContext.renderGraphPool.ReleaseAllTempAlloc();
			ReadOnlySpan<ResourceHandle> readOnlySpan2;
			if (pass.type == RenderGraphPassType.Raster && pass.nativePassIndex >= 0)
			{
				if (pass.mergeState != PassMergeState.End && pass.mergeState != PassMergeState.None)
				{
					return;
				}
				NativeArray<PassData> actualPasses;
				ReadOnlySpan<PassData> readOnlySpan = contextData.nativePassData.ElementAt(pass.nativePassIndex).GraphPasses(contextData, out actualPasses);
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					readOnlySpan2 = readOnlySpan[i].LastUsedResources(contextData);
					for (int j = 0; j < readOnlySpan2.Length; j++)
					{
						ref readonly ResourceHandle reference = ref readOnlySpan2[j];
						if (!contextData.UnversionedResourceData(in reference).isImported)
						{
							resources.ReleasePooledResource(rgContext, reference.iType, reference.index);
						}
					}
				}
				if (actualPasses.IsCreated)
				{
					actualPasses.Dispose();
				}
				return;
			}
			readOnlySpan2 = pass.LastUsedResources(contextData);
			for (int i = 0; i < readOnlySpan2.Length; i++)
			{
				ref readonly ResourceHandle reference2 = ref readOnlySpan2[i];
				if (!contextData.UnversionedResourceData(in reference2).isImported)
				{
					resources.ReleasePooledResource(rgContext, reference2.iType, reference2.index);
				}
			}
		}
	}

	private void ExecuteSetRenderTargets(RenderGraphPass pass, InternalRenderGraphContext rgContext)
	{
		bool flag = pass.depthAccess.textureHandle.IsValid();
		if (!flag && pass.colorBufferMaxIndex == -1)
		{
			return;
		}
		RenderGraphResourceRegistry resourcesForDebugOnly = graph.m_ResourcesForDebugOnly;
		TextureAccess[] colorBufferAccess = pass.colorBufferAccess;
		if (pass.colorBufferMaxIndex > 0)
		{
			RenderTargetIdentifier[] array = m_TempMRTArrays[pass.colorBufferMaxIndex];
			for (int i = 0; i <= pass.colorBufferMaxIndex; i++)
			{
				array[i] = resourcesForDebugOnly.GetTexture(in colorBufferAccess[i].textureHandle);
			}
			if (!flag)
			{
				throw new InvalidOperationException("In pass " + pass.name + " - Setting multiple render textures (MRTs) without a depth buffer is not supported.");
			}
			CommandBuffer cmd = rgContext.cmd;
			TextureAccess depthAccess = pass.depthAccess;
			CoreUtils.SetRenderTarget(cmd, array, resourcesForDebugOnly.GetTexture(in depthAccess.textureHandle));
		}
		else if (flag)
		{
			if (pass.colorBufferMaxIndex > -1)
			{
				CommandBuffer cmd2 = rgContext.cmd;
				RTHandle texture = resourcesForDebugOnly.GetTexture(in pass.colorBufferAccess[0].textureHandle);
				TextureAccess depthAccess = pass.depthAccess;
				CoreUtils.SetRenderTarget(cmd2, texture, resourcesForDebugOnly.GetTexture(in depthAccess.textureHandle));
			}
			else
			{
				CommandBuffer cmd3 = rgContext.cmd;
				TextureAccess depthAccess = pass.depthAccess;
				CoreUtils.SetRenderTarget(cmd3, resourcesForDebugOnly.GetTexture(in depthAccess.textureHandle));
			}
		}
		else
		{
			if (!pass.colorBufferAccess[0].textureHandle.IsValid())
			{
				throw new InvalidOperationException("In pass " + pass.name + " - Neither depth nor color render targets are correctly set up.");
			}
			CoreUtils.SetRenderTarget(rgContext.cmd, resourcesForDebugOnly.GetTexture(in pass.colorBufferAccess[0].textureHandle));
		}
	}

	internal void ExecuteSetRandomWriteTarget(in CommandBuffer cmd, RenderGraphResourceRegistry resources, int index, in ResourceHandle resource, bool preserveCounterValue = true)
	{
		if (resource.type == RenderGraphResourceType.Texture)
		{
			RTHandle texture = resources.GetTexture(resource.index);
			cmd.SetRandomWriteTarget(index, texture);
			return;
		}
		if (resource.type == RenderGraphResourceType.Buffer)
		{
			GraphicsBuffer buffer = resources.GetBuffer(resource.index);
			if (preserveCounterValue)
			{
				cmd.SetRandomWriteTarget(index, buffer);
			}
			else
			{
				cmd.SetRandomWriteTarget(index, buffer, preserveCounterValue: false);
			}
			return;
		}
		string renderGraphResourceName = resources.GetRenderGraphResourceName(in resource);
		throw new Exception($"When trying to use resource '{renderGraphResourceName}' of type {resource.type} - " + "Invalid resource type, expected texture or buffer");
	}

	internal void ExecuteRenderGraphPass(ref InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, RenderGraphPass pass)
	{
		rgContext.executingPass = pass;
		if (!pass.HasRenderFunc())
		{
			throw new InvalidOperationException("In pass " + pass.name + " - RenderPass was not provided with an execute function.");
		}
		using (new ProfilingScope(rgContext.cmd, pass.customSampler))
		{
			pass.Execute(rgContext);
			foreach (var setGlobals in pass.setGlobalsList)
			{
				rgContext.cmd.SetGlobalTexture(setGlobals.Item2, setGlobals.Item1);
			}
		}
	}

	public void ExecuteGraph(InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, in List<RenderGraphPass> passes)
	{
		bool inRenderPass = false;
		previousCommandBuffer = rgContext.cmd;
		rgContext.cmd.ClearRandomWriteTargets();
		for (int i = 0; i < contextData.passData.Length; i++)
		{
			ref PassData reference = ref contextData.passData.ElementAt(i);
			if (reference.culled)
			{
				continue;
			}
			bool nrpBegan = false;
			bool flag = ExecuteInitializeResource(rgContext, resources, in reference);
			if (reference.type == RenderGraphPassType.Compute && reference.asyncCompute)
			{
				GraphicsFence fence = default(GraphicsFence);
				if (flag)
				{
					fence = rgContext.cmd.CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, SynchronisationStageFlags.AllGPUOperations);
				}
				if (!rgContext.contextlessTesting)
				{
					rgContext.renderContext.ExecuteCommandBuffer(rgContext.cmd);
				}
				rgContext.cmd.Clear();
				CommandBuffer commandBuffer = CommandBufferPool.Get("async cmd");
				commandBuffer.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);
				rgContext.cmd = commandBuffer;
				if (flag)
				{
					rgContext.cmd.WaitOnAsyncGraphicsFence(fence, SynchronisationStageFlags.PixelProcessing);
				}
			}
			if (reference.waitOnGraphicsFencePassId != -1)
			{
				rgContext.cmd.WaitOnAsyncGraphicsFence(contextData.fences[reference.waitOnGraphicsFencePassId], SynchronisationStageFlags.PixelProcessing);
			}
			if (reference.type == RenderGraphPassType.Raster && reference.mergeState <= PassMergeState.Begin)
			{
				if (reference.nativePassIndex >= 0)
				{
					ref NativePassData reference2 = ref contextData.nativePassData.ElementAt(reference.nativePassIndex);
					if (reference2.fragments.size > 0)
					{
						ExecuteBeginRenderPass(rgContext, resources, ref reference2);
						nrpBegan = true;
						inRenderPass = true;
					}
				}
			}
			else if (reference.type == RenderGraphPassType.Unsafe)
			{
				ExecuteSetRenderTargets(passes[i], rgContext);
			}
			if (reference.mergeState >= PassMergeState.SubPass && reference.beginNativeSubpass)
			{
				if (!inRenderPass)
				{
					throw new Exception("Compiler error: Pass is marked as beginning a native sub pass but no pass is currently active.");
				}
				rgContext.cmd.NextSubPass();
			}
			if (reference.numRandomAccessResources > 0)
			{
				ReadOnlySpan<PassRandomWriteData> readOnlySpan = reference.RandomWriteTextures(contextData);
				for (int j = 0; j < readOnlySpan.Length; j++)
				{
					ref readonly PassRandomWriteData reference3 = ref readOnlySpan[j];
					ExecuteSetRandomWriteTarget(in rgContext.cmd, resources, reference3.index, in reference3.resource);
				}
			}
			ExecuteRenderGraphPass(ref rgContext, resources, passes[reference.passId]);
			EndRenderGraphPass(ref rgContext, ref reference, ref inRenderPass, resources, nrpBegan);
		}
	}

	private void EndRenderGraphPass(ref InternalRenderGraphContext rgContext, ref PassData passData, ref bool inRenderPass, RenderGraphResourceRegistry resources, bool nrpBegan)
	{
		if (passData.numRandomAccessResources > 0)
		{
			rgContext.cmd.ClearRandomWriteTargets();
		}
		if (passData.insertGraphicsFence)
		{
			GraphicsFence value = rgContext.cmd.CreateAsyncGraphicsFence();
			contextData.fences[passData.passId] = value;
		}
		if (passData.type == RenderGraphPassType.Raster)
		{
			if (((passData.mergeState == PassMergeState.None && nrpBegan) || passData.mergeState == PassMergeState.End) && passData.nativePassIndex >= 0)
			{
				ref NativePassData reference = ref contextData.nativePassData.ElementAt(passData.nativePassIndex);
				if (reference.fragments.size > 0)
				{
					if (!inRenderPass)
					{
						throw new Exception("Compiler error: Generated a subpass pass but no pass is currently active.");
					}
					if (reference.hasFoveatedRasterization)
					{
						rgContext.cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
					}
					rgContext.cmd.EndRenderPass();
					CommandBuffer.ThrowOnSetRenderTarget = false;
					inRenderPass = false;
					if (reference.hasShadingRateStates || reference.hasShadingRateImage)
					{
						rgContext.cmd.ResetShadingRate();
					}
				}
			}
		}
		else if (passData.type == RenderGraphPassType.Compute && passData.asyncCompute)
		{
			rgContext.renderContext.ExecuteCommandBufferAsync(rgContext.cmd, ComputeQueueType.Background);
			CommandBufferPool.Release(rgContext.cmd);
			rgContext.cmd = previousCommandBuffer;
		}
		ExecuteDestroyResource(rgContext, resources, ref passData);
	}

	private static RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.AttachmentInfo MakeAttachmentInfo(CompilerContextData ctx, in NativePassData nativePass, int attachmentIndex)
	{
		NativePassAttachment att = nativePass.attachments[attachmentIndex];
		ResourceUnversionedData resourceUnversionedData = ctx.UnversionedResourceData(in att.handle);
		LoadAudit loadAudit = nativePass.loadAudit[attachmentIndex];
		string text = LoadAudit.LoadReasonMessages[(int)loadAudit.reason];
		if (loadAudit.passId >= 0)
		{
			text = text.Replace("{pass}", "<b>" + ctx.passNames[loadAudit.passId].name + "</b>");
		}
		StoreAudit storeAudit = nativePass.storeAudit[attachmentIndex];
		string text2 = StoreAudit.StoreReasonMessages[(int)storeAudit.reason];
		if (storeAudit.passId >= 0)
		{
			text2 = text2.Replace("{pass}", "<b>" + ctx.passNames[storeAudit.passId].name + "</b>");
		}
		string text3 = string.Empty;
		if (storeAudit.msaaReason != StoreReason.InvalidReason && storeAudit.msaaReason != StoreReason.NoMSAABuffer)
		{
			text3 = StoreAudit.StoreReasonMessages[(int)storeAudit.msaaReason];
			if (storeAudit.msaaPassId >= 0)
			{
				text3 = text3.Replace("{pass}", "<b>" + ctx.passNames[storeAudit.msaaPassId].name + "</b>");
			}
		}
		return new RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.AttachmentInfo
		{
			resourceName = resourceUnversionedData.GetName(ctx, in att.handle),
			attachmentIndex = attachmentIndex,
			loadReason = text,
			storeReason = text2,
			storeMsaaReason = text3,
			attachment = new RenderGraph.DebugData.SerializableNativePassAttachment(att)
		};
	}

	internal static string MakePassBreakInfoMessage(CompilerContextData ctx, in NativePassData nativePass)
	{
		string text = "";
		if (nativePass.breakAudit.breakPass >= 0)
		{
			text = text + "Failed to merge " + ctx.passNames[nativePass.breakAudit.breakPass].name + " into this native pass.\n";
		}
		return text + PassBreakAudit.BreakReasonMessages[(int)nativePass.breakAudit.reason];
	}

	internal static string MakePassMergeMessage(CompilerContextData ctx, in PassData pass, in PassData prevPass, in PassBreakAudit mergeResult)
	{
		string text = ((mergeResult.reason == PassBreakReason.Merged) ? "The passes are <b>compatible</b> to be merged.\n\n" : "The passes are <b>incompatible</b> to be merged.\n\n");
		string text2 = InjectSpaces(pass.GetName(ctx).name);
		string text3 = InjectSpaces(prevPass.GetName(ctx).name);
		switch (mergeResult.reason)
		{
		case PassBreakReason.Merged:
			if (pass.nativePassIndex == prevPass.nativePassIndex && pass.mergeState != PassMergeState.None)
			{
				return text + "Passes are merged.";
			}
			return text + "Passes can be merged but are not recorded consecutively.";
		case PassBreakReason.TargetSizeMismatch:
			return text + "The fragment attachments of the passes have different sizes or sample counts.\n" + $"- {text3}: {prevPass.fragmentInfoWidth}x{prevPass.fragmentInfoHeight}, {prevPass.fragmentInfoSamples} sample(s).\n" + $"- {text2}: {pass.fragmentInfoWidth}x{pass.fragmentInfoHeight}, {pass.fragmentInfoSamples} sample(s).";
		case PassBreakReason.NextPassReadsTexture:
			return text + text3 + " output is sampled by " + text2 + " as a regular texture, the pass needs to break.";
		case PassBreakReason.NextPassTargetsTexture:
			return text + text3 + " reads a texture that " + text2 + " targets to, the pass needs to break.";
		case PassBreakReason.NonRasterPass:
			return text + $"{text3} is type {prevPass.type}. Only Raster passes can be merged.";
		case PassBreakReason.DifferentDepthTextures:
			return text + text3 + " uses a different depth buffer than " + text2 + ".";
		case PassBreakReason.AttachmentLimitReached:
			return text + $"Merging the passes would use more than {8} attachments.";
		case PassBreakReason.SubPassLimitReached:
			return text + $"Merging the passes would use more than {8} native subpasses.";
		case PassBreakReason.EndOfGraph:
			return text + "The pass is the last pass in the graph.";
		case PassBreakReason.DifferentShadingRateImages:
			return text + text3 + " uses a different shading rate image than " + text2 + ".";
		case PassBreakReason.DifferentShadingRateStates:
			return text + text3 + " uses different shading rate states than " + text2 + ".";
		case PassBreakReason.MultisampledShaderResolveMustBeLastPass:
			return text + text3 + " uses multisampled shader resolve and so can't have any more passes merged into it.";
		case PassBreakReason.PassMergingDisabled:
			return text + "The pass merging is disabled.";
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private static string InjectSpaces(string camelCaseString)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < camelCaseString.Length; i++)
		{
			if (char.IsUpper(camelCaseString[i]) && i != 0 && char.IsLower(camelCaseString[i - 1]))
			{
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(camelCaseString[i]);
		}
		return stringBuilder.ToString();
	}

	internal void GenerateNativeCompilerDebugData(ref RenderGraph.DebugData debugData)
	{
		ref CompilerContextData reference = ref contextData;
		debugData.isNRPCompiler = true;
		Dictionary<(RenderGraphResourceType, int), List<int>> dictionary = new Dictionary<(RenderGraphResourceType, int), List<int>>();
		Dictionary<(RenderGraphResourceType, int), List<int>> dictionary2 = new Dictionary<(RenderGraphResourceType, int), List<int>>();
		foreach (RenderGraphPass renderPass in graph.m_RenderPasses)
		{
			for (int i = 0; i < 3; i++)
			{
				int length = reference.resources.unversionedData[i].Length;
				for (int j = 0; j < length; j++)
				{
					foreach (ResourceHandle item3 in renderPass.resourceReadLists[i])
					{
						if (!renderPass.implicitReadsList.Contains(item3) && item3.type == (RenderGraphResourceType)i && item3.index == j)
						{
							(RenderGraphResourceType, int) key = ((RenderGraphResourceType)i, j);
							if (!dictionary.ContainsKey(key))
							{
								dictionary[key] = new List<int>();
							}
							dictionary[key].Add(renderPass.index);
						}
					}
					foreach (ResourceHandle item4 in renderPass.resourceWriteLists[i])
					{
						if (item4.type == (RenderGraphResourceType)i && item4.index == j)
						{
							(RenderGraphResourceType, int) key2 = ((RenderGraphResourceType)i, j);
							if (!dictionary2.ContainsKey(key2))
							{
								dictionary2[key2] = new List<int>();
							}
							dictionary2[key2].Add(renderPass.index);
						}
					}
					foreach (ResourceHandle item5 in renderPass.transientResourceList[i])
					{
						if (item5.type == (RenderGraphResourceType)i && item5.index == j)
						{
							(RenderGraphResourceType, int) key3 = ((RenderGraphResourceType)i, j);
							if (!dictionary.ContainsKey(key3))
							{
								dictionary[key3] = new List<int>();
							}
							dictionary[key3].Add(renderPass.index);
							if (!dictionary2.ContainsKey(key3))
							{
								dictionary2[key3] = new List<int>();
							}
							dictionary2[key3].Add(renderPass.index);
						}
					}
				}
			}
		}
		for (int k = 0; k < 3; k++)
		{
			int length2 = reference.resources.unversionedData[k].Length;
			for (int l = 0; l < length2; l++)
			{
				ref ResourceUnversionedData reference2 = ref reference.resources.unversionedData[k].ElementAt(l);
				RenderGraph.DebugData.ResourceData item = default(RenderGraph.DebugData.ResourceData);
				RenderGraphResourceType renderGraphResourceType = (RenderGraphResourceType)k;
				bool flag = l == 0;
				if (!flag)
				{
					string name = reference.resources.resourceNames[k][l].name;
					item.name = ((!string.IsNullOrEmpty(name)) ? name : "(unnamed)");
					item.imported = reference2.isImported;
				}
				else
				{
					item.name = "<null>";
					item.imported = true;
				}
				RenderTargetInfo outInfo = default(RenderTargetInfo);
				if (renderGraphResourceType == RenderGraphResourceType.Texture && !flag)
				{
					ResourceHandle res = new ResourceHandle(l, renderGraphResourceType, shared: false);
					try
					{
						graph.m_ResourcesForDebugOnly.GetRenderTargetInfo(in res, out outInfo);
					}
					catch (Exception)
					{
					}
				}
				item.creationPassIndex = reference2.firstUsePassID;
				item.releasePassIndex = reference2.lastUsePassID;
				item.textureData = new RenderGraph.DebugData.TextureResourceData();
				item.textureData.width = reference2.width;
				item.textureData.height = reference2.height;
				item.textureData.depth = reference2.volumeDepth;
				item.textureData.samples = reference2.msaaSamples;
				item.textureData.format = outInfo.format;
				item.textureData.bindMS = reference2.bindMS;
				item.textureData.clearBuffer = reference2.clear;
				item.memoryless = reference2.memoryLess;
				item.consumerList = new List<int>();
				item.producerList = new List<int>();
				if (dictionary.ContainsKey(((RenderGraphResourceType)k, l)))
				{
					item.consumerList = dictionary[((RenderGraphResourceType)k, l)];
				}
				if (dictionary2.ContainsKey(((RenderGraphResourceType)k, l)))
				{
					item.producerList = dictionary2[((RenderGraphResourceType)k, l)];
				}
				debugData.resourceLists[k].Add(item);
			}
		}
		for (int m = 0; m < reference.passData.Length; m++)
		{
			RenderGraphPass renderGraphPass = graph.m_RenderPasses[m];
			ref PassData reference3 = ref reference.passData.ElementAt(m);
			string name2 = InjectSpaces(reference3.GetName(reference).name);
			RenderGraph.DebugData.PassData item2 = default(RenderGraph.DebugData.PassData);
			item2.name = name2;
			item2.type = reference3.type;
			item2.culled = reference3.culled;
			item2.async = reference3.asyncCompute;
			item2.nativeSubPassIndex = reference3.nativeSubPassIndex;
			item2.generateDebugData = renderGraphPass.generateDebugData;
			item2.resourceReadLists = new RenderGraph.DebugData.PassData.ResourceIdLists();
			item2.resourceWriteLists = new RenderGraph.DebugData.PassData.ResourceIdLists();
			item2.syncFromPassIndex = reference3.awaitingMyGraphicsFencePassId;
			item2.syncToPassIndex = reference3.waitOnGraphicsFencePassId;
			item2.nrpInfo = new RenderGraph.DebugData.PassData.NRPInfo();
			item2.nrpInfo.width = reference3.fragmentInfoWidth;
			item2.nrpInfo.height = reference3.fragmentInfoHeight;
			item2.nrpInfo.volumeDepth = reference3.fragmentInfoVolumeDepth;
			item2.nrpInfo.samples = reference3.fragmentInfoSamples;
			item2.nrpInfo.hasDepth = reference3.fragmentInfoHasDepth;
			foreach (var setGlobals in renderGraphPass.setGlobalsList)
			{
				item2.nrpInfo.setGlobals.Add(setGlobals.Item1.handle.index);
			}
			for (int n = 0; n < 3; n++)
			{
				item2.resourceReadLists[n] = new List<int>();
				item2.resourceWriteLists[n] = new List<int>();
				foreach (ResourceHandle item6 in renderGraphPass.resourceReadLists[n])
				{
					if (!renderGraphPass.implicitReadsList.Contains(item6))
					{
						item2.resourceReadLists[n].Add(item6.index);
					}
				}
				foreach (ResourceHandle item7 in renderGraphPass.resourceWriteLists[n])
				{
					item2.resourceWriteLists[n].Add(item7.index);
				}
			}
			ReadOnlySpan<PassFragmentData> readOnlySpan = reference3.FragmentInputs(reference);
			for (int num = 0; num < readOnlySpan.Length; num++)
			{
				PassFragmentData passFragmentData = readOnlySpan[num];
				item2.nrpInfo.textureFBFetchList.Add(passFragmentData.resource.index);
			}
			debugData.passList.Add(item2);
		}
		CompilerContextData.NativePassIterator enumerator4 = reference.NativePasses.GetEnumerator();
		while (enumerator4.MoveNext())
		{
			ref readonly NativePassData current8 = ref enumerator4.Current;
			List<int> list = new List<int>();
			for (int num2 = current8.firstGraphPass; num2 < current8.lastGraphPass + 1; num2++)
			{
				list.Add(num2);
			}
			if (current8.numGraphPasses > 0)
			{
				RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo nativeRenderPassInfo = new RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo();
				nativeRenderPassInfo.passBreakReasoning = MakePassBreakInfoMessage(reference, in current8);
				nativeRenderPassInfo.attachmentInfos = new List<RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.AttachmentInfo>();
				for (int num3 = 0; num3 < current8.attachments.size; num3++)
				{
					nativeRenderPassInfo.attachmentInfos.Add(MakeAttachmentInfo(reference, in current8, num3));
				}
				nativeRenderPassInfo.passCompatibility = new SerializedDictionary<int, RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.PassCompatibilityInfo>();
				nativeRenderPassInfo.mergedPassIds = list;
				for (int num4 = 0; num4 < list.Count; num4++)
				{
					int index = list[num4];
					RenderGraph.DebugData.PassData value = debugData.passList[index];
					value.nrpInfo.nativePassInfo = nativeRenderPassInfo;
					debugData.passList[index] = value;
				}
			}
		}
		for (int num5 = 0; num5 < reference.passData.Length; num5++)
		{
			ref PassData reference4 = ref reference.passData.ElementAt(num5);
			RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo nativePassInfo = debugData.passList[reference4.passId].nrpInfo.nativePassInfo;
			if (nativePassInfo == null)
			{
				continue;
			}
			ReadOnlySpan<PassInputData> readOnlySpan2 = reference4.Inputs(reference);
			for (int num = 0; num < readOnlySpan2.Length; num++)
			{
				ref readonly PassInputData reference5 = ref readOnlySpan2[num];
				ref ResourceVersionedData reference6 = ref reference.VersionedResourceData(in reference5.resource);
				if (reference6.written)
				{
					PassData prevPass = reference.passData[reference6.writePassId];
					PassBreakAudit mergeResult = ((prevPass.nativePassIndex >= 0) ? NativePassData.CanMerge(reference, prevPass.nativePassIndex, reference4.passId) : new PassBreakAudit(PassBreakReason.NonRasterPass, reference4.passId));
					string message = "This pass writes to a resource that is read by the currently selected pass.\n\n" + MakePassMergeMessage(reference, in reference4, in prevPass, in mergeResult);
					nativePassInfo.passCompatibility.TryAdd(prevPass.passId, new RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.PassCompatibilityInfo
					{
						message = message,
						isCompatible = (mergeResult.reason == PassBreakReason.Merged)
					});
				}
			}
			if (reference4.nativePassIndex < 0)
			{
				continue;
			}
			ReadOnlySpan<PassOutputData> readOnlySpan3 = reference4.Outputs(reference);
			for (int num = 0; num < readOnlySpan3.Length; num++)
			{
				ref readonly PassOutputData reference7 = ref readOnlySpan3[num];
				if (reference.UnversionedResourceData(in reference7.resource).lastUsePassID != reference4.passId)
				{
					int numReaders = reference.VersionedResourceData(in reference7.resource).numReaders;
					for (int num6 = 0; num6 < numReaders; num6++)
					{
						int index2 = reference.resources.IndexReader(in reference7.resource, num6);
						ref ResourceReaderData reference8 = ref reference.resources.readerData[reference7.resource.iType].ElementAt(index2);
						PassData pass = reference.passData[reference8.passId];
						PassBreakAudit mergeResult2 = NativePassData.CanMerge(reference, reference4.nativePassIndex, pass.passId);
						string message2 = "This pass reads a resource that is written to by the currently selected pass.\n\n" + MakePassMergeMessage(reference, in pass, in reference4, in mergeResult2);
						nativePassInfo.passCompatibility.TryAdd(pass.passId, new RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.PassCompatibilityInfo
						{
							message = message2,
							isCompatible = (mergeResult2.reason == PassBreakReason.Merged)
						});
					}
				}
			}
		}
	}
}
