using System;
using System.IO;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class IsapDigest : IDigest
{
	private readonly MemoryStream buffer = new MemoryStream();

	private ulong x0;

	private ulong x1;

	private ulong x2;

	private ulong x3;

	private ulong x4;

	public string AlgorithmName => "ISAP Hash";

	public int GetDigestSize()
	{
		return 32;
	}

	public int GetByteLength()
	{
		return 8;
	}

	public void Update(byte input)
	{
		buffer.WriteByte(input);
	}

	public void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		Check.DataLength(input, inOff, inLen, "input buffer too short");
		buffer.Write(input, inOff, inLen);
	}

	public int DoFinal(byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, 32, "output buffer too short");
		x0 = 17191252062196199485uL;
		x1 = 10066134719181819906uL;
		x2 = 13009371945472744034uL;
		x3 = 4834782570098516968uL;
		x4 = 3787428097924915520uL;
		byte[] bs = buffer.GetBuffer();
		int num = Convert.ToInt32(buffer.Length);
		int num2 = 0;
		while (num >= 8)
		{
			x0 ^= Pack.BE_To_UInt64(bs, num2);
			num2 += 8;
			num -= 8;
			P12();
		}
		x0 ^= (ulong)(128L << (7 - num << 3));
		if (num > 0)
		{
			x0 ^= Pack.BE_To_UInt64_High(bs, num2, num);
		}
		for (int i = 0; i < 4; i++)
		{
			P12();
			Pack.UInt64_To_BE(x0, output, outOff + (i << 3));
		}
		return 32;
	}

	public void Reset()
	{
		buffer.SetLength(0L);
	}

	private void P12()
	{
		ROUND(240uL);
		ROUND(225uL);
		ROUND(210uL);
		ROUND(195uL);
		ROUND(180uL);
		ROUND(165uL);
		ROUND(150uL);
		ROUND(135uL);
		ROUND(120uL);
		ROUND(105uL);
		ROUND(90uL);
		ROUND(75uL);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ROUND(ulong C)
	{
		ulong num = x0 ^ x1 ^ x2 ^ x3 ^ C ^ (x1 & (x0 ^ x2 ^ x4 ^ C));
		ulong num2 = x0 ^ x2 ^ x3 ^ x4 ^ C ^ ((x1 ^ x2 ^ C) & (x1 ^ x3));
		ulong num3 = x1 ^ x2 ^ x4 ^ C ^ (x3 & x4);
		ulong num4 = x0 ^ x1 ^ x2 ^ C ^ (~x0 & (x3 ^ x4));
		ulong num5 = x1 ^ x3 ^ x4 ^ ((x0 ^ x4) & x1);
		x0 = num ^ Longs.RotateRight(num, 19) ^ Longs.RotateRight(num, 28);
		x1 = num2 ^ Longs.RotateRight(num2, 39) ^ Longs.RotateRight(num2, 61);
		x2 = ~(num3 ^ Longs.RotateRight(num3, 1) ^ Longs.RotateRight(num3, 6));
		x3 = num4 ^ Longs.RotateRight(num4, 10) ^ Longs.RotateRight(num4, 17);
		x4 = num5 ^ Longs.RotateRight(num5, 7) ^ Longs.RotateRight(num5, 41);
	}
}
