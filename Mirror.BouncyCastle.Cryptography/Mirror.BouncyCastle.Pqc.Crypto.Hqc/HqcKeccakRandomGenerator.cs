using System;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

internal sealed class HqcKeccakRandomGenerator
{
	private static readonly ulong[] KeccakRoundConstants = new ulong[24]
	{
		1uL, 32898uL, 9223372036854808714uL, 9223372039002292224uL, 32907uL, 2147483649uL, 9223372039002292353uL, 9223372036854808585uL, 138uL, 136uL,
		2147516425uL, 2147483658uL, 2147516555uL, 9223372036854775947uL, 9223372036854808713uL, 9223372036854808579uL, 9223372036854808578uL, 9223372036854775936uL, 32778uL, 9223372039002259466uL,
		9223372039002292353uL, 9223372036854808704uL, 2147483649uL, 9223372039002292232uL
	};

	private readonly ulong[] state = new ulong[26];

	private readonly byte[] dataQueue = new byte[192];

	private int rate;

	private int fixedOutputLength;

	public HqcKeccakRandomGenerator()
	{
		Init(288);
	}

	public HqcKeccakRandomGenerator(int bitLength)
	{
		Init(bitLength);
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
			throw new ArgumentException("bitLength must be one of 128, 224, 256, 288, 384, or 512.");
		}
	}

	private void InitSponge(int rate)
	{
		if (rate <= 0 || rate >= 1600 || rate % 64 != 0)
		{
			throw new InvalidOperationException("invalid rate value");
		}
		this.rate = rate;
		Arrays.Fill(state, 0uL);
		Arrays.Fill(dataQueue, 0);
		fixedOutputLength = (1600 - rate) / 2;
	}

	private void KeccakIncAbsorb(byte[] input, int inputLen)
	{
		int num = 0;
		int num2 = rate >> 3;
		while ((long)inputLen + (long)state[25] >= num2)
		{
			for (int i = 0; i < (long)num2 - (long)state[25]; i++)
			{
				int num3 = (int)((long)state[25] + (long)i) >> 3;
				state[num3] ^= (ulong)input[i + num] << 8 * (((int)state[25] + i) & 7);
			}
			inputLen -= (int)((long)num2 - (long)state[25]);
			num += (int)((long)num2 - (long)state[25]);
			state[25] = 0uL;
			KeccakDigest.KeccakPermutation(state);
		}
		for (int j = 0; j < inputLen; j++)
		{
			int num4 = (int)((long)state[25] + (long)j) >> 3;
			state[num4] ^= (ulong)input[j + num] << 8 * (((int)state[25] + j) & 7);
		}
		state[25] = state[25] + (ulong)inputLen;
	}

	private void KeccakIncFinalize(int p)
	{
		int num = rate >> 3;
		state[(int)state[25] >> 3] ^= (ulong)((long)p << (int)(8 * (state[25] & 7)));
		state[num - 1 >> 3] ^= (ulong)(128L << 8 * ((num - 1) & 7));
		state[25] = 0uL;
	}

	private void KeccakIncSqueeze(byte[] output, int outLen)
	{
		int num = rate >> 3;
		int i;
		for (i = 0; i < outLen && (long)i < (long)state[25]; i++)
		{
			output[i] = (byte)(state[(int)((long)num - (long)state[25] + i >> 3)] >> (int)(8 * (((long)num - (long)state[25] + i) & 7)));
		}
		int num2 = i;
		outLen -= i;
		state[25] = state[25] - (ulong)i;
		while (outLen > 0)
		{
			KeccakDigest.KeccakPermutation(state);
			for (i = 0; i < outLen && i < num; i++)
			{
				output[num2 + i] = (byte)(state[i >> 3] >> 8 * (i & 7));
			}
			num2 += i;
			outLen -= i;
			state[25] = (ulong)(num - i);
		}
	}

	public void Squeeze(byte[] output, int outLen)
	{
		KeccakIncSqueeze(output, outLen);
	}

	public void RandomGeneratorInit(byte[] entropyInput, byte[] personalizationString, int entropyLen, int perLen)
	{
		byte[] array = new byte[1] { 1 };
		KeccakIncAbsorb(entropyInput, entropyLen);
		KeccakIncAbsorb(personalizationString, perLen);
		KeccakIncAbsorb(array, array.Length);
		KeccakIncFinalize(31);
	}

	public void SeedExpanderInit(byte[] seed, int seedLen)
	{
		byte[] input = new byte[1] { 2 };
		KeccakIncAbsorb(seed, seedLen);
		KeccakIncAbsorb(input, 1);
		KeccakIncFinalize(31);
	}

	public void ExpandSeed(byte[] output, int outLen)
	{
		int num = outLen & 7;
		KeccakIncSqueeze(output, outLen - num);
		if (num != 0)
		{
			byte[] array = new byte[8];
			KeccakIncSqueeze(array, 8);
			Array.Copy(array, 0, output, outLen - num, num);
		}
	}

	public void SHAKE256_512_ds(byte[] output, byte[] input, int inLen, byte[] domain)
	{
		Arrays.Fill(state, 0uL);
		KeccakIncAbsorb(input, inLen);
		KeccakIncAbsorb(domain, domain.Length);
		KeccakIncFinalize(31);
		KeccakIncSqueeze(output, 64);
	}
}
