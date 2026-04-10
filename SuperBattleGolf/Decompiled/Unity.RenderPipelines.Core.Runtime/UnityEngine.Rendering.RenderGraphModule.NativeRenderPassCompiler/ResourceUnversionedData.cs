using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

internal struct ResourceUnversionedData
{
	public readonly bool isImported;

	public bool isShared;

	public int tag;

	public int lastUsePassID;

	public int lastWritePassID;

	public int firstUsePassID;

	public bool memoryLess;

	public readonly int width;

	public readonly int height;

	public readonly int volumeDepth;

	public readonly int msaaSamples;

	public readonly GraphicsFormat graphicsFormat;

	public int latestVersionNumber;

	public readonly bool clear;

	public readonly bool discard;

	public readonly bool bindMS;

	public TextureUVOriginSelection textureUVOrigin;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetName(CompilerContextData ctx, in ResourceHandle h)
	{
		return ctx.GetResourceName(in h);
	}

	public ResourceUnversionedData(TextureResource rll, ref RenderTargetInfo info, ref TextureDesc desc, bool isResourceShared)
	{
		isImported = rll.imported;
		isShared = isResourceShared;
		tag = 0;
		firstUsePassID = -1;
		lastUsePassID = -1;
		lastWritePassID = -1;
		memoryLess = false;
		width = info.width;
		height = info.height;
		volumeDepth = info.volumeDepth;
		msaaSamples = info.msaaSamples;
		latestVersionNumber = (int)rll.writeCount;
		clear = desc.clearBuffer;
		discard = desc.discardBuffer;
		bindMS = info.bindMS;
		textureUVOrigin = rll.textureUVOrigin;
		graphicsFormat = desc.format;
	}

	public ResourceUnversionedData(IRenderGraphResource rll, ref BufferDesc _, bool isResourceShared)
	{
		isImported = rll.imported;
		isShared = isResourceShared;
		tag = 0;
		firstUsePassID = -1;
		lastUsePassID = -1;
		lastWritePassID = -1;
		memoryLess = false;
		width = -1;
		height = -1;
		volumeDepth = -1;
		msaaSamples = -1;
		latestVersionNumber = (int)rll.writeCount;
		clear = false;
		discard = false;
		bindMS = false;
		textureUVOrigin = TextureUVOriginSelection.Unknown;
		graphicsFormat = GraphicsFormat.None;
	}

	public ResourceUnversionedData(IRenderGraphResource rll, ref RayTracingAccelerationStructureDesc _, bool isResourceShared)
	{
		isImported = rll.imported;
		isShared = isResourceShared;
		tag = 0;
		firstUsePassID = -1;
		lastUsePassID = -1;
		lastWritePassID = -1;
		memoryLess = false;
		width = -1;
		height = -1;
		volumeDepth = -1;
		msaaSamples = -1;
		latestVersionNumber = (int)rll.writeCount;
		clear = false;
		discard = false;
		bindMS = false;
		textureUVOrigin = TextureUVOriginSelection.Unknown;
		graphicsFormat = GraphicsFormat.None;
	}

	public void InitializeNullResource()
	{
		firstUsePassID = -1;
		lastUsePassID = -1;
		lastWritePassID = -1;
		textureUVOrigin = TextureUVOriginSelection.Unknown;
	}
}
