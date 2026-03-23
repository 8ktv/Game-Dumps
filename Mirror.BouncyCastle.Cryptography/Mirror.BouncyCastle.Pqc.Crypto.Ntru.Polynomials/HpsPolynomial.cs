using System;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru.Polynomials;

internal class HpsPolynomial : Polynomial
{
	internal HpsPolynomial(NtruParameterSet parameterSet)
		: base(parameterSet)
	{
	}

	public override byte[] SqToBytes(int len)
	{
		byte[] array = new byte[len];
		short[] array2 = new short[8];
		int i;
		int j;
		for (i = 0; i < ParameterSet.PackDegree() / 8; i++)
		{
			for (j = 0; j < 8; j++)
			{
				array2[j] = (short)Polynomial.ModQ((uint)(coeffs[8 * i + j] & 0xFFFF), (uint)ParameterSet.Q());
			}
			array[11 * i] = (byte)(array2[0] & 0xFF);
			array[11 * i + 1] = (byte)((array2[0] >> 8) | ((array2[1] & 0x1F) << 3));
			array[11 * i + 2] = (byte)((array2[1] >> 5) | ((array2[2] & 3) << 6));
			array[11 * i + 3] = (byte)((array2[2] >> 2) & 0xFF);
			array[11 * i + 4] = (byte)((array2[2] >> 10) | ((array2[3] & 0x7F) << 1));
			array[11 * i + 5] = (byte)((array2[3] >> 7) | ((array2[4] & 0xF) << 4));
			array[11 * i + 6] = (byte)((array2[4] >> 4) | ((array2[5] & 1) << 7));
			array[11 * i + 7] = (byte)((array2[5] >> 1) & 0xFF);
			array[11 * i + 8] = (byte)((array2[5] >> 9) | ((array2[6] & 0x3F) << 2));
			array[11 * i + 9] = (byte)((array2[6] >> 6) | ((array2[7] & 7) << 5));
			array[11 * i + 10] = (byte)(array2[7] >> 3);
		}
		for (j = 0; j < ParameterSet.PackDegree() - 8 * i; j++)
		{
			array2[j] = (short)Polynomial.ModQ((uint)(coeffs[8 * i + j] & 0xFFFF), (uint)ParameterSet.Q());
		}
		for (; j < 8; j++)
		{
			array2[j] = 0;
		}
		switch (ParameterSet.PackDegree() & 7)
		{
		case 4:
			array[11 * i] = (byte)(array2[0] & 0xFF);
			array[11 * i + 1] = (byte)((array2[0] >> 8) | ((array2[1] & 0x1F) << 3));
			array[11 * i + 2] = (byte)((array2[1] >> 5) | ((array2[2] & 3) << 6));
			array[11 * i + 3] = (byte)((array2[2] >> 2) & 0xFF);
			array[11 * i + 4] = (byte)((array2[2] >> 10) | ((array2[3] & 0x7F) << 1));
			array[11 * i + 5] = (byte)((array2[3] >> 7) | ((array2[4] & 0xF) << 4));
			break;
		case 2:
			array[11 * i] = (byte)(array2[0] & 0xFF);
			array[11 * i + 1] = (byte)((array2[0] >> 8) | ((array2[1] & 0x1F) << 3));
			array[11 * i + 2] = (byte)((array2[1] >> 5) | ((array2[2] & 3) << 6));
			break;
		}
		return array;
	}

	public override void SqFromBytes(byte[] a)
	{
		int num = coeffs.Length;
		int i;
		for (i = 0; i < ParameterSet.PackDegree() / 8; i++)
		{
			coeffs[8 * i] = (ushort)((a[11 * i] & 0xFF) | (((ushort)(a[11 * i + 1] & 0xFF) & 7) << 8));
			coeffs[8 * i + 1] = (ushort)(((a[11 * i + 1] & 0xFF) >> 3) | (((ushort)(a[11 * i + 2] & 0xFF) & 0x3F) << 5));
			coeffs[8 * i + 2] = (ushort)(((a[11 * i + 2] & 0xFF) >> 6) | (((ushort)(a[11 * i + 3] & 0xFF) & 0xFF) << 2) | (((ushort)(a[11 * i + 4] & 0xFF) & 1) << 10));
			coeffs[8 * i + 3] = (ushort)(((a[11 * i + 4] & 0xFF) >> 1) | (((ushort)(a[11 * i + 5] & 0xFF) & 0xF) << 7));
			coeffs[8 * i + 4] = (ushort)(((a[11 * i + 5] & 0xFF) >> 4) | (((ushort)(a[11 * i + 6] & 0xFF) & 0x7F) << 4));
			coeffs[8 * i + 5] = (ushort)(((a[11 * i + 6] & 0xFF) >> 7) | (((ushort)(a[11 * i + 7] & 0xFF) & 0xFF) << 1) | (((ushort)(a[11 * i + 8] & 0xFF) & 3) << 9));
			coeffs[8 * i + 6] = (ushort)(((a[11 * i + 8] & 0xFF) >> 2) | (((ushort)(a[11 * i + 9] & 0xFF) & 0x1F) << 6));
			coeffs[8 * i + 7] = (ushort)(((a[11 * i + 9] & 0xFF) >> 5) | (((ushort)(a[11 * i + 10] & 0xFF) & 0xFF) << 3));
		}
		switch (ParameterSet.PackDegree() & 7)
		{
		case 4:
			coeffs[8 * i] = (ushort)((a[11 * i] & 0xFF) | (((ushort)(a[11 * i + 1] & 0xFF) & 7) << 8));
			coeffs[8 * i + 1] = (ushort)(((a[11 * i + 1] & 0xFF) >> 3) | (((ushort)(a[11 * i + 2] & 0xFF) & 0x3F) << 5));
			coeffs[8 * i + 2] = (ushort)(((a[11 * i + 2] & 0xFF) >> 6) | (((ushort)(a[11 * i + 3] & 0xFF) & 0xFF) << 2) | (((ushort)(a[11 * i + 4] & 0xFF) & 1) << 10));
			coeffs[8 * i + 3] = (ushort)(((a[11 * i + 4] & 0xFF) >> 1) | (((ushort)(a[11 * i + 5] & 0xFF) & 0xF) << 7));
			break;
		case 2:
			coeffs[8 * i] = (ushort)((a[11 * i] & 0xFF) | (((ushort)(a[11 * i + 1] & 0xFF) & 7) << 8));
			coeffs[8 * i + 1] = (ushort)(((a[11 * i + 1] & 0xFF) >> 3) | (((ushort)(a[11 * i + 2] & 0xFF) & 0x3F) << 5));
			break;
		}
		coeffs[num - 1] = 0;
	}

	public override void Lift(Polynomial a)
	{
		int length = coeffs.Length;
		Array.Copy(a.coeffs, 0, coeffs, 0, length);
		Z3ToZq();
	}

	public override void R2Inv(Polynomial a)
	{
		HpsPolynomial f = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial g = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial v = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial w = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		R2Inv(a, f, g, v, w);
	}

	public override void RqInv(Polynomial a)
	{
		HpsPolynomial ai = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial b = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial c = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial s = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		RqInv(a, ai, b, c, s);
	}

	public override void S3Inv(Polynomial a)
	{
		HpsPolynomial f = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial g = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial v = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		HpsPolynomial w = new HpsPolynomial((NtruHpsParameterSet)ParameterSet);
		S3Inv(a, f, g, v, w);
	}
}
