using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineInternal;

public static class BMath
{
	public const float Pi = MathF.PI;

	public const float Tau = MathF.PI * 2f;

	public const float Infinity = float.PositiveInfinity;

	public const float NegativeInfinity = float.NegativeInfinity;

	public const float Deg2Rad = MathF.PI / 180f;

	public const float Rad2Deg = 180f / MathF.PI;

	public const float Sqrt2 = 1.4142135f;

	public const float Sqrt3 = 1.7320508f;

	public const float InvSqrt2 = 0.70710677f;

	public const float InvSqrt3 = 0.57735026f;

	public const float Root5Of2 = 1.1486983f;

	public const float cos01Deg = 0.99999845f;

	public const float cos1Deg = 0.9998477f;

	public const float cos5Deg = 0.9961947f;

	public const float cos10Deg = 0.9848077f;

	public const float cos15Deg = 0.9659258f;

	public const float cos30Deg = 0.8660254f;

	public const float cos45Deg = 0.70710677f;

	public const float cos89Deg = 0.017452406f;

	public static readonly float Epsilon = (MathfInternal.IsFlushToZeroEnabled ? MathfInternal.FloatMinNormal : MathfInternal.FloatMinDenormal);

	public static float MoveTowards(float current, float target, float maxDelta)
	{
		if (Abs(target - current) <= maxDelta)
		{
			return target;
		}
		return current + (float)Sign(target - current) * maxDelta;
	}

	public static int Average(int a, int b)
	{
		return (a + b) / 2;
	}

	public static int Average(int a, int b, int c)
	{
		return (a + b + c) / 3;
	}

	public static int Average(int a, int b, int c, int d)
	{
		return (a + b + c + d) / 4;
	}

	public static int Average(int[] values)
	{
		int num = 0;
		foreach (int num2 in values)
		{
			num += num2;
		}
		return num / values.Length;
	}

	public static int Average(IEnumerable<int> values)
	{
		int num = 0;
		int num2 = 0;
		foreach (int value in values)
		{
			num += value;
			num2++;
		}
		return num / num2;
	}

	public static float Average(float a, float b)
	{
		return (a + b) / 2f;
	}

	public static float Average(float a, float b, float c)
	{
		return (a + b + c) / 3f;
	}

	public static float Average(float a, float b, float c, float d)
	{
		return (a + b + c + d) / 4f;
	}

	public static float Average(float[] values)
	{
		float num = 0f;
		foreach (float num2 in values)
		{
			num += num2;
		}
		return num / (float)values.Length;
	}

	public static float Average(IEnumerable<float> values)
	{
		float num = 0f;
		int num2 = 0;
		foreach (float value in values)
		{
			num += value;
			num2++;
		}
		return num / (float)num2;
	}

	public static Vector2 Average(Vector2 a, Vector2 b)
	{
		return (a + b) / 2f;
	}

	public static Vector2 Average(Vector2 a, Vector2 b, Vector2 c)
	{
		return (a + b + c) / 3f;
	}

	public static Vector2 Average(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
	{
		return (a + b + c + d) / 4f;
	}

	public static Vector2 Average(Vector2[] values)
	{
		Vector2 zero = Vector2.zero;
		foreach (Vector2 vector in values)
		{
			zero += vector;
		}
		return zero / values.Length;
	}

	public static Vector2 Average(IEnumerable<Vector2> values)
	{
		Vector2 zero = Vector2.zero;
		int num = 0;
		foreach (Vector2 value in values)
		{
			zero += value;
			num++;
		}
		return zero / num;
	}

	public static Vector3 Average(Vector3 a, Vector3 b)
	{
		return (a + b) / 2f;
	}

	public static Vector3 Average(Vector3 a, Vector3 b, Vector3 c)
	{
		return (a + b + c) / 3f;
	}

	public static Vector3 Average(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		return (a + b + c + d) / 4f;
	}

	public static Vector3 Average(Vector3[] values)
	{
		Vector3 zero = Vector3.zero;
		foreach (Vector3 vector in values)
		{
			zero += vector;
		}
		return zero / values.Length;
	}

	public static Vector3 Average(IEnumerable<Vector3> values)
	{
		Vector3 zero = Vector3.zero;
		int num = 0;
		foreach (Vector3 value in values)
		{
			zero += value;
			num++;
		}
		return zero / num;
	}

	public static int WeightedAverage(int a, float aWeight, int b, float bWeight)
	{
		return (int)(((float)a * aWeight + (float)b * bWeight) / (aWeight + bWeight));
	}

	public static int WeightedAverage(int a, float aWeight, int b, float bWeight, int c, float cWeight)
	{
		return (int)(((float)a * aWeight + (float)b * bWeight + (float)c * cWeight) / (aWeight + bWeight + cWeight));
	}

	public static int WeightedAverage(int a, float aWeight, int b, float bWeight, int c, float cWeight, int d, float dWeight)
	{
		return (int)(((float)a * aWeight + (float)b * bWeight + (float)c * cWeight + (float)d * dWeight) / (aWeight + bWeight + cWeight + dWeight));
	}

	public static int WeightedAverage(int[] values, float[] weights)
	{
		if (values.Length != weights.Length)
		{
			throw new InvalidOperationException("The same number of values and weights must be specified.");
		}
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < values.Length; i++)
		{
			float num3 = weights[i];
			num += (float)values[i] * num3;
			num2 += num3;
		}
		return (int)(num / num2);
	}

	public static float WeightedAverage(float a, float aWeight, float b, float bWeight)
	{
		return (a * aWeight + b * bWeight) / (aWeight + bWeight);
	}

	public static float WeightedAverage(float a, float aWeight, float b, float bWeight, float c, float cWeight)
	{
		return (a * aWeight + b * bWeight + c * cWeight) / (aWeight + bWeight + cWeight);
	}

