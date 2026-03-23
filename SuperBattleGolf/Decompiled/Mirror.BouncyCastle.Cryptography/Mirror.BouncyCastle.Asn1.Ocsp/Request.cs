using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class Request : Asn1Encodable
{
	private readonly CertID m_reqCert;

	private readonly X509Extensions m_singleRequestExtensions;

	public CertID ReqCert => m_reqCert;

	public X509Extensions SingleRequestExtensions => m_singleRequestExtensions;

	public static Request GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Request result)
		{
			return result;
		}
		return new Request(Asn1Sequence.GetInstance(obj));
	}

	public static Request GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new Request(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public Request(CertID reqCert, X509Extensions singleRequestExtensions)
	{
		m_reqCert = reqCert ?? throw new ArgumentNullException("reqCert");
		m_singleRequestExtensions = singleRequestExtensions;
	}

	private Request(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_reqCert = CertID.GetInstance(seq[sequencePosition++]);
		m_singleRequestExtensions = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, X509Extensions.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_reqCert);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_singleRequestExtensions);
		return new DerSequence(asn1EncodableVector);
	}
}
