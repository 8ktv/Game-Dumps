using System;
using Mirror.BouncyCastle.Crypto.Digests;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

internal class Poly
{
	private int N;

	private DilithiumEngine Engine;

	private int PolyUniformNBlocks;

	private Symmetric Symmetric;

	public int[] Coeffs { get; set; }

	public Poly(DilithiumEngine engine)
	{
		N = 256;
		Coeffs = new int[N];
		Engine = engine;
		Symmetric = engine.Symmetric;
		PolyUniformNBlocks = (768 + Symmetric.Stream128BlockBytes - 1) / Symmetric.Stream128BlockBytes;
	}

	public void UniformBlocks(byte[] seed, ushort nonce)
	{
		int num = PolyUniformNBlocks * Symmetric.Stream128BlockBytes;
		byte[] array = new byte[num + 2];
		Symmetric.Stream128Init(seed, nonce);
		Symmetric.Stream128SqueezeBlocks(array, 0, num);
		for (int i = RejectUniform(Coeffs, 0, N, array, num); i < N; i += RejectUniform(Coeffs, i, N - i, array, num))
		{
			int num2 = num % 3;
			for (int j = 0; j < num2; j++)
			{
				array[j] = array[num - num2 + j];
			}
			Symmetric.Stream128SqueezeBlocks(array, num2, Symmetric.Stream128BlockBytes);
			num = Symmetric.Stream128BlockBytes + num2;
		}
	}

	private static int RejectUniform(int[] coeffs, int off, int len, byte[] buf, int buflen)
	{
		int num2;
		int num = (num2 = 0);
		while (num < len && num2 + 3 <= buflen)
		{
			uint num3 = (uint)(buf[num2++] & 0xFF);
			num3 |= (uint)((buf[num2++] & 0xFF) << 8);
			num3 |= (uint)((buf[num2++] & 0xFF) << 16);
			num3 &= 0x7FFFFF;
			if (num3 < 8380417)
			{
				coeffs[off + num++] = (int)num3;
			}
		}
		return num;
	}

	public void UniformEta(byte[] seed, ushort nonce)
	{
		int eta = Engine.Eta;
		int num;
		if (Engine.Eta == 2)
		{
			num = (136 + Symmetric.Stream256BlockBytes - 1) / Symmetric.Stream256BlockBytes;
		}
		else
		{
			if (Engine.Eta != 4)
			{
				throw new ArgumentException("Wrong Dilithium Eta!");
			}
			num = (227 + Symmetric.Stream256BlockBytes - 1) / Symmetric.Stream256BlockBytes;
		}
		int num2 = num * Symmetric.Stream256BlockBytes;
		byte[] array = new byte[num2];
		Symmetric.Stream256Init(seed, nonce);
		Symmetric.Stream256SqueezeBlocks(array, 0, num2);
		for (int i = RejectEta(Coeffs, 0, N, array, num2, eta); i < 256; i += RejectEta(Coeffs, i, N - i, array, Symmetric.Stream256BlockBytes, eta))
		{
			Symmetric.Stream256SqueezeBlocks(array, 0, Symmetric.Stream256BlockBytes);
		}
	}

	private static int RejectEta(int[] coeffs, int off, int len, byte[] buf, int buflen, int eta)
	{
		int num2;
		int num = (num2 = 0);
		while (num < len && num2 < buflen)
		{
			uint num3 = (uint)(buf[num2] & 0xFF & 0xF);
			uint num4 = (uint)(buf[num2++] & 0xFF) >> 4;
			switch (eta)
			{
			case 2:
				if (num3 < 15)
				{
					num3 -= (205 * num3 >> 10) * 5;
					coeffs[off + num++] = (int)(2 - num3);
				}
				if (num4 < 15 && num < len)
				{
					num4 -= (205 * num4 >> 10) * 5;
					coeffs[off + num++] = (int)(2 - num4);
				}
				break;
			case 4:
				if (num3 < 9)
				{
					coeffs[off + num++] = (int)(4 - num3);
				}
				if (num4 < 9 && num < len)
				{
					coeffs[off + num++] = (int)(4 - num4);
				}
				break;
			}
		}
		return num;
	}

