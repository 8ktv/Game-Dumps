using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

internal class FastFourierTransform
{
	internal static void FFT(int[] output, int[] elements, int noCoefs, int fft)
	{
		int num = 8;
		int num2 = 1 << fft;
		int[] array = new int[num2];
		int[] array2 = new int[num2];
		int[] array3 = new int[num - 1];
		int[] array4 = new int[128];
		int[] array5 = new int[128];
		int[] array6 = new int[num - 1];
		int[] array7 = new int[128];
		ComputeFFTBetas(array6, num);
		ComputeSubsetSum(array7, array6, num - 1);
		ComputeRadix(array, array2, elements, fft, fft);
		for (int i = 0; i < num - 1; i++)
		{
			array3[i] = GFCalculator.mult(array6[i], array6[i]) ^ array6[i];
		}
		ComputeFFTRec(array4, array, (noCoefs + 1) / 2, num - 1, fft - 1, array3, fft, num);
		ComputeFFTRec(array5, array2, noCoefs / 2, num - 1, fft - 1, array3, fft, num);
		int num3 = 1;
		num3 = 1 << num - 1;
		Array.Copy(array5, 0, output, num3, num3);
		output[0] = array4[0];
		output[num3] ^= array4[0];
		for (int j = 1; j < num3; j++)
		{
			output[j] = array4[j] ^ GFCalculator.mult(array7[j], array5[j]);
			output[num3 + j] ^= output[j];
		}
	}

	internal static void ComputeFFTBetas(int[] betas, int m)
	{
		for (int i = 0; i < m - 1; i++)
		{
			betas[i] = 1 << m - 1 - i;
		}
	}

