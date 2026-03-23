namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PollRepContent : Asn1Encodable
{
	private readonly DerInteger[] m_certReqID;

	private readonly DerInteger[] m_checkAfter;

	private readonly PkiFreeText[] m_reason;

	public virtual int Count => m_certReqID.Length;

	public static PollRepContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PollRepContent result)
		{
			return result;
		}
		return new PollRepContent(Asn1Sequence.GetInstance(obj));
	}

	public static PollRepContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PollRepContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PollRepContent(Asn1Sequence seq)
	{
		int count = seq.Count;
		m_certReqID = new DerInteger[count];
		m_checkAfter = new DerInteger[count];
		m_reason = new PkiFreeText[count];
		for (int i = 0; i != count; i++)
		{
			Asn1Sequence instance = Asn1Sequence.GetInstance(seq[i]);
			m_certReqID[i] = DerInteger.GetInstance(instance[0]);
			m_checkAfter[i] = DerInteger.GetInstance(instance[1]);
			if (instance.Count > 2)
			{
				m_reason[i] = PkiFreeText.GetInstance(instance[2]);
			}
		}
	}

	public PollRepContent(DerInteger certReqID, DerInteger checkAfter)
		: this(certReqID, checkAfter, null)
	{
	}

	public PollRepContent(DerInteger certReqID, DerInteger checkAfter, PkiFreeText reason)
	{
		m_certReqID = new DerInteger[1] { certReqID };
		m_checkAfter = new DerInteger[1] { checkAfter };
		m_reason = new PkiFreeText[1] { reason };
	}

	public virtual DerInteger GetCertReqID(int index)
	{
		return m_certReqID[index];
	}

	public virtual DerInteger GetCheckAfter(int index)
	{
		return m_checkAfter[index];
	}

	public virtual PkiFreeText GetReason(int index)
	{
		return m_reason[index];
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_certReqID.Length);
		for (int i = 0; i != m_certReqID.Length; i++)
		{
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(3);
			asn1EncodableVector2.Add(m_certReqID[i]);
			asn1EncodableVector2.Add(m_checkAfter[i]);
			asn1EncodableVector2.AddOptional(m_reason[i]);
			asn1EncodableVector.Add(new DerSequence(asn1EncodableVector2));
		}
		return new DerSequence(asn1EncodableVector);
	}
}
