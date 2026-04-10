using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsTransform
{
	public Vector2 position;

	public PhysicsRotate rotation;

	public readonly bool isValid => PhysicsLowLevelScripting2D.PhysicsTransform_IsValid(this);

	public static PhysicsTransform identity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new PhysicsTransform();
		}
	}

	public PhysicsTransform()
	{
		position = Vector2.zero;
		rotation = PhysicsRotate.identity;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PhysicsTransform(Vector2 position)
	{
		this.position = position;
		rotation = PhysicsRotate.identity;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PhysicsTransform(Vector2 position, PhysicsRotate rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly void GetPositionAndRotation(out Vector2 position, out PhysicsRotate rotation)
	{
		position = this.position;
		rotation = this.rotation;
	}

	public readonly Vector2 TransformPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.PhysicsTransform_TransformPoint(this, point);
	}

	public readonly Vector2 InverseTransformPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.PhysicsTransform_InverseTransformPoint(this, point);
	}

	public readonly PhysicsTransform MultiplyTransform(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.PhysicsTransform_MultiplyTransform(this, transform);
	}

	public readonly PhysicsTransform InverseMultiplyTransform(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.PhysicsTransform_InverseMultiplyTransform(this, transform);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PhysicsTransform(Vector2 position)
	{
		return new PhysicsTransform(position, PhysicsRotate.identity);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PhysicsTransform(PhysicsRotate rotation)
	{
		return new PhysicsTransform(Vector2.zero, rotation);
	}

	public override readonly string ToString()
	{
		return $"position={position}, rotation={rotation}";
	}
}
