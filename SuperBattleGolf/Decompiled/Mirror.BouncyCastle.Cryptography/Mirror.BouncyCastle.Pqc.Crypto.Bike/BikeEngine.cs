using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Bike;

internal sealed class BikeEngine
{
	private readonly int r;

	private readonly int w;

	private readonly int hw;

	private readonly int t;

	private readonly int nbIter;

	private readonly int tau;

	private readonly BikeRing bikeRing;

	private readonly int L_BYTE;

	private readonly int R_BYTE;

	private readonly int R2_UINT;

	private readonly int R_ULONG;

	private readonly int R2_ULONG;

	internal int SessionKeySize => L_BYTE;

	internal BikeEngine(int r, int w, int t, int l, int nbIter, int tau)
	{
		this.r = r;
		this.w = w;
		this.t = t;
		this.nbIter = nbIter;
		this.tau = tau;
		hw = this.w / 2;
		L_BYTE = l / 8;
		R_BYTE = r + 7 >> 3;
		R2_UINT = 2 * r + 31 >> 5;
		R_ULONG = r + 63 >> 6;
		R2_ULONG = 2 * r + 63 >> 6;
		bikeRing = new BikeRing(r);
	}

	private ulong[] FunctionH(byte[] seed)
	{
		IXof xof = new ShakeDigest(256);
		xof.BlockUpdate(seed, 0, seed.Length);
		ulong[] array = new ulong[2 * R_ULONG];
		BikeUtilities.GenerateRandomUlongs(array, 2 * r, t, xof);
		return array;
	}

	private void FunctionL(ulong[] e01, byte[] c1, int c1Off)
	{
		byte[] array = new byte[48];
		Sha3Digest.CalculateDigest(e01, 0, 16 * R_BYTE, array, 0, 384);
		Array.Copy(array, 0, c1, c1Off, L_BYTE);
	}

	private void FunctionK(byte[] m, byte[] c01, byte[] result)
	{
		byte[] array = new byte[48];
		Sha3Digest sha3Digest = new Sha3Digest(384);
		sha3Digest.BlockUpdate(m, 0, m.Length);
		sha3Digest.BlockUpdate(c01, 0, c01.Length);
		sha3Digest.DoFinal(array, 0);
		Array.Copy(array, 0, result, 0, L_BYTE);
	}

	private void FunctionK(byte[] m, byte[] c0, byte[] c1, byte[] result)
	{
		byte[] array = new byte[48];
		Sha3Digest sha3Digest = new Sha3Digest(384);
		sha3Digest.BlockUpdate(m, 0, m.Length);
		sha3Digest.BlockUpdate(c0, 0, c0.Length);
		sha3Digest.BlockUpdate(c1, 0, c1.Length);
		sha3Digest.DoFinal(array, 0);
		Array.Copy(array, 0, result, 0, L_BYTE);
	}

	internal void GenKeyPair(byte[] h0, byte[] h1, byte[] sigma, byte[] h, SecureRandom random)
	{
		byte[] array = new byte[64];
		random.NextBytes(array);
		IXof xof = new ShakeDigest(256);
		xof.BlockUpdate(array, 0, L_BYTE);
		ulong[] array2 = bikeRing.Create();
		ulong[] array3 = bikeRing.Create();
		BikeUtilities.GenerateRandomUlongs(array2, r, hw, xof);
		BikeUtilities.GenerateRandomUlongs(array3, r, hw, xof);
		bikeRing.EncodeBytes(array2, h0);
		bikeRing.EncodeBytes(array3, h1);
		bikeRing.Inv(array2, array2);
		bikeRing.Multiply(array2, array3, array2);
		bikeRing.EncodeBytes(array2, h);
		Array.Copy(array, L_BYTE, sigma, 0, sigma.Length);
	}

	internal void Encaps(byte[] c01, byte[] k, byte[] h, SecureRandom random)
	{
		byte[] array = new byte[L_BYTE];
		random.NextBytes(array);
		ulong[] array2 = FunctionH(array);
		AlignE01From1To64(array2);
		ulong[] array3 = bikeRing.Create();
		bikeRing.DecodeBytes(h, array3);
		bikeRing.Multiply(array3, 0, array2, R_ULONG, array3);
		bikeRing.Add(array3, array2, array3);
		bikeRing.EncodeBytes(array3, c01);
		AlignE01From64To8(array2);
		FunctionL(array2, c01, R_BYTE);
		Bytes.XorTo(L_BYTE, array, 0, c01, R_BYTE);
		FunctionK(array, c01, k);
	}

