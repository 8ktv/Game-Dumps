using System;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Paddings;

namespace Mirror.BouncyCastle.Crypto.Macs;

public class CbcBlockCipherMac : IMac
{
	private byte[] buf;

	private int bufOff;

	private IBlockCipherMode m_cipherMode;

	private IBlockCipherPadding padding;

	private int macSize;

	public string AlgorithmName => m_cipherMode.AlgorithmName;

	public CbcBlockCipherMac(IBlockCipher cipher)
		: this(cipher, cipher.GetBlockSize() * 8 / 2, null)
	{
	}

	public CbcBlockCipherMac(IBlockCipher cipher, IBlockCipherPadding padding)
		: this(cipher, cipher.GetBlockSize() * 8 / 2, padding)
	{
	}

	public CbcBlockCipherMac(IBlockCipher cipher, int macSizeInBits)
		: this(cipher, macSizeInBits, null)
	{
	}

	public CbcBlockCipherMac(IBlockCipher cipher, int macSizeInBits, IBlockCipherPadding padding)
	{
		if (macSizeInBits % 8 != 0)
		{
			throw new ArgumentException("MAC size must be multiple of 8");
		}
		m_cipherMode = new CbcBlockCipher(cipher);
		this.padding = padding;
		macSize = macSizeInBits / 8;
		buf = new byte[cipher.GetBlockSize()];
		bufOff = 0;
	}

	public void Init(ICipherParameters parameters)
	{
		Reset();
		m_cipherMode.Init(forEncryption: true, parameters);
	}

	public int GetMacSize()
	{
		return macSize;
	}

	public void Update(byte input)
	{
		if (bufOff == buf.Length)
		{
			m_cipherMode.ProcessBlock(buf, 0, buf, 0);
			bufOff = 0;
		}
		buf[bufOff++] = input;
	}

	public void BlockUpdate(byte[] input, int inOff, int len)
	{
		if (len < 0)
		{
			throw new ArgumentException("Can't have a negative input length!");
		}
		int blockSize = m_cipherMode.GetBlockSize();
		int num = blockSize - bufOff;
		if (len > num)
		{
			Array.Copy(input, inOff, buf, bufOff, num);
			m_cipherMode.ProcessBlock(buf, 0, buf, 0);
			bufOff = 0;
			len -= num;
			inOff += num;
			while (len > blockSize)
			{
				m_cipherMode.ProcessBlock(input, inOff, buf, 0);
				len -= blockSize;
				inOff += blockSize;
			}
		}
		Array.Copy(input, inOff, buf, bufOff, len);
		bufOff += len;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		int blockSize = m_cipherMode.GetBlockSize();
		if (padding == null)
		{
			while (bufOff < blockSize)
			{
				buf[bufOff++] = 0;
			}
		}
		else
		{
			if (bufOff == blockSize)
			{
				m_cipherMode.ProcessBlock(buf, 0, buf, 0);
				bufOff = 0;
			}
			padding.AddPadding(buf, bufOff);
		}
		m_cipherMode.ProcessBlock(buf, 0, buf, 0);
		Array.Copy(buf, 0, output, outOff, macSize);
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
