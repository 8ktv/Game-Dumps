using System;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Crypto.Modes;

public class KCtrBlockCipher : IStreamCipher, IBlockCipherMode, IBlockCipher
{
	private byte[] IV;

	private byte[] ofbV;

	private byte[] ofbOutV;

	private bool initialised;

	private int byteCount;

	private readonly int blockSize;

	private readonly IBlockCipher cipher;

	public IBlockCipher UnderlyingCipher => cipher;

	public string AlgorithmName => cipher.AlgorithmName + "/KCTR";

	public bool IsPartialBlockOkay => true;

	public KCtrBlockCipher(IBlockCipher cipher)
	{
		this.cipher = cipher;
		IV = new byte[cipher.GetBlockSize()];
		blockSize = cipher.GetBlockSize();
		ofbV = new byte[cipher.GetBlockSize()];
		ofbOutV = new byte[cipher.GetBlockSize()];
	}

	public void Init(bool forEncryption, ICipherParameters parameters)
	{
		initialised = true;
		if (parameters is ParametersWithIV)
		{
			ParametersWithIV obj = (ParametersWithIV)parameters;
			byte[] iV = obj.GetIV();
			int destinationIndex = IV.Length - iV.Length;
			Array.Clear(IV, 0, IV.Length);
			Array.Copy(iV, 0, IV, destinationIndex, iV.Length);
			parameters = obj.Parameters;
			if (parameters != null)
			{
				cipher.Init(forEncryption: true, parameters);
			}
			Reset();
			return;
		}
		throw new ArgumentException("Invalid parameter passed");
	}

	public int GetBlockSize()
	{
		return cipher.GetBlockSize();
	}

	public byte ReturnByte(byte input)
	{
		return CalculateByte(input);
	}

	public void ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
	{
		Check.DataLength(input, inOff, len, "input buffer too short");
		Check.OutputLength(output, outOff, len, "output buffer too short");
		int num = inOff;
		int num2 = inOff + len;
		int num3 = outOff;
		while (num < num2)
		{
			output[num3++] = CalculateByte(input[num++]);
		}
	}

	protected byte CalculateByte(byte b)
	{
		if (byteCount == 0)
		{
			incrementCounterAt(0);
			checkCounter();
			cipher.ProcessBlock(ofbV, 0, ofbOutV, 0);
			return (byte)(ofbOutV[byteCount++] ^ b);
		}
		byte result = (byte)(ofbOutV[byteCount++] ^ b);
		if (byteCount == ofbV.Length)
		{
			byteCount = 0;
		}
		return result;
	}

	public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
	{
		int num = GetBlockSize();
		Check.DataLength(input, inOff, num, "input buffer too short");
		Check.OutputLength(output, outOff, num, "output buffer too short");
		ProcessBytes(input, inOff, num, output, outOff);
		return num;
	}

	public void Reset()
	{
		if (initialised)
		{
			cipher.ProcessBlock(IV, 0, ofbV, 0);
		}
		byteCount = 0;
	}

	private void incrementCounterAt(int pos)
	{
		int num = pos;
		while (num < ofbV.Length && ++ofbV[num++] == 0)
		{
		}
	}

	private void checkCounter()
	{
	}
}
