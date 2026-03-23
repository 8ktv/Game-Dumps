using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Crypto.Agreement;

public sealed class X448Agreement : IRawAgreement
{
	private X448PrivateKeyParameters m_privateKey;

	public int AgreementSize => X448PrivateKeyParameters.SecretSize;

	public void Init(ICipherParameters parameters)
	{
		m_privateKey = (X448PrivateKeyParameters)parameters;
	}

	public void CalculateAgreement(ICipherParameters publicKey, byte[] buf, int off)
	{
		m_privateKey.GenerateSecret((X448PublicKeyParameters)publicKey, buf, off);
	}
}
