using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct PhysicsMath
{
	public static float PI => PhysicsLowLevelScripting2D.PhysicsMath_PI();

	public static float TAU => PhysicsLowLevelScripting2D.PhysicsMath_TAU();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToDegrees(float radians)
	{
		return PhysicsLowLevelScripting2D.PhysicsMath_ToDegrees(radians);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToRadians(float degrees)
	{
		return PhysicsLowLevelScripting2D.PhysicsMath_ToRadians(degrees);
	}

	public static float Atan2(float y, float x)
	{
		return PhysicsLowLevelScripting2D.PhysicsMath_Atan2(y, x);
	}

	public static void CosSin(float angle, out float cosine, out float sine)
	{
		PhysicsLowLevelScripting2D.PhysicsMath_CosSin(angle, out cosine, out sine);
	}

	public static Vector2 CosSin(float angle)
	{
		CosSin(angle, out var cosine, out var sine);
		return new Vector2(cosine, sine);
	}

	public static float SpringDamper(float frequency, float damping, float translation, float speed, float deltaTime)
	{
		return PhysicsLowLevelScripting2D.PhysicsMath_SpringDamper(frequency, damping, translation, speed, deltaTime);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float MinAbsComponent(Vector2 vector)
	{
		return Math.Min(Math.Abs(vector.x), Math.Abs(vector.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float MinAbsComponent(Vector3 vector)
	{
		return Math.Min(Math.Min(Math.Abs(vector.x), Math.Abs(vector.y)), Math.Abs(vector.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float MaxAbsComponent(Vector2 vector)
	{
		return Math.Max(Math.Abs(vector.x), Math.Abs(vector.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float MaxAbsComponent(Vector3 vector)
	{
		return Math.Max(Math.Max(Math.Abs(vector.x), Math.Abs(vector.y)), Math.Abs(vector.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetTranslationAxes(PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (1 == 0)
		{
		}
		Vector3 result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => Vector3.right + Vector3.up, 
			PhysicsWorld.TransformPlane.XZ => Vector3.right + Vector3.forward, 
			PhysicsWorld.TransformPlane.ZY => Vector3.up + Vector3.forward, 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetTranslationIgnoredAxes(PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (1 == 0)
		{
		}
		Vector3 result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => Vector3.forward, 
			PhysicsWorld.TransformPlane.XZ => Vector3.up, 
			PhysicsWorld.TransformPlane.ZY => Vector3.right, 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetTranslationIgnoredAxis(Vector3 position, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (1 == 0)
		{
		}
		float result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => position.z, 
			PhysicsWorld.TransformPlane.XZ => position.y, 
			PhysicsWorld.TransformPlane.ZY => position.x, 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetRotationAxes(PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (1 == 0)
		{
		}
		Vector3 result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => Vector3.forward, 
			PhysicsWorld.TransformPlane.XZ => Vector3.up, 
			PhysicsWorld.TransformPlane.ZY => Vector3.right, 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetRotationIgnoredAxes(PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (1 == 0)
		{
		}
		Vector3 result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => Vector3.right + Vector3.up, 
			PhysicsWorld.TransformPlane.XZ => Vector3.right + Vector3.forward, 
			PhysicsWorld.TransformPlane.ZY => Vector3.up + Vector3.forward, 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static Matrix4x4 GetRelativeMatrix(Transform transformFrom, Transform transformTo, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY, bool useScale = true)
	{
		if (transformFrom == transformTo)
		{
			if (useScale)
			{
				return Matrix4x4.TRS(Vector3.zero, Quaternion.identity, transformTo.lossyScale);
			}
			return Matrix4x4.identity;
		}
		Quaternion quaternion = Quaternion.Inverse(ToRotationFast3D(ToRotation2D(transformFrom.rotation, transformPlane), transformPlane));
		Vector3 pos = quaternion * -Swizzle(transformFrom.position, transformPlane);
		Matrix4x4 matrix4x = Matrix4x4.TRS(pos, quaternion, Vector3.one);
		if (useScale)
		{
			return matrix4x * Swizzle(transformTo.localToWorldMatrix, transformPlane);
		}
		return matrix4x * Swizzle(transformTo.localToWorldMatrix * Matrix4x4.Scale(transformTo.localScale).inverse, transformPlane);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Swizzle(Vector3 position, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (1 == 0)
		{
		}
		Vector3 result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => position, 
			PhysicsWorld.TransformPlane.XZ => new Vector3(position.x, position.z, position.y), 
			PhysicsWorld.TransformPlane.ZY => new Vector3(position.z, position.y, position.x), 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4 Swizzle(Vector4 position, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		Vector3 vector = Swizzle(new Vector3(position.x, position.y, position.z), transformPlane);
		return new Vector4(vector.x, vector.y, vector.z, position.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Matrix4x4 Swizzle(Matrix4x4 matrix, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		matrix.SetColumn(3, Swizzle(matrix.GetColumn(3), transformPlane));
		return matrix;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 ToPosition3D(Vector2 position, Vector3 reference, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (1 == 0)
		{
		}
		Vector3 result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => new Vector3(position.x, position.y, reference.z), 
			PhysicsWorld.TransformPlane.XZ => new Vector3(position.x, reference.y, position.y), 
			PhysicsWorld.TransformPlane.ZY => new Vector3(reference.x, position.y, position.x), 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 ToPosition2D(Vector3 position, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (1 == 0)
		{
		}
		Vector2 result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => position, 
			PhysicsWorld.TransformPlane.XZ => new Vector2(position.x, position.z), 
			PhysicsWorld.TransformPlane.ZY => new Vector2(position.z, position.y), 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToRotation2D(Quaternion quaternion, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (quaternion.w < 0f)
		{
			quaternion = new Quaternion(0f - quaternion.x, 0f - quaternion.y, 0f - quaternion.z, 0f - quaternion.w);
		}
		if (1 == 0)
		{
		}
		float result = transformPlane switch
		{
			PhysicsWorld.TransformPlane.XY => 2f * Atan2(quaternion.z, quaternion.w), 
			PhysicsWorld.TransformPlane.XZ => -2f * Atan2(quaternion.y, quaternion.w), 
			PhysicsWorld.TransformPlane.ZY => -2f * Atan2(quaternion.x, quaternion.w), 
			_ => throw new InvalidOperationException("Invalid Transform Plane."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static PhysicsTransform ToPhysicsTransform(Transform transform, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		PhysicsTransform result = new PhysicsTransform();
		result.position = ToPosition2D(transform.position, transformPlane);
		result.rotation = new PhysicsRotate(ToRotation2D(transform.rotation, transformPlane));
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion AngularVelocityToQuaternion(float angularVelocity, float deltaTime, PhysicsWorld.TransformPlane transformPlane)
	{
		float num = Mathf.Abs(angularVelocity);
		if (num < 1E-05f)
		{
			return Quaternion.identity;
		}
		PhysicsRotate physicsRotate = new PhysicsRotate(num * deltaTime * 0.5f);
		Vector3 vector = Swizzle(new Vector3(0f, 0f, angularVelocity * (physicsRotate.sin / num)), transformPlane);
		return new Quaternion(vector.x, vector.y, vector.z, physicsRotate.cos).normalized;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion ToRotationFast3D(float angle, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		switch (transformPlane)
		{
		case PhysicsWorld.TransformPlane.XY:
		{
			PhysicsRotate physicsRotate3 = new PhysicsRotate(angle * 0.5f);
			return new Quaternion(0f, 0f, physicsRotate3.sin, physicsRotate3.cos);
		}
		case PhysicsWorld.TransformPlane.XZ:
		{
			PhysicsRotate physicsRotate2 = new PhysicsRotate(angle * -0.5f);
			return new Quaternion(0f, physicsRotate2.sin, 0f, physicsRotate2.cos);
		}
		case PhysicsWorld.TransformPlane.ZY:
		{
			PhysicsRotate physicsRotate = new PhysicsRotate(angle * -0.5f);
			return new Quaternion(physicsRotate.sin, 0f, 0f, physicsRotate.cos);
		}
		default:
			throw new InvalidOperationException("Invalid Transform Plane.");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Quaternion ToRotationSlow3D(float angle, Quaternion reference, PhysicsWorld.TransformPlane transformPlane = PhysicsWorld.TransformPlane.XY)
	{
		if (reference.w < 0f)
		{
			reference = new Quaternion(0f - reference.x, 0f - reference.y, 0f - reference.z, 0f - reference.w);
		}
		Quaternion quaternion = ToRotationFast3D(angle, transformPlane);
		Quaternion quaternion2 = Quaternion.Inverse(ToRotationFast3D(ToRotation2D(reference, transformPlane), transformPlane));
		return quaternion * quaternion2 * reference;
	}
}
