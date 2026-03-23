using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public class KeccakDigest : IDigest, IMemoable
{
	private static readonly ulong[] KeccakRoundConstants = new ulong[24]
	{
		1uL, 32898uL, 9223372036854808714uL, 9223372039002292224uL, 32907uL, 2147483649uL, 9223372039002292353uL, 9223372036854808585uL, 138uL, 136uL,
		2147516425uL, 2147483658uL, 2147516555uL, 9223372036854775947uL, 9223372036854808713uL, 9223372036854808579uL, 9223372036854808578uL, 9223372036854775936uL, 32778uL, 9223372039002259466uL,
		9223372039002292353uL, 9223372036854808704uL, 2147483649uL, 9223372039002292232uL
	};

	private readonly ulong[] state = new ulong[25];

	protected byte[] dataQueue = new byte[192];

	protected int rate;

	protected int bitsInQueue;

	protected internal int fixedOutputLength;

	protected bool squeezing;

	public virtual string AlgorithmName => "Keccak-" + fixedOutputLength;

	public KeccakDigest()
		: this(288)
	{
	}

	public KeccakDigest(int bitLength)
	{
		Init(bitLength);
	}

	public KeccakDigest(KeccakDigest source)
	{
		CopyIn(source);
	}

	private void CopyIn(KeccakDigest source)
	{
		Array.Copy(source.state, 0, state, 0, source.state.Length);
		Array.Copy(source.dataQueue, 0, dataQueue, 0, source.dataQueue.Length);
		rate = source.rate;
		bitsInQueue = source.bitsInQueue;
		fixedOutputLength = source.fixedOutputLength;
		squeezing = source.squeezing;
	}

	public virtual int GetDigestSize()
	{
		return fixedOutputLength >> 3;
	}

	public virtual void Update(byte input)
	{
		Absorb(input);
	}

	public virtual void BlockUpdate(byte[] input, int inOff, int len)
	{
		Absorb(input, inOff, len);
	}

	public virtual int DoFinal(byte[] output, int outOff)
	{
		Squeeze(output, outOff, fixedOutputLength);
		Reset();
		return GetDigestSize();
	}

	protected virtual int DoFinal(byte[] output, int outOff, byte partialByte, int partialBits)
	{
		if (partialBits > 0)
		{
			AbsorbBits(partialByte, partialBits);
		}
		Squeeze(output, outOff, fixedOutputLength);
		Reset();
		return GetDigestSize();
	}

	public virtual void Reset()
	{
		Init(fixedOutputLength);
	}

	public virtual int GetByteLength()
	{
		return rate >> 3;
	}

	private void Init(int bitLength)
	{
		switch (bitLength)
		{
		case 128:
		case 224:
		case 256:
		case 288:
		case 384:
		case 512:
			InitSponge(1600 - (bitLength << 1));
			break;
		default:
			throw new ArgumentException("must be one of 128, 224, 256, 288, 384, or 512.", "bitLength");
		}
	}

	private void InitSponge(int rate)
	{
		if (rate <= 0 || rate >= 1600 || (rate & 0x3F) != 0)
		{
			throw new InvalidOperationException("invalid rate value");
		}
		this.rate = rate;
		Array.Clear(state, 0, state.Length);
		Arrays.Fill(dataQueue, 0);
		bitsInQueue = 0;
		squeezing = false;
		fixedOutputLength = 1600 - rate >> 1;
	}

	protected void Absorb(byte data)
	{
		if ((bitsInQueue & 7) != 0)
		{
			throw new InvalidOperationException("attempt to absorb with odd length queue");
		}
		if (squeezing)
		{
			throw new InvalidOperationException("attempt to absorb while squeezing");
		}
		dataQueue[bitsInQueue >> 3] = data;
		if ((bitsInQueue += 8) == rate)
		{
			KeccakAbsorb(dataQueue, 0);
			bitsInQueue = 0;
		}
	}

	protected void Absorb(byte[] data, int off, int len)
	{
		if ((bitsInQueue & 7) != 0)
		{
			throw new InvalidOperationException("attempt to absorb with odd length queue");
		}
		if (squeezing)
		{
			throw new InvalidOperationException("attempt to absorb while squeezing");
		}
		int num = bitsInQueue >> 3;
		int num2 = rate >> 3;
		int num3 = num2 - num;
		if (len < num3)
		{
			Array.Copy(data, off, dataQueue, num, len);
			bitsInQueue += len << 3;
			return;
		}
		int num4 = 0;
		if (num > 0)
		{
			Array.Copy(data, off, dataQueue, num, num3);
			num4 += num3;
			KeccakAbsorb(dataQueue, 0);
		}
		int num5;
		while ((num5 = len - num4) >= num2)
		{
			KeccakAbsorb(data, off + num4);
			num4 += num2;
		}
		Array.Copy(data, off + num4, dataQueue, 0, num5);
		bitsInQueue = num5 << 3;
	}

	protected void AbsorbBits(int data, int bits)
	{
		if (bits < 1 || bits > 7)
		{
			throw new ArgumentException("must be in the range 1 to 7", "bits");
		}
		if ((bitsInQueue & 7) != 0)
		{
			throw new InvalidOperationException("attempt to absorb with odd length queue");
		}
		if (squeezing)
		{
			throw new InvalidOperationException("attempt to absorb while squeezing");
		}
		int num = (1 << bits) - 1;
		dataQueue[bitsInQueue >> 3] = (byte)(data & num);
		bitsInQueue += bits;
	}

	private void PadAndSwitchToSqueezingPhase()
	{
		dataQueue[bitsInQueue >> 3] |= (byte)(1 << (bitsInQueue & 7));
		if (++bitsInQueue == rate)
		{
			KeccakAbsorb(dataQueue, 0);
		}
		else
		{
			int num = bitsInQueue >> 6;
			int num2 = bitsInQueue & 0x3F;
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				state[i] ^= Pack.LE_To_UInt64(dataQueue, num3);
				num3 += 8;
			}
			if (num2 > 0)
			{
				ulong num4 = (ulong)((1L << num2) - 1);
				state[num] ^= Pack.LE_To_UInt64(dataQueue, num3) & num4;
			}
		}
		state[rate - 1 >> 6] ^= 9223372036854775808uL;
		bitsInQueue = 0;
		squeezing = true;
	}

	protected void Squeeze(byte[] output, int offset, long outputLength)
	{
		if (!squeezing)
		{
			PadAndSwitchToSqueezingPhase();
		}
		if ((outputLength & 7) != 0L)
		{
			throw new InvalidOperationException("outputLength not a multiple of 8");
		}
		int num2;
		for (long num = 0L; num < outputLength; num += num2)
		{
			if (bitsInQueue == 0)
			{
				KeccakExtract();
			}
			num2 = (int)System.Math.Min(bitsInQueue, outputLength - num);
			Array.Copy(dataQueue, rate - bitsInQueue >> 3, output, offset + (int)(num >> 3), num2 >> 3);
			bitsInQueue -= num2;
		}
	}

	private void KeccakAbsorb(byte[] data, int off)
	{
		int num = rate >> 6;
		for (int i = 0; i < num; i++)
		{
			state[i] ^= Pack.LE_To_UInt64(data, off);
			off += 8;
		}
		KeccakPermutation(state);
	}

	private void KeccakExtract()
	{
		KeccakPermutation(state);
		Pack.UInt64_To_LE(state, 0, rate >> 6, dataQueue, 0);
		bitsInQueue = rate;
	}

	internal static void KeccakPermutation(ulong[] A)
	{
		_ = A[24];
		ulong num = A[0];
		ulong num2 = A[1];
		ulong num3 = A[2];
		ulong num4 = A[3];
		ulong num5 = A[4];
		ulong num6 = A[5];
		ulong num7 = A[6];
		ulong num8 = A[7];
		ulong num9 = A[8];
		ulong num10 = A[9];
		ulong num11 = A[10];
		ulong num12 = A[11];
		ulong num13 = A[12];
		ulong num14 = A[13];
		ulong num15 = A[14];
		ulong num16 = A[15];
		ulong num17 = A[16];
		ulong num18 = A[17];
		ulong num19 = A[18];
		ulong num20 = A[19];
		ulong num21 = A[20];
		ulong num22 = A[21];
		ulong num23 = A[22];
		ulong num24 = A[23];
		ulong num25 = A[24];
		for (int i = 0; i < 24; i++)
		{
			ulong num26 = num ^ num6 ^ num11 ^ num16 ^ num21;
			ulong num27 = num2 ^ num7 ^ num12 ^ num17 ^ num22;
			ulong num28 = num3 ^ num8 ^ num13 ^ num18 ^ num23;
			ulong num29 = num4 ^ num9 ^ num14 ^ num19 ^ num24;
			ulong num30 = num5 ^ num10 ^ num15 ^ num20 ^ num25;
			ulong num31 = Longs.RotateLeft(num27, 1) ^ num30;
			ulong num32 = Longs.RotateLeft(num28, 1) ^ num26;
			ulong num33 = Longs.RotateLeft(num29, 1) ^ num27;
			ulong num34 = Longs.RotateLeft(num30, 1) ^ num28;
			ulong num35 = Longs.RotateLeft(num26, 1) ^ num29;
			num ^= num31;
			num6 ^= num31;
			num11 ^= num31;
			num16 ^= num31;
			num21 ^= num31;
			num2 ^= num32;
			num7 ^= num32;
			num12 ^= num32;
			num17 ^= num32;
			num22 ^= num32;
			num3 ^= num33;
			num8 ^= num33;
			num13 ^= num33;
			num18 ^= num33;
			num23 ^= num33;
			num4 ^= num34;
			num9 ^= num34;
			num14 ^= num34;
			num19 ^= num34;
			num24 ^= num34;
			num5 ^= num35;
			num10 ^= num35;
			num15 ^= num35;
			num20 ^= num35;
			num25 ^= num35;
			num27 = Longs.RotateLeft(num2, 1);
			num2 = Longs.RotateLeft(num7, 44);
			num7 = Longs.RotateLeft(num10, 20);
			num10 = Longs.RotateLeft(num23, 61);
			num23 = Longs.RotateLeft(num15, 39);
			num15 = Longs.RotateLeft(num21, 18);
			num21 = Longs.RotateLeft(num3, 62);
			num3 = Longs.RotateLeft(num13, 43);
			num13 = Longs.RotateLeft(num14, 25);
			num14 = Longs.RotateLeft(num20, 8);
			num20 = Longs.RotateLeft(num24, 56);
			num24 = Longs.RotateLeft(num16, 41);
			num16 = Longs.RotateLeft(num5, 27);
			num5 = Longs.RotateLeft(num25, 14);
			num25 = Longs.RotateLeft(num22, 2);
			num22 = Longs.RotateLeft(num9, 55);
			num9 = Longs.RotateLeft(num17, 45);
			num17 = Longs.RotateLeft(num6, 36);
			num6 = Longs.RotateLeft(num4, 28);
			num4 = Longs.RotateLeft(num19, 21);
			num19 = Longs.RotateLeft(num18, 15);
			num18 = Longs.RotateLeft(num12, 10);
			num12 = Longs.RotateLeft(num8, 6);
			num8 = Longs.RotateLeft(num11, 3);
			num11 = num27;
			num26 = num ^ (~num2 & num3);
			num27 = num2 ^ (~num3 & num4);
			num3 ^= ~num4 & num5;
			num4 ^= ~num5 & num;
			num5 ^= ~num & num2;
			num = num26;
			num2 = num27;
			num26 = num6 ^ (~num7 & num8);
			num27 = num7 ^ (~num8 & num9);
			num8 ^= ~num9 & num10;
			num9 ^= ~num10 & num6;
			num10 ^= ~num6 & num7;
			num6 = num26;
			num7 = num27;
			num26 = num11 ^ (~num12 & num13);
			num27 = num12 ^ (~num13 & num14);
			num13 ^= ~num14 & num15;
			num14 ^= ~num15 & num11;
			num15 ^= ~num11 & num12;
			num11 = num26;
			num12 = num27;
			num26 = num16 ^ (~num17 & num18);
			num27 = num17 ^ (~num18 & num19);
			num18 ^= ~num19 & num20;
			num19 ^= ~num20 & num16;
			num20 ^= ~num16 & num17;
			num16 = num26;
			num17 = num27;
			num26 = num21 ^ (~num22 & num23);
			num27 = num22 ^ (~num23 & num24);
			num23 ^= ~num24 & num25;
			num24 ^= ~num25 & num21;
			num25 ^= ~num21 & num22;
			num21 = num26;
			num22 = num27;
			num ^= KeccakRoundConstants[i];
		}
		A[0] = num;
		A[1] = num2;
		A[2] = num3;
		A[3] = num4;
		A[4] = num5;
		A[5] = num6;
		A[6] = num7;
		A[7] = num8;
		A[8] = num9;
		A[9] = num10;
		A[10] = num11;
		A[11] = num12;
		A[12] = num13;
		A[13] = num14;
		A[14] = num15;
		A[15] = num16;
		A[16] = num17;
		A[17] = num18;
		A[18] = num19;
		A[19] = num20;
		A[20] = num21;
		A[21] = num22;
		A[22] = num23;
		A[23] = num24;
		A[24] = num25;
	}

	public virtual IMemoable Copy()
	{
		return new KeccakDigest(this);
	}

	public virtual void Reset(IMemoable other)
	{
		CopyIn((KeccakDigest)other);
	}
}
