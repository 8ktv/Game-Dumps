using System;
using System.Collections.Generic;
using UnityEngine;

namespace Brimstone.Geometry;

public class Polyline
{
	private readonly List<Vector3> points;

	private readonly List<float> lengths;

	public int PointCount { get; private set; }

	public int LineCount { get; private set; }

	public float TotalLength { get; private set; }

	public Polyline(int pointCapacity)
	{
		points = new List<Vector3>(pointCapacity);
		PointCount = points.Count;
		LineCount = PointCount - 1;
		TotalLength = 0f;
		lengths = new List<float>(LineCount);
	}

	public Polyline(IEnumerable<Vector3> points)
	{
		this.points = new List<Vector3>(points);
		this.points.AddRange(points);
		PointCount = this.points.Count;
		LineCount = PointCount - 1;
		TotalLength = 0f;
		lengths = new List<float>(LineCount);
		for (int i = 1; i < PointCount; i++)
		{
			float magnitude = (this.points[i] - this.points[i - 1]).magnitude;
			lengths[i] = magnitude;
			TotalLength += magnitude;
		}
	}

	public Vector3 GetPoint(int i)
	{
		if (i < 0 || i >= PointCount)
		{
			throw new ArgumentOutOfRangeException("i");
		}
		return points[i];
	}

	public Line GetLine(int i)
	{
		if (i < 0 || i >= LineCount)
		{
			throw new ArgumentOutOfRangeException("i");
		}
		return new Line(points[i], points[i + 1], lengths[i]);
	}

	public void AddPoint(Vector3 point)
	{
		float magnitude = (point - points[PointCount]).magnitude;
		points.Add(point);
		PointCount++;
		LineCount++;
		TotalLength += magnitude;
		lengths.Add(magnitude);
	}

	public void RemovePoint(int i)
	{
		if (i < 0 || i >= PointCount)
		{
			throw new ArgumentOutOfRangeException("i");
		}
		if (i == 0)
		{
			TotalLength -= lengths[0];
		}
		else if (i == PointCount - 1)
		{
			TotalLength -= lengths[LineCount - 1];
		}
		else
		{
			float num = lengths[i - 1] + lengths[i];
			float magnitude = (points[i + 1] - points[i - 1]).magnitude;
			float num2 = magnitude - num;
			TotalLength += num2;
			lengths[i - 1] = magnitude;
		}
		points.RemoveAt(i);
		lengths.RemoveAt(i);
		PointCount--;
		LineCount--;
	}

	public Vector3 EvaluateAt(float t)
	{
		if (t < 0f || t > 1f)
		{
			throw new ArgumentOutOfRangeException("t");
		}
		int num = FindLineIndexAtDistance(t * TotalLength);
		Vector3 a = points[num];
		Vector3 b = points[num + 1];
		return Vector3.Lerp(a, b, t);
	}

	public Vector3 EvaluateAtDistance(float distance)
	{
		return EvaluateAt(distance / TotalLength);
	}

	public Vector3 ClosestPoint(Vector3 point)
	{
		float num = float.PositiveInfinity;
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < LineCount; i++)
		{
			Vector3 vector2 = point.ClosestPointOnSegment(points[i], points[i + 1]);
			float sqrMagnitude = (vector - point).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				vector = vector2;
				num = sqrMagnitude;
			}
		}
		return vector;
	}

	private int FindLineIndexAtDistance(float distance)
	{
		if (distance < 0f || distance > TotalLength)
		{
			throw new ArgumentOutOfRangeException("distance");
		}
		if (distance == 0f)
		{
			return 0;
		}
		if (distance == TotalLength)
		{
			return LineCount - 1;
		}
		float num = 0f;
		if (distance > TotalLength * 0.5f)
		{
			for (int num2 = PointCount - 1; num2 > 0; num2--)
			{
				if (TotalLength - num <= distance)
				{
					return num2;
				}
				num += lengths[num2 - 1];
			}
			return 0;
		}
		for (int i = 1; i < PointCount; i++)
		{
			if (num > distance)
			{
				return i - 1;
			}
			num += lengths[i - 1];
		}
		return LineCount - 1;
	}
}
