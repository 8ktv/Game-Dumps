using System;
using Mirror.BouncyCastle.Asn1.Crmf;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CertifiedKeyPair : Asn1Encodable
{
	private readonly CertOrEncCert m_certOrEncCert;

	private readonly EncryptedKey m_privateKey;

	private readonly PkiPublicationInfo m_publicationInfo;

	public virtual CertOrEncCert CertOrEncCert => m_certOrEncCert;

	public virtual EncryptedKey PrivateKey => m_privateKey;

	public virtual PkiPublicationInfo PublicationInfo => m_publicationInfo;

	public static CertifiedKeyPair GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertifiedKeyPair result)
		{
			return result;
		}
		return new CertifiedKeyPair(Asn1Sequence.GetInstance(obj));
	}

	public static CertifiedKeyPair GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CertifiedKeyPair(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CertifiedKeyPair(Asn1Sequence seq)
	{
		m_certOrEncCert = CertOrEncCert.GetInstance(seq[0]);
		if (seq.Count < 2)
		{
			return;
		}
		if (seq.Count == 2)
		{
			Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[1], 128);
			if (instance.TagNo == 0)
			{
				m_privateKey = EncryptedKey.GetInstance(instance.GetExplicitBaseObject());
			}
			else
			{
				m_publicationInfo = PkiPublicationInfo.GetInstance(instance.GetExplicitBaseObject());
			}
		}
		else
		{
			m_privateKey = EncryptedKey.GetInstance(Asn1TaggedObject.GetInstance(seq[1], 128).GetExplicitBaseObject());
			m_publicationInfo = PkiPublicationInfo.GetInstance(Asn1TaggedObject.GetInstance(seq[2], 128).GetExplicitBaseObject());
		}
	}

	public CertifiedKeyPair(CertOrEncCert certOrEncCert)
		: this(certOrEncCert, (EncryptedKey)null, (PkiPublicationInfo)null)
	{
	}

	public CertifiedKeyPair(CertOrEncCert certOrEncCert, EncryptedValue privateKey, PkiPublicationInfo publicationInfo)
		: this(certOrEncCert, (privateKey == null) ? null : new EncryptedKey(privateKey), publicationInfo)
	{
	}

	public CertifiedKeyPair(CertOrEncCert certOrEncCert, EncryptedKey privateKey, PkiPublicationInfo publicationInfo)
	{
		if (certOrEncCert == null)
		{
			throw new ArgumentNullException("certOrEncCert");
		}
		m_certOrEncCert = certOrEncCert;
		m_privateKey = privateKey;
		m_publicationInfo = publicationInfo;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(m_certOrEncCert);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_privateKey);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_publicationInfo);
		return new DerSequence(asn1EncodableVector);
	}
}
