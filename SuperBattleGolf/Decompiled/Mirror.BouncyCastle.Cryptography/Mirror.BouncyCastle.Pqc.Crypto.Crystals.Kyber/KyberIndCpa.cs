using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

internal class KyberIndCpa
{
	private readonly KyberEngine m_engine;

	private Symmetric m_symmetric;

	private int GenerateMatrixNBlocks => (472 + m_symmetric.XofBlockBytes) / m_symmetric.XofBlockBytes;

	internal KyberIndCpa(KyberEngine mEngine)
	{
		m_engine = mEngine;
		m_symmetric = mEngine.Symmetric;
	}

	private void GenerateMatrix(PolyVec[] a, byte[] seed, bool transposed)
	{
		int k = m_engine.K;
		byte[] array = new byte[GenerateMatrixNBlocks * m_symmetric.XofBlockBytes + 2];
		for (int i = 0; i < k; i++)
		{
			for (int j = 0; j < k; j++)
			{
				if (transposed)
				{
					m_symmetric.XofAbsorb(seed, (byte)i, (byte)j);
				}
				else
				{
					m_symmetric.XofAbsorb(seed, (byte)j, (byte)i);
				}
				m_symmetric.XofSqueezeBlocks(array, 0, GenerateMatrixNBlocks * m_symmetric.XofBlockBytes);
				int num = GenerateMatrixNBlocks * m_symmetric.XofBlockBytes;
				for (int l = RejectionSampling(a[i].m_vec[j].m_coeffs, 0, 256, array, num); l < 256; l += RejectionSampling(a[i].m_vec[j].m_coeffs, l, 256 - l, array, num))
				{
					int num2 = num % 3;
					for (int m = 0; m < num2; m++)
					{
						array[m] = array[num - num2 + m];
					}
					m_symmetric.XofSqueezeBlocks(array, num2, m_symmetric.XofBlockBytes * 2);
					num = num2 + m_symmetric.XofBlockBytes;
				}
			}
		}
	}

	private int RejectionSampling(short[] r, int off, int len, byte[] buf, int buflen)
	{
		int num = 0;
		int num2 = 0;
		while (num < len && num2 + 3 <= buflen)
		{
			ushort num3 = (ushort)(((ushort)(buf[num2] & 0xFF) | ((ushort)(buf[num2 + 1] & 0xFF) << 8)) & 0xFFF);
			ushort num4 = (ushort)((((ushort)(buf[num2 + 1] & 0xFF) >> 4) | ((ushort)(buf[num2 + 2] & 0xFF) << 4)) & 0xFFF);
			num2 += 3;
			if (num3 < 3329)
			{
				r[off + num++] = (short)num3;
			}
			if (num < len && num4 < 3329)
			{
				r[off + num++] = (short)num4;
			}
		}
		return num;
	}

	internal void GenerateKeyPair(out byte[] pk, out byte[] sk)
	{
		int k = m_engine.K;
		byte[] array = new byte[2 * KyberEngine.SymBytes];
		byte b = 0;
		PolyVec[] array2 = new PolyVec[k];
		PolyVec polyVec = new PolyVec(m_engine);
		PolyVec polyVec2 = new PolyVec(m_engine);
		PolyVec polyVec3 = new PolyVec(m_engine);
		byte[] array3 = new byte[32];
		m_engine.RandomBytes(array3, 32);
		m_symmetric.Hash_g(array, array3);
		byte[] seed = Arrays.CopyOfRange(array, 0, KyberEngine.SymBytes);
		byte[] seed2 = Arrays.CopyOfRange(array, KyberEngine.SymBytes, 2 * KyberEngine.SymBytes);
		for (int i = 0; i < k; i++)
		{
			array2[i] = new PolyVec(m_engine);
		}
		GenerateMatrix(array2, seed, transposed: false);
		for (int j = 0; j < k; j++)
		{
			polyVec3.m_vec[j].GetNoiseEta1(seed2, b++);
		}
		for (int l = 0; l < k; l++)
		{
			polyVec.m_vec[l].GetNoiseEta1(seed2, b++);
		}
		polyVec3.Ntt();
		polyVec.Ntt();
		for (int m = 0; m < k; m++)
		{
			PolyVec.PointwiseAccountMontgomery(polyVec2.m_vec[m], array2[m], polyVec3, m_engine);
			polyVec2.m_vec[m].ToMont();
		}
		polyVec2.Add(polyVec);
		polyVec2.Reduce();
		PackSecretKey(out sk, polyVec3);
		PackPublicKey(out pk, polyVec2, seed);
	}

