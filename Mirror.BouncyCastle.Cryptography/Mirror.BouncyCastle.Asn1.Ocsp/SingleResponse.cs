using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class SingleResponse : Asn1Encodable
{
	private readonly CertID m_certID;

	private readonly CertStatus m_certStatus;

	private readonly Asn1GeneralizedTime m_thisUpdate;

	private readonly Asn1GeneralizedTime m_nextUpdate;

	private readonly X509Extensions m_singleExtensions;

	public CertID CertId => m_certID;

	public CertStatus CertStatus => m_certStatus;

	public Asn1GeneralizedTime ThisUpdate => m_thisUpdate;

	public Asn1GeneralizedTime NextUpdate => m_nextUpdate;

	public X509Extensions SingleExtensions => m_singleExtensions;

	public static SingleResponse GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SingleResponse result)
		{
			return result;
		}
		return new SingleResponse(Asn1Sequence.GetInstance(obj));
	}

	public static SingleResponse GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new SingleResponse(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public SingleResponse(CertID certID, CertStatus certStatus, Asn1GeneralizedTime thisUpdate, Asn1GeneralizedTime nextUpdate, X509Extensions singleExtensions)
	{
		m_certID = certID ?? throw new ArgumentNullException("certID");
		m_certStatus = certStatus ?? throw new ArgumentNullException("certStatus");
		m_thisUpdate = thisUpdate ?? throw new ArgumentNullException("thisUpdate");
		m_nextUpdate = nextUpdate;
		m_singleExtensions = singleExtensions;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public SingleResponse(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 3 || count > 5)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_certID = CertID.GetInstance(seq[sequencePosition++]);
		m_certStatus = CertStatus.GetInstance(seq[sequencePosition++]);
		m_thisUpdate = Asn1GeneralizedTime.GetInstance(seq[sequencePosition++]);
		m_nextUpdate = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, Asn1GeneralizedTime.GetInstance);
		m_singleExtensions = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 1, state: true, X509Extensions.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(5);
		asn1EncodableVector.Add(m_certID, m_certStatus, m_thisUpdate);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_nextUpdate);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_singleExtensions);
		return new DerSequence(asn1EncodableVector);
	}
}
