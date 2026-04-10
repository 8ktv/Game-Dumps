using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule;

[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
public interface IRasterRenderGraphBuilder : IRenderAttachmentRenderGraphBuilder, IBaseRenderGraphBuilder, IDisposable
{
	void SetInputAttachment(TextureHandle tex, int index, AccessFlags flags = AccessFlags.Read)
	{
		SetInputAttachment(tex, index, flags, 0, -1);
	}

	void SetInputAttachment(TextureHandle tex, int index, AccessFlags flags, int mipLevel, int depthSlice);

	void SetShadingRateImageAttachment(in TextureHandle tex);

	void SetShadingRateFragmentSize(ShadingRateFragmentSize shadingRateFragmentSize);

	void SetShadingRateCombiner(ShadingRateCombinerStage stage, ShadingRateCombiner combiner);

	void SetExtendedFeatureFlags(ExtendedFeatureFlags extendedFeatureFlags);

	void SetRenderFunc<PassData>(BaseRenderFunc<PassData, RasterGraphContext> renderFunc) where PassData : class, new();
}
