namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

internal class Benes12 : Benes
{
	internal Benes12(int n, int t, int m)
		: base(n, t, m)
	{
	}

	internal static void LayerBenes(ulong[] data, ulong[] bits, int lgs)
	{
		int num = 0;
		int num2 = 1 << lgs;
		for (int i = 0; i < 64; i += num2 * 2)
		{
			for (int j = i; j < i + num2; j++)
			{
				ulong num3 = data[j] ^ data[j + num2];
				num3 &= bits[num++];
				data[j] ^= num3;
				data[j + num2] ^= num3;
			}
		}
	}

	private void ApplyBenes(byte[] r, byte[] bits, int rev)
	{
		ulong[] array = new ulong[64];
		ulong[] array2 = new ulong[64];
		Utils.Load8(r, 0, array, 0, 64);
		int num;
		int num2;
		if (rev == 0)
		{
			num = 256;
			num2 = SYS_T * 2 + 40;
		}
		else
		{
			num = -256;
			num2 = SYS_T * 2 + 40 + (2 * GFBITS - 2) * 256;
		}
		Benes.Transpose64x64(array, array);
		for (int i = 0; i <= 5; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				array2[j] = Utils.Load4(bits, num2 + j * 4);
			}
			Benes.Transpose64x64(array2, array2);
			LayerBenes(array, array2, i);
			num2 += num;
		}
		Benes.Transpose64x64(array, array);
		for (int i = 0; i <= 5; i++)
		{
			Utils.Load8(bits, num2, array2, 0, 32);
			LayerBenes(array, array2, i);
			num2 += num;
		}
		for (int i = 4; i >= 0; i--)
		{
			Utils.Load8(bits, num2, array2, 0, 32);
			LayerBenes(array, array2, i);
			num2 += num;
		}
		Benes.Transpose64x64(array, array);
		for (int i = 5; i >= 0; i--)
		{
			for (int j = 0; j < 64; j++)
			{
				array2[j] = Utils.Load4(bits, num2 + j * 4);
			}
			Benes.Transpose64x64(array2, array2);
			LayerBenes(array, array2, i);
			num2 += num;
		}
		Benes.Transpose64x64(array, array);
		Utils.Store8(r, 0, array, 0, 64);
	}

	internal override void SupportGen(ushort[] s, byte[] c)
	{
		byte[][] array = new byte[GFBITS][];
		for (int i = 0; i < GFBITS; i++)
		{
			for (int j = 0; j < (1 << GFBITS) / 8; j++)
			{
				array[i] = new byte[(1 << GFBITS) / 8];
			}
		}
		for (ushort num = 0; num < 1 << GFBITS; num++)
		{
			ushort num2 = Utils.Bitrev(num, GFBITS);
			for (int k = 0; k < GFBITS; k++)
			{
				array[k][num / 8] |= (byte)(((num2 >> k) & 1) << num % 8);
			}
		}
		for (int l = 0; l < GFBITS; l++)
		{
			ApplyBenes(array[l], c, 0);
		}
		for (int m = 0; m < SYS_N; m++)
		{
			s[m] = 0;
			for (int num3 = GFBITS - 1; num3 >= 0; num3--)
			{
				s[m] <<= 1;
				s[m] |= (ushort)((array[num3][m / 8] >> m % 8) & 1);
			}
		}
	}
}
