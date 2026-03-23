using System.IO;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

internal sealed class BcTlsStreamVerifier : TlsStreamVerifier
{
	private readonly SignerSink m_output;

	private readonly byte[] m_signature;

	public Stream Stream => m_output;

	internal BcTlsStreamVerifier(ISigner verifier, byte[] signature)
	{
		m_output = new SignerSink(verifier);
		m_signature = signature;
	}

	public bool IsVerified()
	{
		return m_output.Signer.VerifySignature(m_signature);
	}
}
