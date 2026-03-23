using System;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class ResponseBytes : Asn1Encodable
{
	private readonly DerObjectIdentifier m_responseType;

	private readonly Asn1OctetString m_response;

	public DerObjectIdentifier ResponseType => m_responseType;

	public Asn1OctetString Response => m_response;

	public static ResponseBytes GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ResponseBytes result)
		{
			return result;
		}
		return new ResponseBytes(Asn1Sequence.GetInstance(obj));
	}

	public static ResponseBytes GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new ResponseBytes(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public ResponseBytes(DerObjectIdentifier responseType, Asn1OctetString response)
	{
		m_responseType = responseType ?? throw new ArgumentNullException("responseType");
		m_response = response ?? throw new ArgumentNullException("response");
	}

	private ResponseBytes(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count != 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_responseType = DerObjectIdentifier.GetInstance(seq[0]);
		m_response = Asn1OctetString.GetInstance(seq[1]);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_responseType, m_response);
	}
}
