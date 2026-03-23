using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO.Compression;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpCompressedDataGenerator : IStreamGenerator
{
	private readonly CompressionAlgorithmTag algorithm;

	private readonly int compression;

	private Stream dOut;

	private BcpgOutputStream pkOut;

	public PgpCompressedDataGenerator(CompressionAlgorithmTag algorithm)
		: this(algorithm, -1)
	{
	}

	public PgpCompressedDataGenerator(CompressionAlgorithmTag algorithm, int compression)
	{
		if ((uint)algorithm > 3u)
		{
			throw new ArgumentException("unknown compression algorithm", "algorithm");
		}
		switch (compression)
		{
		default:
			throw new ArgumentException("unknown compression level: " + compression);
		case -1:
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
			this.algorithm = algorithm;
			this.compression = compression;
			break;
		}
	}

	public Stream Open(Stream outStr)
	{
		if (dOut != null)
		{
			throw new InvalidOperationException("generator already in open state");
		}
		if (outStr == null)
		{
			throw new ArgumentNullException("outStr");
		}
		pkOut = new BcpgOutputStream(outStr, PacketTag.CompressedData);
		DoOpen();
		return new WrappedGeneratorStream(this, dOut);
	}

	public Stream Open(Stream outStr, byte[] buffer)
	{
		if (dOut != null)
		{
			throw new InvalidOperationException("generator already in open state");
		}
		if (outStr == null)
		{
			throw new ArgumentNullException("outStr");
		}
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		pkOut = new BcpgOutputStream(outStr, PacketTag.CompressedData, buffer);
		DoOpen();
		return new WrappedGeneratorStream(this, dOut);
	}

	private void DoOpen()
	{
		pkOut.WriteByte((byte)algorithm);
		switch (algorithm)
		{
		case CompressionAlgorithmTag.Uncompressed:
			dOut = pkOut;
			break;
		case CompressionAlgorithmTag.Zip:
			dOut = Zip.CompressOutput(pkOut, compression, leaveOpen: true);
			break;
		case CompressionAlgorithmTag.ZLib:
			dOut = ZLib.CompressOutput(pkOut, compression, leaveOpen: true);
			break;
		case CompressionAlgorithmTag.BZip2:
			dOut = Bzip2.CompressOutput(pkOut, leaveOpen: true);
			break;
		default:
			throw new InvalidOperationException();
		}
	}

	[Obsolete("Dispose any opened Stream directly")]
	public void Close()
	{
		if (dOut != null)
		{
			if (dOut != pkOut)
			{
				dOut.Dispose();
			}
			dOut = null;
			pkOut.Finish();
			pkOut.Flush();
			pkOut = null;
		}
	}
}
