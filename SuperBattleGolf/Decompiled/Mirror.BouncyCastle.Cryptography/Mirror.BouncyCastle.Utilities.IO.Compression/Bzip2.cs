using System.IO;
using Mirror.BouncyCastle.Utilities.Bzip2;

namespace Mirror.BouncyCastle.Utilities.IO.Compression;

internal static class Bzip2
{
	internal static Stream CompressOutput(Stream stream, bool leaveOpen = false)
	{
		if (!leaveOpen)
		{
			return new CBZip2OutputStream(stream);
		}
		return new CBZip2OutputStreamLeaveOpen(stream);
	}

	internal static Stream DecompressInput(Stream stream, bool leaveOpen = false)
	{
		if (!leaveOpen)
		{
			return new CBZip2InputStream(stream);
		}
		return new CBZip2InputStreamLeaveOpen(stream);
	}
}
