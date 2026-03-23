using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

internal class ParticleSystemExtensionsImpl
{
	[FreeFunction(Name = "ParticleSystemScriptBindings::GetSafeCollisionEventSize")]
	internal static int GetSafeCollisionEventSize([NotNull] ParticleSystem ps)
	{
		if ((object)ps == null)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(ps);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		return GetSafeCollisionEventSize_Injected(intPtr);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetCollisionEventsDeprecated")]
	internal unsafe static int GetCollisionEventsDeprecated([NotNull] ParticleSystem ps, GameObject go, [Out] ParticleCollisionEvent[] collisionEvents)
	{
		//The blocks IL_0045 are reachable both inside and outside the pinned region starting at IL_002e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)ps == null)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		BlittableArrayWrapper collisionEvents2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(ps);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(ps, "ps");
			}
			IntPtr go2 = Object.MarshalledUnityObject.Marshal(go);
			if (collisionEvents != null)
			{
				fixed (ParticleCollisionEvent[] array = collisionEvents)
				{
					if (array.Length != 0)
					{
						collisionEvents2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					return GetCollisionEventsDeprecated_Injected(intPtr, go2, out collisionEvents2);
				}
			}
			return GetCollisionEventsDeprecated_Injected(intPtr, go2, out collisionEvents2);
		}
		finally
		{
			collisionEvents2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetSafeTriggerParticlesSize")]
	internal static int GetSafeTriggerParticlesSize([NotNull] ParticleSystem ps, int type)
	{
		if ((object)ps == null)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(ps);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		return GetSafeTriggerParticlesSize_Injected(intPtr, type);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetCollisionEvents")]
	internal unsafe static int GetCollisionEvents([NotNull] ParticleSystem ps, [NotNull] GameObject go, [NotNull] List<ParticleCollisionEvent> collisionEvents)
	{
		if ((object)ps == null)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		if ((object)go == null)
		{
			ThrowHelper.ThrowArgumentNullException(go, "go");
		}
		if (collisionEvents == null)
		{
			ThrowHelper.ThrowArgumentNullException(collisionEvents, "collisionEvents");
		}
		List<ParticleCollisionEvent> list = default(List<ParticleCollisionEvent>);
		BlittableListWrapper collisionEvents2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(ps);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(ps, "ps");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(go);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			list = collisionEvents;
			fixed (ParticleCollisionEvent[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				collisionEvents2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetCollisionEvents_Injected(intPtr, intPtr2, ref collisionEvents2);
			}
		}
		finally
		{
			collisionEvents2.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetTriggerParticles")]
	internal unsafe static int GetTriggerParticles([NotNull] ParticleSystem ps, int type, [NotNull] List<ParticleSystem.Particle> particles)
	{
		if ((object)ps == null)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		if (particles == null)
		{
			ThrowHelper.ThrowArgumentNullException(particles, "particles");
		}
		List<ParticleSystem.Particle> list = default(List<ParticleSystem.Particle>);
		BlittableListWrapper particles2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(ps);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(ps, "ps");
			}
			list = particles;
			fixed (ParticleSystem.Particle[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				particles2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetTriggerParticles_Injected(intPtr, type, ref particles2);
			}
		}
		finally
		{
			particles2.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetTriggerParticlesWithData")]
	internal unsafe static int GetTriggerParticlesWithData([NotNull] ParticleSystem ps, int type, [NotNull] List<ParticleSystem.Particle> particles, ref ParticleSystem.ColliderData colliderData)
	{
		if ((object)ps == null)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		if (particles == null)
		{
			ThrowHelper.ThrowArgumentNullException(particles, "particles");
		}
		List<ParticleSystem.Particle> list = default(List<ParticleSystem.Particle>);
		BlittableListWrapper particles2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(ps);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(ps, "ps");
			}
			list = particles;
			fixed (ParticleSystem.Particle[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				particles2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetTriggerParticlesWithData_Injected(intPtr, type, ref particles2, ref colliderData);
			}
		}
		finally
		{
			particles2.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::SetTriggerParticles")]
	internal unsafe static void SetTriggerParticles([NotNull] ParticleSystem ps, int type, [NotNull] List<ParticleSystem.Particle> particles, int offset, int count)
	{
		if ((object)ps == null)
		{
			ThrowHelper.ThrowArgumentNullException(ps, "ps");
		}
		if (particles == null)
		{
			ThrowHelper.ThrowArgumentNullException(particles, "particles");
		}
		List<ParticleSystem.Particle> list = default(List<ParticleSystem.Particle>);
		BlittableListWrapper particles2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(ps);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(ps, "ps");
			}
			list = particles;
			fixed (ParticleSystem.Particle[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				particles2 = new BlittableListWrapper(arrayWrapper, list.Count);
				SetTriggerParticles_Injected(intPtr, type, ref particles2, offset, count);
			}
		}
		finally
		{
			particles2.Unmarshal(list);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSafeCollisionEventSize_Injected(IntPtr ps);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCollisionEventsDeprecated_Injected(IntPtr ps, IntPtr go, out BlittableArrayWrapper collisionEvents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetSafeTriggerParticlesSize_Injected(IntPtr ps, int type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCollisionEvents_Injected(IntPtr ps, IntPtr go, ref BlittableListWrapper collisionEvents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetTriggerParticles_Injected(IntPtr ps, int type, ref BlittableListWrapper particles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetTriggerParticlesWithData_Injected(IntPtr ps, int type, ref BlittableListWrapper particles, ref ParticleSystem.ColliderData colliderData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTriggerParticles_Injected(IntPtr ps, int type, ref BlittableListWrapper particles, int offset, int count);
}
