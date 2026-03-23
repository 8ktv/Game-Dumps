using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class TimeStampReq : Asn1Encodable
{
	private readonly DerInteger m_version;

	private readonly MessageImprint m_messageImprint;

	private readonly DerObjectIdentifier m_tsaPolicy;

	private readonly DerInteger m_nonce;

	private readonly DerBoolean m_certReq;

	private readonly X509Extensions m_extensions;

	public DerInteger Version => m_version;

	public MessageImprint MessageImprint => m_messageImprint;

	public DerObjectIdentifier ReqPolicy => m_tsaPolicy;

	public DerInteger Nonce => m_nonce;

	public DerBoolean CertReq => m_certReq;

	public X509Extensions Extensions => m_extensions;

	public static TimeStampReq GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is TimeStampReq result)
		{
			return result;
		}
		return new TimeStampReq(Asn1Sequence.GetInstance(obj));
	}

	public static TimeStampReq GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new TimeStampReq(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private TimeStampReq(Asn1Sequence seq)
	{
		int count = seq.Count;
		int num = 0;
		m_version = DerInteger.GetInstance(seq[num++]);
		m_messageImprint = MessageImprint.GetInstance(seq[num++]);
		for (int i = num; i < count; i++)
		{
			if (seq[i] is DerObjectIdentifier tsaPolicy)
			{
				m_tsaPolicy = tsaPolicy;
			}
			else if (seq[i] is DerInteger nonce)
			{
				m_nonce = nonce;
			}
			else if (seq[i] is DerBoolean certReq)
			{
				m_certReq = certReq;
			}
			else if (seq[i] is Asn1TaggedObject { TagNo: 0 } asn1TaggedObject)
			{
				m_extensions = X509Extensions.GetInstance(asn1TaggedObject, declaredExplicit: false);
			}
		}
	}

	public TimeStampReq(MessageImprint messageImprint, DerObjectIdentifier tsaPolicy, DerInteger nonce, DerBoolean certReq, X509Extensions extensions)
	{
		m_version = new DerInteger(1);
		m_messageImprint = messageImprint;
		m_tsaPolicy = tsaPolicy;
		m_nonce = nonce;
		m_certReq = certReq;
		m_extensions = extensions;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(6);
		asn1EncodableVector.Add(m_version, m_messageImprint);
		asn1EncodableVector.AddOptional(m_tsaPolicy, m_nonce);
		if (m_certReq != null && m_certReq.IsTrue)
		{
			asn1EncodableVector.Add(m_certReq);
		}
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, m_extensions);
		return new DerSequence(asn1EncodableVector);
	}
}
