using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule;

[DebuggerDisplay("RenderPass: {name} (Index:{index} Async:{enableAsyncCompute})")]
internal abstract class RenderGraphPass
{
	public struct RandomWriteResourceInfo
	{
		public ResourceHandle h;

		public bool preserveCounterValue;
	}

	public List<ResourceHandle>[] resourceReadLists = new List<ResourceHandle>[3];

	public List<ResourceHandle>[] resourceWriteLists = new List<ResourceHandle>[3];

	public List<ResourceHandle>[] transientResourceList = new List<ResourceHandle>[3];

	public List<RendererListHandle> usedRendererListList = new List<RendererListHandle>();

	public List<(TextureHandle, int)> setGlobalsList = new List<(TextureHandle, int)>();

	public bool useAllGlobalTextures;

	public List<ResourceHandle> implicitReadsList = new List<ResourceHandle>();

	public string name { get; protected set; }

	public int index { get; protected set; }

	public RenderGraphPassType type { get; internal set; }

	public ProfilingSampler customSampler { get; protected set; }

	public bool enableAsyncCompute { get; protected set; }

	public bool allowPassCulling { get; protected set; }

	public bool allowGlobalState { get; protected set; }

	public bool enableFoveatedRasterization { get; protected set; }

	public ExtendedFeatureFlags extendedFeatureFlags { get; protected set; }

	public TextureAccess depthAccess { get; protected set; }

	public TextureAccess[] colorBufferAccess { get; protected set; } = new TextureAccess[RenderGraph.kMaxMRTCount];

	public int colorBufferMaxIndex { get; protected set; } = -1;

	public bool hasShadingRateImage { get; protected set; }

	public TextureAccess shadingRateAccess { get; protected set; }

	public bool hasShadingRateStates { get; protected set; }

	public ShadingRateFragmentSize shadingRateFragmentSize { get; protected set; }

	public ShadingRateCombiner primitiveShadingRateCombiner { get; protected set; }

	public ShadingRateCombiner fragmentShadingRateCombiner { get; protected set; }

	public TextureAccess[] fragmentInputAccess { get; protected set; } = new TextureAccess[RenderGraph.kMaxMRTCount];

	public int fragmentInputMaxIndex { get; protected set; } = -1;

	public RandomWriteResourceInfo[] randomAccessResource { get; protected set; } = new RandomWriteResourceInfo[RenderGraph.kMaxMRTCount];

	public int randomAccessResourceMaxIndex { get; protected set; } = -1;

	public bool generateDebugData { get; protected set; }

	public bool allowRendererListCulling { get; protected set; }

	public abstract void Execute(InternalRenderGraphContext renderGraphContext);

	public abstract void Release(RenderGraphObjectPool pool);

	public abstract bool HasRenderFunc();

	public abstract int GetRenderFuncHash();

	public RenderGraphPass()
	{
		for (int i = 0; i < 3; i++)
		{
			resourceReadLists[i] = new List<ResourceHandle>();
			resourceWriteLists[i] = new List<ResourceHandle>();
			transientResourceList[i] = new List<ResourceHandle>();
		}
	}

