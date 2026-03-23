using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[RequiredByNativeCode]
internal struct AnimationEventBlittable : IDisposable
{
	internal float m_Time;

	internal IntPtr m_FunctionName;

	internal IntPtr m_StringParameter;

	internal IntPtr m_ObjectReferenceParameter;

	internal float m_FloatParameter;

	internal int m_IntParameter;

	internal int m_MessageOptions;

	internal AnimationEventSource m_Source;

	internal IntPtr m_StateSender;

	internal AnimatorStateInfo m_AnimatorStateInfo;

	internal AnimatorClipInfo m_AnimatorClipInfo;

	[ThreadStatic]
	private static GCHandlePool s_handlePool;

	internal static AnimationEventBlittable FromAnimationEvent(AnimationEvent animationEvent)
	{
		if (s_handlePool == null)
		{
			s_handlePool = new GCHandlePool();
		}
		GCHandlePool gCHandlePool = s_handlePool;
		return new AnimationEventBlittable
		{
			m_Time = animationEvent.m_Time,
			m_FunctionName = gCHandlePool.AllocHandleIfNotNull(animationEvent.m_FunctionName),
			m_StringParameter = gCHandlePool.AllocHandleIfNotNull(animationEvent.m_StringParameter),
			m_ObjectReferenceParameter = gCHandlePool.AllocHandleIfNotNull(animationEvent.m_ObjectReferenceParameter),
			m_FloatParameter = animationEvent.m_FloatParameter,
			m_IntParameter = animationEvent.m_IntParameter,
			m_MessageOptions = animationEvent.m_MessageOptions,
			m_Source = animationEvent.m_Source,
			m_StateSender = gCHandlePool.AllocHandleIfNotNull(animationEvent.m_StateSender),
			m_AnimatorStateInfo = animationEvent.m_AnimatorStateInfo,
			m_AnimatorClipInfo = animationEvent.m_AnimatorClipInfo
		};
	}

	internal unsafe static void FromAnimationEvents(AnimationEvent[] animationEvents, AnimationEventBlittable* animationEventBlittables)
	{
		if (s_handlePool == null)
		{
			s_handlePool = new GCHandlePool();
		}
		GCHandlePool gCHandlePool = s_handlePool;
		AnimationEventBlittable* ptr = animationEventBlittables;
		foreach (AnimationEvent animationEvent in animationEvents)
		{
			ptr->m_Time = animationEvent.m_Time;
			ptr->m_FunctionName = gCHandlePool.AllocHandleIfNotNull(animationEvent.m_FunctionName);
			ptr->m_StringParameter = gCHandlePool.AllocHandleIfNotNull(animationEvent.m_StringParameter);
			ptr->m_ObjectReferenceParameter = gCHandlePool.AllocHandleIfNotNull(animationEvent.m_ObjectReferenceParameter);
			ptr->m_FloatParameter = animationEvent.m_FloatParameter;
			ptr->m_IntParameter = animationEvent.m_IntParameter;
			ptr->m_MessageOptions = animationEvent.m_MessageOptions;
			ptr->m_Source = animationEvent.m_Source;
			ptr->m_StateSender = gCHandlePool.AllocHandleIfNotNull(animationEvent.m_StateSender);
			ptr->m_AnimatorStateInfo = animationEvent.m_AnimatorStateInfo;
			ptr->m_AnimatorClipInfo = animationEvent.m_AnimatorClipInfo;
			ptr++;
		}
	}

	[RequiredByNativeCode]
	internal unsafe static AnimationEvent PointerToAnimationEvent(IntPtr animationEventBlittable)
	{
		return ToAnimationEvent(*(AnimationEventBlittable*)(void*)animationEventBlittable);
	}

	internal unsafe static AnimationEvent[] PointerToAnimationEvents(IntPtr animationEventBlittableArray, int size)
	{
		AnimationEvent[] array = new AnimationEvent[size];
		AnimationEventBlittable* ptr = (AnimationEventBlittable*)(void*)animationEventBlittableArray;
		for (int i = 0; i < size; i++)
		{
			array[i] = PointerToAnimationEvent((IntPtr)(ptr + i));
		}
		return array;
	}

	internal unsafe static void DisposeEvents(IntPtr animationEventBlittableArray, int size)
	{
		AnimationEventBlittable* ptr = (AnimationEventBlittable*)(void*)animationEventBlittableArray;
		for (int i = 0; i < size; i++)
		{
			ptr[i].Dispose();
		}
		FreeEventsInternal(animationEventBlittableArray);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "AnimationClipBindings::FreeEventsInternal")]
	private static extern void FreeEventsInternal(IntPtr value);

	internal static AnimationEvent ToAnimationEvent(AnimationEventBlittable animationEventBlittable)
	{
		AnimationEvent animationEvent = new AnimationEvent();
		animationEvent.m_Time = animationEventBlittable.m_Time;
		if (animationEventBlittable.m_FunctionName != IntPtr.Zero)
		{
			animationEvent.m_FunctionName = (string)UnsafeUtility.As<IntPtr, GCHandle>(ref animationEventBlittable.m_FunctionName).Target;
		}
		if (animationEventBlittable.m_StringParameter != IntPtr.Zero)
		{
			animationEvent.m_StringParameter = (string)UnsafeUtility.As<IntPtr, GCHandle>(ref animationEventBlittable.m_StringParameter).Target;
		}
		if (animationEventBlittable.m_ObjectReferenceParameter != IntPtr.Zero)
		{
			animationEvent.m_ObjectReferenceParameter = (Object)UnsafeUtility.As<IntPtr, GCHandle>(ref animationEventBlittable.m_ObjectReferenceParameter).Target;
		}
		animationEvent.m_FloatParameter = animationEventBlittable.m_FloatParameter;
		animationEvent.m_IntParameter = animationEventBlittable.m_IntParameter;
		animationEvent.m_MessageOptions = animationEventBlittable.m_MessageOptions;
		animationEvent.m_Source = animationEventBlittable.m_Source;
		if (animationEventBlittable.m_StateSender != IntPtr.Zero)
		{
			animationEvent.m_StateSender = (AnimationState)UnsafeUtility.As<IntPtr, GCHandle>(ref animationEventBlittable.m_StateSender).Target;
		}
		animationEvent.m_AnimatorStateInfo = animationEventBlittable.m_AnimatorStateInfo;
		animationEvent.m_AnimatorClipInfo = animationEventBlittable.m_AnimatorClipInfo;
		return animationEvent;
	}

	public void Dispose()
	{
		if (s_handlePool == null)
		{
			s_handlePool = new GCHandlePool();
		}
		GCHandlePool gCHandlePool = s_handlePool;
		if (m_FunctionName != IntPtr.Zero)
		{
			gCHandlePool.Free(UnsafeUtility.As<IntPtr, GCHandle>(ref m_FunctionName));
		}
		if (m_StringParameter != IntPtr.Zero)
		{
			gCHandlePool.Free(UnsafeUtility.As<IntPtr, GCHandle>(ref m_StringParameter));
		}
		if (m_ObjectReferenceParameter != IntPtr.Zero)
		{
			gCHandlePool.Free(UnsafeUtility.As<IntPtr, GCHandle>(ref m_ObjectReferenceParameter));
		}
		if (m_StateSender != IntPtr.Zero)
		{
			gCHandlePool.Free(UnsafeUtility.As<IntPtr, GCHandle>(ref m_StateSender));
		}
	}
}
