using System;
using System.Globalization;
using System.Linq;
using Brimstone.Geometry;
using Unity.Mathematics;
using UnityEngine;

public static class VectorExtensions
{
	public static float Min(this Vector2 vector)
	{
		return BMath.Min(vector.x, vector.y);
	}

	public static float Min(this Vector2 vector, out int index)
	{
		return BMath.Min(vector.x, vector.y, out index);
	}

	public static float Max(this Vector2 vector)
	{
		return BMath.Max(vector.x, vector.y);
	}

	public static float Max(this Vector2 vector, out int index)
	{
		return BMath.Max(vector.x, vector.y, out index);
	}

	public static float Min(this Vector3 vector)
	{
		return BMath.Min(vector.x, vector.y, vector.z);
	}

	public static float Min(this Vector3 vector, out int index)
	{
		return BMath.Min(vector.x, vector.y, vector.z, out index);
	}

	public static float Max(this Vector3 vector)
	{
		return BMath.Max(vector.x, vector.y, vector.z);
	}

	public static float Max(this Vector3 vector, out int index)
	{
		return BMath.Max(vector.x, vector.y, vector.z, out index);
	}

	public static Vector2 Ceil(this Vector2 v)
	{
		return new Vector2(BMath.Ceil(v.x), BMath.Ceil(v.y));
	}

	public static Vector2 Floor(this Vector2 v)
	{
		return new Vector2(BMath.Floor(v.x), BMath.Floor(v.y));
	}

	public static Vector2 Round(this Vector2 v)
	{
		return new Vector2(BMath.Round(v.x), BMath.Round(v.y));
	}

	public static Vector2Int CeilToInt(this Vector2Int v)
	{
		return new Vector2Int(BMath.CeilToInt(v.x), BMath.CeilToInt(v.y));
	}

	public static Vector2Int FloorToInt(this Vector2Int v)
	{
		return new Vector2Int(BMath.FloorToInt(v.x), BMath.FloorToInt(v.y));
	}

	public static Vector2Int RoundToInt(this Vector2Int v)
	{
		return new Vector2Int(BMath.RoundToInt(v.x), BMath.RoundToInt(v.y));
	}

	public static Vector3 Ceil(this Vector3 v)
	{
		return new Vector3(BMath.Ceil(v.x), BMath.Ceil(v.y), BMath.Ceil(v.z));
	}

	public static Vector3 Floor(this Vector3 v)
	{
		return new Vector3(BMath.Floor(v.x), BMath.Floor(v.y), BMath.Floor(v.z));
	}

	public static Vector3 Round(this Vector3 v)
	{
		return new Vector3(BMath.Round(v.x), BMath.Round(v.y), BMath.Round(v.z));
	}

	public static Vector3Int CeilToInt(this Vector3Int v)
	{
		return new Vector3Int(BMath.CeilToInt(v.x), BMath.CeilToInt(v.y), BMath.CeilToInt(v.z));
	}

	public static Vector3Int FloorToInt(this Vector3Int v)
	{
		return new Vector3Int(BMath.FloorToInt(v.x), BMath.FloorToInt(v.y), BMath.FloorToInt(v.z));
	}

	public static Vector3Int RoundToInt(this Vector3Int v)
	{
		return new Vector3Int(BMath.RoundToInt(v.x), BMath.RoundToInt(v.y), BMath.RoundToInt(v.z));
	}

	public static Vector2 MultiplyElements(this Vector2 a, Vector2 b)
	{
		return new Vector2(a.x * b.x, a.y * b.y);
	}

	public static Vector2 MultiplyElements(this Vector2Int a, Vector2 b)
	{
		return new Vector2((float)a.x * b.x, (float)a.y * b.y);
	}

	public static Vector2 MultiplyElements(this Vector2 a, Vector2Int b)
	{
		return new Vector2(a.x * (float)b.x, a.y * (float)b.y);
	}

