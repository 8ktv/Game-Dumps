using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class RC532Engine : IBlockCipher
{
	private int _noRounds;

	private int[] _S;

	private static readonly int P32 = -1209970333;

	private static readonly int Q32 = -1640531527;

	private bool forEncryption;

	public virtual string AlgorithmName => "RC5-32";

	public RC532Engine()
	{
		_noRounds = 12;
	}

	public virtual int GetBlockSize()
	{
		return 8;
	}

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (parameters is RC5Parameters rC5Parameters)
		{
			_noRounds = rC5Parameters.Rounds;
			SetKey(rC5Parameters.GetKey());
		}
		else
		{
			if (!(parameters is KeyParameter keyParameter))
			{
				throw new ArgumentException("invalid parameter passed to RC532 init - " + Platform.GetTypeName(parameters));
			}
			SetKey(keyParameter.GetKey());
		}
		this.forEncryption = forEncryption;
	}

	public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
	{
		if (!forEncryption)
		{
			return DecryptBlock(input, inOff, output, outOff);
		}
		return EncryptBlock(input, inOff, output, outOff);
	}

	private void SetKey(byte[] key)
	{
		int[] array = new int[(key.Length + 3) / 4];
		for (int i = 0; i != key.Length; i++)
		{
			array[i / 4] += (key[i] & 0xFF) << 8 * (i % 4);
		}
		_S = new int[2 * (_noRounds + 1)];
		_S[0] = P32;
		for (int j = 1; j < _S.Length; j++)
		{
			_S[j] = _S[j - 1] + Q32;
		}
		int num = ((array.Length <= _S.Length) ? (3 * _S.Length) : (3 * array.Length));
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		for (int k = 0; k < num; k++)
		{
			num2 = (_S[num4] = Integers.RotateLeft(_S[num4] + num2 + num3, 3));
			num3 = (array[num5] = Integers.RotateLeft(array[num5] + num2 + num3, num2 + num3));
			num4 = (num4 + 1) % _S.Length;
			num5 = (num5 + 1) % array.Length;
		}
	}

	private int EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
	{
		int num = (int)Pack.LE_To_UInt32(input, inOff) + _S[0];
		int num2 = (int)Pack.LE_To_UInt32(input, inOff + 4) + _S[1];
		for (int i = 1; i <= _noRounds; i++)
		{
			num = Integers.RotateLeft(num ^ num2, num2) + _S[2 * i];
			num2 = Integers.RotateLeft(num2 ^ num, num) + _S[2 * i + 1];
		}
		Pack.UInt32_To_LE((uint)num, outBytes, outOff);
		Pack.UInt32_To_LE((uint)num2, outBytes, outOff + 4);
		return 8;
	}

	private int DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
	{
		int num = (int)Pack.LE_To_UInt32(input, inOff);
		int num2 = (int)Pack.LE_To_UInt32(input, inOff + 4);
		for (int num3 = _noRounds; num3 >= 1; num3--)
		{
			num2 = Integers.RotateRight(num2 - _S[2 * num3 + 1], num) ^ num;
			num = Integers.RotateRight(num - _S[2 * num3], num2) ^ num2;
		}
		Pack.UInt32_To_LE((uint)(num - _S[0]), outBytes, outOff);
		Pack.UInt32_To_LE((uint)(num2 - _S[1]), outBytes, outOff + 4);
		return 8;
	}
}
