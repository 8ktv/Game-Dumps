namespace Mirror.BouncyCastle.Pqc.Crypto.Saber;

internal class SaberUtilities
{
	private readonly int SABER_N;

	private readonly int SABER_L;

	private readonly int SABER_ET;

	private readonly int SABER_POLYBYTES;

	private readonly int SABER_EP;

	private readonly int SABER_KEYBYTES;

	private readonly bool usingEffectiveMasking;

	internal SaberUtilities(SaberEngine engine)
	{
		SABER_N = engine.N;
		SABER_L = engine.L;
		SABER_ET = engine.ET;
		SABER_POLYBYTES = engine.PolyBytes;
		SABER_EP = engine.EP;
		SABER_KEYBYTES = engine.KeyBytes;
		usingEffectiveMasking = engine.UsingEffectiveMasking;
	}

	public void POLT2BS(byte[] bytes, int byteIndex, short[] data)
	{
		if (SABER_ET == 3)
		{
			for (short num = 0; num < SABER_N / 8; num++)
			{
				short num2 = (short)(3 * num);
				short num3 = (short)(8 * num);
				bytes[byteIndex + num2] = (byte)((data[num3] & 7) | ((data[num3 + 1] & 7) << 3) | ((data[num3 + 2] & 3) << 6));
				bytes[byteIndex + num2 + 1] = (byte)(((data[num3 + 2] >> 2) & 1) | ((data[num3 + 3] & 7) << 1) | ((data[num3 + 4] & 7) << 4) | ((data[num3 + 5] & 1) << 7));
				bytes[byteIndex + num2 + 2] = (byte)(((data[num3 + 5] >> 1) & 3) | ((data[num3 + 6] & 7) << 2) | ((data[num3 + 7] & 7) << 5));
			}
		}
		else if (SABER_ET == 4)
		{
			for (short num = 0; num < SABER_N / 2; num++)
			{
				short num2 = num;
				short num3 = (short)(2 * num);
				bytes[byteIndex + num2] = (byte)((data[num3] & 0xF) | ((data[num3 + 1] & 0xF) << 4));
			}
		}
		else if (SABER_ET == 6)
		{
			for (short num = 0; num < SABER_N / 4; num++)
			{
				short num2 = (short)(3 * num);
				short num3 = (short)(4 * num);
				bytes[byteIndex + num2] = (byte)((data[num3] & 0x3F) | ((data[num3 + 1] & 3) << 6));
				bytes[byteIndex + num2 + 1] = (byte)(((data[num3 + 1] >> 2) & 0xF) | ((data[num3 + 2] & 0xF) << 4));
				bytes[byteIndex + num2 + 2] = (byte)(((data[num3 + 2] >> 4) & 3) | ((data[num3 + 3] & 0x3F) << 2));
			}
		}
	}

	public void BS2POLT(byte[] bytes, int byteIndex, short[] data)
	{
		if (SABER_ET == 3)
		{
			for (short num = 0; num < SABER_N / 8; num++)
			{
				short num2 = (short)(3 * num);
				short num3 = (short)(8 * num);
				data[num3] = (short)(bytes[byteIndex + num2] & 7);
				data[num3 + 1] = (short)((bytes[byteIndex + num2] >> 3) & 7);
				data[num3 + 2] = (short)(((bytes[byteIndex + num2] >> 6) & 3) | ((bytes[byteIndex + num2 + 1] & 1) << 2));
				data[num3 + 3] = (short)((bytes[byteIndex + num2 + 1] >> 1) & 7);
				data[num3 + 4] = (short)((bytes[byteIndex + num2 + 1] >> 4) & 7);
				data[num3 + 5] = (short)(((bytes[byteIndex + num2 + 1] >> 7) & 1) | ((bytes[byteIndex + num2 + 2] & 3) << 1));
				data[num3 + 6] = (short)((bytes[byteIndex + num2 + 2] >> 2) & 7);
				data[num3 + 7] = (short)((bytes[byteIndex + num2 + 2] >> 5) & 7);
			}
		}
		else if (SABER_ET == 4)
		{
			for (short num = 0; num < SABER_N / 2; num++)
			{
				short num2 = num;
				short num3 = (short)(2 * num);
				data[num3] = (short)(bytes[byteIndex + num2] & 0xF);
				data[num3 + 1] = (short)((bytes[byteIndex + num2] >> 4) & 0xF);
			}
		}
		else if (SABER_ET == 6)
		{
			for (short num = 0; num < SABER_N / 4; num++)
			{
				short num2 = (short)(3 * num);
				short num3 = (short)(4 * num);
				data[num3] = (short)(bytes[byteIndex + num2] & 0x3F);
				data[num3 + 1] = (short)(((bytes[byteIndex + num2] >> 6) & 3) | ((bytes[byteIndex + num2 + 1] & 0xF) << 2));
				data[num3 + 2] = (short)(((bytes[byteIndex + num2 + 1] & 0xFF) >> 4) | ((bytes[byteIndex + num2 + 2] & 3) << 4));
				data[num3 + 3] = (short)((bytes[byteIndex + num2 + 2] & 0xFF) >> 2);
			}
		}
	}