	public static Vector2Int MultiplyElements(this Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x * b.x, a.y * b.y);
	}

	public static Vector3 MultiplyElements(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector3 MultiplyElements(this Vector3Int a, Vector3 b)
	{
		return new Vector3((float)a.x * b.x, (float)a.y * b.y, (float)a.z * b.z);
	}

	public static Vector3 MultiplyElements(this Vector3 a, Vector3Int b)
	{
		return new Vector3(a.x * (float)b.x, a.y * (float)b.y, a.z * (float)b.z);
	}

	public static Vector3Int MultiplyElements(this Vector3Int a, Vector3Int b)
	{
		return new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	public static Vector4 MultiplyElements(this Vector4 a, Vector4 b)
	{
		return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
	}

	public static bool Approximately(this Vector2 vector, Vector2 otherVector, float tolerance = 0.001f)
	{
		return (vector - otherVector).sqrMagnitude <= tolerance * tolerance;
	}

	public static bool Approximately(this Vector3 vector, Vector3 otherVector, float tolerance = 0.001f)
	{
		return (vector - otherVector).sqrMagnitude <= tolerance * tolerance;
	}

	public static float Average(this Vector2 vector)
	{
		return BMath.Average(vector.x, vector.y);
	}

	public static float Average(this Vector3 vector)
	{
		return BMath.Average(vector.x, vector.y, vector.z);
	}

	public static Vector2 SwapElements(this Vector2 vector)
	{
		return new Vector2(vector.y, vector.x);
	}

	public static Vector3 ApproxTangent(Vector3 n)
	{
		Vector3 vector = Vector3.Cross(n, Vector3.up);
		Vector3 vector2 = Vector3.Cross(n, Vector3.forward);
		if (vector.magnitude > vector2.magnitude)
		{
			return vector.normalized;
		}
		return vector2.normalized;
	}

	public static Vector3 NearestAxis(this Vector3 direction)
	{
		if (direction == Vector3.zero)
		{
			return Vector3.zero;
		}
		float num = BMath.Abs(direction.x);
		float num2 = BMath.Abs(direction.y);
		float num3 = BMath.Abs(direction.z);
		if (num >= num2 && num >= num3)
		{
			return new Vector3(BMath.Sign(direction.x), 0f, 0f);
		}
		if (num2 >= num && num2 >= num3)
		{
			return new Vector3(0f, BMath.Sign(direction.y), 0f);
		}
		return new Vector3(0f, 0f, BMath.Sign(direction.z));
	}

	public static Vector3 NearestAxisPreferY(this Vector3 direction)
	{
		if (direction == Vector3.zero)
		{
			return Vector3.zero;
		}
		float num = BMath.Abs(direction.x);
		float num2 = BMath.Abs(direction.y);
		float num3 = BMath.Abs(direction.z);
		if (num2 >= num && num2 >= num3)
		{
			return new Vector3(0f, BMath.Sign(direction.y), 0f);
		}
		if (num >= num2 && num >= num3)
		{
			return new Vector3(BMath.Sign(direction.x), 0f, 0f);
		}
		return new Vector3(0f, 0f, BMath.Sign(direction.z));
	}

	public static Vector3Int NearestAxisToInt(this Vector3 direction)
	{
		if (direction == Vector3Int.zero)
		{
			return Vector3Int.zero;
		}
		float num = BMath.Abs(direction.x);
		float num2 = BMath.Abs(direction.y);
		float num3 = BMath.Abs(direction.z);
		if (num >= num2 && num >= num3)
		{
			return new Vector3Int(BMath.Sign(direction.x), 0, 0);
		}
		if (num2 >= num && num2 >= num3)
		{
			return new Vector3Int(0, BMath.Sign(direction.y), 0);
		}
		return new Vector3Int(0, 0, BMath.Sign(direction.z));
	}

	public static Vector3 NearestHorizontalAxis(this Vector3 direction)
	{
		if (direction.x == 0f && direction.z == 0f)
		{
			return Vector3.zero;
		}
		float num = BMath.Abs(direction.x);
		float num2 = BMath.Abs(direction.z);
		if (num >= num2)
		{
			return new Vector3(BMath.Sign(direction.x), 0f, 0f);
		}
		return new Vector3(0f, 0f, BMath.Sign(direction.z));
	}

	public static float AngleProjected(Vector3 a, Vector3 b, Vector3 normal)
	{
		return BMath.Atan2(Vector3.Dot(normal, Vector3.Cross(a, b)), Vector3.Dot(a, b)) * (180f / MathF.PI);
	}

	public static float[] ToArray(this Vector2 vector)
	{
		return new float[2] { vector.x, vector.y };
	}

	public static float[] ToArray(this Vector3 vector)
	{
		return new float[3] { vector.x, vector.y, vector.z };
	}

	public static float[] ToArray(this Vector4 vector)
	{
		return new float[4] { vector.x, vector.y, vector.z, vector.w };
	}

	public static Vector4 FromArray(params float[] values)
	{
		Vector4 result = default(Vector4);
		if (values.Length != 0)
		{
			result.x = values[0];
		}
		if (values.Length > 1)
		{
			result.y = values[1];
		}
		if (values.Length > 2)
		{
			result.z = values[2];
		}
		if (values.Length > 3)
		{
			result.w = values[3];
		}
		return result;
	}

	public static Vector4 FromString(string val)
	{
		return FromArray((from x in val.Split(',')
			select float.Parse(x, CultureInfo.InvariantCulture)).ToArray());
	}

	public static Vector2 Abs(this Vector2 vector)
	{
		return BMath.Abs(vector);
	}

	public static Vector3 Abs(this Vector3 vector)
	{
		return BMath.Abs(vector);
	}

	public static Vector4 Abs(this Vector4 vector)
	{
		return BMath.Abs(vector);
	}

	public static Vector3Int FloorToInt(this Vector3 vector)
	{
		return new Vector3Int
		{
			x = BMath.FloorToInt(vector.x),
			y = BMath.FloorToInt(vector.y),
			z = BMath.FloorToInt(vector.z)
		};
	}

	public static Vector3Int RoundToInt(this Vector3 vector)
	{
		return new Vector3Int
		{
			x = BMath.RoundToInt(vector.x),
			y = BMath.RoundToInt(vector.y),
			z = BMath.RoundToInt(vector.z)
		};
	}

	public static Vector3Int CeilToInt(this Vector3 vector)
	{
		return new Vector3Int
		{
			x = BMath.CeilToInt(vector.x),
			y = BMath.CeilToInt(vector.y),
			z = BMath.CeilToInt(vector.z)
		};
	}

	public static Vector2Int FloorToInt(this Vector2 vector)
	{
		return new Vector2Int
		{
			x = BMath.FloorToInt(vector.x),
			y = BMath.FloorToInt(vector.y)
		};
	}

	public static Vector2Int RoundToInt(this Vector2 vector)
	{
		return new Vector2Int
		{
			x = BMath.RoundToInt(vector.x),
			y = BMath.RoundToInt(vector.y)
		};
	}

	public static Vector2Int CeilToInt(this Vector2 vector)
	{
		return new Vector2Int
		{
			x = BMath.CeilToInt(vector.x),
			y = BMath.CeilToInt(vector.y)
		};
	}

	public static Vector4 MagnitudeMax(Vector4 vector1, Vector4 vector2)
	{
		if (vector1.sqrMagnitude < vector2.sqrMagnitude)
		{
			return vector2;
		}
		return vector1;
	}

	public static bool IsAbovePlane(this Vector3 point, Vector3 planePoint, Vector3 planeNormal)
	{
		return Vector3.Dot(point - planePoint, planeNormal) > 0f;
	}

	public static bool IsAboveOrOnPlane(this Vector3 point, Vector3 planePoint, Vector3 planeNormal)
	{
		return Vector3.Dot(point - planePoint, planeNormal) >= 0f;
	}

	public static float DistanceFromSegment(this Vector3 point, Line line)
	{
		return point.DistanceFromSegment(line.Start, line.Start - line.End);
	}

	public static float DistanceFromSegment(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 vector = lineEnd - lineStart;
		if (point.IsAboveOrOnPlane(lineStart, -vector))
		{
			return (lineStart - point).magnitude;
		}
		if (point.IsAboveOrOnPlane(lineEnd, vector))
		{
			return (lineEnd - point).magnitude;
		}
		return point.DistanceFromLine(lineStart, vector);
	}

	public static float SqrDistanceFromSegment(this Vector3 point, Line line)
	{
		return point.SqrDistanceFromSegment(line.Start, line.Start - line.End);
	}

	public static float SqrDistanceFromSegment(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 vector = lineEnd - lineStart;
		if (point.IsAboveOrOnPlane(lineStart, -vector))
		{
			return (lineStart - point).sqrMagnitude;
		}
		if (point.IsAboveOrOnPlane(lineEnd, vector))
		{
			return (lineEnd - point).sqrMagnitude;
		}
		return point.SqrDistanceFromLine(lineStart, vector);
	}

	public static float DistanceFromLine(this Vector3 point, Vector3 linePoint, Vector3 lineDirection)
	{
		if (BMath.Abs(lineDirection.sqrMagnitude - 1f) > 2E-05f)
		{
			lineDirection.Normalize();
		}
		Vector3 vector = point - linePoint;
		return (vector - Vector3.Dot(vector, lineDirection) * lineDirection).magnitude;
	}

	public static float SqrDistanceFromLine(this Vector3 point, Vector3 linePoint, Vector3 lineDirection)
	{
		if (BMath.Abs(lineDirection.sqrMagnitude - 1f) > 2E-05f)
		{
			lineDirection.Normalize();
		}
		Vector3 vector = point - linePoint;
		return (vector - Vector3.Dot(vector, lineDirection) * lineDirection).sqrMagnitude;
	}

	public static float DistanceFromLineThroughOrigin(this Vector3 point, Vector3 lineDirection)
	{
		if (BMath.Abs(lineDirection.sqrMagnitude - 1f) > 2E-05f)
		{
			lineDirection.Normalize();
		}
		return (point - Vector3.Dot(point, lineDirection) * lineDirection).magnitude;
	}

	public static float SqrDistanceFromLineThroughOrigin(this Vector3 point, Vector3 lineDirection)
	{
		if (BMath.Abs(lineDirection.sqrMagnitude - 1f) > 2E-05f)
		{
			lineDirection.Normalize();
		}
		return (point - Vector3.Dot(point, lineDirection) * lineDirection).sqrMagnitude;
	}

	public static float DistanceFromPlane(this Vector3 point, Vector3 planePoint, Vector3 planeNormal)
	{
		if (BMath.Abs(planeNormal.sqrMagnitude - 1f) > 2E-05f)
		{
			planeNormal.Normalize();
		}
		return Vector3.Dot(point - planePoint, planeNormal);
	}

	public static Vector3 ClosestPointOnSegment(this Vector3 point, Line line)
	{
		return point.ClosestPointOnSegment(line.Start, line.End);
	}

	public static Vector3 ClosestPointOnSegment(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
	{
		Vector3 vector = lineEnd - lineStart;
		if (point.IsAboveOrOnPlane(lineStart, -vector))
		{
			return lineStart;
		}
		if (point.IsAboveOrOnPlane(lineEnd, vector))
		{
			return lineEnd;
		}
		return point.ClosestPointOnLine(lineStart, vector);
	}

	public static Vector3 ClosestPointOnLine(this Vector3 point, Vector3 linePoint, Vector3 lineDirection)
	{
		if (BMath.Abs(lineDirection.sqrMagnitude - 1f) > 2E-05f)
		{
			lineDirection.Normalize();
		}
		return linePoint + Vector3.Dot(point - linePoint, lineDirection) * lineDirection;
	}

	public static Vector3 ClosestPointOnTriangle(this Vector3 point, Vector3 trianglePoint0, Vector3 trianglePoint1, Vector3 trianglePoint2, out Vector3 triangleNormal)
	{
		return BGeo.ClosestPointOnTriangle(point, trianglePoint0, trianglePoint1, trianglePoint2, out triangleNormal);
	}

	public static bool IsOnLine(this Vector3 point, Vector3 linePoint, Vector3 lineDirection, float tolerance = 1E-05f)
	{
		return point.DistanceFromLine(linePoint, lineDirection) <= tolerance;
	}

	public static Vector3 GetPointVelocity(this Vector3 worldPoint, Vector3 objectWorldCenterOfMass, Vector3 objectVelocity, Vector3 objectAngularVelocity)
	{
		Vector3 rhs = worldPoint - worldPoint.ClosestPointOnLine(objectWorldCenterOfMass, objectAngularVelocity);
		Vector3 vector = Vector3.Cross(objectAngularVelocity, rhs);
		return objectVelocity + vector;
	}

	public static Vector3 Horizontalized(this Vector3 vector)
	{
		vector.y = 0f;
		return vector;
	}

	public static Vector3 Verticalized(this Vector3 vector)
	{
		vector.x = 0f;
		vector.z = 0f;
		return vector;
	}

	public static Vector3 AsHorizontal3(this Vector2 vector)
	{
		return new Vector3(vector.x, 0f, vector.y);
	}

	public static Vector2 AsHorizontal2(this Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}

	public static Vector3Int AsHorizontal3Int(this Vector2Int vector)
	{
		return new Vector3Int(vector.x, 0, vector.y);
	}

	public static Vector2Int AsHorizontal2Int(this Vector3Int vector)
	{
		return new Vector2Int(vector.x, vector.z);
	}

	public static Vector3 AsHorizontal3(this Vector2Int vector)
	{
		return new Vector3(vector.x, 0f, vector.y);
	}

	public static Vector2 AsHorizontal2(this Vector3Int vector)
	{
		return new Vector2(vector.x, vector.z);
	}

	public static Vector3 GetPerpendicular(this Vector3 vector)
	{
		if (vector == Vector3.zero)
		{
			return Vector3.zero;
		}
		if (vector.x != 0f)
		{
			return new Vector3((0f - (vector.y + vector.z)) / vector.x, 1f, 1f);
		}
		if (vector.y != 0f)
		{
			return new Vector3(1f, (0f - (vector.x + vector.z)) / vector.y, 1f);
		}
		return new Vector3(1f, 1f, (0f - (vector.x + vector.y)) / vector.z);
	}

	public static Vector3 ProjectPointOnLine(this Vector3 point, Vector3 linePoint, Vector3 lineDirection)
	{
		if (BMath.Abs(lineDirection.sqrMagnitude - 1f) > 2E-05f)
		{
			lineDirection.Normalize();
		}
		return linePoint + Vector3.Dot(point - linePoint, lineDirection) * lineDirection;
	}

	public static Vector3 ProjectPointOnSphere(this Vector3 point, Vector3 sphereCenter, float sphereRadius)
	{
		return sphereCenter + (point - sphereCenter).normalized * sphereRadius;
	}

	public static Vector3 ProjectOnPlaneKeepMagnitude(this Vector3 vector, Vector3 planeNormal)
	{
		return Vector3.ProjectOnPlane(vector, planeNormal).normalized * vector.magnitude;
	}

	public static bool TryProjectOnPlaneAlong(this Vector3 vector, Vector3 alongDirection, Vector3 planeNormal, out Vector3 projection)
	{
		return BGeo.LinePlaneIntersection(vector, alongDirection, Vector3.zero, planeNormal, out projection);
	}

	public static bool TryProjectOnPlaneAlongKeepMagnitude(this Vector3 vector, Vector3 alongDirection, Vector3 planeNormal, out Vector3 projection)
	{
		bool result = vector.TryProjectOnPlaneAlong(alongDirection, planeNormal, out projection);
		projection = projection.normalized * vector.magnitude;
		return result;
	}

	public static Vector2 RandomlyRotatedDeg(this Vector2 vector, float maxAngle)
	{
		float angle = UnityEngine.Random.Range(0f - maxAngle, maxAngle);
		float num = BMath.SinDeg(angle);
		float num2 = BMath.CosDeg(angle);
		return new Vector2(vector.x * num2 - vector.y * num, vector.x * num + vector.y * num2);
	}

	public static Vector2 RandomlyRotatedRad(this Vector2 vector, float maxAngle)
	{
		return vector.RandomlyRotatedDeg(maxAngle * (180f / MathF.PI));
	}

	public static Vector3 RandomlyRotatedDeg(this Vector3 vector, float maxAngle)
	{
		Vector3 perpendicular = vector.GetPerpendicular();
		perpendicular = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), vector) * perpendicular;
		return Quaternion.AngleAxis(UnityEngine.Random.Range(0f, maxAngle), perpendicular) * vector;
	}

	public static Vector3 RandomlyRotatedRad(this Vector3 vector, float maxAngle)
	{
		return vector.RandomlyRotatedDeg(maxAngle * (180f / MathF.PI));
	}

	public static float GetAngleRad(this Vector2 vector)
	{
		return BMath.Atan2(vector.y, vector.x);
	}

	public static float GetAngleRad(this float2 vector)
	{
		return BMath.Atan2(vector.y, vector.x);
	}

	public static float GetAngleDeg(this Vector2 vector)
	{
		return BMath.Atan2Deg(vector.y, vector.x);
	}

	public static float GetAngleDeg(this float2 vector)
	{
		return BMath.Atan2Deg(vector.y, vector.x);
	}

	public static float GetYawRad(this Vector3 vector)
	{
		return BMath.Atan2(vector.x, vector.z);
	}

	public static float GetYawRad(this float3 vector)
	{
		return math.atan2(vector.x, vector.z);
	}

	public static float GetYawDeg(this Vector3 vector)
	{
		return BMath.Atan2Deg(vector.x, vector.z);
	}

	public static float GetPitchRad(this Vector3 vector)
	{
		return BMath.Atan2(0f - vector.y, BMath.Sqrt(vector.x * vector.x + vector.z * vector.z));
	}

	public static float GetPitchRad(this float3 vector)
	{
		return math.atan2(0f - vector.y, BMath.Sqrt(vector.x * vector.x + vector.z * vector.z));
	}

	public static float GetPitchTangent(this Vector3 vector)
	{
		return (0f - vector.y) / BMath.Sqrt(vector.x * vector.x + vector.z * vector.z);
	}

	public static float GetPitchTangent(this float3 vector)
	{
		return (0f - vector.y) / BMath.Sqrt(vector.x * vector.x + vector.z * vector.z);
	}

	public static float GetPitchDeg(this Vector3 vector)
	{
		return BMath.Atan2Deg(0f - vector.y, BMath.Sqrt(vector.x * vector.x + vector.z * vector.z));
	}

	public static Vector3 WrapAngleDeg(this Vector3 vector)
	{
		return new Vector3(vector.x.WrapAngleDeg(), vector.y.WrapAngleDeg(), vector.z.WrapAngleDeg());
	}

	public static Vector3 WrapAngleRad(this Vector3 vector)
	{
		return new Vector3(vector.x.WrapAngleRad(), vector.y.WrapAngleRad(), vector.z.WrapAngleRad());
	}

	public static bool IsWithin01DegFrom(this Vector2 vector, Vector2 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector2.Dot(vector, fromVector) >= 0.99999845f;
	}

	public static bool IsWithin1DegFrom(this Vector2 vector, Vector2 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector2.Dot(vector, fromVector) >= 0.9998477f;
	}

	public static bool IsWithin5DegFrom(this Vector2 vector, Vector2 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector2.Dot(vector, fromVector) >= 0.9961947f;
	}

	public static bool IsWithin10DegFrom(this Vector2 vector, Vector2 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector2.Dot(vector, fromVector) >= 0.9848077f;
	}

	public static bool IsWithin15DegFrom(this Vector2 vector, Vector2 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector2.Dot(vector, fromVector) >= 0.9659258f;
	}

	public static bool IsWithin01DegFrom(this Vector3 vector, Vector3 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector3.Dot(vector, fromVector) >= 0.99999845f;
	}

	public static bool IsWithin1DegFrom(this Vector3 vector, Vector3 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector3.Dot(vector, fromVector) >= 0.9998477f;
	}

	public static bool IsWithin5DegFrom(this Vector3 vector, Vector3 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector3.Dot(vector, fromVector) >= 0.9961947f;
	}

	public static bool IsWithin10DegFrom(this Vector3 vector, Vector3 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector3.Dot(vector, fromVector) >= 0.9848077f;
	}

	public static bool IsWithin15DegFrom(this Vector3 vector, Vector3 fromVector)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector3.Dot(vector, fromVector) >= 0.9659258f;
	}

	public static bool IsWithinConeAroundDeg(this Vector3 vector, Vector3 fromVector, float apexHalfAngle)
	{
		if (BMath.Abs(vector.sqrMagnitude - 1f) > 2E-05f)
		{
			vector.Normalize();
		}
		if (BMath.Abs(fromVector.sqrMagnitude - 1f) > 2E-05f)
		{
			fromVector.Normalize();
		}
		return Vector3.Dot(vector, fromVector) >= BMath.CosDeg(apexHalfAngle);
	}

	public static Vector3 ProjectOnPlane(this Vector3 vector, Vector3 planeNormal)
	{
		return Vector3.ProjectOnPlane(vector, planeNormal);
	}

	public static Vector3 ProjectPointOnPlane(this Vector3 point, Vector3 planePoint, Vector3 planeNormal)
	{
		return point - Vector3.Dot(point - planePoint, planeNormal) * planeNormal;
	}

	public static bool TryProjectPointOnPlaneAlong(this Vector3 vector, Vector3 alongDirection, Vector3 planePoint, Vector3 planeNormal, out Vector3 projection)
	{
		return BGeo.LinePlaneIntersection(vector, alongDirection, planePoint, planeNormal, out projection);
	}

	public static float SignedDistanceFromPlane(this Vector3 point, Vector3 planePoint, Vector3 planeNormal)
	{
		if (BMath.Abs(planeNormal.sqrMagnitude - 1f) > 2E-05f)
		{
			planeNormal.Normalize();
		}
		return Vector3.Dot(point - planePoint, planeNormal);
	}

	public static float InverseLerp(Vector3 a, Vector3 b, Vector3 p)
	{
		Vector3 vector = b - a;
		return Vector3.Dot(p - a, vector) / Vector3.Dot(vector, vector);
	}

	public static bool IsAnyInfinity(this Vector3 vector)
	{
		if (!float.IsInfinity(vector.x) && !float.IsInfinity(vector.y))
		{
			return float.IsInfinity(vector.z);
		}
		return true;
	}

	public static bool IsAnyPositiveInfinity(this Vector3 vector)
	{
		if (!float.IsPositiveInfinity(vector.x) && !float.IsPositiveInfinity(vector.y))
		{
			return float.IsPositiveInfinity(vector.z);
		}
		return true;
	}

	public static bool IsAnyNegativeInfinity(this Vector3 vector)
	{
		if (!float.IsNegativeInfinity(vector.x) && !float.IsNegativeInfinity(vector.y))
		{
			return float.IsNegativeInfinity(vector.z);
		}
		return true;
	}

	public static bool IsAnyNaN(this Vector3 vector)
	{
		if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y))
		{
			return float.IsNaN(vector.z);
		}
		return true;
	}

	public static bool IsAnyPositive(this Vector3 vector)
	{
		if (!(vector.x > 0f) && !(vector.y > 0f))
		{
			return vector.z > 0f;
		}
		return true;
	}

	public static bool IsAnyNegative(this Vector3 vector)
	{
		if (!(vector.x < 0f) && !(vector.y < 0f))
		{
			return vector.z < 0f;
		}
		return true;
	}

	public static bool IsAnyNonPositive(this Vector3 vector)
	{
		if (!(vector.x <= 0f) && !(vector.y <= 0f))
		{
			return vector.z <= 0f;
		}
		return true;
	}

	public static bool IsAnyNonNegative(this Vector3 vector)
	{
		if (!(vector.x <= 0f) && !(vector.y <= 0f))
		{
			return vector.z <= 0f;
		}
		return true;
	}
}
