using System.IO;

namespace Mirror.BouncyCastle.Utilities.Zlib;

public class ZInputStreamLeaveOpen : ZInputStream
{
	public ZInputStreamLeaveOpen(Stream input)
		: base(input)
	{
	}

	public ZInputStreamLeaveOpen(Stream input, bool nowrap)
		: base(input, nowrap)
	{
	}

	public ZInputStreamLeaveOpen(Stream input, ZStream z)
		: base(input, z)
	{
	}

	public ZInputStreamLeaveOpen(Stream input, int level)
		: base(input, level)
	{
	}

	public ZInputStreamLeaveOpen(Stream input, int level, bool nowrap)
		: base(input, level, nowrap)
	{
	}

	protected override void Dispose(bool disposing)
	{
		Detach(disposing);
	}
}