	private void POLq2BS(byte[] bytes, int byteIndex, short[] data)
	{
		if (!usingEffectiveMasking)
		{
			for (short num = 0; num < SABER_N / 8; num++)
			{
				short num2 = (short)(13 * num);
				short num3 = (short)(8 * num);
				bytes[byteIndex + num2] = (byte)(data[num3] & 0xFF);
				bytes[byteIndex + num2 + 1] = (byte)(((data[num3] >> 8) & 0x1F) | ((data[num3 + 1] & 7) << 5));
				bytes[byteIndex + num2 + 2] = (byte)((data[num3 + 1] >> 3) & 0xFF);
				bytes[byteIndex + num2 + 3] = (byte)(((data[num3 + 1] >> 11) & 3) | ((data[num3 + 2] & 0x3F) << 2));
				bytes[byteIndex + num2 + 4] = (byte)(((data[num3 + 2] >> 6) & 0x7F) | ((data[num3 + 3] & 1) << 7));
				bytes[byteIndex + num2 + 5] = (byte)((data[num3 + 3] >> 1) & 0xFF);
				bytes[byteIndex + num2 + 6] = (byte)(((data[num3 + 3] >> 9) & 0xF) | ((data[num3 + 4] & 0xF) << 4));
				bytes[byteIndex + num2 + 7] = (byte)((data[num3 + 4] >> 4) & 0xFF);
				bytes[byteIndex + num2 + 8] = (byte)(((data[num3 + 4] >> 12) & 1) | ((data[num3 + 5] & 0x7F) << 1));
				bytes[byteIndex + num2 + 9] = (byte)(((data[num3 + 5] >> 7) & 0x3F) | ((data[num3 + 6] & 3) << 6));
				bytes[byteIndex + num2 + 10] = (byte)((data[num3 + 6] >> 2) & 0xFF);
				bytes[byteIndex + num2 + 11] = (byte)(((data[num3 + 6] >> 10) & 7) | ((data[num3 + 7] & 0x1F) << 3));
				bytes[byteIndex + num2 + 12] = (byte)((data[num3 + 7] >> 5) & 0xFF);
			}
		}
		else
		{
			for (short num = 0; num < SABER_N / 2; num++)
			{
				short num2 = (short)(3 * num);
				short num3 = (short)(2 * num);
				bytes[byteIndex + num2] = (byte)(data[num3] & 0xFF);
				bytes[byteIndex + num2 + 1] = (byte)(((data[num3] >> 8) & 0xF) | ((data[num3 + 1] & 0xF) << 4));
				bytes[byteIndex + num2 + 2] = (byte)((data[num3 + 1] >> 4) & 0xFF);
			}
		}
	}

	private void BS2POLq(byte[] bytes, int byteIndex, short[] data)
	{
		if (!usingEffectiveMasking)
		{
			for (short num = 0; num < SABER_N / 8; num++)
			{
				short num2 = (short)(13 * num);
				short num3 = (short)(8 * num);
				data[num3] = (short)((bytes[byteIndex + num2] & 0xFF) | ((bytes[byteIndex + num2 + 1] & 0x1F) << 8));
				data[num3 + 1] = (short)(((bytes[byteIndex + num2 + 1] >> 5) & 7) | ((bytes[byteIndex + num2 + 2] & 0xFF) << 3) | ((bytes[byteIndex + num2 + 3] & 3) << 11));
				data[num3 + 2] = (short)(((bytes[byteIndex + num2 + 3] >> 2) & 0x3F) | ((bytes[byteIndex + num2 + 4] & 0x7F) << 6));
				data[num3 + 3] = (short)(((bytes[byteIndex + num2 + 4] >> 7) & 1) | ((bytes[byteIndex + num2 + 5] & 0xFF) << 1) | ((bytes[byteIndex + num2 + 6] & 0xF) << 9));
				data[num3 + 4] = (short)(((bytes[byteIndex + num2 + 6] >> 4) & 0xF) | ((bytes[byteIndex + num2 + 7] & 0xFF) << 4) | ((bytes[byteIndex + num2 + 8] & 1) << 12));
				data[num3 + 5] = (short)(((bytes[byteIndex + num2 + 8] >> 1) & 0x7F) | ((bytes[byteIndex + num2 + 9] & 0x3F) << 7));
				data[num3 + 6] = (short)(((bytes[byteIndex + num2 + 9] >> 6) & 3) | ((bytes[byteIndex + num2 + 10] & 0xFF) << 2) | ((bytes[byteIndex + num2 + 11] & 7) << 10));
				data[num3 + 7] = (short)(((bytes[byteIndex + num2 + 11] >> 3) & 0x1F) | ((bytes[byteIndex + num2 + 12] & 0xFF) << 5));
			}
		}
		else
		{
			for (short num = 0; num < SABER_N / 2; num++)
			{
				short num2 = (short)(3 * num);
				short num3 = (short)(2 * num);
				data[num3] = (short)((bytes[byteIndex + num2] & 0xFF) | ((bytes[byteIndex + num2 + 1] & 0xF) << 8));
				data[num3 + 1] = (short)(((bytes[byteIndex + num2 + 1] >> 4) & 0xF) | ((bytes[byteIndex + num2 + 2] & 0xFF) << 4));
			}
		}
	}

