using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class AuthenticatedDataParser
{
	private Asn1SequenceParser seq;

	private DerInteger version;

	private IAsn1Convertible nextObject;

	private bool originatorInfoCalled;

	public DerInteger Version => version;

	public AuthenticatedDataParser(Asn1SequenceParser seq)
	{
		this.seq = seq;
		version = (DerInteger)seq.ReadObject();
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

	public AlgorithmIdentifier GetMacAlgorithm()
	{
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		if (nextObject != null)
		{
			Asn1SequenceParser obj = (Asn1SequenceParser)nextObject;
			nextObject = null;
			return AlgorithmIdentifier.GetInstance(obj.ToAsn1Object());
		}
		return null;
	}

	public AlgorithmIdentifier GetDigestAlgorithm()
	{
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		if (nextObject is Asn1TaggedObjectParser)
		{
			AlgorithmIdentifier instance = AlgorithmIdentifier.GetInstance((Asn1TaggedObject)nextObject.ToAsn1Object(), explicitly: false);
			nextObject = null;
			return instance;
		}
		return null;
	}

	public ContentInfoParser GetEnapsulatedContentInfo()
	{
		if (nextObject == null)
		{
			nextObject = seq.ReadObject();
		}
		if (nextObject != null)
		{
			Asn1SequenceParser obj = (Asn1SequenceParser)nextObject;
			nextObject = null;
			return new ContentInfoParser(obj);
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
			return (Asn1SetParser)Asn1Utilities.ParseContextBaseUniversal(taggedObjectParser, 2, declaredExplicit: false, 17);
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
			Asn1TaggedObject taggedObjectParser = (Asn1TaggedObject)nextObject;
			nextObject = null;
			return (Asn1SetParser)Asn1Utilities.ParseContextBaseUniversal(taggedObjectParser, 3, declaredExplicit: false, 17);
		}
		return null;
	}
}
