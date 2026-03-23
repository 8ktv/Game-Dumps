using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

internal static class PicnicUtilities
{
	internal static void Fill(uint[] buf, int from, int to, uint b)
	{
		for (int i = from; i < to; i++)
		{
			buf[i] = b;
		}
	}

	internal static int NumBytes(int numBits)
	{
		return numBits + 7 >> 3;
	}

	internal static uint ceil_log2(uint x)
	{
		if (x != 0)
		{
			return (uint)(32 - Integers.NumberOfLeadingZeros((int)(x - 1)));
		}
		return 0u;
	}

	internal static int Parity(byte[] data, int len)
	{
		byte b = data[0];
		for (int i = 1; i < len; i++)
		{
			b ^= data[i];
		}
		return Integers.PopCount(b) & 1;
	}

	internal static uint Parity16(uint x)
	{
		return (uint)(Integers.PopCount(x & 0xFFFF) & 1);
	}

	internal static uint Parity32(uint x)
	{
		return (uint)(Integers.PopCount(x) & 1);
	}

	internal static void SetBitInWordArray(uint[] array, int bitNumber, uint val)
	{
		SetBit(array, bitNumber, val);
	}

	internal static uint GetBitFromWordArray(uint[] array, int bitNumber)
	{
		return GetBit(array, bitNumber);
	}

	internal static byte GetBit(byte[] array, int bitNumber)
	{
		int num = bitNumber >> 3;
		int num2 = (bitNumber & 7) ^ 7;
		return (byte)((array[num] >> num2) & 1);
	}

	internal static byte GetCrumbAligned(byte[] array, int crumbNumber)
	{
		int num = crumbNumber >> 2;
		int num2 = ((crumbNumber << 1) & 6) ^ 6;
		uint num3 = (uint)array[num] >> num2;
		return (byte)(((num3 & 1) << 1) | ((num3 & 2) >> 1));
	}

	internal static uint GetBit(uint word, int bitNumber)
	{
		int num = bitNumber ^ 7;
		return (word >> num) & 1;
	}

	internal static uint GetBit(uint[] array, int bitNumber)
	{
		int num = bitNumber >> 5;
		int num2 = (bitNumber & 0x1F) ^ 7;
		return (array[num] >> num2) & 1;
	}

	internal static void SetBit(byte[] array, int bitNumber, byte val)
	{
		int num = bitNumber >> 3;
		int num2 = (bitNumber & 7) ^ 7;
		uint num3 = array[num];
		num3 &= (uint)(~(1 << num2));
		num3 |= (uint)(val << num2);
		array[num] = (byte)num3;
	}

	internal static uint SetBit(uint word, int bitNumber, uint bit)
	{
		int num = bitNumber ^ 7;
		word &= (uint)(~(1 << num));
		word |= bit << num;
		return word;
	}

	internal static void SetBit(uint[] array, int bitNumber, uint val)
	{
		int num = bitNumber >> 5;
		int num2 = (bitNumber & 0x1F) ^ 7;
		uint num3 = array[num];
		num3 &= (uint)(~(1 << num2));
		num3 |= val << num2;
		array[num] = num3;
	}

	internal static void ZeroTrailingBits(uint[] data, int bitLength)
	{
		if ((bitLength & 0x1F) != 0)
		{
			data[bitLength >> 5] &= GetTrailingBitsMask(bitLength);
		}
	}

	internal static uint GetTrailingBitsMask(int bitLength)
	{
		int num = bitLength & -8;
		uint num2 = (uint)(~(-1 << num));
		int num3 = bitLength & 7;
		if (num3 != 0)
		{
			num2 ^= (uint)(((65280 >>> num3) & 0xFF) << num);
		}
		return num2;
	}
}
