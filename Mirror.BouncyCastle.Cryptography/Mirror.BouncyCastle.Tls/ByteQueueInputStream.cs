using System;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Tls;

public sealed class ByteQueueInputStream : BaseInputStream
{
	private readonly ByteQueue m_buffer;

	public int Available => m_buffer.Available;

	public ByteQueueInputStream()
	{
		m_buffer = new ByteQueue();
	}

	public void AddBytes(byte[] buf)
	{
		m_buffer.AddData(buf, 0, buf.Length);
	}

	public void AddBytes(byte[] buf, int bufOff, int bufLen)
	{
		m_buffer.AddData(buf, bufOff, bufLen);
	}

	public int Peek(byte[] buf)
	{
		int num = System.Math.Min(m_buffer.Available, buf.Length);
		m_buffer.Read(buf, 0, num, 0);
		return num;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		int num = System.Math.Min(m_buffer.Available, count);
		m_buffer.RemoveData(buffer, offset, num, 0);
		return num;
	}

	public override int ReadByte()
	{
		if (m_buffer.Available == 0)
		{
			return -1;
		}
		return m_buffer.RemoveData(1, 0)[0];
	}

	public long Skip(long n)
	{
		int num = System.Math.Min((int)n, m_buffer.Available);
		m_buffer.RemoveData(num);
		return num;
	}
}
