using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

internal class Rounding
{
	public static int[] Power2Round(int a)
	{
		int[] array = new int[2];
		array[0] = a + 4096 - 1 >> 13;
		array[1] = a - (array[0] << 13);
		return array;
	}

	public static int[] Decompose(int a, int gamma2)
	{
		int num = a + 127 >> 7;
		switch (gamma2)
		{
		case 261888:
			num = num * 1025 + 2097152 >> 22;
			num &= 0xF;
			break;
		case 95232:
			num = num * 11275 + 8388608 >> 24;
			num ^= (43 - num >> 31) & num;
			break;
		default:
			throw new ArgumentException("Wrong Gamma2!");
		}
		int num2 = a - num * 2 * gamma2;
		num2 -= (4190208 - num2 >> 31) & 0x7FE001;
		return new int[2] { num2, num };
	}

	public static int MakeHint(int a0, int a1, DilithiumEngine engine)
	{
		int gamma = engine.Gamma2;
		int num = 8380417;
		if (a0 <= gamma || a0 > num - gamma || (a0 == num - gamma && a1 == 0))
		{
			return 0;
		}
		return 1;
	}

	public static int UseHint(int a, int hint, int gamma2)
	{
		int[] array = Decompose(a, gamma2);
		int num = array[0];
		int num2 = array[1];
		if (hint == 0)
		{
			return num2;
		}
		switch (gamma2)
		{
		case 261888:
			if (num > 0)
			{
				return (num2 + 1) & 0xF;
			}
			return (num2 - 1) & 0xF;
		case 95232:
			if (num > 0)
			{
				if (num2 != 43)
				{
					return num2 + 1;
				}
				return 0;
			}
			if (num2 != 0)
			{
				return num2 - 1;
			}
			return 43;
		default:
			throw new ArgumentException("Wrong Gamma2!");
		}
	}
}
