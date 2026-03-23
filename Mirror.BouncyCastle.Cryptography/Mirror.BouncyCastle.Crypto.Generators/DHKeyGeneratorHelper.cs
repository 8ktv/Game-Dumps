using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC.Multiplier;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Generators;

internal static class DHKeyGeneratorHelper
{
	internal static BigInteger CalculatePrivate(DHParameters dhParams, SecureRandom random)
	{
		int l = dhParams.L;
		if (l != 0)
		{
			int num = l >> 2;
			BigInteger bigInteger;
			do
			{
				bigInteger = new BigInteger(l, random).SetBit(l - 1);
			}
			while (WNafUtilities.GetNafWeight(bigInteger) < num);
			return bigInteger;
		}
		BigInteger min = BigInteger.Two;
		int m = dhParams.M;
		if (m != 0)
		{
			min = BigInteger.One.ShiftLeft(m - 1);
		}
		BigInteger bigInteger2 = (dhParams.Q ?? dhParams.P).Subtract(BigInteger.Two);
		int num2 = bigInteger2.BitLength >> 2;
		BigInteger bigInteger3;
		do
		{
			bigInteger3 = BigIntegers.CreateRandomInRange(min, bigInteger2, random);
		}
		while (WNafUtilities.GetNafWeight(bigInteger3) < num2);
		return bigInteger3;
	}

	internal static BigInteger CalculatePublic(DHParameters dhParams, BigInteger x)
	{
		return dhParams.G.ModPow(x, dhParams.P);
	}
}
