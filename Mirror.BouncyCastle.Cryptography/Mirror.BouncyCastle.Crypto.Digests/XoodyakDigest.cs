using System;
using System.IO;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class XoodyakDigest : IDigest
{
	private enum MODE
	{
		ModeHash,
		ModeKeyed
	}

	private const int Rkin = 44;

	private static readonly uint[] RC = new uint[12]
	{
		88u, 56u, 960u, 208u, 288u, 20u, 96u, 44u, 896u, 240u,
		416u, 18u
	};

	private byte[] state;

	private int phase;

	private MODE mode;

	private int Rabsorb;

	private const int f_bPrime = 48;

	private const int Rkout = 24;

	private const int PhaseDown = 1;

	private const int PhaseUp = 2;

	private const int NLANES = 12;

	private const int NROWS = 3;

	private const int NCOLUMNS = 4;

	private const int MAXROUNDS = 12;

	private const int TAGLEN = 16;

	private const int Rhash = 16;

	private readonly MemoryStream buffer = new MemoryStream();

	public string AlgorithmName => "Xoodyak Hash";

	public XoodyakDigest()
	{
		state = new byte[48];
		Reset();
	}

	public int GetDigestSize()
	{
		return 32;
	}

	public int GetByteLength()
	{
		return Rabsorb;
	}

	public void Update(byte input)
	{
		buffer.WriteByte(input);
	}

	public void BlockUpdate(byte[] input, int inOff, int inLen)
	{
		Check.DataLength(input, inOff, inLen, "input buffer too short");
		buffer.Write(input, inOff, inLen);
	}

	public int DoFinal(byte[] output, int outOff)
	{
		Check.OutputLength(output, outOff, 32, "output buffer too short");
		byte[] xi = buffer.GetBuffer();
		int num = (int)buffer.Length;
		int num2 = 0;
		uint cd = 3u;
		do
		{
			if (phase != 2)
			{
				Up(null, 0, 0, 0u);
			}
			int num3 = System.Math.Min(num, Rabsorb);
			Down(xi, num2, num3, cd);
			cd = 0u;
			num2 += num3;
			num -= num3;
		}
		while (num != 0);
		Up(output, outOff, 16, 64u);
		Down(null, 0, 0, 0u);
		Up(output, outOff + 16, 16, 0u);
		return 32;
	}

	public void Reset()
	{
		Array.Clear(state, 0, state.Length);
		phase = 2;
		mode = MODE.ModeHash;
		Rabsorb = 16;
		buffer.SetLength(0L);
	}

	private void Up(byte[] Yi, int YiOff, int YiLen, uint Cu)
	{
		if (mode != MODE.ModeHash)
		{
			state[47] ^= (byte)Cu;
		}
		uint[] array = new uint[12];
		Pack.LE_To_UInt32(state, 0, array, 0, array.Length);
		uint[] array2 = new uint[12];
		uint[] array3 = new uint[4];
		uint[] array4 = new uint[4];
		for (int i = 0; i < 12; i++)
		{
			for (uint num = 0u; num < 4; num++)
			{
				array3[num] = array[index(num, 0u)] ^ array[index(num, 1u)] ^ array[index(num, 2u)];
			}
			for (uint num = 0u; num < 4; num++)
			{
				uint i2 = array3[(num + 3) & 3];
				array4[num] = Integers.RotateLeft(i2, 5) ^ Integers.RotateLeft(i2, 14);
			}
			for (uint num = 0u; num < 4; num++)
			{
				for (uint i2 = 0u; i2 < 3; i2++)
				{
					array[index(num, i2)] ^= array4[num];
				}
			}
			for (uint num = 0u; num < 4; num++)
			{
				array2[index(num, 0u)] = array[index(num, 0u)];
				array2[index(num, 1u)] = array[index(num + 3, 1u)];
				array2[index(num, 2u)] = Integers.RotateLeft(array[index(num, 2u)], 11);
			}
			array2[0] ^= RC[i];
			for (uint num = 0u; num < 4; num++)
			{
				for (uint i2 = 0u; i2 < 3; i2++)
				{
					array[index(num, i2)] = array2[index(num, i2)] ^ (~array2[index(num, i2 + 1)] & array2[index(num, i2 + 2)]);
				}
			}
			for (uint num = 0u; num < 4; num++)
			{
				array2[index(num, 0u)] = array[index(num, 0u)];
				array2[index(num, 1u)] = Integers.RotateLeft(array[index(num, 1u)], 1);
				array2[index(num, 2u)] = Integers.RotateLeft(array[index(num + 2, 2u)], 8);
			}
			Array.Copy(array2, 0, array, 0, 12);
		}
		Pack.UInt32_To_LE(array, 0, array.Length, state, 0);
		phase = 2;
		if (Yi != null)
		{
			Array.Copy(state, 0, Yi, YiOff, YiLen);
		}
	}

	private void Down(byte[] Xi, int XiOff, int XiLen, uint Cd)
	{
		for (int i = 0; i < XiLen; i++)
		{
			state[i] ^= Xi[XiOff++];
		}
		state[XiLen] ^= 1;
		state[47] ^= (byte)((mode == MODE.ModeHash) ? (Cd & 1) : Cd);
		phase = 1;
	}

	private uint index(uint x, uint y)
	{
		return y % 3 * 4 + x % 4;
	}
}
