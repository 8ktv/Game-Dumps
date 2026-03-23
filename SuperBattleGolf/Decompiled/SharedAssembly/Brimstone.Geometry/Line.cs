#define DEBUG_DRAW
using System;
using UnityEngine;

namespace Brimstone.Geometry;

public struct Line : IEquatable<Line>
{
	public struct OverlapInfo
	{
		public enum Info : byte
		{
			noOverlap = 0,
			thisLineFullyContained = 1,
			otherLineFullyContained = 2,
			thisLineAhead = 4,
			otherLineAhead = 8,
			sharingAtLeastOnePoint = 8,
			completeOverlap = 11
		}

		private readonly Info info;

		public bool Overlapping => info != Info.noOverlap;

		public bool ThisLineFullyContained => (info & Info.thisLineFullyContained) == Info.thisLineFullyContained;

		public bool OtherLineFullyContained => (info & Info.otherLineFullyContained) == Info.otherLineFullyContained;

		public bool ThisLineAhead => (info & Info.thisLineAhead) == Info.thisLineAhead;

		public bool OtherLineAhead => (info & Info.otherLineAhead) == Info.otherLineAhead;

		public bool SharingAtLeastOnePoint => (info & Info.otherLineAhead) == Info.otherLineAhead;

		public bool CompleteOverlap => (info & Info.completeOverlap) == Info.completeOverlap;

		public OverlapInfo(Info info)
		{
			this.info = info;
		}

		public static implicit operator Info(OverlapInfo overlapInfo)
		{
			return overlapInfo.info;
		}

		public static implicit operator OverlapInfo(Info info)
		{
			return new OverlapInfo(info);
		}
	}

	private Vector3 inverseDirection;

	public Vector3 Start { get; private set; }

	public Vector3 End { get; private set; }

	public Vector3 Direction { get; private set; }

	public float Length { get; private set; }

	public float LengthSquared { get; private set; }

	public bool IsEmpty { get; private set; }

	public Line Flipped => new Line(End, Start, -Direction, Length);

	public static Line Empty { get; private set; } = new Line(Vector3.zero, Vector3.zero, Vector3.zero, 0f);

	public Line(Vector3 start, Vector3 end)
	{
		Start = start;
		End = end;
		Vector3 vector = End - Start;
		Length = vector.magnitude;
		LengthSquared = Length * Length;
		Direction = vector / Length;
		IsEmpty = Length <= float.Epsilon;
		inverseDirection = new Vector3(1f / Direction.x, 1f / Direction.y, 1f / Direction.z);
	}

	public Line(Vector3 start, Vector3 end, Vector3 direction, float length)
	{
		Start = start;
		End = end;
		Direction = direction;
		Length = length;
		LengthSquared = length * length;
		IsEmpty = length <= float.Epsilon;
		inverseDirection = new Vector3(1f / Direction.x, 1f / Direction.y, 1f / Direction.z);
	}

	public Line(Vector3 start, Vector3 end, float length)
	{
		Start = start;
		End = end;
		Direction = (end - start) / length;
		Length = length;
		LengthSquared = length * length;
		IsEmpty = length <= float.Epsilon;
		inverseDirection = new Vector3(1f / Direction.x, 1f / Direction.y, 1f / Direction.z);
	}

	public void SetStart(Vector3 newStart)
	{
		Start = newStart;
		RecalculateProperties();
	}

	public void SetEnd(Vector3 newEnd)
	{
		End = newEnd;
		RecalculateProperties();
	}

	public void Set(Vector3 newStart, Vector3 newEnd)
	{
		Start = newStart;
		End = newEnd;
		RecalculateProperties();
	}

	public Line Flip()
	{
		this = Flipped;
		return this;
	}

	public bool Approximately(Line otherLine, float tolerance = 0.001f)
	{
		if (Start.Approximately(otherLine.Start, tolerance))
		{
			return End.Approximately(otherLine.End, tolerance);
		}
		return false;
	}

	public Vector3 GetMiddle()
	{
		return BMath.Average(Start, End);
	}

	public Line TransformFast(Matrix4x4 transformationMatrix)
	{
		return new Line(transformationMatrix.MultiplyPoint3x4(Start), transformationMatrix.MultiplyPoint3x4(End));
	}

