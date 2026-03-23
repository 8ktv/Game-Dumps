using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.IO;

public sealed class DigestStream : Stream
{
	private readonly Stream m_stream;

	private readonly IDigest m_readDigest;

	private readonly IDigest m_writeDigest;

	public IDigest ReadDigest => m_readDigest;

	public IDigest WriteDigest => m_writeDigest;

	public override bool CanRead => m_stream.CanRead;

	public override bool CanSeek => false;

	public override bool CanWrite => m_stream.CanWrite;

	public override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	private Stream ReadSource
	{
		get
		{
			if (m_readDigest != null)
			{
				return this;
			}
			return m_stream;
		}
	}

	public DigestStream(Stream stream, IDigest readDigest, IDigest writeDigest)
	{
		m_stream = stream;
		m_readDigest = readDigest;
		m_writeDigest = writeDigest;
	}

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
	{
		return Streams.CopyToAsync(ReadSource, destination, bufferSize, cancellationToken);
	}

	public override void Flush()
	{
		m_stream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = m_stream.Read(buffer, offset, count);
		if (m_readDigest != null && num > 0)
		{
			m_readDigest.BlockUpdate(buffer, offset, num);
		}
		return num;
	}

	public override int ReadByte()
	{
		int num = m_stream.ReadByte();
		if (m_readDigest != null && num >= 0)
		{
			m_readDigest.Update((byte)num);
		}
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long length)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (m_writeDigest != null)
		{
			Streams.ValidateBufferArguments(buffer, offset, count);
			if (count > 0)
			{
				m_writeDigest.BlockUpdate(buffer, offset, count);
			}
		}
		m_stream.Write(buffer, offset, count);
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		if (m_writeDigest != null)
		{
			Streams.ValidateBufferArguments(buffer, offset, count);
			if (count > 0)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Task.FromCanceled(cancellationToken);
				}
				m_writeDigest.BlockUpdate(buffer, offset, count);
			}
		}
		return m_stream.WriteAsync(buffer, offset, count, cancellationToken);
	}

	public override void WriteByte(byte value)
	{
		if (m_writeDigest != null)
		{
			m_writeDigest.Update(value);
		}
		m_stream.WriteByte(value);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			m_stream.Dispose();
		}
		base.Dispose(disposing);
	}
}