	public void PointwiseMontgomery(Poly v, Poly w)
	{
		for (int i = 0; i < N; i++)
		{
			Coeffs[i] = Reduce.MontgomeryReduce((long)v.Coeffs[i] * (long)w.Coeffs[i]);
		}
	}

	public void PointwiseAccountMontgomery(PolyVecL u, PolyVecL v)
	{
		Poly poly = new Poly(Engine);
		PointwiseMontgomery(u.Vec[0], v.Vec[0]);
		for (int i = 1; i < Engine.L; i++)
		{
			poly.PointwiseMontgomery(u.Vec[i], v.Vec[i]);
			AddPoly(poly);
		}
	}

	public void AddPoly(Poly a)
	{
		for (int i = 0; i < N; i++)
		{
			Coeffs[i] += a.Coeffs[i];
		}
	}

	public void Subtract(Poly b)
	{
		for (int i = 0; i < N; i++)
		{
			Coeffs[i] -= b.Coeffs[i];
		}
	}

	public void ReducePoly()
	{
		for (int i = 0; i < N; i++)
		{
			Coeffs[i] = Reduce.Reduce32(Coeffs[i]);
		}
	}

	public void PolyNtt()
	{
		Ntt.NTT(Coeffs);
	}

	public void InverseNttToMont()
	{
		Ntt.InverseNttToMont(Coeffs);
	}

	public void ConditionalAddQ()
	{
		for (int i = 0; i < N; i++)
		{
			Coeffs[i] = Reduce.ConditionalAddQ(Coeffs[i]);
		}
	}

	public void Power2Round(Poly a)
	{
		for (int i = 0; i < N; i++)
		{
			int[] array = Rounding.Power2Round(Coeffs[i]);
			Coeffs[i] = array[0];
			a.Coeffs[i] = array[1];
		}
	}

	public void PolyT0Pack(byte[] r, int off)
	{
		int[] array = new int[8];
		for (int i = 0; i < N / 8; i++)
		{
			array[0] = 4096 - Coeffs[8 * i];
			array[1] = 4096 - Coeffs[8 * i + 1];
			array[2] = 4096 - Coeffs[8 * i + 2];
			array[3] = 4096 - Coeffs[8 * i + 3];
			array[4] = 4096 - Coeffs[8 * i + 4];
			array[5] = 4096 - Coeffs[8 * i + 5];
			array[6] = 4096 - Coeffs[8 * i + 6];
			array[7] = 4096 - Coeffs[8 * i + 7];
			r[off + 13 * i] = (byte)array[0];
			r[off + 13 * i + 1] = (byte)(array[0] >> 8);
			r[off + 13 * i + 1] = (byte)(r[off + 13 * i + 1] | (byte)(array[1] << 5));
			r[off + 13 * i + 2] = (byte)(array[1] >> 3);
			r[off + 13 * i + 3] = (byte)(array[1] >> 11);
			r[off + 13 * i + 3] = (byte)(r[off + 13 * i + 3] | (byte)(array[2] << 2));
			r[off + 13 * i + 4] = (byte)(array[2] >> 6);
			r[off + 13 * i + 4] = (byte)(r[off + 13 * i + 4] | (byte)(array[3] << 7));
			r[off + 13 * i + 5] = (byte)(array[3] >> 1);
			r[off + 13 * i + 6] = (byte)(array[3] >> 9);
			r[off + 13 * i + 6] = (byte)(r[off + 13 * i + 6] | (byte)(array[4] << 4));
			r[off + 13 * i + 7] = (byte)(array[4] >> 4);
			r[off + 13 * i + 8] = (byte)(array[4] >> 12);
			r[off + 13 * i + 8] = (byte)(r[off + 13 * i + 8] | (byte)(array[5] << 1));
			r[off + 13 * i + 9] = (byte)(array[5] >> 7);
			r[off + 13 * i + 9] = (byte)(r[off + 13 * i + 9] | (byte)(array[6] << 6));
			r[off + 13 * i + 10] = (byte)(array[6] >> 2);
			r[off + 13 * i + 11] = (byte)(array[6] >> 10);
			r[off + 13 * i + 11] = (byte)(r[off + 13 * i + 11] | (byte)(array[7] << 3));
			r[off + 13 * i + 12] = (byte)(array[7] >> 5);
		}
	}

