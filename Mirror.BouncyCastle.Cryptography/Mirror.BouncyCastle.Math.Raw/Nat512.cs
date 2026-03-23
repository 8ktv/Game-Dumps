namespace Mirror.BouncyCastle.Math.Raw;

internal static class Nat512
{
	public static void Mul(uint[] x, uint[] y, uint[] zz)
	{
		Nat256.Mul(x, y, zz);
		Nat256.Mul(x, 8, y, 8, zz, 16);
		uint num = Nat256.AddToEachOther(zz, 8, zz, 16);
		uint cIn = num + Nat256.AddTo(zz, 0, zz, 8, 0u);
		num += Nat256.AddTo(zz, 24, zz, 16, cIn);
		uint[] array = Nat256.Create();
		uint[] array2 = Nat256.Create();
		bool flag = Nat256.Diff(x, 8, x, 0, array, 0) != Nat256.Diff(y, 8, y, 0, array2, 0);
		uint[] array3 = Nat256.CreateExt();
		Nat256.Mul(array, array2, array3);
		num += (uint)(flag ? ((int)Nat.AddTo(16, array3, 0, zz, 8)) : Nat.SubFrom(16, array3, 0, zz, 8));
		Nat.AddWordAt(32, num, zz, 24);
	}

	public static void Square(uint[] x, uint[] zz)
	{
		Nat256.Square(x, zz);
		Nat256.Square(x, 8, zz, 16);
		uint num = Nat256.AddToEachOther(zz, 8, zz, 16);
		uint cIn = num + Nat256.AddTo(zz, 0, zz, 8, 0u);
		num += Nat256.AddTo(zz, 24, zz, 16, cIn);
		uint[] array = Nat256.Create();
		Nat256.Diff(x, 8, x, 0, array, 0);
		uint[] array2 = Nat256.CreateExt();
		Nat256.Square(array, array2);
		num += (uint)Nat.SubFrom(16, array2, 0, zz, 8);
		Nat.AddWordAt(32, num, zz, 24);
	}

	public static void Xor(uint[] x, int xOff, uint[] y, int yOff, uint[] z, int zOff)
	{
		for (int i = 0; i < 16; i += 4)
		{
			z[zOff + i] = x[xOff + i] ^ y[yOff + i];
			z[zOff + i + 1] = x[xOff + i + 1] ^ y[yOff + i + 1];
			z[zOff + i + 2] = x[xOff + i + 2] ^ y[yOff + i + 2];
			z[zOff + i + 3] = x[xOff + i + 3] ^ y[yOff + i + 3];
		}
	}

	public static void XorTo(uint[] x, int xOff, uint[] z, int zOff)
	{
		for (int i = 0; i < 16; i += 4)
		{
			z[zOff + i] ^= x[xOff + i];
			z[zOff + i + 1] ^= x[xOff + i + 1];
			z[zOff + i + 2] ^= x[xOff + i + 2];
			z[zOff + i + 3] ^= x[xOff + i + 3];
		}
	}

	public static void Xor64(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff)
	{
		for (int i = 0; i < 8; i += 4)
		{
			z[zOff + i] = x[xOff + i] ^ y[yOff + i];
			z[zOff + i + 1] = x[xOff + i + 1] ^ y[yOff + i + 1];
			z[zOff + i + 2] = x[xOff + i + 2] ^ y[yOff + i + 2];
			z[zOff + i + 3] = x[xOff + i + 3] ^ y[yOff + i + 3];
		}
	}

	public static void XorTo64(ulong[] x, int xOff, ulong[] z, int zOff)
	{
		for (int i = 0; i < 8; i += 4)
		{
			z[zOff + i] ^= x[xOff + i];
			z[zOff + i + 1] ^= x[xOff + i + 1];
			z[zOff + i + 2] ^= x[xOff + i + 2];
			z[zOff + i + 3] ^= x[xOff + i + 3];
		}
	}
}
