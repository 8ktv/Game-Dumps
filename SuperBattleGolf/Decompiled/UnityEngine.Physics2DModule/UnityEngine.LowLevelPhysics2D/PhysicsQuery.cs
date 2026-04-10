using System;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct PhysicsQuery
{
	[Serializable]
	public struct QueryFilter
	{
		public static readonly PhysicsMask DefaultCategories = PhysicsMask.One;

		public static readonly PhysicsMask DefaultHitCategories = PhysicsMask.All;

		public static readonly QueryFilter Everything = new QueryFilter(PhysicsMask.All, PhysicsMask.All);

		public static readonly QueryFilter defaultFilter = new QueryFilter(DefaultCategories, DefaultHitCategories);

		[SerializeField]
		internal PhysicsMask m_Categories;

		[SerializeField]
		internal PhysicsMask m_HitCategories;

		public PhysicsMask categories
		{
			readonly get
			{
				return m_Categories;
			}
			set
			{
				m_Categories = value;
			}
		}

		public PhysicsMask hitCategories
		{
			readonly get
			{
				return m_HitCategories;
			}
			set
			{
				m_HitCategories = value;
			}
		}

		public QueryFilter()
		{
			this = defaultFilter;
		}

		public QueryFilter(PhysicsMask categories, PhysicsMask hitCategories)
		{
			m_Categories = categories;
			m_HitCategories = hitCategories;
		}
	}

	public readonly struct WorldOverlapResult
	{
		private readonly PhysicsShape m_Shape;

		public PhysicsShape shape => m_Shape;

		public bool isValid => m_Shape.isValid;
	}

	public readonly struct WorldCastResult
	{
		private readonly PhysicsShape m_Shape;

		private readonly Vector2 m_Point;

		private readonly Vector2 m_Normal;

		private readonly float m_Fraction;

		public PhysicsShape shape => m_Shape;

		public Vector2 point => m_Point;

		public Vector2 normal => m_Normal;

		public float fraction => m_Fraction;

		public bool isValid => m_Shape.isValid;
	}

	[Serializable]
	public struct WorldMoverInput
	{
		private static WorldMoverInput s_WorldMoverInput = new WorldMoverInput
		{
			geometry = CapsuleGeometry.defaultGeometry,
			overlapFilter = QueryFilter.defaultFilter,
			castFilter = QueryFilter.defaultFilter,
			transform = PhysicsTransform.identity,
			velocity = Vector2.zero,
			targetPosition = Vector2.zero,
			maxIterations = 5,
			moveTolerance = 0.1f
		};

		[SerializeField]
		private CapsuleGeometry m_Geometry;

		[SerializeField]
		private PhysicsTransform m_Transform;

		[SerializeField]
		private Vector2 m_TargetPosition;

		[SerializeField]
		private Vector2 m_Velocity;

		[SerializeField]
		private QueryFilter m_OverlapFilter;

		[SerializeField]
		private QueryFilter m_CastFilter;

		[SerializeField]
		private int m_MaxIterations;

		[SerializeField]
		private float m_MoveTolerance;

		public CapsuleGeometry geometry
		{
			readonly get
			{
				return m_Geometry;
			}
			set
			{
				m_Geometry = value;
			}
		}

		public PhysicsTransform transform
		{
			readonly get
			{
				return m_Transform;
			}
			set
			{
				m_Transform = value;
			}
		}

		public Vector2 targetPosition
		{
			readonly get
			{
				return m_TargetPosition;
			}
			set
			{
				m_TargetPosition = value;
			}
		}

		public Vector2 velocity
		{
			readonly get
			{
				return m_Velocity;
			}
			set
			{
				m_Velocity = value;
			}
		}

		public QueryFilter overlapFilter
		{
			readonly get
			{
				return m_OverlapFilter;
			}
			set
			{
				m_OverlapFilter = value;
			}
		}

		public QueryFilter castFilter
		{
			readonly get
			{
				return m_CastFilter;
			}
			set
			{
				m_CastFilter = value;
			}
		}

		public int maxIterations
		{
			readonly get
			{
				return m_MaxIterations;
			}
			set
			{
				m_MaxIterations = Mathf.Max(0, value);
			}
		}

		public float moveTolerance
		{
			readonly get
			{
				return m_MoveTolerance;
			}
			set
			{
				m_MoveTolerance = Mathf.Max(0.01f, value);
			}
		}

		public static WorldMoverInput defaultInput => s_WorldMoverInput;

		public WorldMoverInput()
		{
			this = s_WorldMoverInput;
		}
	}

	public readonly struct WorldMoverResult
	{
		private readonly PhysicsTransform m_Transform;

		private readonly Vector2 m_Velocity;

		public PhysicsTransform transform => m_Transform;

		public Vector2 velocity => m_Velocity;
	}

	public enum WorldCastMode
	{
		Closest,
		All,
		AllSorted
	}

	[Serializable]
	public struct CastRayInput
	{
		[SerializeField]
		private Vector2 m_Origin;

		[SerializeField]
		private Vector2 m_Translation;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_MaxFraction;

		public Vector2 origin
		{
			readonly get
			{
				return m_Origin;
			}
			set
			{
				m_Origin = value;
			}
		}

		public Vector2 translation
		{
			readonly get
			{
				return m_Translation;
			}
			set
			{
				m_Translation = value;
			}
		}

		public float maxFraction
		{
			readonly get
			{
				return m_MaxFraction;
			}
			set
			{
				m_MaxFraction = Mathf.Clamp01(value);
			}
		}

		public CastRayInput()
		{
			m_Origin = default(Vector2);
			m_Translation = default(Vector2);
			m_MaxFraction = 1f;
		}

		public CastRayInput(Vector2 origin, Vector2 translation)
		{
			m_Origin = origin;
			m_Translation = translation;
			m_MaxFraction = 1f;
		}

		public static CastRayInput FromTo(Vector2 from, Vector2 to)
		{
			return new CastRayInput(from, to - from);
		}
	}

	[Serializable]
	public struct CastShapePairInput
	{
		[SerializeField]
		private PhysicsShape.ShapeProxy m_ShapeProxyA;

		[SerializeField]
		private PhysicsShape.ShapeProxy m_ShapeProxyB;

		[SerializeField]
		private PhysicsTransform m_TransformA;

		[SerializeField]
		private PhysicsTransform m_TransformB;

		[SerializeField]
		private Vector2 m_TranslationB;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_MaxFraction;

		[SerializeField]
		private bool m_CanEncroach;

		public PhysicsShape.ShapeProxy shapeProxyA
		{
			readonly get
			{
				return m_ShapeProxyA;
			}
			set
			{
				m_ShapeProxyA = value;
			}
		}

		public PhysicsShape.ShapeProxy shapeProxyB
		{
			readonly get
			{
				return m_ShapeProxyB;
			}
			set
			{
				m_ShapeProxyB = value;
			}
		}

		public PhysicsTransform transformA
		{
			readonly get
			{
				return m_TransformA;
			}
			set
			{
				m_TransformA = value;
			}
		}

		public PhysicsTransform transformB
		{
			readonly get
			{
				return m_TransformB;
			}
			set
			{
				m_TransformB = value;
			}
		}

		public Vector2 translationB
		{
			readonly get
			{
				return m_TranslationB;
			}
			set
			{
				m_TranslationB = value;
			}
		}

		public float maxFraction
		{
			readonly get
			{
				return m_MaxFraction;
			}
			set
			{
				m_MaxFraction = Mathf.Clamp01(value);
			}
		}

		public bool canEncroach
		{
			readonly get
			{
				return m_CanEncroach;
			}
			set
			{
				m_CanEncroach = value;
			}
		}
	}

	[Serializable]
	public struct CastShapeInput
	{
		[SerializeField]
		private PhysicsShape.ShapeProxy m_ShapeProxy;

		[SerializeField]
		private Vector2 m_Translation;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_MaxFraction;

		[SerializeField]
		private bool m_CanEncroach;

		public PhysicsShape.ShapeProxy shapeProxy
		{
			readonly get
			{
				return m_ShapeProxy;
			}
			set
			{
				m_ShapeProxy = value;
			}
		}

		public Vector2 translation
		{
			readonly get
			{
				return m_Translation;
			}
			set
			{
				m_Translation = value;
			}
		}

		public float maxFraction
		{
			readonly get
			{
				return m_MaxFraction;
			}
			set
			{
				m_MaxFraction = Mathf.Clamp01(value);
			}
		}

		public bool canEncroach
		{
			readonly get
			{
				return m_CanEncroach;
			}
			set
			{
				m_CanEncroach = value;
			}
		}

		public CastShapeInput()
		{
			m_ShapeProxy = default(PhysicsShape.ShapeProxy);
			m_Translation = default(Vector2);
			m_MaxFraction = 1f;
			m_CanEncroach = false;
		}

		public CastShapeInput(CircleGeometry circleGeometry, Vector2 translation)
		{
			m_ShapeProxy = new PhysicsShape.ShapeProxy(circleGeometry);
			m_Translation = translation;
			m_MaxFraction = 1f;
			m_CanEncroach = false;
		}

		public CastShapeInput(CapsuleGeometry capsuleGeometry, Vector2 translation)
		{
			m_ShapeProxy = new PhysicsShape.ShapeProxy(capsuleGeometry);
			m_Translation = translation;
			m_MaxFraction = 1f;
			m_CanEncroach = false;
		}

		public CastShapeInput(SegmentGeometry segmentGeometry, Vector2 translation)
		{
			m_ShapeProxy = new PhysicsShape.ShapeProxy(segmentGeometry);
			m_Translation = translation;
			m_MaxFraction = 1f;
			m_CanEncroach = false;
		}

		public CastShapeInput(PolygonGeometry polygonGeometry, Vector2 translation)
		{
			m_ShapeProxy = new PhysicsShape.ShapeProxy(polygonGeometry);
			m_Translation = translation;
			m_MaxFraction = 1f;
			m_CanEncroach = false;
		}

		public CastShapeInput(ChainSegmentGeometry chainSegmentGeometry, Vector2 translation)
		{
			m_ShapeProxy = new PhysicsShape.ShapeProxy(chainSegmentGeometry);
			m_Translation = translation;
			m_MaxFraction = 1f;
			m_CanEncroach = false;
		}

		public static CastShapeInput FromShape(PhysicsShape shape, Vector2 translation)
		{
			if (!shape.isValid)
			{
				throw new ArgumentException("PhysicsShape is not valid.");
			}
			PhysicsTransform transform = shape.body.transform;
			PhysicsShape.ShapeType shapeType = shape.shapeType;
			if (1 == 0)
			{
			}
			CastShapeInput result = shapeType switch
			{
				PhysicsShape.ShapeType.Circle => new CastShapeInput(shape.circleGeometry.Transform(transform), translation), 
				PhysicsShape.ShapeType.Capsule => new CastShapeInput(shape.capsuleGeometry.Transform(transform), translation), 
				PhysicsShape.ShapeType.Segment => new CastShapeInput(shape.segmentGeometry.Transform(transform), translation), 
				PhysicsShape.ShapeType.Polygon => new CastShapeInput(shape.polygonGeometry.Transform(transform), translation), 
				PhysicsShape.ShapeType.ChainSegment => new CastShapeInput(shape.chainSegmentGeometry.Transform(transform), translation), 
				_ => throw new NotImplementedException(), 
			};
			if (1 == 0)
			{
			}
			return result;
		}
	}

	public readonly struct CastResult
	{
		private readonly Vector2 m_Normal;

		private readonly Vector2 m_Point;

		private readonly float m_Fraction;

		private readonly int m_Iterations;

		private readonly bool m_Hit;

		public Vector2 normal => m_Normal;

		public Vector2 point => m_Point;

		public float fraction => m_Fraction;

		public int iterations => m_Iterations;

		public bool hit => m_Hit;

		public static implicit operator bool(CastResult output)
		{
			return output.hit;
		}
	}

	[Serializable]
	public struct DistanceInput
	{
		[SerializeField]
		private PhysicsShape.ShapeProxy m_ShapeProxyA;

		[SerializeField]
		private PhysicsShape.ShapeProxy m_ShapeProxyB;

		[SerializeField]
		private PhysicsTransform m_TransformA;

		[SerializeField]
		private PhysicsTransform m_TransformB;

		[SerializeField]
		private bool m_UseRadii;

		public PhysicsShape.ShapeProxy shapeProxyA
		{
			readonly get
			{
				return m_ShapeProxyA;
			}
			set
			{
				m_ShapeProxyA = value;
			}
		}

		public PhysicsShape.ShapeProxy shapeProxyB
		{
			readonly get
			{
				return m_ShapeProxyB;
			}
			set
			{
				m_ShapeProxyB = value;
			}
		}

		public PhysicsTransform transformA
		{
			readonly get
			{
				return m_TransformA;
			}
			set
			{
				m_TransformA = value;
			}
		}

		public PhysicsTransform transformB
		{
			readonly get
			{
				return m_TransformB;
			}
			set
			{
				m_TransformB = value;
			}
		}

		public bool useRadii
		{
			readonly get
			{
				return m_UseRadii;
			}
			set
			{
				m_UseRadii = value;
			}
		}
	}

	public readonly struct DistanceResult
	{
		private readonly Vector2 m_PointA;

		private readonly Vector2 m_PointB;

		private readonly Vector2 m_Normal;

		private readonly float m_Distance;

		private readonly int m_Iterations;

		private readonly int m_SimplexCount;

		public Vector2 pointA => m_PointA;

		public Vector2 pointB => m_PointB;

		public Vector2 normal => m_Normal;

		public float distance => m_Distance;

		public int iterations => m_Iterations;
	}

	public readonly struct SegmentDistanceResult
	{
		private readonly Vector2 m_Closest1;

		private readonly Vector2 m_Closest2;

		private readonly float m_Fraction1;

		private readonly float m_Fraction2;

		private readonly float m_Distance;

		public Vector2 closest1 => m_Closest1;

		public Vector2 closest2 => m_Closest2;

		public float fraction1 => m_Fraction1;

		public float fraction2 => m_Fraction2;

		public float distance => m_Distance;
	}

	[Serializable]
	public struct ShapeSweep
	{
		[SerializeField]
		private Vector2 m_LocalCOM;

		[SerializeField]
		private Vector2 m_PositionStart;

		[SerializeField]
		private Vector2 m_PositionEnd;

		[SerializeField]
		private PhysicsRotate m_RotationStart;

		[SerializeField]
		private PhysicsRotate m_RotationEnd;

		public Vector2 localCOM
		{
			readonly get
			{
				return m_LocalCOM;
			}
			set
			{
				m_LocalCOM = value;
			}
		}

		public Vector2 positionStart
		{
			readonly get
			{
				return m_PositionStart;
			}
			set
			{
				m_PositionStart = value;
			}
		}

		public Vector2 positionEnd
		{
			readonly get
			{
				return m_PositionEnd;
			}
			set
			{
				m_PositionEnd = value;
			}
		}

		public PhysicsRotate rotationStart
		{
			readonly get
			{
				return m_RotationStart;
			}
			set
			{
				m_RotationStart = value;
			}
		}

		public PhysicsRotate rotationEnd
		{
			readonly get
			{
				return m_RotationEnd;
			}
			set
			{
				m_RotationEnd = value;
			}
		}
	}

	[Serializable]
	public struct TimeOfImpactInput
	{
		[SerializeField]
		private PhysicsShape.ShapeProxy m_ShapeProxyA;

		[SerializeField]
		private PhysicsShape.ShapeProxy m_ShapeProxyB;

		[SerializeField]
		private ShapeSweep m_ShapeSweepA;

		[SerializeField]
		private ShapeSweep m_ShapeSweepB;

		[SerializeField]
		private float m_MaxFraction;

		public PhysicsShape.ShapeProxy shapeProxyA
		{
			readonly get
			{
				return m_ShapeProxyA;
			}
			set
			{
				m_ShapeProxyA = value;
			}
		}

		public PhysicsShape.ShapeProxy shapeProxyB
		{
			readonly get
			{
				return m_ShapeProxyB;
			}
			set
			{
				m_ShapeProxyB = value;
			}
		}

		public ShapeSweep shapeSweepA
		{
			readonly get
			{
				return m_ShapeSweepA;
			}
			set
			{
				m_ShapeSweepA = value;
			}
		}

		public ShapeSweep shapeSweepB
		{
			readonly get
			{
				return m_ShapeSweepB;
			}
			set
			{
				m_ShapeSweepB = value;
			}
		}

		public float maxFraction
		{
			readonly get
			{
				return m_MaxFraction;
			}
			set
			{
				m_MaxFraction = value;
			}
		}
	}

	public readonly struct TimeOfImpactResult
	{
		public enum State
		{
			Unknown,
			Failed,
			Overlapped,
			Hit,
			Separated
		}

		private readonly Vector2 m_Point;

		private readonly Vector2 m_Normal;

		private readonly State m_ImpactState;

		private readonly float m_Fraction;

		public Vector2 point => m_Point;

		public Vector2 normal => m_Normal;

		public State impactState => m_ImpactState;

		public float fraction => m_Fraction;
	}

	public static PhysicsShape.ContactManifold ShapeAndShape(PhysicsShape shapeA, PhysicsTransform transformA, PhysicsShape shapeB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_ShapeAndShape(shapeA, transformA, shapeB, transformB);
	}

	public static PhysicsShape.ContactManifold CircleAndCircle(CircleGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_CircleAndCircle(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold CapsuleAndCircle(CapsuleGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_CapsuleAndCircle(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold SegmentAndCircle(SegmentGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_SegmentAndCircle(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold PolygonAndCircle(PolygonGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_PolygonAndCircle(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold CapsuleAndCapsule(CapsuleGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_CapsuleAndCapsule(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold SegmentAndCapsule(SegmentGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_SegmentAndCapsule(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold PolygonAndCapsule(PolygonGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_PolygonAndCapsule(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold PolygonAndPolygon(PolygonGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_PolygonAndPolygon(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold SegmentAndPolygon(SegmentGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_SegmentAndPolygon(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold ChainSegmentAndCircle(ChainSegmentGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_ChainSegmentAndCircle(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold ChainSegmentAndCapsule(ChainSegmentGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_ChainSegmentAndCapsule(geometryA, transformA, geometryB, transformB);
	}

	public static PhysicsShape.ContactManifold ChainSegmentAndPolygon(ChainSegmentGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_ChainSegmentAndPolygon(geometryA, transformA, geometryB, transformB);
	}

	public static SegmentDistanceResult SegmentDistance(SegmentGeometry geometryA, PhysicsTransform transformA, SegmentGeometry geometryB, PhysicsTransform transformB)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_SegmentDistance(geometryA, transformA, geometryB, transformB);
	}

	public static void CastShapes(CastShapePairInput castShapePairInput)
	{
		PhysicsLowLevelScripting2D.PhysicsQuery_CastShapes(castShapePairInput);
	}

	public static DistanceResult ShapeDistance(DistanceInput distanceInput)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_ShapeDistance(distanceInput);
	}

	public static TimeOfImpactResult ShapeTimeOfImpact(TimeOfImpactInput toiInput)
	{
		return PhysicsLowLevelScripting2D.PhysicsQuery_ShapeTimeOfImpact(toiInput);
	}
}
