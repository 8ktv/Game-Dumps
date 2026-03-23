using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.Owcpa;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru;

public class NtruKemExtractor : IEncapsulatedSecretExtractor
{
	private readonly NtruParameters _parameters;

	private readonly NtruPrivateKeyParameters _ntruPrivateKey;

	public int EncapsulationLength => _parameters.ParameterSet.NtruCiphertextBytes();

	public NtruKemExtractor(NtruPrivateKeyParameters ntruPrivateKey)
	{
		_parameters = ntruPrivateKey.Parameters;
		_ntruPrivateKey = ntruPrivateKey;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		NtruParameterSet parameterSet = _parameters.ParameterSet;
		byte[] privateKey = _ntruPrivateKey.PrivateKey;
		byte[] array = new byte[parameterSet.PrfKeyBytes + parameterSet.NtruCiphertextBytes()];
		OwcpaDecryptResult owcpaDecryptResult = new NtruOwcpa(parameterSet).Decrypt(encapsulation, _ntruPrivateKey.PrivateKey);
		byte[] rm = owcpaDecryptResult.Rm;
		int fail = owcpaDecryptResult.Fail;
		Sha3Digest sha3Digest = new Sha3Digest(256);
		byte[] array2 = new byte[sha3Digest.GetDigestSize()];
		sha3Digest.BlockUpdate(rm, 0, rm.Length);
		sha3Digest.DoFinal(array2, 0);
		for (int i = 0; i < parameterSet.PrfKeyBytes; i++)
		{
			array[i] = privateKey[i + parameterSet.OwcpaSecretKeyBytes()];
		}
		for (int i = 0; i < parameterSet.NtruCiphertextBytes(); i++)
		{
			array[parameterSet.PrfKeyBytes + i] = encapsulation[i];
		}
		sha3Digest.Reset();
		sha3Digest.BlockUpdate(array, 0, array.Length);
		sha3Digest.DoFinal(rm, 0);
		Cmov(array2, rm, (byte)fail);
		byte[] array3 = new byte[parameterSet.SharedKeyBytes];
		Array.Copy(array2, 0, array3, 0, parameterSet.SharedKeyBytes);
		Array.Clear(array2, 0, array2.Length);
		return array3;
	}

	private static void Cmov(byte[] r, byte[] x, byte b)
	{
		b = (byte)(~b + 1);
		for (int i = 0; i < r.Length; i++)
		{
			r[i] ^= (byte)(b & (x[i] ^ r[i]));
		}
	}
}
