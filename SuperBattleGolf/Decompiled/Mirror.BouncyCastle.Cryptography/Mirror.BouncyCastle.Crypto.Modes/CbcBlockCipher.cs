using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes;

public sealed class CbcBlockCipher : IBlockCipherMode, IBlockCipher
{
	private byte[] IV;

	private byte[] cbcV;

	private byte[] cbcNextV;

	private int blockSize;

	private IBlockCipher cipher;

	private bool encrypting;

	public IBlockCipher UnderlyingCipher => cipher;

	public string AlgorithmName => cipher.AlgorithmName + "/CBC";

	public bool IsPartialBlockOkay => false;

	public CbcBlockCipher(IBlockCipher cipher)
	{
		this.cipher = cipher;
		blockSize = cipher.GetBlockSize();
		IV = new byte[blockSize];
		cbcV = new byte[blockSize];
		cbcNextV = new byte[blockSize];
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
		bool flag = encrypting;
		encrypting = forEncryption;
		if (parameters is ParametersWithIV parametersWithIV)
		{
			if (parametersWithIV.IVLength != blockSize)
			{
				throw new ArgumentException("initialisation vector must be the same length as block size");
			}
			parametersWithIV.CopyIVTo(IV, 0, blockSize);
			parameters = parametersWithIV.Parameters;
		}
		else
		{
			Arrays.Fill(IV, 0);
		}
		Reset();
		if (parameters != null)
		{
			cipher.Init(encrypting, parameters);
		}
		else if (flag != encrypting)
		{
			throw new ArgumentException("cannot change encrypting state without providing key.");
		}
	}

	public int GetBlockSize()
	{
		return cipher.GetBlockSize();
	}

	public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
	{
		if (!encrypting)
		{
			return DecryptBlock(input, inOff, output, outOff);
		}
		return EncryptBlock(input, inOff, output, outOff);
	}

	public void Reset()
	{
		Array.Copy(IV, 0, cbcV, 0, IV.Length);
		Array.Clear(cbcNextV, 0, cbcNextV.Length);
	}

	private int EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
	{
		Check.DataLength(input, inOff, blockSize, "input buffer too short");
		Check.OutputLength(outBytes, outOff, blockSize, "output buffer too short");
		for (int i = 0; i < blockSize; i++)
		{
			cbcV[i] ^= input[inOff + i];
		}
		int result = cipher.ProcessBlock(cbcV, 0, outBytes, outOff);
		Array.Copy(outBytes, outOff, cbcV, 0, cbcV.Length);
		return result;
	}

	private int DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
	{
		Check.DataLength(input, inOff, blockSize, "input buffer too short");
		Check.OutputLength(outBytes, outOff, blockSize, "output buffer too short");
		Array.Copy(input, inOff, cbcNextV, 0, blockSize);
		int result = cipher.ProcessBlock(input, inOff, outBytes, outOff);
		for (int i = 0; i < blockSize; i++)
		{
			outBytes[outOff + i] ^= cbcV[i];
		}
		byte[] array = cbcV;
		cbcV = cbcNextV;
		cbcNextV = array;
		return result;
	}
}
