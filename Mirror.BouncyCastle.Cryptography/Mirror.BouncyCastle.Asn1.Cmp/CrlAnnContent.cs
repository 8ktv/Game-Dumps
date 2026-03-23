using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CrlAnnContent : Asn1Encodable
{
	private readonly Asn1Sequence m_content;

	public static CrlAnnContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CrlAnnContent result)
		{
			return result;
		}
		return new CrlAnnContent(Asn1Sequence.GetInstance(obj));
	}

	public static CrlAnnContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CrlAnnContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CrlAnnContent(Asn1Sequence seq)
	{
		m_content = seq;
	}

	public CrlAnnContent(CertificateList crl)
	{
		m_content = new DerSequence(crl);
	}

	public virtual CertificateList[] ToCertificateListArray()
	{
		return m_content.MapElements(CertificateList.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_content;
	}
}
