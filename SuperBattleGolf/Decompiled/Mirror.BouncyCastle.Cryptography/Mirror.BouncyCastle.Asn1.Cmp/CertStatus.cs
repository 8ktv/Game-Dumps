using System;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CertStatus : Asn1Encodable
{
	private readonly Asn1OctetString m_certHash;

	private readonly DerInteger m_certReqID;

	private readonly PkiStatusInfo m_statusInfo;

	private readonly AlgorithmIdentifier m_hashAlg;

	public virtual Asn1OctetString CertHash => m_certHash;

	public virtual DerInteger CertReqID => m_certReqID;

	public virtual PkiStatusInfo StatusInfo => m_statusInfo;

	public virtual AlgorithmIdentifier HashAlg => m_hashAlg;

	public static CertStatus GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertStatus result)
		{
			return result;
		}
		return new CertStatus(Asn1Sequence.GetInstance(obj));
	}

	public static CertStatus GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CertStatus(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CertStatus(Asn1Sequence seq)
	{
		m_certHash = Asn1OctetString.GetInstance(seq[0]);
		m_certReqID = DerInteger.GetInstance(seq[1]);
		if (seq.Count <= 2)
		{
			return;
		}
		for (int i = 2; i < seq.Count; i++)
		{
			Asn1Object asn1Object = seq[i].ToAsn1Object();
			if (asn1Object is Asn1Sequence obj)
			{
				m_statusInfo = PkiStatusInfo.GetInstance(obj);
			}
			if (asn1Object is Asn1TaggedObject asn1TaggedObject)
			{
				if (!asn1TaggedObject.HasContextTag(0))
				{
					throw new ArgumentException("unknown tag: " + Asn1Utilities.GetTagText(asn1TaggedObject));
				}
				m_hashAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, explicitly: true);
			}
		}
	}

	public CertStatus(byte[] certHash, BigInteger certReqID)
		: this(certHash, new DerInteger(certReqID))
	{
	}

	public CertStatus(byte[] certHash, DerInteger certReqID)
	{
		m_certHash = new DerOctetString(certHash);
		m_certReqID = certReqID;
		m_statusInfo = null;
		m_hashAlg = null;
	}

	public CertStatus(byte[] certHash, BigInteger certReqID, PkiStatusInfo statusInfo)
	{
		m_certHash = new DerOctetString(certHash);
		m_certReqID = new DerInteger(certReqID);
		m_statusInfo = statusInfo;
		m_hashAlg = null;
	}

	public CertStatus(byte[] certHash, BigInteger certReqID, PkiStatusInfo statusInfo, AlgorithmIdentifier hashAlg)
	{
		m_certHash = new DerOctetString(certHash);
		m_certReqID = new DerInteger(certReqID);
		m_statusInfo = statusInfo;
		m_hashAlg = hashAlg;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
		asn1EncodableVector.Add(m_certHash, m_certReqID);
		asn1EncodableVector.AddOptional(m_statusInfo);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_hashAlg);
		return new DerSequence(asn1EncodableVector);
	}
}
