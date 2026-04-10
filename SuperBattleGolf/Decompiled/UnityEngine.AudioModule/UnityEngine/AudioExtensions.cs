using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

public static class AudioExtensions
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "AudioSpeakerModeBindings::InternalIAudioSpeakerModeChannelCount", IsFreeFunction = true)]
	internal static extern int InternalIAudioSpeakerModeChannelCount(AudioSpeakerMode speakerMode);

	public static int ChannelCount(this AudioSpeakerMode speakerMode)
	{
		return speakerMode switch
		{
			AudioSpeakerMode.Mono => 1, 
			AudioSpeakerMode.Stereo => 2, 
			AudioSpeakerMode.Quad => 4, 
			AudioSpeakerMode.Surround => 5, 
			AudioSpeakerMode.Mode5point1 => 6, 
			AudioSpeakerMode.Mode7point1 => 8, 
			AudioSpeakerMode.Prologic => 2, 
			_ => throw new ArgumentException("speakerMode"), 
		};
	}
}
