namespace Mirror.BouncyCastle.Utilities;

public static class Bytes
{
	public const int NumBits = 8;

	public const int NumBytes = 1;

	public static void Xor(int len, byte[] x, byte[] y, byte[] z)
	{
		for (int i = 0; i < len; i++)
		{
			z[i] = (byte)(x[i] ^ y[i]);
		}
	}

	public static void Xor(int len, byte[] x, int xOff, byte[] y, int yOff, byte[] z, int zOff)
	{
		for (int i = 0; i < len; i++)
		{
			z[zOff + i] = (byte)(x[xOff + i] ^ y[yOff + i]);
		}
	}

	public static void XorTo(int len, byte[] x, byte[] z)
	{
		for (int i = 0; i < len; i++)
		{
			z[i] ^= x[i];
		}
	}

	public static void XorTo(int len, byte[] x, int xOff, byte[] z, int zOff)
	{
		for (int i = 0; i < len; i++)
		{
			z[zOff + i] ^= x[xOff + i];
		}
	}
}
