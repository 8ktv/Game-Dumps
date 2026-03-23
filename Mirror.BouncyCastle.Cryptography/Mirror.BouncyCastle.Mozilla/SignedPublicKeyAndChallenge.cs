using System;
using Mirror.BouncyCastle.Asn1.Mozilla;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Mozilla;

public sealed class SignedPublicKeyAndChallenge
{
	private readonly Mirror.BouncyCastle.Asn1.Mozilla.SignedPublicKeyAndChallenge m_spkac;

	public SignedPublicKeyAndChallenge(byte[] encoding)
		: this(Mirror.BouncyCastle.Asn1.Mozilla.SignedPublicKeyAndChallenge.GetInstance(encoding))
	{
	}

	public SignedPublicKeyAndChallenge(Mirror.BouncyCastle.Asn1.Mozilla.SignedPublicKeyAndChallenge spkac)
	{
		m_spkac = spkac ?? throw new ArgumentNullException("spkac");
	}

	public AsymmetricKeyParameter GetPublicKey()
	{
		return PublicKeyFactory.CreateKey(m_spkac.PublicKeyAndChallenge.Spki);
	}

	public bool IsSignatureValid(AsymmetricKeyParameter publicKey)
	{
		return CheckSignatureValid(new Asn1VerifierFactory(m_spkac.SignatureAlgorithm, publicKey));
	}

	public bool IsSignatureValid(IVerifierFactoryProvider verifierProvider)
	{
		return CheckSignatureValid(verifierProvider.CreateVerifierFactory(m_spkac.SignatureAlgorithm));
	}

	public Mirror.BouncyCastle.Asn1.Mozilla.SignedPublicKeyAndChallenge ToAsn1Structure()
	{
		return m_spkac;
	}

	public void Verify(AsymmetricKeyParameter publicKey)
	{
		CheckSignature(new Asn1VerifierFactory(m_spkac.SignatureAlgorithm, publicKey));
	}

	public void Verify(IVerifierFactoryProvider verifierProvider)
	{
		CheckSignature(verifierProvider.CreateVerifierFactory(m_spkac.SignatureAlgorithm));
	}

	private void CheckSignature(IVerifierFactory verifier)
	{
		if (!CheckSignatureValid(verifier))
		{
			throw new InvalidKeyException("Public key presented not for SPKAC signature");
		}
	}

	private bool CheckSignatureValid(IVerifierFactory verifier)
	{
		return Mirror.BouncyCastle.X509.X509Utilities.VerifySignature(verifier, m_spkac.PublicKeyAndChallenge, m_spkac.Signature);
	}
}
