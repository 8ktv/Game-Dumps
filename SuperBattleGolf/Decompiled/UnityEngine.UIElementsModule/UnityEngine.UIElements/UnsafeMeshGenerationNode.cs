using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

internal struct UnsafeMeshGenerationNode
{
	private GCHandle m_Handle;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private MeshGenerationNodeImpl GetManaged()
	{
		return (MeshGenerationNodeImpl)m_Handle.Target;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Create(GCHandle handle, out UnsafeMeshGenerationNode node)
	{
		node = new UnsafeMeshGenerationNode
		{
			m_Handle = handle
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DrawMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null)
	{
		GetManaged().DrawMesh(vertices, indices, texture);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void DrawMeshInternal(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null, TextureOptions textureOptions = TextureOptions.None)
	{
		GetManaged().DrawMesh(vertices, indices, texture, textureOptions);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void DrawGradientsInternal(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, VectorImage gradientsOwner)
	{
		GetManaged().DrawGradients(vertices, indices, gradientsOwner);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Entry GetParentEntry()
	{
		return GetManaged().GetParentEntry();
	}
}