	internal void Decaps(byte[] k, byte[] h0, byte[] h1, byte[] sigma, byte[] c0, byte[] c1)
	{
		int[] array = new int[hw];
		int[] array2 = new int[hw];
		ConvertToCompact(array, h0);
		ConvertToCompact(array2, h1);
		byte[] s = ComputeSyndrome(c0, h0);
		byte[] input = BGFDecoder(s, array, array2);
		ulong[] array3 = new ulong[2 * R_ULONG];
		BikeUtilities.FromBitsToUlongs(array3, input, 0, 2 * r);
		AlignE01From1To64(array3);
		AlignE01From64To8(array3);
		byte[] array4 = new byte[L_BYTE];
		FunctionL(array3, array4, 0);
		Bytes.XorTo(L_BYTE, c1, array4);
		AlignE01From8To1(array3);
		ulong[] b = FunctionH(array4);
		if (Arrays.AreEqual(array3, 0, R2_ULONG, b, 0, R2_ULONG))
		{
			FunctionK(array4, c0, c1, k);
		}
		else
		{
			FunctionK(sigma, c0, c1, k);
		}
	}

	private byte[] ComputeSyndrome(byte[] c0, byte[] h0)
	{
		ulong[] array = bikeRing.Create();
		ulong[] array2 = bikeRing.Create();
		bikeRing.DecodeBytes(c0, array);
		bikeRing.DecodeBytes(h0, array2);
		bikeRing.Multiply(array, array2, array);
		return bikeRing.EncodeBitsTransposed(array);
	}

	private byte[] BGFDecoder(byte[] s, int[] h0Compact, int[] h1Compact)
	{
		byte[] array = new byte[2 * r];
		int[] columnFromCompactVersion = GetColumnFromCompactVersion(h0Compact);
		int[] columnFromCompactVersion2 = GetColumnFromCompactVersion(h1Compact);
		uint[] array2 = new uint[R2_UINT];
		byte[] ctrs = new byte[r];
		uint[] array3 = new uint[R2_UINT];
		int num = Threshold(BikeUtilities.GetHammingWeight(s), r);
		BFIter(s, array, num, h0Compact, h1Compact, columnFromCompactVersion, columnFromCompactVersion2, array2, array3, ctrs);
		BFMaskedIter(s, array, array2, (hw + 3) / 2, h0Compact, h1Compact, columnFromCompactVersion, columnFromCompactVersion2);
		BFMaskedIter(s, array, array3, (hw + 3) / 2, h0Compact, h1Compact, columnFromCompactVersion, columnFromCompactVersion2);
		for (int i = 1; i < nbIter; i++)
		{
			Array.Clear(array2, 0, array2.Length);
			int num2 = Threshold(BikeUtilities.GetHammingWeight(s), r);
			BFIter2(s, array, num2, h0Compact, h1Compact, columnFromCompactVersion, columnFromCompactVersion2, array2, ctrs);
		}
		if (BikeUtilities.GetHammingWeight(s) == 0)
		{
			return array;
		}
		return null;
	}

	private void BFIter(byte[] s, byte[] e, int T, int[] h0Compact, int[] h1Compact, int[] h0CompactCol, int[] h1CompactCol, uint[] black, uint[] gray, byte[] ctrs)
	{
		CtrAll(h0CompactCol, s, ctrs);
		int num = (ctrs[0] - T >> 31) + 1;
		int num2 = (ctrs[0] - (T - tau) >> 31) + 1;
		e[0] ^= (byte)num;
		black[0] |= (uint)num;
		gray[0] |= (uint)num2;
		for (int i = 1; i < r; i++)
		{
			int num3 = (ctrs[i] - T >> 31) + 1;
			int num4 = (ctrs[i] - (T - tau) >> 31) + 1;
			e[r - i] ^= (byte)num3;
			black[i >> 5] |= (uint)(num3 << i);
			gray[i >> 5] |= (uint)(num4 << i);
		}
		CtrAll(h1CompactCol, s, ctrs);
		int num5 = (ctrs[0] - T >> 31) + 1;
		int num6 = (ctrs[0] - (T - tau) >> 31) + 1;
		e[r] ^= (byte)num5;
		black[r >> 5] |= (uint)(num5 << r);
		gray[r >> 5] |= (uint)(num6 << r);
		for (int j = 1; j < r; j++)
		{
			int num7 = (ctrs[j] - T >> 31) + 1;
			int num8 = (ctrs[j] - (T - tau) >> 31) + 1;
			e[r + r - j] ^= (byte)num7;
			black[r + j >> 5] |= (uint)(num7 << r + j);
			gray[r + j >> 5] |= (uint)(num8 << r + j);
		}
		for (int k = 0; k < black.Length; k++)
		{
			uint num9 = black[k];
			while (num9 != 0)
			{
				int num10 = Integers.NumberOfTrailingZeros((int)num9);
				RecomputeSyndrome(s, (k << 5) + num10, h0Compact, h1Compact);
				num9 ^= (uint)(1 << num10);
			}
		}
	}

