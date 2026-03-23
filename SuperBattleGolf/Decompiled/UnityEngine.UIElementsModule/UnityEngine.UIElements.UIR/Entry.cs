using System;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR;

internal class Entry
{
	public EntryType type;

	public EntryFlags flags;

	public NativeSlice<Vertex> vertices;

	public NativeSlice<ushort> indices;

	public Texture texture;

	public float textScale;

	public float fontSharpness;

	public VectorImage gradientsOwner;

	public Material material;

	public Action immediateCallback;

	public TextureId textureId;

	public Entry nextSibling;

	public Entry firstChild;

	public Entry lastChild;

	public void Reset()
	{
		nextSibling = null;
		firstChild = null;
		lastChild = null;
		texture = null;
		material = null;
		gradientsOwner = null;
		flags = (EntryFlags)0;
		immediateCallback = null;
	}
}
