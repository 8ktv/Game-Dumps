using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal sealed class HarakaSXof : HarakaSBase
{
	public string AlgorithmName => "Haraka-S";

	public HarakaSXof(byte[] pkSeed)
	{
		byte[] array = new byte[640];
		BlockUpdate(pkSeed, 0, pkSeed.Length);
		OutputFinal(array, 0, array.Length);
		haraka512_rc = new ulong[10][];
		haraka256_rc = new uint[10][];
		for (int i = 0; i < 10; i++)
		{
			haraka512_rc[i] = new ulong[8];
			haraka256_rc[i] = new uint[8];
			HarakaSBase.InterleaveConstant32(haraka256_rc[i], array, i << 5);
			HarakaSBase.InterleaveConstant(haraka512_rc[i], array, i << 6);
		}
	}

	public void Update(byte input)
	{
		buffer[off++] ^= input;
		if (off == 32)
		{
			Haraka512Perm(buffer);
			off = 0;
		}
	}

	public void BlockUpdate(byte[] input, int inOff, int len)
	{
		int num = inOff;
		int num2 = len + off >> 5;
		for (int i = 0; i < num2; i++)
		{
			while (off < 32)
			{
				buffer[off++] ^= input[num++];
			}
			Haraka512Perm(buffer);
			off = 0;
		}
		while (num < inOff + len)
		{
			buffer[off++] ^= input[num++];
		}
	}

	public int OutputFinal(byte[] output, int outOff, int len)
	{
		int result = len;
		buffer[off] ^= 31;
		buffer[31] ^= 128;
		while (len >= 32)
		{
			Haraka512Perm(buffer);
			Array.Copy(buffer, 0, output, outOff, 32);
			outOff += 32;
			len -= 32;
		}
		if (len > 0)
		{
			Haraka512Perm(buffer);
			Array.Copy(buffer, 0, output, outOff, len);
		}
		Reset();
		return result;
	}
}
