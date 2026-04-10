using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Modules/Physics2D/Public/PhysicsSceneHandle2D.h")]
public struct PhysicsScene2D : IEquatable<PhysicsScene2D>
{
	private int m_Handle;

	public int subStepCount => SubStepCount_Internal(this);

	public float subStepLostTime => SubStepLostTime_Internal(this);

	public override string ToString()
	{
		return $"({m_Handle})";
	}

	public static bool operator ==(PhysicsScene2D lhs, PhysicsScene2D rhs)
	{
		return lhs.m_Handle == rhs.m_Handle;
	}

	public static bool operator !=(PhysicsScene2D lhs, PhysicsScene2D rhs)
	{
		return lhs.m_Handle != rhs.m_Handle;
	}

	public override int GetHashCode()
	{
		return m_Handle;
	}

	public override bool Equals(object other)
	{
		if (!(other is PhysicsScene2D physicsScene2D))
		{
			return false;
		}
		return m_Handle == physicsScene2D.m_Handle;
	}

	public bool Equals(PhysicsScene2D other)
	{
		return m_Handle == other.m_Handle;
	}

	public bool IsValid()
	{
		return IsValid_Internal(this);
	}

	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	[NativeMethod("IsPhysicsSceneValid")]
	private static bool IsValid_Internal(PhysicsScene2D physicsScene)
	{
		return IsValid_Internal_Injected(ref physicsScene);
	}

	public bool IsEmpty()
	{
		if (IsValid())
		{
			return IsEmpty_Internal(this);
		}
		throw new InvalidOperationException("Cannot check if physics scene is empty as it is invalid.");
	}

	[NativeMethod("IsPhysicsWorldEmpty")]
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	private static bool IsEmpty_Internal(PhysicsScene2D physicsScene)
	{
		return IsEmpty_Internal_Injected(ref physicsScene);
	}

	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	[NativeMethod("GetSubStepCount")]
	private static int SubStepCount_Internal(PhysicsScene2D physicsScene)
	{
		return SubStepCount_Internal_Injected(ref physicsScene);
	}

	[NativeMethod("GetSubStepLostTime")]
	[StaticAccessor("GetPhysicsManager2D()", StaticAccessorType.Arrow)]
	private static float SubStepLostTime_Internal(PhysicsScene2D physicsScene)
	{
		return SubStepLostTime_Internal_Injected(ref physicsScene);
	}

	[ExcludeFromDocs]
	public bool Simulate(float deltaTime)
	{
		return Simulate(deltaTime, -1);
	}

	public bool Simulate(float deltaTime, [DefaultValue("Physics2D.AllLayers")] int simulationLayers = -1)
	{
		if (IsValid())
		{
			return Physics2D.Simulate_Internal(this, deltaTime, simulationLayers);
		}
		throw new InvalidOperationException("Cannot simulate the physics scene as it is invalid.");
	}

	public RaycastHit2D Linecast(Vector2 start, Vector2 end, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return Linecast_Internal(this, start, end, contactFilter);
	}

