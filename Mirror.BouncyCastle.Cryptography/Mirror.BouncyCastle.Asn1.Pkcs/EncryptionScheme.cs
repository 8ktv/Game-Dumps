using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Pkcs;

public class EncryptionScheme : AlgorithmIdentifier
{
	public Asn1Object Asn1Object => Parameters.ToAsn1Object();

	public new static EncryptionScheme GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is EncryptionScheme result)
		{
			return result;
		}
		return new EncryptionScheme(Asn1Sequence.GetInstance(obj));
	}

	public new static EncryptionScheme GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public EncryptionScheme(DerObjectIdentifier objectID)
		: base(objectID)
	{
	}

	public EncryptionScheme(DerObjectIdentifier objectID, Asn1Encodable parameters)
		: base(objectID, parameters)
	{
	}

	internal EncryptionScheme(Asn1Sequence seq)
		: this((DerObjectIdentifier)seq[0], seq[1])
	{
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(Algorithm, Parameters);
	}
}
