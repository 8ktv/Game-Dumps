namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PkiConfirmContent : Asn1Encodable
{
	private readonly Asn1Null m_val;

	public static PkiConfirmContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PkiConfirmContent result)
		{
			return result;
		}
		return new PkiConfirmContent(Asn1Null.GetInstance(obj));
	}

	public static PkiConfirmContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PkiConfirmContent(Asn1Null.GetInstance(taggedObject, declaredExplicit));
	}

	public PkiConfirmContent()
		: this(DerNull.Instance)
	{
	}

	private PkiConfirmContent(Asn1Null val)
	{
		m_val = val;
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_val;
	}
}
