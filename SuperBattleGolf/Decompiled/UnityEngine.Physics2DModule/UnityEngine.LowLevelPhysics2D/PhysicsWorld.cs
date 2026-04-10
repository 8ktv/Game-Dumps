using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.LowLevelPhysics2D;

public readonly struct PhysicsWorld : IEquatable<PhysicsWorld>
{
	public enum SimulationType
	{
		FixedUpdate,
		Update,
		Script
	}

	public enum TransformWriteMode
	{
		Off,
		Fast2D,
		Slow3D
	}

	public enum TransformPlane
	{
		XY,
		XZ,
		ZY
	}

	[Serializable]
	public struct ExplosionDefinition
	{
		[SerializeField]
		private PhysicsMask m_HitCategories;

		[SerializeField]
		private Vector2 m_Position;

		[Min(0f)]
		[SerializeField]
		private float m_Radius;

		[SerializeField]
		[Min(0f)]
		private float m_Falloff;

		[SerializeField]
		private float m_ImpulsePerLength;

		public static ExplosionDefinition defaultDefinition => PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultExplosionDefinition();

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

		public Vector2 position
		{
			readonly get
			{
				return m_Position;
			}
			set
			{
				m_Position = value;
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

		public float falloff
		{
			readonly get
			{
				return m_Falloff;
			}
			set
			{
				m_Falloff = Mathf.Max(0f, value);
			}
		}

		public float impulsePerLength
		{
			readonly get
			{
				return m_ImpulsePerLength;
			}
			set
			{
				m_ImpulsePerLength = value;
			}
		}

		public ExplosionDefinition()
		{
			this = PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultExplosionDefinition();
		}
	}

	[Serializable]
	public struct WorldCounters
	{
		[SerializeField]
		private int m_BodyCount;

		[SerializeField]
		private int m_ShapeCount;

		[SerializeField]
		private int m_ContactCount;

		[SerializeField]
		private int m_JointCount;

		[SerializeField]
		private int m_IslandCount;

		[SerializeField]
		private int m_StackUsed;

		[SerializeField]
		private int m_StaticBroadphaseHeight;

		[SerializeField]
		private int m_BroadphaseHeight;

		[SerializeField]
		private int m_MemoryUsed;

		[SerializeField]
		private int m_TaskCount;

		private unsafe fixed int m_ColorCounts[24];

		public int bodyCount
		{
			readonly get
			{
				return m_BodyCount;
			}
			set
			{
				m_BodyCount = value;
			}
		}

		public int shapeCount
		{
			readonly get
			{
				return m_ShapeCount;
			}
			set
			{
				m_ShapeCount = value;
			}
		}

		public int contactCount
		{
			readonly get
			{
				return m_ContactCount;
			}
			set
			{
				m_ContactCount = value;
			}
		}

		public int jointCount
		{
			readonly get
			{
				return m_JointCount;
			}
			set
			{
				m_JointCount = value;
			}
		}

		public int islandCount
		{
			readonly get
			{
				return m_IslandCount;
			}
			set
			{
				m_IslandCount = value;
			}
		}

		public int stackUsed
		{
			readonly get
			{
				return m_StackUsed;
			}
			set
			{
				m_StackUsed = value;
			}
		}

		public int memoryUsed
		{
			readonly get
			{
				return m_MemoryUsed;
			}
			set
			{
				m_MemoryUsed = value;
			}
		}

		public int staticBroadphaseHeight
		{
			readonly get
			{
				return m_StaticBroadphaseHeight;
			}
			set
			{
				m_StaticBroadphaseHeight = value;
			}
		}

		public int broadphaseHeight
		{
			readonly get
			{
				return m_BroadphaseHeight;
			}
			set
			{
				m_BroadphaseHeight = value;
			}
		}

		public int taskCount
		{
			readonly get
			{
				return m_TaskCount;
			}
			set
			{
				m_TaskCount = value;
			}
		}

		public static WorldCounters Add(WorldCounters countersA, WorldCounters countersB)
		{
			return new WorldCounters
			{
				bodyCount = countersA.bodyCount + countersB.bodyCount,
				shapeCount = countersA.shapeCount + countersB.shapeCount,
				contactCount = countersA.contactCount + countersB.contactCount,
				jointCount = countersA.jointCount + countersB.jointCount,
				islandCount = countersA.islandCount + countersB.islandCount,
				stackUsed = countersA.stackUsed + countersB.stackUsed,
				memoryUsed = countersA.memoryUsed + countersB.memoryUsed,
				staticBroadphaseHeight = countersA.staticBroadphaseHeight + countersB.staticBroadphaseHeight,
				broadphaseHeight = countersA.broadphaseHeight + countersB.broadphaseHeight,
				taskCount = countersA.taskCount + countersB.taskCount
			};
		}

		public static WorldCounters Maximum(WorldCounters countersA, WorldCounters countersB)
		{
			return new WorldCounters
			{
				bodyCount = Mathf.Max(countersA.bodyCount, countersB.bodyCount),
				shapeCount = Mathf.Max(countersA.shapeCount, countersB.shapeCount),
				contactCount = Mathf.Max(countersA.contactCount, countersB.contactCount),
				jointCount = Mathf.Max(countersA.jointCount, countersB.jointCount),
				islandCount = Mathf.Max(countersA.islandCount, countersB.islandCount),
				stackUsed = Mathf.Max(countersA.stackUsed, countersB.stackUsed),
				memoryUsed = Mathf.Max(countersA.memoryUsed, countersB.memoryUsed),
				staticBroadphaseHeight = Mathf.Max(countersA.staticBroadphaseHeight, countersB.staticBroadphaseHeight),
				broadphaseHeight = Mathf.Max(countersA.broadphaseHeight, countersB.broadphaseHeight),
				taskCount = Mathf.Max(countersA.taskCount, countersB.taskCount)
			};
		}
	}

	[Serializable]
	public struct WorldProfile
	{
		[SerializeField]
		private float m_SimulationStep;

		[SerializeField]
		private float m_ContactPairs;

		[SerializeField]
		private float m_ContactUpdates;

		[SerializeField]
		private float m_Solving;

		[SerializeField]
		private float m_PrepareStages;

		[SerializeField]
		private float m_SolveConstraints;

		[SerializeField]
		private float m_PrepareConstraints;

		[SerializeField]
		private float m_IntegrateVelocities;

		[SerializeField]
		private float m_WarmStarting;

		[SerializeField]
		private float m_SolveImpulses;

		[SerializeField]
		private float m_IntegrateTransforms;

		[SerializeField]
		private float m_RelaxImpulses;

		[SerializeField]
		private float m_ApplyBounciness;

		[SerializeField]
		private float m_StoreImpulses;

		[SerializeField]
		private float m_SplitIslands;

		[SerializeField]
		private float m_BodyTransforms;

		[SerializeField]
		private float m_FastTriggers;

		[SerializeField]
		private float m_JointEvents;

		[SerializeField]
		private float m_HitEvents;

		[SerializeField]
		private float m_BroadphaseUpdates;

		[SerializeField]
		private float m_SolveContinuous;

		[SerializeField]
		private float m_SleepIslands;

		[SerializeField]
		private float m_UpdateTriggers;

		[SerializeField]
		private float m_WriteTransforms;

		public float simulationStep
		{
			readonly get
			{
				return m_SimulationStep;
			}
			set
			{
				m_SimulationStep = value;
			}
		}

		public float contactPairs
		{
			readonly get
			{
				return m_ContactPairs;
			}
			set
			{
				m_ContactPairs = value;
			}
		}

		public float contactUpdates
		{
			readonly get
			{
				return m_ContactUpdates;
			}
			set
			{
				m_ContactUpdates = value;
			}
		}

		public float solving
		{
			readonly get
			{
				return m_Solving;
			}
			set
			{
				m_Solving = value;
			}
		}

		public float prepareStages
		{
			readonly get
			{
				return m_PrepareStages;
			}
			set
			{
				m_PrepareStages = value;
			}
		}

		public float solveConstraints
		{
			readonly get
			{
				return m_SolveConstraints;
			}
			set
			{
				m_SolveConstraints = value;
			}
		}

		public float prepareConstraints
		{
			readonly get
			{
				return m_PrepareConstraints;
			}
			set
			{
				m_PrepareConstraints = value;
			}
		}

		public float integrateVelocities
		{
			readonly get
			{
				return m_IntegrateVelocities;
			}
			set
			{
				m_IntegrateVelocities = value;
			}
		}

		public float warmStarting
		{
			readonly get
			{
				return m_WarmStarting;
			}
			set
			{
				m_WarmStarting = value;
			}
		}

		public float solveImpulses
		{
			readonly get
			{
				return m_SolveImpulses;
			}
			set
			{
				m_SolveImpulses = value;
			}
		}

		public float integrateTransforms
		{
			readonly get
			{
				return m_IntegrateTransforms;
			}
			set
			{
				m_IntegrateTransforms = value;
			}
		}

		public float relaxImpulses
		{
			readonly get
			{
				return m_RelaxImpulses;
			}
			set
			{
				m_RelaxImpulses = value;
			}
		}

		public float applyBounciness
		{
			readonly get
			{
				return m_ApplyBounciness;
			}
			set
			{
				m_ApplyBounciness = value;
			}
		}

		public float storeImpulses
		{
			readonly get
			{
				return m_StoreImpulses;
			}
			set
			{
				m_StoreImpulses = value;
			}
		}

		public float splitIslands
		{
			readonly get
			{
				return m_SplitIslands;
			}
			set
			{
				m_SplitIslands = value;
			}
		}

		public float bodyTransforms
		{
			readonly get
			{
				return m_BodyTransforms;
			}
			set
			{
				m_BodyTransforms = value;
			}
		}

		public float fastTriggers
		{
			readonly get
			{
				return m_FastTriggers;
			}
			set
			{
				m_FastTriggers = value;
			}
		}

		public float jointEvents
		{
			readonly get
			{
				return m_JointEvents;
			}
			set
			{
				m_JointEvents = value;
			}
		}

		public float hitEvents
		{
			readonly get
			{
				return m_HitEvents;
			}
			set
			{
				m_HitEvents = value;
			}
		}

		public float broadphaseUpdates
		{
			readonly get
			{
				return m_BroadphaseUpdates;
			}
			set
			{
				m_BroadphaseUpdates = value;
			}
		}

		public float solveContinuous
		{
			readonly get
			{
				return m_SolveContinuous;
			}
			set
			{
				m_SolveContinuous = value;
			}
		}

		public float sleepIslands
		{
			readonly get
			{
				return m_SleepIslands;
			}
			set
			{
				m_SleepIslands = value;
			}
		}

		public float updateTriggers
		{
			readonly get
			{
				return m_UpdateTriggers;
			}
			set
			{
				m_UpdateTriggers = value;
			}
		}

		public float writeTransforms
		{
			readonly get
			{
				return m_WriteTransforms;
			}
			set
			{
				m_WriteTransforms = value;
			}
		}

		public static WorldProfile Add(WorldProfile profileA, WorldProfile profileB)
		{
			return new WorldProfile
			{
				simulationStep = profileA.simulationStep + profileB.simulationStep,
				contactPairs = profileA.contactPairs + profileB.contactPairs,
				contactUpdates = profileA.contactUpdates + profileB.contactUpdates,
				solving = profileA.solving + profileB.solving,
				prepareStages = profileA.prepareStages + profileB.prepareStages,
				solveConstraints = profileA.solveConstraints + profileB.solveConstraints,
				prepareConstraints = profileA.prepareConstraints + profileB.prepareConstraints,
				integrateVelocities = profileA.integrateVelocities + profileB.integrateVelocities,
				warmStarting = profileA.warmStarting + profileB.warmStarting,
				solveImpulses = profileA.solveImpulses + profileB.solveImpulses,
				integrateTransforms = profileA.integrateTransforms + profileB.integrateTransforms,
				relaxImpulses = profileA.relaxImpulses + profileB.relaxImpulses,
				applyBounciness = profileA.applyBounciness + profileB.applyBounciness,
				storeImpulses = profileA.storeImpulses + profileB.storeImpulses,
				splitIslands = profileA.splitIslands + profileB.splitIslands,
				bodyTransforms = profileA.bodyTransforms + profileB.bodyTransforms,
				fastTriggers = profileA.fastTriggers + profileB.fastTriggers,
				jointEvents = profileA.jointEvents + profileB.jointEvents,
				hitEvents = profileA.hitEvents + profileB.hitEvents,
				broadphaseUpdates = profileA.broadphaseUpdates + profileB.broadphaseUpdates,
				solveContinuous = profileA.solveContinuous + profileB.solveContinuous,
				sleepIslands = profileA.sleepIslands + profileB.sleepIslands,
				updateTriggers = profileA.updateTriggers + profileB.updateTriggers,
				writeTransforms = profileA.writeTransforms + profileB.writeTransforms
			};
		}

		public static WorldProfile Maximum(WorldProfile profileA, WorldProfile profileB)
		{
			return new WorldProfile
			{
				simulationStep = Mathf.Max(profileA.simulationStep, profileB.simulationStep),
				contactPairs = Mathf.Max(profileA.contactPairs, profileB.contactPairs),
				contactUpdates = Mathf.Max(profileA.contactUpdates, profileB.contactUpdates),
				solving = Mathf.Max(profileA.solving, profileB.solving),
				prepareStages = Mathf.Max(profileA.prepareStages, profileB.prepareStages),
				solveConstraints = Mathf.Max(profileA.solveConstraints, profileB.solveConstraints),
				prepareConstraints = Mathf.Max(profileA.prepareConstraints, profileB.prepareConstraints),
				integrateVelocities = Mathf.Max(profileA.integrateVelocities, profileB.integrateVelocities),
				warmStarting = Mathf.Max(profileA.warmStarting, profileB.warmStarting),
				solveImpulses = Mathf.Max(profileA.solveImpulses, profileB.solveImpulses),
				integrateTransforms = Mathf.Max(profileA.integrateTransforms, profileB.integrateTransforms),
				relaxImpulses = Mathf.Max(profileA.relaxImpulses, profileB.relaxImpulses),
				applyBounciness = Mathf.Max(profileA.applyBounciness, profileB.applyBounciness),
				storeImpulses = Mathf.Max(profileA.storeImpulses, profileB.storeImpulses),
				splitIslands = Mathf.Max(profileA.splitIslands, profileB.splitIslands),
				bodyTransforms = Mathf.Max(profileA.bodyTransforms, profileB.bodyTransforms),
				fastTriggers = Mathf.Max(profileA.fastTriggers, profileB.fastTriggers),
				jointEvents = Mathf.Max(profileA.jointEvents, profileB.jointEvents),
				hitEvents = Mathf.Max(profileA.hitEvents, profileB.hitEvents),
				broadphaseUpdates = Mathf.Max(profileA.broadphaseUpdates, profileB.broadphaseUpdates),
				solveContinuous = Mathf.Max(profileA.solveContinuous, profileB.solveContinuous),
				sleepIslands = Mathf.Max(profileA.sleepIslands, profileB.sleepIslands),
				updateTriggers = Mathf.Max(profileA.updateTriggers, profileB.updateTriggers),
				writeTransforms = Mathf.Max(profileA.writeTransforms, profileB.writeTransforms)
			};
		}
	}

	[Flags]
	public enum DrawOptions
	{
		Off = 0,
		SelectedBodies = 1,
		SelectedShapes = 2,
		SelectedShapeBounds = 4,
		SelectedJoints = 8,
		AllBodies = 0x10,
		AllShapes = 0x20,
		AllShapeBounds = 0x40,
		AllJoints = 0x80,
		AllContactPoints = 0x100,
		AllContactNormal = 0x200,
		AllContactImpulse = 0x400,
		AllContactFriction = 0x800,
		AllCustom = 0x1000,
		AllSolverIslands = 0x2000,
		DefaultAll = 0x10A0,
		DefaultSelected = 0x100A
	}

	[Flags]
	public enum DrawFillOptions
	{
		Interior = 1,
		Outline = 2,
		Orientation = 4,
		All = 7
	}

	internal readonly struct DrawResults
	{
		public readonly struct PolygonGeometryElement
		{
			public readonly PhysicsTransform transform;

			public readonly Vector2 p0;

			public readonly Vector2 p1;

			public readonly Vector2 p2;

			public readonly Vector2 p3;

			public readonly Vector2 p4;

			public readonly Vector2 p5;

			public readonly Vector2 p6;

			public readonly Vector2 p7;

			public readonly int count;

			public readonly float radius;

			public readonly float elementDepth;

			public readonly DrawFillOptions drawFillOptions;

			public readonly Color color;

			public static int Size()
			{
				return 112;
			}
		}

		public readonly struct CircleGeometryElement
		{
			public readonly PhysicsTransform transform;

			public readonly float radius;

			public readonly float elementDepth;

			public readonly DrawFillOptions drawFillOptions;

			public readonly Color color;

			public static int Size()
			{
				return 44;
			}
		}

		public readonly struct CapsuleGeometryElement
		{
			public readonly PhysicsTransform transform;

			public readonly float radius;

			public readonly float length;

			public readonly float elementDepth;

			public readonly DrawFillOptions drawFillOptions;

			public readonly Color color;

			public static int Size()
			{
				return 48;
			}
		}

		public readonly struct LineElement
		{
			public readonly PhysicsTransform transform;

			public readonly float length;

			public readonly float elementDepth;

			public readonly Color color;

			public static int Size()
			{
				return 40;
			}
		}

		public readonly struct PointElement
		{
			public readonly Vector2 position;

			public readonly float radius;

			public readonly float elementDepth;

			public readonly Color color;

			public static int Size()
			{
				return 32;
			}
		}

		internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_PolygonGeometryElements;

		internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_CircleGeometryElements;

		internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_CapsuleGeometryElements;

		internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_LineElements;

		internal readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_PointElements;

		public NativeArray<PolygonGeometryElement> polygonGeometryArray => m_PolygonGeometryElements.ToNativeArray<PolygonGeometryElement>();

		public NativeArray<CircleGeometryElement> circleGeometryArray => m_CircleGeometryElements.ToNativeArray<CircleGeometryElement>();

		public NativeArray<CapsuleGeometryElement> capsuleGeometryArray => m_CapsuleGeometryElements.ToNativeArray<CapsuleGeometryElement>();

		public NativeArray<LineElement> lineArray => m_LineElements.ToNativeArray<LineElement>();

		public NativeArray<PointElement> pointArray => m_PointElements.ToNativeArray<PointElement>();

		public Span<PolygonGeometryElement> polygonGeometrySpan => m_PolygonGeometryElements.ToSpan<PolygonGeometryElement>();

		public Span<CircleGeometryElement> circleGeometrySpan => m_CircleGeometryElements.ToSpan<CircleGeometryElement>();

		public Span<CapsuleGeometryElement> capsuleGeometrySpan => m_CapsuleGeometryElements.ToSpan<CapsuleGeometryElement>();

		public Span<LineElement> lineSpan => m_LineElements.ToSpan<LineElement>();

		public Span<PointElement> pointSpan => m_PointElements.ToSpan<PointElement>();

		public override string ToString()
		{
			return $"PolygonGeometry:{m_PolygonGeometryElements}, CircleGeometry:{m_CircleGeometryElements}, CapsuleGeometry:{m_CapsuleGeometryElements}, Line:{m_LineElements}, Point: {m_PointElements}";
		}
	}

	[Serializable]
	public struct DrawColors
	{
		private struct ConstraintGraphArray
		{
			public Color graphConstraint0;

			public Color graphConstraint1;

			public Color graphConstraint2;

			public Color graphConstraint3;

			public Color graphConstraint4;

			public Color graphConstraint5;

			public Color graphConstraint6;

			public Color graphConstraint7;

			public Color graphConstraint8;

			public Color graphConstraint9;

			public Color graphConstraint10;

			public Color graphConstraint11;

			public Color graphConstraint12;

			public Color graphConstraint13;

			public Color graphConstraint14;

			public Color graphConstraint15;

			public Color graphConstraint16;

			public Color graphConstraint17;

			public Color graphConstraint18;

			public Color graphConstraint19;

			public Color graphConstraint20;

			public Color graphConstraint21;

			public Color graphConstraint22;

			public Color graphConstraint23;

			public unsafe ref Color this[int index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					if (index >= 0 && index < 24)
					{
						fixed (Color* ptr = &graphConstraint0)
						{
							return ref ptr[index];
						}
					}
					throw new IndexOutOfRangeException($"{index} must be in the range [0, {23}]");
				}
			}
		}

		public Color transformAxisX;

		public Color transformAxisY;

		public Color bodyBad;

		public Color bodyDisabled;

		public Color bodyAwake;

		public Color bodyStatic;

		public Color bodyKinematic;

		public Color bodyTimeOfImpactEvent;

		public Color bodyFastCollisions;

		public Color bodyMovingFast;

		public Color bodySpeedCapped;

		public Color shapeTrigger;

		public Color shapeOther;

		public Color shapeBounds;

		public Color contactSpeculative;

		public Color contactAdded;

		public Color contactPersisted;

		public Color contactNormal;

		public Color contactImpulse;

		public Color contactFriction;

		public Color solverIsland;

		private readonly ConstraintGraphArray m_ConstraintGraph;
	}

	internal readonly ushort m_Index1;

	private readonly ushort m_Generation;

	[Obsolete("PhysicsWorld.simulationMode has been deprecated. Please use PhysicsWorld.simulationType instead.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public SimulationMode2D simulationMode
	{
		get
		{
			return (SimulationMode2D)simulationType;
		}
		set
		{
			simulationType = (SimulationType)value;
		}
	}

	public static bool safetyLocksEnabled
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsGlobal_GetSafetyLocksEnabled();
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsGlobal_SetSafetyLocksEnabled(value);
		}
	}

	public static bool bypassLowLevel => PhysicsLowLevelScripting2D.PhysicsGlobal_GetBypassLowLevel();

	public static bool isRenderingAllowed => PhysicsLowLevelScripting2D.PhysicsGlobal_IsRenderingAllowed();

	public static int worldCount => PhysicsLowLevelScripting2D.PhysicsWorld_GetWorldCount();

	public static int concurrentSimulations => PhysicsLowLevelScripting2D.PhysicsGlobal_GetConcurrentSimulations();

	public static float lengthUnitsPerMeter => PhysicsLowLevelScripting2D.PhysicsGlobal_GetLengthUnitsPerMeter();

	public static bool useFullLayers => PhysicsLowLevelScripting2D.PhysicsGlobal_GetUseFullLayers();

	public static float hugeWorldExtent => PhysicsLowLevelScripting2D.PhysicsWorld_GetHugeWorldExtent();

	public static float linearSlop => PhysicsLowLevelScripting2D.PhysicsWorld_GetLinearSlop();

	public static float speculativeContactDistance => PhysicsLowLevelScripting2D.PhysicsWorld_GetSpeculativeContactDistance();

	public static float aabbMargin => PhysicsLowLevelScripting2D.PhysicsWorld_GetAABBMargin();

	public static float bodyMaxRotation => PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyMaxRotation();

	public static float bodyTimeToSleep => PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyTimeToSleep();

	public static PhysicsWorld defaultWorld => PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultWorld();

	public PhysicsWorldDefinition definition
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_ReadDefinition(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_WriteDefinition(this, value, onlyExtendedProperties: false);
		}
	}

	public bool isOwned => PhysicsLowLevelScripting2D.PhysicsWorld_IsOwned(this);

	public PhysicsUserData userData
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetUserData(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetUserData(this, value);
		}
	}

	public bool isValid => PhysicsLowLevelScripting2D.PhysicsWorld_IsValid(this);

	public bool isEmpty => PhysicsLowLevelScripting2D.PhysicsWorld_IsEmpty(this);

	public bool isDefaultWorld => PhysicsLowLevelScripting2D.PhysicsWorld_IsDefaultWorld(this);

	public bool paused
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetPaused(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetPaused(this, value);
		}
	}

	public bool sleepingAllowed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetSleepingAllowed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetSleepingAllowed(this, value);
		}
	}

	public bool continuousAllowed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetContinuousAllowed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetContinuousAllowed(this, value);
		}
	}

	public bool contactFilterCallbacks
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactFilterCallbacks(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetContactFilterCallbacks(this, value);
		}
	}

	public bool preSolveCallbacks
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetPreSolveCallbacks(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetPreSolveCallbacks(this, value);
		}
	}

	public bool autoBodyUpdateCallbacks
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetAutoBodyUpdateCallbacks(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetAutoBodyUpdateCallbacks(this, value);
		}
	}

	public bool autoContactCallbacks
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetAutoContactCallbacks(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetAutoContactCallbacks(this, value);
		}
	}

	public bool autoTriggerCallbacks
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetAutoTriggerCallbacks(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetAutoTriggerCallbacks(this, value);
		}
	}

	public bool autoJointThresholdCallbacks
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetAutoJointThresholdCallbacks(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetAutoJointThresholdCallbacks(this, value);
		}
	}

	public bool warmStartingAllowed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetWarmStartingAllowed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetWarmStartingAllowed(this, value);
		}
	}

	public float bounceThreshold
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetBounceThreshold(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetBounceThreshold(this, value);
		}
	}

	public float contactHitEventThreshold
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactHitEventThreshold(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetContactHitEventThreshold(this, value);
		}
	}

	public float contactFrequency
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactFrequency(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetContactFrequency(this, value);
		}
	}

	public float contactDamping
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactDamping(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetContactDamping(this, value);
		}
	}

	public float contactSpeed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactSpeed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetContactSpeed(this, value);
		}
	}

	public float maximumLinearSpeed
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetMaximumLinearSpeed(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetMaximumLinearSpeed(this, value);
		}
	}

	public Vector2 gravity
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetGravity(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetGravity(this, value);
		}
	}

	public int simulationWorkers
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetSimulationWorkers(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetSimulationWorkers(this, value);
		}
	}

	public SimulationType simulationType
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetSimulationType(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetSimulationType(this, value);
		}
	}

	public int simulationSubSteps
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetSimulationSubSteps(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetSimulationSubSteps(this, value);
		}
	}

	public double lastSimulationTimestamp => PhysicsLowLevelScripting2D.PhysicsWorld_GetLastSimulationTimestamp(this);

	public float lastSimulationDeltaTime => PhysicsLowLevelScripting2D.PhysicsWorld_GetLastSimulationDeltaTime(this);

	public TransformPlane transformPlane
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetTransformPlane(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetTransformPlane(this, value);
		}
	}

	public TransformWriteMode transformWriteMode
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetTransformWriteMode(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetTransformWriteMode(this, value);
		}
	}

	public bool transformTweening
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetTransformTweening(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetTransformTweening(this, value);
		}
	}

	public ReadOnlySpan<PhysicsEvents.BodyUpdateEvent> bodyUpdateEvents => PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyUpdateEvents(this).ToReadOnlySpan<PhysicsEvents.BodyUpdateEvent>();

	public ReadOnlySpan<PhysicsEvents.TriggerBeginEvent> triggerBeginEvents => PhysicsLowLevelScripting2D.PhysicsWorld_GetTriggerBeginEvents(this).ToReadOnlySpan<PhysicsEvents.TriggerBeginEvent>();

	public ReadOnlySpan<PhysicsEvents.TriggerEndEvent> triggerEndEvents => PhysicsLowLevelScripting2D.PhysicsWorld_GetTriggerEndEvents(this).ToReadOnlySpan<PhysicsEvents.TriggerEndEvent>();

	public ReadOnlySpan<PhysicsEvents.ContactBeginEvent> contactBeginEvents => PhysicsLowLevelScripting2D.PhysicsWorld_GetContactBeginEvents(this).ToReadOnlySpan<PhysicsEvents.ContactBeginEvent>();

	public ReadOnlySpan<PhysicsEvents.ContactEndEvent> contactEndEvents => PhysicsLowLevelScripting2D.PhysicsWorld_GetContactEndEvents(this).ToReadOnlySpan<PhysicsEvents.ContactEndEvent>();

	public ReadOnlySpan<PhysicsEvents.ContactHitEvent> contactHitEvents => PhysicsLowLevelScripting2D.PhysicsWorld_GetContactHitEvents(this).ToReadOnlySpan<PhysicsEvents.ContactHitEvent>();

	public ReadOnlySpan<PhysicsEvents.JointThresholdEvent> jointThresholdEvents => PhysicsLowLevelScripting2D.PhysicsWorld_GetJointThresholdEvents(this).ToReadOnlySpan<PhysicsEvents.JointThresholdEvent>();

	public int awakeBodyCount => PhysicsLowLevelScripting2D.PhysicsWorld_GetAwakeBodyCount(this);

	public WorldCounters counters => PhysicsLowLevelScripting2D.PhysicsWorld_GetCounters(this);

	public static WorldCounters globalCounters => PhysicsLowLevelScripting2D.PhysicsWorld_GetGlobalCounters();

	public WorldProfile profile => PhysicsLowLevelScripting2D.PhysicsWorld_GetProfile(this);

	public static WorldProfile globalProfile => PhysicsLowLevelScripting2D.PhysicsWorld_GetGlobalProfile();

	public DrawOptions drawOptions
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawOptions(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawOptions(this, value);
		}
	}

	public DrawFillOptions drawFillOptions
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawFillOptions(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawFillOptions(this, value);
		}
	}

	public float drawThickness
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawThickness(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawThickness(this, value);
		}
	}

	public float drawFillAlpha
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawFillAlpha(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawFillAlpha(this, value);
		}
	}

	public float drawPointScale
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawPointScale(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawPointScale(this, value);
		}
	}

	public float drawNormalScale
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawNormalScale(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawNormalScale(this, value);
		}
	}

	public float drawImpulseScale
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawImpulseScale(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawImpulseScale(this, value);
		}
	}

	public int drawCapacity
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawCapacity(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawCapacity(this, value);
		}
	}

	public DrawColors drawColors
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawColors(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetDrawColors(this, value);
		}
	}

	public float elementDepth
	{
		get
		{
			return PhysicsLowLevelScripting2D.PhysicsWorld_GetElementDepth(this);
		}
		set
		{
			PhysicsLowLevelScripting2D.PhysicsWorld_SetElementDepth(this, value);
		}
	}

	internal DrawResults drawResults => PhysicsLowLevelScripting2D.PhysicsWorld_GetDrawResults(this);

	public override string ToString()
	{
		return isValid ? $"index={m_Index1}, generation={m_Generation}" : "<INVALID>";
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public bool Equals(PhysicsWorld other)
	{
		return m_Index1 == other.m_Index1 && m_Generation == other.m_Generation;
	}

	public static bool operator ==(PhysicsWorld lhs, PhysicsWorld rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PhysicsWorld lhs, PhysicsWorld rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(m_Index1, m_Generation);
	}

	public static NativeArray<PhysicsWorld> GetWorlds(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetWorlds(allocator).ToNativeArray<PhysicsWorld>();
	}

	public NativeArray<PhysicsBody> GetBodies(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodies(this, allocator).ToNativeArray<PhysicsBody>();
	}

	public NativeArray<PhysicsJoint> GetJoints(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetJoints(this, allocator).ToNativeArray<PhysicsJoint>();
	}

	public static PhysicsWorld Create()
	{
		return Create(PhysicsWorldDefinition.defaultDefinition);
	}

	public static PhysicsWorld Create(PhysicsWorldDefinition definition)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_Create(definition);
	}

	public bool Destroy(int ownerKey = 0)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_Destroy(this, ownerKey);
	}

	public int SetOwner(Object owner)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_SetOwner(this, owner);
	}

	public Object GetOwner()
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetOwner(this);
	}

	public void Reset()
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_Reset(this);
	}

	internal void ClearTransformWriteTweens()
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_ClearTransformWriteTweens(this);
	}

	internal void SetTransformWriteTweens(ReadOnlySpan<PhysicsBody.TransformWriteTween> transformWriteTweens)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_SetTransformWriteTweens(this, transformWriteTweens);
	}

	public NativeArray<PhysicsBody.TransformWriteTween> GetTransformWriteTweens()
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetTransformWriteTweens(this).ToNativeArray<PhysicsBody.TransformWriteTween>();
	}

	public void Simulate(float deltaTime)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_Simulate(this, deltaTime, SimulationType.Script);
	}

	public static void Simulate(ReadOnlySpan<PhysicsWorld> worlds, float deltaTime)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_SimulateBatch(worlds, deltaTime, SimulationType.Script);
	}

	public void Explode(ExplosionDefinition definition)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_Explode(this, definition);
	}

	public NativeArray<PhysicsUserData> GetBodyUpdateUserData(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyUpdateUserData(this, allocator).ToNativeArray<PhysicsUserData>();
	}

	public void SendAllCallbacks()
	{
		SendBodyUpdateCallbacks();
		SendTriggerCallbacks();
		SendContactCallbacks();
		SendJointThresholdCallbacks();
	}

	public void SendBodyUpdateCallbacks()
	{
		using PhysicsCallbacks.BodyUpdateCallbackTargets bodyUpdateCallbackTargets = PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyUpdateCallbackTargets(this, Allocator.Temp);
		ReadOnlySpan<PhysicsCallbacks.BodyUpdateCallbackTargets.BodyUpdateTarget> bodyUpdateCallbackTargets2 = bodyUpdateCallbackTargets.bodyUpdateCallbackTargets;
		for (int i = 0; i < bodyUpdateCallbackTargets2.Length; i++)
		{
			PhysicsCallbacks.BodyUpdateCallbackTargets.BodyUpdateTarget bodyUpdateTarget = bodyUpdateCallbackTargets2[i];
			bodyUpdateTarget.bodyTarget?.OnBodyUpdate2D(bodyUpdateTarget.bodyUpdateEvent);
		}
	}

	public void SendContactCallbacks()
	{
		using PhysicsCallbacks.ContactCallbackTargets contactCallbackTargets = PhysicsLowLevelScripting2D.PhysicsWorld_GetContactCallbackTargets(this, Allocator.Temp);
		ReadOnlySpan<PhysicsCallbacks.ContactCallbackTargets.ContactBeginTarget> beginCallbackTargets = contactCallbackTargets.BeginCallbackTargets;
		for (int i = 0; i < beginCallbackTargets.Length; i++)
		{
			PhysicsCallbacks.ContactCallbackTargets.ContactBeginTarget contactBeginTarget = beginCallbackTargets[i];
			contactBeginTarget.shapeTargetA?.OnContactBegin2D(contactBeginTarget.beginEvent);
			contactBeginTarget.shapeTargetB?.OnContactBegin2D(contactBeginTarget.beginEvent);
		}
		ReadOnlySpan<PhysicsCallbacks.ContactCallbackTargets.ContactEndTarget> endCallbackTargets = contactCallbackTargets.EndCallbackTargets;
		for (int j = 0; j < endCallbackTargets.Length; j++)
		{
			PhysicsCallbacks.ContactCallbackTargets.ContactEndTarget contactEndTarget = endCallbackTargets[j];
			contactEndTarget.shapeTargetA?.OnContactEnd2D(contactEndTarget.endEvent);
			contactEndTarget.shapeTargetB?.OnContactEnd2D(contactEndTarget.endEvent);
		}
	}

	public void SendTriggerCallbacks()
	{
		using PhysicsCallbacks.TriggerCallbackTargets triggerCallbackTargets = PhysicsLowLevelScripting2D.PhysicsWorld_GetTriggerCallbackTargets(this, Allocator.Temp);
		ReadOnlySpan<PhysicsCallbacks.TriggerCallbackTargets.TriggerBeginTarget> beginCallbackTargets = triggerCallbackTargets.BeginCallbackTargets;
		for (int i = 0; i < beginCallbackTargets.Length; i++)
		{
			PhysicsCallbacks.TriggerCallbackTargets.TriggerBeginTarget triggerBeginTarget = beginCallbackTargets[i];
			triggerBeginTarget.triggerShapeTarget?.OnTriggerBegin2D(triggerBeginTarget.beginEvent);
			triggerBeginTarget.visitorShapeTarget?.OnTriggerBegin2D(triggerBeginTarget.beginEvent);
		}
		ReadOnlySpan<PhysicsCallbacks.TriggerCallbackTargets.TriggerEndTarget> endCallbackTargets = triggerCallbackTargets.EndCallbackTargets;
		for (int j = 0; j < endCallbackTargets.Length; j++)
		{
			PhysicsCallbacks.TriggerCallbackTargets.TriggerEndTarget triggerEndTarget = endCallbackTargets[j];
			triggerEndTarget.triggerShapeTarget?.OnTriggerEnd2D(triggerEndTarget.endEvent);
			triggerEndTarget.visitorShapeTarget?.OnTriggerEnd2D(triggerEndTarget.endEvent);
		}
	}

	public void SendJointThresholdCallbacks()
	{
		using PhysicsCallbacks.JointThresholdCallbackTargets jointThresholdCallbackTargets = PhysicsLowLevelScripting2D.PhysicsWorld_GetJointThresholdCallbackTargets(this, Allocator.Temp);
		ReadOnlySpan<PhysicsCallbacks.JointThresholdCallbackTargets.JointThresholdTarget> jointThresholdCallbackTargets2 = jointThresholdCallbackTargets.jointThresholdCallbackTargets;
		for (int i = 0; i < jointThresholdCallbackTargets2.Length; i++)
		{
			PhysicsCallbacks.JointThresholdCallbackTargets.JointThresholdTarget jointThresholdTarget = jointThresholdCallbackTargets2[i];
			jointThresholdTarget.jointTarget?.OnJointThreshold2D(jointThresholdTarget.jointThresholdEvent);
		}
	}

	public PhysicsCallbacks.BodyUpdateCallbackTargets GetBodyUpdateCallbackTargets(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetBodyUpdateCallbackTargets(this, allocator);
	}

	public PhysicsCallbacks.TriggerCallbackTargets GetTriggerCallbackTargets(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetTriggerCallbackTargets(this, allocator);
	}

	public PhysicsCallbacks.ContactCallbackTargets GetContactCallbackTargets(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetContactCallbackTargets(this, allocator);
	}

	public PhysicsCallbacks.JointThresholdCallbackTargets GetJointThresholdCallbackTargets(Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_GetJointThresholdCallbackTargets(this, allocator);
	}

	public bool TestOverlapAABB(PhysicsAABB aabb, PhysicsQuery.QueryFilter filter)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_TestOverlapAABB(this, aabb, filter);
	}

	public bool TestOverlapShape(PhysicsShape shape, PhysicsQuery.QueryFilter filter)
	{
		PhysicsShape.ShapeType shapeType = shape.shapeType;
		if (1 == 0)
		{
		}
		bool result = shapeType switch
		{
			PhysicsShape.ShapeType.Circle => TestOverlapGeometry(shape.circleGeometry.Transform(shape.body.transform), filter), 
			PhysicsShape.ShapeType.Capsule => TestOverlapGeometry(shape.capsuleGeometry.Transform(shape.body.transform), filter), 
			PhysicsShape.ShapeType.Polygon => TestOverlapGeometry(shape.polygonGeometry.Transform(shape.body.transform), filter), 
			PhysicsShape.ShapeType.Segment => TestOverlapGeometry(shape.segmentGeometry.Transform(shape.body.transform), filter), 
			PhysicsShape.ShapeType.ChainSegment => TestOverlapGeometry(shape.chainSegmentGeometry.Transform(shape.body.transform), filter), 
			_ => throw new ArgumentException("Invalid shape type used.", "shape"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public bool TestOverlapShapeProxy(PhysicsShape.ShapeProxy shapeProxy, PhysicsQuery.QueryFilter filter)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_TestOverlapShapeProxy(this, shapeProxy, filter);
	}

	public bool TestOverlapPoint(Vector2 point, PhysicsQuery.QueryFilter filter)
	{
		return TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(point), filter);
	}

	public bool TestOverlapGeometry(CircleGeometry geometry, PhysicsQuery.QueryFilter filter)
	{
		return TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
	}

	public bool TestOverlapGeometry(CapsuleGeometry geometry, PhysicsQuery.QueryFilter filter)
	{
		return TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
	}

	public bool TestOverlapGeometry(PolygonGeometry geometry, PhysicsQuery.QueryFilter filter)
	{
		return TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
	}

	public bool TestOverlapGeometry(SegmentGeometry geometry, PhysicsQuery.QueryFilter filter)
	{
		return TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
	}

	public bool TestOverlapGeometry(ChainSegmentGeometry geometry, PhysicsQuery.QueryFilter filter)
	{
		return TestOverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter);
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapAABB(PhysicsAABB aabb, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_OverlapAABB(this, aabb, filter, allocator).ToNativeArray<PhysicsQuery.WorldOverlapResult>();
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapShape(PhysicsShape shape, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		PhysicsShape.ShapeType shapeType = shape.shapeType;
		if (1 == 0)
		{
		}
		NativeArray<PhysicsQuery.WorldOverlapResult> result = shapeType switch
		{
			PhysicsShape.ShapeType.Circle => OverlapGeometry(shape.circleGeometry.Transform(shape.body.transform), filter, allocator), 
			PhysicsShape.ShapeType.Capsule => OverlapGeometry(shape.capsuleGeometry.Transform(shape.body.transform), filter, allocator), 
			PhysicsShape.ShapeType.Polygon => OverlapGeometry(shape.polygonGeometry.Transform(shape.body.transform), filter, allocator), 
			PhysicsShape.ShapeType.Segment => OverlapGeometry(shape.segmentGeometry.Transform(shape.body.transform), filter, allocator), 
			PhysicsShape.ShapeType.ChainSegment => OverlapGeometry(shape.chainSegmentGeometry.segment.Transform(shape.body.transform), filter, allocator), 
			_ => throw new ArgumentException("Invalid shape type used.", "shape"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapShapeProxy(PhysicsShape.ShapeProxy shapeProxy, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_OverlapShapeProxy(this, shapeProxy, filter, allocator).ToNativeArray<PhysicsQuery.WorldOverlapResult>();
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapPoint(Vector2 point, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		return OverlapShapeProxy(new PhysicsShape.ShapeProxy(point), filter, allocator);
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(CircleGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		return OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(CapsuleGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		return OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(PolygonGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		return OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(SegmentGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		return OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
	}

	public NativeArray<PhysicsQuery.WorldOverlapResult> OverlapGeometry(ChainSegmentGeometry geometry, PhysicsQuery.QueryFilter filter, Allocator allocator = Allocator.Temp)
	{
		return OverlapShapeProxy(new PhysicsShape.ShapeProxy(geometry), filter, allocator);
	}

	public NativeArray<PhysicsQuery.WorldCastResult> CastRay(PhysicsQuery.CastRayInput input, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_CastRay(this, input, filter, castMode, allocator).ToNativeArray<PhysicsQuery.WorldCastResult>();
	}

	public NativeArray<PhysicsQuery.WorldCastResult> CastShape(PhysicsShape shape, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
	{
		PhysicsShape.ShapeType shapeType = shape.shapeType;
		if (1 == 0)
		{
		}
		NativeArray<PhysicsQuery.WorldCastResult> result = shapeType switch
		{
			PhysicsShape.ShapeType.Circle => CastGeometry(shape.circleGeometry.Transform(shape.body.transform), translation, filter, castMode, allocator), 
			PhysicsShape.ShapeType.Capsule => CastGeometry(shape.capsuleGeometry.Transform(shape.body.transform), translation, filter, castMode, allocator), 
			PhysicsShape.ShapeType.Polygon => CastGeometry(shape.polygonGeometry.Transform(shape.body.transform), translation, filter, castMode, allocator), 
			_ => throw new ArgumentException("Invalid shape type used for cast.", "shape"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public NativeArray<PhysicsQuery.WorldCastResult> CastShapeProxy(PhysicsShape.ShapeProxy shapeProxy, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_CastShapeProxy(this, shapeProxy, translation, filter, castMode, allocator).ToNativeArray<PhysicsQuery.WorldCastResult>();
	}

	public PhysicsQuery.WorldMoverResult CastMover(PhysicsQuery.WorldMoverInput input)
	{
		return PhysicsLowLevelScripting2D.PhysicsWorld_CastMover(this, input);
	}

	public NativeArray<PhysicsQuery.WorldCastResult> CastGeometry(CircleGeometry geometry, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
	{
		return CastShapeProxy(new PhysicsShape.ShapeProxy(geometry), translation, filter, castMode, allocator);
	}

	public NativeArray<PhysicsQuery.WorldCastResult> CastGeometry(CapsuleGeometry geometry, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
	{
		return CastShapeProxy(new PhysicsShape.ShapeProxy(geometry), translation, filter, castMode, allocator);
	}

	public NativeArray<PhysicsQuery.WorldCastResult> CastGeometry(PolygonGeometry geometry, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode = PhysicsQuery.WorldCastMode.Closest, Allocator allocator = Allocator.Temp)
	{
		return CastShapeProxy(new PhysicsShape.ShapeProxy(geometry), translation, filter, castMode, allocator);
	}

	public PhysicsBody CreateBody()
	{
		return PhysicsBody.Create(this);
	}

	public PhysicsBody CreateBody(PhysicsBodyDefinition definition)
	{
		return PhysicsBody.Create(this, definition);
	}

	public NativeArray<PhysicsBody> CreateBodyBatch(PhysicsBodyDefinition definition, int bodyCount, Allocator allocator = Allocator.Temp)
	{
		return PhysicsBody.CreateBatch(this, definition, bodyCount, allocator);
	}

	public NativeArray<PhysicsBody> CreateBodyBatch(ReadOnlySpan<PhysicsBodyDefinition> definitions, Allocator allocator = Allocator.Temp)
	{
		return PhysicsBody.CreateBatch(this, definitions, allocator);
	}

	public static void DestroyBodyBatch(ReadOnlySpan<PhysicsBody> bodies)
	{
		PhysicsBody.DestroyBatch(bodies);
	}

	public static void DestroyShapeBatch(ReadOnlySpan<PhysicsShape> shapes, bool updateBodyMass)
	{
		PhysicsShape.DestroyBatch(shapes, updateBodyMass);
	}

	public static void DestroyJointBatch(ReadOnlySpan<PhysicsJoint> joints)
	{
		PhysicsJoint.DestroyBatch(joints);
	}

	public PhysicsDistanceJoint CreateJoint(PhysicsDistanceJointDefinition definition)
	{
		return PhysicsDistanceJoint.Create(this, definition);
	}

	public PhysicsRelativeJoint CreateJoint(PhysicsRelativeJointDefinition definition)
	{
		return PhysicsRelativeJoint.Create(this, definition);
	}

	public PhysicsIgnoreJoint CreateJoint(PhysicsIgnoreJointDefinition definition)
	{
		return PhysicsIgnoreJoint.Create(this, definition);
	}

	public PhysicsSliderJoint CreateJoint(PhysicsSliderJointDefinition definition)
	{
		return PhysicsSliderJoint.Create(this, definition);
	}

	public PhysicsHingeJoint CreateJoint(PhysicsHingeJointDefinition definition)
	{
		return PhysicsHingeJoint.Create(this, definition);
	}

	public PhysicsFixedJoint CreateJoint(PhysicsFixedJointDefinition definition)
	{
		return PhysicsFixedJoint.Create(this, definition);
	}

	public PhysicsWheelJoint CreateJoint(PhysicsWheelJointDefinition definition)
	{
		return PhysicsWheelJoint.Create(this, definition);
	}

	public void SetElementDepth3D(Vector3 position)
	{
		elementDepth = PhysicsMath.GetTranslationIgnoredAxis(position, transformPlane);
	}

	public void ClearDraw()
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_ClearDraw(this, clearWorldDraw: false, clearCustomDraw: true, clearTimedDraw: true);
	}

	public void DrawGeometry(CircleGeometry geometry, PhysicsTransform transform, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.All)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawCircleGeometry(this, geometry, transform, color, lifetime, drawFillOptions);
	}

	public void DrawGeometry(ReadOnlySpan<CircleGeometry> geometry, PhysicsTransform transform, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.All)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawCircleGeometrySpan(this, geometry, transform, color, lifetime, drawFillOptions);
	}

	public void DrawGeometry(CapsuleGeometry geometry, PhysicsTransform transform, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.All)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawCapsuleGeometry(this, geometry, transform, color, lifetime, drawFillOptions);
	}

	public void DrawGeometry(ReadOnlySpan<CapsuleGeometry> geometry, PhysicsTransform transform, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.All)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawCapsuleGeometrySpan(this, geometry, transform, color, lifetime, drawFillOptions);
	}

	public void DrawGeometry(PolygonGeometry geometry, PhysicsTransform transform, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.All)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawPolygonGeometry(this, geometry, transform, color, lifetime, drawFillOptions);
	}

	public void DrawGeometry(ReadOnlySpan<PolygonGeometry> geometry, PhysicsTransform transform, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.All)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawPolygonGeometrySpan(this, geometry, transform, color, lifetime, drawFillOptions);
	}

	public void DrawGeometry(SegmentGeometry geometry, PhysicsTransform transform, Color color, float lifetime = 0f)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawSegmentGeometry(this, geometry, transform, color, lifetime);
	}

	public void DrawGeometry(ReadOnlySpan<SegmentGeometry> geometry, PhysicsTransform transform, Color color, float lifetime = 0f)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawSegmentGeometrySpan(this, geometry, transform, color, lifetime);
	}

	public void DrawBox(PhysicsTransform transform, Vector2 size, float radius, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.Outline)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawBox(this, transform, size, radius, color, lifetime, drawFillOptions);
	}

	public void DrawCircle(Vector2 center, float radius, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.Outline)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawCircle(this, center, radius, color, lifetime, drawFillOptions);
	}

	public void DrawCapsule(PhysicsTransform transform, Vector2 center1, Vector2 center2, float radius, Color color, float lifetime = 0f, DrawFillOptions drawFillOptions = DrawFillOptions.Outline)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawCapsule(this, transform, center1, center2, radius, color, lifetime, drawFillOptions);
	}

	public void DrawPoint(Vector2 position, float radius, Color color, float lifetime = 0f)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawPoint(this, position, radius, color, lifetime);
	}

	public void DrawLine(Vector2 point0, Vector2 point1, Color color, float lifetime = 0f)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawLine(this, point0, point1, color, lifetime);
	}

	public void DrawLineStrip(PhysicsTransform transform, ReadOnlySpan<Vector2> vertices, bool loop, Color color, float lifetime = 0f)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawLineStrip(this, transform, vertices, loop, color, lifetime);
	}

	public void DrawTransformAxis(PhysicsTransform transform, float scale, float lifetime = 0f)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawTransformAxis(this, transform, scale, lifetime);
	}

	internal void Draw(PhysicsAABB drawAABB)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_Draw(this, drawAABB);
	}

	internal static void DrawAllWorlds(PhysicsAABB drawAABB)
	{
		PhysicsLowLevelScripting2D.PhysicsWorld_DrawAllWorlds(drawAABB);
	}
}
