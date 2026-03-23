using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Macs;

public class Gost28147Mac : IMac
{
	private const int BlockSize = 8;

	private const int MacSize = 4;

	private int bufOff;

	private byte[] buf;

	private byte[] mac;

	private bool firstStep = true;

	private int[] workingKey;

	private byte[] macIV;

	private byte[] S = new byte[128]
	{
		9, 6, 3, 2, 8, 11, 1, 7, 10, 4,
		14, 15, 12, 0, 13, 5, 3, 7, 14, 9,
		8, 10, 15, 0, 5, 2, 6, 12, 11, 4,
		13, 1, 14, 4, 6, 2, 11, 3, 13, 8,
		12, 15, 5, 10, 0, 7, 1, 9, 14, 7,
		10, 12, 13, 1, 3, 9, 0, 2, 11, 4,
		15, 8, 5, 6, 11, 5, 1, 9, 8, 13,
		15, 0, 14, 4, 2, 3, 12, 7, 10, 6,
		3, 10, 13, 12, 1, 2, 0, 11, 7, 5,
		9, 4, 8, 15, 14, 6, 1, 13, 2, 9,
		7, 10, 6, 0, 8, 12, 4, 5, 15, 3,
		11, 14, 11, 10, 15, 5, 0, 12, 14, 8,
		6, 2, 3, 9, 1, 7, 13, 4
	};

	public string AlgorithmName => "Gost28147Mac";

	public Gost28147Mac()
	{
		mac = new byte[8];
		buf = new byte[8];
		bufOff = 0;
	}

	private static int[] GenerateWorkingKey(byte[] userKey)
	{
		if (userKey.Length != 32)
		{
			throw new ArgumentException("Key length invalid. Key needs to be 32 byte - 256 bit!!!");
		}
		int[] array = new int[8];
		for (int i = 0; i != 8; i++)
		{
			array[i] = (int)Pack.LE_To_UInt32(userKey, i * 4);
		}
		return array;
	}

	public void Init(ICipherParameters parameters)
	{
		Reset();
		buf = new byte[8];
		macIV = null;
		if (parameters is ParametersWithSBox parametersWithSBox)
		{
			parametersWithSBox.GetSBox().CopyTo(S, 0);
			if (parametersWithSBox.Parameters != null)
			{
				workingKey = GenerateWorkingKey(((KeyParameter)parametersWithSBox.Parameters).GetKey());
			}
			return;
		}
		if (parameters is KeyParameter keyParameter)
		{
			workingKey = GenerateWorkingKey(keyParameter.GetKey());
			return;
		}
		if (parameters is ParametersWithIV parametersWithIV)
		{
			workingKey = GenerateWorkingKey(((KeyParameter)parametersWithIV.Parameters).GetKey());
			macIV = parametersWithIV.GetIV();
			Array.Copy(macIV, 0, mac, 0, mac.Length);
			return;
		}
		throw new ArgumentException("invalid parameter passed to Gost28147 init - " + Platform.GetTypeName(parameters));
	}

	public int GetMacSize()
	{
		return 4;
	}

	private int Gost28147_mainStep(int n1, int key)
	{
		int num = key + n1;
		int num2 = S[num & 0xF] + (S[16 + ((num >> 4) & 0xF)] << 4) + (S[32 + ((num >> 8) & 0xF)] << 8) + (S[48 + ((num >> 12) & 0xF)] << 12) + (S[64 + ((num >> 16) & 0xF)] << 16) + (S[80 + ((num >> 20) & 0xF)] << 20) + (S[96 + ((num >> 24) & 0xF)] << 24) + (S[112 + ((num >> 28) & 0xF)] << 28);
		int num3 = num2 << 11;
		int num4 = num2 >>> 21;
		return num3 | num4;
	}

	private void Gost28147MacFunc(int[] workingKey, byte[] input, int inOff, byte[] output, int outOff)
	{
		int num = (int)Pack.LE_To_UInt32(input, inOff);
		int num2 = (int)Pack.LE_To_UInt32(input, inOff + 4);
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				int num3 = num;
				num = num2 ^ Gost28147_mainStep(num, workingKey[j]);
				num2 = num3;
			}
		}
		Pack.UInt32_To_LE((uint)num, output, outOff);
		Pack.UInt32_To_LE((uint)num2, output, outOff + 4);
	}

	public void Update(byte input)
	{
		if (bufOff == buf.Length)
		{
			byte[] array = new byte[buf.Length];
			if (firstStep)
			{
				firstStep = false;
				if (macIV != null)
				{
					Cm5Func(buf, 0, macIV, array);
				}
				else
				{
					Array.Copy(buf, 0, array, 0, mac.Length);
				}
			}
			else
			{
				Cm5Func(buf, 0, mac, array);
			}
			Gost28147MacFunc(workingKey, array, 0, mac, 0);
			bufOff = 0;
		}
		buf[bufOff++] = input;
	}

	public void BlockUpdate(byte[] input, int inOff, int len)
	{
		if (len < 0)
		{
			throw new ArgumentException("Can't have a negative input length!");
		}
		int num = 8 - bufOff;
		if (len > num)
		{
			Array.Copy(input, inOff, buf, bufOff, num);
			byte[] array = new byte[buf.Length];
			if (firstStep)
			{
				firstStep = false;
				if (macIV != null)
				{
					Cm5Func(buf, 0, macIV, array);
				}
				else
				{
					Array.Copy(buf, 0, array, 0, mac.Length);
				}
			}
			else
			{
				Cm5Func(buf, 0, mac, array);
			}
			Gost28147MacFunc(workingKey, array, 0, mac, 0);
			bufOff = 0;
			len -= num;
			inOff += num;
			while (len > 8)
			{
				Cm5Func(input, inOff, mac, array);
				Gost28147MacFunc(workingKey, array, 0, mac, 0);
				len -= 8;
				inOff += 8;
			}
		}
		Array.Copy(input, inOff, buf, bufOff, len);
		bufOff += len;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		while (bufOff < 8)
		{
			buf[bufOff++] = 0;
		}
		byte[] array = new byte[buf.Length];
		if (firstStep)
		{
			firstStep = false;
			Array.Copy(buf, 0, array, 0, mac.Length);
		}
		else
		{
			Cm5Func(buf, 0, mac, array);
		}
		Gost28147MacFunc(workingKey, array, 0, mac, 0);
		Array.Copy(mac, mac.Length / 2 - 4, output, outOff, 4);
		Reset();
		return 4;
	}

	public void Reset()
	{
		Array.Clear(buf, 0, buf.Length);
		bufOff = 0;
		firstStep = true;
	}

	private static void Cm5Func(byte[] buf, int bufOff, byte[] mac, byte[] sum)
	{
		for (int i = 0; i < 8; i++)
		{
			sum[i] = (byte)(buf[bufOff + i] ^ mac[i]);
		}
	}
}
