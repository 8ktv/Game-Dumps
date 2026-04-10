using System;

namespace UnityEngine.Audio;

public ref struct ChannelBuffer
{
	internal Span<float> Buffer;

	private int m_ChannelCount;

	private int m_FrameCount;

	public int channelCount => m_ChannelCount;

	public int frameCount => m_FrameCount;

	public float this[int channel, int frame]
	{
		get
		{
			return Buffer[frame * m_ChannelCount + channel];
		}
		set
		{
			Buffer[frame * m_ChannelCount + channel] = value;
		}
	}

	public void Clear()
	{
		Buffer.Clear();
	}

	public ChannelBuffer(Span<float> buffer, int channels)
	{
		if (channels < 1)
		{
			throw new ArgumentException("channels must be positive and non-zero");
		}
		Buffer = buffer;
		m_ChannelCount = channels;
		m_FrameCount = buffer.Length / channels;
	}
}
