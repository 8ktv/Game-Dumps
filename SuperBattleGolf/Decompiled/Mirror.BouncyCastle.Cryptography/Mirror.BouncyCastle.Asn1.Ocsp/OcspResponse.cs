using System;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class OcspResponse : Asn1Encodable
{
	private readonly OcspResponseStatus m_responseStatus;

	private readonly ResponseBytes m_responseBytes;

	public OcspResponseStatus ResponseStatus => m_responseStatus;

	public ResponseBytes ResponseBytes => m_responseBytes;

	public static OcspResponse GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OcspResponse result)
		{
			return result;
		}
		return new OcspResponse(Asn1Sequence.GetInstance(obj));
	}

	public static OcspResponse GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new OcspResponse(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public OcspResponse(OcspResponseStatus responseStatus, ResponseBytes responseBytes)
	{
		m_responseStatus = responseStatus ?? throw new ArgumentNullException("responseStatus");
		m_responseBytes = responseBytes;
	}

	private OcspResponse(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_responseStatus = new OcspResponseStatus(DerEnumerated.GetInstance(seq[sequencePosition++]));
		m_responseBytes = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, ResponseBytes.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_responseStatus);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_responseBytes);
		return new DerSequence(asn1EncodableVector);
	}
}
