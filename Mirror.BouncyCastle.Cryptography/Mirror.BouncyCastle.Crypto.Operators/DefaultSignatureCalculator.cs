using System.IO;
using Mirror.BouncyCastle.Crypto.IO;

namespace Mirror.BouncyCastle.Crypto.Operators;

public class DefaultSignatureCalculator : IStreamCalculator<IBlockResult>
{
	private readonly SignerSink m_signerSink;

	public Stream Stream => m_signerSink;

	public DefaultSignatureCalculator(ISigner signer)
	{
		m_signerSink = new SignerSink(signer);
	}

	public IBlockResult GetResult()
	{
		return new DefaultSignatureResult(m_signerSink.Signer);
	}
}
