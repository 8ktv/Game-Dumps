using System;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[Serializable]
[NativeHeader("Runtime/Export/Graphics/GraphicsTexture.bindings.h")]
[UsedByNativeCode]
public struct GraphicsTextureDescriptor
{
	public int width;

	public int height;

	public int depth;

	public int arrayLength;

	public GraphicsFormat format;

	public TextureDimension dimension;

	public int mipCount;

	public int numSamples;

	public GraphicsTextureDescriptorFlags flags;

	[ExcludeFromDocs]
	public GraphicsTextureDescriptor()
		: this(0, 0, 1, 0, GraphicsFormat.None, TextureDimension.None)
	{
	}

	internal GraphicsTextureDescriptor(int width, int height, int depth, int arrayLength, GraphicsFormat format, TextureDimension dimension, int mipCount = 0, int numSamples = 1, GraphicsTextureDescriptorFlags flags = GraphicsTextureDescriptorFlags.None)
	{
		this.width = width;
		this.height = height;
		this.depth = depth;
		this.arrayLength = arrayLength;
		this.format = format;
		this.dimension = dimension;
		this.mipCount = mipCount;
		this.numSamples = numSamples;
		this.flags = flags;
	}
}
