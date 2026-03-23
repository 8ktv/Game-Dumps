using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

internal class Poly
{
	private KyberEngine m_engine;

	public short[] m_coeffs = new short[256];

	private Symmetric m_symmetric;

	internal short[] Coeffs => m_coeffs;

	public Poly(KyberEngine mEngine)
	{
		m_engine = mEngine;
		m_symmetric = mEngine.Symmetric;
	}

	internal void GetNoiseEta1(byte[] seed, byte nonce)
	{
		byte[] array = new byte[m_engine.Eta1 * 256 / 4];
		m_symmetric.Prf(array, seed, nonce);
		Cbd.Eta(this, array, m_engine.Eta1);
	}

	internal void GetNoiseEta2(byte[] seed, byte nonce)
	{
		byte[] array = new byte[128];
		m_symmetric.Prf(array, seed, nonce);
		Cbd.Eta(this, array, 2);
	}

	internal void PolyNtt()
	{
		Ntt.NTT(Coeffs);
		PolyReduce();
	}

	internal void PolyInverseNttToMont()
	{
		Ntt.InvNTT(Coeffs);
	}

	internal static void BaseMultMontgomery(Poly r, Poly a, Poly b)
	{
		for (int i = 0; i < 64; i++)
		{
			Ntt.BaseMult(r.Coeffs, 4 * i, a.Coeffs[4 * i], a.Coeffs[4 * i + 1], b.Coeffs[4 * i], b.Coeffs[4 * i + 1], Ntt.Zetas[64 + i]);
			Ntt.BaseMult(r.Coeffs, 4 * i + 2, a.Coeffs[4 * i + 2], a.Coeffs[4 * i + 3], b.Coeffs[4 * i + 2], b.Coeffs[4 * i + 3], (short)(-1 * Ntt.Zetas[64 + i]));
		}
	}

	internal void ToMont()
	{
		for (int i = 0; i < 256; i++)
		{
			Coeffs[i] = Reduce.MontgomeryReduce(Coeffs[i] * 1353);
		}
	}

	internal void Add(Poly a)
	{
		for (int i = 0; i < 256; i++)
		{
			Coeffs[i] += a.Coeffs[i];
		}
	}

	internal void Subtract(Poly a)
	{
		for (int i = 0; i < 256; i++)
		{
			Coeffs[i] = (short)(a.Coeffs[i] - Coeffs[i]);
		}
	}

	internal void PolyReduce()
	{
		for (int i = 0; i < 256; i++)
		{
			Coeffs[i] = Reduce.BarrettReduce(Coeffs[i]);
		}
	}

