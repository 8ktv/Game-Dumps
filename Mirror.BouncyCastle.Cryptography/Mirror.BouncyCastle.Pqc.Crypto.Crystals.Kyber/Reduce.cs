namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

internal static class Reduce
{
	internal static short MontgomeryReduce(int a)
	{
		int num = (short)(a * 62209) * 3329;
		num = a - num;
		num >>= 16;
		return (short)num;
	}

	internal static short BarrettReduce(short a)
	{
		short num = (short)(20159 * a >> 26);
		num *= 3329;
		return (short)(a - num);
	}

	internal static short CondSubQ(short a)
	{
		a -= 3329;
		a += (short)((a >> 15) & 0xD01);
		return a;
	}
}
