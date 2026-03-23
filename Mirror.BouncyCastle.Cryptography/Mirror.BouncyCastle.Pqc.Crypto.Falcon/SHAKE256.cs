namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class SHAKE256
{
	private ulong[] A;

	private byte[] dubf;

	private ulong dptr;

	private ulong[] RC = new ulong[24]
	{
		1uL, 32898uL, 9223372036854808714uL, 9223372039002292224uL, 32907uL, 2147483649uL, 9223372039002292353uL, 9223372036854808585uL, 138uL, 136uL,
		2147516425uL, 2147483658uL, 2147516555uL, 9223372036854775947uL, 9223372036854808713uL, 9223372036854808579uL, 9223372036854808578uL, 9223372036854775936uL, 32778uL, 9223372039002259466uL,
		9223372039002292353uL, 9223372036854808704uL, 2147483649uL, 9223372039002292232uL
	};

	private void process_block(ulong[] A)
	{
		A[1] = ~A[1];
		A[2] = ~A[2];
		A[8] = ~A[8];
		A[12] = ~A[12];
		A[17] = ~A[17];
		A[20] = ~A[20];
		for (int i = 0; i < 24; i += 2)
		{
			ulong num = A[1] ^ A[6];
			ulong num2 = A[11] ^ A[16];
			num ^= A[21] ^ num2;
			num = (num << 1) | (num >> 63);
			ulong num3 = A[4] ^ A[9];
			ulong num4 = A[14] ^ A[19];
			num ^= A[24];
			num3 ^= num4;
			ulong num5 = num ^ num3;
			num = A[2] ^ A[7];
			num2 = A[12] ^ A[17];
			num ^= A[22] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[0] ^ A[5];
			num4 = A[10] ^ A[15];
			num ^= A[20];
			num3 ^= num4;
			ulong num6 = num ^ num3;
			num = A[3] ^ A[8];
			num2 = A[13] ^ A[18];
			num ^= A[23] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[1] ^ A[6];
			num4 = A[11] ^ A[16];
			num ^= A[21];
			num3 ^= num4;
			ulong num7 = num ^ num3;
			num = A[4] ^ A[9];
			num2 = A[14] ^ A[19];
			num ^= A[24] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[2] ^ A[7];
			num4 = A[12] ^ A[17];
			num ^= A[22];
			num3 ^= num4;
			ulong num8 = num ^ num3;
			num = A[0] ^ A[5];
			num2 = A[10] ^ A[15];
			num ^= A[20] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[3] ^ A[8];
			num4 = A[13] ^ A[18];
			num ^= A[23];
			num3 ^= num4;
			ulong num9 = num ^ num3;
			A[0] ^= num5;
			A[5] ^= num5;
			A[10] ^= num5;
			A[15] ^= num5;
			A[20] ^= num5;
			A[1] ^= num6;
			A[6] ^= num6;
			A[11] ^= num6;
			A[16] ^= num6;
			A[21] ^= num6;
			A[2] ^= num7;
			A[7] ^= num7;
			A[12] ^= num7;
			A[17] ^= num7;
			A[22] ^= num7;
			A[3] ^= num8;
			A[8] ^= num8;
			A[13] ^= num8;
			A[18] ^= num8;
			A[23] ^= num8;
			A[4] ^= num9;
			A[9] ^= num9;
			A[14] ^= num9;
			A[19] ^= num9;
			A[24] ^= num9;
			A[5] = (A[5] << 36) | (A[5] >> 28);
			A[10] = (A[10] << 3) | (A[10] >> 61);
			A[15] = (A[15] << 41) | (A[15] >> 23);
			A[20] = (A[20] << 18) | (A[20] >> 46);
			A[1] = (A[1] << 1) | (A[1] >> 63);
			A[6] = (A[6] << 44) | (A[6] >> 20);
			A[11] = (A[11] << 10) | (A[11] >> 54);
			A[16] = (A[16] << 45) | (A[16] >> 19);
			A[21] = (A[21] << 2) | (A[21] >> 62);
			A[2] = (A[2] << 62) | (A[2] >> 2);
			A[7] = (A[7] << 6) | (A[7] >> 58);
			A[12] = (A[12] << 43) | (A[12] >> 21);
			A[17] = (A[17] << 15) | (A[17] >> 49);
			A[22] = (A[22] << 61) | (A[22] >> 3);
			A[3] = (A[3] << 28) | (A[3] >> 36);
			A[8] = (A[8] << 55) | (A[8] >> 9);
			A[13] = (A[13] << 25) | (A[13] >> 39);
			A[18] = (A[18] << 21) | (A[18] >> 43);
			A[23] = (A[23] << 56) | (A[23] >> 8);
			A[4] = (A[4] << 27) | (A[4] >> 37);
			A[9] = (A[9] << 20) | (A[9] >> 44);
			A[14] = (A[14] << 39) | (A[14] >> 25);
			A[19] = (A[19] << 8) | (A[19] >> 56);
			A[24] = (A[24] << 14) | (A[24] >> 50);
			ulong num10 = ~A[12];
			ulong num11 = A[6] | A[12];
			ulong num12 = A[0] ^ num11;
			num11 = num10 | A[18];
			ulong num13 = A[6] ^ num11;
			num11 = A[18] & A[24];
			ulong num14 = A[12] ^ num11;
			num11 = A[24] | A[0];
			ulong num15 = A[18] ^ num11;
			num11 = A[0] & A[6];
			ulong num16 = A[24] ^ num11;
			A[0] = num12;
			A[6] = num13;
			A[12] = num14;
			A[18] = num15;
			A[24] = num16;
			num10 = ~A[22];
			num11 = A[9] | A[10];
			num12 = A[3] ^ num11;
			num11 = A[10] & A[16];
			num13 = A[9] ^ num11;
			num11 = A[16] | num10;
			num14 = A[10] ^ num11;
			num11 = A[22] | A[3];
			num15 = A[16] ^ num11;
			num11 = A[3] & A[9];
			num16 = A[22] ^ num11;
			A[3] = num12;
			A[9] = num13;
			A[10] = num14;
			A[16] = num15;
			A[22] = num16;
			num10 = ~A[19];
			num11 = A[7] | A[13];
			num12 = A[1] ^ num11;
			num11 = A[13] & A[19];
			num13 = A[7] ^ num11;
			num11 = num10 & A[20];
			num14 = A[13] ^ num11;
			num11 = A[20] | A[1];
			num15 = num10 ^ num11;
			num11 = A[1] & A[7];
			num16 = A[20] ^ num11;
			A[1] = num12;
			A[7] = num13;
			A[13] = num14;
			A[19] = num15;
			A[20] = num16;
			num10 = ~A[17];
			num11 = A[5] & A[11];
			num12 = A[4] ^ num11;
			num11 = A[11] | A[17];
			num13 = A[5] ^ num11;
			num11 = num10 | A[23];
			num14 = A[11] ^ num11;
			num11 = A[23] & A[4];
			num15 = num10 ^ num11;
			num11 = A[4] | A[5];
			num16 = A[23] ^ num11;
			A[4] = num12;
			A[5] = num13;
			A[11] = num14;
			A[17] = num15;
			A[23] = num16;
			num10 = ~A[8];
			num11 = num10 & A[14];
			num12 = A[2] ^ num11;
			num11 = A[14] | A[15];
			num13 = num10 ^ num11;
			num11 = A[15] & A[21];
			num14 = A[14] ^ num11;
			num11 = A[21] | A[2];
			num15 = A[15] ^ num11;
			num11 = A[2] & A[8];
			num16 = A[21] ^ num11;
			A[2] = num12;
			A[8] = num13;
			A[14] = num14;
			A[15] = num15;
			A[21] = num16;
			A[0] ^= RC[i];
			num = A[6] ^ A[9];
			num2 = A[7] ^ A[5];
			num ^= A[8] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[24] ^ A[22];
			num4 = A[20] ^ A[23];
			num ^= A[21];
			num3 ^= num4;
			num5 = num ^ num3;
			num = A[12] ^ A[10];
			num2 = A[13] ^ A[11];
			num ^= A[14] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[0] ^ A[3];
			num4 = A[1] ^ A[4];
			num ^= A[2];
			num3 ^= num4;
			num6 = num ^ num3;
			num = A[18] ^ A[16];
			num2 = A[19] ^ A[17];
			num ^= A[15] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[6] ^ A[9];
			num4 = A[7] ^ A[5];
			num ^= A[8];
			num3 ^= num4;
			num7 = num ^ num3;
			num = A[24] ^ A[22];
			num2 = A[20] ^ A[23];
			num ^= A[21] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[12] ^ A[10];
			num4 = A[13] ^ A[11];
			num ^= A[14];
			num3 ^= num4;
			num8 = num ^ num3;
			num = A[0] ^ A[3];
			num2 = A[1] ^ A[4];
			num ^= A[2] ^ num2;
			num = (num << 1) | (num >> 63);
			num3 = A[18] ^ A[16];
			num4 = A[19] ^ A[17];
			num ^= A[15];
			num3 ^= num4;
			num9 = num ^ num3;
			A[0] ^= num5;
			A[3] ^= num5;
			A[1] ^= num5;
			A[4] ^= num5;
			A[2] ^= num5;
			A[6] ^= num6;
			A[9] ^= num6;
			A[7] ^= num6;
			A[5] ^= num6;
			A[8] ^= num6;
			A[12] ^= num7;
			A[10] ^= num7;
			A[13] ^= num7;
			A[11] ^= num7;
			A[14] ^= num7;
			A[18] ^= num8;
			A[16] ^= num8;
			A[19] ^= num8;
			A[17] ^= num8;
			A[15] ^= num8;
			A[24] ^= num9;
			A[22] ^= num9;
			A[20] ^= num9;
			A[23] ^= num9;
			A[21] ^= num9;
			A[3] = (A[3] << 36) | (A[3] >> 28);
			A[1] = (A[1] << 3) | (A[1] >> 61);
			A[4] = (A[4] << 41) | (A[4] >> 23);
			A[2] = (A[2] << 18) | (A[2] >> 46);
			A[6] = (A[6] << 1) | (A[6] >> 63);
			A[9] = (A[9] << 44) | (A[9] >> 20);
			A[7] = (A[7] << 10) | (A[7] >> 54);
			A[5] = (A[5] << 45) | (A[5] >> 19);
			A[8] = (A[8] << 2) | (A[8] >> 62);
			A[12] = (A[12] << 62) | (A[12] >> 2);
			A[10] = (A[10] << 6) | (A[10] >> 58);
			A[13] = (A[13] << 43) | (A[13] >> 21);
			A[11] = (A[11] << 15) | (A[11] >> 49);
			A[14] = (A[14] << 61) | (A[14] >> 3);
			A[18] = (A[18] << 28) | (A[18] >> 36);
			A[16] = (A[16] << 55) | (A[16] >> 9);
			A[19] = (A[19] << 25) | (A[19] >> 39);
			A[17] = (A[17] << 21) | (A[17] >> 43);
			A[15] = (A[15] << 56) | (A[15] >> 8);
			A[24] = (A[24] << 27) | (A[24] >> 37);
			A[22] = (A[22] << 20) | (A[22] >> 44);
			A[20] = (A[20] << 39) | (A[20] >> 25);
			A[23] = (A[23] << 8) | (A[23] >> 56);
			A[21] = (A[21] << 14) | (A[21] >> 50);
			num10 = ~A[13];
			num11 = A[9] | A[13];
			num12 = A[0] ^ num11;
			num11 = num10 | A[17];
			num13 = A[9] ^ num11;
			num11 = A[17] & A[21];
			num14 = A[13] ^ num11;
			num11 = A[21] | A[0];
			num15 = A[17] ^ num11;
			num11 = A[0] & A[9];
			num16 = A[21] ^ num11;
			A[0] = num12;
			A[9] = num13;
			A[13] = num14;
			A[17] = num15;
			A[21] = num16;
			num10 = ~A[14];
			num11 = A[22] | A[1];
			num12 = A[18] ^ num11;
			num11 = A[1] & A[5];
			num13 = A[22] ^ num11;
			num11 = A[5] | num10;
			num14 = A[1] ^ num11;
			num11 = A[14] | A[18];
			num15 = A[5] ^ num11;
			num11 = A[18] & A[22];
			num16 = A[14] ^ num11;
			A[18] = num12;
			A[22] = num13;
			A[1] = num14;
			A[5] = num15;
			A[14] = num16;
			num10 = ~A[23];
			num11 = A[10] | A[19];
			num12 = A[6] ^ num11;
			num11 = A[19] & A[23];
			num13 = A[10] ^ num11;
			num11 = num10 & A[2];
			num14 = A[19] ^ num11;
			num11 = A[2] | A[6];
			num15 = num10 ^ num11;
			num11 = A[6] & A[10];
			num16 = A[2] ^ num11;
			A[6] = num12;
			A[10] = num13;
			A[19] = num14;
			A[23] = num15;
			A[2] = num16;
			num10 = ~A[11];
			num11 = A[3] & A[7];
			num12 = A[24] ^ num11;
			num11 = A[7] | A[11];
			num13 = A[3] ^ num11;
			num11 = num10 | A[15];
			num14 = A[7] ^ num11;
			num11 = A[15] & A[24];
			num15 = num10 ^ num11;
			num11 = A[24] | A[3];
			num16 = A[15] ^ num11;
			A[24] = num12;
			A[3] = num13;
			A[7] = num14;
			A[11] = num15;
			A[15] = num16;
			num10 = ~A[16];
			num11 = num10 & A[20];
			num12 = A[12] ^ num11;
			num11 = A[20] | A[4];
			num13 = num10 ^ num11;
			num11 = A[4] & A[8];
			num14 = A[20] ^ num11;
			num11 = A[8] | A[12];
			num15 = A[4] ^ num11;
			num11 = A[12] & A[16];
			num16 = A[8] ^ num11;
			A[12] = num12;
			A[16] = num13;
			A[20] = num14;
			A[4] = num15;
			A[8] = num16;
			A[0] ^= RC[i + 1];
			ulong num17 = A[5];
			A[5] = A[18];
			A[18] = A[11];
			A[11] = A[10];
			A[10] = A[6];
			A[6] = A[22];
			A[22] = A[20];
			A[20] = A[12];
			A[12] = A[19];
			A[19] = A[15];
			A[15] = A[24];
			A[24] = A[8];
			A[8] = num17;
			num17 = A[1];
			A[1] = A[9];
			A[9] = A[14];
			A[14] = A[2];
			A[2] = A[13];
			A[13] = A[23];
			A[23] = A[4];
			A[4] = A[21];
			A[21] = A[16];
			A[16] = A[3];
			A[3] = A[17];
			A[17] = A[7];
			A[7] = num17;
		}
		A[1] = ~A[1];
		A[2] = ~A[2];
		A[8] = ~A[8];
		A[12] = ~A[12];
		A[17] = ~A[17];
		A[20] = ~A[20];
	}

	internal void i_shake256_init()
	{
		dptr = 0uL;
		A = new ulong[25];
		dubf = new byte[200];
		for (int i = 0; i < A.Length; i++)
		{
			A[i] = 0uL;
		}
	}

	internal void i_shake256_inject(byte[] insrc, int inarray, int len)
	{
		ulong num = dptr;
		while (len > 0)
		{
			int num2 = 136 - (int)num;
			if (num2 > len)
			{
				num2 = len;
			}
			for (int i = 0; i < num2; i++)
			{
				int num3 = i + (int)num;
				A[num3 >> 3] ^= (ulong)insrc[inarray + i] << ((num3 & 7) << 3);
			}
			num += (ulong)num2;
			inarray += num2;
			len -= num2;
			if (num == 136)
			{
				process_block(A);
				num = 0uL;
			}
		}
		dptr = num;
	}

	internal void i_shake256_flip()
	{
		uint num = (uint)dptr;
		A[num >> 3] ^= (ulong)(31L << (int)((num & 7) << 3));
		A[16] ^= 9223372036854775808uL;
		dptr = 136uL;
	}

	internal void i_shake256_extract(byte[] outsrc, int outarray, int len)
	{
		ulong num = dptr;
		while (len > 0)
		{
			if (num == 136)
			{
				process_block(A);
				num = 0uL;
			}
			int num2 = 136 - (int)num;
			if (num2 > len)
			{
				num2 = len;
			}
			len -= num2;
			while (num2-- > 0)
			{
				outsrc[outarray++] = (byte)(A[num >> 3] >> (int)((num & 7) << 3));
				num++;
			}
		}
		dptr = num;
	}
}
