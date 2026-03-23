using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class BasicOcspResponse : Asn1Encodable
{
	private readonly ResponseData m_tbsResponseData;

	private readonly AlgorithmIdentifier m_signatureAlgorithm;

	private readonly DerBitString m_signature;

	private readonly Asn1Sequence m_certs;

	public ResponseData TbsResponseData => m_tbsResponseData;

	public AlgorithmIdentifier SignatureAlgorithm => m_signatureAlgorithm;

	public DerBitString Signature => m_signature;

	public Asn1Sequence Certs => m_certs;

	public static BasicOcspResponse GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is BasicOcspResponse result)
		{
			return result;
		}
		return new BasicOcspResponse(Asn1Sequence.GetInstance(obj));
	}

	public static BasicOcspResponse GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new BasicOcspResponse(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public BasicOcspResponse(ResponseData tbsResponseData, AlgorithmIdentifier signatureAlgorithm, DerBitString signature, Asn1Sequence certs)
	{
		m_tbsResponseData = tbsResponseData ?? throw new ArgumentNullException("tbsResponseData");
		m_signatureAlgorithm = signatureAlgorithm ?? throw new ArgumentNullException("signatureAlgorithm");
		m_signature = signature ?? throw new ArgumentNullException("signature");
		m_certs = certs;
	}

	private BasicOcspResponse(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 3 || count > 4)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_tbsResponseData = ResponseData.GetInstance(seq[sequencePosition++]);
		m_signatureAlgorithm = AlgorithmIdentifier.GetInstance(seq[sequencePosition++]);
		m_signature = DerBitString.GetInstance(seq[sequencePosition++]);
		m_certs = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, Asn1Sequence.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public byte[] GetSignatureOctets()
	{
		return m_signature.GetOctets();
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
		asn1EncodableVector.Add(m_tbsResponseData, m_signatureAlgorithm, m_signature);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_certs);
		return new DerSequence(asn1EncodableVector);
	}
}
