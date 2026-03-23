using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mirror.BouncyCastle.Utilities.IO;

public class PushbackStream : FilterStream
{
	private int m_buf = -1;

	public PushbackStream(Stream s)
		: base(s)
	{
	}

	public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
	{
		if (m_buf != -1)
		{
			byte[] buffer = new byte[1] { (byte)m_buf };
			await destination.WriteAsync(buffer, 0, 1, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			m_buf = -1;
		}
		await Streams.CopyToAsync(s, destination, bufferSize, cancellationToken);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (m_buf != -1)
		{
			if (count < 1)
			{
				return 0;
			}
			buffer[offset] = (byte)m_buf;
			m_buf = -1;
			return 1;
		}
		return s.Read(buffer, offset, count);
	}

	public override int ReadByte()
	{
		if (m_buf != -1)
		{
			int buf = m_buf;
			m_buf = -1;
			return buf;
		}
		return s.ReadByte();
	}

	public virtual void Unread(int b)
	{
		if (m_buf != -1)
		{
			throw new InvalidOperationException("Can only push back one byte");
		}
		m_buf = b & 0xFF;
	}
}
