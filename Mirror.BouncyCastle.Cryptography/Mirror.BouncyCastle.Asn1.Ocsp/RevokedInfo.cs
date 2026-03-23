using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class RevokedInfo : Asn1Encodable
{
	private readonly Asn1GeneralizedTime m_revocationTime;

	private readonly CrlReason m_revocationReason;

	public Asn1GeneralizedTime RevocationTime => m_revocationTime;

	public CrlReason RevocationReason => m_revocationReason;

	public static RevokedInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RevokedInfo result)
		{
			return result;
		}
		return new RevokedInfo(Asn1Sequence.GetInstance(obj));
	}

	public static RevokedInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new RevokedInfo(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public RevokedInfo(Asn1GeneralizedTime revocationTime)
		: this(revocationTime, null)
	{
	}

	public RevokedInfo(Asn1GeneralizedTime revocationTime, CrlReason revocationReason)
	{
		m_revocationTime = revocationTime ?? throw new ArgumentNullException("revocationTime");
		m_revocationReason = revocationReason;
	}

	private RevokedInfo(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_revocationTime = Asn1GeneralizedTime.GetInstance(seq[sequencePosition++]);
		m_revocationReason = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, (Asn1TaggedObject t, bool e) => new CrlReason(DerEnumerated.GetInstance(t, e)));
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_revocationTime);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_revocationReason);
		return new DerSequence(asn1EncodableVector);
	}
}