	public RaycastHit2D Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter)
	{
		return Linecast_Internal(this, start, end, contactFilter);
	}

	public int Linecast(Vector2 start, Vector2 end, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return LinecastArray_Internal(this, start, end, contactFilter, results);
	}

	public int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return LinecastArray_Internal(this, start, end, contactFilter, results);
	}

	public int Linecast(Vector2 start, Vector2 end, ContactFilter2D contactFilter, List<RaycastHit2D> results)
	{
		return LinecastList_Internal(this, start, end, contactFilter, results);
	}

	[NativeMethod("Linecast_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static RaycastHit2D Linecast_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter)
	{
		Linecast_Internal_Injected(ref physicsScene, ref start, ref end, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("LinecastArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int LinecastArray_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = LinecastArray_Internal_Injected(ref physicsScene, ref start, ref end, ref contactFilter, ref results2);
		}
		return result;
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("LinecastList_Binding")]
	private unsafe static int LinecastList_Internal(PhysicsScene2D physicsScene, Vector2 start, Vector2 end, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return LinecastList_Internal_Injected(ref physicsScene, ref start, ref end, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	public RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return Raycast_Internal(this, origin, direction, distance, contactFilter);
	}

	public RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		return Raycast_Internal(this, origin, direction, distance, contactFilter);
	}

	public int Raycast(Vector2 origin, Vector2 direction, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastArray_Internal(this, origin, direction, distance, contactFilter, results);
	}

	public int Raycast(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return RaycastArray_Internal(this, origin, direction, distance, contactFilter, results);
	}

	public int Raycast(Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, List<RaycastHit2D> results)
	{
		return RaycastList_Internal(this, origin, direction, distance, contactFilter, results);
	}

	[NativeMethod("Raycast_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static RaycastHit2D Raycast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		Raycast_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("RaycastArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int RaycastArray_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = RaycastArray_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, ref contactFilter, ref results2);
		}
		return result;
	}

	[NativeMethod("RaycastList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int RaycastList_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return RaycastList_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	public RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCast_Internal(this, origin, radius, direction, distance, contactFilter);
	}

	public RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		return CircleCast_Internal(this, origin, radius, direction, distance, contactFilter);
	}

	public int CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CircleCastArray_Internal(this, origin, radius, direction, distance, contactFilter, results);
	}

	public int CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return CircleCastArray_Internal(this, origin, radius, direction, distance, contactFilter, results);
	}

	public int CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, List<RaycastHit2D> results)
	{
		return CircleCastList_Internal(this, origin, radius, direction, distance, contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("CircleCast_Binding")]
	private static RaycastHit2D CircleCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		CircleCast_Internal_Injected(ref physicsScene, ref origin, radius, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("CircleCastArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int CircleCastArray_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = CircleCastArray_Internal_Injected(ref physicsScene, ref origin, radius, ref direction, distance, ref contactFilter, ref results2);
		}
		return result;
	}

	[NativeMethod("CircleCastList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int CircleCastList_Internal(PhysicsScene2D physicsScene, Vector2 origin, float radius, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CircleCastList_Internal_Injected(ref physicsScene, ref origin, radius, ref direction, distance, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	public RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCast_Internal(this, origin, size, angle, direction, distance, contactFilter);
	}

	public RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		return BoxCast_Internal(this, origin, size, angle, direction, distance, contactFilter);
	}

	public int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return BoxCastArray_Internal(this, origin, size, angle, direction, distance, contactFilter, results);
	}

	public int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return BoxCastArray_Internal(this, origin, size, angle, direction, distance, contactFilter, results);
	}

	public int BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, List<RaycastHit2D> results)
	{
		return BoxCastList_Internal(this, origin, size, angle, direction, distance, contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("BoxCast_Binding")]
	private static RaycastHit2D BoxCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		BoxCast_Internal_Injected(ref physicsScene, ref origin, ref size, angle, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("BoxCastArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int BoxCastArray_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = BoxCastArray_Internal_Injected(ref physicsScene, ref origin, ref size, angle, ref direction, distance, ref contactFilter, ref results2);
		}
		return result;
	}

	[NativeMethod("BoxCastList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int BoxCastList_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return BoxCastList_Internal_Injected(ref physicsScene, ref origin, ref size, angle, ref direction, distance, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	public RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCast_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	public RaycastHit2D CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		return CapsuleCast_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter);
	}

	public int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return CapsuleCastArray_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	public int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return CapsuleCastArray_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	public int CapsuleCast(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, List<RaycastHit2D> results)
	{
		return CapsuleCastList_Internal(this, origin, size, capsuleDirection, angle, direction, distance, contactFilter, results);
	}

	[NativeMethod("CapsuleCast_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static RaycastHit2D CapsuleCast_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter)
	{
		CapsuleCast_Internal_Injected(ref physicsScene, ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, out var ret);
		return ret;
	}

	[NativeMethod("CapsuleCastArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int CapsuleCastArray_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = CapsuleCastArray_Internal_Injected(ref physicsScene, ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, ref results2);
		}
		return result;
	}

	[NativeMethod("CapsuleCastList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int CapsuleCastList_Internal(PhysicsScene2D physicsScene, Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CapsuleCastList_Internal_Injected(ref physicsScene, ref origin, ref size, capsuleDirection, angle, ref direction, distance, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	public RaycastHit2D GetRayIntersection(Ray ray, float distance, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		return GetRayIntersection_Internal(this, ray.origin, ray.direction, distance, layerMask);
	}

	public int GetRayIntersection(Ray ray, float distance, RaycastHit2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		return GetRayIntersectionArray_Internal(this, ray.origin, ray.direction, distance, layerMask, results);
	}

	public int GetRayIntersection(Ray ray, float distance, List<RaycastHit2D> results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		return GetRayIntersectionList_Internal(this, ray.origin, ray.direction, distance, layerMask, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetRayIntersection_Binding")]
	private static RaycastHit2D GetRayIntersection_Internal(PhysicsScene2D physicsScene, Vector3 origin, Vector3 direction, float distance, int layerMask)
	{
		GetRayIntersection_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, layerMask, out var ret);
		return ret;
	}

	[NativeMethod("GetRayIntersectionArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private unsafe static int GetRayIntersectionArray_Internal(PhysicsScene2D physicsScene, Vector3 origin, Vector3 direction, float distance, int layerMask, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int rayIntersectionArray_Internal_Injected;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			rayIntersectionArray_Internal_Injected = GetRayIntersectionArray_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, layerMask, ref results2);
		}
		return rayIntersectionArray_Internal_Injected;
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("GetRayIntersectionList_Binding")]
	private unsafe static int GetRayIntersectionList_Internal(PhysicsScene2D physicsScene, Vector3 origin, Vector3 direction, float distance, int layerMask, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetRayIntersectionList_Internal_Injected(ref physicsScene, ref origin, ref direction, distance, layerMask, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	public Collider2D OverlapPoint(Vector2 point, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPoint_Internal(this, point, contactFilter);
	}

	public Collider2D OverlapPoint(Vector2 point, ContactFilter2D contactFilter)
	{
		return OverlapPoint_Internal(this, point, contactFilter);
	}

	public int OverlapPoint(Vector2 point, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapPointArray_Internal(this, point, contactFilter, results);
	}

	public int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapPointArray_Internal(this, point, contactFilter, results);
	}

	public int OverlapPoint(Vector2 point, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return OverlapPointList_Internal(this, point, contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapPoint_Binding")]
	private static Collider2D OverlapPoint_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter)
	{
		return Unmarshal.UnmarshalUnityObject<Collider2D>(OverlapPoint_Internal_Injected(ref physicsScene, ref point, ref contactFilter));
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapPointArray_Binding")]
	private static int OverlapPointArray_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter, [UnityMarshalAs(NativeType.ScriptingObjectPtr)][NotNull] Collider2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		return OverlapPointArray_Internal_Injected(ref physicsScene, ref point, ref contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapPointList_Binding")]
	private static int OverlapPointList_Internal(PhysicsScene2D physicsScene, Vector2 point, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		return OverlapPointList_Internal_Injected(ref physicsScene, ref point, ref contactFilter, results);
	}

	public Collider2D OverlapCircle(Vector2 point, float radius, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircle_Internal(this, point, radius, contactFilter);
	}

	public Collider2D OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter)
	{
		return OverlapCircle_Internal(this, point, radius, contactFilter);
	}

	public int OverlapCircle(Vector2 point, float radius, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCircleArray_Internal(this, point, radius, contactFilter, results);
	}

	public int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapCircleArray_Internal(this, point, radius, contactFilter, results);
	}

	public int OverlapCircle(Vector2 point, float radius, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return OverlapCircleList_Internal(this, point, radius, contactFilter, results);
	}

	[NativeMethod("OverlapCircle_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static Collider2D OverlapCircle_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter)
	{
		return Unmarshal.UnmarshalUnityObject<Collider2D>(OverlapCircle_Internal_Injected(ref physicsScene, ref point, radius, ref contactFilter));
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapCircleArray_Binding")]
	private static int OverlapCircleArray_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter, [UnityMarshalAs(NativeType.ScriptingObjectPtr)][NotNull] Collider2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		return OverlapCircleArray_Internal_Injected(ref physicsScene, ref point, radius, ref contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapCircleList_Binding")]
	private static int OverlapCircleList_Internal(PhysicsScene2D physicsScene, Vector2 point, float radius, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		return OverlapCircleList_Internal_Injected(ref physicsScene, ref point, radius, ref contactFilter, results);
	}

	public Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBox_Internal(this, point, size, angle, contactFilter);
	}

	public Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
	{
		return OverlapBox_Internal(this, point, size, angle, contactFilter);
	}

	public int OverlapBox(Vector2 point, Vector2 size, float angle, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapBoxArray_Internal(this, point, size, angle, contactFilter, results);
	}

	public int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapBoxArray_Internal(this, point, size, angle, contactFilter, results);
	}

	public int OverlapBox(Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return OverlapBoxList_Internal(this, point, size, angle, contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapBox_Binding")]
	private static Collider2D OverlapBox_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter)
	{
		return Unmarshal.UnmarshalUnityObject<Collider2D>(OverlapBox_Internal_Injected(ref physicsScene, ref point, ref size, angle, ref contactFilter));
	}

	[NativeMethod("OverlapBoxArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int OverlapBoxArray_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, [NotNull][UnityMarshalAs(NativeType.ScriptingObjectPtr)] Collider2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		return OverlapBoxArray_Internal_Injected(ref physicsScene, ref point, ref size, angle, ref contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapBoxList_Binding")]
	private static int OverlapBoxList_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, float angle, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		return OverlapBoxList_Internal_Injected(ref physicsScene, ref point, ref size, angle, ref contactFilter, results);
	}

	public Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapAreaToBoxArray_Internal(pointA, pointB, contactFilter);
	}

	public Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter)
	{
		return OverlapAreaToBoxArray_Internal(pointA, pointB, contactFilter);
	}

	private Collider2D OverlapAreaToBoxArray_Internal(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter)
	{
		Vector2 point = (pointA + pointB) * 0.5f;
		Vector2 size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
		return OverlapBox(point, size, 0f, contactFilter);
	}

	public int OverlapArea(Vector2 pointA, Vector2 pointB, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapAreaToBoxArray_Internal(pointA, pointB, contactFilter, results);
	}

	public int OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapAreaToBoxArray_Internal(pointA, pointB, contactFilter, results);
	}

	private int OverlapAreaToBoxArray_Internal(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, Collider2D[] results)
	{
		Vector2 point = (pointA + pointB) * 0.5f;
		Vector2 size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
		return OverlapBox(point, size, 0f, contactFilter, results);
	}

	public int OverlapArea(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return OverlapAreaToBoxList_Internal(pointA, pointB, contactFilter, results);
	}

	private int OverlapAreaToBoxList_Internal(Vector2 pointA, Vector2 pointB, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		Vector2 point = (pointA + pointB) * 0.5f;
		Vector2 size = new Vector2(Mathf.Abs(pointA.x - pointB.x), Math.Abs(pointA.y - pointB.y));
		return OverlapBox(point, size, 0f, contactFilter, results);
	}

	public Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsule_Internal(this, point, size, direction, angle, contactFilter);
	}

	public Collider2D OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
	{
		return OverlapCapsule_Internal(this, point, size, direction, angle, contactFilter);
	}

	public int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapCapsuleArray_Internal(this, point, size, direction, angle, contactFilter, results);
	}

	public int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapCapsuleArray_Internal(this, point, size, direction, angle, contactFilter, results);
	}

	public int OverlapCapsule(Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return OverlapCapsuleList_Internal(this, point, size, direction, angle, contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapCapsule_Binding")]
	private static Collider2D OverlapCapsule_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter)
	{
		return Unmarshal.UnmarshalUnityObject<Collider2D>(OverlapCapsule_Internal_Injected(ref physicsScene, ref point, ref size, direction, angle, ref contactFilter));
	}

	[NativeMethod("OverlapCapsuleArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int OverlapCapsuleArray_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, [NotNull][UnityMarshalAs(NativeType.ScriptingObjectPtr)] Collider2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		return OverlapCapsuleArray_Internal_Injected(ref physicsScene, ref point, ref size, direction, angle, ref contactFilter, results);
	}

	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	[NativeMethod("OverlapCapsuleList_Binding")]
	private static int OverlapCapsuleList_Internal(PhysicsScene2D physicsScene, Vector2 point, Vector2 size, CapsuleDirection2D direction, float angle, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		return OverlapCapsuleList_Internal_Injected(ref physicsScene, ref point, ref size, direction, angle, ref contactFilter, results);
	}

	public static int OverlapCollider(Collider2D collider, Collider2D[] results, [DefaultValue("Physics2D.DefaultRaycastLayers")] int layerMask = -5)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return OverlapColliderFilteredArray_Internal(collider, contactFilter, results);
	}

	public static int OverlapCollider(Collider2D collider, ContactFilter2D contactFilter, Collider2D[] results)
	{
		return OverlapColliderFilteredArray_Internal(collider, contactFilter, results);
	}

	public static int OverlapCollider(Collider2D collider, List<Collider2D> results)
	{
		return OverlapColliderList_Internal(collider, results);
	}

	public static int OverlapCollider(Collider2D collider, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return OverlapColliderFilteredList_Internal(collider, contactFilter, results);
	}

	public static int OverlapCollider(Vector2 position, float angle, Collider2D collider, List<Collider2D> results)
	{
		if ((bool)collider.attachedRigidbody)
		{
			return OverlapColliderFromList_Internal(position, angle, collider, results);
		}
		throw new InvalidOperationException("Cannot perform a Collider Overlap at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
	}

	public static int OverlapCollider(Vector2 position, float angle, Collider2D collider, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		if ((bool)collider.attachedRigidbody)
		{
			return OverlapColliderFromFilteredList_Internal(position, angle, collider, contactFilter, results);
		}
		throw new InvalidOperationException("Cannot perform a Collider Overlap at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
	}

	[NativeMethod("OverlapColliderFilteredArray_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int OverlapColliderFilteredArray_Internal([NotNull] Collider2D collider, ContactFilter2D contactFilter, [UnityMarshalAs(NativeType.ScriptingObjectPtr)][NotNull] Collider2D[] results)
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
		return OverlapColliderFilteredArray_Internal_Injected(intPtr, ref contactFilter, results);
	}

	[NativeMethod("OverlapColliderList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int OverlapColliderList_Internal([NotNull] Collider2D collider, [NotNull] List<Collider2D> results)
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
		return OverlapColliderList_Internal_Injected(intPtr, results);
	}

	[NativeMethod("OverlapColliderFilteredList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int OverlapColliderFilteredList_Internal([NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
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
		return OverlapColliderFilteredList_Internal_Injected(intPtr, ref contactFilter, results);
	}

	[NativeMethod("OverlapColliderFromList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int OverlapColliderFromList_Internal(Vector2 position, float angle, [NotNull] Collider2D collider, [NotNull] List<Collider2D> results)
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
		return OverlapColliderFromList_Internal_Injected(ref position, angle, intPtr, results);
	}

	[NativeMethod("OverlapColliderFromFilteredList_Binding")]
	[StaticAccessor("PhysicsQuery2D", StaticAccessorType.DoubleColon)]
	private static int OverlapColliderFromFilteredList_Internal(Vector2 position, float angle, [NotNull] Collider2D collider, ContactFilter2D contactFilter, [NotNull] List<Collider2D> results)
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
		return OverlapColliderFromFilteredList_Internal_Injected(ref position, angle, intPtr, ref contactFilter, results);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValid_Internal_Injected([In] ref PhysicsScene2D physicsScene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsEmpty_Internal_Injected([In] ref PhysicsScene2D physicsScene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int SubStepCount_Internal_Injected([In] ref PhysicsScene2D physicsScene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SubStepLostTime_Internal_Injected([In] ref PhysicsScene2D physicsScene);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Linecast_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 start, [In] ref Vector2 end, [In] ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int LinecastArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 start, [In] ref Vector2 end, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int LinecastList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 start, [In] ref Vector2 end, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Raycast_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int RaycastArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int RaycastList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CircleCast_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, float radius, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CircleCastArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, float radius, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CircleCastList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, float radius, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BoxCast_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int BoxCastArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int BoxCastList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapsuleCast_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CapsuleCastArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CapsuleCastList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 origin, [In] ref Vector2 size, CapsuleDirection2D capsuleDirection, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRayIntersection_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector3 origin, [In] ref Vector3 direction, float distance, int layerMask, out RaycastHit2D ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRayIntersectionArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector3 origin, [In] ref Vector3 direction, float distance, int layerMask, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetRayIntersectionList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector3 origin, [In] ref Vector3 direction, float distance, int layerMask, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr OverlapPoint_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapPointArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapPointList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr OverlapCircle_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, float radius, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapCircleArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, float radius, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapCircleList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, float radius, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr OverlapBox_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, float angle, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapBoxArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, float angle, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapBoxList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, float angle, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr OverlapCapsule_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, CapsuleDirection2D direction, float angle, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapCapsuleArray_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, CapsuleDirection2D direction, float angle, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapCapsuleList_Internal_Injected([In] ref PhysicsScene2D physicsScene, [In] ref Vector2 point, [In] ref Vector2 size, CapsuleDirection2D direction, float angle, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapColliderFilteredArray_Internal_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, Collider2D[] results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapColliderList_Internal_Injected(IntPtr collider, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapColliderFilteredList_Internal_Injected(IntPtr collider, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapColliderFromList_Internal_Injected([In] ref Vector2 position, float angle, IntPtr collider, List<Collider2D> results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int OverlapColliderFromFilteredList_Internal_Injected([In] ref Vector2 position, float angle, IntPtr collider, [In] ref ContactFilter2D contactFilter, List<Collider2D> results);
}
