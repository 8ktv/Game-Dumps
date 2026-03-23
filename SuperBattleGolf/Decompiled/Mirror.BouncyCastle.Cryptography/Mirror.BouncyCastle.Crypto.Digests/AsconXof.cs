using System;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class AsconXof : IXof, IDigest
{
	public enum AsconParameters
	{
		AsconXof,
		AsconXofA
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

	private bool m_squeezing;

	public string AlgorithmName => m_asconParameters switch
	{
		AsconParameters.AsconXof => "Ascon-Xof", 
		AsconParameters.AsconXofA => "Ascon-XofA", 
		_ => throw new InvalidOperationException(), 
	};

	public AsconXof(AsconParameters parameters)
	{
		m_asconParameters = parameters;
		switch (parameters)
		{
		case AsconParameters.AsconXof:
			ASCON_PB_ROUNDS = 12;
			break;
		case AsconParameters.AsconXofA:
			ASCON_PB_ROUNDS = 8;
			break;
		default:
			throw new ArgumentException("Invalid parameter settings for Ascon XOF");
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
		if (m_squeezing)
		{
			throw new InvalidOperationException("attempt to absorb while squeezing");
		}
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
		if (m_squeezing)
		{
			throw new InvalidOperationException("attempt to absorb while squeezing");
		}
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
		return OutputFinal(output, outOff, GetDigestSize());
	}

	public int OutputFinal(byte[] output, int outOff, int outLen)
	{
		Check.OutputLength(output, outOff, outLen, "output buffer is too short");
		int result = Output(output, outOff, outLen);
		Reset();
		return result;
	}

	public int Output(byte[] output, int outOff, int outLen)
	{
		Check.OutputLength(output, outOff, outLen, "output buffer is too short");
		int result = outLen;
		if (!m_squeezing)
		{
			FinishAbsorbing();
			if (outLen >= 8)
			{
				Pack.UInt64_To_BE(x0, output, outOff);
				outOff += 8;
				outLen -= 8;
			}
			else
			{
				Pack.UInt64_To_BE(x0, m_buf);
				m_bufPos = 0;
			}
		}
		if (m_bufPos < 8)
		{
			int num = 8 - m_bufPos;
			if (outLen <= num)
			{
				Array.Copy(m_buf, m_bufPos, output, outOff, outLen);
				m_bufPos += outLen;
				return result;
			}
			Array.Copy(m_buf, m_bufPos, output, outOff, num);
			outOff += num;
			outLen -= num;
		}
		while (outLen >= 8)
		{
			P(ASCON_PB_ROUNDS);
			Pack.UInt64_To_BE(x0, output, outOff);
			outOff += 8;
			outLen -= 8;
		}
		if (outLen > 0)
		{
			P(ASCON_PB_ROUNDS);
			Pack.UInt64_To_BE(x0, m_buf);
			Array.Copy(m_buf, 0, output, outOff, outLen);
		}
		m_bufPos = outLen;
		return result;
	}

	public void Reset()
	{
		Array.Clear(m_buf, 0, m_buf.Length);
		m_bufPos = 0;
		m_squeezing = false;
		switch (m_asconParameters)
		{
		case AsconParameters.AsconXof:
			x0 = 13077933504456348694uL;
			x1 = 3121280575360345120uL;
			x2 = 7395939140700676632uL;
			x3 = 6533890155656471820uL;
			x4 = 5710016986865767350uL;
			break;
		case AsconParameters.AsconXofA:
			x0 = 4940560291654768690uL;
			x1 = 14811614245468591410uL;
			x2 = 17849209150987444521uL;
			x3 = 2623493988082852443uL;
			x4 = 12162917349548726079uL;
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
		m_bufPos = 8;
		m_squeezing = true;
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
