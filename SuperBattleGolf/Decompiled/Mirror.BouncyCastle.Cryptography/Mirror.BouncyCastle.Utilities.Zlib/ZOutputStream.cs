using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Utilities.Zlib;

public class ZOutputStream : BaseOutputStream
{
	private const int BufferSize = 4096;

	protected ZStream z;

	protected int flushLevel;

	protected byte[] buf = new byte[4096];

	protected byte[] buf1 = new byte[1];

	protected bool compress;

	protected Stream output;

	protected bool closed;

	public virtual int FlushMode
	{
		get
		{
			return flushLevel;
		}
		set
		{
			flushLevel = value;
		}
	}

	public virtual long TotalIn => z.total_in;

	public virtual long TotalOut => z.total_out;

	private static ZStream GetDefaultZStream(bool nowrap)
	{
		ZStream zStream = new ZStream();
		zStream.inflateInit(nowrap);
		return zStream;
	}

	public ZOutputStream(Stream output)
		: this(output, nowrap: false)
	{
	}

	public ZOutputStream(Stream output, bool nowrap)
		: this(output, GetDefaultZStream(nowrap))
	{
	}

	public ZOutputStream(Stream output, ZStream z)
	{
		if (z == null)
		{
			z = new ZStream();
		}
		if (z.istate == null && z.dstate == null)
		{
			z.inflateInit();
		}
		this.output = output;
		compress = z.istate == null;
		this.z = z;
	}

	public ZOutputStream(Stream output, int level)
		: this(output, level, nowrap: false)
	{
	}

	public ZOutputStream(Stream output, int level, bool nowrap)
	{
		this.output = output;
		compress = true;
		z = new ZStream();
		z.deflateInit(level, nowrap);
	}

	protected void Detach(bool disposing)
	{
		if (disposing)
		{
			ImplDisposing(disposeOutput: false);
		}
		base.Dispose(disposing);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ImplDisposing(disposeOutput: true);
		}
		base.Dispose(disposing);
	}

	private void ImplDisposing(bool disposeOutput)
	{
		if (closed)
		{
			return;
		}
		try
		{
			Finish();
		}
		catch (IOException)
		{
		}
		finally
		{
			closed = true;
			End();
			if (disposeOutput)
			{
				output.Dispose();
			}
			output = null;
		}
	}

	public virtual void End()
	{
		if (z != null)
		{
			if (compress)
			{
				z.deflateEnd();
			}
			else
			{
				z.inflateEnd();
			}
			z.free();
			z = null;
		}
	}

	public virtual void Finish()
	{
		do
		{
			z.next_out = buf;
			z.next_out_index = 0;
			z.avail_out = buf.Length;
			int num = (compress ? z.deflate(4) : z.inflate(4));
			if (num != 1 && num != 0)
			{
				throw new IOException((compress ? "de" : "in") + "flating: " + z.msg);
			}
			int num2 = buf.Length - z.avail_out;
			if (num2 > 0)
			{
				output.Write(buf, 0, num2);
			}
		}
		while (z.avail_in > 0 || z.avail_out == 0);
		Flush();
	}

	public override void Flush()
	{
		output.Flush();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count == 0)
		{
			return;
		}
		z.next_in = buffer;
		z.next_in_index = offset;
		z.avail_in = count;
		do
		{
			z.next_out = buf;
			z.next_out_index = 0;
			z.avail_out = buf.Length;
			if ((compress ? z.deflate(flushLevel) : z.inflate(flushLevel)) != 0)
			{
				throw new IOException((compress ? "de" : "in") + "flating: " + z.msg);
			}
			output.Write(buf, 0, buf.Length - z.avail_out);
		}
		while (z.avail_in > 0 || z.avail_out == 0);
	}

	public override void WriteByte(byte value)
	{
		buf1[0] = value;
		Write(buf1, 0, 1);
	}
}
