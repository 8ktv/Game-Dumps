using System;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.Polynomials;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru.Owcpa;

internal class NtruOwcpa
{
	private readonly NtruParameterSet _parameterSet;

	private readonly NtruSampling _sampling;

	internal NtruOwcpa(NtruParameterSet parameterSet)
	{
		_parameterSet = parameterSet;
		_sampling = new NtruSampling(parameterSet);
	}

	internal OwcpaKeyPair KeyPair(byte[] seed)
	{
		byte[] array = new byte[_parameterSet.OwcpaSecretKeyBytes()];
		int n = _parameterSet.N;
		_parameterSet.Q();
		_parameterSet.CreatePolynomial();
		_parameterSet.CreatePolynomial();
		Polynomial polynomial = _parameterSet.CreatePolynomial();
		Polynomial polynomial2 = _parameterSet.CreatePolynomial();
		Polynomial polynomial3 = _parameterSet.CreatePolynomial();
		Polynomial polynomial4 = polynomial;
		Polynomial polynomial5 = polynomial;
		Polynomial polynomial6 = polynomial2;
		Polynomial polynomial7 = polynomial3;
		Polynomial polynomial8 = polynomial;
		Polynomial polynomial9 = polynomial;
		PolynomialPair polynomialPair = _sampling.SampleFg(seed);
		Polynomial polynomial10 = polynomialPair.F();
		Polynomial polynomial11 = polynomialPair.G();
		polynomial4.S3Inv(polynomial10);
		byte[] array2 = polynomial10.S3ToBytes(_parameterSet.OwcpaMsgBytes());
		Array.Copy(array2, 0, array, 0, array2.Length);
		byte[] array3 = polynomial4.S3ToBytes(array.Length - _parameterSet.PackTrinaryBytes());
		Array.Copy(array3, 0, array, _parameterSet.PackTrinaryBytes(), array3.Length);
		polynomial10.Z3ToZq();
		polynomial11.Z3ToZq();
		if (_parameterSet is NtruHrssParameterSet)
		{
			for (int num = n - 1; num > 0; num--)
			{
				polynomial11.coeffs[num] = (ushort)(3 * (polynomial11.coeffs[num - 1] - polynomial11.coeffs[num]));
			}
			polynomial11.coeffs[0] = (ushort)(-(3 * polynomial11.coeffs[0]));
		}
		else
		{
			for (int num = 0; num < n; num++)
			{
				polynomial11.coeffs[num] = (ushort)(3 * polynomial11.coeffs[num]);
			}
		}
		polynomial5.RqMul(polynomial11, polynomial10);
		polynomial6.RqInv(polynomial5);
		polynomial7.RqMul(polynomial6, polynomial10);
		polynomial8.SqMul(polynomial7, polynomial10);
		byte[] array4 = polynomial8.SqToBytes(array.Length - 2 * _parameterSet.PackTrinaryBytes());
		Array.Copy(array4, 0, array, 2 * _parameterSet.PackTrinaryBytes(), array4.Length);
		polynomial7.RqMul(polynomial6, polynomial11);
		polynomial9.RqMul(polynomial7, polynomial11);
		return new OwcpaKeyPair(polynomial9.RqSumZeroToBytes(_parameterSet.OwcpaPublicKeyBytes()), array);
	}

	internal byte[] Encrypt(Polynomial r, Polynomial m, byte[] publicKey)
	{
		Polynomial polynomial = _parameterSet.CreatePolynomial();
		Polynomial polynomial2 = _parameterSet.CreatePolynomial();
		Polynomial polynomial3 = polynomial;
		Polynomial polynomial4 = polynomial;
		Polynomial polynomial5 = polynomial2;
		polynomial3.RqSumZeroFromBytes(publicKey);
		polynomial5.RqMul(r, polynomial3);
		polynomial4.Lift(m);
		for (int i = 0; i < _parameterSet.N; i++)
		{
			polynomial5.coeffs[i] += polynomial4.coeffs[i];
		}
		return polynomial5.RqSumZeroToBytes(_parameterSet.NtruCiphertextBytes());
	}

