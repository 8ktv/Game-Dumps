using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;

namespace UnityEngine.Animations;

[NativeHeader("Runtime/Director/Core/HPlayable.h")]
[NativeHeader("Modules/Animation/AnimationClip.h")]
[NativeHeader("Modules/Animation/Director/AnimationPlayableExtensions.h")]
public static class AnimationPlayableExtensions
{
	public static void SetAnimatedProperties<U>(this U playable, AnimationClip clip) where U : struct, IPlayable
	{
		PlayableHandle playable2 = playable.GetHandle();
		SetAnimatedPropertiesInternal(ref playable2, clip);
	}

	[NativeThrows]
	internal static void SetAnimatedPropertiesInternal(ref PlayableHandle playable, AnimationClip animatedProperties)
	{
		SetAnimatedPropertiesInternal_Injected(ref playable, Object.MarshalledUnityObject.Marshal(animatedProperties));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAnimatedPropertiesInternal_Injected(ref PlayableHandle playable, IntPtr animatedProperties);
}
