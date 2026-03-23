using System;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class OcspRequest : Asn1Encodable
{
	private readonly TbsRequest m_tbsRequest;

	private readonly Signature m_optionalSignature;

	public TbsRequest TbsRequest => m_tbsRequest;

	public Signature OptionalSignature => m_optionalSignature;

	public static OcspRequest GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OcspRequest result)
		{
			return result;
		}
		return new OcspRequest(Asn1Sequence.GetInstance(obj));
	}

	public static OcspRequest GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new OcspRequest(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public OcspRequest(TbsRequest tbsRequest, Signature optionalSignature)
	{
		m_tbsRequest = tbsRequest ?? throw new ArgumentNullException("tbsRequest");
		m_optionalSignature = optionalSignature;
	}

	private OcspRequest(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_tbsRequest = TbsRequest.GetInstance(seq[sequencePosition++]);
		m_optionalSignature = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, Signature.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_tbsRequest);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_optionalSignature);
		return new DerSequence(asn1EncodableVector);
	}
}
