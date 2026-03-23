using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl;

public class TlsBlockCipher : TlsCipher, TlsCipherExt
{
	protected readonly TlsCryptoParameters m_cryptoParams;

	protected readonly byte[] m_randomData;

	protected readonly bool m_encryptThenMac;

	protected readonly bool m_useExplicitIV;

	protected readonly bool m_acceptExtraPadding;

	protected readonly bool m_useExtraPadding;

	protected readonly TlsBlockCipherImpl m_decryptCipher;

	protected readonly TlsBlockCipherImpl m_encryptCipher;

	protected readonly TlsSuiteHmac m_readMac;

	protected readonly TlsSuiteHmac m_writeMac;

	protected readonly byte[] m_decryptConnectionID;

	protected readonly byte[] m_encryptConnectionID;

	protected readonly bool m_decryptUseInnerPlaintext;

	protected readonly bool m_encryptUseInnerPlaintext;

	public virtual bool UsesOpaqueRecordType => false;

	public TlsBlockCipher(TlsCryptoParameters cryptoParams, TlsBlockCipherImpl encryptCipher, TlsBlockCipherImpl decryptCipher, TlsHmac clientMac, TlsHmac serverMac, int cipherKeySize)
	{
		SecurityParameters securityParameters = cryptoParams.SecurityParameters;
		ProtocolVersion negotiatedVersion = securityParameters.NegotiatedVersion;
		if (TlsImplUtilities.IsTlsV13(negotiatedVersion))
		{
			throw new TlsFatalAlert(80);
		}
		m_decryptConnectionID = securityParameters.ConnectionIDPeer;
		m_encryptConnectionID = securityParameters.ConnectionIDLocal;
		m_decryptUseInnerPlaintext = !Arrays.IsNullOrEmpty(m_decryptConnectionID);
		m_encryptUseInnerPlaintext = !Arrays.IsNullOrEmpty(m_encryptConnectionID);
		m_cryptoParams = cryptoParams;
		m_randomData = cryptoParams.NonceGenerator.GenerateNonce(256);
		m_encryptThenMac = securityParameters.IsEncryptThenMac;
		m_useExplicitIV = TlsImplUtilities.IsTlsV11(negotiatedVersion);
		m_acceptExtraPadding = !negotiatedVersion.IsSsl;
		m_useExtraPadding = securityParameters.IsExtendedPadding && ProtocolVersion.TLSv10.IsEqualOrEarlierVersionOf(negotiatedVersion) && (m_encryptThenMac || !securityParameters.IsTruncatedHmac);
		m_encryptCipher = encryptCipher;
		m_decryptCipher = decryptCipher;
		TlsBlockCipherImpl tlsBlockCipherImpl;
		TlsBlockCipherImpl tlsBlockCipherImpl2;
		if (cryptoParams.IsServer)
		{
			tlsBlockCipherImpl = decryptCipher;
			tlsBlockCipherImpl2 = encryptCipher;
		}
		else
		{
			tlsBlockCipherImpl = encryptCipher;
			tlsBlockCipherImpl2 = decryptCipher;
		}
		int num = 2 * cipherKeySize + clientMac.MacLength + serverMac.MacLength;
		if (!m_useExplicitIV)
		{
			num += tlsBlockCipherImpl.GetBlockSize() + tlsBlockCipherImpl2.GetBlockSize();
		}
		byte[] array = TlsImplUtilities.CalculateKeyBlock(cryptoParams, num);
		int num2 = 0;
		clientMac.SetKey(array, num2, clientMac.MacLength);
		num2 += clientMac.MacLength;
		serverMac.SetKey(array, num2, serverMac.MacLength);
		num2 += serverMac.MacLength;
		tlsBlockCipherImpl.SetKey(array, num2, cipherKeySize);
		num2 += cipherKeySize;
		tlsBlockCipherImpl2.SetKey(array, num2, cipherKeySize);
		num2 += cipherKeySize;
		int blockSize = tlsBlockCipherImpl.GetBlockSize();
		int blockSize2 = tlsBlockCipherImpl2.GetBlockSize();
		if (m_useExplicitIV)
		{
			tlsBlockCipherImpl.Init(new byte[blockSize], 0, blockSize);
			tlsBlockCipherImpl2.Init(new byte[blockSize2], 0, blockSize2);
		}
		else
		{
			tlsBlockCipherImpl.Init(array, num2, blockSize);
			num2 += blockSize;
			tlsBlockCipherImpl2.Init(array, num2, blockSize2);
			num2 += blockSize2;
		}
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
		int blockSize = m_decryptCipher.GetBlockSize();
		int size = m_readMac.Size;
		int maxPadding = 256;
		int plaintextLength = plaintextLimit + (m_decryptUseInnerPlaintext ? 1 : 0);
		return GetCiphertextLength(blockSize, size, maxPadding, plaintextLength);
	}

