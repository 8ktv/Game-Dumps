using System;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OcspIdentifier : Asn1Encodable
{
	private readonly ResponderID m_ocspResponderID;

	private readonly Asn1GeneralizedTime m_producedAt;

	public ResponderID OcspResponderID => m_ocspResponderID;

	public Asn1GeneralizedTime ProducedAtData => m_producedAt;

	public DateTime ProducedAt => m_producedAt.ToDateTime();

	public static OcspIdentifier GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OcspIdentifier result)
		{
			return result;
		}
		return new OcspIdentifier(Asn1Sequence.GetInstance(obj));
	}

	public static OcspIdentifier GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new OcspIdentifier(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private OcspIdentifier(Asn1Sequence seq)
	{
		int num = 2;
		if (num != 2)
		{
			throw new ArgumentException("Bad sequence size: " + num, "seq");
		}
		m_ocspResponderID = ResponderID.GetInstance(seq[0]);
		m_producedAt = Asn1GeneralizedTime.GetInstance(seq[1]);
	}

	public OcspIdentifier(ResponderID ocspResponderID, DateTime producedAt)
		: this(ocspResponderID, Rfc5280Asn1Utilities.CreateGeneralizedTime(producedAt))
	{
	}

	public OcspIdentifier(ResponderID ocspResponderID, Asn1GeneralizedTime producedAt)
	{
		m_ocspResponderID = ocspResponderID ?? throw new ArgumentNullException("ocspResponderID");
		m_producedAt = producedAt ?? throw new ArgumentNullException("producedAt");
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_ocspResponderID, m_producedAt);
	}
}
