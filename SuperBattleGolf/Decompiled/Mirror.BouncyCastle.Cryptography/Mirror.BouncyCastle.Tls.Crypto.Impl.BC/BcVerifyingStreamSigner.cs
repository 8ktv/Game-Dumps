using System.IO;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

internal sealed class BcVerifyingStreamSigner : TlsStreamSigner
{
	private readonly ISigner m_signer;

	private readonly ISigner m_verifier;

	private readonly TeeOutputStream m_output;

	public Stream Stream => m_output;

	internal BcVerifyingStreamSigner(ISigner signer, ISigner verifier)
	{
		Stream output = new SignerSink(signer);
		Stream tee = new SignerSink(verifier);
		m_signer = signer;
		m_verifier = verifier;
		m_output = new TeeOutputStream(output, tee);
	}

	public byte[] GetSignature()
	{
		try
		{
			byte[] array = m_signer.GenerateSignature();
			if (m_verifier.VerifySignature(array))
			{
				return array;
			}
		}
		catch (CryptoException alertCause)
		{
			throw new TlsFatalAlert(80, alertCause);
		}
		throw new TlsFatalAlert(80);
	}
}
