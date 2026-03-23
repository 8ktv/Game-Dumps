using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class PkiPublicationInfo : Asn1Encodable
{
	public static readonly DerInteger DontPublish = new DerInteger(0);

	public static readonly DerInteger PleasePublish = new DerInteger(1);

	private readonly DerInteger m_action;

	private readonly Asn1Sequence m_pubInfos;

	public virtual DerInteger Action => m_action;

	public static PkiPublicationInfo GetInstance(object obj)
	{
		if (obj is PkiPublicationInfo result)
		{
			return result;
		}
		if (obj != null)
		{
			return new PkiPublicationInfo(Asn1Sequence.GetInstance(obj));
		}
		return null;
	}

	private PkiPublicationInfo(Asn1Sequence seq)
	{
		m_action = DerInteger.GetInstance(seq[0]);
		if (seq.Count > 1)
		{
			m_pubInfos = Asn1Sequence.GetInstance(seq[1]);
		}
	}

	public PkiPublicationInfo(BigInteger action)
		: this(new DerInteger(action))
	{
	}

	public PkiPublicationInfo(DerInteger action)
	{
		m_action = action;
	}

	public PkiPublicationInfo(SinglePubInfo pubInfo)
		: this((pubInfo == null) ? null : new SinglePubInfo[1] { pubInfo })
	{
	}

	public PkiPublicationInfo(SinglePubInfo[] pubInfos)
	{
		m_action = PleasePublish;
		if (pubInfos != null)
		{
			m_pubInfos = new DerSequence(pubInfos);
		}
	}

	public virtual SinglePubInfo[] GetPubInfos()
	{
		return m_pubInfos?.MapElements(SinglePubInfo.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		if (m_pubInfos == null)
		{
			return new DerSequence(m_action);
		}
		return new DerSequence(m_action, m_pubInfos);
	}
}
