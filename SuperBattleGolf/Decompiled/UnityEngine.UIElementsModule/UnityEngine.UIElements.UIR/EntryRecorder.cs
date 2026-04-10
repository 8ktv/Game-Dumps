#define UNITY_ASSERTIONS
using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.UIElements.UIR;

internal class EntryRecorder
{
	private EntryPool m_EntryPool;

	public EntryRecorder(EntryPool entryPool)
	{
		Debug.Assert(entryPool != null);
		m_EntryPool = entryPool;
	}

	public void DrawMesh(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices)
	{
		DrawMesh(parentEntry, vertices, indices, null);
	}

	public void DrawMesh(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture, TextureOptions textureOptions = TextureOptions.None)
	{
		Entry entry = m_EntryPool.Get();
		entry.vertices = vertices;
		entry.indices = indices;
		entry.texture = texture;
		entry.flags = (((textureOptions & TextureOptions.PremultipliedAlpha) != TextureOptions.None) ? EntryFlags.IsPremultiplied : ((EntryFlags)0));
		if ((object)texture == null)
		{
			entry.type = EntryType.DrawSolidMesh;
		}
		else
		{
			bool flag = (entry.flags & EntryFlags.IsPremultiplied) != 0 || (textureOptions & TextureOptions.SkipDynamicAtlas) != 0;
			entry.type = ((!flag) ? EntryType.DrawTexturedMesh : EntryType.DrawTexturedMeshSkipAtlas);
		}
		AppendMeshEntry(parentEntry, entry);
	}

	public void DrawMesh(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, TextureId textureId, bool isPremultiplied = false)
	{
		Debug.Assert(textureId.IsValid());
		Entry entry = m_EntryPool.Get();
		entry.vertices = vertices;
		entry.indices = indices;
		entry.textureId = textureId;
		entry.flags = (isPremultiplied ? EntryFlags.IsPremultiplied : ((EntryFlags)0));
		entry.type = EntryType.DrawDynamicTexturedMesh;
		AppendMeshEntry(parentEntry, entry);
	}

	public void DrawRasterText(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture, bool multiChannel)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = (multiChannel ? EntryType.DrawTexturedMeshSkipAtlas : EntryType.DrawTextMesh);
		entry.flags = EntryFlags.UsesTextCoreSettings;
		entry.vertices = vertices;
		entry.indices = indices;
		entry.texture = texture;
		entry.textScale = 0f;
		entry.fontSharpness = 0f;
		AppendMeshEntry(parentEntry, entry);
	}

	public void DrawSdfText(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture, float scale, float sharpness)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.DrawTextMesh;
		entry.flags = EntryFlags.UsesTextCoreSettings;
		entry.vertices = vertices;
		entry.indices = indices;
		entry.texture = texture;
		entry.textScale = scale;
		entry.fontSharpness = sharpness;
		AppendMeshEntry(parentEntry, entry);
	}

	public void DrawGradients(Entry parentEntry, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, VectorImage gradientsOwner)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.DrawGradients;
		entry.vertices = vertices;
		entry.indices = indices;
		entry.gradientsOwner = gradientsOwner;
		AppendMeshEntry(parentEntry, entry);
	}

	public void DrawImmediate(Entry parentEntry, Action callback, bool cullingEnabled)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = (cullingEnabled ? EntryType.DrawImmediateCull : EntryType.DrawImmediate);
		entry.immediateCallback = callback;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DrawChildren(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.DrawChildren;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void BeginStencilMask(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.BeginStencilMask;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void EndStencilMask(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.EndStencilMask;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PopStencilMask(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PopStencilMask;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PushClippingRect(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PushClippingRect;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PopClippingRect(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PopClippingRect;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PushScissors(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PushScissors;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PopScissors(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PopScissors;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PushGroupMatrix(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PushGroupMatrix;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PopGroupMatrix(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PopGroupMatrix;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PushDefaultMaterial(Entry parentEntry, MaterialDefinition matDef)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PushDefaultMaterial;
		entry.material = matDef.material;
		entry.userProps = matDef.BuildPropertyBlock();
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void PopDefaultMaterial(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.PopDefaultMaterial;
		Append(parentEntry, entry);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void CutRenderChain(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.CutRenderChain;
		Append(parentEntry, entry);
	}

	public Entry InsertPlaceholder(Entry parentEntry)
	{
		Entry entry = m_EntryPool.Get();
		entry.type = EntryType.DedicatedPlaceholder;
		Append(parentEntry, entry);
		return entry;
	}

	private static void AppendMeshEntry(Entry parentEntry, Entry entry)
	{
		int length = entry.vertices.Length;
		int length2 = entry.indices.Length;
		if (length == 0)
		{
			Debug.LogError("Attempting to add an entry without vertices.");
		}
		else if (length > UIRenderDevice.maxVerticesPerPage)
		{
			Debug.LogError($"Attempting to add an entry with {length} vertices. The maximum number of vertices per entry is {UIRenderDevice.maxVerticesPerPage}.");
		}
		else if (length2 == 0)
		{
			Debug.LogError("Attempting to add an entry without indices.");
		}
		else
		{
			Append(parentEntry, entry);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Append(Entry parentEntry, Entry entry)
	{
		if (parentEntry.lastChild == null)
		{
			Debug.Assert(parentEntry.firstChild == null);
			parentEntry.firstChild = entry;
			parentEntry.lastChild = entry;
		}
		else
		{
			parentEntry.lastChild.nextSibling = entry;
			parentEntry.lastChild = entry;
		}
	}
}
