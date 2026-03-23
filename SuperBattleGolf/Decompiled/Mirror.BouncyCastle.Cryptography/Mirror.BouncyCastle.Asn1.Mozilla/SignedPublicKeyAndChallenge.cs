using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Mozilla;

public class SignedPublicKeyAndChallenge : Asn1Encodable
{
	private readonly PublicKeyAndChallenge m_publicKeyAndChallenge;

	private readonly AlgorithmIdentifier m_signatureAlgorithm;

	private readonly DerBitString m_signature;

	public PublicKeyAndChallenge PublicKeyAndChallenge => m_publicKeyAndChallenge;

	public DerBitString Signature => m_signature;

	public AlgorithmIdentifier SignatureAlgorithm => m_signatureAlgorithm;

	public static SignedPublicKeyAndChallenge GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SignedPublicKeyAndChallenge result)
		{
			return result;
		}
		return new SignedPublicKeyAndChallenge(Asn1Sequence.GetInstance(obj));
	}

	public static SignedPublicKeyAndChallenge GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new SignedPublicKeyAndChallenge(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public SignedPublicKeyAndChallenge(PublicKeyAndChallenge publicKeyAndChallenge, AlgorithmIdentifier signatureAlgorithm, DerBitString signature)
	{
		m_publicKeyAndChallenge = publicKeyAndChallenge ?? throw new ArgumentNullException("publicKeyAndChallenge");
		m_signatureAlgorithm = signatureAlgorithm ?? throw new ArgumentNullException("signatureAlgorithm");
		m_signature = signature ?? throw new ArgumentNullException("signature");
	}

	private SignedPublicKeyAndChallenge(Asn1Sequence seq)
	{
		if (seq == null)
		{
			throw new ArgumentNullException("seq");
		}
		if (seq.Count != 3)
		{
			throw new ArgumentException($"Expected 3 elements, but found {seq.Count}", "seq");
		}
		m_publicKeyAndChallenge = PublicKeyAndChallenge.GetInstance(seq[0]);
		m_signatureAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
		m_signature = DerBitString.GetInstance(seq[2]);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_publicKeyAndChallenge, m_signatureAlgorithm, m_signature);
	}
}
