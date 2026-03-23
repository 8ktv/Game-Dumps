using System;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.Polynomials;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru;

internal class NtruSampling
{
	private readonly NtruParameterSet _parameterSet;

	internal NtruSampling(NtruParameterSet parameterSet)
	{
		_parameterSet = parameterSet;
	}

	internal PolynomialPair SampleFg(byte[] uniformBytes)
	{
		NtruParameterSet parameterSet = _parameterSet;
		if (!(parameterSet is NtruHrssParameterSet))
		{
			if (parameterSet is NtruHpsParameterSet)
			{
				HpsPolynomial a = (HpsPolynomial)SampleIid(Arrays.CopyOfRange(uniformBytes, 0, _parameterSet.SampleIidBytes()));
				HpsPolynomial b = SampleFixedType(Arrays.CopyOfRange(uniformBytes, _parameterSet.SampleIidBytes(), uniformBytes.Length));
				return new PolynomialPair(a, b);
			}
			throw new ArgumentException("Invalid polynomial type");
		}
		HrssPolynomial a2 = SampleIidPlus(Arrays.CopyOfRange(uniformBytes, 0, _parameterSet.SampleIidBytes()));
		HrssPolynomial b2 = SampleIidPlus(Arrays.CopyOfRange(uniformBytes, _parameterSet.SampleIidBytes(), uniformBytes.Length));
		return new PolynomialPair(a2, b2);
	}

	internal PolynomialPair SampleRm(byte[] uniformBytes)
	{
		NtruParameterSet parameterSet = _parameterSet;
		if (!(parameterSet is NtruHrssParameterSet))
		{
			if (parameterSet is NtruHpsParameterSet)
			{
				HpsPolynomial a = (HpsPolynomial)SampleIid(Arrays.CopyOfRange(uniformBytes, 0, _parameterSet.SampleIidBytes()));
				HpsPolynomial b = SampleFixedType(Arrays.CopyOfRange(uniformBytes, _parameterSet.SampleIidBytes(), uniformBytes.Length));
				return new PolynomialPair(a, b);
			}
			throw new ArgumentException("Invalid polynomial type");
		}
		HrssPolynomial a2 = (HrssPolynomial)SampleIid(Arrays.CopyOfRange(uniformBytes, 0, _parameterSet.SampleIidBytes()));
		HrssPolynomial b2 = (HrssPolynomial)SampleIid(Arrays.CopyOfRange(uniformBytes, _parameterSet.SampleIidBytes(), uniformBytes.Length));
		return new PolynomialPair(a2, b2);
	}

	internal Polynomial SampleIid(byte[] uniformBytes)
	{
		Polynomial polynomial = _parameterSet.CreatePolynomial();
		for (int i = 0; i < _parameterSet.N - 1; i++)
		{
			polynomial.coeffs[i] = (ushort)Mod3(uniformBytes[i]);
		}
		polynomial.coeffs[_parameterSet.N - 1] = 0;
		return polynomial;
	}

	internal HpsPolynomial SampleFixedType(byte[] uniformBytes)
	{
		int n = _parameterSet.N;
		int num = ((NtruHpsParameterSet)_parameterSet).Weight();
		HpsPolynomial hpsPolynomial = (HpsPolynomial)_parameterSet.CreatePolynomial();
		int[] array = new int[n - 1];
		for (int i = 0; i < (n - 1) / 4; i++)
		{
			array[4 * i] = (uniformBytes[15 * i] << 2) + (uniformBytes[15 * i + 1] << 10) + (uniformBytes[15 * i + 2] << 18) + (uniformBytes[15 * i + 3] << 26);
			array[4 * i + 1] = ((uniformBytes[15 * i + 3] & 0xC0) >> 4) + (uniformBytes[15 * i + 4] << 4) + (uniformBytes[15 * i + 5] << 12) + (uniformBytes[15 * i + 6] << 20) + (uniformBytes[15 * i + 7] << 28);
			array[4 * i + 2] = ((uniformBytes[15 * i + 7] & 0xF0) >> 2) + (uniformBytes[15 * i + 8] << 6) + (uniformBytes[15 * i + 9] << 14) + (uniformBytes[15 * i + 10] << 22) + (uniformBytes[15 * i + 11] << 30);
			array[4 * i + 3] = (uniformBytes[15 * i + 11] & 0xFC) + (uniformBytes[15 * i + 12] << 8) + (uniformBytes[15 * i + 13] << 16) + (uniformBytes[15 * i + 14] << 24);
		}
		if (n - 1 > (n - 1) / 4 * 4)
		{
			int i = (n - 1) / 4;
			array[4 * i] = (uniformBytes[15 * i] << 2) + (uniformBytes[15 * i + 1] << 10) + (uniformBytes[15 * i + 2] << 18) + (uniformBytes[15 * i + 3] << 26);
			array[4 * i + 1] = ((uniformBytes[15 * i + 3] & 0xC0) >> 4) + (uniformBytes[15 * i + 4] << 4) + (uniformBytes[15 * i + 5] << 12) + (uniformBytes[15 * i + 6] << 20) + (uniformBytes[15 * i + 7] << 28);
		}
		for (int i = 0; i < num / 2; i++)
		{
			array[i] |= 1;
		}
		for (int i = num / 2; i < num; i++)
		{
			array[i] |= 2;
		}
		Array.Sort(array);
		for (int i = 0; i < n - 1; i++)
		{
			hpsPolynomial.coeffs[i] = (ushort)(array[i] & 3);
		}
		hpsPolynomial.coeffs[n - 1] = 0;
		return hpsPolynomial;
	}

	internal HrssPolynomial SampleIidPlus(byte[] uniformBytes)
	{
		int n = _parameterSet.N;
		ushort num = 0;
		HrssPolynomial hrssPolynomial = (HrssPolynomial)SampleIid(uniformBytes);
		for (int i = 0; i < n - 1; i++)
		{
			hrssPolynomial.coeffs[i] = (ushort)(hrssPolynomial.coeffs[i] | -(hrssPolynomial.coeffs[i] >> 1));
		}
		for (int i = 0; i < n - 1; i++)
		{
			num += (ushort)(hrssPolynomial.coeffs[i + 1] * hrssPolynomial.coeffs[i]);
		}
		num = (ushort)(1 | -(num >> 15));
		for (int i = 0; i < n - 1; i += 2)
		{
			hrssPolynomial.coeffs[i] = (ushort)(num * hrssPolynomial.coeffs[i]);
		}
		for (int i = 0; i < n - 1; i++)
		{
			hrssPolynomial.coeffs[i] = (ushort)(3 & (hrssPolynomial.coeffs[i] ^ (hrssPolynomial.coeffs[i] >> 15)));
		}
		return hrssPolynomial;
	}

	private static int Mod3(int a)
	{
		return a % 3;
	}
}
