using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations;

[MovedFrom("UnityEngine.Experimental.Animations")]
[NativeHeader("Modules/Animation/ScriptBindings/AnimationStream.bindings.h")]
[NativeHeader("Modules/Animation/Director/AnimationStream.h")]
[RequiredByNativeCode]
public struct AnimationStream
{
	private uint m_AnimatorBindingsVersion;

	private IntPtr constant;

	private IntPtr input;

	private IntPtr output;

	private IntPtr workspace;

	private IntPtr inputStreamAccessor;

	private IntPtr animationHandleBinder;

	internal const int InvalidIndex = -1;

	internal uint animatorBindingsVersion => m_AnimatorBindingsVersion;

	public bool isValid => m_AnimatorBindingsVersion >= 2 && constant != IntPtr.Zero && input != IntPtr.Zero && output != IntPtr.Zero && workspace != IntPtr.Zero && animationHandleBinder != IntPtr.Zero;

	public float deltaTime
	{
		get
		{
			CheckIsValid();
			return GetDeltaTime();
		}
	}

	public Vector3 velocity
	{
		get
		{
			CheckIsValid();
			return GetVelocity();
		}
		set
		{
			CheckIsValid();
			SetVelocity(value);
		}
	}

	public Vector3 angularVelocity
	{
		get
		{
			CheckIsValid();
			return GetAngularVelocity();
		}
		set
		{
			CheckIsValid();
			SetAngularVelocity(value);
		}
	}

	public Vector3 rootMotionPosition
	{
		get
		{
			CheckIsValid();
			return GetRootMotionPosition();
		}
	}

	public Quaternion rootMotionRotation
	{
		get
		{
			CheckIsValid();
			return GetRootMotionRotation();
		}
	}

	public bool isHumanStream
	{
		get
		{
			CheckIsValid();
			return GetIsHumanStream();
		}
	}

	public int inputStreamCount
	{
		get
		{
			CheckIsValid();
			return GetInputStreamCount();
		}
	}

	internal void CheckIsValid()
	{
		if (!isValid)
		{
			throw new InvalidOperationException("The AnimationStream is invalid.");
		}
	}

	public AnimationHumanStream AsHuman()
	{
		CheckIsValid();
		if (!GetIsHumanStream())
		{
			throw new InvalidOperationException("Cannot create an AnimationHumanStream for a generic rig.");
		}
		return GetHumanStream();
	}

	public AnimationStream GetInputStream(int index)
	{
		CheckIsValid();
		return InternalGetInputStream(index);
	}

	public float GetInputWeight(int index)
	{
		CheckIsValid();
		return InternalGetInputWeight(index);
	}

	public void CopyAnimationStreamMotion(AnimationStream animationStream)
	{
		CheckIsValid();
		animationStream.CheckIsValid();
		CopyAnimationStreamMotionInternal(animationStream);
	}

	private void ReadSceneTransforms()
	{
		CheckIsValid();
		InternalReadSceneTransforms();
	}

	private void WriteSceneTransforms()
	{
		CheckIsValid();
		InternalWriteSceneTransforms();
	}

	[NativeMethod(Name = "AnimationStreamBindings::CopyAnimationStreamMotion", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void CopyAnimationStreamMotionInternal(AnimationStream animationStream)
	{
		CopyAnimationStreamMotionInternal_Injected(ref this, ref animationStream);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern float GetDeltaTime();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern bool GetIsHumanStream();

	[NativeMethod(Name = "AnimationStreamBindings::GetVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 GetVelocity()
	{
		GetVelocity_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationStreamBindings::SetVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void SetVelocity(Vector3 velocity)
	{
		SetVelocity_Injected(ref this, ref velocity);
	}

	[NativeMethod(Name = "AnimationStreamBindings::GetAngularVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 GetAngularVelocity()
	{
		GetAngularVelocity_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationStreamBindings::SetAngularVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private void SetAngularVelocity(Vector3 velocity)
	{
		SetAngularVelocity_Injected(ref this, ref velocity);
	}

	[NativeMethod(Name = "AnimationStreamBindings::GetRootMotionPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Vector3 GetRootMotionPosition()
	{
		GetRootMotionPosition_Injected(ref this, out var ret);
		return ret;
	}

	[NativeMethod(Name = "AnimationStreamBindings::GetRootMotionRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
	private Quaternion GetRootMotionRotation()
	{
		GetRootMotionRotation_Injected(ref this, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true)]
	private extern int GetInputStreamCount();

	[NativeMethod(Name = "GetInputStream", IsThreadSafe = true)]
	private AnimationStream InternalGetInputStream(int index)
	{
		InternalGetInputStream_Injected(ref this, index, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "GetInputWeight", IsThreadSafe = true)]
	private extern float InternalGetInputWeight(int index);

	[NativeMethod(IsThreadSafe = true)]
	private AnimationHumanStream GetHumanStream()
	{
		GetHumanStream_Injected(ref this, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "ReadSceneTransforms", IsThreadSafe = true)]
	private extern void InternalReadSceneTransforms();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "WriteSceneTransforms", IsThreadSafe = true)]
	private extern void InternalWriteSceneTransforms();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyAnimationStreamMotionInternal_Injected(ref AnimationStream _unity_self, [In] ref AnimationStream animationStream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVelocity_Injected(ref AnimationStream _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVelocity_Injected(ref AnimationStream _unity_self, [In] ref Vector3 velocity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAngularVelocity_Injected(ref AnimationStream _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAngularVelocity_Injected(ref AnimationStream _unity_self, [In] ref Vector3 velocity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRootMotionPosition_Injected(ref AnimationStream _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetRootMotionRotation_Injected(ref AnimationStream _unity_self, out Quaternion ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalGetInputStream_Injected(ref AnimationStream _unity_self, int index, out AnimationStream ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetHumanStream_Injected(ref AnimationStream _unity_self, out AnimationHumanStream ret);
}
