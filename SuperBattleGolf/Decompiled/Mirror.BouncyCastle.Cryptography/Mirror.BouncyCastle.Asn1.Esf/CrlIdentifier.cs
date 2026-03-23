using System;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CrlIdentifier : Asn1Encodable
{
	private readonly X509Name m_crlIssuer;

	private readonly Asn1UtcTime m_crlIssuedTime;

	private readonly DerInteger m_crlNumber;

	public X509Name CrlIssuer => m_crlIssuer;

	public DateTime CrlIssuedTime => m_crlIssuedTime.ToDateTime(2049);

	public BigInteger CrlNumber => m_crlNumber?.Value;

	public static CrlIdentifier GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CrlIdentifier result)
		{
			return result;
		}
		return new CrlIdentifier(Asn1Sequence.GetInstance(obj));
	}

	public static CrlIdentifier GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CrlIdentifier(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 2 || count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_crlIssuer = X509Name.GetInstance(seq[0]);
		m_crlIssuedTime = Asn1UtcTime.GetInstance(seq[1]);
		m_crlIssuedTime.ToDateTime(2049);
		if (count > 2)
		{
			m_crlNumber = DerInteger.GetInstance(seq[2]);
		}
	}

	public CrlIdentifier(X509Name crlIssuer, DateTime crlIssuedTime)
		: this(crlIssuer, crlIssuedTime, null)
	{
	}

	public CrlIdentifier(X509Name crlIssuer, DateTime crlIssuedTime, BigInteger crlNumber)
		: this(crlIssuer, Rfc5280Asn1Utilities.CreateUtcTime(crlIssuedTime), crlNumber)
	{
	}

	public CrlIdentifier(X509Name crlIssuer, Asn1UtcTime crlIssuedTime)
		: this(crlIssuer, crlIssuedTime, null)
	{
	}

	public CrlIdentifier(X509Name crlIssuer, Asn1UtcTime crlIssuedTime, BigInteger crlNumber)
	{
		m_crlIssuer = crlIssuer ?? throw new ArgumentNullException("crlIssuer");
		m_crlIssuedTime = crlIssuedTime ?? throw new ArgumentNullException("crlIssuedTime");
		if (crlNumber != null)
		{
			m_crlNumber = new DerInteger(crlNumber);
		}
		m_crlIssuedTime.ToDateTime(2049);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(m_crlIssuer, m_crlIssuedTime);
		asn1EncodableVector.AddOptional(m_crlNumber);
		return new DerSequence(asn1EncodableVector);
	}
}