	public static float WeightedAverage(float a, float aWeight, float b, float bWeight, float c, float cWeight, float d, float dWeight)
	{
		return (a * aWeight + b * bWeight + c * cWeight + d * dWeight) / (aWeight + bWeight + cWeight + dWeight);
	}

	public static float WeightedAverage(float[] values, float[] weights)
	{
		if (values.Length != weights.Length)
		{
			throw new InvalidOperationException("The same number of values and weights must be specified.");
		}
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < values.Length; i++)
		{
			float num3 = weights[i];
			num += values[i] * num3;
			num2 += num3;
		}
		return num / num2;
	}

	public static float Frac(float value)
	{
		return value % 1f;
	}

	public static float DeltaAngleDeg(float a, float b)
	{
		float num = Wrap(b - a, 360f);
		if (num > 180f)
		{
			num -= 360f;
		}
		return num;
	}

	public static float DeltaAngleRad(float a, float b)
	{
		return Wrap(b - a + MathF.PI, MathF.PI * 2f) - MathF.PI;
	}

	public static float Lerp(float from, float to, float t)
	{
		return from + (to - from) * t;
	}

	public static float LerpClamped(float from, float to, float t)
	{
		return Lerp(from, to, Clamp01(t));
	}

	public static float InverseLerp(float from, float to, float value)
	{
		return (value - from) / (to - from);
	}

	public static float InverseLerpClamped(float from, float to, float value)
	{
		return Clamp01(InverseLerp(from, to, value));
	}

	public static float LerpAngleDeg(float from, float to, float t)
	{
		return (from + (to - from).WrapAngleDeg() * t).WrapAngleDeg();
	}

	public static float LerpAngleRad(float from, float to, float t)
	{
		return (from + (to - from).WrapAngleRad() * t).WrapAngleRad();
	}

	public static Matrix4x4 Lerp(Matrix4x4 from, Matrix4x4 to, float t)
	{
		Matrix4x4 result = default(Matrix4x4);
		for (int i = 0; i < 16; i++)
		{
			result[i] = Lerp(from[i], to[i], t);
		}
		return result;
	}

	public static Matrix4x4 LerpClamped(Matrix4x4 from, Matrix4x4 to, float t)
	{
		Matrix4x4 result = default(Matrix4x4);
		for (int i = 0; i < 16; i++)
		{
			result[i] = Lerp(from[i], to[i], t);
		}
		return result;
	}

	public static int Clamp(int value, int min, int max)
	{
		if (value <= min)
		{
			return min;
		}
		if (value >= max)
		{
			return max;
		}
		return value;
	}

	public static float Clamp(float value, float min, float max)
	{
		if (value <= min)
		{
			return min;
		}
		if (value >= max)
		{
			return max;
		}
		return value;
	}

	public static float Clamp01(float value)
	{
		if (value <= 0f)
		{
			return 0f;
		}
		if (value >= 1f)
		{
			return 1f;
		}
		return value;
	}

	public static float ClampUnknownOrder(float value, float extremumA, float extremumB)
	{
		if (extremumA < extremumB)
		{
			return Clamp(value, extremumA, extremumB);
		}
		return Clamp(value, extremumB, extremumA);
	}

	public static float ClampMagnitude(float value, float min, float max)
	{
		if (min < 0f)
		{
			throw new ArgumentOutOfRangeException("min", "Cannot clamp magnitude to values below 0.");
		}
		if (max < 0f)
		{
			throw new ArgumentOutOfRangeException("max", "Cannot clamp magnitude to values below 0.");
		}
		return (float)SignNonZero(value) * Clamp(Abs(value), min, max);
	}

	public static Vector2 Clamp(Vector2 vector, float min, float max)
	{
		return new Vector2(Clamp(vector.x, min, max), Clamp(vector.y, min, max));
	}

	public static Vector2Int Clamp(Vector2Int vector, int min, int max)
	{
		return new Vector2Int(Clamp(vector.x, min, max), Clamp(vector.y, min, max));
	}

	public static Vector2 Clamp(Vector2 vector, Vector2 min, Vector2 max)
	{
		return new Vector2(Clamp(vector.x, min.x, max.x), Clamp(vector.y, min.y, max.y));
	}

	public static Vector2Int Clamp(Vector2Int vector, Vector2Int min, Vector2Int max)
	{
		return new Vector2Int(Clamp(vector.x, min.x, max.x), Clamp(vector.y, min.y, max.y));
	}

	public static Vector3 Clamp(Vector3 vector, float min, float max)
	{
		return new Vector3(Clamp(vector.x, min, max), Clamp(vector.y, min, max), Clamp(vector.z, min, max));
	}

	public static Vector3Int Clamp(Vector3Int vector, int min, int max)
	{
		return new Vector3Int(Clamp(vector.x, min, max), Clamp(vector.y, min, max), Clamp(vector.z, min, max));
	}

	public static Vector3 Clamp(Vector3 vector, Vector3 min, Vector3 max)
	{
		return new Vector3(Clamp(vector.x, min.x, max.x), Clamp(vector.y, min.y, max.y), Clamp(vector.z, min.z, max.z));
	}

	public static Vector3Int Clamp(Vector3Int vector, Vector3Int min, Vector3Int max)
	{
		return new Vector3Int(Clamp(vector.x, min.x, max.x), Clamp(vector.y, min.y, max.y), Clamp(vector.z, min.z, max.z));
	}

	public static Vector3 SnapToHorizontalCardinalAndOrdinal(Vector3 vector, bool normalized = true)
	{
		float num = Max(Abs(vector.x), Abs(vector.z));
		float num2 = 1f / num;
		vector = new Vector3(RoundToInt(vector.x * num2), 0f, RoundToInt(vector.z * num2));
		if (normalized)
		{
			return vector.normalized;
		}
		return vector;
	}

	public static float Remap(float fromMin, float fromMax, float toMin, float toMax, float value)
	{
		float t = InverseLerp(fromMin, fromMax, value);
		return Lerp(toMin, toMax, t);
	}

	public static float RemapClamped(float fromMin, float fromMax, float toMin, float toMax, float value)
	{
		float t = InverseLerpClamped(fromMin, fromMax, value);
		return Lerp(toMin, toMax, t);
	}

	public static float Remap(float fromMin, float fromMax, float toMin, float toMax, float value, Func<float, float> Easing)
	{
		float arg = InverseLerp(fromMin, fromMax, value);
		return Lerp(toMin, toMax, Easing(arg));
	}

	public static float RemapClamped(float fromMin, float fromMax, float toMin, float toMax, float value, Func<float, float> Easing)
	{
		float arg = InverseLerpClamped(fromMin, fromMax, value);
		return Lerp(toMin, toMax, Easing(arg));
	}

	public static Vector3 Remap(float fromMin, float fromMax, Vector3 toMin, Vector3 toMax, float value)
	{
		float t = InverseLerp(fromMin, fromMax, value);
		return new Vector3(Lerp(toMin.x, toMax.x, t), Lerp(toMin.y, toMax.y, t), Lerp(toMin.z, toMax.z, t));
	}

	public static Vector3 RemapClamped(float fromMin, float fromMax, Vector3 toMin, Vector3 toMax, float value)
	{
		float t = InverseLerpClamped(fromMin, fromMax, value);
		return new Vector3(Lerp(toMin.x, toMax.x, t), Lerp(toMin.y, toMax.y, t), Lerp(toMin.z, toMax.z, t));
	}

	public static Vector3 Remap(Vector3 fromMin, Vector3 fromMax, Vector3 toMin, Vector3 toMax, Vector3 value)
	{
		return new Vector3(Remap(fromMin.x, fromMax.x, toMin.x, toMax.x, value.x), Remap(fromMin.y, fromMax.y, toMin.y, toMax.y, value.y), Remap(fromMin.z, fromMax.z, toMin.z, toMax.z, value.z));
	}

	public static Vector3 RemapClamped(Vector3 fromMin, Vector3 fromMax, Vector3 toMin, Vector3 toMax, Vector3 value)
	{
		return new Vector3(Remap(fromMin.x, fromMax.x, toMin.x, toMax.x, Clamp01(value.x)), Remap(fromMin.y, fromMax.y, toMin.y, toMax.y, Clamp01(value.y)), Remap(fromMin.z, fromMax.z, toMin.z, toMax.z, Clamp01(value.z)));
	}

	public static int Wrap(int value, int around)
	{
		int num = value % around;
		if (num < 0)
		{
			return around + num;
		}
		return num;
	}

	public static int Wrap(int value, int around, out int wrapCount)
	{
		wrapCount = FloorToInt((float)value / (float)around);
		int num = value % around;
		if (num < 0)
		{
			return around + num;
		}
		return num;
	}

	public static float Wrap(float value, float around)
	{
		float num = value % around;
		if (num < 0f)
		{
			return around + num;
		}
		return num;
	}

	public static float Wrap(float value, float around, out int wrapCount)
	{
		wrapCount = FloorToInt(value / around);
		float num = value % around;
		if (num < 0f)
		{
			return around + num;
		}
		return num;
	}

	public static float WrapAngleRad(float angle)
	{
		while (angle <= -MathF.PI)
		{
			angle += MathF.PI * 2f;
		}
		while (angle > MathF.PI)
		{
			angle -= MathF.PI * 2f;
		}
		return angle;
	}

	public static float WrapAngleDeg(float angle)
	{
		while (angle <= -180f)
		{
			angle += 360f;
		}
		while (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public static float WrapAngleTauRad(float angle)
	{
		while (angle < 0f)
		{
			angle += MathF.PI * 2f;
		}
		while (angle >= MathF.PI * 2f)
		{
			angle -= MathF.PI * 2f;
		}
		return angle;
	}

	public static float WrapAngle360Deg(float angle)
	{
		while (angle < 0f)
		{
			angle += 360f;
		}
		while (angle >= 360f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public static float EaseInSine(float t)
	{
		return 1f - Cos(t * MathF.PI * 0.5f);
	}

	public static float EaseInSineClamped(float t)
	{
		if (t <= 0f)
		{
			return 0f;
		}
		if (t >= 1f)
		{
			return 1f;
		}
		return 1f - Cos(t * MathF.PI * 0.5f);
	}

	public static float EaseOutSine(float t)
	{
		return Sin(t * MathF.PI * 0.5f);
	}

	public static float EaseOutSineClamped(float t)
	{
		if (t <= 0f)
		{
			return 0f;
		}
		if (t >= 1f)
		{
			return 1f;
		}
		return Sin(t * MathF.PI * 0.5f);
	}

	public static float EaseInOutSine(float t)
	{
		return (0f - (Cos(MathF.PI * t) - 1f)) * 0.5f;
	}

	public static float EaseInOutSineClamped(float t)
	{
		if (t <= 0f)
		{
			return 0f;
		}
		if (t >= 1f)
		{
			return 1f;
		}
		return (0f - (Cos(MathF.PI * t) - 1f)) * 0.5f;
	}

	public static float EaseIn(float t)
	{
		return t * t;
	}

	public static float InvertEaseIn(float t)
	{
		return Sqrt(t);
	}

	public static float EaseInClamped(float t)
	{
		if (t <= 0f)
		{
			return 0f;
		}
		if (t >= 1f)
		{
			return 1f;
		}
		return t * t;
	}

	public static float EaseOut(float t)
	{
		float num = 1f - t;
		return 1f - num * num;
	}

	public static float EaseOutClamped(float t)
	{
		if (t <= 0f)
		{
			return 0f;
		}
		if (t >= 1f)
		{
			return 1f;
		}
		float num = 1f - t;
		return 1f - num * num;
	}

	public static float EaseInOut(float t)
	{
		if (t < 0.5f)
		{
			return 2f * t * t;
		}
		float num = -2f * t + 2f;
		return 1f - num * num * 0.5f;
	}

	public static float EaseInOutClamped(float t)
	{
		if (t <= 0f)
		{
			return 0f;
		}
		if (t >= 1f)
		{
			return 1f;
		}
		if (t < 0.5f)
		{
			return 2f * t * t;
		}
		float num = -2f * t + 2f;
		return 1f - num * num * 0.5f;
	}

	public static (float position, float speed) UpdateSpring(float position, float speed, float targetPosition, float zeta, float omega, float deltaTime)
	{
		float num = 1f + 2f * deltaTime * zeta * omega;
		float num2 = omega * omega;
		float num3 = deltaTime * num2;
		float num4 = deltaTime * num3;
		float num5 = 1f / (num + num4);
		float num6 = num * position + deltaTime * speed + num4 * targetPosition;
		float num7 = speed + num3 * (targetPosition - position);
		return (position: num6 * num5, speed: num7 * num5);
	}

	public static (Vector3 position, Vector3 velocity) UpdateSpring(Vector3 position, Vector3 velocity, Vector3 targetPosition, float zeta, float omega, float deltaTime)
	{
		float num = 1f + 2f * deltaTime * zeta * omega;
		float num2 = omega * omega;
		float num3 = deltaTime * num2;
		float num4 = deltaTime * num3;
		float num5 = 1f / (num + num4);
		Vector3 vector = num * position + deltaTime * velocity + num4 * targetPosition;
		return new ValueTuple<Vector3, Vector3>(item2: (velocity + num3 * (targetPosition - position)) * num5, item1: vector * num5);
	}

	public static (Vector3 position, Vector3 velocity) UpdateSpring(Vector3 position, Vector3 velocity, Vector3 targetPosition, Vector3 zeta, Vector3 omega, float deltaTime)
	{
		Vector3 a = Vector3.one + 2f * deltaTime * Vector3.Scale(zeta, omega);
		Vector3 vector = Vector3.Scale(omega, omega);
		Vector3 vector2 = deltaTime * vector;
		Vector3 a2 = deltaTime * vector2;
		Vector3 b = new Vector3(1f / (a.x + a2.x), 1f / (a.y + a2.y), 1f / (a.z + a2.z));
		Vector3 a3 = Vector3.Scale(a, position) + deltaTime * velocity + Vector3.Scale(a2, targetPosition);
		return new ValueTuple<Vector3, Vector3>(item2: Vector3.Scale(velocity + Vector3.Scale(vector2, targetPosition - position), b), item1: Vector3.Scale(a3, b));
	}

	public static float PerlinNoise(float x, float y)
	{
		return Mathf.PerlinNoise(x, y);
	}

	public static float Cos(float angle)
	{
		return (float)Math.Cos(angle);
	}

	public static float CosDeg(float angle)
	{
		return (float)Math.Cos(angle * (MathF.PI / 180f));
	}

	public static float Sin(float angle)
	{
		return (float)Math.Sin(angle);
	}

	public static float SinDeg(float angle)
	{
		return (float)Math.Sin(angle * (MathF.PI / 180f));
	}

	public static float Tan(float angle)
	{
		return (float)Math.Tan(angle);
	}

	public static float TanDeg(float angle)
	{
		return (float)Math.Tan(angle * (MathF.PI / 180f));
	}

	public static float Acos(float value)
	{
		return (float)Math.Acos(value);
	}

	public static float AcosDeg(float value)
	{
		return (float)Math.Acos(value) * (180f / MathF.PI);
	}

	public static float Asin(float value)
	{
		return (float)Math.Asin(value);
	}

	public static float AsinDeg(float value)
	{
		return (float)Math.Asin(value) * (180f / MathF.PI);
	}

	public static float Atan(float value)
	{
		return (float)Math.Atan(value);
	}

	public static float AtanDeg(float value)
	{
		return (float)Math.Atan(value) * (180f / MathF.PI);
	}

	public static float Atan2(float y, float x)
	{
		return (float)Math.Atan2(y, x);
	}

	public static float Atan2Deg(float y, float x)
	{
		return (float)Math.Atan2(y, x) * (180f / MathF.PI);
	}

	public static int Abs(int value)
	{
		return Math.Abs(value);
	}

	public static float Abs(float value)
	{
		return Math.Abs(value);
	}

	public static double Abs(double value)
	{
		return Math.Abs(value);
	}

	public static Vector2 Abs(Vector2 vector)
	{
		vector.x = Abs(vector.x);
		vector.y = Abs(vector.y);
		return vector;
	}

	public static Vector3 Abs(Vector3 vector)
	{
		vector.x = Abs(vector.x);
		vector.y = Abs(vector.y);
		vector.z = Abs(vector.z);
		return vector;
	}

	public static Vector4 Abs(Vector4 vector)
	{
		vector.x = Abs(vector.x);
		vector.y = Abs(vector.y);
		vector.z = Abs(vector.z);
		vector.w = Abs(vector.w);
		return vector;
	}

	public static int Sign(int f)
	{
		if (f == 0)
		{
			return 0;
		}
		if (f <= 0)
		{
			return -1;
		}
		return 1;
	}

	public static int Sign(float f)
	{
		if (f == 0f)
		{
			return 0;
		}
		if (!(f > 0f))
		{
			return -1;
		}
		return 1;
	}

	public static int SignNonZero(int f)
	{
		if (f < 0)
		{
			return -1;
		}
		return 1;
	}

	public static int SignNonZero(float f)
	{
		if (!(f >= 0f))
		{
			return -1;
		}
		return 1;
	}

	public static Vector3Int Sign(Vector3Int vector)
	{
		return new Vector3Int(Sign(vector.x), Sign(vector.y), Sign(vector.z));
	}

	public static Vector3Int Sign(Vector3 vector)
	{
		return new Vector3Int(Sign(vector.x), Sign(vector.y), Sign(vector.z));
	}

	public static Vector3Int SignNonZero(Vector3Int vector)
	{
		return new Vector3Int(SignNonZero(vector.x), SignNonZero(vector.y), SignNonZero(vector.z));
	}

	public static Vector3Int SignNonZero(Vector3 vector)
	{
		return new Vector3Int(SignNonZero(vector.x), SignNonZero(vector.y), SignNonZero(vector.z));
	}

	public static int Min(int a, int b)
	{
		if (a >= b)
		{
			return b;
		}
		return a;
	}

	public static int Min(int a, int b, out int index)
	{
		if (a < b)
		{
			index = 0;
			return a;
		}
		index = 1;
		return b;
	}

	public static int Min(int a, int b, int c)
	{
		if (a < b)
		{
			if (a >= c)
			{
				return c;
			}
			return a;
		}
		if (b >= c)
		{
			return c;
		}
		return b;
	}

	public static int Min(int a, int b, int c, out int index)
	{
		if (a < b)
		{
			if (a < c)
			{
				index = 0;
				return a;
			}
			index = 2;
			return c;
		}
		if (b < c)
		{
			index = 1;
			return b;
		}
		index = 2;
		return c;
	}

	public static int Min(int a, int b, int c, int d)
	{
		if (a < b)
		{
			if (a < c)
			{
				if (a >= d)
				{
					return d;
				}
				return a;
			}
			if (c >= d)
			{
				return d;
			}
			return c;
		}
		if (b < c)
		{
			if (b >= d)
			{
				return d;
			}
			return b;
		}
		if (c >= d)
		{
			return d;
		}
		return c;
	}

	public static int Min(int a, int b, int c, int d, out int index)
	{
		if (a < b)
		{
			if (a < c)
			{
				if (a < d)
				{
					index = 0;
					return a;
				}
				index = 3;
				return d;
			}
			if (c < d)
			{
				index = 2;
				return c;
			}
			index = 3;
			return d;
		}
		if (b < c)
		{
			if (b < d)
			{
				index = 1;
				return b;
			}
			index = 3;
			return d;
		}
		if (c < d)
		{
			index = 2;
			return c;
		}
		index = 3;
		return d;
	}

	public static int Min(int[] values)
	{
		if ((float)values.Length == 0f)
		{
			return 0;
		}
		int num = values[0];
		foreach (int num2 in values)
		{
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public static int Min(int[] values, out int index)
	{
		index = -1;
		if ((float)values.Length == 0f)
		{
			return 0;
		}
		int num = values[0];
		for (int i = 0; i < values.Length; i++)
		{
			int num2 = values[i];
			if (num2 < num)
			{
				index = i;
				num = num2;
			}
		}
		return num;
	}

	public static int Max(int a, int b)
	{
		if (a <= b)
		{
			return b;
		}
		return a;
	}

	public static int Max(int a, int b, out int index)
	{
		if (a > b)
		{
			index = 0;
			return a;
		}
		index = 1;
		return b;
	}

	public static int Max(int a, int b, int c)
	{
		if (a > b)
		{
			if (a <= c)
			{
				return c;
			}
			return a;
		}
		if (b <= c)
		{
			return c;
		}
		return b;
	}

	public static int Max(int a, int b, int c, out int index)
	{
		if (a > b)
		{
			if (a > c)
			{
				index = 0;
				return a;
			}
			index = 2;
			return c;
		}
		if (b > c)
		{
			index = 1;
			return b;
		}
		index = 2;
		return c;
	}

	public static int Max(int a, int b, int c, int d)
	{
		if (a > b)
		{
			if (a > c)
			{
				if (a <= d)
				{
					return d;
				}
				return a;
			}
			if (c <= d)
			{
				return d;
			}
			return c;
		}
		if (b > c)
		{
			if (b <= d)
			{
				return d;
			}
			return b;
		}
		if (c <= d)
		{
			return d;
		}
		return c;
	}

	public static int Max(int a, int b, int c, int d, out int index)
	{
		if (a > b)
		{
			if (a > c)
			{
				if (a > d)
				{
					index = 0;
					return a;
				}
				index = 3;
				return d;
			}
			if (c > d)
			{
				index = 2;
				return c;
			}
			index = 3;
			return d;
		}
		if (b > c)
		{
			if (b > d)
			{
				index = 1;
				return b;
			}
			index = 3;
			return d;
		}
		if (c > d)
		{
			index = 2;
			return c;
		}
		index = 3;
		return d;
	}

	public static int Max(int[] values)
	{
		if ((float)values.Length == 0f)
		{
			return 0;
		}
		int num = values[0];
		foreach (int num2 in values)
		{
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public static int Max(int[] values, out int index)
	{
		index = -1;
		if ((float)values.Length == 0f)
		{
			return 0;
		}
		int num = values[0];
		for (int i = 0; i < values.Length; i++)
		{
			int num2 = values[i];
			if (num2 > num)
			{
				index = i;
				num = num2;
			}
		}
		return num;
	}

	public static uint Min(uint a, uint b)
	{
		if (a >= b)
		{
			return b;
		}
		return a;
	}

	public static uint Min(uint a, uint b, out int index)
	{
		if (a < b)
		{
			index = 0;
			return a;
		}
		index = 1;
		return b;
	}

	public static uint Min(uint a, uint b, uint c)
	{
		if (a < b)
		{
			if (a >= c)
			{
				return c;
			}
			return a;
		}
		if (b >= c)
		{
			return c;
		}
		return b;
	}

	public static uint Min(uint a, uint b, uint c, out int index)
	{
		if (a < b)
		{
			if (a < c)
			{
				index = 0;
				return a;
			}
			index = 2;
			return c;
		}
		if (b < c)
		{
			index = 1;
			return b;
		}
		index = 2;
		return c;
	}

	public static uint Min(uint a, uint b, uint c, uint d)
	{
		if (a < b)
		{
			if (a < c)
			{
				if (a >= d)
				{
					return d;
				}
				return a;
			}
			if (c >= d)
			{
				return d;
			}
			return c;
		}
		if (b < c)
		{
			if (b >= d)
			{
				return d;
			}
			return b;
		}
		if (c >= d)
		{
			return d;
		}
		return c;
	}

	public static uint Min(uint a, uint b, uint c, uint d, out int index)
	{
		if (a < b)
		{
			if (a < c)
			{
				if (a < d)
				{
					index = 0;
					return a;
				}
				index = 3;
				return d;
			}
			if (c < d)
			{
				index = 2;
				return c;
			}
			index = 3;
			return d;
		}
		if (b < c)
		{
			if (b < d)
			{
				index = 1;
				return b;
			}
			index = 3;
			return d;
		}
		if (c < d)
		{
			index = 2;
			return c;
		}
		index = 3;
		return d;
	}

	public static uint Min(uint[] values)
	{
		if ((float)values.Length == 0f)
		{
			return 0u;
		}
		uint num = values[0];
		foreach (uint num2 in values)
		{
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public static uint Min(uint[] values, out int index)
	{
		index = -1;
		if ((float)values.Length == 0f)
		{
			return 0u;
		}
		uint num = values[0];
		for (int i = 0; i < values.Length; i++)
		{
			uint num2 = values[i];
			if (num2 < num)
			{
				index = i;
				num = num2;
			}
		}
		return num;
	}

	public static uint Max(uint a, uint b)
	{
		if (a <= b)
		{
			return b;
		}
		return a;
	}

	public static uint Max(uint a, uint b, out int index)
	{
		if (a > b)
		{
			index = 0;
			return a;
		}
		index = 1;
		return b;
	}

	public static uint Max(uint a, uint b, uint c)
	{
		if (a > b)
		{
			if (a <= c)
			{
				return c;
			}
			return a;
		}
		if (b <= c)
		{
			return c;
		}
		return b;
	}

	public static uint Max(uint a, uint b, uint c, out int index)
	{
		if (a > b)
		{
			if (a > c)
			{
				index = 0;
				return a;
			}
			index = 2;
			return c;
		}
		if (b > c)
		{
			index = 1;
			return b;
		}
		index = 2;
		return c;
	}

	public static uint Max(uint a, uint b, uint c, uint d)
	{
		if (a > b)
		{
			if (a > c)
			{
				if (a <= d)
				{
					return d;
				}
				return a;
			}
			if (c <= d)
			{
				return d;
			}
			return c;
		}
		if (b > c)
		{
			if (b <= d)
			{
				return d;
			}
			return b;
		}
		if (c <= d)
		{
			return d;
		}
		return c;
	}

	public static uint Max(uint a, uint b, uint c, uint d, out int index)
	{
		if (a > b)
		{
			if (a > c)
			{
				if (a > d)
				{
					index = 0;
					return a;
				}
				index = 3;
				return d;
			}
			if (c > d)
			{
				index = 2;
				return c;
			}
			index = 3;
			return d;
		}
		if (b > c)
		{
			if (b > d)
			{
				index = 1;
				return b;
			}
			index = 3;
			return d;
		}
		if (c > d)
		{
			index = 2;
			return c;
		}
		index = 3;
		return d;
	}

	public static uint Max(uint[] values)
	{
		if ((float)values.Length == 0f)
		{
			return 0u;
		}
		uint num = values[0];
		foreach (uint num2 in values)
		{
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public static uint Max(uint[] values, out int index)
	{
		index = -1;
		if ((float)values.Length == 0f)
		{
			return 0u;
		}
		uint num = values[0];
		for (int i = 0; i < values.Length; i++)
		{
			uint num2 = values[i];
			if (num2 > num)
			{
				index = i;
				num = num2;
			}
		}
		return num;
	}

	public static float Min(float a, float b)
	{
		if (!(a < b))
		{
			return b;
		}
		return a;
	}

	public static float Min(float a, float b, out int index)
	{
		if (a < b)
		{
			index = 0;
			return a;
		}
		index = 1;
		return b;
	}

	public static float Min(float a, float b, float c)
	{
		if (a < b)
		{
			if (!(a < c))
			{
				return c;
			}
			return a;
		}
		if (!(b < c))
		{
			return c;
		}
		return b;
	}

	public static float Min(float a, float b, float c, out int index)
	{
		if (a < b)
		{
			if (a < c)
			{
				index = 0;
				return a;
			}
			index = 2;
			return c;
		}
		if (b < c)
		{
			index = 1;
			return b;
		}
		index = 2;
		return c;
	}

	public static float Min(float a, float b, float c, float d)
	{
		if (a < b)
		{
			if (a < c)
			{
				if (!(a < d))
				{
					return d;
				}
				return a;
			}
			if (!(c < d))
			{
				return d;
			}
			return c;
		}
		if (b < c)
		{
			if (!(b < d))
			{
				return d;
			}
			return b;
		}
		if (!(c < d))
		{
			return d;
		}
		return c;
	}

	public static float Min(float a, float b, float c, float d, out int index)
	{
		if (a < b)
		{
			if (a < c)
			{
				if (a < d)
				{
					index = 0;
					return a;
				}
				index = 3;
				return d;
			}
			if (c < d)
			{
				index = 2;
				return c;
			}
			index = 3;
			return d;
		}
		if (b < c)
		{
			if (b < d)
			{
				index = 1;
				return b;
			}
			index = 3;
			return d;
		}
		if (c < d)
		{
			index = 2;
			return c;
		}
		index = 3;
		return d;
	}

	public static float Min(float[] values)
	{
		if ((float)values.Length == 0f)
		{
			return 0f;
		}
		float num = values[0];
		foreach (float num2 in values)
		{
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public static float Min(float[] values, out int index)
	{
		index = -1;
		if ((float)values.Length == 0f)
		{
			return 0f;
		}
		float num = values[0];
		for (int i = 0; i < values.Length; i++)
		{
			float num2 = values[i];
			if (num2 < num)
			{
				index = i;
				num = num2;
			}
		}
		return num;
	}

	public static float Max(float a, float b)
	{
		if (!(a > b))
		{
			return b;
		}
		return a;
	}

	public static float Max(float a, float b, out int index)
	{
		if (a > b)
		{
			index = 0;
			return a;
		}
		index = 1;
		return b;
	}

	public static float Max(float a, float b, float c)
	{
		if (a > b)
		{
			if (!(a > c))
			{
				return c;
			}
			return a;
		}
		if (!(b > c))
		{
			return c;
		}
		return b;
	}

	public static float Max(float a, float b, float c, out int index)
	{
		if (a > b)
		{
			if (a > c)
			{
				index = 0;
				return a;
			}
			index = 2;
			return c;
		}
		if (b > c)
		{
			index = 1;
			return b;
		}
		index = 2;
		return c;
	}

	public static float Max(float a, float b, float c, float d)
	{
		if (a > b)
		{
			if (a > c)
			{
				if (!(a > d))
				{
					return d;
				}
				return a;
			}
			if (!(c > d))
			{
				return d;
			}
			return c;
		}
		if (b > c)
		{
			if (!(b > d))
			{
				return d;
			}
			return b;
		}
		if (!(c > d))
		{
			return d;
		}
		return c;
	}

	public static float Max(float a, float b, float c, float d, out int index)
	{
		if (a > b)
		{
			if (a > c)
			{
				if (a > d)
				{
					index = 0;
					return a;
				}
				index = 3;
				return d;
			}
			if (c > d)
			{
				index = 2;
				return c;
			}
			index = 3;
			return d;
		}
		if (b > c)
		{
			if (b > d)
			{
				index = 1;
				return b;
			}
			index = 3;
			return d;
		}
		if (c > d)
		{
			index = 2;
			return c;
		}
		index = 3;
		return d;
	}

	public static float Max(float[] values)
	{
		if ((float)values.Length == 0f)
		{
			return 0f;
		}
		float num = values[0];
		foreach (float num2 in values)
		{
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public static float Max(float[] values, out int index)
	{
		index = -1;
		if ((float)values.Length == 0f)
		{
			return 0f;
		}
		float num = values[0];
		for (int i = 0; i < values.Length; i++)
		{
			float num2 = values[i];
			if (num2 > num)
			{
				index = i;
				num = num2;
			}
		}
		return num;
	}

	public static float MinMagnitude(float a, float b)
	{
		if (!(Abs(a) < Abs(b)))
		{
			return b;
		}
		return a;
	}

	public static float MaxMagnitude(float a, float b)
	{
		if (!(Abs(a) > Abs(b)))
		{
			return b;
		}
		return a;
	}

	public static Vector2 Min(Vector2 vector, float value)
	{
		return new Vector2(Min(vector.x, value), Min(vector.y, value));
	}

	public static Vector3 Min(Vector3 vector, float value)
	{
		return new Vector3(Min(vector.x, value), Min(vector.y, value), Min(vector.z, value));
	}

	public static Vector4 Min(Vector4 vector, float value)
	{
		return new Vector4(Min(vector.x, value), Min(vector.y, value), Min(vector.z, value), Min(vector.w, value));
	}

	public static Vector2 Max(Vector2 vector, float value)
	{
		return new Vector2(Max(vector.x, value), Max(vector.y, value));
	}

	public static Vector3 Max(Vector3 vector, float value)
	{
		return new Vector3(Max(vector.x, value), Max(vector.y, value), Max(vector.z, value));
	}

	public static Vector4 Max(Vector4 vector, float value)
	{
		return new Vector4(Max(vector.x, value), Max(vector.y, value), Max(vector.z, value), Max(vector.w, value));
	}

	public static bool AreSignsOpposite(float a, float b)
	{
		return a * b < 0f;
	}

	public static bool AreSignsOpposite(int a, int b)
	{
		return a * b < 0;
	}

	public static bool AreSignsEqual(float a, float b)
	{
		return a * b > 0f;
	}

	public static bool AreSignsEqual(int a, int b)
	{
		return a * b > 0;
	}

	public static float Pow(float f, float p)
	{
		return (float)Math.Pow(f, p);
	}

	public static float Exp(float power)
	{
		return (float)Math.Exp(power);
	}

	public static float Sqrt(float value)
	{
		return (float)Math.Sqrt(value);
	}

	public static float Log(float f)
	{
		return (float)Math.Log(f);
	}

	public static float Log10(float f)
	{
		return (float)Math.Log10(f);
	}

	public static float Ceil(float f)
	{
		return (float)Math.Ceiling(f);
	}

	public static float Floor(float f)
	{
		return (float)Math.Floor(f);
	}

	public static float Round(float f)
	{
		return (float)Math.Round(f);
	}

	public static int CeilToInt(float f)
	{
		return (int)Math.Ceiling(f);
	}

	public static int FloorToInt(float f)
	{
		return (int)Math.Floor(f);
	}

	public static int RoundToInt(float f)
	{
		return (int)Math.Round(f);
	}

	public static float CeilToMultipleOf(float f, float factor)
	{
		return Ceil(f / factor) * factor;
	}

	public static float FloorToMultipleOf(float f, float factor)
	{
		return Floor(f / factor) * factor;
	}

	public static float RoundToMultipleOf(float f, float factor)
	{
		return Round(f / factor) * factor;
	}

	public static int RoundUpToPowerOfTwo(int value)
	{
		int num;
		for (num = 1; num < value; num *= 2)
		{
		}
		return num;
	}

	public static int RoundDownToPowerOfTwo(int value)
	{
		int result = 1;
		for (int num = 1; num < value; num *= 2)
		{
			result = num;
		}
		return result;
	}

	public static Vector3 CeilElements(Vector3 vector)
	{
		return new Vector3(Ceil(vector.x), Ceil(vector.y), Ceil(vector.z));
	}

	public static Vector3 FloorElements(Vector3 vector)
	{
		return new Vector3(Floor(vector.x), Floor(vector.y), Floor(vector.z));
	}

	public static Vector3 RoundElements(Vector3 vector)
	{
		return new Vector3(Round(vector.x), Round(vector.y), Round(vector.z));
	}

	public static Vector3Int CeilElementsToInt(Vector3 vector)
	{
		return new Vector3Int(CeilToInt(vector.x), CeilToInt(vector.y), CeilToInt(vector.z));
	}

	public static Vector3Int FloorElementsToInt(Vector3 vector)
	{
		return new Vector3Int(FloorToInt(vector.x), FloorToInt(vector.y), FloorToInt(vector.z));
	}

	public static Vector3Int RoundElementsToInt(Vector3 vector)
	{
		return new Vector3Int(RoundToInt(vector.x), RoundToInt(vector.y), RoundToInt(vector.z));
	}

	public static Vector3 CeilElementsToMultipleOf(Vector3 vector, float factor)
	{
		return new Vector3(CeilToMultipleOf(vector.x, factor), CeilToMultipleOf(vector.y, factor), CeilToMultipleOf(vector.z, factor));
	}

	public static Vector3 FloorElementsToMultipleOf(Vector3 vector, float factor)
	{
		return new Vector3(FloorToMultipleOf(vector.x, factor), FloorToMultipleOf(vector.y, factor), FloorToMultipleOf(vector.z, factor));
	}

	public static Vector3 RoundElementsToMultipleOf(Vector3 vector, float factor)
	{
		return new Vector3(RoundToMultipleOf(vector.x, factor), RoundToMultipleOf(vector.y, factor), RoundToMultipleOf(vector.z, factor));
	}

	public static Vector3 CeilElementsToMultipleOf(Vector3 vector, Vector3 factors)
	{
		return new Vector3(CeilToMultipleOf(vector.x, factors.x), CeilToMultipleOf(vector.y, factors.y), CeilToMultipleOf(vector.z, factors.z));
	}

	public static Vector3 FloorElementsToMultipleOf(Vector3 vector, Vector3 factors)
	{
		return new Vector3(FloorToMultipleOf(vector.x, factors.x), FloorToMultipleOf(vector.y, factors.y), FloorToMultipleOf(vector.z, factors.z));
	}

	public static Vector3 RoundElementsToMultipleOf(Vector3 vector, Vector3 factors)
	{
		return new Vector3(RoundToMultipleOf(vector.x, factors.x), RoundToMultipleOf(vector.y, factors.y), RoundToMultipleOf(vector.z, factors.z));
	}

	public static int SolveQuadratic(float a, float b, float c, out float root1, out float root2)
	{
		root1 = 0f;
		root2 = 0f;
		if (math.abs(a) <= math.max(1E-05f, 0f))
		{
			if (math.abs(b) <= math.max(1E-05f, 0f))
			{
				if (math.abs(c) <= math.max(1E-05f, 0f))
				{
					return -1;
				}
				return 0;
			}
			root1 = (0f - c) / b;
			return 1;
		}
		float num = b * b - 4f * a * c;
		if (math.abs(num) < 0.0001f)
		{
			root1 = (0f - b) / (2f * a);
			return 1;
		}
		if (num > 0f)
		{
			float num2 = 1f / (2f * a);
			float num3 = math.sqrt(num);
			root1 = (0f - b + num3) * num2;
			root2 = (0f - b - num3) * num2;
			return 2;
		}
		return 0;
	}

	public static float GetTimeSince(double timestamp)
	{
		return (float)(Time.timeAsDouble - timestamp);
	}

	public static float GetUnscaledTimeSince(double timestamp)
	{
		return (float)(Time.unscaledTimeAsDouble - timestamp);
	}

	public static (T, T) Swap<T>(T a, T b)
	{
		return (b, a);
	}

	public static Vector3 Barycentric(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
	{
		return Barycentric(new Vector3(a.x, a.y), new Vector3(b.x, b.y), new Vector3(c.x, c.y), new Vector3(p.x, p.y));
	}

	public static Vector3 Barycentric(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
	{
		Vector3 vector = b - a;
		Vector3 vector2 = c - a;
		Vector3 lhs = p - a;
		float num = Vector3.Dot(vector, vector);
		float num2 = Vector3.Dot(vector, vector2);
		float num3 = Vector3.Dot(vector2, vector2);
		float num4 = Vector3.Dot(lhs, vector);
		float num5 = Vector3.Dot(lhs, vector2);
		float num6 = num * num3 - num2 * num2;
		Vector3 result = default(Vector3);
		result.y = (num3 * num4 - num2 * num5) / num6;
		result.z = (num * num5 - num2 * num4) / num6;
		result.x = 1f - result.y - result.z;
		return result;
	}
}
