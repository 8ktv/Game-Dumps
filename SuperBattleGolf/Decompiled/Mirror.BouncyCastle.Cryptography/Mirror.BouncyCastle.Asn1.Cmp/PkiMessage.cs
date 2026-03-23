namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PkiMessage : Asn1Encodable
{
	private readonly PkiHeader m_header;

	private readonly PkiBody m_body;

	private readonly DerBitString m_protection;

	private readonly Asn1Sequence m_extraCerts;

	public virtual PkiHeader Header => m_header;

	public virtual PkiBody Body => m_body;

	public virtual DerBitString Protection => m_protection;

	public static PkiMessage GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PkiMessage result)
		{
			return result;
		}
		return new PkiMessage(Asn1Sequence.GetInstance(obj));
	}

	public static PkiMessage GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PkiMessage(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PkiMessage(Asn1Sequence seq)
	{
		m_header = PkiHeader.GetInstance(seq[0]);
		m_body = PkiBody.GetInstance(seq[1]);
		for (int i = 2; i < seq.Count; i++)
		{
			Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i]);
			if (instance.HasContextTag(0))
			{
				m_protection = DerBitString.GetInstance(instance, isExplicit: true);
			}
			else if (instance.HasContextTag(1))
			{
				m_extraCerts = Asn1Sequence.GetInstance(instance, declaredExplicit: true);
			}
		}
	}

	public PkiMessage(PkiHeader header, PkiBody body, DerBitString protection, CmpCertificate[] extraCerts)
	{
		m_header = header;
		m_body = body;
		m_protection = protection;
		if (extraCerts != null)
		{
			m_extraCerts = new DerSequence(extraCerts);
		}
	}

	public PkiMessage(PkiHeader header, PkiBody body, DerBitString protection)
		: this(header, body, protection, null)
	{
	}

	public PkiMessage(PkiHeader header, PkiBody body)
		: this(header, body, null, null)
	{
	}

	public virtual CmpCertificate[] GetExtraCerts()
	{
		return m_extraCerts?.MapElements(CmpCertificate.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
		asn1EncodableVector.Add(m_header, m_body);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_protection);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_extraCerts);
		return new DerSequence(asn1EncodableVector);
	}
}
