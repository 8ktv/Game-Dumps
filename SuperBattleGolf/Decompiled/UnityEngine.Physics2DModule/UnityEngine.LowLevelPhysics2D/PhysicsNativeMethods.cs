using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Jobs;
using UnityEngine.Scripting;

namespace UnityEngine.LowLevelPhysics2D;

[RequiredByNativeCode(GenerateProxy = true)]
internal readonly struct PhysicsNativeMethods
{
	private struct FastWriteTransformsJob : IJobParallelForTransform
	{
		public NativeArray<PhysicsBody.TransformWriteTween> TransformWriteTweens;

		[ReadOnly]
		public PhysicsWorld.TransformPlane TransformPlane;

		[ReadOnly]
		public bool TransformTweening;

		public void Execute(int index, TransformAccess transform)
		{
			if (transform.isValid)
			{
				PhysicsBody.TransformWriteTween value = TransformWriteTweens[index];
				value.physicsTransform.GetPositionAndRotation(out var position, out var rotation);
				Vector3 vector = PhysicsMath.ToPosition3D(position, value.positionFrom, TransformPlane);
				Quaternion quaternion = PhysicsMath.ToRotationFast3D(rotation.angle, TransformPlane);
				transform.SetPositionAndRotation(vector, quaternion);
				if (TransformTweening && value.transformWriteMode == PhysicsBody.TransformWriteMode.Extrapolate)
				{
					value.positionFrom = vector;
					value.rotationFrom = quaternion;
					TransformWriteTweens[index] = value;
				}
			}
		}
	}

	private struct Slow3DWriteTransformsJob : IJobParallelForTransform
	{
		public NativeArray<PhysicsBody.TransformWriteTween> TransformWriteTweens;

		[ReadOnly]
		public PhysicsWorld.TransformPlane TransformPlane;

		[ReadOnly]
		public bool TransformTweening;

		public void Execute(int index, TransformAccess transform)
		{
			if (transform.isValid)
			{
				PhysicsBody.TransformWriteTween value = TransformWriteTweens[index];
				value.physicsTransform.GetPositionAndRotation(out var position, out var rotation);
				Vector3 vector = PhysicsMath.ToPosition3D(position, value.positionFrom, TransformPlane);
				Quaternion quaternion = PhysicsMath.ToRotationSlow3D(rotation.angle, value.rotationFrom, TransformPlane);
				transform.SetPositionAndRotation(vector, quaternion);
				if (TransformTweening && value.transformWriteMode == PhysicsBody.TransformWriteMode.Extrapolate)
				{
					value.positionFrom = vector;
					value.rotationFrom = quaternion;
					TransformWriteTweens[index] = value;
				}
			}
		}
	}

	private struct WriteTransformTweensJob : IJobParallelForTransform
	{
		[ReadOnly]
		public NativeArray<PhysicsBody.TransformWriteTween> TransformWriteTweens;

		[ReadOnly]
		public PhysicsWorld.TransformWriteMode TransformWriteMode;

		[ReadOnly]
		public PhysicsWorld.TransformPlane TransformPlane;

		[ReadOnly]
		public float InterpolationTime;

		[ReadOnly]
		public float ExtrapolationTime;

		public void Execute(int index, TransformAccess transform)
		{
			if (!transform.isValid)
			{
				return;
			}
			PhysicsBody.TransformWriteTween transformWriteTween = TransformWriteTweens[index];
			if (transformWriteTween.body.isValid)
			{
				switch (transformWriteTween.transformWriteMode)
				{
				case PhysicsBody.TransformWriteMode.Interpolate:
				{
					Vector3 positionFrom2 = transformWriteTween.positionFrom;
					Quaternion rotationFrom2 = transformWriteTween.rotationFrom;
					PhysicsTransform physicsTransform = transformWriteTween.physicsTransform;
					Vector3 b = PhysicsMath.ToPosition3D(physicsTransform.position, positionFrom2, TransformPlane);
					Quaternion b2 = ((TransformWriteMode == PhysicsWorld.TransformWriteMode.Fast2D) ? PhysicsMath.ToRotationFast3D(physicsTransform.rotation.angle, TransformPlane) : PhysicsMath.ToRotationSlow3D(physicsTransform.rotation.angle, rotationFrom2, TransformPlane));
					Vector3 position2 = Vector3.Lerp(positionFrom2, b, InterpolationTime);
					Quaternion rotation2 = Quaternion.Slerp(rotationFrom2, b2, InterpolationTime);
					transform.SetPositionAndRotation(position2, rotation2);
					break;
				}
				case PhysicsBody.TransformWriteMode.Extrapolate:
				{
					Vector2 linearVelocity = transformWriteTween.linearVelocity;
					Vector3 vector = PhysicsMath.Swizzle(new Vector3(linearVelocity.x * ExtrapolationTime, linearVelocity.y * ExtrapolationTime, 0f), TransformPlane);
					Vector3 positionFrom = transformWriteTween.positionFrom;
					Quaternion rotationFrom = transformWriteTween.rotationFrom;
					float angularVelocity = transformWriteTween.angularVelocity;
					Vector3 position = positionFrom + vector;
					Quaternion rotation = PhysicsMath.AngularVelocityToQuaternion(angularVelocity, ExtrapolationTime, TransformPlane) * rotationFrom;
					transform.SetPositionAndRotation(position, rotation);
					break;
				}
				}
			}
		}
	}

	private static TransformAccessArray[] s_WorldTransformAccessArrays = new TransformAccessArray[128];

	[RequiredByNativeCode]
	private static void CreateWorldTransformAccessArray(PhysicsWorld world, int capacity, int desiredJobCount)
	{
		int num = world.m_Index1 - 1;
		TransformAccessArray transformAccessArray = s_WorldTransformAccessArrays[num];
		if (transformAccessArray.isCreated)
		{
			transformAccessArray.Dispose();
		}
		transformAccessArray = new TransformAccessArray(capacity, desiredJobCount);
		s_WorldTransformAccessArrays[num] = transformAccessArray;
	}

	[RequiredByNativeCode]
	private static void DestroyWorldTransformAccessArray(PhysicsWorld world)
	{
		int num = world.m_Index1 - 1;
		TransformAccessArray transformAccessArray = s_WorldTransformAccessArrays[num];
		if (transformAccessArray.isCreated)
		{
			transformAccessArray.Dispose();
		}
		s_WorldTransformAccessArrays[num] = default(TransformAccessArray);
	}

	private static TransformAccessArray GetWorldTransformAccessArray(PhysicsWorld world)
	{
		int num = world.m_Index1 - 1;
		TransformAccessArray result = s_WorldTransformAccessArrays[num];
		if (result.isCreated)
		{
			return result;
		}
		throw new InvalidOperationException($"Cannot access world transform access array for world {world}");
	}

	[RequiredByNativeCode]
	private unsafe static void WriteWorldTransforms(PhysicsWorld world, PhysicsWorld.TransformWriteMode transformWriteMode, PhysicsWorld.TransformPlane transformPlane, int eventCount, bool transformTweening)
	{
		TransformAccessArray worldTransformAccessArray = GetWorldTransformAccessArray(world);
		NativeArray<PhysicsBody.TransformWriteTween> nativeArray = new NativeArray<PhysicsBody.TransformWriteTween>(eventCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		int num = PhysicsLowLevelScripting2D.PhysicsGlobal_PopulateWorldTransformWrite(world, new IntPtr(&worldTransformAccessArray), nativeArray.AsSpan());
		if (num > 0)
		{
			switch (transformWriteMode)
			{
			case PhysicsWorld.TransformWriteMode.Fast2D:
				new FastWriteTransformsJob
				{
					TransformTweening = transformTweening,
					TransformWriteTweens = nativeArray,
					TransformPlane = transformPlane
				}.Schedule(worldTransformAccessArray).Complete();
				break;
			case PhysicsWorld.TransformWriteMode.Slow3D:
				new Slow3DWriteTransformsJob
				{
					TransformTweening = transformTweening,
					TransformWriteTweens = nativeArray,
					TransformPlane = transformPlane
				}.Schedule(worldTransformAccessArray).Complete();
				break;
			default:
				throw new Exception("Invalid PhysicsWorld Transform Write Mode.");
			}
			if (transformTweening)
			{
				world.SetTransformWriteTweens(new Span<PhysicsBody.TransformWriteTween>(nativeArray.GetUnsafeReadOnlyPtr(), num));
			}
		}
		nativeArray.Dispose();
	}

	[RequiredByNativeCode]
	private static void WriteTransformTweens(PhysicsWorld world, double lastSimulationTimestamp, float lastSimulationDeltaTime, PhysicsWorld.TransformWriteMode transformWriteMode, PhysicsWorld.TransformPlane transformPlane, PhysicsLowLevelScripting2D.PhysicsBuffer transformWriteTweensBuffer)
	{
		if (transformWriteMode != PhysicsWorld.TransformWriteMode.Off && !transformWriteTweensBuffer.IsEmpty)
		{
			NativeArray<PhysicsBody.TransformWriteTween> transformWriteTweens = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray(transformWriteTweensBuffer.ToSpan<PhysicsBody.TransformWriteTween>(), Allocator.None);
			int length = transformWriteTweens.Length;
			TransformAccessArray worldTransformAccessArray = GetWorldTransformAccessArray(world);
			if (length == worldTransformAccessArray.length)
			{
				float num = (float)(Time.timeAsDouble - lastSimulationTimestamp);
				float interpolationTime = Mathf.Clamp01(num / lastSimulationDeltaTime);
				float extrapolationTime = num;
				new WriteTransformTweensJob
				{
					TransformWriteTweens = transformWriteTweens,
					TransformWriteMode = transformWriteMode,
					TransformPlane = transformPlane,
					InterpolationTime = interpolationTime,
					ExtrapolationTime = extrapolationTime
				}.Schedule(worldTransformAccessArray).Complete();
			}
		}
	}
}