	private void POLp2BS(byte[] bytes, int byteIndex, short[] data)
	{
		for (short num = 0; num < SABER_N / 4; num++)
		{
			short num2 = (short)(5 * num);
			short num3 = (short)(4 * num);
			bytes[byteIndex + num2] = (byte)(data[num3] & 0xFF);
			bytes[byteIndex + num2 + 1] = (byte)(((data[num3] >> 8) & 3) | ((data[num3 + 1] & 0x3F) << 2));
			bytes[byteIndex + num2 + 2] = (byte)(((data[num3 + 1] >> 6) & 0xF) | ((data[num3 + 2] & 0xF) << 4));
			bytes[byteIndex + num2 + 3] = (byte)(((data[num3 + 2] >> 4) & 0x3F) | ((data[num3 + 3] & 3) << 6));
			bytes[byteIndex + num2 + 4] = (byte)((data[num3 + 3] >> 2) & 0xFF);
		}
	}

	public void BS2POLp(byte[] bytes, int byteIndex, short[] data)
	{
		for (short num = 0; num < SABER_N / 4; num++)
		{
			short num2 = (short)(5 * num);
			short num3 = (short)(4 * num);
			data[num3] = (short)((bytes[byteIndex + num2] & 0xFF) | ((bytes[byteIndex + num2 + 1] & 3) << 8));
			data[num3 + 1] = (short)(((bytes[byteIndex + num2 + 1] >> 2) & 0x3F) | ((bytes[byteIndex + num2 + 2] & 0xF) << 6));
			data[num3 + 2] = (short)(((bytes[byteIndex + num2 + 2] >> 4) & 0xF) | ((bytes[byteIndex + num2 + 3] & 0x3F) << 4));
			data[num3 + 3] = (short)(((bytes[byteIndex + num2 + 3] >> 6) & 3) | ((bytes[byteIndex + num2 + 4] & 0xFF) << 2));
		}
	}

	public void POLVECq2BS(byte[] bytes, short[][] data)
	{
		for (byte b = 0; b < SABER_L; b++)
		{
			POLq2BS(bytes, b * SABER_POLYBYTES, data[b]);
		}
	}

	public void BS2POLVECq(byte[] bytes, int byteIndex, short[][] data)
	{
		for (byte b = 0; b < SABER_L; b++)
		{
			BS2POLq(bytes, byteIndex + b * SABER_POLYBYTES, data[b]);
		}
	}

	public void POLVECp2BS(byte[] bytes, short[][] data)
	{
		for (byte b = 0; b < SABER_L; b++)
		{
			POLp2BS(bytes, b * (SABER_EP * SABER_N / 8), data[b]);
		}
	}

	public void BS2POLVECp(byte[] bytes, short[][] data)
	{
		for (byte b = 0; b < SABER_L; b++)
		{
			BS2POLp(bytes, b * (SABER_EP * SABER_N / 8), data[b]);
		}
	}

	public void BS2POLmsg(byte[] bytes, short[] data)
	{
		for (byte b = 0; b < SABER_KEYBYTES; b++)
		{
			for (byte b2 = 0; b2 < 8; b2++)
			{
				data[b * 8 + b2] = (short)((bytes[b] >> (int)b2) & 1);
			}
		}
	}

	public void POLmsg2BS(byte[] bytes, short[] data)
	{
		for (byte b = 0; b < SABER_KEYBYTES; b++)
		{
			for (byte b2 = 0; b2 < 8; b2++)
			{
				bytes[b] = (byte)(bytes[b] | ((data[b * 8 + b2] & 1) << (int)b2));
			}
		}
	}
}