	public void PolyT0Unpack(byte[] a, int off)
	{
		for (int i = 0; i < N / 8; i++)
		{
			Coeffs[8 * i] = ((a[off + 13 * i] & 0xFF) | ((a[off + 13 * i + 1] & 0xFF) << 8)) & 0x1FFF;
			Coeffs[8 * i + 1] = (((a[off + 13 * i + 1] & 0xFF) >> 5) | ((a[off + 13 * i + 2] & 0xFF) << 3) | ((a[off + 13 * i + 3] & 0xFF) << 11)) & 0x1FFF;
			Coeffs[8 * i + 2] = (((a[off + 13 * i + 3] & 0xFF) >> 2) | ((a[off + 13 * i + 4] & 0xFF) << 6)) & 0x1FFF;
			Coeffs[8 * i + 3] = (((a[off + 13 * i + 4] & 0xFF) >> 7) | ((a[off + 13 * i + 5] & 0xFF) << 1) | ((a[off + 13 * i + 6] & 0xFF) << 9)) & 0x1FFF;
			Coeffs[8 * i + 4] = (((a[off + 13 * i + 6] & 0xFF) >> 4) | ((a[off + 13 * i + 7] & 0xFF) << 4) | ((a[off + 13 * i + 8] & 0xFF) << 12)) & 0x1FFF;
			Coeffs[8 * i + 5] = (((a[off + 13 * i + 8] & 0xFF) >> 1) | ((a[off + 13 * i + 9] & 0xFF) << 7)) & 0x1FFF;
			Coeffs[8 * i + 6] = (((a[off + 13 * i + 9] & 0xFF) >> 6) | ((a[off + 13 * i + 10] & 0xFF) << 2) | ((a[off + 13 * i + 11] & 0xFF) << 10)) & 0x1FFF;
			Coeffs[8 * i + 7] = (((a[off + 13 * i + 11] & 0xFF) >> 3) | ((a[off + 13 * i + 12] & 0xFF) << 5)) & 0x1FFF;
			Coeffs[8 * i] = 4096 - Coeffs[8 * i];
			Coeffs[8 * i + 1] = 4096 - Coeffs[8 * i + 1];
			Coeffs[8 * i + 2] = 4096 - Coeffs[8 * i + 2];
			Coeffs[8 * i + 3] = 4096 - Coeffs[8 * i + 3];
			Coeffs[8 * i + 4] = 4096 - Coeffs[8 * i + 4];
			Coeffs[8 * i + 5] = 4096 - Coeffs[8 * i + 5];
			Coeffs[8 * i + 6] = 4096 - Coeffs[8 * i + 6];
			Coeffs[8 * i + 7] = 4096 - Coeffs[8 * i + 7];
		}
	}

	public byte[] PolyT1Pack()
	{
		byte[] array = new byte[320];
		for (int i = 0; i < N / 4; i++)
		{
			array[5 * i] = (byte)Coeffs[4 * i];
			array[5 * i + 1] = (byte)((Coeffs[4 * i] >> 8) | (Coeffs[4 * i + 1] << 2));
			array[5 * i + 2] = (byte)((Coeffs[4 * i + 1] >> 6) | (Coeffs[4 * i + 2] << 4));
			array[5 * i + 3] = (byte)((Coeffs[4 * i + 2] >> 4) | (Coeffs[4 * i + 3] << 6));
			array[5 * i + 4] = (byte)(Coeffs[4 * i + 3] >> 2);
		}
		return array;
	}

	public void PolyT1Unpack(byte[] a)
	{
		for (int i = 0; i < N / 4; i++)
		{
			Coeffs[4 * i] = ((a[5 * i] & 0xFF) | ((a[5 * i + 1] & 0xFF) << 8)) & 0x3FF;
			Coeffs[4 * i + 1] = (((a[5 * i + 1] & 0xFF) >> 2) | ((a[5 * i + 2] & 0xFF) << 6)) & 0x3FF;
			Coeffs[4 * i + 2] = (((a[5 * i + 2] & 0xFF) >> 4) | ((a[5 * i + 3] & 0xFF) << 4)) & 0x3FF;
			Coeffs[4 * i + 3] = (((a[5 * i + 3] & 0xFF) >> 6) | ((a[5 * i + 4] & 0xFF) << 2)) & 0x3FF;
		}
	}

