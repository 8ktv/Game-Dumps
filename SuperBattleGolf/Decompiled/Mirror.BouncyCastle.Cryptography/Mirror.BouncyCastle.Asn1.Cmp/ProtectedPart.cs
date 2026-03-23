namespace Mirror.BouncyCastle.Asn1.Cmp;

public class ProtectedPart : Asn1Encodable
{
	private readonly PkiHeader m_header;

	private readonly PkiBody m_body;

	public virtual PkiHeader Header => m_header;

	public virtual PkiBody Body => m_body;

	public static ProtectedPart GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ProtectedPart result)
		{
			return result;
		}
		return new ProtectedPart(Asn1Sequence.GetInstance(obj));
	}

	public static ProtectedPart GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new ProtectedPart(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private ProtectedPart(Asn1Sequence seq)
	{
		m_header = PkiHeader.GetInstance(seq[0]);
		m_body = PkiBody.GetInstance(seq[1]);
	}

	public ProtectedPart(PkiHeader header, PkiBody body)
	{
		m_header = header;
		m_body = body;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_header, m_body);
	}
}
