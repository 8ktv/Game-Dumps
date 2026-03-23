using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru.Polynomials;

internal class HrssPolynomial : Polynomial
{
	internal HrssPolynomial(NtruParameterSet parameterSet)
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
			array[13 * i] = (byte)(array2[0] & 0xFF);
			array[13 * i + 1] = (byte)((array2[0] >> 8) | ((array2[1] & 7) << 5));
			array[13 * i + 2] = (byte)((array2[1] >> 3) & 0xFF);
			array[13 * i + 3] = (byte)((array2[1] >> 11) | ((array2[2] & 0x3F) << 2));
			array[13 * i + 4] = (byte)((array2[2] >> 6) | ((array2[3] & 1) << 7));
			array[13 * i + 5] = (byte)((array2[3] >> 1) & 0xFF);
			array[13 * i + 6] = (byte)((array2[3] >> 9) | ((array2[4] & 0xF) << 4));
			array[13 * i + 7] = (byte)((array2[4] >> 4) & 0xFF);
			array[13 * i + 8] = (byte)((array2[4] >> 12) | ((array2[5] & 0x7F) << 1));
			array[13 * i + 9] = (byte)((array2[5] >> 7) | ((array2[6] & 3) << 6));
			array[13 * i + 10] = (byte)((array2[6] >> 2) & 0xFF);
			array[13 * i + 11] = (byte)((array2[6] >> 10) | ((array2[7] & 0x1F) << 3));
			array[13 * i + 12] = (byte)(array2[7] >> 5);
		}
		for (j = 0; j < ParameterSet.PackDegree() - 8 * i; j++)
		{
			array2[j] = (short)Polynomial.ModQ((uint)(coeffs[8 * i + j] & 0xFFFF), (uint)ParameterSet.Q());
		}
		for (; j < 8; j++)
		{
			array2[j] = 0;
		}
		switch (ParameterSet.PackDegree() - 8 * (ParameterSet.PackDegree() / 8))
		{
		case 4:
			array[13 * i] = (byte)(array2[0] & 0xFF);
			array[13 * i + 1] = (byte)((array2[0] >> 8) | ((array2[1] & 7) << 5));
			array[13 * i + 2] = (byte)((array2[1] >> 3) & 0xFF);
			array[13 * i + 3] = (byte)((array2[1] >> 11) | ((array2[2] & 0x3F) << 2));
			array[13 * i + 4] = (byte)((array2[2] >> 6) | ((array2[3] & 1) << 7));
			array[13 * i + 5] = (byte)((array2[3] >> 1) & 0xFF);
			array[13 * i + 6] = (byte)((array2[3] >> 9) | ((array2[4] & 0xF) << 4));
			break;
		case 2:
			array[13 * i] = (byte)(array2[0] & 0xFF);
			array[13 * i + 1] = (byte)((array2[0] >> 8) | ((array2[1] & 7) << 5));
			array[13 * i + 2] = (byte)((array2[1] >> 3) & 0xFF);
			array[13 * i + 3] = (byte)((array2[1] >> 11) | ((array2[2] & 0x3F) << 2));
			break;
		}
		return array;
	}

	public override void SqFromBytes(byte[] a)
	{
		int i;
		for (i = 0; i < ParameterSet.PackDegree() / 8; i++)
		{
			coeffs[8 * i] = (ushort)((a[13 * i] & 0xFF) | (((ushort)(a[13 * i + 1] & 0xFF) & 0x1F) << 8));
			coeffs[8 * i + 1] = (ushort)(((a[13 * i + 1] & 0xFF) >> 5) | ((ushort)(a[13 * i + 2] & 0xFF) << 3) | (((short)(a[13 * i + 3] & 0xFF) & 3) << 11));
			coeffs[8 * i + 2] = (ushort)(((a[13 * i + 3] & 0xFF) >> 2) | (((ushort)(a[13 * i + 4] & 0xFF) & 0x7F) << 6));
			coeffs[8 * i + 3] = (ushort)(((a[13 * i + 4] & 0xFF) >> 7) | ((ushort)(a[13 * i + 5] & 0xFF) << 1) | (((short)(a[13 * i + 6] & 0xFF) & 0xF) << 9));
			coeffs[8 * i + 4] = (ushort)(((a[13 * i + 6] & 0xFF) >> 4) | ((ushort)(a[13 * i + 7] & 0xFF) << 4) | (((short)(a[13 * i + 8] & 0xFF) & 1) << 12));
			coeffs[8 * i + 5] = (ushort)(((a[13 * i + 8] & 0xFF) >> 1) | (((ushort)(a[13 * i + 9] & 0xFF) & 0x3F) << 7));
			coeffs[8 * i + 6] = (ushort)(((a[13 * i + 9] & 0xFF) >> 6) | ((ushort)(a[13 * i + 10] & 0xFF) << 2) | (((short)(a[13 * i + 11] & 0xFF) & 7) << 10));
			coeffs[8 * i + 7] = (ushort)(((a[13 * i + 11] & 0xFF) >> 3) | ((ushort)(a[13 * i + 12] & 0xFF) << 5));
		}
		switch (ParameterSet.PackDegree() & 7)
		{
		case 4:
			coeffs[8 * i] = (ushort)((a[13 * i] & 0xFF) | (((short)(a[13 * i + 1] & 0xFF) & 0x1F) << 8));
			coeffs[8 * i + 1] = (ushort)(((a[13 * i + 1] & 0xFF) >> 5) | ((short)(a[13 * i + 2] & 0xFF) << 3) | (((short)(a[13 * i + 3] & 0xFF) & 3) << 11));
			coeffs[8 * i + 2] = (ushort)(((a[13 * i + 3] & 0xFF) >> 2) | (((short)(a[13 * i + 4] & 0xFF) & 0x7F) << 6));
			coeffs[8 * i + 3] = (ushort)(((a[13 * i + 4] & 0xFF) >> 7) | ((short)(a[13 * i + 5] & 0xFF) << 1) | (((short)(a[13 * i + 6] & 0xFF) & 0xF) << 9));
			break;
		case 2:
			coeffs[8 * i] = (ushort)((a[13 * i] & 0xFF) | (((short)(a[13 * i + 1] & 0xFF) & 0x1F) << 8));
			coeffs[8 * i + 1] = (ushort)(((a[13 * i + 1] & 0xFF) >> 5) | ((short)(a[13 * i + 2] & 0xFF) << 3) | (((short)(a[13 * i + 3] & 0xFF) & 3) << 11));
			break;
		}
		coeffs[ParameterSet.N - 1] = 0;
	}

	public override void Lift(Polynomial a)
	{
		int num = coeffs.Length;
		HrssPolynomial hrssPolynomial = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		ushort num2 = (ushort)(3 - num % 3);
		ushort[] array = hrssPolynomial.coeffs;
		int num3 = a.coeffs[0] * (2 - num2);
		_ = a.coeffs[1];
		array[0] = (ushort)(num3 + 0 + a.coeffs[2] * num2);
		ushort[] array2 = hrssPolynomial.coeffs;
		int num4 = a.coeffs[1] * (2 - num2);
		_ = a.coeffs[2];
		array2[1] = (ushort)(num4 + 0);
		hrssPolynomial.coeffs[2] = (ushort)(a.coeffs[2] * (2 - num2));
		ushort num5 = 0;
		for (int i = 3; i < num; i++)
		{
			hrssPolynomial.coeffs[0] += (ushort)(a.coeffs[i] * (num5 + 2 * num2));
			hrssPolynomial.coeffs[1] += (ushort)(a.coeffs[i] * (num5 + num2));
			hrssPolynomial.coeffs[2] += (ushort)(a.coeffs[i] * num5);
			num5 = (ushort)((num5 + num2) % 3);
		}
		hrssPolynomial.coeffs[1] += (ushort)(a.coeffs[0] * (num5 + num2));
		hrssPolynomial.coeffs[2] += (ushort)(a.coeffs[0] * num5);
		hrssPolynomial.coeffs[2] += (ushort)(a.coeffs[1] * (num5 + num2));
		for (int i = 3; i < num; i++)
		{
			hrssPolynomial.coeffs[i] = (ushort)(hrssPolynomial.coeffs[i - 3] + 2 * (a.coeffs[i] + a.coeffs[i - 1] + a.coeffs[i - 2]));
		}
		hrssPolynomial.Mod3PhiN();
		hrssPolynomial.Z3ToZq();
		coeffs[0] = (ushort)(-hrssPolynomial.coeffs[0]);
		for (int i = 0; i < num - 1; i++)
		{
			coeffs[i + 1] = (ushort)(hrssPolynomial.coeffs[i] - hrssPolynomial.coeffs[i + 1]);
		}
	}

	public override void R2Inv(Polynomial a)
	{
		HrssPolynomial f = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial g = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial v = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial w = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		R2Inv(a, f, g, v, w);
	}

	public override void RqInv(Polynomial a)
	{
		HrssPolynomial ai = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial b = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial c = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial s = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		RqInv(a, ai, b, c, s);
	}

	public override void S3Inv(Polynomial a)
	{
		HrssPolynomial f = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial g = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial v = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		HrssPolynomial w = new HrssPolynomial((NtruHrssParameterSet)ParameterSet);
		S3Inv(a, f, g, v, w);
	}
}
