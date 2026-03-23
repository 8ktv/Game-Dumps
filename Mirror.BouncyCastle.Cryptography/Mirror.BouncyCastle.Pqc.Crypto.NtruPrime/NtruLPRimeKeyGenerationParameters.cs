using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

public class NtruLPRimeKeyGenerationParameters : KeyGenerationParameters
{
	private NtruLPRimeParameters _primeParameters;

	public NtruLPRimeParameters Parameters => _primeParameters;

	public NtruLPRimeKeyGenerationParameters(SecureRandom random, NtruLPRimeParameters ntruPrimeParameters)
		: base(random, 256)
	{
		_primeParameters = ntruPrimeParameters;
	}
}
