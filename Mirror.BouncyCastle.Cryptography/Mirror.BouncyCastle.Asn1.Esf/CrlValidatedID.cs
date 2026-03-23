using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CrlValidatedID : Asn1Encodable
{
	private readonly OtherHash m_crlHash;

	private readonly CrlIdentifier m_crlIdentifier;

	public OtherHash CrlHash => m_crlHash;

	public CrlIdentifier CrlIdentifier => m_crlIdentifier;

	public static CrlValidatedID GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CrlValidatedID result)
		{
			return result;
		}
		return new CrlValidatedID(Asn1Sequence.GetInstance(obj));
	}

	public static CrlValidatedID GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CrlValidatedID(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CrlValidatedID(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_crlHash = OtherHash.GetInstance(seq[0]);
		if (count > 1)
		{
			m_crlIdentifier = CrlIdentifier.GetInstance(seq[1]);
		}
	}

	public CrlValidatedID(OtherHash crlHash)
		: this(crlHash, null)
	{
	}

	public CrlValidatedID(OtherHash crlHash, CrlIdentifier crlIdentifier)
	{
		m_crlHash = crlHash ?? throw new ArgumentNullException("crlHash");
		m_crlIdentifier = crlIdentifier;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_crlHash);
		asn1EncodableVector.AddOptional(m_crlIdentifier);
		return new DerSequence(asn1EncodableVector);
	}
}
