using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class Haraka512Digest : HarakaBase
{
	private readonly byte[] m_buf;

	private int m_bufPos;

	public override string AlgorithmName => "Haraka-512";

	public Haraka512Digest()
	{
		m_buf = new byte[64];
		m_bufPos = 0;
	}

	public override int GetByteLength()
	{
		return 64;
	}

	public override void Update(byte input)
	{
		if (m_bufPos > 63)
		{
			throw new ArgumentException("total input cannot be more than 64 bytes");
		}
		m_buf[m_bufPos++] = input;
	}

	public override void BlockUpdate(byte[] input, int inOff, int len)
	{
		if (m_bufPos > 64 - len)
		{
			throw new ArgumentException("total input cannot be more than 64 bytes");
		}
		Array.Copy(input, inOff, m_buf, m_bufPos, len);
		m_bufPos += len;
	}

	public override int DoFinal(byte[] output, int outOff)
	{
		if (m_bufPos != 64)
		{
			throw new ArgumentException("input must be exactly 64 bytes");
		}
		if (output.Length - outOff < 32)
		{
			throw new ArgumentException("output too short to receive digest");
		}
		int result = Haraka512256(m_buf, output, outOff);
		Reset();
		return result;
	}

	public override void Reset()
	{
		m_bufPos = 0;
		Array.Clear(m_buf, 0, 64);
	}

	private static int Haraka512256(byte[] msg, byte[] output, int outOff)
	{
		byte[][] array = new byte[4][]
		{
			new byte[16],
			new byte[16],
			new byte[16],
			new byte[16]
		};
		byte[][] array2 = new byte[4][]
		{
			new byte[16],
			new byte[16],
			new byte[16],
			new byte[16]
		};
		Array.Copy(msg, 0, array[0], 0, 16);
		Array.Copy(msg, 16, array[1], 0, 16);
		Array.Copy(msg, 32, array[2], 0, 16);
		Array.Copy(msg, 48, array[3], 0, 16);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[0]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[1]);
		array[2] = HarakaBase.AesEnc(array[2], HarakaBase.RC[2]);
		array[3] = HarakaBase.AesEnc(array[3], HarakaBase.RC[3]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[4]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[5]);
		array[2] = HarakaBase.AesEnc(array[2], HarakaBase.RC[6]);
		array[3] = HarakaBase.AesEnc(array[3], HarakaBase.RC[7]);
		Mix512(array, array2);
		array[0] = HarakaBase.AesEnc(array2[0], HarakaBase.RC[8]);
		array[1] = HarakaBase.AesEnc(array2[1], HarakaBase.RC[9]);
		array[2] = HarakaBase.AesEnc(array2[2], HarakaBase.RC[10]);
		array[3] = HarakaBase.AesEnc(array2[3], HarakaBase.RC[11]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[12]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[13]);
		array[2] = HarakaBase.AesEnc(array[2], HarakaBase.RC[14]);
		array[3] = HarakaBase.AesEnc(array[3], HarakaBase.RC[15]);
		Mix512(array, array2);
		array[0] = HarakaBase.AesEnc(array2[0], HarakaBase.RC[16]);
		array[1] = HarakaBase.AesEnc(array2[1], HarakaBase.RC[17]);
		array[2] = HarakaBase.AesEnc(array2[2], HarakaBase.RC[18]);
		array[3] = HarakaBase.AesEnc(array2[3], HarakaBase.RC[19]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[20]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[21]);
		array[2] = HarakaBase.AesEnc(array[2], HarakaBase.RC[22]);
		array[3] = HarakaBase.AesEnc(array[3], HarakaBase.RC[23]);
		Mix512(array, array2);
		array[0] = HarakaBase.AesEnc(array2[0], HarakaBase.RC[24]);
		array[1] = HarakaBase.AesEnc(array2[1], HarakaBase.RC[25]);
		array[2] = HarakaBase.AesEnc(array2[2], HarakaBase.RC[26]);
		array[3] = HarakaBase.AesEnc(array2[3], HarakaBase.RC[27]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[28]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[29]);
		array[2] = HarakaBase.AesEnc(array[2], HarakaBase.RC[30]);
		array[3] = HarakaBase.AesEnc(array[3], HarakaBase.RC[31]);
		Mix512(array, array2);
		array[0] = HarakaBase.AesEnc(array2[0], HarakaBase.RC[32]);
		array[1] = HarakaBase.AesEnc(array2[1], HarakaBase.RC[33]);
		array[2] = HarakaBase.AesEnc(array2[2], HarakaBase.RC[34]);
		array[3] = HarakaBase.AesEnc(array2[3], HarakaBase.RC[35]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[36]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[37]);
		array[2] = HarakaBase.AesEnc(array[2], HarakaBase.RC[38]);
		array[3] = HarakaBase.AesEnc(array[3], HarakaBase.RC[39]);
		Mix512(array, array2);
		Bytes.Xor(16, array2[0], 0, msg, 0, array[0], 0);
		Bytes.Xor(16, array2[1], 0, msg, 16, array[1], 0);
		Bytes.Xor(16, array2[2], 0, msg, 32, array[2], 0);
		Bytes.Xor(16, array2[3], 0, msg, 48, array[3], 0);
		Array.Copy(array[0], 8, output, outOff, 8);
		Array.Copy(array[1], 8, output, outOff + 8, 8);
		Array.Copy(array[2], 0, output, outOff + 16, 8);
		Array.Copy(array[3], 0, output, outOff + 24, 8);
		return HarakaBase.DIGEST_SIZE;
	}

	private static void Mix512(byte[][] s1, byte[][] s2)
	{
		Array.Copy(s1[0], 12, s2[0], 0, 4);
		Array.Copy(s1[2], 12, s2[0], 4, 4);
		Array.Copy(s1[1], 12, s2[0], 8, 4);
		Array.Copy(s1[3], 12, s2[0], 12, 4);
		Array.Copy(s1[2], 0, s2[1], 0, 4);
		Array.Copy(s1[0], 0, s2[1], 4, 4);
		Array.Copy(s1[3], 0, s2[1], 8, 4);
		Array.Copy(s1[1], 0, s2[1], 12, 4);
		Array.Copy(s1[2], 4, s2[2], 0, 4);
		Array.Copy(s1[0], 4, s2[2], 4, 4);
		Array.Copy(s1[3], 4, s2[2], 8, 4);
		Array.Copy(s1[1], 4, s2[2], 12, 4);
		Array.Copy(s1[0], 8, s2[3], 0, 4);
		Array.Copy(s1[2], 8, s2[3], 4, 4);
		Array.Copy(s1[1], 8, s2[3], 8, 4);
		Array.Copy(s1[3], 8, s2[3], 12, 4);
	}
}
