using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[ExcludeFromObjectFactory]
[UsedByNativeCode]
[NativeHeader("Modules/Animation/RuntimeAnimatorController.h")]
public class RuntimeAnimatorController : Object
{
	public AnimationClip[] animationClips
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_animationClips_Injected(intPtr);
		}
	}

	protected RuntimeAnimatorController()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern AnimationClip[] get_animationClips_Injected(IntPtr _unity_self);
}
