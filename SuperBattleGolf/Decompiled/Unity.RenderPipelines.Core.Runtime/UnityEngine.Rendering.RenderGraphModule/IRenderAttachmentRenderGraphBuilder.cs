using System;

namespace UnityEngine.Rendering.RenderGraphModule;

public interface IRenderAttachmentRenderGraphBuilder : IBaseRenderGraphBuilder, IDisposable
{
	void SetRenderAttachment(TextureHandle tex, int index, AccessFlags flags = AccessFlags.Write)
	{
		SetRenderAttachment(tex, index, flags, 0, -1);
	}

	void SetRenderAttachment(TextureHandle tex, int index, AccessFlags flags, int mipLevel, int depthSlice);

	void SetRenderAttachmentDepth(TextureHandle tex, AccessFlags flags = AccessFlags.Write)
	{
		SetRenderAttachmentDepth(tex, flags, 0, -1);
	}

	void SetRenderAttachmentDepth(TextureHandle tex, AccessFlags flags, int mipLevel, int depthSlice);

	TextureHandle SetRandomAccessAttachment(TextureHandle tex, int index, AccessFlags flags = AccessFlags.ReadWrite);

	BufferHandle UseBufferRandomAccess(BufferHandle tex, int index, AccessFlags flags = AccessFlags.Read);

	BufferHandle UseBufferRandomAccess(BufferHandle tex, int index, bool preserveCounterValue, AccessFlags flags = AccessFlags.Read);
}
