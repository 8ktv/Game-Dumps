using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Audio;

[NativeHeader("AudioScriptingClasses.h")]
[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioClipExtensions.bindings.h")]
[NativeHeader("Modules/Audio/Public/AudioClip.h")]
internal static class AudioClipExtensionsInternal
{
	[NativeMethod(IsFreeFunction = true, ThrowsException = true)]
	public static uint Internal_CreateAudioClipSampleProvider([NotNull] this AudioClip audioClip, ulong start, long end, bool loop, bool allowDrop, bool loopPointIsStart = false)
	{
		if ((object)audioClip == null)
		{
			ThrowHelper.ThrowArgumentNullException(audioClip, "audioClip");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(audioClip);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(audioClip, "audioClip");
		}
		return Internal_CreateAudioClipSampleProvider_Injected(intPtr, start, end, loop, allowDrop, loopPointIsStart);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint Internal_CreateAudioClipSampleProvider_Injected(IntPtr audioClip, ulong start, long end, bool loop, bool allowDrop, bool loopPointIsStart);
}
