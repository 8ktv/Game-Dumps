using System;
using Mirror.BouncyCastle.Crypto.Modes;

namespace Mirror.BouncyCastle.Crypto;

public class StreamBlockCipher : IStreamCipher
{
	private readonly IBlockCipherMode m_cipherMode;

	private readonly byte[] oneByte = new byte[1];

	public string AlgorithmName => m_cipherMode.AlgorithmName;

	public StreamBlockCipher(IBlockCipherMode cipherMode)
	{
		if (cipherMode == null)
		{
			throw new ArgumentNullException("cipherMode");
		}
		if (cipherMode.GetBlockSize() != 1)
		{
			throw new ArgumentException("block cipher block size != 1.", "cipherMode");
		}
		m_cipherMode = cipherMode;
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
		m_cipherMode.Init(forEncryption, parameters);
	}

	public byte ReturnByte(byte input)
	{
		oneByte[0] = input;
		m_cipherMode.ProcessBlock(oneByte, 0, oneByte, 0);
		return oneByte[0];
	}

	public void ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
	{
		Check.DataLength(input, inOff, length, "input buffer too short");
		Check.OutputLength(output, outOff, length, "output buffer too short");
		for (int i = 0; i != length; i++)
		{
			m_cipherMode.ProcessBlock(input, inOff + i, output, outOff + i);
		}
	}

	public void Reset()
	{
		m_cipherMode.Reset();
	}
}
