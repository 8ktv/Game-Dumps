namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

internal class Benes13 : Benes
{
	internal Benes13(int n, int t, int m)
		: base(n, t, m)
	{
	}

	internal static void LayerIn(ulong[] data, ulong[] bits, int lgs)
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
				num3 = data[64 + j] ^ data[64 + j + num2];
				num3 &= bits[num++];
				data[64 + j] ^= num3;
				data[64 + j + num2] ^= num3;
			}
		}
	}

	internal static void LayerEx(ulong[] data, ulong[] bits, int lgs)
	{
		int num = 0;
		int num2 = 1 << lgs;
		for (int i = 0; i < 128; i += num2 * 2)
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

	internal void ApplyBenes(byte[] r, byte[] bits, int rev)
	{
		int num = 0;
		int num2 = 0;
		ulong[] array = new ulong[128];
		ulong[] array2 = new ulong[128];
		ulong[] array3 = new ulong[64];
		ulong[] array4 = new ulong[64];
		int num3;
		if (rev == 0)
		{
			num2 = SYS_T * 2 + 40;
			num3 = 0;
		}
		else
		{
			num2 = SYS_T * 2 + 40 + 12288;
			num3 = -1024;
		}
		for (int i = 0; i < 64; i++)
		{
			array[i] = Utils.Load8(r, num + i * 16);
			array[i + 64] = Utils.Load8(r, num + i * 16 + 8);
		}
		Benes.Transpose64x64(array2, array, 0);
		Benes.Transpose64x64(array2, array, 64);
		for (int j = 0; j <= 6; j++)
		{
			for (int i = 0; i < 64; i++)
			{
				array3[i] = Utils.Load8(bits, num2);
				num2 += 8;
			}
			num2 += num3;
			Benes.Transpose64x64(array4, array3);
			LayerEx(array2, array4, j);
		}
		Benes.Transpose64x64(array, array2, 0);
		Benes.Transpose64x64(array, array2, 64);
		for (int j = 0; j <= 5; j++)
		{
			for (int i = 0; i < 64; i++)
			{
				array3[i] = Utils.Load8(bits, num2);
				num2 += 8;
			}
			num2 += num3;
			LayerIn(array, array3, j);
		}
		for (int j = 4; j >= 0; j--)
		{
			for (int i = 0; i < 64; i++)
			{
				array3[i] = Utils.Load8(bits, num2);
				num2 += 8;
			}
			num2 += num3;
			LayerIn(array, array3, j);
		}
		Benes.Transpose64x64(array2, array, 0);
		Benes.Transpose64x64(array2, array, 64);
		for (int j = 6; j >= 0; j--)
		{
			for (int i = 0; i < 64; i++)
			{
				array3[i] = Utils.Load8(bits, num2);
				num2 += 8;
			}
			num2 += num3;
			Benes.Transpose64x64(array4, array3);
			LayerEx(array2, array4, j);
		}
		Benes.Transpose64x64(array, array2, 0);
		Benes.Transpose64x64(array, array2, 64);
		for (int i = 0; i < 64; i++)
		{
			Utils.Store8(r, num + i * 16, array[i]);
			Utils.Store8(r, num + i * 16 + 8, array[64 + i]);
		}
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
		for (int i = 0; i < 1 << GFBITS; i++)
		{
			ushort num = Utils.Bitrev((ushort)i, GFBITS);
			for (int j = 0; j < GFBITS; j++)
			{
				array[j][i / 8] |= (byte)(((num >> j) & 1) << i % 8);
			}
		}
		for (int j = 0; j < GFBITS; j++)
		{
			ApplyBenes(array[j], c, 0);
		}
		for (int i = 0; i < SYS_N; i++)
		{
			s[i] = 0;
			for (int j = GFBITS - 1; j >= 0; j--)
			{
				s[i] <<= 1;
				s[i] |= (ushort)((array[j][i / 8] >> i % 8) & 1);
			}
		}
	}
}
