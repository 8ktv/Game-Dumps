namespace UnityEngine.Audio;

public struct AudioFormat
{
	private AudioConfiguration m_Config;

	public readonly int channelCount => m_Config.speakerMode.ChannelCount();

	public readonly int bufferFrameCount => m_Config.dspBufferSize;

	public readonly int sampleRate => m_Config.sampleRate;

	public readonly AudioSpeakerMode speakerMode => m_Config.speakerMode;

	internal readonly AudioConfiguration audioConfiguration => m_Config;

	public AudioFormat(AudioConfiguration config)
	{
		m_Config = config;
	}

	public AudioFormat(AudioSpeakerMode speakerMode, int sampleRate, int bufferSize)
	{
		m_Config = new AudioConfiguration
		{
			sampleRate = sampleRate,
			dspBufferSize = bufferSize,
			speakerMode = speakerMode
		};
	}
}
