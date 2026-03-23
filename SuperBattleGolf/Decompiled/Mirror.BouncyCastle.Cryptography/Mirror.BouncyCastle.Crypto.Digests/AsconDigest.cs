using System;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class AsconDigest : IDigest
{
	public enum AsconParameters
	{
		AsconHash,
		AsconHashA
	}

	private readonly AsconParameters m_asconParameters;

	private readonly int ASCON_PB_ROUNDS;

	private ulong x0;

	private ulong x1;

	private ulong x2;

	private ulong x3;

	private ulong x4;

	private readonly byte[] m_buf = new byte[8];

	private int m_bufPos;

	public string AlgorithmName => m_asconParameters switch
	{
		AsconParameters.AsconHash => "Ascon-Hash", 
		AsconParameters.AsconHashA => "Ascon-HashA", 
		_ => throw new InvalidOperationException(), 
	};

	public AsconDigest(AsconParameters parameters)
	{
		m_asconParameters = parameters;
		switch (parameters)
		{
		case AsconParameters.AsconHash:
			ASCON_PB_ROUNDS = 12;
			break;
		case AsconParameters.AsconHashA:
			ASCON_PB_ROUNDS = 8;
			break;
		default:
			throw new ArgumentException("Invalid parameter settings for Ascon Hash");
		}
		Reset();
	}

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
		m_buf[m_bufPos] = input;
		if (++m_bufPos == 8)
		{
			x0 ^= Pack.BE_To_UInt64(m_buf, 0);
			P(ASCON_PB_ROUNDS);
			m_bufPos = 0;
		}
	}

	public void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		Check.DataLength(input, inOff, inLen, "input buffer too short");
		if (inLen < 1)
		{
			return;
		}
		int num = 8 - m_bufPos;
		if (inLen < num)
		{
			Array.Copy(input, inOff, m_buf, m_bufPos, inLen);
			m_bufPos += inLen;
			return;
		}
		int num2 = 0;
		if (m_bufPos > 0)
		{
			Array.Copy(input, inOff, m_buf, m_bufPos, num);
			num2 += num;
			x0 ^= Pack.BE_To_UInt64(m_buf, 0);
			P(ASCON_PB_ROUNDS);
		}
		int num3;
		while ((num3 = inLen - num2) >= 8)
		{
			x0 ^= Pack.BE_To_UInt64(input, inOff + num2);
			P(ASCON_PB_ROUNDS);
			num2 += 8;
		}
		Array.Copy(input, inOff + num2, m_buf, 0, num3);
		m_bufPos = num3;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, 32, "output buffer too short");
		FinishAbsorbing();
		Pack.UInt64_To_BE(x0, output, outOff);
		for (int i = 0; i < 3; i++)
		{
			outOff += 8;
			P(ASCON_PB_ROUNDS);
			Pack.UInt64_To_BE(x0, output, outOff);
		}
		Reset();
		return 32;
	}

	public void Reset()
	{
		Array.Clear(m_buf, 0, m_buf.Length);
		m_bufPos = 0;
		switch (m_asconParameters)
		{
		case AsconParameters.AsconHashA:
			x0 = 92044056785660070uL;
			x1 = 8326807761760157607uL;
			x2 = 3371194088139667532uL;
			x3 = 15489749720654559101uL;
			x4 = 11618234402860862855uL;
			break;
		case AsconParameters.AsconHash:
			x0 = 17191252062196199485uL;
			x1 = 10066134719181819906uL;
			x2 = 13009371945472744034uL;
			x3 = 4834782570098516968uL;
			x4 = 3787428097924915520uL;
			break;
		default:
			throw new InvalidOperationException();
		}
	}

	private void FinishAbsorbing()
	{
		m_buf[m_bufPos] = 128;
		x0 ^= Pack.BE_To_UInt64(m_buf, 0) & (ulong)(-1L << 56 - (m_bufPos << 3));
		P(12);
	}

	private void P(int nr)
	{
		if (nr == 12)
		{
			ROUND(240uL);
			ROUND(225uL);
			ROUND(210uL);
			ROUND(195uL);
		}
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
	private void ROUND(ulong c)
	{
		ulong num = x0 ^ x1 ^ x2 ^ x3 ^ c ^ (x1 & (x0 ^ x2 ^ x4 ^ c));
		ulong num2 = x0 ^ x2 ^ x3 ^ x4 ^ c ^ ((x1 ^ x2 ^ c) & (x1 ^ x3));
		ulong num3 = x1 ^ x2 ^ x4 ^ c ^ (x3 & x4);
		ulong num4 = x0 ^ x1 ^ x2 ^ c ^ (~x0 & (x3 ^ x4));
		ulong num5 = x1 ^ x3 ^ x4 ^ ((x0 ^ x4) & x1);
		x0 = num ^ Longs.RotateRight(num, 19) ^ Longs.RotateRight(num, 28);
		x1 = num2 ^ Longs.RotateRight(num2, 39) ^ Longs.RotateRight(num2, 61);
		x2 = ~(num3 ^ Longs.RotateRight(num3, 1) ^ Longs.RotateRight(num3, 6));
		x3 = num4 ^ Longs.RotateRight(num4, 10) ^ Longs.RotateRight(num4, 17);
		x4 = num5 ^ Longs.RotateRight(num5, 7) ^ Longs.RotateRight(num5, 41);
	}
}