	private void BFIter2(byte[] s, byte[] e, int T, int[] h0Compact, int[] h1Compact, int[] h0CompactCol, int[] h1CompactCol, uint[] black, byte[] ctrs)
	{
		CtrAll(h0CompactCol, s, ctrs);
		int num = (ctrs[0] - T >> 31) + 1;
		e[0] ^= (byte)num;
		black[0] |= (uint)num;
		for (int i = 1; i < r; i++)
		{
			int num2 = (ctrs[i] - T >> 31) + 1;
			e[r - i] ^= (byte)num2;
			black[i >> 5] |= (uint)(num2 << i);
		}
		CtrAll(h1CompactCol, s, ctrs);
		int num3 = (ctrs[0] - T >> 31) + 1;
		e[r] ^= (byte)num3;
		black[r >> 5] |= (uint)(num3 << r);
		for (int j = 1; j < r; j++)
		{
			int num4 = (ctrs[j] - T >> 31) + 1;
			e[r + r - j] ^= (byte)num4;
			black[r + j >> 5] |= (uint)(num4 << r + j);
		}
		for (int k = 0; k < black.Length; k++)
		{
			uint num5 = black[k];
			while (num5 != 0)
			{
				int num6 = Integers.NumberOfTrailingZeros((int)num5);
				RecomputeSyndrome(s, (k << 5) + num6, h0Compact, h1Compact);
				num5 ^= (uint)(1 << num6);
			}
		}
	}

	private void BFMaskedIter(byte[] s, byte[] e, uint[] mask, int T, int[] h0Compact, int[] h1Compact, int[] h0CompactCol, int[] h1CompactCol)
	{
		uint[] array = new uint[R2_UINT];
		for (int i = 0; i < r; i++)
		{
			if ((mask[i >> 5] & (uint)(1 << i)) != 0)
			{
				int num = (Ctr(h0CompactCol, s, i) - T >> 31) + 1;
				int num2 = -i;
				num2 += (num2 >> 31) & r;
				e[num2] ^= (byte)num;
				array[i >> 5] |= (uint)(num << i);
			}
		}
		for (int j = 0; j < r; j++)
		{
			if ((mask[r + j >> 5] & (uint)(1 << r + j)) != 0)
			{
				int num3 = (Ctr(h1CompactCol, s, j) - T >> 31) + 1;
				int num4 = -j;
				num4 += (num4 >> 31) & r;
				e[r + num4] ^= (byte)num3;
				array[r + j >> 5] |= (uint)(num3 << r + j);
			}
		}
		for (int k = 0; k < array.Length; k++)
		{
			uint num5 = array[k];
			while (num5 != 0)
			{
				int num6 = Integers.NumberOfTrailingZeros((int)num5);
				RecomputeSyndrome(s, (k << 5) + num6, h0Compact, h1Compact);
				num5 ^= (uint)(1 << num6);
			}
		}
	}

	private static int Threshold(int hammingWeight, int r)
	{
		return r switch
		{
			12323 => ThresholdFromParameters(hammingWeight, 0.0069722, 13.53, 36), 
			24659 => ThresholdFromParameters(hammingWeight, 0.005265, 15.2588, 52), 
			40973 => ThresholdFromParameters(hammingWeight, 0.00402312, 17.8785, 69), 
			_ => throw new ArgumentException(), 
		};
	}

	private static int ThresholdFromParameters(int hammingWeight, double dm, double da, int min)
	{
		return System.Math.Max(min, Convert.ToInt32(System.Math.Floor(dm * (double)hammingWeight + da)));
	}

	private int Ctr(int[] hCompactCol, byte[] s, int j)
	{
		int num = 0;
		int i = 0;
		for (int num2 = hw - 4; i <= num2; i += 4)
		{
			int num3 = hCompactCol[i] + j - r;
			int num4 = hCompactCol[i + 1] + j - r;
			int num5 = hCompactCol[i + 2] + j - r;
			int num6 = hCompactCol[i + 3] + j - r;
			num3 += (num3 >> 31) & r;
			num4 += (num4 >> 31) & r;
			num5 += (num5 >> 31) & r;
			num6 += (num6 >> 31) & r;
			num += s[num3];
			num += s[num4];
			num += s[num5];
			num += s[num6];
		}
		for (; i < hw; i++)
		{
			int num7 = hCompactCol[i] + j - r;
			num7 += (num7 >> 31) & r;
			num += s[num7];
		}
		return num;
	}

