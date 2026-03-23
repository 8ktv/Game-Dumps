using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

[NativeContainer]
public struct MeshGenerationNode
{
	private UnsafeMeshGenerationNode m_UnsafeNode;

	internal UnsafeMeshGenerationNode unsafeNode => m_UnsafeNode;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Create(GCHandle handle, out MeshGenerationNode node)
	{
		node = default(MeshGenerationNode);
		UnsafeMeshGenerationNode.Create(handle, out node.m_UnsafeNode);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DrawMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null)
	{
		m_UnsafeNode.DrawMesh(vertices, indices, texture);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal Entry GetParentEntry()
	{
		return m_UnsafeNode.GetParentEntry();
	}
}
