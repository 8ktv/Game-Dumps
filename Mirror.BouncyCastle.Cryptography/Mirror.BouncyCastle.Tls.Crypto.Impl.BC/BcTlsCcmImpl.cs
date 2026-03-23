using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Modes;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

internal class BcTlsCcmImpl : BcTlsAeadCipherImpl
{
	internal BcTlsCcmImpl(CcmBlockCipher cipher, bool isEncrypting)
		: base(cipher, isEncrypting)
	{
	}

	public override int DoFinal(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset)
	{
		if (!(m_cipher is CcmBlockCipher ccmBlockCipher))
		{
			throw new InvalidOperationException();
		}
		try
		{
			return ccmBlockCipher.ProcessPacket(input, inputOffset, inputLength, output, outputOffset);
		}
		catch (InvalidCipherTextException alertCause)
		{
			throw new TlsFatalAlert(20, alertCause);
		}
	}
}
