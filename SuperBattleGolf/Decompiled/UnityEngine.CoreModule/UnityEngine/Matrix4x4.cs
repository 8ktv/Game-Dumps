using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Math/MathScripting.h")]
[NativeClass("Matrix4x4f")]
[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
[Il2CppEagerStaticClassConstruction]
[NativeType(Header = "Runtime/Math/Matrix4x4.h")]
public struct Matrix4x4 : IEquatable<Matrix4x4>, IFormattable
{
	[NativeName("m_Data[0]")]
	public float m00;

	[NativeName("m_Data[1]")]
	public float m10;

	[NativeName("m_Data[2]")]
	public float m20;

	[NativeName("m_Data[3]")]
	public float m30;

	[NativeName("m_Data[4]")]
	public float m01;

	[NativeName("m_Data[5]")]
	public float m11;

	[NativeName("m_Data[6]")]
	public float m21;

	[NativeName("m_Data[7]")]
	public float m31;

	[NativeName("m_Data[8]")]
	public float m02;

	[NativeName("m_Data[9]")]
	public float m12;

	[NativeName("m_Data[10]")]
	public float m22;

	[NativeName("m_Data[11]")]
	public float m32;

	[NativeName("m_Data[12]")]
	public float m03;

	[NativeName("m_Data[13]")]
	public float m13;

	[NativeName("m_Data[14]")]
	public float m23;

	[NativeName("m_Data[15]")]
	public float m33;

	private static readonly Matrix4x4 zeroMatrix = new Matrix4x4(new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f), new Vector4(0f, 0f, 0f, 0f));

	private static readonly Matrix4x4 identityMatrix = new Matrix4x4(new Vector4(1f, 0f, 0f, 0f), new Vector4(0f, 1f, 0f, 0f), new Vector4(0f, 0f, 1f, 0f), new Vector4(0f, 0f, 0f, 1f));

	public readonly Quaternion rotation
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetRotation();
		}
	}

	public readonly Vector3 lossyScale
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetLossyScale();
		}
	}

	public readonly bool isIdentity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return IsIdentity();
		}
	}

	public readonly float determinant
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return GetDeterminant();
		}
	}

	public readonly FrustumPlanes decomposeProjection
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return DecomposeProjection();
		}
	}

	public readonly Matrix4x4 inverse
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Internal_Inverse(in this);
		}
	}

	public readonly Matrix4x4 transpose
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Internal_Transpose(in this);
		}
	}

	public float this[int row, int column]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return this[row + column * 4];
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			this[row + column * 4] = value;
		}
	}

	public float this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return index switch
			{
				0 => m00, 
				1 => m10, 
				2 => m20, 
				3 => m30, 
				4 => m01, 
				5 => m11, 
				6 => m21, 
				7 => m31, 
				8 => m02, 
				9 => m12, 
				10 => m22, 
				11 => m32, 
				12 => m03, 
				13 => m13, 
				14 => m23, 
				15 => m33, 
				_ => throw new IndexOutOfRangeException("Invalid matrix index!"), 
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (index)
			{
			case 0:
				m00 = value;
				break;
			case 1:
				m10 = value;
				break;
			case 2:
				m20 = value;
				break;
			case 3:
				m30 = value;
				break;
			case 4:
				m01 = value;
				break;
			case 5:
				m11 = value;
				break;
			case 6:
				m21 = value;
				break;
			case 7:
				m31 = value;
				break;
			case 8:
				m02 = value;
				break;
			case 9:
				m12 = value;
				break;
			case 10:
				m22 = value;
				break;
			case 11:
				m32 = value;
				break;
			case 12:
				m03 = value;
				break;
			case 13:
				m13 = value;
				break;
			case 14:
				m23 = value;
				break;
			case 15:
				m33 = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid matrix index!");
			}
		}
	}

	public static Matrix4x4 zero
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return zeroMatrix;
		}
	}

	public static Matrix4x4 identity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return identityMatrix;
		}
	}

	[ThreadSafe]
	private readonly Quaternion GetRotation()
	{
		GetRotation_Injected(ref this, out var ret);
		return ret;
	}

	[ThreadSafe]
	private readonly Vector3 GetLossyScale()
	{
		GetLossyScale_Injected(ref this, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private readonly extern bool IsIdentity();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private readonly extern float GetDeterminant();

	[ThreadSafe]
	private readonly FrustumPlanes DecomposeProjection()
	{
		DecomposeProjection_Injected(ref this, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public readonly extern bool ValidTRS();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Determinant(Matrix4x4 m)
	{
		return m.determinant;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Determinant(in Matrix4x4 m)
	{
		return m.determinant;
	}

	[FreeFunction("MatrixScripting::TRS", IsThreadSafe = true)]
	private static Matrix4x4 Internal_TRS(in Vector3 pos, in Quaternion q, in Vector3 s)
	{
		Internal_TRS_Injected(in pos, in q, in s, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
	{
		return Internal_TRS(in pos, in q, in s);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 TRS(in Vector3 pos, in Quaternion q, in Vector3 s)
	{
		return Internal_TRS(in pos, in q, in s);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MatrixScripting::SetTRS", IsThreadSafe = true)]
	private static extern void Internal_SetTRS(ref Matrix4x4 m, in Vector3 pos, in Quaternion q, in Vector3 s);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetTRS(Vector3 pos, Quaternion q, Vector3 s)
	{
		Internal_SetTRS(ref this, in pos, in q, in s);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetTRS(in Vector3 pos, in Quaternion q, in Vector3 s)
	{
		Internal_SetTRS(ref this, in pos, in q, in s);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MatrixScripting::Inverse3DAffine", IsThreadSafe = true)]
	private static extern bool Internal_Inverse3DAffine(in Matrix4x4 input, ref Matrix4x4 result);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Inverse3DAffine(Matrix4x4 input, ref Matrix4x4 result)
	{
		return Internal_Inverse3DAffine(in input, ref result);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Inverse3DAffine(in Matrix4x4 input, ref Matrix4x4 result)
	{
		return Internal_Inverse3DAffine(in input, ref result);
	}

	[FreeFunction("MatrixScripting::Inverse", IsThreadSafe = true)]
	private static Matrix4x4 Internal_Inverse(in Matrix4x4 m)
	{
		Internal_Inverse_Injected(in m, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Inverse(Matrix4x4 m)
	{
		return Internal_Inverse(in m);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Inverse(in Matrix4x4 m)
	{
		return Internal_Inverse(in m);
	}

	[FreeFunction("MatrixScripting::Transpose", IsThreadSafe = true)]
	private static Matrix4x4 Internal_Transpose(in Matrix4x4 m)
	{
		Internal_Transpose_Injected(in m, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Transpose(Matrix4x4 m)
	{
		return Internal_Transpose(in m);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Transpose(in Matrix4x4 m)
	{
		return Internal_Transpose(in m);
	}

	[FreeFunction("MatrixScripting::Ortho", IsThreadSafe = true)]
	public static Matrix4x4 Ortho(float left, float right, float bottom, float top, float zNear, float zFar)
	{
		Ortho_Injected(left, right, bottom, top, zNear, zFar, out var ret);
		return ret;
	}

	[FreeFunction("MatrixScripting::Perspective", IsThreadSafe = true)]
	public static Matrix4x4 Perspective(float fov, float aspect, float zNear, float zFar)
	{
		Perspective_Injected(fov, aspect, zNear, zFar, out var ret);
		return ret;
	}

	[FreeFunction("MatrixScripting::LookAt", IsThreadSafe = true)]
	private static Matrix4x4 Internal_LookAt(in Vector3 from, in Vector3 to, in Vector3 up)
	{
		Internal_LookAt_Injected(in from, in to, in up, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 LookAt(Vector3 from, Vector3 to, Vector3 up)
	{
		return Internal_LookAt(in from, in to, in up);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 LookAt(in Vector3 from, in Vector3 to, in Vector3 up)
	{
		return Internal_LookAt(in from, in to, in up);
	}

	[FreeFunction("MatrixScripting::Frustum", IsThreadSafe = true)]
	public static Matrix4x4 Frustum(float left, float right, float bottom, float top, float zNear, float zFar)
	{
		Frustum_Injected(left, right, bottom, top, zNear, zFar, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Frustum(FrustumPlanes fp)
	{
		return Frustum(fp.left, fp.right, fp.bottom, fp.top, fp.zNear, fp.zFar);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Frustum(in FrustumPlanes fp)
	{
		return Frustum(fp.left, fp.right, fp.bottom, fp.top, fp.zNear, fp.zFar);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MatrixScripting::Internal_CompareApproximately", IsThreadSafe = true)]
	private static extern bool Internal_CompareApproximately(in Matrix4x4 a, in Matrix4x4 b, float threshold);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool CompareApproximately(Matrix4x4 a, Matrix4x4 b, float threshold)
	{
		return Internal_CompareApproximately(in a, in b, threshold);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool CompareApproximately(in Matrix4x4 a, in Matrix4x4 b, float threshold)
	{
		return Internal_CompareApproximately(in a, in b, threshold);
	}

	public Matrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
	{
		m00 = column0.x;
		m01 = column1.x;
		m02 = column2.x;
		m03 = column3.x;
		m10 = column0.y;
		m11 = column1.y;
		m12 = column2.y;
		m13 = column3.y;
		m20 = column0.z;
		m21 = column1.z;
		m22 = column2.z;
		m23 = column3.z;
		m30 = column0.w;
		m31 = column1.w;
		m32 = column2.w;
		m33 = column3.w;
	}

	public Matrix4x4(in Vector4 column0, in Vector4 column1, in Vector4 column2, in Vector4 column3)
	{
		m00 = column0.x;
		m01 = column1.x;
		m02 = column2.x;
		m03 = column3.x;
		m10 = column0.y;
		m11 = column1.y;
		m12 = column2.y;
		m13 = column3.y;
		m20 = column0.z;
		m21 = column1.z;
		m22 = column2.z;
		m23 = column3.z;
		m30 = column0.w;
		m31 = column1.w;
		m32 = column2.w;
		m33 = column3.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return GetColumn(0).GetHashCode() ^ (GetColumn(1).GetHashCode() << 2) ^ (GetColumn(2).GetHashCode() >> 2) ^ (GetColumn(3).GetHashCode() >> 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals(object other)
	{
		if (other is Matrix4x4 other2)
		{
			return Equals(in other2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Matrix4x4 other)
	{
		return GetColumn(0).Equals(other.GetColumn(0)) && GetColumn(1).Equals(other.GetColumn(1)) && GetColumn(2).Equals(other.GetColumn(2)) && GetColumn(3).Equals(other.GetColumn(3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in Matrix4x4 other)
	{
		return GetColumn(0).Equals(other.GetColumn(0)) && GetColumn(1).Equals(other.GetColumn(1)) && GetColumn(2).Equals(other.GetColumn(2)) && GetColumn(3).Equals(other.GetColumn(3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
	{
		return new Matrix4x4
		{
			m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30,
			m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31,
			m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32,
			m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33,
			m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30,
			m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31,
			m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32,
			m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33,
			m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30,
			m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31,
			m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32,
			m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33,
			m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30,
			m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31,
			m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32,
			m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 operator *(Matrix4x4 lhs, Vector4 vector)
	{
		return new Vector4
		{
			x = lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w,
			y = lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w,
			z = lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w,
			w = lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Matrix4x4 lhs, Matrix4x4 rhs)
	{
		return lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2) && lhs.GetColumn(3) == rhs.GetColumn(3);
	}

	public static bool operator !=(Matrix4x4 lhs, Matrix4x4 rhs)
	{
		return !(lhs == rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 GetColumn(int index)
	{
		return index switch
		{
			0 => new Vector4
			{
				x = m00,
				y = m10,
				z = m20,
				w = m30
			}, 
			1 => new Vector4
			{
				x = m01,
				y = m11,
				z = m21,
				w = m31
			}, 
			2 => new Vector4
			{
				x = m02,
				y = m12,
				z = m22,
				w = m32
			}, 
			3 => new Vector4
			{
				x = m03,
				y = m13,
				z = m23,
				w = m33
			}, 
			_ => throw new IndexOutOfRangeException("Invalid column index!"), 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector4 GetRow(int index)
	{
		return index switch
		{
			0 => new Vector4
			{
				x = m00,
				y = m01,
				z = m02,
				w = m03
			}, 
			1 => new Vector4
			{
				x = m10,
				y = m11,
				z = m12,
				w = m13
			}, 
			2 => new Vector4
			{
				x = m20,
				y = m21,
				z = m22,
				w = m23
			}, 
			3 => new Vector4
			{
				x = m30,
				y = m31,
				z = m32,
				w = m33
			}, 
			_ => throw new IndexOutOfRangeException("Invalid row index!"), 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 GetPosition()
	{
		return new Vector3
		{
			x = m03,
			y = m13,
			z = m23
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetColumn(int index, Vector4 column)
	{
		this[0, index] = column.x;
		this[1, index] = column.y;
		this[2, index] = column.z;
		this[3, index] = column.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetColumn(int index, in Vector4 column)
	{
		this[0, index] = column.x;
		this[1, index] = column.y;
		this[2, index] = column.z;
		this[3, index] = column.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetRow(int index, Vector4 row)
	{
		this[index, 0] = row.x;
		this[index, 1] = row.y;
		this[index, 2] = row.z;
		this[index, 3] = row.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetRow(int index, in Vector4 row)
	{
		this[index, 0] = row.x;
		this[index, 1] = row.y;
		this[index, 2] = row.z;
		this[index, 3] = row.w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 MultiplyPoint(Vector3 point)
	{
		Vector3 result = default(Vector3);
		result.x = m00 * point.x + m01 * point.y + m02 * point.z + m03;
		result.y = m10 * point.x + m11 * point.y + m12 * point.z + m13;
		result.z = m20 * point.x + m21 * point.y + m22 * point.z + m23;
		float num = m30 * point.x + m31 * point.y + m32 * point.z + m33;
		num = 1f / num;
		result.x *= num;
		result.y *= num;
		result.z *= num;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 MultiplyPoint(in Vector3 point)
	{
		Vector3 result = default(Vector3);
		result.x = m00 * point.x + m01 * point.y + m02 * point.z + m03;
		result.y = m10 * point.x + m11 * point.y + m12 * point.z + m13;
		result.z = m20 * point.x + m21 * point.y + m22 * point.z + m23;
		float num = m30 * point.x + m31 * point.y + m32 * point.z + m33;
		num = 1f / num;
		result.x *= num;
		result.y *= num;
		result.z *= num;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 MultiplyPoint3x4(Vector3 point)
	{
		return new Vector3
		{
			x = m00 * point.x + m01 * point.y + m02 * point.z + m03,
			y = m10 * point.x + m11 * point.y + m12 * point.z + m13,
			z = m20 * point.x + m21 * point.y + m22 * point.z + m23
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 MultiplyPoint3x4(in Vector3 point)
	{
		return new Vector3
		{
			x = m00 * point.x + m01 * point.y + m02 * point.z + m03,
			y = m10 * point.x + m11 * point.y + m12 * point.z + m13,
			z = m20 * point.x + m21 * point.y + m22 * point.z + m23
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 MultiplyVector(Vector3 vector)
	{
		return new Vector3
		{
			x = m00 * vector.x + m01 * vector.y + m02 * vector.z,
			y = m10 * vector.x + m11 * vector.y + m12 * vector.z,
			z = m20 * vector.x + m21 * vector.y + m22 * vector.z
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 MultiplyVector(in Vector3 vector)
	{
		return new Vector3
		{
			x = m00 * vector.x + m01 * vector.y + m02 * vector.z,
			y = m10 * vector.x + m11 * vector.y + m12 * vector.z,
			z = m20 * vector.x + m21 * vector.y + m22 * vector.z
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Plane TransformPlane(Plane plane)
	{
		Matrix4x4 matrix4x = inverse;
		Vector3 normal = plane.normal;
		float x = normal.x;
		float y = normal.y;
		float z = normal.z;
		float distance = plane.distance;
		float x2 = matrix4x.m00 * x + matrix4x.m10 * y + matrix4x.m20 * z + matrix4x.m30 * distance;
		float y2 = matrix4x.m01 * x + matrix4x.m11 * y + matrix4x.m21 * z + matrix4x.m31 * distance;
		float z2 = matrix4x.m02 * x + matrix4x.m12 * y + matrix4x.m22 * z + matrix4x.m32 * distance;
		float d = matrix4x.m03 * x + matrix4x.m13 * y + matrix4x.m23 * z + matrix4x.m33 * distance;
		Vector3 inNormal = new Vector3
		{
			x = x2,
			y = y2,
			z = z2
		};
		return new Plane(in inNormal, d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Plane TransformPlane(in Plane plane)
	{
		Matrix4x4 matrix4x = inverse;
		Vector3 normal = plane.normal;
		float x = normal.x;
		float y = normal.y;
		float z = normal.z;
		float distance = plane.distance;
		float x2 = matrix4x.m00 * x + matrix4x.m10 * y + matrix4x.m20 * z + matrix4x.m30 * distance;
		float y2 = matrix4x.m01 * x + matrix4x.m11 * y + matrix4x.m21 * z + matrix4x.m31 * distance;
		float z2 = matrix4x.m02 * x + matrix4x.m12 * y + matrix4x.m22 * z + matrix4x.m32 * distance;
		float d = matrix4x.m03 * x + matrix4x.m13 * y + matrix4x.m23 * z + matrix4x.m33 * distance;
		Vector3 inNormal = new Vector3
		{
			x = x2,
			y = y2,
			z = z2
		};
		return new Plane(in inNormal, d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Scale(Vector3 vector)
	{
		return new Matrix4x4
		{
			m00 = vector.x,
			m01 = 0f,
			m02 = 0f,
			m03 = 0f,
			m10 = 0f,
			m11 = vector.y,
			m12 = 0f,
			m13 = 0f,
			m20 = 0f,
			m21 = 0f,
			m22 = vector.z,
			m23 = 0f,
			m30 = 0f,
			m31 = 0f,
			m32 = 0f,
			m33 = 1f
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Scale(in Vector3 vector)
	{
		return new Matrix4x4
		{
			m00 = vector.x,
			m01 = 0f,
			m02 = 0f,
			m03 = 0f,
			m10 = 0f,
			m11 = vector.y,
			m12 = 0f,
			m13 = 0f,
			m20 = 0f,
			m21 = 0f,
			m22 = vector.z,
			m23 = 0f,
			m30 = 0f,
			m31 = 0f,
			m32 = 0f,
			m33 = 1f
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Translate(Vector3 vector)
	{
		return new Matrix4x4
		{
			m00 = 1f,
			m01 = 0f,
			m02 = 0f,
			m03 = vector.x,
			m10 = 0f,
			m11 = 1f,
			m12 = 0f,
			m13 = vector.y,
			m20 = 0f,
			m21 = 0f,
			m22 = 1f,
			m23 = vector.z,
			m30 = 0f,
			m31 = 0f,
			m32 = 0f,
			m33 = 1f
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Translate(in Vector3 vector)
	{
		return new Matrix4x4
		{
			m00 = 1f,
			m01 = 0f,
			m02 = 0f,
			m03 = vector.x,
			m10 = 0f,
			m11 = 1f,
			m12 = 0f,
			m13 = vector.y,
			m20 = 0f,
			m21 = 0f,
			m22 = 1f,
			m23 = vector.z,
			m30 = 0f,
			m31 = 0f,
			m32 = 0f,
			m33 = 1f
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Rotate(Quaternion q)
	{
		float num = q.x * 2f;
		float num2 = q.y * 2f;
		float num3 = q.z * 2f;
		float num4 = q.x * num;
		float num5 = q.y * num2;
		float num6 = q.z * num3;
		float num7 = q.x * num2;
		float num8 = q.x * num3;
		float num9 = q.y * num3;
		float num10 = q.w * num;
		float num11 = q.w * num2;
		float num12 = q.w * num3;
		Matrix4x4 result = default(Matrix4x4);
		result.m00 = 1f - (num5 + num6);
		result.m10 = num7 + num12;
		result.m20 = num8 - num11;
		result.m30 = 0f;
		result.m01 = num7 - num12;
		result.m11 = 1f - (num4 + num6);
		result.m21 = num9 + num10;
		result.m31 = 0f;
		result.m02 = num8 + num11;
		result.m12 = num9 - num10;
		result.m22 = 1f - (num4 + num5);
		result.m32 = 0f;
		result.m03 = 0f;
		result.m13 = 0f;
		result.m23 = 0f;
		result.m33 = 1f;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Rotate(in Quaternion q)
	{
		float num = q.x * 2f;
		float num2 = q.y * 2f;
		float num3 = q.z * 2f;
		float num4 = q.x * num;
		float num5 = q.y * num2;
		float num6 = q.z * num3;
		float num7 = q.x * num2;
		float num8 = q.x * num3;
		float num9 = q.y * num3;
		float num10 = q.w * num;
		float num11 = q.w * num2;
		float num12 = q.w * num3;
		Matrix4x4 result = default(Matrix4x4);
		result.m00 = 1f - (num5 + num6);
		result.m10 = num7 + num12;
		result.m20 = num8 - num11;
		result.m30 = 0f;
		result.m01 = num7 - num12;
		result.m11 = 1f - (num4 + num6);
		result.m21 = num9 + num10;
		result.m31 = 0f;
		result.m02 = num8 + num11;
		result.m12 = num9 - num10;
		result.m22 = 1f - (num4 + num5);
		result.m32 = 0f;
		result.m03 = 0f;
		result.m13 = 0f;
		result.m23 = 0f;
		result.m33 = 1f;
		return result;
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
		return $"{m00.ToString(format, formatProvider)}\t{m01.ToString(format, formatProvider)}\t{m02.ToString(format, formatProvider)}\t{m03.ToString(format, formatProvider)}\n{m10.ToString(format, formatProvider)}\t{m11.ToString(format, formatProvider)}\t{m12.ToString(format, formatProvider)}\t{m13.ToString(format, formatProvider)}\n{m20.ToString(format, formatProvider)}\t{m21.ToString(format, formatProvider)}\t{m22.ToString(format, formatProvider)}\t{m23.ToString(format, formatProvider)}\n{m30.ToString(format, formatProvider)}\t{m31.ToString(format, formatProvider)}\t{m32.ToString(format, formatProvider)}\t{m33.ToString(format, formatProvider)}\n";
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRotation_Injected(ref Matrix4x4 _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLossyScale_Injected(ref Matrix4x4 _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DecomposeProjection_Injected(ref Matrix4x4 _unity_self, out FrustumPlanes ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_TRS_Injected(in Vector3 pos, in Quaternion q, in Vector3 s, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Inverse_Injected(in Matrix4x4 m, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Transpose_Injected(in Matrix4x4 m, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Ortho_Injected(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Perspective_Injected(float fov, float aspect, float zNear, float zFar, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_LookAt_Injected(in Vector3 from, in Vector3 to, in Vector3 up, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Frustum_Injected(float left, float right, float bottom, float top, float zNear, float zFar, out Matrix4x4 ret);
}
