using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Tls;

internal class TlsStream : Stream
{
	private readonly TlsProtocol m_handler;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

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

	internal TlsStream(TlsProtocol handler)
	{
		m_handler = handler;
	}

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
	{
		return Streams.CopyToAsync(this, destination, bufferSize, cancellationToken);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			m_handler.Close();
		}
		base.Dispose(disposing);
	}

	public override void Flush()
	{
		m_handler.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return m_handler.ReadApplicationData(buffer, offset, count);
	}

	public override int ReadByte()
	{
		byte[] array = new byte[1];
		if (m_handler.ReadApplicationData(array, 0, 1) > 0)
		{
			return array[0];
		}
		return -1;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		m_handler.WriteApplicationData(buffer, offset, count);
	}

	public override void WriteByte(byte value)
	{
		m_handler.WriteApplicationData(new byte[1] { value }, 0, 1);
	}
}
