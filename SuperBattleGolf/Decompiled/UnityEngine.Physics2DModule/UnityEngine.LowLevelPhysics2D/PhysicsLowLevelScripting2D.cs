using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.LowLevelPhysics2D;

[NativeHeader("Modules/Physics2D/LowLevel/PhysicsLowLevel2D.h")]
[StaticAccessor("PhysicsLowLevel2D", StaticAccessorType.DoubleColon)]
[NativeHeader("Modules/Physics2D/LowLevel/PhysicsWorldManager2D.h")]
internal static class PhysicsLowLevelScripting2D
{
	internal struct PhysicsBuffer : IDisposable
	{
		private IntPtr m_Buffer;

		private int m_Size;

		private Allocator m_Allocator;

		public readonly IntPtr buffer => m_Buffer;

		public readonly int size => m_Size;

		public readonly Allocator allocator => m_Allocator;

		public readonly bool IsEmpty => m_Size == 0;

		public readonly bool IsValid => !IsEmpty;

		public PhysicsBuffer()
		{
			m_Buffer = IntPtr.Zero;
			m_Size = 0;
			m_Allocator = Allocator.None;
		}

		public PhysicsBuffer(IntPtr buffer, int size, Allocator allocator)
		{
			m_Buffer = buffer;
			m_Size = size;
			m_Allocator = allocator;
		}

		public unsafe static PhysicsBuffer FromNativeArray<T>(NativeArray<T> nativeArray) where T : struct
		{
			return new PhysicsBuffer((IntPtr)nativeArray.GetUnsafePtr(), nativeArray.Length, Allocator.None);
		}

		public unsafe static PhysicsBuffer FromSpan<T>(ReadOnlySpan<T> span) where T : unmanaged
		{
			fixed (T* ptr = span)
			{
				return new PhysicsBuffer((IntPtr)ptr, span.Length, Allocator.None);
			}
		}

		public unsafe readonly NativeArray<T> ToNativeArray<T>() where T : struct
		{
			if (m_Size == 0)
			{
				return default(NativeArray<T>);
			}
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(m_Buffer.ToPointer(), m_Size, m_Allocator);
		}

		public unsafe readonly Span<T> ToSpan<T>() where T : struct
		{
			return new Span<T>(m_Buffer.ToPointer(), m_Size);
		}

		public unsafe readonly ReadOnlySpan<T> ToReadOnlySpan<T>() where T : struct
		{
			return new ReadOnlySpan<T>(m_Buffer.ToPointer(), m_Size);
		}

		public unsafe void Dispose()
		{
			if (m_Size != 0)
			{
				UnsafeUtility.FreeTracked(m_Buffer.ToPointer(), m_Allocator);
				m_Buffer = IntPtr.Zero;
				m_Size = 0;
				m_Allocator = Allocator.None;
			}
		}

		public override readonly string ToString()
		{
			return $"size={m_Size}, allocator={m_Allocator}";
		}
	}

	internal readonly struct PhysicsBufferPair
	{
		public readonly PhysicsBuffer buffer1;

		public readonly PhysicsBuffer buffer2;
	}

