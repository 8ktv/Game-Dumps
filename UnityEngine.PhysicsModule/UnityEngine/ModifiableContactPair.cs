using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Physics/PhysXContactModification.h")]
[NativeHeader("Modules/Physics/PhysicsCollisionGeometry.h")]
public struct ModifiableContactPair
{
	private IntPtr actor;

	private IntPtr otherActor;

	private IntPtr shape;

	private IntPtr otherShape;

	public Quaternion rotation;

	public Vector3 position;

	public Quaternion otherRotation;

	public Vector3 otherPosition;

	private int numContacts;

	private IntPtr contacts;

	public int colliderInstanceID => ResolveShapeToInstanceID(shape);

	public int otherColliderInstanceID => ResolveShapeToInstanceID(otherShape);

	public int bodyInstanceID => ResolveActorToInstanceID(actor);

	public int otherBodyInstanceID => ResolveActorToInstanceID(otherActor);

	public Vector3 bodyVelocity => GetActorLinearVelocity(actor);

	public Vector3 bodyAngularVelocity => GetActorAngularVelocity(actor);

	public Vector3 otherBodyVelocity => GetActorLinearVelocity(otherActor);

	public Vector3 otherBodyAngularVelocity => GetActorAngularVelocity(otherActor);

	public int contactCount => numContacts;

	public unsafe ModifiableMassProperties massProperties
	{
		get
		{
			return GetContactPatch()->massProperties;
		}
		set
		{
			ModifiableContactPatch* contactPatch = GetContactPatch();
			contactPatch->massProperties = value;
			byte* internalFlags = &contactPatch->internalFlags;
			*internalFlags |= 8;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Physics::PhysxCompatibility::TranslateTriangleIndex", true)]
	internal static extern uint TranslateTriangleIndex(IntPtr shapePtr, uint rawIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Physics::PhysxCompatibility::ResolveShapeToInstanceID", true)]
	internal static extern int ResolveShapeToInstanceID(IntPtr shapePtr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Physics::PhysxCompatibility::ResolveActorToInstanceID", true)]
	internal static extern int ResolveActorToInstanceID(IntPtr actorPtr);

	[FreeFunction("Physics::PhysxCompatibility::GetActorLinearVelocity", true)]
	internal static Vector3 GetActorLinearVelocity(IntPtr actorPtr)
	{
		GetActorLinearVelocity_Injected(actorPtr, out var ret);
		return ret;
	}

	[FreeFunction("Physics::PhysxCompatibility::GetActorAngularVelocity", true)]
	internal static Vector3 GetActorAngularVelocity(IntPtr actorPtr)
	{
		GetActorAngularVelocity_Injected(actorPtr, out var ret);
		return ret;
	}

	public unsafe Vector3 GetPoint(int i)
	{
		return GetContact(i)->contact;
	}

	public unsafe void SetPoint(int i, Vector3 v)
	{
		GetContact(i)->contact = v;
	}

	public unsafe Vector3 GetNormal(int i)
	{
		return GetContact(i)->normal;
	}

	public unsafe void SetNormal(int i, Vector3 normal)
	{
		GetContact(i)->normal = normal;
		byte* internalFlags = &GetContactPatch()->internalFlags;
		*internalFlags |= 0x40;
	}

	public unsafe float GetSeparation(int i)
	{
		return GetContact(i)->separation;
	}

	public unsafe void SetSeparation(int i, float separation)
	{
		GetContact(i)->separation = separation;
	}

	public unsafe Vector3 GetTargetVelocity(int i)
	{
		return GetContact(i)->targetVelocity;
	}

	public unsafe void SetTargetVelocity(int i, Vector3 velocity)
	{
		GetContact(i)->targetVelocity = velocity;
		byte* internalFlags = &GetContactPatch()->internalFlags;
		*internalFlags |= 0x10;
	}

	public unsafe float GetBounciness(int i)
	{
		return GetContact(i)->restitution;
	}

	public unsafe void SetBounciness(int i, float bounciness)
	{
		GetContact(i)->restitution = bounciness;
		byte* internalFlags = &GetContactPatch()->internalFlags;
		*internalFlags |= 0x40;
	}

	public unsafe float GetStaticFriction(int i)
	{
		return GetContact(i)->staticFriction;
	}

	public unsafe void SetStaticFriction(int i, float staticFriction)
	{
		GetContact(i)->staticFriction = staticFriction;
		byte* internalFlags = &GetContactPatch()->internalFlags;
		*internalFlags |= 0x40;
	}

	public unsafe float GetDynamicFriction(int i)
	{
		return GetContact(i)->dynamicFriction;
	}

	public unsafe void SetDynamicFriction(int i, float dynamicFriction)
	{
		GetContact(i)->dynamicFriction = dynamicFriction;
		byte* internalFlags = &GetContactPatch()->internalFlags;
		*internalFlags |= 0x40;
	}

	public unsafe float GetMaxImpulse(int i)
	{
		return GetContact(i)->maxImpulse;
	}

	public unsafe void SetMaxImpulse(int i, float value)
	{
		GetContact(i)->maxImpulse = value;
		byte* internalFlags = &GetContactPatch()->internalFlags;
		*internalFlags |= 0x20;
	}

	public void IgnoreContact(int i)
	{
		SetMaxImpulse(i, 0f);
	}

	public unsafe uint GetFaceIndex(int i)
	{
		if ((GetContactPatch()->internalFlags & 1) != 0)
		{
			IntPtr intPtr = new IntPtr(contacts.ToInt64() + numContacts * sizeof(ModifiableContact) + (numContacts + i) * 4);
			uint rawIndex = *(uint*)(void*)intPtr;
			return TranslateTriangleIndex(otherShape, rawIndex);
		}
		return uint.MaxValue;
	}

	private unsafe ModifiableContact* GetContact(int index)
	{
		IntPtr intPtr = new IntPtr(contacts.ToInt64() + index * sizeof(ModifiableContact));
		return (ModifiableContact*)(void*)intPtr;
	}

	private unsafe ModifiableContactPatch* GetContactPatch()
	{
		IntPtr intPtr = new IntPtr(contacts.ToInt64() - numContacts * sizeof(ModifiableContactPatch));
		return (ModifiableContactPatch*)(void*)intPtr;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetActorLinearVelocity_Injected(IntPtr actorPtr, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetActorAngularVelocity_Injected(IntPtr actorPtr, out Vector3 ret);
}
