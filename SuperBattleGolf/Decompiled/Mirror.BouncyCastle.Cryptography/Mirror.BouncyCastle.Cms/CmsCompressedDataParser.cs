using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Utilities.IO.Compression;

namespace Mirror.BouncyCastle.Cms;

public class CmsCompressedDataParser : CmsContentInfoParser
{
	public CmsCompressedDataParser(byte[] compressedData)
		: this(new MemoryStream(compressedData, writable: false))
	{
	}

	public CmsCompressedDataParser(Stream compressedData)
		: base(compressedData)
	{
	}

	public CmsTypedStream GetContent()
	{
		try
		{
			ContentInfoParser encapContentInfo = new CompressedDataParser((Asn1SequenceParser)contentInfo.GetContent(16)).GetEncapContentInfo();
			return new CmsTypedStream(inStream: ZLib.DecompressInput(((Asn1OctetStringParser)encapContentInfo.GetContent(4)).GetOctetStream()), oid: encapContentInfo.ContentType.Id);
		}
		catch (IOException innerException)
		{
			throw new CmsException("IOException reading compressed content.", innerException);
		}
	}
}
