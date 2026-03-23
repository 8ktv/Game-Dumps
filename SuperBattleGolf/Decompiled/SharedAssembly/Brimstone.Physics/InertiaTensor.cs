using System;
using Brimstone.Math;
using UnityEngine;

namespace Brimstone.Physics;

[Serializable]
public struct InertiaTensor
{
	public float Ixx;

	public float Ixy;

	public float Ixz;

	public float Iyy;

	public float Iyz;

	public float Izz;

	public static readonly InertiaTensor zero = new InertiaTensor(0f, 0f, 0f, 0f, 0f, 0f);

	public override readonly string ToString()
	{
		return "Ixx: " + Ixx + ", Iyy: " + Iyy + ", Izz: " + Izz + ", Ixy: " + Ixy + ", Ixz: " + Ixz + ", Iyz: " + Iyz;
	}

	public InertiaTensor(float Ixx, float Ixy, float Ixz, float Iyy, float Iyz, float Izz)
	{
		this.Ixx = Ixx;
		this.Ixy = Ixy;
		this.Ixz = Ixz;
		this.Iyy = Iyy;
		this.Iyz = Iyz;
		this.Izz = Izz;
	}

	public bool IsValid()
	{
		if (!float.IsNaN(Ixx) && !float.IsInfinity(Ixx) && !float.IsNaN(Ixy) && !float.IsInfinity(Ixy) && !float.IsNaN(Ixz) && !float.IsInfinity(Ixz) && !float.IsNaN(Iyy) && !float.IsInfinity(Iyy) && !float.IsNaN(Iyz) && !float.IsInfinity(Iyz) && !float.IsNaN(Izz))
		{
			return !float.IsInfinity(Izz);
		}
		return false;
	}

	public static InertiaTensor FromPointMass(float mass, Vector3 massPoint, Vector3 relativeTo)
	{
		if (mass < 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Cannot create an inertia tensor with negative mass.");
		}
		if (mass == 0f)
		{
			return zero;
		}
		Vector3 vector = massPoint - relativeTo;
		float num = vector.x * vector.x;
		float num2 = vector.y * vector.y;
		float num3 = vector.z * vector.z;
		float ixx = mass * (num2 + num3);
		float iyy = mass * (num + num3);
		float izz = mass * (num + num2);
		return new InertiaTensor(ixx, (0f - mass) * vector.x * vector.y, (0f - mass) * vector.x * vector.z, iyy, (0f - mass) * vector.y * vector.z, izz);
	}

	public static InertiaTensor FromSphere(float mass, Vector3 sphereCenter, float sphereRadius, Vector3 relativeTo)
	{
		if (mass < 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Cannot create an inertia tensor with negative mass.");
		}
		if (sphereRadius < 0f)
		{
			throw new ArgumentOutOfRangeException("sphereRadius", "Cannot create an inertia tensor from a sphere with a negative radius.");
		}
		if (mass == 0f)
		{
			return zero;
		}
		InertiaTensor result = FromPrincipal(0.4f * mass * sphereRadius * sphereRadius * Vector3.one, Quaternion.identity);
		if (relativeTo - sphereCenter != Vector3.zero)
		{
			result.TranslateFromCenterOfMass(sphereCenter, relativeTo, mass);
		}
		return result;
	}

	public static InertiaTensor FromBox(float mass, Vector3 boxCenter, Vector3 boxSize, Vector3 relativeTo)
	{
		if (mass < 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Cannot create an inertia tensor with negative mass.");
		}
		if (boxSize.x < 0f || boxSize.y < 0f || boxSize.z < 0f)
		{
			throw new ArgumentOutOfRangeException("boxSize", "Cannot create an inertia tensor from a box with a negative size.");
		}
		if (mass == 0f)
		{
			return zero;
		}
		float num = mass / 12f;
		float num2 = num * boxSize.x * boxSize.x;
		float num3 = num * boxSize.y * boxSize.y;
		float num4 = num * boxSize.z * boxSize.z;
		InertiaTensor result = FromPrincipal(new Vector3(num3 + num4, num2 + num4, num2 + num3), Quaternion.identity);
		if (relativeTo - boxCenter != Vector3.zero)
		{
			result.TranslateFromCenterOfMass(boxCenter, relativeTo, mass);
		}
		return result;
	}

	public static InertiaTensor FromCube(float mass, Vector3 cubeCenter, float sideLength, Vector3 relativeTo)
	{
		if (mass < 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Cannot create an inertia tensor with negative mass.");
		}
		if (sideLength < 0f)
		{
			throw new ArgumentOutOfRangeException("sideLength", "Cannot create an inertia tensor from a cube with a negative side length.");
		}
		if (mass == 0f)
		{
			return zero;
		}
		InertiaTensor result = FromPrincipal(mass * sideLength * sideLength / 6f * Vector3.one, Quaternion.identity);
		if (relativeTo - cubeCenter != Vector3.zero)
		{
			result.TranslateFromCenterOfMass(cubeCenter, relativeTo, mass);
		}
		return result;
	}

