using System.Collections.Generic;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public class NaccacheSternPrivateKeyParameters : NaccacheSternKeyParameters
{
	private readonly BigInteger phiN;

	private readonly IList<BigInteger> smallPrimes;

	public BigInteger PhiN => phiN;

	public IList<BigInteger> SmallPrimesList => smallPrimes;

	public NaccacheSternPrivateKeyParameters(BigInteger g, BigInteger n, int lowerSigmaBound, IList<BigInteger> smallPrimes, BigInteger phiN)
		: base(privateKey: true, g, n, lowerSigmaBound)
	{
		this.smallPrimes = smallPrimes;
		this.phiN = phiN;
	}
}