	private void CtrAll(int[] hCompactCol, byte[] s, byte[] ctrs)
	{
		int num = hCompactCol[0];
		int num2 = r - num;
		Array.Copy(s, num, ctrs, 0, num2);
		Array.Copy(s, 0, ctrs, num2, num);
		for (int i = 1; i < hw; i++)
		{
			int num3 = hCompactCol[i];
			int num4 = r - num3;
			int j = 0;
			for (int num5 = num4 - 4; j <= num5; j += 4)
			{
				ctrs[j] += s[num3 + j];
				ctrs[j + 1] += s[num3 + j + 1];
				ctrs[j + 2] += s[num3 + j + 2];
				ctrs[j + 3] += s[num3 + j + 3];
			}
			for (; j < num4; j++)
			{
				ctrs[j] += s[num3 + j];
			}
			int k = num4;
			for (int num6 = r - 4; k <= num6; k += 4)
			{
				ctrs[k] += s[k - num4];
				ctrs[k + 1] += s[k + 1 - num4];
				ctrs[k + 2] += s[k + 2 - num4];
				ctrs[k + 3] += s[k + 3 - num4];
			}
			for (; k < r; k++)
			{
				ctrs[k] += s[k - num4];
			}
		}
	}

	private void ConvertToCompact(int[] compactVersion, byte[] h)
	{
		int num = 0;
		for (int i = 0; i < R_BYTE; i++)
		{
			for (int j = 0; j < 8 && i * 8 + j != r; j++)
			{
				int num2 = (h[i] >> j) & 1;
				compactVersion[num] = ((i * 8 + j) & -num2) | (compactVersion[num] & ~(-num2));
				num += num2 - hw;
				num += (num >> 31) & hw;
			}
		}
	}

	private int[] GetColumnFromCompactVersion(int[] hCompact)
	{
		int[] array = new int[hw];
		if (hCompact[0] == 0)
		{
			array[0] = 0;
			for (int i = 1; i < hw; i++)
			{
				array[i] = r - hCompact[hw - i];
			}
		}
		else
		{
			for (int j = 0; j < hw; j++)
			{
				array[j] = r - hCompact[hw - 1 - j];
			}
		}
		return array;
	}

	private void RecomputeSyndrome(byte[] syndrome, int index, int[] h0Compact, int[] h1Compact)
	{
		if (index < r)
		{
			for (int i = 0; i < hw; i++)
			{
				if (h0Compact[i] <= index)
				{
					syndrome[index - h0Compact[i]] ^= 1;
				}
				else
				{
					syndrome[r + index - h0Compact[i]] ^= 1;
				}
			}
			return;
		}
		for (int j = 0; j < hw; j++)
		{
			if (h1Compact[j] <= index - r)
			{
				syndrome[index - r - h1Compact[j]] ^= 1;
			}
			else
			{
				syndrome[r - h1Compact[j] + (index - r)] ^= 1;
			}
		}
	}

	private void AlignE01From1To64(ulong[] e01)
	{
		int num = r & 0x3F;
		int bits = 64 - num;
		ulong num2 = (ulong)(-1L << num);
		ulong num3 = e01[R_ULONG - 1];
		ulong c = num3 & num2;
		Nat.ShiftUpBits64(R_ULONG, e01, R_ULONG, bits, c);
		e01[R_ULONG - 1] = num3 & ~num2;
	}

	private void AlignE01From64To8(ulong[] e01)
	{
		int num = (8 * R_BYTE) & 0x3F;
		int bits = 64 - num;
		ulong num2 = Nat.ShiftDownBits64(R_ULONG, e01, R_ULONG, bits, 0uL);
		e01[R_ULONG - 1] |= num2;
	}

	private void AlignE01From8To1(ulong[] e01)
	{
		int num = r & 0x3F;
		int num2 = 8 * R_BYTE - r;
		ulong num3 = (ulong)(-1L << num);
		ulong num4 = e01[R_ULONG - 1];
		ulong num5 = Nat.ShiftDownBits64(R_ULONG, e01, R_ULONG, num2, 0uL);
		e01[R_ULONG - 1] = (num4 & ~num3) | ((num4 >> num2) & num3) | num5;
	}
}
