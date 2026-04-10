#define UNITY_ASSERTIONS
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.UIElements.UIR;

internal static class RenderTreeAtlas
{
	public struct AtlasBlock
	{
		public int width;

		public int height;

		public RectInt rect;

		public Rect uvRect;

		public RenderTexture texture;

		public AtlasBlock(int w, int h, RectInt r, Rect uv)
		{
			width = w;
			height = h;
			rect = r;
			uvRect = uv;
			texture = null;
		}
	}

	private const int k_LeftMargin = 2;

	private const int k_TopMargin = 2;

	private const int k_RightMargin = 2;

	private const int k_BottomMargin = 2;

	public static bool ReserveSize(int width, int height, out AtlasBlock block)
	{
		int num = width + 2 + 2;
		int num2 = height + 2 + 2;
		RectInt r = new RectInt(2, 2, width, height);
		Rect uv = new Rect(2f / (float)num, 2f / (float)num2, (float)width / (float)num, (float)height / (float)num2);
		r.y = num2 - (r.y + r.height);
		uv.y = 1f - uv.yMax;
		block = new AtlasBlock(width, height, r, uv);
		return true;
	}

	public static bool CreateTextureForAtlasBlock(ref AtlasBlock block, bool forceGammaRendering, out bool allocatedNewTexture)
	{
		Debug.Assert(block.texture == null, "Entry already has a texture assigned.");
		Debug.Assert(block.width > 0 && block.height > 0, "Invalid texture size requested.");
		int width = block.width + 2 + 2;
		int height = block.height + 2 + 2;
		ColorSpace colorSpace = QualitySettings.activeColorSpace;
		if (forceGammaRendering)
		{
			colorSpace = ColorSpace.Gamma;
		}
		GraphicsFormat colorFormat = ((colorSpace == ColorSpace.Linear) ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm);
		RenderTextureDescriptor desc = new RenderTextureDescriptor(width, height, colorFormat, GraphicsFormat.D24_UNorm_S8_UInt);
		desc.useMipMap = false;
		block.texture = RenderTexture.GetTemporary(desc);
		allocatedNewTexture = true;
		if (block.texture == null)
		{
			Debug.LogError($"Failed to allocate RenderTexture of size {block.width}x{block.height}.");
			return false;
		}
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = block.texture;
		GL.Clear(clearDepth: true, clearColor: true, Color.clear, 1f);
		RenderTexture.active = active;
		return true;
	}
}
