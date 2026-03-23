using System.Runtime.InteropServices;
using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct GF12 : GF
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
			for (int k = 0; k < poly.Length - 1; k++)
			{
				temp[num3 - length + poly[k]] ^= num4;
			}
			temp[num3 - length] ^= num4 << 1;
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
			for (int j = 0; j < poly.Length - 1; j++)
			{
				temp[num - length + poly[j]] ^= num2;
			}
			temp[num - length] ^= num2 << 1;
		}
		for (int k = 0; k < length; k++)
		{
			output[k] = GFReduce(temp[k]);
		}
	}

	public ushort GFFrac(ushort den, ushort num)
	{
		return GFMul(GFInv(den), num);
	}

	public ushort GFInv(ushort input)
	{
		ushort input2 = input;
		input2 = GFSq(input2);
		ushort num = GFMul(input2, input);
		input2 = GFSq(num);
		input2 = GFSq(input2);
		ushort num2 = GFMul(input2, num);
		input2 = GFSq(num2);
		input2 = GFSq(input2);
		input2 = GFSq(input2);
		input2 = GFSq(input2);
		input2 = GFMul(input2, num2);
		input2 = GFSq(input2);
		input2 = GFSq(input2);
		input2 = GFMul(input2, num);
		input2 = GFSq(input2);
		input2 = GFMul(input2, input);
		return GFSq(input2);
	}

	public ushort GFIsZero(ushort a)
	{
		return (ushort)(a - 1 >> 31);
	}

	public ushort GFMul(ushort left, ushort right)
	{
		int num = left * (right & 1);
		for (int i = 1; i < 12; i++)
		{
			num ^= left * (right & (1 << i));
		}
		return GFReduce((uint)num);
	}

	public uint GFMulExt(ushort left, ushort right)
	{
		int num = left * (right & 1);
		for (int i = 1; i < 12; i++)
		{
			num ^= left * (right & (1 << i));
		}
		return (uint)num;
	}

	private uint GFMulExtPar(ushort left0, ushort right0, ushort left1, ushort right1)
	{
		int num = left0 * (right0 & 1);
		int num2 = left1 * (right1 & 1);
		for (int i = 1; i < 12; i++)
		{
			num ^= left0 * (right0 & (1 << i));
			num2 ^= left1 * (right1 & (1 << i));
		}
		return (uint)(num ^ num2);
	}

	public ushort GFReduce(uint x)
	{
		uint num = x & 0xFFF;
		uint num2 = x >> 12;
		uint num3 = (x & 0x1FF000) >> 9;
		uint num4 = (x & 0xE00000) >> 18;
		uint num5 = x >> 21;
		return (ushort)(num ^ num2 ^ num3 ^ num4 ^ num5);
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
}
