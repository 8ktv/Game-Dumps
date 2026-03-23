using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.Owcpa;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.Polynomials;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru;

public class NtruKemGenerator : IEncapsulatedSecretGenerator
{
	private readonly SecureRandom _random;

	public NtruKemGenerator(SecureRandom random)
	{
		_random = random;
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		NtruParameterSet parameterSet = ((NtruPublicKeyParameters)recipientKey).Parameters.ParameterSet;
		NtruSampling ntruSampling = new NtruSampling(parameterSet);
		NtruOwcpa ntruOwcpa = new NtruOwcpa(parameterSet);
		byte[] array = new byte[parameterSet.OwcpaMsgBytes()];
		byte[] array2 = new byte[parameterSet.SampleRmBytes()];
		_random.NextBytes(array2);
		PolynomialPair polynomialPair = ntruSampling.SampleRm(array2);
		Polynomial polynomial = polynomialPair.R();
		Polynomial polynomial2 = polynomialPair.M();
		byte[] array3 = polynomial.S3ToBytes(parameterSet.OwcpaMsgBytes());
		Array.Copy(array3, 0, array, 0, array3.Length);
		byte[] array4 = polynomial2.S3ToBytes(array.Length - parameterSet.PackTrinaryBytes());
		Array.Copy(array4, 0, array, parameterSet.PackTrinaryBytes(), array4.Length);
		Sha3Digest sha3Digest = new Sha3Digest(256);
		sha3Digest.BlockUpdate(array, 0, array.Length);
		byte[] array5 = new byte[sha3Digest.GetDigestSize()];
		sha3Digest.DoFinal(array5, 0);
		polynomial.Z3ToZq();
		byte[] ciphertext = ntruOwcpa.Encrypt(polynomial, polynomial2, ((NtruPublicKeyParameters)recipientKey).PublicKey);
		byte[] array6 = new byte[parameterSet.SharedKeyBytes];
		Array.Copy(array5, 0, array6, 0, array6.Length);
		Array.Clear(array5, 0, array5.Length);
		return new NtruEncapsulation(array6, ciphertext);
	}
}
