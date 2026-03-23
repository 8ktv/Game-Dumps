using System;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Paddings;

public class PaddedBufferedBlockCipher : BufferedBlockCipher
{
	private readonly IBlockCipherPadding padding;

	public PaddedBufferedBlockCipher(IBlockCipher cipher, IBlockCipherPadding padding)
		: this(EcbBlockCipher.GetBlockCipherMode(cipher), padding)
	{
	}

	public PaddedBufferedBlockCipher(IBlockCipherMode cipherMode, IBlockCipherPadding padding)
	{
		m_cipherMode = cipherMode;
		this.padding = padding;
		buf = new byte[m_cipherMode.GetBlockSize()];
		bufOff = 0;
	}

	public PaddedBufferedBlockCipher(IBlockCipherMode cipherMode)
		: this(cipherMode, new Pkcs7Padding())
	{
	}

	public override void Init(bool forEncryption, ICipherParameters parameters)
	{
		base.forEncryption = forEncryption;
		SecureRandom random = null;
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			random = parametersWithRandom.Random;
			parameters = parametersWithRandom.Parameters;
		}
		Reset();
		padding.Init(random);
		m_cipherMode.Init(forEncryption, parameters);
	}

	public override int GetOutputSize(int length)
	{
		int num = length + bufOff;
		int num2 = num % buf.Length;
		if (num2 == 0)
		{
			if (forEncryption)
			{
				return num + buf.Length;
			}
			return num;
		}
		return num - num2 + buf.Length;
	}

	public override int GetUpdateOutputSize(int length)
	{
		int num = length + bufOff;
		int num2 = num % buf.Length;
		if (num2 == 0)
		{
			return num - buf.Length;
		}
		return num - num2;
	}

	public override int ProcessByte(byte input, byte[] output, int outOff)
	{
		int result = 0;
		if (bufOff == buf.Length)
		{
			result = m_cipherMode.ProcessBlock(buf, 0, output, outOff);
			bufOff = 0;
		}
		buf[bufOff++] = input;
		return result;
	}

	public override int ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
	{
		if (length < 0)
		{
			throw new ArgumentException("Can't have a negative input length!");
		}
		int blockSize = GetBlockSize();
		int updateOutputSize = GetUpdateOutputSize(length);
		if (updateOutputSize > 0)
		{
			Check.OutputLength(output, outOff, updateOutputSize, "output buffer too short");
		}
		int num = 0;
		int num2 = buf.Length - bufOff;
		if (length > num2)
		{
			Array.Copy(input, inOff, buf, bufOff, num2);
			num = m_cipherMode.ProcessBlock(buf, 0, output, outOff);
			bufOff = 0;
			length -= num2;
			inOff += num2;
			while (length > buf.Length)
			{
				num += m_cipherMode.ProcessBlock(input, inOff, output, outOff + num);
				length -= blockSize;
				inOff += blockSize;
			}
		}
		Array.Copy(input, inOff, buf, bufOff, length);
		bufOff += length;
		return num;
	}

	public override int DoFinal(byte[] output, int outOff)
	{
		int blockSize = m_cipherMode.GetBlockSize();
		int num = 0;
		if (forEncryption)
		{
			if (bufOff == blockSize)
			{
				if (outOff + 2 * blockSize > output.Length)
				{
					Reset();
					throw new OutputLengthException("output buffer too short");
				}
				num = m_cipherMode.ProcessBlock(buf, 0, output, outOff);
				bufOff = 0;
			}
			padding.AddPadding(buf, bufOff);
			num += m_cipherMode.ProcessBlock(buf, 0, output, outOff + num);
			Reset();
		}
		else
		{
			if (bufOff != blockSize)
			{
				Reset();
				throw new DataLengthException("last block incomplete in decryption");
			}
			num = m_cipherMode.ProcessBlock(buf, 0, buf, 0);
			bufOff = 0;
			try
			{
				num -= padding.PadCount(buf);
				Array.Copy(buf, 0, output, outOff, num);
			}
			finally
			{
				Reset();
			}
		}
		return num;
	}
}
