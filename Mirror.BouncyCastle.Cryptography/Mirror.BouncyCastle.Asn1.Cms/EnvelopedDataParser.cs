namespace Mirror.BouncyCastle.Asn1.Cms;

public class EnvelopedDataParser
{
	private Asn1SequenceParser _seq;

	private DerInteger _version;

	private IAsn1Convertible _nextObject;

	private bool _originatorInfoCalled;

	public DerInteger Version => _version;

	public EnvelopedDataParser(Asn1SequenceParser seq)
	{
		_seq = seq;
		_version = (DerInteger)seq.ReadObject();
	}

	public OriginatorInfo GetOriginatorInfo()
	{
		_originatorInfoCalled = true;
		if (_nextObject == null)
		{
			_nextObject = _seq.ReadObject();
		}
		if (_nextObject is Asn1TaggedObjectParser asn1TaggedObjectParser && asn1TaggedObjectParser.HasContextTag(0))
		{
			Asn1SequenceParser obj = (Asn1SequenceParser)asn1TaggedObjectParser.ParseBaseUniversal(declaredExplicit: false, 16);
			_nextObject = null;
			return OriginatorInfo.GetInstance(obj.ToAsn1Object());
		}
		return null;
	}

	public Asn1SetParser GetRecipientInfos()
	{
		if (!_originatorInfoCalled)
		{
			GetOriginatorInfo();
		}
		if (_nextObject == null)
		{
			_nextObject = _seq.ReadObject();
		}
		Asn1SetParser result = (Asn1SetParser)_nextObject;
		_nextObject = null;
		return result;
	}

	public EncryptedContentInfoParser GetEncryptedContentInfo()
	{
		if (_nextObject == null)
		{
			_nextObject = _seq.ReadObject();
		}
		if (_nextObject != null)
		{
			Asn1SequenceParser seq = (Asn1SequenceParser)_nextObject;
			_nextObject = null;
			return new EncryptedContentInfoParser(seq);
		}
		return null;
	}

	public Asn1SetParser GetUnprotectedAttrs()
	{
		if (_nextObject == null)
		{
			_nextObject = _seq.ReadObject();
		}
		if (_nextObject != null)
		{
			Asn1TaggedObjectParser taggedObjectParser = (Asn1TaggedObjectParser)_nextObject;
			_nextObject = null;
			return (Asn1SetParser)Asn1Utilities.ParseContextBaseUniversal(taggedObjectParser, 1, declaredExplicit: false, 17);
		}
		return null;
	}
}
