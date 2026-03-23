using System;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public class NaccacheSternKeyGenerationParameters : KeyGenerationParameters
{
	private readonly int certainty;

	private readonly int countSmallPrimes;

	public int Certainty => certainty;

	public int CountSmallPrimes => countSmallPrimes;

	public NaccacheSternKeyGenerationParameters(SecureRandom random, int strength, int certainty, int countSmallPrimes)
		: base(random, strength)
	{
		if (countSmallPrimes % 2 == 1)
		{
			throw new ArgumentException("countSmallPrimes must be a multiple of 2");
		}
		if (countSmallPrimes < 30)
		{
			throw new ArgumentException("countSmallPrimes must be >= 30 for security reasons");
		}
		this.certainty = certainty;
		this.countSmallPrimes = countSmallPrimes;
	}
}