	public virtual int GetCiphertextEncodeLimit(int plaintextLength, int plaintextLimit)
	{
		plaintextLimit = System.Math.Min(plaintextLength, plaintextLimit);
		int blockSize = m_encryptCipher.GetBlockSize();
		int size = m_writeMac.Size;
		int maxPadding = (m_useExtraPadding ? 256 : blockSize);
		int plaintextLength2 = plaintextLimit + (m_encryptUseInnerPlaintext ? 1 : 0);
		return GetCiphertextLength(blockSize, size, maxPadding, plaintextLength2);
	}

	public virtual int GetPlaintextLimit(int ciphertextLimit)
	{
		return GetPlaintextEncodeLimit(ciphertextLimit);
	}

	public virtual int GetPlaintextDecodeLimit(int ciphertextLimit)
	{
		int blockSize = m_decryptCipher.GetBlockSize();
		int size = m_readMac.Size;
		return GetPlaintextLength(blockSize, size, ciphertextLimit) - (m_decryptUseInnerPlaintext ? 1 : 0);
	}

	public virtual int GetPlaintextEncodeLimit(int ciphertextLimit)
	{
		int blockSize = m_encryptCipher.GetBlockSize();
		int size = m_writeMac.Size;
		return GetPlaintextLength(blockSize, size, ciphertextLimit) - (m_encryptUseInnerPlaintext ? 1 : 0);
	}

	public virtual TlsEncodeResult EncodePlaintext(long seqNo, short contentType, ProtocolVersion recordVersion, int headerAllocation, byte[] plaintext, int offset, int len)
	{
		int blockSize = m_encryptCipher.GetBlockSize();
		int size = m_writeMac.Size;
		int num = len + (m_encryptUseInnerPlaintext ? 1 : 0);
		int num2 = num;
		if (!m_encryptThenMac)
		{
			num2 += size;
		}
		int num3 = blockSize - num2 % blockSize;
		if (m_useExtraPadding)
		{
			int max = (256 - num3) / blockSize;
			int num4 = ChooseExtraPadBlocks(max);
			num3 += num4 * blockSize;
		}
		int num5 = num + size + num3;
		if (m_useExplicitIV)
		{
			num5 += blockSize;
		}
		byte[] array = new byte[headerAllocation + num5];
		int num6 = headerAllocation;
		if (m_useExplicitIV)
		{
			Array.Copy(m_cryptoParams.NonceGenerator.GenerateNonce(blockSize), 0, array, num6, blockSize);
			num6 += blockSize;
		}
		int msgOff = num6;
		Array.Copy(plaintext, offset, array, num6, len);
		num6 += len;
		short num7 = contentType;
		if (m_encryptUseInnerPlaintext)
		{
			array[num6++] = (byte)contentType;
			num7 = 25;
		}
		if (!m_encryptThenMac)
		{
			byte[] array2 = m_writeMac.CalculateMac(seqNo, num7, m_encryptConnectionID, array, msgOff, num);
			Array.Copy(array2, 0, array, num6, array2.Length);
			num6 += array2.Length;
		}
		byte b = (byte)(num3 - 1);
		for (int i = 0; i < num3; i++)
		{
			array[num6++] = b;
		}
		m_encryptCipher.DoFinal(array, headerAllocation, num6 - headerAllocation, array, headerAllocation);
		if (m_encryptThenMac)
		{
			byte[] array3 = m_writeMac.CalculateMac(seqNo, num7, m_encryptConnectionID, array, headerAllocation, num6 - headerAllocation);
			Array.Copy(array3, 0, array, num6, array3.Length);
			num6 += array3.Length;
		}
		if (num6 != array.Length)
		{
			throw new TlsFatalAlert(80);
		}
		return new TlsEncodeResult(array, 0, array.Length, num7);
	}

