namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconCodec
{
	internal byte[] max_fg_bits = new byte[11]
	{
		0, 8, 8, 8, 8, 8, 7, 7, 6, 6,
		5
	};

	internal byte[] max_FG_bits = new byte[11]
	{
		0, 8, 8, 8, 8, 8, 8, 8, 8, 8,
		8
	};

	internal byte[] max_sig_bits = new byte[11]
	{
		0, 10, 11, 11, 12, 12, 12, 12, 12, 12,
		12
	};

	internal FalconCodec()
	{
	}

	internal int modq_encode(byte[] outarrsrc, int outarr, int max_out_len, ushort[] xsrc, int x, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			if (xsrc[x + i] >= 12289)
			{
				return 0;
			}
		}
		int num2 = num * 14 + 7 >> 3;
		if (outarrsrc == null)
		{
			return num2;
		}
		if (num2 > max_out_len)
		{
			return 0;
		}
		int num3 = outarr;
		uint num4 = 0u;
		int num5 = 0;
		for (int i = 0; i < num; i++)
		{
			num4 = (num4 << 14) | xsrc[x + i];
			num5 += 14;
			while (num5 >= 8)
			{
				num5 -= 8;
				outarrsrc[num3++] = (byte)(num4 >> num5);
			}
		}
		if (num5 > 0)
		{
			outarrsrc[num3] = (byte)(num4 << 8 - num5);
		}
		return num2;
	}

	internal int modq_decode(ushort[] xsrc, int x, uint logn, byte[] inarrsrc, int inarr, int max_in_len)
	{
		int num = 1 << (int)logn;
		int num2 = num * 14 + 7 >> 3;
		if (num2 > max_in_len)
		{
			return 0;
		}
		int num3 = inarr;
		uint num4 = 0u;
		int num5 = 0;
		int num6 = 0;
		while (num6 < num)
		{
			num4 = (num4 << 8) | inarrsrc[num3++];
			num5 += 8;
			if (num5 >= 14)
			{
				num5 -= 14;
				uint num7 = (num4 >> num5) & 0x3FFF;
				if (num7 >= 12289)
				{
					return 0;
				}
				xsrc[x + num6] = (ushort)num7;
				num6++;
			}
		}
		if ((num4 & (uint)((1 << num5) - 1)) != 0)
		{
			return 0;
		}
		return num2;
	}

	internal int trim_i16_encode(byte[] outarrsrc, int outarr, int max_out_len, short[] xsrc, int x, uint logn, uint bits)
	{
		int num = 1 << (int)logn;
		int num2 = (1 << (int)(bits - 1)) - 1;
		int num3 = -num2;
		for (int i = 0; i < num; i++)
		{
			if (xsrc[x + i] < num3 || xsrc[x + i] > num2)
			{
				return 0;
			}
		}
		int num4 = (int)(num * bits + 7) >> 3;
		if (outarrsrc == null)
		{
			return num4;
		}
		if (num4 > max_out_len)
		{
			return 0;
		}
		int num5 = outarr;
		uint num6 = 0u;
		uint num7 = 0u;
		uint num8 = (uint)((1 << (int)bits) - 1);
		for (int i = 0; i < num; i++)
		{
			num6 = (num6 << (int)bits) | ((ushort)xsrc[x + i] & num8);
			num7 += bits;
			while (num7 >= 8)
			{
				num7 -= 8;
				outarrsrc[num5++] = (byte)(num6 >> (int)num7);
			}
		}
		if (num7 != 0)
		{
			outarrsrc[num5++] = (byte)(num6 << (int)(8 - num7));
		}
		return num4;
	}

	internal int trim_i16_decode(short[] xsrc, int x, uint logn, uint bits, byte[] inarrsrc, int inarr, int max_in_len)
	{
		int num = 1 << (int)logn;
		int num2 = (int)(num * bits + 7) >> 3;
		if (num2 > max_in_len)
		{
			return 0;
		}
		int num3 = inarr;
		int num4 = 0;
		uint num5 = 0u;
		uint num6 = 0u;
		uint num7 = (uint)((1 << (int)bits) - 1);
		uint num8 = (uint)(1 << (int)(bits - 1));
		while (num4 < num)
		{
			num5 = (num5 << 8) | inarrsrc[num3++];
			num6 += 8;
			while (num6 >= bits && num4 < num)
			{
				num6 -= bits;
				uint num9 = (num5 >> (int)num6) & num7;
				num9 = (uint)(num9 | (0L - (long)(num9 & num8)));
				num9 |= (uint)(int)(0L - (long)(num9 & num8));
				if (num9 == 0L - (long)num8)
				{
					return 0;
				}
				num9 |= (uint)(int)(0L - (long)(num9 & num8));
				xsrc[x + num4] = (short)num9;
				num4++;
			}
		}
		if ((num5 & (uint)((1 << (int)num6) - 1)) != 0)
		{
			return 0;
		}
		return num2;
	}

	internal int trim_i8_encode(byte[] outarrsrc, int outarr, int max_out_len, sbyte[] xsrc, int x, uint logn, uint bits)
	{
		int num = 1 << (int)logn;
		int num2 = (1 << (int)(bits - 1)) - 1;
		int num3 = -num2;
		for (int i = 0; i < num; i++)
		{
			if (xsrc[x + i] < num3 || xsrc[x + i] > num2)
			{
				return 0;
			}
		}
		int num4 = (int)(num * bits + 7) >> 3;
		if (outarrsrc == null)
		{
			return num4;
		}
		if (num4 > max_out_len)
		{
			return 0;
		}
		int num5 = outarr;
		uint num6 = 0u;
		uint num7 = 0u;
		uint num8 = (uint)((1 << (int)bits) - 1);
		for (int i = 0; i < num; i++)
		{
			num6 = (num6 << (int)bits) | ((byte)xsrc[x + i] & num8);
			num7 += bits;
			while (num7 >= 8)
			{
				num7 -= 8;
				outarrsrc[num5++] = (byte)(num6 >> (int)num7);
			}
		}
		if (num7 != 0)
		{
			outarrsrc[num5++] = (byte)(num6 << (int)(8 - num7));
		}
		return num4;
	}

	internal int trim_i8_decode(sbyte[] xsrc, int x, uint logn, uint bits, byte[] inarrsrc, int inarr, int max_in_len)
	{
		int num = 1 << (int)logn;
		int num2 = (int)(num * bits + 7) >> 3;
		if (num2 > max_in_len)
		{
			return 0;
		}
		int num3 = inarr;
		int num4 = 0;
		uint num5 = 0u;
		uint num6 = 0u;
		uint num7 = (uint)((1 << (int)bits) - 1);
		uint num8 = (uint)(1 << (int)(bits - 1));
		while (num4 < num)
		{
			num5 = (num5 << 8) | inarrsrc[num3++];
			num6 += 8;
			while (num6 >= bits && num4 < num)
			{
				num6 -= bits;
				uint num9 = (num5 >> (int)num6) & num7;
				num9 |= (uint)(int)(0L - (long)(num9 & num8));
				if (num9 == 0L - (long)num8)
				{
					return 0;
				}
				xsrc[x + num4] = (sbyte)num9;
				num4++;
			}
		}
		if ((num5 & (uint)((1 << (int)num6) - 1)) != 0)
		{
			return 0;
		}
		return num2;
	}

	internal int comp_encode(byte[] outarrsrc, int outarr, int max_out_len, short[] xsrc, int x, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			if (xsrc[x + i] < -2047 || xsrc[x + i] > 2047)
			{
				return 0;
			}
		}
		uint num2 = 0u;
		uint num3 = 0u;
		int num4 = 0;
		for (int i = 0; i < num; i++)
		{
			num2 <<= 1;
			int num5 = xsrc[x + i];
			if (num5 < 0)
			{
				num5 = -num5;
				num2 |= 1;
			}
			uint num6 = (uint)num5;
			num2 <<= 7;
			num2 |= num6 & 0x7F;
			num6 >>= 7;
			num3 += 8;
			num2 <<= (int)(num6 + 1);
			num2 |= 1;
			num3 += num6 + 1;
			while (num3 >= 8)
			{
				num3 -= 8;
				if (outarrsrc != null)
				{
					if (num4 >= max_out_len)
					{
						return 0;
					}
					outarrsrc[outarr + num4] = (byte)(num2 >> (int)num3);
				}
				num4++;
			}
		}
		if (num3 != 0)
		{
			if (outarrsrc != null)
			{
				if (num4 >= max_out_len)
				{
					return 0;
				}
				outarrsrc[outarr + num4] = (byte)(num2 << (int)(8 - num3));
			}
			num4++;
		}
		return num4;
	}

	internal int comp_decode(short[] xsrc, int x, uint logn, byte[] inarrsrc, int inarr, int max_in_len)
	{
		int num = 1 << (int)logn;
		uint num2 = 0u;
		uint num3 = 0u;
		int num4 = 0;
		for (int i = 0; i < num; i++)
		{
			if (num4 >= max_in_len)
			{
				return 0;
			}
			num2 = (num2 << 8) | inarrsrc[inarr + num4];
			num4++;
			uint num5 = num2 >> (int)num3;
			uint num6 = num5 & 0x80;
			uint num7 = num5 & 0x7F;
			while (true)
			{
				if (num3 == 0)
				{
					if (num4 >= max_in_len)
					{
						return 0;
					}
					num2 = (num2 << 8) | inarrsrc[inarr + num4];
					num4++;
					num3 = 8u;
				}
				num3--;
				if (((num2 >> (int)num3) & 1) != 0)
				{
					break;
				}
				num7 += 128;
				if (num7 > 2047)
				{
					return 0;
				}
			}
			if (num6 != 0 && num7 == 0)
			{
				return 0;
			}
			xsrc[x + i] = (short)((num6 != 0) ? (0 - num7) : num7);
		}
		if ((num2 & (uint)((1 << (int)num3) - 1)) != 0)
		{
			return 0;
		}
		return num4;
	}
}
