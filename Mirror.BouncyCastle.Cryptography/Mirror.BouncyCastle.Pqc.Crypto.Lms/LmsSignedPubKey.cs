using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public class LmsSignedPubKey : IEncodable
{
	private readonly LmsSignature m_signature;

	private readonly LmsPublicKeyParameters m_publicKey;

	public LmsPublicKeyParameters PublicKey => m_publicKey;

	public LmsSignature Signature => m_signature;

	public LmsSignedPubKey(LmsSignature signature, LmsPublicKeyParameters publicKey)
	{
		m_signature = signature;
		m_publicKey = publicKey;
	}

	[Obsolete("Use 'PublicKey' instead")]
	public LmsPublicKeyParameters GetPublicKey()
	{
		return m_publicKey;
	}

	[Obsolete("Use 'Signature' instead")]
	public LmsSignature GetSignature()
	{
		return m_signature;
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj is LmsSignedPubKey lmsSignedPubKey && object.Equals(m_signature, lmsSignedPubKey.m_signature))
		{
			return object.Equals(m_publicKey, lmsSignedPubKey.m_publicKey);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int hashCode = Objects.GetHashCode(m_signature);
		return 31 * hashCode + Objects.GetHashCode(m_publicKey);
	}

	public byte[] GetEncoded()
	{
		return Composer.Compose().Bytes(m_signature.GetEncoded()).Bytes(m_publicKey.GetEncoded())
			.Build();
	}
}