	public virtual TlsDecodeResult DecodeCiphertext(long seqNo, short recordType, ProtocolVersion recordVersion, byte[] ciphertext, int offset, int len)
	{
		int blockSize = m_decryptCipher.GetBlockSize();
		int size = m_readMac.Size;
		int num = blockSize;
		num = ((!m_encryptThenMac) ? System.Math.Max(num, size + 1) : (num + size));
		if (m_useExplicitIV)
		{
			num += blockSize;
		}
		if (len < num)
		{
			throw new TlsFatalAlert(50);
		}
		int num2 = len;
		if (m_encryptThenMac)
		{
			num2 -= size;
		}
		if (num2 % blockSize != 0)
		{
			throw new TlsFatalAlert(21);
		}
		if (m_encryptThenMac)
		{
			byte[] a = m_readMac.CalculateMac(seqNo, recordType, m_decryptConnectionID, ciphertext, offset, len - size);
			if (!TlsUtilities.ConstantTimeAreEqual(size, a, 0, ciphertext, offset + len - size))
			{
				throw new TlsFatalAlert(20);
			}
		}
		m_decryptCipher.DoFinal(ciphertext, offset, num2, ciphertext, offset);
		if (m_useExplicitIV)
		{
			offset += blockSize;
			num2 -= blockSize;
		}
		int num3 = CheckPaddingConstantTime(ciphertext, offset, num2, blockSize, (!m_encryptThenMac) ? size : 0);
		bool flag = num3 == 0;
		int num4 = num2 - num3;
		if (!m_encryptThenMac)
		{
			num4 -= size;
			byte[] a2 = m_readMac.CalculateMacConstantTime(seqNo, recordType, m_decryptConnectionID, ciphertext, offset, num4, num2 - size, m_randomData);
			flag |= !TlsUtilities.ConstantTimeAreEqual(size, a2, 0, ciphertext, offset + num4);
		}
		if (flag)
		{
			throw new TlsFatalAlert(20);
		}
		short contentType = recordType;
		int num5 = num4;
		if (m_decryptUseInnerPlaintext)
		{
			byte b;
			do
			{
				if (--num5 < 0)
				{
					throw new TlsFatalAlert(10);
				}
				b = ciphertext[offset + num5];
			}
			while (b == 0);
			contentType = (short)(b & 0xFF);
		}
		return new TlsDecodeResult(ciphertext, offset, num5, contentType);
	}

	public virtual void RekeyDecoder()
	{
		throw new TlsFatalAlert(80);
	}

	public virtual void RekeyEncoder()
	{
		throw new TlsFatalAlert(80);
	}

	protected virtual int CheckPaddingConstantTime(byte[] buf, int off, int len, int blockSize, int macSize)
	{
		int num = off + len;
		byte b = buf[num - 1];
		int num2 = (b & 0xFF) + 1;
		int num3 = 0;
		byte b2 = 0;
		int num4 = System.Math.Min(m_acceptExtraPadding ? 256 : blockSize, len - macSize);
		if (num2 > num4)
		{
			num2 = 0;
		}
		else
		{
			int num5 = num - num2;
			do
			{
				b2 |= (byte)(buf[num5++] ^ b);
			}
			while (num5 < num);
			num3 = num2;
			if (b2 != 0)
			{
				num2 = 0;
			}
		}
		byte[] randomData = m_randomData;
		while (num3 < 256)
		{
			b2 |= (byte)(randomData[num3++] ^ b);
		}
		randomData[0] ^= b2;
		return num2;
	}

	protected virtual int ChooseExtraPadBlocks(int max)
	{
		return System.Math.Min(Integers.NumberOfTrailingZeros((int)Pack.LE_To_UInt32(m_cryptoParams.NonceGenerator.GenerateNonce(4), 0)), max);
	}

	protected virtual int GetCiphertextLength(int blockSize, int macSize, int maxPadding, int plaintextLength)
	{
		int num = plaintextLength;
		if (m_useExplicitIV)
		{
			num += blockSize;
		}
		num += maxPadding;
		if (m_encryptThenMac)
		{
			num -= num % blockSize;
			return num + macSize;
		}
		num += macSize;
		return num - num % blockSize;
	}

	protected virtual int GetPlaintextLength(int blockSize, int macSize, int ciphertextLength)
	{
		int num = ciphertextLength;
		if (m_encryptThenMac)
		{
			num -= macSize;
			num -= num % blockSize;
		}
		else
		{
			num -= num % blockSize;
			num -= macSize;
		}
		num--;
		if (m_useExplicitIV)
		{
			num -= blockSize;
		}
		return num;
	}
}
