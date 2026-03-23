using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mirror.BouncyCastle.Utilities.IO;

public static class Streams
{
	private static readonly int MaxStackAlloc = (Platform.Is64BitProcess ? 4096 : 1024);

	public static int DefaultBufferSize => MaxStackAlloc;

	public static void CopyTo(Stream source, Stream destination)
	{
		CopyTo(source, destination, DefaultBufferSize);
	}

	public static void CopyTo(Stream source, Stream destination, int bufferSize)
	{
		byte[] array = new byte[bufferSize];
		int count;
		while ((count = source.Read(array, 0, array.Length)) != 0)
		{
			destination.Write(array, 0, count);
		}
	}

	public static Task CopyToAsync(Stream source, Stream destination)
	{
		return CopyToAsync(source, destination, DefaultBufferSize);
	}

	public static Task CopyToAsync(Stream source, Stream destination, int bufferSize)
	{
		return CopyToAsync(source, destination, bufferSize, CancellationToken.None);
	}

	public static Task CopyToAsync(Stream source, Stream destination, CancellationToken cancellationToken)
	{
		return CopyToAsync(source, destination, DefaultBufferSize, cancellationToken);
	}

	public static async Task CopyToAsync(Stream source, Stream destination, int bufferSize, CancellationToken cancellationToken)
	{
		byte[] buffer = new byte[bufferSize];
		int count;
		while ((count = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) != 0)
		{
			await destination.WriteAsync(buffer, 0, count, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public static void Drain(Stream inStr)
	{
		CopyTo(inStr, Stream.Null, DefaultBufferSize);
	}

	public static void PipeAll(Stream inStr, Stream outStr)
	{
		PipeAll(inStr, outStr, DefaultBufferSize);
	}

	public static void PipeAll(Stream inStr, Stream outStr, int bufferSize)
	{
		CopyTo(inStr, outStr, bufferSize);
	}

	public static long PipeAllLimited(Stream inStr, long limit, Stream outStr)
	{
		return PipeAllLimited(inStr, limit, outStr, DefaultBufferSize);
	}

	public static long PipeAllLimited(Stream inStr, long limit, Stream outStr, int bufferSize)
	{
		LimitedInputStream limitedInputStream = new LimitedInputStream(inStr, limit);
		CopyTo(limitedInputStream, outStr, bufferSize);
		return limit - limitedInputStream.CurrentLimit;
	}

	public static byte[] ReadAll(Stream inStr)
	{
		MemoryStream memoryStream = new MemoryStream();
		PipeAll(inStr, memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] ReadAll(MemoryStream inStr)
	{
		return inStr.ToArray();
	}

	public static byte[] ReadAllLimited(Stream inStr, int limit)
	{
		MemoryStream memoryStream = new MemoryStream();
		PipeAllLimited(inStr, limit, memoryStream);
		return memoryStream.ToArray();
	}

	public static int ReadFully(Stream inStr, byte[] buf)
	{
		return ReadFully(inStr, buf, 0, buf.Length);
	}

	public static int ReadFully(Stream inStr, byte[] buf, int off, int len)
	{
		int i;
		int num;
		for (i = 0; i < len; i += num)
		{
			num = inStr.Read(buf, off + i, len - i);
			if (num < 1)
			{
				break;
			}
		}
		return i;
	}

	public static void ValidateBufferArguments(byte[] buffer, int offset, int count)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		int num = buffer.Length - offset;
		if ((offset | num) < 0)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		int num2 = num - count;
		if ((count | num2) < 0)
		{
			throw new ArgumentOutOfRangeException("count");
		}
	}

	internal static async Task WriteAsyncCompletion(Task writeTask, byte[] localBuffer)
	{
		try
		{
			await writeTask.ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			Array.Clear(localBuffer, 0, localBuffer.Length);
		}
	}

	internal static Task WriteAsyncDirect(Stream destination, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		destination.Write(buffer, offset, count);
		return Task.CompletedTask;
	}

	public static int WriteBufTo(MemoryStream buf, byte[] output, int offset)
	{
		int num = Convert.ToInt32(buf.Length);
		buf.WriteTo(new MemoryStream(output, offset, num));
		return num;
	}
}