	public void PolyEtaPack(byte[] r, int off)
	{
		byte[] array = new byte[8];
		if (Engine.Eta == 2)
		{
			for (int i = 0; i < N / 8; i++)
			{
				array[0] = (byte)(Engine.Eta - Coeffs[8 * i]);
				array[1] = (byte)(Engine.Eta - Coeffs[8 * i + 1]);
				array[2] = (byte)(Engine.Eta - Coeffs[8 * i + 2]);
				array[3] = (byte)(Engine.Eta - Coeffs[8 * i + 3]);
				array[4] = (byte)(Engine.Eta - Coeffs[8 * i + 4]);
				array[5] = (byte)(Engine.Eta - Coeffs[8 * i + 5]);
				array[6] = (byte)(Engine.Eta - Coeffs[8 * i + 6]);
				array[7] = (byte)(Engine.Eta - Coeffs[8 * i + 7]);
				r[off + 3 * i] = (byte)(array[0] | (array[1] << 3) | (array[2] << 6));
				r[off + 3 * i + 1] = (byte)((array[2] >> 2) | (array[3] << 1) | (array[4] << 4) | (array[5] << 7));
				r[off + 3 * i + 2] = (byte)((array[5] >> 1) | (array[6] << 2) | (array[7] << 5));
			}
		}
		else
		{
			if (Engine.Eta != 4)
			{
				throw new ArgumentException("Eta needs to be 2 or 4!");
			}
			for (int i = 0; i < N / 2; i++)
			{
				array[0] = (byte)(Engine.Eta - Coeffs[2 * i]);
				array[1] = (byte)(Engine.Eta - Coeffs[2 * i + 1]);
				r[off + i] = (byte)(array[0] | (array[1] << 4));
			}
		}
	}

	public void PolyEtaUnpack(byte[] a, int off)
	{
		int eta = Engine.Eta;
		switch (eta)
		{
		case 2:
		{
			for (int i = 0; i < N / 8; i++)
			{
				Coeffs[8 * i] = a[off + 3 * i] & 0xFF & 7;
				Coeffs[8 * i + 1] = ((a[off + 3 * i] & 0xFF) >> 3) & 7;
				Coeffs[8 * i + 2] = ((a[off + 3 * i] & 0xFF) >> 6) | (((a[off + 3 * i + 1] & 0xFF) << 2) & 7);
				Coeffs[8 * i + 3] = ((a[off + 3 * i + 1] & 0xFF) >> 1) & 7;
				Coeffs[8 * i + 4] = ((a[off + 3 * i + 1] & 0xFF) >> 4) & 7;
				Coeffs[8 * i + 5] = ((a[off + 3 * i + 1] & 0xFF) >> 7) | (((a[off + 3 * i + 2] & 0xFF) << 1) & 7);
				Coeffs[8 * i + 6] = ((a[off + 3 * i + 2] & 0xFF) >> 2) & 7;
				Coeffs[8 * i + 7] = ((a[off + 3 * i + 2] & 0xFF) >> 5) & 7;
				Coeffs[8 * i] = eta - Coeffs[8 * i];
				Coeffs[8 * i + 1] = eta - Coeffs[8 * i + 1];
				Coeffs[8 * i + 2] = eta - Coeffs[8 * i + 2];
				Coeffs[8 * i + 3] = eta - Coeffs[8 * i + 3];
				Coeffs[8 * i + 4] = eta - Coeffs[8 * i + 4];
				Coeffs[8 * i + 5] = eta - Coeffs[8 * i + 5];
				Coeffs[8 * i + 6] = eta - Coeffs[8 * i + 6];
				Coeffs[8 * i + 7] = eta - Coeffs[8 * i + 7];
			}
			break;
		}
		case 4:
		{
			for (int i = 0; i < N / 2; i++)
			{
				Coeffs[2 * i] = a[off + i] & 0xFF & 0xF;
				Coeffs[2 * i + 1] = (a[off + i] & 0xFF) >> 4;
				Coeffs[2 * i] = eta - Coeffs[2 * i];
				Coeffs[2 * i + 1] = eta - Coeffs[2 * i + 1];
			}
			break;
		}
		}
	}

