using System;
using UnityEngine;

namespace Brimstone.Math;

public struct Matrix3 : IEquatable<Matrix3>
{
	private readonly float[,] e;

	public float e00 => e[0, 0];

	public float e01 => e[0, 1];

	public float e02 => e[0, 2];

	public float e10 => e[1, 0];

	public float e11 => e[1, 1];

	public float e12 => e[1, 2];

	public float e20 => e[2, 0];

	public float e21 => e[2, 1];

	public float e22 => e[2, 2];

	public float this[int i, int j]
	{
		get
		{
			return e[i, j];
		}
		set
		{
			e[i, j] = value;
		}
	}

	public static Matrix3 operator +(Matrix3 matrix, float scalar)
	{
		return new Matrix3(matrix[0, 0] + scalar, matrix[0, 1] + scalar, matrix[0, 2] + scalar, matrix[1, 0] + scalar, matrix[1, 1] + scalar, matrix[1, 2] + scalar, matrix[2, 0] + scalar, matrix[2, 1] + scalar, matrix[2, 2] + scalar);
	}

	public static Matrix3 operator +(float scalar, Matrix3 matrix)
	{
		return matrix + scalar;
	}

	public static Matrix3 operator +(Matrix3 a, Matrix3 b)
	{
		return new Matrix3(a[0, 0] + b[0, 0], a[0, 1] + b[0, 1], a[0, 2] + b[0, 2], a[1, 0] + b[1, 0], a[1, 1] + b[1, 1], a[1, 2] + b[1, 2], a[2, 0] + b[2, 0], a[2, 1] + b[2, 1], a[2, 2] + b[2, 2]);
	}

	public static Matrix3 operator -(Matrix3 matrix, float scalar)
	{
		return matrix + (0f - scalar);
	}

	public static Matrix3 operator -(float scalar, Matrix3 matrix)
	{
		return matrix + (0f - scalar);
	}

	public static Matrix3 operator -(Matrix3 a, Matrix3 b)
	{
		return new Matrix3(a[0, 0] - b[0, 0], a[0, 1] - b[0, 1], a[0, 2] - b[0, 2], a[1, 0] - b[1, 0], a[1, 1] - b[1, 1], a[1, 2] - b[1, 2], a[2, 0] - b[2, 0], a[2, 1] - b[2, 1], a[2, 2] - b[2, 2]);
	}

	public static Matrix3 operator -(Matrix3 matrix)
	{
		return new Matrix3(0f - matrix[0, 0], 0f - matrix[0, 1], 0f - matrix[0, 2], 0f - matrix[1, 0], 0f - matrix[1, 1], 0f - matrix[1, 2], 0f - matrix[2, 0], 0f - matrix[2, 1], 0f - matrix[2, 2]);
	}

	public static Matrix3 operator *(Matrix3 matrix, float scalar)
	{
		return new Matrix3(matrix[0, 0] * scalar, matrix[0, 1] * scalar, matrix[0, 2] * scalar, matrix[1, 0] * scalar, matrix[1, 1] * scalar, matrix[1, 2] * scalar, matrix[2, 0] * scalar, matrix[2, 1] * scalar, matrix[2, 2] * scalar);
	}

	public static Matrix3 operator *(float scalar, Matrix3 matrix)
	{
		return matrix * scalar;
	}