	internal OwcpaDecryptResult Decrypt(byte[] ciphertext, byte[] privateKey)
	{
		byte[] array = new byte[_parameterSet.OwcpaMsgBytes()];
		Polynomial polynomial = _parameterSet.CreatePolynomial();
		Polynomial polynomial2 = _parameterSet.CreatePolynomial();
		Polynomial polynomial3 = _parameterSet.CreatePolynomial();
		Polynomial polynomial4 = _parameterSet.CreatePolynomial();
		Polynomial polynomial5 = polynomial;
		Polynomial polynomial6 = polynomial2;
		Polynomial polynomial7 = polynomial3;
		Polynomial polynomial8 = polynomial2;
		Polynomial polynomial9 = polynomial3;
		Polynomial polynomial10 = polynomial4;
		Polynomial polynomial11 = polynomial2;
		Polynomial polynomial12 = polynomial3;
		Polynomial polynomial13 = polynomial4;
		Polynomial polynomial14 = polynomial;
		polynomial5.RqSumZeroFromBytes(ciphertext);
		polynomial6.S3FromBytes(privateKey);
		polynomial6.Z3ToZq();
		polynomial7.RqMul(polynomial5, polynomial6);
		polynomial8.RqToS3(polynomial7);
		polynomial9.S3FromBytes(Arrays.CopyOfRange(privateKey, _parameterSet.PackTrinaryBytes(), privateKey.Length));
		polynomial10.S3Mul(polynomial8, polynomial9);
		byte[] array2 = polynomial10.S3ToBytes(array.Length - _parameterSet.PackTrinaryBytes());
		int num = 0;
		num |= CheckCiphertext(ciphertext);
		if (_parameterSet is NtruHpsParameterSet)
		{
			num |= CheckM((HpsPolynomial)polynomial10);
		}
		polynomial11.Lift(polynomial10);
		for (int i = 0; i < _parameterSet.N; i++)
		{
			polynomial14.coeffs[i] = (ushort)(polynomial5.coeffs[i] - polynomial11.coeffs[i]);
		}
		polynomial12.SqFromBytes(Arrays.CopyOfRange(privateKey, 2 * _parameterSet.PackTrinaryBytes(), privateKey.Length));
		polynomial13.SqMul(polynomial14, polynomial12);
		num |= CheckR(polynomial13);
		polynomial13.TrinaryZqToZ3();
		byte[] array3 = polynomial13.S3ToBytes(_parameterSet.OwcpaMsgBytes());
		Array.Copy(array3, 0, array, 0, array3.Length);
		Array.Copy(array2, 0, array, _parameterSet.PackTrinaryBytes(), array2.Length);
		return new OwcpaDecryptResult(array, num);
	}

	private int CheckCiphertext(byte[] ciphertext)
	{
		ushort num = ciphertext[_parameterSet.NtruCiphertextBytes() - 1];
		num &= (ushort)(255 << 8 - (7 & (_parameterSet.LogQ * _parameterSet.PackDegree())));
		return 1 & (~num + 1 >> 15);
	}

	private int CheckR(Polynomial r)
	{
		int num = 0;
		for (int i = 0; i < _parameterSet.N - 1; i++)
		{
			ushort num2 = r.coeffs[i];
			num |= (num2 + 1) & (_parameterSet.Q() - 4);
			num |= (num2 + 2) & 4;
		}
		num |= r.coeffs[_parameterSet.N - 1];
		return 1 & (~num + 1 >> 31);
	}

	private int CheckM(HpsPolynomial m)
	{
		int num = 0;
		ushort num2 = 0;
		ushort num3 = 0;
		for (int i = 0; i < _parameterSet.N - 1; i++)
		{
			num2 += (ushort)(m.coeffs[i] & 1);
			num3 += (ushort)(m.coeffs[i] & 2);
		}
		num |= num2 ^ (num3 >> 1);
		num |= num3 ^ ((NtruHpsParameterSet)_parameterSet).Weight();
		return 1 & (~num + 1 >> 31);
	}
}
