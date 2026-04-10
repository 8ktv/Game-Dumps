using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule;

[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
[Obsolete("RenderGraphContext is deprecated, use RasterGraphContext/ComputeGraphContext/UnsafeGraphContext instead.")]
public struct RenderGraphContext : IDerivedRendergraphContext
{
	private InternalRenderGraphContext wrappedContext;

	public ScriptableRenderContext renderContext => wrappedContext.renderContext;

	public CommandBuffer cmd => wrappedContext.cmd;

	public RenderGraphObjectPool renderGraphPool => wrappedContext.renderGraphPool;

	public RenderGraphDefaultResources defaultResources => wrappedContext.defaultResources;

	public void FromInternalContext(InternalRenderGraphContext context)
	{
		wrappedContext = context;
	}

	public readonly TextureUVOrigin GetTextureUVOrigin(in TextureHandle textureHandle)
	{
		return TextureUVOrigin.BottomLeft;
	}

	TextureUVOrigin IDerivedRendergraphContext.GetTextureUVOrigin(in TextureHandle textureHandle)
	{
		return GetTextureUVOrigin(in textureHandle);
	}
}
