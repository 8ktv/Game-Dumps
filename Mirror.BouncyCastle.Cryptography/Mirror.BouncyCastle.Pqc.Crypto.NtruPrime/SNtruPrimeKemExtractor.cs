using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

public class SNtruPrimeKemExtractor : IEncapsulatedSecretExtractor
{
	private NtruPrimeEngine _primeEngine;

	private readonly SNtruPrimeKeyParameters _primeKey;

	public int EncapsulationLength => _primeEngine.CipherTextSize;

	public SNtruPrimeKemExtractor(SNtruPrimeKeyParameters privParams)
	{
		_primeKey = privParams;
		InitCipher(_primeKey.Parameters);
	}

	private void InitCipher(SNtruPrimeParameters param)
	{
		_primeEngine = param.PrimeEngine;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		byte[] array = new byte[_primeEngine.SessionKeySize];
		_primeEngine.kem_dec(array, encapsulation, ((SNtruPrimePrivateKeyParameters)_primeKey).privKey);
		return array;
	}
}
