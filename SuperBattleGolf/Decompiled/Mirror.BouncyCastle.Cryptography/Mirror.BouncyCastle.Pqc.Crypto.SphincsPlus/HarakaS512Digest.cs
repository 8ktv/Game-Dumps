using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal sealed class HarakaS512Digest : HarakaSBase
{
	public string AlgorithmName => "HarakaS-512";

	public HarakaS512Digest(HarakaSBase harakaSBase)
	{
		haraka512_rc = harakaSBase.haraka512_rc;
	}

	public int GetDigestSize()
	{
		return 32;
	}

	public void Update(byte input)
	{
		if (off > 63)
		{
			throw new ArgumentException("total input cannot be more than 64 bytes");
		}
		buffer[off++] = input;
	}

	public void BlockUpdate(byte[] input, int inOff, int len)
	{
		if (off > 64 - len)
		{
			throw new ArgumentException("total input cannot be more than 64 bytes");
		}
		Array.Copy(input, inOff, buffer, off, len);
		off += len;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		byte[] array = new byte[64];
		Haraka512Perm(array);
		HarakaSBase.Xor(array, 8, buffer, 8, output, outOff, 8);
		HarakaSBase.Xor(array, 24, buffer, 24, output, outOff + 8, 16);
		HarakaSBase.Xor(array, 48, buffer, 48, output, outOff + 24, 8);
		Reset();
		return 32;
	}
}
