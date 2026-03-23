using System;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class KeyRecRepContent : Asn1Encodable
{
	private readonly PkiStatusInfo m_status;

	private readonly CmpCertificate m_newSigCert;

	private readonly Asn1Sequence m_caCerts;

	private readonly Asn1Sequence m_keyPairHist;

	public virtual PkiStatusInfo Status => m_status;

	public virtual CmpCertificate NewSigCert => m_newSigCert;

	public static KeyRecRepContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KeyRecRepContent result)
		{
			return result;
		}
		return new KeyRecRepContent(Asn1Sequence.GetInstance(obj));
	}

	public static KeyRecRepContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new KeyRecRepContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private KeyRecRepContent(Asn1Sequence seq)
	{
		m_status = PkiStatusInfo.GetInstance(seq[0]);
		for (int i = 1; i < seq.Count; i++)
		{
			Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i], 128);
			switch (instance.TagNo)
			{
			case 0:
				m_newSigCert = CmpCertificate.GetInstance(instance.GetExplicitBaseObject());
				break;
			case 1:
				m_caCerts = Asn1Sequence.GetInstance(instance.GetExplicitBaseObject());
				break;
			case 2:
				m_keyPairHist = Asn1Sequence.GetInstance(instance.GetExplicitBaseObject());
				break;
			default:
				throw new ArgumentException("unknown tag number: " + instance.TagNo);
			}
		}
	}

	public virtual CmpCertificate[] GetCACerts()
	{
		return m_caCerts?.MapElements(CmpCertificate.GetInstance);
	}

	public virtual CertifiedKeyPair[] GetKeyPairHist()
	{
		return m_keyPairHist?.MapElements(CertifiedKeyPair.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
		asn1EncodableVector.Add(m_status);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_newSigCert);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_caCerts);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, m_keyPairHist);
		return new DerSequence(asn1EncodableVector);
	}
}
