#define UNITY_ASSERTIONS
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal class MeshGenerationNodeImpl : IDisposable
{
	private GCHandle m_SelfHandle;

	private Entry m_ParentEntry;

	private EntryRecorder m_EntryRecorder;

	protected bool disposed { get; private set; }

	public MeshGenerationNodeImpl()
	{
		m_SelfHandle = GCHandle.Alloc(this);
	}

	public void Init(Entry parentEntry, EntryRecorder entryRecorder, bool safe)
	{
		Debug.Assert(m_ParentEntry == null);
		Debug.Assert(parentEntry != null);
		Debug.Assert(entryRecorder != null);
		m_ParentEntry = parentEntry;
		m_EntryRecorder = entryRecorder;
	}

	public void Reset()
	{
		Debug.Assert(m_ParentEntry != null);
		Debug.Assert(m_EntryRecorder != null);
		m_ParentEntry = null;
		m_EntryRecorder = null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GetNode(out MeshGenerationNode node)
	{
		MeshGenerationNode.Create(m_SelfHandle, out node);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GetUnsafeNode(out UnsafeMeshGenerationNode node)
	{
		UnsafeMeshGenerationNode.Create(m_SelfHandle, out node);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Entry GetParentEntry()
	{
		return m_ParentEntry;
	}

	public void DrawMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null, TextureOptions textureOptions = TextureOptions.None)
	{
		if (vertices.Length != 0 && indices.Length != 0)
		{
			m_EntryRecorder.DrawMesh(m_ParentEntry, vertices, indices, texture, textureOptions);
		}
	}

	public void DrawGradients(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, VectorImage gradientsOwner)
	{
		if (vertices.Length != 0 && indices.Length != 0 && !(gradientsOwner == null))
		{
			m_EntryRecorder.DrawGradients(m_ParentEntry, vertices, indices, gradientsOwner);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			if (m_ParentEntry != null)
			{
				Reset();
			}
			m_SelfHandle.Free();
		}
		disposed = true;
	}
}