	public void UniformGamma1(byte[] seed, ushort nonce)
	{
		byte[] array = new byte[Engine.PolyUniformGamma1NBytes * Symmetric.Stream256BlockBytes];
		Symmetric.Stream256Init(seed, nonce);
		Symmetric.Stream256SqueezeBlocks(array, 0, array.Length);
		UnpackZ(array);
	}

	public void PackZ(byte[] r, int offset)
	{
		uint[] array = new uint[4];
		if (Engine.Gamma1 == 131072)
		{
			for (int i = 0; i < N / 4; i++)
			{
				array[0] = (uint)(Engine.Gamma1 - Coeffs[4 * i]);
				array[1] = (uint)(Engine.Gamma1 - Coeffs[4 * i + 1]);
				array[2] = (uint)(Engine.Gamma1 - Coeffs[4 * i + 2]);
				array[3] = (uint)(Engine.Gamma1 - Coeffs[4 * i + 3]);
				r[offset + 9 * i] = (byte)array[0];
				r[offset + 9 * i + 1] = (byte)(array[0] >> 8);
				r[offset + 9 * i + 2] = (byte)((byte)(array[0] >> 16) | (array[1] << 2));
				r[offset + 9 * i + 3] = (byte)(array[1] >> 6);
				r[offset + 9 * i + 4] = (byte)((byte)(array[1] >> 14) | (array[2] << 4));
				r[offset + 9 * i + 5] = (byte)(array[2] >> 4);
				r[offset + 9 * i + 6] = (byte)((byte)(array[2] >> 12) | (array[3] << 6));
				r[offset + 9 * i + 7] = (byte)(array[3] >> 2);
				r[offset + 9 * i + 8] = (byte)(array[3] >> 10);
			}
		}
		else
		{
			if (Engine.Gamma1 != 524288)
			{
				throw new ArgumentException("Wrong Dilithium Gamma1!");
			}
			for (int i = 0; i < N / 2; i++)
			{
				array[0] = (uint)(Engine.Gamma1 - Coeffs[2 * i]);
				array[1] = (uint)(Engine.Gamma1 - Coeffs[2 * i + 1]);
				r[offset + 5 * i] = (byte)array[0];
				r[offset + 5 * i + 1] = (byte)(array[0] >> 8);
				r[offset + 5 * i + 2] = (byte)((byte)(array[0] >> 16) | (array[1] << 4));
				r[offset + 5 * i + 3] = (byte)(array[1] >> 4);
				r[offset + 5 * i + 4] = (byte)(array[1] >> 12);
			}
		}
	}

	public void UnpackZ(byte[] a)
	{
		if (Engine.Gamma1 == 131072)
		{
			for (int i = 0; i < N / 4; i++)
			{
				Coeffs[4 * i] = ((a[9 * i] & 0xFF) | ((a[9 * i + 1] & 0xFF) << 8) | ((a[9 * i + 2] & 0xFF) << 16)) & 0x3FFFF;
				Coeffs[4 * i + 1] = (((a[9 * i + 2] & 0xFF) >> 2) | ((a[9 * i + 3] & 0xFF) << 6) | ((a[9 * i + 4] & 0xFF) << 14)) & 0x3FFFF;
				Coeffs[4 * i + 2] = (((a[9 * i + 4] & 0xFF) >> 4) | ((a[9 * i + 5] & 0xFF) << 4) | ((a[9 * i + 6] & 0xFF) << 12)) & 0x3FFFF;
				Coeffs[4 * i + 3] = (((a[9 * i + 6] & 0xFF) >> 6) | ((a[9 * i + 7] & 0xFF) << 2) | ((a[9 * i + 8] & 0xFF) << 10)) & 0x3FFFF;
				Coeffs[4 * i] = Engine.Gamma1 - Coeffs[4 * i];
				Coeffs[4 * i + 1] = Engine.Gamma1 - Coeffs[4 * i + 1];
				Coeffs[4 * i + 2] = Engine.Gamma1 - Coeffs[4 * i + 2];
				Coeffs[4 * i + 3] = Engine.Gamma1 - Coeffs[4 * i + 3];
			}
		}
		else
		{
			if (Engine.Gamma1 != 524288)
			{
				throw new ArgumentException("Wrong Dilithiumn Gamma1!");
			}
			for (int i = 0; i < N / 2; i++)
			{
				Coeffs[2 * i] = ((a[5 * i] & 0xFF) | ((a[5 * i + 1] & 0xFF) << 8) | ((a[5 * i + 2] & 0xFF) << 16)) & 0xFFFFF;
				Coeffs[2 * i + 1] = (((a[5 * i + 2] & 0xFF) >> 4) | ((a[5 * i + 3] & 0xFF) << 4) | ((a[5 * i + 4] & 0xFF) << 12)) & 0xFFFFF;
				Coeffs[2 * i] = Engine.Gamma1 - Coeffs[2 * i];
				Coeffs[2 * i + 1] = Engine.Gamma1 - Coeffs[2 * i + 1];
			}
		}
	}

