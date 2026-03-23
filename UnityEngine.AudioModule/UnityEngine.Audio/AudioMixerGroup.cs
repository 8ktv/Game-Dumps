using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.Audio;

[NativeHeader("Modules/Audio/Public/AudioMixerGroup.h")]
public class AudioMixerGroup : Object, ISubAssetNotDuplicatable
{
	[NativeProperty]
	public AudioMixer audioMixer
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<AudioMixer>(get_audioMixer_Injected(intPtr));
		}
	}

	internal AudioMixerGroup()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_audioMixer_Injected(IntPtr _unity_self);
}
