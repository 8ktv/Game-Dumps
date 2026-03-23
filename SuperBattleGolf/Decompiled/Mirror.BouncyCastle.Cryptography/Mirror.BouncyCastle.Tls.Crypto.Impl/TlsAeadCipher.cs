using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl;

public class TlsAeadCipher : TlsCipher, TlsCipherExt
{
	public const int AEAD_CCM = 1;

	public const int AEAD_CHACHA20_POLY1305 = 2;

	public const int AEAD_GCM = 3;

	private const int NONCE_RFC5288 = 1;

	private const int NONCE_RFC7905 = 2;

	private const long SequenceNumberPlaceholder = -1L;

	protected readonly TlsCryptoParameters m_cryptoParams;

	protected readonly int m_keySize;

	protected readonly int m_macSize;

	protected readonly int m_fixed_iv_length;

	protected readonly int m_record_iv_length;

	protected readonly TlsAeadCipherImpl m_decryptCipher;

	protected readonly TlsAeadCipherImpl m_encryptCipher;

	protected readonly byte[] m_decryptNonce;

	protected readonly byte[] m_encryptNonce;

	protected readonly byte[] m_decryptConnectionID;

	protected readonly byte[] m_encryptConnectionID;

	protected readonly bool m_decryptUseInnerPlaintext;

	protected readonly bool m_encryptUseInnerPlaintext;

	protected readonly bool m_isTlsV13;

	protected readonly int m_nonceMode;

	public virtual bool UsesOpaqueRecordType => m_isTlsV13;

