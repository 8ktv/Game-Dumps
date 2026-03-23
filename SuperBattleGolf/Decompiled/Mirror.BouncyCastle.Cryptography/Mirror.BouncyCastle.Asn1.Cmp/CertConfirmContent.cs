namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CertConfirmContent : Asn1Encodable
{
	private readonly Asn1Sequence m_content;

	public static CertConfirmContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertConfirmContent result)
		{
			return result;
		}
		return new CertConfirmContent(Asn1Sequence.GetInstance(obj));
	}

	public static CertConfirmContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CertConfirmContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CertConfirmContent(Asn1Sequence seq)
	{
		m_content = seq;
	}

	public virtual CertStatus[] ToCertStatusArray()
	{
		return m_content.MapElements(CertStatus.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_content;
	}
}
