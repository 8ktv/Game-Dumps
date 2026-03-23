using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Mozilla;

public class PublicKeyAndChallenge : Asn1Encodable
{
	private readonly SubjectPublicKeyInfo m_spki;

	private readonly DerIA5String m_challenge;

	public DerIA5String Challenge => m_challenge;

	public SubjectPublicKeyInfo Spki => m_spki;

	[Obsolete("Use 'Spki' instead")]
	public SubjectPublicKeyInfo SubjectPublicKeyInfo => m_spki;

	public static PublicKeyAndChallenge GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PublicKeyAndChallenge result)
		{
			return result;
		}
		return new PublicKeyAndChallenge(Asn1Sequence.GetInstance(obj));
	}

	public static PublicKeyAndChallenge GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PublicKeyAndChallenge(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public PublicKeyAndChallenge(SubjectPublicKeyInfo spki, DerIA5String challenge)
	{
		m_spki = spki ?? throw new ArgumentNullException("spki");
		m_challenge = challenge ?? throw new ArgumentNullException("m_challenge");
	}

	[Obsolete("Use 'GetInstance' instead")]
	public PublicKeyAndChallenge(Asn1Sequence seq)
	{
		if (seq == null)
		{
			throw new ArgumentNullException("seq");
		}
		if (seq.Count != 2)
		{
			throw new ArgumentException($"Expected 2 elements, but found {seq.Count}", "seq");
		}
		m_spki = SubjectPublicKeyInfo.GetInstance(seq[0]);
		m_challenge = DerIA5String.GetInstance(seq[1]);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_spki, m_challenge);
	}
}
