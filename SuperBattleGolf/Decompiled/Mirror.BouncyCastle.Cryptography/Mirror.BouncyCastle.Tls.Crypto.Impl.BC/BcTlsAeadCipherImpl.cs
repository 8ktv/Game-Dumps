using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

internal class BcTlsAeadCipherImpl : TlsAeadCipherImpl
{
	private readonly bool m_isEncrypting;

	internal readonly IAeadCipher m_cipher;

	private KeyParameter key;

	internal BcTlsAeadCipherImpl(IAeadCipher cipher, bool isEncrypting)
	{
		m_cipher = cipher;
		m_isEncrypting = isEncrypting;
	}

	public void SetKey(byte[] key, int keyOff, int keyLen)
	{
		this.key = new KeyParameter(key, keyOff, keyLen);
	}

	public void Init(byte[] nonce, int macSize, byte[] additionalData)
	{
		m_cipher.Init(m_isEncrypting, new AeadParameters(key, macSize * 8, nonce, additionalData));
	}

	public int GetOutputSize(int inputLength)
	{
		return m_cipher.GetOutputSize(inputLength);
	}

	public virtual int DoFinal(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset)
	{
		int num = m_cipher.ProcessBytes(input, inputOffset, inputLength, output, outputOffset);
		try
		{
			return num + m_cipher.DoFinal(output, outputOffset + num);
		}
		catch (InvalidCipherTextException alertCause)
		{
			throw new TlsFatalAlert(20, alertCause);
		}
	}

	public virtual int DoFinal(byte[] additionalData, byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset)
	{
		if (!Arrays.IsNullOrEmpty(additionalData))
		{
			m_cipher.ProcessAadBytes(additionalData, 0, additionalData.Length);
		}
		return DoFinal(input, inputOffset, inputLength, output, outputOffset);
	}

	public void Reset()
	{
	}
}
