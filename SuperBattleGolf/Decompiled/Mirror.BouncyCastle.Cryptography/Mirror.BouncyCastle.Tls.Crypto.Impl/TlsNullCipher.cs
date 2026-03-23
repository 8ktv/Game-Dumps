using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl;

public class TlsNullCipher : TlsCipher, TlsCipherExt
{
	protected readonly TlsCryptoParameters m_cryptoParams;

	protected readonly TlsSuiteHmac m_readMac;

	protected readonly TlsSuiteHmac m_writeMac;

	protected readonly byte[] m_decryptConnectionID;

	protected readonly byte[] m_encryptConnectionID;

	protected readonly bool m_decryptUseInnerPlaintext;

	protected readonly bool m_encryptUseInnerPlaintext;

	public virtual bool UsesOpaqueRecordType => false;

	public TlsNullCipher(TlsCryptoParameters cryptoParams, TlsHmac clientMac, TlsHmac serverMac)
	{
		SecurityParameters securityParameters = cryptoParams.SecurityParameters;
		if (TlsImplUtilities.IsTlsV13(securityParameters.NegotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		m_decryptConnectionID = securityParameters.ConnectionIDPeer;
		m_encryptConnectionID = securityParameters.ConnectionIDLocal;
		m_decryptUseInnerPlaintext = !Arrays.IsNullOrEmpty(m_decryptConnectionID);
		m_encryptUseInnerPlaintext = !Arrays.IsNullOrEmpty(m_encryptConnectionID);
		m_cryptoParams = cryptoParams;
		int num = clientMac.MacLength + serverMac.MacLength;
		byte[] key = TlsImplUtilities.CalculateKeyBlock(cryptoParams, num);
		int num2 = 0;
		clientMac.SetKey(key, num2, clientMac.MacLength);
		num2 += clientMac.MacLength;
		serverMac.SetKey(key, num2, serverMac.MacLength);
		num2 += serverMac.MacLength;
		if (num2 != num)
		{
			throw new TlsFatalAlert(80);
		}
		if (cryptoParams.IsServer)
		{
			m_writeMac = new TlsSuiteHmac(cryptoParams, serverMac);
			m_readMac = new TlsSuiteHmac(cryptoParams, clientMac);
		}
		else
		{
			m_writeMac = new TlsSuiteHmac(cryptoParams, clientMac);
			m_readMac = new TlsSuiteHmac(cryptoParams, serverMac);
		}
	}

	public virtual int GetCiphertextDecodeLimit(int plaintextLimit)
	{
		return plaintextLimit + (m_decryptUseInnerPlaintext ? 1 : 0) + m_readMac.Size;
	}

	public virtual int GetCiphertextEncodeLimit(int plaintextLength, int plaintextLimit)
	{
		plaintextLimit = System.Math.Min(plaintextLength, plaintextLimit);
		return plaintextLimit + (m_encryptUseInnerPlaintext ? 1 : 0) + m_writeMac.Size;
	}

	public virtual int GetPlaintextLimit(int ciphertextLimit)
	{
		return GetPlaintextEncodeLimit(ciphertextLimit);
	}

	public virtual int GetPlaintextDecodeLimit(int ciphertextLimit)
	{
		return ciphertextLimit - m_readMac.Size - (m_decryptUseInnerPlaintext ? 1 : 0);
	}

	public virtual int GetPlaintextEncodeLimit(int ciphertextLimit)
	{
		return ciphertextLimit - m_writeMac.Size - (m_encryptUseInnerPlaintext ? 1 : 0);
	}

	public virtual TlsEncodeResult EncodePlaintext(long seqNo, short contentType, ProtocolVersion recordVersion, int headerAllocation, byte[] plaintext, int offset, int len)
	{
		int size = m_writeMac.Size;
		int num = len + (m_encryptUseInnerPlaintext ? 1 : 0);
		byte[] array = new byte[headerAllocation + num + size];
		Array.Copy(plaintext, offset, array, headerAllocation, len);
		short num2 = contentType;
		if (m_encryptUseInnerPlaintext)
		{
			array[headerAllocation + len] = (byte)contentType;
			num2 = 25;
		}
		byte[] array2 = m_writeMac.CalculateMac(seqNo, num2, m_encryptConnectionID, array, headerAllocation, num);
		Array.Copy(array2, 0, array, headerAllocation + num, array2.Length);
		return new TlsEncodeResult(array, 0, array.Length, num2);
	}

	public virtual TlsDecodeResult DecodeCiphertext(long seqNo, short recordType, ProtocolVersion recordVersion, byte[] ciphertext, int offset, int len)
	{
		int size = m_readMac.Size;
		int num = len - size;
		if (num < (m_decryptUseInnerPlaintext ? 1 : 0))
		{
			throw new TlsFatalAlert(50);
		}
		byte[] a = m_readMac.CalculateMac(seqNo, recordType, m_decryptConnectionID, ciphertext, offset, num);
		if (!TlsUtilities.ConstantTimeAreEqual(size, a, 0, ciphertext, offset + num))
		{
			throw new TlsFatalAlert(20);
		}
		short contentType = recordType;
		int num2 = num;
		if (m_decryptUseInnerPlaintext)
		{
			byte b;
			do
			{
				if (--num2 < 0)
				{
					throw new TlsFatalAlert(10);
				}
				b = ciphertext[offset + num2];
			}
			while (b == 0);
			contentType = (short)(b & 0xFF);
		}
		return new TlsDecodeResult(ciphertext, offset, num2, contentType);
	}

	public virtual void RekeyDecoder()
	{
		throw new TlsFatalAlert(80);
	}

	public virtual void RekeyEncoder()
	{
		throw new TlsFatalAlert(80);
	}
}
