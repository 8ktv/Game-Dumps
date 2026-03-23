using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Operators;

public class DefaultVerifierResult : IVerifier
{
	private readonly ISigner m_signer;

	public DefaultVerifierResult(ISigner signer)
	{
		m_signer = signer;
	}

	public bool IsVerified(byte[] signature)
	{
		return m_signer.VerifySignature(signature);
	}

	public bool IsVerified(byte[] sig, int sigOff, int sigLen)
	{
		return IsVerified(Arrays.CopyOfRange(sig, sigOff, sigOff + sigLen));
	}
}
