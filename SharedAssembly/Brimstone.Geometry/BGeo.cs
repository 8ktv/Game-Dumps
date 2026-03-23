using System;
using UnityEngine;

namespace Brimstone.Geometry;

public static class BGeo
{
	public static float CosineRuleRad(float a, float b, float gamma)
	{
		if (gamma < 0f)
		{
			throw new ArgumentOutOfRangeException("gamma", "Angle must be non-negative.");
		}
		return BMath.Sqrt(a * a + b * b + 2f * a * b * BMath.Cos(gamma));
	}

	public static float CosineRuleDeg(float a, float b, float gamma)
	{
		if (gamma < 0f)
		{
			throw new ArgumentOutOfRangeException("gamma", "Angle must be non-negative.");
		}
		return BMath.Sqrt(a * a + b * b + 2f * a * b * BMath.CosDeg(gamma));
	}

	public static float IsoscelesTriangleSideLengthRad(float baseLength, float vertexAngle)
	{
		if (vertexAngle < 0f)
		{
			throw new ArgumentOutOfRangeException("vertexAngle", "Vertex angle must be non-negative.");
		}
		return baseLength / (2f * BMath.Sin(vertexAngle / 2f));
	}

	public static float IsoscelesTriangleSideLengthDeg(float baseLength, float vertexAngle)
	{
		if (vertexAngle < 0f)
		{
			throw new ArgumentOutOfRangeException("vertexAngle", "Vertex angle must be non-negative.");
		}
		return baseLength / (2f * BMath.SinDeg(vertexAngle / 2f));
	}

	public static Vector2 ClosestPointInAxisAlignedRect(Vector2 rectMin, Vector2 rectMax, Vector2 point)
	{
		return BMath.Clamp(point, rectMin, rectMax);
	}

	public static bool LineLineIntersection2d(Vector2 line1Point, Vector2 line1Direction, Vector2 line2Point, Vector2 line2Direction, out Vector2 intersection)
	{
		intersection = Vector2.zero;
		float num = line2Direction.x * line1Direction.y - line1Direction.x * line2Direction.y;
		if (BMath.Abs(num) < 0.0001f)
		{
			return false;
		}
		float num2 = (line2Direction.y * (line1Point.x - line2Point.x) + line2Direction.x * (line2Point.y - line1Point.y)) / num;
		intersection = line1Point + num2 * line1Direction;
		return true;
	}

	public static bool RayLineIntersection2d(Vector2 rayPoint, Vector2 rayDirection, Vector2 linePoint, Vector2 lineDirection, out Vector2 intersection)
	{
		intersection = Vector2.zero;
		float num = lineDirection.x * rayDirection.y - rayDirection.x * lineDirection.y;
		if (BMath.Abs(num) < 0.0001f)
		{
			return false;
		}
		float num2 = (lineDirection.y * (rayPoint.x - linePoint.x) + lineDirection.x * (linePoint.y - rayPoint.y)) / num;
		if (num2 < 0f)
		{
			return false;
		}
		intersection = rayPoint + num2 * rayDirection;
		return true;
	}

	public static bool RaySegmentIntersection2d(Vector2 rayPoint, Vector2 rayDirection, Vector2 segmentStart, Vector2 segmentEnd, out Vector2 intersection)
	{
		intersection = Vector2.zero;
		Vector2 vector = segmentEnd - segmentStart;
		float num = vector.x * rayDirection.y - rayDirection.x * vector.y;
		if (BMath.Abs(num) < 0.0001f)
		{
			return false;
		}
		float num2 = 1f / num;
		float num3 = (vector.y * (rayPoint.x - segmentStart.x) + vector.x * (segmentStart.y - rayPoint.y)) * num2;
		if (num3 < 0f)
		{
			return false;
		}
		float num4 = (rayDirection.y * (rayPoint.x - segmentStart.x) + rayDirection.x * (segmentStart.y - rayPoint.y)) * num2;
		if (num4 < 0f || num4 > 1f)
		{
			return false;
		}
		intersection = rayPoint + num3 * rayDirection;
		return true;
	}

	public static bool SegmentSegmentIntersection2d(Vector2 segment1Start, Vector2 segment1End, Vector2 segment2Start, Vector2 segment2End, out Vector2 intersection)
	{
		intersection = Vector2.zero;
		Vector2 vector = segment1End - segment1Start;
		Vector2 vector2 = segment2End - segment2Start;
		float num = vector2.x * vector.y - vector.x * vector2.y;
		if (BMath.Abs(num) < 0.0001f)
		{
			return false;
		}
		float num2 = (vector2.y * (segment1Start.x - segment2Start.x) + vector2.x * (segment2Start.y - segment1Start.y)) / num;
		if (num2 < 0f || num2 > 1f)
		{
			return false;
		}
		float num3 = (vector.y * (segment1Start.x - segment2Start.x) + vector.x * (segment2Start.y - segment1Start.y)) / num;
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		intersection = segment1Start + num2 * vector;
		return true;
	}

	public static int SegmentRectangleIntersection2d(Vector2 segmentStart, Vector2 segmentEnd, Vector2 boxCenter, Vector2 boxSize, out Vector2 closeIntersection, out Vector2 farIntersection)
	{
		closeIntersection = Vector2.zero;
		farIntersection = Vector2.zero;
		Vector2 vector = boxSize * 0.5f;
		Vector2 vector2 = new Vector2(0f - vector.x, vector.y);
		Vector2 vector3 = boxCenter - vector;
		Vector2 vector4 = boxCenter - vector2;
		Vector2 vector5 = boxCenter + vector;
		Vector2 vector6 = boxCenter + vector2;
		int num = 0;
		if (SegmentSegmentIntersection2d(segmentStart, segmentEnd, vector3, vector4, out var intersection))
		{
			closeIntersection = intersection;
			num++;
		}
		if (SegmentSegmentIntersection2d(segmentStart, segmentEnd, vector4, vector5, out intersection))
		{
			if (num == 1)
			{
				if (!intersection.Approximately(closeIntersection, 1E-05f))
				{
					farIntersection = intersection;
					num++;
				}
			}
			else
			{
				closeIntersection = intersection;
				num++;
			}
		}
		if (num == 2)
		{
			(closeIntersection, farIntersection) = SortIntersections(closeIntersection, farIntersection);
			return 2;
		}
		if (SegmentSegmentIntersection2d(segmentStart, segmentEnd, vector5, vector6, out intersection))
		{
			if (num == 1)
			{
				if (!intersection.Approximately(closeIntersection, 1E-05f))
				{
					farIntersection = intersection;
					num++;
				}
			}
			else
			{
				closeIntersection = intersection;
				num++;
			}
		}
		if (num == 2)
		{
			(closeIntersection, farIntersection) = SortIntersections(closeIntersection, farIntersection);
			return 2;
		}
		if (SegmentSegmentIntersection2d(segmentStart, segmentEnd, vector6, vector3, out intersection))
		{
			if (num == 1)
			{
				if (!intersection.Approximately(closeIntersection, 1E-05f))
				{
					farIntersection = intersection;
					num++;
				}
			}
			else
			{
				closeIntersection = intersection;
				num++;
			}
		}
		if (num == 2)
		{
			(closeIntersection, farIntersection) = SortIntersections(closeIntersection, farIntersection);
			return 2;
		}
		return num;
		(Vector2 close, Vector2 far) SortIntersections(Vector2 vector7, Vector2 vector8)
		{
			if ((vector7 - segmentStart).sqrMagnitude <= (vector8 - segmentStart).sqrMagnitude)
			{
				return (close: vector7, far: vector8);
			}
			return (close: vector8, far: vector7);
		}
	}

