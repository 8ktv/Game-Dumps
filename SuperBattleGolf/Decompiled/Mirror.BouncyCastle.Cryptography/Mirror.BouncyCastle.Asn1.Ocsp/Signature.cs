using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class Signature : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_signatureAlgorithm;

	private readonly DerBitString m_signatureValue;

	private readonly Asn1Sequence m_certs;

	public AlgorithmIdentifier SignatureAlgorithm => m_signatureAlgorithm;

	public DerBitString SignatureValue => m_signatureValue;

	public Asn1Sequence Certs => m_certs;

	public static Signature GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Signature result)
		{
			return result;
		}
		return new Signature(Asn1Sequence.GetInstance(obj));
	}

	public static Signature GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new Signature(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public Signature(AlgorithmIdentifier signatureAlgorithm, DerBitString signatureValue)
		: this(signatureAlgorithm, signatureValue, null)
	{
	}

	public Signature(AlgorithmIdentifier signatureAlgorithm, DerBitString signatureValue, Asn1Sequence certs)
	{
		m_signatureAlgorithm = signatureAlgorithm ?? throw new ArgumentNullException("signatureAlgorithm");
		m_signatureValue = signatureValue ?? throw new ArgumentNullException("signatureValue");
		m_certs = certs;
	}

	private Signature(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 2 || count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_signatureAlgorithm = AlgorithmIdentifier.GetInstance(seq[sequencePosition++]);
		m_signatureValue = DerBitString.GetInstance(seq[sequencePosition++]);
		m_certs = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, Asn1Sequence.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public byte[] GetSignatureOctets()
	{
		return m_signatureValue.GetOctets();
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(m_signatureAlgorithm, m_signatureValue);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_certs);
		return new DerSequence(asn1EncodableVector);
	}
}