	public Line Transform(Matrix4x4 transformationMatrix)
	{
		return new Line(transformationMatrix.MultiplyPoint(Start), transformationMatrix.MultiplyPoint(End));
	}

	public Vector3 ClosestPoint(Vector3 point)
	{
		return point.ClosestPointOnSegment(this);
	}

	public Vector3 ClosestPoint(Vector3 planePoint, Vector3 planeNormal)
	{
		float num = Start.SignedDistanceFromPlane(planePoint, planeNormal);
		float num2 = End.SignedDistanceFromPlane(planePoint, planeNormal);
		if (num == num2)
		{
			return Start;
		}
		bool num3 = num > 0f;
		bool flag = num2 > 0f;
		if (num3 != flag)
		{
			BGeo.LinePlaneIntersection(Start, Direction, planePoint, planeNormal, out var intersection);
			return intersection;
		}
		if (BMath.Abs(num) < BMath.Abs(num2))
		{
			return Start;
		}
		return End;
	}

	public bool TryOverlapLine(Line otherLine, out Line newLine, out Line newOtherLine, out Line overlap, out OverlapInfo overlapInfo, float distanceTolerance = 0.001f)
	{
		newLine = this;
		newOtherLine = otherLine;
		overlap = default(Line);
		overlapInfo = OverlapInfo.Info.noOverlap;
		bool flag = false;
		if (!Direction.IsWithin01DegFrom(otherLine.Direction))
		{
			if (!Direction.IsWithin01DegFrom(-otherLine.Direction))
			{
				return false;
			}
			flag = true;
		}
		if (!Start.IsOnLine(otherLine.Start, otherLine.Direction, distanceTolerance))
		{
			return false;
		}
		if (flag)
		{
			otherLine = otherLine.Flipped;
		}
		if (Vector3.Dot(otherLine.Start - End, Direction) >= 0f)
		{
			return false;
		}
		if (Vector3.Dot(Start - otherLine.End, Direction) >= 0f)
		{
			return false;
		}
		if (Start.Approximately(otherLine.Start))
		{
			if (End.Approximately(otherLine.End))
			{
				newLine = Empty;
				newOtherLine = Empty;
				overlap = new Line(Start, End, Direction, Length);
				overlapInfo = OverlapInfo.Info.completeOverlap;
				return true;
			}
			if (Vector3.Dot(otherLine.End - End, Direction) > 0f)
			{
				newLine = Empty;
				overlap = this;
				if (flag)
				{
					newOtherLine = new Line(otherLine.End, End);
				}
				else
				{
					newOtherLine = new Line(End, otherLine.End);
				}
				overlapInfo = (OverlapInfo.Info)9;
				return true;
			}
			newLine = new Line(otherLine.End, End);
			newOtherLine = Empty;
			overlap = otherLine;
			overlapInfo = (OverlapInfo.Info)10;
			return true;
		}
		if (End.Approximately(otherLine.End))
		{
			if (Vector3.Dot(otherLine.Start - Start, Direction) > 0f)
			{
				newLine = new Line(Start, otherLine.Start);
				overlap = otherLine;
				newOtherLine = Empty;
				overlapInfo = (OverlapInfo.Info)10;
				return true;
			}
			newLine = Empty;
			overlap = this;
			if (flag)
			{
				newOtherLine = new Line(Start, otherLine.Start);
			}
			else
			{
				newOtherLine = new Line(otherLine.Start, Start);
			}
			overlapInfo = (OverlapInfo.Info)9;
			return true;
		}
		if (Vector3.Dot(otherLine.Start - Start, Direction) > 0f)
		{
			newLine = new Line(Start, otherLine.Start);
			if (Vector3.Dot(otherLine.End - End, Direction) > 0f)
			{
				overlap = new Line(otherLine.Start, End);
				if (flag)
				{
					newOtherLine = new Line(otherLine.End, End);
				}
				else
				{
					newOtherLine = new Line(End, otherLine.End);
				}
				overlapInfo = OverlapInfo.Info.otherLineAhead;
			}
			else
			{
				overlap = new Line(otherLine.Start, otherLine.End);
				if (flag)
				{
					newOtherLine = new Line(End, otherLine.End);
				}
				else
				{
					newOtherLine = new Line(otherLine.End, End);
				}
				overlapInfo = OverlapInfo.Info.otherLineFullyContained;
			}
			return true;
		}
		if (flag)
		{
			newOtherLine = new Line(Start, otherLine.Start);
		}
		else
		{
			newOtherLine = new Line(otherLine.Start, Start);
		}
		if (Vector3.Dot(otherLine.End - End, Direction) > 0f)
		{
			overlap = new Line(Start, End);
			newLine = new Line(End, otherLine.End);
			overlapInfo = OverlapInfo.Info.thisLineFullyContained;
		}
		else
		{
			overlap = new Line(Start, otherLine.End);
			newLine = new Line(otherLine.End, End);
			overlapInfo = OverlapInfo.Info.thisLineAhead;
		}
		return true;
	}

