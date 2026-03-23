using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class RevDetails : Asn1Encodable
{
	private readonly CertTemplate m_certDetails;

	private readonly X509Extensions m_crlEntryDetails;

	public virtual CertTemplate CertDetails => m_certDetails;

	public virtual X509Extensions CrlEntryDetails => m_crlEntryDetails;

	public static RevDetails GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RevDetails result)
		{
			return result;
		}
		return new RevDetails(Asn1Sequence.GetInstance(obj));
	}

	public static RevDetails GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new RevDetails(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private RevDetails(Asn1Sequence seq)
	{
		m_certDetails = CertTemplate.GetInstance(seq[0]);
		if (seq.Count > 1)
		{
			m_crlEntryDetails = X509Extensions.GetInstance(seq[1]);
		}
	}

	public RevDetails(CertTemplate certDetails)
		: this(certDetails, null)
	{
	}

	public RevDetails(CertTemplate certDetails, X509Extensions crlEntryDetails)
	{
		m_certDetails = certDetails;
		m_crlEntryDetails = crlEntryDetails;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_certDetails);
		asn1EncodableVector.AddOptional(m_crlEntryDetails);
		return new DerSequence(asn1EncodableVector);
	}
}
