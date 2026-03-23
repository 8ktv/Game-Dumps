using System.IO;

namespace Mirror.BouncyCastle.Utilities.Bzip2;

public class CBZip2OutputStreamLeaveOpen : CBZip2OutputStream
{
	public CBZip2OutputStreamLeaveOpen(Stream outStream)
		: base(outStream)
	{
	}

	public CBZip2OutputStreamLeaveOpen(Stream outStream, int blockSize)
		: base(outStream, blockSize)
	{
	}

	protected override void Dispose(bool disposing)
	{
		Detach(disposing);
	}
}
