using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class IssuerAndSerialNumber : Asn1Encodable
{
	private X509Name m_name;

	private DerInteger m_serialNumber;

	public X509Name Name => m_name;

	public DerInteger SerialNumber => m_serialNumber;

	public static IssuerAndSerialNumber GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is IssuerAndSerialNumber result)
		{
			return result;
		}
		return new IssuerAndSerialNumber(Asn1Sequence.GetInstance(obj));
	}

	public static IssuerAndSerialNumber GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new IssuerAndSerialNumber(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private IssuerAndSerialNumber(Asn1Sequence seq)
	{
		m_name = X509Name.GetInstance(seq[0]);
		m_serialNumber = DerInteger.GetInstance(seq[1]);
	}

	public IssuerAndSerialNumber(X509Name name, BigInteger serialNumber)
	{
		m_name = name;
		m_serialNumber = new DerInteger(serialNumber);
	}

	public IssuerAndSerialNumber(X509Name name, DerInteger serialNumber)
	{
		m_name = name;
		m_serialNumber = serialNumber;
	}

	public IssuerAndSerialNumber(X509CertificateStructure x509CertificateStructure)
	{
		m_name = x509CertificateStructure.Issuer;
		m_serialNumber = x509CertificateStructure.SerialNumber;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_name, m_serialNumber);
	}
}
