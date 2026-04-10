using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct SegmentGeometry
{
	public static readonly SegmentGeometry defaultGeometry = new SegmentGeometry
	{
		point1 = Vector2.right * 0.5f,
		point2 = Vector2.left * 0.5f
	};

	[SerializeField]
	private Vector2 m_Point1;

	[SerializeField]
	private Vector2 m_Point2;

	public readonly bool isValid => PhysicsLowLevelScripting2D.SegmentGeometry_IsValid(this);

	public Vector2 point1
	{
		readonly get
		{
			return m_Point1;
		}
		set
		{
			m_Point1 = value;
		}
	}

	public Vector2 point2
	{
		readonly get
		{
			return m_Point2;
		}
		set
		{
			m_Point2 = value;
		}
	}

	public readonly Vector2 midPoint => (point1 + point2) * 0.5f;

	public readonly Vector2 forward => point2 - point1;

	public readonly Vector2 backward => point1 - point2;

	public SegmentGeometry()
	{
		m_Point1 = Vector2.right * 0.5f;
		m_Point2 = Vector2.left * 0.5f;
	}

	public static SegmentGeometry Create(Vector2 point1, Vector2 point2)
	{
		SegmentGeometry result = new SegmentGeometry();
		result.point1 = point1;
		result.point2 = point2;
		return result;
	}

	public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.SegmentGeometry_CalculateAABB(this, transform);
	}

	public readonly Vector2 ClosestPoint(PhysicsTransform transform, Vector2 point)
	{
		return PhysicsLowLevelScripting2D.SegmentGeometry_ClosestPoint(this, transform.TransformPoint(point));
	}

	public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput, bool oneSided = false)
	{
		return PhysicsLowLevelScripting2D.SegmentGeometry_CastRay(this, castRayInput, oneSided);
	}

	public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
	{
		return PhysicsLowLevelScripting2D.SegmentGeometry_CastShape(this, input);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CircleGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.SegmentAndCircle(this, transform, otherGeometry, otherTransform);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CapsuleGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.SegmentAndCapsule(this, transform, otherGeometry, otherTransform);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, PolygonGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.SegmentAndPolygon(this, transform, otherGeometry, otherTransform);
	}

	public readonly SegmentGeometry Transform(PhysicsTransform transform)
	{
		SegmentGeometry result = new SegmentGeometry();
		result.point1 = transform.TransformPoint(point1);
		result.point2 = transform.TransformPoint(point2);
		return result;
	}

	public readonly SegmentGeometry InverseTransform(PhysicsTransform transform)
	{
		SegmentGeometry result = new SegmentGeometry();
		result.point1 = transform.InverseTransformPoint(point1);
		result.point2 = transform.InverseTransformPoint(point2);
		return result;
	}

	public readonly SegmentGeometry Transform(Matrix4x4 transform)
	{
		SegmentGeometry result = new SegmentGeometry();
		result.point1 = transform.MultiplyPoint3x4((Vector3)point1);
		result.point2 = transform.MultiplyPoint3x4((Vector3)point2);
		return result;
	}

	public readonly SegmentGeometry InverseTransform(Matrix4x4 transform)
	{
		transform = transform.inverse;
		SegmentGeometry result = new SegmentGeometry();
		result.point1 = transform.MultiplyPoint3x4((Vector3)point1);
		result.point2 = transform.MultiplyPoint3x4((Vector3)point2);
		return result;
	}

	public readonly SegmentGeometry Scale(float scale)
	{
		Vector2 vector = midPoint;
		Vector2 vector2 = forward * 0.5f * scale;
		SegmentGeometry result = new SegmentGeometry();
		result.point1 = midPoint - vector2;
		result.point2 = midPoint + vector2;
		return result;
	}
}
