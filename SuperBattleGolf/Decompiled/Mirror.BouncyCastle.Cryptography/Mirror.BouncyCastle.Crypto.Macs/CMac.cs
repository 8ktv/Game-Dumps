using System;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Paddings;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Crypto.Macs;

public class CMac : IMac
{
	private const byte CONSTANT_128 = 135;

	private const byte CONSTANT_64 = 27;

	private byte[] ZEROES;

	private byte[] mac;

	private byte[] buf;

	private int bufOff;

	private IBlockCipherMode m_cipherMode;

	private int macSize;

	private byte[] L;

	private byte[] Lu;

	private byte[] Lu2;

	public string AlgorithmName => m_cipherMode.AlgorithmName;

	public CMac(IBlockCipher cipher)
		: this(cipher, cipher.GetBlockSize() * 8)
	{
	}

	public CMac(IBlockCipher cipher, int macSizeInBits)
	{
		if (macSizeInBits % 8 != 0)
		{
			throw new ArgumentException("MAC size must be multiple of 8");
		}
		if (macSizeInBits > cipher.GetBlockSize() * 8)
		{
			throw new ArgumentException("MAC size must be less or equal to " + cipher.GetBlockSize() * 8);
		}
		if (cipher.GetBlockSize() != 8 && cipher.GetBlockSize() != 16)
		{
			throw new ArgumentException("Block size must be either 64 or 128 bits");
		}
		m_cipherMode = new CbcBlockCipher(cipher);
		macSize = macSizeInBits / 8;
		mac = new byte[cipher.GetBlockSize()];
		buf = new byte[cipher.GetBlockSize()];
		ZEROES = new byte[cipher.GetBlockSize()];
		bufOff = 0;
	}

	private static int ShiftLeft(byte[] block, byte[] output)
	{
		int num = block.Length;
		uint num2 = 0u;
		while (--num >= 0)
		{
			uint num3 = block[num];
			output[num] = (byte)((num3 << 1) | num2);
			num2 = (num3 >> 7) & 1;
		}
		return (int)num2;
	}

	private static byte[] DoubleLu(byte[] input)
	{
		byte[] array = new byte[input.Length];
		int num = ShiftLeft(input, array);
		int num2 = ((input.Length == 16) ? 135 : 27);
		array[input.Length - 1] ^= (byte)(num2 >> (1 - num << 3));
		return array;
	}

	public void Init(ICipherParameters parameters)
	{
		if (parameters is KeyParameter)
		{
			m_cipherMode.Init(forEncryption: true, parameters);
			L = new byte[ZEROES.Length];
			m_cipherMode.ProcessBlock(ZEROES, 0, L, 0);
			Lu = DoubleLu(L);
			Lu2 = DoubleLu(Lu);
		}
		else if (parameters != null)
		{
			throw new ArgumentException("CMac mode only permits key to be set.", "parameters");
		}
		Reset();
	}

	public int GetMacSize()
	{
		return macSize;
	}

	public void Update(byte input)
	{
		if (bufOff == buf.Length)
		{
			m_cipherMode.ProcessBlock(buf, 0, mac, 0);
			bufOff = 0;
		}
		buf[bufOff++] = input;
	}

	public void BlockUpdate(byte[] inBytes, int inOff, int len)
	{
		if (len < 0)
		{
			throw new ArgumentException("Can't have a negative input length!");
		}
		int blockSize = m_cipherMode.GetBlockSize();
		int num = blockSize - bufOff;
		if (len > num)
		{
			Array.Copy(inBytes, inOff, buf, bufOff, num);
			m_cipherMode.ProcessBlock(buf, 0, mac, 0);
			bufOff = 0;
			len -= num;
			inOff += num;
			while (len > blockSize)
			{
				m_cipherMode.ProcessBlock(inBytes, inOff, mac, 0);
				len -= blockSize;
				inOff += blockSize;
			}
		}
		Array.Copy(inBytes, inOff, buf, bufOff, len);
		bufOff += len;
	}

	public int DoFinal(byte[] outBytes, int outOff)
	{
		int blockSize = m_cipherMode.GetBlockSize();
		byte[] array;
		if (bufOff == blockSize)
		{
			array = Lu;
		}
		else
		{
			new ISO7816d4Padding().AddPadding(buf, bufOff);
			array = Lu2;
		}
		for (int i = 0; i < mac.Length; i++)
		{
			buf[i] ^= array[i];
		}
		m_cipherMode.ProcessBlock(buf, 0, mac, 0);
		Array.Copy(mac, 0, outBytes, outOff, macSize);
		Reset();
		return macSize;
	}

	public void Reset()
	{
		Array.Clear(buf, 0, buf.Length);
		bufOff = 0;
		m_cipherMode.Reset();
	}
}
