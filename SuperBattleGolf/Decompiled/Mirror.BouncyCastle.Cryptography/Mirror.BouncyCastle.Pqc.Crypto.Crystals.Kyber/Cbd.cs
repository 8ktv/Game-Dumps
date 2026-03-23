using System;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

internal static class Cbd
{
	internal static void Eta(Poly r, byte[] bytes, int eta)
	{
		switch (eta)
		{
		case 2:
		{
			for (int k = 0; k < 32; k++)
			{
				uint num5 = Pack.LE_To_UInt32(bytes, 4 * k);
				uint num6 = num5 & 0x55555555;
				num6 += (num5 >> 1) & 0x55555555;
				for (int l = 0; l < 8; l++)
				{
					short num7 = (short)((num6 >> 4 * l) & 3);
					short num8 = (short)((num6 >> 4 * l + eta) & 3);
					r.m_coeffs[8 * k + l] = (short)(num7 - num8);
				}
			}
			break;
		}
		case 3:
		{
			for (int i = 0; i < 64; i++)
			{
				uint num = Pack.LE_To_UInt24(bytes, 3 * i);
				uint num2 = num & 0x249249;
				num2 += (num >> 1) & 0x249249;
				num2 += (num >> 2) & 0x249249;
				for (int j = 0; j < 4; j++)
				{
					short num3 = (short)((num2 >> 6 * j) & 7);
					short num4 = (short)((num2 >> 6 * j + 3) & 7);
					r.m_coeffs[4 * i + j] = (short)(num3 - num4);
				}
			}
			break;
		}
		default:
			throw new ArgumentException("Wrong Eta");
		}
	}
}
