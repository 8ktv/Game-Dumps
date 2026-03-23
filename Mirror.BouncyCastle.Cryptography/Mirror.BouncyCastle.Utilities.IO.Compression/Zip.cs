using System.IO;
using Mirror.BouncyCastle.Utilities.Zlib;

namespace Mirror.BouncyCastle.Utilities.IO.Compression;

internal static class Zip
{
	internal static Stream CompressOutput(Stream stream, int zlibCompressionLevel, bool leaveOpen = false)
	{
		if (!leaveOpen)
		{
			return new ZOutputStream(stream, zlibCompressionLevel, nowrap: true);
		}
		return new ZOutputStreamLeaveOpen(stream, zlibCompressionLevel, nowrap: true);
	}

	internal static Stream DecompressInput(Stream stream, bool leaveOpen = false)
	{
		if (!leaveOpen)
		{
			return new ZInputStream(stream, nowrap: true);
		}
		return new ZInputStreamLeaveOpen(stream, nowrap: true);
	}
}
