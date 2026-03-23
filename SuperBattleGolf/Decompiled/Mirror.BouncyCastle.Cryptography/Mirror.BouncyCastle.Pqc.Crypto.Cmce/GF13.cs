using System.Runtime.InteropServices;
using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct GF13 : GF
{
	public void GFMulPoly(int length, int[] poly, ushort[] output, ushort[] left, ushort[] right, uint[] temp)
	{
		temp[0] = GFMulExt(left[0], right[0]);
		for (int i = 1; i < length; i++)
		{
			temp[i + i - 1] = 0u;
			ushort num = left[i];
			ushort num2 = right[i];
			for (int j = 0; j < i; j++)
			{
				temp[i + j] ^= GFMulExtPar(num, right[j], left[j], num2);
			}
			temp[i + i] = GFMulExt(num, num2);
		}
		for (int num3 = (length - 1) * 2; num3 >= length; num3--)
		{
			uint num4 = temp[num3];
			for (int k = 0; k < poly.Length; k++)
			{
				temp[num3 - length + poly[k]] ^= num4;
			}
		}
		for (int l = 0; l < length; l++)
		{
			output[l] = GFReduce(temp[l]);
		}
	}

	public void GFSqrPoly(int length, int[] poly, ushort[] output, ushort[] input, uint[] temp)
	{
		temp[0] = GFSqExt(input[0]);
		for (int i = 1; i < length; i++)
		{
			temp[i + i - 1] = 0u;
			temp[i + i] = GFSqExt(input[i]);
		}
		for (int num = (length - 1) * 2; num >= length; num--)
		{
			uint num2 = temp[num];
			for (int j = 0; j < poly.Length; j++)
			{
				temp[num - length + poly[j]] ^= num2;
			}
		}
		for (int k = 0; k < length; k++)
		{
			output[k] = GFReduce(temp[k]);
		}
	}

	public ushort GFFrac(ushort den, ushort num)
	{
		ushort num2 = GFSqMul(den, den);
		ushort num3 = GFSq2Mul(num2, num2);
		ushort input = GFSq2(num3);
		input = GFSq2Mul(input, num3);
		input = GFSq2(input);
		input = GFSq2Mul(input, num3);
		return GFSqMul(input, num);
	}

	public ushort GFInv(ushort den)
	{
		return GFFrac(den, 1);
	}

	public ushort GFIsZero(ushort a)
	{
		return (ushort)(a - 1 >> 31);
	}

	public ushort GFMul(ushort in0, ushort in1)
	{
		int num = in0 * (in1 & 1);
		for (int i = 1; i < 13; i++)
		{
			num ^= in0 * (in1 & (1 << i));
		}
		return GFReduce((uint)num);
	}

	public uint GFMulExt(ushort left, ushort right)
	{
		int num = left * (right & 1);
		for (int i = 1; i < 13; i++)
		{
			num ^= left * (right & (1 << i));
		}
		return (uint)num;
	}

	private uint GFMulExtPar(ushort left0, ushort right0, ushort left1, ushort right1)
	{
		int num = left0 * (right0 & 1);
		int num2 = left1 * (right1 & 1);
		for (int i = 1; i < 13; i++)
		{
			num ^= left0 * (right0 & (1 << i));
			num2 ^= left1 * (right1 & (1 << i));
		}
		return (uint)(num ^ num2);
	}

	public ushort GFReduce(uint x)
	{
		uint num = x & 0x1FFF;
		uint num2 = x >> 13;
		uint num3 = (num2 << 4) ^ (num2 << 3) ^ (num2 << 1);
		uint num4 = num3 >> 13;
		uint num5 = num3 & 0x1FFF;
		uint num6 = (num4 << 4) ^ (num4 << 3) ^ (num4 << 1);
		return (ushort)(num ^ num2 ^ num4 ^ num5 ^ num6);
	}

	public ushort GFSq(ushort input)
	{
		uint x = Interleave.Expand16to32(input);
		return GFReduce(x);
	}

	public uint GFSqExt(ushort input)
	{
		return Interleave.Expand16to32(input);
	}

	private ushort GFSq2(ushort input)
	{
		uint x = Interleave.Expand16to32(input);
		input = GFReduce(x);
		uint x2 = Interleave.Expand16to32(input);
		return GFReduce(x2);
	}

	private ushort GFSqMul(ushort input, ushort m)
	{
		long num = input;
		long num2 = m;
		long num3 = (num2 << 6) * (num & 0x40);
		num ^= num << 7;
		num3 ^= num2 * (num & 0x4001);
		num3 ^= (num2 << 1) * (num & 0x8002);
		num3 ^= (num2 << 2) * (num & 0x10004);
		num3 ^= (num2 << 3) * (num & 0x20008);
		num3 ^= (num2 << 4) * (num & 0x40010);
		num3 ^= (num2 << 5) * (num & 0x80020);
		long num4 = num3 & 0x1FFC000000L;
		num3 ^= (num4 >> 18) ^ (num4 >> 20) ^ (num4 >> 24) ^ (num4 >> 26);
		return GFReduce((uint)((int)num3 & 0x3FFFFFF));
	}

	private ushort GFSq2Mul(ushort input, ushort m)
	{
		long num = input;
		long num2 = m;
		long num3 = (num2 << 18) * (num & 0x40);
		num ^= num << 21;
		num3 ^= num2 * (num & 0x10000001);
		num3 ^= (num2 << 3) * (num & 0x20000002);
		num3 ^= (num2 << 6) * (num & 0x40000004);
		num3 ^= (num2 << 9) * (num & 0x80000008u);
		num3 ^= (num2 << 12) * (num & 0x100000010L);
		num3 ^= (num2 << 15) * (num & 0x200000020L);
		long num4 = num3 & 0x1FFFF80000000000L;
		num3 ^= (num4 >> 18) ^ (num4 >> 20) ^ (num4 >> 24) ^ (num4 >> 26);
		num4 = num3 & 0x7FFFC000000L;
		num3 ^= (num4 >> 18) ^ (num4 >> 20) ^ (num4 >> 24) ^ (num4 >> 26);
		return GFReduce((uint)((int)num3 & 0x3FFFFFF));
	}
}
