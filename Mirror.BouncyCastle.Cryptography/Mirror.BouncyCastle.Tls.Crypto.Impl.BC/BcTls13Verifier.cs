using System;
using System.IO;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

internal sealed class BcTls13Verifier : Tls13Verifier
{
	private readonly SignerSink m_output;

	public Stream Stream => m_output;

	internal BcTls13Verifier(ISigner verifier)
	{
		if (verifier == null)
		{
			throw new ArgumentNullException("verifier");
		}
		m_output = new SignerSink(verifier);
	}

	public bool VerifySignature(byte[] signature)
	{
		return m_output.Signer.VerifySignature(signature);
	}
}