	public static int LineCircleIntersection(Vector2 linePoint, Vector2 lineDirection, Vector2 circleCenter, float circleRadius, out Vector2 closeIntersection, out Vector2 farIntersection)
	{
		float closeIntersectionLineT;
		float farIntersectionLineT;
		return LineCircleIntersection(linePoint, lineDirection, circleCenter, circleRadius, out closeIntersection, out farIntersection, out closeIntersectionLineT, out farIntersectionLineT);
	}

	public static int LineCircleIntersection(Vector2 linePoint, Vector2 lineDirection, Vector2 circleCenter, float circleRadius, out Vector2 closeIntersection, out Vector2 farIntersection, out float closeIntersectionLineT, out float farIntersectionLineT)
	{
		closeIntersection = default(Vector2);
		farIntersection = default(Vector2);
		closeIntersectionLineT = 0f;
		farIntersectionLineT = 0f;
		Vector2 rhs = linePoint - circleCenter;
		float sqrMagnitude = lineDirection.sqrMagnitude;
		float b = 2f * Vector2.Dot(lineDirection, rhs);
		float c = rhs.sqrMagnitude - circleRadius * circleRadius;
		float root;
		float root2;
		switch (BMath.SolveQuadratic(sqrMagnitude, b, c, out root, out root2))
		{
		case 0:
			return 0;
		case 1:
			closeIntersection = linePoint + root * lineDirection;
			closeIntersectionLineT = root;
			return 1;
		default:
			if (BMath.Abs(root) <= BMath.Abs(root2))
			{
				closeIntersection = linePoint + root * lineDirection;
				farIntersection = linePoint + root2 * lineDirection;
				closeIntersectionLineT = root;
				farIntersectionLineT = root2;
			}
			else
			{
				closeIntersection = linePoint + root2 * lineDirection;
				farIntersection = linePoint + root * lineDirection;
				closeIntersectionLineT = root2;
				farIntersectionLineT = root;
			}
			return 2;
		}
	}

	public static int SegmentCircleIntersection(Vector2 segmentStart, Vector2 segmentEnd, Vector2 circleCenter, float circleRadius, out Vector2 closeIntersection, out Vector2 farIntersection)
	{
		float closeIntersectionLineT;
		float farIntersectionLineT;
		return SegmentCircleIntersection(segmentStart, segmentEnd, circleCenter, circleRadius, out closeIntersection, out farIntersection, out closeIntersectionLineT, out farIntersectionLineT);
	}

	public static int SegmentCircleIntersection(Vector2 segmentStart, Vector2 segmentEnd, Vector2 circleCenter, float circleRadius, out Vector2 closeIntersection, out Vector2 farIntersection, out float closeIntersectionLineT, out float farIntersectionLineT)
	{
		closeIntersection = default(Vector2);
		farIntersection = default(Vector2);
		closeIntersectionLineT = 0f;
		farIntersectionLineT = 0f;
		Vector2 rhs = segmentStart - circleCenter;
		Vector2 vector = segmentEnd - segmentStart;
		float sqrMagnitude = vector.sqrMagnitude;
		float b = 2f * Vector2.Dot(vector, rhs);
		float c = rhs.sqrMagnitude - circleRadius * circleRadius;
		float root;
		float root2;
		switch (BMath.SolveQuadratic(sqrMagnitude, b, c, out root, out root2))
		{
		case 0:
			return 0;
		case 1:
			if (root >= 0f)
			{
				closeIntersection = segmentStart + root * vector;
				closeIntersectionLineT = root;
				return 1;
			}
			return 0;
		default:
		{
			bool flag = 0f <= root && root <= 1f;
			bool flag2 = 0f <= root2 && root2 <= 1f;
			if (flag && flag2)
			{
				if (BMath.Abs(root) <= BMath.Abs(root2))
				{
					closeIntersection = segmentStart + root * vector;
					farIntersection = segmentStart + root2 * vector;
					closeIntersectionLineT = root;
					farIntersectionLineT = root2;
				}
				else
				{
					closeIntersection = segmentStart + root2 * vector;
					farIntersection = segmentStart + root * vector;
					closeIntersectionLineT = root2;
					farIntersectionLineT = root;
				}
				return 2;
			}
			if (flag)
			{
				closeIntersection = segmentStart + root * vector;
				closeIntersectionLineT = root;
				return 1;
			}
			if (flag2)
			{
				closeIntersection = segmentStart + root2 * vector;
				closeIntersectionLineT = root2;
				return 1;
			}
			return 0;
		}
		}
	}

