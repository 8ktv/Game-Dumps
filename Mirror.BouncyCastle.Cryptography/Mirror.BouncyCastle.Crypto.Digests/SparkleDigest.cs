using System;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class SparkleDigest : IDigest
{
	public enum SparkleParameters
	{
		ESCH256,
		ESCH384
	}

	private const int RATE_BYTES = 16;

	private const int RATE_WORDS = 4;

	private string algorithmName;

	private readonly uint[] state;

	private readonly byte[] m_buf = new byte[16];

	private readonly int DIGEST_BYTES;

	private readonly int SPARKLE_STEPS_SLIM;

	private readonly int SPARKLE_STEPS_BIG;

	private readonly int STATE_WORDS;

	private int m_bufPos;

	public string AlgorithmName => algorithmName;

	public SparkleDigest(SparkleParameters sparkleParameters)
	{
		switch (sparkleParameters)
		{
		case SparkleParameters.ESCH256:
			algorithmName = "ESCH-256";
			DIGEST_BYTES = 32;
			SPARKLE_STEPS_SLIM = 7;
			SPARKLE_STEPS_BIG = 11;
			STATE_WORDS = 12;
			break;
		case SparkleParameters.ESCH384:
			algorithmName = "ESCH-384";
			DIGEST_BYTES = 48;
			SPARKLE_STEPS_SLIM = 8;
			SPARKLE_STEPS_BIG = 12;
			STATE_WORDS = 16;
			break;
		default:
			throw new ArgumentException("Invalid definition of ESCH instance");
		}
		state = new uint[STATE_WORDS];
	}

	public int GetDigestSize()
	{
		return DIGEST_BYTES;
	}

	public int GetByteLength()
	{
		return 16;
	}

	public void Update(byte input)
	{
		if (m_bufPos == 16)
		{
			ProcessBlock(m_buf, 0, SPARKLE_STEPS_SLIM);
			m_bufPos = 0;
		}
		m_buf[m_bufPos++] = input;
	}

	public void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		Check.DataLength(input, inOff, inLen, "input buffer too short");
		if (inLen < 1)
		{
			return;
		}
		int num = 16 - m_bufPos;
		if (inLen <= num)
		{
			Array.Copy(input, inOff, m_buf, m_bufPos, inLen);
			m_bufPos += inLen;
			return;
		}
		int num2 = 0;
		if (m_bufPos > 0)
		{
			Array.Copy(input, inOff, m_buf, m_bufPos, num);
			ProcessBlock(m_buf, 0, SPARKLE_STEPS_SLIM);
			num2 += num;
		}
		int num3;
		while ((num3 = inLen - num2) > 16)
		{
			ProcessBlock(input, inOff + num2, SPARKLE_STEPS_SLIM);
			num2 += 16;
		}
		Array.Copy(input, inOff + num2, m_buf, 0, num3);
		m_bufPos = num3;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, DIGEST_BYTES, "output buffer too short");
		if (m_bufPos < 16)
		{
			state[(STATE_WORDS >> 1) - 1] ^= 16777216u;
			m_buf[m_bufPos] = 128;
			while (++m_bufPos < 16)
			{
				m_buf[m_bufPos] = 0;
			}
		}
		else
		{
			state[(STATE_WORDS >> 1) - 1] ^= 33554432u;
		}
		ProcessBlock(m_buf, 0, SPARKLE_STEPS_BIG);
		Pack.UInt32_To_LE(state, 0, 4, output, outOff);
		if (STATE_WORDS == 16)
		{
			SparkleEngine.SparkleOpt16(state, SPARKLE_STEPS_SLIM);
			Pack.UInt32_To_LE(state, 0, 4, output, outOff + 16);
			SparkleEngine.SparkleOpt16(state, SPARKLE_STEPS_SLIM);
			Pack.UInt32_To_LE(state, 0, 4, output, outOff + 32);
		}
		else
		{
			SparkleEngine.SparkleOpt12(state, SPARKLE_STEPS_SLIM);
			Pack.UInt32_To_LE(state, 0, 4, output, outOff + 16);
		}
		Reset();
		return DIGEST_BYTES;
	}

	public void Reset()
	{
		Arrays.Fill(state, 0u);
		Arrays.Fill(m_buf, 0);
		m_bufPos = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ProcessBlock(byte[] buf, int off, int steps)
	{
		uint num = Pack.LE_To_UInt32(buf, off);
		uint num2 = Pack.LE_To_UInt32(buf, off + 4);
		uint num3 = Pack.LE_To_UInt32(buf, off + 8);
		uint num4 = Pack.LE_To_UInt32(buf, off + 12);
		uint num5 = ELL(num ^ num3);
		uint num6 = ELL(num2 ^ num4);
		state[0] ^= num ^ num6;
		state[1] ^= num2 ^ num5;
		state[2] ^= num3 ^ num6;
		state[3] ^= num4 ^ num5;
		state[4] ^= num6;
		state[5] ^= num5;
		if (STATE_WORDS == 16)
		{
			state[6] ^= num6;
			state[7] ^= num5;
			SparkleEngine.SparkleOpt16(state, steps);
		}
		else
		{
			SparkleEngine.SparkleOpt12(state, steps);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ELL(uint x)
	{
		return Integers.RotateRight(x, 16) ^ (x & 0xFFFF);
	}
}
