using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities.IO.Compression;

namespace Mirror.BouncyCastle.Cms;

public class CmsCompressedDataGenerator
{
	public static readonly string ZLib = CmsObjectIdentifiers.ZlibCompress.Id;

	public CmsCompressedData Generate(CmsProcessable content, string compressionOid)
	{
		if (ZLib != compressionOid)
		{
			throw new ArgumentException("Unsupported compression algorithm: " + compressionOid, "compressionOid");
		}
		AlgorithmIdentifier compressionAlgorithm;
		Asn1OctetString content2;
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			using (Stream outStream = Mirror.BouncyCastle.Utilities.IO.Compression.ZLib.CompressOutput(memoryStream, -1))
			{
				content.Write(outStream);
			}
			compressionAlgorithm = new AlgorithmIdentifier(CmsObjectIdentifiers.ZlibCompress);
			content2 = new BerOctetString(memoryStream.ToArray());
		}
		catch (IOException innerException)
		{
			throw new CmsException("exception encoding data.", innerException);
		}
		ContentInfo encapContentInfo = new ContentInfo(CmsObjectIdentifiers.Data, content2);
		return new CmsCompressedData(new ContentInfo(CmsObjectIdentifiers.CompressedData, new CompressedData(compressionAlgorithm, encapContentInfo)));
	}
}
