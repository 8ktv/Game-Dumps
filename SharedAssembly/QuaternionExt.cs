using UnityEngine;

public static class QuaternionExt
{
	public static float Magnitude(this Quaternion quaternion)
	{
		return BMath.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
	}

	public static float SqrMagnitude(this Quaternion quaternion)
	{
		return quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w;
	}

	public static Quaternion DeltaRotation(Quaternion from, Quaternion to)
	{
		return Quaternion.Inverse(to) * from;
	}

	public static Quaternion ClockwiseLerp(Quaternion from, Quaternion to, Vector3 axis, float t)
	{
		Vector3 vector = VectorExtensions.ApproxTangent(axis);
		Vector3 vector2 = from * vector;
		Vector3 to2 = to * vector;
		float b = Vector3.SignedAngle(vector2, to2, axis);
		return Quaternion.AngleAxis(Mathf.LerpAngle(0f, b, t), axis) * from;
	}

	public static Vector4 ToVector4(this Quaternion quaternion)
	{
		return new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
	}
}
