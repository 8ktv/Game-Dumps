using System;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D;

[Serializable]
public struct PolygonGeometry
{
	[Serializable]
	public struct ConvexHull
	{
		public PhysicsShape.ShapeArray vertices;

		[Range(3f, 8f)]
		[SerializeField]
		internal int m_Count;

		public int count
		{
			readonly get
			{
				return m_Count;
			}
			set
			{
				m_Count = Mathf.Clamp(value, 3, 8);
			}
		}

		public unsafe ReadOnlySpan<Vector2> AsReadOnlySpan()
		{
			fixed (Vector2* pointer = &vertices[0])
			{
				return new ReadOnlySpan<Vector2>(pointer, m_Count);
			}
		}
	}

	public static readonly PolygonGeometry defaultGeometry = CreateBox(Vector2.one);

	public PhysicsShape.ShapeArray vertices;

	public PhysicsShape.ShapeArray normals;

	[SerializeField]
	internal Vector2 m_Centroid;

	[SerializeField]
	[Min(0f)]
	internal float m_Radius;

	[Range(3f, 8f)]
	[SerializeField]
	internal int m_Count;

	public readonly bool isValid => PhysicsLowLevelScripting2D.PolygonGeometry_IsValid(this);

	public Vector2 centroid
	{
		readonly get
		{
			return m_Centroid;
		}
		set
		{
			m_Centroid = value;
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

	public int count
	{
		readonly get
		{
			return m_Count;
		}
		set
		{
			m_Count = Mathf.Clamp(value, 3, 8);
		}
	}

	public PolygonGeometry()
	{
		vertices = new PhysicsShape.ShapeArray
		{
			vertex0 = new Vector2(-0.5f, -0.5f),
			vertex1 = new Vector2(0.5f, -0.5f),
			vertex2 = new Vector2(0.5f, 0.5f),
			vertex3 = new Vector2(-0.5f, 0.5f)
		};
		normals = new PhysicsShape.ShapeArray
		{
			vertex0 = Vector2.down,
			vertex1 = Vector2.right,
			vertex2 = Vector2.right,
			vertex3 = Vector2.left
		};
		m_Count = 4;
		m_Centroid = Vector2.zero;
		m_Radius = 0f;
	}

	public static PolygonGeometry CreateBox(Vector2 size, float radius = 0f, bool inscribe = false)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_CreateBox(size, radius, PhysicsTransform.identity, inscribe);
	}

	public static NativeArray<PolygonGeometry> CreatePolygons(ReadOnlySpan<Vector2> vertices, PhysicsTransform transform, Vector2 vertexScale, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_CreatePolygons(vertices, transform, vertexScale, allocator).ToNativeArray<PolygonGeometry>();
	}

	public static PolygonGeometry CreateBox(Vector2 size, float radius, PhysicsTransform transform, bool inscribe = false)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_CreateBox(size, radius, transform, inscribe);
	}

	public static PolygonGeometry Create(ReadOnlySpan<Vector2> vertices, float radius = 0f)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_Create_WithPhysicsTransform(vertices, radius, PhysicsTransform.identity);
	}

	public static PolygonGeometry Create(ReadOnlySpan<Vector2> vertices, float radius, PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_Create_WithPhysicsTransform(vertices, radius, transform);
	}

	public static PolygonGeometry Create(ReadOnlySpan<Vector2> vertices, float radius, Matrix4x4 transform)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_Create_WithMatrix(vertices, radius, transform);
	}

	public static PolygonGeometry Create(ref ConvexHull convexHull, float radius)
	{
		PolygonGeometry polygonGeometry = new PolygonGeometry();
		polygonGeometry.vertices = convexHull.vertices;
		polygonGeometry.count = convexHull.count;
		polygonGeometry.radius = radius;
		return polygonGeometry.Validate();
	}

	public static PolygonGeometry InsertVertex(PolygonGeometry geometry, int index, Vector2 vertex)
	{
		if (geometry.count == 8 || (index < 0 && index >= 8))
		{
			throw new ArgumentOutOfRangeException("index", "Invalid index.");
		}
		geometry.count++;
		ref PhysicsShape.ShapeArray reference = ref geometry.vertices;
		for (int num = geometry.count - 1; num > index; num--)
		{
			reference[num] = reference[num - 1];
		}
		reference[index] = vertex;
		return geometry.Validate();
	}

	public static PolygonGeometry DeleteVertex(PolygonGeometry geometry, int index)
	{
		if (geometry.count == 3 || (index < 0 && index >= 8))
		{
			throw new ArgumentOutOfRangeException("index", "Invalid index.");
		}
		int num = geometry.count - 1;
		geometry.count = num;
		ref PhysicsShape.ShapeArray reference = ref geometry.vertices;
		for (int i = index; i < geometry.count; i++)
		{
			reference[i] = reference[i + 1];
		}
		return geometry.Validate();
	}

	public unsafe ReadOnlySpan<Vector2> AsReadOnlySpan()
	{
		fixed (Vector2* pointer = &vertices[0])
		{
			return new ReadOnlySpan<Vector2>(pointer, m_Count);
		}
	}

	public readonly PolygonGeometry Validate()
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_Validate(this);
	}

	public readonly PhysicsBody.MassConfiguration CalculateMassConfiguration(float density = 1f)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_CalculateMassConfiguration(this, density);
	}

	public readonly PhysicsAABB CalculateAABB(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_CalculateAABB(this, transform);
	}

	public readonly bool OverlapPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_OverlapPoint(this, point);
	}

	public readonly Vector2 ClosestPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_ClosestPoint(this, point);
	}

	public readonly PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_CastRay(this, castRayInput);
	}

	public readonly PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_CastShape(this, input);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CircleGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.PolygonAndCircle(this, transform, otherGeometry, otherTransform);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, CapsuleGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.PolygonAndCapsule(this, transform, otherGeometry, otherTransform);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, PolygonGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.PolygonAndPolygon(this, transform, otherGeometry, otherTransform);
	}

	public readonly PhysicsShape.ContactManifold Intersect(PhysicsTransform transform, SegmentGeometry otherGeometry, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.SegmentAndPolygon(otherGeometry, otherTransform, this, transform);
	}

	public readonly PolygonGeometry Transform(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_Transform_WithPhysicsTransform(this, transform);
	}

	public readonly PolygonGeometry InverseTransform(PhysicsTransform transform)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_InverseTransform_WithPhysicsTransform(this, transform);
	}

	public readonly PolygonGeometry Transform(Matrix4x4 transform, bool scaleRadius)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_Transform_WithMatrix(this, transform, scaleRadius);
	}

	public readonly PolygonGeometry InverseTransform(Matrix4x4 transform, bool scaleRadius)
	{
		return PhysicsLowLevelScripting2D.PolygonGeometry_InverseTransform_WithMatrix(this, transform, scaleRadius);
	}
}
