using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

public class NtruLPRimeKeyPairGenerator
{
	private NtruLPRimeKeyGenerationParameters _ntruPrimeParams;

	private int p;

	private int q;

	private SecureRandom random;

	private void Initialize(KeyGenerationParameters param)
	{
		_ntruPrimeParams = (NtruLPRimeKeyGenerationParameters)param;
		random = param.Random;
		p = _ntruPrimeParams.Parameters.P;
		q = _ntruPrimeParams.Parameters.Q;
	}

	private AsymmetricCipherKeyPair GenKeyPair()
	{
		NtruPrimeEngine primeEngine = _ntruPrimeParams.Parameters.PrimeEngine;
		byte[] array = new byte[primeEngine.PrivateKeySize];
		byte[] array2 = new byte[primeEngine.PublicKeySize];
		primeEngine.kem_keypair(array2, array, random);
		NtruLPRimePublicKeyParameters publicParameter = new NtruLPRimePublicKeyParameters(_ntruPrimeParams.Parameters, array2);
		NtruLPRimePrivateKeyParameters privateParameter = new NtruLPRimePrivateKeyParameters(_ntruPrimeParams.Parameters, array);
		return new AsymmetricCipherKeyPair(publicParameter, privateParameter);
	}

	public void Init(KeyGenerationParameters param)
	{
		Initialize(param);
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		return GenKeyPair();
	}
}
