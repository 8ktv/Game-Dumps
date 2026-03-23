using System;

namespace Mirror.BouncyCastle.Crypto.Modes;

public class EcbBlockCipher : IBlockCipherMode, IBlockCipher
{
	private readonly IBlockCipher m_cipher;

	public bool IsPartialBlockOkay => false;

	public string AlgorithmName => m_cipher.AlgorithmName + "/ECB";

	public IBlockCipher UnderlyingCipher => m_cipher;

	internal static IBlockCipherMode GetBlockCipherMode(IBlockCipher blockCipher)
	{
		if (blockCipher is IBlockCipherMode result)
		{
			return result;
		}
		return new EcbBlockCipher(blockCipher);
	}

	public EcbBlockCipher(IBlockCipher cipher)
	{
		m_cipher = cipher ?? throw new ArgumentNullException("cipher");
	}

	public int GetBlockSize()
	{
		return m_cipher.GetBlockSize();
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
		m_cipher.Init(forEncryption, parameters);
	}

	public int ProcessBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
	{
		return m_cipher.ProcessBlock(inBuf, inOff, outBuf, outOff);
	}

	public void Reset()
	{
	}
}
