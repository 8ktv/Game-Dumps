using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

public class SNtruPrimeKeyPairGenerator
{
	private SNtruPrimeKeyGenerationParameters _ntruPrimeParams;

	private int p;

	private int q;

	private SecureRandom random;

	private void Initialize(KeyGenerationParameters param)
	{
		_ntruPrimeParams = (SNtruPrimeKeyGenerationParameters)param;
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
		SNtruPrimePublicKeyParameters publicParameter = new SNtruPrimePublicKeyParameters(_ntruPrimeParams.Parameters, array2);
		SNtruPrimePrivateKeyParameters privateParameter = new SNtruPrimePrivateKeyParameters(_ntruPrimeParams.Parameters, array);
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
