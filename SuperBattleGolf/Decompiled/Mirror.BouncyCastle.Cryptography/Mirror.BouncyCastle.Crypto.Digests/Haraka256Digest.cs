using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class Haraka256Digest : HarakaBase
{
	private readonly byte[] m_buf;

	private int m_bufPos;

	public override string AlgorithmName => "Haraka-256";

	public Haraka256Digest()
	{
		m_buf = new byte[32];
		m_bufPos = 0;
	}

	public override int GetByteLength()
	{
		return 32;
	}

	public override void Update(byte input)
	{
		if (m_bufPos > 31)
		{
			throw new ArgumentException("total input cannot be more than 32 bytes");
		}
		m_buf[m_bufPos++] = input;
	}

	public override void BlockUpdate(byte[] input, int inOff, int len)
	{
		if (m_bufPos > 32 - len)
		{
			throw new ArgumentException("total input cannot be more than 32 bytes");
		}
		Array.Copy(input, inOff, m_buf, m_bufPos, len);
		m_bufPos += len;
	}

	public override int DoFinal(byte[] output, int outOff)
	{
		if (m_bufPos != 32)
		{
			throw new ArgumentException("input must be exactly 32 bytes");
		}
		if (output.Length - outOff < 32)
		{
			throw new ArgumentException("output too short to receive digest");
		}
		int result = Haraka256256(m_buf, output, outOff);
		Reset();
		return result;
	}

	public override void Reset()
	{
		m_bufPos = 0;
		Array.Clear(m_buf, 0, 32);
	}

	private static int Haraka256256(byte[] msg, byte[] output, int outOff)
	{
		byte[][] array = new byte[2][]
		{
			new byte[16],
			new byte[16]
		};
		byte[][] array2 = new byte[2][]
		{
			new byte[16],
			new byte[16]
		};
		Array.Copy(msg, 0, array[0], 0, 16);
		Array.Copy(msg, 16, array[1], 0, 16);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[0]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[1]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[2]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[3]);
		Mix256(array, array2);
		array[0] = HarakaBase.AesEnc(array2[0], HarakaBase.RC[4]);
		array[1] = HarakaBase.AesEnc(array2[1], HarakaBase.RC[5]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[6]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[7]);
		Mix256(array, array2);
		array[0] = HarakaBase.AesEnc(array2[0], HarakaBase.RC[8]);
		array[1] = HarakaBase.AesEnc(array2[1], HarakaBase.RC[9]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[10]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[11]);
		Mix256(array, array2);
		array[0] = HarakaBase.AesEnc(array2[0], HarakaBase.RC[12]);
		array[1] = HarakaBase.AesEnc(array2[1], HarakaBase.RC[13]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[14]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[15]);
		Mix256(array, array2);
		array[0] = HarakaBase.AesEnc(array2[0], HarakaBase.RC[16]);
		array[1] = HarakaBase.AesEnc(array2[1], HarakaBase.RC[17]);
		array[0] = HarakaBase.AesEnc(array[0], HarakaBase.RC[18]);
		array[1] = HarakaBase.AesEnc(array[1], HarakaBase.RC[19]);
		Mix256(array, array2);
		Bytes.Xor(16, array2[0], 0, msg, 0, output, outOff);
		Bytes.Xor(16, array2[1], 0, msg, 16, output, outOff + 16);
		return HarakaBase.DIGEST_SIZE;
	}

	private static void Mix256(byte[][] s1, byte[][] s2)
	{
		Array.Copy(s1[0], 0, s2[0], 0, 4);
		Array.Copy(s1[1], 0, s2[0], 4, 4);
		Array.Copy(s1[0], 4, s2[0], 8, 4);
		Array.Copy(s1[1], 4, s2[0], 12, 4);
		Array.Copy(s1[0], 8, s2[1], 0, 4);
		Array.Copy(s1[1], 8, s2[1], 4, 4);
		Array.Copy(s1[0], 12, s2[1], 8, 4);
		Array.Copy(s1[1], 12, s2[1], 12, 4);
	}
}