	public void Decompose(Poly a)
	{
		for (int i = 0; i < N; i++)
		{
			int[] array = Rounding.Decompose(Coeffs[i], Engine.Gamma2);
			a.Coeffs[i] = array[0];
			Coeffs[i] = array[1];
		}
	}

	public void PackW1(byte[] r, int off)
	{
		if (Engine.Gamma2 == 95232)
		{
			for (int i = 0; i < N / 4; i++)
			{
				r[off + 3 * i] = (byte)((byte)Coeffs[4 * i] | (Coeffs[4 * i + 1] << 6));
				r[off + 3 * i + 1] = (byte)((byte)(Coeffs[4 * i + 1] >> 2) | (Coeffs[4 * i + 2] << 4));
				r[off + 3 * i + 2] = (byte)((byte)(Coeffs[4 * i + 2] >> 4) | (Coeffs[4 * i + 3] << 2));
			}
		}
		else if (Engine.Gamma2 == 261888)
		{
			for (int i = 0; i < N / 2; i++)
			{
				r[off + i] = (byte)(Coeffs[2 * i] | (Coeffs[2 * i + 1] << 4));
			}
		}
	}

	public void Challenge(byte[] seed)
	{
		byte[] array = new byte[Symmetric.Stream256BlockBytes];
		ShakeDigest shakeDigest = new ShakeDigest(256);
		shakeDigest.BlockUpdate(seed, 0, 32);
		shakeDigest.Output(array, 0, Symmetric.Stream256BlockBytes);
		ulong num = 0uL;
		for (int i = 0; i < 8; i++)
		{
			num |= (ulong)((long)(array[i] & 0xFF) << 8 * i);
		}
		int num2 = 8;
		for (int i = 0; i < N; i++)
		{
			Coeffs[i] = 0;
		}
		for (int i = N - Engine.Tau; i < N; i++)
		{
			int num3;
			do
			{
				if (num2 >= Symmetric.Stream256BlockBytes)
				{
					shakeDigest.Output(array, 0, Symmetric.Stream256BlockBytes);
					num2 = 0;
				}
				num3 = array[num2++] & 0xFF;
			}
			while (num3 > i);
			Coeffs[i] = Coeffs[num3];
			Coeffs[num3] = (int)(1 - 2 * (num & 1));
			num >>= 1;
		}
	}

	public bool CheckNorm(int B)
	{
		if (B > 1047552)
		{
			return true;
		}
		for (int i = 0; i < N; i++)
		{
			int num = Coeffs[i] >> 31;
			num = Coeffs[i] - (num & (2 * Coeffs[i]));
			if (num >= B)
			{
				return true;
			}
		}
		return false;
	}

	public int PolyMakeHint(Poly a0, Poly a1)
	{
		int num = 0;
		for (int i = 0; i < N; i++)
		{
			Coeffs[i] = Rounding.MakeHint(a0.Coeffs[i], a1.Coeffs[i], Engine);
			num += Coeffs[i];
		}
		return num;
	}

	public void PolyUseHint(Poly a, Poly h)
	{
		for (int i = 0; i < 256; i++)
		{
			Coeffs[i] = Rounding.UseHint(a.Coeffs[i], h.Coeffs[i], Engine.Gamma2);
		}
	}

	public void ShiftLeft()
	{
		for (int i = 0; i < N; i++)
		{
			Coeffs[i] <<= 13;
		}
	}
}
