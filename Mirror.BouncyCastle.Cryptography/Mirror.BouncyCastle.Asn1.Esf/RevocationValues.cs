using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class RevocationValues : Asn1Encodable
{
	private readonly Asn1Sequence m_crlVals;

	private readonly Asn1Sequence m_ocspVals;

	private readonly OtherRevVals m_otherRevVals;

	public OtherRevVals OtherRevVals => m_otherRevVals;

	public static RevocationValues GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RevocationValues result)
		{
			return result;
		}
		return new RevocationValues(Asn1Sequence.GetInstance(obj));
	}

	public static RevocationValues GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new RevocationValues(Asn1Sequence.GetInstance(obj, explicitly));
	}

	private RevocationValues(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 0 || count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_crlVals = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, Asn1Sequence.GetInstance);
		m_crlVals?.MapElements(CertificateList.GetInstance);
		m_ocspVals = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 1, state: true, Asn1Sequence.GetInstance);
		m_ocspVals?.MapElements(BasicOcspResponse.GetInstance);
		m_otherRevVals = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 2, state: true, OtherRevVals.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public RevocationValues(CertificateList[] crlVals, BasicOcspResponse[] ocspVals, OtherRevVals otherRevVals)
	{
		if (crlVals != null)
		{
			Asn1Encodable[] elements = crlVals;
			m_crlVals = DerSequence.FromElements(elements);
		}
		if (ocspVals != null)
		{
			Asn1Encodable[] elements = ocspVals;
			m_ocspVals = DerSequence.FromElements(elements);
		}
		m_otherRevVals = otherRevVals;
	}

	public RevocationValues(IEnumerable<CertificateList> crlVals, IEnumerable<BasicOcspResponse> ocspVals, OtherRevVals otherRevVals)
	{
		if (crlVals != null)
		{
			m_crlVals = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(crlVals));
		}
		if (ocspVals != null)
		{
			m_ocspVals = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(ocspVals));
		}
		m_otherRevVals = otherRevVals;
	}

	public CertificateList[] GetCrlVals()
	{
		return m_crlVals?.MapElements(CertificateList.GetInstance);
	}

	public BasicOcspResponse[] GetOcspVals()
	{
		return m_ocspVals?.MapElements(BasicOcspResponse.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_crlVals);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_ocspVals);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, m_otherRevVals);
		return DerSequence.FromVector(asn1EncodableVector);
	}
}
