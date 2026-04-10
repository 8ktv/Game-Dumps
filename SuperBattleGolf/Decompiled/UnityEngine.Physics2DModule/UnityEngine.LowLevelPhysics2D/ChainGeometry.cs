using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.LowLevelPhysics2D;

public struct ChainGeometry
{
	private IntPtr m_Points;

	private int m_Count;

	public readonly bool isValid => PhysicsLowLevelScripting2D.ChainGeometry_IsValid(this);

	public unsafe readonly ReadOnlySpan<Vector2> vertices => new ReadOnlySpan<Vector2>(m_Points.ToPointer(), m_Count);

	public unsafe ChainGeometry(NativeArray<Vector2> vertices)
	{
		if (vertices.Length < 4)
		{
			throw new ArgumentOutOfRangeException("vertices", "Chain Geometry must contain a minimum of 4 vertices.");
		}
		m_Points = new IntPtr(vertices.GetUnsafeReadOnlyPtr());
		m_Count = vertices.Length;
	}

	public unsafe ChainGeometry(ReadOnlySpan<Vector2> vertices)
	{
		if (vertices.Length < 4)
		{
			throw new ArgumentOutOfRangeException("vertices", "Chain Geometry must contain a minimum of 4 vertices.");
		}
		fixed (Vector2* value = vertices)
		{
			m_Points = new IntPtr(value);
			m_Count = vertices.Length;
		}
	}

	public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.ChainGeometry_CalculateAABB(this, transform);
	}

	public readonly Vector2 ClosestPoint(PhysicsTransform transform, Vector2 point)
	{
		return PhysicsLowLevelScripting2D.ChainGeometry_ClosestPoint(this, transform.TransformPoint(point));
	}

	public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput, bool oneSided = true)
	{
		return PhysicsLowLevelScripting2D.ChainGeometry_CastRay(this, castRayInput, oneSided);
	}

	public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
	{
		return PhysicsLowLevelScripting2D.ChainGeometry_CastShape(this, input);
	}
}