	internal static void ComputeSubsetSum(int[] subsetSum, int[] set, int size)
	{
		subsetSum[0] = 0;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < 1 << i; j++)
			{
				subsetSum[(1 << i) + j] = set[i] ^ subsetSum[j];
			}
		}
	}

	internal static void ComputeRadix(int[] f0, int[] f1, int[] f, int mf, int fft)
	{
		switch (mf)
		{
		case 4:
			f0[4] = f[8] ^ f[12];
			f0[6] = f[12] ^ f[14];
			f0[7] = f[14] ^ f[15];
			f1[5] = f[11] ^ f[13];
			f1[6] = f[13] ^ f[14];
			f1[7] = f[15];
			f0[5] = f[10] ^ f[12] ^ f1[5];
			f1[4] = f[9] ^ f[13] ^ f0[5];
			f0[0] = f[0];
			f1[3] = f[7] ^ f[11] ^ f[15];
			f0[3] = f[6] ^ f[10] ^ f[14] ^ f1[3];
			f0[2] = f[4] ^ f0[4] ^ f0[3] ^ f1[3];
			f1[1] = f[3] ^ f[5] ^ f[9] ^ f[13] ^ f1[3];
			f1[2] = f[3] ^ f1[1] ^ f0[3];
			f0[1] = f[2] ^ f0[2] ^ f1[1];
			f1[0] = f[1] ^ f0[1];
			break;
		case 3:
			f0[0] = f[0];
			f0[2] = f[4] ^ f[6];
			f0[3] = f[6] ^ f[7];
			f1[1] = f[3] ^ f[5] ^ f[7];
			f1[2] = f[5] ^ f[6];
			f1[3] = f[7];
			f0[1] = f[2] ^ f0[2] ^ f1[1];
			f1[0] = f[1] ^ f0[1];
			break;
		case 2:
			f0[0] = f[0];
			f0[1] = f[2] ^ f[3];
			f1[0] = f[1] ^ f0[1];
			f1[1] = f[3];
			break;
		case 1:
			f0[0] = f[0];
			f1[0] = f[1];
			break;
		default:
			ComputeRadixBig(f0, f1, f, mf, fft);
			break;
		}
	}

	internal static void ComputeRadixBig(int[] f0, int[] f1, int[] f, int mf, int fft)
	{
		int num = 1;
		num <<= mf - 2;
		int num2 = 1 << fft - 2;
		int[] array = new int[2 * num2];
		int[] array2 = new int[2 * num2];
		int[] array3 = new int[num2];
		int[] array4 = new int[num2];
		int[] array5 = new int[num2];
		int[] array6 = new int[num2];
		Utils.CopyBytes(f, 3 * num, array, 0, 2 * num);
		Utils.CopyBytes(f, 3 * num, array, num, 2 * num);
		Utils.CopyBytes(f, 0, array2, 0, 4 * num);
		for (int i = 0; i < num; i++)
		{
			array[i] ^= f[2 * num + i];
			array2[num + i] ^= array[i];
		}
		ComputeRadix(array3, array4, array, mf - 1, fft);
		ComputeRadix(array5, array6, array2, mf - 1, fft);
		Utils.CopyBytes(array5, 0, f0, 0, 2 * num);
		Utils.CopyBytes(array3, 0, f0, num, 2 * num);
		Utils.CopyBytes(array6, 0, f1, 0, 2 * num);
		Utils.CopyBytes(array4, 0, f1, num, 2 * num);
	}

	internal static void ComputeFFTRec(int[] output, int[] func, int noCoeffs, int noOfBetas, int noCoeffsPlus, int[] betaSet, int fft, int m)
	{
		int num = 1 << fft - 2;
		int num2 = 1 << m - 2;
		int[] array = new int[num];
		int[] array2 = new int[num];
		int[] array3 = new int[m - 2];
		int[] array4 = new int[m - 2];
		int num3 = 1;
		int[] array5 = new int[num2];
		int[] array6 = new int[num2];
		int[] array7 = new int[num2];
		int[] array8 = new int[m - fft + 1];
		int num4 = 0;
		if (noCoeffsPlus == 1)
		{
			for (int i = 0; i < noOfBetas; i++)
			{
				array8[i] = GFCalculator.mult(betaSet[i], func[1]);
			}
			output[0] = func[0];
			num4 = 1;
			for (int j = 0; j < noOfBetas; j++)
			{
				for (int k = 0; k < num4; k++)
				{
					output[num4 + k] = output[k] ^ array8[j];
				}
				num4 <<= 1;
			}
			return;
		}
		if (betaSet[noOfBetas - 1] != 1)
		{
			int a = 1;
			num4 = 1;
			num4 <<= noCoeffsPlus;
			for (int l = 1; l < num4; l++)
			{
				a = GFCalculator.mult(a, betaSet[noOfBetas - 1]);
				func[l] = GFCalculator.mult(a, func[l]);
			}
		}
		ComputeRadix(array, array2, func, noCoeffsPlus, fft);
		for (int n = 0; n < noOfBetas - 1; n++)
		{
			array3[n] = GFCalculator.mult(betaSet[n], GFCalculator.inverse(betaSet[noOfBetas - 1]));
			array4[n] = GFCalculator.mult(array3[n], array3[n]) ^ array3[n];
		}
		ComputeSubsetSum(array5, array3, noOfBetas - 1);
		ComputeFFTRec(array6, array, (noCoeffs + 1) / 2, noOfBetas - 1, noCoeffsPlus - 1, array4, fft, m);
		num3 = 1;
		num3 <<= (noOfBetas - 1) & 0xF;
		if (noCoeffs <= 3)
		{
			output[0] = array6[0];
			output[num3] = array6[0] ^ array2[0];
			for (int num5 = 1; num5 < num3; num5++)
			{
				output[num5] = array6[num5] ^ GFCalculator.mult(array5[num5], array2[0]);
				output[num3 + num5] = output[num5] ^ array2[0];
			}
			return;
		}
		ComputeFFTRec(array7, array2, noCoeffs / 2, noOfBetas - 1, noCoeffsPlus - 1, array4, fft, m);
		Array.Copy(array7, 0, output, num3, num3);
		output[0] = array6[0];
		output[num3] ^= array6[0];
		for (int num6 = 1; num6 < num3; num6++)
		{
			output[num6] = array6[num6] ^ GFCalculator.mult(array5[num6], array7[num6]);
			output[num3 + num6] ^= output[num6];
		}
	}

	internal static void FastFourierTransformGetError(byte[] errorSet, int[] input, int mSize, int[] logArrays)
	{
		int num = 8;
		int num2 = 255;
		int[] array = new int[num - 1];
		int[] array2 = new int[mSize];
		ComputeFFTBetas(array, num);
		ComputeSubsetSum(array2, array, num - 1);
		errorSet[0] ^= (byte)(1 ^ Utils.ToUnsigned16Bits(-input[0] >> 15));
		errorSet[0] ^= (byte)(1 ^ Utils.ToUnsigned16Bits(-input[mSize] >> 15));
		for (int i = 1; i < mSize; i++)
		{
			int num3 = num2 - logArrays[array2[i]];
			errorSet[num3] ^= (byte)(1 ^ System.Math.Abs(-input[i] >> 15));
			num3 = num2 - logArrays[array2[i] ^ 1];
			errorSet[num3] ^= (byte)(1 ^ System.Math.Abs(-input[mSize + i] >> 15));
		}
	}
}
