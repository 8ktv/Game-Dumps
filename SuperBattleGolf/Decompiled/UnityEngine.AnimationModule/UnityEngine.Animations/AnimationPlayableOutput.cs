using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations;

[StaticAccessor("AnimationPlayableOutputBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Modules/Animation/Director/AnimationPlayableOutput.h")]
[NativeHeader("Modules/Animation/Animator.h")]
[NativeHeader("Runtime/Director/Core/HPlayableGraph.h")]
[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
[RequiredByNativeCode]
[NativeHeader("Modules/Animation/ScriptBindings/AnimationPlayableOutput.bindings.h")]
public struct AnimationPlayableOutput : IPlayableOutput
{
	private PlayableOutputHandle m_Handle;

	public static AnimationPlayableOutput Null => new AnimationPlayableOutput(PlayableOutputHandle.Null);

	public static AnimationPlayableOutput Create(PlayableGraph graph, string name, Animator target)
	{
		if (!AnimationPlayableGraphExtensions.InternalCreateAnimationOutput(ref graph, name, out var handle))
		{
			return Null;
		}
		AnimationPlayableOutput result = new AnimationPlayableOutput(handle);
		result.SetTarget(target);
		return result;
	}

	internal AnimationPlayableOutput(PlayableOutputHandle handle)
	{
		if (handle.IsValid() && !handle.IsPlayableOutputOfType<AnimationPlayableOutput>())
		{
			throw new InvalidCastException("Can't set handle: the playable is not an AnimationPlayableOutput.");
		}
		m_Handle = handle;
	}

	public PlayableOutputHandle GetHandle()
	{
		return m_Handle;
	}

	public static implicit operator PlayableOutput(AnimationPlayableOutput output)
	{
		return new PlayableOutput(output.GetHandle());
	}

	public static explicit operator AnimationPlayableOutput(PlayableOutput output)
	{
		return new AnimationPlayableOutput(output.GetHandle());
	}

	public Animator GetTarget()
	{
		return InternalGetTarget(ref m_Handle);
	}

	public void SetTarget(Animator value)
	{
		InternalSetTarget(ref m_Handle, value);
	}

	[NativeThrows]
	private static Animator InternalGetTarget(ref PlayableOutputHandle handle)
	{
		return Unmarshal.UnmarshalUnityObject<Animator>(InternalGetTarget_Injected(ref handle));
	}

	[NativeThrows]
	private static void InternalSetTarget(ref PlayableOutputHandle handle, Animator target)
	{
		InternalSetTarget_Injected(ref handle, Object.MarshalledUnityObject.Marshal(target));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr InternalGetTarget_Injected(ref PlayableOutputHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetTarget_Injected(ref PlayableOutputHandle handle, IntPtr target);
}
