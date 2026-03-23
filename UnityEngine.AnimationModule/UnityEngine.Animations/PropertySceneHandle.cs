using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations;

[NativeHeader("Modules/Animation/Director/AnimationSceneHandles.h")]
[MovedFrom("UnityEngine.Experimental.Animations")]
public struct PropertySceneHandle
{
	private uint valid;

	private int handleIndex;

	private bool createdByNative => valid != 0;

	private bool hasHandleIndex => handleIndex != -1;

	public bool IsValid(AnimationStream stream)
	{
		return IsValidInternal(ref stream);
	}

	private bool IsValidInternal(ref AnimationStream stream)
	{
		return stream.isValid && createdByNative && hasHandleIndex && HasValidTransform(ref stream);
	}

	public void Resolve(AnimationStream stream)
	{
		CheckIsValid(ref stream);
		ResolveInternal(ref stream);
	}

	public bool IsResolved(AnimationStream stream)
	{
		return IsValidInternal(ref stream) && IsBound(ref stream);
	}

	private void CheckIsValid(ref AnimationStream stream)
	{
		stream.CheckIsValid();
		if (!createdByNative || !hasHandleIndex)
		{
			throw new InvalidOperationException("The PropertySceneHandle is invalid. Please use proper function to create the handle.");
		}
		if (!HasValidTransform(ref stream))
		{
			throw new NullReferenceException("The transform is invalid.");
		}
	}

	public float GetFloat(AnimationStream stream)
	{
		CheckIsValid(ref stream);
		return GetFloatInternal(ref stream);
	}

	[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
	public void SetFloat(AnimationStream stream, float value)
	{
	}

	public int GetInt(AnimationStream stream)
	{
		CheckIsValid(ref stream);
		return GetIntInternal(ref stream);
	}

	public EntityId GetEntityId(AnimationStream stream)
	{
		CheckIsValid(ref stream);
		return GetEntityIdInternal(ref stream);
	}

	[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
	public void SetInt(AnimationStream stream, int value)
	{
	}

	public bool GetBool(AnimationStream stream)
	{
		CheckIsValid(ref stream);
		return GetBoolInternal(ref stream);
	}

	[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
	public void SetBool(AnimationStream stream, bool value)
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private extern bool HasValidTransform(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private extern bool IsBound(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "Resolve", IsThreadSafe = true)]
	private extern void ResolveInternal(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetFloat", IsThreadSafe = true)]
	private extern float GetFloatInternal(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetInt", IsThreadSafe = true)]
	private extern int GetIntInternal(ref AnimationStream stream);

	[NativeMethod(Name = "GetEntityId", IsThreadSafe = true)]
	private EntityId GetEntityIdInternal(ref AnimationStream stream)
	{
		GetEntityIdInternal_Injected(ref this, ref stream, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetBool", IsThreadSafe = true)]
	private extern bool GetBoolInternal(ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetEntityIdInternal_Injected(ref PropertySceneHandle _unity_self, ref AnimationStream stream, out EntityId ret);
}
