using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mirror.BouncyCastle.Utilities.IO;

public abstract class BaseInputStream : Stream
{
	public sealed override bool CanRead => true;

	public sealed override bool CanSeek => false;

	public sealed override bool CanWrite => false;

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
		return Streams.CopyToAsync(this, destination, bufferSize, cancellationToken);
	}

	public sealed override void Flush()
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		int num = 0;
		try
		{
			while (num < count)
			{
				int num2 = ReadByte();
				if (num2 >= 0)
				{
					buffer[offset + num++] = (byte)num2;
					continue;
				}
				break;
			}
		}
		catch (IOException)
		{
			if (num == 0)
			{
				throw;
			}
		}
		return num;
	}

	public sealed override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public sealed override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public sealed override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}
}
