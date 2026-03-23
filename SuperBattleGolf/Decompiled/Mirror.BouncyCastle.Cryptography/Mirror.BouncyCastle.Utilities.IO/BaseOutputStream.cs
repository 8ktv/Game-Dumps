using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mirror.BouncyCastle.Utilities.IO;

public abstract class BaseOutputStream : Stream
{
	public sealed override bool CanRead => false;

	public sealed override bool CanSeek => false;

	public sealed override bool CanWrite => true;

	public sealed override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public sealed override long Position
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

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
	{
		throw new NotSupportedException();
	}

	public override void Flush()
	{
	}

	public sealed override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}

	public sealed override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public sealed override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		for (int i = 0; i < count; i++)
		{
			WriteByte(buffer[offset + i]);
		}
	}

	public virtual void Write(params byte[] buffer)
	{
		Write(buffer, 0, buffer.Length);
	}
}
