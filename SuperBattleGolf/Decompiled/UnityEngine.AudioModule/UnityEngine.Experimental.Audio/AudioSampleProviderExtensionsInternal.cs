using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Audio;

[NativeHeader("Modules/Audio/Public/ScriptBindings/AudioSampleProviderExtensions.bindings.h")]
[StaticAccessor("AudioSampleProviderExtensionsBindings", StaticAccessorType.DoubleColon)]
internal static class AudioSampleProviderExtensionsInternal
{
	public static float GetSpeed(this AudioSampleProvider provider)
	{
		return InternalGetAudioSampleProviderSpeed(provider.id);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
	private static extern float InternalGetAudioSampleProviderSpeed(uint providerId);
}
