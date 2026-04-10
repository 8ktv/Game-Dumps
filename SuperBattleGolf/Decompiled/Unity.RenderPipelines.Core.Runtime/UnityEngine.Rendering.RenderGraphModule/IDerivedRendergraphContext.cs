namespace UnityEngine.Rendering.RenderGraphModule;

internal interface IDerivedRendergraphContext
{
	void FromInternalContext(InternalRenderGraphContext context);

	TextureUVOrigin GetTextureUVOrigin(in TextureHandle textureHandle);
}
