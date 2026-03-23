using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class HC128Engine : IStreamCipher
{
	private readonly uint[] p = new uint[512];

	private readonly uint[] q = new uint[512];

	private uint cnt;

	private byte[] key;

	private byte[] iv;

	private bool initialised;

	private readonly byte[] buf = new byte[4];

	private int idx;

	public virtual string AlgorithmName => "HC-128";

	private static uint F1(uint x)
	{
		return Integers.RotateRight(x, 7) ^ Integers.RotateRight(x, 18) ^ (x >> 3);
	}

	private static uint F2(uint x)
	{
		return Integers.RotateRight(x, 17) ^ Integers.RotateRight(x, 19) ^ (x >> 10);
	}

	private uint G1(uint x, uint y, uint z)
	{
		return (Integers.RotateRight(x, 10) ^ Integers.RotateRight(z, 23)) + Integers.RotateRight(y, 8);
	}

	private uint G2(uint x, uint y, uint z)
	{
		return (Integers.RotateLeft(x, 10) ^ Integers.RotateLeft(z, 23)) + Integers.RotateLeft(y, 8);
	}

	private uint H1(uint x)
	{
		return q[x & 0xFF] + q[((x >> 16) & 0xFF) + 256];
	}

	private uint H2(uint x)
	{
		return p[x & 0xFF] + p[((x >> 16) & 0xFF) + 256];
	}

	private static uint Mod1024(uint x)
	{
		return x & 0x3FF;
	}

	private static uint Mod512(uint x)
	{
		return x & 0x1FF;
	}

	private static uint Dim(uint x, uint y)
	{
		return Mod512(x - y);
	}

	private uint Step()
	{
		uint num = Mod512(cnt);
		uint result;
		if (cnt < 512)
		{
			p[num] += G1(p[Dim(num, 3u)], p[Dim(num, 10u)], p[Dim(num, 511u)]);
			result = H1(p[Dim(num, 12u)]) ^ p[num];
		}
		else
		{
			q[num] += G2(q[Dim(num, 3u)], q[Dim(num, 10u)], q[Dim(num, 511u)]);
			result = H2(q[Dim(num, 12u)]) ^ q[num];
		}
		cnt = Mod1024(cnt + 1);
		return result;
	}

	private void Init()
	{
		if (key.Length != 16)
		{
			throw new ArgumentException("The key must be 128 bits long");
		}
		if (iv.Length != 16)
		{
			throw new ArgumentException("The IV must be 128 bits long");
		}
		idx = 0;
		cnt = 0u;
		uint[] array = new uint[1280];
		Pack.LE_To_UInt32(key, 0, array, 0, 4);
		Array.Copy(array, 0, array, 4, 4);
		Pack.LE_To_UInt32(iv, 0, array, 8, 4);
		Array.Copy(array, 8, array, 12, 4);
		for (uint num = 16u; num < 1280; num++)
		{
			array[num] = F2(array[num - 2]) + array[num - 7] + F1(array[num - 15]) + array[num - 16] + num;
		}
		Array.Copy(array, 256, p, 0, 512);
		Array.Copy(array, 768, q, 0, 512);
		for (int i = 0; i < 512; i++)
		{
			p[i] = Step();
		}
		for (int j = 0; j < 512; j++)
		{
			q[j] = Step();
		}
		cnt = 0u;
	}

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (!(parameters is ParametersWithIV parametersWithIV))
		{
			throw new ArgumentException("HC-128 Init parameters must include an IV");
		}
		if (!(parametersWithIV.Parameters is KeyParameter keyParameter))
		{
			throw new ArgumentException("Invalid parameter passed to HC128 init - " + Platform.GetTypeName(parameters), "parameters");
		}
		key = keyParameter.GetKey();
		iv = parametersWithIV.GetIV();
		Init();
		initialised = true;
	}

	private byte GetByte()
	{
		if (idx == 0)
		{
			Pack.UInt32_To_LE(Step(), buf);
		}
		byte result = buf[idx];
		idx = (idx + 1) & 3;
		return result;
	}

	public virtual void ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
	{
		if (!initialised)
		{
			throw new InvalidOperationException(AlgorithmName + " not initialised");
		}
		Check.DataLength(input, inOff, len, "input buffer too short");
		Check.OutputLength(output, outOff, len, "output buffer too short");
		for (int i = 0; i < len; i++)
		{
			output[outOff + i] = (byte)(input[inOff + i] ^ GetByte());
		}
	}

	public virtual void Reset()
	{
		Init();
	}

	public virtual byte ReturnByte(byte input)
	{
		return (byte)(input ^ GetByte());
	}
}
