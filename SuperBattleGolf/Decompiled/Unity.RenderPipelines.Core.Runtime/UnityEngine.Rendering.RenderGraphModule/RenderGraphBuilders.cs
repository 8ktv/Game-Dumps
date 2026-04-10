using System;
using System.Diagnostics;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace UnityEngine.Rendering.RenderGraphModule;

internal class RenderGraphBuilders : IBaseRenderGraphBuilder, IDisposable, IComputeRenderGraphBuilder, IRasterRenderGraphBuilder, IRenderAttachmentRenderGraphBuilder, IUnsafeRenderGraphBuilder
{
	private RenderGraphPass m_RenderPass;

	private RenderGraphResourceRegistry m_Resources;

	private RenderGraph m_RenderGraph;

	private bool m_Disposed;

	public RenderGraphBuilders()
	{
		m_RenderPass = null;
		m_Resources = null;
		m_RenderGraph = null;
		m_Disposed = true;
	}

	public void Setup(RenderGraphPass renderPass, RenderGraphResourceRegistry resources, RenderGraph renderGraph)
	{
		m_RenderPass = renderPass;
		m_Resources = resources;
		m_RenderGraph = renderGraph;
		m_Disposed = false;
		renderPass.useAllGlobalTextures = false;
		if (renderPass.type == RenderGraphPassType.Raster)
		{
			CommandBuffer.ThrowOnSetRenderTarget = true;
		}
	}

	public void EnableAsyncCompute(bool value)
	{
		m_RenderPass.EnableAsyncCompute(value);
	}

	public void AllowPassCulling(bool value)
	{
		if (!value || !m_RenderPass.allowGlobalState)
		{
			m_RenderPass.AllowPassCulling(value);
		}
	}

	public void AllowGlobalStateModification(bool value)
	{
		m_RenderPass.AllowGlobalState(value);
		if (value)
		{
			AllowPassCulling(value: false);
		}
	}

	public void EnableFoveatedRasterization(bool value)
	{
		m_RenderPass.EnableFoveatedRasterization(value);
	}

	public BufferHandle CreateTransientBuffer(in BufferDesc desc)
	{
		BufferHandle result = m_Resources.CreateBuffer(in desc, m_RenderPass.index);
		UseTransientResource(in result.handle);
		return result;
	}

	public BufferHandle CreateTransientBuffer(in BufferHandle computebuffer)
	{
		return CreateTransientBuffer(in m_Resources.GetBufferResourceDesc(in computebuffer.handle));
	}

	public TextureHandle CreateTransientTexture(in TextureDesc desc)
	{
		TextureHandle result = m_Resources.CreateTexture(in desc, m_RenderPass.index);
		UseTransientResource(in result.handle);
		return result;
	}

	public TextureHandle CreateTransientTexture(in TextureHandle texture)
	{
		return CreateTransientTexture(in m_Resources.GetTextureResourceDesc(in texture.handle));
	}

	public void GenerateDebugData(bool value)
	{
		m_RenderPass.GenerateDebugData(value);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (m_Disposed)
		{
			return;
		}
		try
		{
			if (!disposing)
			{
				return;
			}
			m_RenderGraph.RenderGraphState = RenderGraphState.RecordingGraph;
			if (m_RenderPass.useAllGlobalTextures)
			{
				foreach (TextureHandle item in m_RenderGraph.AllGlobals())
				{
					TextureHandle input = item;
					if (input.IsValid())
					{
						UseTexture(in input, AccessFlags.Read);
					}
				}
			}
			foreach (var setGlobals in m_RenderPass.setGlobalsList)
			{
				(TextureHandle, int) current = setGlobals;
				m_RenderGraph.SetGlobal(in current.Item1, current.Item2);
			}
			m_RenderGraph.OnPassAdded(m_RenderPass);
		}
		finally
		{
			if (m_RenderPass.type == RenderGraphPassType.Raster)
			{
				CommandBuffer.ThrowOnSetRenderTarget = false;
			}
			m_RenderPass = null;
			m_Resources = null;
			m_RenderGraph = null;
			m_Disposed = true;
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckWriteTo(in ResourceHandle handle)
	{
		if (RenderGraph.enableValidityChecks)
		{
			if (handle.IsVersioned)
			{
				string renderGraphResourceName = m_Resources.GetRenderGraphResourceName(in handle);
				throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName}' of type {handle.type} at index {handle.index} - " + "The pass writes to a versioned resource handle. You can only write to unversioned resource handles to avoid branches in the resource history.");
			}
			if (m_RenderPass.IsWritten(in handle))
			{
				string renderGraphResourceName2 = m_Resources.GetRenderGraphResourceName(in handle);
				throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName2}' of type {handle.type} at index {handle.index} - " + "The pass writes to a resource twice. You can only write the same resource once within a pass.");
			}
		}
	}

