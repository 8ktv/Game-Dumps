using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Crypto.IO;

public sealed class CipherStream : Stream
{
	private readonly Stream m_stream;

	private readonly IBufferedCipher m_readCipher;

	private readonly IBufferedCipher m_writeCipher;

	private byte[] m_readBuf;

	private int m_readBufPos;

	private bool m_readEnded;

	public IBufferedCipher ReadCipher => m_readCipher;

	public IBufferedCipher WriteCipher => m_writeCipher;

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
			if (m_readCipher != null)
			{
				return this;
			}
			return m_stream;
		}
	}

	public CipherStream(Stream stream, IBufferedCipher readCipher, IBufferedCipher writeCipher)
	{
		m_stream = stream;
		if (readCipher != null)
		{
			m_readCipher = readCipher;
			m_readBuf = null;
		}
		if (writeCipher != null)
		{
			m_writeCipher = writeCipher;
		}
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
		if (m_readCipher == null)
		{
			return m_stream.Read(buffer, offset, count);
		}
		Streams.ValidateBufferArguments(buffer, offset, count);
		int i;
		int num;
		for (i = 0; i < count; i += num)
		{
			if ((m_readBuf == null || m_readBufPos >= m_readBuf.Length) && !FillInBuf())
			{
				break;
			}
			num = System.Math.Min(count - i, m_readBuf.Length - m_readBufPos);
			Array.Copy(m_readBuf, m_readBufPos, buffer, offset + i, num);
			m_readBufPos += num;
		}
		return i;
	}

	public override int ReadByte()
	{
		if (m_readCipher == null)
		{
			return m_stream.ReadByte();
		}
		if ((m_readBuf == null || m_readBufPos >= m_readBuf.Length) && !FillInBuf())
		{
			return -1;
		}
		return m_readBuf[m_readBufPos++];
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
		if (m_writeCipher == null)
		{
			m_stream.Write(buffer, offset, count);
			return;
		}
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count <= 0)
		{
			return;
		}
		byte[] array = new byte[m_writeCipher.GetUpdateOutputSize(count)];
		int num = m_writeCipher.ProcessBytes(buffer, offset, count, array, 0);
		if (num <= 0)
		{
			return;
		}
		try
		{
			m_stream.Write(array, 0, num);
		}
		finally
		{
			Array.Clear(array, 0, array.Length);
		}
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		if (m_writeCipher == null)
		{
			return m_stream.WriteAsync(buffer, offset, count, cancellationToken);
		}
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count > 0)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			byte[] array = new byte[m_writeCipher.GetUpdateOutputSize(count)];
			int num = m_writeCipher.ProcessBytes(buffer, offset, count, array, 0);
			if (num > 0)
			{
				return Streams.WriteAsyncCompletion(m_stream.WriteAsync(array, 0, num, cancellationToken), array);
			}
		}
		return Task.CompletedTask;
	}

	public override void WriteByte(byte value)
	{
		if (m_writeCipher == null)
		{
			m_stream.WriteByte(value);
			return;
		}
		byte[] array = m_writeCipher.ProcessByte(value);
		if (array == null)
		{
			return;
		}
		try
		{
			m_stream.Write(array, 0, array.Length);
		}
		finally
		{
			Array.Clear(array, 0, array.Length);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (m_writeCipher != null)
			{
				byte[] array = new byte[m_writeCipher.GetOutputSize(0)];
				int count = m_writeCipher.DoFinal(array, 0);
				m_stream.Write(array, 0, count);
				Array.Clear(array, 0, array.Length);
			}
			m_stream.Dispose();
		}
		base.Dispose(disposing);
	}

	private bool FillInBuf()
	{
		if (m_readEnded)
		{
			return false;
		}
		m_readBufPos = 0;
		do
		{
			m_readBuf = ReadAndProcessBlock();
		}
		while (!m_readEnded && m_readBuf == null);
		return m_readBuf != null;
	}

	private byte[] ReadAndProcessBlock()
	{
		int blockSize = m_readCipher.GetBlockSize();
		byte[] array = new byte[(blockSize == 0) ? 256 : blockSize];
		int num = 0;
		do
		{
			int num2 = m_stream.Read(array, num, array.Length - num);
			if (num2 < 1)
			{
				m_readEnded = true;
				break;
			}
			num += num2;
		}
		while (num < array.Length);
		byte[] array2 = (m_readEnded ? m_readCipher.DoFinal(array, 0, num) : m_readCipher.ProcessBytes(array));
		if (array2 != null && array2.Length == 0)
		{
			array2 = null;
		}
		return array2;
	}
}