	public void Clear()
	{
		name = "";
		index = -1;
		customSampler = null;
		for (int i = 0; i < 3; i++)
		{
			resourceReadLists[i].Clear();
			resourceWriteLists[i].Clear();
			transientResourceList[i].Clear();
		}
		usedRendererListList.Clear();
		setGlobalsList.Clear();
		useAllGlobalTextures = false;
		implicitReadsList.Clear();
		enableAsyncCompute = false;
		allowPassCulling = true;
		allowRendererListCulling = true;
		allowGlobalState = false;
		enableFoveatedRasterization = false;
		generateDebugData = true;
		colorBufferMaxIndex = -1;
		fragmentInputMaxIndex = -1;
		randomAccessResourceMaxIndex = -1;
		depthAccess = default(TextureAccess);
		hasShadingRateImage = false;
		hasShadingRateStates = false;
		shadingRateFragmentSize = ShadingRateFragmentSize.FragmentSize1x1;
		primitiveShadingRateCombiner = ShadingRateCombiner.Keep;
		fragmentShadingRateCombiner = ShadingRateCombiner.Keep;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasRenderAttachments()
	{
		if (!depthAccess.textureHandle.IsValid() && !colorBufferAccess[0].textureHandle.IsValid())
		{
			return colorBufferMaxIndex > 0;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsTransient(in ResourceHandle res)
	{
		for (int i = 0; i < transientResourceList[res.iType].Count; i++)
		{
			if (transientResourceList[res.iType][i].index == res.index)
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsWritten(in ResourceHandle res)
	{
		for (int i = 0; i < resourceWriteLists[res.iType].Count; i++)
		{
			if (resourceWriteLists[res.iType][i].index == res.index)
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsRead(in ResourceHandle res)
	{
		if (res.IsVersioned)
		{
			return resourceReadLists[res.iType].Contains(res);
		}
		for (int i = 0; i < resourceReadLists[res.iType].Count; i++)
		{
			if (resourceReadLists[res.iType][i].index == res.index)
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsAttachment(in TextureHandle res)
	{
		if (depthAccess.textureHandle.IsValid() && depthAccess.textureHandle.handle.index == res.handle.index)
		{
			return true;
		}
		for (int i = 0; i < colorBufferAccess.Length; i++)
		{
			if (colorBufferAccess[i].textureHandle.IsValid() && colorBufferAccess[i].textureHandle.handle.index == res.handle.index)
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddResourceWrite(in ResourceHandle res)
	{
		resourceWriteLists[res.iType].Add(res);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddResourceRead(in ResourceHandle res)
	{
		resourceReadLists[res.iType].Add(res);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddTransientResource(in ResourceHandle res)
	{
		transientResourceList[res.iType].Add(res);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UseRendererList(in RendererListHandle rendererList)
	{
		usedRendererListList.Add(rendererList);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void EnableAsyncCompute(bool value)
	{
		enableAsyncCompute = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AllowPassCulling(bool value)
	{
		allowPassCulling = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void EnableFoveatedRasterization(bool value)
	{
		enableFoveatedRasterization = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AllowRendererListCulling(bool value)
	{
		allowRendererListCulling = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AllowGlobalState(bool value)
	{
		allowGlobalState = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GenerateDebugData(bool value)
	{
		generateDebugData = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetColorBuffer(in TextureHandle resource, int index)
	{
		colorBufferMaxIndex = Math.Max(colorBufferMaxIndex, index);
		colorBufferAccess[index] = new TextureAccess(in colorBufferAccess[index], in resource);
		AddResourceWrite(in resource.handle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetColorBufferRaw(in TextureHandle resource, int index, AccessFlags accessFlags, int mipLevel, int depthSlice)
	{
		if (colorBufferAccess[index].textureHandle.handle.Equals(resource.handle) || !colorBufferAccess[index].textureHandle.IsValid())
		{
			colorBufferMaxIndex = Math.Max(colorBufferMaxIndex, index);
			colorBufferAccess[index] = new TextureAccess(in resource, accessFlags, mipLevel, depthSlice);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetFragmentInputRaw(in TextureHandle resource, int index, AccessFlags accessFlags, int mipLevel, int depthSlice)
	{
		if (fragmentInputAccess[index].textureHandle.handle.Equals(resource.handle) || !fragmentInputAccess[index].textureHandle.IsValid())
		{
			fragmentInputMaxIndex = Math.Max(fragmentInputMaxIndex, index);
			fragmentInputAccess[index] = new TextureAccess(in resource, accessFlags, mipLevel, depthSlice);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetRandomWriteResourceRaw(in ResourceHandle resource, int index, bool preserveCounterValue, AccessFlags accessFlags)
	{
		if (randomAccessResource[index].h.Equals(resource) || !randomAccessResource[index].h.IsValid())
		{
			randomAccessResourceMaxIndex = Math.Max(randomAccessResourceMaxIndex, index);
			ref RandomWriteResourceInfo reference = ref randomAccessResource[index];
			reference.h = resource;
			reference.preserveCounterValue = preserveCounterValue;
			return;
		}
		throw new InvalidOperationException($"In pass '{name}' when trying to call SetRandomAccessAttachment/UseBufferRandomAccess with resource of type {resource.type} at index {index} - " + "You can only bind a single texture to a random write input index. Verify your indexes are correct.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDepthBuffer(in TextureHandle resource, DepthAccess flags)
	{
		depthAccess = new TextureAccess(in resource, (AccessFlags)flags, 0, 0);
		if ((flags & DepthAccess.Read) != 0)
		{
			AddResourceRead(in resource.handle);
		}
		if ((flags & DepthAccess.Write) != 0)
		{
			AddResourceWrite(in resource.handle);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDepthBufferRaw(in TextureHandle resource, AccessFlags accessFlags, int mipLevel, int depthSlice)
	{
		if (depthAccess.textureHandle.handle.Equals(resource.handle) || !depthAccess.textureHandle.IsValid())
		{
			depthAccess = new TextureAccess(in resource, accessFlags, mipLevel, depthSlice);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ComputeTextureHash(ref HashFNV1A32 generator, in ResourceHandle handle, RenderGraphResourceRegistry resources)
	{
		if (handle.index == 0)
		{
			return;
		}
		if (resources.IsRenderGraphResourceImported(in handle))
		{
			TextureResource textureResource = resources.GetTextureResource(in handle);
			RTHandle graphicsResource = textureResource.graphicsResource;
			ref TextureDesc desc = ref textureResource.desc;
			Texture externalTexture = graphicsResource.externalTexture;
			if (externalTexture != null)
			{
				generator.Append((int)externalTexture.graphicsFormat);
				generator.Append((int)externalTexture.dimension);
				generator.Append(externalTexture.width);
				generator.Append(externalTexture.height);
				if (externalTexture is RenderTexture renderTexture)
				{
					generator.Append(renderTexture.antiAliasing);
				}
			}
			else if (graphicsResource.rt != null)
			{
				RenderTexture rt = graphicsResource.rt;
				generator.Append((int)rt.graphicsFormat);
				generator.Append((int)rt.dimension);
				generator.Append(rt.antiAliasing);
				if (graphicsResource.useScaling)
				{
					if (graphicsResource.scaleFunc != null)
					{
						generator.Append(DelegateHashCodeUtils.GetFuncHashCode(graphicsResource.scaleFunc));
					}
					else
					{
						generator.Append(graphicsResource.scaleFactor);
					}
				}
				else
				{
					generator.Append(rt.width);
					generator.Append(rt.height);
				}
			}
			else if (graphicsResource.nameID != default(RenderTargetIdentifier))
			{
				int input = (int)desc.format;
				generator.Append(in input);
				input = (int)desc.dimension;
				generator.Append(in input);
				input = (int)desc.msaaSamples;
				generator.Append(in input);
				generator.Append(in desc.width);
				generator.Append(in desc.height);
			}
			generator.Append(in desc.clearBuffer);
			generator.Append(in desc.discardBuffer);
		}
		else
		{
			ref readonly TextureDesc textureResourceDesc = ref resources.GetTextureResourceDesc(in handle);
			int input = (int)textureResourceDesc.format;
			generator.Append(in input);
			input = (int)textureResourceDesc.dimension;
			generator.Append(in input);
			input = (int)textureResourceDesc.msaaSamples;
			generator.Append(in input);
			generator.Append(in textureResourceDesc.clearBuffer);
			generator.Append(in textureResourceDesc.discardBuffer);
			switch (textureResourceDesc.sizeMode)
			{
			case TextureSizeMode.Explicit:
				generator.Append(in textureResourceDesc.width);
				generator.Append(in textureResourceDesc.height);
				break;
			case TextureSizeMode.Scale:
				generator.Append(in textureResourceDesc.scale);
				break;
			case TextureSizeMode.Functor:
				generator.Append(DelegateHashCodeUtils.GetFuncHashCode(textureResourceDesc.func));
				break;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ComputeHashForTextureAccess(ref HashFNV1A32 generator, in ResourceHandle handle, in TextureAccess textureAccess)
	{
		generator.Append(handle.index);
		int input = (int)textureAccess.flags;
		generator.Append(in input);
		generator.Append(in textureAccess.mipLevel);
		generator.Append(in textureAccess.depthSlice);
	}

	public void ComputeHash(ref HashFNV1A32 generator, RenderGraphResourceRegistry resources)
	{
		generator.Append((int)type);
		generator.Append(enableAsyncCompute);
		generator.Append(allowPassCulling);
		generator.Append(allowGlobalState);
		generator.Append(enableFoveatedRasterization);
		ResourceHandle handle = depthAccess.textureHandle.handle;
		TextureAccess textureAccess;
		if (handle.IsValid())
		{
			ComputeTextureHash(ref generator, in handle, resources);
			textureAccess = depthAccess;
			ComputeHashForTextureAccess(ref generator, in handle, in textureAccess);
		}
		for (int i = 0; i < colorBufferMaxIndex + 1; i++)
		{
			TextureAccess textureAccess2 = colorBufferAccess[i];
			ResourceHandle handle2 = textureAccess2.textureHandle.handle;
			if (handle2.IsValid())
			{
				ComputeTextureHash(ref generator, in handle2, resources);
				ComputeHashForTextureAccess(ref generator, in handle2, in textureAccess2);
			}
		}
		generator.Append(colorBufferMaxIndex);
		generator.Append(hasShadingRateImage);
		if (hasShadingRateImage)
		{
			ResourceHandle handle3 = shadingRateAccess.textureHandle.handle;
			if (handle3.IsValid())
			{
				ComputeTextureHash(ref generator, in handle3, resources);
				textureAccess = shadingRateAccess;
				ComputeHashForTextureAccess(ref generator, in handle3, in textureAccess);
			}
		}
		generator.Append(hasShadingRateStates);
		generator.Append((int)shadingRateFragmentSize);
		generator.Append((int)primitiveShadingRateCombiner);
		generator.Append((int)fragmentShadingRateCombiner);
		for (int j = 0; j < fragmentInputMaxIndex + 1; j++)
		{
			TextureAccess textureAccess3 = fragmentInputAccess[j];
			ResourceHandle handle4 = textureAccess3.textureHandle.handle;
			if (handle4.IsValid())
			{
				ComputeTextureHash(ref generator, in handle4, resources);
				ComputeHashForTextureAccess(ref generator, in handle4, in textureAccess3);
			}
		}
		for (int k = 0; k < randomAccessResourceMaxIndex + 1; k++)
		{
			RandomWriteResourceInfo randomWriteResourceInfo = randomAccessResource[k];
			if (randomWriteResourceInfo.h.IsValid())
			{
				generator.Append(randomWriteResourceInfo.h.index);
				generator.Append(in randomWriteResourceInfo.preserveCounterValue);
			}
		}
		generator.Append(randomAccessResourceMaxIndex);
		generator.Append(fragmentInputMaxIndex);
		generator.Append(generateDebugData);
		generator.Append(allowRendererListCulling);
		for (int l = 0; l < 3; l++)
		{
			List<ResourceHandle> list = resourceReadLists[l];
			int count = list.Count;
			for (int m = 0; m < count; m++)
			{
				generator.Append(list[m].index);
			}
			List<ResourceHandle> list2 = resourceWriteLists[l];
			int count2 = list2.Count;
			for (int n = 0; n < count2; n++)
			{
				generator.Append(list2[n].index);
			}
			List<ResourceHandle> list3 = transientResourceList[l];
			int count3 = list3.Count;
			for (int num = 0; num < count3; num++)
			{
				generator.Append(list3[num].index);
			}
		}
		int count4 = usedRendererListList.Count;
		for (int num2 = 0; num2 < count4; num2++)
		{
			generator.Append(usedRendererListList[num2].handle);
		}
		int count5 = setGlobalsList.Count;
		for (int num3 = 0; num3 < count5; num3++)
		{
			(TextureHandle, int) tuple = setGlobalsList[num3];
			generator.Append(tuple.Item1.handle.index);
			generator.Append(in tuple.Item2);
		}
		generator.Append(in useAllGlobalTextures);
		int count6 = implicitReadsList.Count;
		for (int num4 = 0; num4 < count6; num4++)
		{
			generator.Append(implicitReadsList[num4].index);
		}
		generator.Append(GetRenderFuncHash());
	}

	public void SetShadingRateImageRaw(in TextureHandle shadingRateImage)
	{
		if (ShadingRateInfo.supportsPerImageTile)
		{
			hasShadingRateImage = true;
			shadingRateAccess = new TextureAccess(in shadingRateImage, AccessFlags.Read, 0, 0);
		}
	}

	public void SetShadingRateImage(in TextureHandle shadingRateImage, AccessFlags accessFlags, int mipLevel, int depthSlice)
	{
		if (ShadingRateInfo.supportsPerImageTile)
		{
			hasShadingRateImage = true;
			shadingRateAccess = new TextureAccess(in shadingRateImage, accessFlags, mipLevel, depthSlice);
			TextureAccess textureAccess = shadingRateAccess;
			AddResourceRead(in textureAccess.textureHandle.handle);
		}
	}

	public void SetShadingRateFragmentSize(ShadingRateFragmentSize shadingRateFragmentSize)
	{
		if (ShadingRateInfo.supportsPerDrawCall)
		{
			hasShadingRateStates = true;
			this.shadingRateFragmentSize = shadingRateFragmentSize;
		}
	}

	public void SetShadingRateCombiner(ShadingRateCombinerStage stage, ShadingRateCombiner combiner)
	{
		if (ShadingRateInfo.supportsPerImageTile)
		{
			switch (stage)
			{
			case ShadingRateCombinerStage.Primitive:
				hasShadingRateStates = true;
				primitiveShadingRateCombiner = combiner;
				break;
			case ShadingRateCombinerStage.Fragment:
				hasShadingRateStates = true;
				fragmentShadingRateCombiner = combiner;
				break;
			}
		}
	}

	public void SetExtendedFeatureFlags(ExtendedFeatureFlags value)
	{
		extendedFeatureFlags |= value;
	}
}
[DebuggerDisplay("RenderPass: {name} (Index:{index} Async:{enableAsyncCompute})")]
[Obsolete("RenderGraphPass is deprecated, use RasterRenderGraphPass/ComputeRenderGraphPass/UnsafeRenderGraphPass instead.")]
internal sealed class RenderGraphPass<PassData> : BaseRenderGraphPass<PassData, RenderGraphContext> where PassData : class, new()
{
	internal static RenderGraphContext c;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Execute(InternalRenderGraphContext renderGraphContext)
	{
		c.FromInternalContext(renderGraphContext);
		renderFunc(data, c);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override void Release(RenderGraphObjectPool pool)
	{
		base.Release(pool);
		pool.Release(this);
	}
}
