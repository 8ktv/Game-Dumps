using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Crypto.Agreement;

public sealed class X25519Agreement : IRawAgreement
{
	private X25519PrivateKeyParameters m_privateKey;

	public int AgreementSize => X25519PrivateKeyParameters.SecretSize;

	public void Init(ICipherParameters parameters)
	{
		m_privateKey = (X25519PrivateKeyParameters)parameters;
	}

	public void CalculateAgreement(ICipherParameters publicKey, byte[] buf, int off)
	{
		m_privateKey.GenerateSecret((X25519PublicKeyParameters)publicKey, buf, off);
	}
}
