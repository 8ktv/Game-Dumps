using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public class MeshWriteData
{
	internal NativeSlice<Vertex> m_Vertices;

	internal NativeSlice<ushort> m_Indices;

	internal int currentIndex;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int currentVertex;

	public int vertexCount => m_Vertices.Length;

	public int indexCount => m_Indices.Length;

	[Obsolete("Texture coordinates are now automatically remapped by the renderer. You are no longer required to remap the UV coordinates in the provided rectangle.")]
	public Rect uvRegion => new Rect(0f, 0f, 1f, 1f);

	internal MeshWriteData()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetNextVertex(Vertex vertex)
	{
		m_Vertices[currentVertex++] = vertex;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetNextIndex(ushort index)
	{
		m_Indices[currentIndex++] = index;
	}

	public void SetAllVertices(Vertex[] vertices)
	{
		if (currentVertex == 0)
		{
			m_Vertices.CopyFrom(vertices);
			currentVertex = m_Vertices.Length;
			return;
		}
		throw new InvalidOperationException("SetAllVertices may not be called after using SetNextVertex");
	}

	public void SetAllVertices(NativeSlice<Vertex> vertices)
	{
		if (currentVertex == 0)
		{
			m_Vertices.CopyFrom(vertices);
			currentVertex = m_Vertices.Length;
			return;
		}
		throw new InvalidOperationException("SetAllVertices may not be called after using SetNextVertex");
	}

	public void SetAllIndices(ushort[] indices)
	{
		if (currentIndex == 0)
		{
			m_Indices.CopyFrom(indices);
			currentIndex = m_Indices.Length;
			return;
		}
		throw new InvalidOperationException("SetAllIndices may not be called after using SetNextIndex");
	}

	public void SetAllIndices(NativeSlice<ushort> indices)
	{
		if (currentIndex == 0)
		{
			m_Indices.CopyFrom(indices);
			currentIndex = m_Indices.Length;
			return;
		}
		throw new InvalidOperationException("SetAllIndices may not be called after using SetNextIndex");
	}

	internal void Reset(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices)
	{
		m_Vertices = vertices;
		m_Indices = indices;
		currentIndex = (currentVertex = 0);
	}
}