	private ResourceHandle UseTransientResource(in ResourceHandle inputHandle)
	{
		ResourceHandle res = (inputHandle.IsVersioned ? inputHandle : m_Resources.GetLatestVersionHandle(in inputHandle));
		m_RenderPass.AddTransientResource(in res);
		return res;
	}

	private ResourceHandle UseResource(in ResourceHandle inputHandle, AccessFlags flags)
	{
		bool num = (flags & AccessFlags.Discard) != 0;
		bool flag = (flags & AccessFlags.Read) != 0;
		bool flag2 = (flags & AccessFlags.Write) != 0;
		ResourceHandle res = (inputHandle.IsVersioned ? inputHandle : m_Resources.GetLatestVersionHandle(in inputHandle));
		if (!num)
		{
			m_Resources.IncrementReadCount(in res);
			m_RenderPass.AddResourceRead(in res);
			if (!flag)
			{
				m_RenderPass.implicitReadsList.Add(res);
			}
		}
		else if (flag)
		{
			ResourceHandle res2 = m_Resources.GetZeroVersionHandle(in res);
			m_Resources.IncrementReadCount(in res2);
			m_RenderPass.AddResourceRead(in res2);
		}
		if (flag2)
		{
			res = m_Resources.IncrementWriteCount(in inputHandle);
			m_RenderPass.AddResourceWrite(in res);
		}
		return res;
	}

