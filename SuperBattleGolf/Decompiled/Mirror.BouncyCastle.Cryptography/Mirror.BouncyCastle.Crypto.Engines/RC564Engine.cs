using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class RC564Engine : IBlockCipher
{
	private int _noRounds;

	private long[] _S;

	private static readonly long P64 = -5196783011329398165L;

	private static readonly long Q64 = -7046029254386353131L;

	private bool forEncryption;

	public virtual string AlgorithmName => "RC5-64";

	public RC564Engine()
	{
		_noRounds = 12;
	}

	public virtual int GetBlockSize()
	{
		return 16;
	}

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (!(parameters is RC5Parameters rC5Parameters))
		{
			throw new ArgumentException("invalid parameter passed to RC564 init - " + Platform.GetTypeName(parameters));
		}
		this.forEncryption = forEncryption;
		_noRounds = rC5Parameters.Rounds;
		SetKey(rC5Parameters.GetKey());
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
		long[] array = new long[(key.Length + 7) / 8];
		for (int i = 0; i != key.Length; i++)
		{
			array[i / 8] += (long)(key[i] & 0xFF) << 8 * (i % 8);
		}
		_S = new long[2 * (_noRounds + 1)];
		_S[0] = P64;
		for (int j = 1; j < _S.Length; j++)
		{
			_S[j] = _S[j - 1] + Q64;
		}
		int num = ((array.Length <= _S.Length) ? (3 * _S.Length) : (3 * array.Length));
		long num2 = 0L;
		long num3 = 0L;
		int num4 = 0;
		int num5 = 0;
		for (int k = 0; k < num; k++)
		{
			num2 = (_S[num4] = Longs.RotateLeft(_S[num4] + num2 + num3, 3));
			num3 = (array[num5] = Longs.RotateLeft(array[num5] + num2 + num3, (int)(num2 + num3)));
			num4 = (num4 + 1) % _S.Length;
			num5 = (num5 + 1) % array.Length;
		}
	}

	private int EncryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
	{
		long num = (long)Pack.LE_To_UInt64(input, inOff) + _S[0];
		long num2 = (long)Pack.LE_To_UInt64(input, inOff + 8) + _S[1];
		for (int i = 1; i <= _noRounds; i++)
		{
			num = Longs.RotateLeft(num ^ num2, (int)num2) + _S[2 * i];
			num2 = Longs.RotateLeft(num2 ^ num, (int)num) + _S[2 * i + 1];
		}
		Pack.UInt64_To_LE((ulong)num, outBytes, outOff);
		Pack.UInt64_To_LE((ulong)num2, outBytes, outOff + 8);
		return 16;
	}

	private int DecryptBlock(byte[] input, int inOff, byte[] outBytes, int outOff)
	{
		long num = (long)Pack.LE_To_UInt64(input, inOff);
		long num2 = (long)Pack.LE_To_UInt64(input, inOff + 8);
		for (int num3 = _noRounds; num3 >= 1; num3--)
		{
			num2 = Longs.RotateRight(num2 - _S[2 * num3 + 1], (int)num) ^ num;
			num = Longs.RotateRight(num - _S[2 * num3], (int)num2) ^ num2;
		}
		Pack.UInt64_To_LE((ulong)(num - _S[0]), outBytes, outOff);
		Pack.UInt64_To_LE((ulong)(num2 - _S[1]), outBytes, outOff + 8);
		return 16;
	}
}
