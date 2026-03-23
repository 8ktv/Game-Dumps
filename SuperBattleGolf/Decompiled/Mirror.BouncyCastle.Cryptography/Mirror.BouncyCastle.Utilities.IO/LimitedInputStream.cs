using System.IO;

namespace Mirror.BouncyCastle.Utilities.IO;

internal sealed class LimitedInputStream : BaseInputStream
{
	private readonly Stream m_stream;

	private long m_limit;

	internal long CurrentLimit => m_limit;

	internal LimitedInputStream(Stream stream, long limit)
	{
		m_stream = stream;
		m_limit = limit;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = m_stream.Read(buffer, offset, count);
		if (num > 0 && (m_limit -= num) < 0)
		{
			throw new StreamOverflowException("Data Overflow");
		}
		return num;
	}

	public override int ReadByte()
	{
		int num = m_stream.ReadByte();
		if (num >= 0 && --m_limit < 0)
		{
			throw new StreamOverflowException("Data Overflow");
		}
		return num;
	}
}
