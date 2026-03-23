using System;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

internal class Utils
{
	internal static void ResizeArray(long[] output, int sizeOutBits, long[] input, int sizeInBits, int n1n2ByteSize, int n1n2Byte64Size)
	{
		long num = long.MaxValue;
		int num2 = 0;
		if (sizeOutBits < sizeInBits)
		{
			if (sizeOutBits % 64 != 0)
			{
				num2 = 64 - sizeOutBits % 64;
			}
			Array.Copy(input, 0, output, 0, n1n2ByteSize);
			for (int i = 0; i < num2; i++)
			{
				output[n1n2Byte64Size - 1] &= num >> i;
			}
		}
		else
		{
			Array.Copy(input, 0, output, 0, (sizeInBits + 7) / 8);
		}
	}

	internal static void FromLongArrayToByteArray(byte[] output, long[] input)
	{
		int num = output.Length / 8;
		for (int i = 0; i != num; i++)
		{
			Pack.UInt64_To_LE((ulong)input[i], output, i * 8);
		}
		if (output.Length % 8 != 0)
		{
			int num2 = num * 8;
			int num3 = 0;
			while (num2 < output.Length)
			{
				output[num2++] = (byte)(input[num] >> num3++ * 8);
			}
		}
	}

	internal static long BitMask(ulong a, ulong b)
	{
		uint num = (uint)(a % b);
		return (1L << (int)num) - 1;
	}

	internal static void FromByteArrayToLongArray(long[] output, byte[] input)
	{
		byte[] array = input;
		if (input.Length % 8 != 0)
		{
			array = new byte[(input.Length + 7) / 8 * 8];
			Array.Copy(input, 0, array, 0, input.Length);
		}
		int num = 0;
		for (int i = 0; i < output.Length; i++)
		{
			output[i] = (long)Pack.LE_To_UInt64(array, num);
			num += 8;
		}
	}

	internal static void FromByteArrayToByte16Array(int[] output, byte[] input)
	{
		byte[] array = input;
		if (input.Length % 2 != 0)
		{
			array = new byte[(input.Length + 1) / 2 * 2];
			Array.Copy(input, 0, array, 0, input.Length);
		}
		int num = 0;
		for (int i = 0; i < output.Length; i++)
		{
			output[i] = Pack.LE_To_UInt16(array, num);
			num += 2;
		}
	}

	internal static void FromByte32ArrayToLongArray(long[] output, int[] input)
	{
		for (int i = 0; i != input.Length; i += 2)
		{
			output[i / 2] = (uint)input[i];
			output[i / 2] |= (long)input[i + 1] << 32;
		}
	}

	internal static void FromByte16ArrayToULongArray(ulong[] output, ushort[] input)
	{
		for (int i = 0; i != input.Length; i += 4)
		{
			output[i / 4] = input[i];
			output[i / 4] |= (ulong)input[i + 1] << 16;
			output[i / 4] |= (ulong)input[i + 2] << 32;
			output[i / 4] |= (ulong)input[i + 3] << 48;
		}
	}

	internal static void FromLongArrayToByte32Array(int[] output, long[] input)
	{
		for (int i = 0; i != input.Length; i++)
		{
			output[2 * i] = (int)input[i];
			output[2 * i + 1] = (int)(input[i] >> 32);
		}
	}

	internal static void CopyBytes(int[] src, int offsetSrc, int[] dst, int offsetDst, int lengthBytes)
	{
		Array.Copy(src, offsetSrc, dst, offsetDst, lengthBytes / 2);
	}

	internal static int GetByteSizeFromBitSize(int size)
	{
		return (size + 7) / 8;
	}

	internal static int GetByte64SizeFromBitSize(int size)
	{
		return (size + 63) / 64;
	}

	internal static int ToUnsigned8bits(int a)
	{
		return a & 0xFF;
	}

	internal static int ToUnsigned16Bits(int a)
	{
		return a & 0xFFFF;
	}

	internal static void XorULongToByte16Array(ushort[] output, int outOff, ulong input)
	{
		output[outOff] ^= (ushort)input;
		output[outOff + 1] ^= (ushort)(input >> 16);
		output[outOff + 2] ^= (ushort)(input >> 32);
		output[outOff + 3] ^= (ushort)(input >> 48);
	}
}
