using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

internal class PolyVec
{
	private KyberEngine m_engine;

	internal Poly[] m_vec;

	internal PolyVec(KyberEngine engine)
	{
		m_engine = engine;
		m_vec = new Poly[engine.K];
		for (int i = 0; i < engine.K; i++)
		{
			m_vec[i] = new Poly(engine);
		}
	}

	internal void Ntt()
	{
		for (int i = 0; i < m_engine.K; i++)
		{
			m_vec[i].PolyNtt();
		}
	}

	internal void InverseNttToMont()
	{
		for (int i = 0; i < m_engine.K; i++)
		{
			m_vec[i].PolyInverseNttToMont();
		}
	}

	internal static void PointwiseAccountMontgomery(Poly r, PolyVec a, PolyVec b, KyberEngine engine)
	{
		Poly poly = new Poly(engine);
		Poly.BaseMultMontgomery(r, a.m_vec[0], b.m_vec[0]);
		for (int i = 1; i < engine.K; i++)
		{
			Poly.BaseMultMontgomery(poly, a.m_vec[i], b.m_vec[i]);
			r.Add(poly);
		}
		r.PolyReduce();
	}

	internal void Add(PolyVec a)
	{
		for (int i = 0; i < m_engine.K; i++)
		{
			m_vec[i].Add(a.m_vec[i]);
		}
	}

	internal void Reduce()
	{
		for (int i = 0; i < m_engine.K; i++)
		{
			m_vec[i].PolyReduce();
		}
	}

