using System.IO;
using Mirror.BouncyCastle.Utilities.IO.Compression;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpCompressedData : PgpObject
{
	private readonly CompressedDataPacket data;

	public CompressionAlgorithmTag Algorithm => data.Algorithm;

	public PgpCompressedData(BcpgInputStream bcpgInput)
	{
		Packet packet = bcpgInput.ReadPacket();
		if (!(packet is CompressedDataPacket compressedDataPacket))
		{
			throw new IOException("unexpected packet in stream: " + packet);
		}
		data = compressedDataPacket;
	}

	public Stream GetInputStream()
	{
		return data.GetInputStream();
	}

	public Stream GetDataStream()
	{
		return Algorithm switch
		{
			CompressionAlgorithmTag.Uncompressed => GetInputStream(), 
			CompressionAlgorithmTag.Zip => Zip.DecompressInput(GetInputStream()), 
			CompressionAlgorithmTag.ZLib => ZLib.DecompressInput(GetInputStream()), 
			CompressionAlgorithmTag.BZip2 => Bzip2.DecompressInput(GetInputStream()), 
			_ => throw new PgpException("can't recognise compression algorithm: " + Algorithm), 
		};
	}
}
