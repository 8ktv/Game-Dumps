using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal sealed class HarakaS256Digest : HarakaSBase
{
	public string AlgorithmName => "HarakaS-256";

	public HarakaS256Digest(HarakaSXof harakaSXof)
	{
		haraka256_rc = harakaSXof.haraka256_rc;
	}

	public int GetDigestSize()
	{
		return 32;
	}

	public void Update(byte input)
	{
		if (off > 31)
		{
			throw new ArgumentException("total input cannot be more than 32 bytes");
		}
		buffer[off++] = input;
	}

	public void BlockUpdate(byte[] input, int inOff, int len)
	{
		if (off > 32 - len)
		{
			throw new ArgumentException("total input cannot be more than 32 bytes");
		}
		Array.Copy(input, inOff, buffer, off, len);
		off += len;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		byte[] array = new byte[32];
		Haraka256Perm(array);
		HarakaSBase.Xor(array, 0, buffer, 0, output, outOff, 32);
		Reset();
		return 32;
	}
}