	public static int RayCircleIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 circleCenter, float circleRadius, out Vector2 closeIntersection, out Vector2 farIntersection)
	{
		float closeIntersectionLineT;
		float farIntersectionLineT;
		return RayCircleIntersection(rayOrigin, rayDirection, circleCenter, circleRadius, out closeIntersection, out farIntersection, out closeIntersectionLineT, out farIntersectionLineT);
	}

	public static int RayCircleIntersection(Vector2 rayOrigin, Vector2 rayDirection, Vector2 circleCenter, float circleRadius, out Vector2 closeIntersection, out Vector2 farIntersection, out float closeIntersectionLineT, out float farIntersectionLineT)
	{
		closeIntersection = default(Vector2);
		farIntersection = default(Vector2);
		closeIntersectionLineT = 0f;
		farIntersectionLineT = 0f;
		Vector2 rhs = rayOrigin - circleCenter;
		float sqrMagnitude = rayDirection.sqrMagnitude;
		float b = 2f * Vector2.Dot(rayDirection, rhs);
		float c = rhs.sqrMagnitude - circleRadius * circleRadius;
		float root;
		float root2;
		switch (BMath.SolveQuadratic(sqrMagnitude, b, c, out root, out root2))
		{
		case 0:
			return 0;
		case 1:
			if (root >= 0f)
			{
				closeIntersection = rayOrigin + root * rayDirection;
				closeIntersectionLineT = root;
				return 1;
			}
			return 0;
		default:
		{
			bool flag = root >= 0f;
			bool flag2 = root2 >= 0f;
			if (flag && flag2)
			{
				if (BMath.Abs(root) <= BMath.Abs(root2))
				{
					closeIntersection = rayOrigin + root * rayDirection;
					farIntersection = rayOrigin + root2 * rayDirection;
					closeIntersectionLineT = root;
					farIntersectionLineT = root2;
				}
				else
				{
					closeIntersection = rayOrigin + root2 * rayDirection;
					farIntersection = rayOrigin + root * rayDirection;
					closeIntersectionLineT = root2;
					farIntersectionLineT = root;
				}
				return 2;
			}
			if (flag)
			{
				closeIntersection = rayOrigin + root * rayDirection;
				closeIntersectionLineT = root;
				return 1;
			}
			if (flag2)
			{
				closeIntersection = rayOrigin + root2 * rayDirection;
				closeIntersectionLineT = root2;
				return 1;
			}
			return 0;
		}
		}
	}

	public static void ForEachBresenhamLine(Vector2Int start, Vector2Int end, Action<Vector2Int> Action, bool contiguous = false, Func<bool> ShouldExitEarly = null)
	{
		bool hasEarlyExit = ShouldExitEarly != null;
		float num = BMath.Abs(end.x - start.x);
		float num2 = BMath.Abs(end.y - start.y);
		if (num == 0f)
		{
			ForEachVerticalLine();
		}
		else if (num2 == 0f)
		{
			ForEachHorizontalLine();
		}
		else if (num2 < num)
		{
			ForEachShallowLine();
		}
		else
		{
			ForEachSteepLine();
		}
		void ForEachHorizontalLine()
		{
			int y = start.y;
			int num3 = ((end.x - start.x > 0) ? 1 : (-1));
			for (int i = start.x; i != end.x + num3; i += num3)
			{
				if (hasEarlyExit && ShouldExitEarly())
				{
					break;
				}
				Action(new Vector2Int(i, y));
			}
		}
		void ForEachShallowLine()
		{
			int num3 = end.x - start.x;
			int num4 = end.y - start.y;
			int num5 = ((num3 > 0) ? 1 : (-1));
			int num6 = 1;
			if (num4 < 0)
			{
				num6 = -1;
				num4 = -num4;
			}
			int num7 = 2 * num4 - num3;
			int num8 = start.y;
			num3 = BMath.Abs(num3);
			for (int i = start.x; i != end.x + num5; i += num5)
			{
				if (hasEarlyExit && ShouldExitEarly())
				{
					break;
				}
				Action(new Vector2Int(i, num8));
				if (num7 > 0)
				{
					num8 += num6;
					num7 += 2 * (num4 - num3);
					if (contiguous)
					{
						if (hasEarlyExit && ShouldExitEarly())
						{
							break;
						}
						Action(new Vector2Int(i, num8));
					}
				}
				else
				{
					num7 += 2 * num4;
				}
			}
		}
		void ForEachSteepLine()
		{
			int num3 = end.x - start.x;
			int num4 = end.y - start.y;
			int num5 = 1;
			int num6 = ((num4 > 0) ? 1 : (-1));
			if (num3 < 0)
			{
				num5 = -1;
				num3 = -num3;
			}
			int num7 = 2 * num3 - num4;
			int num8 = start.x;
			num4 = BMath.Abs(num4);
			for (int i = start.y; i != end.y + num6; i += num6)
			{
				if (hasEarlyExit && ShouldExitEarly())
				{
					break;
				}
				Action(new Vector2Int(num8, i));
				if (num7 > 0)
				{
					num8 += num5;
					num7 += 2 * (num3 - num4);
					if (contiguous)
					{
						if (hasEarlyExit && ShouldExitEarly())
						{
							break;
						}
						Action(new Vector2Int(num8, i));
					}
				}
				else
				{
					num7 += 2 * num3;
				}
			}
		}
		void ForEachVerticalLine()
		{
			int x = start.x;
			int num3 = ((end.y - start.y > 0) ? 1 : (-1));
			for (int i = start.y; i != end.y + num3; i += num3)
			{
				if (hasEarlyExit && ShouldExitEarly())
				{
					break;
				}
				Action(new Vector2Int(x, i));
			}
		}
	}

	public static Vector3 ClosestPointOnTriangle(Vector3 point, Vector3 trianglePoint0, Vector3 trianglePoint1, Vector3 trianglePoint2, out Vector3 triangleNormal)
	{
		Vector3 lhs = trianglePoint1 - trianglePoint0;
		Vector3 vector = trianglePoint2 - trianglePoint1;
		Vector3 lhs2 = trianglePoint0 - trianglePoint2;
		triangleNormal = Vector3.Cross(lhs, vector).normalized;
		if (triangleNormal.sqrMagnitude < 0.0001f)
		{
			return Vector3.negativeInfinity;
		}
		Vector3 vector2 = point.ProjectPointOnPlane(trianglePoint0, triangleNormal);
		Vector3 planeNormal = Vector3.Cross(lhs, triangleNormal);
		Vector3 planeNormal2 = Vector3.Cross(vector, triangleNormal);
		Vector3 planeNormal3 = Vector3.Cross(lhs2, triangleNormal);
		if (vector2.IsAbovePlane(trianglePoint0, planeNormal))
		{
			if (vector2.IsAbovePlane(trianglePoint1, planeNormal2))
			{
				return trianglePoint1;
			}
			if (vector2.IsAbovePlane(trianglePoint2, planeNormal3))
			{
				return trianglePoint0;
			}
			return vector2.ClosestPointOnSegment(trianglePoint0, trianglePoint1);
		}
		if (vector2.IsAbovePlane(trianglePoint1, planeNormal2))
		{
			if (vector2.IsAbovePlane(trianglePoint2, planeNormal3))
			{
				return trianglePoint2;
			}
			return vector2.ClosestPointOnSegment(trianglePoint1, trianglePoint2);
		}
		if (vector2.IsAbovePlane(trianglePoint2, planeNormal3))
		{
			return vector2.ClosestPointOnSegment(trianglePoint2, trianglePoint0);
		}
		return vector2;
	}

	public static bool ClosestPointsOnTwoLines(Vector3 line1Point, Vector3 line1Direction, Vector3 line2Point, Vector3 line2Direction, out Vector3 closestPointLine1, out Vector3 closestPointLine2)
	{
		closestPointLine1 = Vector3.zero;
		closestPointLine2 = Vector3.zero;
		float sqrMagnitude = line1Direction.sqrMagnitude;
		float num = Vector3.Dot(line1Direction, line2Direction);
		float sqrMagnitude2 = line2Direction.sqrMagnitude;
		float num2 = sqrMagnitude * sqrMagnitude2 - num * num;
		if ((double)num2 <= 1E-05)
		{
			return false;
		}
		Vector3 rhs = line1Point - line2Point;
		float num3 = Vector3.Dot(line1Direction, rhs);
		float num4 = Vector3.Dot(line2Direction, rhs);
		float num5 = (num * num4 - num3 * sqrMagnitude2) / num2;
		float num6 = (sqrMagnitude * num4 - num3 * num) / num2;
		closestPointLine1 = line1Point + num5 * line1Direction;
		closestPointLine2 = line2Point + num6 * line2Direction;
		return true;
	}

	public static bool ClosestPointsOnTwoSegments(Vector3 segment1Start, Vector3 segment1End, Vector3 segment2Start, Vector3 segment2End, out Vector3 closestPointSegment1, out Vector3 closestPointSegment2, float distanceTolerance = float.PositiveInfinity)
	{
		Vector3 vector = segment1End - segment1Start;
		Vector3 vector2 = segment2End - segment2Start;
		closestPointSegment1 = Vector3.zero;
		closestPointSegment2 = Vector3.zero;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = Vector3.Dot(vector, vector2);
		float sqrMagnitude2 = vector2.sqrMagnitude;
		float num2 = sqrMagnitude * sqrMagnitude2 - num * num;
		if (BMath.Abs(num2) <= 1E-05f)
		{
			Vector3 rhs = segment1Start - segment2Start;
			Vector3 rhs2 = segment1Start - segment2End;
			float num3 = Vector3.Dot(vector, rhs);
			float num4 = Vector3.Dot(vector, rhs2);
			if ((double)num3 >= -0.001 && (double)num4 >= -0.001)
			{
				closestPointSegment1 = segment1Start;
				if (num3 < num4)
				{
					closestPointSegment2 = segment2Start;
				}
				else
				{
					closestPointSegment2 = segment2End;
				}
				if ((closestPointSegment1 - closestPointSegment2).sqrMagnitude > distanceTolerance * distanceTolerance)
				{
					return false;
				}
				return true;
			}
			Vector3 rhs3 = segment2Start - segment1End;
			Vector3 rhs4 = segment2End - segment1End;
			float num5 = Vector3.Dot(vector, rhs3);
			float num6 = Vector3.Dot(vector, rhs4);
			if ((double)num5 >= -0.001 && (double)num6 >= -0.001)
			{
				closestPointSegment1 = segment1End;
				if (num5 < num6)
				{
					closestPointSegment2 = segment2Start;
				}
				else
				{
					closestPointSegment2 = segment2End;
				}
				if ((closestPointSegment1 - closestPointSegment2).sqrMagnitude > distanceTolerance * distanceTolerance)
				{
					return false;
				}
				return true;
			}
			return false;
		}
		Vector3 rhs5 = segment1Start - segment2Start;
		float num7 = Vector3.Dot(vector, rhs5);
		float num8 = Vector3.Dot(vector2, rhs5);
		float num9 = BMath.Clamp01((num * num8 - num7 * sqrMagnitude2) / num2);
		float num10 = BMath.Clamp01((sqrMagnitude * num8 - num7 * num) / num2);
		closestPointSegment1 = segment1Start + num9 * vector;
		closestPointSegment2 = segment2Start + num10 * vector2;
		if ((closestPointSegment1 - closestPointSegment2).sqrMagnitude > distanceTolerance * distanceTolerance)
		{
			return false;
		}
		return true;
	}

	public static Vector3 ClosestPointInAxisAlignedBox(Vector3 boxMin, Vector3 boxMax, Vector3 point)
	{
		return BMath.Clamp(point, boxMin, boxMax);
	}

	public static bool BoxPointOverlap(Vector3 boxCenter, Vector3 boxExtents, Quaternion boxRotation, Vector3 point)
	{
		Vector3 vector = (Matrix4x4.Rotate(Quaternion.Inverse(boxRotation)) * Matrix4x4.Translate(-boxCenter)).MultiplyPoint3x4(point);
		if (BMath.Abs(vector.x) <= boxExtents.x && BMath.Abs(vector.y) <= boxExtents.y)
		{
			return BMath.Abs(vector.z) <= boxExtents.z;
		}
		return false;
	}

	public static bool LineLineIntersection(Vector3 line1Point, Vector3 line1Direction, Vector3 line2Point, Vector3 line2Direction, out Vector3 intersection, float distanceTolerance = float.PositiveInfinity)
	{
		intersection = Vector3.zero;
		Vector3 lhs = line2Point - line1Point;
		Vector3 rhs = Vector3.Cross(line1Direction, line2Direction);
		if (rhs.sqrMagnitude < 0.0001f)
		{
			return false;
		}
		if (BMath.Abs(Vector3.Dot(lhs, rhs)) < 0.0001f)
		{
			float num = Vector3.Dot(Vector3.Cross(lhs, line2Direction), rhs) / rhs.sqrMagnitude;
			intersection = line1Point + line1Direction * num;
			return true;
		}
		if (distanceTolerance <= 0f)
		{
			return false;
		}
		ClosestPointsOnTwoLines(line1Point, line1Direction, line2Point, line2Direction, out var closestPointLine, out var closestPointLine2);
		if ((closestPointLine - closestPointLine2).sqrMagnitude > distanceTolerance * distanceTolerance)
		{
			return false;
		}
		intersection = (closestPointLine + closestPointLine2) * 0.5f;
		return true;
	}

	public static bool LinePlaneIntersection(Vector3 linePoint, Vector3 lineDirection, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection)
	{
		intersection = Vector3.negativeInfinity;
		if (BMath.Abs(planeNormal.sqrMagnitude - 1f) > 2E-05f)
		{
			planeNormal.Normalize();
		}
		float num = Vector3.Dot(lineDirection, planeNormal);
		if ((double)BMath.Abs(num) < 1E-05)
		{
			return false;
		}
		float num2 = Vector3.Dot(planePoint, planeNormal);
		float num3 = Vector3.Dot(linePoint, planeNormal);
		float num4 = (num2 - num3) / num;
		intersection = linePoint + num4 * lineDirection;
		return true;
	}

	public static bool SegmentPlaneIntersection(Vector3 segmentStart, Vector3 segmentEnd, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection)
	{
		intersection = Vector3.negativeInfinity;
		Vector3 vector = segmentEnd - segmentStart;
		if (BMath.Abs(planeNormal.sqrMagnitude - 1f) > 2E-05f)
		{
			planeNormal.Normalize();
		}
		float num = Vector3.Dot(vector, planeNormal);
		if ((double)BMath.Abs(num) < 1E-05)
		{
			return false;
		}
		float num2 = Vector3.Dot(planePoint, planeNormal);
		float num3 = Vector3.Dot(segmentStart, planeNormal);
		float num4 = (num2 - num3) / num;
		if (num4 < 0f || num4 > 1f)
		{
			return false;
		}
		intersection = segmentStart + num4 * vector;
		return true;
	}

	public static bool RayPlaneIntersection(Ray ray, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection)
	{
		return RayPlaneIntersection(ray.origin, ray.direction, planePoint, planeNormal, out intersection);
	}

	public static bool RayPlaneIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection)
	{
		intersection = Vector3.negativeInfinity;
		if (BMath.Abs(planeNormal.sqrMagnitude - 1f) > 2E-05f)
		{
			planeNormal.Normalize();
		}
		float num = Vector3.Dot(rayDirection, planeNormal);
		if ((double)BMath.Abs(num) < 1E-05)
		{
			return false;
		}
		float num2 = Vector3.Dot(planePoint, planeNormal);
		float num3 = Vector3.Dot(rayOrigin, planeNormal);
		float num4 = (num2 - num3) / num;
		if (num4 < 0f)
		{
			return false;
		}
		intersection = rayOrigin + num4 * rayDirection;
		return true;
	}

	public static bool SpherePointOverlap(Vector3 sphereCenter, float sphereRadius, Vector3 point)
	{
		return (point - sphereCenter).sqrMagnitude <= sphereRadius * sphereRadius;
	}

	public static int LineSphereIntersection(Vector3 linePoint, Vector3 lineDirection, Vector3 sphereCenter, float sphereRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		float closeIntersectionLineT;
		float farIntersectionLineT;
		return LineSphereIntersection(linePoint, lineDirection, sphereCenter, sphereRadius, out closeIntersection, out farIntersection, out closeIntersectionLineT, out farIntersectionLineT);
	}

	public static int LineSphereIntersection(Vector3 linePoint, Vector3 lineDirection, Vector3 sphereCenter, float sphereRadius, out Vector3 closeIntersection, out Vector3 farIntersection, out float closeIntersectionLineT, out float farIntersectionLineT)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		closeIntersectionLineT = 0f;
		farIntersectionLineT = 0f;
		Vector3 rhs = linePoint - sphereCenter;
		float sqrMagnitude = lineDirection.sqrMagnitude;
		float b = 2f * Vector3.Dot(lineDirection, rhs);
		float c = rhs.sqrMagnitude - sphereRadius * sphereRadius;
		float root;
		float root2;
		switch (BMath.SolveQuadratic(sqrMagnitude, b, c, out root, out root2))
		{
		case 0:
			return 0;
		case 1:
			closeIntersection = linePoint + root * lineDirection;
			closeIntersectionLineT = root;
			return 1;
		default:
			if (BMath.Abs(root) <= BMath.Abs(root2))
			{
				closeIntersection = linePoint + root * lineDirection;
				farIntersection = linePoint + root2 * lineDirection;
				closeIntersectionLineT = root;
				farIntersectionLineT = root2;
			}
			else
			{
				closeIntersection = linePoint + root2 * lineDirection;
				farIntersection = linePoint + root * lineDirection;
				closeIntersectionLineT = root2;
				farIntersectionLineT = root;
			}
			return 2;
		}
	}

	public static int SegmentSphereIntersection(Vector3 segmentStart, Vector3 segmentEnd, Vector3 sphereCenter, float sphereRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		float closeIntersectionLineT;
		float farIntersectionLineT;
		return SegmentSphereIntersection(segmentStart, segmentEnd, sphereCenter, sphereRadius, out closeIntersection, out farIntersection, out closeIntersectionLineT, out farIntersectionLineT);
	}

	public static int SegmentSphereIntersection(Vector3 segmentStart, Vector3 segmentEnd, Vector3 sphereCenter, float sphereRadius, out Vector3 closeIntersection, out Vector3 farIntersection, out float closeIntersectionLineT, out float farIntersectionLineT)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		closeIntersectionLineT = 0f;
		farIntersectionLineT = 0f;
		Vector3 rhs = segmentStart - sphereCenter;
		Vector3 vector = segmentEnd - segmentStart;
		float sqrMagnitude = vector.sqrMagnitude;
		float b = 2f * Vector3.Dot(vector, rhs);
		float c = rhs.sqrMagnitude - sphereRadius * sphereRadius;
		float root;
		float root2;
		switch (BMath.SolveQuadratic(sqrMagnitude, b, c, out root, out root2))
		{
		case 0:
			return 0;
		case 1:
			if (root >= 0f)
			{
				closeIntersection = segmentStart + root * vector;
				closeIntersectionLineT = root;
				return 1;
			}
			return 0;
		default:
		{
			bool flag = 0f <= root && root <= 1f;
			bool flag2 = 0f <= root2 && root2 <= 1f;
			if (flag && flag2)
			{
				if (BMath.Abs(root) <= BMath.Abs(root2))
				{
					closeIntersection = segmentStart + root * vector;
					farIntersection = segmentStart + root2 * vector;
					closeIntersectionLineT = root;
					farIntersectionLineT = root2;
				}
				else
				{
					closeIntersection = segmentStart + root2 * vector;
					farIntersection = segmentStart + root * vector;
					closeIntersectionLineT = root2;
					farIntersectionLineT = root;
				}
				return 2;
			}
			if (flag)
			{
				closeIntersection = segmentStart + root * vector;
				closeIntersectionLineT = root;
				return 1;
			}
			if (flag2)
			{
				closeIntersection = segmentStart + root2 * vector;
				closeIntersectionLineT = root2;
				return 1;
			}
			return 0;
		}
		}
	}

	public static int RaySphereIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		float closeIntersectionLineT;
		float farIntersectionLineT;
		return RaySphereIntersection(rayOrigin, rayDirection, sphereCenter, sphereRadius, out closeIntersection, out farIntersection, out closeIntersectionLineT, out farIntersectionLineT);
	}

	public static int RaySphereIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius, out Vector3 closeIntersection, out Vector3 farIntersection, out float closeIntersectionLineT, out float farIntersectionLineT)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		closeIntersectionLineT = 0f;
		farIntersectionLineT = 0f;
		Vector3 rhs = rayOrigin - sphereCenter;
		float sqrMagnitude = rayDirection.sqrMagnitude;
		float b = 2f * Vector3.Dot(rayDirection, rhs);
		float c = rhs.sqrMagnitude - sphereRadius * sphereRadius;
		float root;
		float root2;
		switch (BMath.SolveQuadratic(sqrMagnitude, b, c, out root, out root2))
		{
		case 0:
			return 0;
		case 1:
			if (root >= 0f)
			{
				closeIntersection = rayOrigin + root * rayDirection;
				closeIntersectionLineT = root;
				return 1;
			}
			return 0;
		default:
		{
			bool flag = root >= 0f;
			bool flag2 = root2 >= 0f;
			if (flag && flag2)
			{
				if (BMath.Abs(root) <= BMath.Abs(root2))
				{
					closeIntersection = rayOrigin + root * rayDirection;
					farIntersection = rayOrigin + root2 * rayDirection;
					closeIntersectionLineT = root;
					farIntersectionLineT = root2;
				}
				else
				{
					closeIntersection = rayOrigin + root2 * rayDirection;
					farIntersection = rayOrigin + root * rayDirection;
					closeIntersectionLineT = root2;
					farIntersectionLineT = root;
				}
				return 2;
			}
			if (flag)
			{
				closeIntersection = rayOrigin + root * rayDirection;
				closeIntersectionLineT = root;
				return 1;
			}
			if (flag2)
			{
				closeIntersection = rayOrigin + root2 * rayDirection;
				closeIntersectionLineT = root2;
				return 1;
			}
			return 0;
		}
		}
	}

	public static int LineHemisphereIntersection(Vector3 linePoint, Vector3 lineDirection, Vector3 sphereCenter, float sphereRadius, Vector3 hemisphereDirection, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		Vector3 closeIntersection2;
		Vector3 farIntersection2;
		int num = LineSphereIntersection(linePoint, lineDirection, sphereCenter, sphereRadius, out closeIntersection2, out farIntersection2);
		if (num == 0)
		{
			return 0;
		}
		bool flag = Vector3.Dot(closeIntersection2 - sphereCenter, hemisphereDirection) < 0f;
		if (num == 1)
		{
			if (!flag)
			{
				return 0;
			}
			closeIntersection = closeIntersection2;
			return 1;
		}
		bool flag2 = Vector3.Dot(farIntersection2 - sphereCenter, hemisphereDirection) < 0f;
		if (flag == flag2)
		{
			if (flag)
			{
				closeIntersection = closeIntersection2;
				farIntersection = farIntersection2;
				return 2;
			}
			return 0;
		}
		if (flag)
		{
			closeIntersection = closeIntersection2;
			return 1;
		}
		closeIntersection = farIntersection2;
		return 1;
	}

	public static int RayHemisphereIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius, Vector3 hemisphereDirection, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		Vector3 closeIntersection2;
		Vector3 farIntersection2;
		int num = RaySphereIntersection(rayOrigin, rayDirection, sphereCenter, sphereRadius, out closeIntersection2, out farIntersection2);
		if (num == 0)
		{
			return 0;
		}
		bool flag = Vector3.Dot(closeIntersection2 - sphereCenter, hemisphereDirection) >= 0f;
		if (num == 1)
		{
			if (!flag)
			{
				return 0;
			}
			closeIntersection = closeIntersection2;
			return 1;
		}
		bool flag2 = Vector3.Dot(farIntersection2 - sphereCenter, hemisphereDirection) >= 0f;
		if (flag == flag2)
		{
			if (flag)
			{
				closeIntersection = closeIntersection2;
				farIntersection = farIntersection2;
				return 2;
			}
			return 0;
		}
		if (flag)
		{
			closeIntersection = closeIntersection2;
			return 1;
		}
		closeIntersection = farIntersection2;
		return 1;
	}

	public static int LineCylinderIntersection(Vector3 linePoint, Vector3 lineDirection, Vector3 cylinderPoint, Vector3 cylinderDirection, float cylinderRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		if (BMath.Abs(cylinderPoint.sqrMagnitude - 1f) > 2E-05f)
		{
			cylinderPoint.Normalize();
		}
		if (lineDirection.IsWithin01DegFrom(cylinderDirection))
		{
			return 0;
		}
		Vector3 closestPointLine = Vector3.Cross(lineDirection, cylinderDirection);
		Vector3 normalized = closestPointLine.normalized;
		Vector3 rhs = Vector3.Cross(cylinderDirection, normalized);
		float num = Vector3.Dot(linePoint - cylinderPoint, normalized);
		float num2 = Vector3.Dot(lineDirection, rhs);
		float a = num2 * num2;
		float b = 0f;
		float c = num * num - cylinderRadius * cylinderRadius;
		float root;
		float root2;
		int num3 = BMath.SolveQuadratic(a, b, c, out root, out root2);
		if (num3 == 0)
		{
			return 0;
		}
		ClosestPointsOnTwoLines(linePoint, lineDirection, cylinderPoint, cylinderDirection, out var closestPointLine2, out closestPointLine);
		float num4 = BMath.Sqrt((closestPointLine2 - linePoint).sqrMagnitude / lineDirection.sqrMagnitude);
		root += num4;
		root2 += num4;
		if (num3 == 1)
		{
			closeIntersection = linePoint + root * lineDirection;
			return 1;
		}
		if (BMath.Abs(root) <= BMath.Abs(root2))
		{
			closeIntersection = linePoint + root * lineDirection;
			farIntersection = linePoint + root2 * lineDirection;
		}
		else
		{
			closeIntersection = linePoint + root2 * lineDirection;
			farIntersection = linePoint + root * lineDirection;
		}
		return 2;
	}

	public static int LineFiniteCylinderIntersection(Vector3 linePoint, Vector3 lineDirection, Vector3 cylinderStart, Vector3 cylinderEnd, float cylinderRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		Vector3 normalized = (cylinderEnd - cylinderStart).normalized;
		if (lineDirection.IsWithin01DegFrom(normalized))
		{
			return 0;
		}
		Vector3 vector = normalized;
		Vector3 closestPointLine = Vector3.Cross(lineDirection, vector);
		Vector3 normalized2 = closestPointLine.normalized;
		Vector3 rhs = Vector3.Cross(vector, normalized2);
		float num = Vector3.Dot(linePoint - cylinderStart, normalized2);
		float num2 = Vector3.Dot(lineDirection, rhs);
		float a = num2 * num2;
		float b = 0f;
		float c = num * num - cylinderRadius * cylinderRadius;
		float root;
		float root2;
		int num3 = BMath.SolveQuadratic(a, b, c, out root, out root2);
		if (num3 == 0)
		{
			return 0;
		}
		ClosestPointsOnTwoLines(linePoint, lineDirection, cylinderStart, normalized, out var closestPointLine2, out closestPointLine);
		float num4 = BMath.Sqrt((closestPointLine2 - linePoint).sqrMagnitude / lineDirection.sqrMagnitude);
		root += num4;
		root2 += num4;
		Vector3 vector2 = linePoint + root * lineDirection;
		bool flag = vector2.IsAboveOrOnPlane(cylinderStart, normalized) && vector2.IsAboveOrOnPlane(cylinderEnd, -normalized);
		if (num3 == 1)
		{
			if (flag)
			{
				closeIntersection = vector2;
				return 1;
			}
			return 0;
		}
		Vector3 vector3 = linePoint + root2 * lineDirection;
		bool flag2 = vector3.IsAboveOrOnPlane(cylinderStart, normalized) && vector3.IsAboveOrOnPlane(cylinderEnd, -normalized);
		if (flag && flag2)
		{
			if (BMath.Abs(root) <= BMath.Abs(root2))
			{
				closeIntersection = vector2;
				farIntersection = vector3;
			}
			else
			{
				closeIntersection = vector3;
				farIntersection = vector2;
			}
			return 2;
		}
		if (flag)
		{
			closeIntersection = vector2;
			return 1;
		}
		if (flag2)
		{
			closeIntersection = vector3;
			return 1;
		}
		return 0;
	}

	public static int RayCylinderIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 cylinderPoint, Vector3 cylinderDirection, float cylinderRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		if (BMath.Abs(cylinderPoint.sqrMagnitude - 1f) > 2E-05f)
		{
			cylinderPoint.Normalize();
		}
		if (rayDirection.IsWithin01DegFrom(cylinderDirection))
		{
			return 0;
		}
		Vector3 closestPointLine = Vector3.Cross(rayDirection, cylinderDirection);
		Vector3 normalized = closestPointLine.normalized;
		Vector3 rhs = Vector3.Cross(cylinderDirection, normalized);
		float num = Vector3.Dot(rayOrigin - cylinderPoint, normalized);
		float num2 = Vector3.Dot(rayDirection, rhs);
		float a = num2 * num2;
		float b = 0f;
		float c = num * num - cylinderRadius * cylinderRadius;
		float root;
		float root2;
		int num3 = BMath.SolveQuadratic(a, b, c, out root, out root2);
		if (num3 == 0)
		{
			return 0;
		}
		ClosestPointsOnTwoLines(rayOrigin, rayDirection, cylinderPoint, cylinderDirection, out var closestPointLine2, out closestPointLine);
		float num4 = BMath.Sqrt((closestPointLine2 - rayOrigin).sqrMagnitude / rayDirection.sqrMagnitude);
		root += num4;
		root2 += num4;
		if (num3 == 1)
		{
			if (root >= 0f)
			{
				closeIntersection = rayOrigin + root * rayDirection;
				return 1;
			}
			return 0;
		}
		bool flag = root >= 0f;
		bool flag2 = root2 >= 0f;
		if (flag && flag2)
		{
			if (BMath.Abs(root) <= BMath.Abs(root2))
			{
				closeIntersection = rayOrigin + root * rayDirection;
				farIntersection = rayOrigin + root2 * rayDirection;
			}
			else
			{
				closeIntersection = rayOrigin + root2 * rayDirection;
				farIntersection = rayOrigin + root * rayDirection;
			}
			return 2;
		}
		if (flag)
		{
			closeIntersection = rayOrigin + root * rayDirection;
			return 1;
		}
		if (flag2)
		{
			closeIntersection = rayOrigin + root2 * rayDirection;
			return 1;
		}
		return 0;
	}

	public static int RayFiniteCylinderIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 cylinderStart, Vector3 cylinderEnd, float cylinderRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		Vector3 normalized = (cylinderEnd - cylinderStart).normalized;
		if (rayDirection.IsWithin01DegFrom(normalized))
		{
			return 0;
		}
		Vector3 vector = normalized;
		Vector3 closestPointLine = Vector3.Cross(rayDirection, vector);
		Vector3 normalized2 = closestPointLine.normalized;
		Vector3 rhs = Vector3.Cross(vector, normalized2);
		float num = Vector3.Dot(rayOrigin - cylinderStart, normalized2);
		float num2 = Vector3.Dot(rayDirection, rhs);
		float a = num2 * num2;
		float b = 0f;
		float c = num * num - cylinderRadius * cylinderRadius;
		float root;
		float root2;
		int num3 = BMath.SolveQuadratic(a, b, c, out root, out root2);
		if (num3 == 0)
		{
			return 0;
		}
		ClosestPointsOnTwoLines(rayOrigin, rayDirection, cylinderStart, normalized, out var closestPointLine2, out closestPointLine);
		float num4 = BMath.Sqrt((closestPointLine2 - rayOrigin).sqrMagnitude / rayDirection.sqrMagnitude);
		root += num4;
		root2 += num4;
		Vector3 vector2 = rayOrigin + root * rayDirection;
		bool flag = root >= 0f && vector2.IsAboveOrOnPlane(cylinderStart, normalized) && vector2.IsAboveOrOnPlane(cylinderEnd, -normalized);
		if (num3 == 1)
		{
			if (flag)
			{
				closeIntersection = rayOrigin + root * rayDirection;
				return 1;
			}
			return 0;
		}
		Vector3 vector3 = rayOrigin + root2 * rayDirection;
		bool flag2 = root2 >= 0f && vector3.IsAboveOrOnPlane(cylinderStart, normalized) && vector3.IsAboveOrOnPlane(cylinderEnd, -normalized);
		if (flag && flag2)
		{
			if (BMath.Abs(root) <= BMath.Abs(root2))
			{
				closeIntersection = vector2;
				farIntersection = vector3;
			}
			else
			{
				closeIntersection = vector3;
				farIntersection = vector2;
			}
			return 2;
		}
		if (flag)
		{
			closeIntersection = vector2;
			return 1;
		}
		if (flag2)
		{
			closeIntersection = vector3;
			return 1;
		}
		return 0;
	}

	public static bool CapsulePointOverlap(Vector3 capsuleCenter, Vector3 capsuleAxis, float capsuleRadius, float capsuleHeight, Vector3 point)
	{
		var (capsuleCenter2, capsuleCenter3) = GetCapsuleSphereCenters(capsuleCenter, capsuleAxis, capsuleRadius, capsuleHeight);
		return CapsulePointOverlap(capsuleCenter2, capsuleCenter3, capsuleRadius, point);
	}

	public static bool CapsulePointOverlap(Vector3 capsuleCenter1, Vector3 capsuleCenter2, float capsuleRadius, Vector3 point)
	{
		if (SpherePointOverlap(capsuleCenter1, capsuleRadius, point))
		{
			return true;
		}
		if (SpherePointOverlap(capsuleCenter2, capsuleRadius, point))
		{
			return true;
		}
		Vector3 vector = capsuleCenter2 - capsuleCenter1;
		if (point.IsAbovePlane(capsuleCenter2, vector) || point.IsAbovePlane(capsuleCenter1, -vector))
		{
			return false;
		}
		return point.DistanceFromLine(capsuleCenter1, vector) <= capsuleRadius;
	}

	public static int LineCapsuleIntersection(Vector3 linePoint, Vector3 lineDirection, Vector3 capsuleCenter1, Vector3 capsuleCenter2, float capsuleRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		Vector3 closeIntersection2;
		Vector3 farIntersection2;
		switch (LineFiniteCylinderIntersection(linePoint, lineDirection, capsuleCenter1, capsuleCenter2, capsuleRadius, out closeIntersection2, out farIntersection2))
		{
		case 2:
			closeIntersection = closeIntersection2;
			farIntersection = farIntersection2;
			return 2;
		case 1:
		{
			closeIntersection = closeIntersection2;
			if (LineSphereIntersection(linePoint, lineDirection, capsuleCenter1, capsuleRadius, out var closeIntersection5, out farIntersection2) == 2)
			{
				farIntersection = farIntersection2;
				return 2;
			}
			if (LineSphereIntersection(linePoint, lineDirection, capsuleCenter2, capsuleRadius, out closeIntersection5, out farIntersection2) == 2)
			{
				farIntersection = farIntersection2;
				return 2;
			}
			throw new InvalidOperationException("Line interesects capsule's cylinder section at one point, but doesn't exit through either sphere. This should never happen.");
		}
		default:
		{
			Vector3 closeIntersection3;
			Vector3 farIntersection3;
			int num = LineSphereIntersection(linePoint, lineDirection, capsuleCenter1, capsuleRadius, out closeIntersection3, out farIntersection3);
			Vector3 closeIntersection4;
			Vector3 farIntersection4;
			int num2 = LineSphereIntersection(linePoint, lineDirection, capsuleCenter2, capsuleRadius, out closeIntersection4, out farIntersection4);
			switch (num)
			{
			case 2:
				if (num2 == 2)
				{
					if ((closeIntersection3 - linePoint).sqrMagnitude <= (closeIntersection4 - linePoint).sqrMagnitude)
					{
						closeIntersection = closeIntersection3;
						farIntersection = farIntersection4;
					}
					else
					{
						closeIntersection = closeIntersection4;
						farIntersection = farIntersection3;
					}
					return 2;
				}
				closeIntersection = closeIntersection3;
				farIntersection = farIntersection3;
				return 2;
			case 1:
				switch (num2)
				{
				case 1:
					if ((closeIntersection3 - linePoint).sqrMagnitude <= (closeIntersection4 - linePoint).sqrMagnitude)
					{
						closeIntersection = closeIntersection3;
					}
					else
					{
						closeIntersection = closeIntersection4;
					}
					return 1;
				case 0:
					closeIntersection = closeIntersection3;
					return 1;
				default:
					throw new InvalidOperationException("Line is tangential to capsule's sphere 1 and doesn't interesect its cylinder section, but passes through sphere 2. This should never happen.");
				}
			default:
				switch (num2)
				{
				case 2:
					closeIntersection = closeIntersection4;
					farIntersection = farIntersection4;
					return 2;
				case 1:
					closeIntersection = closeIntersection4;
					return 1;
				default:
					return 0;
				}
			}
		}
		}
	}

	public static int RayCapsuleIntersection(Vector3 rayOrigin, Vector3 rayDirection, Vector3 capsuleCenter1, Vector3 capsuleCenter2, float capsuleRadius, out Vector3 closeIntersection, out Vector3 farIntersection)
	{
		closeIntersection = default(Vector3);
		farIntersection = default(Vector3);
		Vector3 closeIntersection2;
		Vector3 farIntersection2;
		int num = RayFiniteCylinderIntersection(rayOrigin, rayDirection, capsuleCenter1, capsuleCenter2, capsuleRadius, out closeIntersection2, out farIntersection2);
		if (num == 2)
		{
			closeIntersection = closeIntersection2;
			farIntersection = farIntersection2;
			return 2;
		}
		Vector3 vector = capsuleCenter2 - capsuleCenter1;
		Vector3 closeIntersection3;
		Vector3 farIntersection3;
		int num2 = RayHemisphereIntersection(rayOrigin, rayDirection, capsuleCenter1, capsuleRadius, -vector, out closeIntersection3, out farIntersection3);
		Vector3 closeIntersection4;
		Vector3 farIntersection4;
		int num3 = RayHemisphereIntersection(rayOrigin, rayDirection, capsuleCenter2, capsuleRadius, vector, out closeIntersection4, out farIntersection4);
		if (num == 1)
		{
			if (num2 == 1)
			{
				if ((closeIntersection2 - rayOrigin).sqrMagnitude < (closeIntersection3 - rayOrigin).sqrMagnitude)
				{
					closeIntersection = closeIntersection2;
					farIntersection = closeIntersection3;
				}
				else
				{
					closeIntersection = closeIntersection3;
					farIntersection = closeIntersection2;
				}
				return 2;
			}
			if (num3 == 1)
			{
				if ((closeIntersection2 - rayOrigin).sqrMagnitude < (closeIntersection4 - rayOrigin).sqrMagnitude)
				{
					closeIntersection = closeIntersection2;
					farIntersection = closeIntersection4;
				}
				else
				{
					closeIntersection = closeIntersection4;
					farIntersection = closeIntersection2;
				}
				return 2;
			}
			if (num2 == 0 && num3 == 0)
			{
				closeIntersection = closeIntersection2;
				return 1;
			}
			throw new InvalidOperationException("Found more than two ray-capsule intersections. This should never happen.");
		}
		if (num2 == 2)
		{
			if (num3 == 0)
			{
				closeIntersection = closeIntersection3;
				farIntersection = farIntersection3;
				return 2;
			}
			throw new InvalidOperationException("Found more than two ray-capsule intersections. This should never happen.");
		}
		if (num3 == 2)
		{
			if (num2 == 0)
			{
				closeIntersection = closeIntersection4;
				farIntersection = farIntersection4;
				return 2;
			}
			throw new InvalidOperationException("Found more than two ray-capsule intersections. This should never happen.");
		}
		if (num2 == 1)
		{
			switch (num3)
			{
			case 0:
				closeIntersection = closeIntersection3;
				return 1;
			case 1:
				if ((closeIntersection3 - rayOrigin).sqrMagnitude < (closeIntersection4 - rayOrigin).sqrMagnitude)
				{
					closeIntersection = closeIntersection3;
					farIntersection = closeIntersection4;
				}
				else
				{
					closeIntersection = closeIntersection4;
					farIntersection = closeIntersection3;
				}
				return 2;
			default:
				throw new InvalidOperationException("Found more than two ray-capsule intersections. This should never happen.");
			}
		}
		switch (num3)
		{
		case 2:
			closeIntersection = closeIntersection4;
			farIntersection = farIntersection4;
			return 2;
		case 1:
			closeIntersection = closeIntersection4;
			return 1;
		default:
			return 0;
		}
	}

	public static float GetFrustumDistanceFromHeight(float fieldOfView, float height)
	{
		return height * 0.5f / BMath.Tan(fieldOfView * 0.5f * (MathF.PI / 180f));
	}

	public static float GetFrustumDistanceFromWidth(float fieldOfView, float aspectRatio, float width)
	{
		return GetFrustumDistanceFromHeight(fieldOfView, GetFrustumHeightFromWidth(aspectRatio, width));
	}

	public static float GetFrustumHeightFromDistance(float fieldOfView, float distance)
	{
		return 2f * distance * BMath.Tan(fieldOfView * 0.5f * (MathF.PI / 180f));
	}

	public static float GetFrustumWidthFromDistance(float fieldOfView, float aspectRatio, float distance)
	{
		return GetFrustumWidthFromHeight(aspectRatio, GetFrustumHeightFromDistance(fieldOfView, distance));
	}

	public static float GetFrustumWidthFromHeight(float aspectRatio, float height)
	{
		return height * aspectRatio;
	}

	public static float GetFrustumHeightFromWidth(float aspectRatio, float width)
	{
		return width / aspectRatio;
	}

	public static bool IsPointInPlanesVolume(Plane[] planes, Vector3 point)
	{
		foreach (Plane plane in planes)
		{
			if (!plane.GetSide(point))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsBoxInPlanesVolume(Plane[] planes, Box box)
	{
		for (int i = 0; i < planes.Length; i++)
		{
			Plane plane = planes[i];
			bool flag = false;
			foreach (Vector3 corner in box.GetCorners())
			{
				if (plane.GetSide(corner))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsBoundsInPlanesVolume(Plane[] planes, Bounds bounds)
	{
		for (int i = 0; i < planes.Length; i++)
		{
			Plane plane = planes[i];
			bool flag = false;
			foreach (Vector3 localCorner in bounds.GetLocalCorners())
			{
				if (plane.GetSide(localCorner))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public static Vector3 ReflectPoint(Vector3 point, Vector3 reflectionPlanePoint, Vector3 reflectionPlaneNormal)
	{
		return reflectionPlanePoint + ReflectVector(point - reflectionPlanePoint, reflectionPlaneNormal);
	}

	public static Vector3 ReflectVector(Vector3 vector, Vector3 reflectionPlaneNormal)
	{
		if (BMath.Abs(reflectionPlaneNormal.sqrMagnitude - 1f) > 1E-05f)
		{
			reflectionPlaneNormal.Normalize();
		}
		float num = Vector3.Dot(vector, reflectionPlaneNormal);
		return vector - 2f * num * reflectionPlaneNormal;
	}

	public static bool DepenetrateSphereFromPlane(Vector3 sphereCenter, float sphereRadius, Vector3 planePoint, Vector3 planeNormal, out float depenetrationDistance)
	{
		depenetrationDistance = Vector3.Dot(sphereCenter - planePoint, -planeNormal) + sphereRadius;
		return depenetrationDistance > 0f;
	}

	public static bool DepenetrateCapsuleFromPlane(Vector3 capsuleCenter, Vector3 capsuleAxis, float capsuleRadius, float capsuleHeight, Vector3 planePoint, Vector3 planeNormal, out float depenetrationDistance)
	{
		var (sphere1Center, sphere2Center) = GetCapsuleSphereCenters(capsuleCenter, capsuleAxis, capsuleRadius, capsuleHeight);
		return DepenetrateCapsuleFromPlane(sphere1Center, sphere2Center, capsuleRadius, planePoint, planeNormal, out depenetrationDistance);
	}

	public static bool DepenetrateCapsuleFromPlane(Vector3 sphere1Center, Vector3 sphere2Center, float capsuleRadius, Vector3 planePoint, Vector3 planeNormal, out float depenetrationDistance)
	{
		DepenetrateSphereFromPlane(sphere1Center, capsuleRadius, planePoint, planeNormal, out var depenetrationDistance2);
		DepenetrateSphereFromPlane(sphere2Center, capsuleRadius, planePoint, planeNormal, out var depenetrationDistance3);
		depenetrationDistance = BMath.Max(depenetrationDistance2, depenetrationDistance3);
		return depenetrationDistance > 0f;
	}

	public static (Vector3 centerAlongAxis, Vector3 centerOppositeAxis) GetCapsuleSphereCenters(Vector3 center, Vector3 axis, float radius, float height)
	{
		Vector3 vector = (height / 2f - radius) * axis;
		return (centerAlongAxis: center + vector, centerOppositeAxis: center - vector);
	}

	public static Vector3 GetCapsuleNormalTowards(Vector3 point, Vector3 capsuleCenter1, Vector3 capsuleCenter2, float capsuleRadius)
	{
		Vector3 vector = capsuleCenter2 - capsuleCenter1;
		if (point.IsAboveOrOnPlane(capsuleCenter2, vector))
		{
			return (point - capsuleCenter2).normalized;
		}
		if (point.IsAboveOrOnPlane(capsuleCenter1, -vector))
		{
			return (point - capsuleCenter1).normalized;
		}
		return Vector3.Cross(Vector3.Cross(vector, point - capsuleCenter1), vector).normalized;
	}
}
