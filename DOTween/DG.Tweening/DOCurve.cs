using System.Collections.Generic;
using UnityEngine;

namespace DG.Tweening;

public static class DOCurve
{
	public static class CubicBezier
	{
		public static Vector3 GetPointOnSegment(Vector3 startPoint, Vector3 startControlPoint, Vector3 endPoint, Vector3 endControlPoint, float factor)
		{
			float num = 1f - factor;
			float num2 = factor * factor;
			float num3 = num * num;
			float num4 = num3 * num;
			float num5 = num2 * factor;
			return num4 * startPoint + 3f * num3 * factor * startControlPoint + 3f * num * num2 * endControlPoint + num5 * endPoint;
		}

		public static Vector3[] GetSegmentPointCloud(Vector3 startPoint, Vector3 startControlPoint, Vector3 endPoint, Vector3 endControlPoint, int resolution = 10)
		{
			if (resolution < 2)
			{
				resolution = 2;
			}
			Vector3[] array = new Vector3[resolution];
			float num = 1f / (float)(resolution - 1);
			for (int i = 0; i < resolution; i++)
			{
				array[i] = GetPointOnSegment(startPoint, startControlPoint, endPoint, endControlPoint, num * (float)i);
			}
			return array;
		}

		public static void GetSegmentPointCloud(List<Vector3> addToList, Vector3 startPoint, Vector3 startControlPoint, Vector3 endPoint, Vector3 endControlPoint, int resolution = 10)
		{
			if (resolution < 2)
			{
				resolution = 2;
			}
			float num = 1f / (float)(resolution - 1);
			for (int i = 0; i < resolution; i++)
			{
				addToList.Add(GetPointOnSegment(startPoint, startControlPoint, endPoint, endControlPoint, num * (float)i));
			}
		}
	}
}
