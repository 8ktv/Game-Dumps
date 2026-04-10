using System.Collections.Generic;
using ClipperLib;
using LibTessDotNet;
using UnityEngine;

namespace Unity.VectorGraphics;

internal static class VectorClip
{
	private const int k_ClipperScale = 100000;

	private static Stack<List<List<IntPoint>>> m_ClipStack = new Stack<List<List<IntPoint>>>();

	internal static void ResetClip()
	{
		m_ClipStack.Clear();
	}

	internal static void PushClip(List<Vector2[]> clipper, Matrix2D transform)
	{
		List<List<IntPoint>> list = new List<List<IntPoint>>(10);
		foreach (Vector2[] item in clipper)
		{
			List<IntPoint> list2 = new List<IntPoint>(item.Length);
			Vector2[] array = item;
			foreach (Vector2 vector in array)
			{
				Vector2 vector2 = transform * vector;
				list2.Add(new IntPoint(vector2.x * 100000f, vector2.y * 100000f));
			}
			list.Add(list2);
		}
		m_ClipStack.Push(list);
	}

	internal static void PopClip()
	{
		m_ClipStack.Pop();
	}

	internal static void ClipGeometry(VectorUtils.Geometry geom)
	{
		Clipper clipper = new Clipper();
		foreach (List<List<IntPoint>> item in m_ClipStack)
		{
			List<Vector2> list = new List<Vector2>(geom.Vertices.Length);
			List<ushort> list2 = new List<ushort>(geom.Indices.Length);
			List<List<IntPoint>> list3 = BuildTriangleClipPaths(geom);
			List<List<IntPoint>> list4 = new List<List<IntPoint>>();
			ushort maxIndex = 0;
			foreach (List<IntPoint> item2 in list3)
			{
				clipper.AddPaths(item, PolyType.ptClip, closed: true);
				clipper.AddPath(item2, PolyType.ptSubject, Closed: true);
				clipper.Execute(ClipType.ctIntersection, list4, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
				if (list4.Count > 0)
				{
					BuildGeometryFromClipPaths(geom, list4, list, list2, ref maxIndex);
				}
				clipper.Clear();
				list4.Clear();
			}
			geom.Vertices = list.ToArray();
			geom.Indices = list2.ToArray();
		}
	}

	private static List<List<IntPoint>> BuildTriangleClipPaths(VectorUtils.Geometry geom)
	{
		List<List<IntPoint>> list = new List<List<IntPoint>>(geom.Indices.Length / 3);
		Vector2[] vertices = geom.Vertices;
		ushort[] indices = geom.Indices;
		int num = geom.Indices.Length;
		Matrix2D worldTransform = geom.WorldTransform;
		for (int i = 0; i < num; i += 3)
		{
			Vector2 vector = worldTransform * vertices[indices[i]];
			Vector2 vector2 = worldTransform * vertices[indices[i + 1]];
			Vector2 vector3 = worldTransform * vertices[indices[i + 2]];
			List<IntPoint> list2 = new List<IntPoint>(3);
			list2.Add(new IntPoint(vector.x * 100000f, vector.y * 100000f));
			list2.Add(new IntPoint(vector2.x * 100000f, vector2.y * 100000f));
			list2.Add(new IntPoint(vector3.x * 100000f, vector3.y * 100000f));
			list.Add(list2);
		}
		return list;
	}

	private static void BuildGeometryFromClipPaths(VectorUtils.Geometry geom, List<List<IntPoint>> paths, List<Vector2> outVerts, List<ushort> outInds, ref ushort maxIndex)
	{
		List<Vector2> list = new List<Vector2>(100);
		List<ushort> list2 = new List<ushort>(list.Capacity * 3);
		Dictionary<IntPoint, ushort> vertexIndex = new Dictionary<IntPoint, ushort>();
		foreach (List<IntPoint> path in paths)
		{
			if (path.Count == 3)
			{
				foreach (IntPoint item in path)
				{
					StoreClipVertex(vertexIndex, list, list2, item, ref maxIndex);
				}
			}
			else if (path.Count > 3)
			{
				Tess tess = new Tess();
				ContourVertex[] array = new ContourVertex[path.Count];
				for (int i = 0; i < path.Count; i++)
				{
					array[i] = new ContourVertex
					{
						Position = new Vec3
						{
							X = path[i].X,
							Y = path[i].Y,
							Z = 0f
						}
					};
				}
				tess.AddContour(array, ContourOrientation.Original);
				WindingRule windingRule = WindingRule.NonZero;
				tess.Tessellate(windingRule, ElementType.Polygons, 3);
				int[] elements = tess.Elements;
				foreach (int num in elements)
				{
					ContourVertex contourVertex = tess.Vertices[num];
					IntPoint pt = new IntPoint(contourVertex.Position.X, contourVertex.Position.Y);
					StoreClipVertex(vertexIndex, list, list2, pt, ref maxIndex);
				}
			}
		}
		Matrix2D matrix2D = geom.WorldTransform.Inverse();
		for (int k = 0; k < list.Count; k++)
		{
			outVerts.Add(matrix2D * list[k]);
		}
		outInds.AddRange(list2);
	}

	private static void StoreClipVertex(Dictionary<IntPoint, ushort> vertexIndex, List<Vector2> vertices, List<ushort> indices, IntPoint pt, ref ushort index)
	{
		if (vertexIndex.TryGetValue(pt, out var value))
		{
			indices.Add(value);
			return;
		}
		vertices.Add(new Vector2((float)pt.X / 100000f, (float)pt.Y / 100000f));
		indices.Add(index);
		vertexIndex[pt] = index;
		index++;
	}
}
