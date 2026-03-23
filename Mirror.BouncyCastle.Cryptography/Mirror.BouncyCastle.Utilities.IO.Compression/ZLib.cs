using System.IO;
using Mirror.BouncyCastle.Utilities.Zlib;

namespace Mirror.BouncyCastle.Utilities.IO.Compression;

internal static class ZLib
{
	internal static Stream CompressOutput(Stream stream, int zlibCompressionLevel, bool leaveOpen = false)
	{
		if (!leaveOpen)
		{
			return new ZOutputStream(stream, zlibCompressionLevel, nowrap: false);
		}
		return new ZOutputStreamLeaveOpen(stream, zlibCompressionLevel, nowrap: false);
	}

	internal static Stream DecompressInput(Stream stream, bool leaveOpen = false)
	{
		if (!leaveOpen)
		{
			return new ZInputStream(stream);
		}
		return new ZInputStreamLeaveOpen(stream);
	}
}