	public TlsAeadCipher(TlsCryptoParameters cryptoParams, TlsAeadCipherImpl encryptCipher, TlsAeadCipherImpl decryptCipher, int keySize, int macSize, int aeadType)
	{
		SecurityParameters securityParameters = cryptoParams.SecurityParameters;
		ProtocolVersion negotiatedVersion = securityParameters.NegotiatedVersion;
		if (!TlsImplUtilities.IsTlsV12(negotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		m_isTlsV13 = TlsImplUtilities.IsTlsV13(negotiatedVersion);
		m_nonceMode = GetNonceMode(m_isTlsV13, aeadType);
		m_decryptConnectionID = securityParameters.ConnectionIDPeer;
		m_encryptConnectionID = securityParameters.ConnectionIDLocal;
		m_decryptUseInnerPlaintext = m_isTlsV13 || !Arrays.IsNullOrEmpty(m_decryptConnectionID);
		m_encryptUseInnerPlaintext = m_isTlsV13 || !Arrays.IsNullOrEmpty(m_encryptConnectionID);
		switch (m_nonceMode)
		{
		case 1:
			m_fixed_iv_length = 4;
			m_record_iv_length = 8;
			break;
		case 2:
			m_fixed_iv_length = 12;
			m_record_iv_length = 0;
			break;
		default:
			throw new TlsFatalAlert(80);
		}
		m_cryptoParams = cryptoParams;
		m_keySize = keySize;
		m_macSize = macSize;
		m_decryptCipher = decryptCipher;
		m_encryptCipher = encryptCipher;
		m_decryptNonce = new byte[m_fixed_iv_length];
		m_encryptNonce = new byte[m_fixed_iv_length];
		bool isServer = cryptoParams.IsServer;
		if (m_isTlsV13)
		{
			RekeyCipher(securityParameters, decryptCipher, m_decryptNonce, !isServer);
			RekeyCipher(securityParameters, encryptCipher, m_encryptNonce, isServer);
			return;
		}
		int num = 2 * keySize + 2 * m_fixed_iv_length;
		byte[] array = TlsImplUtilities.CalculateKeyBlock(cryptoParams, num);
		int num2 = 0;
		if (isServer)
		{
			decryptCipher.SetKey(array, num2, keySize);
			num2 += keySize;
			encryptCipher.SetKey(array, num2, keySize);
			num2 += keySize;
			Array.Copy(array, num2, m_decryptNonce, 0, m_fixed_iv_length);
			num2 += m_fixed_iv_length;
			Array.Copy(array, num2, m_encryptNonce, 0, m_fixed_iv_length);
			num2 += m_fixed_iv_length;
		}
		else
		{
			encryptCipher.SetKey(array, num2, keySize);
			num2 += keySize;
			decryptCipher.SetKey(array, num2, keySize);
			num2 += keySize;
			Array.Copy(array, num2, m_encryptNonce, 0, m_fixed_iv_length);
			num2 += m_fixed_iv_length;
			Array.Copy(array, num2, m_decryptNonce, 0, m_fixed_iv_length);
			num2 += m_fixed_iv_length;
		}
		if (num2 == num)
		{
			return;
		}
		throw new TlsFatalAlert(80);
	}

	public virtual int GetCiphertextDecodeLimit(int plaintextLimit)
	{
		return plaintextLimit + (m_decryptUseInnerPlaintext ? 1 : 0) + m_macSize + m_record_iv_length;
	}

	public virtual int GetCiphertextEncodeLimit(int plaintextLength, int plaintextLimit)
	{
		plaintextLimit = System.Math.Min(plaintextLength, plaintextLimit);
		return plaintextLimit + (m_encryptUseInnerPlaintext ? 1 : 0) + m_macSize + m_record_iv_length;
	}

	public virtual int GetPlaintextLimit(int ciphertextLimit)
	{
		return GetPlaintextEncodeLimit(ciphertextLimit);
	}

	public virtual int GetPlaintextDecodeLimit(int ciphertextLimit)
	{
		return ciphertextLimit - m_macSize - m_record_iv_length - (m_decryptUseInnerPlaintext ? 1 : 0);
	}

	public virtual int GetPlaintextEncodeLimit(int ciphertextLimit)
	{
		return ciphertextLimit - m_macSize - m_record_iv_length - (m_encryptUseInnerPlaintext ? 1 : 0);
	}

	public virtual TlsEncodeResult EncodePlaintext(long seqNo, short contentType, ProtocolVersion recordVersion, int headerAllocation, byte[] plaintext, int plaintextOffset, int plaintextLength)
	{
		byte[] array = new byte[m_encryptNonce.Length + m_record_iv_length];
		switch (m_nonceMode)
		{
		case 1:
			Array.Copy(m_encryptNonce, 0, array, 0, m_encryptNonce.Length);
			TlsUtilities.WriteUint64(seqNo, array, m_encryptNonce.Length);
			break;
		case 2:
		{
			TlsUtilities.WriteUint64(seqNo, array, array.Length - 8);
			for (int i = 0; i < m_encryptNonce.Length; i++)
			{
				array[i] ^= m_encryptNonce[i];
			}
			break;
		}
		default:
			throw new TlsFatalAlert(80);
		}
		int num = plaintextLength + (m_encryptUseInnerPlaintext ? 1 : 0);
		m_encryptCipher.Init(array, m_macSize, null);
		int outputSize = m_encryptCipher.GetOutputSize(num);
		int num2 = m_record_iv_length + outputSize;
		byte[] array2 = new byte[headerAllocation + num2];
		int num3 = headerAllocation;
		if (m_record_iv_length != 0)
		{
			Array.Copy(array, array.Length - m_record_iv_length, array2, num3, m_record_iv_length);
			num3 += m_record_iv_length;
		}
		short recordType = contentType;
		if (m_encryptUseInnerPlaintext)
		{
			recordType = (short)(m_isTlsV13 ? 23 : 25);
		}
		byte[] additionalData = GetAdditionalData(seqNo, recordType, recordVersion, num2, num, m_encryptConnectionID);
		try
		{
			Array.Copy(plaintext, plaintextOffset, array2, num3, plaintextLength);
			if (m_encryptUseInnerPlaintext)
			{
				array2[num3 + plaintextLength] = (byte)contentType;
			}
			num3 += m_encryptCipher.DoFinal(additionalData, array2, num3, num, array2, num3);
		}
		catch (IOException)
		{
			throw;
		}
		catch (Exception alertCause)
		{
			throw new TlsFatalAlert(80, alertCause);
		}
		if (num3 != array2.Length)
		{
			throw new TlsFatalAlert(80);
		}
		return new TlsEncodeResult(array2, 0, array2.Length, recordType);
	}

	public virtual TlsDecodeResult DecodeCiphertext(long seqNo, short recordType, ProtocolVersion recordVersion, byte[] ciphertext, int ciphertextOffset, int ciphertextLength)
	{
		if (GetPlaintextDecodeLimit(ciphertextLength) < 0)
		{
			throw new TlsFatalAlert(50);
		}
		byte[] array = new byte[m_decryptNonce.Length + m_record_iv_length];
		switch (m_nonceMode)
		{
		case 1:
			Array.Copy(m_decryptNonce, 0, array, 0, m_decryptNonce.Length);
			Array.Copy(ciphertext, ciphertextOffset, array, array.Length - m_record_iv_length, m_record_iv_length);
			break;
		case 2:
		{
			TlsUtilities.WriteUint64(seqNo, array, array.Length - 8);
			for (int i = 0; i < m_decryptNonce.Length; i++)
			{
				array[i] ^= m_decryptNonce[i];
			}
			break;
		}
		default:
			throw new TlsFatalAlert(80);
		}
		m_decryptCipher.Init(array, m_macSize, null);
		int num = ciphertextOffset + m_record_iv_length;
		int inputLength = ciphertextLength - m_record_iv_length;
		int outputSize = m_decryptCipher.GetOutputSize(inputLength);
		byte[] additionalData = GetAdditionalData(seqNo, recordType, recordVersion, ciphertextLength, outputSize, m_decryptConnectionID);
		int num2;
		try
		{
			num2 = m_decryptCipher.DoFinal(additionalData, ciphertext, num, inputLength, ciphertext, num);
		}
		catch (IOException)
		{
			throw;
		}
		catch (Exception alertCause)
		{
			throw new TlsFatalAlert(20, alertCause);
		}
		if (num2 != outputSize)
		{
			throw new TlsFatalAlert(80);
		}
		short contentType = recordType;
		int num3 = outputSize;
		if (m_decryptUseInnerPlaintext)
		{
			byte b;
			do
			{
				if (--num3 < 0)
				{
					throw new TlsFatalAlert(10);
				}
				b = ciphertext[num + num3];
			}
			while (b == 0);
			contentType = (short)(b & 0xFF);
		}
		return new TlsDecodeResult(ciphertext, num, num3, contentType);
	}

	public virtual void RekeyDecoder()
	{
		RekeyCipher(m_cryptoParams.SecurityParameters, m_decryptCipher, m_decryptNonce, !m_cryptoParams.IsServer);
	}

	public virtual void RekeyEncoder()
	{
		RekeyCipher(m_cryptoParams.SecurityParameters, m_encryptCipher, m_encryptNonce, m_cryptoParams.IsServer);
	}

	protected virtual byte[] GetAdditionalData(long seqNo, short recordType, ProtocolVersion recordVersion, int ciphertextLength, int plaintextLength)
	{
		if (m_isTlsV13)
		{
			byte[] array = new byte[5];
			TlsUtilities.WriteUint8(recordType, array, 0);
			TlsUtilities.WriteVersion(recordVersion, array, 1);
			TlsUtilities.WriteUint16(ciphertextLength, array, 3);
			return array;
		}
		byte[] array2 = new byte[13];
		TlsUtilities.WriteUint64(seqNo, array2, 0);
		TlsUtilities.WriteUint8(recordType, array2, 8);
		TlsUtilities.WriteVersion(recordVersion, array2, 9);
		TlsUtilities.WriteUint16(plaintextLength, array2, 11);
		return array2;
	}

	protected virtual byte[] GetAdditionalData(long seqNo, short recordType, ProtocolVersion recordVersion, int ciphertextLength, int plaintextLength, byte[] connectionID)
	{
		if (Arrays.IsNullOrEmpty(connectionID))
		{
			return GetAdditionalData(seqNo, recordType, recordVersion, ciphertextLength, plaintextLength);
		}
		int num = connectionID.Length;
		byte[] array = new byte[23 + num];
		TlsUtilities.WriteUint64(-1L, array, 0);
		TlsUtilities.WriteUint8((short)25, array, 8);
		TlsUtilities.WriteUint8(num, array, 9);
		TlsUtilities.WriteUint8((short)25, array, 10);
		TlsUtilities.WriteVersion(recordVersion, array, 11);
		TlsUtilities.WriteUint64(seqNo, array, 13);
		Array.Copy(connectionID, 0, array, 21, num);
		TlsUtilities.WriteUint16(plaintextLength, array, 21 + num);
		return array;
	}

	protected virtual void RekeyCipher(SecurityParameters securityParameters, TlsAeadCipherImpl cipher, byte[] nonce, bool serverSecret)
	{
		if (!m_isTlsV13)
		{
			throw new TlsFatalAlert(80);
		}
		TlsSecret tlsSecret = (serverSecret ? securityParameters.TrafficSecretServer : securityParameters.TrafficSecretClient);
		if (tlsSecret == null)
		{
			throw new TlsFatalAlert(80);
		}
		Setup13Cipher(cipher, nonce, tlsSecret, securityParameters.PrfCryptoHashAlgorithm);
	}

	protected virtual void Setup13Cipher(TlsAeadCipherImpl cipher, byte[] nonce, TlsSecret secret, int cryptoHashAlgorithm)
	{
		byte[] key = TlsCryptoUtilities.HkdfExpandLabel(secret, cryptoHashAlgorithm, "key", TlsUtilities.EmptyBytes, m_keySize).Extract();
		byte[] sourceArray = TlsCryptoUtilities.HkdfExpandLabel(secret, cryptoHashAlgorithm, "iv", TlsUtilities.EmptyBytes, m_fixed_iv_length).Extract();
		cipher.SetKey(key, 0, m_keySize);
		Array.Copy(sourceArray, 0, nonce, 0, m_fixed_iv_length);
	}

	private static int GetNonceMode(bool isTLSv13, int aeadType)
	{
		switch (aeadType)
		{
		case 1:
		case 3:
			if (!isTLSv13)
			{
				return 1;
			}
			return 2;
		case 2:
			return 2;
		default:
			throw new TlsFatalAlert(80);
		}
	}
}
