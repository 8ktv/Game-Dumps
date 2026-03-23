using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class RC6Engine : IBlockCipher
{
	private static readonly int _noRounds = 20;

	private int[] _S;

	private static readonly int P32 = -1209970333;

	private static readonly int Q32 = -1640531527;

	private static readonly int LGW = 5;

	private bool forEncryption;

	public virtual string AlgorithmName => "RC6";

	public virtual int GetBlockSize()
	{
		return 16;
	}

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (!(parameters is KeyParameter keyParameter))
		{
			throw new ArgumentException("invalid parameter passed to RC6 init - " + Platform.GetTypeName(parameters));
		}
		this.forEncryption = forEncryption;
		SetKey(keyParameter.GetKey());
	}

	public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
	{
		if (_S == null)
		{
			throw new InvalidOperationException("RC6 engine not initialised");
		}
		int blockSize = GetBlockSize();
		Check.DataLength(input, inOff, blockSize, "input buffer too short");
		Check.OutputLength(output, outOff, blockSize, "output buffer too short");
		if (!forEncryption)
		{
			return DecryptBlock(input, inOff, output, outOff);
		}
		return EncryptBlock(input, inOff, output, outOff);
	}

	private void SetKey(byte[] key)
	{
		_ = (key.Length + 3) / 4;
		int[] array = new int[(key.Length + 3) / 4];
		for (int num = key.Length - 1; num >= 0; num--)
		{
			array[num / 4] = (array[num / 4] << 8) + (key[num] & 0xFF);
		}
		_S = new int[2 + 2 * _noRounds + 2];
		_S[0] = P32;
		for (int i = 1; i < _S.Length; i++)
		{
			_S[i] = _S[i - 1] + Q32;
		}
		int num2 = ((array.Length <= _S.Length) ? (3 * _S.Length) : (3 * array.Length));
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		for (int j = 0; j < num2; j++)
		{
			num3 = (_S[num5] = Integers.RotateLeft(_S[num5] + num3 + num4, 3));
			num4 = (array[num6] = Integers.RotateLeft(array[num6] + num3 + num4, num3 + num4));
			num5 = (num5 + 1) % _S.Length;
			num6 = (num6 + 1) % array.Length;
		}
	}

	private int EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
	{
		int num = (int)Pack.LE_To_UInt32(input, inOff);
		int num2 = (int)Pack.LE_To_UInt32(input, inOff + 4);
		int num3 = (int)Pack.LE_To_UInt32(input, inOff + 8);
		int num4 = (int)Pack.LE_To_UInt32(input, inOff + 12);
		num2 += _S[0];
		num4 += _S[1];
		for (int i = 1; i <= _noRounds; i++)
		{
			int num5 = 0;
			int num6 = 0;
			num5 = num2 * (2 * num2 + 1);
			num5 = Integers.RotateLeft(num5, 5);
			num6 = num4 * (2 * num4 + 1);
			num6 = Integers.RotateLeft(num6, 5);
			num ^= num5;
			num = Integers.RotateLeft(num, num6);
			num += _S[2 * i];
			num3 ^= num6;
			num3 = Integers.RotateLeft(num3, num5);
			num3 += _S[2 * i + 1];
			int num7 = num;
			num = num2;
			num2 = num3;
			num3 = num4;
			num4 = num7;
		}
		num += _S[2 * _noRounds + 2];
		num3 += _S[2 * _noRounds + 3];
		Pack.UInt32_To_LE((uint)num, outBytes, outOff);
		Pack.UInt32_To_LE((uint)num2, outBytes, outOff + 4);
		Pack.UInt32_To_LE((uint)num3, outBytes, outOff + 8);
		Pack.UInt32_To_LE((uint)num4, outBytes, outOff + 12);
		return 16;
	}

	private int DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
	{
		int num = (int)Pack.LE_To_UInt32(input, inOff);
		int num2 = (int)Pack.LE_To_UInt32(input, inOff + 4);
		int num3 = (int)Pack.LE_To_UInt32(input, inOff + 8);
		int num4 = (int)Pack.LE_To_UInt32(input, inOff + 12);
		num3 -= _S[2 * _noRounds + 3];
		num -= _S[2 * _noRounds + 2];
		for (int num5 = _noRounds; num5 >= 1; num5--)
		{
			int num6 = 0;
			int num7 = 0;
			int num8 = num4;
			num4 = num3;
			num3 = num2;
			num2 = num;
			num = num8;
			num6 = num2 * (2 * num2 + 1);
			num6 = Integers.RotateLeft(num6, LGW);
			num7 = num4 * (2 * num4 + 1);
			num7 = Integers.RotateLeft(num7, LGW);
			num3 -= _S[2 * num5 + 1];
			num3 = Integers.RotateRight(num3, num6);
			num3 ^= num7;
			num -= _S[2 * num5];
			num = Integers.RotateRight(num, num7);
			num ^= num6;
		}
		num4 -= _S[1];
		num2 -= _S[0];
		Pack.UInt32_To_LE((uint)num, outBytes, outOff);
		Pack.UInt32_To_LE((uint)num2, outBytes, outOff + 4);
		Pack.UInt32_To_LE((uint)num3, outBytes, outOff + 8);
		Pack.UInt32_To_LE((uint)num4, outBytes, outOff + 12);
		return 16;
	}
}