	public static Matrix3 operator *(Matrix3 a, Matrix3 b)
	{
		return new Matrix3(a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0], a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1], a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2], a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0], a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1], a[1, 0] * b[0, 2] + a[1, 1] * b[1, 2] + a[1, 2] * b[2, 2], a[2, 0] * b[0, 0] + a[2, 1] * b[1, 0] + a[2, 2] * b[2, 0], a[2, 0] * b[0, 1] + a[2, 1] * b[1, 1] + a[2, 2] * b[2, 1], a[2, 0] * b[0, 2] + a[2, 1] * b[1, 2] + a[2, 2] * b[2, 2]);
	}

	public static Matrix3 operator /(Matrix3 matrix, float scalar)
	{
		return matrix * (1f / scalar);
	}

	public static Vector3 operator *(Matrix3 matrix, Vector3 vector)
	{
		return new Vector3(matrix[0, 0] * vector[0] + matrix[0, 1] * vector[1] + matrix[0, 2] * vector[2], matrix[1, 0] * vector[0] + matrix[1, 1] * vector[1] + matrix[1, 2] * vector[2], matrix[2, 0] * vector[0] + matrix[2, 1] * vector[1] + matrix[2, 2] * vector[2]);
	}

	public static bool operator ==(Matrix3 matrix3, Matrix3 otherMatrix3)
	{
		if (matrix3[0, 0] == otherMatrix3[0, 0] && matrix3[0, 1] == otherMatrix3[0, 1] && matrix3[0, 2] == otherMatrix3[0, 2] && matrix3[1, 0] == otherMatrix3[1, 0] && matrix3[1, 1] == otherMatrix3[1, 1] && matrix3[1, 2] == otherMatrix3[1, 2] && matrix3[2, 0] == otherMatrix3[2, 0] && matrix3[2, 1] == otherMatrix3[2, 1])
		{
			return matrix3[2, 2] == otherMatrix3[2, 2];
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is Matrix3 other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(Matrix3 other)
	{
		return e == other.e;
	}

	public override int GetHashCode()
	{
		return -572131872 + e.GetHashCode();
	}

	public override string ToString()
	{
		return "(" + e00 + ", " + e01 + ", " + e02 + "; " + e10 + ", " + e11 + ", " + e12 + "; " + e20 + ", " + e21 + ", " + e22 + ")";
	}

	public static bool operator !=(Matrix3 matrix3, Matrix3 otherMatrix3)
	{
		return !(matrix3 == otherMatrix3);
	}

	public Matrix3(float e00, float e01, float e02, float e10, float e11, float e12, float e20, float e21, float e22)
	{
		e = new float[3, 3];
		e[0, 0] = e00;
		e[0, 1] = e01;
		e[0, 2] = e02;
		e[1, 0] = e10;
		e[1, 1] = e11;
		e[1, 2] = e12;
		e[2, 0] = e20;
		e[2, 1] = e21;
		e[2, 2] = e22;
	}

	public static Matrix3 FromColumns(Vector3 column0, Vector3 column1, Vector3 column2)
	{
		return new Matrix3(column0[0], column1[0], column2[0], column0[1], column1[1], column2[1], column0[2], column1[2], column2[2]);
	}

	public static Matrix3 FromRows(Vector3 row0, Vector3 row1, Vector3 row2)
	{
		return new Matrix3(row0[0], row0[1], row0[2], row1[0], row1[1], row1[2], row2[0], row2[1], row2[2]);
	}

	public static Matrix3 CreateDiagonal(Vector3 diagonalElements)
	{
		return new Matrix3(diagonalElements[0], 0f, 0f, 0f, diagonalElements[1], 0f, 0f, 0f, diagonalElements[2]);
	}

	public static Matrix3 CreateRotation(Quaternion fromQuaternion)
	{
		float num = 1f / fromQuaternion.SqrMagnitude() * 2f;
		float num2 = num * fromQuaternion.x * fromQuaternion.x;
		float num3 = num * fromQuaternion.y * fromQuaternion.y;
		float num4 = num * fromQuaternion.z * fromQuaternion.z;
		float num5 = num * fromQuaternion.w * fromQuaternion.x;
		float num6 = num * fromQuaternion.w * fromQuaternion.y;
		float num7 = num * fromQuaternion.w * fromQuaternion.z;
		float num8 = num * fromQuaternion.x * fromQuaternion.y;
		float num9 = num * fromQuaternion.x * fromQuaternion.z;
		float num10 = num * fromQuaternion.y * fromQuaternion.z;
		return new Matrix3(1f - (num3 + num4), num8 - num7, num9 + num6, num8 + num7, 1f - (num2 + num4), num10 - num5, num9 - num6, num10 + num5, 1f - (num2 + num3));
	}

	public Vector3 GetColumn(int j)
	{
		return new Vector3(e[0, j], e[1, j], e[2, j]);
	}

	public Vector3 GetRow(int i)
	{
		return new Vector3(e[i, 0], e[i, 1], e[i, 2]);
	}

	public bool IsSymmetric()
	{
		if (e01 == e10 && e02 == e20)
		{
			return e12 == e21;
		}
		return false;
	}

	public bool IsAntisymmetric()
	{
		if (e00 == 0f && e11 == 0f && e22 == 0f && e01 == 0f - e10 && e02 == 0f - e20)
		{
			return e12 == 0f - e21;
		}
		return false;
	}

	public bool Approximately(Matrix3 otherMatrix)
	{
		if (e[0, 0].Approximately(otherMatrix[0, 0]) && e[0, 1].Approximately(otherMatrix[0, 1]) && e[0, 2].Approximately(otherMatrix[0, 2]) && e[1, 0].Approximately(otherMatrix[1, 0]) && e[1, 1].Approximately(otherMatrix[1, 1]) && e[1, 2].Approximately(otherMatrix[1, 2]) && e[2, 0].Approximately(otherMatrix[2, 0]) && e[2, 1].Approximately(otherMatrix[2, 1]))
		{
			return e[2, 2].Approximately(otherMatrix[2, 2]);
		}
		return false;
	}

	public float Determinant()
	{
		return e00 * e11 * e22 + e01 * e12 * e20 + e02 * e10 * e21 - e20 * e11 * e02 - e21 * e12 * e00 - e22 * e10 * e01;
	}

	public Matrix3 Transpose()
	{
		return new Matrix3(e00, e10, e20, e01, e11, e21, e02, e12, e22);
	}

	public bool TryInvert(out Matrix3 inverse)
	{
		float num = Determinant();
		if (num.Approximately(0f))
		{
			inverse = default(Matrix3);
			return false;
		}
		float num2 = 1f / num;
		inverse = new Matrix3((e11 * e22 - e12 * e21) * num2, (e02 * e21 - e01 * e22) * num2, (e01 * e12 - e02 * e11) * num2, (e12 * e20 - e10 * e22) * num2, (e00 * e22 - e02 * e20) * num2, (e02 * e10 - e00 * e12) * num2, (e10 * e21 - e11 * e20) * num2, (e01 * e20 - e00 * e21) * num2, (e00 * e11 - e01 * e10) * num2);
		return true;
	}

	public bool DiagonalizeSymmetric(out Vector3 diagonal, out Vector3 direction0, out Vector3 direction1, out Vector3 direction2)
	{
		diagonal = (direction0 = (direction1 = (direction2 = default(Vector3))));
		if (!IsSymmetric())
		{
			return false;
		}
		float num = e01 * e12;
		float num2 = e01 * e01;
		float num3 = e12 * e12;
		float num4 = e02 * e02;
		float num5 = e00 + e11 + e22;
		float num6 = e22 * num2 + e00 * num3 + e11 * num4 - e00 * e11 * e22 - 2f * e02 * num;
		float num7 = e00 * e11 + e00 * e22 + e11 * e22 - num2 - num3 - num4;
		float num8 = num5 * num5 - 3f * num7;
		float num9 = num5 * (num8 - 1.5f * num7) - 13.5f * num6;
		float num10 = BMath.Sqrt(BMath.Abs(num8));
		float value = 27f * (0.25f * num7 * num7 * (num8 - num7) + num6 * (num9 + 6.75f * num6));
		value = BMath.Atan2(BMath.Sqrt(BMath.Abs(value)), num9) / 3f;
		float num11 = num10 * BMath.Cos(value);
		float num12 = 0.57735026f * num10 * BMath.Sin(value);
		diagonal[1] = (num5 - num11) / 3f;
		diagonal[2] = diagonal[1] + num12;
		diagonal[0] = diagonal[1] + num11;
		diagonal[1] -= num12;
		if (diagonal.IsAnyNaN() || diagonal.IsAnyInfinity())
		{
			return DiagonalizeSymmetricDoublePrecision(out diagonal, out direction0, out direction1, out direction2);
		}
		float num13 = diagonal.Abs().Max();
		float num14 = 64f * num13 * num13 * BMath.Epsilon * BMath.Epsilon;
		float num15 = num2 + num4;
		float num16 = num2 + num3;
		direction1[0] = e01 * e12 - e02 * e11;
		direction1[1] = e02 * e01 - e12 * e00;
		direction1[2] = num2;
		float num17 = e00 - diagonal[0];
		float num18 = e11 - diagonal[0];
		direction0[0] = direction1[0] + e02 * diagonal[0];
		direction0[1] = direction1[1] + e12 * diagonal[0];
		direction0[2] = num17 * num18 - direction1[2];
		float sqrMagnitude = direction0.sqrMagnitude;
		float num19 = num15 + num17 * num17;
		float num20 = num16 + num18 * num18;
		float num21 = num19 * num20;
		if (num19 <= num14)
		{
			direction0[0] = 1f;
			direction0[1] = 0f;
			direction0[2] = 0f;
		}
		else if (num20 <= num14)
		{
			direction0[0] = 0f;
			direction0[1] = 1f;
			direction0[2] = 0f;
		}
		else if (sqrMagnitude < 4096f * BMath.Epsilon * BMath.Epsilon * num21)
		{
			float num22 = num2;
			float num23 = (0f - num17) / e01;
			if (num18 * num18 > num22)
			{
				num22 = num18 * num18;
				num23 = (0f - e01) / num18;
			}
			if (num3 > num22)
			{
				num23 = (0f - e02) / e12;
			}
			sqrMagnitude = (direction0[0] = 1f / BMath.Sqrt(1f + num23 * num23));
			direction0[1] = num23 * sqrMagnitude;
			direction0[2] = 0f;
		}
		else
		{
			sqrMagnitude = BMath.Sqrt(1f / sqrMagnitude);
			for (int i = 0; i < 3; i++)
			{
				direction0[i] *= sqrMagnitude;
			}
		}
		float num25 = diagonal[0] - diagonal[1];
		if (BMath.Abs(num25) > 8f * BMath.Epsilon * num13)
		{
			num17 += num25;
			num18 += num25;
			direction1[0] += e02 * diagonal[1];
			direction1[1] += e12 * diagonal[1];
			direction1[2] = num17 * num18 - direction1[2];
			sqrMagnitude = direction1.sqrMagnitude;
			num19 = num15 + num17 * num17;
			num20 = num16 + num18 * num18;
			num21 = num19 * num20;
			if (num19 <= num14)
			{
				direction1[0] = 1f;
				direction1[1] = 0f;
				direction1[2] = 0f;
			}
			else if (num20 <= num14)
			{
				direction1[0] = 0f;
				direction1[1] = 1f;
				direction1[2] = 0f;
			}
			else if (sqrMagnitude < 4096f * BMath.Epsilon * BMath.Epsilon * num21)
			{
				float num26 = num2;
				float num27 = (0f - num17) / e01;
				if (num18 * num18 > num26)
				{
					num26 = num18 * num18;
					num27 = (0f - e01) / num18;
				}
				if (num3 > num26)
				{
					num27 = (0f - e02) / e12;
				}
				sqrMagnitude = (direction1[0] = 1f / BMath.Sqrt(1f + num27 * num27));
				direction1[1] = num27 * sqrMagnitude;
				direction1[2] = 0f;
			}
			else
			{
				sqrMagnitude = BMath.Sqrt(1f / sqrMagnitude);
				for (int j = 0; j < 3; j++)
				{
					direction1[j] *= sqrMagnitude;
				}
			}
		}
		else
		{
			int k;
			for (k = 0; k < 3; k++)
			{
				num19 = GetColumn(k).sqrMagnitude;
				if (!(num19 > num14))
				{
					continue;
				}
				direction1[0] = direction0[1] * e[2, k] - e[2, 0] * e[1, k];
				direction1[1] = direction0[2] * e[0, k] - e[0, 0] * e[2, k];
				direction1[2] = direction0[0] * e[1, k] - e[1, 0] * e[0, k];
				sqrMagnitude = direction1.sqrMagnitude;
				if (sqrMagnitude > 65536f * BMath.Epsilon * BMath.Epsilon * num19)
				{
					sqrMagnitude = BMath.Sqrt(1f / sqrMagnitude);
					for (int l = 0; l < 3; l++)
					{
						direction1[l] *= sqrMagnitude;
					}
					break;
				}
			}
			if (k == 3)
			{
				for (int l = 0; l < 3; l++)
				{
					if ((double)direction0[l] != 0.0)
					{
						int num29 = (l + 1) % 3;
						sqrMagnitude = 1f / BMath.Sqrt(direction0[l] * direction0[l] + direction0[num29] * direction0[num29]);
						direction1[l] = direction0[num29] * sqrMagnitude;
						direction1[num29] = (0f - direction0[l]) * sqrMagnitude;
						direction1[(num29 + 1) % 3] = 0f;
						break;
					}
				}
			}
		}
		direction2 = Vector3.Cross(direction0, direction1);
		if (BMath.Abs(direction0.sqrMagnitude - 1f) > 2E-05f)
		{
			direction0.Normalize();
		}
		if (BMath.Abs(direction1.sqrMagnitude - 1f) > 2E-05f)
		{
			direction1.Normalize();
		}
		if (BMath.Abs(direction2.sqrMagnitude - 1f) > 2E-05f)
		{
			direction2.Normalize();
		}
		return true;
	}

	private bool DiagonalizeSymmetricDoublePrecision(out Vector3 diagonal, out Vector3 direction0, out Vector3 direction1, out Vector3 direction2)
	{
		diagonal = (direction0 = (direction1 = (direction2 = default(Vector3))));
		double[] array = new double[3];
		double[] array2 = new double[3];
		double[] array3 = new double[3];
		if (!IsSymmetric())
		{
			return false;
		}
		double num = e01 * e12;
		double num2 = e01 * e01;
		double num3 = e12 * e12;
		double num4 = e02 * e02;
		double num5 = e00 + e11 + e22;
		double num6 = (double)e22 * num2 + (double)e00 * num3 + (double)e11 * num4 - (double)(e00 * e11 * e22) - 2.0 * (double)e02 * num;
		double num7 = (double)(e00 * e11 + e00 * e22 + e11 * e22) - num2 - num3 - num4;
		double num8 = num5 * num5 - 3.0 * num7;
		double num9 = num5 * (num8 - 1.5 * num7) - 13.5 * num6;
		double num10 = System.Math.Sqrt(System.Math.Abs(num8));
		double value = 27.0 * (0.25 * num7 * num7 * (num8 - num7) + num6 * (num9 + 6.75 * num6));
		value = System.Math.Atan2(System.Math.Sqrt(System.Math.Abs(value)), num9) / 3.0;
		double num11 = num10 * System.Math.Cos(value);
		double num12 = 0.5773502588272095 * num10 * System.Math.Sin(value);
		array[1] = (num5 - num11) / 3.0;
		array[2] = array[1] + num12;
		array[0] = array[1] + num11;
		array[1] -= num12;
		diagonal = new Vector3((float)array[0], (float)array[1], (float)array[2]);
		if (diagonal.IsAnyNaN() || diagonal.IsAnyInfinity())
		{
			throw new NotFiniteNumberException();
		}
		double num13 = diagonal.Abs().Max();
		double num14 = 64.0 * num13 * num13 * (double)BMath.Epsilon * (double)BMath.Epsilon;
		double num15 = num2 + num4;
		double num16 = num2 + num3;
		array3[0] = e01 * e12 - e02 * e11;
		array3[1] = e02 * e01 - e12 * e00;
		array3[2] = num2;
		double num17 = (double)e00 - array[0];
		double num18 = (double)e11 - array[0];
		array2[0] = array3[0] + (double)e02 * array[0];
		array2[1] = array3[1] + (double)e12 * array[0];
		array2[2] = num17 * num18 - array3[2];
		double num19 = array2[0] * array2[0] + array2[1] * array2[1] + array2[2] * array2[2];
		double num20 = num15 + num17 * num17;
		double num21 = num16 + num18 * num18;
		double num22 = num20 * num21;
		if (num20 <= num14)
		{
			array2[0] = 1.0;
			array2[1] = 0.0;
			array2[2] = 0.0;
		}
		else if (num21 <= num14)
		{
			array2[0] = 0.0;
			array2[1] = 1.0;
			array2[2] = 0.0;
		}
		else if (num19 < 4096.0 * (double)BMath.Epsilon * (double)BMath.Epsilon * num22)
		{
			double num23 = num2;
			double num24 = (0.0 - num17) / (double)e01;
			if (num18 * num18 > num23)
			{
				num23 = num18 * num18;
				num24 = (double)(0f - e01) / num18;
			}
			if (num3 > num23)
			{
				num24 = (0f - e02) / e12;
			}
			array2[1] = num24 * (array2[0] = 1.0 / System.Math.Sqrt(1.0 + num24 * num24));
			array2[2] = 0.0;
		}
		else
		{
			num19 = System.Math.Sqrt(1.0 / num19);
			for (int i = 0; i < 3; i++)
			{
				array2[i] *= num19;
			}
		}
		double num25 = array[0] - array[1];
		if (System.Math.Abs(num25) > 8.0 * (double)BMath.Epsilon * num13)
		{
			num17 += num25;
			num18 += num25;
			array3[0] += (double)e02 * array[1];
			array3[1] += (double)e12 * array[1];
			array3[2] = num17 * num18 - array3[2];
			num19 = array3[0] * array3[0] + array3[1] * array3[1] + array3[2] * array3[2];
			num20 = num15 + num17 * num17;
			num21 = num16 + num18 * num18;
			num22 = num20 * num21;
			if (num20 <= num14)
			{
				array3[0] = 1.0;
				array3[1] = 0.0;
				array3[2] = 0.0;
			}
			else if (num21 <= num14)
			{
				array3[0] = 0.0;
				array3[1] = 1.0;
				array3[2] = 0.0;
			}
			else if (num19 < 4096.0 * (double)BMath.Epsilon * (double)BMath.Epsilon * num22)
			{
				double num26 = num2;
				double num27 = (0.0 - num17) / (double)e01;
				if (num18 * num18 > num26)
				{
					num26 = num18 * num18;
					num27 = (double)(0f - e01) / num18;
				}
				if (num3 > num26)
				{
					num27 = (0f - e02) / e12;
				}
				array3[1] = num27 * (array3[0] = 1.0 / System.Math.Sqrt(1.0 + num27 * num27));
				array3[2] = 0.0;
			}
			else
			{
				num19 = System.Math.Sqrt(1.0 / num19);
				for (int j = 0; j < 3; j++)
				{
					array3[j] *= num19;
				}
			}
		}
		else
		{
			int k;
			for (k = 0; k < 3; k++)
			{
				num20 = GetColumn(k).sqrMagnitude;
				if (!(num20 > num14))
				{
					continue;
				}
				array3[0] = array2[1] * (double)e[2, k] - (double)(e[2, 0] * e[1, k]);
				array3[1] = array2[2] * (double)e[0, k] - (double)(e[0, 0] * e[2, k]);
				array3[2] = array2[0] * (double)e[1, k] - (double)(e[1, 0] * e[0, k]);
				num19 = direction1.sqrMagnitude;
				if (num19 > 65536.0 * (double)BMath.Epsilon * (double)BMath.Epsilon * num20)
				{
					num19 = System.Math.Sqrt(1.0 / num19);
					for (int l = 0; l < 3; l++)
					{
						array3[l] *= num19;
					}
					break;
				}
			}
			if (k == 3)
			{
				for (int l = 0; l < 3; l++)
				{
					if (array2[l] != 0.0)
					{
						int num28 = (l + 1) % 3;
						num19 = 1.0 / System.Math.Sqrt(array2[l] * array2[l] + array2[num28] * array2[num28]);
						array3[l] = array2[num28] * num19;
						array3[num28] = (0.0 - array2[l]) * num19;
						array3[(num28 + 1) % 3] = 0.0;
						break;
					}
				}
			}
		}
		direction0 = new Vector3((float)array2[0], (float)array2[1], (float)array2[2]);
		direction1 = new Vector3((float)array3[0], (float)array3[1], (float)array3[2]);
		direction2 = Vector3.Cross(direction0, direction1);
		if (System.Math.Abs((double)direction0.sqrMagnitude - 1.0) > 2E-05)
		{
			direction0.Normalize();
		}
		if (System.Math.Abs((double)direction1.sqrMagnitude - 1.0) > 2E-05)
		{
			direction1.Normalize();
		}
		if (System.Math.Abs((double)direction2.sqrMagnitude - 1.0) > 2E-05)
		{
			direction2.Normalize();
		}
		return true;
	}
}
