using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Math/MathScripting.h")]
[NativeType(Header = "Runtime/Math/Quaternion.h")]
[UsedByNativeCode]
[Il2CppEagerStaticClassConstruction]
public struct Quaternion : IEquatable<Quaternion>, IFormattable
{
	public float x;

	public float y;

	public float z;

	public float w;

	private static readonly Quaternion identityQuaternion = new Quaternion(0f, 0f, 0f, 1f);

	public const float kEpsilon = 1E-06f;

	public float this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				2 => z, 
				3 => w, 
				_ => throw new IndexOutOfRangeException("Invalid Quaternion index!"), 
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (index)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			case 2:
				z = value;
				break;
			case 3:
				w = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid Quaternion index!");
			}
		}
	}

	public static Quaternion identity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return identityQuaternion;
		}
	}

	public Vector3 eulerAngles
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Internal_MakePositive(Internal_ToEulerRad(in this) * 57.29578f);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			this = Internal_FromEulerRad(value * (MathF.PI / 180f));
		}
	}

	public readonly Quaternion normalized
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Normalize(in this);
		}
	}

	[FreeFunction("FromToQuaternionSafe", IsThreadSafe = true)]
	private static Quaternion Internal_FromToRotation(in Vector3 fromDirection, in Vector3 toDirection)
	{
		Internal_FromToRotation_Injected(in fromDirection, in toDirection, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
	{
		return Internal_FromToRotation(in fromDirection, in toDirection);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion FromToRotation(in Vector3 fromDirection, in Vector3 toDirection)
	{
		return Internal_FromToRotation(in fromDirection, in toDirection);
	}

	[FreeFunction("QuaternionScripting::Inverse", IsThreadSafe = true)]
	private static Quaternion Internal_Inverse(in Quaternion rotation)
	{
		Internal_Inverse_Injected(in rotation, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Inverse(Quaternion rotation)
	{
		return Internal_Inverse(in rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Inverse(in Quaternion rotation)
	{
		return Internal_Inverse(in rotation);
	}

	[FreeFunction("QuaternionScripting::Slerp", IsThreadSafe = true)]
	private static Quaternion Internal_Slerp(in Quaternion a, in Quaternion b, float t)
	{
		Internal_Slerp_Injected(in a, in b, t, out var ret);
		return ret;
	}

	[FreeFunction("QuaternionScripting::SlerpUnclamped", IsThreadSafe = true)]
	private static Quaternion Internal_SlerpUnclamped(in Quaternion a, in Quaternion b, float t)
	{
		Internal_SlerpUnclamped_Injected(in a, in b, t, out var ret);
		return ret;
	}

	[FreeFunction("QuaternionScripting::Lerp", IsThreadSafe = true)]
	private static Quaternion Internal_Lerp(in Quaternion a, in Quaternion b, float t)
	{
		Internal_Lerp_Injected(in a, in b, t, out var ret);
		return ret;
	}

	[FreeFunction("QuaternionScripting::LerpUnclamped", IsThreadSafe = true)]
	private static Quaternion Internal_LerpUnclamped(in Quaternion a, in Quaternion b, float t)
	{
		Internal_LerpUnclamped_Injected(in a, in b, t, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Slerp(Quaternion a, Quaternion b, float t)
	{
		return Internal_Slerp(in a, in b, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t)
	{
		return Internal_SlerpUnclamped(in a, in b, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Lerp(Quaternion a, Quaternion b, float t)
	{
		return Internal_Lerp(in a, in b, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, float t)
	{
		return Internal_LerpUnclamped(in a, in b, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Slerp(in Quaternion a, in Quaternion b, float t)
	{
		return Internal_Slerp(in a, in b, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion SlerpUnclamped(in Quaternion a, in Quaternion b, float t)
	{
		return Internal_SlerpUnclamped(in a, in b, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Lerp(in Quaternion a, in Quaternion b, float t)
	{
		return Internal_Lerp(in a, in b, t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion LerpUnclamped(in Quaternion a, in Quaternion b, float t)
	{
		return Internal_LerpUnclamped(in a, in b, t);
	}

	[FreeFunction("EulerToQuaternion", IsThreadSafe = true)]
	private static Quaternion Internal_FromEulerRad(in Vector3 euler)
	{
		Internal_FromEulerRad_Injected(in euler, out var ret);
		return ret;
	}

	[FreeFunction("QuaternionScripting::ToEuler", IsThreadSafe = true)]
	private static Vector3 Internal_ToEulerRad(in Quaternion rotation)
	{
		Internal_ToEulerRad_Injected(in rotation, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("QuaternionScripting::ToAxisAngle", IsThreadSafe = true)]
	private static extern void Internal_ToAxisAngleRad(in Quaternion q, out Vector3 axis, out float angle);

	[FreeFunction("QuaternionScripting::AngleAxis", IsThreadSafe = true)]
	private static Quaternion Internal_AngleAxis(float angle, in Vector3 axis)
	{
		Internal_AngleAxis_Injected(angle, in axis, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion AngleAxis(float angle, Vector3 axis)
	{
		return Internal_AngleAxis(angle, in axis);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion AngleAxis(float angle, in Vector3 axis)
	{
		return Internal_AngleAxis(angle, in axis);
	}

	[FreeFunction("QuaternionScripting::LookRotation", IsThreadSafe = true)]
	private static Quaternion Internal_LookRotation(in Vector3 forward, [DefaultValue("Vector3.up")] in Vector3 upwards)
	{
		Internal_LookRotation_Injected(in forward, in upwards, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion LookRotation(Vector3 forward, [DefaultValue("Vector3.up")] Vector3 upwards)
	{
		return Internal_LookRotation(in forward, in upwards);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion LookRotation(in Vector3 forward, [DefaultValue("Vector3.up")] in Vector3 upwards)
	{
		return Internal_LookRotation(in forward, in upwards);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExcludeFromDocs]
	public static Quaternion LookRotation(Vector3 forward)
	{
		return Internal_LookRotation(in forward, Vector3.up);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExcludeFromDocs]
	public static Quaternion LookRotation(in Vector3 forward)
	{
		return Internal_LookRotation(in forward, Vector3.up);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Quaternion(float x, float y, float z, float w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set(float newX, float newY, float newZ, float newW)
	{
		x = newX;
		y = newY;
		z = newZ;
		w = newW;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
	{
		return new Quaternion
		{
			x = lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
			y = lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
			z = lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
			w = lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z
		};
	}

	public static Vector3 operator *(Quaternion rotation, Vector3 point)
	{
		float num = rotation.x * 2f;
		float num2 = rotation.y * 2f;
		float num3 = rotation.z * 2f;
		float num4 = rotation.x * num;
		float num5 = rotation.y * num2;
		float num6 = rotation.z * num3;
		float num7 = rotation.x * num2;
		float num8 = rotation.x * num3;
		float num9 = rotation.y * num3;
		float num10 = rotation.w * num;
		float num11 = rotation.w * num2;
		float num12 = rotation.w * num3;
		Vector3 result = default(Vector3);
		result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
		result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
		result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Quaternion lhs, Quaternion rhs)
	{
		return IsEqualUsingDot(Dot(in lhs, in rhs));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Quaternion lhs, Quaternion rhs)
	{
		return !(lhs == rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsEqualUsingDot(float dot)
	{
		return dot > 0.999999f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dot(Quaternion a, Quaternion b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dot(in Quaternion a, in Quaternion b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExcludeFromDocs]
	public void SetLookRotation(Vector3 view)
	{
		SetLookRotation(in view, Vector3.up);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExcludeFromDocs]
	public void SetLookRotation(in Vector3 view)
	{
		SetLookRotation(in view, Vector3.up);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetLookRotation(Vector3 view, [DefaultValue("Vector3.up")] Vector3 up)
	{
		this = LookRotation(in view, in up);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetLookRotation(in Vector3 view, [DefaultValue("Vector3.up")] in Vector3 up)
	{
		this = LookRotation(in view, in up);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Angle(Quaternion a, Quaternion b)
	{
		float num = Mathf.Min(Mathf.Abs(Dot(in a, in b)), 1f);
		return IsEqualUsingDot(num) ? 0f : (Mathf.Acos(num) * 2f * 57.29578f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Angle(in Quaternion a, in Quaternion b)
	{
		float num = Mathf.Min(Mathf.Abs(Dot(in a, in b)), 1f);
		return IsEqualUsingDot(num) ? 0f : (Mathf.Acos(num) * 2f * 57.29578f);
	}

	private static Vector3 Internal_MakePositive(Vector3 euler)
	{
		float num = -0.005729578f;
		float num2 = 360f + num;
		if (euler.x < num)
		{
			euler.x += 360f;
		}
		else if (euler.x > num2)
		{
			euler.x -= 360f;
		}
		if (euler.y < num)
		{
			euler.y += 360f;
		}
		else if (euler.y > num2)
		{
			euler.y -= 360f;
		}
		if (euler.z < num)
		{
			euler.z += 360f;
		}
		else if (euler.z > num2)
		{
			euler.z -= 360f;
		}
		return euler;
	}

	private static Vector3 Internal_MakePositive(in Vector3 eulerAngles)
	{
		float num = -0.005729578f;
		float num2 = 360f + num;
		Vector3 result = eulerAngles;
		if (result.x < num)
		{
			result.x += 360f;
		}
		else if (result.x > num2)
		{
			result.x -= 360f;
		}
		if (result.y < num)
		{
			result.y += 360f;
		}
		else if (result.y > num2)
		{
			result.y -= 360f;
		}
		if (result.z < num)
		{
			result.z += 360f;
		}
		else if (result.z > num2)
		{
			result.z -= 360f;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Euler(float x, float y, float z)
	{
		Vector3 euler = new Vector3
		{
			x = x * (MathF.PI / 180f),
			y = y * (MathF.PI / 180f),
			z = z * (MathF.PI / 180f)
		};
		return Internal_FromEulerRad(in euler);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Euler(Vector3 euler)
	{
		return Internal_FromEulerRad(euler * (MathF.PI / 180f));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Euler(in Vector3 euler)
	{
		return Internal_FromEulerRad(euler * (MathF.PI / 180f));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ToAngleAxis(out float angle, out Vector3 axis)
	{
		Internal_ToAxisAngleRad(in this, out axis, out angle);
		angle *= 57.29578f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetFromToRotation(Vector3 fromDirection, Vector3 toDirection)
	{
		this = FromToRotation(in fromDirection, in toDirection);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetFromToRotation(in Vector3 fromDirection, in Vector3 toDirection)
	{
		this = FromToRotation(in fromDirection, in toDirection);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
	{
		float num = Angle(in from, in to);
		if (num == 0f)
		{
			return to;
		}
		return SlerpUnclamped(in from, in to, Mathf.Min(1f, maxDegreesDelta / num));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion RotateTowards(in Quaternion from, in Quaternion to, float maxDegreesDelta)
	{
		float num = Angle(in from, in to);
		if (num == 0f)
		{
			return to;
		}
		return SlerpUnclamped(in from, in to, Mathf.Min(1f, maxDegreesDelta / num));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Normalize(Quaternion q)
	{
		float num = Mathf.Sqrt(Dot(in q, in q));
		if (num < Mathf.Epsilon)
		{
			return identity;
		}
		return new Quaternion
		{
			x = q.x / num,
			y = q.y / num,
			z = q.z / num,
			w = q.w / num
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion Normalize(in Quaternion q)
	{
		float num = Mathf.Sqrt(Dot(in q, in q));
		if (num < Mathf.Epsilon)
		{
			return identity;
		}
		return new Quaternion
		{
			x = q.x / num,
			y = q.y / num,
			z = q.z / num,
			w = q.w / num
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalize()
	{
		this = Normalize(in this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals(object other)
	{
		if (other is Quaternion other2)
		{
			return Equals(in other2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Quaternion other)
	{
		return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in Quaternion other)
	{
		return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString()
	{
		return ToString(null, null);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string format)
	{
		return ToString(format, null);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string format, IFormatProvider formatProvider)
	{
		if (string.IsNullOrEmpty(format))
		{
			format = "F5";
		}
		if (formatProvider == null)
		{
			formatProvider = CultureInfo.InvariantCulture.NumberFormat;
		}
		return $"({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)}, {z.ToString(format, formatProvider)}, {w.ToString(format, formatProvider)})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
	public static Quaternion EulerRotation(float x, float y, float z)
	{
		return Internal_FromEulerRad(new Vector3(x, y, z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
	public static Quaternion EulerRotation(Vector3 euler)
	{
		return Internal_FromEulerRad(in euler);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
	public void SetEulerRotation(float x, float y, float z)
	{
		this = Internal_FromEulerRad(new Vector3(x, y, z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
	public void SetEulerRotation(Vector3 euler)
	{
		this = Internal_FromEulerRad(in euler);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
	public readonly Vector3 ToEuler()
	{
		return Internal_ToEulerRad(in this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
	public static Quaternion EulerAngles(float x, float y, float z)
	{
		return Internal_FromEulerRad(new Vector3(x, y, z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
	public static Quaternion EulerAngles(Vector3 euler)
	{
		return Internal_FromEulerRad(in euler);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
	public readonly void ToAxisAngle(out Vector3 axis, out float angle)
	{
		Internal_ToAxisAngleRad(in this, out axis, out angle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
	public void SetEulerAngles(float x, float y, float z)
	{
		SetEulerRotation(new Vector3(x, y, z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees.")]
	public void SetEulerAngles(Vector3 euler)
	{
		this = Internal_FromEulerRad(in euler);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
	public static Vector3 ToEulerAngles(Quaternion rotation)
	{
		return Internal_ToEulerRad(in rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees.")]
	public readonly Vector3 ToEulerAngles()
	{
		return Internal_ToEulerRad(in this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees.")]
	public void SetAxisAngle(Vector3 axis, float angle)
	{
		this = Internal_AngleAxis(57.29578f * angle, in axis);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
	public static Quaternion AxisAngle(Vector3 axis, float angle)
	{
		return Internal_AngleAxis(57.29578f * angle, in axis);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_FromToRotation_Injected(in Vector3 fromDirection, in Vector3 toDirection, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Inverse_Injected(in Quaternion rotation, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Slerp_Injected(in Quaternion a, in Quaternion b, float t, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SlerpUnclamped_Injected(in Quaternion a, in Quaternion b, float t, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Lerp_Injected(in Quaternion a, in Quaternion b, float t, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_LerpUnclamped_Injected(in Quaternion a, in Quaternion b, float t, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_FromEulerRad_Injected(in Vector3 euler, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_ToEulerRad_Injected(in Quaternion rotation, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_AngleAxis_Injected(float angle, in Vector3 axis, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_LookRotation_Injected(in Vector3 forward, [DefaultValue("Vector3.up")] in Vector3 upwards, out Quaternion ret);
}
