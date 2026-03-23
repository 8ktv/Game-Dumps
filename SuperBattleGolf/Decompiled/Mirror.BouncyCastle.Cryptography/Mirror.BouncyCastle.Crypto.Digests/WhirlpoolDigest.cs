using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public sealed class WhirlpoolDigest : IDigest, IMemoable
{
	private const int BITCOUNT_ARRAY_SIZE = 32;

	private const int BYTE_LENGTH = 64;

	private const int DIGEST_LENGTH_BYTES = 64;

	private const int REDUCTION_POLYNOMIAL = 285;

	private const int ROUNDS = 10;

	private static readonly int[] SBOX;

	private static readonly ulong[] C0;

	private static readonly ulong[] C1;

	private static readonly ulong[] C2;

	private static readonly ulong[] C3;

	private static readonly ulong[] C4;

	private static readonly ulong[] C5;

	private static readonly ulong[] C6;

	private static readonly ulong[] C7;

	private static readonly short[] EIGHT;

	private readonly ulong[] _rc = new ulong[11];

	private byte[] _buffer = new byte[64];

	private int _bufferPos;

	private short[] _bitCount = new short[32];

	private ulong[] _hash = new ulong[8];

	private ulong[] _K = new ulong[8];

	private ulong[] _L = new ulong[8];

	private ulong[] _block = new ulong[8];

	private ulong[] _state = new ulong[8];

	public string AlgorithmName => "Whirlpool";

	static WhirlpoolDigest()
	{
		SBOX = new int[256]
		{
			24, 35, 198, 232, 135, 184, 1, 79, 54, 166,
			210, 245, 121, 111, 145, 82, 96, 188, 155, 142,
			163, 12, 123, 53, 29, 224, 215, 194, 46, 75,
			254, 87, 21, 119, 55, 229, 159, 240, 74, 218,
			88, 201, 41, 10, 177, 160, 107, 133, 189, 93,
			16, 244, 203, 62, 5, 103, 228, 39, 65, 139,
			167, 125, 149, 216, 251, 238, 124, 102, 221, 23,
			71, 158, 202, 45, 191, 7, 173, 90, 131, 51,
			99, 2, 170, 113, 200, 25, 73, 217, 242, 227,
			91, 136, 154, 38, 50, 176, 233, 15, 213, 128,
			190, 205, 52, 72, 255, 122, 144, 95, 32, 104,
			26, 174, 180, 84, 147, 34, 100, 241, 115, 18,
			64, 8, 195, 236, 219, 161, 141, 61, 151, 0,
			207, 43, 118, 130, 214, 27, 181, 175, 106, 80,
			69, 243, 48, 239, 63, 85, 162, 234, 101, 186,
			47, 192, 222, 28, 253, 77, 146, 117, 6, 138,
			178, 230, 14, 31, 98, 212, 168, 150, 249, 197,
			37, 89, 132, 114, 57, 76, 94, 120, 56, 140,
			209, 165, 226, 97, 179, 33, 156, 30, 67, 199,
			252, 4, 81, 153, 109, 13, 250, 223, 126, 36,
			59, 171, 206, 17, 143, 78, 183, 235, 60, 129,
			148, 247, 185, 19, 44, 211, 231, 110, 196, 3,
			86, 68, 127, 169, 42, 187, 193, 83, 220, 11,
			157, 108, 49, 116, 246, 70, 172, 137, 20, 225,
			22, 58, 105, 9, 112, 182, 208, 237, 204, 66,
			152, 164, 40, 92, 248, 134
		};
		C0 = new ulong[256];
		C1 = new ulong[256];
		C2 = new ulong[256];
		C3 = new ulong[256];
		C4 = new ulong[256];
		C5 = new ulong[256];
		C6 = new ulong[256];
		C7 = new ulong[256];
		EIGHT = new short[32];
		EIGHT[31] = 8;
		for (int i = 0; i < 256; i++)
		{
			int num = SBOX[i];
			int num2 = MulX(num);
			int num3 = MulX(num2);
			int num4 = num3 ^ num;
			int num5 = MulX(num3);
			int num6 = num5 ^ num;
			C0[i] = PackIntoUInt64(num, num, num3, num, num5, num4, num2, num6);
			C1[i] = PackIntoUInt64(num6, num, num, num3, num, num5, num4, num2);
			C2[i] = PackIntoUInt64(num2, num6, num, num, num3, num, num5, num4);
			C3[i] = PackIntoUInt64(num4, num2, num6, num, num, num3, num, num5);
			C4[i] = PackIntoUInt64(num5, num4, num2, num6, num, num, num3, num);
			C5[i] = PackIntoUInt64(num, num5, num4, num2, num6, num, num, num3);
			C6[i] = PackIntoUInt64(num3, num, num5, num4, num2, num6, num, num);
			C7[i] = PackIntoUInt64(num, num3, num, num5, num4, num2, num6, num);
		}
	}

	private static int MulX(int input)
	{
		return (input << 1) ^ (-(input >> 7) & 0x11D);
	}

	private static ulong PackIntoUInt64(int b7, int b6, int b5, int b4, int b3, int b2, int b1, int b0)
	{
		return (ulong)(((long)b7 << 56) ^ ((long)b6 << 48) ^ ((long)b5 << 40) ^ ((long)b4 << 32) ^ ((long)b3 << 24) ^ ((long)b2 << 16) ^ ((long)b1 << 8) ^ b0);
	}

	public WhirlpoolDigest()
	{
		_rc[0] = 0uL;
		for (int i = 1; i <= 10; i++)
		{
			int num = 8 * (i - 1);
			_rc[i] = (C0[num] & 0xFF00000000000000uL) ^ (C1[num + 1] & 0xFF000000000000L) ^ (C2[num + 2] & 0xFF0000000000L) ^ (C3[num + 3] & 0xFF00000000L) ^ (C4[num + 4] & 0xFF000000u) ^ (C5[num + 5] & 0xFF0000) ^ (C6[num + 6] & 0xFF00) ^ (C7[num + 7] & 0xFF);
		}
	}

	public WhirlpoolDigest(WhirlpoolDigest originalDigest)
	{
		Reset(originalDigest);
	}

	public int GetDigestSize()
	{
		return 64;
	}

	public int DoFinal(byte[] output, int outOff)
	{
		Finish();
		Pack.UInt64_To_BE(_hash, output, outOff);
		Reset();
		return GetDigestSize();
	}

	public void Reset()
	{
		_bufferPos = 0;
		Array.Clear(_bitCount, 0, _bitCount.Length);
		Array.Clear(_buffer, 0, _buffer.Length);
		Array.Clear(_hash, 0, _hash.Length);
		Array.Clear(_K, 0, _K.Length);
		Array.Clear(_L, 0, _L.Length);
		Array.Clear(_block, 0, _block.Length);
		Array.Clear(_state, 0, _state.Length);
	}

	private void ProcessFilledBuffer()
	{
		Pack.BE_To_UInt64(_buffer, 0, _block);
		ProcessBlock();
		_bufferPos = 0;
		Array.Clear(_buffer, 0, _buffer.Length);
	}

	private void ProcessBlock()
	{
		for (int i = 0; i < 8; i++)
		{
			_state[i] = _block[i] ^ (_K[i] = _hash[i]);
		}
		for (int j = 1; j <= 10; j++)
		{
			for (int k = 0; k < 8; k++)
			{
				_L[k] = C0[(int)(_K[k & 7] >> 56) & 0xFF];
				_L[k] ^= C1[(int)(_K[(k - 1) & 7] >> 48) & 0xFF];
				_L[k] ^= C2[(int)(_K[(k - 2) & 7] >> 40) & 0xFF];
				_L[k] ^= C3[(int)(_K[(k - 3) & 7] >> 32) & 0xFF];
				_L[k] ^= C4[(int)(_K[(k - 4) & 7] >> 24) & 0xFF];
				_L[k] ^= C5[(int)(_K[(k - 5) & 7] >> 16) & 0xFF];
				_L[k] ^= C6[(int)(_K[(k - 6) & 7] >> 8) & 0xFF];
				_L[k] ^= C7[(int)_K[(k - 7) & 7] & 0xFF];
			}
			Array.Copy(_L, 0, _K, 0, _K.Length);
			_K[0] ^= _rc[j];
			for (int l = 0; l < 8; l++)
			{
				_L[l] = _K[l];
				_L[l] ^= C0[(int)(_state[l & 7] >> 56) & 0xFF];
				_L[l] ^= C1[(int)(_state[(l - 1) & 7] >> 48) & 0xFF];
				_L[l] ^= C2[(int)(_state[(l - 2) & 7] >> 40) & 0xFF];
				_L[l] ^= C3[(int)(_state[(l - 3) & 7] >> 32) & 0xFF];
				_L[l] ^= C4[(int)(_state[(l - 4) & 7] >> 24) & 0xFF];
				_L[l] ^= C5[(int)(_state[(l - 5) & 7] >> 16) & 0xFF];
				_L[l] ^= C6[(int)(_state[(l - 6) & 7] >> 8) & 0xFF];
				_L[l] ^= C7[(int)_state[(l - 7) & 7] & 0xFF];
			}
			Array.Copy(_L, 0, _state, 0, _state.Length);
		}
		for (int m = 0; m < 8; m++)
		{
			_hash[m] ^= _state[m] ^ _block[m];
		}
	}

	public void Update(byte input)
	{
		_buffer[_bufferPos] = input;
		if (++_bufferPos == _buffer.Length)
		{
			ProcessFilledBuffer();
		}
		Increment();
	}

	private void Increment()
	{
		int num = 0;
		for (int num2 = _bitCount.Length - 1; num2 >= 0; num2--)
		{
			int num3 = (_bitCount[num2] & 0xFF) + EIGHT[num2] + num;
			num = num3 >> 8;
			_bitCount[num2] = (short)(num3 & 0xFF);
		}
	}

	public void BlockUpdate(byte[] input, int inOff, int length)
	{
		while (length > 0)
		{
			Update(input[inOff]);
			inOff++;
			length--;
		}
	}

	private void Finish()
	{
		byte[] array = CopyBitLength();
		_buffer[_bufferPos] |= 128;
		if (++_bufferPos == _buffer.Length)
		{
			ProcessFilledBuffer();
		}
		if (_bufferPos > 32)
		{
			while (_bufferPos != 0)
			{
				Update(0);
			}
		}
		while (_bufferPos <= 32)
		{
			Update(0);
		}
		Array.Copy(array, 0, _buffer, 32, array.Length);
		ProcessFilledBuffer();
	}

	private byte[] CopyBitLength()
	{
		byte[] array = new byte[32];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(_bitCount[i] & 0xFF);
		}
		return array;
	}

	public int GetByteLength()
	{
		return 64;
	}

	public IMemoable Copy()
	{
		return new WhirlpoolDigest(this);
	}

	public void Reset(IMemoable other)
	{
		WhirlpoolDigest whirlpoolDigest = (WhirlpoolDigest)other;
		Array.Copy(whirlpoolDigest._rc, 0, _rc, 0, _rc.Length);
		Array.Copy(whirlpoolDigest._buffer, 0, _buffer, 0, _buffer.Length);
		_bufferPos = whirlpoolDigest._bufferPos;
		Array.Copy(whirlpoolDigest._bitCount, 0, _bitCount, 0, _bitCount.Length);
		Array.Copy(whirlpoolDigest._hash, 0, _hash, 0, _hash.Length);
		Array.Copy(whirlpoolDigest._K, 0, _K, 0, _K.Length);
		Array.Copy(whirlpoolDigest._L, 0, _L, 0, _L.Length);
		Array.Copy(whirlpoolDigest._block, 0, _block, 0, _block.Length);
		Array.Copy(whirlpoolDigest._state, 0, _state, 0, _state.Length);
	}
}
