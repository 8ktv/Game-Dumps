using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

internal class ReedMuller
{
	internal class Codeword
	{
		internal int[] type32;

		internal int[] type8;

		public Codeword()
		{
			type32 = new int[4];
			type8 = new int[16];
		}
	}

	private static void EncodeSub(Codeword codeword, int m)
	{
		int num = Bit0Mask(m >> 7);
		num ^= (int)(Bit0Mask(m) & 0xAAAAAAAAu);
		num ^= (int)(Bit0Mask(m >> 1) & 0xCCCCCCCCu);
		num ^= (int)(Bit0Mask(m >> 2) & 0xF0F0F0F0u);
		num ^= (int)(Bit0Mask(m >> 3) & 0xFF00FF00u);
		num ^= (int)(Bit0Mask(m >> 4) & 0xFFFF0000u);
		codeword.type32[0] = num;
		num ^= Bit0Mask(m >> 5);
		codeword.type32[1] = num;
		num ^= Bit0Mask(m >> 6);
		codeword.type32[3] = num;
		num ^= Bit0Mask(m >> 5);
		codeword.type32[2] = num;
	}

	private static void HadamardTransform(int[] srcCode, int[] desCode)
	{
		int[] array = Arrays.Clone(srcCode);
		int[] array2 = Arrays.Clone(desCode);
		for (int i = 0; i < 7; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				array2[j] = array[2 * j] + array[2 * j + 1];
				array2[j + 64] = array[2 * j] - array[2 * j + 1];
			}
			int[] array3 = array;
			array = array2;
			array2 = array3;
		}
		Array.Copy(array2, 0, srcCode, 0, srcCode.Length);
		Array.Copy(array, 0, desCode, 0, desCode.Length);
	}

	private static void ExpandThenSum(int[] desCode, Codeword[] srcCode, int off, int mulParam)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				_ = srcCode[off].type32[i];
				desCode[i * 32 + j] = (srcCode[off].type32[i] >> j) & 1;
			}
		}
		for (int k = 1; k < mulParam; k++)
		{
			for (int l = 0; l < 4; l++)
			{
				for (int m = 0; m < 32; m++)
				{
					desCode[l * 32 + m] += (srcCode[k + off].type32[l] >> m) & 1;
				}
			}
		}
	}

	private static int FindPeaks(int[] input)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < 128; i++)
		{
			int num4 = input[i];
			int num5 = ((num4 > 0) ? (-1) : 0);
			int num6 = (num5 & num4) | (~num5 & -num4);
			num2 = ((num6 > num) ? num4 : num2);
			num3 = ((num6 > num) ? i : num3);
			num = ((num6 > num) ? num6 : num);
		}
		int num7 = ((num2 > 0) ? 1 : 0);
		return num3 | (128 * num7);
	}

	private static int Bit0Mask(int b)
	{
		return (int)(-(b & 1) & 0xFFFFFFFFu);
	}

	public static void Encode(long[] codeword, byte[] m, int n1, int mulParam)
	{
		byte[] array = Arrays.Clone(m);
		Codeword[] array2 = new Codeword[n1 * mulParam];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new Codeword();
		}
		for (int j = 0; j < n1; j++)
		{
			int num = j * mulParam;
			EncodeSub(array2[num], array[j]);
			for (int k = 1; k < mulParam; k++)
			{
				array2[num + k] = array2[num];
			}
		}
		int[] array3 = new int[array2.Length * 4];
		int num2 = 0;
		for (int l = 0; l < array2.Length; l++)
		{
			Array.Copy(array2[l].type32, 0, array3, num2, array2[l].type32.Length);
			num2 += 4;
		}
		Utils.FromByte32ArrayToLongArray(codeword, array3);
	}

	public static void Decode(byte[] m, long[] codeword, int n1, int mulParam)
	{
		byte[] array = Arrays.Clone(m);
		Codeword[] array2 = new Codeword[codeword.Length / 2];
		int[] array3 = new int[codeword.Length * 2];
		Utils.FromLongArrayToByte32Array(array3, codeword);
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = new Codeword();
			for (int j = 0; j < 4; j++)
			{
				array2[i].type32[j] = array3[i * 4 + j];
			}
		}
		int[] array4 = new int[128];
		for (int k = 0; k < n1; k++)
		{
			ExpandThenSum(array4, array2, k * mulParam, mulParam);
			int[] array5 = new int[128];
			HadamardTransform(array4, array5);
			array5[0] -= 64 * mulParam;
			array[k] = (byte)FindPeaks(array5);
		}
		int[] array6 = new int[array2.Length * 4];
		int num = 0;
		for (int l = 0; l < array2.Length; l++)
		{
			Array.Copy(array2[l].type32, 0, array6, num, array2[l].type32.Length);
			num += 4;
		}
		Utils.FromByte32ArrayToLongArray(codeword, array6);
		Array.Copy(array, 0, m, 0, m.Length);
	}
}
