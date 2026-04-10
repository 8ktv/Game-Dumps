using System;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D;

public readonly struct PhysicsComposer : IEquatable<PhysicsComposer>
{
	[Serializable]
	internal readonly struct Layer
	{
		public enum LayerType
		{
			Geometry,
			Shape,
			Vertex
		}

		private readonly LayerType m_LayerType;

		private readonly PhysicsShape.ShapeType m_GeometryType;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_DataBuffer;

		private readonly PhysicsTransform m_Transform;

		private readonly Operation m_Operation;

		private readonly int m_Order;

		private readonly float m_CurveStride;

		private readonly bool m_ReverseWinding;

		public LayerType layerType => m_LayerType;

		public PhysicsShape.ShapeType geometryType => m_GeometryType;

		public PhysicsLowLevelScripting2D.PhysicsBuffer dataBuffer => m_DataBuffer;

		public PhysicsTransform transform => m_Transform;

		public Operation operation => m_Operation;

		public int order => m_Order;

		public float curveStride => m_CurveStride;

		public bool reverseWinding => m_ReverseWinding;

		internal Layer(ReadOnlySpan<CircleGeometry> geometry, PhysicsTransform transform, Operation operation, int order, float curveStride, bool reverseWinding)
		{
			if (geometry.Length < 1)
			{
				throw new ArgumentOutOfRangeException("geometry", "At least a single geometry must be specified.");
			}
			if (curveStride < 0.01f || curveStride > 1f)
			{
				throw new ArgumentOutOfRangeException("curveStride", $"Curve Stride must be in the range [{0.01f}, 1.0]");
			}
			m_LayerType = LayerType.Geometry;
			m_GeometryType = PhysicsShape.ShapeType.Circle;
			m_DataBuffer = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry);
			m_Transform = transform;
			m_Operation = operation;
			m_Order = order;
			m_CurveStride = curveStride;
			m_ReverseWinding = reverseWinding;
		}

		internal Layer(ReadOnlySpan<CapsuleGeometry> geometry, PhysicsTransform transform, Operation operation, int order, float curveStride, bool reverseWinding)
		{
			if (geometry.Length < 1)
			{
				throw new ArgumentOutOfRangeException("geometry", "At least a single geometry must be specified.");
			}
			if (curveStride < 0.01f || curveStride > 1f)
			{
				throw new ArgumentOutOfRangeException("curveStride", $"Curve Stride must be in the range [{0.01f}, 1.0]");
			}
			m_LayerType = LayerType.Geometry;
			m_GeometryType = PhysicsShape.ShapeType.Capsule;
			m_DataBuffer = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry);
			m_Transform = transform;
			m_Operation = operation;
			m_Order = order;
			m_CurveStride = curveStride;
			m_ReverseWinding = reverseWinding;
		}

		internal Layer(ReadOnlySpan<PolygonGeometry> geometry, PhysicsTransform transform, Operation operation, int order, float curveStride, bool reverseWinding)
		{
			if (geometry.Length < 1)
			{
				throw new ArgumentOutOfRangeException("geometry", "At least a single geometry must be specified.");
			}
			if (curveStride < 0.01f || curveStride > 1f)
			{
				throw new ArgumentOutOfRangeException("curveStride", $"Curve Stride must be in the range [{0.01f}, 1.0]");
			}
			m_LayerType = LayerType.Geometry;
			m_GeometryType = PhysicsShape.ShapeType.Polygon;
			m_DataBuffer = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry);
			m_Transform = transform;
			m_Operation = operation;
			m_Order = order;
			m_CurveStride = curveStride;
			m_ReverseWinding = reverseWinding;
		}

		internal Layer(ReadOnlySpan<PhysicsShape> shapes, PhysicsTransform transform, Operation operation, int order, float curveStride, bool reverseWinding)
		{
			if (shapes.Length < 1)
			{
				throw new ArgumentOutOfRangeException("shapes", "At least a single PhysicsShape must be specified.");
			}
			ReadOnlySpan<PhysicsShape> readOnlySpan = shapes;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				PhysicsShape physicsShape = readOnlySpan[i];
				if (!physicsShape.isValid)
				{
					throw new ArgumentException("shapes", "At least one of the shapes was invalid.");
				}
			}
			if (curveStride < 0.01f || curveStride > 1f)
			{
				throw new ArgumentOutOfRangeException("curveStride", $"Curve Stride must be in the range [{0.01f}, 1.0]");
			}
			m_LayerType = LayerType.Shape;
			m_DataBuffer = PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(shapes);
			m_Transform = transform;
			m_Operation = operation;
			m_Order = order;
			m_CurveStride = curveStride;
			m_ReverseWinding = reverseWinding;
			m_GeometryType = PhysicsShape.ShapeType.Circle;
		}

		internal Layer(PhysicsLowLevelScripting2D.PhysicsBuffer vertices, PhysicsTransform transform, Operation operation, int order, bool reverseWinding)
		{
			if (vertices.size < 3)
			{
				throw new ArgumentOutOfRangeException("vertices", "A minimum of 3 vertices must be specified.");
			}
			m_LayerType = LayerType.Vertex;
			m_DataBuffer = vertices;
			m_Transform = transform;
			m_Operation = operation;
			m_Order = order;
			m_CurveStride = 1f;
			m_ReverseWinding = reverseWinding;
			m_GeometryType = PhysicsShape.ShapeType.Circle;
		}
	}

	public readonly struct LayerHandle
	{
		private readonly int m_IndexId;

		private readonly int m_Composer;

		private readonly ushort m_Revision;

		public override string ToString()
		{
			return $"index={m_IndexId}, composer={m_Composer}, generation={m_Revision}";
		}
	}

	public enum Operation
	{
		OR,
		AND,
		NOT,
		XOR
	}

	private readonly int m_Index1;

	private readonly ushort m_Generation;

	public const float DefaultCurveStride = 0.06f;

	public const float MinCurveStride = 0.01f;

	public bool isValid => PhysicsComposerScripting2D.Composer_IsValid(this);

	public bool useDelaunay
	{
		get
		{
			return PhysicsComposerScripting2D.PhysicsComposer_GetDelaunay(this);
		}
		set
		{
			PhysicsComposerScripting2D.PhysicsComposer_SetDelaunay(this, value);
		}
	}

	public int maxPolygonVertices
	{
		get
		{
			return PhysicsComposerScripting2D.PhysicsComposer_GetMaxPolygonVertices(this);
		}
		set
		{
			PhysicsComposerScripting2D.PhysicsComposer_SetMaxPolygonVertices(this, value);
		}
	}

	public int layerCount => PhysicsComposerScripting2D.PhysicsComposer_GetLayerCount(this);

	public NativeArray<LayerHandle> layerHandles => PhysicsComposerScripting2D.PhysicsComposer_GetLayerHandles(this).ToNativeArray<LayerHandle>();

	public int rejectedGeometryCount => PhysicsComposerScripting2D.PhysicsComposer_GetRejectedGeometryCount(this);

	public override string ToString()
	{
		return isValid ? $"index={m_Index1}, generation={m_Generation}" : "<INVALID>";
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public bool Equals(PhysicsComposer other)
	{
		return m_Index1 == other.m_Index1 && m_Generation == other.m_Generation;
	}

	public static bool operator ==(PhysicsComposer lhs, PhysicsComposer rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PhysicsComposer lhs, PhysicsComposer rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(m_Index1, m_Generation);
	}

	public static PhysicsComposer Create(Allocator allocator = Allocator.Temp)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_Create(allocator);
	}

	public bool Destroy()
	{
		return PhysicsComposerScripting2D.PhysicsComposer_Destroy(this);
	}

	public unsafe LayerHandle AddLayer(CircleGeometry geometry, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
	{
		return AddLayer(new ReadOnlySpan<CircleGeometry>(&geometry, 1), transform, operation, order, curveStride, reverseWinding);
	}

	public LayerHandle AddLayer(ReadOnlySpan<CircleGeometry> geometry, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, new Layer(geometry, transform, operation, order, curveStride, reverseWinding));
	}

	public unsafe LayerHandle AddLayer(CapsuleGeometry geometry, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
	{
		return AddLayer(new ReadOnlySpan<CapsuleGeometry>(&geometry, 1), transform, operation, order, curveStride, reverseWinding);
	}

	public LayerHandle AddLayer(ReadOnlySpan<CapsuleGeometry> geometry, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, new Layer(geometry, transform, operation, order, curveStride, reverseWinding));
	}

	public unsafe LayerHandle AddLayer(PolygonGeometry geometry, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
	{
		return AddLayer(new ReadOnlySpan<PolygonGeometry>(&geometry, 1), transform, operation, order, curveStride, reverseWinding);
	}

	public LayerHandle AddLayer(ReadOnlySpan<PolygonGeometry> geometry, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, new Layer(geometry, transform, operation, order, curveStride, reverseWinding));
	}

	public unsafe LayerHandle AddLayer(PhysicsShape shape, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
	{
		return AddLayer(new ReadOnlySpan<PhysicsShape>(&shape, 1), transform, operation, order, curveStride, reverseWinding);
	}

	public LayerHandle AddLayer(ReadOnlySpan<PhysicsShape> shapes, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, float curveStride = 0.06f, bool reverseWinding = false)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(this, new Layer(shapes, transform, operation, order, curveStride, reverseWinding));
	}

	public LayerHandle AddLayer(ReadOnlySpan<Vector2> vertices, PhysicsTransform transform, Operation operation = Operation.OR, int order = 0, bool reverseWinding = false)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_AddLayer(layer: new Layer(PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(vertices), transform, operation, order, reverseWinding), composer: this);
	}

	public void RemoveLayer(LayerHandle layerHandle)
	{
		PhysicsComposerScripting2D.PhysicsComposer_RemoveLayer(this, layerHandle);
	}

	public void ClearLayers()
	{
		PhysicsComposerScripting2D.PhysicsComposer_ClearLayers(this);
	}

	public NativeArray<RangeInt> GetGeometryIslands(Allocator allocator)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_GetGeometryIslands(this, allocator).ToNativeArray<RangeInt>();
	}

	public NativeArray<PolygonGeometry> CreatePolygonGeometry(Vector2 vertexScale, Allocator allocator)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_CreatePolygonGeometry(this, vertexScale, allocator).ToNativeArray<PolygonGeometry>();
	}

	public NativeArray<PolygonGeometry.ConvexHull> CreateConvexHulls(Vector2 vertexScale, Allocator allocator)
	{
		return PhysicsComposerScripting2D.PhysicsComposer_CreateConvexHulls(this, vertexScale, allocator).ToNativeArray<PolygonGeometry.ConvexHull>();
	}

	public NativeArray<ChainGeometry> CreateChainGeometry(out NativeArray<Vector2> vertices, Vector2 vertexScale, Allocator allocator)
	{
		PhysicsLowLevelScripting2D.PhysicsBufferPair physicsBufferPair = PhysicsComposerScripting2D.PhysicsComposer_CreateChainGeometry(this, vertexScale, allocator);
		vertices = physicsBufferPair.buffer1.ToNativeArray<Vector2>();
		using NativeArray<PhysicsLowLevelScripting2D.PhysicsBuffer> nativeArray = physicsBufferPair.buffer2.ToNativeArray<PhysicsLowLevelScripting2D.PhysicsBuffer>();
		if (vertices.Length == 0 || nativeArray.Length == 0)
		{
			if (vertices.Length > 0)
			{
				vertices.Dispose();
			}
			return default(NativeArray<ChainGeometry>);
		}
		NativeArray<ChainGeometry> result = new NativeArray<ChainGeometry>(nativeArray.Length, physicsBufferPair.buffer2.allocator, NativeArrayOptions.UninitializedMemory);
		for (int i = 0; i < nativeArray.Length; i++)
		{
			result[i] = new ChainGeometry(nativeArray[i].ToSpan<Vector2>());
		}
		return result;
	}
}
