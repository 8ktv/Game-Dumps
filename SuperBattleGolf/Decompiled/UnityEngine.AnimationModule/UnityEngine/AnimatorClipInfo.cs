using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/Animation/AnimatorInfo.h")]
[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
[UsedByNativeCode]
public struct AnimatorClipInfo
{
	private int m_ClipInstanceID;

	private float m_Weight;

	public AnimationClip clip => (m_ClipInstanceID != 0) ? InstanceIDToAnimationClipPPtr(m_ClipInstanceID) : null;

	public float weight => m_Weight;

	[FreeFunction("AnimationBindings::InstanceIDToAnimationClipPPtr")]
	private static AnimationClip InstanceIDToAnimationClipPPtr(EntityId entityId)
	{
		return Unmarshal.UnmarshalUnityObject<AnimationClip>(InstanceIDToAnimationClipPPtr_Injected(ref entityId));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr InstanceIDToAnimationClipPPtr_Injected([In] ref EntityId entityId);
}
