using System;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct ChainSegmentGeometry
{
	public static readonly ChainSegmentGeometry defaultGeometry = new ChainSegmentGeometry
	{
		segment = SegmentGeometry.defaultGeometry,
		ghost1 = SegmentGeometry.defaultGeometry.point1 * 2f,
		ghost2 = SegmentGeometry.defaultGeometry.point2 * 2f
	};

	[SerializeField]
	private Vector2 m_Ghost1;

	[SerializeField]
	private SegmentGeometry m_Segment;

	[SerializeField]
	private Vector2 m_Ghost2;

	private readonly int m_ChainId;

	public readonly bool isValid => PhysicsLowLevelScripting2D.ChainSegmentGeometry_IsValid(this);

	public Vector2 ghost1
	{
		readonly get
		{
			return m_Ghost1;
		}
		set
		{
			m_Ghost1 = value;
		}
	}

	public SegmentGeometry segment
	{
		readonly get
		{
			return m_Segment;
		}
		set
		{
			m_Segment = value;
		}
	}

	public Vector2 ghost2
	{
		readonly get
		{
			return m_Ghost2;
		}
		set
		{
			m_Ghost2 = value;
		}
	}

	public ChainSegmentGeometry()
	{
		m_Segment = new SegmentGeometry();
		m_Ghost1 = m_Segment.point1 * 2f;
		m_Ghost2 = m_Segment.point2 * 2f;
		m_ChainId = 0;
	}

	public ChainSegmentGeometry(SegmentGeometry segmentGeometry, Vector2 ghost1, Vector2 ghost2)
	{
		m_Segment = segmentGeometry;
		m_Ghost1 = ghost1;
		m_Ghost2 = ghost2;
		m_ChainId = 0;
	}

	public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.ChainSegmentGeometry_CalculateAABB(this, transform);
	}

	public readonly Vector2 ClosestPoint(PhysicsTransform transform, Vector2 point)
	{
		return PhysicsLowLevelScripting2D.ChainSegmentGeometry_ClosestPoint(this, transform.TransformPoint(point));
	}

	public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput, bool oneSided)
	{
		return PhysicsLowLevelScripting2D.ChainSegmentGeometry_CastRay(this, castRayInput, oneSided);
	}

	public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
	{
		return PhysicsLowLevelScripting2D.ChainSegmentGeometry_CastShape(this, input);
	}

	public readonly ChainSegmentGeometry Transform(PhysicsTransform transform)
	{
		ChainSegmentGeometry result = new ChainSegmentGeometry();
		result.ghost1 = transform.TransformPoint(ghost1);
		result.segment = segment.Transform(transform);
		result.ghost2 = transform.TransformPoint(ghost2);
		return result;
	}

	public readonly ChainSegmentGeometry InverseTransform(PhysicsTransform transform)
	{
		ChainSegmentGeometry result = new ChainSegmentGeometry();
		result.ghost1 = transform.InverseTransformPoint(ghost1);
		result.segment = segment.InverseTransform(transform);
		result.ghost2 = transform.TransformPoint(ghost2);
		return result;
	}

	public readonly ChainSegmentGeometry Transform(Matrix4x4 transform)
	{
		ChainSegmentGeometry result = new ChainSegmentGeometry();
		result.ghost1 = transform.MultiplyPoint3x4((Vector3)ghost1);
		result.segment = segment.Transform(transform);
		result.ghost2 = transform.MultiplyPoint3x4((Vector3)ghost2);
		return result;
	}

	public readonly ChainSegmentGeometry InverseTransform(Matrix4x4 transform)
	{
		transform = transform.inverse;
		ChainSegmentGeometry result = new ChainSegmentGeometry();
		result.ghost1 = transform.MultiplyPoint3x4((Vector3)ghost1);
		result.segment = segment.Transform(transform);
		result.ghost2 = transform.MultiplyPoint3x4((Vector3)ghost2);
		return result;
	}
}