	private void PackSecretKey(out byte[] sk, PolyVec skpv)
	{
		sk = new byte[m_engine.PolyVecBytes];
		skpv.ToBytes(sk);
	}

	private void UnpackSecretKey(PolyVec skpv, byte[] sk)
	{
		skpv.FromBytes(sk);
	}

	private void PackPublicKey(out byte[] pk, PolyVec pkpv, byte[] seed)
	{
		pk = new byte[m_engine.IndCpaPublicKeyBytes];
		pkpv.ToBytes(pk);
		Array.Copy(seed, 0, pk, m_engine.PolyVecBytes, KyberEngine.SymBytes);
	}

	private void UnpackPublicKey(PolyVec pkpv, byte[] seed, byte[] pk)
	{
		pkpv.FromBytes(pk);
		Array.Copy(pk, m_engine.PolyVecBytes, seed, 0, KyberEngine.SymBytes);
	}

	public void Encrypt(byte[] c, byte[] m, byte[] pk, byte[] coins)
	{
		int k = m_engine.K;
		byte[] seed = new byte[KyberEngine.SymBytes];
		byte b = 0;
		PolyVec polyVec = new PolyVec(m_engine);
		PolyVec polyVec2 = new PolyVec(m_engine);
		PolyVec polyVec3 = new PolyVec(m_engine);
		PolyVec polyVec4 = new PolyVec(m_engine);
		PolyVec[] array = new PolyVec[k];
		Poly poly = new Poly(m_engine);
		Poly poly2 = new Poly(m_engine);
		Poly poly3 = new Poly(m_engine);
		UnpackPublicKey(polyVec2, seed, pk);
		poly2.FromMsg(m);
		for (int i = 0; i < k; i++)
		{
			array[i] = new PolyVec(m_engine);
		}
		GenerateMatrix(array, seed, transposed: true);
		for (int j = 0; j < k; j++)
		{
			polyVec.m_vec[j].GetNoiseEta1(coins, b++);
		}
		for (int l = 0; l < k; l++)
		{
			polyVec3.m_vec[l].GetNoiseEta2(coins, b++);
		}
		poly3.GetNoiseEta2(coins, b++);
		polyVec.Ntt();
		for (int n = 0; n < k; n++)
		{
			PolyVec.PointwiseAccountMontgomery(polyVec4.m_vec[n], array[n], polyVec, m_engine);
		}
		PolyVec.PointwiseAccountMontgomery(poly, polyVec2, polyVec, m_engine);
		polyVec4.InverseNttToMont();
		poly.PolyInverseNttToMont();
		polyVec4.Add(polyVec3);
		poly.Add(poly3);
		poly.Add(poly2);
		polyVec4.Reduce();
		poly.PolyReduce();
		PackCipherText(c, polyVec4, poly);
	}

	private void PackCipherText(byte[] r, PolyVec b, Poly v)
	{
		b.CompressPolyVec(r);
		v.CompressPoly(r, m_engine.PolyVecCompressedBytes);
	}

	private void UnpackCipherText(PolyVec b, Poly v, byte[] c)
	{
		b.DecompressPolyVec(c);
		v.DecompressPoly(c, m_engine.PolyVecCompressedBytes);
	}

	internal void Decrypt(byte[] m, byte[] c, byte[] sk)
	{
		PolyVec polyVec = new PolyVec(m_engine);
		PolyVec polyVec2 = new PolyVec(m_engine);
		Poly poly = new Poly(m_engine);
		Poly poly2 = new Poly(m_engine);
		UnpackCipherText(polyVec, poly, c);
		UnpackSecretKey(polyVec2, sk);
		polyVec.Ntt();
		PolyVec.PointwiseAccountMontgomery(poly2, polyVec2, polyVec, m_engine);
		poly2.PolyInverseNttToMont();
		poly2.Subtract(poly);
		poly2.PolyReduce();
		poly2.ToMsg(m);
	}
}
