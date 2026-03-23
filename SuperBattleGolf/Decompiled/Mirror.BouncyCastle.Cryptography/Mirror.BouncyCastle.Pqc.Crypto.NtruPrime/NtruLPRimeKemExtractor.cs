using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

public class NtruLPRimeKemExtractor : IEncapsulatedSecretExtractor
{
	private NtruPrimeEngine _primeEngine;

	private readonly NtruLPRimeKeyParameters _primeKey;

	public int EncapsulationLength => _primeEngine.CipherTextSize;

	public NtruLPRimeKemExtractor(NtruLPRimeKeyParameters privParams)
	{
		_primeKey = privParams;
		InitCipher(_primeKey.Parameters);
	}

	private void InitCipher(NtruLPRimeParameters param)
	{
		_primeEngine = param.PrimeEngine;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		byte[] array = new byte[_primeEngine.SessionKeySize];
		_primeEngine.kem_dec(array, encapsulation, ((NtruLPRimePrivateKeyParameters)_primeKey).privKey);
		return array;
	}
}
