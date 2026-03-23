using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Graphics/ShadingRateImage.h")]
public static class ShadingRateImage
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShadingRateImage::GetAllocSizeInternal")]
	internal static extern void GetAllocSizeInternal(int pixelWidth, int pixelHeight, out int tileWidth, out int tileHeight);

	public static Vector2Int GetAllocTileSize(Vector2Int pixelSize)
	{
		return GetAllocTileSize(pixelSize.x, pixelSize.y);
	}

	public static Vector2Int GetAllocTileSize(int pixelWidth, int pixelHeight)
	{
		GetAllocSizeInternal(pixelWidth, pixelHeight, out var tileWidth, out var tileHeight);
		return new Vector2Int(tileWidth, tileHeight);
	}

	public static RenderTexture AllocFromPixelSize(in RenderTextureDescriptor rtDesc)
	{
		Vector2Int allocTileSize = GetAllocTileSize(rtDesc.width, rtDesc.height);
		RenderTextureDescriptor desc = rtDesc;
		desc.width = allocTileSize.x;
		desc.height = allocTileSize.y;
		return new RenderTexture(desc);
	}

	public static RenderTextureDescriptor GetRenderTextureDescriptor(int width, int height, int volumeDepth = 1, TextureDimension textureDimension = TextureDimension.Tex2D)
	{
		RenderTextureDescriptor result;
		if (ShadingRateInfo.supportsPerImageTile)
		{
			result = new RenderTextureDescriptor(width, height);
			result.msaaSamples = 1;
			result.autoGenerateMips = false;
			result.volumeDepth = volumeDepth;
			result.dimension = textureDimension;
			result.graphicsFormat = ShadingRateInfo.graphicsFormat;
			result.enableRandomWrite = true;
			result.enableShadingRate = true;
			return result;
		}
		result = new RenderTextureDescriptor(0, 0);
		result.msaaSamples = 0;
		result.autoGenerateMips = false;
		result.volumeDepth = 0;
		result.dimension = TextureDimension.None;
		result.graphicsFormat = GraphicsFormat.None;
		return result;
	}
}
