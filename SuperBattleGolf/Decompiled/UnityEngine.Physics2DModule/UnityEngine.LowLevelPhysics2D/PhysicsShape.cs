using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine.Serialization;

namespace UnityEngine.LowLevelPhysics2D;

public readonly struct PhysicsShape : IEquatable<PhysicsShape>
{
	[Serializable]
	public struct SurfaceMaterial
	{
		public enum MixingMode
		{
			Average,
			Mean,
			Multiply,
			Minimum,
			Maximum
		}

		[Min(0f)]
		[SerializeField]
		private float m_Friction;

		[Min(0f)]
		[SerializeField]
		private float m_Bounciness;

		[SerializeField]
		[FormerlySerializedAs("m_FrictionCombine")]
		private MixingMode m_FrictionMixing;

		[SerializeField]
		[FormerlySerializedAs("m_BouncinessCombine")]
		private MixingMode m_BouncinessMixing;

		[SerializeField]
		[Range(0f, 65535f)]
		private ushort m_FrictionPriority;

		[SerializeField]
		[Range(0f, 65535f)]
		private ushort m_BouncinessPriority;

		[SerializeField]
		[Min(0f)]
		private float m_RollingResistance;

		[SerializeField]
		private float m_TangentSpeed;

		[SerializeField]
		private Color32 m_CustomColor;

		[Obsolete("PhysicsShape.SurfaceMaterial.frictionCombine has been deprecated. Please use PhysicsShape.SurfaceMaterial.frictionMixing instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PhysicsMaterialCombine2D frictionCombine
		{
			readonly get
			{
				return (PhysicsMaterialCombine2D)frictionMixing;
			}
			set
			{
				frictionMixing = (MixingMode)value;
			}
		}

		[Obsolete("PhysicsShape.SurfaceMaterial.bouncinessCombine has been deprecated. Please use PhysicsShape.SurfaceMaterial.bouncinessMixing instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PhysicsMaterialCombine2D bouncinessCombine
		{
			readonly get
			{
				return (PhysicsMaterialCombine2D)bouncinessMixing;
			}
			set
			{
				bouncinessMixing = (MixingMode)value;
			}
		}

		public static SurfaceMaterial Default => PhysicsLowLevelScripting2D.PhysicsShape_GetDefaultSurfaceMaterial();

		public float friction
		{
			readonly get
			{
				return m_Friction;
			}
			set
			{
				m_Friction = Mathf.Max(0f, value);
			}
		}

		public float bounciness
		{
			readonly get
			{
				return m_Bounciness;
			}
			set
			{
				m_Bounciness = Mathf.Max(0f, value);
			}
		}

		public MixingMode frictionMixing
		{
			readonly get
			{
				return m_FrictionMixing;
			}
			set
			{
				m_FrictionMixing = value;
			}
		}

		public MixingMode bouncinessMixing
		{
			readonly get
			{
				return m_BouncinessMixing;
			}
			set
			{
				m_BouncinessMixing = value;
			}
		}

		public ushort frictionPriority
		{
			readonly get
			{
				return m_FrictionPriority;
			}
			set
			{
				m_FrictionPriority = value;
			}
		}

		public ushort bouncinessPriority
		{
			readonly get
			{
				return m_BouncinessPriority;
			}
			set
			{
				m_BouncinessPriority = value;
			}
		}

		public float rollingResistance
		{
			readonly get
			{
				return m_RollingResistance;
			}
			set
			{
				m_RollingResistance = Mathf.Max(0f, value);
			}
		}

		public float tangentSpeed
		{
			readonly get
			{
				return m_TangentSpeed;
			}
			set
			{
				m_TangentSpeed = value;
			}
		}

		public Color32 customColor
		{
			readonly get
			{
				return m_CustomColor;
			}
			set
			{
				m_CustomColor = value;
			}
		}

		public SurfaceMaterial()
		{
			this = Default;
		}
	}

	public enum ShapeType
	{
		Circle,
		Capsule,
		Segment,
		Polygon,
		ChainSegment
	}

	public readonly struct ContactManifold : IEnumerable<ContactManifold.ManifoldPoint>, IEnumerable
	{
		public readonly struct ManifoldPoint
		{
			private readonly Vector2 m_Point;

			private readonly Vector2 m_AnchorA;

			private readonly Vector2 m_AnchorB;

			private readonly float m_Separation;

			private readonly float m_NormalImpulse;

			private readonly float m_TangentImpulse;

			private readonly float m_TotalNormalImpulse;

			private readonly float m_NormalVelocity;

			private readonly ushort m_Id;

			private readonly bool m_Persisted;

			public Vector2 point => m_Point;

			public Vector2 anchorA => m_AnchorA;

			public Vector2 anchorB => m_AnchorB;

			public float separation => m_Separation;

			public float normalImpulse => m_NormalImpulse;

			public float tangentImpulse => m_TangentImpulse;

			public float totalNormalImpulse => m_TotalNormalImpulse;

			public float normalVelocity => m_NormalVelocity;

			public ushort id => m_Id;

			public bool persisted => m_Persisted;

			public bool speculative => totalNormalImpulse > 0f;
		}

		public readonly struct ManifoldPointArray
		{
			private readonly ManifoldPoint m_ContactInfo0;

			private readonly ManifoldPoint m_ContactInfo1;

			public ManifoldPoint contactInfo0 => m_ContactInfo0;

			public ManifoldPoint contactInfo1 => m_ContactInfo1;

			public unsafe ManifoldPoint this[int index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					if (index >= 0 && index < 2)
					{
						fixed (ManifoldPoint* ptr = &m_ContactInfo0)
						{
							return ptr[index];
						}
					}
					throw new IndexOutOfRangeException($"{index} must be in the range [0, 1]");
				}
			}

			public int speculativePointCount => (m_ContactInfo0.speculative ? 1 : 0) + (m_ContactInfo1.speculative ? 1 : 0);
		}

		public struct ManifoldPointIterator : IEnumerator<ManifoldPoint>, IEnumerator, IDisposable
		{
			private ContactManifold m_ContactManifold;

			private int m_PointIndex;

			readonly ManifoldPoint IEnumerator<ManifoldPoint>.Current => m_ContactManifold[m_PointIndex];

			private readonly object Current => m_ContactManifold[m_PointIndex];

			readonly object IEnumerator.Current => Current;

			public ManifoldPointIterator(ContactManifold contactManifold)
			{
				m_ContactManifold = contactManifold;
				m_PointIndex = -1;
			}

			bool IEnumerator.MoveNext()
			{
				return ++m_PointIndex < m_ContactManifold.pointCount;
			}

			void IEnumerator.Reset()
			{
				m_PointIndex = -1;
			}

			readonly void IDisposable.Dispose()
			{
			}
		}

		private readonly Vector2 m_Normal;

		private readonly float m_RollingImpulse;

		private readonly ManifoldPointArray m_Points;

		private readonly int m_PointCount;

		public Vector2 normal => m_Normal;

		public float rollingImpulse => m_RollingImpulse;

		public ManifoldPointArray points => m_Points;

		public int pointCount => m_PointCount;

		public int speculativePointCount => m_Points.speculativePointCount;

		public ManifoldPoint this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= 0 && index < pointCount)
				{
					return points[index];
				}
				throw new IndexOutOfRangeException($"{index} is not valid. The current number of valid points is {pointCount}");
			}
		}

		public IEnumerator<ManifoldPoint> GetEnumerator()
		{
			return new ManifoldPointIterator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ManifoldPointIterator(this);
		}
	}

	public readonly struct Contact
	{
		private readonly ContactId m_ContactId;

		private readonly PhysicsShape m_ShapeA;

		private readonly PhysicsShape m_ShapeB;

		private readonly ContactManifold m_Manifold;

		public ContactId contactId => m_ContactId;

		public PhysicsShape shapeA => m_ShapeA;

		public PhysicsShape shapeB => m_ShapeB;

		public ContactManifold manifold => m_Manifold;
	}

	public readonly struct ContactId
	{
		private readonly int m_IndexId;

		private readonly ushort m_WorldId;

		private readonly ushort m_Padding;

		private readonly int m_GenerationId;

		public bool isValid => PhysicsLowLevelScripting2D.PhysicsContactId_IsValid(this);

		public Contact contact => PhysicsLowLevelScripting2D.PhysicsContactId_GetContact(this);

		public override string ToString()
		{
			return isValid ? $"index={m_IndexId}, world={m_WorldId}, generation={m_GenerationId}" : "<INVALID>";
		}
	}

	[Serializable]
	public struct ContactFilter
	{
		public static PhysicsMask DefaultCategories = PhysicsMask.One;

		public static PhysicsMask DefaultContacts = PhysicsMask.All;

		public static ContactFilter Everything = new ContactFilter(PhysicsMask.All, PhysicsMask.All);

		public static ContactFilter defaultFilter = new ContactFilter(DefaultCategories, DefaultContacts);

		[SerializeField]
		internal PhysicsMask m_Categories;

		[SerializeField]
		internal PhysicsMask m_Contacts;

		[SerializeField]
		internal int m_GroupIndex;

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

		public PhysicsMask contacts
		{
			readonly get
			{
				return m_Contacts;
			}
			set
			{
				m_Contacts = value;
			}
		}

		public int groupIndex
		{
			readonly get
			{
				return m_GroupIndex;
			}
			set
			{
				m_GroupIndex = value;
			}
		}

		public ContactFilter(PhysicsMask categories, PhysicsMask contacts, int groupIndex = 0)
		{
			m_Categories = categories;
			m_Contacts = contacts;
			m_GroupIndex = groupIndex;
		}
	}

	[Serializable]
	public struct ShapeArray
	{
		[SerializeField]
		internal Vector2 m_Vertex0;

		[SerializeField]
		private Vector2 m_Vertex1;

		[SerializeField]
		private Vector2 m_Vertex2;

		[SerializeField]
		private Vector2 m_Vertex3;

		[SerializeField]
		private Vector2 m_Vertex4;

		[SerializeField]
		private Vector2 m_Vertex5;

		[SerializeField]
		private Vector2 m_Vertex6;

		[SerializeField]
		private Vector2 m_Vertex7;

		public Vector2 vertex0
		{
			readonly get
			{
				return m_Vertex0;
			}
			set
			{
				m_Vertex0 = value;
			}
		}

		public Vector2 vertex1
		{
			readonly get
			{
				return m_Vertex1;
			}
			set
			{
				m_Vertex1 = value;
			}
		}

		public Vector2 vertex2
		{
			readonly get
			{
				return m_Vertex2;
			}
			set
			{
				m_Vertex2 = value;
			}
		}

		public Vector2 vertex3
		{
			readonly get
			{
				return m_Vertex3;
			}
			set
			{
				m_Vertex3 = value;
			}
		}

		public Vector2 vertex4
		{
			readonly get
			{
				return m_Vertex4;
			}
			set
			{
				m_Vertex4 = value;
			}
		}

		public Vector2 vertex5
		{
			readonly get
			{
				return m_Vertex5;
			}
			set
			{
				m_Vertex5 = value;
			}
		}

		public Vector2 vertex6
		{
			readonly get
			{
				return m_Vertex6;
			}
			set
			{
				m_Vertex6 = value;
			}
		}

		public Vector2 vertex7
		{
			readonly get
			{
				return m_Vertex7;
			}
			set
			{
				m_Vertex7 = value;
			}
		}

		public unsafe ref Vector2 this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (index >= 0 && index < 8)
				{
					fixed (Vector2* ptr = &m_Vertex0)
					{
						return ref ptr[index];
					}
				}
				throw new IndexOutOfRangeException($"{index} must be in the range [0, {7}]");
			}
		}
	}

	[Serializable]
	public struct MoverData
	{
		[Min(0f)]
		[SerializeField]
		private float m_PushLimit;

		[SerializeField]
		private bool m_ClipVelocity;

		public float pushLimit
		{
			readonly get
			{
				return m_PushLimit;
			}
			set
			{
				m_PushLimit = Mathf.Max(0f, value);
			}
		}

		public bool clipVelocity
		{
			readonly get
			{
				return m_ClipVelocity;
			}
			set
			{
				m_ClipVelocity = value;
			}
		}

		public MoverData()
		{
			m_PushLimit = float.MaxValue;
			m_ClipVelocity = true;
		}
	}

	[Serializable]
	public struct ShapeProxy
	{
		[SerializeField]
		private ShapeArray m_Vertices;

		[SerializeField]
		[Min(1f)]
		private int m_Count;

		[SerializeField]
		[Min(0f)]
		private float m_Radius;

		public CircleGeometry circleGeometry
		{
			get
			{
				if (m_Count == 1)
				{
					CircleGeometry result = new CircleGeometry();
					result.center = m_Vertices[0];
					result.radius = m_Radius;
					return result;
				}
				throw new InvalidOperationException("Expected a vertex count of 1.");
			}
		}

		public CapsuleGeometry capsuleGeometry
		{
			get
			{
				if (m_Count == 2)
				{
					CapsuleGeometry result = new CapsuleGeometry();
					result.center1 = m_Vertices[0];
					result.center2 = m_Vertices[1];
					result.radius = m_Radius;
					return result;
				}
				throw new InvalidOperationException("Expected a vertex count of 2.");
			}
		}

		public unsafe PolygonGeometry polygonGeometry
		{
			get
			{
				fixed (Vector2* vertex = &m_Vertices.m_Vertex0)
				{
					return PolygonGeometry.Create(new ReadOnlySpan<Vector2>(vertex, m_Count), m_Radius);
				}
			}
		}

		public SegmentGeometry segmentGeometry
		{
			get
			{
				if (m_Count == 2)
				{
					SegmentGeometry result = new SegmentGeometry();
					result.point1 = m_Vertices[0];
					result.point2 = m_Vertices[1];
					return result;
				}
				throw new InvalidOperationException("Expected a vertex count of 2.");
			}
		}

		public ShapeArray vertices
		{
			readonly get
			{
				return m_Vertices;
			}
			set
			{
				m_Vertices = value;
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
				m_Count = Mathf.Max(1, value);
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

		public ShapeProxy(Vector2 point)
		{
			m_Vertices = new ShapeArray
			{
				vertex0 = point
			};
			m_Count = 1;
			m_Radius = 0f;
		}

		public ShapeProxy(CircleGeometry circleGeometry)
		{
			if (!circleGeometry.isValid)
			{
				throw new ArgumentException("circleGeometry", "Circle Geometry is not valid.");
			}
			m_Vertices = new ShapeArray
			{
				vertex0 = circleGeometry.center
			};
			m_Count = 1;
			m_Radius = circleGeometry.radius;
		}

		public ShapeProxy(CapsuleGeometry capsuleGeometry)
		{
			if (!capsuleGeometry.isValid)
			{
				throw new ArgumentException("capsuleGeometry", "Capsule Geometry is not valid.");
			}
			m_Vertices = new ShapeArray
			{
				vertex0 = capsuleGeometry.center1,
				vertex1 = capsuleGeometry.center2
			};
			m_Count = 2;
			m_Radius = capsuleGeometry.radius;
		}

		public ShapeProxy(PolygonGeometry polygonGeometry)
		{
			if (!polygonGeometry.isValid)
			{
				throw new ArgumentException("polygonGeometry", "Polygon Geometry is not valid.");
			}
			m_Vertices = polygonGeometry.vertices;
			m_Count = polygonGeometry.count;
			m_Radius = polygonGeometry.radius;
		}

		public ShapeProxy(SegmentGeometry segmentGeometry)
		{
			if (!segmentGeometry.isValid)
			{
				throw new ArgumentException("segmentGeometry", "Segment Geometry is not valid.");
			}
			m_Vertices = new ShapeArray
			{
				vertex0 = segmentGeometry.point1,
				vertex1 = segmentGeometry.point2
			};
			m_Count = 2;
			m_Radius = 0f;
		}

		public ShapeProxy(ChainSegmentGeometry chainSegmentGeometry)
		{
			if (!chainSegmentGeometry.isValid)
			{
				throw new ArgumentException("chainSegmentGeometry", "Chain Segment Geometry is not valid.");
			}
			m_Vertices = new ShapeArray
			{
				vertex0 = chainSegmentGeometry.segment.point1,
				vertex1 = chainSegmentGeometry.segment.point2
			};
			m_Count = 2;
			m_Radius = 0f;
		}
	}

	private readonly int m_Index1;

	private readonly ushort m_World0;

	private readonly ushort m_Generation;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("PhysicsShape.frictionCombine has been deprecated. Please use PhysicsShape.frictionMixing instead.", false)]
	public PhysicsMaterialCombine2D frictionCombine
	{
		get
		{
			return (PhysicsMaterialCombine2D)frictionMixing;
		}
		set
		{
			frictionMixing = (SurfaceMaterial.MixingMode)value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("PhysicsShape.bouncinessCombine has been deprecated. Please use PhysicsShape.bouncinessMixing instead.", false)]
	public PhysicsMaterialCombine2D bouncinessCombine
	{
		get
		{
			return (PhysicsMaterialCombine2D)bouncinessMixing;
		}
		set
		{
			bouncinessMixing = (SurfaceMaterial.MixingMode)value;
		}
	}

	public PhysicsShapeDefinition definition
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_ReadDefinition(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_WriteDefinition(this, value, onlyExtendedProperties: false);
		}
	}

	public bool isValid => PhysicsLowLevelScripting2D.PhysicsShape_IsValid(this);

	public PhysicsWorld world => PhysicsLowLevelScripting2D.PhysicsShape_GetWorld(this);

	public PhysicsBody body => PhysicsLowLevelScripting2D.PhysicsShape_GetBody(this);

	public bool isTrigger
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetIsTrigger(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetIsTrigger(this, value);
		}
	}

	public ShapeType shapeType => PhysicsLowLevelScripting2D.PhysicsShape_GetShapeType(this);

	public PhysicsTransform transform => body.transform;

	public PhysicsBody.MassConfiguration massConfiguration => PhysicsLowLevelScripting2D.PhysicsShape_GetMassConfiguration(this);

	public float friction
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetFriction(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetFriction(this, value);
		}
	}

	public float bounciness
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetBounciness(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetBounciness(this, value);
		}
	}

	public SurfaceMaterial.MixingMode frictionMixing
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetFrictionMixing(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetFrictionMixing(this, value);
		}
	}

	public SurfaceMaterial.MixingMode bouncinessMixing
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetBouncinessMixing(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetBouncinessMixing(this, value);
		}
	}

	public ushort frictionPriority
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetFrictionPriority(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetFrictionPriority(this, value);
		}
	}

	public ushort bouncinessPriority
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetBouncinessPriority(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetBouncinessPriority(this, value);
		}
	}

	public float rollingResistance
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetRollingResistance(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetRollingResistance(this, value);
		}
	}

	public float tangentSpeed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetTangentSpeed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetTangentSpeed(this, value);
		}
	}

	public Color32 customColor
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetCustomColor(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetCustomColor(this, value);
		}
	}

	public SurfaceMaterial surfaceMaterial
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetSurfaceMaterial(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetSurfaceMaterial(this, value);
		}
	}

	public ContactFilter contactFilter
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetContactFilter(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetContactFilter(this, value);
		}
	}

	public MoverData moverData
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetMoverData(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetMoverData(this, value);
		}
	}

	public bool triggerEvents
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetTriggerEvents(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetTriggerEvents(this, value);
		}
	}

	public bool contactEvents
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetContactEvents(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetContactEvents(this, value);
		}
	}

	public bool hitEvents
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetHitEvents(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetHitEvents(this, value);
		}
	}

	public bool contactFilterCallbacks
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetContactFilterCallbacks(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetContacFiltertCallbacks(this, value);
		}
	}

	public bool preSolveCallbacks
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetPreSolveCallbacks(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetPreSolveCallbacks(this, value);
		}
	}

	public bool startStaticContacts => PhysicsLowLevelScripting2D.PhysicsShape_GetStartStaticContacts(this);

	public bool startMassUpdate => PhysicsLowLevelScripting2D.PhysicsShape_GetStartMassUpdate(this);

	public CircleGeometry circleGeometry
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetCircleGeometry(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetCircleGeometry(this, value);
		}
	}

	public CapsuleGeometry capsuleGeometry
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetCapsuleGeometry(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetCapsuleGeometry(this, value);
		}
	}

	public PolygonGeometry polygonGeometry
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetPolygonGeometry(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetPolygonGeometry(this, value);
		}
	}

	public SegmentGeometry segmentGeometry
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetSegmentGeometry(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetSegmentGeometry(this, value);
		}
	}

	public ChainSegmentGeometry chainSegmentGeometry => PhysicsLowLevelScripting2D.PhysicsShape_GetChainSegmentGeometry(this);

	public bool isChainSegment => PhysicsLowLevelScripting2D.PhysicsShape_IsChainSegmentShape(this);

	public PhysicsChain chain => PhysicsLowLevelScripting2D.PhysicsShape_GetChain(this);

	public PhysicsAABB aabb => PhysicsLowLevelScripting2D.PhysicsShape_CalculateAABB(this);

	public Vector2 localCenter => PhysicsLowLevelScripting2D.PhysicsShape_GetLocalCenter(this);

	public bool isOwned => PhysicsLowLevelScripting2D.PhysicsShape_IsOwned(this);

	public object callbackTarget
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetCallbackTarget(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetCallbackTarget(this, value);
		}
	}

	public PhysicsUserData userData
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsShape_GetUserData(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsShape_SetUserData(this, value);
		}
	}

	public override string ToString()
	{
		return isValid ? $"type={shapeType}, index={m_Index1}, world={m_World0}, generation={m_Generation}" : "<INVALID>";
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public bool Equals(PhysicsShape other)
	{
		return m_Index1 == other.m_Index1 && m_World0 == other.m_World0 && m_Generation == other.m_Generation;
	}

	public static bool operator ==(PhysicsShape lhs, PhysicsShape rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PhysicsShape lhs, PhysicsShape rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(m_Index1, m_World0, m_Generation);
	}

	public static PhysicsShape CreateShape(PhysicsBody body, CircleGeometry geometry)
	{
		return CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
	}

	public static PhysicsShape CreateShape(PhysicsBody body, CircleGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateCircleShape(body, geometry, definition);
	}

	public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<CircleGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry), ShapeType.Circle, definition, allocator).ToNativeArray<PhysicsShape>();
	}

	public static PhysicsShape CreateShape(PhysicsBody body, PolygonGeometry geometry)
	{
		return CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
	}

	public static PhysicsShape CreateShape(PhysicsBody body, PolygonGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreatePolygonShape(body, geometry, definition);
	}

	public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<PolygonGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry), ShapeType.Polygon, definition, allocator).ToNativeArray<PhysicsShape>();
	}

	public static PhysicsShape CreateShape(PhysicsBody body, CapsuleGeometry geometry)
	{
		return CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
	}

	public static PhysicsShape CreateShape(PhysicsBody body, CapsuleGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateCapsuleShape(body, geometry, definition);
	}

	public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<CapsuleGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry), ShapeType.Capsule, definition, allocator).ToNativeArray<PhysicsShape>();
	}

	public static PhysicsShape CreateShape(PhysicsBody body, SegmentGeometry geometry)
	{
		return CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
	}

	public static PhysicsShape CreateShape(PhysicsBody body, SegmentGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateSegmentShape(body, geometry, definition);
	}

	public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<SegmentGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry), ShapeType.Segment, definition, allocator).ToNativeArray<PhysicsShape>();
	}

	public static PhysicsShape CreateShape(PhysicsBody body, ChainSegmentGeometry geometry, PhysicsShapeDefinition definition)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateChainSegmenShapet(body, geometry, definition);
	}

	public static PhysicsShape CreateShape(PhysicsBody body, ChainSegmentGeometry geometry)
	{
		return CreateShape(body, geometry, PhysicsShapeDefinition.defaultDefinition);
	}

	public static NativeArray<PhysicsShape> CreateShapeBatch(PhysicsBody body, ReadOnlySpan<ChainSegmentGeometry> geometry, PhysicsShapeDefinition definition, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CreateShapeBatch(body, PhysicsLowLevelScripting2D.PhysicsBuffer.FromSpan(geometry), ShapeType.ChainSegment, definition, allocator).ToNativeArray<PhysicsShape>();
	}

	public bool Destroy(bool updateBodyMass = true, int ownerKey = 0)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_Destroy(this, updateBodyMass, ownerKey);
	}

	public static void DestroyBatch(ReadOnlySpan<PhysicsShape> shapes, bool updateBodyMass)
	{
		PhysicsLowLevelScripting2D.PhysicsShape_DestroyBatch(shapes, updateBodyMass);
	}

	public void SetDensity(float density, bool updateBodyMass)
	{
		PhysicsLowLevelScripting2D.PhysicsShape_SetDensity(this, density, updateBodyMass);
	}

	public float GetDensity()
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_GetDensity(this);
	}

	public void ApplyWind(Vector2 force, float drag, float lift, bool wake = true)
	{
		PhysicsLowLevelScripting2D.PhysicsShape_ApplyWind(this, force, drag, lift, wake);
	}

	public bool OverlapPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_OverlapPoint(this, point);
	}

	public Vector2 ClosestPoint(Vector2 point)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_ClosestPoint(this, point);
	}

	public PhysicsQuery.CastResult CastRay(PhysicsQuery.CastRayInput castRayInput)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CastRay(this, castRayInput);
	}

	public PhysicsQuery.CastResult CastShape(PhysicsQuery.CastShapeInput input)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_CastShape(this, input);
	}

	public ContactManifold Intersect(PhysicsShape otherShape)
	{
		return PhysicsQuery.ShapeAndShape(this, body.transform, otherShape, otherShape.body.transform);
	}

	public ContactManifold Intersect(PhysicsTransform transform, PhysicsShape otherShape, PhysicsTransform otherTransform)
	{
		return PhysicsQuery.ShapeAndShape(this, transform, otherShape, otherTransform);
	}

	public NativeArray<Contact> GetContacts(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_GetContacts(this, allocator).ToNativeArray<Contact>();
	}

	public NativeArray<PhysicsShape> GetTriggerVisitors(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_GetTriggerVisitors(this, allocator).ToNativeArray<PhysicsShape>();
	}

	public float GetPerimeter()
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_GetPerimeter(this);
	}

	public float GetPerimeterProjected(Vector2 axis)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_GetPerimeterProjected(this, axis);
	}

	public int SetOwner(Object owner)
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_SetOwner(this, owner);
	}

	public Object GetOwner()
	{
		return PhysicsLowLevelScripting2D.PhysicsShape_GetOwner(this);
	}

	public ShapeProxy CreateShapeProxy()
	{
		if (!isValid)
		{
			throw new ArgumentException("PhysicsShape is not valid.");
		}
		ShapeType shapeType = this.shapeType;
		if (1 == 0)
		{
		}
		ShapeProxy result = shapeType switch
		{
			ShapeType.Circle => new ShapeProxy(circleGeometry), 
			ShapeType.Capsule => new ShapeProxy(capsuleGeometry), 
			ShapeType.Segment => new ShapeProxy(segmentGeometry), 
			ShapeType.Polygon => new ShapeProxy(polygonGeometry), 
			_ => throw new ArgumentException("PhysicsShape cannot be a Chain."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public void Draw()
	{
		PhysicsLowLevelScripting2D.PhysicsShape_Draw(this);
	}
}
