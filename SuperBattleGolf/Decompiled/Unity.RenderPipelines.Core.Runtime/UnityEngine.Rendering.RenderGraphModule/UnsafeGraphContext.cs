using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule;

[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
public class UnsafeGraphContext : IDerivedRendergraphContext
{
	private InternalRenderGraphContext wrappedContext;

	public UnsafeCommandBuffer cmd;

	internal static UnsafeCommandBuffer unsCmd = new UnsafeCommandBuffer(null, null, isAsync: false);

	public RenderGraphDefaultResources defaultResources => wrappedContext.defaultResources;

	public RenderGraphObjectPool renderGraphPool => wrappedContext.renderGraphPool;

	public void FromInternalContext(InternalRenderGraphContext context)
	{
		wrappedContext = context;
		unsCmd.m_WrappedCommandBuffer = wrappedContext.cmd;
		unsCmd.m_ExecutingPass = context.executingPass;
		cmd = unsCmd;
	}

	public TextureUVOrigin GetTextureUVOrigin(in TextureHandle textureHandle)
	{
		if (!SystemInfo.graphicsUVStartsAtTop)
		{
			return TextureUVOrigin.BottomLeft;
		}
		if (wrappedContext.compilerContext != null)
		{
			return wrappedContext.compilerContext.GetTextureUVOrigin(in textureHandle);
		}
		return TextureUVOrigin.BottomLeft;
	}

	TextureUVOrigin IDerivedRendergraphContext.GetTextureUVOrigin(in TextureHandle textureHandle)
	{
		return GetTextureUVOrigin(in textureHandle);
	}
}