	internal void CompressPolyVec(byte[] r)
	{
		ConditionalSubQ();
		int num = 0;
		if (m_engine.PolyVecCompressedBytes == m_engine.K * 320)
		{
			short[] array = new short[4];
			for (int i = 0; i < m_engine.K; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						array[k] = (short)(((uint)((m_vec[i].m_coeffs[4 * j + k] << 10) + 1664) / 3329u) & 0x3FF);
					}
					r[num] = (byte)array[0];
					r[num + 1] = (byte)((array[0] >> 8) | (array[1] << 2));
					r[num + 2] = (byte)((array[1] >> 6) | (array[2] << 4));
					r[num + 3] = (byte)((array[2] >> 4) | (array[3] << 6));
					r[num + 4] = (byte)(array[3] >> 2);
					num += 5;
				}
			}
			return;
		}
		if (m_engine.PolyVecCompressedBytes == m_engine.K * 352)
		{
			short[] array2 = new short[8];
			for (int l = 0; l < m_engine.K; l++)
			{
				for (int m = 0; m < 32; m++)
				{
					for (int n = 0; n < 8; n++)
					{
						array2[n] = (short)(((uint)((m_vec[l].m_coeffs[8 * m + n] << 11) + 1664) / 3329u) & 0x7FF);
					}
					r[num] = (byte)array2[0];
					r[num + 1] = (byte)((array2[0] >> 8) | (array2[1] << 3));
					r[num + 2] = (byte)((array2[1] >> 5) | (array2[2] << 6));
					r[num + 3] = (byte)(array2[2] >> 2);
					r[num + 4] = (byte)((array2[2] >> 10) | (array2[3] << 1));
					r[num + 5] = (byte)((array2[3] >> 7) | (array2[4] << 4));
					r[num + 6] = (byte)((array2[4] >> 4) | (array2[5] << 7));
					r[num + 7] = (byte)(array2[5] >> 1);
					r[num + 8] = (byte)((array2[5] >> 9) | (array2[6] << 2));
					r[num + 9] = (byte)((array2[6] >> 6) | (array2[7] << 5));
					r[num + 10] = (byte)(array2[7] >> 3);
					num += 11;
				}
			}
			return;
		}
		throw new ArgumentException("Kyber PolyVecCompressedBytes neither 320 * KyberK or 352 * KyberK!");
	}

	internal void DecompressPolyVec(byte[] compressedCipherText)
	{
		int num = 0;
		if (m_engine.PolyVecCompressedBytes == m_engine.K * 320)
		{
			short[] array = new short[4];
			for (int i = 0; i < m_engine.K; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					array[0] = (short)((compressedCipherText[num] & 0xFF) | ((ushort)(compressedCipherText[num + 1] & 0xFF) << 8));
					array[1] = (short)(((compressedCipherText[num + 1] & 0xFF) >> 2) | ((ushort)(compressedCipherText[num + 2] & 0xFF) << 6));
					array[2] = (short)(((compressedCipherText[num + 2] & 0xFF) >> 4) | ((ushort)(compressedCipherText[num + 3] & 0xFF) << 4));
					array[3] = (short)(((compressedCipherText[num + 3] & 0xFF) >> 6) | ((ushort)(compressedCipherText[num + 4] & 0xFF) << 2));
					num += 5;
					for (int k = 0; k < 4; k++)
					{
						m_vec[i].m_coeffs[4 * j + k] = (short)((array[k] & 0x3FF) * 3329 + 512 >> 10);
					}
				}
			}
			return;
		}
		if (m_engine.PolyVecCompressedBytes == m_engine.K * 352)
		{
			short[] array2 = new short[8];
			for (int l = 0; l < m_engine.K; l++)
			{
				for (int m = 0; m < 32; m++)
				{
					array2[0] = (short)((compressedCipherText[num] & 0xFF) | ((ushort)(compressedCipherText[num + 1] & 0xFF) << 8));
					array2[1] = (short)(((compressedCipherText[num + 1] & 0xFF) >> 3) | ((ushort)(compressedCipherText[num + 2] & 0xFF) << 5));
					array2[2] = (short)(((compressedCipherText[num + 2] & 0xFF) >> 6) | ((ushort)(compressedCipherText[num + 3] & 0xFF) << 2) | (ushort)((compressedCipherText[num + 4] & 0xFF) << 10));
					array2[3] = (short)(((compressedCipherText[num + 4] & 0xFF) >> 1) | ((ushort)(compressedCipherText[num + 5] & 0xFF) << 7));
					array2[4] = (short)(((compressedCipherText[num + 5] & 0xFF) >> 4) | ((ushort)(compressedCipherText[num + 6] & 0xFF) << 4));
					array2[5] = (short)(((compressedCipherText[num + 6] & 0xFF) >> 7) | ((ushort)(compressedCipherText[num + 7] & 0xFF) << 1) | (ushort)((compressedCipherText[num + 8] & 0xFF) << 9));
					array2[6] = (short)(((compressedCipherText[num + 8] & 0xFF) >> 2) | ((ushort)(compressedCipherText[num + 9] & 0xFF) << 6));
					array2[7] = (short)(((compressedCipherText[num + 9] & 0xFF) >> 5) | ((ushort)(compressedCipherText[num + 10] & 0xFF) << 3));
					num += 11;
					for (int n = 0; n < 8; n++)
					{
						m_vec[l].m_coeffs[8 * m + n] = (short)((array2[n] & 0x7FF) * 3329 + 1024 >> 11);
					}
				}
			}
			return;
		}
		throw new ArgumentException("Kyber PolyVecCompressedBytes neither 320 * KyberK or 352 * KyberK!");
	}

	internal void ToBytes(byte[] r)
	{
		for (int i = 0; i < m_engine.K; i++)
		{
			m_vec[i].ToBytes(r, i * KyberEngine.PolyBytes);
		}
	}

	internal void FromBytes(byte[] pk)
	{
		for (int i = 0; i < m_engine.K; i++)
		{
			m_vec[i].FromBytes(pk, i * KyberEngine.PolyBytes);
		}
	}

	private void ConditionalSubQ()
	{
		for (int i = 0; i < m_engine.K; i++)
		{
			m_vec[i].CondSubQ();
		}
	}
}
