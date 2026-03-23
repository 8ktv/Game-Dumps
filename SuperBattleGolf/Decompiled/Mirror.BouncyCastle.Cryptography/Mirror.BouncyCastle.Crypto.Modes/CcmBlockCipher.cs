using System;
using System.IO;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes;

public class CcmBlockCipher : IAeadBlockCipher, IAeadCipher
{
	private static readonly int BlockSize = 16;

	private readonly IBlockCipher cipher;

	private readonly byte[] macBlock;

	private bool forEncryption;

	private byte[] nonce;

	private byte[] initialAssociatedText;

	private int macSize;

	private ICipherParameters keyParam;

	private readonly MemoryStream associatedText = new MemoryStream();

	private readonly MemoryStream data = new MemoryStream();

	public virtual IBlockCipher UnderlyingCipher => cipher;

	public virtual string AlgorithmName => cipher.AlgorithmName + "/CCM";

	public CcmBlockCipher(IBlockCipher cipher)
	{
		this.cipher = cipher;
		macBlock = new byte[BlockSize];
		if (cipher.GetBlockSize() != BlockSize)
		{
			int blockSize = BlockSize;
			throw new ArgumentException("cipher required with a block size of " + blockSize + ".");
		}
	}

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		this.forEncryption = forEncryption;
		ICipherParameters cipherParameters;
		if (parameters is AeadParameters aeadParameters)
		{
			nonce = aeadParameters.GetNonce();
			initialAssociatedText = aeadParameters.GetAssociatedText();
			macSize = GetMacSize(forEncryption, aeadParameters.MacSize);
			cipherParameters = aeadParameters.Key;
		}
		else
		{
			if (!(parameters is ParametersWithIV parametersWithIV))
			{
				throw new ArgumentException("invalid parameters passed to CCM");
			}
			nonce = parametersWithIV.GetIV();
			initialAssociatedText = null;
			macSize = GetMacSize(forEncryption, 64);
			cipherParameters = parametersWithIV.Parameters;
		}
		if (cipherParameters != null)
		{
			keyParam = cipherParameters;
		}
		if (nonce.Length < 7 || nonce.Length > 13)
		{
			throw new ArgumentException("nonce must have length from 7 to 13 octets");
		}
		Reset();
	}

	public virtual int GetBlockSize()
	{
		return cipher.GetBlockSize();
	}

	public virtual void ProcessAadByte(byte input)
	{
		associatedText.WriteByte(input);
	}

	public virtual void ProcessAadBytes(byte[] inBytes, int inOff, int len)
	{
		associatedText.Write(inBytes, inOff, len);
	}

	public virtual int ProcessByte(byte input, byte[] outBytes, int outOff)
	{
		data.WriteByte(input);
		return 0;
	}

	public virtual int ProcessBytes(byte[] inBytes, int inOff, int inLen, byte[] outBytes, int outOff)
	{
		Check.DataLength(inBytes, inOff, inLen, "input buffer too short");
		data.Write(inBytes, inOff, inLen);
		return 0;
	}

	public virtual int DoFinal(byte[] outBytes, int outOff)
	{
		byte[] buffer = data.GetBuffer();
		int inLen = Convert.ToInt32(data.Length);
		int result = ProcessPacket(buffer, 0, inLen, outBytes, outOff);
		Reset();
		return result;
	}

	public virtual void Reset()
	{
		associatedText.SetLength(0L);
		data.SetLength(0L);
	}

	public virtual byte[] GetMac()
	{
		return Arrays.CopyOfRange(macBlock, 0, macSize);
	}

	public virtual int GetUpdateOutputSize(int len)
	{
		return 0;
	}

	public virtual int GetOutputSize(int len)
	{
		int num = Convert.ToInt32(data.Length) + len;
		if (forEncryption)
		{
			return num + macSize;
		}
		if (num >= macSize)
		{
			return num - macSize;
		}
		return 0;
	}

	public virtual byte[] ProcessPacket(byte[] input, int inOff, int inLen)
	{
		Check.DataLength(input, inOff, inLen, "input buffer too short");
		byte[] array;
		if (forEncryption)
		{
			array = new byte[inLen + macSize];
		}
		else
		{
			if (inLen < macSize)
			{
				throw new InvalidCipherTextException("data too short");
			}
			array = new byte[inLen - macSize];
		}
		ProcessPacket(input, inOff, inLen, array, 0);
		return array;
	}

	public virtual int ProcessPacket(byte[] input, int inOff, int inLen, byte[] output, int outOff)
	{
		Check.DataLength(input, inOff, inLen, "input buffer too short");
		if (keyParam == null)
		{
			throw new InvalidOperationException("CCM cipher unitialized.");
		}
		int num = nonce.Length;
		int num2 = 15 - num;
		if (num2 < 4)
		{
			int num3 = 1 << 8 * num2;
			int num4 = 0;
			if (!forEncryption)
			{
				num4 = 16;
			}
			if (inLen - num4 >= num3)
			{
				throw new InvalidOperationException("CCM packet too large for choice of q.");
			}
		}
		byte[] array = new byte[BlockSize];
		array[0] = (byte)((num2 - 1) & 7);
		nonce.CopyTo(array, 1);
		SicBlockCipher sicBlockCipher = new SicBlockCipher(cipher);
		sicBlockCipher.Init(forEncryption, new ParametersWithIV(keyParam, array));
		int i = inOff;
		int num5 = outOff;
		int num6;
		if (forEncryption)
		{
			num6 = inLen + macSize;
			Check.OutputLength(output, outOff, num6, "output buffer too short");
			CalculateMac(input, inOff, inLen, macBlock);
			byte[] array2 = new byte[BlockSize];
			sicBlockCipher.ProcessBlock(macBlock, 0, array2, 0);
			for (; i < inOff + inLen - BlockSize; i += BlockSize)
			{
				sicBlockCipher.ProcessBlock(input, i, output, num5);
				num5 += BlockSize;
			}
			byte[] array3 = new byte[BlockSize];
			Array.Copy(input, i, array3, 0, inLen + inOff - i);
			sicBlockCipher.ProcessBlock(array3, 0, array3, 0);
			Array.Copy(array3, 0, output, num5, inLen + inOff - i);
			Array.Copy(array2, 0, output, outOff + inLen, macSize);
		}
		else
		{
			if (inLen < macSize)
			{
				throw new InvalidCipherTextException("data too short");
			}
			num6 = inLen - macSize;
			Check.OutputLength(output, outOff, num6, "output buffer too short");
			Array.Copy(input, inOff + num6, macBlock, 0, macSize);
			sicBlockCipher.ProcessBlock(macBlock, 0, macBlock, 0);
			for (int j = macSize; j != macBlock.Length; j++)
			{
				macBlock[j] = 0;
			}
			for (; i < inOff + num6 - BlockSize; i += BlockSize)
			{
				sicBlockCipher.ProcessBlock(input, i, output, num5);
				num5 += BlockSize;
			}
			byte[] array4 = new byte[BlockSize];
			Array.Copy(input, i, array4, 0, num6 - (i - inOff));
			sicBlockCipher.ProcessBlock(array4, 0, array4, 0);
			Array.Copy(array4, 0, output, num5, num6 - (i - inOff));
			byte[] b = new byte[BlockSize];
			CalculateMac(output, outOff, num6, b);
			if (!Arrays.FixedTimeEquals(macBlock, b))
			{
				throw new InvalidCipherTextException("mac check in CCM failed");
			}
		}
		return num6;
	}

	private int CalculateMac(byte[] data, int dataOff, int dataLen, byte[] macBlock)
	{
		CbcBlockCipherMac cbcBlockCipherMac = new CbcBlockCipherMac(cipher, macSize * 8);
		cbcBlockCipherMac.Init(keyParam);
		byte[] array = new byte[16];
		if (HasAssociatedText())
		{
			array[0] |= 64;
		}
		array[0] |= (byte)((((cbcBlockCipherMac.GetMacSize() - 2) / 2) & 7) << 3);
		array[0] |= (byte)((15 - nonce.Length - 1) & 7);
		Array.Copy(nonce, 0, array, 1, nonce.Length);
		int num = dataLen;
		int num2 = 1;
		while (num > 0)
		{
			array[^num2] = (byte)(num & 0xFF);
			num >>= 8;
			num2++;
		}
		cbcBlockCipherMac.BlockUpdate(array, 0, array.Length);
		if (HasAssociatedText())
		{
			int associatedTextLength = GetAssociatedTextLength();
			int num3;
			if (associatedTextLength < 65280)
			{
				cbcBlockCipherMac.Update((byte)(associatedTextLength >> 8));
				cbcBlockCipherMac.Update((byte)associatedTextLength);
				num3 = 2;
			}
			else
			{
				cbcBlockCipherMac.Update(byte.MaxValue);
				cbcBlockCipherMac.Update(254);
				cbcBlockCipherMac.Update((byte)(associatedTextLength >> 24));
				cbcBlockCipherMac.Update((byte)(associatedTextLength >> 16));
				cbcBlockCipherMac.Update((byte)(associatedTextLength >> 8));
				cbcBlockCipherMac.Update((byte)associatedTextLength);
				num3 = 6;
			}
			if (initialAssociatedText != null)
			{
				cbcBlockCipherMac.BlockUpdate(initialAssociatedText, 0, initialAssociatedText.Length);
			}
			if (associatedText.Length > 0)
			{
				byte[] buffer = associatedText.GetBuffer();
				int len = Convert.ToInt32(associatedText.Length);
				cbcBlockCipherMac.BlockUpdate(buffer, 0, len);
			}
			num3 = (num3 + associatedTextLength) % 16;
			if (num3 != 0)
			{
				for (int i = num3; i < 16; i++)
				{
					cbcBlockCipherMac.Update(0);
				}
			}
		}
		cbcBlockCipherMac.BlockUpdate(data, dataOff, dataLen);
		return cbcBlockCipherMac.DoFinal(macBlock, 0);
	}

	private int GetMacSize(bool forEncryption, int requestedMacBits)
	{
		if (forEncryption && (requestedMacBits < 32 || requestedMacBits > 128 || (requestedMacBits & 0xF) != 0))
		{
			throw new ArgumentException("tag length in octets must be one of {4,6,8,10,12,14,16}");
		}
		return requestedMacBits >> 3;
	}

	private int GetAssociatedTextLength()
	{
		return Convert.ToInt32(associatedText.Length) + ((initialAssociatedText != null) ? initialAssociatedText.Length : 0);
	}

	private bool HasAssociatedText()
	{
		return GetAssociatedTextLength() > 0;
	}
}
