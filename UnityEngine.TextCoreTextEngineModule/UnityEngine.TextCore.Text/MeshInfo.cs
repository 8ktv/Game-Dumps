using System;
using UnityEngine.Bindings;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
internal struct MeshInfo
{
	public int vertexCount;

	public TextCoreVertex[] vertexData;

	public Material material;

	[Ignore]
	public int vertexBufferSize;

	[Ignore]
	public bool applySDF;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal GlyphRenderMode glyphRenderMode;

	public MeshInfo(int size, bool isIMGUI)
	{
		this = default(MeshInfo);
		applySDF = true;
		material = null;
		if (isIMGUI)
		{
			size = Mathf.Min(size, 16383);
		}
		int num = size * 4;
		int num2 = size * 6;
		vertexCount = 0;
		vertexBufferSize = num;
		vertexData = new TextCoreVertex[num];
		material = null;
		glyphRenderMode = GlyphRenderMode.DEFAULT;
	}

	internal void ResizeMeshInfo(int size, bool isIMGUI)
	{
		if (isIMGUI)
		{
			size = Mathf.Min(size, 16383);
		}
		int newSize = size * 4;
		int num = size * 6;
		vertexBufferSize = newSize;
		Array.Resize(ref vertexData, newSize);
	}

	internal void Clear(bool uploadChanges)
	{
		if (vertexData != null)
		{
			Array.Clear(vertexData, 0, vertexData.Length);
			vertexBufferSize = vertexData.Length;
			vertexCount = 0;
		}
	}

	internal void ClearUnusedVertices()
	{
		int num = vertexData.Length - vertexCount;
		if (num > 0)
		{
			Array.Clear(vertexData, vertexCount, num);
		}
		vertexBufferSize = vertexData.Length;
	}
}
