using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PhysicsAABB
{
	[SerializeField]
	private Vector2 m_LowerBound;

	[SerializeField]
	private Vector2 m_UpperBound;

	public readonly bool isValid => PhysicsLowLevelScripting2D.PhysicsAABB_IsValid(this);

	public Vector2 lowerBound
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_LowerBound;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_LowerBound = value;
		}
	}

	public Vector2 upperBound
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_UpperBound;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_UpperBound = value;
		}
	}

	public readonly PhysicsAABB normalized => new PhysicsAABB
	{
		lowerBound = Vector2.Min(lowerBound, upperBound),
		upperBound = Vector2.Max(lowerBound, upperBound)
	};

	public readonly Vector2 center => PhysicsLowLevelScripting2D.PhysicsAABB_Center(this);

	public readonly Vector2 extents => PhysicsLowLevelScripting2D.PhysicsAABB_Extents(this);

	public readonly float perimeter => PhysicsLowLevelScripting2D.PhysicsAABB_Perimeter(this);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PhysicsAABB(Vector2 lowerBound, Vector2 upperBound)
	{
		m_LowerBound = lowerBound;
		m_UpperBound = upperBound;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalized()
	{
		this = normalized;
	}

	public readonly bool OverlapPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.PhysicsAABB_OverlapPoint(this, point);
	}

	public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput)
	{
		return PhysicsLowLevelScripting2D.PhysicsAABB_CastRay(this, castRayInput);
	}

	public readonly bool Overlap(PhysicsAABB aabb)
	{
		return PhysicsLowLevelScripting2D.PhysicsAABB_Overlap(this, aabb);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Overlap(Vector2 point)
	{
		return OverlapPoint(point);
	}

	public readonly PhysicsAABB Union(PhysicsAABB aabb)
	{
		return PhysicsLowLevelScripting2D.PhysicsAABB_Union(this, aabb);
	}

	public readonly bool Contains(PhysicsAABB aabb)
	{
		return PhysicsLowLevelScripting2D.PhysicsAABB_Contains(this, aabb);
	}

	public override readonly string ToString()
	{
		return $"lowerBound={lowerBound}, upperBound={upperBound}, isValid={isValid}";
	}
}
