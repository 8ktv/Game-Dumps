using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/Physics/PhysicsQuery.h")]
[NativeHeader("Modules/Physics/PhysicsManager.h")]
[StaticAccessor("GetPhysicsManager()", StaticAccessorType.Dot)]
public class Physics
{
	public delegate void ContactEventDelegate(PhysicsScene scene, NativeArray<ContactPairHeader>.ReadOnly headerArray);

	internal const float k_MaxFloatMinusEpsilon = 3.4028233E+38f;

	public const int IgnoreRaycastLayer = 4;

	public const int DefaultRaycastLayers = -5;

	public const int AllLayers = -1;

	private static readonly Collision s_ReusableCollision;

	public static Vector3 gravity
	{
		[ThreadSafe]
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

	public static extern float defaultContactOffset
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern float sleepThreshold
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool queriesHitTriggers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool queriesHitBackfaces
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern float bounceThreshold
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern float defaultMaxDepenetrationVelocity
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern int defaultSolverIterations
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern int defaultSolverVelocityIterations
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern SimulationMode simulationMode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern float defaultMaxAngularSpeed
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool improvedPatchFriction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool invokeCollisionCallbacks
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static PhysicsScene defaultPhysicsScene => PhysicsScene.GetDefaultScene();

	public static extern bool autoSyncTransforms
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static extern bool reuseCollisionCallbacks
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[StaticAccessor("GetPhysicsManager()")]
	public static extern float interCollisionDistance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetClothInterCollisionDistance")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetClothInterCollisionDistance")]
		set;
	}

	[StaticAccessor("GetPhysicsManager()")]
	public static extern float interCollisionStiffness
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetClothInterCollisionStiffness")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetClothInterCollisionStiffness")]
		set;
	}

	[StaticAccessor("GetPhysicsManager()")]
	public static extern bool interCollisionSettingsToggle
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetClothInterCollisionSettingsToggle")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetClothInterCollisionSettingsToggle")]
		set;
	}

	public static Vector3 clothGravity
	{
		[ThreadSafe]
		get
		{
			get_clothGravity_Injected(out var ret);
			return ret;
		}
		set
		{
			set_clothGravity_Injected(ref value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Physics.autoSimulation has been replaced by Physics.simulationMode", false)]
	public static bool autoSimulation
	{
		get
		{
			return simulationMode != SimulationMode.Script;
		}
		set
		{
			simulationMode = ((!value) ? SimulationMode.Script : SimulationMode.FixedUpdate);
		}
	}

	public static event Action<PhysicsScene, NativeArray<ModifiableContactPair>> ContactModifyEvent;

	public static event Action<PhysicsScene, NativeArray<ModifiableContactPair>> ContactModifyEventCCD;

	internal static event Action<PhysicsScene, IntPtr, int, bool> GenericContactModifyEvent;

	public static event ContactEventDelegate ContactEvent;

	[RequiredByNativeCode]
	private static void OnSceneContactModify(PhysicsScene scene, IntPtr buffer, int count, bool isCCD)
	{
		Physics.GenericContactModifyEvent?.Invoke(scene, buffer, count, isCCD);
	}

	private unsafe static void PhysXOnSceneContactModify(PhysicsScene scene, IntPtr buffer, int count, bool isCCD)
	{
		NativeArray<ModifiableContactPair> arg = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ModifiableContactPair>(buffer.ToPointer(), count, Allocator.None);
		if (!isCCD)
		{
			Physics.ContactModifyEvent?.Invoke(scene, arg);
		}
		else
		{
			Physics.ContactModifyEventCCD?.Invoke(scene, arg);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetIntegrationInfos(out IntPtr integrations, out ulong integrationCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCurrentIntegrationInfo(out IntPtr integration);

	internal unsafe static ReadOnlySpan<IntegrationInfo> GetIntegrationInfos()
	{
		GetIntegrationInfos(out var integrations, out var integrationCount);
		return new ReadOnlySpan<IntegrationInfo>(integrations.ToPointer(), (int)integrationCount);
	}

	public unsafe static IntegrationInfo GetCurrentIntegrationInfo()
	{
		GetCurrentIntegrationInfo(out var integration);
		return *(IntegrationInfo*)integration.ToPointer();
	}

	public static void IgnoreCollision([NotNull] Collider collider1, [NotNull] Collider collider2, [UnityEngine.Internal.DefaultValue("true")] bool ignore)
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

	[ExcludeFromDocs]
	public static void IgnoreCollision(Collider collider1, Collider collider2)
	{
		IgnoreCollision(collider1, collider2, ignore: true);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeName("IgnoreCollision")]
	public static extern void IgnoreLayerCollision(int layer1, int layer2, [UnityEngine.Internal.DefaultValue("true")] bool ignore);

	[ExcludeFromDocs]
	public static void IgnoreLayerCollision(int layer1, int layer2)
	{
		IgnoreLayerCollision(layer1, layer2, ignore: true);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);

	public static bool GetIgnoreCollision([NotNull] Collider collider1, [NotNull] Collider collider2)
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

	public static bool Raycast(Vector3 origin, Vector3 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.Raycast(origin, direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
	{
		return defaultPhysicsScene.Raycast(origin, direction, maxDistance, layerMask);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance)
	{
		return defaultPhysicsScene.Raycast(origin, direction, maxDistance);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction)
	{
		return defaultPhysicsScene.Raycast(origin, direction);
	}

	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[RequiredByNativeCode]
	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		return defaultPhysicsScene.Raycast(origin, direction, out hitInfo, maxDistance, layerMask);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
	{
		return defaultPhysicsScene.Raycast(origin, direction, out hitInfo, maxDistance);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
	{
		return defaultPhysicsScene.Raycast(origin, direction, out hitInfo);
	}

	public static bool Raycast(Ray ray, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, float maxDistance, int layerMask)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, maxDistance, layerMask);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, float maxDistance)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, maxDistance);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction);
	}

	public static bool Raycast(Ray ray, out RaycastHit hitInfo, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		return Raycast(ray.origin, ray.direction, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, out hitInfo, maxDistance);
	}

	[ExcludeFromDocs]
	public static bool Raycast(Ray ray, out RaycastHit hitInfo)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, out hitInfo);
	}

	public static bool Linecast(Vector3 start, Vector3 end, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		Vector3 direction = end - start;
		return defaultPhysicsScene.Raycast(start, direction, direction.magnitude, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Linecast(Vector3 start, Vector3 end, int layerMask)
	{
		return Linecast(start, end, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool Linecast(Vector3 start, Vector3 end)
	{
		return Linecast(start, end, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		Vector3 direction = end - start;
		return defaultPhysicsScene.Raycast(start, direction, out hitInfo, direction.magnitude, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask)
	{
		return Linecast(start, end, out hitInfo, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
	{
		return Linecast(start, end, out hitInfo, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		RaycastHit hitInfo;
		return defaultPhysicsScene.CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask)
	{
		return CapsuleCast(point1, point2, radius, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
	{
		return CapsuleCast(point1, point2, radius, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
	{
		return CapsuleCast(point1, point2, radius, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		return CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
	{
		return CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo)
	{
		return CapsuleCast(point1, point2, radius, direction, out hitInfo, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		return SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
	{
		return SphereCast(origin, radius, direction, out hitInfo, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo)
	{
		return SphereCast(origin, radius, direction, out hitInfo, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static bool SphereCast(Ray ray, float radius, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		RaycastHit hitInfo;
		return SphereCast(ray.origin, radius, ray.direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Ray ray, float radius, float maxDistance, int layerMask)
	{
		return SphereCast(ray, radius, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Ray ray, float radius, float maxDistance)
	{
		return SphereCast(ray, radius, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Ray ray, float radius)
	{
		return SphereCast(ray, radius, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return SphereCast(ray.origin, radius, ray.direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		return SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance)
	{
		return SphereCast(ray, radius, out hitInfo, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo)
	{
		return SphereCast(ray, radius, out hitInfo, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, [UnityEngine.Internal.DefaultValue("Quaternion.identity")] Quaternion orientation, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		RaycastHit hitInfo;
		return defaultPhysicsScene.BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask)
	{
		return BoxCast(center, halfExtents, direction, orientation, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance)
	{
		return BoxCast(center, halfExtents, direction, orientation, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation)
	{
		return BoxCast(center, halfExtents, direction, orientation, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction)
	{
		return BoxCast(center, halfExtents, direction, Quaternion.identity, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, [UnityEngine.Internal.DefaultValue("Quaternion.identity")] Quaternion orientation, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation, float maxDistance, int layerMask)
	{
		return BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation, float maxDistance)
	{
		return BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation)
	{
		return BoxCast(center, halfExtents, direction, out hitInfo, orientation, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo)
	{
		return BoxCast(center, halfExtents, direction, out hitInfo, Quaternion.identity, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::RaycastAll")]
	private static RaycastHit[] Internal_RaycastAll(PhysicsScene physicsScene, Ray ray, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit[] result;
		try
		{
			Internal_RaycastAll_Injected(ref physicsScene, ref ray, maxDistance, mask, queryTriggerInteraction, out ret);
		}
		finally
		{
			RaycastHit[] array = default(RaycastHit[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		float magnitude = direction.magnitude;
		if (magnitude > float.Epsilon)
		{
			Vector3 direction2 = direction / magnitude;
			return Internal_RaycastAll(ray: new Ray(origin, direction2), physicsScene: defaultPhysicsScene, maxDistance: maxDistance, mask: layerMask, queryTriggerInteraction: queryTriggerInteraction);
		}
		return new RaycastHit[0];
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
	{
		return RaycastAll(origin, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance)
	{
		return RaycastAll(origin, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction)
	{
		return RaycastAll(origin, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static RaycastHit[] RaycastAll(Ray ray, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return RaycastAll(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	[RequiredByNativeCode]
	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Ray ray, float maxDistance, int layerMask)
	{
		return RaycastAll(ray.origin, ray.direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Ray ray, float maxDistance)
	{
		return RaycastAll(ray.origin, ray.direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] RaycastAll(Ray ray)
	{
		return RaycastAll(ray.origin, ray.direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	[RequiredByNativeCode]
	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance, int layerMask)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, results, maxDistance, layerMask);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, results, maxDistance);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Ray ray, RaycastHit[] results)
	{
		return defaultPhysicsScene.Raycast(ray.origin, ray.direction, results);
	}

	public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.Raycast(origin, direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask)
	{
		return defaultPhysicsScene.Raycast(origin, direction, results, maxDistance, layerMask);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance)
	{
		return defaultPhysicsScene.Raycast(origin, direction, results, maxDistance);
	}

	[ExcludeFromDocs]
	public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results)
	{
		return defaultPhysicsScene.Raycast(origin, direction, results);
	}

	[FreeFunction("Physics::CapsuleCastAll")]
	private static RaycastHit[] Query_CapsuleCastAll(PhysicsScene physicsScene, Vector3 p0, Vector3 p1, float radius, Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit[] result;
		try
		{
			Query_CapsuleCastAll_Injected(ref physicsScene, ref p0, ref p1, radius, ref direction, maxDistance, mask, queryTriggerInteraction, out ret);
		}
		finally
		{
			RaycastHit[] array = default(RaycastHit[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		float magnitude = direction.magnitude;
		if (magnitude > float.Epsilon)
		{
			Vector3 direction2 = direction / magnitude;
			return Query_CapsuleCastAll(defaultPhysicsScene, point1, point2, radius, direction2, maxDistance, layerMask, queryTriggerInteraction);
		}
		return new RaycastHit[0];
	}

	[ExcludeFromDocs]
	public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask)
	{
		return CapsuleCastAll(point1, point2, radius, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
	{
		return CapsuleCastAll(point1, point2, radius, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
	{
		return CapsuleCastAll(point1, point2, radius, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::SphereCastAll")]
	private static RaycastHit[] Query_SphereCastAll(PhysicsScene physicsScene, Vector3 origin, float radius, Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit[] result;
		try
		{
			Query_SphereCastAll_Injected(ref physicsScene, ref origin, radius, ref direction, maxDistance, mask, queryTriggerInteraction, out ret);
		}
		finally
		{
			RaycastHit[] array = default(RaycastHit[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		float magnitude = direction.magnitude;
		if (magnitude > float.Epsilon)
		{
			Vector3 direction2 = direction / magnitude;
			return Query_SphereCastAll(defaultPhysicsScene, origin, radius, direction2, maxDistance, layerMask, queryTriggerInteraction);
		}
		return new RaycastHit[0];
	}

	[ExcludeFromDocs]
	public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask)
	{
		return SphereCastAll(origin, radius, direction, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance)
	{
		return SphereCastAll(origin, radius, direction, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction)
	{
		return SphereCastAll(origin, radius, direction, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static RaycastHit[] SphereCastAll(Ray ray, float radius, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return SphereCastAll(ray.origin, radius, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance, int layerMask)
	{
		return SphereCastAll(ray, radius, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance)
	{
		return SphereCastAll(ray, radius, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] SphereCastAll(Ray ray, float radius)
	{
		return SphereCastAll(ray, radius, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::OverlapCapsule")]
	private static Collider[] OverlapCapsule_Internal(PhysicsScene physicsScene, Vector3 point0, Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return OverlapCapsule_Internal_Injected(ref physicsScene, ref point0, ref point1, radius, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, [UnityEngine.Internal.DefaultValue("AllLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return OverlapCapsule_Internal(defaultPhysicsScene, point0, point1, radius, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius, int layerMask)
	{
		return OverlapCapsule(point0, point1, radius, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static Collider[] OverlapCapsule(Vector3 point0, Vector3 point1, float radius)
	{
		return OverlapCapsule(point0, point1, radius, -1, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::OverlapSphere")]
	private static Collider[] OverlapSphere_Internal(PhysicsScene physicsScene, Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return OverlapSphere_Internal_Injected(ref physicsScene, ref position, radius, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapSphere(Vector3 position, float radius, [UnityEngine.Internal.DefaultValue("AllLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return OverlapSphere_Internal(defaultPhysicsScene, position, radius, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask)
	{
		return OverlapSphere(position, radius, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static Collider[] OverlapSphere(Vector3 position, float radius)
	{
		return OverlapSphere(position, radius, -1, QueryTriggerInteraction.UseGlobal);
	}

	[NativeName("Simulate")]
	internal static void Simulate_Internal(PhysicsScene physicsScene, float step, SimulationStage stages, SimulationOption options)
	{
		Simulate_Internal_Injected(ref physicsScene, step, stages, options);
	}

	public static void Simulate(float step)
	{
		if (simulationMode != SimulationMode.Script)
		{
			Debug.LogWarning("Physics.Simulate(...) was called but simulation mode is not set to Script. You should set simulation mode to Script first before calling this function therefore the simulation was not run.");
		}
		else
		{
			Simulate_Internal(defaultPhysicsScene, step, SimulationStage.All, SimulationOption.All);
		}
	}

	[NativeName("InterpolateBodies")]
	internal static void InterpolateBodies_Internal(PhysicsScene physicsScene)
	{
		InterpolateBodies_Internal_Injected(ref physicsScene);
	}

	[NativeName("ResetInterpolatedTransformPosition")]
	internal static void ResetInterpolationPoses_Internal(PhysicsScene physicsScene)
	{
		ResetInterpolationPoses_Internal_Injected(ref physicsScene);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void SyncTransforms();

	[FreeFunction("Physics::ComputePenetration")]
	private static bool Query_ComputePenetration([NotNull] Collider colliderA, Vector3 positionA, Quaternion rotationA, [NotNull] Collider colliderB, Vector3 positionB, Quaternion rotationB, ref Vector3 direction, ref float distance)
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
		return Query_ComputePenetration_Injected(intPtr, ref positionA, ref rotationA, intPtr2, ref positionB, ref rotationB, ref direction, ref distance);
	}

	public static bool ComputePenetration(Collider colliderA, Vector3 positionA, Quaternion rotationA, Collider colliderB, Vector3 positionB, Quaternion rotationB, out Vector3 direction, out float distance)
	{
		direction = Vector3.zero;
		distance = 0f;
		return Query_ComputePenetration(colliderA, positionA, rotationA, colliderB, positionB, rotationB, ref direction, ref distance);
	}

	[FreeFunction("Physics::ClosestPoint")]
	private static Vector3 Query_ClosestPoint([NotNull] Collider collider, Vector3 position, Quaternion rotation, Vector3 point)
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
		Query_ClosestPoint_Injected(intPtr, ref position, ref rotation, ref point, out var ret);
		return ret;
	}

	public static Vector3 ClosestPoint(Vector3 point, Collider collider, Vector3 position, Quaternion rotation)
	{
		return Query_ClosestPoint(collider, position, rotation, point);
	}

	public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, [UnityEngine.Internal.DefaultValue("AllLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.OverlapSphere(position, radius, results, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, int layerMask)
	{
		return OverlapSphereNonAlloc(position, radius, results, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results)
	{
		return OverlapSphereNonAlloc(position, radius, results, -1, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::SphereTest")]
	private static bool CheckSphere_Internal(PhysicsScene physicsScene, Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return CheckSphere_Internal_Injected(ref physicsScene, ref position, radius, layerMask, queryTriggerInteraction);
	}

	public static bool CheckSphere(Vector3 position, float radius, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return CheckSphere_Internal(defaultPhysicsScene, position, radius, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool CheckSphere(Vector3 position, float radius, int layerMask)
	{
		return CheckSphere(position, radius, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool CheckSphere(Vector3 position, float radius)
	{
		return CheckSphere(position, radius, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.CapsuleCast(point1, point2, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask)
	{
		return CapsuleCastNonAlloc(point1, point2, radius, direction, results, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance)
	{
		return CapsuleCastNonAlloc(point1, point2, radius, direction, results, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results)
	{
		return CapsuleCastNonAlloc(point1, point2, radius, direction, results, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.SphereCast(origin, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask)
	{
		return SphereCastNonAlloc(origin, radius, direction, results, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance)
	{
		return SphereCastNonAlloc(origin, radius, direction, results, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results)
	{
		return SphereCastNonAlloc(origin, radius, direction, results, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return SphereCastNonAlloc(ray.origin, radius, ray.direction, results, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance, int layerMask)
	{
		return SphereCastNonAlloc(ray, radius, results, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance)
	{
		return SphereCastNonAlloc(ray, radius, results, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results)
	{
		return SphereCastNonAlloc(ray, radius, results, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::CapsuleTest")]
	private static bool CheckCapsule_Internal(PhysicsScene physicsScene, Vector3 start, Vector3 end, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return CheckCapsule_Internal_Injected(ref physicsScene, ref start, ref end, radius, layerMask, queryTriggerInteraction);
	}

	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return CheckCapsule_Internal(defaultPhysicsScene, start, end, radius, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask)
	{
		return CheckCapsule(start, end, radius, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius)
	{
		return CheckCapsule(start, end, radius, -5, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::BoxTest")]
	private static bool CheckBox_Internal(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Quaternion orientation, int layermask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return CheckBox_Internal_Injected(ref physicsScene, ref center, ref halfExtents, ref orientation, layermask, queryTriggerInteraction);
	}

	public static bool CheckBox(Vector3 center, Vector3 halfExtents, [UnityEngine.Internal.DefaultValue("Quaternion.identity")] Quaternion orientation, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layermask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return CheckBox_Internal(defaultPhysicsScene, center, halfExtents, orientation, layermask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask)
	{
		return CheckBox(center, halfExtents, orientation, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation)
	{
		return CheckBox(center, halfExtents, orientation, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static bool CheckBox(Vector3 center, Vector3 halfExtents)
	{
		return CheckBox(center, halfExtents, Quaternion.identity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::OverlapBox")]
	private static Collider[] OverlapBox_Internal(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
	{
		return OverlapBox_Internal_Injected(ref physicsScene, ref center, ref halfExtents, ref orientation, layerMask, queryTriggerInteraction);
	}

	public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, [UnityEngine.Internal.DefaultValue("Quaternion.identity")] Quaternion orientation, [UnityEngine.Internal.DefaultValue("AllLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return OverlapBox_Internal(defaultPhysicsScene, center, halfExtents, orientation, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask)
	{
		return OverlapBox(center, halfExtents, orientation, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation)
	{
		return OverlapBox(center, halfExtents, orientation, -1, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents)
	{
		return OverlapBox(center, halfExtents, Quaternion.identity, -1, QueryTriggerInteraction.UseGlobal);
	}

	public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, [UnityEngine.Internal.DefaultValue("Quaternion.identity")] Quaternion orientation, [UnityEngine.Internal.DefaultValue("AllLayers")] int mask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.OverlapBox(center, halfExtents, results, orientation, mask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int mask)
	{
		return OverlapBoxNonAlloc(center, halfExtents, results, orientation, mask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation)
	{
		return OverlapBoxNonAlloc(center, halfExtents, results, orientation, -1, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results)
	{
		return OverlapBoxNonAlloc(center, halfExtents, results, Quaternion.identity, -1, QueryTriggerInteraction.UseGlobal);
	}

	public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, [UnityEngine.Internal.DefaultValue("Quaternion.identity")] Quaternion orientation, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.BoxCast(center, halfExtents, direction, results, orientation, maxDistance, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation)
	{
		return BoxCastNonAlloc(center, halfExtents, direction, results, orientation, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance)
	{
		return BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance, int layerMask)
	{
		return BoxCastNonAlloc(center, halfExtents, direction, results, orientation, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results)
	{
		return BoxCastNonAlloc(center, halfExtents, direction, results, Quaternion.identity, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[FreeFunction("Physics::BoxCastAll")]
	private static RaycastHit[] Internal_BoxCastAll(PhysicsScene physicsScene, Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		RaycastHit[] result;
		try
		{
			Internal_BoxCastAll_Injected(ref physicsScene, ref center, ref halfExtents, ref direction, ref orientation, maxDistance, layerMask, queryTriggerInteraction, out ret);
		}
		finally
		{
			RaycastHit[] array = default(RaycastHit[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, [UnityEngine.Internal.DefaultValue("Quaternion.identity")] Quaternion orientation, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDistance, [UnityEngine.Internal.DefaultValue("DefaultRaycastLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		float magnitude = direction.magnitude;
		if (magnitude > float.Epsilon)
		{
			Vector3 direction2 = direction / magnitude;
			return Internal_BoxCastAll(defaultPhysicsScene, center, halfExtents, direction2, orientation, maxDistance, layerMask, queryTriggerInteraction);
		}
		return new RaycastHit[0];
	}

	[ExcludeFromDocs]
	public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask)
	{
		return BoxCastAll(center, halfExtents, direction, orientation, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance)
	{
		return BoxCastAll(center, halfExtents, direction, orientation, maxDistance, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation)
	{
		return BoxCastAll(center, halfExtents, direction, orientation, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction)
	{
		return BoxCastAll(center, halfExtents, direction, Quaternion.identity, float.PositiveInfinity, -5, QueryTriggerInteraction.UseGlobal);
	}

	public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, [UnityEngine.Internal.DefaultValue("AllLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
	{
		return defaultPhysicsScene.OverlapCapsule(point0, point1, radius, results, layerMask, queryTriggerInteraction);
	}

	[ExcludeFromDocs]
	public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results, int layerMask)
	{
		return OverlapCapsuleNonAlloc(point0, point1, radius, results, layerMask, QueryTriggerInteraction.UseGlobal);
	}

	[ExcludeFromDocs]
	public static int OverlapCapsuleNonAlloc(Vector3 point0, Vector3 point1, float radius, Collider[] results)
	{
		return OverlapCapsuleNonAlloc(point0, point1, radius, results, -1, QueryTriggerInteraction.UseGlobal);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysicsManager()")]
	[ThreadSafe]
	public static extern void BakeMesh(int meshID, bool convex, MeshColliderCookingOptions cookingOptions);

	public static void BakeMesh(int meshID, bool convex)
	{
		BakeMesh(meshID, convex, MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
	internal static extern bool ConnectPhysicsSDKVisualDebugger();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
	internal static extern void DisconnectPhysicsSDKVisualDebugger();

	[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
	internal static Collider GetColliderByInstanceID(EntityId entityId)
	{
		return Unmarshal.UnmarshalUnityObject<Collider>(GetColliderByInstanceID_Injected(ref entityId));
	}

	[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
	internal static Component GetBodyByInstanceID(EntityId entityId)
	{
		return Unmarshal.UnmarshalUnityObject<Component>(GetBodyByInstanceID_Injected(ref entityId));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
	internal static extern uint TranslateTriangleIndexFromID(int instanceID, uint faceIndex);

	[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
	private static void SendOnCollisionEnter(Component component, Collision collision)
	{
		SendOnCollisionEnter_Injected(Object.MarshalledUnityObject.Marshal(component), collision);
	}

	[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
	private static void SendOnCollisionStay(Component component, Collision collision)
	{
		SendOnCollisionStay_Injected(Object.MarshalledUnityObject.Marshal(component), collision);
	}

	[StaticAccessor("PhysicsManager", StaticAccessorType.DoubleColon)]
	private static void SendOnCollisionExit(Component component, Collision collision)
	{
		SendOnCollisionExit_Injected(Object.MarshalledUnityObject.Marshal(component), collision);
	}

	[Obsolete("Physics.RebuildBroadphaseRegions has been deprecated alongside Multi Box Pruning. Use Automatic Box Pruning instead.", false)]
	public static void RebuildBroadphaseRegions(Bounds worldBounds, int subdivisions)
	{
	}

	[RequiredByNativeCode]
	private unsafe static void OnSceneContact(PhysicsScene scene, IntPtr buffer, int count)
	{
		if (count == 0)
		{
			return;
		}
		NativeArray<ContactPairHeader> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ContactPairHeader>(buffer.ToPointer(), count, Allocator.None);
		try
		{
			Physics.ContactEvent?.Invoke(scene, nativeArray.AsReadOnly());
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		finally
		{
			ReportContacts(nativeArray.AsReadOnly());
		}
	}

	private static void ReportContacts(NativeArray<ContactPairHeader>.ReadOnly array)
	{
		if (!invokeCollisionCallbacks)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			ContactPairHeader header = array[i];
			if (header.hasRemovedBody)
			{
				continue;
			}
			for (int j = 0; j < header.m_NbPairs; j++)
			{
				ref readonly ContactPair contactPair = ref header.GetContactPair(j);
				if (contactPair.hasRemovedCollider)
				{
					continue;
				}
				Component body = header.body;
				Component otherBody = header.otherBody;
				Component component = ((body != null) ? body : contactPair.collider);
				Component component2 = ((otherBody != null) ? otherBody : contactPair.otherCollider);
				if ((bool)component && (bool)component2)
				{
					if (contactPair.isCollisionEnter)
					{
						SendOnCollisionEnter(component, GetCollisionToReport(in header, in contactPair, flipped: false));
						SendOnCollisionEnter(component2, GetCollisionToReport(in header, in contactPair, flipped: true));
					}
					if (contactPair.isCollisionStay)
					{
						SendOnCollisionStay(component, GetCollisionToReport(in header, in contactPair, flipped: false));
						SendOnCollisionStay(component2, GetCollisionToReport(in header, in contactPair, flipped: true));
					}
					if (contactPair.isCollisionExit)
					{
						SendOnCollisionExit(component, GetCollisionToReport(in header, in contactPair, flipped: false));
						SendOnCollisionExit(component2, GetCollisionToReport(in header, in contactPair, flipped: true));
					}
				}
			}
		}
	}

	private static Collision GetCollisionToReport(in ContactPairHeader header, in ContactPair pair, bool flipped)
	{
		if (reuseCollisionCallbacks)
		{
			s_ReusableCollision.Reuse(in header, in pair);
			s_ReusableCollision.Flipped = flipped;
			return s_ReusableCollision;
		}
		return new Collision(in header, in pair, flipped);
	}

	static Physics()
	{
		Physics.GenericContactModifyEvent = PhysXOnSceneContactModify;
		s_ReusableCollision = new Collision();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_gravity_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_gravity_Injected([In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void IgnoreCollision_Injected(IntPtr collider1, IntPtr collider2, [UnityEngine.Internal.DefaultValue("true")] bool ignore);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIgnoreCollision_Injected(IntPtr collider1, IntPtr collider2);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_RaycastAll_Injected([In] ref PhysicsScene physicsScene, [In] ref Ray ray, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Query_CapsuleCastAll_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 p0, [In] ref Vector3 p1, float radius, [In] ref Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Query_SphereCastAll_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 origin, float radius, [In] ref Vector3 direction, float maxDistance, int mask, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider[] OverlapCapsule_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 point0, [In] ref Vector3 point1, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider[] OverlapSphere_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Simulate_Internal_Injected([In] ref PhysicsScene physicsScene, float step, SimulationStage stages, SimulationOption options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InterpolateBodies_Internal_Injected([In] ref PhysicsScene physicsScene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetInterpolationPoses_Internal_Injected([In] ref PhysicsScene physicsScene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Query_ComputePenetration_Injected(IntPtr colliderA, [In] ref Vector3 positionA, [In] ref Quaternion rotationA, IntPtr colliderB, [In] ref Vector3 positionB, [In] ref Quaternion rotationB, ref Vector3 direction, ref float distance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Query_ClosestPoint_Injected(IntPtr collider, [In] ref Vector3 position, [In] ref Quaternion rotation, [In] ref Vector3 point, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_clothGravity_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_clothGravity_Injected([In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CheckSphere_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CheckCapsule_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 start, [In] ref Vector3 end, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CheckBox_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 center, [In] ref Vector3 halfExtents, [In] ref Quaternion orientation, int layermask, QueryTriggerInteraction queryTriggerInteraction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Collider[] OverlapBox_Internal_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 center, [In] ref Vector3 halfExtents, [In] ref Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_BoxCastAll_Injected([In] ref PhysicsScene physicsScene, [In] ref Vector3 center, [In] ref Vector3 halfExtents, [In] ref Vector3 direction, [In] ref Quaternion orientation, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetColliderByInstanceID_Injected([In] ref EntityId entityId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetBodyByInstanceID_Injected([In] ref EntityId entityId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendOnCollisionEnter_Injected(IntPtr component, Collision collision);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendOnCollisionStay_Injected(IntPtr component, Collision collision);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendOnCollisionExit_Injected(IntPtr component, Collision collision);
}
