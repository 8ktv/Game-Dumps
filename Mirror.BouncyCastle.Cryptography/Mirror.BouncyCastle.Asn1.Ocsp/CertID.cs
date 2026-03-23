using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class CertID : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_hashAlgorithm;

	private readonly Asn1OctetString m_issuerNameHash;

	private readonly Asn1OctetString m_issuerKeyHash;

	private readonly DerInteger m_serialNumber;

	public AlgorithmIdentifier HashAlgorithm => m_hashAlgorithm;

	public Asn1OctetString IssuerNameHash => m_issuerNameHash;

	public Asn1OctetString IssuerKeyHash => m_issuerKeyHash;

	public DerInteger SerialNumber => m_serialNumber;

	public static CertID GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertID result)
		{
			return result;
		}
		return new CertID(Asn1Sequence.GetInstance(obj));
	}

	public static CertID GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new CertID(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public CertID(AlgorithmIdentifier hashAlgorithm, Asn1OctetString issuerNameHash, Asn1OctetString issuerKeyHash, DerInteger serialNumber)
	{
		m_hashAlgorithm = hashAlgorithm ?? throw new ArgumentNullException("hashAlgorithm");
		m_issuerNameHash = issuerNameHash ?? throw new ArgumentNullException("issuerNameHash");
		m_issuerKeyHash = issuerKeyHash ?? throw new ArgumentNullException("issuerKeyHash");
		m_serialNumber = serialNumber ?? throw new ArgumentNullException("serialNumber");
	}

	private CertID(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count != 4)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_hashAlgorithm = AlgorithmIdentifier.GetInstance(seq[0]);
		m_issuerNameHash = Asn1OctetString.GetInstance(seq[1]);
		m_issuerKeyHash = Asn1OctetString.GetInstance(seq[2]);
		m_serialNumber = DerInteger.GetInstance(seq[3]);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_hashAlgorithm, m_issuerNameHash, m_issuerKeyHash, m_serialNumber);
	}
}