	public BufferHandle UseBuffer(in BufferHandle input, AccessFlags flags)
	{
		UseResource(in input.handle, flags);
		return input;
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckNotUseFragment(in TextureHandle tex)
	{
		if (!RenderGraph.enableValidityChecks)
		{
			return;
		}
		bool flag = m_RenderPass.depthAccess.textureHandle.IsValid() && m_RenderPass.depthAccess.textureHandle.handle.index == tex.handle.index;
		if (!flag)
		{
			for (int i = 0; i <= m_RenderPass.colorBufferMaxIndex; i++)
			{
				if (m_RenderPass.colorBufferAccess[i].textureHandle.IsValid() && m_RenderPass.colorBufferAccess[i].textureHandle.handle.index == tex.handle.index)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			string renderGraphResourceName = m_Resources.GetRenderGraphResourceName(in tex.handle);
			throw new ArgumentException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName}' of type {tex.handle.type} at index {tex.handle.index} - " + "UseTexture is called on a texture that is already used through SetRenderAttachment. Check your code and make sure the texture is only used once.");
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckTextureUVOriginIsValid(in ResourceHandle handle, TextureResource texRes)
	{
		if (texRes.textureUVOrigin == TextureUVOriginSelection.TopLeft)
		{
			string renderGraphResourceName = m_Resources.GetRenderGraphResourceName(in handle);
			throw new ArgumentException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName}' of type `{handle.type}` at index `{handle.index}` - " + RenderGraph.RenderGraphExceptionMessages.IncompatibleTextureUVOriginUseTexture(texRes.textureUVOrigin));
		}
	}

	public void UseTexture(in TextureHandle input, AccessFlags flags)
	{
		UseResource(in input.handle, flags);
		if ((flags & AccessFlags.Read) == AccessFlags.Read && m_RenderGraph.renderTextureUVOriginStrategy == RenderTextureUVOriginStrategy.PropagateAttachmentOrientation)
		{
			m_Resources.GetTextureResource(in input.handle).textureUVOrigin = TextureUVOriginSelection.BottomLeft;
		}
	}

	public void UseGlobalTexture(int propertyId, AccessFlags flags)
	{
		TextureHandle input = m_RenderGraph.GetGlobal(propertyId);
		if (input.IsValid())
		{
			UseTexture(in input, flags);
			return;
		}
		string renderGraphResourceName = m_Resources.GetRenderGraphResourceName(in input.handle);
		throw new ArgumentException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName}' of type {input.handle.type} at index {input.handle.index} - " + RenderGraph.RenderGraphExceptionMessages.NoGlobalTextureAtPropertyID(propertyId));
	}

	public void UseAllGlobalTextures(bool enable)
	{
		m_RenderPass.useAllGlobalTextures = enable;
	}

	public void SetGlobalTextureAfterPass(in TextureHandle input, int propertyId)
	{
		m_RenderPass.setGlobalsList.Add(ValueTuple.Create(input, propertyId));
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckUseFragment(in TextureHandle tex, bool isDepth)
	{
		if (!RenderGraph.enableValidityChecks)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < m_RenderPass.resourceReadLists[tex.handle.iType].Count; i++)
		{
			if (m_RenderPass.resourceReadLists[tex.handle.iType][i].index == tex.handle.index)
			{
				flag = true;
				break;
			}
		}
		for (int j = 0; j < m_RenderPass.resourceWriteLists[tex.handle.iType].Count; j++)
		{
			if (m_RenderPass.resourceWriteLists[tex.handle.iType][j].index == tex.handle.index)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			string renderGraphResourceName = m_Resources.GetRenderGraphResourceName(in tex.handle);
			throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName}' of type {tex.handle.type} at index {tex.handle.index} - " + "SetRenderAttachment is called on a texture that is already used through UseTexture/SetRenderAttachment. Check your code and make sure the texture is only used once.");
		}
		m_Resources.GetRenderTargetInfo(in tex.handle, out var outInfo);
		if (m_RenderGraph.nativeRenderPassesEnabled)
		{
			if (isDepth)
			{
				if (!GraphicsFormatUtility.IsDepthFormat(outInfo.format))
				{
					string renderGraphResourceName2 = m_Resources.GetRenderGraphResourceName(in tex.handle);
					throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName2}' of type {tex.handle.type} at index {tex.handle.index} - " + RenderGraph.RenderGraphExceptionMessages.UseDepthWithColorFormat(outInfo.format));
				}
			}
			else if (GraphicsFormatUtility.IsDepthFormat(outInfo.format))
			{
				string renderGraphResourceName3 = m_Resources.GetRenderGraphResourceName(in tex.handle);
				throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName3}' of type {tex.handle.type} at index {tex.handle.index} - " + "SetRenderAttachment is called on a texture that has a depth format. Use a texture with a color format instead, or call SetRenderDepthAttachment.");
			}
			if (m_RenderGraph.renderTextureUVOriginStrategy == RenderTextureUVOriginStrategy.PropagateAttachmentOrientation)
			{
				TextureResource textureResource = m_Resources.GetTextureResource(in tex.handle);
				TextureResource textureResource2 = null;
				for (int k = 0; k < m_RenderPass.fragmentInputMaxIndex + 1; k++)
				{
					if (m_RenderPass.fragmentInputAccess[k].textureHandle.IsValid())
					{
						ref readonly TextureHandle textureHandle = ref m_RenderPass.fragmentInputAccess[k].textureHandle;
						textureResource2 = m_Resources.GetTextureResource(in textureHandle.handle);
						if (textureResource.textureUVOrigin != TextureUVOriginSelection.Unknown && textureResource2.textureUVOrigin != TextureUVOriginSelection.Unknown && textureResource.textureUVOrigin != textureResource2.textureUVOrigin)
						{
							string renderGraphResourceName4 = m_Resources.GetRenderGraphResourceName(in tex.handle);
							string renderGraphResourceName5 = m_Resources.GetRenderGraphResourceName(in textureHandle.handle);
							throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName4}' of type {tex.handle.type} at index {tex.handle.index} - " + RenderGraph.RenderGraphExceptionMessages.IncompatibleTextureUVOrigin(textureResource.textureUVOrigin, "input", renderGraphResourceName5, textureHandle.handle.type, textureHandle.handle.index, textureResource2.textureUVOrigin));
						}
					}
				}
				for (int l = 0; l < m_RenderPass.colorBufferMaxIndex + 1; l++)
				{
					if (m_RenderPass.colorBufferAccess[l].textureHandle.IsValid())
					{
						ref readonly TextureHandle textureHandle2 = ref m_RenderPass.colorBufferAccess[l].textureHandle;
						textureResource2 = m_Resources.GetTextureResource(in textureHandle2.handle);
						if (textureResource.textureUVOrigin != TextureUVOriginSelection.Unknown && textureResource2.textureUVOrigin != TextureUVOriginSelection.Unknown && textureResource.textureUVOrigin != textureResource2.textureUVOrigin)
						{
							string renderGraphResourceName6 = m_Resources.GetRenderGraphResourceName(in tex.handle);
							string renderGraphResourceName7 = m_Resources.GetRenderGraphResourceName(in textureHandle2.handle);
							throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName6}' of type {tex.handle.type} at index {tex.handle.index} - " + RenderGraph.RenderGraphExceptionMessages.IncompatibleTextureUVOrigin(textureResource.textureUVOrigin, "render", renderGraphResourceName7, textureHandle2.handle.type, textureHandle2.handle.index, textureResource2.textureUVOrigin));
						}
					}
				}
				if (!isDepth && m_RenderPass.depthAccess.textureHandle.IsValid())
				{
					TextureHandle textureHandle3 = m_RenderPass.depthAccess.textureHandle;
					textureResource2 = m_Resources.GetTextureResource(in textureHandle3.handle);
					if (textureResource.textureUVOrigin != TextureUVOriginSelection.Unknown && textureResource2.textureUVOrigin != TextureUVOriginSelection.Unknown && textureResource.textureUVOrigin != textureResource2.textureUVOrigin)
					{
						string renderGraphResourceName8 = m_Resources.GetRenderGraphResourceName(in tex.handle);
						string renderGraphResourceName9 = m_Resources.GetRenderGraphResourceName(in textureHandle3.handle);
						throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName8}' of type {tex.handle.type} at index {tex.handle.index} - " + RenderGraph.RenderGraphExceptionMessages.IncompatibleTextureUVOrigin(textureResource.textureUVOrigin, "depth", renderGraphResourceName9, textureHandle3.handle.type, textureHandle3.handle.index, textureResource2.textureUVOrigin));
					}
				}
			}
		}
		foreach (var setGlobals in m_RenderPass.setGlobalsList)
		{
			if (setGlobals.Item1.handle.index == tex.handle.index)
			{
				string renderGraphResourceName10 = m_Resources.GetRenderGraphResourceName(in tex.handle);
				throw new InvalidOperationException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName10}' of type {tex.handle.type} at index {tex.handle.index} - " + "SetRenderAttachment is called on a texture that is currently bound to a global texture slot. Shaders might be using the texture using samplers. Make sure textures are not set as globals when using them as fragment attachments.");
			}
		}
	}

	public void SetRenderAttachment(TextureHandle tex, int index, AccessFlags flags, int mipLevel, int depthSlice)
	{
		TextureHandle resource = new TextureHandle(UseResource(in tex.handle, flags));
		m_RenderPass.SetColorBufferRaw(in resource, index, flags, mipLevel, depthSlice);
	}

	public void SetInputAttachment(TextureHandle tex, int index, AccessFlags flags, int mipLevel, int depthSlice)
	{
		TextureHandle resource = new TextureHandle(UseResource(in tex.handle, flags));
		m_RenderPass.SetFragmentInputRaw(in resource, index, flags, mipLevel, depthSlice);
	}

	public void SetRenderAttachmentDepth(TextureHandle tex, AccessFlags flags, int mipLevel, int depthSlice)
	{
		TextureHandle resource = new TextureHandle(UseResource(in tex.handle, flags));
		m_RenderPass.SetDepthBufferRaw(in resource, flags, mipLevel, depthSlice);
	}

	public TextureHandle SetRandomAccessAttachment(TextureHandle input, int index, AccessFlags flags = AccessFlags.Read)
	{
		ResourceHandle resource = UseResource(in input.handle, flags);
		m_RenderPass.SetRandomWriteResourceRaw(in resource, index, preserveCounterValue: false, flags);
		return input;
	}

	public void SetShadingRateImageAttachment(in TextureHandle tex)
	{
		TextureHandle shadingRateImage = new TextureHandle(UseResource(in tex.handle, AccessFlags.Read));
		m_RenderPass.SetShadingRateImageRaw(in shadingRateImage);
	}

	public BufferHandle UseBufferRandomAccess(BufferHandle input, int index, AccessFlags flags = AccessFlags.Read)
	{
		BufferHandle bufferHandle = UseBuffer(in input, flags);
		m_RenderPass.SetRandomWriteResourceRaw(in bufferHandle.handle, index, preserveCounterValue: true, flags);
		return input;
	}

	public BufferHandle UseBufferRandomAccess(BufferHandle input, int index, bool preserveCounterValue, AccessFlags flags = AccessFlags.Read)
	{
		BufferHandle bufferHandle = UseBuffer(in input, flags);
		m_RenderPass.SetRandomWriteResourceRaw(in bufferHandle.handle, index, preserveCounterValue, flags);
		return input;
	}

	public void SetRenderFunc<PassData>(BaseRenderFunc<PassData, ComputeGraphContext> renderFunc) where PassData : class, new()
	{
		((ComputeRenderGraphPass<PassData>)m_RenderPass).renderFunc = renderFunc;
	}

	public void SetRenderFunc<PassData>(BaseRenderFunc<PassData, RasterGraphContext> renderFunc) where PassData : class, new()
	{
		((RasterRenderGraphPass<PassData>)m_RenderPass).renderFunc = renderFunc;
	}

	public void SetRenderFunc<PassData>(BaseRenderFunc<PassData, UnsafeGraphContext> renderFunc) where PassData : class, new()
	{
		((UnsafeRenderGraphPass<PassData>)m_RenderPass).renderFunc = renderFunc;
	}

	public void UseRendererList(in RendererListHandle input)
	{
		m_RenderPass.UseRendererList(in input);
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckResource(in ResourceHandle res, bool checkTransientReadWrite = false)
	{
		if (RenderGraph.enableValidityChecks)
		{
			if (!res.IsValid())
			{
				string renderGraphResourceName = m_Resources.GetRenderGraphResourceName(in res);
				throw new Exception($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName}' of type {res.type} at index {res.index} - " + "Using an invalid resource. Invalid resources can be resources leftover from a previous execution.");
			}
			int renderGraphResourceTransientIndex = m_Resources.GetRenderGraphResourceTransientIndex(in res);
			if (renderGraphResourceTransientIndex == m_RenderPass.index && checkTransientReadWrite)
			{
				string renderGraphResourceName2 = m_Resources.GetRenderGraphResourceName(in res);
				Debug.LogError($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName2}' of type {res.type} at index {res.index} - " + "This pass is reading or writing a transient resource. Transient resources are always assumed to be both read and written using 'AccessFlags.ReadWrite'.");
			}
			if (renderGraphResourceTransientIndex != -1 && renderGraphResourceTransientIndex != m_RenderPass.index)
			{
				string renderGraphResourceName3 = m_Resources.GetRenderGraphResourceName(in res);
				throw new ArgumentException($"In pass '{m_RenderPass.name}' when trying to use resource '{renderGraphResourceName3}' of type {res.type} at index {res.index} - " + RenderGraph.RenderGraphExceptionMessages.UseTransientTextureInWrongPass(renderGraphResourceTransientIndex));
			}
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private void CheckFrameBufferFetchEmulationIsSupported(in TextureHandle tex)
	{
		if (RenderGraph.enableValidityChecks)
		{
			if (!RenderGraphUtils.IsFramebufferFetchEmulationSupportedOnCurrentPlatform())
			{
				throw new InvalidOperationException($"This API is not supported on the current platform: {SystemInfo.graphicsDeviceType}");
			}
			if (!RenderGraphUtils.IsFramebufferFetchEmulationMSAASupportedOnCurrentPlatform() && m_RenderGraph.GetRenderTargetInfo(tex).bindMS)
			{
				throw new InvalidOperationException($"This API is not supported with MSAA attachments on the current platform: {SystemInfo.graphicsDeviceType}");
			}
		}
	}

	public void SetShadingRateFragmentSize(ShadingRateFragmentSize shadingRateFragmentSize)
	{
		m_RenderPass.SetShadingRateFragmentSize(shadingRateFragmentSize);
	}

	public void SetShadingRateCombiner(ShadingRateCombinerStage stage, ShadingRateCombiner combiner)
	{
		m_RenderPass.SetShadingRateCombiner(stage, combiner);
	}

	public void SetExtendedFeatureFlags(ExtendedFeatureFlags extendedFeatureFlags)
	{
		m_RenderPass.SetExtendedFeatureFlags(extendedFeatureFlags);
	}

	void IRasterRenderGraphBuilder.SetShadingRateImageAttachment(in TextureHandle tex)
	{
		SetShadingRateImageAttachment(in tex);
	}

	void IBaseRenderGraphBuilder.UseTexture(in TextureHandle input, AccessFlags flags)
	{
		UseTexture(in input, flags);
	}

	void IBaseRenderGraphBuilder.SetGlobalTextureAfterPass(in TextureHandle input, int propertyId)
	{
		SetGlobalTextureAfterPass(in input, propertyId);
	}

	BufferHandle IBaseRenderGraphBuilder.UseBuffer(in BufferHandle input, AccessFlags flags)
	{
		return UseBuffer(in input, flags);
	}

	TextureHandle IBaseRenderGraphBuilder.CreateTransientTexture(in TextureDesc desc)
	{
		return CreateTransientTexture(in desc);
	}

	TextureHandle IBaseRenderGraphBuilder.CreateTransientTexture(in TextureHandle texture)
	{
		return CreateTransientTexture(in texture);
	}

	BufferHandle IBaseRenderGraphBuilder.CreateTransientBuffer(in BufferDesc desc)
	{
		return CreateTransientBuffer(in desc);
	}

	BufferHandle IBaseRenderGraphBuilder.CreateTransientBuffer(in BufferHandle computebuffer)
	{
		return CreateTransientBuffer(in computebuffer);
	}

	void IBaseRenderGraphBuilder.UseRendererList(in RendererListHandle input)
	{
		UseRendererList(in input);
	}
}
