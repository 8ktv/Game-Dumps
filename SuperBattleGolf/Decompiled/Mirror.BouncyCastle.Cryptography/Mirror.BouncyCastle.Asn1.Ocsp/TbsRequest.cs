using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class TbsRequest : Asn1Encodable
{
	private static readonly DerInteger V1 = new DerInteger(0);

	private readonly DerInteger m_version;

	private readonly bool m_versionPresent;

	private readonly GeneralName m_requestorName;

	private readonly Asn1Sequence m_requestList;

	private readonly X509Extensions m_requestExtensions;

	public DerInteger Version => m_version;

	public GeneralName RequestorName => m_requestorName;

	public Asn1Sequence RequestList => m_requestList;

	public X509Extensions RequestExtensions => m_requestExtensions;

	public static TbsRequest GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is TbsRequest result)
		{
			return result;
		}
		return new TbsRequest(Asn1Sequence.GetInstance(obj));
	}

	public static TbsRequest GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new TbsRequest(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public TbsRequest(GeneralName requestorName, Asn1Sequence requestList, X509Extensions requestExtensions)
	{
		m_version = V1;
		m_versionPresent = false;
		m_requestorName = requestorName;
		m_requestList = requestList ?? throw new ArgumentNullException("requestList");
		m_requestExtensions = requestExtensions;
	}

	private TbsRequest(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 4)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		DerInteger derInteger = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, DerInteger.GetInstance);
		m_version = derInteger ?? V1;
		m_versionPresent = derInteger != null;
		m_requestorName = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 1, state: true, GeneralName.GetInstance);
		m_requestList = Asn1Sequence.GetInstance(seq[sequencePosition++]);
		m_requestExtensions = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 2, state: true, X509Extensions.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
		if (m_versionPresent || !V1.Equals(m_version))
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 0, m_version));
		}
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_requestorName);
		asn1EncodableVector.Add(m_requestList);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, m_requestExtensions);
		return new DerSequence(asn1EncodableVector);
	}
}
