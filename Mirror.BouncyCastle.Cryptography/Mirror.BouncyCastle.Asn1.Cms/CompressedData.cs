using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class CompressedData : Asn1Encodable
{
	private DerInteger version;

	private AlgorithmIdentifier compressionAlgorithm;

	private ContentInfo encapContentInfo;

	public DerInteger Version => version;

	public AlgorithmIdentifier CompressionAlgorithmIdentifier => compressionAlgorithm;

	public ContentInfo EncapContentInfo => encapContentInfo;

	public static CompressedData GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CompressedData result)
		{
			return result;
		}
		return new CompressedData(Asn1Sequence.GetInstance(obj));
	}

	public static CompressedData GetInstance(Asn1TaggedObject ato, bool explicitly)
	{
		return new CompressedData(Asn1Sequence.GetInstance(ato, explicitly));
	}

	public CompressedData(AlgorithmIdentifier compressionAlgorithm, ContentInfo encapContentInfo)
	{
		version = new DerInteger(0);
		this.compressionAlgorithm = compressionAlgorithm;
		this.encapContentInfo = encapContentInfo;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public CompressedData(Asn1Sequence seq)
	{
		version = (DerInteger)seq[0];
		compressionAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
		encapContentInfo = ContentInfo.GetInstance(seq[2]);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new BerSequence(version, compressionAlgorithm, encapContentInfo);
	}
}
