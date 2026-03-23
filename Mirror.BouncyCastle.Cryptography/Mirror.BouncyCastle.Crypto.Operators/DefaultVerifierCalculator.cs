using System.IO;
using Mirror.BouncyCastle.Crypto.IO;

namespace Mirror.BouncyCastle.Crypto.Operators;

public class DefaultVerifierCalculator : IStreamCalculator<IVerifier>
{
	private readonly SignerSink m_signerSink;

	public Stream Stream => m_signerSink;

	public DefaultVerifierCalculator(ISigner signer)
	{
		m_signerSink = new SignerSink(signer);
	}

	public IVerifier GetResult()
	{
		return new DefaultVerifierResult(m_signerSink.Signer);
	}
}
