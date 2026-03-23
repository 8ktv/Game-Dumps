using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Utilities.IO.Compression;

namespace Mirror.BouncyCastle.Cms;

public class CmsCompressedData
{
	internal ContentInfo contentInfo;

	public ContentInfo ContentInfo => contentInfo;

	public CmsCompressedData(byte[] compressedData)
		: this(CmsUtilities.ReadContentInfo(compressedData))
	{
	}

	public CmsCompressedData(Stream compressedDataStream)
		: this(CmsUtilities.ReadContentInfo(compressedDataStream))
	{
	}

	public CmsCompressedData(ContentInfo contentInfo)
	{
		this.contentInfo = contentInfo;
	}

	public byte[] GetContent()
	{
		Stream stream = ZLib.DecompressInput(((Asn1OctetString)CompressedData.GetInstance(contentInfo.Content).EncapContentInfo.Content).GetOctetStream());
		try
		{
			return CmsUtilities.StreamToByteArray(stream);
		}
		catch (IOException innerException)
		{
			throw new CmsException("exception reading compressed stream.", innerException);
		}
		finally
		{
			stream.Dispose();
		}
	}

	public byte[] GetContent(int limit)
	{
		Stream inStream = ZLib.DecompressInput(((Asn1OctetString)CompressedData.GetInstance(contentInfo.Content).EncapContentInfo.Content).GetOctetStream());
		try
		{
			return CmsUtilities.StreamToByteArray(inStream, limit);
		}
		catch (IOException innerException)
		{
			throw new CmsException("exception reading compressed stream.", innerException);
		}
	}

	public byte[] GetEncoded()
	{
		return contentInfo.GetEncoded();
	}
}
