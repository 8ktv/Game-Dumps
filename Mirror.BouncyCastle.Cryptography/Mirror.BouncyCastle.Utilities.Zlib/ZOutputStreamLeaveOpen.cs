using System.IO;

namespace Mirror.BouncyCastle.Utilities.Zlib;

public class ZOutputStreamLeaveOpen : ZOutputStream
{
	public ZOutputStreamLeaveOpen(Stream output)
		: base(output)
	{
	}

	public ZOutputStreamLeaveOpen(Stream output, bool nowrap)
		: base(output, nowrap)
	{
	}

	public ZOutputStreamLeaveOpen(Stream output, ZStream z)
		: base(output, z)
	{
	}

	public ZOutputStreamLeaveOpen(Stream output, int level)
		: base(output, level)
	{
	}

	public ZOutputStreamLeaveOpen(Stream output, int level, bool nowrap)
		: base(output, level, nowrap)
	{
	}

	protected override void Dispose(bool disposing)
	{
		Detach(disposing);
	}
}
