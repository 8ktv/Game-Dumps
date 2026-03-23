using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes;

public class SicBlockCipher : IBlockCipherMode, IBlockCipher
{
	private readonly IBlockCipher cipher;

	private readonly int blockSize;

	private readonly byte[] counter;

	private readonly byte[] counterOut;

	private byte[] IV;

	public IBlockCipher UnderlyingCipher => cipher;

	public virtual string AlgorithmName => cipher.AlgorithmName + "/SIC";

	public virtual bool IsPartialBlockOkay => true;

	public SicBlockCipher(IBlockCipher cipher)
	{
		this.cipher = cipher;
		blockSize = cipher.GetBlockSize();
		counter = new byte[blockSize];
		counterOut = new byte[blockSize];
		IV = new byte[blockSize];
	}

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (!(parameters is ParametersWithIV parametersWithIV))
		{
			throw new ArgumentException("CTR/SIC mode requires ParametersWithIV", "parameters");
		}
		IV = Arrays.Clone(parametersWithIV.GetIV());
		if (blockSize < IV.Length)
		{
			int num = blockSize;
			throw new ArgumentException("CTR/SIC mode requires IV no greater than: " + num + " bytes.");
		}
		int num2 = System.Math.Min(8, blockSize / 2);
		if (blockSize - IV.Length > num2)
		{
			throw new ArgumentException("CTR/SIC mode requires IV of at least: " + (blockSize - num2) + " bytes.");
		}
		Reset();
		if (parametersWithIV.Parameters != null)
		{
			cipher.Init(forEncryption: true, parametersWithIV.Parameters);
		}
	}

	public virtual int GetBlockSize()
	{
		return cipher.GetBlockSize();
	}

	public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
	{
		cipher.ProcessBlock(counter, 0, counterOut, 0);
		for (int i = 0; i < counterOut.Length; i++)
		{
			output[outOff + i] = (byte)(counterOut[i] ^ input[inOff + i]);
		}
		int num = counter.Length;
		while (--num >= 0 && ++counter[num] == 0)
		{
		}
		return counter.Length;
	}

	public virtual void Reset()
	{
		Arrays.Fill(counter, 0);
		Array.Copy(IV, 0, counter, 0, IV.Length);
	}
}
