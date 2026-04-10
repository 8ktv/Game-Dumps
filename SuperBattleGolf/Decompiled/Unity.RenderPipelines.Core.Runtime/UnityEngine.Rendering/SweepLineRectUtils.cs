using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;

namespace UnityEngine.Rendering;

public static class SweepLineRectUtils
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct EventComparer : IComparer<Vector4>
	{
		public int Compare(Vector4 a, Vector4 b)
		{
			int num = a.x.CompareTo(b.x);
			if (num != 0)
			{
				return num;
			}
			return b.y.CompareTo(a.y);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct ActiveComparer : IComparer<Vector2>
	{
		public int Compare(Vector2 a, Vector2 b)
		{
			return a.x.CompareTo(b.x);
		}
	}

	public static float CalculateRectUnionArea(List<Rect> rects)
	{
		int count = rects.Count;
		Vector4[] array = ArrayPool<Vector4>.Shared.Rent(count * 2);
		Vector2[] array2 = ArrayPool<Vector2>.Shared.Rent(count);
		int eventCount = 0;
		foreach (Rect rect in rects)
		{
			InsertEvents(rect, array, ref eventCount);
		}
		float result = CalculateRectUnionArea(array, array2, eventCount);
		ArrayPool<Vector4>.Shared.Return(array);
		ArrayPool<Vector2>.Shared.Return(array2);
		return result;
	}

	private static float MergeLengthY(Vector2[] activeBuffer, int count)
	{
		if (count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		float num2 = activeBuffer[0].x;
		float num3 = activeBuffer[0].y;
		for (int i = 1; i < count; i++)
		{
			float x = activeBuffer[i].x;
			float y = activeBuffer[i].y;
			if (x <= num3)
			{
				if (y > num3)
				{
					num3 = y;
				}
			}
			else
			{
				num += num3 - num2;
				num2 = x;
				num3 = y;
			}
		}
		num += num3 - num2;
		return Mathf.Clamp01(num);
	}

	private unsafe static float CalculateRectUnionArea(Vector4[] eventsBuffer, Vector2[] activeBuffer, int eventCount)
	{
		if (eventCount == 0)
		{
			return 0f;
		}
		fixed (Vector4* array = eventsBuffer)
		{
			NativeSortExtension.Sort(array, eventCount, default(EventComparer));
		}
		int num = 0;
		float num2 = 0f;
		float num3 = eventsBuffer[0].x;
		bool flag = false;
		int num4 = 0;
		while (num4 < eventCount)
		{
			float x = eventsBuffer[num4].x;
			if (flag)
			{
				num3 = x;
				flag = false;
			}
			float num5 = x - num3;
			if (num5 > 0f && num > 0)
			{
				fixed (Vector2* array2 = activeBuffer)
				{
					NativeSortExtension.Sort(array2, num, default(ActiveComparer));
				}
				num2 += MergeLengthY(activeBuffer, num) * num5;
				num3 = x;
			}
			do
			{
				Vector4 vector = eventsBuffer[num4];
				float z = vector.z;
				float w = vector.w;
				if (vector.y > 0f)
				{
					activeBuffer[num++] = new Vector2(z, w);
				}
				else
				{
					for (int i = 0; i < num; i++)
					{
						Vector2 vector2 = activeBuffer[i];
						if (Mathf.Approximately(vector2.x, z) && Mathf.Approximately(vector2.y, w))
						{
							int num6 = num - 1;
							activeBuffer[i] = activeBuffer[num6];
							num = num6;
							break;
						}
					}
					if (num == 0)
					{
						flag = true;
					}
				}
				num4++;
			}
			while (num4 < eventCount && Mathf.Approximately(eventsBuffer[num4].x, x));
		}
		return num2;
	}

	private static void InsertEvents(in Rect rect, Vector4[] eventsBuffer, ref int eventCount)
	{
		if (rect.width > 0f && rect.height > 0f)
		{
			float z = Mathf.Clamp01(rect.yMin);
			float w = Mathf.Clamp01(rect.yMax);
			eventsBuffer[eventCount++] = new Vector4(Mathf.Clamp01(rect.xMin), 1f, z, w);
			eventsBuffer[eventCount++] = new Vector4(Mathf.Clamp01(rect.xMax), -1f, z, w);
		}
	}
}