	public bool TryIntersectBox(Box box, out Line intersection)
	{
		return TryIntersectBox(box.center, box.Size, box.orientation, out intersection);
	}

	public bool TryIntersectBox(Vector3 boxCenter, Vector3 boxSize, Quaternion orientation, out Line intersection)
	{
		intersection = this;
		Matrix4x4 inverse = Matrix4x4.TRS(boxCenter, orientation, boxSize).inverse;
		if (!TransformFast(inverse).TryIntersectAABB(-0.5f * Vector3.one, 0.5f * Vector3.one, out var intersection2))
		{
			return false;
		}
		intersection = intersection2.TransformFast(inverse.inverse);
		return true;
	}

	public bool TryIntersectAABB(Vector3 min, Vector3 max, out Line intersection)
	{
		intersection = this;
		float num = (min.x - Start.x) * inverseDirection.x;
		float num2 = (max.x - Start.x) * inverseDirection.x;
		if (float.IsNaN(num))
		{
			num = float.NegativeInfinity;
		}
		if (float.IsNaN(num2))
		{
			num = float.PositiveInfinity;
		}
		if (num > num2)
		{
			(num, num2) = BMath.Swap(num, num2);
		}
		float num3 = (min.y - Start.y) * inverseDirection.y;
		float num4 = (max.y - Start.y) * inverseDirection.y;
		if (float.IsNaN(num3))
		{
			num3 = float.NegativeInfinity;
		}
		if (float.IsNaN(num4))
		{
			num3 = float.PositiveInfinity;
		}
		if (num3 > num4)
		{
			(num3, num4) = BMath.Swap(num3, num4);
		}
		if (num > num4 || num3 > num2)
		{
			return false;
		}
		num = BMath.Max(num, num3);
		num2 = BMath.Min(num2, num4);
		float num5 = (min.z - Start.z) * inverseDirection.z;
		float num6 = (max.z - Start.z) * inverseDirection.z;
		if (float.IsNaN(num5))
		{
			num5 = float.NegativeInfinity;
		}
		if (float.IsNaN(num6))
		{
			num5 = float.PositiveInfinity;
		}
		if (num5 > num6)
		{
			(num5, num6) = BMath.Swap(num5, num6);
		}
		if (num > num6 || num5 > num2)
		{
			return false;
		}
		num = BMath.Max(num, num5);
		num2 = BMath.Min(num2, num6);
		if (num > Length)
		{
			return false;
		}
		if (num2 < 0f)
		{
			return false;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		if (num2 > Length)
		{
			num2 = Length;
		}
		intersection = new Line(Start + num * Direction, Start + num2 * Direction);
		return true;
	}

	public void Draw()
	{
		BDebug.DrawLine(Start, End, Color.white);
	}

	public void Draw(Color color)
	{
		BDebug.DrawLine(Start, End, color);
	}

	private void RecalculateProperties()
	{
		Vector3 vector = End - Start;
		Length = vector.magnitude;
		LengthSquared = Length * Length;
		Direction = vector / Length;
		IsEmpty = Length <= float.Epsilon;
	}

	public override bool Equals(object other)
	{
		if (other is Line other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(Line other)
	{
		if (Start.Equals(other.Start))
		{
			return End.Equals(other.End);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (-1676728671 * -1521134295 + Start.GetHashCode()) * -1521134295 + End.GetHashCode();
	}

	public static bool operator ==(Line left, Line right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Line left, Line right)
	{
		return !(left == right);
	}
}
