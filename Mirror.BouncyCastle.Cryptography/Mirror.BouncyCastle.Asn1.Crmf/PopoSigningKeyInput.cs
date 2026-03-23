using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class PopoSigningKeyInput : Asn1Encodable
{
	private readonly GeneralName m_sender;

	private readonly PKMacValue m_publicKeyMac;

	private readonly SubjectPublicKeyInfo m_publicKey;

	public virtual GeneralName Sender => m_sender;

	public virtual PKMacValue PublicKeyMac => m_publicKeyMac;

	public virtual SubjectPublicKeyInfo PublicKey => m_publicKey;

	public static PopoSigningKeyInput GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PopoSigningKeyInput result)
		{
			return result;
		}
		return new PopoSigningKeyInput(Asn1Sequence.GetInstance(obj));
	}

	public static PopoSigningKeyInput GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PopoSigningKeyInput(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PopoSigningKeyInput(Asn1Sequence seq)
	{
		Asn1Encodable asn1Encodable = seq[0];
		if (asn1Encodable is Asn1TaggedObject taggedObject)
		{
			m_sender = GeneralName.GetInstance(Asn1Utilities.GetExplicitContextBaseObject(taggedObject, 0));
		}
		else
		{
			m_publicKeyMac = PKMacValue.GetInstance(asn1Encodable);
		}
		m_publicKey = SubjectPublicKeyInfo.GetInstance(seq[1]);
	}

	public PopoSigningKeyInput(GeneralName sender, SubjectPublicKeyInfo spki)
	{
		m_sender = sender;
		m_publicKey = spki;
	}

	public PopoSigningKeyInput(PKMacValue pkmac, SubjectPublicKeyInfo spki)
	{
		m_publicKeyMac = pkmac;
		m_publicKey = spki;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		if (m_sender != null)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: false, 0, m_sender));
		}
		else
		{
			asn1EncodableVector.Add(m_publicKeyMac);
		}
		asn1EncodableVector.Add(m_publicKey);
		return new DerSequence(asn1EncodableVector);
	}
}
