using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct CapsuleGeometry
{
	public static readonly CapsuleGeometry defaultGeometry = new CapsuleGeometry
	{
		center1 = Vector2.up * 0.5f,
		center2 = Vector2.down * 0.5f,
		radius = 0.5f
	};

	[SerializeField]
	private Vector2 m_Center1;

	[SerializeField]
	private Vector2 m_Center2;

	[Min(0f)]
	[SerializeField]
	private float m_Radius;

	public readonly bool isValid => PhysicsLowLevelScripting2D.CapsuleGeometry_IsValid(this);

	public Vector2 center1
	{
		readonly get
		{
			return m_Center1;
		}
		set
		{
			m_Center1 = value;
		}
	}

	public Vector2 center2
	{
		readonly get
		{
			return m_Center2;
		}
		set
		{
			m_Center2 = value;
		}
	}

	public float radius
	{
		readonly get
		{
			return m_Radius;
		}
		set
		{
			m_Radius = Mathf.Max(0f, value);
		}
	}

	public CapsuleGeometry()
	{
		m_Center1 = Vector2.up * 0.5f;
		m_Center2 = Vector2.down * 0.5f;
		m_Radius = 0.5f;
	}

	public static CapsuleGeometry Create(Vector2 center1, Vector2 center2, float radius)
	{
		CapsuleGeometry result = new CapsuleGeometry();
		result.center1 = center1;
		result.center2 = center2;
		result.radius = radius;
		return result;
	}

	public readonly PhysicsBody.MassConfiguration CalculateMassConfiguration(float density = 1f)
	{
		return PhysicsLowLevelScripting2D.CapsuleGeometry_CalculateMassConfiguration(this, density);
	}

	public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.CapsuleGeometry_CalculateAABB(this, transform);
	}

	public readonly bool OverlapPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.CapsuleGeometry_OverlapPoint(this, point);
	}

	public readonly Vector2 ClosestPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.CapsuleGeometry_ClosestPoint(this, point);
	}

	public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput)
	{
		return PhysicsLowLevelScripting2D.CapsuleGeometry_CastRay(this, castRayInput);
	}

	public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
	{
		return PhysicsLowLevelScripting2D.CapsuleGeometry_CastShape(this, input);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CircleGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.CapsuleAndCircle(this, transform, otherGeometry, otherTransform);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CapsuleGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.CapsuleAndCapsule(otherGeometry, otherTransform, this, transform);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, PolygonGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.PolygonAndCapsule(otherGeometry, otherTransform, this, transform);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, SegmentGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.SegmentAndCapsule(otherGeometry, otherTransform, this, transform);
	}

	public readonly CapsuleGeometry Transform(PhysicsTransform transform)
	{
		CapsuleGeometry result = new CapsuleGeometry();
		result.center1 = transform.TransformPoint(center1);
		result.center2 = transform.TransformPoint(center2);
		result.radius = radius;
		return result;
	}

	public readonly CapsuleGeometry InverseTransform(PhysicsTransform transform)
	{
		CapsuleGeometry result = new CapsuleGeometry();
		result.center1 = transform.InverseTransformPoint(center1);
		result.center2 = transform.InverseTransformPoint(center2);
		result.radius = radius;
		return result;
	}

	public readonly CapsuleGeometry Transform(Matrix4x4 transform, bool scaleRadius)
	{
		CapsuleGeometry result = new CapsuleGeometry();
		result.center1 = transform.MultiplyPoint3x4((Vector3)center1);
		result.center2 = transform.MultiplyPoint3x4((Vector3)center2);
		result.radius = (scaleRadius ? (PhysicsMath.MaxAbsComponent((Vector2)transform.lossyScale) * radius) : radius);
		return result;
	}

	public readonly CapsuleGeometry InverseTransform(Matrix4x4 transform, bool scaleRadius)
	{
		transform = transform.inverse;
		CapsuleGeometry result = new CapsuleGeometry();
		result.center1 = transform.MultiplyPoint3x4((Vector3)center1);
		result.center2 = transform.MultiplyPoint3x4((Vector3)center2);
		result.radius = (scaleRadius ? (PhysicsMath.MinAbsComponent((Vector2)transform.lossyScale) * radius) : radius);
		return result;
	}
}
