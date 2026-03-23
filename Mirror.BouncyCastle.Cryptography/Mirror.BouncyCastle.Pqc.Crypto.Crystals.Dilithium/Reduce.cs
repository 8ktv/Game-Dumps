namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

internal class Reduce
{
	public static int MontgomeryReduce(long a)
	{
		int num = (int)(a * 58728449);
		return (int)(a - (long)num * 8380417L >> 32);
	}

	public static int Reduce32(int a)
	{
		int num = a + 4194304 >> 23;
		return a - num * 8380417;
	}

	public static int ConditionalAddQ(int a)
	{
		return a + ((a >> 31) & 0x7FE001);
	}
}
