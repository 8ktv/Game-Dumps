using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class ResponseData : Asn1Encodable
{
	private static readonly DerInteger V1 = new DerInteger(0);

	private readonly DerInteger m_version;

	private readonly bool m_versionPresent;

	private readonly ResponderID m_responderID;

	private readonly Asn1GeneralizedTime m_producedAt;

	private readonly Asn1Sequence m_responses;

	private readonly X509Extensions m_responseExtensions;

	public DerInteger Version => m_version;

	public ResponderID ResponderID => m_responderID;

	public Asn1GeneralizedTime ProducedAt => m_producedAt;

	public Asn1Sequence Responses => m_responses;

	public X509Extensions ResponseExtensions => m_responseExtensions;

	public static ResponseData GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ResponseData result)
		{
			return result;
		}
		return new ResponseData(Asn1Sequence.GetInstance(obj));
	}

	public static ResponseData GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new ResponseData(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public ResponseData(ResponderID responderID, Asn1GeneralizedTime producedAt, Asn1Sequence responses, X509Extensions responseExtensions)
		: this(V1, responderID, producedAt, responses, responseExtensions)
	{
	}

	public ResponseData(DerInteger version, ResponderID responderID, Asn1GeneralizedTime producedAt, Asn1Sequence responses, X509Extensions responseExtensions)
	{
		m_version = version ?? V1;
		m_versionPresent = false;
		m_responderID = responderID ?? throw new ArgumentNullException("responderID");
		m_producedAt = producedAt ?? throw new ArgumentNullException("producedAt");
		m_responses = responses ?? throw new ArgumentNullException("responses");
		m_responseExtensions = responseExtensions;
	}

	private ResponseData(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 3 || count > 5)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		DerInteger derInteger = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, DerInteger.GetInstance);
		m_version = derInteger ?? V1;
		m_versionPresent = derInteger != null;
		m_responderID = ResponderID.GetInstance(seq[sequencePosition++]);
		m_producedAt = Asn1GeneralizedTime.GetInstance(seq[sequencePosition++]);
		m_responses = Asn1Sequence.GetInstance(seq[sequencePosition++]);
		m_responseExtensions = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 1, state: true, X509Extensions.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(5);
		if (m_versionPresent || !V1.Equals(m_version))
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 0, m_version));
		}
		asn1EncodableVector.Add(m_responderID, m_producedAt, m_responses);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_responseExtensions);
		return new DerSequence(asn1EncodableVector);
	}
}
