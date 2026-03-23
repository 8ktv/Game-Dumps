namespace Mirror.BouncyCastle.Asn1.Cms;

public class AuthEnvelopedDataParser
{
	private Asn1SequenceParser seq;

	private DerInteger version;

	private IAsn1Convertible nextObject;

	private bool originatorInfoCalled;

	private bool isData;

	public DerInteger Version => version;

	public AuthEnvelopedDataParser(Asn1SequenceParser seq)
	{
		this.seq = seq;
		version = (DerInteger)seq.ReadObject();
		if (!version.HasValue(0))
		{
			throw new Asn1ParsingException("AuthEnvelopedData version number must be 0");
		}
	}

	public OriginatorInfo GetOriginatorInfo()
	{
		originatorInfoCalled = true;
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		if (nextObject is Asn1TaggedObjectParser asn1TaggedObjectParser && asn1TaggedObjectParser.HasContextTag(0))
		{
			Asn1SequenceParser obj = (Asn1SequenceParser)asn1TaggedObjectParser.ParseBaseUniversal(declaredExplicit: false, 16);
			nextObject = null;
			return OriginatorInfo.GetInstance(obj.ToAsn1Object());
		}
		return null;
	}

	public Asn1SetParser GetRecipientInfos()
	{
		if (!originatorInfoCalled)
		{
			GetOriginatorInfo();
		}
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		Asn1SetParser result = (Asn1SetParser)nextObject;
		nextObject = null;
		return result;
	}

	public EncryptedContentInfoParser GetAuthEncryptedContentInfo()
	{
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		if (nextObject != null)
		{
			Asn1SequenceParser obj = (Asn1SequenceParser)nextObject;
			nextObject = null;
			EncryptedContentInfoParser encryptedContentInfoParser = new EncryptedContentInfoParser(obj);
			isData = CmsObjectIdentifiers.Data.Equals(encryptedContentInfoParser.ContentType);
			return encryptedContentInfoParser;
		}
		return null;
	}

	public Asn1SetParser GetAuthAttrs()
	{
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		if (nextObject is Asn1TaggedObjectParser taggedObjectParser)
		{
			nextObject = null;
			return (Asn1SetParser)Asn1Utilities.ParseContextBaseUniversal(taggedObjectParser, 1, declaredExplicit: false, 17);
		}
		if (!isData)
		{
			throw new Asn1ParsingException("authAttrs must be present with non-data content");
		}
		return null;
	}

	public Asn1OctetString GetMac()
	{
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		IAsn1Convertible asn1Convertible = nextObject;
		nextObject = null;
		return Asn1OctetString.GetInstance(asn1Convertible.ToAsn1Object());
	}

	public Asn1SetParser GetUnauthAttrs()
	{
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		if (nextObject != null)
		{
			Asn1TaggedObjectParser taggedObjectParser = (Asn1TaggedObjectParser)nextObject;
			nextObject = null;
			return (Asn1SetParser)Asn1Utilities.ParseContextBaseUniversal(taggedObjectParser, 2, declaredExplicit: false, 17);
		}
		return null;
	}
}