	internal void CompressPoly(byte[] r, int off)
	{
		byte[] array = new byte[8];
		int num = 0;
		CondSubQ();
		if (m_engine.PolyCompressedBytes == 128)
		{
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					array[j] = (byte)((((Coeffs[8 * i + j] << 4) + 1664) / 3329) & 0xF);
				}
				r[off + num] = (byte)(array[0] | (array[1] << 4));
				r[off + num + 1] = (byte)(array[2] | (array[3] << 4));
				r[off + num + 2] = (byte)(array[4] | (array[5] << 4));
				r[off + num + 3] = (byte)(array[6] | (array[7] << 4));
				num += 4;
			}
			return;
		}
		if (m_engine.PolyCompressedBytes == 160)
		{
			for (int k = 0; k < 32; k++)
			{
				for (int l = 0; l < 8; l++)
				{
					array[l] = (byte)((((Coeffs[8 * k + l] << 5) + 1664) / 3329) & 0x1F);
				}
				r[off + num] = (byte)(array[0] | (array[1] << 5));
				r[off + num + 1] = (byte)((array[1] >> 3) | (array[2] << 2) | (array[3] << 7));
				r[off + num + 2] = (byte)((array[3] >> 1) | (array[4] << 4));
				r[off + num + 3] = (byte)((array[4] >> 4) | (array[5] << 1) | (array[6] << 6));
				r[off + num + 4] = (byte)((array[6] >> 2) | (array[7] << 3));
				num += 5;
			}
			return;
		}
		throw new ArgumentException("PolyCompressedBytes is neither 128 or 160!");
	}

	internal void DecompressPoly(byte[] CompressedCipherText, int off)
	{
		int num = off;
		if (m_engine.PolyCompressedBytes == 128)
		{
			for (int i = 0; i < 128; i++)
			{
				Coeffs[2 * i] = (short)((short)(CompressedCipherText[num] & 0xFF & 0xF) * 3329 + 8 >> 4);
				Coeffs[2 * i + 1] = (short)((short)((CompressedCipherText[num] & 0xFF) >> 4) * 3329 + 8 >> 4);
				num++;
			}
			return;
		}
		if (m_engine.PolyCompressedBytes == 160)
		{
			byte[] array = new byte[8];
			for (int j = 0; j < 32; j++)
			{
				array[0] = (byte)(CompressedCipherText[num] & 0xFF);
				array[1] = (byte)(((CompressedCipherText[num] & 0xFF) >> 5) | ((CompressedCipherText[num + 1] & 0xFF) << 3));
				array[2] = (byte)((CompressedCipherText[num + 1] & 0xFF) >> 2);
				array[3] = (byte)(((CompressedCipherText[num + 1] & 0xFF) >> 7) | ((CompressedCipherText[num + 2] & 0xFF) << 1));
				array[4] = (byte)(((CompressedCipherText[num + 2] & 0xFF) >> 4) | ((CompressedCipherText[num + 3] & 0xFF) << 4));
				array[5] = (byte)((CompressedCipherText[num + 3] & 0xFF) >> 1);
				array[6] = (byte)(((CompressedCipherText[num + 3] & 0xFF) >> 6) | ((CompressedCipherText[num + 4] & 0xFF) << 2));
				array[7] = (byte)((CompressedCipherText[num + 4] & 0xFF) >> 3);
				num += 5;
				for (int k = 0; k < 8; k++)
				{
					Coeffs[8 * j + k] = (short)((array[k] & 0x1F) * 3329 + 16 >> 5);
				}
			}
			return;
		}
		throw new ArgumentException("PolyCompressedBytes is neither 128 or 160!");
	}

	internal void ToBytes(byte[] r, int off)
	{
		CondSubQ();
		for (int i = 0; i < 128; i++)
		{
			ushort num = (ushort)Coeffs[2 * i];
			ushort num2 = (ushort)Coeffs[2 * i + 1];
			r[off + 3 * i] = (byte)num;
			r[off + 3 * i + 1] = (byte)((num >> 8) | (ushort)(num2 << 4));
			r[off + 3 * i + 2] = (byte)(ushort)(num2 >> 4);
		}
	}

	internal void FromBytes(byte[] a, int off)
	{
		for (int i = 0; i < 128; i++)
		{
			Coeffs[2 * i] = (short)(((a[off + 3 * i] & 0xFF) | (ushort)((a[off + 3 * i + 1] & 0xFF) << 8)) & 0xFFF);
			Coeffs[2 * i + 1] = (short)((((a[off + 3 * i + 1] & 0xFF) >> 4) | (ushort)((a[off + 3 * i + 2] & 0xFF) << 4)) & 0xFFF);
		}
	}

	internal void ToMsg(byte[] msg)
	{
		CondSubQ();
		for (int i = 0; i < 32; i++)
		{
			msg[i] = 0;
			for (int j = 0; j < 8; j++)
			{
				int num = Coeffs[8 * i + j] & 0xFFFF;
				num <<= 1;
				num += 1665;
				num *= 80635;
				num >>= 28;
				num &= 1;
				msg[i] |= (byte)(num << j);
			}
		}
	}

	internal void FromMsg(byte[] m)
	{
		if (m.Length != 32)
		{
			throw new ArgumentException("KYBER_INDCPA_MSGBYTES must be equal to KYBER_N/8 bytes!");
		}
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				short num = (short)(-1 * (short)(((m[i] & 0xFF) >> j) & 1));
				Coeffs[8 * i + j] = (short)(num & 0x681);
			}
		}
	}

	internal void CondSubQ()
	{
		for (int i = 0; i < 256; i++)
		{
			Coeffs[i] = Reduce.CondSubQ(Coeffs[i]);
		}
	}
}
