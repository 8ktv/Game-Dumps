using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Bike;

internal class BikeUtilities
{
	internal static int GetHammingWeight(byte[] bytes)
	{
		int num = 0;
		for (int i = 0; i < bytes.Length; i++)
		{
			num += bytes[i];
		}
		return num;
	}

	internal static void FromBitsToUlongs(ulong[] output, byte[] input, int inputOff, int inputLen)
	{
		for (int i = 0; i < inputLen; i++)
		{
			ulong num = (ulong)input[inputOff + i] & 1uL;
			output[i >> 6] |= num << i;
		}
	}

	internal static void GenerateRandomUlongs(ulong[] res, int size, int weight, IXof digest)
	{
		byte[] array = new byte[4];
		for (int num = weight - 1; num >= 0; num--)
		{
			digest.Output(array, 0, 4);
			ulong num2 = Pack.LE_To_UInt32(array, 0);
			num2 *= (uint)(size - num);
			uint position = (uint)(num + (int)(num2 >> 32));
			if (CheckBit(res, position))
			{
				position = (uint)num;
			}
			SetBit(res, position);
		}
	}

	private static bool CheckBit(ulong[] tmp, uint position)
	{
		uint num = position >> 6;
		uint num2 = position & 0x3F;
		return ((tmp[num] >> (int)num2) & 1) != 0;
	}

	private static void SetBit(ulong[] tmp, uint position)
	{
		uint num = position >> 6;
		uint num2 = position & 0x3F;
		tmp[num] |= (ulong)(1L << (int)num2);
	}
}
