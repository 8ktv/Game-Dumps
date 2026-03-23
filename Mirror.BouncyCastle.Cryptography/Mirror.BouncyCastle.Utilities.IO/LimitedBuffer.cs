using System;

namespace Mirror.BouncyCastle.Utilities.IO;

public sealed class LimitedBuffer : BaseOutputStream
{
	private readonly byte[] m_buf;

	private int m_count;

	public int Count => m_count;

	public int Limit => m_buf.Length;

	public LimitedBuffer(int limit)
	{
		m_buf = new byte[limit];
		m_count = 0;
	}

	public int CopyTo(byte[] buffer, int offset)
	{
		Array.Copy(m_buf, 0, buffer, offset, m_count);
		return m_count;
	}

	public void Reset()
	{
		m_count = 0;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Array.Copy(buffer, offset, m_buf, m_count, count);
		m_count += count;
	}

	public override void WriteByte(byte value)
	{
		m_buf[m_count++] = value;
	}
}