	public static InertiaTensor FromPrincipal(Vector3 principalMomentsOfInertia, Quaternion principalAxesRotation)
	{
		Matrix3 matrix = Matrix3.CreateRotation(principalAxesRotation);
		Matrix3 matrix2 = matrix * Matrix3.CreateDiagonal(principalMomentsOfInertia) * matrix.Transpose();
		return new InertiaTensor(matrix2[0, 0], Iyy: matrix2[1, 1], Izz: matrix2[2, 2], Ixy: matrix2[0, 1], Ixz: matrix2[0, 2], Iyz: matrix2[1, 2]);
	}

	public static InertiaTensor operator +(InertiaTensor a, InertiaTensor b)
	{
		return new InertiaTensor(a.Ixx + b.Ixx, a.Ixy + b.Ixy, a.Ixz + b.Ixz, a.Iyy + b.Iyy, a.Iyz + b.Iyz, a.Izz + b.Izz);
	}

	public static InertiaTensor operator -(InertiaTensor a, InertiaTensor b)
	{
		return new InertiaTensor(a.Ixx - b.Ixx, a.Ixy - b.Ixy, a.Ixz - b.Ixz, a.Iyy - b.Iyy, a.Iyz - b.Iyz, a.Izz - b.Izz);
	}

	public static InertiaTensor operator *(InertiaTensor tensor, float factor)
	{
		return new InertiaTensor(tensor.Ixx * factor, tensor.Ixy * factor, tensor.Ixz * factor, tensor.Iyy * factor, tensor.Iyz * factor, tensor.Izz * factor);
	}

	public static InertiaTensor operator *(float factor, InertiaTensor tensor)
	{
		return new InertiaTensor(tensor.Ixx * factor, tensor.Ixy * factor, tensor.Ixz * factor, tensor.Iyy * factor, tensor.Iyz * factor, tensor.Izz * factor);
	}

	public static InertiaTensor operator /(InertiaTensor tensor, float divisor)
	{
		return tensor * (1f / divisor);
	}

	public static explicit operator Matrix3(InertiaTensor tensor)
	{
		return new Matrix3(tensor.Ixx, tensor.Ixy, tensor.Ixz, tensor.Ixy, tensor.Iyy, tensor.Iyz, tensor.Ixz, tensor.Iyz, tensor.Izz);
	}

	public readonly bool TryInvert(out Matrix3 inverse)
	{
		return ((Matrix3)this).TryInvert(out inverse);
	}

	public void TranslateFromCenterOfMass(Vector3 centerOfMass, Vector3 newPoint, float mass)
	{
		if (!newPoint.Approximately(centerOfMass))
		{
			Vector3 vector = newPoint - centerOfMass;
			float num = mass * vector.sqrMagnitude;
			Ixx += num - mass * vector.x * vector.x;
			Iyy += num - mass * vector.y * vector.y;
			Izz += num - mass * vector.z * vector.z;
			Ixy -= mass * vector.x * vector.y;
			Ixz -= mass * vector.x * vector.z;
			Iyz -= mass * vector.y * vector.z;
		}
	}

	public void TranslateToCenterOfMass(Vector3 originalPoint, Vector3 centerOfMass, float mass)
	{
		if (!originalPoint.Approximately(centerOfMass))
		{
			Vector3 vector = originalPoint - centerOfMass;
			float num = mass * vector.sqrMagnitude;
			Ixx -= num - mass * vector.x * vector.x;
			Iyy -= num - mass * vector.y * vector.y;
			Izz -= num - mass * vector.z * vector.z;
			Ixy += mass * vector.x * vector.y;
			Ixz += mass * vector.x * vector.z;
			Iyz += mass * vector.y * vector.z;
		}
	}

	public readonly bool ToPrincipal(out Vector3 principalMomentsOfInertia, out Quaternion principalRotation)
	{
		Vector3 direction;
		Vector3 direction2;
		Vector3 direction3;
		bool num = new Matrix3(Ixx, Ixy, Ixz, Ixy, Iyy, Iyz, Ixz, Iyz, Izz).DiagonalizeSymmetric(out principalMomentsOfInertia, out direction, out direction2, out direction3);
		if (principalMomentsOfInertia.IsAnyNonPositive())
		{
			throw new InvalidOperationException("Inertia tensor diagonlization resulted in a non-positive principal moment of inertia. This is not physically possible.");
		}
		if (!num)
		{
			principalRotation = default(Quaternion);
			return false;
		}
		principalRotation = Quaternion.LookRotation(direction, direction2);
		return true;
	}
}
