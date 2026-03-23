using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
[NativeHeader("Physics2DScriptingClasses.h")]
[NativeHeader("Modules/Physics2D/PhysicsManager2D.h")]
[NativeHeader("Physics2DScriptingClasses.h")]
public class Physics2D
{
	[Flags]
	internal enum GizmoOptions
	{
		AllColliders = 1,
		CollidersOutlined = 2,
		CollidersFilled = 4,
		CollidersSleeping = 8,
		ColliderContacts = 0x10,
		ColliderBounds = 0x20
	}

	public const int IgnoreRaycastLayer = 4;

	public const int DefaultRaycastLayers = -5;

	public const int AllLayers = -1;

	public const int MaxPolygonShapeVertices = 8;

	private static List<Rigidbody2D> m_LastDisabledRigidbody2D = new List<Rigidbody2D>();

	public static PhysicsScene2D defaultPhysicsScene => default(PhysicsScene2D);

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern int velocityIterations
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern int positionIterations
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static Vector2 gravity
	{
		get
		{
			get_gravity_Injected(out var ret);
			return ret;
		}
		set
		{
			set_gravity_Injected(ref value);
		}
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool queriesHitTriggers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool queriesStartInColliders
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool callbacksOnDisable
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool reuseCollisionCallbacks
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool autoSyncTransforms
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern SimulationMode2D simulationMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static LayerMask simulationLayers
	{
		get
		{
			get_simulationLayers_Injected(out var ret);
			return ret;
		}
		set
		{
			set_simulationLayers_Injected(ref value);
		}
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool useSubStepping
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern bool useSubStepContacts
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float minSubStepFPS
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern int maxSubStepCount
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static PhysicsJobOptions2D jobOptions
	{
		get
		{
			get_jobOptions_Injected(out var ret);
			return ret;
		}
		set
		{
			set_jobOptions_Injected(ref value);
		}
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float bounceThreshold
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float contactThreshold
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float maxLinearCorrection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float maxAngularCorrection
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float maxTranslationSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float maxRotationSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float defaultContactOffset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float baumgarteScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float baumgarteTOIScale
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float timeToSleep
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float linearSleepTolerance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysics2DSettings()")]
	public static extern float angularSleepTolerance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[Obsolete("Physics2D.raycastsHitTriggers is obsolete. Use Physics2D.queriesHitTriggers instead. (UnityUpgradable) -> queriesHitTriggers", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool raycastsHitTriggers
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Physics2D.raycastsStartInColliders is obsolete. Use Physics2D.queriesStartInColliders instead. (UnityUpgradable) -> queriesStartInColliders", true)]
	public static bool raycastsStartInColliders
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.deleteStopsCallbacks is obsolete.(UnityUpgradable) -> changeStopsCallbacks", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool deleteStopsCallbacks
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.changeStopsCallbacks is obsolete and will always return false.", true)]
	public static bool changeStopsCallbacks
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.minPenetrationForPenalty is obsolete. Use Physics2D.defaultContactOffset instead. (UnityUpgradable) -> defaultContactOffset", true)]
	public static float minPenetrationForPenalty
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.velocityThreshold is obsolete. Use Physics2D.bounceThreshold instead. (UnityUpgradable) -> bounceThreshold", true)]
	[ExcludeFromDocs]
	public static float velocityThreshold
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Physics2D.autoSimulation is obsolete. Use Physics2D.simulationMode instead.", true)]
	public static bool autoSimulation
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[ExcludeFromDocs]
	[Obsolete("Physics2D.colliderAwakeColor is obsolete. This options has been moved to 2D Preferences.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Color colliderAwakeColor
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.colliderAsleepColor is obsolete. This options has been moved to 2D Preferences.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static Color colliderAsleepColor
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Physics2D.colliderContactColor is obsolete. This options has been moved to 2D Preferences.", true)]
	public static Color colliderContactColor
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.colliderAABBColor is obsolete. All Physics 2D colors moved to Preferences. This is now known as 'Collider Bounds Color'.", true)]
	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Color colliderAABBColor
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.contactArrowScale is obsolete. This options has been moved to 2D Preferences.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static float contactArrowScale
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Physics2D.alwaysShowColliders is obsolete. It is no longer available in the Editor or Builds.", true)]
	[ExcludeFromDocs]
	public static bool alwaysShowColliders
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Physics2D.showCollidersFilled is obsolete. It is no longer available in the Editor or Builds.", true)]
	public static bool showCollidersFilled
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Physics2D.showColliderSleep is obsolete. It is no longer available in the Editor or Builds.", true)]
	public static bool showColliderSleep
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.showColliderContacts is obsolete. It is no longer available in the Editor or Builds.", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static bool showColliderContacts
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[Obsolete("Physics2D.showColliderAABB is obsolete. It is no longer available in the Editor or Builds.", true)]
	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool showColliderAABB
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	[ExcludeFromDocs]
	public static bool Simulate(float deltaTime)
	{
		return Simulate_Internal(defaultPhysicsScene, deltaTime, -1);
	}

	public static bool Simulate(float deltaTime, [UnityEngine.Internal.DefaultValue("Physics2D.AllLayers")] int simulationLayers = -1)
	{
		return Simulate_Internal(defaultPhysicsScene, deltaTime, simulationLayers);
	}

	[NativeMethod("Simulate_Binding")]
	internal static bool Simulate_Internal(PhysicsScene2D physicsScene, float deltaTime, int simulationLayers)
	{
		return Simulate_Internal_Injected(ref physicsScene, deltaTime, simulationLayers);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void SyncTransforms();

	[ExcludeFromDocs]
	public static void IgnoreCollision(Collider2D collider1, Collider2D collider2)
	{
		IgnoreCollision(collider1, collider2, ignore: true);
	}

	[StaticAccessor("PhysicsScene2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("IgnoreCollision_Binding")]
	public static void IgnoreCollision([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, [UnityEngine.Internal.DefaultValue("true")] bool ignore)
	{
		if ((object)collider1 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		if ((object)collider2 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider1);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(collider2);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		IgnoreCollision_Injected(intPtr, intPtr2, ignore);
	}

	[StaticAccessor("PhysicsScene2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetIgnoreCollision_Binding")]
	public static bool GetIgnoreCollision([NotNull] Collider2D collider1, [NotNull] Collider2D collider2)
	{
		if ((object)collider1 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		if ((object)collider2 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider1);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(collider2);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		return GetIgnoreCollision_Injected(intPtr, intPtr2);
	}

	[ExcludeFromDocs]
	public static void IgnoreLayerCollision(int layer1, int layer2)
	{
		IgnoreLayerCollision(layer1, layer2, ignore: true);
	}

	public static void IgnoreLayerCollision(int layer1, int layer2, bool ignore)
	{
		if (layer1 < 0 || layer1 > 31)
		{
			throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		if (layer2 < 0 || layer2 > 31)
		{
			throw new ArgumentOutOfRangeException("layer2 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		IgnoreLayerCollision_Internal(layer1, layer2, ignore);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysics2DSettings()")]
	[NativeMethod("IgnoreLayerCollision")]
	private static extern void IgnoreLayerCollision_Internal(int layer1, int layer2, bool ignore);

	public static bool GetIgnoreLayerCollision(int layer1, int layer2)
	{
		if (layer1 < 0 || layer1 > 31)
		{
			throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		if (layer2 < 0 || layer2 > 31)
		{
			throw new ArgumentOutOfRangeException("layer2 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		return GetIgnoreLayerCollision_Internal(layer1, layer2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysics2DSettings()")]
	[NativeMethod("GetIgnoreLayerCollision")]
	private static extern bool GetIgnoreLayerCollision_Internal(int layer1, int layer2);

	public static void SetLayerCollisionMask(int layer, int layerMask)
	{
		if (layer < 0 || layer > 31)
		{
			throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		SetLayerCollisionMask_Internal(layer, layerMask);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("SetLayerCollisionMask")]
	[StaticAccessor("GetPhysics2DSettings()")]
	private static extern void SetLayerCollisionMask_Internal(int layer, int layerMask);

	public static int GetLayerCollisionMask(int layer)
	{
		if (layer < 0 || layer > 31)
		{
			throw new ArgumentOutOfRangeException("layer1 is out of range. Layer numbers must be in the range 0 to 31.");
		}
		return GetLayerCollisionMask_Internal(layer);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysics2DSettings()")]
	[NativeMethod("GetLayerCollisionMask")]
	private static extern int GetLayerCollisionMask_Internal(int layer);

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	public static bool IsTouching([NotNull] Collider2D collider1, [NotNull] Collider2D collider2)
	{
		if ((object)collider1 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		if ((object)collider2 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider1);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(collider2);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		return IsTouching_Injected(intPtr, intPtr2);
	}

	public static bool IsTouching(Collider2D collider1, Collider2D collider2, ContactFilter2D contactFilter)
	{
		return IsTouching_TwoCollidersWithFilter(collider1, collider2, contactFilter);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("IsTouching")]
	private static bool IsTouching_TwoCollidersWithFilter([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, ContactFilter2D contactFilter)
	{
		if ((object)collider1 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		if ((object)collider2 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider1);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(collider2);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		return IsTouching_TwoCollidersWithFilter_Injected(intPtr, intPtr2, ref contactFilter);
	}

	public static bool IsTouching(Collider2D collider, ContactFilter2D contactFilter)
	{
		return IsTouching_SingleColliderWithFilter(collider, contactFilter);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("IsTouching")]
	private static bool IsTouching_SingleColliderWithFilter([NotNull] Collider2D collider, ContactFilter2D contactFilter)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return IsTouching_SingleColliderWithFilter_Injected(intPtr, ref contactFilter);
	}

	[ExcludeFromDocs]
	public static bool IsTouchingLayers(Collider2D collider)
	{
		return IsTouchingLayers(collider, -1);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	public static bool IsTouchingLayers([NotNull] Collider2D collider, [UnityEngine.Internal.DefaultValue("Physics2D.AllLayers")] int layerMask)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return IsTouchingLayers_Injected(intPtr, layerMask);
	}

	public static ColliderDistance2D Distance(Collider2D colliderA, Collider2D colliderB)
	{
		if (colliderA == colliderB)
		{
			throw new ArgumentException("Cannot calculate the distance between the same collider.");
		}
		return Distance_Internal(colliderA, colliderB);
	}

	public static ColliderDistance2D Distance(Collider2D colliderA, Vector2 positionA, float angleA, Collider2D colliderB, Vector2 positionB, float angleB)
	{
		if (colliderA == colliderB)
		{
			throw new ArgumentException("Cannot calculate the distance between the same collider.");
		}
		if (!colliderA.attachedRigidbody || !colliderB.attachedRigidbody)
		{
			throw new InvalidOperationException("Cannot perform a Collider Distance at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
		}
		return DistanceFrom_Internal(colliderA, positionA, angleA, colliderB, positionB, angleB);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("Distance")]
	private static ColliderDistance2D Distance_Internal([NotNull] Collider2D colliderA, [NotNull] Collider2D colliderB)
	{
		if ((object)colliderA == null)
		{
			ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
		}
		if ((object)colliderB == null)
		{
			ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(colliderA);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(colliderB);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
		}
		Distance_Internal_Injected(intPtr, intPtr2, out var ret);
		return ret;
	}

	[NativeMethod("DistanceFrom")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static ColliderDistance2D DistanceFrom_Internal([NotNull] Collider2D colliderA, Vector2 positionA, float angleA, [NotNull] Collider2D colliderB, Vector2 positionB, float angleB)
	{
		if ((object)colliderA == null)
		{
			ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
		}
		if ((object)colliderB == null)
		{
			ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(colliderA);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(colliderA, "colliderA");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(colliderB);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(colliderB, "colliderB");
		}
		DistanceFrom_Internal_Injected(intPtr, ref positionA, angleA, intPtr2, ref positionB, angleB, out var ret);
		return ret;
	}

	public static Vector2 ClosestPoint(Vector2 position, Collider2D collider)
	{
		if (collider == null)
		{
			throw new ArgumentNullException("Collider cannot be NULL.");
		}
		return ClosestPoint_Collider(position, collider);
	}

	public static Vector2 ClosestPoint(Vector2 position, Rigidbody2D rigidbody)
	{
		if (rigidbody == null)
		{
			throw new ArgumentNullException("Rigidbody cannot be NULL.");
		}
		return ClosestPoint_Rigidbody(position, rigidbody);
	}

	[NativeMethod("ClosestPoint")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static Vector2 ClosestPoint_Collider(Vector2 position, [NotNull] Collider2D collider)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		ClosestPoint_Collider_Injected(ref position, intPtr, out var ret);
		return ret;
	}

	[NativeMethod("ClosestPoint")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static Vector2 ClosestPoint_Rigidbody(Vector2 position, [NotNull] Rigidbody2D rigidbody)
	{
		if ((object)rigidbody == null)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(rigidbody);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		ClosestPoint_Rigidbody_Injected(ref position, intPtr, out var ret);
		return ret;
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Linecast(Vector2 start, Vector2 end)
	{
		return defaultPhysicsScene.Linecast(start, end);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.Linecast(start, end, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.Linecast(start, end, contactFilter);
	}

	public static RaycastHit2D Linecast(Vector2 start, Vector2 end, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.Linecast(start, end, contactFilter);
	}

	public static int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.Linecast(start, end, contactFilter, results);
	}

	public static int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, List<RaycastHit2D> results)
	{
		return defaultPhysicsScene.Linecast(start, end, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return LinecastAll_Internal(defaultPhysicsScene, start, end, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return LinecastAll_Internal(defaultPhysicsScene, start, end, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return LinecastAll_Internal(defaultPhysicsScene, start, end, contactFilter);
	}

	public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return LinecastAll_Internal(defaultPhysicsScene, start, end, contactFilter);
	}

	[NativeMethod("LinecastAll_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static RaycastHit2D[] LinecastAll_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit2D[] result;
		try
		{
			LinecastAll_Internal_Injected(ref physicsScene, ref start, ref end, ref contactFilter, out ret);
		}
		finally
		{
			RaycastHit2D[] array = default(RaycastHit2D[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction)
	{
		return defaultPhysicsScene.Raycast(origin, direction, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
	{
		return defaultPhysicsScene.Raycast(origin, direction, distance);
	}

	[ExcludeFromDocs]
	[RequiredByNativeCode]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter);
	}

	public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.Raycast(origin, direction, float.PositiveInfinity, contactFilter, results);
	}

	public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance)
	{
		return defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter, results);
	}

	public static int Raycast(Vector2 origin, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastAll_Internal(defaultPhysicsScene, origin, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastAll_Internal(defaultPhysicsScene, origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastAll_Internal(defaultPhysicsScene, origin, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return RaycastAll_Internal(defaultPhysicsScene, origin, direction, distance, contactFilter);
	}

	public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return RaycastAll_Internal(defaultPhysicsScene, origin, direction, distance, contactFilter);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("RaycastAll_Binding")]
	private static RaycastHit2D[] RaycastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit2D[] result;
		try
		{
			RaycastAll_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, ref contactFilter, out ret);
		}
		finally
		{
			RaycastHit2D[] array = default(RaycastHit2D[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction)
	{
		return defaultPhysicsScene.CircleCast(origin, radius, direction, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance)
	{
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter);
	}

	public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.CircleCast(origin, radius, direction, float.PositiveInfinity, contactFilter, results);
	}

	public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance)
	{
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter, results);
	}

	public static int CircleCast(Vector2 origin, float radius, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastAll_Internal(defaultPhysicsScene, origin, radius, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastAll_Internal(defaultPhysicsScene, origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastAll_Internal(defaultPhysicsScene, origin, radius, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return CircleCastAll_Internal(defaultPhysicsScene, origin, radius, direction, distance, contactFilter);
	}

	public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return CircleCastAll_Internal(defaultPhysicsScene, origin, radius, direction, distance, contactFilter);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("CircleCastAll_Binding")]
	private static RaycastHit2D[] CircleCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit2D[] result;
		try
		{
			CircleCastAll_Internal_Injected(ref physicsScene, ref origin, radius, ref direction, distance, ref contactFilter, out ret);
		}
		finally
		{
			RaycastHit2D[] array = default(RaycastHit2D[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[ExcludeFromDocs]
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction)
	{
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
	{
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter);
	}

	public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("Physics2D.AllLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, float.PositiveInfinity, contactFilter, results);
	}

	public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance)
	{
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter, results);
	}

	public static int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastAll_Internal(defaultPhysicsScene, origin, size, angle, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastAll_Internal(defaultPhysicsScene, origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastAll_Internal(defaultPhysicsScene, origin, size, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return BoxCastAll_Internal(defaultPhysicsScene, origin, size, angle, direction, distance, contactFilter);
	}

	public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return BoxCastAll_Internal(defaultPhysicsScene, origin, size, angle, direction, distance, contactFilter);
	}

	[NativeMethod("BoxCastAll_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static RaycastHit2D[] BoxCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit2D[] result;
		try
		{
			BoxCastAll_Internal_Injected(ref physicsScene, ref origin, ref size, angle, ref direction, distance, ref contactFilter, out ret);
		}
		finally
		{
			RaycastHit2D[] array = default(RaycastHit2D[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
	{
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
	{
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	public static RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, contactFilter, results);
	}

	public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance)
	{
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	public static int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastAll_Internal(defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastAll_Internal(defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[NativeMethod("CapsuleCastAll_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static RaycastHit2D[] CapsuleCastAll_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit2D[] result;
		try
		{
			CapsuleCastAll_Internal_Injected(ref physicsScene, ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, out ret);
		}
		finally
		{
			RaycastHit2D[] array = default(RaycastHit2D[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastAll_Internal(defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return CapsuleCastAll_Internal(defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	public static RaycastHit2D[] CapsuleCastAll(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return CapsuleCastAll_Internal(defaultPhysicsScene, origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D GetRayIntersection(Ray ray)
	{
		return defaultPhysicsScene.GetRayIntersection(ray, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D GetRayIntersection(Ray ray, float distance)
	{
		return defaultPhysicsScene.GetRayIntersection(ray, distance);
	}

	public static RaycastHit2D GetRayIntersection(Ray ray, float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask = -5)
	{
		return defaultPhysicsScene.GetRayIntersection(ray, distance, layerMask);
	}

	public static int GetRayIntersection(Ray ray, float distance, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		return defaultPhysicsScene.GetRayIntersection(ray, distance, results, layerMask);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray)
	{
		return GetRayIntersectionAll_Internal(defaultPhysicsScene, ray.origin, ray.direction, float.PositiveInfinity, -5);
	}

	[ExcludeFromDocs]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance)
	{
		return GetRayIntersectionAll_Internal(defaultPhysicsScene, ray.origin, ray.direction, distance, -5);
	}

	[RequiredByNativeCode]
	public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask = -5)
	{
		return GetRayIntersectionAll_Internal(defaultPhysicsScene, ray.origin, ray.direction, distance, layerMask);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetRayIntersectionAll_Binding")]
	private static RaycastHit2D[] GetRayIntersectionAll_Internal(PhysicsScene2D physicsScene, Vector3 origin, Vector3 direction, float distance, int layerMask)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit2D[] result;
		try
		{
			GetRayIntersectionAll_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, layerMask, out ret);
		}
		finally
		{
			RaycastHit2D[] array = default(RaycastHit2D[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[RequiredByNativeCode]
	[ExcludeFromDocs]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask = -5)
	{
		return defaultPhysicsScene.GetRayIntersection(ray, distance, results, layerMask);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapPoint(Vector2 point)
	{
		return defaultPhysicsScene.OverlapPoint(point);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapPoint(Vector2 point, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapPoint(point, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapPoint(Vector2 point, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapPoint(point, contactFilter);
	}

	public static Collider2D OverlapPoint(Vector2 point, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapPoint(point, contactFilter);
	}

	public static int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, [Unmarshalled] Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapPoint(point, contactFilter, results);
	}

	public static int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return defaultPhysicsScene.OverlapPoint(point, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapPointAll(Vector2 point)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPointAll_Internal(defaultPhysicsScene, point, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPointAll_Internal(defaultPhysicsScene, point, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapPointAll_Internal(defaultPhysicsScene, point, contactFilter);
	}

	public static Collider2D[] OverlapPointAll(Vector2 point, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapPointAll_Internal(defaultPhysicsScene, point, contactFilter);
	}

	[NativeMethod("OverlapPointAll_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static Collider2D[] OverlapPointAll_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter)
	{
		return OverlapPointAll_Internal_Injected(ref physicsScene, ref point, ref contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCircle(Vector2 point, float radius)
	{
		return defaultPhysicsScene.OverlapCircle(point, radius);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapCircle(point, radius, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapCircle(point, radius, contactFilter);
	}

	public static Collider2D OverlapCircle(Vector2 point, float radius, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapCircle(point, radius, contactFilter);
	}

	public static int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapCircle(point, radius, contactFilter, results);
	}

	public static int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return defaultPhysicsScene.OverlapCircle(point, radius, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCircleAll(Vector2 point, float radius)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircleAll_Internal(defaultPhysicsScene, point, radius, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircleAll_Internal(defaultPhysicsScene, point, radius, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapCircleAll_Internal(defaultPhysicsScene, point, radius, contactFilter);
	}

	public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapCircleAll_Internal(defaultPhysicsScene, point, radius, contactFilter);
	}

	[NativeMethod("OverlapCircleAll_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static Collider2D[] OverlapCircleAll_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter)
	{
		return OverlapCircleAll_Internal_Injected(ref physicsScene, ref point, radius, ref contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle)
	{
		return defaultPhysicsScene.OverlapBox(point, size, angle);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter);
	}

	public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter);
	}

	public static int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter, results);
	}

	public static int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBoxAll_Internal(defaultPhysicsScene, point, size, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBoxAll_Internal(defaultPhysicsScene, point, size, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapBoxAll_Internal(defaultPhysicsScene, point, size, angle, contactFilter);
	}

	public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapBoxAll_Internal(defaultPhysicsScene, point, size, angle, contactFilter);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapBoxAll_Binding")]
	private static Collider2D[] OverlapBoxAll_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
	{
		return OverlapBoxAll_Internal_Injected(ref physicsScene, ref point, ref size, angle, ref contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB)
	{
		return defaultPhysicsScene.OverlapArea(pointA, pointB);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter);
	}

	public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter);
	}

	public static int OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter, results);
	}

	public static int OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB)
	{
		return OverlapAreaAllToBox_Internal(pointA, pointB, -5, float.NegativeInfinity, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask)
	{
		return OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, float.NegativeInfinity, float.PositiveInfinity);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
	{
		return OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, minDepth, float.PositiveInfinity);
	}

	public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		return OverlapAreaAllToBox_Internal(pointA, pointB, layerMask, minDepth, maxDepth);
	}

	private static Collider2D[] OverlapAreaAllToBox_Internal(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth, float maxDepth)
	{
		Vector2 point = (pointA + pointB) * 0.5f;
		Vector2 size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
		return OverlapBoxAll(point, size, 0f, layerMask, minDepth, maxDepth);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
	{
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter);
	}

	public static Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter);
	}

	public static int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter, results);
	}

	public static int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-5, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsuleAll_Internal(defaultPhysicsScene, point, size, direction, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsuleAll_Internal(defaultPhysicsScene, point, size, direction, angle, contactFilter);
	}

	[ExcludeFromDocs]
	public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return OverlapCapsuleAll_Internal(defaultPhysicsScene, point, size, direction, angle, contactFilter);
	}

	public static Collider2D[] OverlapCapsuleAll(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return OverlapCapsuleAll_Internal(defaultPhysicsScene, point, size, direction, angle, contactFilter);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapCapsuleAll_Binding")]
	private static Collider2D[] OverlapCapsuleAll_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
	{
		return OverlapCapsuleAll_Internal_Injected(ref physicsScene, ref point, ref size, direction, angle, ref contactFilter);
	}

	public static int OverlapCollider(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return PhysicsScene2D.OverlapCollider(collider, contactFilter, results);
	}

	public static int OverlapCollider(Collider2D collider, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return PhysicsScene2D.OverlapCollider(collider, contactFilter, results);
	}

	public static int OverlapCollider(Collider2D collider, List<Collider2D> results)
	{
		return PhysicsScene2D.OverlapCollider(collider, results);
	}

	public static int GetContacts(Collider2D collider1, Collider2D collider2, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
	{
		return GetColliderColliderContactsArray(collider1, collider2, contactFilter, contacts);
	}

	public static int GetContacts(Collider2D collider, ContactPoint2D[] contacts)
	{
		return GetColliderContactsArray(collider, ContactFilter2D.noFilter, contacts);
	}

	public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
	{
		return GetColliderContactsArray(collider, contactFilter, contacts);
	}

	public static int GetContacts(Collider2D collider, Collider2D[] colliders)
	{
		return GetColliderContactsCollidersOnlyArray(collider, ContactFilter2D.noFilter, colliders);
	}

	public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] colliders)
	{
		return GetColliderContactsCollidersOnlyArray(collider, contactFilter, colliders);
	}

	public static int GetContacts(Rigidbody2D rigidbody, ContactPoint2D[] contacts)
	{
		return GetRigidbodyContactsArray(rigidbody, ContactFilter2D.noFilter, contacts);
	}

	public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, ContactPoint2D[] contacts)
	{
		return GetRigidbodyContactsArray(rigidbody, contactFilter, contacts);
	}

	public static int GetContacts(Rigidbody2D rigidbody, Collider2D[] colliders)
	{
		return GetRigidbodyContactsCollidersOnlyArray(rigidbody, ContactFilter2D.noFilter, colliders);
	}

	public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, Collider2D[] colliders)
	{
		return GetRigidbodyContactsCollidersOnlyArray(rigidbody, contactFilter, colliders);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetColliderContactsArray_Binding")]
	private unsafe static int GetColliderContactsArray([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] ContactPoint2D[] results)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		Span<ContactPoint2D> span = new Span<ContactPoint2D>(results);
		int colliderContactsArray_Injected;
		fixed (ContactPoint2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			colliderContactsArray_Injected = GetColliderContactsArray_Injected(intPtr, ref contactFilter, ref results2);
		}
		return colliderContactsArray_Injected;
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetColliderColliderContactsArray_Binding")]
	private unsafe static int GetColliderColliderContactsArray([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, ContactFilter2D contactFilter, [NotNull] ContactPoint2D[] results)
	{
		if ((object)collider1 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		if ((object)collider2 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider1);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(collider2);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		Span<ContactPoint2D> span = new Span<ContactPoint2D>(results);
		int colliderColliderContactsArray_Injected;
		fixed (ContactPoint2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			colliderColliderContactsArray_Injected = GetColliderColliderContactsArray_Injected(intPtr, intPtr2, ref contactFilter, ref results2);
		}
		return colliderColliderContactsArray_Injected;
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetRigidbodyContactsArray_Binding")]
	private unsafe static int GetRigidbodyContactsArray([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [NotNull] ContactPoint2D[] results)
	{
		if ((object)rigidbody == null)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(rigidbody);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		Span<ContactPoint2D> span = new Span<ContactPoint2D>(results);
		int rigidbodyContactsArray_Injected;
		fixed (ContactPoint2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			rigidbodyContactsArray_Injected = GetRigidbodyContactsArray_Injected(intPtr, ref contactFilter, ref results2);
		}
		return rigidbodyContactsArray_Injected;
	}

	[NativeMethod("GetColliderContactsCollidersOnlyArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int GetColliderContactsCollidersOnlyArray([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull][Unmarshalled] Collider2D[] results)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return GetColliderContactsCollidersOnlyArray_Injected(intPtr, ref contactFilter, results);
	}

	[NativeMethod("GetRigidbodyContactsCollidersOnlyArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int GetRigidbodyContactsCollidersOnlyArray([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [NotNull][Unmarshalled] Collider2D[] results)
	{
		if ((object)rigidbody == null)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(rigidbody);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		return GetRigidbodyContactsCollidersOnlyArray_Injected(intPtr, ref contactFilter, results);
	}

	public static int GetContacts(Collider2D collider1, Collider2D collider2, ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
	{
		return GetColliderColliderContactsList(collider1, collider2, contactFilter, contacts);
	}

	public static int GetContacts(Collider2D collider, List<ContactPoint2D> contacts)
	{
		return GetColliderContactsList(collider, ContactFilter2D.noFilter, contacts);
	}

	public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
	{
		return GetColliderContactsList(collider, contactFilter, contacts);
	}

	public static int GetContacts(Collider2D collider, List<Collider2D> colliders)
	{
		return GetColliderContactsCollidersOnlyList(collider, ContactFilter2D.noFilter, colliders);
	}

	public static int GetContacts(Collider2D collider, ContactFilter2D contactFilter, List<Collider2D> colliders)
	{
		return GetColliderContactsCollidersOnlyList(collider, contactFilter, colliders);
	}

	public static int GetContacts(Rigidbody2D rigidbody, List<ContactPoint2D> contacts)
	{
		return GetRigidbodyContactsList(rigidbody, ContactFilter2D.noFilter, contacts);
	}

	public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
	{
		return GetRigidbodyContactsList(rigidbody, contactFilter, contacts);
	}

	public static int GetContacts(Rigidbody2D rigidbody, List<Collider2D> colliders)
	{
		return GetRigidbodyContactsCollidersOnlyList(rigidbody, ContactFilter2D.noFilter, colliders);
	}

	public static int GetContacts(Rigidbody2D rigidbody, ContactFilter2D contactFilter, List<Collider2D> colliders)
	{
		return GetRigidbodyContactsCollidersOnlyList(rigidbody, contactFilter, colliders);
	}

	[NativeMethod("GetColliderContactsList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int GetColliderContactsList([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] List<ContactPoint2D> results)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<ContactPoint2D> list = default(List<ContactPoint2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(collider, "collider");
			}
			list = results;
			fixed (ContactPoint2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetColliderContactsList_Injected(intPtr, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetColliderColliderContactsList_Binding")]
	private unsafe static int GetColliderColliderContactsList([NotNull] Collider2D collider1, [NotNull] Collider2D collider2, ContactFilter2D contactFilter, [NotNull] List<ContactPoint2D> results)
	{
		if ((object)collider1 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
		}
		if ((object)collider2 == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<ContactPoint2D> list = default(List<ContactPoint2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider1);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(collider1, "collider1");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(collider2);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(collider2, "collider2");
			}
			list = results;
			fixed (ContactPoint2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetColliderColliderContactsList_Injected(intPtr, intPtr2, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetRigidbodyContactsList_Binding")]
	private unsafe static int GetRigidbodyContactsList([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [NotNull] List<ContactPoint2D> results)
	{
		if ((object)rigidbody == null)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<ContactPoint2D> list = default(List<ContactPoint2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(rigidbody);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
			}
			list = results;
			fixed (ContactPoint2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetRigidbodyContactsList_Injected(intPtr, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetColliderContactsCollidersOnlyList_Binding")]
	private static int GetColliderContactsCollidersOnlyList([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return GetColliderContactsCollidersOnlyList_Injected(intPtr, ref contactFilter, results);
	}

	[NativeMethod("GetRigidbodyContactsCollidersOnlyList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int GetRigidbodyContactsCollidersOnlyList([NotNull] Rigidbody2D rigidbody, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
	{
		if ((object)rigidbody == null)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(rigidbody);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(rigidbody, "rigidbody");
		}
		return GetRigidbodyContactsCollidersOnlyList_Injected(intPtr, ref contactFilter, results);
	}

	internal static void SetEditorDragMovement(bool dragging, GameObject[] objs)
	{
		foreach (Rigidbody2D item in m_LastDisabledRigidbody2D)
		{
			if (item != null)
			{
				item.SetDragBehaviour(dragged: false);
			}
		}
		m_LastDisabledRigidbody2D.Clear();
		if (!dragging)
		{
			return;
		}
		foreach (GameObject gameObject in objs)
		{
			Rigidbody2D[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody2D>(includeInactive: false);
			Rigidbody2D[] array = componentsInChildren;
			foreach (Rigidbody2D rigidbody2D in array)
			{
				m_LastDisabledRigidbody2D.Add(rigidbody2D);
				rigidbody2D.SetDragBehaviour(dragged: true);
			}
		}
	}

	[Obsolete("LinecastNonAlloc has neen deprecated. Please use Linecast.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.Linecast(start, end, results);
	}

	[Obsolete("LinecastNonAlloc has been deprecated. Please use Linecast.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.Linecast(start, end, contactFilter, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	[Obsolete("LinecastNonAlloc has been deprecated. Please use Linecast.", false)]
	public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.Linecast(start, end, contactFilter, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	[Obsolete("LinecastNonAlloc has been deprecated. Please use Linecast.", false)]
	public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.Linecast(start, end, contactFilter, results);
	}

	[Obsolete("RaycastNonAlloc has been deprecated. Please use Raycast.", false)]
	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.Raycast(origin, direction, float.PositiveInfinity, results);
	}

	[ExcludeFromDocs]
	[Obsolete("RaycastNonAlloc has been deprecated. Please use Raycast.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance)
	{
		return defaultPhysicsScene.Raycast(origin, direction, distance, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	[Obsolete("RaycastNonAlloc has been deprecated. Please use Raycast.", false)]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter, results);
	}

	[Obsolete("RaycastNonAlloc has been deprecated. Please use Raycast.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.CircleCast(origin, radius, direction, float.PositiveInfinity, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
	[ExcludeFromDocs]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance)
	{
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
	[ExcludeFromDocs]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CircleCastNonAlloc has been deprecated. Please use CircleCast instead.", false)]
	public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, float.PositiveInfinity, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
	{
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter, results);
	}

	[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter, results);
	}

	[Obsolete("BoxCastNonAlloc has been deprecated. Please use BoxCast.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.BoxCast(origin, size, angle, direction, distance, contactFilter, results);
	}

	[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, float.PositiveInfinity, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
	{
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CapsuleCastNonAlloc has been deprecated. Please use CapsuleCast.", false)]
	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[Obsolete("GetRayIntersectionNonAlloc is deprecated. Please use GetRayIntersection.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results)
	{
		return defaultPhysicsScene.GetRayIntersection(ray, float.PositiveInfinity, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("GetRayIntersectionNonAlloc is deprecated. Please use GetRayIntersection.", false)]
	[ExcludeFromDocs]
	public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance)
	{
		return defaultPhysicsScene.GetRayIntersection(ray, distance, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapPointNonAlloc has been deprecated. Please use OverlapPoint.", false)]
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapPoint(point, results);
	}

	[Obsolete("OverlapPointNonAlloc has been deprecated. Please use OverlapPoint.", false)]
	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapPoint(point, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapPointNonAlloc has been deprecated. Please use OverlapPoint.", false)]
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapPoint(point, contactFilter, results);
	}

	[Obsolete("OverlapPointNonAlloc has been deprecated. Please use OverlapPoint.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapPoint(point, contactFilter, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	[Obsolete("OverlapCircleNonAlloc has been deprecated. Please use OverlapCircle.", false)]
	public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapCircle(point, radius, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapCircleNonAlloc has been deprecated. Please use OverlapCircle.", false)]
	public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapCircle(point, radius, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapCircleNonAlloc has been deprecated. Please use OverlapCircle.", false)]
	public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapCircle(point, radius, contactFilter, results);
	}

	[Obsolete("OverlapCircleNonAlloc has been deprecated. Please use OverlapCircle.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapCircle(point, radius, contactFilter, results);
	}

	[Obsolete("OverlapBoxNonAlloc has been deprecated. Please use OverlapBox.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapBox(point, size, angle, results);
	}

	[Obsolete("OverlapBoxNonAlloc has been deprecated. Please use OverlapBox.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapBoxNonAlloc has been deprecated. Please use OverlapBox.", false)]
	public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter, results);
	}

	[Obsolete("OverlapBoxNonAlloc has been deprecated. Please use OverlapBox.", false)]
	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int OverlapBoxNonAlloc(Vector2 point, Vector2 size, float angle, Collider2D[] results, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapBox(point, size, angle, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapAreaNonAlloc has been deprecated. Please use OverlapArea.", false)]
	public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapArea(pointA, pointB, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapAreaNonAlloc has been deprecated. Please use OverlapArea.", false)]
	[ExcludeFromDocs]
	public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapAreaNonAlloc has been deprecated. Please use OverlapArea.", false)]
	public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter, results);
	}

	[ExcludeFromDocs]
	[Obsolete("OverlapAreaNonAlloc has been deprecated. Please use OverlapArea.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapArea(pointA, pointB, contactFilter, results);
	}

	[Obsolete("OverlapCapsuleNonAlloc has been deprecated. Please use OverlapCapsule.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results)
	{
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, results);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapCapsuleNonAlloc has been deprecated. Please use OverlapCapsule.", false)]
	[ExcludeFromDocs]
	public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter, results);
	}

	[Obsolete("OverlapCapsuleNonAlloc has been deprecated. Please use OverlapCapsule.", false)]
	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter, results);
	}

	[Obsolete("OverlapCapsuleNonAlloc has been deprecated. Please use OverlapCapsule.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ExcludeFromDocs]
	public static int OverlapCapsuleNonAlloc(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return defaultPhysicsScene.OverlapCapsule(point, size, direction, angle, contactFilter, results);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_gravity_Injected(out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_gravity_Injected([In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_simulationLayers_Injected(out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_simulationLayers_Injected([In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_jobOptions_Injected(out PhysicsJobOptions2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_jobOptions_Injected([In] ref PhysicsJobOptions2D value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Simulate_Internal_Injected([In] ref PhysicsScene2D physicsScene, float deltaTime, int simulationLayers);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void IgnoreCollision_Injected(IntPtr collider1, IntPtr collider2, [UnityEngine.Internal.DefaultValue("true")] bool ignore);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIgnoreCollision_Injected(IntPtr collider1, IntPtr collider2);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_Injected(IntPtr collider1, IntPtr collider2);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_TwoCollidersWithFilter_Injected(IntPtr collider1, IntPtr collider2, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_SingleColliderWithFilter_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouchingLayers_Injected(IntPtr collider, [UnityEngine.Internal.DefaultValue("Physics2D.AllLayers")] int layerMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Distance_Internal_Injected(IntPtr colliderA, IntPtr colliderB, out ColliderDistance2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceFrom_Internal_Injected(IntPtr colliderA, [In] ref Vector2 positionA, float angleA, IntPtr colliderB, [In] ref Vector2 positionB, float angleB, out ColliderDistance2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClosestPoint_Collider_Injected([In] ref Vector2 position, IntPtr collider, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClosestPoint_Rigidbody_Injected([In] ref Vector2 position, IntPtr rigidbody, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LinecastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 start, [In] ref Vector2 end, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RaycastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CircleCastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, float radius, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BoxCastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapsuleCastAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRayIntersectionAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector3 origin, [In] ref Vector3 direction, float distance, int layerMask, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D[] OverlapPointAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D[] OverlapCircleAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, float radius, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D[] OverlapBoxAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, float angle, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider2D[] OverlapCapsuleAll_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, CapsuleDirection2D direction, float angle, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderContactsArray_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderColliderContactsArray_Injected(IntPtr collider1, IntPtr collider2, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRigidbodyContactsArray_Injected(IntPtr rigidbody, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderContactsCollidersOnlyArray_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRigidbodyContactsCollidersOnlyArray_Injected(IntPtr rigidbody, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderContactsList_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderColliderContactsList_Injected(IntPtr collider1, IntPtr collider2, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRigidbodyContactsList_Injected(IntPtr rigidbody, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetColliderContactsCollidersOnlyList_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRigidbodyContactsCollidersOnlyList_Injected(IntPtr rigidbody, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);
}