	[NativeMethod(Name = "PhysicsBody::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsBodyDefinition PhysicsBody_GetDefaultDefinition(bool useSettings)
	{
		PhysicsBody_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::Create")]
	internal static PhysicsBody PhysicsBody_Create(PhysicsWorld world, PhysicsBodyDefinition definition)
	{
		PhysicsBody_Create_Injected(ref world, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::CreateBatch")]
	internal unsafe static PhysicsBuffer PhysicsBody_CreateBatch(PhysicsWorld world, ReadOnlySpan<PhysicsBodyDefinition> definitions, int bodyCount, Allocator allocator)
	{
		ReadOnlySpan<PhysicsBodyDefinition> readOnlySpan = definitions;
		PhysicsBuffer ret;
		fixed (PhysicsBodyDefinition* begin = readOnlySpan)
		{
			ManagedSpanWrapper definitions2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsBody_CreateBatch_Injected(ref world, ref definitions2, bodyCount, allocator, out ret);
		}
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::Destroy")]
	internal static bool PhysicsBody_Destroy(PhysicsBody body, int ownerKey)
	{
		return PhysicsBody_Destroy_Injected(ref body, ownerKey);
	}

	[NativeMethod(Name = "PhysicsBody::DestroyBatch")]
	internal unsafe static void PhysicsBody_DestroyBatch(ReadOnlySpan<PhysicsBody> bodies)
	{
		ReadOnlySpan<PhysicsBody> readOnlySpan = bodies;
		fixed (PhysicsBody* begin = readOnlySpan)
		{
			ManagedSpanWrapper bodies2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsBody_DestroyBatch_Injected(ref bodies2);
		}
	}

	[NativeMethod(Name = "PhysicsBody::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsBody_IsValid(PhysicsBody body)
	{
		return PhysicsBody_IsValid_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetBatchVelocity", IsThreadSafe = true)]
	internal unsafe static void PhysicsBody_SetBatchVelocity(ReadOnlySpan<PhysicsBody.BatchVelocity> batch)
	{
		ReadOnlySpan<PhysicsBody.BatchVelocity> readOnlySpan = batch;
		fixed (PhysicsBody.BatchVelocity* begin = readOnlySpan)
		{
			ManagedSpanWrapper batch2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsBody_SetBatchVelocity_Injected(ref batch2);
		}
	}

	[NativeMethod(Name = "PhysicsBody::SetBatchForce", IsThreadSafe = true)]
	internal unsafe static void PhysicsBody_SetBatchForce(ReadOnlySpan<PhysicsBody.BatchForce> batch)
	{
		ReadOnlySpan<PhysicsBody.BatchForce> readOnlySpan = batch;
		fixed (PhysicsBody.BatchForce* begin = readOnlySpan)
		{
			ManagedSpanWrapper batch2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsBody_SetBatchForce_Injected(ref batch2);
		}
	}

	[NativeMethod(Name = "PhysicsBody::SetBatchImpulse", IsThreadSafe = true)]
	internal unsafe static void PhysicsBody_SetBatchImpulse(ReadOnlySpan<PhysicsBody.BatchImpulse> batch)
	{
		ReadOnlySpan<PhysicsBody.BatchImpulse> readOnlySpan = batch;
		fixed (PhysicsBody.BatchImpulse* begin = readOnlySpan)
		{
			ManagedSpanWrapper batch2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsBody_SetBatchImpulse_Injected(ref batch2);
		}
	}

	[NativeMethod(Name = "PhysicsBody::SetBatchTransform", IsThreadSafe = true)]
	internal unsafe static void PhysicsBody_SetBatchTransform(ReadOnlySpan<PhysicsBody.BatchTransform> batch)
	{
		ReadOnlySpan<PhysicsBody.BatchTransform> readOnlySpan = batch;
		fixed (PhysicsBody.BatchTransform* begin = readOnlySpan)
		{
			ManagedSpanWrapper batch2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsBody_SetBatchTransform_Injected(ref batch2);
		}
	}

	[NativeMethod(Name = "PhysicsBody::WriteDefinition")]
	internal static void PhysicsBody_WriteDefinition(PhysicsBody body, PhysicsBodyDefinition definition, bool onlyExtendedProperties)
	{
		PhysicsBody_WriteDefinition_Injected(ref body, ref definition, onlyExtendedProperties);
	}

	[NativeMethod(Name = "PhysicsBody::ReadDefinition")]
	internal static PhysicsBodyDefinition PhysicsBody_ReadDefinition(PhysicsBody body)
	{
		PhysicsBody_ReadDefinition_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetWorld", IsThreadSafe = true)]
	internal static PhysicsWorld PhysicsBody_GetWorld(PhysicsBody body)
	{
		PhysicsBody_GetWorld_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetBodyType", IsThreadSafe = true)]
	internal static PhysicsBody.BodyType PhysicsBody_GetBodyType(PhysicsBody body)
	{
		return PhysicsBody_GetBodyType_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetBodyType")]
	internal static void PhysicsBody_SetBodyType(PhysicsBody body, PhysicsBody.BodyType type)
	{
		PhysicsBody_SetBodyType_Injected(ref body, type);
	}

	[NativeMethod(Name = "PhysicsBody::SetBodyConstraints")]
	internal static void PhysicsBody_SetBodyConstraints(PhysicsBody body, PhysicsBody.BodyConstraints constraints)
	{
		PhysicsBody_SetBodyConstraints_Injected(ref body, constraints);
	}

	[NativeMethod(Name = "PhysicsBody::GetBodyConstraints", IsThreadSafe = true)]
	internal static PhysicsBody.BodyConstraints PhysicsBody_GetBodyConstraints(PhysicsBody body)
	{
		return PhysicsBody_GetBodyConstraints_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::GetPosition", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetPosition(PhysicsBody body)
	{
		PhysicsBody_GetPosition_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::SetPosition", IsThreadSafe = true)]
	internal static void PhysicsBody_SetPosition(PhysicsBody body, Vector2 position)
	{
		PhysicsBody_SetPosition_Injected(ref body, ref position);
	}

	[NativeMethod(Name = "PhysicsBody::GetRotation", IsThreadSafe = true)]
	internal static PhysicsRotate PhysicsBody_GetRotation(PhysicsBody body)
	{
		PhysicsBody_GetRotation_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::SetRotation", IsThreadSafe = true)]
	internal static void PhysicsBody_SetRotation(PhysicsBody body, PhysicsRotate rotation)
	{
		PhysicsBody_SetRotation_Injected(ref body, ref rotation);
	}

	[NativeMethod(Name = "PhysicsBody::GetTransform", IsThreadSafe = true)]
	internal static PhysicsTransform PhysicsBody_GetTransform(PhysicsBody body)
	{
		PhysicsBody_GetTransform_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::SetTransform", IsThreadSafe = true)]
	internal static void PhysicsBody_SetTransform(PhysicsBody body, PhysicsTransform transform)
	{
		PhysicsBody_SetTransform_Injected(ref body, ref transform);
	}

	[NativeMethod(Name = "PhysicsBody::SetTransformTarget", IsThreadSafe = true)]
	internal static void PhysicsBody_SetTransformTarget(PhysicsBody body, PhysicsTransform transform, float deltaTime)
	{
		PhysicsBody_SetTransformTarget_Injected(ref body, ref transform, deltaTime);
	}

	[NativeMethod(Name = "PhysicsBody::GetLocalPoint", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetLocalPoint(PhysicsBody body, Vector2 worldPoint)
	{
		PhysicsBody_GetLocalPoint_Injected(ref body, ref worldPoint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetWorldPoint", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetWorldPoint(PhysicsBody body, Vector2 localPoint)
	{
		PhysicsBody_GetWorldPoint_Injected(ref body, ref localPoint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetLocalVector", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetLocalVector(PhysicsBody body, Vector2 worldVector)
	{
		PhysicsBody_GetLocalVector_Injected(ref body, ref worldVector, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetWorldVector", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetWorldVector(PhysicsBody body, Vector2 localVector)
	{
		PhysicsBody_GetWorldVector_Injected(ref body, ref localVector, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetLocalPointVelocity", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetLocalPointVelocity(PhysicsBody body, Vector2 localPoint)
	{
		PhysicsBody_GetLocalPointVelocity_Injected(ref body, ref localPoint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetWorldPointVelocity", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetWorldPointVelocity(PhysicsBody body, Vector2 worldPoint)
	{
		PhysicsBody_GetWorldPointVelocity_Injected(ref body, ref worldPoint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetLinearVelocity", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetLinearVelocity(PhysicsBody body)
	{
		PhysicsBody_GetLinearVelocity_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::SetLinearVelocity", IsThreadSafe = true)]
	internal static void PhysicsBody_SetLinearVelocity(PhysicsBody body, Vector2 linearVelocity)
	{
		PhysicsBody_SetLinearVelocity_Injected(ref body, ref linearVelocity);
	}

	[NativeMethod(Name = "PhysicsBody::GetAngularVelocity", IsThreadSafe = true)]
	internal static float PhysicsBody_GetAngularVelocity(PhysicsBody body)
	{
		return PhysicsBody_GetAngularVelocity_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetAngularVelocity", IsThreadSafe = true)]
	internal static void PhysicsBody_SetAngularVelocity(PhysicsBody body, float angularVelocity)
	{
		PhysicsBody_SetAngularVelocity_Injected(ref body, angularVelocity);
	}

	[NativeMethod(Name = "PhysicsBody::GetMass", IsThreadSafe = true)]
	internal static float PhysicsBody_GetMass(PhysicsBody body)
	{
		return PhysicsBody_GetMass_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::GetRotationalInertia", IsThreadSafe = true)]
	internal static float PhysicsBody_GetRotationalInertia(PhysicsBody body)
	{
		return PhysicsBody_GetRotationalInertia_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::GetLocalCenterOfMass", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetLocalCenterOfMass(PhysicsBody body)
	{
		PhysicsBody_GetLocalCenterOfMass_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetWorldCenterOfMass", IsThreadSafe = true)]
	internal static Vector2 PhysicsBody_GetWorldCenterOfMass(PhysicsBody body)
	{
		PhysicsBody_GetWorldCenterOfMass_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::SetMassConfiguration", IsThreadSafe = true)]
	internal static void PhysicsBody_SetMassConfiguration(PhysicsBody body, PhysicsBody.MassConfiguration massData)
	{
		PhysicsBody_SetMassConfiguration_Injected(ref body, ref massData);
	}

	[NativeMethod(Name = "PhysicsBody::GetMassConfiguration", IsThreadSafe = true)]
	internal static PhysicsBody.MassConfiguration PhysicsBody_GetMassConfiguration(PhysicsBody body)
	{
		PhysicsBody_GetMassConfiguration_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::ApplyMassFromShapes", IsThreadSafe = true)]
	internal static void PhysicsBody_ApplyMassFromShapes(PhysicsBody body)
	{
		PhysicsBody_ApplyMassFromShapes_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetLinearDamping", IsThreadSafe = true)]
	internal static void PhysicsBody_SetLinearDamping(PhysicsBody body, float linearDamping)
	{
		PhysicsBody_SetLinearDamping_Injected(ref body, linearDamping);
	}

	[NativeMethod(Name = "PhysicsBody::GetLinearDamping", IsThreadSafe = true)]
	internal static float PhysicsBody_GetLinearDamping(PhysicsBody body)
	{
		return PhysicsBody_GetLinearDamping_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetAngularDamping", IsThreadSafe = true)]
	internal static void PhysicsBody_SetAngularDamping(PhysicsBody body, float angularDamping)
	{
		PhysicsBody_SetAngularDamping_Injected(ref body, angularDamping);
	}

	[NativeMethod(Name = "PhysicsBody::GetAngularDamping", IsThreadSafe = true)]
	internal static float PhysicsBody_GetAngularDamping(PhysicsBody body)
	{
		return PhysicsBody_GetAngularDamping_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetGravityScale", IsThreadSafe = true)]
	internal static void PhysicsBody_SetGravityScale(PhysicsBody body, float gravityScale)
	{
		PhysicsBody_SetGravityScale_Injected(ref body, gravityScale);
	}

	[NativeMethod(Name = "PhysicsBody::GetGravityScale", IsThreadSafe = true)]
	internal static float PhysicsBody_GetGravityScale(PhysicsBody body)
	{
		return PhysicsBody_GetGravityScale_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetAwake", IsThreadSafe = true)]
	internal static void PhysicsBody_SetAwake(PhysicsBody body, bool flag)
	{
		PhysicsBody_SetAwake_Injected(ref body, flag);
	}

	[NativeMethod(Name = "PhysicsBody::GetAwake", IsThreadSafe = true)]
	internal static bool PhysicsBody_GetAwake(PhysicsBody body)
	{
		return PhysicsBody_GetAwake_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetSleepingAllowed", IsThreadSafe = true)]
	internal static void PhysicsBody_SetSleepingAllowed(PhysicsBody body, bool enableSleep)
	{
		PhysicsBody_SetSleepingAllowed_Injected(ref body, enableSleep);
	}

	[NativeMethod(Name = "PhysicsBody::GetSleepingAllowed", IsThreadSafe = true)]
	internal static bool PhysicsBody_GetSleepingAllowed(PhysicsBody body)
	{
		return PhysicsBody_GetSleepingAllowed_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetSleepThreshold", IsThreadSafe = true)]
	internal static void PhysicsBody_SetSleepThreshold(PhysicsBody body, float threshold)
	{
		PhysicsBody_SetSleepThreshold_Injected(ref body, threshold);
	}

	[NativeMethod(Name = "PhysicsBody::GetSleepThreshold", IsThreadSafe = true)]
	internal static float PhysicsBody_GetSleepThreshold(PhysicsBody body)
	{
		return PhysicsBody_GetSleepThreshold_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetEnabled")]
	internal static void PhysicsBody_SetEnabled(PhysicsBody body, bool flag)
	{
		PhysicsBody_SetEnabled_Injected(ref body, flag);
	}

	[NativeMethod(Name = "PhysicsBody::GetEnabled", IsThreadSafe = true)]
	internal static bool PhysicsBody_GetEnabled(PhysicsBody body)
	{
		return PhysicsBody_GetEnabled_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetFastRotationAllowed", IsThreadSafe = true)]
	internal static void PhysicsBody_SetFastRotationAllowed(PhysicsBody body, bool flag)
	{
		PhysicsBody_SetFastRotationAllowed_Injected(ref body, flag);
	}

	[NativeMethod(Name = "PhysicsBody::GetFastRotationAllowed", IsThreadSafe = true)]
	internal static bool PhysicsBody_GetFastRotationAllowed(PhysicsBody body)
	{
		return PhysicsBody_GetFastRotationAllowed_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetFastCollisionsAllowed")]
	internal static void PhysicsBody_SetFastCollisionsAllowed(PhysicsBody body, bool flag)
	{
		PhysicsBody_SetFastCollisionsAllowed_Injected(ref body, flag);
	}

	[NativeMethod(Name = "PhysicsBody::GetFastCollisionsAllowed", IsThreadSafe = true)]
	internal static bool PhysicsBody_GetFastCollisionsAllowed(PhysicsBody body)
	{
		return PhysicsBody_GetFastCollisionsAllowed_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::ApplyForce", IsThreadSafe = true)]
	internal static void PhysicsBody_ApplyForce(PhysicsBody body, Vector2 force, Vector2 point, bool wake)
	{
		PhysicsBody_ApplyForce_Injected(ref body, ref force, ref point, wake);
	}

	[NativeMethod(Name = "PhysicsBody::ApplyForceToCenter", IsThreadSafe = true)]
	internal static void PhysicsBody_ApplyForceToCenter(PhysicsBody body, Vector2 force, bool wake)
	{
		PhysicsBody_ApplyForceToCenter_Injected(ref body, ref force, wake);
	}

	[NativeMethod(Name = "PhysicsBody::ApplyTorque", IsThreadSafe = true)]
	internal static void PhysicsBody_ApplyTorque(PhysicsBody body, float torque, bool wake)
	{
		PhysicsBody_ApplyTorque_Injected(ref body, torque, wake);
	}

	[NativeMethod(Name = "PhysicsBody::ApplyLinearImpulse", IsThreadSafe = true)]
	internal static void PhysicsBody_ApplyLinearImpulse(PhysicsBody body, Vector2 impulse, Vector2 point, bool wake)
	{
		PhysicsBody_ApplyLinearImpulse_Injected(ref body, ref impulse, ref point, wake);
	}

	[NativeMethod(Name = "PhysicsBody::ApplyLinearImpulseToCenter", IsThreadSafe = true)]
	internal static void PhysicsBody_ApplyLinearImpulseToCenter(PhysicsBody body, Vector2 impulse, bool wake)
	{
		PhysicsBody_ApplyLinearImpulseToCenter_Injected(ref body, ref impulse, wake);
	}

	[NativeMethod(Name = "PhysicsBody::ApplyAngularImpulse", IsThreadSafe = true)]
	internal static void PhysicsBody_ApplyAngularImpulse(PhysicsBody body, float impulse, bool wake)
	{
		PhysicsBody_ApplyAngularImpulse_Injected(ref body, impulse, wake);
	}

	[NativeMethod(Name = "PhysicsBody::ClearForces", IsThreadSafe = true)]
	internal static void PhysicsBody_ClearForces(PhysicsBody body)
	{
		PhysicsBody_ClearForces_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::WakeTouching", IsThreadSafe = true)]
	internal static void PhysicsBody_WakeTouching(PhysicsBody body)
	{
		PhysicsBody_WakeTouching_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetContactEvents", IsThreadSafe = true)]
	internal static void PhysicsBody_SetContactEvents(PhysicsBody body, bool flag)
	{
		PhysicsBody_SetContactEvents_Injected(ref body, flag);
	}

	[NativeMethod(Name = "PhysicsBody::SetHitEvents", IsThreadSafe = true)]
	internal static void PhysicsBody_SetHitEvents(PhysicsBody body, bool flag)
	{
		PhysicsBody_SetHitEvents_Injected(ref body, flag);
	}

	[NativeMethod(Name = "PhysicsBody::GetShapeCount", IsThreadSafe = true)]
	internal static int PhysicsBody_GetShapeCount(PhysicsBody body)
	{
		return PhysicsBody_GetShapeCount_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::GetShapes", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsBody_GetShapes(PhysicsBody PhysicsBody, Allocator allocator)
	{
		PhysicsBody_GetShapes_Injected(ref PhysicsBody, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetJointCount", IsThreadSafe = true)]
	internal static int PhysicsBody_GetJointCount(PhysicsBody body)
	{
		return PhysicsBody_GetJointCount_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::GetJoints", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsBody_GetJoints(PhysicsBody body, Allocator allocator)
	{
		PhysicsBody_GetJoints_Injected(ref body, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::GetContacts", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsBody_GetContacts(PhysicsBody body, Allocator allocator)
	{
		PhysicsBody_GetContacts_Injected(ref body, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB PhysicsBody_CalculateAABB(PhysicsBody body)
	{
		PhysicsBody_CalculateAABB_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::Draw", IsThreadSafe = true)]
	internal static void PhysicsBody_Draw(PhysicsBody body)
	{
		PhysicsBody_Draw_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetOwner", IsThreadSafe = true)]
	internal static int PhysicsBody_SetOwner(PhysicsBody body, Object ownerObject)
	{
		return PhysicsBody_SetOwner_Injected(ref body, Object.MarshalledUnityObject.Marshal(ownerObject));
	}

	[NativeMethod(Name = "PhysicsBody::GetOwner", IsThreadSafe = true)]
	internal static Object PhysicsBody_GetOwner(PhysicsBody body)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(PhysicsBody_GetOwner_Injected(ref body));
	}

	[NativeMethod(Name = "PhysicsBody::IsOwned", IsThreadSafe = true)]
	internal static bool PhysicsBody_IsOwned(PhysicsBody body)
	{
		return PhysicsBody_IsOwned_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetCallbackTarget", IsThreadSafe = true)]
	internal static void PhysicsBody_SetCallbackTarget(PhysicsBody body, object callbackTarget)
	{
		PhysicsBody_SetCallbackTarget_Injected(ref body, callbackTarget);
	}

	[NativeMethod(Name = "PhysicsBody::GetCallbackTarget", IsThreadSafe = true)]
	internal static object PhysicsBody_GetCallbackTarget(PhysicsBody body)
	{
		return PhysicsBody_GetCallbackTarget_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsBody::SetUserData", IsThreadSafe = true)]
	internal static void PhysicsBody_SetUserData(PhysicsBody body, PhysicsUserData physicsUserData)
	{
		PhysicsBody_SetUserData_Injected(ref body, ref physicsUserData);
	}

	[NativeMethod(Name = "PhysicsBody::GetUserData", IsThreadSafe = true)]
	internal static PhysicsUserData PhysicsBody_GetUserData(PhysicsBody body)
	{
		PhysicsBody_GetUserData_Injected(ref body, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsBody::SetTransformObject", IsThreadSafe = true)]
	internal static void PhysicsBody_SetTransformObject(PhysicsBody body, Transform transform)
	{
		PhysicsBody_SetTransformObject_Injected(ref body, Object.MarshalledUnityObject.Marshal(transform));
	}

	[NativeMethod(Name = "PhysicsBody::GetTransformObject", IsThreadSafe = true)]
	internal static Transform PhysicsBody_GetTransformObject(PhysicsBody body)
	{
		return Unmarshal.UnmarshalUnityObject<Transform>(PhysicsBody_GetTransformObject_Injected(ref body));
	}

	[NativeMethod(Name = "PhysicsBody::SetTransformWriteMode", IsThreadSafe = true)]
	internal static void PhysicsBody_SetTransformWriteMode(PhysicsBody body, PhysicsBody.TransformWriteMode writeMode)
	{
		PhysicsBody_SetTransformWriteMode_Injected(ref body, writeMode);
	}

	[NativeMethod(Name = "PhysicsBody::GetTransformWriteMode", IsThreadSafe = true)]
	internal static PhysicsBody.TransformWriteMode PhysicsBody_GetTransformWriteMode(PhysicsBody body)
	{
		return PhysicsBody_GetTransformWriteMode_Injected(ref body);
	}

	[NativeMethod(Name = "PhysicsChain::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsChainDefinition PhysicsChain_GetDefaultDefinition(bool useSettings)
	{
		PhysicsChain_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::Create")]
	internal static PhysicsChain PhysicsChain_Create(PhysicsBody body, ChainGeometry geometry, PhysicsChainDefinition definition)
	{
		PhysicsChain_Create_Injected(ref body, ref geometry, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::Destroy")]
	internal static bool PhysicsChain_Destroy(PhysicsChain chain, int ownerKey)
	{
		return PhysicsChain_Destroy_Injected(ref chain, ownerKey);
	}

	[NativeMethod(Name = "PhysicsChain::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsChain_IsValid(PhysicsChain chain)
	{
		return PhysicsChain_IsValid_Injected(ref chain);
	}

	[NativeMethod(Name = "PhysicsChain::GetWorld", IsThreadSafe = true)]
	internal static PhysicsWorld PhysicsChain_GetWorld(PhysicsChain chain)
	{
		PhysicsChain_GetWorld_Injected(ref chain, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::GetBody", IsThreadSafe = true)]
	internal static PhysicsBody PhysicsChain_GetBody(PhysicsChain chain)
	{
		PhysicsChain_GetBody_Injected(ref chain, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::SetFriction", IsThreadSafe = true)]
	internal static void PhysicsChain_SetFriction(PhysicsChain chain, float friction)
	{
		PhysicsChain_SetFriction_Injected(ref chain, friction);
	}

	[NativeMethod(Name = "PhysicsChain::GetFriction", IsThreadSafe = true)]
	internal static float PhysicsChain_GetFriction(PhysicsChain chain)
	{
		return PhysicsChain_GetFriction_Injected(ref chain);
	}

	[NativeMethod(Name = "PhysicsChain::SetBounciness", IsThreadSafe = true)]
	internal static void PhysicsChain_SetBounciness(PhysicsChain chain, float bounciness)
	{
		PhysicsChain_SetBounciness_Injected(ref chain, bounciness);
	}

	[NativeMethod(Name = "PhysicsChain::GetBounciness", IsThreadSafe = true)]
	internal static float PhysicsChain_GetBounciness(PhysicsChain chain)
	{
		return PhysicsChain_GetBounciness_Injected(ref chain);
	}

	[NativeMethod(Name = "PhysicsChain::SetFrictionMixing", IsThreadSafe = true)]
	internal static void PhysicsChain_SetFrictionMixing(PhysicsChain chain, PhysicsShape.SurfaceMaterial.MixingMode frictionMixing)
	{
		PhysicsChain_SetFrictionMixing_Injected(ref chain, frictionMixing);
	}

	[NativeMethod(Name = "PhysicsChain::GetFrictionMixing", IsThreadSafe = true)]
	internal static PhysicsShape.SurfaceMaterial.MixingMode PhysicsChain_GetFrictionMixing(PhysicsChain chain)
	{
		return PhysicsChain_GetFrictionMixing_Injected(ref chain);
	}

	[NativeMethod(Name = "PhysicsChain::SetBouncinessMixing", IsThreadSafe = true)]
	internal static void PhysicsChain_SetBouncinessMixing(PhysicsChain chain, PhysicsShape.SurfaceMaterial.MixingMode bouncinessMixing)
	{
		PhysicsChain_SetBouncinessMixing_Injected(ref chain, bouncinessMixing);
	}

	[NativeMethod(Name = "PhysicsChain::GetBouncinessMixing", IsThreadSafe = true)]
	internal static PhysicsShape.SurfaceMaterial.MixingMode PhysicsChain_GetBouncinessMixing(PhysicsChain chain)
	{
		return PhysicsChain_GetBouncinessMixing_Injected(ref chain);
	}

	[NativeMethod(Name = "PhysicsChain::GetSegmentCount", IsThreadSafe = true)]
	internal static int PhysicsChain_GetSegmentCount(PhysicsChain chain)
	{
		return PhysicsChain_GetSegmentCount_Injected(ref chain);
	}

	[NativeMethod(Name = "PhysicsChain::GetSegments", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsChain_GetSegments(PhysicsChain chain, Allocator allocator)
	{
		PhysicsChain_GetSegments_Injected(ref chain, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::GetSegmentIndex", IsThreadSafe = true)]
	internal static int PhysicsChain_GetSegmentIndex(PhysicsChain chain, PhysicsShape chainSegmentShape)
	{
		return PhysicsChain_GetSegmentIndex_Injected(ref chain, ref chainSegmentShape);
	}

	[NativeMethod(Name = "PhysicsChain::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB PhysicsChain_CalculateAABB(PhysicsChain chain)
	{
		PhysicsChain_CalculateAABB_Injected(ref chain, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::ClosestPoint", IsThreadSafe = true)]
	internal static Vector2 PhysicsChain_ClosestPoint(PhysicsChain chain, Vector2 point, out PhysicsShape chainSegmentShape)
	{
		PhysicsChain_ClosestPoint_Injected(ref chain, ref point, out chainSegmentShape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult PhysicsChain_CastRay(PhysicsChain chain, PhysicsQuery.CastRayInput input, out PhysicsShape chainSegmentShape)
	{
		PhysicsChain_CastRay_Injected(ref chain, ref input, out chainSegmentShape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::CastShape", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult PhysicsChain_CastShape(PhysicsChain chain, PhysicsQuery.CastShapeInput input, out PhysicsShape chainSegmentShape)
	{
		PhysicsChain_CastShape_Injected(ref chain, ref input, out chainSegmentShape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsChain::SetOwner", IsThreadSafe = true)]
	internal static int PhysicsChain_SetOwner(PhysicsChain chain, Object ownerObject)
	{
		return PhysicsChain_SetOwner_Injected(ref chain, Object.MarshalledUnityObject.Marshal(ownerObject));
	}

	[NativeMethod(Name = "PhysicsChain::GetOwner", IsThreadSafe = true)]
	internal static Object PhysicsChain_GetOwner(PhysicsChain chain)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(PhysicsChain_GetOwner_Injected(ref chain));
	}

	[NativeMethod(Name = "PhysicsChain::IsOwned", IsThreadSafe = true)]
	internal static bool PhysicsChain_IsOwned(PhysicsChain chain)
	{
		return PhysicsChain_IsOwned_Injected(ref chain);
	}

	[NativeMethod(Name = "PhysicsChain::SetCallbackTarget", IsThreadSafe = true)]
	internal static void PhysicsChain_SetCallbackTarget(PhysicsChain chain, object callbackTarget)
	{
		PhysicsChain_SetCallbackTarget_Injected(ref chain, callbackTarget);
	}

	[NativeMethod(Name = "PhysicsChain::GetCallbackTarget", IsThreadSafe = true)]
	internal static object PhysicsChain_GetCallbackTarget(PhysicsChain chain)
	{
		return PhysicsChain_GetCallbackTarget_Injected(ref chain);
	}

	[NativeMethod(Name = "PhysicsChain::SetUserData", IsThreadSafe = true)]
	internal static void PhysicsChain_SetUserData(PhysicsChain chain, PhysicsUserData physicsUserData)
	{
		PhysicsChain_SetUserData_Injected(ref chain, ref physicsUserData);
	}

	[NativeMethod(Name = "PhysicsChain::GetUserData", IsThreadSafe = true)]
	internal static PhysicsUserData PhysicsChain_GetUserData(PhysicsChain chain)
	{
		PhysicsChain_GetUserData_Injected(ref chain, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CircleGeometry::IsValid", IsThreadSafe = true)]
	internal static bool CircleGeometry_IsValid(CircleGeometry geometry)
	{
		return CircleGeometry_IsValid_Injected(ref geometry);
	}

	[NativeMethod(Name = "CircleGeometry::CalculateMass", IsThreadSafe = true)]
	internal static PhysicsBody.MassConfiguration CircleGeometry_CalculateMassConfiguration(CircleGeometry geometry, float density)
	{
		CircleGeometry_CalculateMassConfiguration_Injected(ref geometry, density, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CircleGeometry::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB CircleGeometry_CalculateAABB(CircleGeometry geometry, PhysicsTransform transform)
	{
		CircleGeometry_CalculateAABB_Injected(ref geometry, ref transform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CircleGeometry::OverlapPoint", IsThreadSafe = true)]
	internal static bool CircleGeometry_OverlapPoint(CircleGeometry geometry, Vector2 point)
	{
		return CircleGeometry_OverlapPoint_Injected(ref geometry, ref point);
	}

	[NativeMethod(Name = "CircleGeometry::ClosestPoint", IsThreadSafe = true)]
	internal static Vector2 CircleGeometry_ClosestPoint(CircleGeometry geometry, Vector2 point)
	{
		CircleGeometry_ClosestPoint_Injected(ref geometry, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CircleGeometry::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult CircleGeometry_CastRay(CircleGeometry geometry, PhysicsQuery.CastRayInput input)
	{
		CircleGeometry_CastRay_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CircleGeometry::CastShape", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult CircleGeometry_CastShape(CircleGeometry geometry, PhysicsQuery.CastShapeInput input)
	{
		CircleGeometry_CastShape_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CapsuleGeometry::IsValid", IsThreadSafe = true)]
	internal static bool CapsuleGeometry_IsValid(CapsuleGeometry geometry)
	{
		return CapsuleGeometry_IsValid_Injected(ref geometry);
	}

	[NativeMethod(Name = "CapsuleGeometry::CalculateMass", IsThreadSafe = true)]
	internal static PhysicsBody.MassConfiguration CapsuleGeometry_CalculateMassConfiguration(CapsuleGeometry geometry, float density)
	{
		CapsuleGeometry_CalculateMassConfiguration_Injected(ref geometry, density, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CapsuleGeometry::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB CapsuleGeometry_CalculateAABB(CapsuleGeometry geometry, PhysicsTransform transform)
	{
		CapsuleGeometry_CalculateAABB_Injected(ref geometry, ref transform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CapsuleGeometry::OverlapPoint", IsThreadSafe = true)]
	internal static bool CapsuleGeometry_OverlapPoint(CapsuleGeometry geometry, Vector2 point)
	{
		return CapsuleGeometry_OverlapPoint_Injected(ref geometry, ref point);
	}

	[NativeMethod(Name = "CapsuleGeometry::ClosestPoint", IsThreadSafe = true)]
	internal static Vector2 CapsuleGeometry_ClosestPoint(CapsuleGeometry geometry, Vector2 point)
	{
		CapsuleGeometry_ClosestPoint_Injected(ref geometry, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CapsuleGeometry::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult CapsuleGeometry_CastRay(CapsuleGeometry geometry, PhysicsQuery.CastRayInput input)
	{
		CapsuleGeometry_CastRay_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "CapsuleGeometry::CastShape", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult CapsuleGeometry_CastShape(CapsuleGeometry geometry, PhysicsQuery.CastShapeInput input)
	{
		CapsuleGeometry_CastShape_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::CreateBox", IsThreadSafe = true)]
	internal static PolygonGeometry PolygonGeometry_CreateBox(Vector2 size, float radius, PhysicsTransform transform, bool inscribe)
	{
		PolygonGeometry_CreateBox_Injected(ref size, radius, ref transform, inscribe, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::CreatePolygons", IsThreadSafe = true)]
	internal unsafe static PhysicsBuffer PolygonGeometry_CreatePolygons(ReadOnlySpan<Vector2> vertices, PhysicsTransform transform, Vector2 vertexScale, Allocator allocator)
	{
		ReadOnlySpan<Vector2> readOnlySpan = vertices;
		PhysicsBuffer ret;
		fixed (Vector2* begin = readOnlySpan)
		{
			ManagedSpanWrapper vertices2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PolygonGeometry_CreatePolygons_Injected(ref vertices2, ref transform, ref vertexScale, allocator, out ret);
		}
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::Create_WithPhysicsTransform", IsThreadSafe = true)]
	internal unsafe static PolygonGeometry PolygonGeometry_Create_WithPhysicsTransform(ReadOnlySpan<Vector2> vertices, float radius, PhysicsTransform transform)
	{
		ReadOnlySpan<Vector2> readOnlySpan = vertices;
		PolygonGeometry ret;
		fixed (Vector2* begin = readOnlySpan)
		{
			ManagedSpanWrapper vertices2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PolygonGeometry_Create_WithPhysicsTransform_Injected(ref vertices2, radius, ref transform, out ret);
		}
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::Create_WithMatrix", IsThreadSafe = true)]
	internal unsafe static PolygonGeometry PolygonGeometry_Create_WithMatrix(ReadOnlySpan<Vector2> vertices, float radius, Matrix4x4 transform)
	{
		ReadOnlySpan<Vector2> readOnlySpan = vertices;
		PolygonGeometry ret;
		fixed (Vector2* begin = readOnlySpan)
		{
			ManagedSpanWrapper vertices2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PolygonGeometry_Create_WithMatrix_Injected(ref vertices2, radius, ref transform, out ret);
		}
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::Transform_WithPhysicsTransform", IsThreadSafe = true)]
	internal static PolygonGeometry PolygonGeometry_Transform_WithPhysicsTransform(PolygonGeometry geometry, PhysicsTransform transform)
	{
		PolygonGeometry_Transform_WithPhysicsTransform_Injected(ref geometry, ref transform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::InverseTransform_WithPhysicsTransform", IsThreadSafe = true)]
	internal static PolygonGeometry PolygonGeometry_InverseTransform_WithPhysicsTransform(PolygonGeometry geometry, PhysicsTransform transform)
	{
		PolygonGeometry_InverseTransform_WithPhysicsTransform_Injected(ref geometry, ref transform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::Transform_WithMatrix", IsThreadSafe = true)]
	internal static PolygonGeometry PolygonGeometry_Transform_WithMatrix(PolygonGeometry geometry, Matrix4x4 transform, bool scaleRadius)
	{
		PolygonGeometry_Transform_WithMatrix_Injected(ref geometry, ref transform, scaleRadius, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::InverseTransform_WithMatrix", IsThreadSafe = true)]
	internal static PolygonGeometry PolygonGeometry_InverseTransform_WithMatrix(PolygonGeometry geometry, Matrix4x4 transform, bool scaleRadius)
	{
		PolygonGeometry_InverseTransform_WithMatrix_Injected(ref geometry, ref transform, scaleRadius, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::IsValid", IsThreadSafe = true)]
	internal static bool PolygonGeometry_IsValid(PolygonGeometry geometry)
	{
		return PolygonGeometry_IsValid_Injected(ref geometry);
	}

	[NativeMethod(Name = "PolygonGeometry::Validate", IsThreadSafe = true)]
	internal static PolygonGeometry PolygonGeometry_Validate(PolygonGeometry geometry)
	{
		PolygonGeometry_Validate_Injected(ref geometry, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::CalculateMass", IsThreadSafe = true)]
	internal static PhysicsBody.MassConfiguration PolygonGeometry_CalculateMassConfiguration(PolygonGeometry geometry, float density)
	{
		PolygonGeometry_CalculateMassConfiguration_Injected(ref geometry, density, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB PolygonGeometry_CalculateAABB(PolygonGeometry geometry, PhysicsTransform transform)
	{
		PolygonGeometry_CalculateAABB_Injected(ref geometry, ref transform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::OverlapPoint", IsThreadSafe = true)]
	internal static bool PolygonGeometry_OverlapPoint(PolygonGeometry geometry, Vector2 point)
	{
		return PolygonGeometry_OverlapPoint_Injected(ref geometry, ref point);
	}

	[NativeMethod(Name = "PolygonGeometry::ClosestPoint", IsThreadSafe = true)]
	internal static Vector2 PolygonGeometry_ClosestPoint(PolygonGeometry geometry, Vector2 point)
	{
		PolygonGeometry_ClosestPoint_Injected(ref geometry, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult PolygonGeometry_CastRay(PolygonGeometry geometry, PhysicsQuery.CastRayInput input)
	{
		PolygonGeometry_CastRay_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PolygonGeometry::CastShape", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult PolygonGeometry_CastShape(PolygonGeometry geometry, PhysicsQuery.CastShapeInput input)
	{
		PolygonGeometry_CastShape_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "SegmentGeometry::IsValid", IsThreadSafe = true)]
	internal static bool SegmentGeometry_IsValid(SegmentGeometry geometry)
	{
		return SegmentGeometry_IsValid_Injected(ref geometry);
	}

	[NativeMethod(Name = "SegmentGeometry::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB SegmentGeometry_CalculateAABB(SegmentGeometry geometry, PhysicsTransform transform)
	{
		SegmentGeometry_CalculateAABB_Injected(ref geometry, ref transform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "SegmentGeometry::ClosestPoint", IsThreadSafe = true)]
	internal static Vector2 SegmentGeometry_ClosestPoint(SegmentGeometry geometry, Vector2 point)
	{
		SegmentGeometry_ClosestPoint_Injected(ref geometry, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "SegmentGeometry::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult SegmentGeometry_CastRay(SegmentGeometry geometry, PhysicsQuery.CastRayInput input, bool oneSided)
	{
		SegmentGeometry_CastRay_Injected(ref geometry, ref input, oneSided, out var ret);
		return ret;
	}

	[NativeMethod(Name = "SegmentGeometry::CastShape", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult SegmentGeometry_CastShape(SegmentGeometry geometry, PhysicsQuery.CastShapeInput input)
	{
		SegmentGeometry_CastShape_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "ChainSegmentGeometry::IsValid", IsThreadSafe = true)]
	internal static bool ChainSegmentGeometry_IsValid(ChainSegmentGeometry geometry)
	{
		return ChainSegmentGeometry_IsValid_Injected(ref geometry);
	}

	[NativeMethod(Name = "ChainSegmentGeometry::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB ChainSegmentGeometry_CalculateAABB(ChainSegmentGeometry geometry, PhysicsTransform transform)
	{
		ChainSegmentGeometry_CalculateAABB_Injected(ref geometry, ref transform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "ChainSegmentGeometry::ClosestPoint", IsThreadSafe = true)]
	internal static Vector2 ChainSegmentGeometry_ClosestPoint(ChainSegmentGeometry geometry, Vector2 point)
	{
		ChainSegmentGeometry_ClosestPoint_Injected(ref geometry, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "ChainSegmentGeometry::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult ChainSegmentGeometry_CastRay(ChainSegmentGeometry geometry, PhysicsQuery.CastRayInput input, bool oneSided)
	{
		ChainSegmentGeometry_CastRay_Injected(ref geometry, ref input, oneSided, out var ret);
		return ret;
	}

	[NativeMethod(Name = "ChainSegmentGeometry::CastShape", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult ChainSegmentGeometry_CastShape(ChainSegmentGeometry geometry, PhysicsQuery.CastShapeInput input)
	{
		ChainSegmentGeometry_CastShape_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "ChainGeometry::IsValid", IsThreadSafe = true)]
	internal static bool ChainGeometry_IsValid(ChainGeometry geometry)
	{
		return ChainGeometry_IsValid_Injected(ref geometry);
	}

	[NativeMethod(Name = "ChainGeometry::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB ChainGeometry_CalculateAABB(ChainGeometry geometry, PhysicsTransform transform)
	{
		ChainGeometry_CalculateAABB_Injected(ref geometry, ref transform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "ChainGeometry::ClosestPoint", IsThreadSafe = true)]
	internal static Vector2 ChainGeometry_ClosestPoint(ChainGeometry geometry, Vector2 point)
	{
		ChainGeometry_ClosestPoint_Injected(ref geometry, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "ChainGeometry::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult ChainGeometry_CastRay(ChainGeometry geometry, PhysicsQuery.CastRayInput input, bool oneSided)
	{
		ChainGeometry_CastRay_Injected(ref geometry, ref input, oneSided, out var ret);
		return ret;
	}

	[NativeMethod(Name = "ChainGeometry::CastShape", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult ChainGeometry_CastShape(ChainGeometry geometry, PhysicsQuery.CastShapeInput input)
	{
		ChainGeometry_CastShape_Injected(ref geometry, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsJoint::Destroy")]
	internal static bool PhysicsJoint_Destroy(PhysicsJoint joint, int ownerKey)
	{
		return PhysicsJoint_Destroy_Injected(ref joint, ownerKey);
	}

	[NativeMethod(Name = "PhysicsJoint::DestroyBatch")]
	internal unsafe static void PhysicsJoint_DestroyBatch(ReadOnlySpan<PhysicsJoint> joints)
	{
		ReadOnlySpan<PhysicsJoint> readOnlySpan = joints;
		fixed (PhysicsJoint* begin = readOnlySpan)
		{
			ManagedSpanWrapper joints2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsJoint_DestroyBatch_Injected(ref joints2);
		}
	}

	[NativeMethod(Name = "PhysicsJoint::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsJoint_IsValid(PhysicsJoint joint)
	{
		return PhysicsJoint_IsValid_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::GetWorld", IsThreadSafe = true)]
	internal static PhysicsWorld PhysicsJoint_GetWorld(PhysicsJoint joint)
	{
		PhysicsJoint_GetWorld_Injected(ref joint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsJoint::GetJointType", IsThreadSafe = true)]
	internal static PhysicsJoint.JointType PhysicsJoint_GetJointType(PhysicsJoint joint)
	{
		return PhysicsJoint_GetJointType_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::GetBodyA", IsThreadSafe = true)]
	internal static PhysicsBody PhysicsJoint_GetBodyA(PhysicsJoint joint)
	{
		PhysicsJoint_GetBodyA_Injected(ref joint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsJoint::GetBodyB", IsThreadSafe = true)]
	internal static PhysicsBody PhysicsJoint_GetBodyB(PhysicsJoint joint)
	{
		PhysicsJoint_GetBodyB_Injected(ref joint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsJoint::SetLocalAnchorA", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetLocalAnchorA(PhysicsJoint joint, PhysicsTransform localAnchor)
	{
		PhysicsJoint_SetLocalAnchorA_Injected(ref joint, ref localAnchor);
	}

	[NativeMethod(Name = "PhysicsJoint::GetLocalAnchorA", IsThreadSafe = true)]
	internal static PhysicsTransform PhysicsJoint_GetLocalAnchorA(PhysicsJoint joint)
	{
		PhysicsJoint_GetLocalAnchorA_Injected(ref joint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsJoint::SetLocalAnchorB", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetLocalAnchorB(PhysicsJoint joint, PhysicsTransform localAnchor)
	{
		PhysicsJoint_SetLocalAnchorB_Injected(ref joint, ref localAnchor);
	}

	[NativeMethod(Name = "PhysicsJoint::GetLocalAnchorB", IsThreadSafe = true)]
	internal static PhysicsTransform PhysicsJoint_GetLocalAnchorB(PhysicsJoint joint)
	{
		PhysicsJoint_GetLocalAnchorB_Injected(ref joint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsJoint::SetForceThreshold", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetForceThreshold(PhysicsJoint joint, float forceThreshold)
	{
		PhysicsJoint_SetForceThreshold_Injected(ref joint, forceThreshold);
	}

	[NativeMethod(Name = "PhysicsJoint::GetForceThreshold", IsThreadSafe = true)]
	internal static float PhysicsJoint_GetForceThreshold(PhysicsJoint joint)
	{
		return PhysicsJoint_GetForceThreshold_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::SetTorqueThreshold", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetTorqueThreshold(PhysicsJoint joint, float torqueThreshold)
	{
		PhysicsJoint_SetTorqueThreshold_Injected(ref joint, torqueThreshold);
	}

	[NativeMethod(Name = "PhysicsJoint::GetTorqueThreshold", IsThreadSafe = true)]
	internal static float PhysicsJoint_GetTorqueThreshold(PhysicsJoint joint)
	{
		return PhysicsJoint_GetTorqueThreshold_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::SetCollideConnected", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetCollideConnected(PhysicsJoint joint, bool shouldCollide)
	{
		PhysicsJoint_SetCollideConnected_Injected(ref joint, shouldCollide);
	}

	[NativeMethod(Name = "PhysicsJoint::GetCollideConnected", IsThreadSafe = true)]
	internal static bool PhysicsJoint_GetCollideConnected(PhysicsJoint joint)
	{
		return PhysicsJoint_GetCollideConnected_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::SetTuningFrequency", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetTuningFrequency(PhysicsJoint joint, float tuningFrequency)
	{
		PhysicsJoint_SetTuningFrequency_Injected(ref joint, tuningFrequency);
	}

	[NativeMethod(Name = "PhysicsJoint::GetTuningFrequency", IsThreadSafe = true)]
	internal static float PhysicsJoint_GetTuningFrequency(PhysicsJoint joint)
	{
		return PhysicsJoint_GetTuningFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::SetTuningDamping", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetTuningDamping(PhysicsJoint joint, float tuningDamping)
	{
		PhysicsJoint_SetTuningDamping_Injected(ref joint, tuningDamping);
	}

	[NativeMethod(Name = "PhysicsJoint::GetTuningDamping", IsThreadSafe = true)]
	internal static float PhysicsJoint_GetTuningDamping(PhysicsJoint joint)
	{
		return PhysicsJoint_GetTuningDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::SetDrawScale", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetDrawScale(PhysicsJoint joint, float drawScale)
	{
		PhysicsJoint_SetDrawScale_Injected(ref joint, drawScale);
	}

	[NativeMethod(Name = "PhysicsJoint::GetDrawScale", IsThreadSafe = true)]
	internal static float PhysicsJoint_GetDrawScale(PhysicsJoint joint)
	{
		return PhysicsJoint_GetDrawScale_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::WakeBodies", IsThreadSafe = true)]
	internal static void PhysicsJoint_WakeBodies(PhysicsJoint joint)
	{
		PhysicsJoint_WakeBodies_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::GetCurrentConstraintForce", IsThreadSafe = true)]
	internal static Vector2 PhysicsJoint_GetCurrentConstraintForce(PhysicsJoint joint)
	{
		PhysicsJoint_GetCurrentConstraintForce_Injected(ref joint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsJoint::GetCurrentConstraintTorque", IsThreadSafe = true)]
	internal static float PhysicsJoint_GetCurrentConstraintTorque(PhysicsJoint joint)
	{
		return PhysicsJoint_GetCurrentConstraintTorque_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::GetCurrentLinearSeparation", IsThreadSafe = true)]
	internal static float PhysicsJoint_GetCurrentLinearSeparation(PhysicsJoint joint)
	{
		return PhysicsJoint_GetCurrentLinearSeparation_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::GetCurrentAngularSeparation", IsThreadSafe = true)]
	internal static float PhysicsJoint_GetCurrentAngularSeparation(PhysicsJoint joint)
	{
		return PhysicsJoint_GetCurrentAngularSeparation_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::Draw", IsThreadSafe = true)]
	internal static void PhysicsJoint_Draw(PhysicsJoint joint)
	{
		PhysicsJoint_Draw_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::SetOwner", IsThreadSafe = true)]
	internal static int PhysicsJoint_SetOwner(PhysicsJoint joint, Object ownerObject)
	{
		return PhysicsJoint_SetOwner_Injected(ref joint, Object.MarshalledUnityObject.Marshal(ownerObject));
	}

	[NativeMethod(Name = "PhysicsJoint::GetOwner", IsThreadSafe = true)]
	internal static Object PhysicsJoint_GetOwner(PhysicsJoint joint)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(PhysicsJoint_GetOwner_Injected(ref joint));
	}

	[NativeMethod(Name = "PhysicsJoint::IsOwned", IsThreadSafe = true)]
	internal static bool PhysicsJoint_IsOwned(PhysicsJoint joint)
	{
		return PhysicsJoint_IsOwned_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::SetCallbackTarget", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetCallbackTarget(PhysicsJoint joint, object callbackTarget)
	{
		PhysicsJoint_SetCallbackTarget_Injected(ref joint, callbackTarget);
	}

	[NativeMethod(Name = "PhysicsJoint::GetCallbackTarget", IsThreadSafe = true)]
	internal static object PhysicsJoint_GetCallbackTarget(PhysicsJoint joint)
	{
		return PhysicsJoint_GetCallbackTarget_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsJoint::SetUserData", IsThreadSafe = true)]
	internal static void PhysicsJoint_SetUserData(PhysicsJoint joint, PhysicsUserData physicsUserData)
	{
		PhysicsJoint_SetUserData_Injected(ref joint, ref physicsUserData);
	}

	[NativeMethod(Name = "PhysicsJoint::GetUserData", IsThreadSafe = true)]
	internal static PhysicsUserData PhysicsJoint_GetUserData(PhysicsJoint joint)
	{
		PhysicsJoint_GetUserData_Injected(ref joint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsDistanceJointDefinition DistanceJoint_GetDefaultDefinition(bool useSettings)
	{
		DistanceJoint_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::Create")]
	internal static PhysicsDistanceJoint DistanceJoint_Create(PhysicsWorld world, PhysicsDistanceJointDefinition definition)
	{
		DistanceJoint_Create_Injected(ref world, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetDistance", IsThreadSafe = true)]
	internal static void DistanceJoint_SetDistance(PhysicsDistanceJoint joint, float distance)
	{
		DistanceJoint_SetDistance_Injected(ref joint, distance);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetDistance", IsThreadSafe = true)]
	internal static float DistanceJoint_GetDistance(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetDistance_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetCurrentDistance", IsThreadSafe = true)]
	internal static float DistanceJoint_GetCurrentDistance(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetCurrentDistance_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetEnableSpring", IsThreadSafe = true)]
	internal static void DistanceJoint_SetEnableSpring(PhysicsDistanceJoint joint, bool enableSpring)
	{
		DistanceJoint_SetEnableSpring_Injected(ref joint, enableSpring);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetEnableSpring", IsThreadSafe = true)]
	internal static bool DistanceJoint_GetEnableSpring(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetEnableSpring_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetSpringLowerForce", IsThreadSafe = true)]
	internal static void DistanceJoint_SetSpringLowerForce(PhysicsDistanceJoint joint, float springLowerForce)
	{
		DistanceJoint_SetSpringLowerForce_Injected(ref joint, springLowerForce);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetSpringLowerForce", IsThreadSafe = true)]
	internal static float DistanceJoint_GetSpringLowerForce(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetSpringLowerForce_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetSpringUpperForce", IsThreadSafe = true)]
	internal static void DistanceJoint_SetSpringUpperForce(PhysicsDistanceJoint joint, float springUpperForce)
	{
		DistanceJoint_SetSpringUpperForce_Injected(ref joint, springUpperForce);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetSpringUpperForce", IsThreadSafe = true)]
	internal static float DistanceJoint_GetSpringUpperForce(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetSpringUpperForce_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetSpringFrequency", IsThreadSafe = true)]
	internal static void DistanceJoint_SetSpringFrequency(PhysicsDistanceJoint joint, float springFrequency)
	{
		DistanceJoint_SetSpringFrequency_Injected(ref joint, springFrequency);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetSpringFrequency", IsThreadSafe = true)]
	internal static float DistanceJoint_GetSpringFrequency(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetSpringFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetSpringDamping", IsThreadSafe = true)]
	internal static void DistanceJoint_SetSpringDamping(PhysicsDistanceJoint joint, float springDamping)
	{
		DistanceJoint_SetSpringDamping_Injected(ref joint, springDamping);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetSpringDamping", IsThreadSafe = true)]
	internal static float DistanceJoint_GetSpringDamping(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetSpringDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetEnableMotor", IsThreadSafe = true)]
	internal static void DistanceJoint_SetEnableMotor(PhysicsDistanceJoint joint, bool enableMotor)
	{
		DistanceJoint_SetEnableMotor_Injected(ref joint, enableMotor);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetEnableMotor", IsThreadSafe = true)]
	internal static bool DistanceJoint_GetEnableMotor(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetEnableMotor_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetMotorSpeed", IsThreadSafe = true)]
	internal static void DistanceJoint_SetMotorSpeed(PhysicsDistanceJoint joint, float motorSpeed)
	{
		DistanceJoint_SetMotorSpeed_Injected(ref joint, motorSpeed);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetMotorSpeed", IsThreadSafe = true)]
	internal static float DistanceJoint_GetMotorSpeed(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetMotorSpeed_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetMaxMotorForce", IsThreadSafe = true)]
	internal static void DistanceJoint_SetMaxMotorForce(PhysicsDistanceJoint joint, float maxMotorForce)
	{
		DistanceJoint_SetMaxMotorForce_Injected(ref joint, maxMotorForce);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetMaxMotorForce", IsThreadSafe = true)]
	internal static float DistanceJoint_GetMaxMotorForce(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetMaxMotorForce_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetCurrentMotorForce", IsThreadSafe = true)]
	internal static float DistanceJoint_GetCurrentMotorForce(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetCurrentMotorForce_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetEnableLimit", IsThreadSafe = true)]
	internal static void DistanceJoint_SetEnableLimit(PhysicsDistanceJoint joint, bool enableLimit)
	{
		DistanceJoint_SetEnableLimit_Injected(ref joint, enableLimit);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetEnableLimit", IsThreadSafe = true)]
	internal static bool DistanceJoint_GetEnableLimit(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetEnableLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetMinDistanceLimit", IsThreadSafe = true)]
	internal static void DistanceJoint_SetMinDistanceLimit(PhysicsDistanceJoint joint, float minDistanceLimit)
	{
		DistanceJoint_SetMinDistanceLimit_Injected(ref joint, minDistanceLimit);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetMinDistanceLimit", IsThreadSafe = true)]
	internal static float DistanceJoint_GetMinDistanceLimit(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetMinDistanceLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::SetMaxDistanceLimit", IsThreadSafe = true)]
	internal static void DistanceJoint_SetMaxDistanceLimit(PhysicsDistanceJoint joint, float maxDistanceLimit)
	{
		DistanceJoint_SetMaxDistanceLimit_Injected(ref joint, maxDistanceLimit);
	}

	[NativeMethod(Name = "PhysicsDistanceJoint::GetMaxDistanceLimit", IsThreadSafe = true)]
	internal static float DistanceJoint_GetMaxDistanceLimit(PhysicsDistanceJoint joint)
	{
		return DistanceJoint_GetMaxDistanceLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsRelativeJointDefinition RelativeJoint_GetDefaultDefinition(bool useSettings)
	{
		RelativeJoint_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::Create")]
	internal static PhysicsRelativeJoint RelativeJoint_Create(PhysicsWorld world, PhysicsRelativeJointDefinition definition)
	{
		RelativeJoint_Create_Injected(ref world, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetLinearVelocity", IsThreadSafe = true)]
	internal static void RelativeJoint_SetLinearVelocity(PhysicsRelativeJoint joint, Vector2 linearVelocity)
	{
		RelativeJoint_SetLinearVelocity_Injected(ref joint, ref linearVelocity);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetLinearVelocity", IsThreadSafe = true)]
	internal static Vector2 RelativeJoint_GetLinearVelocity(PhysicsRelativeJoint joint)
	{
		RelativeJoint_GetLinearVelocity_Injected(ref joint, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetAngularVelocity", IsThreadSafe = true)]
	internal static void RelativeJoint_SetAngularVelocity(PhysicsRelativeJoint joint, float angularVelocity)
	{
		RelativeJoint_SetAngularVelocity_Injected(ref joint, angularVelocity);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetAngularVelocity", IsThreadSafe = true)]
	internal static float RelativeJoint_GetAngularVelocity(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetAngularVelocity_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetMaxForce", IsThreadSafe = true)]
	internal static void RelativeJoint_SetMaxForce(PhysicsRelativeJoint joint, float maxForce)
	{
		RelativeJoint_SetMaxForce_Injected(ref joint, maxForce);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetMaxForce", IsThreadSafe = true)]
	internal static float RelativeJoint_GetMaxForce(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetMaxForce_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetMaxTorque", IsThreadSafe = true)]
	internal static void RelativeJoint_SetMaxTorque(PhysicsRelativeJoint joint, float maxTorque)
	{
		RelativeJoint_SetMaxTorque_Injected(ref joint, maxTorque);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetMaxTorque", IsThreadSafe = true)]
	internal static float RelativeJoint_GetMaxTorque(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetMaxTorque_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetSpringLinearFrequency", IsThreadSafe = true)]
	internal static void RelativeJoint_SetSpringLinearFrequency(PhysicsRelativeJoint joint, float springLinearFrequency)
	{
		RelativeJoint_SetSpringLinearFrequency_Injected(ref joint, springLinearFrequency);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetSpringLinearFrequency", IsThreadSafe = true)]
	internal static float RelativeJoint_GetSpringLinearFrequency(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetSpringLinearFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetSpringAngularFrequency", IsThreadSafe = true)]
	internal static void RelativeJoint_SetSpringAngularFrequency(PhysicsRelativeJoint joint, float springAngularFrequency)
	{
		RelativeJoint_SetSpringAngularFrequency_Injected(ref joint, springAngularFrequency);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetSpringAngularFrequency", IsThreadSafe = true)]
	internal static float RelativeJoint_GetSpringAngularFrequency(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetSpringAngularFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetSpringLinearDamping", IsThreadSafe = true)]
	internal static void RelativeJoint_SetSpringLinearDamping(PhysicsRelativeJoint joint, float springLinearDamping)
	{
		RelativeJoint_SetSpringLinearDamping_Injected(ref joint, springLinearDamping);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetSpringLinearDamping", IsThreadSafe = true)]
	internal static float RelativeJoint_GetSpringLinearDamping(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetSpringLinearDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetSpringAngularDamping", IsThreadSafe = true)]
	internal static void RelativeJoint_SetSpringAngularDamping(PhysicsRelativeJoint joint, float springAngularDamping)
	{
		RelativeJoint_SetSpringAngularDamping_Injected(ref joint, springAngularDamping);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetSpringAngularDamping", IsThreadSafe = true)]
	internal static float RelativeJoint_GetSpringAngularDamping(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetSpringAngularDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetSpringMaxForce", IsThreadSafe = true)]
	internal static void RelativeJoint_SetSpringMaxForce(PhysicsRelativeJoint joint, float springMaxForce)
	{
		RelativeJoint_SetSpringMaxForce_Injected(ref joint, springMaxForce);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetSpringMaxForce", IsThreadSafe = true)]
	internal static float RelativeJoint_GetSpringMaxForce(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetSpringMaxForce_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::SetSpringMaxTorque", IsThreadSafe = true)]
	internal static void RelativeJoint_SetSpringMaxTorque(PhysicsRelativeJoint joint, float springMaxTorque)
	{
		RelativeJoint_SetSpringMaxTorque_Injected(ref joint, springMaxTorque);
	}

	[NativeMethod(Name = "PhysicsRelativeJoint::GetSpringMaxTorque", IsThreadSafe = true)]
	internal static float RelativeJoint_GetSpringMaxTorque(PhysicsRelativeJoint joint)
	{
		return RelativeJoint_GetSpringMaxTorque_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsIgnoreJoint::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsIgnoreJointDefinition IgnorePhysicsJoint_GetDefaultDefinition()
	{
		IgnorePhysicsJoint_GetDefaultDefinition_Injected(out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsIgnoreJoint::Create")]
	internal static PhysicsIgnoreJoint IgnorePhysicsJoint_Create(PhysicsWorld world, PhysicsIgnoreJointDefinition definition)
	{
		IgnorePhysicsJoint_Create_Injected(ref world, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsSliderJointDefinition SliderJoint_GetDefaultDefinition(bool useSettings)
	{
		SliderJoint_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsSliderJoint::Create")]
	internal static PhysicsSliderJoint SliderJoint_Create(PhysicsWorld world, PhysicsSliderJointDefinition definition)
	{
		SliderJoint_Create_Injected(ref world, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetEnableSpring", IsThreadSafe = true)]
	internal static void SliderJoint_SetEnableSpring(PhysicsSliderJoint joint, bool enableSpring)
	{
		SliderJoint_SetEnableSpring_Injected(ref joint, enableSpring);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetEnableSpring", IsThreadSafe = true)]
	internal static bool SliderJoint_GetEnableSpring(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetEnableSpring_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetSpringFrequency", IsThreadSafe = true)]
	internal static void SliderJoint_SetSpringFrequency(PhysicsSliderJoint joint, float springFrequency)
	{
		SliderJoint_SetSpringFrequency_Injected(ref joint, springFrequency);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetSpringFrequency", IsThreadSafe = true)]
	internal static float SliderJoint_GetSpringFrequency(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetSpringFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetSpringDamping", IsThreadSafe = true)]
	internal static void SliderJoint_SetSpringDamping(PhysicsSliderJoint joint, float damping)
	{
		SliderJoint_SetSpringDamping_Injected(ref joint, damping);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetSpringDamping", IsThreadSafe = true)]
	internal static float SliderJoint_GetSpringDamping(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetSpringDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetSpringTargetTranslation", IsThreadSafe = true)]
	internal static void SliderJoint_SetSpringTargetTranslation(PhysicsSliderJoint joint, float targetTranslation)
	{
		SliderJoint_SetSpringTargetTranslation_Injected(ref joint, targetTranslation);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetSpringTargetTranslation", IsThreadSafe = true)]
	internal static float SliderJoint_GetSpringTargetTranslation(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetSpringTargetTranslation_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetEnableMotor", IsThreadSafe = true)]
	internal static void SliderJoint_SetEnableMotor(PhysicsSliderJoint joint, bool enableMotor)
	{
		SliderJoint_SetEnableMotor_Injected(ref joint, enableMotor);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetEnableMotor", IsThreadSafe = true)]
	internal static bool SliderJoint_GetEnableMotor(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetEnableMotor_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetMotorSpeed", IsThreadSafe = true)]
	internal static void SliderJoint_SetMotorSpeed(PhysicsSliderJoint joint, float motorSpeed)
	{
		SliderJoint_SetMotorSpeed_Injected(ref joint, motorSpeed);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetMotorSpeed", IsThreadSafe = true)]
	internal static float SliderJoint_GetMotorSpeed(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetMotorSpeed_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetMaxMotorForce", IsThreadSafe = true)]
	internal static void SliderJoint_SetMaxMotorForce(PhysicsSliderJoint joint, float force)
	{
		SliderJoint_SetMaxMotorForce_Injected(ref joint, force);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetMaxMotorForce", IsThreadSafe = true)]
	internal static float SliderJoint_GetMaxMotorForce(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetMaxMotorForce_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetCurrentMotorForce", IsThreadSafe = true)]
	internal static float SliderJoint_GetCurrentMotorForce(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetCurrentMotorForce_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetCurrentTranslation", IsThreadSafe = true)]
	internal static float SliderJoint_GetCurrentTranslation(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetCurrentTranslation_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetCurrentSpeed", IsThreadSafe = true)]
	internal static float SliderJoint_GetCurrentSpeed(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetCurrentSpeed_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetEnableLimit", IsThreadSafe = true)]
	internal static void SliderJoint_SetEnableLimit(PhysicsSliderJoint joint, bool enableLimit)
	{
		SliderJoint_SetEnableLimit_Injected(ref joint, enableLimit);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetEnableLimit", IsThreadSafe = true)]
	internal static bool SliderJoint_GetEnableLimit(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetEnableLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetLowerTranslationLimit", IsThreadSafe = true)]
	internal static void SliderJoint_SetLowerTranslationLimit(PhysicsSliderJoint joint, float lowerTranslationLimit)
	{
		SliderJoint_SetLowerTranslationLimit_Injected(ref joint, lowerTranslationLimit);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetLowerTranslationLimit", IsThreadSafe = true)]
	internal static float SliderJoint_GetLowerTranslationLimit(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetLowerTranslationLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::SetUpperTranslationLimit", IsThreadSafe = true)]
	internal static void SliderJoint_SetUpperTranslationLimit(PhysicsSliderJoint joint, float upperTranslationLimit)
	{
		SliderJoint_SetUpperTranslationLimit_Injected(ref joint, upperTranslationLimit);
	}

	[NativeMethod(Name = "PhysicsSliderJoint::GetUpperTranslationLimit", IsThreadSafe = true)]
	internal static float SliderJoint_GetUpperTranslationLimit(PhysicsSliderJoint joint)
	{
		return SliderJoint_GetUpperTranslationLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsHingeJointDefinition HingeJoint_GetDefaultDefinition(bool useSettings)
	{
		HingeJoint_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsHingeJoint::Create")]
	internal static PhysicsHingeJoint HingeJoint_Create(PhysicsWorld world, PhysicsHingeJointDefinition definition)
	{
		HingeJoint_Create_Injected(ref world, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetEnableSpring", IsThreadSafe = true)]
	internal static void HingeJoint_SetEnableSpring(PhysicsHingeJoint joint, bool enableSpring)
	{
		HingeJoint_SetEnableSpring_Injected(ref joint, enableSpring);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetEnableSpring", IsThreadSafe = true)]
	internal static bool HingeJoint_GetEnableSpring(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetEnableSpring_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetSpringFrequency", IsThreadSafe = true)]
	internal static void HingeJoint_SetSpringFrequency(PhysicsHingeJoint joint, float springFrequency)
	{
		HingeJoint_SetSpringFrequency_Injected(ref joint, springFrequency);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetSpringFrequency", IsThreadSafe = true)]
	internal static float HingeJoint_GetSpringFrequency(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetSpringFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetSpringDamping", IsThreadSafe = true)]
	internal static void HingeJoint_SetSpringDamping(PhysicsHingeJoint joint, float damping)
	{
		HingeJoint_SetSpringDamping_Injected(ref joint, damping);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetSpringDamping", IsThreadSafe = true)]
	internal static float HingeJoint_GetSpringDamping(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetSpringDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetSpringTargetAngle", IsThreadSafe = true)]
	internal static void HingeJoint_SetSpringTargetAngle(PhysicsHingeJoint joint, float targetAngle)
	{
		HingeJoint_SetSpringTargetAngle_Injected(ref joint, targetAngle);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetSpringTargetAngle", IsThreadSafe = true)]
	internal static float HingeJoint_GetSpringTargetAngle(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetSpringTargetAngle_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetAngle", IsThreadSafe = true)]
	internal static float HingeJoint_GetAngle(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetAngle_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetEnableMotor", IsThreadSafe = true)]
	internal static void HingeJoint_SetEnableMotor(PhysicsHingeJoint joint, bool enableMotor)
	{
		HingeJoint_SetEnableMotor_Injected(ref joint, enableMotor);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetEnableMotor", IsThreadSafe = true)]
	internal static bool HingeJoint_GetEnableMotor(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetEnableMotor_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetMotorSpeed", IsThreadSafe = true)]
	internal static void HingeJoint_SetMotorSpeed(PhysicsHingeJoint joint, float motorSpeed)
	{
		HingeJoint_SetMotorSpeed_Injected(ref joint, motorSpeed);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetMotorSpeed", IsThreadSafe = true)]
	internal static float HingeJoint_GetMotorSpeed(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetMotorSpeed_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetMaxMotorTorque", IsThreadSafe = true)]
	internal static void HingeJoint_SetMaxMotorTorque(PhysicsHingeJoint joint, float torque)
	{
		HingeJoint_SetMaxMotorTorque_Injected(ref joint, torque);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetMaxMotorTorque", IsThreadSafe = true)]
	internal static float HingeJoint_GetMaxMotorTorque(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetMaxMotorTorque_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetCurrentMotorTorque", IsThreadSafe = true)]
	internal static float HingeJoint_GetCurrentMotorTorque(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetCurrentMotorTorque_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetEnableLimit", IsThreadSafe = true)]
	internal static void HingeJoint_SetEnableLimit(PhysicsHingeJoint joint, bool enableLimit)
	{
		HingeJoint_SetEnableLimit_Injected(ref joint, enableLimit);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetEnableLimit", IsThreadSafe = true)]
	internal static bool HingeJoint_GetEnableLimit(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetEnableLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetLowerAngleLimit", IsThreadSafe = true)]
	internal static void HingeJoint_SetLowerLimit(PhysicsHingeJoint joint, float lowerAngleLimit)
	{
		HingeJoint_SetLowerLimit_Injected(ref joint, lowerAngleLimit);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetLowerAngleLimit", IsThreadSafe = true)]
	internal static float HingeJoint_GetLowerLimit(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetLowerLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::SetUpperAngleLimit", IsThreadSafe = true)]
	internal static void HingeJoint_SetUpperLimit(PhysicsHingeJoint joint, float upperAngleLimit)
	{
		HingeJoint_SetUpperLimit_Injected(ref joint, upperAngleLimit);
	}

	[NativeMethod(Name = "PhysicsHingeJoint::GetUpperAngleLimit", IsThreadSafe = true)]
	internal static float HingeJoint_GetUpperLimit(PhysicsHingeJoint joint)
	{
		return HingeJoint_GetUpperLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsFixedJoint::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsFixedJointDefinition FixedJoint_GetDefaultDefinition(bool useSettings)
	{
		FixedJoint_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsFixedJoint::Create")]
	internal static PhysicsFixedJoint FixedJoint_Create(PhysicsWorld world, PhysicsFixedJointDefinition definition)
	{
		FixedJoint_Create_Injected(ref world, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsFixedJoint::SetLinearFrequency", IsThreadSafe = true)]
	internal static void FixedJoint_SetLinearFrequency(PhysicsFixedJoint joint, float linearFrequency)
	{
		FixedJoint_SetLinearFrequency_Injected(ref joint, linearFrequency);
	}

	[NativeMethod(Name = "PhysicsFixedJoint::GetLinearFrequency", IsThreadSafe = true)]
	internal static float FixedJoint_GetLinearFrequency(PhysicsFixedJoint joint)
	{
		return FixedJoint_GetLinearFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsFixedJoint::SetLinearDamping", IsThreadSafe = true)]
	internal static void FixedJoint_SetLinearDamping(PhysicsFixedJoint joint, float damping)
	{
		FixedJoint_SetLinearDamping_Injected(ref joint, damping);
	}

	[NativeMethod(Name = "PhysicsFixedJoint::GetLinearDamping", IsThreadSafe = true)]
	internal static float FixedJoint_GetLinearDamping(PhysicsFixedJoint joint)
	{
		return FixedJoint_GetLinearDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsFixedJoint::SetAngularFrequency", IsThreadSafe = true)]
	internal static void FixedJoint_SetAngularFrequency(PhysicsFixedJoint joint, float angularFrequency)
	{
		FixedJoint_SetAngularFrequency_Injected(ref joint, angularFrequency);
	}

	[NativeMethod(Name = "PhysicsFixedJoint::GetAngularFrequency", IsThreadSafe = true)]
	internal static float FixedJoint_GetAngularFrequency(PhysicsFixedJoint joint)
	{
		return FixedJoint_GetAngularFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsFixedJoint::SetAngularDamping", IsThreadSafe = true)]
	internal static void FixedJoint_SetAngularDamping(PhysicsFixedJoint joint, float damping)
	{
		FixedJoint_SetAngularDamping_Injected(ref joint, damping);
	}

	[NativeMethod(Name = "PhysicsFixedJoint::GetAngularDamping", IsThreadSafe = true)]
	internal static float FixedJoint_GetAngularDamping(PhysicsFixedJoint joint)
	{
		return FixedJoint_GetAngularDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsWheelJointDefinition WheelJoint_GetDefaultDefinition(bool useSettings)
	{
		WheelJoint_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWheelJoint::Create")]
	internal static PhysicsWheelJoint WheelJoint_Create(PhysicsWorld world, PhysicsWheelJointDefinition definition)
	{
		WheelJoint_Create_Injected(ref world, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetEnableSpring", IsThreadSafe = true)]
	internal static void WheelJoint_SetEnableSpring(PhysicsWheelJoint joint, bool enableSpring)
	{
		WheelJoint_SetEnableSpring_Injected(ref joint, enableSpring);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetEnableSpring", IsThreadSafe = true)]
	internal static bool WheelJoint_GetEnableSpring(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetEnableSpring_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetSpringFrequency", IsThreadSafe = true)]
	internal static void WheelJoint_SetSpringFrequency(PhysicsWheelJoint joint, float springFrequency)
	{
		WheelJoint_SetSpringFrequency_Injected(ref joint, springFrequency);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetSpringFrequency", IsThreadSafe = true)]
	internal static float WheelJoint_GetSpringFrequency(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetSpringFrequency_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetSpringDamping", IsThreadSafe = true)]
	internal static void WheelJoint_SetSpringDamping(PhysicsWheelJoint joint, float damping)
	{
		WheelJoint_SetSpringDamping_Injected(ref joint, damping);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetSpringDamping", IsThreadSafe = true)]
	internal static float WheelJoint_GetSpringDamping(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetSpringDamping_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetEnableMotor", IsThreadSafe = true)]
	internal static void WheelJoint_SetEnableMotor(PhysicsWheelJoint joint, bool enableMotor)
	{
		WheelJoint_SetEnableMotor_Injected(ref joint, enableMotor);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetEnableMotor", IsThreadSafe = true)]
	internal static bool WheelJoint_GetEnableMotor(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetEnableMotor_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetMotorSpeed", IsThreadSafe = true)]
	internal static void WheelJoint_SetMotorSpeed(PhysicsWheelJoint joint, float motorSpeed)
	{
		WheelJoint_SetMotorSpeed_Injected(ref joint, motorSpeed);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetMotorSpeed", IsThreadSafe = true)]
	internal static float WheelJoint_GetMotorSpeed(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetMotorSpeed_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetMaxMotorTorque", IsThreadSafe = true)]
	internal static void WheelJoint_SetMaxMotorTorque(PhysicsWheelJoint joint, float torque)
	{
		WheelJoint_SetMaxMotorTorque_Injected(ref joint, torque);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetMaxMotorTorque", IsThreadSafe = true)]
	internal static float WheelJoint_GetMaxMotorTorque(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetMaxMotorTorque_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetCurrentMotorTorque", IsThreadSafe = true)]
	internal static float WheelJoint_GetCurrentMotorTorque(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetCurrentMotorTorque_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetEnableLimit", IsThreadSafe = true)]
	internal static void WheelJoint_SetEnableLimit(PhysicsWheelJoint joint, bool enableLimit)
	{
		WheelJoint_SetEnableLimit_Injected(ref joint, enableLimit);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetEnableLimit", IsThreadSafe = true)]
	internal static bool WheelJoint_GetEnableLimit(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetEnableLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetLowerTranslationLimit", IsThreadSafe = true)]
	internal static void WheelJoint_SetLowerTranslationLimit(PhysicsWheelJoint joint, float lowerTranslationLimit)
	{
		WheelJoint_SetLowerTranslationLimit_Injected(ref joint, lowerTranslationLimit);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetLowerTranslationLimit", IsThreadSafe = true)]
	internal static float WheelJoint_GetLowerTranslationLimit(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetLowerTranslationLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::SetUpperTranslationLimit", IsThreadSafe = true)]
	internal static void WheelJoint_SetUpperTranslationLimit(PhysicsWheelJoint joint, float upperTranslationLimit)
	{
		WheelJoint_SetUpperTranslationLimit_Injected(ref joint, upperTranslationLimit);
	}

	[NativeMethod(Name = "PhysicsWheelJoint::GetUpperTranslationLimit", IsThreadSafe = true)]
	internal static float WheelJoint_GetUpperTranslationLimit(PhysicsWheelJoint joint)
	{
		return WheelJoint_GetUpperTranslationLimit_Injected(ref joint);
	}

	[NativeMethod(Name = "PhysicsTransform::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsTransform_IsValid(PhysicsTransform transform)
	{
		return PhysicsTransform_IsValid_Injected(ref transform);
	}

	[NativeMethod(Name = "PhysicsTransform::TransformPoint", IsThreadSafe = true)]
	internal static Vector2 PhysicsTransform_TransformPoint(PhysicsTransform transform, Vector2 point)
	{
		PhysicsTransform_TransformPoint_Injected(ref transform, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsTransform::InverseTransformPoint", IsThreadSafe = true)]
	internal static Vector2 PhysicsTransform_InverseTransformPoint(PhysicsTransform transform, Vector2 point)
	{
		PhysicsTransform_InverseTransformPoint_Injected(ref transform, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsTransform::MultiplyTransform", IsThreadSafe = true)]
	internal static PhysicsTransform PhysicsTransform_MultiplyTransform(PhysicsTransform transform1, PhysicsTransform PhysicsTransform)
	{
		PhysicsTransform_MultiplyTransform_Injected(ref transform1, ref PhysicsTransform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsTransform::InverseMultiplyTransform", IsThreadSafe = true)]
	internal static PhysicsTransform PhysicsTransform_InverseMultiplyTransform(PhysicsTransform transform1, PhysicsTransform PhysicsTransform)
	{
		PhysicsTransform_InverseMultiplyTransform_Injected(ref transform1, ref PhysicsTransform, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::Create", IsThreadSafe = true)]
	internal static PhysicsRotate PhysicsRotate_CreateAngle(float angle)
	{
		PhysicsRotate_CreateAngle_Injected(angle, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsRotate_IsValid(PhysicsRotate rotation)
	{
		return PhysicsRotate_IsValid_Injected(ref rotation);
	}

	[NativeMethod(Name = "PhysicsRotate::GetAngle", IsThreadSafe = true)]
	internal static float PhysicsRotate_GetAngle(PhysicsRotate rotate)
	{
		return PhysicsRotate_GetAngle_Injected(ref rotate);
	}

	[NativeMethod(Name = "PhysicsRotate::GetRelativeAngle", IsThreadSafe = true)]
	internal static float PhysicsRotate_GetRelativeAngle(PhysicsRotate rotation1, PhysicsRotate rotation2)
	{
		return PhysicsRotate_GetRelativeAngle_Injected(ref rotation1, ref rotation2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsRotate::UnwindAngle", IsThreadSafe = true)]
	internal static extern float PhysicsRotate_UnwindAngle(float angle);

	[NativeMethod(Name = "PhysicsRotate::IntegrateRotation", IsThreadSafe = true)]
	internal static PhysicsRotate PhysicsRotate_IntegrateRotation(PhysicsRotate rotation, float deltaAngle)
	{
		PhysicsRotate_IntegrateRotation_Injected(ref rotation, deltaAngle, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::LerpRotation", IsThreadSafe = true)]
	internal static PhysicsRotate PhysicsRotate_LerpRotation(PhysicsRotate rotationA, PhysicsRotate rotationB, float interval)
	{
		PhysicsRotate_LerpRotation_Injected(ref rotationA, ref rotationB, interval, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::AngularVelocity", IsThreadSafe = true)]
	internal static float PhysicsRotate_AngularVelocity(PhysicsRotate rotationA, PhysicsRotate rotationB, float deltaTime)
	{
		return PhysicsRotate_AngularVelocity_Injected(ref rotationA, ref rotationB, deltaTime);
	}

	[NativeMethod(Name = "PhysicsRotate::Normalize", IsThreadSafe = true)]
	internal static PhysicsRotate PhysicsRotate_Normalize(in Vector2 direction)
	{
		PhysicsRotate_Normalize_Injected(in direction, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::IsNormalized", IsThreadSafe = true)]
	internal static bool PhysicsRotate_IsNormalized(PhysicsRotate rotation)
	{
		return PhysicsRotate_IsNormalized_Injected(ref rotation);
	}

	[NativeMethod(Name = "PhysicsRotate::MultiplyRotation", IsThreadSafe = true)]
	internal static PhysicsRotate PhysicsRotate_MultiplyRotation(PhysicsRotate rotation1, PhysicsRotate rotation2)
	{
		PhysicsRotate_MultiplyRotation_Injected(ref rotation1, ref rotation2, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::InverseMultiplyRotation", IsThreadSafe = true)]
	internal static PhysicsRotate PhysicsRotate_InverseMultiplyRotation(PhysicsRotate rotation1, PhysicsRotate rotation2)
	{
		PhysicsRotate_InverseMultiplyRotation_Injected(ref rotation1, ref rotation2, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::RotateVector", IsThreadSafe = true)]
	internal static Vector2 PhysicsRotate_RotateVector(PhysicsRotate rotation, Vector2 vector)
	{
		PhysicsRotate_RotateVector_Injected(ref rotation, ref vector, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::InverseRotateVector", IsThreadSafe = true)]
	internal static Vector2 PhysicsRotate_InverseRotateVector(PhysicsRotate rotation, Vector2 vector)
	{
		PhysicsRotate_InverseRotateVector_Injected(ref rotation, ref vector, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsRotate::Rotate", IsThreadSafe = true)]
	internal static PhysicsRotate PhysicsRotate_Rotate(PhysicsRotate rotation, float deltaAngle)
	{
		PhysicsRotate_Rotate_Injected(ref rotation, deltaAngle, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsAABB::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsAABB_IsValid(PhysicsAABB aabb)
	{
		return PhysicsAABB_IsValid_Injected(ref aabb);
	}

	[NativeMethod(Name = "PhysicsAABB::OverlapPoint", IsThreadSafe = true)]
	internal static bool PhysicsAABB_OverlapPoint(PhysicsAABB aabb, Vector2 point)
	{
		return PhysicsAABB_OverlapPoint_Injected(ref aabb, ref point);
	}

	[NativeMethod(Name = "PhysicsAABB::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult PhysicsAABB_CastRay(PhysicsAABB aabb, PhysicsQuery.CastRayInput input)
	{
		PhysicsAABB_CastRay_Injected(ref aabb, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsAABB::Overlap", IsThreadSafe = true)]
	internal static bool PhysicsAABB_Overlap(PhysicsAABB aabb1, PhysicsAABB aabb2)
	{
		return PhysicsAABB_Overlap_Injected(ref aabb1, ref aabb2);
	}

	[NativeMethod(Name = "PhysicsAABB::Union", IsThreadSafe = true)]
	internal static PhysicsAABB PhysicsAABB_Union(PhysicsAABB aabb1, PhysicsAABB aabb2)
	{
		PhysicsAABB_Union_Injected(ref aabb1, ref aabb2, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsAABB::Contains", IsThreadSafe = true)]
	internal static bool PhysicsAABB_Contains(PhysicsAABB aabb1, PhysicsAABB aabb2)
	{
		return PhysicsAABB_Contains_Injected(ref aabb1, ref aabb2);
	}

	[NativeMethod(Name = "PhysicsAABB::Center", IsThreadSafe = true)]
	internal static Vector2 PhysicsAABB_Center(PhysicsAABB aabb)
	{
		PhysicsAABB_Center_Injected(ref aabb, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsAABB::Extents", IsThreadSafe = true)]
	internal static Vector2 PhysicsAABB_Extents(PhysicsAABB aabb)
	{
		PhysicsAABB_Extents_Injected(ref aabb, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsAABB::Perimeter", IsThreadSafe = true)]
	internal static float PhysicsAABB_Perimeter(PhysicsAABB aabb)
	{
		return PhysicsAABB_Perimeter_Injected(ref aabb);
	}

	[NativeMethod(Name = "PhysicsPlane::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsPlane_IsValid(PhysicsPlane plane)
	{
		return PhysicsPlane_IsValid_Injected(ref plane);
	}

	[NativeMethod(Name = "PhysicsPlane::GetSeparation", IsThreadSafe = true)]
	internal static float PhysicsPlane_GetSeparation(PhysicsPlane plane, Vector2 point)
	{
		return PhysicsPlane_GetSeparation_Injected(ref plane, ref point);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsMath::PI", IsThreadSafe = true)]
	internal static extern float PhysicsMath_PI();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsMath::TAU", IsThreadSafe = true)]
	internal static extern float PhysicsMath_TAU();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsMath::ToDegrees", IsThreadSafe = true)]
	internal static extern float PhysicsMath_ToDegrees(float radians);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsMath::ToRadians", IsThreadSafe = true)]
	internal static extern float PhysicsMath_ToRadians(float degrees);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsMath::Atan2", IsThreadSafe = true)]
	internal static extern float PhysicsMath_Atan2(float y, float x);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsMath::CosSin", IsThreadSafe = true)]
	internal static extern void PhysicsMath_CosSin(float angle, out float cos, out float sin);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsMath::SpringDamper", IsThreadSafe = true)]
	internal static extern float PhysicsMath_SpringDamper(float frequency, float damping, float translation, float speed, float deltaTime);

	[NativeMethod(Name = "PhysicsUserData::GetObject", IsThreadSafe = true)]
	internal static Object PhysicsUserData_GetObject(EntityId entityId)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(PhysicsUserData_GetObject_Injected(ref entityId));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysicsLowLevel2D()", StaticAccessorType.Arrow)]
	[NativeMethod(Name = "PhysicsLowLevel2D::SetSafetyLocksEnabled")]
	internal static extern void PhysicsGlobal_SetSafetyLocksEnabled(bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsLowLevel2D::GetSafetyLocksEnabled")]
	[StaticAccessor("GetPhysicsLowLevel2D()", StaticAccessorType.Arrow)]
	internal static extern bool PhysicsGlobal_GetSafetyLocksEnabled();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsLowLevel2D::GetBypassLowLevel")]
	[StaticAccessor("GetPhysicsLowLevel2D()", StaticAccessorType.Arrow)]
	internal static extern bool PhysicsGlobal_GetBypassLowLevel();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysicsLowLevel2D()", StaticAccessorType.Arrow)]
	[NativeMethod(Name = "PhysicsLowLevel2D::IsRenderingAllowed")]
	internal static extern bool PhysicsGlobal_IsRenderingAllowed();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysicsLowLevel2D()", StaticAccessorType.Arrow)]
	[NativeMethod(Name = "PhysicsLowLevel2D::GetConcurrentSimulations")]
	internal static extern int PhysicsGlobal_GetConcurrentSimulations();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsLowLevel2D::GetLengthUnitsPerMeter")]
	internal static extern float PhysicsGlobal_GetLengthUnitsPerMeter();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsLowLevel2D::GetPhysicsLayerNames")]
	[StaticAccessor("GetPhysicsLowLevel2D()", StaticAccessorType.Arrow)]
	internal static extern object PhysicsGlobal_GetPhysicsLayers();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetPhysicsLowLevel2D()", StaticAccessorType.Arrow)]
	[NativeMethod(Name = "PhysicsLowLevel2D::GetUseFullLayers")]
	internal static extern bool PhysicsGlobal_GetUseFullLayers();

	[NativeMethod(Name = "PhysicsWorldManager2D::PopulateWorldTransformWrite")]
	[StaticAccessor("GetPhysicsLowLevel2D()->GetWorldManager2D()", StaticAccessorType.Arrow)]
	internal unsafe static int PhysicsGlobal_PopulateWorldTransformWrite(PhysicsWorld world, IntPtr transformAccessArrayIntPtr, Span<PhysicsBody.TransformWriteTween> transformWriteTweensArray)
	{
		Span<PhysicsBody.TransformWriteTween> span = transformWriteTweensArray;
		int result;
		fixed (PhysicsBody.TransformWriteTween* begin = span)
		{
			ManagedSpanWrapper transformWriteTweensArray2 = new ManagedSpanWrapper(begin, span.Length);
			result = PhysicsGlobal_PopulateWorldTransformWrite_Injected(ref world, transformAccessArrayIntPtr, ref transformWriteTweensArray2);
		}
		return result;
	}

	[NativeMethod(Name = "PhysicsQuery::ShapeAndShape", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_ShapeAndShape(PhysicsShape shapeA, PhysicsTransform transformA, PhysicsShape shapeB, PhysicsTransform transformB)
	{
		PhysicsQuery_ShapeAndShape_Injected(ref shapeA, ref transformA, ref shapeB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::CircleAndCircle", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_CircleAndCircle(CircleGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_CircleAndCircle_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::CapsuleAndCircle", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_CapsuleAndCircle(CapsuleGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_CapsuleAndCircle_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::SegmentAndCircle", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_SegmentAndCircle(SegmentGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_SegmentAndCircle_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::PolygonAndCircle", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_PolygonAndCircle(PolygonGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_PolygonAndCircle_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::CapsuleAndCapsule", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_CapsuleAndCapsule(CapsuleGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_CapsuleAndCapsule_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::SegmentAndCapsule", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_SegmentAndCapsule(SegmentGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_SegmentAndCapsule_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::PolygonAndCapsule", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_PolygonAndCapsule(PolygonGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_PolygonAndCapsule_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::PolygonAndPolygon", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_PolygonAndPolygon(PolygonGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_PolygonAndPolygon_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::SegmentAndPolygon", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_SegmentAndPolygon(SegmentGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_SegmentAndPolygon_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::ChainSegmentAndCircle", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_ChainSegmentAndCircle(ChainSegmentGeometry geometryA, PhysicsTransform transformA, CircleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_ChainSegmentAndCircle_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::ChainSegmentAndCapsule", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_ChainSegmentAndCapsule(ChainSegmentGeometry geometryA, PhysicsTransform transformA, CapsuleGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_ChainSegmentAndCapsule_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::ChainSegmentAndPolygon", IsThreadSafe = true)]
	internal static PhysicsShape.ContactManifold PhysicsQuery_ChainSegmentAndPolygon(ChainSegmentGeometry geometryA, PhysicsTransform transformA, PolygonGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_ChainSegmentAndPolygon_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::CastShapes", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult PhysicsQuery_CastShapes(PhysicsQuery.CastShapePairInput input)
	{
		PhysicsQuery_CastShapes_Injected(ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::SegmentDistance", IsThreadSafe = true)]
	internal static PhysicsQuery.SegmentDistanceResult PhysicsQuery_SegmentDistance(SegmentGeometry geometryA, PhysicsTransform transformA, SegmentGeometry geometryB, PhysicsTransform transformB)
	{
		PhysicsQuery_SegmentDistance_Injected(ref geometryA, ref transformA, ref geometryB, ref transformB, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::ShapeDistance", IsThreadSafe = true)]
	internal static PhysicsQuery.DistanceResult PhysicsQuery_ShapeDistance(PhysicsQuery.DistanceInput distanceInput)
	{
		PhysicsQuery_ShapeDistance_Injected(ref distanceInput, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsQuery::ShapeTimeOfImpact", IsThreadSafe = true)]
	internal static PhysicsQuery.TimeOfImpactResult PhysicsQuery_ShapeTimeOfImpact(PhysicsQuery.TimeOfImpactInput toiInput)
	{
		PhysicsQuery_ShapeTimeOfImpact_Injected(ref toiInput, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsShapeDefinition PhysicsShape_GetDefaultDefinition(bool useSettings)
	{
		PhysicsShape_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetDefaultSurfaceMaterial", IsThreadSafe = true)]
	internal static PhysicsShape.SurfaceMaterial PhysicsShape_GetDefaultSurfaceMaterial()
	{
		PhysicsShape_GetDefaultSurfaceMaterial_Injected(out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CreateCircleShape")]
	internal static PhysicsShape PhysicsShape_CreateCircleShape(PhysicsBody body, CircleGeometry geometry, PhysicsShapeDefinition definition)
	{
		PhysicsShape_CreateCircleShape_Injected(ref body, ref geometry, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CreatePolygonShape")]
	internal static PhysicsShape PhysicsShape_CreatePolygonShape(PhysicsBody body, PolygonGeometry geometry, PhysicsShapeDefinition definition)
	{
		PhysicsShape_CreatePolygonShape_Injected(ref body, ref geometry, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CreateCapsuleShape")]
	internal static PhysicsShape PhysicsShape_CreateCapsuleShape(PhysicsBody body, CapsuleGeometry geometry, PhysicsShapeDefinition definition)
	{
		PhysicsShape_CreateCapsuleShape_Injected(ref body, ref geometry, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CreateSegmentShape")]
	internal static PhysicsShape PhysicsShape_CreateSegmentShape(PhysicsBody body, SegmentGeometry geometry, PhysicsShapeDefinition definition)
	{
		PhysicsShape_CreateSegmentShape_Injected(ref body, ref geometry, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CreateChainSegmentShape")]
	internal static PhysicsShape PhysicsShape_CreateChainSegmenShapet(PhysicsBody body, ChainSegmentGeometry geometry, PhysicsShapeDefinition definition)
	{
		PhysicsShape_CreateChainSegmenShapet_Injected(ref body, ref geometry, ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CreateShapeBatch")]
	internal static PhysicsBuffer PhysicsShape_CreateShapeBatch(PhysicsBody body, PhysicsBuffer spanGeometry, PhysicsShape.ShapeType shapeType, PhysicsShapeDefinition definition, Allocator allocator)
	{
		PhysicsShape_CreateShapeBatch_Injected(ref body, ref spanGeometry, shapeType, ref definition, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::Destroy")]
	internal static bool PhysicsShape_Destroy(PhysicsShape shape, bool updateBodyMass, int ownerKey)
	{
		return PhysicsShape_Destroy_Injected(ref shape, updateBodyMass, ownerKey);
	}

	[NativeMethod(Name = "PhysicsShape::DestroyBatch")]
	internal unsafe static void PhysicsShape_DestroyBatch(ReadOnlySpan<PhysicsShape> shapes, bool updateBodyMass)
	{
		ReadOnlySpan<PhysicsShape> readOnlySpan = shapes;
		fixed (PhysicsShape* begin = readOnlySpan)
		{
			ManagedSpanWrapper shapes2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsShape_DestroyBatch_Injected(ref shapes2, updateBodyMass);
		}
	}

	[NativeMethod(Name = "PhysicsShape::WriteDefinition")]
	internal static void PhysicsShape_WriteDefinition(PhysicsShape shape, PhysicsShapeDefinition definition, bool onlyExtendedProperties)
	{
		PhysicsShape_WriteDefinition_Injected(ref shape, ref definition, onlyExtendedProperties);
	}

	[NativeMethod(Name = "PhysicsShape::ReadDefinition")]
	internal static PhysicsShapeDefinition PhysicsShape_ReadDefinition(PhysicsShape shape)
	{
		PhysicsShape_ReadDefinition_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsShape_IsValid(PhysicsShape shape)
	{
		return PhysicsShape_IsValid_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::GetWorld", IsThreadSafe = true)]
	internal static PhysicsWorld PhysicsShape_GetWorld(PhysicsShape shape)
	{
		PhysicsShape_GetWorld_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetBody", IsThreadSafe = true)]
	internal static PhysicsBody PhysicsShape_GetBody(PhysicsShape shape)
	{
		PhysicsShape_GetBody_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::SetIsTrigger", IsThreadSafe = true)]
	internal static void PhysicsShape_SetIsTrigger(PhysicsShape shape, bool flag)
	{
		PhysicsShape_SetIsTrigger_Injected(ref shape, flag);
	}

	[NativeMethod(Name = "PhysicsShape::GetIsTrigger", IsThreadSafe = true)]
	internal static bool PhysicsShape_GetIsTrigger(PhysicsShape shape)
	{
		return PhysicsShape_GetIsTrigger_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::GetShapeType", IsThreadSafe = true)]
	internal static PhysicsShape.ShapeType PhysicsShape_GetShapeType(PhysicsShape shape)
	{
		return PhysicsShape_GetShapeType_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetDensity", IsThreadSafe = true)]
	internal static void PhysicsShape_SetDensity(PhysicsShape shape, float density, bool updateBodyMass)
	{
		PhysicsShape_SetDensity_Injected(ref shape, density, updateBodyMass);
	}

	[NativeMethod(Name = "PhysicsShape::GetDensity", IsThreadSafe = true)]
	internal static float PhysicsShape_GetDensity(PhysicsShape shape)
	{
		return PhysicsShape_GetDensity_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::GetMassData", IsThreadSafe = true)]
	internal static PhysicsBody.MassConfiguration PhysicsShape_GetMassConfiguration(PhysicsShape shape)
	{
		PhysicsShape_GetMassConfiguration_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::SetFriction", IsThreadSafe = true)]
	internal static void PhysicsShape_SetFriction(PhysicsShape shape, float friction)
	{
		PhysicsShape_SetFriction_Injected(ref shape, friction);
	}

	[NativeMethod(Name = "PhysicsShape::GetFriction", IsThreadSafe = true)]
	internal static float PhysicsShape_GetFriction(PhysicsShape shape)
	{
		return PhysicsShape_GetFriction_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetBounciness", IsThreadSafe = true)]
	internal static void PhysicsShape_SetBounciness(PhysicsShape shape, float bounciness)
	{
		PhysicsShape_SetBounciness_Injected(ref shape, bounciness);
	}

	[NativeMethod(Name = "PhysicsShape::GetBounciness", IsThreadSafe = true)]
	internal static float PhysicsShape_GetBounciness(PhysicsShape shape)
	{
		return PhysicsShape_GetBounciness_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetFrictionMixing", IsThreadSafe = true)]
	internal static void PhysicsShape_SetFrictionMixing(PhysicsShape shape, PhysicsShape.SurfaceMaterial.MixingMode frictionMixing)
	{
		PhysicsShape_SetFrictionMixing_Injected(ref shape, frictionMixing);
	}

	[NativeMethod(Name = "PhysicsShape::GetFrictionMixing", IsThreadSafe = true)]
	internal static PhysicsShape.SurfaceMaterial.MixingMode PhysicsShape_GetFrictionMixing(PhysicsShape shape)
	{
		return PhysicsShape_GetFrictionMixing_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetBouncinessMixing", IsThreadSafe = true)]
	internal static void PhysicsShape_SetBouncinessMixing(PhysicsShape shape, PhysicsShape.SurfaceMaterial.MixingMode bouncinessMixing)
	{
		PhysicsShape_SetBouncinessMixing_Injected(ref shape, bouncinessMixing);
	}

	[NativeMethod(Name = "PhysicsShape::GetBouncinessMixing", IsThreadSafe = true)]
	internal static PhysicsShape.SurfaceMaterial.MixingMode PhysicsShape_GetBouncinessMixing(PhysicsShape shape)
	{
		return PhysicsShape_GetBouncinessMixing_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetFrictionPriority", IsThreadSafe = true)]
	internal static void PhysicsShape_SetFrictionPriority(PhysicsShape shape, ushort frictionPriority)
	{
		PhysicsShape_SetFrictionPriority_Injected(ref shape, frictionPriority);
	}

	[NativeMethod(Name = "PhysicsShape::GetFrictionPriority", IsThreadSafe = true)]
	internal static ushort PhysicsShape_GetFrictionPriority(PhysicsShape shape)
	{
		return PhysicsShape_GetFrictionPriority_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetBouncinessPriority", IsThreadSafe = true)]
	internal static void PhysicsShape_SetBouncinessPriority(PhysicsShape shape, ushort bouncinessPriority)
	{
		PhysicsShape_SetBouncinessPriority_Injected(ref shape, bouncinessPriority);
	}

	[NativeMethod(Name = "PhysicsShape::GetBouncinessPriority", IsThreadSafe = true)]
	internal static ushort PhysicsShape_GetBouncinessPriority(PhysicsShape shape)
	{
		return PhysicsShape_GetBouncinessPriority_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetRollingResistance", IsThreadSafe = true)]
	internal static void PhysicsShape_SetRollingResistance(PhysicsShape shape, float rollingResistance)
	{
		PhysicsShape_SetRollingResistance_Injected(ref shape, rollingResistance);
	}

	[NativeMethod(Name = "PhysicsShape::GetRollingResistance", IsThreadSafe = true)]
	internal static float PhysicsShape_GetRollingResistance(PhysicsShape shape)
	{
		return PhysicsShape_GetRollingResistance_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetTangentSpeed", IsThreadSafe = true)]
	internal static void PhysicsShape_SetTangentSpeed(PhysicsShape shape, float tangentSpeed)
	{
		PhysicsShape_SetTangentSpeed_Injected(ref shape, tangentSpeed);
	}

	[NativeMethod(Name = "PhysicsShape::GetTangentSpeed", IsThreadSafe = true)]
	internal static float PhysicsShape_GetTangentSpeed(PhysicsShape shape)
	{
		return PhysicsShape_GetTangentSpeed_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetCustomColor", IsThreadSafe = true)]
	internal static void PhysicsShape_SetCustomColor(PhysicsShape shape, Color32 customColor)
	{
		PhysicsShape_SetCustomColor_Injected(ref shape, ref customColor);
	}

	[NativeMethod(Name = "PhysicsShape::GetCustomColor", IsThreadSafe = true)]
	internal static Color32 PhysicsShape_GetCustomColor(PhysicsShape shape)
	{
		PhysicsShape_GetCustomColor_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::SetSurfaceMaterial", IsThreadSafe = true)]
	internal static void PhysicsShape_SetSurfaceMaterial(PhysicsShape shape, PhysicsShape.SurfaceMaterial surfaceMateria)
	{
		PhysicsShape_SetSurfaceMaterial_Injected(ref shape, ref surfaceMateria);
	}

	[NativeMethod(Name = "PhysicsShape::GetSurfaceMaterial", IsThreadSafe = true)]
	internal static PhysicsShape.SurfaceMaterial PhysicsShape_GetSurfaceMaterial(PhysicsShape shape)
	{
		PhysicsShape_GetSurfaceMaterial_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::SetContactFilter", IsThreadSafe = true)]
	internal static void PhysicsShape_SetContactFilter(PhysicsShape shape, PhysicsShape.ContactFilter filter)
	{
		PhysicsShape_SetContactFilter_Injected(ref shape, ref filter);
	}

	[NativeMethod(Name = "PhysicsShape::GetContactFilter", IsThreadSafe = true)]
	internal static PhysicsShape.ContactFilter PhysicsShape_GetContactFilter(PhysicsShape shape)
	{
		PhysicsShape_GetContactFilter_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::SetMoverData", IsThreadSafe = true)]
	internal static void PhysicsShape_SetMoverData(PhysicsShape shape, PhysicsShape.MoverData moverData)
	{
		PhysicsShape_SetMoverData_Injected(ref shape, ref moverData);
	}

	[NativeMethod(Name = "PhysicsShape::GetMoverData", IsThreadSafe = true)]
	internal static PhysicsShape.MoverData PhysicsShape_GetMoverData(PhysicsShape shape)
	{
		PhysicsShape_GetMoverData_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::ApplyWind", IsThreadSafe = true)]
	internal static void PhysicsShape_ApplyWind(PhysicsShape shape, Vector2 force, float drag, float lift, bool wake)
	{
		PhysicsShape_ApplyWind_Injected(ref shape, ref force, drag, lift, wake);
	}

	[NativeMethod(Name = "PhysicsShape::SetTriggerEvents", IsThreadSafe = true)]
	internal static void PhysicsShape_SetTriggerEvents(PhysicsShape shape, bool enableContactEvents)
	{
		PhysicsShape_SetTriggerEvents_Injected(ref shape, enableContactEvents);
	}

	[NativeMethod(Name = "PhysicsShape::GetTriggerEvents", IsThreadSafe = true)]
	internal static bool PhysicsShape_GetTriggerEvents(PhysicsShape shape)
	{
		return PhysicsShape_GetTriggerEvents_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetContactEvents", IsThreadSafe = true)]
	internal static void PhysicsShape_SetContactEvents(PhysicsShape shape, bool enableContactEvents)
	{
		PhysicsShape_SetContactEvents_Injected(ref shape, enableContactEvents);
	}

	[NativeMethod(Name = "PhysicsShape::GetContactEvents", IsThreadSafe = true)]
	internal static bool PhysicsShape_GetContactEvents(PhysicsShape shape)
	{
		return PhysicsShape_GetContactEvents_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetHitEvents", IsThreadSafe = true)]
	internal static void PhysicsShape_SetHitEvents(PhysicsShape shape, bool enableHitEvents)
	{
		PhysicsShape_SetHitEvents_Injected(ref shape, enableHitEvents);
	}

	[NativeMethod(Name = "PhysicsShape::GetHitEvents", IsThreadSafe = true)]
	internal static bool PhysicsShape_GetHitEvents(PhysicsShape shape)
	{
		return PhysicsShape_GetHitEvents_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetContactFilterCallbacks", IsThreadSafe = true)]
	internal static void PhysicsShape_SetContacFiltertCallbacks(PhysicsShape shape, bool flag)
	{
		PhysicsShape_SetContacFiltertCallbacks_Injected(ref shape, flag);
	}

	[NativeMethod(Name = "PhysicsShape::GetContactFilterCallbacks", IsThreadSafe = true)]
	internal static bool PhysicsShape_GetContactFilterCallbacks(PhysicsShape shape)
	{
		return PhysicsShape_GetContactFilterCallbacks_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetPreSolveCallbacks", IsThreadSafe = true)]
	internal static void PhysicsShape_SetPreSolveCallbacks(PhysicsShape shape, bool flag)
	{
		PhysicsShape_SetPreSolveCallbacks_Injected(ref shape, flag);
	}

	[NativeMethod(Name = "PhysicsShape::GetPreSolveCallbacks", IsThreadSafe = true)]
	internal static bool PhysicsShape_GetPreSolveCallbacks(PhysicsShape shape)
	{
		return PhysicsShape_GetPreSolveCallbacks_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::GetStartStaticContacts", IsThreadSafe = true)]
	internal static bool PhysicsShape_GetStartStaticContacts(PhysicsShape shape)
	{
		return PhysicsShape_GetStartStaticContacts_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::GetStartMassUpdate", IsThreadSafe = true)]
	internal static bool PhysicsShape_GetStartMassUpdate(PhysicsShape shape)
	{
		return PhysicsShape_GetStartMassUpdate_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::OverlapPoint", IsThreadSafe = true)]
	internal static bool PhysicsShape_OverlapPoint(PhysicsShape shape, Vector2 point)
	{
		return PhysicsShape_OverlapPoint_Injected(ref shape, ref point);
	}

	[NativeMethod(Name = "PhysicsShape::ClosestPoint", IsThreadSafe = true)]
	internal static Vector2 PhysicsShape_ClosestPoint(PhysicsShape shape, Vector2 point)
	{
		PhysicsShape_ClosestPoint_Injected(ref shape, ref point, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CastRay", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult PhysicsShape_CastRay(PhysicsShape shape, PhysicsQuery.CastRayInput input)
	{
		PhysicsShape_CastRay_Injected(ref shape, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CastShape", IsThreadSafe = true)]
	internal static PhysicsQuery.CastResult PhysicsShape_CastShape(PhysicsShape shape, PhysicsQuery.CastShapeInput input)
	{
		PhysicsShape_CastShape_Injected(ref shape, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetCircleGeometry", IsThreadSafe = true)]
	internal static CircleGeometry PhysicsShape_GetCircleGeometry(PhysicsShape shape)
	{
		PhysicsShape_GetCircleGeometry_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetCapsuleGeometry", IsThreadSafe = true)]
	internal static CapsuleGeometry PhysicsShape_GetCapsuleGeometry(PhysicsShape shape)
	{
		PhysicsShape_GetCapsuleGeometry_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetPolygonGeometry", IsThreadSafe = true)]
	internal static PolygonGeometry PhysicsShape_GetPolygonGeometry(PhysicsShape shape)
	{
		PhysicsShape_GetPolygonGeometry_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetSegmentGeometry", IsThreadSafe = true)]
	internal static SegmentGeometry PhysicsShape_GetSegmentGeometry(PhysicsShape shape)
	{
		PhysicsShape_GetSegmentGeometry_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetChainSegmentGeometry", IsThreadSafe = true)]
	internal static ChainSegmentGeometry PhysicsShape_GetChainSegmentGeometry(PhysicsShape shape)
	{
		PhysicsShape_GetChainSegmentGeometry_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::SetCircleGeometry", IsThreadSafe = true)]
	internal static void PhysicsShape_SetCircleGeometry(PhysicsShape shape, CircleGeometry geometry)
	{
		PhysicsShape_SetCircleGeometry_Injected(ref shape, ref geometry);
	}

	[NativeMethod(Name = "PhysicsShape::SetCapsuleGeometry", IsThreadSafe = true)]
	internal static void PhysicsShape_SetCapsuleGeometry(PhysicsShape shape, CapsuleGeometry geometry)
	{
		PhysicsShape_SetCapsuleGeometry_Injected(ref shape, ref geometry);
	}

	[NativeMethod(Name = "PhysicsShape::SetPolygonGeometry", IsThreadSafe = true)]
	internal static void PhysicsShape_SetPolygonGeometry(PhysicsShape shape, PolygonGeometry geometry)
	{
		PhysicsShape_SetPolygonGeometry_Injected(ref shape, ref geometry);
	}

	[NativeMethod(Name = "PhysicsShape::SetSegmentGeometry", IsThreadSafe = true)]
	internal static void PhysicsShape_SetSegmentGeometry(PhysicsShape shape, SegmentGeometry geometry)
	{
		PhysicsShape_SetSegmentGeometry_Injected(ref shape, ref geometry);
	}

	[NativeMethod(Name = "PhysicsShape::IsChainSegmentShape", IsThreadSafe = true)]
	internal static bool PhysicsShape_IsChainSegmentShape(PhysicsShape shape)
	{
		return PhysicsShape_IsChainSegmentShape_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::GetChain", IsThreadSafe = true)]
	internal static PhysicsChain PhysicsShape_GetChain(PhysicsShape shape)
	{
		PhysicsShape_GetChain_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetContacts", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsShape_GetContacts(PhysicsShape shape, Allocator allocator)
	{
		PhysicsShape_GetContacts_Injected(ref shape, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetTriggerVisitors", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsShape_GetTriggerVisitors(PhysicsShape shape, Allocator allocator)
	{
		PhysicsShape_GetTriggerVisitors_Injected(ref shape, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::CalculateAABB", IsThreadSafe = true)]
	internal static PhysicsAABB PhysicsShape_CalculateAABB(PhysicsShape shape)
	{
		PhysicsShape_CalculateAABB_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetLocalCenter", IsThreadSafe = true)]
	internal static Vector2 PhysicsShape_GetLocalCenter(PhysicsShape shape)
	{
		PhysicsShape_GetLocalCenter_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsShape::GetPerimeter", IsThreadSafe = true)]
	internal static float PhysicsShape_GetPerimeter(PhysicsShape shape)
	{
		return PhysicsShape_GetPerimeter_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::GetPerimeterProjected", IsThreadSafe = true)]
	internal static float PhysicsShape_GetPerimeterProjected(PhysicsShape shape, Vector2 axis)
	{
		return PhysicsShape_GetPerimeterProjected_Injected(ref shape, ref axis);
	}

	[NativeMethod(Name = "PhysicsShape::Draw", IsThreadSafe = true)]
	internal static void PhysicsShape_Draw(PhysicsShape shape)
	{
		PhysicsShape_Draw_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetOwner", IsThreadSafe = true)]
	internal static int PhysicsShape_SetOwner(PhysicsShape shape, Object ownerObject)
	{
		return PhysicsShape_SetOwner_Injected(ref shape, Object.MarshalledUnityObject.Marshal(ownerObject));
	}

	[NativeMethod(Name = "PhysicsShape::GetOwner", IsThreadSafe = true)]
	internal static Object PhysicsShape_GetOwner(PhysicsShape shape)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(PhysicsShape_GetOwner_Injected(ref shape));
	}

	[NativeMethod(Name = "PhysicsShape::IsOwned", IsThreadSafe = true)]
	internal static bool PhysicsShape_IsOwned(PhysicsShape shape)
	{
		return PhysicsShape_IsOwned_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetCallbackTarget", IsThreadSafe = true)]
	internal static void PhysicsShape_SetCallbackTarget(PhysicsShape shape, object callbackTarget)
	{
		PhysicsShape_SetCallbackTarget_Injected(ref shape, callbackTarget);
	}

	[NativeMethod(Name = "PhysicsShape::GetCallbackTarget", IsThreadSafe = true)]
	internal static object PhysicsShape_GetCallbackTarget(PhysicsShape shape)
	{
		return PhysicsShape_GetCallbackTarget_Injected(ref shape);
	}

	[NativeMethod(Name = "PhysicsShape::SetUserData", IsThreadSafe = true)]
	internal static void PhysicsShape_SetUserData(PhysicsShape shape, PhysicsUserData physicsUserData)
	{
		PhysicsShape_SetUserData_Injected(ref shape, ref physicsUserData);
	}

	[NativeMethod(Name = "PhysicsShape::GetUserData", IsThreadSafe = true)]
	internal static PhysicsUserData PhysicsShape_GetUserData(PhysicsShape shape)
	{
		PhysicsShape_GetUserData_Injected(ref shape, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsLowLevel2D::PhysicsContactId::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsContactId_IsValid(PhysicsShape.ContactId contactId)
	{
		return PhysicsContactId_IsValid_Injected(ref contactId);
	}

	[NativeMethod(Name = "PhysicsLowLevel2D::PhysicsContactId::GetContact", IsThreadSafe = true)]
	internal static PhysicsShape.Contact PhysicsContactId_GetContact(PhysicsShape.ContactId contactId)
	{
		PhysicsContactId_GetContact_Injected(ref contactId, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetDefaultDefinition", IsThreadSafe = true)]
	internal static PhysicsWorldDefinition PhysicsWorld_GetDefaultDefinition(bool useSettings)
	{
		PhysicsWorld_GetDefaultDefinition_Injected(useSettings, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetDefaultExplosionDefinition", IsThreadSafe = true)]
	internal static PhysicsWorld.ExplosionDefinition PhysicsWorld_GetDefaultExplosionDefinition()
	{
		PhysicsWorld_GetDefaultExplosionDefinition_Injected(out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::Create")]
	internal static PhysicsWorld PhysicsWorld_Create(PhysicsWorldDefinition definition)
	{
		PhysicsWorld_Create_Injected(ref definition, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::Destroy")]
	internal static bool PhysicsWorld_Destroy(PhysicsWorld world, int ownerKey)
	{
		return PhysicsWorld_Destroy_Injected(ref world, ownerKey);
	}

	[NativeMethod(Name = "PhysicsWorld::WriteDefinition")]
	internal static void PhysicsWorld_WriteDefinition(PhysicsWorld world, PhysicsWorldDefinition definition, bool onlyExtendedProperties)
	{
		PhysicsWorld_WriteDefinition_Injected(ref world, ref definition, onlyExtendedProperties);
	}

	[NativeMethod(Name = "PhysicsWorld::ReadDefinition")]
	internal static PhysicsWorldDefinition PhysicsWorld_ReadDefinition(PhysicsWorld world)
	{
		PhysicsWorld_ReadDefinition_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::Reset")]
	internal static void PhysicsWorld_Reset(PhysicsWorld world)
	{
		PhysicsWorld_Reset_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::IsValid", IsThreadSafe = true)]
	internal static bool PhysicsWorld_IsValid(PhysicsWorld world)
	{
		return PhysicsWorld_IsValid_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::IsEmpty", IsThreadSafe = true)]
	internal static bool PhysicsWorld_IsEmpty(PhysicsWorld world)
	{
		return PhysicsWorld_IsEmpty_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetPaused", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetPaused(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetPaused_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetPaused", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetPaused(PhysicsWorld world)
	{
		return PhysicsWorld_GetPaused_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetSleepingAllowed")]
	internal static void PhysicsWorld_SetSleepingAllowed(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetSleepingAllowed_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetSleepingAllowed", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetSleepingAllowed(PhysicsWorld world)
	{
		return PhysicsWorld_GetSleepingAllowed_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetContinuousAllowed")]
	internal static void PhysicsWorld_SetContinuousAllowed(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetContinuousAllowed_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetContinuousAllowed", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetContinuousAllowed(PhysicsWorld world)
	{
		return PhysicsWorld_GetContinuousAllowed_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetWarmStartingAllowed")]
	internal static void PhysicsWorld_SetWarmStartingAllowed(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetWarmStartingAllowed_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetWarmStartingAllowed", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetWarmStartingAllowed(PhysicsWorld world)
	{
		return PhysicsWorld_GetWarmStartingAllowed_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetContactFilterCallbacks")]
	internal static void PhysicsWorld_SetContactFilterCallbacks(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetContactFilterCallbacks_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactFilterCallbacks", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetContactFilterCallbacks(PhysicsWorld world)
	{
		return PhysicsWorld_GetContactFilterCallbacks_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetPreSolveCallbacks")]
	internal static void PhysicsWorld_SetPreSolveCallbacks(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetPreSolveCallbacks_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetPreSolveCallbacks", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetPreSolveCallbacks(PhysicsWorld world)
	{
		return PhysicsWorld_GetPreSolveCallbacks_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetAutoBodyUpdateCallbacks")]
	internal static void PhysicsWorld_SetAutoBodyUpdateCallbacks(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetAutoBodyUpdateCallbacks_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetAutoBodyUpdateCallbacks", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetAutoBodyUpdateCallbacks(PhysicsWorld world)
	{
		return PhysicsWorld_GetAutoBodyUpdateCallbacks_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetAutoContactCallbacks")]
	internal static void PhysicsWorld_SetAutoContactCallbacks(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetAutoContactCallbacks_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetAutoContactCallbacks", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetAutoContactCallbacks(PhysicsWorld world)
	{
		return PhysicsWorld_GetAutoContactCallbacks_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetAutoTriggerCallbacks")]
	internal static void PhysicsWorld_SetAutoTriggerCallbacks(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetAutoTriggerCallbacks_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetAutoTriggerCallbacks", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetAutoTriggerCallbacks(PhysicsWorld world)
	{
		return PhysicsWorld_GetAutoTriggerCallbacks_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetAutoJointThresholdCallbacks")]
	internal static void PhysicsWorld_SetAutoJointThresholdCallbacks(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetAutoJointThresholdCallbacks_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetAutoJointThresholdCallbacks", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetAutoJointThresholdCallbacks(PhysicsWorld world)
	{
		return PhysicsWorld_GetAutoJointThresholdCallbacks_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetBounceThreshold")]
	internal static void PhysicsWorld_SetBounceThreshold(PhysicsWorld world, float value)
	{
		PhysicsWorld_SetBounceThreshold_Injected(ref world, value);
	}

	[NativeMethod(Name = "PhysicsWorld::GetBounceThreshold", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetBounceThreshold(PhysicsWorld world)
	{
		return PhysicsWorld_GetBounceThreshold_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetContactHitEventThreshold")]
	internal static void PhysicsWorld_SetContactHitEventThreshold(PhysicsWorld world, float value)
	{
		PhysicsWorld_SetContactHitEventThreshold_Injected(ref world, value);
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactHitEventThreshold", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetContactHitEventThreshold(PhysicsWorld world)
	{
		return PhysicsWorld_GetContactHitEventThreshold_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetContactFrequency")]
	internal static void PhysicsWorld_SetContactFrequency(PhysicsWorld world, float contactFrequency)
	{
		PhysicsWorld_SetContactFrequency_Injected(ref world, contactFrequency);
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactFrequency", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetContactFrequency(PhysicsWorld world)
	{
		return PhysicsWorld_GetContactFrequency_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetContactDamping")]
	internal static void PhysicsWorld_SetContactDamping(PhysicsWorld world, float damping)
	{
		PhysicsWorld_SetContactDamping_Injected(ref world, damping);
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactDamping", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetContactDamping(PhysicsWorld world)
	{
		return PhysicsWorld_GetContactDamping_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetContactSpeed")]
	internal static void PhysicsWorld_SetContactSpeed(PhysicsWorld world, float contactSpeed)
	{
		PhysicsWorld_SetContactSpeed_Injected(ref world, contactSpeed);
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactSpeed", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetContactSpeed(PhysicsWorld world)
	{
		return PhysicsWorld_GetContactSpeed_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetMaximumLinearSpeed")]
	internal static void PhysicsWorld_SetMaximumLinearSpeed(PhysicsWorld world, float maximumLinearSpeed)
	{
		PhysicsWorld_SetMaximumLinearSpeed_Injected(ref world, maximumLinearSpeed);
	}

	[NativeMethod(Name = "PhysicsWorld::GetMaximumLinearSpeed")]
	internal static float PhysicsWorld_GetMaximumLinearSpeed(PhysicsWorld world)
	{
		return PhysicsWorld_GetMaximumLinearSpeed_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetGravity")]
	internal static void PhysicsWorld_SetGravity(PhysicsWorld world, Vector2 gravity)
	{
		PhysicsWorld_SetGravity_Injected(ref world, ref gravity);
	}

	[NativeMethod(Name = "PhysicsWorld::GetGravity", IsThreadSafe = true)]
	internal static Vector2 PhysicsWorld_GetGravity(PhysicsWorld world)
	{
		PhysicsWorld_GetGravity_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::SetSimulationWorkers")]
	internal static void PhysicsWorld_SetSimulationWorkers(PhysicsWorld world, int simulationWorkers)
	{
		PhysicsWorld_SetSimulationWorkers_Injected(ref world, simulationWorkers);
	}

	[NativeMethod(Name = "PhysicsWorld::GetSimulationWorkers", IsThreadSafe = true)]
	internal static int PhysicsWorld_GetSimulationWorkers(PhysicsWorld world)
	{
		return PhysicsWorld_GetSimulationWorkers_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetSimulationType")]
	internal static void PhysicsWorld_SetSimulationType(PhysicsWorld world, PhysicsWorld.SimulationType simulationType)
	{
		PhysicsWorld_SetSimulationType_Injected(ref world, simulationType);
	}

	[NativeMethod(Name = "PhysicsWorld::GetSimulationType", IsThreadSafe = true)]
	internal static PhysicsWorld.SimulationType PhysicsWorld_GetSimulationType(PhysicsWorld world)
	{
		return PhysicsWorld_GetSimulationType_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetSimulationSubSteps")]
	internal static void PhysicsWorld_SetSimulationSubSteps(PhysicsWorld world, int subStepCpunt)
	{
		PhysicsWorld_SetSimulationSubSteps_Injected(ref world, subStepCpunt);
	}

	[NativeMethod(Name = "PhysicsWorld::GetSimulationSubSteps", IsThreadSafe = true)]
	internal static int PhysicsWorld_GetSimulationSubSteps(PhysicsWorld world)
	{
		return PhysicsWorld_GetSimulationSubSteps_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::GetLastSimulationTimestamp", IsThreadSafe = true)]
	internal static double PhysicsWorld_GetLastSimulationTimestamp(PhysicsWorld world)
	{
		return PhysicsWorld_GetLastSimulationTimestamp_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::GetLastSimulationDeltaTime", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetLastSimulationDeltaTime(PhysicsWorld world)
	{
		return PhysicsWorld_GetLastSimulationDeltaTime_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetTransformPlane", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetTransformPlane(PhysicsWorld world, PhysicsWorld.TransformPlane transformPlane)
	{
		PhysicsWorld_SetTransformPlane_Injected(ref world, transformPlane);
	}

	[NativeMethod(Name = "PhysicsWorld::GetTransformPlane", IsThreadSafe = true)]
	internal static PhysicsWorld.TransformPlane PhysicsWorld_GetTransformPlane(PhysicsWorld world)
	{
		return PhysicsWorld_GetTransformPlane_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetTransformWriteMode", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetTransformWriteMode(PhysicsWorld world, PhysicsWorld.TransformWriteMode transformWriteMode)
	{
		PhysicsWorld_SetTransformWriteMode_Injected(ref world, transformWriteMode);
	}

	[NativeMethod(Name = "PhysicsWorld::GetTransformWriteMode", IsThreadSafe = true)]
	internal static PhysicsWorld.TransformWriteMode PhysicsWorld_GetTransformWriteMode(PhysicsWorld world)
	{
		return PhysicsWorld_GetTransformWriteMode_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetTransformTweening", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetTransformTweening(PhysicsWorld world, bool flag)
	{
		PhysicsWorld_SetTransformTweening_Injected(ref world, flag);
	}

	[NativeMethod(Name = "PhysicsWorld::GetTransformTweening", IsThreadSafe = true)]
	internal static bool PhysicsWorld_GetTransformTweening(PhysicsWorld world)
	{
		return PhysicsWorld_GetTransformTweening_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::ClearTransformWriteTweens", IsThreadSafe = true)]
	internal static void PhysicsWorld_ClearTransformWriteTweens(PhysicsWorld world)
	{
		PhysicsWorld_ClearTransformWriteTweens_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetTransformWriteTweens", IsThreadSafe = true)]
	internal unsafe static void PhysicsWorld_SetTransformWriteTweens(PhysicsWorld world, ReadOnlySpan<PhysicsBody.TransformWriteTween> transformWriteTweens)
	{
		ReadOnlySpan<PhysicsBody.TransformWriteTween> readOnlySpan = transformWriteTweens;
		fixed (PhysicsBody.TransformWriteTween* begin = readOnlySpan)
		{
			ManagedSpanWrapper transformWriteTweens2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsWorld_SetTransformWriteTweens_Injected(ref world, ref transformWriteTweens2);
		}
	}

	[NativeMethod(Name = "PhysicsWorld::GetTransformWriteTweens", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetTransformWriteTweens(PhysicsWorld world)
	{
		PhysicsWorld_GetTransformWriteTweens_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::Simulate")]
	internal static void PhysicsWorld_Simulate(PhysicsWorld world, float timeStep, PhysicsWorld.SimulationType expectedSimulationType)
	{
		PhysicsWorld_Simulate_Injected(ref world, timeStep, expectedSimulationType);
	}

	[NativeMethod(Name = "PhysicsWorld::SimulateBatch")]
	internal unsafe static void PhysicsWorld_SimulateBatch(ReadOnlySpan<PhysicsWorld> worlds, float timeStep, PhysicsWorld.SimulationType expectedSimulationType)
	{
		ReadOnlySpan<PhysicsWorld> readOnlySpan = worlds;
		fixed (PhysicsWorld* begin = readOnlySpan)
		{
			ManagedSpanWrapper worlds2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsWorld_SimulateBatch_Injected(ref worlds2, timeStep, expectedSimulationType);
		}
	}

	[NativeMethod(Name = "PhysicsWorld::Explode")]
	internal static void PhysicsWorld_Explode(PhysicsWorld world, PhysicsWorld.ExplosionDefinition definition)
	{
		PhysicsWorld_Explode_Injected(ref world, ref definition);
	}

	[NativeMethod(Name = "PhysicsWorld::GetBodyUpdateUserData", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetBodyUpdateUserData(PhysicsWorld world, Allocator allocator)
	{
		PhysicsWorld_GetBodyUpdateUserData_Injected(ref world, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetBodyUpdateEvents", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetBodyUpdateEvents(PhysicsWorld world)
	{
		PhysicsWorld_GetBodyUpdateEvents_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetTriggerBeginEvents", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetTriggerBeginEvents(PhysicsWorld world)
	{
		PhysicsWorld_GetTriggerBeginEvents_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetTriggerEndEvents", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetTriggerEndEvents(PhysicsWorld world)
	{
		PhysicsWorld_GetTriggerEndEvents_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactBeginEvents", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetContactBeginEvents(PhysicsWorld world)
	{
		PhysicsWorld_GetContactBeginEvents_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactEndEvents", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetContactEndEvents(PhysicsWorld world)
	{
		PhysicsWorld_GetContactEndEvents_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactHitEvents", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetContactHitEvents(PhysicsWorld world)
	{
		PhysicsWorld_GetContactHitEvents_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetJointThresholdEvents", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetJointThresholdEvents(PhysicsWorld world)
	{
		PhysicsWorld_GetJointThresholdEvents_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetBodyUpdateCallbackTargets", IsThreadSafe = true)]
	internal static PhysicsCallbacks.BodyUpdateCallbackTargets PhysicsWorld_GetBodyUpdateCallbackTargets(PhysicsWorld world, Allocator allocator)
	{
		PhysicsWorld_GetBodyUpdateCallbackTargets_Injected(ref world, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetTriggerCallbackTargets", IsThreadSafe = true)]
	internal static PhysicsCallbacks.TriggerCallbackTargets PhysicsWorld_GetTriggerCallbackTargets(PhysicsWorld world, Allocator allocator)
	{
		PhysicsWorld_GetTriggerCallbackTargets_Injected(ref world, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetContactCallbackTargets", IsThreadSafe = true)]
	internal static PhysicsCallbacks.ContactCallbackTargets PhysicsWorld_GetContactCallbackTargets(PhysicsWorld world, Allocator allocator)
	{
		PhysicsWorld_GetContactCallbackTargets_Injected(ref world, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetJointThresholdCallbackTargets", IsThreadSafe = true)]
	internal static PhysicsCallbacks.JointThresholdCallbackTargets PhysicsWorld_GetJointThresholdCallbackTargets(PhysicsWorld world, Allocator allocator)
	{
		PhysicsWorld_GetJointThresholdCallbackTargets_Injected(ref world, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::TestOverlapAABB", IsThreadSafe = true)]
	internal static bool PhysicsWorld_TestOverlapAABB(PhysicsWorld world, PhysicsAABB aabb, PhysicsQuery.QueryFilter filter)
	{
		return PhysicsWorld_TestOverlapAABB_Injected(ref world, ref aabb, ref filter);
	}

	[NativeMethod(Name = "PhysicsWorld::TestOverlapShapeProxy", IsThreadSafe = true)]
	internal static bool PhysicsWorld_TestOverlapShapeProxy(PhysicsWorld world, PhysicsShape.ShapeProxy shapeProxy, PhysicsQuery.QueryFilter filter)
	{
		return PhysicsWorld_TestOverlapShapeProxy_Injected(ref world, ref shapeProxy, ref filter);
	}

	[NativeMethod(Name = "PhysicsWorld::OverlapAABB", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_OverlapAABB(PhysicsWorld world, PhysicsAABB aabb, PhysicsQuery.QueryFilter filter, Allocator allocator)
	{
		PhysicsWorld_OverlapAABB_Injected(ref world, ref aabb, ref filter, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::OverlapShapeProxy", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_OverlapShapeProxy(PhysicsWorld world, PhysicsShape.ShapeProxy shapeProxy, PhysicsQuery.QueryFilter filter, Allocator allocator)
	{
		PhysicsWorld_OverlapShapeProxy_Injected(ref world, ref shapeProxy, ref filter, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::CastRay", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_CastRay(PhysicsWorld world, PhysicsQuery.CastRayInput input, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode, Allocator allocator)
	{
		PhysicsWorld_CastRay_Injected(ref world, ref input, ref filter, castMode, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::CastShapeProxy", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_CastShapeProxy(PhysicsWorld world, PhysicsShape.ShapeProxy shapeProxy, Vector2 translation, PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode, Allocator allocator)
	{
		PhysicsWorld_CastShapeProxy_Injected(ref world, ref shapeProxy, ref translation, ref filter, castMode, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::CastMover", IsThreadSafe = true)]
	internal static PhysicsQuery.WorldMoverResult PhysicsWorld_CastMover(PhysicsWorld world, PhysicsQuery.WorldMoverInput input)
	{
		PhysicsWorld_CastMover_Injected(ref world, ref input, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetAwakeBodyCount", IsThreadSafe = true)]
	internal static int PhysicsWorld_GetAwakeBodyCount(PhysicsWorld world)
	{
		return PhysicsWorld_GetAwakeBodyCount_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::GetCounters", IsThreadSafe = true)]
	internal static PhysicsWorld.WorldCounters PhysicsWorld_GetCounters(PhysicsWorld world)
	{
		PhysicsWorld_GetCounters_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetProfile", IsThreadSafe = true)]
	internal static PhysicsWorld.WorldProfile PhysicsWorld_GetProfile(PhysicsWorld world)
	{
		PhysicsWorld_GetProfile_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetGlobalCounters")]
	internal static PhysicsWorld.WorldCounters PhysicsWorld_GetGlobalCounters()
	{
		PhysicsWorld_GetGlobalCounters_Injected(out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetGlobalProfile")]
	internal static PhysicsWorld.WorldProfile PhysicsWorld_GetGlobalProfile()
	{
		PhysicsWorld_GetGlobalProfile_Injected(out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsWorld::GetWorldCount", IsThreadSafe = true)]
	internal static extern int PhysicsWorld_GetWorldCount();

	[NativeMethod(Name = "PhysicsWorld::GetWorlds", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetWorlds(Allocator allocator)
	{
		PhysicsWorld_GetWorlds_Injected(allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetBodies", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetBodies(PhysicsWorld world, Allocator allocator)
	{
		PhysicsWorld_GetBodies_Injected(ref world, allocator, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::GetJoints", IsThreadSafe = true)]
	internal static PhysicsBuffer PhysicsWorld_GetJoints(PhysicsWorld world, Allocator allocator)
	{
		PhysicsWorld_GetJoints_Injected(ref world, allocator, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsWorld::GetHugeWorldExtent", IsThreadSafe = true)]
	internal static extern float PhysicsWorld_GetHugeWorldExtent();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsWorld::GetLinearSlop", IsThreadSafe = true)]
	internal static extern float PhysicsWorld_GetLinearSlop();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsWorld::GetSpeculativeContactDistance", IsThreadSafe = true)]
	internal static extern float PhysicsWorld_GetSpeculativeContactDistance();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsWorld::GetAABBMargin", IsThreadSafe = true)]
	internal static extern float PhysicsWorld_GetAABBMargin();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsWorld::GetBodyMaxRotation", IsThreadSafe = true)]
	internal static extern float PhysicsWorld_GetBodyMaxRotation();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "PhysicsWorld::GetBodyTimeToSleep", IsThreadSafe = true)]
	internal static extern float PhysicsWorld_GetBodyTimeToSleep();

	[NativeMethod(Name = "PhysicsWorld::SetDrawOptions", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawOptions(PhysicsWorld world, PhysicsWorld.DrawOptions drawOptions)
	{
		PhysicsWorld_SetDrawOptions_Injected(ref world, drawOptions);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawOptions", IsThreadSafe = true)]
	internal static PhysicsWorld.DrawOptions PhysicsWorld_GetDrawOptions(PhysicsWorld world)
	{
		return PhysicsWorld_GetDrawOptions_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawFillOptions", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawFillOptions(PhysicsWorld world, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		PhysicsWorld_SetDrawFillOptions_Injected(ref world, drawFillOptions);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawFillOptions", IsThreadSafe = true)]
	internal static PhysicsWorld.DrawFillOptions PhysicsWorld_GetDrawFillOptions(PhysicsWorld world)
	{
		return PhysicsWorld_GetDrawFillOptions_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawColors", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawColors(PhysicsWorld world, PhysicsWorld.DrawColors drawColors)
	{
		PhysicsWorld_SetDrawColors_Injected(ref world, ref drawColors);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawColors", IsThreadSafe = true)]
	internal static PhysicsWorld.DrawColors PhysicsWorld_GetDrawColors(PhysicsWorld world)
	{
		PhysicsWorld_GetDrawColors_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawThickness", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawThickness(PhysicsWorld world, float thickness)
	{
		PhysicsWorld_SetDrawThickness_Injected(ref world, thickness);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawThickness", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetDrawThickness(PhysicsWorld world)
	{
		return PhysicsWorld_GetDrawThickness_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawFillAlpha", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawFillAlpha(PhysicsWorld world, float alpha)
	{
		PhysicsWorld_SetDrawFillAlpha_Injected(ref world, alpha);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawFillAlpha", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetDrawFillAlpha(PhysicsWorld world)
	{
		return PhysicsWorld_GetDrawFillAlpha_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawPointScale", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawPointScale(PhysicsWorld world, float scale)
	{
		PhysicsWorld_SetDrawPointScale_Injected(ref world, scale);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawPointScale", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetDrawPointScale(PhysicsWorld world)
	{
		return PhysicsWorld_GetDrawPointScale_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawNormalScale", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawNormalScale(PhysicsWorld world, float scale)
	{
		PhysicsWorld_SetDrawNormalScale_Injected(ref world, scale);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawNormalScale", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetDrawNormalScale(PhysicsWorld world)
	{
		return PhysicsWorld_GetDrawNormalScale_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawImpulseScale", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawImpulseScale(PhysicsWorld world, float scale)
	{
		PhysicsWorld_SetDrawImpulseScale_Injected(ref world, scale);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawImpulseScale", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetDrawImpulseScale(PhysicsWorld world)
	{
		return PhysicsWorld_GetDrawImpulseScale_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawCapacity", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetDrawCapacity(PhysicsWorld world, int capacity)
	{
		PhysicsWorld_SetDrawCapacity_Injected(ref world, capacity);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawCapacity", IsThreadSafe = true)]
	internal static int PhysicsWorld_GetDrawCapacity(PhysicsWorld world)
	{
		return PhysicsWorld_GetDrawCapacity_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetDrawElementDepth", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetElementDepth(PhysicsWorld world, float elementDepth)
	{
		PhysicsWorld_SetElementDepth_Injected(ref world, elementDepth);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawElementDepth", IsThreadSafe = true)]
	internal static float PhysicsWorld_GetElementDepth(PhysicsWorld world)
	{
		return PhysicsWorld_GetElementDepth_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::GetDrawResults", IsThreadSafe = true)]
	internal static PhysicsWorld.DrawResults PhysicsWorld_GetDrawResults(PhysicsWorld world)
	{
		PhysicsWorld_GetDrawResults_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::ClearDraw", IsThreadSafe = true)]
	internal static void PhysicsWorld_ClearDraw(PhysicsWorld world, bool clearWorldDraw, bool clearCustomDraw, bool clearTimedDraw)
	{
		PhysicsWorld_ClearDraw_Injected(ref world, clearWorldDraw, clearCustomDraw, clearTimedDraw);
	}

	[NativeMethod(Name = "PhysicsWorld::Draw", IsThreadSafe = true)]
	internal static void PhysicsWorld_Draw(PhysicsWorld world, PhysicsAABB viewAABB)
	{
		PhysicsWorld_Draw_Injected(ref world, ref viewAABB);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawCircleGeometry", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawCircleGeometry(PhysicsWorld world, CircleGeometry geometry, PhysicsTransform transform, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		PhysicsWorld_DrawCircleGeometry_Injected(ref world, ref geometry, ref transform, ref color, lifetime, drawFillOptions);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawCapsuleGeometry", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawCapsuleGeometry(PhysicsWorld world, CapsuleGeometry geometry, PhysicsTransform transform, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		PhysicsWorld_DrawCapsuleGeometry_Injected(ref world, ref geometry, ref transform, ref color, lifetime, drawFillOptions);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawPolygonGeometry", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawPolygonGeometry(PhysicsWorld world, PolygonGeometry geometry, PhysicsTransform transform, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		PhysicsWorld_DrawPolygonGeometry_Injected(ref world, ref geometry, ref transform, ref color, lifetime, drawFillOptions);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawSegmentGeometry", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawSegmentGeometry(PhysicsWorld world, SegmentGeometry geometry, PhysicsTransform transform, Color color, float lifetime)
	{
		PhysicsWorld_DrawSegmentGeometry_Injected(ref world, ref geometry, ref transform, ref color, lifetime);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawCircleGeometrySpan", IsThreadSafe = true)]
	internal unsafe static void PhysicsWorld_DrawCircleGeometrySpan(PhysicsWorld world, ReadOnlySpan<CircleGeometry> geometry, PhysicsTransform transform, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		ReadOnlySpan<CircleGeometry> readOnlySpan = geometry;
		fixed (CircleGeometry* begin = readOnlySpan)
		{
			ManagedSpanWrapper geometry2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsWorld_DrawCircleGeometrySpan_Injected(ref world, ref geometry2, ref transform, ref color, lifetime, drawFillOptions);
		}
	}

	[NativeMethod(Name = "PhysicsWorld::DrawCapsuleGeometrySpan", IsThreadSafe = true)]
	internal unsafe static void PhysicsWorld_DrawCapsuleGeometrySpan(PhysicsWorld world, ReadOnlySpan<CapsuleGeometry> geometry, PhysicsTransform transform, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		ReadOnlySpan<CapsuleGeometry> readOnlySpan = geometry;
		fixed (CapsuleGeometry* begin = readOnlySpan)
		{
			ManagedSpanWrapper geometry2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsWorld_DrawCapsuleGeometrySpan_Injected(ref world, ref geometry2, ref transform, ref color, lifetime, drawFillOptions);
		}
	}

	[NativeMethod(Name = "PhysicsWorld::DrawPolygonGeometrySpan", IsThreadSafe = true)]
	internal unsafe static void PhysicsWorld_DrawPolygonGeometrySpan(PhysicsWorld world, ReadOnlySpan<PolygonGeometry> geometry, PhysicsTransform transform, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		ReadOnlySpan<PolygonGeometry> readOnlySpan = geometry;
		fixed (PolygonGeometry* begin = readOnlySpan)
		{
			ManagedSpanWrapper geometry2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsWorld_DrawPolygonGeometrySpan_Injected(ref world, ref geometry2, ref transform, ref color, lifetime, drawFillOptions);
		}
	}

	[NativeMethod(Name = "PhysicsWorld::DrawSegmentGeometrySpan", IsThreadSafe = true)]
	internal unsafe static void PhysicsWorld_DrawSegmentGeometrySpan(PhysicsWorld world, ReadOnlySpan<SegmentGeometry> geometry, PhysicsTransform transform, Color color, float lifetime)
	{
		ReadOnlySpan<SegmentGeometry> readOnlySpan = geometry;
		fixed (SegmentGeometry* begin = readOnlySpan)
		{
			ManagedSpanWrapper geometry2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsWorld_DrawSegmentGeometrySpan_Injected(ref world, ref geometry2, ref transform, ref color, lifetime);
		}
	}

	[NativeMethod(Name = "PhysicsWorld::DrawBox", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawBox(PhysicsWorld world, PhysicsTransform transform, Vector2 size, float radius, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		PhysicsWorld_DrawBox_Injected(ref world, ref transform, ref size, radius, ref color, lifetime, drawFillOptions);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawCircle", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawCircle(PhysicsWorld world, Vector2 center, float radius, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		PhysicsWorld_DrawCircle_Injected(ref world, ref center, radius, ref color, lifetime, drawFillOptions);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawCapsule", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawCapsule(PhysicsWorld world, PhysicsTransform transform, Vector2 center1, Vector2 center2, float radius, Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions)
	{
		PhysicsWorld_DrawCapsule_Injected(ref world, ref transform, ref center1, ref center2, radius, ref color, lifetime, drawFillOptions);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawPoint", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawPoint(PhysicsWorld world, Vector2 center, float radius, Color color, float lifetime)
	{
		PhysicsWorld_DrawPoint_Injected(ref world, ref center, radius, ref color, lifetime);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawLine", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawLine(PhysicsWorld world, Vector2 point0, Vector2 point1, Color color, float lifetime)
	{
		PhysicsWorld_DrawLine_Injected(ref world, ref point0, ref point1, ref color, lifetime);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawLineStrip", IsThreadSafe = true)]
	internal unsafe static void PhysicsWorld_DrawLineStrip(PhysicsWorld world, PhysicsTransform transform, ReadOnlySpan<Vector2> vertices, bool loop, Color color, float lifetime)
	{
		ReadOnlySpan<Vector2> readOnlySpan = vertices;
		fixed (Vector2* begin = readOnlySpan)
		{
			ManagedSpanWrapper vertices2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			PhysicsWorld_DrawLineStrip_Injected(ref world, ref transform, ref vertices2, loop, ref color, lifetime);
		}
	}

	[NativeMethod(Name = "PhysicsWorld::DrawTransformAxis", IsThreadSafe = true)]
	internal static void PhysicsWorld_DrawTransformAxis(PhysicsWorld world, PhysicsTransform transform, float scale, float lifetime)
	{
		PhysicsWorld_DrawTransformAxis_Injected(ref world, ref transform, scale, lifetime);
	}

	[NativeMethod(Name = "PhysicsWorld::GetRenderMaterial", IsThreadSafe = true)]
	internal unsafe static Material PhysicsWorld_GetRenderMaterial(string editorResourceName, string playerResourceName)
	{
		//The blocks IL_0029, IL_0036, IL_0044, IL_0052, IL_0057 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		Material result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper editorResourceName2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(editorResourceName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(editorResourceName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					editorResourceName2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(playerResourceName, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(playerResourceName);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							gcHandlePtr = PhysicsWorld_GetRenderMaterial_Injected(ref editorResourceName2, ref managedSpanWrapper2);
						}
					}
					else
					{
						gcHandlePtr = PhysicsWorld_GetRenderMaterial_Injected(ref editorResourceName2, ref managedSpanWrapper2);
					}
				}
			}
			else
			{
				editorResourceName2 = ref managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(playerResourceName, ref managedSpanWrapper2))
				{
					readOnlySpan2 = MemoryExtensions.AsSpan(playerResourceName);
					fixed (char* begin2 = readOnlySpan2)
					{
						managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
						gcHandlePtr = PhysicsWorld_GetRenderMaterial_Injected(ref editorResourceName2, ref managedSpanWrapper2);
					}
				}
				else
				{
					gcHandlePtr = PhysicsWorld_GetRenderMaterial_Injected(ref editorResourceName2, ref managedSpanWrapper2);
				}
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Material>(gcHandlePtr);
		}
		return result;
	}

	[NativeMethod(Name = "PhysicsWorld::SetOwner", IsThreadSafe = true)]
	internal static int PhysicsWorld_SetOwner(PhysicsWorld world, Object ownerObject)
	{
		return PhysicsWorld_SetOwner_Injected(ref world, Object.MarshalledUnityObject.Marshal(ownerObject));
	}

	[NativeMethod(Name = "PhysicsWorld::GetOwner", IsThreadSafe = true)]
	internal static Object PhysicsWorld_GetOwner(PhysicsWorld world)
	{
		return Unmarshal.UnmarshalUnityObject<Object>(PhysicsWorld_GetOwner_Injected(ref world));
	}

	[NativeMethod(Name = "PhysicsWorld::IsOwned", IsThreadSafe = true)]
	internal static bool PhysicsWorld_IsOwned(PhysicsWorld world)
	{
		return PhysicsWorld_IsOwned_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::SetUserData", IsThreadSafe = true)]
	internal static void PhysicsWorld_SetUserData(PhysicsWorld world, PhysicsUserData physicsUserData)
	{
		PhysicsWorld_SetUserData_Injected(ref world, ref physicsUserData);
	}

	[NativeMethod(Name = "PhysicsWorld::GetUserData", IsThreadSafe = true)]
	internal static PhysicsUserData PhysicsWorld_GetUserData(PhysicsWorld world)
	{
		PhysicsWorld_GetUserData_Injected(ref world, out var ret);
		return ret;
	}

	[NativeMethod(Name = "PhysicsWorld::IsDefaultWorld", IsThreadSafe = true)]
	internal static bool PhysicsWorld_IsDefaultWorld(PhysicsWorld world)
	{
		return PhysicsWorld_IsDefaultWorld_Injected(ref world);
	}

	[NativeMethod(Name = "PhysicsWorld::DrawAllWorlds")]
	internal static void PhysicsWorld_DrawAllWorlds(PhysicsAABB drawAABB)
	{
		PhysicsWorld_DrawAllWorlds_Injected(ref drawAABB);
	}

	[StaticAccessor("GetPhysicsLowLevel2D()", StaticAccessorType.Arrow)]
	[NativeMethod(Name = "PhysicsLowLevel2D::GetDefaultWorld")]
	internal static PhysicsWorld PhysicsWorld_GetDefaultWorld()
	{
		PhysicsWorld_GetDefaultWorld_Injected(out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetDefaultDefinition_Injected(bool useSettings, out PhysicsBodyDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_Create_Injected([In] ref PhysicsWorld world, [In] ref PhysicsBodyDefinition definition, out PhysicsBody ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_CreateBatch_Injected([In] ref PhysicsWorld world, ref ManagedSpanWrapper definitions, int bodyCount, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsBody_Destroy_Injected([In] ref PhysicsBody body, int ownerKey);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_DestroyBatch_Injected(ref ManagedSpanWrapper bodies);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsBody_IsValid_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetBatchVelocity_Injected(ref ManagedSpanWrapper batch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetBatchForce_Injected(ref ManagedSpanWrapper batch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetBatchImpulse_Injected(ref ManagedSpanWrapper batch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetBatchTransform_Injected(ref ManagedSpanWrapper batch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_WriteDefinition_Injected([In] ref PhysicsBody body, [In] ref PhysicsBodyDefinition definition, bool onlyExtendedProperties);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ReadDefinition_Injected([In] ref PhysicsBody body, out PhysicsBodyDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetWorld_Injected([In] ref PhysicsBody body, out PhysicsWorld ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsBody.BodyType PhysicsBody_GetBodyType_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetBodyType_Injected([In] ref PhysicsBody body, PhysicsBody.BodyType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetBodyConstraints_Injected([In] ref PhysicsBody body, PhysicsBody.BodyConstraints constraints);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsBody.BodyConstraints PhysicsBody_GetBodyConstraints_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetPosition_Injected([In] ref PhysicsBody body, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetPosition_Injected([In] ref PhysicsBody body, [In] ref Vector2 position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetRotation_Injected([In] ref PhysicsBody body, out PhysicsRotate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetRotation_Injected([In] ref PhysicsBody body, [In] ref PhysicsRotate rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetTransform_Injected([In] ref PhysicsBody body, out PhysicsTransform ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetTransform_Injected([In] ref PhysicsBody body, [In] ref PhysicsTransform transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetTransformTarget_Injected([In] ref PhysicsBody body, [In] ref PhysicsTransform transform, float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetLocalPoint_Injected([In] ref PhysicsBody body, [In] ref Vector2 worldPoint, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetWorldPoint_Injected([In] ref PhysicsBody body, [In] ref Vector2 localPoint, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetLocalVector_Injected([In] ref PhysicsBody body, [In] ref Vector2 worldVector, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetWorldVector_Injected([In] ref PhysicsBody body, [In] ref Vector2 localVector, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetLocalPointVelocity_Injected([In] ref PhysicsBody body, [In] ref Vector2 localPoint, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetWorldPointVelocity_Injected([In] ref PhysicsBody body, [In] ref Vector2 worldPoint, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetLinearVelocity_Injected([In] ref PhysicsBody body, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetLinearVelocity_Injected([In] ref PhysicsBody body, [In] ref Vector2 linearVelocity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsBody_GetAngularVelocity_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetAngularVelocity_Injected([In] ref PhysicsBody body, float angularVelocity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsBody_GetMass_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsBody_GetRotationalInertia_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetLocalCenterOfMass_Injected([In] ref PhysicsBody body, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetWorldCenterOfMass_Injected([In] ref PhysicsBody body, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetMassConfiguration_Injected([In] ref PhysicsBody body, [In] ref PhysicsBody.MassConfiguration massData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetMassConfiguration_Injected([In] ref PhysicsBody body, out PhysicsBody.MassConfiguration ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ApplyMassFromShapes_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetLinearDamping_Injected([In] ref PhysicsBody body, float linearDamping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsBody_GetLinearDamping_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetAngularDamping_Injected([In] ref PhysicsBody body, float angularDamping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsBody_GetAngularDamping_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetGravityScale_Injected([In] ref PhysicsBody body, float gravityScale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsBody_GetGravityScale_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetAwake_Injected([In] ref PhysicsBody body, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsBody_GetAwake_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetSleepingAllowed_Injected([In] ref PhysicsBody body, bool enableSleep);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsBody_GetSleepingAllowed_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetSleepThreshold_Injected([In] ref PhysicsBody body, float threshold);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsBody_GetSleepThreshold_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetEnabled_Injected([In] ref PhysicsBody body, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsBody_GetEnabled_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetFastRotationAllowed_Injected([In] ref PhysicsBody body, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsBody_GetFastRotationAllowed_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetFastCollisionsAllowed_Injected([In] ref PhysicsBody body, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsBody_GetFastCollisionsAllowed_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ApplyForce_Injected([In] ref PhysicsBody body, [In] ref Vector2 force, [In] ref Vector2 point, bool wake);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ApplyForceToCenter_Injected([In] ref PhysicsBody body, [In] ref Vector2 force, bool wake);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ApplyTorque_Injected([In] ref PhysicsBody body, float torque, bool wake);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ApplyLinearImpulse_Injected([In] ref PhysicsBody body, [In] ref Vector2 impulse, [In] ref Vector2 point, bool wake);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ApplyLinearImpulseToCenter_Injected([In] ref PhysicsBody body, [In] ref Vector2 impulse, bool wake);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ApplyAngularImpulse_Injected([In] ref PhysicsBody body, float impulse, bool wake);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_ClearForces_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_WakeTouching_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetContactEvents_Injected([In] ref PhysicsBody body, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetHitEvents_Injected([In] ref PhysicsBody body, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsBody_GetShapeCount_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetShapes_Injected([In] ref PhysicsBody PhysicsBody, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsBody_GetJointCount_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetJoints_Injected([In] ref PhysicsBody body, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetContacts_Injected([In] ref PhysicsBody body, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_CalculateAABB_Injected([In] ref PhysicsBody body, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_Draw_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsBody_SetOwner_Injected([In] ref PhysicsBody body, IntPtr ownerObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr PhysicsBody_GetOwner_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsBody_IsOwned_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetCallbackTarget_Injected([In] ref PhysicsBody body, object callbackTarget);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object PhysicsBody_GetCallbackTarget_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetUserData_Injected([In] ref PhysicsBody body, [In] ref PhysicsUserData physicsUserData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_GetUserData_Injected([In] ref PhysicsBody body, out PhysicsUserData ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetTransformObject_Injected([In] ref PhysicsBody body, IntPtr transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr PhysicsBody_GetTransformObject_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsBody_SetTransformWriteMode_Injected([In] ref PhysicsBody body, PhysicsBody.TransformWriteMode writeMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsBody.TransformWriteMode PhysicsBody_GetTransformWriteMode_Injected([In] ref PhysicsBody body);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_GetDefaultDefinition_Injected(bool useSettings, out PhysicsChainDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_Create_Injected([In] ref PhysicsBody body, [In] ref ChainGeometry geometry, [In] ref PhysicsChainDefinition definition, out PhysicsChain ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsChain_Destroy_Injected([In] ref PhysicsChain chain, int ownerKey);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsChain_IsValid_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_GetWorld_Injected([In] ref PhysicsChain chain, out PhysicsWorld ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_GetBody_Injected([In] ref PhysicsChain chain, out PhysicsBody ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_SetFriction_Injected([In] ref PhysicsChain chain, float friction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsChain_GetFriction_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_SetBounciness_Injected([In] ref PhysicsChain chain, float bounciness);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsChain_GetBounciness_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_SetFrictionMixing_Injected([In] ref PhysicsChain chain, PhysicsShape.SurfaceMaterial.MixingMode frictionMixing);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsShape.SurfaceMaterial.MixingMode PhysicsChain_GetFrictionMixing_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_SetBouncinessMixing_Injected([In] ref PhysicsChain chain, PhysicsShape.SurfaceMaterial.MixingMode bouncinessMixing);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsShape.SurfaceMaterial.MixingMode PhysicsChain_GetBouncinessMixing_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsChain_GetSegmentCount_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_GetSegments_Injected([In] ref PhysicsChain chain, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsChain_GetSegmentIndex_Injected([In] ref PhysicsChain chain, [In] ref PhysicsShape chainSegmentShape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_CalculateAABB_Injected([In] ref PhysicsChain chain, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_ClosestPoint_Injected([In] ref PhysicsChain chain, [In] ref Vector2 point, out PhysicsShape chainSegmentShape, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_CastRay_Injected([In] ref PhysicsChain chain, [In] ref PhysicsQuery.CastRayInput input, out PhysicsShape chainSegmentShape, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_CastShape_Injected([In] ref PhysicsChain chain, [In] ref PhysicsQuery.CastShapeInput input, out PhysicsShape chainSegmentShape, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsChain_SetOwner_Injected([In] ref PhysicsChain chain, IntPtr ownerObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr PhysicsChain_GetOwner_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsChain_IsOwned_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_SetCallbackTarget_Injected([In] ref PhysicsChain chain, object callbackTarget);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object PhysicsChain_GetCallbackTarget_Injected([In] ref PhysicsChain chain);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_SetUserData_Injected([In] ref PhysicsChain chain, [In] ref PhysicsUserData physicsUserData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsChain_GetUserData_Injected([In] ref PhysicsChain chain, out PhysicsUserData ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CircleGeometry_IsValid_Injected([In] ref CircleGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CircleGeometry_CalculateMassConfiguration_Injected([In] ref CircleGeometry geometry, float density, out PhysicsBody.MassConfiguration ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CircleGeometry_CalculateAABB_Injected([In] ref CircleGeometry geometry, [In] ref PhysicsTransform transform, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CircleGeometry_OverlapPoint_Injected([In] ref CircleGeometry geometry, [In] ref Vector2 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CircleGeometry_ClosestPoint_Injected([In] ref CircleGeometry geometry, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CircleGeometry_CastRay_Injected([In] ref CircleGeometry geometry, [In] ref PhysicsQuery.CastRayInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CircleGeometry_CastShape_Injected([In] ref CircleGeometry geometry, [In] ref PhysicsQuery.CastShapeInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CapsuleGeometry_IsValid_Injected([In] ref CapsuleGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapsuleGeometry_CalculateMassConfiguration_Injected([In] ref CapsuleGeometry geometry, float density, out PhysicsBody.MassConfiguration ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapsuleGeometry_CalculateAABB_Injected([In] ref CapsuleGeometry geometry, [In] ref PhysicsTransform transform, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CapsuleGeometry_OverlapPoint_Injected([In] ref CapsuleGeometry geometry, [In] ref Vector2 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapsuleGeometry_ClosestPoint_Injected([In] ref CapsuleGeometry geometry, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapsuleGeometry_CastRay_Injected([In] ref CapsuleGeometry geometry, [In] ref PhysicsQuery.CastRayInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CapsuleGeometry_CastShape_Injected([In] ref CapsuleGeometry geometry, [In] ref PhysicsQuery.CastShapeInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_CreateBox_Injected([In] ref Vector2 size, float radius, [In] ref PhysicsTransform transform, bool inscribe, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_CreatePolygons_Injected(ref ManagedSpanWrapper vertices, [In] ref PhysicsTransform transform, [In] ref Vector2 vertexScale, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_Create_WithPhysicsTransform_Injected(ref ManagedSpanWrapper vertices, float radius, [In] ref PhysicsTransform transform, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_Create_WithMatrix_Injected(ref ManagedSpanWrapper vertices, float radius, [In] ref Matrix4x4 transform, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_Transform_WithPhysicsTransform_Injected([In] ref PolygonGeometry geometry, [In] ref PhysicsTransform transform, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_InverseTransform_WithPhysicsTransform_Injected([In] ref PolygonGeometry geometry, [In] ref PhysicsTransform transform, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_Transform_WithMatrix_Injected([In] ref PolygonGeometry geometry, [In] ref Matrix4x4 transform, bool scaleRadius, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_InverseTransform_WithMatrix_Injected([In] ref PolygonGeometry geometry, [In] ref Matrix4x4 transform, bool scaleRadius, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PolygonGeometry_IsValid_Injected([In] ref PolygonGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_Validate_Injected([In] ref PolygonGeometry geometry, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_CalculateMassConfiguration_Injected([In] ref PolygonGeometry geometry, float density, out PhysicsBody.MassConfiguration ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_CalculateAABB_Injected([In] ref PolygonGeometry geometry, [In] ref PhysicsTransform transform, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PolygonGeometry_OverlapPoint_Injected([In] ref PolygonGeometry geometry, [In] ref Vector2 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_ClosestPoint_Injected([In] ref PolygonGeometry geometry, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_CastRay_Injected([In] ref PolygonGeometry geometry, [In] ref PhysicsQuery.CastRayInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PolygonGeometry_CastShape_Injected([In] ref PolygonGeometry geometry, [In] ref PhysicsQuery.CastShapeInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SegmentGeometry_IsValid_Injected([In] ref SegmentGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SegmentGeometry_CalculateAABB_Injected([In] ref SegmentGeometry geometry, [In] ref PhysicsTransform transform, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SegmentGeometry_ClosestPoint_Injected([In] ref SegmentGeometry geometry, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SegmentGeometry_CastRay_Injected([In] ref SegmentGeometry geometry, [In] ref PhysicsQuery.CastRayInput input, bool oneSided, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SegmentGeometry_CastShape_Injected([In] ref SegmentGeometry geometry, [In] ref PhysicsQuery.CastShapeInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ChainSegmentGeometry_IsValid_Injected([In] ref ChainSegmentGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ChainSegmentGeometry_CalculateAABB_Injected([In] ref ChainSegmentGeometry geometry, [In] ref PhysicsTransform transform, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ChainSegmentGeometry_ClosestPoint_Injected([In] ref ChainSegmentGeometry geometry, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ChainSegmentGeometry_CastRay_Injected([In] ref ChainSegmentGeometry geometry, [In] ref PhysicsQuery.CastRayInput input, bool oneSided, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ChainSegmentGeometry_CastShape_Injected([In] ref ChainSegmentGeometry geometry, [In] ref PhysicsQuery.CastShapeInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ChainGeometry_IsValid_Injected([In] ref ChainGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ChainGeometry_CalculateAABB_Injected([In] ref ChainGeometry geometry, [In] ref PhysicsTransform transform, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ChainGeometry_ClosestPoint_Injected([In] ref ChainGeometry geometry, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ChainGeometry_CastRay_Injected([In] ref ChainGeometry geometry, [In] ref PhysicsQuery.CastRayInput input, bool oneSided, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ChainGeometry_CastShape_Injected([In] ref ChainGeometry geometry, [In] ref PhysicsQuery.CastShapeInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsJoint_Destroy_Injected([In] ref PhysicsJoint joint, int ownerKey);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_DestroyBatch_Injected(ref ManagedSpanWrapper joints);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsJoint_IsValid_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_GetWorld_Injected([In] ref PhysicsJoint joint, out PhysicsWorld ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsJoint.JointType PhysicsJoint_GetJointType_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_GetBodyA_Injected([In] ref PhysicsJoint joint, out PhysicsBody ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_GetBodyB_Injected([In] ref PhysicsJoint joint, out PhysicsBody ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetLocalAnchorA_Injected([In] ref PhysicsJoint joint, [In] ref PhysicsTransform localAnchor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_GetLocalAnchorA_Injected([In] ref PhysicsJoint joint, out PhysicsTransform ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetLocalAnchorB_Injected([In] ref PhysicsJoint joint, [In] ref PhysicsTransform localAnchor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_GetLocalAnchorB_Injected([In] ref PhysicsJoint joint, out PhysicsTransform ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetForceThreshold_Injected([In] ref PhysicsJoint joint, float forceThreshold);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsJoint_GetForceThreshold_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetTorqueThreshold_Injected([In] ref PhysicsJoint joint, float torqueThreshold);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsJoint_GetTorqueThreshold_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetCollideConnected_Injected([In] ref PhysicsJoint joint, bool shouldCollide);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsJoint_GetCollideConnected_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetTuningFrequency_Injected([In] ref PhysicsJoint joint, float tuningFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsJoint_GetTuningFrequency_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetTuningDamping_Injected([In] ref PhysicsJoint joint, float tuningDamping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsJoint_GetTuningDamping_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetDrawScale_Injected([In] ref PhysicsJoint joint, float drawScale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsJoint_GetDrawScale_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_WakeBodies_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_GetCurrentConstraintForce_Injected([In] ref PhysicsJoint joint, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsJoint_GetCurrentConstraintTorque_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsJoint_GetCurrentLinearSeparation_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsJoint_GetCurrentAngularSeparation_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_Draw_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsJoint_SetOwner_Injected([In] ref PhysicsJoint joint, IntPtr ownerObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr PhysicsJoint_GetOwner_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsJoint_IsOwned_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetCallbackTarget_Injected([In] ref PhysicsJoint joint, object callbackTarget);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object PhysicsJoint_GetCallbackTarget_Injected([In] ref PhysicsJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_SetUserData_Injected([In] ref PhysicsJoint joint, [In] ref PhysicsUserData physicsUserData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsJoint_GetUserData_Injected([In] ref PhysicsJoint joint, out PhysicsUserData ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_GetDefaultDefinition_Injected(bool useSettings, out PhysicsDistanceJointDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_Create_Injected([In] ref PhysicsWorld world, [In] ref PhysicsDistanceJointDefinition definition, out PhysicsDistanceJoint ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetDistance_Injected([In] ref PhysicsDistanceJoint joint, float distance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetDistance_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetCurrentDistance_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetEnableSpring_Injected([In] ref PhysicsDistanceJoint joint, bool enableSpring);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DistanceJoint_GetEnableSpring_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetSpringLowerForce_Injected([In] ref PhysicsDistanceJoint joint, float springLowerForce);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetSpringLowerForce_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetSpringUpperForce_Injected([In] ref PhysicsDistanceJoint joint, float springUpperForce);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetSpringUpperForce_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetSpringFrequency_Injected([In] ref PhysicsDistanceJoint joint, float springFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetSpringFrequency_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetSpringDamping_Injected([In] ref PhysicsDistanceJoint joint, float springDamping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetSpringDamping_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetEnableMotor_Injected([In] ref PhysicsDistanceJoint joint, bool enableMotor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DistanceJoint_GetEnableMotor_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetMotorSpeed_Injected([In] ref PhysicsDistanceJoint joint, float motorSpeed);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetMotorSpeed_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetMaxMotorForce_Injected([In] ref PhysicsDistanceJoint joint, float maxMotorForce);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetMaxMotorForce_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetCurrentMotorForce_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetEnableLimit_Injected([In] ref PhysicsDistanceJoint joint, bool enableLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool DistanceJoint_GetEnableLimit_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetMinDistanceLimit_Injected([In] ref PhysicsDistanceJoint joint, float minDistanceLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetMinDistanceLimit_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DistanceJoint_SetMaxDistanceLimit_Injected([In] ref PhysicsDistanceJoint joint, float maxDistanceLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float DistanceJoint_GetMaxDistanceLimit_Injected([In] ref PhysicsDistanceJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_GetDefaultDefinition_Injected(bool useSettings, out PhysicsRelativeJointDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_Create_Injected([In] ref PhysicsWorld world, [In] ref PhysicsRelativeJointDefinition definition, out PhysicsRelativeJoint ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetLinearVelocity_Injected([In] ref PhysicsRelativeJoint joint, [In] ref Vector2 linearVelocity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_GetLinearVelocity_Injected([In] ref PhysicsRelativeJoint joint, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetAngularVelocity_Injected([In] ref PhysicsRelativeJoint joint, float angularVelocity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetAngularVelocity_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetMaxForce_Injected([In] ref PhysicsRelativeJoint joint, float maxForce);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetMaxForce_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetMaxTorque_Injected([In] ref PhysicsRelativeJoint joint, float maxTorque);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetMaxTorque_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetSpringLinearFrequency_Injected([In] ref PhysicsRelativeJoint joint, float springLinearFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetSpringLinearFrequency_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetSpringAngularFrequency_Injected([In] ref PhysicsRelativeJoint joint, float springAngularFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetSpringAngularFrequency_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetSpringLinearDamping_Injected([In] ref PhysicsRelativeJoint joint, float springLinearDamping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetSpringLinearDamping_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetSpringAngularDamping_Injected([In] ref PhysicsRelativeJoint joint, float springAngularDamping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetSpringAngularDamping_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetSpringMaxForce_Injected([In] ref PhysicsRelativeJoint joint, float springMaxForce);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetSpringMaxForce_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RelativeJoint_SetSpringMaxTorque_Injected([In] ref PhysicsRelativeJoint joint, float springMaxTorque);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float RelativeJoint_GetSpringMaxTorque_Injected([In] ref PhysicsRelativeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void IgnorePhysicsJoint_GetDefaultDefinition_Injected(out PhysicsIgnoreJointDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void IgnorePhysicsJoint_Create_Injected([In] ref PhysicsWorld world, [In] ref PhysicsIgnoreJointDefinition definition, out PhysicsIgnoreJoint ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_GetDefaultDefinition_Injected(bool useSettings, out PhysicsSliderJointDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_Create_Injected([In] ref PhysicsWorld world, [In] ref PhysicsSliderJointDefinition definition, out PhysicsSliderJoint ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetEnableSpring_Injected([In] ref PhysicsSliderJoint joint, bool enableSpring);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SliderJoint_GetEnableSpring_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetSpringFrequency_Injected([In] ref PhysicsSliderJoint joint, float springFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetSpringFrequency_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetSpringDamping_Injected([In] ref PhysicsSliderJoint joint, float damping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetSpringDamping_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetSpringTargetTranslation_Injected([In] ref PhysicsSliderJoint joint, float targetTranslation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetSpringTargetTranslation_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetEnableMotor_Injected([In] ref PhysicsSliderJoint joint, bool enableMotor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SliderJoint_GetEnableMotor_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetMotorSpeed_Injected([In] ref PhysicsSliderJoint joint, float motorSpeed);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetMotorSpeed_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetMaxMotorForce_Injected([In] ref PhysicsSliderJoint joint, float force);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetMaxMotorForce_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetCurrentMotorForce_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetCurrentTranslation_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetCurrentSpeed_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetEnableLimit_Injected([In] ref PhysicsSliderJoint joint, bool enableLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SliderJoint_GetEnableLimit_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetLowerTranslationLimit_Injected([In] ref PhysicsSliderJoint joint, float lowerTranslationLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetLowerTranslationLimit_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SliderJoint_SetUpperTranslationLimit_Injected([In] ref PhysicsSliderJoint joint, float upperTranslationLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float SliderJoint_GetUpperTranslationLimit_Injected([In] ref PhysicsSliderJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_GetDefaultDefinition_Injected(bool useSettings, out PhysicsHingeJointDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_Create_Injected([In] ref PhysicsWorld world, [In] ref PhysicsHingeJointDefinition definition, out PhysicsHingeJoint ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetEnableSpring_Injected([In] ref PhysicsHingeJoint joint, bool enableSpring);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HingeJoint_GetEnableSpring_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetSpringFrequency_Injected([In] ref PhysicsHingeJoint joint, float springFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetSpringFrequency_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetSpringDamping_Injected([In] ref PhysicsHingeJoint joint, float damping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetSpringDamping_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetSpringTargetAngle_Injected([In] ref PhysicsHingeJoint joint, float targetAngle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetSpringTargetAngle_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetAngle_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetEnableMotor_Injected([In] ref PhysicsHingeJoint joint, bool enableMotor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HingeJoint_GetEnableMotor_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetMotorSpeed_Injected([In] ref PhysicsHingeJoint joint, float motorSpeed);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetMotorSpeed_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetMaxMotorTorque_Injected([In] ref PhysicsHingeJoint joint, float torque);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetMaxMotorTorque_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetCurrentMotorTorque_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetEnableLimit_Injected([In] ref PhysicsHingeJoint joint, bool enableLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HingeJoint_GetEnableLimit_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetLowerLimit_Injected([In] ref PhysicsHingeJoint joint, float lowerAngleLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetLowerLimit_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void HingeJoint_SetUpperLimit_Injected([In] ref PhysicsHingeJoint joint, float upperAngleLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float HingeJoint_GetUpperLimit_Injected([In] ref PhysicsHingeJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FixedJoint_GetDefaultDefinition_Injected(bool useSettings, out PhysicsFixedJointDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FixedJoint_Create_Injected([In] ref PhysicsWorld world, [In] ref PhysicsFixedJointDefinition definition, out PhysicsFixedJoint ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FixedJoint_SetLinearFrequency_Injected([In] ref PhysicsFixedJoint joint, float linearFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float FixedJoint_GetLinearFrequency_Injected([In] ref PhysicsFixedJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FixedJoint_SetLinearDamping_Injected([In] ref PhysicsFixedJoint joint, float damping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float FixedJoint_GetLinearDamping_Injected([In] ref PhysicsFixedJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FixedJoint_SetAngularFrequency_Injected([In] ref PhysicsFixedJoint joint, float angularFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float FixedJoint_GetAngularFrequency_Injected([In] ref PhysicsFixedJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FixedJoint_SetAngularDamping_Injected([In] ref PhysicsFixedJoint joint, float damping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float FixedJoint_GetAngularDamping_Injected([In] ref PhysicsFixedJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_GetDefaultDefinition_Injected(bool useSettings, out PhysicsWheelJointDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_Create_Injected([In] ref PhysicsWorld world, [In] ref PhysicsWheelJointDefinition definition, out PhysicsWheelJoint ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetEnableSpring_Injected([In] ref PhysicsWheelJoint joint, bool enableSpring);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool WheelJoint_GetEnableSpring_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetSpringFrequency_Injected([In] ref PhysicsWheelJoint joint, float springFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float WheelJoint_GetSpringFrequency_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetSpringDamping_Injected([In] ref PhysicsWheelJoint joint, float damping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float WheelJoint_GetSpringDamping_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetEnableMotor_Injected([In] ref PhysicsWheelJoint joint, bool enableMotor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool WheelJoint_GetEnableMotor_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetMotorSpeed_Injected([In] ref PhysicsWheelJoint joint, float motorSpeed);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float WheelJoint_GetMotorSpeed_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetMaxMotorTorque_Injected([In] ref PhysicsWheelJoint joint, float torque);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float WheelJoint_GetMaxMotorTorque_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float WheelJoint_GetCurrentMotorTorque_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetEnableLimit_Injected([In] ref PhysicsWheelJoint joint, bool enableLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool WheelJoint_GetEnableLimit_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetLowerTranslationLimit_Injected([In] ref PhysicsWheelJoint joint, float lowerTranslationLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float WheelJoint_GetLowerTranslationLimit_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WheelJoint_SetUpperTranslationLimit_Injected([In] ref PhysicsWheelJoint joint, float upperTranslationLimit);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float WheelJoint_GetUpperTranslationLimit_Injected([In] ref PhysicsWheelJoint joint);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsTransform_IsValid_Injected([In] ref PhysicsTransform transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsTransform_TransformPoint_Injected([In] ref PhysicsTransform transform, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsTransform_InverseTransformPoint_Injected([In] ref PhysicsTransform transform, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsTransform_MultiplyTransform_Injected([In] ref PhysicsTransform transform1, [In] ref PhysicsTransform PhysicsTransform, out PhysicsTransform ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsTransform_InverseMultiplyTransform_Injected([In] ref PhysicsTransform transform1, [In] ref PhysicsTransform PhysicsTransform, out PhysicsTransform ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_CreateAngle_Injected(float angle, out PhysicsRotate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsRotate_IsValid_Injected([In] ref PhysicsRotate rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsRotate_GetAngle_Injected([In] ref PhysicsRotate rotate);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsRotate_GetRelativeAngle_Injected([In] ref PhysicsRotate rotation1, [In] ref PhysicsRotate rotation2);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_IntegrateRotation_Injected([In] ref PhysicsRotate rotation, float deltaAngle, out PhysicsRotate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_LerpRotation_Injected([In] ref PhysicsRotate rotationA, [In] ref PhysicsRotate rotationB, float interval, out PhysicsRotate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsRotate_AngularVelocity_Injected([In] ref PhysicsRotate rotationA, [In] ref PhysicsRotate rotationB, float deltaTime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_Normalize_Injected(in Vector2 direction, out PhysicsRotate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsRotate_IsNormalized_Injected([In] ref PhysicsRotate rotation);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_MultiplyRotation_Injected([In] ref PhysicsRotate rotation1, [In] ref PhysicsRotate rotation2, out PhysicsRotate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_InverseMultiplyRotation_Injected([In] ref PhysicsRotate rotation1, [In] ref PhysicsRotate rotation2, out PhysicsRotate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_RotateVector_Injected([In] ref PhysicsRotate rotation, [In] ref Vector2 vector, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_InverseRotateVector_Injected([In] ref PhysicsRotate rotation, [In] ref Vector2 vector, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsRotate_Rotate_Injected([In] ref PhysicsRotate rotation, float deltaAngle, out PhysicsRotate ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsAABB_IsValid_Injected([In] ref PhysicsAABB aabb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsAABB_OverlapPoint_Injected([In] ref PhysicsAABB aabb, [In] ref Vector2 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsAABB_CastRay_Injected([In] ref PhysicsAABB aabb, [In] ref PhysicsQuery.CastRayInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsAABB_Overlap_Injected([In] ref PhysicsAABB aabb1, [In] ref PhysicsAABB aabb2);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsAABB_Union_Injected([In] ref PhysicsAABB aabb1, [In] ref PhysicsAABB aabb2, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsAABB_Contains_Injected([In] ref PhysicsAABB aabb1, [In] ref PhysicsAABB aabb2);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsAABB_Center_Injected([In] ref PhysicsAABB aabb, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsAABB_Extents_Injected([In] ref PhysicsAABB aabb, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsAABB_Perimeter_Injected([In] ref PhysicsAABB aabb);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsPlane_IsValid_Injected([In] ref PhysicsPlane plane);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsPlane_GetSeparation_Injected([In] ref PhysicsPlane plane, [In] ref Vector2 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr PhysicsUserData_GetObject_Injected([In] ref EntityId entityId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsGlobal_PopulateWorldTransformWrite_Injected([In] ref PhysicsWorld world, IntPtr transformAccessArrayIntPtr, ref ManagedSpanWrapper transformWriteTweensArray);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_ShapeAndShape_Injected([In] ref PhysicsShape shapeA, [In] ref PhysicsTransform transformA, [In] ref PhysicsShape shapeB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_CircleAndCircle_Injected([In] ref CircleGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CircleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_CapsuleAndCircle_Injected([In] ref CapsuleGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CircleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_SegmentAndCircle_Injected([In] ref SegmentGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CircleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_PolygonAndCircle_Injected([In] ref PolygonGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CircleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_CapsuleAndCapsule_Injected([In] ref CapsuleGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CapsuleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_SegmentAndCapsule_Injected([In] ref SegmentGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CapsuleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_PolygonAndCapsule_Injected([In] ref PolygonGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CapsuleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_PolygonAndPolygon_Injected([In] ref PolygonGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref PolygonGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_SegmentAndPolygon_Injected([In] ref SegmentGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref PolygonGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_ChainSegmentAndCircle_Injected([In] ref ChainSegmentGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CircleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_ChainSegmentAndCapsule_Injected([In] ref ChainSegmentGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref CapsuleGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_ChainSegmentAndPolygon_Injected([In] ref ChainSegmentGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref PolygonGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsShape.ContactManifold ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_CastShapes_Injected([In] ref PhysicsQuery.CastShapePairInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_SegmentDistance_Injected([In] ref SegmentGeometry geometryA, [In] ref PhysicsTransform transformA, [In] ref SegmentGeometry geometryB, [In] ref PhysicsTransform transformB, out PhysicsQuery.SegmentDistanceResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_ShapeDistance_Injected([In] ref PhysicsQuery.DistanceInput distanceInput, out PhysicsQuery.DistanceResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsQuery_ShapeTimeOfImpact_Injected([In] ref PhysicsQuery.TimeOfImpactInput toiInput, out PhysicsQuery.TimeOfImpactResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetDefaultDefinition_Injected(bool useSettings, out PhysicsShapeDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetDefaultSurfaceMaterial_Injected(out PhysicsShape.SurfaceMaterial ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CreateCircleShape_Injected([In] ref PhysicsBody body, [In] ref CircleGeometry geometry, [In] ref PhysicsShapeDefinition definition, out PhysicsShape ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CreatePolygonShape_Injected([In] ref PhysicsBody body, [In] ref PolygonGeometry geometry, [In] ref PhysicsShapeDefinition definition, out PhysicsShape ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CreateCapsuleShape_Injected([In] ref PhysicsBody body, [In] ref CapsuleGeometry geometry, [In] ref PhysicsShapeDefinition definition, out PhysicsShape ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CreateSegmentShape_Injected([In] ref PhysicsBody body, [In] ref SegmentGeometry geometry, [In] ref PhysicsShapeDefinition definition, out PhysicsShape ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CreateChainSegmenShapet_Injected([In] ref PhysicsBody body, [In] ref ChainSegmentGeometry geometry, [In] ref PhysicsShapeDefinition definition, out PhysicsShape ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CreateShapeBatch_Injected([In] ref PhysicsBody body, [In] ref PhysicsBuffer spanGeometry, PhysicsShape.ShapeType shapeType, [In] ref PhysicsShapeDefinition definition, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_Destroy_Injected([In] ref PhysicsShape shape, bool updateBodyMass, int ownerKey);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_DestroyBatch_Injected(ref ManagedSpanWrapper shapes, bool updateBodyMass);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_WriteDefinition_Injected([In] ref PhysicsShape shape, [In] ref PhysicsShapeDefinition definition, bool onlyExtendedProperties);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_ReadDefinition_Injected([In] ref PhysicsShape shape, out PhysicsShapeDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_IsValid_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetWorld_Injected([In] ref PhysicsShape shape, out PhysicsWorld ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetBody_Injected([In] ref PhysicsShape shape, out PhysicsBody ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetIsTrigger_Injected([In] ref PhysicsShape shape, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_GetIsTrigger_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsShape.ShapeType PhysicsShape_GetShapeType_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetDensity_Injected([In] ref PhysicsShape shape, float density, bool updateBodyMass);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsShape_GetDensity_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetMassConfiguration_Injected([In] ref PhysicsShape shape, out PhysicsBody.MassConfiguration ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetFriction_Injected([In] ref PhysicsShape shape, float friction);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsShape_GetFriction_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetBounciness_Injected([In] ref PhysicsShape shape, float bounciness);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsShape_GetBounciness_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetFrictionMixing_Injected([In] ref PhysicsShape shape, PhysicsShape.SurfaceMaterial.MixingMode frictionMixing);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsShape.SurfaceMaterial.MixingMode PhysicsShape_GetFrictionMixing_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetBouncinessMixing_Injected([In] ref PhysicsShape shape, PhysicsShape.SurfaceMaterial.MixingMode bouncinessMixing);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsShape.SurfaceMaterial.MixingMode PhysicsShape_GetBouncinessMixing_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetFrictionPriority_Injected([In] ref PhysicsShape shape, ushort frictionPriority);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ushort PhysicsShape_GetFrictionPriority_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetBouncinessPriority_Injected([In] ref PhysicsShape shape, ushort bouncinessPriority);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ushort PhysicsShape_GetBouncinessPriority_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetRollingResistance_Injected([In] ref PhysicsShape shape, float rollingResistance);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsShape_GetRollingResistance_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetTangentSpeed_Injected([In] ref PhysicsShape shape, float tangentSpeed);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsShape_GetTangentSpeed_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetCustomColor_Injected([In] ref PhysicsShape shape, [In] ref Color32 customColor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetCustomColor_Injected([In] ref PhysicsShape shape, out Color32 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetSurfaceMaterial_Injected([In] ref PhysicsShape shape, [In] ref PhysicsShape.SurfaceMaterial surfaceMateria);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetSurfaceMaterial_Injected([In] ref PhysicsShape shape, out PhysicsShape.SurfaceMaterial ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetContactFilter_Injected([In] ref PhysicsShape shape, [In] ref PhysicsShape.ContactFilter filter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetContactFilter_Injected([In] ref PhysicsShape shape, out PhysicsShape.ContactFilter ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetMoverData_Injected([In] ref PhysicsShape shape, [In] ref PhysicsShape.MoverData moverData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetMoverData_Injected([In] ref PhysicsShape shape, out PhysicsShape.MoverData ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_ApplyWind_Injected([In] ref PhysicsShape shape, [In] ref Vector2 force, float drag, float lift, bool wake);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetTriggerEvents_Injected([In] ref PhysicsShape shape, bool enableContactEvents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_GetTriggerEvents_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetContactEvents_Injected([In] ref PhysicsShape shape, bool enableContactEvents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_GetContactEvents_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetHitEvents_Injected([In] ref PhysicsShape shape, bool enableHitEvents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_GetHitEvents_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetContacFiltertCallbacks_Injected([In] ref PhysicsShape shape, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_GetContactFilterCallbacks_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetPreSolveCallbacks_Injected([In] ref PhysicsShape shape, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_GetPreSolveCallbacks_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_GetStartStaticContacts_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_GetStartMassUpdate_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_OverlapPoint_Injected([In] ref PhysicsShape shape, [In] ref Vector2 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_ClosestPoint_Injected([In] ref PhysicsShape shape, [In] ref Vector2 point, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CastRay_Injected([In] ref PhysicsShape shape, [In] ref PhysicsQuery.CastRayInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CastShape_Injected([In] ref PhysicsShape shape, [In] ref PhysicsQuery.CastShapeInput input, out PhysicsQuery.CastResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetCircleGeometry_Injected([In] ref PhysicsShape shape, out CircleGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetCapsuleGeometry_Injected([In] ref PhysicsShape shape, out CapsuleGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetPolygonGeometry_Injected([In] ref PhysicsShape shape, out PolygonGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetSegmentGeometry_Injected([In] ref PhysicsShape shape, out SegmentGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetChainSegmentGeometry_Injected([In] ref PhysicsShape shape, out ChainSegmentGeometry ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetCircleGeometry_Injected([In] ref PhysicsShape shape, [In] ref CircleGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetCapsuleGeometry_Injected([In] ref PhysicsShape shape, [In] ref CapsuleGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetPolygonGeometry_Injected([In] ref PhysicsShape shape, [In] ref PolygonGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetSegmentGeometry_Injected([In] ref PhysicsShape shape, [In] ref SegmentGeometry geometry);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_IsChainSegmentShape_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetChain_Injected([In] ref PhysicsShape shape, out PhysicsChain ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetContacts_Injected([In] ref PhysicsShape shape, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetTriggerVisitors_Injected([In] ref PhysicsShape shape, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_CalculateAABB_Injected([In] ref PhysicsShape shape, out PhysicsAABB ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetLocalCenter_Injected([In] ref PhysicsShape shape, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsShape_GetPerimeter_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsShape_GetPerimeterProjected_Injected([In] ref PhysicsShape shape, [In] ref Vector2 axis);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_Draw_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsShape_SetOwner_Injected([In] ref PhysicsShape shape, IntPtr ownerObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr PhysicsShape_GetOwner_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsShape_IsOwned_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetCallbackTarget_Injected([In] ref PhysicsShape shape, object callbackTarget);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object PhysicsShape_GetCallbackTarget_Injected([In] ref PhysicsShape shape);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_SetUserData_Injected([In] ref PhysicsShape shape, [In] ref PhysicsUserData physicsUserData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsShape_GetUserData_Injected([In] ref PhysicsShape shape, out PhysicsUserData ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsContactId_IsValid_Injected([In] ref PhysicsShape.ContactId contactId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsContactId_GetContact_Injected([In] ref PhysicsShape.ContactId contactId, out PhysicsShape.Contact ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetDefaultDefinition_Injected(bool useSettings, out PhysicsWorldDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetDefaultExplosionDefinition_Injected(out PhysicsWorld.ExplosionDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_Create_Injected([In] ref PhysicsWorldDefinition definition, out PhysicsWorld ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_Destroy_Injected([In] ref PhysicsWorld world, int ownerKey);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_WriteDefinition_Injected([In] ref PhysicsWorld world, [In] ref PhysicsWorldDefinition definition, bool onlyExtendedProperties);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_ReadDefinition_Injected([In] ref PhysicsWorld world, out PhysicsWorldDefinition ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_Reset_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_IsValid_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_IsEmpty_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetPaused_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetPaused_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetSleepingAllowed_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetSleepingAllowed_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetContinuousAllowed_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetContinuousAllowed_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetWarmStartingAllowed_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetWarmStartingAllowed_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetContactFilterCallbacks_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetContactFilterCallbacks_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetPreSolveCallbacks_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetPreSolveCallbacks_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetAutoBodyUpdateCallbacks_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetAutoBodyUpdateCallbacks_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetAutoContactCallbacks_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetAutoContactCallbacks_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetAutoTriggerCallbacks_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetAutoTriggerCallbacks_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetAutoJointThresholdCallbacks_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetAutoJointThresholdCallbacks_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetBounceThreshold_Injected([In] ref PhysicsWorld world, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetBounceThreshold_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetContactHitEventThreshold_Injected([In] ref PhysicsWorld world, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetContactHitEventThreshold_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetContactFrequency_Injected([In] ref PhysicsWorld world, float contactFrequency);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetContactFrequency_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetContactDamping_Injected([In] ref PhysicsWorld world, float damping);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetContactDamping_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetContactSpeed_Injected([In] ref PhysicsWorld world, float contactSpeed);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetContactSpeed_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetMaximumLinearSpeed_Injected([In] ref PhysicsWorld world, float maximumLinearSpeed);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetMaximumLinearSpeed_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetGravity_Injected([In] ref PhysicsWorld world, [In] ref Vector2 gravity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetGravity_Injected([In] ref PhysicsWorld world, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetSimulationWorkers_Injected([In] ref PhysicsWorld world, int simulationWorkers);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsWorld_GetSimulationWorkers_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetSimulationType_Injected([In] ref PhysicsWorld world, PhysicsWorld.SimulationType simulationType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsWorld.SimulationType PhysicsWorld_GetSimulationType_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetSimulationSubSteps_Injected([In] ref PhysicsWorld world, int subStepCpunt);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsWorld_GetSimulationSubSteps_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double PhysicsWorld_GetLastSimulationTimestamp_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetLastSimulationDeltaTime_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetTransformPlane_Injected([In] ref PhysicsWorld world, PhysicsWorld.TransformPlane transformPlane);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsWorld.TransformPlane PhysicsWorld_GetTransformPlane_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetTransformWriteMode_Injected([In] ref PhysicsWorld world, PhysicsWorld.TransformWriteMode transformWriteMode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsWorld.TransformWriteMode PhysicsWorld_GetTransformWriteMode_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetTransformTweening_Injected([In] ref PhysicsWorld world, bool flag);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_GetTransformTweening_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_ClearTransformWriteTweens_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetTransformWriteTweens_Injected([In] ref PhysicsWorld world, ref ManagedSpanWrapper transformWriteTweens);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetTransformWriteTweens_Injected([In] ref PhysicsWorld world, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_Simulate_Injected([In] ref PhysicsWorld world, float timeStep, PhysicsWorld.SimulationType expectedSimulationType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SimulateBatch_Injected(ref ManagedSpanWrapper worlds, float timeStep, PhysicsWorld.SimulationType expectedSimulationType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_Explode_Injected([In] ref PhysicsWorld world, [In] ref PhysicsWorld.ExplosionDefinition definition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetBodyUpdateUserData_Injected([In] ref PhysicsWorld world, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetBodyUpdateEvents_Injected([In] ref PhysicsWorld world, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetTriggerBeginEvents_Injected([In] ref PhysicsWorld world, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetTriggerEndEvents_Injected([In] ref PhysicsWorld world, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetContactBeginEvents_Injected([In] ref PhysicsWorld world, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetContactEndEvents_Injected([In] ref PhysicsWorld world, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetContactHitEvents_Injected([In] ref PhysicsWorld world, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetJointThresholdEvents_Injected([In] ref PhysicsWorld world, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetBodyUpdateCallbackTargets_Injected([In] ref PhysicsWorld world, Allocator allocator, out PhysicsCallbacks.BodyUpdateCallbackTargets ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetTriggerCallbackTargets_Injected([In] ref PhysicsWorld world, Allocator allocator, out PhysicsCallbacks.TriggerCallbackTargets ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetContactCallbackTargets_Injected([In] ref PhysicsWorld world, Allocator allocator, out PhysicsCallbacks.ContactCallbackTargets ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetJointThresholdCallbackTargets_Injected([In] ref PhysicsWorld world, Allocator allocator, out PhysicsCallbacks.JointThresholdCallbackTargets ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_TestOverlapAABB_Injected([In] ref PhysicsWorld world, [In] ref PhysicsAABB aabb, [In] ref PhysicsQuery.QueryFilter filter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_TestOverlapShapeProxy_Injected([In] ref PhysicsWorld world, [In] ref PhysicsShape.ShapeProxy shapeProxy, [In] ref PhysicsQuery.QueryFilter filter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_OverlapAABB_Injected([In] ref PhysicsWorld world, [In] ref PhysicsAABB aabb, [In] ref PhysicsQuery.QueryFilter filter, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_OverlapShapeProxy_Injected([In] ref PhysicsWorld world, [In] ref PhysicsShape.ShapeProxy shapeProxy, [In] ref PhysicsQuery.QueryFilter filter, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_CastRay_Injected([In] ref PhysicsWorld world, [In] ref PhysicsQuery.CastRayInput input, [In] ref PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_CastShapeProxy_Injected([In] ref PhysicsWorld world, [In] ref PhysicsShape.ShapeProxy shapeProxy, [In] ref Vector2 translation, [In] ref PhysicsQuery.QueryFilter filter, PhysicsQuery.WorldCastMode castMode, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_CastMover_Injected([In] ref PhysicsWorld world, [In] ref PhysicsQuery.WorldMoverInput input, out PhysicsQuery.WorldMoverResult ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsWorld_GetAwakeBodyCount_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetCounters_Injected([In] ref PhysicsWorld world, out PhysicsWorld.WorldCounters ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetProfile_Injected([In] ref PhysicsWorld world, out PhysicsWorld.WorldProfile ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetGlobalCounters_Injected(out PhysicsWorld.WorldCounters ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetGlobalProfile_Injected(out PhysicsWorld.WorldProfile ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetWorlds_Injected(Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetBodies_Injected([In] ref PhysicsWorld world, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetJoints_Injected([In] ref PhysicsWorld world, Allocator allocator, out PhysicsBuffer ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawOptions_Injected([In] ref PhysicsWorld world, PhysicsWorld.DrawOptions drawOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsWorld.DrawOptions PhysicsWorld_GetDrawOptions_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawFillOptions_Injected([In] ref PhysicsWorld world, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsWorld.DrawFillOptions PhysicsWorld_GetDrawFillOptions_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawColors_Injected([In] ref PhysicsWorld world, [In] ref PhysicsWorld.DrawColors drawColors);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetDrawColors_Injected([In] ref PhysicsWorld world, out PhysicsWorld.DrawColors ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawThickness_Injected([In] ref PhysicsWorld world, float thickness);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetDrawThickness_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawFillAlpha_Injected([In] ref PhysicsWorld world, float alpha);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetDrawFillAlpha_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawPointScale_Injected([In] ref PhysicsWorld world, float scale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetDrawPointScale_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawNormalScale_Injected([In] ref PhysicsWorld world, float scale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetDrawNormalScale_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawImpulseScale_Injected([In] ref PhysicsWorld world, float scale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetDrawImpulseScale_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetDrawCapacity_Injected([In] ref PhysicsWorld world, int capacity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsWorld_GetDrawCapacity_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetElementDepth_Injected([In] ref PhysicsWorld world, float elementDepth);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float PhysicsWorld_GetElementDepth_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetDrawResults_Injected([In] ref PhysicsWorld world, out PhysicsWorld.DrawResults ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_ClearDraw_Injected([In] ref PhysicsWorld world, bool clearWorldDraw, bool clearCustomDraw, bool clearTimedDraw);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_Draw_Injected([In] ref PhysicsWorld world, [In] ref PhysicsAABB viewAABB);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawCircleGeometry_Injected([In] ref PhysicsWorld world, [In] ref CircleGeometry geometry, [In] ref PhysicsTransform transform, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawCapsuleGeometry_Injected([In] ref PhysicsWorld world, [In] ref CapsuleGeometry geometry, [In] ref PhysicsTransform transform, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawPolygonGeometry_Injected([In] ref PhysicsWorld world, [In] ref PolygonGeometry geometry, [In] ref PhysicsTransform transform, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawSegmentGeometry_Injected([In] ref PhysicsWorld world, [In] ref SegmentGeometry geometry, [In] ref PhysicsTransform transform, [In] ref Color color, float lifetime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawCircleGeometrySpan_Injected([In] ref PhysicsWorld world, ref ManagedSpanWrapper geometry, [In] ref PhysicsTransform transform, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawCapsuleGeometrySpan_Injected([In] ref PhysicsWorld world, ref ManagedSpanWrapper geometry, [In] ref PhysicsTransform transform, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawPolygonGeometrySpan_Injected([In] ref PhysicsWorld world, ref ManagedSpanWrapper geometry, [In] ref PhysicsTransform transform, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawSegmentGeometrySpan_Injected([In] ref PhysicsWorld world, ref ManagedSpanWrapper geometry, [In] ref PhysicsTransform transform, [In] ref Color color, float lifetime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawBox_Injected([In] ref PhysicsWorld world, [In] ref PhysicsTransform transform, [In] ref Vector2 size, float radius, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawCircle_Injected([In] ref PhysicsWorld world, [In] ref Vector2 center, float radius, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawCapsule_Injected([In] ref PhysicsWorld world, [In] ref PhysicsTransform transform, [In] ref Vector2 center1, [In] ref Vector2 center2, float radius, [In] ref Color color, float lifetime, PhysicsWorld.DrawFillOptions drawFillOptions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawPoint_Injected([In] ref PhysicsWorld world, [In] ref Vector2 center, float radius, [In] ref Color color, float lifetime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawLine_Injected([In] ref PhysicsWorld world, [In] ref Vector2 point0, [In] ref Vector2 point1, [In] ref Color color, float lifetime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawLineStrip_Injected([In] ref PhysicsWorld world, [In] ref PhysicsTransform transform, ref ManagedSpanWrapper vertices, bool loop, [In] ref Color color, float lifetime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawTransformAxis_Injected([In] ref PhysicsWorld world, [In] ref PhysicsTransform transform, float scale, float lifetime);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr PhysicsWorld_GetRenderMaterial_Injected(ref ManagedSpanWrapper editorResourceName, ref ManagedSpanWrapper playerResourceName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PhysicsWorld_SetOwner_Injected([In] ref PhysicsWorld world, IntPtr ownerObject);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr PhysicsWorld_GetOwner_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_IsOwned_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_SetUserData_Injected([In] ref PhysicsWorld world, [In] ref PhysicsUserData physicsUserData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetUserData_Injected([In] ref PhysicsWorld world, out PhysicsUserData ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PhysicsWorld_IsDefaultWorld_Injected([In] ref PhysicsWorld world);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_DrawAllWorlds_Injected([In] ref PhysicsAABB drawAABB);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PhysicsWorld_GetDefaultWorld_Injected(out PhysicsWorld ret);
}
