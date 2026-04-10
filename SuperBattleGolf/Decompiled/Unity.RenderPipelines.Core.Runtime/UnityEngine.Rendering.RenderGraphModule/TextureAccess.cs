namespace UnityEngine.Rendering.RenderGraphModule;

internal readonly struct TextureAccess
{
	public readonly TextureHandle textureHandle;

	public readonly int mipLevel;

	public readonly int depthSlice;

	public readonly AccessFlags flags;

	public TextureAccess(in TextureHandle handle, AccessFlags flags, int mipLevel, int depthSlice)
	{
		textureHandle = handle;
		this.flags = flags;
		this.mipLevel = mipLevel;
		this.depthSlice = depthSlice;
	}

	public TextureAccess(in TextureAccess access, in TextureHandle handle)
	{
		textureHandle = handle;
		flags = access.flags;
		mipLevel = access.mipLevel;
		depthSlice = access.depthSlice;
	}
}
